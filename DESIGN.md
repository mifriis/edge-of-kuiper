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

## Adding things

**New body:** add to `data/solsystem.json` with `distance` (AU semi-major axis) and `longitudeJ2000` (J2000 ecliptic longitude, degrees). Nothing else changes.

**New module:** add to `MODULES` in `game/modules.js` with a `stats` block. Nothing else changes.

**New stat:** add to `BASE_STATS` in `game/ships.js`, add to relevant module `stats` blocks,
expose in `getEffectiveStats()` return object.

**New event type:**
1. Resolver function in `lib/processor.js` → `resolveEvent()` switch
2. Outcome logic in `game/outcomes.js` if randomness involved
3. Enqueue from command via `enqueueEvent()`
4. Add CLI command in `src/cli.js` for `fasttick` testing

---

## Data model notes

`ships.modules` — JSON column, maps slot type → module ID:
```json
{ "drive": "drive_ion", "cargo": "cargo_basic", "scanner": "scanner_basic" }
```

Player IDs are arbitrary strings (`TEXT`). Discord sends snowflakes, CLI uses short IDs like `p1`.
Never assume format.

Multiple ships per player is supported by the schema. Commands currently use `LIMIT 1`.

---

## Not yet implemented (hooks exist)

- **Fuel:** add `fuel`/`fuel_max` to ships, consume in transit resolver
- **Hull damage:** random chance in mining/transit outcomes, repair at market
- **Price fluctuation:** `orePrice()` in `game/outcomes.js` is the hook
- **Multiple ships:** remove `LIMIT 1` in `getShip()`, add `/ships` command
- **Crew:** separate table, skills feed into `getEffectiveStats()` same pattern as modules
