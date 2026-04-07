# Specs

Feature specs live here. They are written before implementation, not after.

Files are named `NN-short-description.md` where `NN` is a zero-padded sequence number.

---

## Why specs first

A spec is a contract between the designer (you) and the implementer (often an AI assistant).
Writing it first forces the scope conversation to happen before code exists and is easy to
change. It also gives future AI sessions enough context to implement correctly without
re-asking all the same questions.

---

## Workflow

1. **Ask first.** Before writing the spec, the AI should ask clarifying questions —
   framework choices, where constants live, whether existing rules need amending, etc.
   Ambiguity resolved here is cheaper than ambiguity resolved in code review.

2. **Write the spec.** Using the format below. Target audience is AI assistants, so keep
   prose tight and prefer tables and pseudocode over paragraphs.

3. **Iterate.** Treat the spec as a draft. The user reviews it and feeds back corrections
   before implementation starts.

4. **DESIGN.md is part of the feature.** If the feature amends or supersedes a hard rule,
   that change is in the spec and applied to `DESIGN.md` as part of the same PR.
   DESIGN.md is the source of truth for invariants — keep it current.

5. **Implement from the spec.** The spec is the plan. Don't invent scope during
   implementation. If something is missing, pause and update the spec first.

6. **Tests are in the spec.** Test cases (inputs, expected outputs, tolerances) are defined
   in the spec, not discovered during implementation.

---

## Spec format

```markdown
# NN — Feature Name

## Goal
One paragraph. What problem does this solve and why now.

## What this is, and what it is not
**In scope** — bullet list
**Out of scope — do not add** — bullet list
Short prose clarifying intentional constraints.

## DESIGN.md amendment  (omit if no hard rules change)
Quote the rule being superseded, then write the replacement.

## File changes
One subsection per file. For new files: full intended exports and constants.
For changed files: what is added/changed/removed, with pseudocode for non-trivial logic.

## Test cases
For each test: inputs, expected output, tolerance, and a one-line rationale.
Hardcode expected values — do not derive them from the same formula under test.
```

---

## What does not go in a spec

- Implementation details the code can decide (variable names, loop style)
- Anything that is already captured in `DESIGN.md` and not being changed
- Open questions — resolve them before finalising the spec, or explicitly mark them
  as decisions to be made and follow up before implementation starts
