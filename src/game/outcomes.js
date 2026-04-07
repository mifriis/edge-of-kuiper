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
