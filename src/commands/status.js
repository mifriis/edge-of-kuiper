/**
 * /status — Show current ship and player state, plus pending events.
 */

import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';
import { ensurePlayer, getShip, getActiveEvents } from '../lib/db.js';
import { BODIES, formatRealTime } from '../lib/orbital.js';

const STATUS_EMOJI = {
  docked:  '🟢',
  transit: '🔵',
  mining:  '🟡',
  scanning:'🟣',
};

export const data = new SlashCommandBuilder()
  .setName('status')
  .setDescription('Check your ship and account status');

export async function execute(interaction) {
  const player = ensurePlayer(interaction.user.id, interaction.user.username);
  const ship   = getShip(interaction.user.id);

  if (!ship) return interaction.reply({ content: '❌ No ship found.', ephemeral: true });

  const body        = BODIES[ship.location];
  const statusEmoji = STATUS_EMOJI[ship.status] ?? '⚪';
  const events      = getActiveEvents(interaction.user.id);
  const now         = Math.floor(Date.now() / 1000);

  const embed = new EmbedBuilder()
    .setColor(0x4A9EFF)
    .setTitle(`📊 Status — ${ship.name}`)
    .addFields(
      { name: 'Location',  value: body?.name ?? ship.location,                inline: true },
      { name: 'Status',    value: `${statusEmoji} ${ship.status}`,            inline: true },
      { name: 'Credits',   value: `₡${player.credits.toLocaleString()}`,      inline: true },
      { name: 'Cargo',     value: `${ship.cargo_ore}/${ship.cargo_max}t ore`, inline: true },
    )
    .setTimestamp()
    .setFooter({ text: 'SOLARWIND' });

  if (events.length > 0) {
    const lines = events.map(e => {
      const remaining = e.resolve_at - now;
      const eta = remaining > 0 ? formatRealTime(remaining) : 'arriving now…';
      const payload = JSON.parse(e.payload);
      let desc = e.type;
      if (e.type === 'TRANSIT')  desc = `→ ${BODIES[payload.destination]?.name ?? payload.destination}`;
      if (e.type === 'MINE')     desc = `mining ${BODIES[payload.location]?.name ?? payload.location}`;
      return `**${e.type}** ${desc} — ETA ${eta}`;
    });
    embed.addFields({ name: 'Active operations', value: lines.join('\n') });
  }

  await interaction.reply({ embeds: [embed], ephemeral: true });
}
