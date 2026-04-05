# Edge of Kuiper

A realtime Discord space game. Ships travel the solar system, mine asteroids, and trade — all on a **7× time compression** so 1 real minute = 7 game minutes.

---

## Setup

`docker run -it -v "${PWD}:/app" -w /app node:24-slim sh`

```bash
npm install
cp .env.example .env
# Fill in DISCORD_TOKEN, CLIENT_ID, GUILD_ID
node src/deploy-commands.js   # register slash commands once
npm start
```



---

## Project structure

```
src/
  index.js              — Bot entry point, interaction routing
  deploy-commands.js    — One-shot command registration
  lib/
    db.js               — SQLite schema + all query helpers
    orbital.js          — Travel time math, body definitions
    processor.js        — Event queue heartbeat (runs every 15s)
  commands/
    route.js            — /route  — plan and launch a transit
    mine.js             — /mine   — extract ore at current body
    sell.js             — /sell   — sell cargo at local market
    scan.js             — /scan   — spectral scan for intel
    status.js           — /status — ship + account overview
data/
  kuiper.db          — SQLite database (auto-created)
```

---

## How the event system works

Every player action creates a row in the `events` table:

```
type       | what happens when it resolves
-----------+-----------------------------------------------
TRANSIT    | ship.location updated, player notified on arrival
MINE       | random ore added to cargo hold
SELL       | cargo cleared, credits added to player account
SCAN       | random intel on target body returned to player
```

`resolve_at` is a Unix timestamp in **real** seconds.  
Travel durations are calculated in real seconds, so the 7× compression is applied at the display layer only — `formatGameTime()` multiplies by 7 for the "game time" shown to players.

The processor polls every 15 real seconds. You can tighten this as needed — just watch SQLite write contention if you go below ~5s.

---

## Time maths

```
Earth → Luna        ~3 game days    = ~10 real hours
Earth → Mars        ~60 game days   = ~8.5 real days   (opposition distance)
Earth → Ceres       ~130 game days  = ~19 real days
Earth → Jupiter     ~320 game days  = ~46 real days
```

Drive model: constant 0.3g Epstein-style acceleration, flip-and-burn at midpoint.  
`travelTimeSeconds()` in `orbital.js` is the single source of truth.

---

## Extending

**Add a new body** — drop a row into `BODIES` in `orbital.js`. That's it; `/route` picks it up automatically.

**Add a new command** — create `src/commands/yourcommand.js` exporting `data` (SlashCommandBuilder) and `execute(interaction)`. Import and register in `index.js` and `deploy-commands.js`, then re-run `deploy-commands.js`.

**Add a new event type** — add a resolver function in `processor.js` and a `case` in `resolveEvent()`. Enqueue it from any command with `enqueueEvent({ type: 'YOUR_TYPE', ... })`.

**Multiplayer economy ideas:**
- Market prices that fluctuate based on supply (track aggregate sales per body)
- Intel trading: sell scan results to other players
- Piracy events: random chance of being interdicted in transit
- Ship upgrades: bigger cargo hold, better drive (shorter travel time multiplier)
- Factions: alignment affects prices and event outcomes