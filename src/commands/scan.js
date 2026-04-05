/**
 * /scan — Run a spectral scan on a nearby body.
 *
 * Duration: 1 game hour (real: ~8.5 min).
 * Returns random ore quality intel, resolved by processor.
 */

import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';
import { ensurePlayer, getShip, enqueueEvent, hasActiveEvent, updateShip } from '../lib/db.js';
import { BODIES, formatRealTime, formatGameTime, TIME_COMPRESSION } from '../lib/orbital.js';

const SCAN_DURATION_GAME_SEC = 1 * 60 * 60; // 1 game hour
const SCAN_DURATION_REAL_SEC = Math.ceil(SCAN_DURATION_GAME_SEC / TIME_COMPRESSION);

const SCANNABLE = ['ceres', 'vesta', 'mars', 'luna'];

export const data = new SlashCommandBuilder()
  .setName('scan')
  .setDescription('Run a spectral scan on a nearby body for mining intel')
  .addStringOption(opt =>
    opt.setName('target')
       .setDescription('Body to scan')
       .setRequired(true)
       .addChoices(
         ...SCANNABLE.map(k => ({ name: BODIES[k].name, value: k }))
       )
  );

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

  const target    = interaction.options.getString('target');
  const resolveAt = Math.floor(Date.now() / 1000) + SCAN_DURATION_REAL_SEC;
  const targetBody = BODIES[target];

  enqueueEvent({
    playerId:  interaction.user.id,
    shipId:    ship.id,
    type:      'SCAN',
    payload:   { target },
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
        .setFooter({ text: 'SOLARWIND' }),
    ],
    ephemeral: true,
  });
}
