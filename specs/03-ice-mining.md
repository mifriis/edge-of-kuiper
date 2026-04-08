# 03 — Ice Mining

## Goal

Introduce ice mining as the asteroid belt's primary industry. Ships can scan the belt
for a quality hint, net a haul of ice, then haul it to Ceres or the Outer Belt Refinery
to sell. Ice quality — from silica-heavy "black" to near-pure "blue" — is rolled at mine
resolution and determines sell price. Scanning is optional but strongly incentivised: a
blind net is unlikely to hit good ice.

---

## What this is, and what it is not

**In scope**
- New `belt` location (asteroid belt mining zone, 2.7 AU)
- New `outer_station` location (Outer Belt Refinery, 4.0 AU)
- Both new bodies in `data/solsystem.json` and the `BODIES` map in `lib/orbital.js`
- Belt zone rendered as a scattered-dot orbit ring in `lib/renderer.js` (see renderer section)
- `Station` type (`outer_station`) rendered as a named dot like any other body
- `/scan` at `belt`: caches a grade hint on the ship, 2 game hours
- `/mine` at `belt` only: thumper confirm + netting, 6 game hours; ice grade rolled at resolve
- `/sell` rewritten for ice: valid only at `ceres` and `outer_station`; all other ore markets removed
- DB migration: `cargo_grade TEXT DEFAULT NULL` and `scan_result TEXT DEFAULT NULL` on ships
- Ice pricing logic in `game/outcomes.js`
- Transit resolver clears `scan_result` when ship departs any location

**Out of scope — do not add**
- New module types (hasThumper, netting, mine_quality). All ships have a manual thumper
  and netting gear by default; the module system is not touched in this spec.
- Shiplift: dry/wet mass effects on travel time after loading a haul (future spec)
- Costs, fees, and mining permits (future spec)
- Price volatility — all ice prices are static
- Ship position marker (deferred)
- Multiple scan stacking (one `scan_result` per ship, last-write wins)
- Renaming `cargo_ore` (tracked as tech debt; the field stores ice tonnage for now)

All ships start with enough belt-capable kit to mine. Stats work (scan_quality from
scanner module) but no new stat keys are added here.

---

## DESIGN.md amendment

None. All new behaviour fits within existing rules.

---

## Bodies

| Key | `name` | `distance` (AU) | `longitudeJ2000` (°) | `type` |
|---|---|---|---|---|
| `belt` | Asteroid Belt | 2.7 | 90.0 | `Zone` |
| `outer_station` | Outer Belt Refinery | 4.0 | 270.0 | `Station` |

- Ceres (`ceres`, 2.8 AU) gains an ice market. Its previous ore market (`₡110/t` in `sell.js`) is removed alongside the others.

---

## Command / event flow

```
1.  /route belt          → TRANSIT event
2.  arrival at belt      → status: docked

3.  /scan                → SCAN event (mode: 'ice', 2 game hrs)
4.  scan resolves        → ship.scan_result = { hint, location: 'belt' }
                           embed: grade hint + estimated quality description

5.  /mine                → MINE event (mode: 'ice', 6 game hrs)
6.  mine resolves        → grade roll using hint
                           ship.cargo_ore += yield
                           ship.cargo_grade = grade
                           ship.scan_result = null
                           embed: tonnage + grade + description

7.  /route ceres         → TRANSIT event (clears scan_result on depart)
8.  /sell                → SELL event (10s, ice market)
9.  sell resolves        → credits += yield × icePrice(location, grade)
                           ship.cargo_ore = 0, ship.cargo_grade = null
```

/scan is optional; step 3–4 may be skipped. Without a scan, the mine step uses
`hint = null` which sharply reduces blue ice probability.

---

## File changes

### `data/solsystem.json`

Add two entries as direct children in Sol's `satellites` array:

```json
{
  "position": 5,
  "name": "Asteroid Belt",
  "distance": 2.7,
  "longitudeJ2000": 90.0,
  "type": "Zone",
  "color": "Gray",
  "satellites": []
},
{
  "position": 6,
  "name": "Outer Belt Refinery",
  "distance": 4.0,
  "longitudeJ2000": 270.0,
  "type": "Station",
  "color": "Cyan",
  "satellites": []
}
```

Existing entries keep their `position` numbers; Jupiter becomes position 7, etc.

---

### `src/lib/orbital.js` — BODIES map

Append two entries:

```js
belt:          { name: 'Asteroid Belt',      dist: 2.7, type: 'zone'    },
outer_station: { name: 'Outer Belt Refinery', dist: 4.0, type: 'station' },
```

---

### `src/lib/db.js` — `migrate()`

After the existing `CREATE TABLE` block, add idiomatic SQLite column insertion
(silently ignored on subsequent startups because SQLite raises on duplicate column names):

```js
try { db.exec(`ALTER TABLE ships ADD COLUMN cargo_grade TEXT DEFAULT NULL`); } catch {}
try { db.exec(`ALTER TABLE ships ADD COLUMN scan_result TEXT DEFAULT NULL`); } catch {}
```

No new helper functions are needed; `updateShip()` handles arbitrary field maps.

---

### `src/commands/scan.js`

Branch on `ship.location`:

**When `ship.location === 'belt'`:**
- No target option is read or required
- Duration: `SCAN_ICE_DURATION_GAME_SEC = 2 * 60 * 60` (2 game hours)
  → `SCAN_ICE_DURATION_REAL_SEC = Math.ceil(SCAN_ICE_DURATION_GAME_SEC / TIME_COMPRESSION)` → **1029 real seconds**
- Enqueue `SCAN` with `payload: { target: 'belt', mode: 'ice' }`
- `updateShip(ship.id, { status: 'scanning' })`
- Embed copy: "Passive spectral + lidar sweep underway. Results in [duration]."

**When `ship.location !== 'belt'`:**
- Existing behaviour unchanged (target option, existing SCAN event payload, no scan_result written)

---

### `src/commands/mine.js`

Replace `MINABLE` — belt is the only minable location:

```js
const MINABLE = new Set(['belt']);
```

Duration: `MINE_ICE_DURATION_GAME_SEC = 6 * 60 * 60` (6 game hours)
→ `MINE_ICE_DURATION_REAL_SEC = Math.ceil(MINE_ICE_DURATION_GAME_SEC / TIME_COMPRESSION)` → **3086 real seconds**

Payload: `{ location: 'belt', mode: 'ice' }`

Embed copy: "Thumper confirmed. Netting operation underway."

The existing ore-mine branch (ceres, vesta) is **removed**. All mine events from this point
forward originate at belt and carry `mode: 'ice'`. No fallback ore-mine path.

---

### `src/game/outcomes.js`

Append four new exported functions. Do not change any existing functions.

---

**`iceScanResult({ scanQuality = 1.0 })`**

Returns a grade hint. `scanQuality` shifts probability toward 'promising'.
Interpolate linearly by `(scanQuality - 1)` clamped to [0, 1].

| scanQuality | 'poor' | 'standard' | 'promising' |
|---|---|---|---|
| 1.0 | 0.25 | 0.55 | 0.20 |
| 2.0 | 0.15 | 0.40 | 0.45 |

```js
export function iceScanResult({ scanQuality = 1.0 }) {
  const t = Math.min(Math.max(scanQuality - 1, 0), 1);
  const pPoor      = 0.25 - 0.10 * t;
  const pPromising = 0.20 + 0.25 * t;
  // pStandard = 1 - pPoor - pPromising
  const roll = Math.random();
  const hint = roll < pPoor ? 'poor'
             : roll < (1 - pPromising) ? 'standard'
             : 'promising';
  return { hint, estQuality: SCAN_DESCRIPTIONS[hint] };
}

const SCAN_DESCRIPTIONS = {
  poor:      'Mostly silica. Low water content.',
  standard:  'Mixed composition. Moderate ice veins.',
  promising: 'Strong H₂O spectral signature. Possible blue ice.',
};
```

---

**`iceMiningGrade({ hint = null })`**

Rolls the actual ice grade at mine resolution.

| hint | 'black' | 'dirty' | 'blue' |
|---|---|---|---|
| `null` (no scan) | 0.75 | 0.22 | 0.03 |
| `'poor'` | 0.60 | 0.35 | 0.05 |
| `'standard'` | 0.30 | 0.50 | 0.20 |
| `'promising'` | 0.15 | 0.30 | 0.55 |

```js
const GRADE_TABLES = {
  null:       [0.75, 0.22, 0.03],
  poor:       [0.60, 0.35, 0.05],
  standard:   [0.30, 0.50, 0.20],
  promising:  [0.15, 0.30, 0.55],
};
const GRADE_KEYS = ['black', 'dirty', 'blue'];
const GRADE_DESCRIPTIONS = {
  black: 'Mostly silicates. Minimal recoverable water.',
  dirty: 'C-type rock with ice veins. Refinery-grade.',
  blue:  'Near-pure water ice. Premium grade.',
};

export function iceMiningGrade({ hint = null }) {
  const [pBlack, pDirty] = GRADE_TABLES[hint] ?? GRADE_TABLES[null];
  const roll = Math.random();
  const grade = roll < pBlack ? 'black'
              : roll < pBlack + pDirty ? 'dirty'
              : 'blue';
  return { grade, description: GRADE_DESCRIPTIONS[grade] };
}
```

---

**`iceMiningYield({ cargoAvailable })`**

Yield in tonnes, capped by available hold space. Range: 80–400t.

```js
export function iceMiningYield({ cargoAvailable }) {
  const raw = Math.floor(80 + Math.random() * 320);
  return Math.min(raw, cargoAvailable);
}
```

---

**`icePrice(location, grade)`**

Returns credits per tonne. Game-tunable numbers live only here.

```js
const ICE_BASE = { ceres: 180, outer_station: 240 };
const GRADE_MULT = { black: 0.3, dirty: 0.75, blue: 1.5 };

export function icePrice(location, grade) {
  const base = ICE_BASE[location] ?? 180;
  const mult = GRADE_MULT[grade]  ?? 0.75;
  return Math.round(base * mult);
}
```

---

### `src/lib/processor.js`

**`resolveTransit()`**

Add `scan_result: null` to the `updateShip` call so the cached hint is cleared on every departure:

```js
updateShip(event.ship_id, {
  location:    destination,
  status:      'docked',
  scan_result: null,
});
```

---

**`resolveScan()`**

Add a branch at the top, before existing logic:

```js
if (payload.mode === 'ice') {
  const ship  = getShip(event.player_id);
  const stats = getEffectiveStats(ship);          // from game/ships.js
  const result = iceScanResult({ scanQuality: stats.scanQuality });

  updateShip(event.ship_id, {
    status:      'docked',
    scan_result: JSON.stringify({ hint: result.hint, location: 'belt' }),
  });

  return new EmbedBuilder()
    .setColor(0x9B59B6)
    .setTitle('📡 Belt scan results')
    .setDescription(result.estQuality)
    .addFields(
      { name: 'Quality hint', value: result.hint,    inline: true },
      { name: 'Assessment',   value: result.estQuality, inline: true },
    )
    .setTimestamp()
    .setFooter({ text: 'Edge of Kuiper' });
}
// else: fall through to existing resolveScan logic
```

Import `iceScanResult` from `game/outcomes.js` and `getEffectiveStats` from `game/ships.js`
at the top of the file.

---

**`resolveMine()`**

Add a branch at the top, before existing logic:

```js
if (payload.mode === 'ice') {
  const ship   = getShip(event.player_id);
  const stats  = getEffectiveStats(ship);

  const scanData = ship.scan_result ? JSON.parse(ship.scan_result) : null;
  const hint     = scanData?.location === 'belt' ? scanData.hint : null;

  const { grade, description } = iceMiningGrade({ hint });
  const actual  = iceMiningYield({ cargoAvailable: stats.cargoMax - ship.cargo_ore });
  const full    = ship.cargo_ore + actual >= stats.cargoMax;

  updateShip(event.ship_id, {
    cargo_ore:   ship.cargo_ore + actual,
    cargo_grade: grade,
    scan_result: null,
    status:      'docked',
  });

  return new EmbedBuilder()
    .setColor(0xF4A736)
    .setTitle('⛏️ Net retrieved — Asteroid Belt')
    .setDescription(full
      ? `Hold is **full**. Head to Ceres or the Outer Belt Refinery to sell.`
      : `Net retrieved. Hold at ${ship.cargo_ore + actual}/${stats.cargoMax}t.`
    )
    .addFields(
      { name: 'Hauled',       value: `${actual}t`,   inline: true },
      { name: 'Ice grade',    value: grade,           inline: true },
      { name: 'Assessment',   value: description,     inline: false },
    )
    .setTimestamp()
    .setFooter({ text: 'Edge of Kuiper' });
}
// else: fall through to existing resolveMine logic
```

Import `iceMiningGrade`, `iceMiningYield` from `game/outcomes.js`.

---

**`resolveSell()`**

Add ice haul handling. If `ship.cargo_grade` is set, compute price via `icePrice()`:

```js
const ship      = getShip(event.player_id);
const grade     = ship.cargo_grade ?? 'dirty'; // 'dirty' fallback if somehow unset
const pricePerT = icePrice(payload.location, grade);
const ore       = ship.cargo_ore;
const revenue = ore * pricePerT;

adjustCredits(event.player_id, revenue, `Sold ${ore}t ice (${grade}) at ${payload.location}`);
updateShip(event.ship_id, {
  cargo_ore:   0,
  cargo_grade: null,
  status:      'docked',
});
```

Import `icePrice` from `game/outcomes.js`.

---

### `src/lib/renderer.js` — zone ring rendering

The renderer already filters `type !== 'Star' && type !== 'Moon'`, so `Zone` and `Station`
bodies are included in the `planets` array automatically. No change to the sort or ring
assignment logic.

Add a branch in the per-body draw loop: when `body.type === 'Zone'`, skip the single dot
and instead draw a **scattered-dot ring**:

```js
if (body.type === 'Zone') {
  // Deterministic seed from body name so the pattern is stable across renders
  const seed = seedFrom(body.name); // simple LCG seed
  const DOT_COUNT = 180;
  const BAND_WIDTH = ringStep * 0.45; // belt spans ~45% of one ring step either side

  ctx.fillStyle = 'rgba(255, 255, 255, 0.55)';
  for (let n = 0; n < DOT_COUNT; n++) {
    const angle  = lcgAngle(seed, n);            // pseudo-random [0, 2π)
    const radial = lcgRadial(seed, n, BAND_WIDTH); // pseudo-random radial offset in [-BAND_WIDTH, +BAND_WIDTH]
    const r      = orbitR + radial;
    const x      = CENTRE + r * Math.cos(angle);
    const y      = CENTRE + r * Math.sin(angle);
    ctx.beginPath();
    ctx.arc(x, y, 1.5, 0, Math.PI * 2);
    ctx.fill();
  }
  // Label at the top of the ring, not at a planet position
  ctx.fillStyle = 'rgba(204, 204, 204, 0.7)';
  ctx.fillText(body.name, CENTRE + orbitR + 6, CENTRE - 6);
  return; // skip normal dot drawing for this body
}
```

**LCG helpers** (pure, no external deps, placed at module scope):

```js
function seedFrom(str) {
  return [...str].reduce((h, c) => (Math.imul(31, h) + c.charCodeAt(0)) | 0, 0x12345678);
}

function lcgNext(seed, n) {
  // Park-Miller LCG — cheap, deterministic, good enough for visual scatter
  let s = (seed + n * 2654435761) >>> 0;
  s = Math.imul(s ^ (s >>> 16), 0x45d9f3b);
  s = Math.imul(s ^ (s >>> 16), 0x45d9f3b);
  return (s >>> 0) / 0x100000000; // [0, 1)
}

function lcgAngle(seed, n)   { return lcgNext(seed, n * 2)     * Math.PI * 2; }
function lcgRadial(seed, n, band) { return (lcgNext(seed, n * 2 + 1) - 0.5) * 2 * band; }
```

`Station` bodies (outer_station) render as a normal named dot — no special case needed.

---

### `src/tests/renderer.test.js` — zone render smoke test

Add one test: render at a fixed date that includes the new bodies and assert the buffer
is non-empty and has PNG magic bytes (`\x89PNG`). No pixel-level assertions.

---


Replace the existing `MARKETS` map and all sell logic with ice-only handling.
The existing ore markets (earth, luna, mars) are **removed** — `/sell` is now
ice-refinery only.

```js
const ICE_MARKETS = {
  ceres:         { name: 'Ceres Freeport',        basePerT: 180 },
  outer_station: { name: 'Outer Belt Refinery',   basePerT: 240 },
};
```

In `execute()`:

```js
const market = ICE_MARKETS[ship.location];
if (!market) {
  return interaction.reply({
    content: `⛔ **${BODIES[ship.location]?.name ?? ship.location}** cannot refine ice.\nHead to **Ceres** or the **Outer Belt Refinery**.`,
    ephemeral: true,
  });
}

if (ship.cargo_ore === 0) { /* nothing to sell */ }

const grade      = ship.cargo_grade ?? 'dirty'; // fallback for any legacy ore
const pricePerT  = icePrice(ship.location, grade);
const estRevenue = ship.cargo_ore * pricePerT;

// enqueue SELL with payload: { location: ship.location }
// (pricePerT omitted — processor derives from cargo_grade)
```

Import `icePrice` from `../game/outcomes.js`.

---

## Test cases

New test file: `src/tests/ice-mining.test.js`
Add to `package.json`: `"test:ice": "node ... src/tests/ice-mining.test.js"`
(covered by existing `test` glob — only the explicit alias is new)

---

### `icePrice()` — deterministic

| location | grade | expected |
|---|---|---|
| `ceres` | `blue` | 270 |
| `ceres` | `dirty` | 135 |
| `ceres` | `black` | 54 |
| `outer_station` | `blue` | 360 |
| `outer_station` | `black` | 72 |
| `unknown` | `dirty` | 135 (fallback base 180) |

---

### `iceMiningYield()` — bounds

- `iceMiningYield({ cargoAvailable: 0 })` → `0`
- `iceMiningYield({ cargoAvailable: 50 })` → `≤ 50`
- Run 1 000 iterations with `cargoAvailable: 1000`:
  - All results `≥ 80`
  - All results `≤ 400`

---

### `iceScanResult()` — probability calibration

Run 10 000 iterations with `scanQuality: 1.0`:

| hint | expected p | tolerance |
|---|---|---|
| `poor` | 0.25 | ±0.03 |
| `standard` | 0.55 | ±0.03 |
| `promising` | 0.20 | ±0.03 |

---

### `iceMiningGrade()` — probability calibration

Run 10 000 iterations with `hint: 'promising'`:

| grade | expected p | tolerance |
|---|---|---|
| `blue` | 0.55 | ±0.04 |
| `dirty` | 0.30 | ±0.04 |
| `black` | 0.15 | ±0.04 |

Run 10 000 iterations with `hint: null`:

| grade | expected p | tolerance |
|---|---|---|
| `black` | 0.75 | ±0.04 |
| `blue` | 0.03 | ±0.02 |

---

### Travel time — new bodies (add to `transit.integration.test.js`)

All at `engineSpeed: 1.0`. Values derived from
`Math.ceil(|dist_a - dist_b| / engineSpeed × 86400 / 7)`.

| from | to | expected real seconds |
|---|---|---|
| `mars` | `belt` | 14 516 |
| `belt` | `outer_station` | 16 046 |
| `belt` | `ceres` | 1 235 |
| `outer_station` | `jupiter` | 14 812 |

---

### Duration constants (deterministic)

```
SCAN_ICE_DURATION_REAL_SEC = Math.ceil(2 * 3600 / 7) = 1029
MINE_ICE_DURATION_REAL_SEC = Math.ceil(6 * 3600 / 7) = 3086
```

Assert these exact values in the test file.

---

## Resolved decisions

- **Belt display name**: "Asteroid Belt" — confirmed.
- **Station display name**: "Outer Belt Refinery" — confirmed.
