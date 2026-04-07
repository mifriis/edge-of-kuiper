/**
 * /mine — Begin extraction at current location.
 *
 * Only valid at asteroid/dwarf bodies.
 * Duration: 4 game hours (real: ~34 minutes).
 * Outcome resolved by processor.js → resolveMine()
 */

import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';
import { ensurePlayer, getShip, enqueueEvent, hasActiveEvent, updateShip } from '../lib/db.js';
import { BODIES, formatGameTime, formatRealTime, TIME_COMPRESSION } from '../lib/orbital.js';

// Mining duration in game-seconds → convert to real seconds
const MINE_DURATION_GAME_SEC = 4 * 60 * 60; // 4 game hours
const MINE_DURATION_REAL_SEC = Math.ceil(MINE_DURATION_GAME_SEC / TIME_COMPRESSION);

const MINABLE = new Set(['ceres', 'vesta']); // expand as you add more rocks

export const data = new SlashCommandBuilder()
  .setName('mine')
  .setDescription('Begin ore extraction at your current location');

export async function execute(interaction) {
  ensurePlayer(interaction.user.id, interaction.user.username);
  const ship = getShip(interaction.user.id);

  if (!ship) return interaction.reply({ content: '❌ No ship.', ephemeral: true });

  if (ship.status !== 'docked') {
    return interaction.reply({
      content: `⚠️ Ship is **${ship.status}** — finish current operation first.`,
      ephemeral: true,
    });
  }

  if (!MINABLE.has(ship.location)) {
    const body = BODIES[ship.location];
    return interaction.reply({
      content: `⛔ **${body?.name ?? ship.location}** has no minable resources. Head to an asteroid belt.`,
      ephemeral: true,
    });
  }

  if (ship.cargo_ore >= ship.cargo_max) {
    return interaction.reply({
      content: `⚠️ Cargo hold is **full** (${ship.cargo_ore}/${ship.cargo_max}t). Sell before mining.`,
      ephemeral: true,
    });
  }

  if (hasActiveEvent(ship.id, 'MINE')) {
    return interaction.reply({ content: '⚠️ Already mining.', ephemeral: true });
  }

  const resolveAt = Math.floor(Date.now() / 1000) + MINE_DURATION_REAL_SEC;
  const body = BODIES[ship.location];

  enqueueEvent({
    playerId:  interaction.user.id,
    shipId:    ship.id,
    type:      'MINE',
    payload:   { location: ship.location },
    resolveAt,
  });

  updateShip(ship.id, { status: 'mining' });

  await interaction.reply({
    embeds: [
      new EmbedBuilder()
        .setColor(0xF4A736)
        .setTitle(`⛏️ Mining — ${body.name}`)
        .setDescription('Extraction underway. You\'ll be notified when the hold is loaded.')
        .addFields(
          { name: 'Duration (game)', value: formatGameTime(MINE_DURATION_REAL_SEC),    inline: true },
          { name: 'Real ETA',        value: formatRealTime(MINE_DURATION_REAL_SEC),    inline: true },
          { name: 'Hold',            value: `${ship.cargo_ore}/${ship.cargo_max}t`,    inline: true },
        )
        .setTimestamp()
        .setFooter({ text: 'Edge of Kuiper' }),
    ],
    ephemeral: true,
  });
}
