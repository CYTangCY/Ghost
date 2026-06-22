# CURRENT_TASK.md

## ID

M0-T23

## Goal

Create the static Act 3 node-graph UI prototype scene (display-only): a node-type palette, an (empty)
graph canvas region, a panel showing the level goal / test cases, and a placeholder Validate button +
feedback, built via an Editor menu scene builder and backed by a `DialogGraphSession`. No node
placement or connection interaction yet.

## Context

ROADMAP Phase C = Act 3 (flagship node graph). The Act 3 logic is done and Editor-verified: M0-T21 core
(`DialogGraph`/simulator/validator/sample data) and M0-T22 session (`DialogGraphSession`). Act 1/2 built
UI incrementally — static scene first, then interaction, then validation feedback — and that worked
well. This is the Act 3 equivalent of M0-T16: a static, display-only scene built through an Editor menu
builder (Codex batch mode cannot reliably generate `.unity`, so ship the builder + a manual run step).
Confirmed mechanic (Docs/LEARNING_CONTENT.md, Act 3): node assembly.

## Scope

- Add an Act 3 presentation layer under `Assets/Presentation/Act3DialogGraph/` (runtime scripts compile
  into the existing `Ghost.Presentation` assembly; a new Act 3 Editor asmdef for the scene builder is
  allowed, mirroring the Act 2 Editor asmdef).
- A presenter that, backed by a `DialogGraphSession` created from `Act3DialogGraphSampleData`, renders:
  - a **node-type palette** listing the available node types (Start, IntentBranch, SlotCheck, Response)
    and the level vocabulary (intents / entity types / response ids) for later placement;
  - an **(empty) graph canvas** region where nodes/edges will later be placed;
  - a **goal / test panel** summarising the level's target conversation(s) (the test cases: each
    turn's intent + entities and the expected response) so the player knows what to build;
  - a **placeholder Validate button + feedback text** (display only — wiring real validation is M0-T25).
- An Editor menu scene builder (in `Assets/Presentation/Act3DialogGraph/Editor/` with its own Editor
  asmdef) that generates `Assets/Scenes/Act3DialogGraphPrototype.unity` and creates a runtime
  `EventSystem` if needed. Do NOT add the scene to Build Settings in this task.
- Keep it display-only: no node placement, no edge drawing, no real validation; no scoring, save/load,
  backend, LLM, dialogue, or final art.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md; create a Codex run log.

## Out of Scope

- No node placement / connection interaction (M0-T24) or validation wiring (M0-T25).
- No Act 4–6 node types, node graph for other Acts, backend, LLM, save/load.
- Do not add the Act 3 scene to Build Settings, and do not change the Game Shell or its act list yet.
- Do not edit ProjectSettings, Packages, `.meta` files, existing asmdefs, the Act 3 pure logic
  (M0-T21/M0-T22), or Act 1 / Act 2 / Game Shell scripts. (Creating the one new Act 3 Editor asmdef for
  the scene builder is allowed.)

## Acceptance Criteria

- A new scene `Assets/Scenes/Act3DialogGraphPrototype.unity` can be generated via a `Ghost > …` Editor
  menu item.
- The scene shows a node-type palette (Start/IntentBranch/SlotCheck/Response + level vocabulary), an
  empty graph canvas region, a goal/test panel describing the target conversation(s), and a placeholder
  Validate button + feedback.
- The scene is display-only (no placement/connection/validation yet) and has no Console errors in Play
  Mode.
- The scene is NOT added to Build Settings; Act 1/2, the Game Shell, and the Act 3 pure logic are
  unchanged.
- CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md are updated, and a Codex run log is created.
