import { test } from 'node:test';
import assert from 'node:assert/strict';
import { renderSolarSystem } from '../lib/renderer.js';
import { GAME_START_DATE } from '../lib/physics.js';

// ---------------------------------------------------------------------------
// Test 1 — Buffer starts with PNG magic bytes
// PNG signature: 0x89 0x50 0x4E 0x47 (first 4 bytes)
// ---------------------------------------------------------------------------
test('renderSolarSystem returns a valid PNG buffer', async () => {
  const buf = await renderSolarSystem(GAME_START_DATE);
  assert.ok(buf instanceof Buffer, 'result must be a Buffer');
  assert.equal(buf[0], 0x89, 'byte 0 must be 0x89');
  assert.equal(buf[1], 0x50, 'byte 1 must be 0x50 (P)');
  assert.equal(buf[2], 0x4E, 'byte 2 must be 0x4E (N)');
  assert.equal(buf[3], 0x47, 'byte 3 must be 0x47 (G)');
});

// ---------------------------------------------------------------------------
// Test 2 — Buffer is non-trivially sized (800×800 with content > 10 KB)
// ---------------------------------------------------------------------------
test('renderSolarSystem buffer is larger than 10 KB', async () => {
  const buf = await renderSolarSystem(GAME_START_DATE);
  assert.ok(
    buf.length > 10_000,
    `PNG buffer should be >10 KB, got ${buf.length} bytes`
  );
});

// ---------------------------------------------------------------------------
// Test 3 — Two renders at different dates produce different buffers
// Mars opposition vs conjunction → visibly different planet positions → different bytes
// ---------------------------------------------------------------------------
test('renders at different dates produce different buffers', async () => {
  const [a, b] = await Promise.all([
    renderSolarSystem(new Date('2003-08-27T00:00:00Z')), // Mars opposition
    renderSolarSystem(new Date('2023-11-01T00:00:00Z')), // Mars conjunction
  ]);
  assert.notEqual(Buffer.compare(a, b), 0, 'buffers for different dates must differ');
});
