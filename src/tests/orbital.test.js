import { test } from 'node:test';
import assert from 'node:assert/strict';
import { getBodyPosition, distanceBetweenBodies, SOLAR_BODIES } from '../lib/orbital.js';
import { J2000, AU_IN_KM } from '../lib/physics.js';

// ---------------------------------------------------------------------------
// Test 1 — Pure opposition geometry (deterministic)
//
// Two synthetic bodies at longitude 0° at J2000 (t=0, no motion).
// They lie on the same radial line from Sol: minimum separation.
// Expected: (1.524 - 1.000) AU × AU_IN_KM = 78_429_284 km
// ---------------------------------------------------------------------------
test('opposition geometry: two bodies at same longitude, t=J2000', () => {
  const bodyA = { distance: 1.000, longitudeJ2000: 0 };
  const bodyB = { distance: 1.524, longitudeJ2000: 0 };
  const dist = distanceBetweenBodies(bodyA, bodyB, J2000);
  const expected = (1.524 - 1.000) * AU_IN_KM; // 78_429_284
  assert.ok(
    Math.abs(dist - expected) < 1,
    `Expected ~${expected.toFixed(0)} km, got ${dist.toFixed(0)} km`
  );
});

// ---------------------------------------------------------------------------
// Test 2 — Pure conjunction geometry (deterministic)
//
// Bodies 180° apart at t=0 (opposite sides of Sol).
// Expected: (1.000 + 1.524) AU × AU_IN_KM = 377_783_005 km
// ---------------------------------------------------------------------------
test('conjunction geometry: two bodies 180° apart, t=J2000', () => {
  const bodyA = { distance: 1.000, longitudeJ2000:   0 };
  const bodyB = { distance: 1.524, longitudeJ2000: 180 };
  const dist = distanceBetweenBodies(bodyA, bodyB, J2000);
  const expected = (1.000 + 1.524) * AU_IN_KM; // 377_783_005
  assert.ok(
    Math.abs(dist - expected) < 1,
    `Expected ~${expected.toFixed(0)} km, got ${dist.toFixed(0)} km`
  );
});

// ---------------------------------------------------------------------------
// Test 3 — 2003 Mars opposition (historical calibration)
//
// The August 27, 2003 Earth–Mars close approach (55.76 million km) is the
// closest in recorded history. That record was set because Mars was near
// perihelion — its actual distance from the Sun was ~1.38 AU, well below its
// mean of 1.524 AU. A circular orbit model using mean distances is physically
// incapable of reproducing this figure; the circular-model minimum
// Earth–Mars distance is 0.524 AU ≈ 78.4 million km.
//
// What the circular model *does* reproduce correctly is that both planets were
// near the same ecliptic longitude on that date (near opposition). The test
// asserts the model places them within 10° of each other and returns a
// distance consistent with near-opposition geometry (78–82 million km).
// ---------------------------------------------------------------------------
test('2003 Mars opposition: near-opposition geometry and distance', () => {
  const date = new Date('2003-08-27T00:00:00Z');
  const earth = SOLAR_BODIES['earth'];
  const mars  = SOLAR_BODIES['mars'];

  const posEarth = getBodyPosition(earth, date);
  const posMars  = getBodyPosition(mars,  date);

  // Angular separation as seen from Sol
  const lonEarth = Math.atan2(posEarth.y, posEarth.x) * (180 / Math.PI);
  const lonMars  = Math.atan2(posMars.y,  posMars.x)  * (180 / Math.PI);
  let separation = Math.abs(lonEarth - lonMars) % 360;
  if (separation > 180) separation = 360 - separation;

  assert.ok(
    separation < 10,
    `Angular separation should be <10° for opposition, got ${separation.toFixed(2)}°`
  );

  const dist = distanceBetweenBodies(earth, mars, date);
  const minKm = 78_000_000;
  const maxKm = 82_000_000;
  assert.ok(
    dist >= minKm && dist <= maxKm,
    `Distance should be 78–82 million km for circular opposition, got ${(dist / 1e6).toFixed(2)} million km`
  );
});

test('2023 Solar conjunction: In November. Earth and Mars was on opposite sides of the Sun', () => {
  const date = new Date('2023-11-01T00:00:00Z');
  const earth = SOLAR_BODIES['earth'];
  const mars  = SOLAR_BODIES['mars'];

  const dist = distanceBetweenBodies(earth, mars, date);
  const minKm = 370_000_000;
  const maxKm = 400_000_000;
  assert.ok(
    dist >= minKm && dist <= maxKm,
    `Distance should be 370–400 million km for solar conjunction, got ${(dist / 1e6).toFixed(2)} million km`
  );
});
