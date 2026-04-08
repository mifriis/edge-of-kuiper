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
 *   player create <id> <name>   Ensure player exists, print state
 *   status <player_id>          Ship + account overview
 *   route <player_id> <dest>    Schedule a transit
 *   mine <player_id>            Start ice netting at belt
 *   sell <player_id>            Sell cargo at current location
 *   scan <player_id>            Run belt ice scan (or scan <player_id> <target> for legacy)
 *   tick                        Run the event processor once (resolve all due events)
 *   fasttick <player_id>        Expire all pending events for a player, then tick
 *   travel <from> <to> [speed]  Print travel time between two bodies
 *   bodies                      List all bodies
 *   reset <player_id>           Delete player + ship + events (clean slate)
 */

import 'dotenv/config';
import { ensurePlayer, getPlayer, getShip, getActiveEvents, enqueueEvent,
         updateShip, adjustCredits, getDb } from './lib/db.js';
import { BODIES, travelTimeSeconds, formatGameTime, formatRealTime, displayStatus } from './lib/orbital.js';
import { renderSolarSystem } from './lib/renderer.js';
import { iceScanResult, iceMiningGrade, iceMiningYield, icePrice } from './game/outcomes.js';
import { getEffectiveStats } from './game/ships.js';
import { writeFileSync } from 'fs';

const ICE_MARKETS = new Set(['ceres', 'outer_station']);
const MINABLE     = new Set(['belt']);

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
    console.log(`  Status:       ${displayStatus(ship)}`);
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
        updateShip(event.ship_id, { location: payload.destination, status: 'docked', scan_result: null });
        console.log(`     Ship moved to ${BODIES[payload.destination]?.name ?? payload.destination}`);
        break;
      }
      case 'MINE': {
        const ship  = db.prepare('SELECT * FROM ships WHERE id = ?').get(event.ship_id);
        const stats = getEffectiveStats(ship);
        const scanData = ship.scan_result ? JSON.parse(ship.scan_result) : null;
        const hint     = scanData?.location === 'belt' ? scanData.hint : null;
        const { grade, description } = iceMiningGrade({ hint });
        const actual = iceMiningYield({ cargoAvailable: stats.cargoMax - ship.cargo_ore });
        updateShip(event.ship_id, { cargo_ore: ship.cargo_ore + actual, cargo_grade: grade, scan_result: null, status: 'docked' });
        console.log(`     Net retrieved: ${actual}t (${grade}) — ${description}`);
        break;
      }
      case 'SELL': {
        const ship  = db.prepare('SELECT * FROM ships WHERE id = ?').get(event.ship_id);
        const grade = ship.cargo_grade ?? null;
        const pricePerT = grade ? icePrice(payload.location, grade) : (payload.pricePerT ?? 120);
        const revenue   = ship.cargo_ore * pricePerT;
        adjustCredits(event.player_id, revenue, `Sold ${ship.cargo_ore}t ice (${grade ?? 'ore'}) at ${payload.location}`);
        updateShip(event.ship_id, { cargo_ore: 0, cargo_grade: null, status: 'docked' });
        console.log(`     Sold ${ship.cargo_ore}t (${grade ?? 'ore'}) for ₡${revenue.toLocaleString()}`);
        break;
      }
      case 'SCAN': {
        if (payload.mode === 'ice') {
          const ship   = db.prepare('SELECT * FROM ships WHERE id = ?').get(event.ship_id);
          const stats  = getEffectiveStats(ship);
          const prior  = ship.scan_result ? JSON.parse(ship.scan_result) : null;
          const result = iceScanResult({ scanQuality: stats.scanQuality, currentHint: payload.currentHint ?? null, scans: prior?.scans ?? 0 });
          updateShip(event.ship_id, { status: 'docked', scan_result: JSON.stringify({ hint: result.hint, location: 'belt', scans: result.scans }) });
          const [pB, pD, pBl] = result.gradeOdds;
          console.log(`     Belt scan #${result.scans}: hint=${result.hint}  odds: black ${Math.round(pB*100)}% dirty ${Math.round(pD*100)}% blue ${Math.round(pBl*100)}%`);
        } else {
          const roll = Math.random();
          const find = roll > 0.65 ? 'rich vein (300–450t)' : roll > 0.3 ? 'moderate vein (100–220t)' : 'played-out (10–40t)';
          updateShip(event.ship_id, { status: 'docked' });
          console.log(`     Scan result for ${payload.target}: ${find}`);
        }
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

  player([sub, id, name]) {
    if (sub === 'create') {
      if (!id || !name) return console.log('Usage: player create <id> <name>');
      ensurePlayer(id, name);
      printPlayer(id);
    } else {
      // legacy: player <id> — just show status
      if (!sub) return console.log('Usage: player create <id> <name>');
      printPlayer(sub);
    }
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
    if (ship.status !== 'docked')        return console.log(`Ship is ${ship.status}.`);
    if (!MINABLE.has(ship.location))     return console.log(`Can't mine at ${BODIES[ship.location]?.name ?? ship.location}. Head to the Asteroid Belt.`);
    if (ship.cargo_ore >= ship.cargo_max) return console.log('Cargo hold full.');

    const MINE_REAL_SECS = Math.ceil((6 * 3600) / 7); // 6 game hours
    const resolveAt = Math.floor(Date.now() / 1000) + MINE_REAL_SECS;

    enqueueEvent({ playerId: id, shipId: ship.id, type: 'MINE',
                   payload: { location: ship.location, mode: 'ice' }, resolveAt });
    updateShip(ship.id, { status: 'mining' });

    console.log(`\n⛏️  Netting operation underway at ${BODIES[ship.location].name}`);
    console.log(`   Real ETA: ${formatRealTime(MINE_REAL_SECS)}\n`);
  },

  sell([id]) {
    if (!id) return console.log('Usage: sell <player_id>');
    const ship = getShip(id);
    if (!ship) return console.log('No ship.');
    if (ship.status !== 'docked')       return console.log(`Ship is ${ship.status}.`);
    if (!ICE_MARKETS.has(ship.location)) return console.log(`No ice market at ${BODIES[ship.location]?.name ?? ship.location}. Head to Ceres or the Outer Belt Refinery.`);
    if (ship.cargo_ore === 0)           return console.log('Nothing to sell.');

    const grade     = ship.cargo_grade ?? 'dirty';
    const pricePerT = icePrice(ship.location, grade);
    const resolveAt = Math.floor(Date.now() / 1000) + 5;

    enqueueEvent({ playerId: id, shipId: ship.id, type: 'SELL',
                   payload: { location: ship.location }, resolveAt });

    console.log(`\n💼 Sale queued`);
    console.log(`   ${ship.cargo_ore}t (${grade}) × ₡${pricePerT} = ₡${(ship.cargo_ore * pricePerT).toLocaleString()}`);
    console.log(`   Run 'tick' in 5 seconds to settle.\n`);
  },

  scan([id, target]) {
    if (!id) return console.log('Usage: scan <player_id>');
    const ship = getShip(id);
    if (!ship) return console.log('No ship.');
    if (ship.status !== 'docked') return console.log(`Ship is ${ship.status}.`);

    if (ship.location === 'belt') {
      const prior = ship.scan_result ? JSON.parse(ship.scan_result) : null;
      if (prior?.hint === 'promising') return console.log('Sensors are already singing. Net it.');
      const currentHint = prior?.hint ?? null;
      const SCAN_REAL_SECS = Math.ceil((2 * 3600) / 7); // 2 game hours
      const resolveAt = Math.floor(Date.now() / 1000) + SCAN_REAL_SECS;
      enqueueEvent({ playerId: id, shipId: ship.id, type: 'SCAN',
                     payload: { target: 'belt', mode: 'ice', currentHint }, resolveAt });
      updateShip(ship.id, { status: 'scanning' });
      console.log(`\n📡 Belt scan underway (scan ${(prior?.scans ?? 0) + 1})`);
      console.log(`   Real ETA: ${formatRealTime(SCAN_REAL_SECS)}\n`);
    } else {
      if (!target)        return console.log('Usage: scan <player_id> <target>');
      if (!BODIES[target]) return console.log(`Unknown body: ${target}`);
      const SCAN_REAL_SECS = Math.ceil((1 * 3600) / 7); // 1 game hour
      const resolveAt = Math.floor(Date.now() / 1000) + SCAN_REAL_SECS;
      enqueueEvent({ playerId: id, shipId: ship.id, type: 'SCAN',
                     payload: { target }, resolveAt });
      updateShip(ship.id, { status: 'scanning' });
      console.log(`\n📡 Scanning ${BODIES[target].name}`);
      console.log(`   Real ETA: ${formatRealTime(SCAN_REAL_SECS)}\n`);
    }
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

  async map() {
    const buf  = await renderSolarSystem(new Date());
    const path = './tmp/kuiper-map.png';
    writeFileSync(path, buf);
    console.log(path);
  },
};

// ─── Entry ─────────────────────────────────────────────────────────────────

const [,, cmd, ...args] = process.argv;

if (!cmd || !commands[cmd]) {
  console.log('Kuiper CLI\n');
  console.log('Commands:');
  console.log('  player create <id> <name>   Create/show player');
  console.log('  status <player_id>          Ship + account overview');
  console.log('  route <player_id> <dest>    Launch a transit');
  console.log('  mine <player_id>            Start ice netting (belt only)');
  console.log('  sell <player_id>            Sell cargo (Ceres/Outer Belt Refinery)');
  console.log('  scan <player_id>            Belt ice scan (auto re-scan if prior result)');
  console.log('  tick                        Resolve all due events');
  console.log('  fasttick <player_id>        Expire + resolve all events for a player');
  console.log('  travel <from> <to> [speed]  Show travel time');
  console.log('  bodies                      List all bodies');
  console.log('  reset <player_id>           Wipe player data');
  console.log('  map                         Render solar system → /tmp/kuiper-map.png');
  process.exit(0);
}

Promise.resolve(commands[cmd](args)).catch(err => {
  console.error(err);
  process.exit(1);
});
