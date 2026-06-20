# HANDOFF_LOG.md

## Purpose

This file records what was completed, tested, and decided after each task.

It keeps a short chronological summary. Fuller task records live in `Docs/completed_tasks/`.
Previous task files must not be deleted.

---

## Chronological Log

### 2026-06-17 — M0-T01: Initialize planning docs — Completed
Created and aligned the initial planning document set with the confirmed Ghost direction.
Archive: `Docs/completed_tasks/M0-T01_initialize_planning_docs.md`. Next: M0-T02.

### 2026-06-17 — M0-T02: Learning-content mapping (Acts 0–2) — Completed
Filled Act 0 (flow-diagram pipeline, Option A) and confirmed Act 1 (drag-and-drop classification)
and Act 2 (span annotation with entity typing), using the IBM SkillsBuild source for the five
components and four challenges. Also added the task archiving convention.
Archive: `Docs/completed_tasks/M0-T02_learning_content_mapping.md`. Next: M0-T03.

### 2026-06-17 — M0-T03: Unity repo inventory — Completed
Read-only inventory: clean Unity 6 Universal 2D / URP project, new Input System active,
SampleScene only, no scripts, no game scenes, Test Framework installed. No files modified.
Archive: `Docs/completed_tasks/M0-T03_repo_inventory.md`. Next: M0-T04.

### 2026-06-17 — Convention: Codex run log added
Every Codex implementation/debugging run now creates one log in `Docs/codex_runs/`
(see `Docs/codex_runs/README.md`); AGENTS.md and CLAUDE.md updated accordingly.

### 2026-06-19 — M0-T04: Act 1 intent-classification validator — Completed
Codex implemented the pure C# intent-classification validator + EditMode tests (runtime/test
asmdefs). The Codex run log honestly recorded Unity tests as "Not run" in-session; the user later
verified in the Unity Editor that the project compiled and the EditMode tests passed, then
committed and pushed. Archive: `Docs/completed_tasks/M0-T04_intent_classification_validator.md`.
Next: M0-T05.

### 2026-06-19 — M0-T05: Act 1 sample puzzle data — Completed
Codex added Act1IntentClassificationSampleData (3 intent groups, 9 differently-worded cards) +
EditMode tests; run 002 fixed a failing test by checking `groups.Count` directly instead of the
NUnit `Has.Count` constraint. Both run logs recorded Unity tests as "Not run" in-session; the user
later verified in the Editor that the project compiled and both EditMode suites passed, then
committed and pushed. Archive: `Docs/completed_tasks/M0-T05_act1_sample_puzzle_data.md`. Next: M0-T06.

### 2026-06-19 — M0-T06: Act 1 session/state layer — Completed
Codex added IntentClassificationSession (tracks unassigned + grouped cards, move/validate, exposes
submitted groups for the validator) + 10 EditMode tests. The run log recorded Unity tests as
"Not run" in-session; the user later verified in the Editor that the project compiled and all three
EditMode suites passed, then committed and pushed. Archive:
`Docs/completed_tasks/M0-T06_intent_classification_session_state.md`. Next: M0-T07.

### 2026-06-19 — M0-T07: Static Act 1 UI prototype scene — Completed
Codex (run 001) added a static UGUI presenter + an Editor menu scene builder (Codex batch mode could
not generate the scene, so it shipped the builder + a manual setup plan); the user ran
`Ghost > Build Act 1 Intent Classification Prototype Scene` to generate
`Assets/Scenes/Act1IntentClassificationPrototype.unity`. Run 002 fixed blank-looking cards (compact
card template, explicit font/contrast, layout rebuild). Run logs recorded Play Mode/tests as "Not
run" in-session; the user later verified in the Editor that the project compiled, the scene showed
9 cards + 3 intent areas (find_item/ask_location/ask_identity), and Play Mode had no Console errors
(no interaction, as expected). Scene not added to Build Settings.
Archive: `Docs/completed_tasks/M0-T07_static_act1_ui_scene.md`. Next: M0-T08.

### 2026-06-19 — M0-T08: Click-to-assign interaction — Completed
Codex extended the Act 1 presenter + scene builder for click-to-select / click-to-assign via the
existing IntentClassificationSession, and added an EventSystem with InputSystemUIInputModule. Run
002 added deselect (click selected card again), auto-clear selection after assignment, and
RectMask2D clipping + compact rows so assigned text stays inside the group panels. Run logs recorded
Play Mode/tests as "Not run" in-session; the user later verified in the Editor that selection,
deselection, assignment, selection-clearing, no text overflow, and Console-clean Play Mode all work
(no drag-and-drop). Known issue deferred to M0-T09: assigning many cards to one group can hide some
(clipping without scrolling/overflow indicator).
Archive: `Docs/completed_tasks/M0-T08_click_to_assign_interaction.md`. Next: M0-T09.

### 2026-06-19 — M0-T09: Assignment editing + group capacity + validation feedback — Completed
Codex added clickable "Back:" rows to unassign cards (MoveCardToUnassigned), kept reassignment via
MoveCardToGroup, replaced the clipped group area with a per-group vertical ScrollRect (many assigned
cards stay inspectable), and added a Validate button + feedback text wired to
IntentClassificationSession.ValidateCurrentState(). Run log recorded Play Mode/tests as "Not run"
in-session; the user later verified in the Editor that compile, click flow, remove/correct,
no-card-loss scrolling, correct/incorrect feedback, and Console-clean Play Mode all work (no
drag-and-drop). Archive:
`Docs/completed_tasks/M0-T09_assignment_editing_validation_feedback.md`. Next: M0-T10 (architecture review).

### 2026-06-19 — M0-T10: Act 1 UI/code architecture review — Completed (review)
Claude reviewed the Act 1 prototype before drag-and-drop. The pure logic layer is clean and
compiler-separated (Ghost.Runtime.asmdef `noEngineReferences`); validator/session/sample-data/card
have single responsibilities. Main risk is the presentation layer:
Act1IntentClassificationStaticPresenter (~824 lines) mixes rendering, session ownership, interaction,
visual state, assignment editing, and validation feedback; presenter/builder duplicate UI
construction; the scene builder injects fields via reflection and regeneration overwrites manual
edits. Recommendation: small behaviour-preserving presentation refactor before drag-and-drop. No code
changed (review only).
Archive: `Docs/completed_tasks/M0-T10_act1_ui_code_architecture_review.md`. Next: M0-T11 (refactor).

### 2026-06-19 — M0-T11: Act 1 presentation refactor — Completed
Codex added a presentation assembly boundary (Ghost.Presentation.asmdef + Ghost.Presentation.Editor.asmdef)
and extracted session/interaction orchestration out of the presenter into a new
Act1IntentClassificationInteractionController (owns session, selected card, select/deselect, assign,
unassign, validate, state/feedback callbacks); the presenter now renders from controller state and
forwards clicks. Scene builder unchanged (presenter creates the controller internally). Run log
recorded Play Mode/tests as "Not run" in-session; the user later verified in the Editor that the
project compiled, EditMode tests passed, and all M0-T09 behaviour still works with no Console errors
(no drag-and-drop, no pure-logic/test changes). Deferred from M0-T10 and still open: presenter/builder
UI-construction duplication (R2) and reflection field injection (R4). Archive:
`Docs/completed_tasks/M0-T11_presentation_refactor.md`. Next: M0-T12 (drag-to-assign).

### 2026-06-20 — M0-T12: Drag-to-assign interaction — Completed
Codex added drag-to-assign across three runs: 001 = minimal drag (Act1IntentClassificationDraggableCard
+ Act1IntentClassificationDropTarget, new controller AssignCardToIntent); 002 = bidirectional drag
(assigned rows draggable back to unassigned or to another group, group-wide drop zones, compact rows);
003 = drag visual cleanup (single active preview, reliable cleanup on success/fail/source-destroy,
compact chip rows). All assign/unassign/reassign route through Act1IntentClassificationInteractionController
+ IntentClassificationSession; pure logic untouched. Run logs recorded Play Mode/tests as "Not run"
in-session; the user later verified in the Editor that compile, click-to-assign, drag left→group,
group-based drop, drag→unassign, drag→reassign, drop-outside safety, Back, Validate/feedback, and no
ghost artifacts all work. Deferred cleanups (R2 duplication, R4 reflection) still open.
Archive: `Docs/completed_tasks/M0-T12_drag_to_assign_interaction.md`. Next: M0-T13 (visual/instruction polish).

---

## Entry Template

### Date

TBD

### Task ID

TBD

### Completed

- TBD

### Changed Files

- TBD

### Unity Test Result

Not applicable / Pass / Fail

### Issues Found

- TBD

### Decisions Made

- TBD

### Next Task

TBD
