/**
 * orbital.js — Solar system bodies, travel time, and orbital positions
 *
 * Two models coexist:
 *
 * 1. Travel-time model (unchanged): fixed dist values, travelTimeSeconds().
 * 2. Position model: circular 2D ecliptic orbits anchored to J2000 longitudes.
 *    getBodyPosition(body, date) → {x, y} AU
 *    distanceBetweenBodies(bodyA, bodyB, date) → km
 *    Functions accept plain body objects — DB rows and procedural bodies work identically.
 */

import { createRequire } from 'module';
import { AU_IN_KM, J2000 } from './physics.js';

const require = createRequire(import.meta.url);
const solarSystem = require('../../data/solsystem.json');

// Flat map of body name (lowercase) → body object, built once at startup.
function flattenBodies(node, map = {}) {
  map[node.name.toLowerCase()] = node;
  for (const s of node.satellites ?? []) flattenBodies(s, map);
  return map;
}
export const SOLAR_BODIES = flattenBodies(solarSystem);

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
  ceres:         { name: 'Ceres',              dist: 2.8, type: 'dwarf'   },
  vesta:         { name: '4 Vesta',            dist: 2.4, type: 'asteroid'},
  belt:          { name: 'Asteroid Belt',      dist: 2.7, type: 'zone'    },
  outer_station: { name: 'Outer Belt Refinery', dist: 4.0, type: 'station' },
  jupiter:       { name: 'Jupiter',            dist: 5.2, type: 'planet'  },
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
 * Display-friendly status label for a ship.
 * Ships at zone-type bodies (e.g. the Asteroid Belt) show "drifting"
 * instead of "docked" — the internal value stays 'docked' everywhere.
 *
 * @param {{ status: string, location: string }} ship
 * @returns {string}
 */
export function displayStatus(ship) {
  if (ship.status === 'docked' && BODIES[ship.location]?.type === 'zone') return 'drifting';
  return ship.status;
}

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

/**
 * Cartesian position of a body in AU at a given date.
 * body must have { distance, longitudeJ2000 } fields (AU semi-major axis, J2000 ecliptic degrees).
 *
 * @param {{ distance: number, longitudeJ2000: number }} body
 * @param {Date} date
 * @param {{ distance: number, longitudeJ2000: number }|null} [parent] - parent body for moons
 * @returns {{ x: number, y: number }}
 */
export function getBodyPosition(body, date, parent = null) {
  const daysSinceJ2000 = (date - J2000) / 86_400_000;
  const orbitalPeriodDays = Math.pow(body.distance, 1.5) * 365.25;
  const meanMotionDeg = 360 / orbitalPeriodDays;
  const longitude = ((body.longitudeJ2000 + meanMotionDeg * daysSinceJ2000) % 360 + 360) % 360;
  const rad = longitude * (Math.PI / 180);
  const x = body.distance * Math.cos(rad);
  const y = body.distance * Math.sin(rad);

  if (parent) {
    const parentPos = getBodyPosition(parent, date);
    return { x: parentPos.x + x, y: parentPos.y + y };
  }
  return { x, y };
}

/**
 * Distance in km between two bodies at a given date.
 *
 * @param {{ distance: number, longitudeJ2000: number }} bodyA
 * @param {{ distance: number, longitudeJ2000: number }} bodyB
 * @param {Date} date
 * @param {{ distance: number, longitudeJ2000: number }|null} [parentA] - parent body if bodyA is a moon
 * @param {{ distance: number, longitudeJ2000: number }|null} [parentB] - parent body if bodyB is a moon
 * @returns {number} distance in km
 */
export function distanceBetweenBodies(bodyA, bodyB, date, parentA = null, parentB = null) {
  const posA = getBodyPosition(bodyA, date, parentA);
  const posB = getBodyPosition(bodyB, date, parentB);
  const dx = posA.x - posB.x;
  const dy = posA.y - posB.y;
  return Math.sqrt(dx * dx + dy * dy) * AU_IN_KM;
}
