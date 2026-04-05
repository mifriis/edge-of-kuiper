/**
 * db.js — SQLite layer via node:sqlite (built into Node 22+)
 *
 * Tables:
 *   players   — one row per Discord user
 *   ships     — one row per ship (players start with one)
 *   events    — the event queue
 *   ledger    — credit transaction log
 */

import { DatabaseSync } from 'node:sqlite';
import { fileURLToPath } from 'url';
import path from 'path';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const DB_PATH = process.env.DB_PATH ?? path.join(__dirname, '../../data/kuiper.db');

let _db;

export function getDb() {
  if (_db) return _db;
  _db = new DatabaseSync(DB_PATH);
  migrate(_db);
  return _db;
}

function migrate(db) {
  db.exec(`
    CREATE TABLE IF NOT EXISTS players (
      id          TEXT PRIMARY KEY,
      name        TEXT NOT NULL,
      credits     INTEGER NOT NULL DEFAULT 5000,
      created_at  INTEGER NOT NULL DEFAULT (unixepoch())
    );

    CREATE TABLE IF NOT EXISTS ships (
      id           INTEGER PRIMARY KEY AUTOINCREMENT,
      player_id    TEXT NOT NULL REFERENCES players(id),
      name         TEXT NOT NULL,
      location     TEXT NOT NULL DEFAULT 'earth',
      status       TEXT NOT NULL DEFAULT 'docked',
      engine_speed REAL    NOT NULL DEFAULT 1.0,
      cargo_ore    INTEGER NOT NULL DEFAULT 0,
      cargo_max    INTEGER NOT NULL DEFAULT 500,
      updated_at   INTEGER NOT NULL DEFAULT (unixepoch())
    );

    CREATE TABLE IF NOT EXISTS events (
      id          INTEGER PRIMARY KEY AUTOINCREMENT,
      player_id   TEXT NOT NULL REFERENCES players(id),
      ship_id     INTEGER REFERENCES ships(id),
      type        TEXT NOT NULL,
      payload     TEXT NOT NULL DEFAULT '{}',
      resolve_at  INTEGER NOT NULL,
      resolved    INTEGER NOT NULL DEFAULT 0,
      created_at  INTEGER NOT NULL DEFAULT (unixepoch())
    );

    CREATE INDEX IF NOT EXISTS idx_events_resolve ON events(resolved, resolve_at);

    CREATE TABLE IF NOT EXISTS ledger (
      id          INTEGER PRIMARY KEY AUTOINCREMENT,
      player_id   TEXT NOT NULL REFERENCES players(id),
      delta       INTEGER NOT NULL,
      reason      TEXT NOT NULL,
      created_at  INTEGER NOT NULL DEFAULT (unixepoch())
    );
  `);
}

// ─── Players ───────────────────────────────────────────────────────────────

export function ensurePlayer(discordId, name) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM players WHERE id = ?').get(discordId);
  if (existing) return existing;

  db.prepare('INSERT INTO players (id, name) VALUES (?, ?)').run(discordId, name);
  db.prepare(`INSERT INTO ships (player_id, name, location) VALUES (?, ?, 'earth')`)
    .run(discordId, `${name}'s Hauler`);

  return db.prepare('SELECT * FROM players WHERE id = ?').get(discordId);
}

export function getPlayer(discordId) {
  return getDb().prepare('SELECT * FROM players WHERE id = ?').get(discordId);
}

export function adjustCredits(discordId, delta, reason) {
  const db = getDb();
  db.prepare('UPDATE players SET credits = credits + ? WHERE id = ?').run(delta, discordId);
  db.prepare('INSERT INTO ledger (player_id, delta, reason) VALUES (?, ?, ?)').run(discordId, delta, reason);
}

// ─── Ships ─────────────────────────────────────────────────────────────────

export function getShip(discordId) {
  return getDb().prepare('SELECT * FROM ships WHERE player_id = ? LIMIT 1').get(discordId);
}

export function updateShip(shipId, fields) {
  const db   = getDb();
  const sets = Object.keys(fields).map(k => `${k} = ?`).join(', ');
  const vals = [...Object.values(fields), Math.floor(Date.now() / 1000), shipId];
  db.prepare(`UPDATE ships SET ${sets}, updated_at = ? WHERE id = ?`).run(...vals);
}

// ─── Events ────────────────────────────────────────────────────────────────

export function enqueueEvent({ playerId, shipId, type, payload, resolveAt }) {
  return getDb().prepare(`
    INSERT INTO events (player_id, ship_id, type, payload, resolve_at)
    VALUES (?, ?, ?, ?, ?)
  `).run(playerId, shipId ?? null, type, JSON.stringify(payload), resolveAt);
}

export function getPendingEvents() {
  const now = Math.floor(Date.now() / 1000);
  return getDb().prepare(`
    SELECT * FROM events
    WHERE resolved = 0 AND resolve_at <= ?
    ORDER BY resolve_at ASC
  `).all(now);
}

export function markResolved(eventId) {
  getDb().prepare('UPDATE events SET resolved = 1 WHERE id = ?').run(eventId);
}

export function getActiveEvents(playerId) {
  const now = Math.floor(Date.now() / 1000);
  return getDb().prepare(`
    SELECT * FROM events
    WHERE player_id = ? AND resolved = 0 AND resolve_at > ?
    ORDER BY resolve_at ASC
  `).all(playerId, now);
}

export function hasActiveEvent(shipId, type) {
  const now = Math.floor(Date.now() / 1000);
  return getDb().prepare(`
    SELECT 1 FROM events
    WHERE ship_id = ? AND type = ? AND resolved = 0 AND resolve_at > ?
  `).get(shipId, type, now);
}
