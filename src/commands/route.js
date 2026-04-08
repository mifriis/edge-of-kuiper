/**
 * /route — Plot and launch a course to a solar system body.
 *
 * Usage: /route destination:mars
 *
 * Shows travel time preview, then on confirm schedules a TRANSIT event.
 */

import { SlashCommandBuilder, EmbedBuilder, ActionRowBuilder, ButtonBuilder, ButtonStyle } from 'discord.js';
import { ensurePlayer, getShip, enqueueEvent, hasActiveEvent } from '../lib/db.js';
import { BODIES, travelTimeSeconds, formatGameTime, formatRealTime, TIME_COMPRESSION, displayStatus } from '../lib/orbital.js';

export const data = new SlashCommandBuilder()
  .setName('route')
  .setDescription('Plot a course to another body in the solar system')
  .addStringOption(opt =>
    opt.setName('destination')
       .setDescription('Where to go')
       .setRequired(true)
       .addChoices(
         ...Object.entries(BODIES)
           .filter(([k]) => k !== 'sun')
           .map(([k, v]) => ({ name: v.name, value: k }))
       )
  );

export async function execute(interaction) {
  const player = ensurePlayer(interaction.user.id, interaction.user.username);
  const ship   = getShip(interaction.user.id);

  if (!ship) {
    return interaction.reply({ content: '❌ No ship found. Something went wrong.', ephemeral: true });
  }

  const destination = interaction.options.getString('destination');

  if (ship.location === destination) {
    return interaction.reply({
      content: `⚠️ You're already at **${BODIES[destination].name}**.`,
      ephemeral: true,
    });
  }

  if (ship.status !== 'docked') {
    return interaction.reply({
      content: `⚠️ Your ship is currently **${displayStatus(ship)}**. Wait for current operation to complete.`,
      ephemeral: true,
    });
  }

  if (hasActiveEvent(ship.id, 'TRANSIT')) {
    return interaction.reply({ content: '⚠️ Already a transit in progress.', ephemeral: true });
  }

  const realSeconds  = travelTimeSeconds(ship.location, destination, ship.engine_speed);
  const gameTimeStr  = formatGameTime(realSeconds);
  const realTimeStr  = formatRealTime(realSeconds);
  const fromBody     = BODIES[ship.location];
  const toBody       = BODIES[destination];
  const resolveAt    = Math.floor(Date.now() / 1000) + Math.ceil(realSeconds);

  const embed = new EmbedBuilder()
    .setColor(0x4A9EFF)
    .setTitle(`🗺️ Route: ${fromBody.name} → ${toBody.name}`)
    .setDescription('Review your flight plan before committing.')
    .addFields(
      { name: 'From',          value: fromBody.name,  inline: true },
      { name: 'To',            value: toBody.name,    inline: true },
      { name: '\u200B',        value: '\u200B',       inline: true },
      { name: 'Flight time',   value: gameTimeStr,    inline: true },
      { name: 'Real-world ETA',value: realTimeStr,    inline: true },
      { name: 'Drive',         value: 'Epstein (0.3g)',inline: true },
    )
    .setFooter({ text: `Time compression ${TIME_COMPRESSION}× · Confirm to launch` });

  const row = new ActionRowBuilder().addComponents(
    new ButtonBuilder()
      .setCustomId(`launch:${destination}:${resolveAt}:${ship.id}`)
      .setLabel('🚀 Launch')
      .setStyle(ButtonStyle.Primary),
    new ButtonBuilder()
      .setCustomId('cancel_route')
      .setLabel('Cancel')
      .setStyle(ButtonStyle.Secondary),
  );

  await interaction.reply({ embeds: [embed], components: [row], ephemeral: true });
}

// Button interaction handler — called from index.js
export async function handleButton(interaction) {
  if (interaction.customId === 'cancel_route') {
    return interaction.update({ content: '❌ Route cancelled.', embeds: [], components: [] });
  }

  if (interaction.customId.startsWith('launch:')) {
    const [, destination, resolveAtStr, shipIdStr] = interaction.customId.split(':');
    const resolveAt = parseInt(resolveAtStr, 10);
    const shipId    = parseInt(shipIdStr, 10);

    // Double-check ship still docked
    const ship = getShip(interaction.user.id);
    if (ship.status !== 'docked') {
      return interaction.update({ content: '⚠️ Ship is no longer available.', embeds: [], components: [] });
    }

    enqueueEvent({
      playerId:  interaction.user.id,
      shipId,
      type:      'TRANSIT',
      payload:   { from: ship.location, destination },
      resolveAt,
    });

    // Mark ship in transit immediately
    const { updateShip } = await import('../lib/db.js');
    updateShip(shipId, { status: 'transit' });

    const toBody       = BODIES[destination];
    const realSeconds  = resolveAt - Math.floor(Date.now() / 1000);
    const realTimeStr  = formatRealTime(realSeconds);

    return interaction.update({
      content: '',
      embeds: [
        new EmbedBuilder()
          .setColor(0x4A9EFF)
          .setTitle(`🚀 En route to ${toBody.name}`)
          .setDescription(`Your ship has departed. You'll be notified on arrival.`)
          .addFields(
            { name: 'ETA (real)', value: realTimeStr, inline: true },
            { name: 'Destination', value: toBody.name, inline: true },
          )
          .setTimestamp()
      ],
      components: [],
    });
  }
}
