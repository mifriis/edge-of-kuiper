/**
 * /sell — Sell ice cargo at a refinery.
 *
 * Valid at: ceres, outer_station only.
 * Price = base × grade multiplier, computed by processor from ship.cargo_grade.
 * Sale is near-instant (scheduled 10s out so it passes through the event system).
 */

import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';
import { ensurePlayer, getShip, enqueueEvent, updateShip } from '../lib/db.js';
import { BODIES } from '../lib/orbital.js';
import { icePrice } from '../game/outcomes.js';

const ICE_MARKETS = {
  ceres:         { name: 'Ceres Freeport'       },
  outer_station: { name: 'Outer Belt Refinery'  },
};

export const data = new SlashCommandBuilder()
  .setName('sell')
  .setDescription('Sell your ore cargo at the current market');

export async function execute(interaction) {
  ensurePlayer(interaction.user.id, interaction.user.username);
  const ship = getShip(interaction.user.id);

  if (!ship) return interaction.reply({ content: '❌ No ship.', ephemeral: true });

  if (ship.status !== 'docked') {
    return interaction.reply({
      content: `⚠️ Ship is **${ship.status}**. Dock first.`,
      ephemeral: true,
    });
  }

  const market = ICE_MARKETS[ship.location];
  if (!market) {
    return interaction.reply({
      content: `⛔ **${BODIES[ship.location]?.name ?? ship.location}** cannot refine ice.\nHead to **Ceres** or the **Outer Belt Refinery**.`,
      ephemeral: true,
    });
  }

  if (ship.cargo_ore === 0) {
    return interaction.reply({
      content: `⚠️ Nothing to sell — cargo hold is empty.`,
      ephemeral: true,
    });
  }

  const grade      = ship.cargo_grade ?? 'dirty';
  const pricePerT  = icePrice(ship.location, grade);
  const estRevenue = ship.cargo_ore * pricePerT;

  // Schedule near-instant (10s) so it flows through processor
  const resolveAt = Math.floor(Date.now() / 1000) + 10;

  enqueueEvent({
    playerId:  interaction.user.id,
    shipId:    ship.id,
    type:      'SELL',
    payload:   { location: ship.location },
    resolveAt,
  });

  updateShip(ship.id, { status: 'docked' });

  await interaction.reply({
    embeds: [
      new EmbedBuilder()
        .setColor(0x3ECF8E)
        .setTitle(`💼 Sale pending — ${market.name}`)
        .setDescription(`Offloading **${ship.cargo_ore}t** of ice. Credits incoming shortly.`)
        .addFields(
          { name: 'Cargo',      value: `${ship.cargo_ore}t`,            inline: true },
          { name: 'Grade',      value: grade,                           inline: true },
          { name: 'Est. total', value: `₡${estRevenue.toLocaleString()}`, inline: true },
        )
        .setTimestamp()
        .setFooter({ text: 'Edge of Kuiper' }),
    ],
    ephemeral: true,
  });
}
