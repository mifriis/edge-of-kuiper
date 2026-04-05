/**
 * deploy-commands.js
 *
 * Run once (or when commands change):
 *   node src/deploy-commands.js
 *
 * Set GUILD_ID in .env for instant guild-scoped deploy during dev.
 * Omit GUILD_ID to deploy globally (takes up to 1 hour to propagate).
 */

import 'dotenv/config';
import { REST, Routes } from 'discord.js';

import * as route  from './commands/route.js';
import * as mine   from './commands/mine.js';
import * as sell   from './commands/sell.js';
import * as scan   from './commands/scan.js';
import * as status from './commands/status.js';

const commands = [route, mine, sell, scan, status].map(c => c.data.toJSON());

const rest = new REST().setToken(process.env.DISCORD_TOKEN);

const { CLIENT_ID, GUILD_ID } = process.env;

const route_path = GUILD_ID
  ? Routes.applicationGuildCommands(CLIENT_ID, GUILD_ID)
  : Routes.applicationCommands(CLIENT_ID);

try {
  console.log(`Deploying ${commands.length} command(s)${GUILD_ID ? ` to guild ${GUILD_ID}` : ' globally'}...`);
  const data = await rest.put(route_path, { body: commands });
  console.log(`✅ Deployed ${data.length} command(s).`);
} catch (err) {
  console.error(err);
}
