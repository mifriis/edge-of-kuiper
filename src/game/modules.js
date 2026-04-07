/**
 * modules.js — Module catalogue
 *
 * A module is a piece of equipment installed in a ship slot.
 * Each module has a type, a tier, and a stats block that describes
 * what it changes on the ship. Stats are additive modifiers unless
 * noted as multipliers (key ending in _mult).
 *
 * Adding a new module: drop an entry in MODULES. Nothing else needs
 * to change — getEffectiveStats() in ships.js picks it up automatically.
 */

export const SLOT_TYPES = {
  DRIVE:   'drive',    // propulsion — affects engine_speed
  CARGO:   'cargo',    // hold expansion — affects cargo_max
  SCANNER: 'scanner',  // survey gear — affects scan quality/duration
  ARMOR:   'armor',    // hull — affects future damage/risk systems
};

// tier 1 = starter, tier 2 = mid, tier 3 = endgame
export const MODULES = {
  // ── Drives ──────────────────────────────────────────────────────
  drive_basic: {
    name:        'Basic Thruster',
    slot:        SLOT_TYPES.DRIVE,
    tier:        1,
    description: 'Standard chemical drive. Gets you there eventually.',
    stats:       { engine_speed: 1.0 },
    cost:        0,   // starter — given for free
  },
  drive_ion: {
    name:        'Ion Drive Mk1',
    slot:        SLOT_TYPES.DRIVE,
    tier:        2,
    description: 'Efficient ion drive. Meaningfully faster on long hauls.',
    stats:       { engine_speed: 2.0 },
    cost:        8000,
  },
  drive_epstein: {
    name:        'Epstein Drive',
    slot:        SLOT_TYPES.DRIVE,
    tier:        3,
    description: 'The good stuff. Continuous high-thrust. Outer system viable.',
    stats:       { engine_speed: 5.0 },
    cost:        40000,
  },

  // ── Cargo ────────────────────────────────────────────────────────
  cargo_basic: {
    name:        'Standard Hold',
    slot:        SLOT_TYPES.CARGO,
    tier:        1,
    description: 'Default cargo configuration.',
    stats:       { cargo_max: 500 },
    cost:        0,
  },
  cargo_expanded: {
    name:        'Expanded Hold',
    slot:        SLOT_TYPES.CARGO,
    tier:        2,
    description: 'Retrofitted bulk storage. More haul per trip.',
    stats:       { cargo_max: 1000 },
    cost:        6000,
  },

  // ── Scanners ─────────────────────────────────────────────────────
  scanner_basic: {
    name:        'Passive Array',
    slot:        SLOT_TYPES.SCANNER,
    tier:        1,
    description: 'Basic spectral sensor. Results are approximate.',
    stats:       { scan_quality: 1.0 },
    cost:        0,
  },
  scanner_deep: {
    name:        'Deep Scan Suite',
    slot:        SLOT_TYPES.SCANNER,
    tier:        2,
    description: 'Active/passive combo. Shifts ore estimate odds toward richer finds.',
    stats:       { scan_quality: 2.0 },
    cost:        5000,
  },
};

export function getModule(id) {
  const mod = MODULES[id];
  if (!mod) throw new Error(`Unknown module: ${id}`);
  return { id, ...mod };
}
