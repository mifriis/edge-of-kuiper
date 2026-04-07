/**
 * index.js — Kuiper Discord bot entry point
 *
 * Loads commands, starts event processor, handles interactions.
 */

import 'dotenv/config';
import { Client, GatewayIntentBits, Collection } from 'discord.js';
import { startProcessor } from './lib/processor.js';
import { getDb } from './lib/db.js';

// ─── Commands ──────────────────────────────────────────────────────────────
import * as route  from './commands/route.js';
import * as mine   from './commands/mine.js';
import * as sell   from './commands/sell.js';
import * as scan   from './commands/scan.js';
import * as status from './commands/status.js';
import * as map    from './commands/map.js';

const COMMANDS = [route, mine, sell, scan, status, map];

// ─── Client ────────────────────────────────────────────────────────────────
const client = new Client({
  intents: [GatewayIntentBits.Guilds],
});

client.commands = new Collection();
for (const cmd of COMMANDS) {
  client.commands.set(cmd.data.name, cmd);
}

// ─── Events ────────────────────────────────────────────────────────────────
client.once('ready', () => {
  console.log(`[kuiper] Logged in as ${client.user.tag}`);
  getDb(); // ensure schema is migrated
  startProcessor(client, process.env.NOTIFY_CHANNEL_ID);
});

client.on('interactionCreate', async interaction => {
  if (interaction.isChatInputCommand()) {
    const cmd = client.commands.get(interaction.commandName);
    if (!cmd) return;
    try {
      await cmd.execute(interaction);
    } catch (err) {
      console.error(`[command] ${interaction.commandName} failed:`, err);
      const reply = { content: '❌ Something went wrong.', ephemeral: true };
      if (interaction.replied || interaction.deferred) {
        await interaction.followUp(reply);
      } else {
        await interaction.reply(reply);
      }
    }
    return;
  }

  if (interaction.isButton()) {
    // Route buttons to the right handler
    const id = interaction.customId;
    if (id.startsWith('launch:') || id === 'cancel_route') {
      await route.handleButton(interaction);
    }
    return;
  }
});

// ─── Boot ──────────────────────────────────────────────────────────────────
client.login(process.env.DISCORD_TOKEN);
