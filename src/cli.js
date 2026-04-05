#!/usr/bin/env node
/**
 * cli.js — Kuiper test harness
 *
 * Run game functions without touching Discord.
 *
 * Usage:
 *   node src/cli.js <command> [args]
 *
 * Commands:
 *   player <id> <name>          Ensure player exists, print state
 *   status <player_id>          Ship + account overview
 *   route <player_id> <dest>    Schedule a transit
 *   mine <player_id>            Start mining at current location
 *   sell <player_id>            Sell cargo at current location
 *   scan <player_id> <target>   Run a spectral scan
 *   tick                        Run the event processor once (resolve all due events)
 *   fasttick <player_id>        Expire all pending events for a player, then tick
 *   travel <from> <to> [speed]  Print travel time between two bodies
 *   bodies                      List all bodies
 *   reset <player_id>           Delete player + ship + events (clean slate)
 */

import 'dotenv/config';
import { ensurePlayer, getPlayer, getShip, getActiveEvents, enqueueEvent,
         updateShip, adjustCredits, getDb } from './lib/db.js';
import { BODIES, travelTimeSeconds, formatGameTime, formatRealTime } from './lib/orbital.js';

const MARKETS = {
  earth: { name: 'Earth Orbital Exchange', pricePerT: 150 },
  luna:  { name: 'Lunar Commodities',      pricePerT: 140 },
  mars:  { name: 'Mars Port Authority',    pricePerT: 130 },
  ceres: { name: 'Ceres Freeport',         pricePerT: 110 },
};

const MINABLE = new Set(['ceres', 'vesta']);

// ─── Helpers ───────────────────────────────────────────────────────────────

function printPlayer(id) {
  const player = getPlayer(id);
  const ship   = getShip(id);
  const events = getActiveEvents(id);
  const now    = Math.floor(Date.now() / 1000);

  if (!player) { console.log('Player not found:', id); return; }

  console.log('\n── Player ─────────────────────────────');
  console.log(`  ID:      ${player.id}`);
  console.log(`  Name:    ${player.name}`);
  console.log(`  Credits: ₡${player.credits.toLocaleString()}`);

  if (ship) {
    console.log('\n── Ship ───────────────────────────────');
    console.log(`  Name:         ${ship.name}`);
    console.log(`  Location:     ${BODIES[ship.location]?.name ?? ship.location}`);
    console.log(`  Status:       ${ship.status}`);
    console.log(`  Cargo:        ${ship.cargo_ore}/${ship.cargo_max}t`);
    console.log(`  Engine speed: ${ship.engine_speed} u/day`);
  }

  if (events.length > 0) {
    console.log('\n── Pending events ─────────────────────');
    for (const e of events) {
      const remaining = Math.max(0, e.resolve_at - now);
      const payload   = JSON.parse(e.payload);
      console.log(`  [${e.id}] ${e.type.padEnd(8)} — ETA ${formatRealTime(remaining)} real  |`, payload);
    }
  }

  console.log('');
}

function resolveEvents() {
  // Inline processor tick — mirrors processor.js but prints to console
  const db  = getDb();
  const now = Math.floor(Date.now() / 1000);
  const due = db.prepare(`
    SELECT * FROM events WHERE resolved = 0 AND resolve_at <= ? ORDER BY resolve_at ASC
  `).all(now);

  if (due.length === 0) {
    console.log('[tick] No events due.');
    return;
  }

  console.log(`[tick] Resolving ${due.length} event(s)...\n`);

  for (const event of due) {
    const payload = JSON.parse(event.payload);
    console.log(`  → [${event.id}] ${event.type}`, payload);

    switch (event.type) {
      case 'TRANSIT': {
        updateShip(event.ship_id, { location: payload.destination, status: 'docked' });
        console.log(`     Ship moved to ${BODIES[payload.destination]?.name ?? payload.destination}`);
        break;
      }
      case 'MINE': {
        const ship    = db.prepare('SELECT * FROM ships WHERE id = ?').get(event.ship_id);
        const avail   = ship.cargo_max - ship.cargo_ore;
        const raw     = Math.floor(60 + Math.random() * 120);
        const actual  = Math.min(raw, avail);
        updateShip(event.ship_id, { cargo_ore: ship.cargo_ore + actual, status: 'docked' });
        console.log(`     Extracted ${actual}t ore. Hold: ${ship.cargo_ore + actual}/${ship.cargo_max}t`);
        break;
      }
      case 'SELL': {
        const ship    = db.prepare('SELECT * FROM ships WHERE id = ?').get(event.ship_id);
        const revenue = ship.cargo_ore * (payload.pricePerT ?? 120);
        adjustCredits(event.player_id, revenue, `Sold ${ship.cargo_ore}t at ${payload.location}`);
        updateShip(event.ship_id, { cargo_ore: 0, status: 'docked' });
        console.log(`     Sold ${ship.cargo_ore}t for ₡${revenue.toLocaleString()}`);
        break;
      }
      case 'SCAN': {
        const roll  = Math.random();
        const find  = roll > 0.65 ? 'rich vein (300–450t)' : roll > 0.3 ? 'moderate vein (100–220t)' : 'played-out (10–40t)';
        updateShip(event.ship_id, { status: 'docked' });
        console.log(`     Scan result for ${payload.target}: ${find}`);
        break;
      }
      default:
        console.log(`     Unknown event type, skipping.`);
    }

    db.prepare('UPDATE events SET resolved = 1 WHERE id = ?').run(event.id);
  }
  console.log('');
}

// ─── Commands ──────────────────────────────────────────────────────────────

const commands = {

  player([id, name]) {
    if (!id || !name) return console.log('Usage: player <id> <name>');
    ensurePlayer(id, name);
    printPlayer(id);
  },

  status([id]) {
    if (!id) return console.log('Usage: status <player_id>');
    printPlayer(id);
  },

  route([id, dest]) {
    if (!id || !dest) return console.log('Usage: route <player_id> <destination>');
    if (!BODIES[dest]) return console.log(`Unknown body: ${dest}\nValid:`, Object.keys(BODIES).join(', '));

    const ship = getShip(id);
    if (!ship) return console.log('No ship found for player:', id);
    if (ship.status !== 'docked') return console.log(`Ship is ${ship.status}, not docked.`);
    if (ship.location === dest) return console.log(`Already at ${BODIES[dest].name}.`);

    const realSecs  = travelTimeSeconds(ship.location, dest, ship.engine_speed);
    const resolveAt = Math.floor(Date.now() / 1000) + realSecs;

    enqueueEvent({ playerId: id, shipId: ship.id, type: 'TRANSIT',
                   payload: { from: ship.location, destination: dest }, resolveAt });
    updateShip(ship.id, { status: 'transit' });

    console.log(`\n🚀 Launched: ${BODIES[ship.location].name} → ${BODIES[dest].name}`);
    console.log(`   Game time : ${formatGameTime(realSecs)}`);
    console.log(`   Real ETA  : ${formatRealTime(realSecs)}\n`);
  },

  mine([id]) {
    if (!id) return console.log('Usage: mine <player_id>');
    const ship = getShip(id);
    if (!ship) return console.log('No ship.');
    if (ship.status !== 'docked')  return console.log(`Ship is ${ship.status}.`);
    if (!MINABLE.has(ship.location)) return console.log(`Can't mine at ${BODIES[ship.location]?.name ?? ship.location}.`);
    if (ship.cargo_ore >= ship.cargo_max) return console.log('Cargo hold full.');

    const MINE_REAL_SECS = Math.ceil((4 * 3600) / 7); // 4 game hours
    const resolveAt = Math.floor(Date.now() / 1000) + MINE_REAL_SECS;

    enqueueEvent({ playerId: id, shipId: ship.id, type: 'MINE',
                   payload: { location: ship.location }, resolveAt });
    updateShip(ship.id, { status: 'mining' });

    console.log(`\n⛏️  Mining at ${BODIES[ship.location].name}`);
    console.log(`   Real ETA: ${formatRealTime(MINE_REAL_SECS)}\n`);
  },

  sell([id]) {
    if (!id) return console.log('Usage: sell <player_id>');
    const ship = getShip(id);
    if (!ship) return console.log('No ship.');
    if (ship.status !== 'docked') return console.log(`Ship is ${ship.status}.`);
    if (!MARKETS[ship.location]) return console.log(`No market at ${BODIES[ship.location]?.name ?? ship.location}.`);
    if (ship.cargo_ore === 0) return console.log('Nothing to sell.');

    const market    = MARKETS[ship.location];
    const resolveAt = Math.floor(Date.now() / 1000) + 5; // near-instant

    enqueueEvent({ playerId: id, shipId: ship.id, type: 'SELL',
                   payload: { location: ship.location, pricePerT: market.pricePerT }, resolveAt });

    console.log(`\n💼 Sale queued at ${market.name}`);
    console.log(`   ${ship.cargo_ore}t × ₡${market.pricePerT} = ₡${(ship.cargo_ore * market.pricePerT).toLocaleString()}`);
    console.log(`   Run 'tick' in 5 seconds to settle.\n`);
  },

  scan([id, target]) {
    if (!id || !target) return console.log('Usage: scan <player_id> <target>');
    if (!BODIES[target]) return console.log(`Unknown body: ${target}`);
    const ship = getShip(id);
    if (!ship) return console.log('No ship.');
    if (ship.status !== 'docked') return console.log(`Ship is ${ship.status}.`);

    const SCAN_REAL_SECS = Math.ceil((1 * 3600) / 7); // 1 game hour
    const resolveAt = Math.floor(Date.now() / 1000) + SCAN_REAL_SECS;

    enqueueEvent({ playerId: id, shipId: ship.id, type: 'SCAN',
                   payload: { target }, resolveAt });
    updateShip(ship.id, { status: 'scanning' });

    console.log(`\n📡 Scanning ${BODIES[target].name}`);
    console.log(`   Real ETA: ${formatRealTime(SCAN_REAL_SECS)}\n`);
  },

  tick() {
    resolveEvents();
  },

  fasttick([id]) {
    if (!id) return console.log('Usage: fasttick <player_id>');
    const db  = getDb();
    const now = Math.floor(Date.now() / 1000);
    const res = db.prepare(`
      UPDATE events SET resolve_at = ? - 1
      WHERE player_id = ? AND resolved = 0
    `).run(now, id);
    console.log(`[fasttick] Expired ${res.changes} event(s) for ${id}`);
    resolveEvents();
    printPlayer(id);
  },

  travel([from, to, speed = '1.0']) {
    if (!from || !to) return console.log('Usage: travel <from> <to> [engine_speed]');
    if (!BODIES[from]) return console.log(`Unknown body: ${from}`);
    if (!BODIES[to])   return console.log(`Unknown body: ${to}`);
    const s = parseFloat(speed);
    const secs = travelTimeSeconds(from, to, s);
    console.log(`\n${BODIES[from].name} → ${BODIES[to].name} @ ${s} u/day`);
    console.log(`  Game time : ${formatGameTime(secs)}`);
    console.log(`  Real time : ${formatRealTime(secs)}\n`);
  },

  bodies() {
    console.log('\n── Bodies ──────────────────────────────');
    for (const [key, b] of Object.entries(BODIES)) {
      console.log(`  ${key.padEnd(10)} ${b.name.padEnd(12)} dist: ${b.dist}  type: ${b.type}`);
    }
    console.log('');
  },

  reset([id]) {
    if (!id) return console.log('Usage: reset <player_id>');
    const db = getDb();
    db.prepare('DELETE FROM events WHERE player_id = ?').run(id);
    db.prepare('DELETE FROM ships  WHERE player_id = ?').run(id);
    db.prepare('DELETE FROM ledger WHERE player_id = ?').run(id);
    db.prepare('DELETE FROM players WHERE id = ?').run(id);
    console.log(`Reset complete for player ${id}.`);
  },
};

// ─── Entry ─────────────────────────────────────────────────────────────────

const [,, cmd, ...args] = process.argv;

if (!cmd || !commands[cmd]) {
  console.log('Kuiper CLI\n');
  console.log('Commands:');
  console.log('  player <id> <name>          Create/show player');
  console.log('  status <player_id>          Ship + account overview');
  console.log('  route <player_id> <dest>    Launch a transit');
  console.log('  mine <player_id>            Start mining');
  console.log('  sell <player_id>            Sell cargo');
  console.log('  scan <player_id> <target>   Run a scan');
  console.log('  tick                        Resolve all due events');
  console.log('  fasttick <player_id>        Expire + resolve all events for a player');
  console.log('  travel <from> <to> [speed]  Show travel time');
  console.log('  bodies                      List all bodies');
  console.log('  reset <player_id>           Wipe player data');
  process.exit(0);
}

commands[cmd](args);
