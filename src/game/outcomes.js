/**
 * outcomes.js — Randomised event outcomes
 *
 * All RNG and modifier logic lives here.
 * processor.js calls these functions and applies the results — it
 * doesn't make any probabilistic decisions itself.
 *
 * scan_quality and other stats are passed in from getEffectiveStats()
 * so this layer never touches the DB directly.
 */

/**
 * Mining yield in tonnes.
 * Capped by available hold space.
 *
 * Future: body richness, mining module tier could widen the range.
 */
export function miningYield({ cargoAvailable }) {
  const raw = Math.floor(60 + Math.random() * 120);
  return Math.min(raw, cargoAvailable);
}

/**
 * Scan result.
 * Higher scan_quality shifts odds toward richer finds.
 *
 * quality 1.0 (basic):  65% moderate, 20% rich, 15% poor
 * quality 2.0 (deep):   40% moderate, 45% rich, 15% poor
 */
export function scanResult({ scanQuality = 1.0 }) {
  const richThreshold = Math.min(0.2 + (scanQuality - 1) * 0.25, 0.7);
  const roll = Math.random();

  if (roll > (1 - richThreshold)) {
    return { type: 'rich vein',     estYield: '300–450t', quality: 'high' };
  } else if (roll > 0.15) {
    return { type: 'moderate vein', estYield: '100–220t', quality: 'medium' };
  } else {
    return { type: 'played-out',    estYield: '10–40t',   quality: 'low' };
  }
}

/**
 * Market price per tonne at a given location.
 * Static for now — hook price fluctuation in here later.
 */
const BASE_PRICES = {
  earth: 150,
  luna:  140,
  mars:  130,
  ceres: 110,
};

export function orePrice(location) {
  return BASE_PRICES[location] ?? 100;
}

// ─── Ice mining ────────────────────────────────────────────────────────────

const GRADE_TABLES = {
  null:      [0.75, 0.22, 0.03],
  poor:      [0.60, 0.35, 0.05],
  standard:  [0.30, 0.50, 0.20],
  promising: [0.15, 0.30, 0.55],
};

const SCAN_DESCRIPTIONS = {
  poor:      'Mostly silica. Low water content.',
  standard:  'Mixed composition. Moderate ice veins.',
  promising: 'Strong H₂O spectral signature. Possible blue ice.',
};

/**
 * Ice scan result — initial or re-scan upgrade.
 *
 * @param {{ scanQuality?: number, currentHint?: string|null, scans?: number }} opts
 */
export function iceScanResult({ scanQuality = 1.0, currentHint = null, scans = 0 } = {}) {
  const t = Math.min(Math.max(scanQuality - 1, 0), 1);
  let hint;

  if (currentHint === null) {
    const pPoor      = 0.25 - 0.10 * t;
    const pPromising = 0.20 + 0.25 * t;
    const roll = Math.random();
    hint = roll < pPoor ? 'poor'
         : roll < (1 - pPromising) ? 'standard'
         : 'promising';
  } else if (currentHint === 'poor') {
    hint = Math.random() < (0.55 + 0.15 * t) ? 'standard' : 'poor';
  } else if (currentHint === 'standard') {
    hint = Math.random() < (0.50 + 0.20 * t) ? 'promising' : 'standard';
  } else {
    hint = currentHint; // 'promising' — already maxed
  }

  return {
    hint,
    scans:      scans + 1,
    estQuality: SCAN_DESCRIPTIONS[hint],
    gradeOdds:  GRADE_TABLES[hint],
  };
}

const GRADE_DESCRIPTIONS = {
  black: 'Mostly silicates. Minimal recoverable water.',
  dirty: 'C-type rock with ice veins. Refinery-grade.',
  blue:  'Near-pure water ice. Premium grade.',
};

/**
 * Roll actual ice grade at mine resolution.
 *
 * @param {{ hint?: string|null }} opts
 */
export function iceMiningGrade({ hint = null } = {}) {
  const [pBlack, pDirty] = GRADE_TABLES[hint] ?? GRADE_TABLES[null];
  const roll = Math.random();
  const grade = roll < pBlack ? 'black'
              : roll < pBlack + pDirty ? 'dirty'
              : 'blue';
  return { grade, description: GRADE_DESCRIPTIONS[grade] };
}

/**
 * Ice yield in tonnes, capped by available hold space.
 *
 * @param {{ cargoAvailable: number }} opts
 */
export function iceMiningYield({ cargoAvailable }) {
  const raw = Math.floor(80 + Math.random() * 320);
  return Math.min(raw, cargoAvailable);
}

const ICE_BASE  = { ceres: 180, outer_station: 240 };
const GRADE_MULT = { black: 0.3, dirty: 0.75, blue: 1.5 };

/**
 * Credits per tonne for ice at a refinery location.
 *
 * @param {string} location
 * @param {string} grade
 */
export function icePrice(location, grade) {
  const base = ICE_BASE[location]  ?? 180;
  const mult = GRADE_MULT[grade]   ?? 0.75;
  return Math.round(base * mult);
}
