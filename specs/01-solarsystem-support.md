# 01 — Solar System Support

## Goal

Introduce a physically-grounded but intentionally simple 2D circular orbit model.
Bodies advance along their real mean orbital rates, anchored to published J2000 ecliptic
longitudes, so the solar system has a meaningful spatial layout at any game timestamp and
genuine inter-body distances can be calculated.

---

## What this is, and what it is not

**In scope**
- Circular orbits in the 2D ecliptic plane
- Mean motion derived from Kepler's third law (`T = a^1.5 × 365.25 days`)
- Positions anchored to J2000.0 ecliptic longitudes (NASA JPL planetary fact sheets)
- `getBodyPosition(body, date)` → `{x, y}` in AU
- `distanceBetweenBodies(bodyA, bodyB, date)` → distance in km
- Inner system, asteroid belt, and Kuiper Belt form the intended route structure;
  bodies are added incrementally as those routes are designed

**Out of scope — do not add**
- Orbital eccentricity / elliptical orbits
- Orbital inclinations (everything is in the ecliptic plane)
- Lagrange points
- Perturbations and gravitational interactions
- Three-body problem

The model is intentionally "on rails". A ship's travel time is still governed by the existing
`travelTimeSeconds` formula (no change). The new system only adds the ability to ask
"where is body X at time T?" and "how far apart are X and Y at time T?".

---

## DESIGN.md amendment

The existing hard rule _"Do not add real orbital mechanics. Ever."_ is superseded by:

> **Orbital model — circular 2D ecliptic (rings model)**
> - Bodies move in circular orbits in the ecliptic plane. No eccentricity, no inclination.
> - Position: `longitude(t) = longitudeJ2000 + (360 / orbitalPeriodDays) × daysSinceJ2000(t)`
> - `orbitalPeriodDays` is **derived** from the semi-major axis via Kepler's third law:
>   `T = distance^1.5 × 365.25` — it is never stored in the data file.
> - Physical constants (AU, epoch dates) live in `src/lib/physics.js`. Import from there.
> - No Lagrange points, no perturbations, no three-body problem. Ever.

---

## File changes

### `data/solsystem.json` — moved and updated

Move from the repository root to `data/solsystem.json`.

**Schema changes per body node:**

| Field | Change | Notes |
|---|---|---|
| `originDegrees` | **Replaced** by `longitudeJ2000` | Real mean ecliptic longitude at J2000.0, degrees |
| `velocity` | **Removed** | km/s orbital speed is no longer used; period is derived from `distance` |
| `distance` | Clarified meaning | This is the **semi-major axis in AU** (it was already close to that) |

Luna is a special case: its `distance` (0.00257 AU) is relative to Earth, and its
`longitudeJ2000` is its mean ecliptic longitude *relative to Earth* at J2000.

**J2000 ecliptic longitudes** (source: NASA JPL planetary fact sheets / Meeus _Astronomical Algorithms_):

| Body | `longitudeJ2000` (°) | `distance` (AU, semi-major axis) |
|---|---|---|
| Mercury | 252.25 | 0.387 |
| Venus | 181.98 | 0.723 |
| Earth | 100.46 | 1.000 |
| Luna | 218.32 *(relative to Earth)* | 0.00257 |
| Mars | 355.45 | 1.524 |
| Jupiter | 34.40 | 5.203 |
| Saturn | 49.94 | 9.537 |
| Uranus | 313.23 | 19.191 |
| Neptune | 304.88 | 30.069 |
| Pluto | 238.93 | 39.482 |

---

### `src/lib/physics.js` — new file

Physics constants and epoch dates. All other modules import constants from here; nothing is
duplicated elsewhere.

```js
// Physical constants
export const AU_IN_KM = 149_597_871;  // 1 AU in km (IAU 2012 exact value)

// Epoch anchors
export const J2000 = new Date('2000-01-01T12:00:00Z');  // J2000.0 standard epoch

// Game epoch — the in-universe "present" at server first start
export const GAME_START_DATE = new Date('2078-01-01T00:00:00Z');
```

Future constants (gravity, rocket equation, etc.) belong here too.

---

### `src/lib/orbital.js` — updated

- Import `solsystem.json` from `data/solsystem.json` synchronously at **module load time**
  (one `JSON.parse` at startup, never per-request).
- Import `AU_IN_KM` and `J2000` from `src/lib/physics.js`.
- Add `getBodyPosition(body, date)` and `distanceBetweenBodies(bodyA, bodyB, date)`.
- Keep `BODIES`, `travelTimeSeconds`, `formatGameTime`, `formatRealTime` unchanged.
  The travel-time model is unaffected; it uses the same `dist` values as before.

**Function signatures accept body objects, not string keys.**
`getBodyPosition(body, date)` and `distanceBetweenBodies(bodyA, bodyB, date)` receive plain
objects with `distance` and `longitudeJ2000` fields. The functions have no awareness of
where a body came from. This is intentional:
- When bodies move to the DB, callers pass DB rows directly — orbital math is unchanged.
- Procedurally generated bodies (e.g. ore-rich asteroids spawned by the game loop) get
  stamped with `distance` + `longitudeJ2000` at creation and pass straight in.
Only the lookup/loading layer changes on DB migration, not the orbital functions.

**`getBodyPosition(body, date)`**

Returns the body's Cartesian position `{x, y}` in AU at the given JS `Date`.

```
daysSinceJ2000     = (date - J2000) / 86_400_000          // ms → days
orbitalPeriodDays  = body.distance^1.5 × 365.25            // Kepler's third law
meanMotionDeg      = 360 / orbitalPeriodDays               // degrees per day
longitude          = (body.longitudeJ2000 + meanMotionDeg × daysSinceJ2000) mod 360
x                  = body.distance × cos(longitude × π/180)
y                  = body.distance × sin(longitude × π/180)
return { x, y }                                            // AU
```

Luna special-case: compute Earth's position first, then add Luna's local `{x, y}` to it.

**`distanceBetweenBodies(bodyA, bodyB, date)`**

```
posA   = getBodyPosition(bodyA, date)
posB   = getBodyPosition(bodyB, date)
dx     = posA.x - posB.x
dy     = posA.y - posB.y
distAU = sqrt(dx² + dy²)
return distAU × AU_IN_KM                                   // km
```

Both functions should throw a clear error if either body is not found in the solar system
data (same pattern as existing `travelTimeSeconds`).

---

## Test cases

### File: `src/tests/orbital.test.js`

Use `node:test` (zero additional dependencies). Run via:

```json
"test": "node --experimental-sqlite --disable-warning=ExperimentalWarning src/tests/orbital.test.js"
```

---

### Test 1 — Pure opposition geometry (deterministic, no real date needed)

Construct two synthetic body objects directly in the test (bypassing JSON loading) so the
result is mathematically certain.

```
bodyA = { distance: 1.000, longitudeJ2000: 0 }  // "Earth-like"
bodyB = { distance: 1.524, longitudeJ2000: 0 }  // "Mars-like"
date  = J2000                                   // t = 0 → no motion applied
```

Both longitudes are 0° at t=0. They are on the same radial line from Sol (opposition).

Expected distance:
```
(1.524 - 1.000) AU × 149_597_871 km/AU = 78_429_284 km  (≈ 78.4 million km)
```

Assert within ±1 km (pure arithmetic, no floating point accumulation).

---

### Test 2 — Pure conjunction geometry (deterministic)

```
bodyA = { distance: 1.000, longitudeJ2000:   0 }
bodyB = { distance: 1.524, longitudeJ2000: 180 }
date  = J2000
```

Both are on opposite sides of Sol.

Expected distance:
```
(1.000 + 1.524) AU × 149_597_871 km/AU = 377_783_005 km  (≈ 377.8 million km)
```

Assert within ±1 km.

---

### Test 3 — 2003 Mars opposition (historical calibration test)

**Background note (must appear as a comment in the test file):**

> The August 27, 2003 Earth–Mars close approach (55.76 million km) is the
> closest in recorded history. That record was set because Mars was near
> **perihelion** — its actual distance from the Sun was ~1.38 AU, well below
> its mean of 1.524 AU. A circular orbit model using mean distances is
> physically incapable of reproducing this figure; the circular-model minimum
> Earth–Mars distance is 0.524 AU ≈ 78.4 million km.
>
> What the circular model *does* reproduce correctly is that both planets were
> near the same ecliptic longitude on that date (near opposition). The test
> below asserts the model places them within ±5° of each other and returns a
> distance consistent with near-opposition geometry.

Test inputs:
```
date  = new Date('2003-08-27T00:00:00Z')
bodyA = Earth  (from solsystem.json)
bodyB = Mars   (from solsystem.json)
```

Assertions:
1. The angular separation between the two bodies (as seen from Sol) is less than 10°.
   (Confirms the model correctly reproduces the opposition geometry for that date.)
2. The computed distance is between 78 million km and 82 million km.
   (Circular-orbit minimum ± small tolerance for floating point and rounding in
   the J2000 longitude table values.)


