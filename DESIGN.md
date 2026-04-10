# Edge of Kuiper — Design rules

Asynchronous Discord space game. Players issue commands → events are
scheduled → processor resolves them in real time at 7× compression.

---

## Hard rules

These are invariants. Do not violate them, do not work around them.

**Time**
- All `resolve_at` values in DB are real-world Unix seconds. Always.
- Game-time (7×) is display-only. `formatGameTime()` at the boundary, nowhere else.
- Never store game-time. Never do arithmetic in game-time.

**Orbital model**
- Circular 2D orbits. No eccentricity, no inclination, no Lagrange points, no three-body problem.
- `longitude(t) = longitudeJ2000 + (360 / (distance^1.5 × 365.25)) × daysSinceJ2000(t)`
- `getBodyPosition(body, date)` → `{x, y}` AU. `distanceBetweenBodies(bodyA, bodyB, date)` → km.
- Functions accept plain body objects — DB rows and procedural bodies work without changes to orbital math.
- Travel time is still `abs(a.dist - b.dist) / engineSpeed`. Orbital positions don't affect routing.
- Physical constants (`AU_IN_KM`, `J2000`, `GAME_START_DATE`, future gravity/rocket constants) live in `lib/physics.js`.

**Ship stats**
- Never read `ship.engine_speed` or `ship.cargo_max` directly in commands or game logic.
- Always use `getEffectiveStats(ship)` from `game/ships.js`.
- Stats are derived from equipped modules. The DB stores module IDs, not computed stats.

**Event system**
- Check `hasActiveEvent(shipId, type)` before enqueueing. No duplicate active events per type per ship.
- Set ship `status` immediately on enqueue (e.g. `transit`, `mining`). Don't wait for resolution.
- Resolvers reset status to `docked`. Commands do not reset status.
- All RNG and probability logic lives in `game/outcomes.js`. Resolvers call it, they don't roll dice.

**Layer boundaries**
- `commands/` — Discord interaction handling only. No SQL. No game logic.
- `game/` — Kuiper-specific logic: stats, modules, outcomes, prices.
- `lib/` — Infrastructure: DB queries, orbital math, event processor. No game design decisions.
- Numbers a game designer might tune (prices, yields, probabilities) belong in `game/`, never `lib/`.

---

## Body representations

There are **two independent body maps** in `src/lib/orbital.js`. They serve different purposes and must be kept in sync whenever a new navigable body is added.

| | `BODIES` | `SOLAR_BODIES` |
|---|---|---|
| Source | Hardcoded in `orbital.js` | Built at startup from `data/solsystem.json` via `flattenBodies()` |
| Key | Lowercase game ID (`earth`, `belt`, `outer_station`) | `node.name.toLowerCase()` from JSON (`sol`, `asteroid belt`, …) |
| `dist` values | Rounded for routing (`mars: 1.5`, not 1.524) | Full semi-major axis AU used for orbital math |
| Used for | `travelTimeSeconds()`, route validation, all command logic | `getBodyPosition()`, `distanceBetweenBodies()`, renderer |
| Includes | Navigable bodies only (ships can be *here*) | Every body in the JSON tree, including satellite-only bodies |

**Adding a new navigable body requires updating both:**
1. `data/solsystem.json` — for the renderer and orbital math (needs `distance`, `longitudeJ2000`)
2. `BODIES` in `orbital.js` — for routing, travel time, and all game logic (needs `dist`, `type`)

A body in `SOLAR_BODIES` but not `BODIES` will appear on the map but ships cannot travel to it.
A body in `BODIES` but not `SOLAR_BODIES` can be navigated to but will not appear on the map or in orbital calculations.

**`type` values in `BODIES`** drive display and game rules:
- `zone` — ships display as *drifting* (not docked); `/scan` runs an ice spectral scan
- `station` — ships display as *docked*; ice market
- `planet`, `moon`, `dwarf`, `star` — display as *docked*; `/scan` runs a legacy ore survey

`/scan` always targets `ship.location` — no target option. Sensors don't work at astronomical range.

---

**New body (map/orbital only):** add to `data/solsystem.json` with `distance` (AU semi-major axis) and `longitudeJ2000`.

**New navigable body (ships can travel there):** update *both* `data/solsystem.json` *and* `BODIES` in `orbital.js`. See *Body representations* above for the rules.

**New module:** add to `MODULES` in `game/modules.js` with a `stats` block. Nothing else changes.

**New stat:** add to `BASE_STATS` in `game/ships.js`, add to relevant module `stats` blocks,
expose in `getEffectiveStats()` return object.

**New event type:**
1. Resolver function in `lib/processor.js` → `resolveEvent()` switch
2. Outcome logic in `game/outcomes.js` if randomness involved
3. Enqueue from command via `enqueueEvent()`
4. Add CLI command in `src/cli.js` for `fasttick` testing

---

## Breaking change checklist

Run this mentally before merging any feature. A change is breaking if it affects players
who are mid-session — ship in flight, event queued, or cargo in hold.

**Schema**
- New columns must have `DEFAULT NULL` or a safe default. Wrap in try/catch `ALTER TABLE` — never drop or rename columns.
- New JSON payload fields must be optional (`??` fallback in resolvers) so in-flight events with the old payload still resolve.

**Markets / locations**
- Removing a sell location strands players with cargo there. Either keep a fallback in `resolveSell`, or note the forced migration in the spec.
- Removing a minable location is safe only if no ship is currently `status = 'mining'` there.

**Status values**
- Resolver resets are `status: 'docked'` — keep it that way. Display label (`displayStatus()`) handles zone / station flavour text without touching the stored value.

**Events**
- Old event types in the queue must still resolve after a deploy. Keep legacy branches until the queue is confirmed drained, or add a migration that cancels stale events.

**Specs must call it out**
- Any spec that hits one of the above categories must have an explicit "Breaking change:" note in its *What this is, and what it is not* section and in the relevant file-changes section.

---



`ships.modules` — JSON column, maps slot type → module ID:
```json
{ "drive": "drive_ion", "cargo": "cargo_basic", "scanner": "scanner_basic" }
```

Player IDs are arbitrary strings (`TEXT`). Discord sends snowflakes, CLI uses short IDs like `p1`.
Never assume format.

Multiple ships per player is supported by the schema. Commands currently use `LIMIT 1`.

---

## Discord command sanity check

After implementing any command (new or modified), manually verify the following in Discord before marking the feature done. Tests run against the CLI; Discord has extra constraints that tests cannot catch.

1. **Deploy commands** — run `node src/deploy-commands.js`. Slash command changes (new options, changed `required`, added/removed choices) are invisible to Discord until redeployed.
2. **Option visibility** — open the command in Discord and confirm every option and choice appears as expected. Check `required` flags: a required option forces the player to pick *before* submitting, which can block context-driven logic.
3. **Happy path** — navigate to the correct location, run the command, confirm the embed looks right and the ship status updates.
4. **Wrong-location path** — run the command from a location it shouldn't work at. Confirm a clear error message (not a generic ❌ Something went wrong).
5. **Busy ship path** — trigger the command while a scan/mine/transit is active. Confirm it is blocked.
6. **After resolution** — let the processor resolve the event (or fasttick via CLI). Confirm the Discord notification embed looks right and the ship/cargo state is correct.

---

## Not yet implemented (hooks exist)

- **Fuel:** add `fuel`/`fuel_max` to ships, consume in transit resolver
- **Hull damage:** random chance in mining/transit outcomes, repair at market
- **Price fluctuation:** `orePrice()` in `game/outcomes.js` is the hook
- **Multiple ships:** remove `LIMIT 1` in `getShip()`, add `/ships` command
- **Crew:** separate table, skills feed into `getEffectiveStats()` same pattern as modules
