# CURRENT_TASK.md

## ID

M0-T17

## Goal

Add chip selection + entity-type assignment interaction to the Act 2 span-annotation scene: the player
selects a word chip and clicks an entity type to tag it, building an `EntitySpan` in an
`EntityExtractionSession`; tagged chips show their assigned type and can be untagged. No validation
feedback wiring yet.

## Context

ROADMAP Phase B = Act 2 Entity Extraction. M0-T16 added the display-only scene (word chips with stored
character offsets via `Act2EntityChipView`, an entity-type legend, and a placeholder Validate button).
Act 1 added interaction incrementally — click-to-assign (M0-T08) then validation feedback (M0-T09).
This task is the Act 2 equivalent of M0-T08: wire chip + type interaction through the
`EntityExtractionSession` (M0-T15). Confirmed mechanic (Docs/LEARNING_CONTENT.md, Act 2): span
annotation with entity typing (chip tagging).

## Scope

- Add an Act 2 interaction controller in the Act 2 presentation folder (preferred — mirror Act 1's
  `Act1IntentClassificationInteractionController`), and extend `Act2EntityExtractionStaticPresenter` to
  use it, so that:
  - clicking a word chip selects / deselects it (visible highlight);
  - with a chip selected, clicking an entity type tags that chip: build
    `EntitySpan(chip.Start, chip.Length, selectedType)` and add it to an `EntityExtractionSession`
    created from the sample message;
  - a tagged chip visibly shows its assigned entity type (e.g. colour by System/Custom + a small type
    label) and can be untagged (removing the span from the session);
  - all add/remove route through `EntityExtractionSession` (no re-implemented matching — correctness
    stays with the validator, wired in the next task).
- Keep the M0-T16 chips and entity-type palette; make chips raycast-targetable for clicks; ensure an
  `EventSystem` exists.
- Regenerate the scene via the existing menu builder if needed, but do NOT add it to Build Settings.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md; create a Codex run log.

## Out of Scope

- Do not wire the Validate button or show correct/incorrect feedback yet (that is M0-T18). The Validate
  button stays a placeholder.
- Do not implement multi-chip / phrase spans (single-chip spans only for now), the node graph, Act 3+,
  backend, LLM, save/load, scoring, or final art.
- Do not add the scene to Build Settings; do not change the Game Shell or its act list.
- Do not edit ProjectSettings, Packages, `.meta` files, existing non-Act-2 asmdefs, the Act 2 pure
  logic (M0-T14/M0-T15), or Act 1 / Game Shell scripts.

## Acceptance Criteria

- Clicking a word chip selects it (visible highlight); clicking again or selecting another chip updates
  the selection.
- With a chip selected, clicking an entity type tags the chip with that type and adds an
  `EntitySpan(start, length, type)` to the session; the chip visibly shows its assigned type.
- A tagged chip can be untagged, removing the span from the session.
- All assignment / removal route through `EntityExtractionSession`; the Act 2 pure logic is unchanged.
- The Validate button remains a placeholder (no feedback yet); the scene is not in Build Settings;
  Act 1 and the Game Shell are unchanged.
- No Console errors in Play Mode; CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md are updated; a Codex
  run log is created.
