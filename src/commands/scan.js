/**
 * /scan — Run a spectral scan at your current location.
 *
 * No target option — sensors only work at short range.
 * Belt: ice spectral scan (2 game hours). Elsewhere: ore survey (1 game hour).
 */

import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';
import { ensurePlayer, getShip, enqueueEvent, hasActiveEvent, updateShip } from '../lib/db.js';
import { BODIES, formatRealTime, formatGameTime, TIME_COMPRESSION } from '../lib/orbital.js';

const SCAN_DURATION_GAME_SEC     = 1 * 60 * 60; // 1 game hour (non-belt)
const SCAN_DURATION_REAL_SEC     = Math.ceil(SCAN_DURATION_GAME_SEC / TIME_COMPRESSION);
const SCAN_ICE_DURATION_GAME_SEC = 2 * 60 * 60; // 2 game hours (belt)
const SCAN_ICE_DURATION_REAL_SEC = Math.ceil(SCAN_ICE_DURATION_GAME_SEC / TIME_COMPRESSION); // 1029

export const data = new SlashCommandBuilder()
  .setName('scan')
  .setDescription('Run a spectral scan at your current location');

export async function execute(interaction) {
  ensurePlayer(interaction.user.id, interaction.user.username);
  const ship = getShip(interaction.user.id);

  if (!ship) return interaction.reply({ content: '❌ No ship.', ephemeral: true });

  if (ship.status !== 'docked') {
    return interaction.reply({
      content: `⚠️ Ship is **${ship.status}**. Cannot scan during operations.`,
      ephemeral: true,
    });
  }

  if (hasActiveEvent(ship.id, 'SCAN')) {
    return interaction.reply({ content: '⚠️ Scan already in progress.', ephemeral: true });
  }

  // ── Belt ice scan ───────────────────────────────────────────────────────
  if (ship.location === 'belt') {
    const prior = ship.scan_result ? JSON.parse(ship.scan_result) : null;

    if (prior?.hint === 'promising') {
      return interaction.reply({
        content: 'Sensors are already singing. Any more scanning is just burning time.\nNet it.',
        ephemeral: true,
      });
    }

    const currentHint = prior?.hint ?? null;
    const isRescan    = currentHint !== null;
    const resolveAt   = Math.floor(Date.now() / 1000) + SCAN_ICE_DURATION_REAL_SEC;

    enqueueEvent({
      playerId:  interaction.user.id,
      shipId:    ship.id,
      type:      'SCAN',
      payload:   { target: 'belt', mode: 'ice', currentHint },
      resolveAt,
    });

    updateShip(ship.id, { status: 'scanning' });

    const desc = isRescan
      ? 'Not satisfied. You fire another lidar pass, closer this time — watching for cracks in the surface, anything that catches the light differently.'
      : 'You pull yourself out the airlock and hang in the black, suit pinging the rocks ahead.\nLidar\'s sweeping. Should know something in a couple hours.';

    return interaction.reply({
      embeds: [
        new EmbedBuilder()
          .setColor(0x9B59B6)
          .setTitle('📡 Belt scan underway')
          .setDescription(desc)
          .addFields(
            { name: 'Duration (game)', value: formatGameTime(SCAN_ICE_DURATION_REAL_SEC), inline: true },
            { name: 'Real ETA',        value: formatRealTime(SCAN_ICE_DURATION_REAL_SEC), inline: true },
          )
          .setTimestamp()
          .setFooter({ text: 'Edge of Kuiper' }),
      ],
      ephemeral: true,
    });
  }

  // ── Non-belt scan ───────────────────────────────────────────────────────
  const target     = ship.location;
  const targetBody = BODIES[target] ?? { name: target };
  const resolveAt  = Math.floor(Date.now() / 1000) + SCAN_DURATION_REAL_SEC;

  enqueueEvent({
    playerId:  interaction.user.id,
    shipId:    ship.id,
    type:      'SCAN',
    payload:   { target: ship.location },
    resolveAt,
  });

  updateShip(ship.id, { status: 'scanning' });

  await interaction.reply({
    embeds: [
      new EmbedBuilder()
        .setColor(0x9B59B6)
        .setTitle(`📡 Scan initiated — ${targetBody.name}`)
        .setDescription('Passive spectral analysis underway. Results will be transmitted when complete.')
        .addFields(
          { name: 'Target',          value: targetBody.name,                        inline: true },
          { name: 'Duration (game)', value: formatGameTime(SCAN_DURATION_REAL_SEC), inline: true },
          { name: 'Real ETA',        value: formatRealTime(SCAN_DURATION_REAL_SEC), inline: true },
        )
        .setTimestamp()
        .setFooter({ text: 'Edge of Kuiper' }),
    ],
    ephemeral: true,
  });
}
