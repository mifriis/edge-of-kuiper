# 02 — Solar System Map Render

## Goal

Render the solar system as a PNG image that can be attached to a Discord message or written
to disk for CLI use. Positions come from the live orbital model — the map is always correct
for the current game timestamp. Distances are normalised so all orbits are visible; the
visual is about alignment, not true scale.

---

## What this is, and what it is not

**In scope**
- PNG render of the full solar system: Sol at centre, planets on normalised circular orbits
- Body name labels and colour dots matching `data/solsystem.json` colors
- A `centerBody` option reserved for future planet-system renders (moons) — accepted but
  not yet implemented; full solar system is the only rendered view for now
- Discord: new `/map` slash command that attaches the PNG inline
- CLI: `node src/cli.js map` writes PNG to `/tmp/kuiper-map.png` and prints the path

**Out of scope — do not add**
- Moons rendered on the full solar system view
- Ship position marker (planned follow-on feature)
- True-scale distances (orbits are normalised, that's intentional)
- Animation or SVG output
- Any orbital path arcs or trajectory lines

---

## File changes

### `src/lib/renderer.js` — new file

Pure rendering function. No Discord, no CLI, no DB. Takes a date and returns a `Buffer`
(PNG bytes).

**Exports:**

```js
export async function renderSolarSystem(date, options = {})
// options.centerBody — reserved, ignored for now
// returns: Buffer (PNG)
```

**Layout algorithm:**

```
CANVAS_SIZE = 800px × 800px
CENTRE = { x: 400, y: 400 }
MARGIN = 40px

// Sort planets by distance (AU) ascending — this gives real orbital order:
// Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, Neptune, Pluto

// Assign each planet an evenly-spaced pixel orbit radius:
//   ringStep = (CANVAS_SIZE/2 - MARGIN) / totalPlanets
//   orbitRadiusPx[i] = MARGIN + (i + 1) * ringStep
//
// This means the pixel gap between adjacent rings is identical for all planets.
// Mercury–Venus gap = Venus–Earth gap = Earth–Mars gap = … (all equal px).
// The vast AU gaps (Earth–Jupiter, Jupiter–Saturn) are intentionally collapsed.
// Orbital ORDER is preserved. Orbital SCALE is not.

// For each planet at date:
//   { x, y } = getBodyPosition(body, date)   // AU coords from orbital model
//   longitude = atan2(y, x)                  // real ecliptic angle — preserved exactly
//   dotX = CENTRE.x + orbitRadiusPx * cos(longitude)
//   dotY = CENTRE.y + orbitRadiusPx * sin(longitude)
//
// The dot sits on its evenly-spaced ring at the real orbital longitude for that date.
```

**Color mapping** (`body.color` → CSS color string):

| JSON value | CSS |
|---|---|
| `Blue` | `#4A9EFF` |
| `Red` | `#FF4444` |
| `Gray` | `#888888` |
| `DarkRed` | `#AA3333` |
| `DarkYellow` | `#CCAA00` |
| `White` | `#DDDDDD` |
| `Magenta` | `#CC44CC` |
| `DarkBlue` | `#224488` |
| `DarkMagenta` | `#882288` |
| Fallback | `#FFFFFF` |

**Dependencies:** `@napi-rs/canvas` — prebuilt binaries for macOS/Linux/Windows, no system
libraries required, drop-in `createCanvas` API. No Dockerfile changes needed.

---

### `src/commands/map.js` — new file

Discord `/map` command. Calls `renderSolarSystem(new Date())`, wraps the buffer in a
`discord.js` `AttachmentBuilder`, replies with the attachment.

```js
import { AttachmentBuilder } from 'discord.js';
import { renderSolarSystem } from '../lib/renderer.js';

// reply: ephemeral: false — the map is public
// filename for attachment: 'solar-system.png'
```

Register in `src/index.js` and `src/deploy-commands.js` alongside existing commands.

---

### `src/cli.js` — updated

Add `map` subcommand:

```
node src/cli.js map
```

Calls `renderSolarSystem(new Date())`, writes buffer to `/tmp/kuiper-map.png`,
prints the path. No other output.

---

### `package.json` — updated

Add `@napi-rs/canvas` to dependencies:

```json
"@napi-rs/canvas": "^0.1.0"
```

---

## Test cases

### File: `src/tests/renderer.test.js`

```bash
npm run test:render
```

Add to `package.json`:

```json
"test:render": "node --disable-warning=ExperimentalWarning --experimental-sqlite src/tests/renderer.test.js"
```

---

### Test 1 — Buffer is valid PNG

```
input:  date = GAME_START_DATE
output: Buffer that starts with PNG magic bytes [0x89, 0x50, 0x4E, 0x47]
assert: first 4 bytes equal PNG signature
```

---

### Test 2 — Buffer has non-trivial size

A valid 800×800 PNG with content should be larger than 10 KB.

```
input:  date = GAME_START_DATE
output: buffer.length > 10_000
assert: buffer.length > 10_000
```

---

### Test 3 — Two renders at different dates produce different buffers

Opposition and conjunction produce visibly different planet positions, so the PNG bytes
must differ.

```
inputA: date = new Date('2003-08-27T00:00:00Z')   // Mars opposition
inputB: date = new Date('2023-11-01T00:00:00Z')   // Mars conjunction
assert: Buffer.compare(bufferA, bufferB) !== 0
```

---

## Future extension points

- `options.centerBody` — when implemented, render that body at centre with its moons on
  normalised rings. Same algorithm, different body set. No changes to `renderSolarSystem`
  signature needed.
- Ship position marker — `options.shipLocation` body key; draw a distinct glyph (e.g. `▲`)
  at the corresponding orbit ring position.
