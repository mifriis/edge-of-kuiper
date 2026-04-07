/**
 * ships.js — Ship stats and module resolution
 *
 * The DB ships table stores raw fields (location, status, etc.) plus
 * a JSON `modules` column listing installed module IDs per slot.
 *
 * getEffectiveStats(ship) is the single source of truth for what a
 * ship can actually do. Commands should always call this rather than
 * reading raw DB columns for anything module-affected.
 *
 * Raw DB columns that are now DERIVED (do not read directly):
 *   engine_speed  → getEffectiveStats().engineSpeed
 *   cargo_max     → getEffectiveStats().cargoMax
 *
 * Raw DB columns that are still direct (not module-affected yet):
 *   location, status, cargo_ore, player_id, name
 */

import { MODULES, SLOT_TYPES } from './modules.js';

// Default module loadout for a new ship
export const STARTER_MODULES = {
  [SLOT_TYPES.DRIVE]:   'drive_basic',
  [SLOT_TYPES.CARGO]:   'cargo_basic',
  [SLOT_TYPES.SCANNER]: 'scanner_basic',
};

// Base stats before any modules are applied
const BASE_STATS = {
  engine_speed: 0,
  cargo_max:    0,
  scan_quality: 0,
};

/**
 * Derive effective ship stats from installed modules.
 *
 * @param {object} ship - ship row from DB (with modules as JSON string or object)
 * @returns {object} effective stats
 */
export function getEffectiveStats(ship) {
  const modules = parseModules(ship.modules) ?? STARTER_MODULES;
  const stats   = { ...BASE_STATS };

  for (const moduleId of Object.values(modules)) {
    if (!moduleId) continue;
    const mod = MODULES[moduleId];
    if (!mod) continue;
    for (const [key, val] of Object.entries(mod.stats)) {
      stats[key] = (stats[key] ?? 0) + val;
    }
  }

  return {
    engineSpeed:  stats.engine_speed,
    cargoMax:     stats.cargo_max,
    scanQuality:  stats.scan_quality,
    // add more derived stats here as systems expand
  };
}

/**
 * Install a module into a slot.
 * Returns the new modules object (caller must persist to DB).
 *
 * @param {object} ship
 * @param {string} slotType  - from SLOT_TYPES
 * @param {string} moduleId  - key from MODULES
 * @returns {object} updated modules map
 */
export function installModule(ship, slotType, moduleId) {
  const mod = MODULES[moduleId];
  if (!mod) throw new Error(`Unknown module: ${moduleId}`);
  if (mod.slot !== slotType) throw new Error(`Module ${moduleId} is not a ${slotType} module`);

  const modules = parseModules(ship.modules) ?? { ...STARTER_MODULES };
  modules[slotType] = moduleId;
  return modules;
}

/**
 * Return a human-readable summary of installed modules.
 */
export function describeModules(ship) {
  const modules = parseModules(ship.modules) ?? STARTER_MODULES;
  return Object.entries(modules)
    .filter(([, id]) => id)
    .map(([slot, id]) => {
      const mod = MODULES[id];
      return `${slot}: ${mod?.name ?? id} (tier ${mod?.tier ?? '?'})`;
    });
}

function parseModules(raw) {
  if (!raw) return null;
  if (typeof raw === 'object') return raw;
  try { return JSON.parse(raw); } catch { return null; }
}
