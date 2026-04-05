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
import { BODIES } from './orbital.js';

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
    location: destination,
    status: 'docked',
  });

  return new EmbedBuilder()
    .setColor(0x4A9EFF)
    .setTitle(`🚀 Arrival: ${body.name}`)
    .setDescription(`Your ship has arrived at **${body.name}** and is now docked.`)
    .addFields(
      { name: 'Location', value: body.name, inline: true },
      { name: 'Status',   value: 'Docked',  inline: true },
    )
    .setTimestamp()
    .setFooter({ text: 'SOLARWIND' });
}

function resolveMine(event, payload) {
  const ship = getShip(event.player_id);
  if (!ship) return null;

  // Yield: base 80t ± 60t, capped by available cargo space
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
    .setFooter({ text: 'SOLARWIND' });
}

function resolveSell(event, payload) {
  const ship = getShip(event.player_id);
  if (!ship) return null;

  const ore       = ship.cargo_ore;
  const pricePerT = payload.pricePerT ?? 120;
  const revenue   = ore * pricePerT;

  adjustCredits(event.player_id, revenue, `Sold ${ore}t ore at ${payload.location}`);
  updateShip(event.ship_id, { cargo_ore: 0, status: 'docked' });

  const player = getPlayer(event.player_id);

  return new EmbedBuilder()
    .setColor(0x3ECF8E)
    .setTitle(`💰 Sale complete`)
    .setDescription(`Offloaded **${ore}t** of ore at **${BODIES[payload.location]?.name ?? payload.location}**.`)
    .addFields(
      { name: 'Revenue',   value: `₡${revenue.toLocaleString()}`,         inline: true },
      { name: 'Balance',   value: `₡${player.credits.toLocaleString()}`,  inline: true },
      { name: 'Price/t',   value: `₡${pricePerT}`,                        inline: true },
    )
    .setTimestamp()
    .setFooter({ text: 'SOLARWIND' });
}

function resolveScan(event, payload) {
  // Simple scan: random intel on a nearby asteroid
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
    .setFooter({ text: 'SOLARWIND' });
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
