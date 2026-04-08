import { test } from 'node:test';
import assert from 'node:assert/strict';
import {
  icePrice,
  iceMiningYield,
  iceScanResult,
  iceMiningGrade,
} from '../game/outcomes.js';
import { travelTimeSeconds } from '../lib/orbital.js';

// ─── Duration constants ────────────────────────────────────────────────────

test('SCAN_ICE_DURATION_REAL_SEC equals 1029', () => {
  assert.equal(Math.ceil(2 * 3600 / 7), 1029);
});

test('MINE_ICE_DURATION_REAL_SEC equals 3086', () => {
  assert.equal(Math.ceil(6 * 3600 / 7), 3086);
});

// ─── icePrice() — deterministic ───────────────────────────────────────────

test('icePrice: ceres blue = 270', () => {
  assert.equal(icePrice('ceres', 'blue'), 270);
});

test('icePrice: ceres dirty = 135', () => {
  assert.equal(icePrice('ceres', 'dirty'), 135);
});

test('icePrice: ceres black = 54', () => {
  assert.equal(icePrice('ceres', 'black'), 54);
});

test('icePrice: outer_station blue = 360', () => {
  assert.equal(icePrice('outer_station', 'blue'), 360);
});

test('icePrice: outer_station black = 72', () => {
  assert.equal(icePrice('outer_station', 'black'), 72);
});

test('icePrice: unknown location falls back to base 180, dirty = 135', () => {
  assert.equal(icePrice('unknown', 'dirty'), 135);
});

// ─── iceMiningYield() — bounds ────────────────────────────────────────────

test('iceMiningYield: 0 available → 0', () => {
  assert.equal(iceMiningYield({ cargoAvailable: 0 }), 0);
});

test('iceMiningYield: always ≤ cargoAvailable when small', () => {
  for (let i = 0; i < 100; i++) {
    assert.ok(iceMiningYield({ cargoAvailable: 50 }) <= 50);
  }
});

test('iceMiningYield: 1000 iterations in [80, 400]', () => {
  for (let i = 0; i < 1000; i++) {
    const y = iceMiningYield({ cargoAvailable: 1000 });
    assert.ok(y >= 80, `yield ${y} below 80`);
    assert.ok(y <= 400, `yield ${y} above 400`);
  }
});

// ─── iceScanResult() — initial scan calibration ───────────────────────────

test('iceScanResult initial scan: probabilities near expected (scanQuality 1.0)', () => {
  const N = 10_000;
  const counts = { poor: 0, standard: 0, promising: 0 };
  for (let i = 0; i < N; i++) counts[iceScanResult({ scanQuality: 1.0 }).hint]++;
  assert.ok(Math.abs(counts.poor      / N - 0.25) < 0.03, `poor p=${(counts.poor/N).toFixed(3)}`);
  assert.ok(Math.abs(counts.standard  / N - 0.55) < 0.03, `standard p=${(counts.standard/N).toFixed(3)}`);
  assert.ok(Math.abs(counts.promising / N - 0.20) < 0.03, `promising p=${(counts.promising/N).toFixed(3)}`);
});

// ─── iceScanResult() — re-scan upgrade calibration ────────────────────────

test('iceScanResult rescan from poor: upgrade to standard ~55%', () => {
  const N = 10_000;
  let upgraded = 0;
  for (let i = 0; i < N; i++) {
    if (iceScanResult({ scanQuality: 1.0, currentHint: 'poor', scans: 1 }).hint === 'standard') upgraded++;
  }
  assert.ok(Math.abs(upgraded / N - 0.55) < 0.04, `upgraded p=${(upgraded/N).toFixed(3)}`);
});

test('iceScanResult rescan from standard: upgrade to promising ~50%', () => {
  const N = 10_000;
  let upgraded = 0;
  for (let i = 0; i < N; i++) {
    if (iceScanResult({ scanQuality: 1.0, currentHint: 'standard', scans: 1 }).hint === 'promising') upgraded++;
  }
  assert.ok(Math.abs(upgraded / N - 0.50) < 0.04, `upgraded p=${(upgraded/N).toFixed(3)}`);
});

test('iceScanResult rescan from promising: always stays promising', () => {
  for (let i = 0; i < 1000; i++) {
    assert.equal(
      iceScanResult({ scanQuality: 1.0, currentHint: 'promising', scans: 2 }).hint,
      'promising',
    );
  }
});

test('iceScanResult increments scans counter', () => {
  const r1 = iceScanResult({ scans: 0 });
  assert.equal(r1.scans, 1);
  const r2 = iceScanResult({ currentHint: r1.hint, scans: r1.scans });
  assert.equal(r2.scans, 2);
});

// ─── iceMiningGrade() — probability calibration ───────────────────────────

test('iceMiningGrade promising: blue ~55%', () => {
  const N = 10_000;
  const counts = { black: 0, dirty: 0, blue: 0 };
  for (let i = 0; i < N; i++) counts[iceMiningGrade({ hint: 'promising' }).grade]++;
  assert.ok(Math.abs(counts.blue  / N - 0.55) < 0.04, `blue p=${(counts.blue/N).toFixed(3)}`);
  assert.ok(Math.abs(counts.dirty / N - 0.30) < 0.04, `dirty p=${(counts.dirty/N).toFixed(3)}`);
  assert.ok(Math.abs(counts.black / N - 0.15) < 0.04, `black p=${(counts.black/N).toFixed(3)}`);
});

test('iceMiningGrade null (no scan): mostly black, rarely blue', () => {
  const N = 10_000;
  const counts = { black: 0, dirty: 0, blue: 0 };
  for (let i = 0; i < N; i++) counts[iceMiningGrade({ hint: null }).grade]++;
  assert.ok(Math.abs(counts.black / N - 0.75) < 0.04, `black p=${(counts.black/N).toFixed(3)}`);
  assert.ok(Math.abs(counts.blue  / N - 0.03) < 0.02, `blue p=${(counts.blue/N).toFixed(3)}`);
});

// ─── Travel time — new bodies ──────────────────────────────────────────────

test('travel time mars → belt = 14 812 real seconds', () => {
  // mars dist in BODIES is 1.5 (game rounded), belt is 2.7 → |1.2| / 1.0 × 86400 / 7 = 14812
  assert.equal(travelTimeSeconds('mars', 'belt', 1.0), 14_812);
});

test('travel time belt → outer_station = 16 046 real seconds', () => {
  assert.equal(travelTimeSeconds('belt', 'outer_station', 1.0), 16_046);
});

test('travel time belt → ceres = 1 235 real seconds', () => {
  assert.equal(travelTimeSeconds('belt', 'ceres', 1.0), 1_235);
});

test('travel time outer_station → jupiter = 14 812 real seconds', () => {
  assert.equal(travelTimeSeconds('outer_station', 'jupiter', 1.0), 14_812);
});
