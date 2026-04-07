/**
 * /map — Render and post the current solar system map.
 */

import { SlashCommandBuilder, AttachmentBuilder } from 'discord.js';
import { renderSolarSystem } from '../lib/renderer.js';

export const data = new SlashCommandBuilder()
  .setName('map')
  .setDescription('Render the current solar system map');

export async function execute(interaction) {
  await interaction.deferReply({ ephemeral: true });
  const buffer     = await renderSolarSystem(new Date());
  const attachment = new AttachmentBuilder(buffer, { name: 'solar-system.png' });
  await interaction.editReply({ files: [attachment] });
}
