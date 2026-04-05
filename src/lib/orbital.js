/**
 * orbital.js — Solar system bodies and travel time
 *
 * Orbital positions are fixed on rails — no phase angles, no synodic math.
 * Distances are in "units" (loosely AU-inspired) and tuned for fun, not science.
 *
 * Travel time = distance / engine.speed  (both in units/day, game-time)
 * Real-world seconds = game-seconds / TIME_COMPRESSION
 *
 * Engine speed lives on the ship record in the DB.
 * Default starter engine: speed 1.0 u/day
 */

export const TIME_COMPRESSION = 7; // 1 real minute = 7 game minutes

// Distance in arbitrary "units" from Sol.
// Differences between bodies are what matters for travel.
export const BODIES = {
  sun:     { name: 'Sol',      dist: 0,    type: 'star'     },
  mercury: { name: 'Mercury',  dist: 0.4,  type: 'planet'   },
  venus:   { name: 'Venus',    dist: 0.7,  type: 'planet'   },
  earth:   { name: 'Earth',    dist: 1.0,  type: 'planet'   },
  luna:    { name: 'Luna',     dist: 1.03, type: 'moon'     },
  mars:    { name: 'Mars',     dist: 1.5,  type: 'planet'   },
  ceres:   { name: 'Ceres',    dist: 2.8,  type: 'dwarf'    },
  vesta:   { name: '4 Vesta',  dist: 2.4,  type: 'asteroid' },
  jupiter: { name: 'Jupiter',  dist: 5.2,  type: 'planet'   },
  saturn:  { name: 'Saturn',   dist: 9.6,  type: 'planet'   },
  titan:   { name: 'Titan',    dist: 9.65, type: 'moon'     },
};

const GAME_SECONDS_PER_DAY = 86400;

/**
 * Travel time in real-world seconds.
 *
 * @param {string} fromKey    - origin body key
 * @param {string} toKey      - destination body key
 * @param {number} engineSpeed - ship engine speed in units/day (game-time)
 */
export function travelTimeSeconds(fromKey, toKey, engineSpeed) {
  const a = BODIES[fromKey];
  const b = BODIES[toKey];
  if (!a || !b) throw new Error(`Unknown body: ${fromKey} or ${toKey}`);

  const dist = Math.abs(a.dist - b.dist);
  const gameDays = dist / engineSpeed;
  const gameSeconds = gameDays * GAME_SECONDS_PER_DAY;
  const realSeconds = gameSeconds / TIME_COMPRESSION;

  return Math.ceil(realSeconds);
}

/**
 * Real seconds → human-readable game-time string.
 */
export function formatGameTime(realSeconds) {
  const gameSeconds = realSeconds * TIME_COMPRESSION;
  const d = Math.floor(gameSeconds / 86400);
  const h = Math.floor((gameSeconds % 86400) / 3600);
  const m = Math.floor((gameSeconds % 3600) / 60);

  const parts = [];
  if (d > 0) parts.push(`${d}d`);
  if (h > 0) parts.push(`${h}h`);
  if (m > 0 || parts.length === 0) parts.push(`${m}m`);
  return parts.join(' ');
}

/**
 * Real seconds → human-readable real-time string.
 */
export function formatRealTime(realSeconds) {
  const d = Math.floor(realSeconds / 86400);
  const h = Math.floor((realSeconds % 86400) / 3600);
  const m = Math.floor((realSeconds % 3600) / 60);

  const parts = [];
  if (d > 0) parts.push(`${d}d`);
  if (h > 0) parts.push(`${h}h`);
  if (m > 0 || parts.length === 0) parts.push(`${m}m`);
  return parts.join(' ');
}
