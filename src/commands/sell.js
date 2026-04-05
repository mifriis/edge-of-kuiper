/**
 * /sell — Sell ore at the current location's market.
 *
 * Valid at: earth, luna, mars, ceres (add more as needed).
 * Price varies by location — Earth pays premium, belt stations are cheaper.
 * Sale is near-instant (scheduled 10s out so it passes through the event system).
 */

import { SlashCommandBuilder, EmbedBuilder } from 'discord.js';
import { ensurePlayer, getShip, enqueueEvent, updateShip } from '../lib/db.js';
import { BODIES } from '../lib/orbital.js';

const MARKETS = {
  earth: { name: 'Earth Orbital Exchange', pricePerT: 150 },
  luna:  { name: 'Lunar Commodities',      pricePerT: 140 },
  mars:  { name: 'Mars Port Authority',    pricePerT: 130 },
  ceres: { name: 'Ceres Freeport',         pricePerT: 110 },
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

  const market = MARKETS[ship.location];
  if (!market) {
    return interaction.reply({
      content: `⛔ No market at **${BODIES[ship.location]?.name ?? ship.location}**.`,
      ephemeral: true,
    });
  }

  if (ship.cargo_ore === 0) {
    return interaction.reply({
      content: `⚠️ Nothing to sell — cargo hold is empty.`,
      ephemeral: true,
    });
  }

  // Schedule as a near-instant event (10s) so it flows through processor
  const resolveAt = Math.floor(Date.now() / 1000) + 10;
  const revenue   = ship.cargo_ore * market.pricePerT;

  enqueueEvent({
    playerId:  interaction.user.id,
    shipId:    ship.id,
    type:      'SELL',
    payload:   { location: ship.location, pricePerT: market.pricePerT },
    resolveAt,
  });

  updateShip(ship.id, { status: 'docked' }); // stays docked during transaction

  await interaction.reply({
    embeds: [
      new EmbedBuilder()
        .setColor(0x3ECF8E)
        .setTitle(`💼 Sale pending — ${market.name}`)
        .setDescription(`Offloading **${ship.cargo_ore}t** of ore. Credits incoming shortly.`)
        .addFields(
          { name: 'Cargo',      value: `${ship.cargo_ore}t`, inline: true },
          { name: 'Price/t',    value: `₡${market.pricePerT}`, inline: true },
          { name: 'Est. total', value: `₡${revenue.toLocaleString()}`, inline: true },
        )
        .setTimestamp()
        .setFooter({ text: 'SOLARWIND' }),
    ],
    ephemeral: true,
  });
}
