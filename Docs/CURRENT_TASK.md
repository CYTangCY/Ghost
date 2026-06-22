# CURRENT_TASK.md

## ID

M0-T16

## Goal

Create the static Act 2 span-annotation UI prototype scene (display-only): render one sample message
as word chips, an entity-type palette/legend, and placeholder Validate + feedback, built via an Editor
menu scene builder and wired to `Act2EntityExtractionSampleData` / `EntityExtractionSession`. No
span-selection interaction yet.

## Context

ROADMAP Phase B = Act 2 Entity Extraction. The Act 2 logic is done and Editor-verified: M0-T14 core
(`EntityType`, `EntitySpan`, `EntityExtractionValidator`, `Act2EntityExtractionSampleData`) and M0-T15
session (`EntityExtractionSession`). Act 1 was built UI-incrementally — static scene first (M0-T07),
then click (M0-T08), then editing/validation feedback (M0-T09) — and that worked well. This task is
the Act 2 equivalent of M0-T07: a static, display-only span-annotation scene built through an Editor
menu builder (Codex batch mode cannot reliably generate `.unity`, so ship the builder + a manual run
step, like Act 1). Confirmed mechanic (Docs/LEARNING_CONTENT.md, Act 2): span annotation with entity
typing, presented as word/phrase chip tagging.

## Scope

- Add an Act 2 presentation layer under `Assets/Presentation/Act2EntityExtraction/` (runtime scripts
  compile into the existing `Ghost.Presentation` assembly).
- A presenter that, from one `Act2EntityExtractionSampleData` sample message, renders:
  - the message as **word chips**, where each chip carries its character start index + length (so a
    later task can build `EntitySpan`s that exactly match the sample data — the current sample entities
    are single words);
  - an **entity-type palette / legend** showing the available types and their System/Custom category
    (e.g. `time` (System), `room` (Custom), `object` (Custom));
  - a **placeholder Validate button and feedback text** (display only — wiring real validation is a
    later task).
- An Editor menu scene builder (in a new `Assets/Presentation/Act2EntityExtraction/Editor/` folder
  with its own Editor asmdef, mirroring the Game Shell builder) that generates
  `Assets/Scenes/Act2EntityExtractionPrototype.unity` and creates a runtime `EventSystem` if needed.
  Do NOT add the scene to Build Settings in this task.
- Keep it display-only: no span selection, no type assignment, no real validation flow, no scoring,
  save/load, animation, backend, LLM, dialogue, or final art.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md.
- Create a Codex run log during implementation.

## Out of Scope

- Do not implement span-selection interaction, type assignment, or validation feedback wiring (later
  sub-tasks).
- Do not implement the node graph, Act 3+, backend, LLM, dialogue, or save/load.
- Do not add the Act 2 scene to Build Settings, and do not change the Game Shell or its act list yet.
- Do not edit ProjectSettings, Packages, `.meta` files, the existing asmdefs, the Act 2 pure logic
  (M0-T14/M0-T15), or any Act 1 / Game Shell script. (Creating the one new Act 2 Editor asmdef for the
  scene builder is allowed.)

## Acceptance Criteria

- A new scene `Assets/Scenes/Act2EntityExtractionPrototype.unity` can be generated via a `Ghost > …`
  Editor menu item.
- The scene shows a sample message rendered as word chips, an entity-type palette/legend with
  System/Custom categories, and a placeholder Validate button + feedback text.
- Each rendered word chip is associated with its character start index + length (verifiable in code,
  ready for a later interaction task).
- The scene is display-only (no span selection, assignment, or working validation yet) and has no
  Console errors in Play Mode.
- The scene is NOT added to Build Settings; Act 1, the Game Shell, and the Act 2 pure logic are
  unchanged.
- CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md are updated, and a Codex run log is created.
