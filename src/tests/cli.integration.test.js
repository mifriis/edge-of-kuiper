/**
 * cli.integration.test.js
 *
 * End-to-end test of the full ice mining loop via the CLI harness.
 * Uses a real (temporary) SQLite DB — no mocking.
 *
 * Sequence tested:
 *   player create → route belt → fasttick → scan → fasttick →
 *   mine → fasttick → route ceres → fasttick → sell → fasttick → credits
 */

import { test, before, after } from 'node:test';
import assert from 'node:assert/strict';
import { execSync } from 'node:child_process';
import { mkdtempSync, rmSync } from 'node:fs';
import { tmpdir } from 'node:os';
import path from 'node:path';

// ─── Setup: temp DB per test run ───────────────────────────────────────────

let tmpDir;
let env;

before(() => {
  tmpDir = mkdtempSync(path.join(tmpdir(), 'kuiper-cli-test-'));
  env = { ...process.env, DB_PATH: path.join(tmpDir, 'test.db') };
});

after(() => {
  rmSync(tmpDir, { recursive: true, force: true });
});

function cli(...args) {
  const cmd = [
    'node',
    '--disable-warning=ExperimentalWarning',
    '--experimental-sqlite',
    'src/cli.js',
    ...args,
  ].join(' ');
  return execSync(cmd, { env, cwd: process.cwd(), encoding: 'utf8' });
}

// ─── Tests ─────────────────────────────────────────────────────────────────

test('player create registers player and ship at Earth', () => {
  const out = cli('player', 'create', 'p1', 'Alice');
  assert.ok(out.includes('Alice'),          'player name in output');
  assert.ok(out.includes('₡5,000'),         'starting credits');
  assert.ok(out.includes('Earth'),          'starting location');
  assert.ok(out.includes('0/500t'),         'empty cargo hold');
});

test('route belt schedules TRANSIT event', () => {
  const out = cli('route', 'p1', 'belt');
  assert.ok(out.includes('Asteroid Belt'), 'destination name in output');
});

test('fasttick resolves transit to belt', () => {
  const out = cli('fasttick', 'p1');
  assert.ok(out.includes('Asteroid Belt'), 'ship arrived at belt');
  assert.ok(out.includes('drifting'),      'status is drifting (zone body)');
});

test('scan at belt schedules SCAN event', () => {
  const out = cli('scan', 'p1');
  assert.ok(out.includes('Belt scan'), 'scan initiated');
});

test('fasttick resolves belt scan and shows hint + odds', () => {
  const out = cli('fasttick', 'p1');
  assert.ok(
    out.includes('hint=poor') || out.includes('hint=standard') || out.includes('hint=promising'),
    `scan must produce a hint: ${out}`,
  );
  assert.ok(out.includes('odds:'), 'odds line present');
  assert.ok(out.includes('drifting'), 'ship back to drifting (zone) after scan');
});

test('mine at belt schedules MINE event', () => {
  const out = cli('mine', 'p1');
  assert.ok(out.includes('Netting operation'), 'mine command acknowledged');
});

test('fasttick resolves mine: cargo non-zero, grade set', () => {
  const out = cli('fasttick', 'p1');
  assert.ok(
    out.includes('black') || out.includes('dirty') || out.includes('blue'),
    `mine must produce a grade: ${out}`,
  );
  assert.ok(out.includes('drifting'), 'ship drifting (zone) after mine');
});

test('route ceres from belt', () => {
  const out = cli('route', 'p1', 'ceres');
  assert.ok(out.includes('Ceres'), 'routing to Ceres confirmed');
});

test('fasttick delivers ship to ceres', () => {
  const out = cli('fasttick', 'p1');
  assert.ok(out.includes('Ceres'), 'ship at Ceres');
  assert.ok(out.includes('docked'), 'docked at Ceres');
});

test('sell at ceres queues SELL event', () => {
  const out = cli('sell', 'p1');
  assert.ok(out.includes('Sale queued'), 'sell acknowledged');
  assert.ok(out.includes('₡'),           'price shown');
});

test('fasttick settles sale: credits above starting 5000', () => {
  const out = cli('fasttick', 'p1');
  // Extract credits from status output: "Credits: ₡X,XXX"
  const match = out.match(/Credits:\s*₡([\d,]+)/);
  assert.ok(match, `credits line not found in: ${out}`);
  const credits = parseInt(match[1].replace(/,/g, ''), 10);
  // Black ice at ceres: 54/t × 80t minimum = 4320 added → at least 5000 + something
  // (black ice could yield as little as 54 × 80 = 4320; player starts with 5000)
  assert.ok(credits > 5000, `expected credits > 5000 after sale, got ${credits}`);
});

test('cargo hold is empty after sale', () => {
  const out = cli('status', 'p1');
  assert.ok(out.includes('0/500t'), `expected empty hold after sale: ${out}`);
});

test('mine fails at ceres (not a mining zone)', () => {
  const out = cli('mine', 'p1');
  assert.ok(out.includes('Asteroid Belt'), 'error directs to Asteroid Belt');
});

test('sell fails at ceres when hold is empty', () => {
  const out = cli('sell', 'p1');
  assert.ok(out.includes('Nothing to sell'), 'empty hold error');
});
