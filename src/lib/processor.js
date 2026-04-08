/**
 * processor.js — Event queue heartbeat
 *
 * Call startProcessor(client) once on bot ready.
 * Polls every 15 real seconds for due events and resolves them.
 *
 * Each event type has a resolver function that:
 *   1. Applies state changes to the DB
 *   2. Returns a Discord embed to DM/post to the player
 */

import { EmbedBuilder } from 'discord.js';
import { getPendingEvents, markResolved, updateShip, adjustCredits, getPlayer, getShip } from './db.js';
import { BODIES, displayStatus } from './orbital.js';
import { iceScanResult, iceMiningGrade, iceMiningYield, icePrice } from '../game/outcomes.js';
import { getEffectiveStats } from '../game/ships.js';

const POLL_INTERVAL_MS = 15_000; // check every 15 seconds

export function startProcessor(client, notifyChannelId) {
  console.log('[processor] Starting event loop...');
  setInterval(() => tick(client, notifyChannelId), POLL_INTERVAL_MS);
}

async function tick(client, channelId) {
  const due = getPendingEvents();
  if (due.length === 0) return;

  console.log(`[processor] Resolving ${due.length} event(s)`);

  for (const event of due) {
    try {
      const payload = JSON.parse(event.payload);
      const embed = await resolveEvent(event, payload);
      markResolved(event.id);

      if (embed) {
        await notify(client, event.player_id, channelId, embed);
      }
    } catch (err) {
      console.error(`[processor] Failed to resolve event ${event.id}:`, err);
    }
  }
}

// ─── Resolvers ─────────────────────────────────────────────────────────────

async function resolveEvent(event, payload) {
  switch (event.type) {
    case 'TRANSIT': return resolveTransit(event, payload);
    case 'MINE':    return resolveMine(event, payload);
    case 'SELL':    return resolveSell(event, payload);
    case 'SCAN':    return resolveScan(event, payload);
    default:
      console.warn(`[processor] Unknown event type: ${event.type}`);
      return null;
  }
}

function resolveTransit(event, payload) {
  const { destination } = payload;
  const body = BODIES[destination];

  updateShip(event.ship_id, {
    location:    destination,
    status:      'docked',
    scan_result: null,
  });

  const arrivedShip = { status: 'docked', location: destination };
  return new EmbedBuilder()
    .setColor(0x4A9EFF)
    .setTitle(`🚀 Arrival: ${body.name}`)
    .setDescription(`Your ship has arrived at **${body.name}**.`)
    .addFields(
      { name: 'Location', value: body.name,                    inline: true },
      { name: 'Status',   value: displayStatus(arrivedShip),   inline: true },
    )
    .setTimestamp()
    .setFooter({ text: 'Edge of Kuiper' });
}

function resolveMine(event, payload) {
  const ship = getShip(event.player_id);
  if (!ship) return null;

  // ── Ice mining (belt) ───────────────────────────────────────────────────
  if (payload.mode === 'ice') {
    const stats    = getEffectiveStats(ship);
    const scanData = ship.scan_result ? JSON.parse(ship.scan_result) : null;
    const hint     = scanData?.location === 'belt' ? scanData.hint : null;

    const { grade, description } = iceMiningGrade({ hint });
    const actual = iceMiningYield({ cargoAvailable: stats.cargoMax - ship.cargo_ore });
    const full   = ship.cargo_ore + actual >= stats.cargoMax;

    updateShip(event.ship_id, {
      cargo_ore:   ship.cargo_ore + actual,
      cargo_grade: grade,
      scan_result: null,
      status:      'docked',
    });

    const MINE_LORE = {
      black: 'Net\'s in. Mostly slag — silicates and iron oxide with water locked up too deep to be worth anything. You\'ll get something for it, but not much.',
      dirty: 'Net\'s in. Mixed haul — dark rock with ice veins running through it like cracked glass. Refinery can work with this.',
      blue:  'Net\'s in. *keya*, that\'s the real thing — nearly pure water ice, almost glows under the work lights. Ceres will pay good for this.',
      full:  'Hold\'s topped off. Head to Ceres or the Outer Belt Refinery to offload.',
    };

    return new EmbedBuilder()
      .setColor(0xF4A736)
      .setTitle('⛏️ Net retrieved — Asteroid Belt')
      .setDescription(MINE_LORE[full ? 'full' : grade])
      .addFields(
        { name: 'Hauled',     value: `${actual}t`, inline: true },
        { name: 'Ice grade',  value: grade,        inline: true },
        { name: 'Assessment', value: description,  inline: false },
      )
      .setTimestamp()
      .setFooter({ text: 'Edge of Kuiper' });
  }

  // ── Legacy ore mine (fallback — no active body uses this path) ──────────
  const available = ship.cargo_max - ship.cargo_ore;
  const rawYield  = Math.floor(60 + Math.random() * 120);
  const actual    = Math.min(rawYield, available);
  const full      = ship.cargo_ore + actual >= ship.cargo_max;

  updateShip(event.ship_id, {
    cargo_ore: ship.cargo_ore + actual,
    status: 'docked',
  });

  const body = BODIES[payload.location] ?? { name: payload.location };

  return new EmbedBuilder()
    .setColor(0xF4A736)
    .setTitle(`⛏️ Mining complete — ${body.name}`)
    .setDescription(full
      ? `Cargo hold is **full**. Head to a market to sell.`
      : `Extraction finished. Hold at ${ship.cargo_ore + actual}/${ship.cargo_max}t.`
    )
    .addFields(
      { name: 'Ore extracted', value: `${actual}t`,                                inline: true },
      { name: 'Hold',          value: `${ship.cargo_ore + actual}/${ship.cargo_max}t`, inline: true },
    )
    .setTimestamp()
    .setFooter({ text: 'Edge of Kuiper' });
}

function resolveSell(event, payload) {
  const ship = getShip(event.player_id);
  if (!ship) return null;

  const grade     = ship.cargo_grade ?? null;
  const pricePerT = grade
    ? icePrice(payload.location, grade)
    : (payload.pricePerT ?? 120);
  const ore     = ship.cargo_ore;
  const revenue = ore * pricePerT;

  adjustCredits(event.player_id, revenue, `Sold ${ore}t ice (${grade ?? 'ore'}) at ${payload.location}`);
  updateShip(event.ship_id, {
    cargo_ore:   0,
    cargo_grade: null,
    status:      'docked',
  });

  const player = getPlayer(event.player_id);

  return new EmbedBuilder()
    .setColor(0x3ECF8E)
    .setTitle('💰 Sale complete')
    .setDescription('Cargo offloaded and weighed. Credits transferred. Another run in the books.')
    .addFields(
      { name: 'Revenue',  value: `₡${revenue.toLocaleString()}`,        inline: true },
      { name: 'Balance',  value: `₡${player.credits.toLocaleString()}`, inline: true },
      { name: 'Grade',    value: grade ?? 'ore',                        inline: true },
    )
    .setTimestamp()
    .setFooter({ text: 'Edge of Kuiper' });
}

function resolveScan(event, payload) {
  // ── Ice scan (belt) ─────────────────────────────────────────────────────
  if (payload.mode === 'ice') {
    const ship  = getShip(event.player_id);
    if (!ship) return null;
    const stats = getEffectiveStats(ship);

    const prior     = ship.scan_result ? JSON.parse(ship.scan_result) : null;
    const prevScans = prior?.scans ?? 0;

    const result = iceScanResult({
      scanQuality: stats.scanQuality,
      currentHint: payload.currentHint ?? null,
      scans:       prevScans,
    });

    updateShip(event.ship_id, {
      status:      'docked',
      scan_result: JSON.stringify({ hint: result.hint, location: 'belt', scans: result.scans }),
    });

    const SCAN_LORE = {
      poor:      'Bounced lidar off a dozen rocks. Mostly silica — dark and heavy, water locked up in the mineral structure if it\'s there at all. Not great.',
      standard:  'You can see ice veins in the surface cracks. Dirty, mixed with carbonates, but there\'s water in there. Workable.',
      promising: 'The 1.5-micron band is lit up. Spectral signature is almost pure H₂O — that blue tint under your work light isn\'t a trick. That\'s ice.',
    };

    const [pBlack, pDirty, pBlue] = result.gradeOdds;
    const oddsStr   = `Black ${Math.round(pBlack * 100)}%  ·  Dirty ${Math.round(pDirty * 100)}%  ·  Blue ${Math.round(pBlue * 100)}%`;
    const rescanNote = result.hint === 'promising'
      ? 'Peak signal reached. No benefit to scanning again.'
      : `Scan ${result.scans} complete. Run /scan again to improve your odds.`;

    return new EmbedBuilder()
      .setColor(0x9B59B6)
      .setTitle('📡 Belt scan results')
      .setDescription(SCAN_LORE[result.hint])
      .addFields(
        { name: 'Quality hint', value: result.hint,        inline: true },
        { name: 'Scans run',    value: `${result.scans}`,  inline: true },
        { name: 'Mine odds',    value: oddsStr,            inline: false },
        { name: '\u200b',       value: rescanNote,         inline: false },
      )
      .setTimestamp()
      .setFooter({ text: 'Edge of Kuiper' });
  }

  // ── Non-belt scan (existing) ────────────────────────────────────────────
  const finds = [
    { type: 'rich vein',     ore: '300–450t', quality: 'high' },
    { type: 'moderate vein', ore: '100–220t', quality: 'medium' },
    { type: 'played-out',    ore: '10–40t',   quality: 'low' },
  ];
  const roll  = Math.random();
  const find  = roll > 0.65 ? finds[0] : roll > 0.3 ? finds[1] : finds[2];
  const body  = BODIES[payload.target] ?? { name: payload.target };

  updateShip(event.ship_id, { status: 'docked' });

  return new EmbedBuilder()
    .setColor(0x9B59B6)
    .setTitle(`📡 Scan results — ${body.name}`)
    .setDescription(`Spectral analysis complete.`)
    .addFields(
      { name: 'Find',      value: find.type,    inline: true },
      { name: 'Est. yield',value: find.ore,     inline: true },
      { name: 'Quality',   value: find.quality, inline: true },
    )
    .setTimestamp()
    .setFooter({ text: 'Edge of Kuiper' });
}

// ─── Notification ──────────────────────────────────────────────────────────

async function notify(client, playerId, channelId, embed) {
  // Try DM first, fall back to channel
  try {
    const user = await client.users.fetch(playerId);
    await user.send({ embeds: [embed] });
    return;
  } catch {
    // DMs disabled — fall through to channel
  }

  if (!channelId) return;
  try {
    const channel = await client.channels.fetch(channelId);
    await channel.send({ content: `<@${playerId}>`, embeds: [embed] });
  } catch (err) {
    console.error('[processor] Notification failed:', err);
  }
}
