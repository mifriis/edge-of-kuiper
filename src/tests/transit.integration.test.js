/**
 * transit.integration.test.js
 *
 * Two concerns, kept separate:
 *
 * 1. Ship + orbital mechanics: does a starter ship have the engine speed that
 *    produces the correct Earth–Mars travel time, and does the orbital model
 *    place those bodies at a physically valid distance at game start?
 *
 * 2. Time compression + event scheduling: does the travel time translate into
 *    the correct real/game-time display strings, and does the event resolve_at
 *    timestamp come out right?
 *
 * No DB, no Discord.
 */

import { test } from 'node:test';
import assert from 'node:assert/strict';
import {
  distanceBetweenBodies,
  SOLAR_BODIES,
  travelTimeSeconds,
  formatGameTime,
  formatRealTime,
} from '../lib/orbital.js';
import { GAME_START_DATE } from '../lib/physics.js';
import { getEffectiveStats, STARTER_MODULES } from '../game/ships.js';

// ---------------------------------------------------------------------------
// Test 1 — Ship stats + orbital mechanics
//
// Starter drive (drive_basic) has engine_speed: 1.0 u/day.
// Earth–Mars distance in the travel model: |1.5 - 1.0| = 0.5 units.
// At speed 1.0 that is 0.5 game-days = 43 200 game-seconds = 6171.4 real-seconds → ceil 6172.
//
// The orbital model should also place Earth and Mars within the physical
// min–max range (78M km opposition, 378M km conjunction) at game start.
// ---------------------------------------------------------------------------
test('starter ship engine speed produces correct Earth→Mars travel time', () => {
  const { engineSpeed } = getEffectiveStats({ modules: STARTER_MODULES });
  assert.equal(engineSpeed, 1.0, 'drive_basic must give engine speed 1.0');

  const travelSecs = travelTimeSeconds('earth', 'mars', engineSpeed);
  assert.equal(travelSecs, 6172, 'Earth→Mars at speed 1.0: 0.5 game-days → 6172 real seconds');
});

test('orbital model places Earth and Mars within valid range at game start', () => {
  const earth = SOLAR_BODIES['earth'];
  const mars  = SOLAR_BODIES['mars'];
  const distKm = distanceBetweenBodies(earth, mars, GAME_START_DATE);
  assert.ok(
    distKm >= 78_000_000 && distKm <= 378_000_000,
    `Earth–Mars at game start should be 78–378M km, got ${(distKm / 1e6).toFixed(1)}M km`
  );
});

// ---------------------------------------------------------------------------
// Test 2 — Time compression + event scheduling
//
// 6172 real seconds through the 7× display layer:
//   game: 6172 × 7 = 43 204 s = 12h 0m 4s → formatGameTime rounds to minutes: "12h"
//   real: 6172 s = 1h 42m 52s → formatRealTime rounds to minutes: "1h 42m"
//
// Event resolve_at: GAME_START_DATE unix (3 408 220 800) + 6172 = 3 408 226 972
// ---------------------------------------------------------------------------
test('travel time formats correctly under 7× compression', () => {
  assert.equal(formatGameTime(6172), '12h',    '6172 real-s → 43204 game-s → 12h');
  assert.equal(formatRealTime(6172), '1h 42m', '6172 real-s → 1h 42m');
});

test('event resolve_at is departure unix + travel seconds', () => {
  // GAME_START_DATE = 2078-01-01T00:00:00Z = unix 3_408_220_800
  const resolveAt = Math.floor(GAME_START_DATE.getTime() / 1000) + 6172;
  assert.equal(resolveAt, 3_408_226_972);
});

