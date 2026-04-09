---
name: spec-planning
description: "Spec-first feature planning for Edge of Kuiper. Use when: starting a new feature, writing a spec, asked to plan something, told 'planning mode', or before implementing anything non-trivial. Covers the full workflow: ask clarifying questions → write specs/NN-name.md → iterate with user → update DESIGN.md → implement → verify tests."
argument-hint: "Feature name or description to plan"
---

# Spec Planning — Edge of Kuiper

## When to use

- User says "planning mode" or "let's plan"
- User asks to plan, design, or spec a feature
- A task is non-trivial (touches multiple files, introduces new rules, or has real design choices)

Do not skip straight to implementation. Write the spec first.

---

## Workflow

### 1. Read before asking

Before asking anything, read:
- `DESIGN.md` — hard rules and invariants; note any that the feature might conflict with
- Relevant source files for the area being changed
- Existing specs in `specs/` for conventions and numbering

### 2. Ask clarifying questions

Use the ask-questions tool. Typical questions for this codebase:

- Does this feature conflict with or supersede a hard rule in `DESIGN.md`? If so, confirm the amendment.
- Where do new constants live? (physics → `lib/physics.js`, game tuning → `game/`, infrastructure → `lib/`)
- What test framework / runner? (default: `node:test`, zero deps)
- Are expected test values derived from real data or synthetic? If real, confirm the source.

Do not ask about things already settled in `DESIGN.md`.

### 3. Write the spec

File: `specs/NN-short-description.md` (next available sequence number, zero-padded).

Follow the format in [`specs/README.md`](../../../specs/README.md).
Use `specs/01-solarsystem-support.md` as a worked example.

Target audience is AI assistants: keep prose tight, prefer tables and pseudocode.

### 4. Iterate

Present the spec to the user. Incorporate feedback. Do not start implementation until the
user approves the spec or explicitly says to proceed.

### 5. DESIGN.md

If the spec amends a hard rule, apply the change to `DESIGN.md` before or alongside
implementation. DESIGN.md is the single source of truth for invariants.

### 6. Implement from the spec

Follow the spec. Do not add scope during implementation. If something is missing,
update the spec first, then continue.

### 7. Tests

Write tests as specified. Expected values must be hardcoded — do not compute them with
the same formula under test. Add new test scripts to `package.json` following the existing
`test`, `test:unit`, `test:integration` pattern.

### 7. Discord sanity check

After implementing, run through the [Discord command sanity check](../../../DESIGN.md#discord-command-sanity-check) in DESIGN.md before closing the feature:

- Redeploy slash commands (`node src/deploy-commands.js`)
- Verify all options and choices appear correctly in Discord
- Walk the happy path, wrong-location path, and busy-ship path manually
- Let the processor resolve and confirm the notification embed is correct

Tests run against the CLI. Discord has extra constraints (required options, choice lists, deploy lag) that unit tests cannot catch.

---

## Layer rules (from DESIGN.md)

Always respect these when placing new code:

| Layer | Path | Rule |
|---|---|---|
| Commands | `commands/` | Discord interaction only. No SQL, no game logic. |
| Game logic | `game/` | Kuiper-specific: stats, modules, outcomes, prices. |
| Infrastructure | `lib/` | DB, orbital math, event processor. No game design decisions. |
| Constants | `lib/physics.js` | Physical constants, epoch dates, game start date. |

---

## Spec file reference

See [`specs/README.md`](../../../specs/README.md) for the full format guide and worked example.
