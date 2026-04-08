/**
 * renderer.js — Solar system PNG renderer
 *
 * Pure function: takes a date, returns a PNG Buffer. No Discord, no DB, no CLI.
 *
 * Layout: Sol at centre, planets on evenly-spaced concentric rings.
 * Orbital ORDER (inner → outer) is preserved. Orbital SCALE is not.
 * Each planet's angle is the real ecliptic longitude from the orbital model.
 */

import { createCanvas, GlobalFonts } from '@napi-rs/canvas';
import { SOLAR_BODIES, getBodyPosition } from './orbital.js';
import { fileURLToPath } from 'url';
import path from 'path';
import fs from 'fs';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const fontPath = path.join(__dirname, 'renderer-font/inter.ttf');
GlobalFonts.registerFromPath(
  fontPath,
  'Inter'
);

const CANVAS_SIZE = 800;
const CENTRE      = CANVAS_SIZE / 2;

// ─── LCG helpers for deterministic asteroid belt scatter ───────────────────

function seedFrom(str) {
  return [...str].reduce((h, c) => (Math.imul(31, h) + c.charCodeAt(0)) | 0, 0x12345678);
}

function lcgNext(seed, n) {
  let s = (seed + n * 2654435761) >>> 0;
  s = Math.imul(s ^ (s >>> 16), 0x45d9f3b);
  s = Math.imul(s ^ (s >>> 16), 0x45d9f3b);
  return (s >>> 0) / 0x100000000;
}

function lcgAngle(seed, n)        { return lcgNext(seed, n * 2) * Math.PI * 2; }
function lcgRadial(seed, n, band) { return (lcgNext(seed, n * 2 + 1) - 0.5) * 2 * band; }

const COLOR_MAP = {
  Blue:        '#4A9EFF',
  Red:         '#FF4444',
  Gray:        '#888888',
  DarkRed:     '#AA3333',
  DarkYellow:  '#CCAA00',
  White:       '#DDDDDD',
  Magenta:     '#CC44CC',
  DarkBlue:    '#224488',
  DarkMagenta: '#882288',
  Cyan:        '#44DDDD',
};

/**
 * Render the solar system at the given date and return a PNG Buffer.
 *
 * @param {Date} date
 * @param {object} [options]
 * @param {string} [options.centerBody]  Reserved — not yet implemented
 * @returns {Buffer} PNG bytes
 */
export async function renderSolarSystem(date, options = {}) {
  // Collect planets (no Star, no Moon) sorted by distance ascending → orbital order
  const planets = Object.values(SOLAR_BODIES)
    .filter(b => b.type !== 'Star' && b.type !== 'Moon')
    .sort((a, b) => a.distance - b.distance);

  // Ring step: evenly space rings. Dividing by (planets.length + 2) gives two steps
  // of margin — one inside the innermost ring, one outside the outermost — so labels
  // on edge planets have room to breathe.
  const ringStep    = CANVAS_SIZE / 2 / (planets.length + 2);
  const canvas      = createCanvas(CANVAS_SIZE, CANVAS_SIZE);
  const ctx         = canvas.getContext('2d');

  // Background
  ctx.fillStyle = '#0a0a0f';
  ctx.fillRect(0, 0, CANVAS_SIZE, CANVAS_SIZE);

  // Orbit rings
  ctx.strokeStyle = 'rgba(255, 255, 255, 0.25)';
  ctx.lineWidth   = 1;
  planets.forEach((_, i) => {
    const r = (i + 1) * ringStep;
    ctx.beginPath();
    ctx.arc(CENTRE, CENTRE, r, 0, Math.PI * 2);
    ctx.stroke();
  });

  // Sol — no label
  ctx.fillStyle = COLOR_MAP.DarkYellow;
  ctx.beginPath();
  ctx.arc(CENTRE, CENTRE, 20, 0, Math.PI * 2);
  ctx.fill();

  // Planets
  ctx.font = '22px Inter';
  planets.forEach((body, i) => {
    const orbitR    = (i + 1) * ringStep;
    const pos       = getBodyPosition(body, date);
    const longitude = Math.atan2(pos.y, pos.x);
    const dotX      = CENTRE + orbitR * Math.cos(longitude);
    const dotY      = CENTRE + orbitR * Math.sin(longitude);

    // ── Zone bodies: scattered-dot ring instead of a single dot ───────────
    if (body.type === 'Zone') {
      const seed      = seedFrom(body.name);
      const DOT_COUNT = 180;
      const BAND_WIDTH = ringStep * 0.45;

      ctx.fillStyle = 'rgba(255, 255, 255, 0.55)';
      for (let n = 0; n < DOT_COUNT; n++) {
        const angle  = lcgAngle(seed, n);
        const radial = lcgRadial(seed, n, BAND_WIDTH);
        const r      = orbitR + radial;
        const x      = CENTRE + r * Math.cos(angle);
        const y      = CENTRE + r * Math.sin(angle);
        ctx.beginPath();
        ctx.arc(x, y, 1.5, 0, Math.PI * 2);
        ctx.fill();
      }
      ctx.fillStyle = 'rgba(204, 204, 204, 0.7)';
      ctx.fillText(body.name, CENTRE + orbitR + 6, CENTRE - 6);
      return;
    }

    // ── Normal body dot ────────────────────────────────────────────────────
    ctx.fillStyle = COLOR_MAP[body.color] ?? '#ffffff';
    ctx.beginPath();
    ctx.arc(dotX, dotY, 10, 0, Math.PI * 2);
    ctx.fill();

    // Label — offset to avoid overlapping the dot
    ctx.fillStyle = '#cccccc';
    ctx.fillText(body.name, dotX + 14, dotY + 7);
  });

  return canvas.toBuffer('image/png');
}
