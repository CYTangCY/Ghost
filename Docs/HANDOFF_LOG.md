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

### 2026-06-20 — Milestone & roadmap revision (planning, no code)
Act 1 core gameplay is a completed playable milestone (click + drag assignment, bidirectional
reassign, Back/unassign, Validate, feedback). Roadmap revised after Act 1: created `Docs/ROADMAP.md`;
revised Act structure to Act 1 Intent → Act 2 Entity → Act 3 Dialog Node Graph (flagship) →
Acts 4–6 graph extensions → Act 7 NLP Lab → Act 8 Capstone "Repair Ghost's Voice". Act 1 polish is
deferred. **CURRENT_TASK M0-T13 changed from Act 1 polish to Game Shell + Lily + Act Select.** The
former Act 0 fundamentals move to the Game Shell intro (Lily) + the Act 8 capstone. Updated
LEARNING_CONTENT, DESIGN_RATIONALE, ARCHITECTURE, AGENTS, CLAUDE; recorded the AI-collaboration
workflow (ChatGPT plans, Claude reviews/docs/closure, Codex implements only CURRENT_TASK + run logs).
Open item: `Docs/CONFIRMED_PROJECT_CONTEXT.md` §5 still shows the earlier Act structure and should be
reconciled. Next: M0-T13 (Game Shell prototype).

### 2026-06-20 — M0-T13: Game Shell prototype — Completed
Codex run 002 added the Game Shell (title, act-select/hub, reusable Lily dialogue frame with text from
ShellDialogueData, Ghost placeholder, SceneManager shell↔Act1 navigation, runtime Return-to-Hub
overlay, menu builder + shell Editor asmdef). Codex batch mode could not generate the scene, so the
user ran `Ghost > Build Game Shell Scene`: `GameShellPrototype.unity` now exists and both it and
`Act1IntentClassificationPrototype.unity` are in Build Settings (the approved exception). Run logs
recorded Play Mode/tests as "Not run" in-session; the user verified in the Editor that the shell,
shell↔Act1 navigation, Return to Hub, and all Act 1 mechanics work with no Console errors ("全部完成了").
Note: M0-T13 has two run logs under one ID — 001 = superseded Act-1 visual/instruction polish (earlier
definition, also landed in the tree), 002 = this Game Shell. `CONFIRMED_PROJECT_CONTEXT.md` §5 was also
reconciled to the 8-Act structure this round (resolving the open item above). Archive:
`Docs/completed_tasks/M0-T13_game_shell_prototype.md`. Next: M0-T14 (Act 2 entity-extraction logic).

### 2026-06-22 — Full-system direction sync (planning, no code)
Synced the planning/workflow docs to the revised full-system direction: LLM, backend, and database
are now recorded as **required** final-system components (previously framed as deferrable). Added the
deterministic-correctness rule — validators / graph simulation / test cases / backend scoring decide
puzzle correctness; the LLM may hint/explain/generate language but never decides scoring. ROADMAP
restructured: inserted **Phase D — Full-System Foundation** (backend API, DB schema, progress,
content, attempt logs, LLM orchestration); graph extensions → Phase E; NLP Lab + Capstone → Phase F;
status refreshed (Phase A Game Shell done; **M0-T14 Act 2 is the active task**). ARCHITECTURE gained
backend/database/LLM layers + the deterministic rule; LEARNING_CONTENT gained per-Act backend/DB/LLM
interaction notes; DESIGN_RATIONALE gained the "why required / why LLM is not the correctness source /
why skeleton first" rationale; REQUIREMENTS gained FR7/FR8/NFR5; CONFIRMED_PROJECT_CONTEXT §8/§12/§14
reconciled (LLM now required; §14 item 13 resolved); AGENTS/CLAUDE gained the "plan full-system in
ROADMAP/ARCHITECTURE before implementing" rule. **CURRENT_TASK.md left unchanged at M0-T14** — the
original prompt's "revert M0-T13 to Game Shell" was based on a stale repo state (M0-T13 is already
done/archived). Docs only; no Unity/Assets/ProjectSettings/Packages changes. Next: M0-T14
(implementation, by Codex).

### 2026-06-22 — Workflow transition: Claude becomes commander/reviewer (planning, no code)
Established the two-agent workflow in `Docs/AI_COLLABORATION_PROTOCOL.md` (new canonical doc): Claude
is now the repo-aware project commander and reviewer (plans, writes Codex prompts, reviews, archives,
advances CURRENT_TASK after verification); Codex remains the implementation agent (active task only,
run logs, and now returns a Claude review/closure prompt). ChatGPT is removed from the official
workflow (ad-hoc strategy only). Repo docs — not chat memory — are the source of truth, with the user
as final decision maker and manual Unity verifier. Both agents must include a Chinese STAR summary
(S情境 / T任務 / A行動 / R結果). Recorded git-hygiene rules (check `git status --short` +
`git diff --name-only` before commits; do not commit ProjectSettings side effects / unrelated scene
rewrites). Updated AGENTS.md and CLAUDE.md to match. Docs only; no Unity/Assets/ProjectSettings/
Packages changes. Next: M0-T14 (Act 2 entity-extraction logic, by Codex).

### 2026-06-22 — M0-T14: Act 2 entity-extraction core — Completed
Codex run 001 added the Act 2 logic core (pure C#, scene-free) in `Ghost.Runtime`: `EntityType`
(System/Custom), `EntitySpan` (start+length+type), a deterministic `EntityExtractionValidator`
(missing / wrong-type / wrong-boundary / extra / duplicate / null errors), and
`Act2EntityExtractionSampleData` (system `time` + custom `room`/`object` + `lab`/`laboratory` synonym
pair), plus EditMode tests for the validator and the sample data. The run log honestly recorded Unity
tests as "Not run" in-session; the user then ran the EditMode tests in the Editor and reported all
passing ("測試都通過"). Claude reviewed scope, deterministic correctness, learning coverage,
docs, and the run log (one low-severity note: same-type spans in one message could misattribute an
error message; `IsCorrect` unaffected). No asmdef / ProjectSettings / Packages / scenes / `.meta` /
Act 1 / Shell / CURRENT_TASK changes. Archive:
`Docs/completed_tasks/M0-T14_act2_entity_extraction_core.md`. Next: M0-T15 (Act 2 session/state layer).

### 2026-06-22 — M0-T15: Act 2 entity-extraction session/state — Completed
Codex run 001 added `EntityExtractionSession` (pure C#, `Ghost.Runtime`): tracks message text +
expected spans + distinct current player spans, with AddSpan/RemoveSpan/CurrentSpans/
ValidateCurrentState delegating correctness to `EntityExtractionValidator`; + 6 EditMode tests. Run
002 fixed a failing test by asserting `session.CurrentSpans.Count` directly instead of NUnit
`Has.Count` (same gotcha as M0-T05 run 002). Run logs honestly recorded Unity tests "Not run"
in-session; the user ran the EditMode suite in the Editor and reported success ("測試成功"). Claude
reviewed scope (clean), validator delegation, tests, docs, and run logs (minor cosmetic doc nit: a
duplicated "Act 2 Entity Extraction EditMode Tests" header in CODE_WALKTHROUGH). No asmdef /
ProjectSettings / Packages / scenes / CURRENT_TASK changes. Archive:
`Docs/completed_tasks/M0-T15_act2_entity_extraction_session.md`. Next: M0-T16 (Act 2 static
span-annotation UI).

### 2026-06-22 — M0-T16: Act 2 static span-annotation UI — Completed
Codex run 001 added the display-only Act 2 UI: `Act2EntityExtractionStaticPresenter` +
`Act2EntityChipView` (each chip stores Start/Length/Text; the word tokenizer trims surrounding
punctuation so sample entities align to one chip) + an Editor scene builder (menu `Ghost > Build Act 2
Entity Extraction Prototype Scene`) + a new Act 2 Editor asmdef. The user ran the builder, generating
`Assets/Scenes/Act2EntityExtractionPrototype.unity` (NOT added to Build Settings), and verified the
rendered scene (title, word chips, entity-type legend with System/Custom, placeholder Validate +
feedback, no interaction) via screenshot. Run log honest ("Not run" for scene-gen/Play Mode/tests in
the Codex session). Claude reviewed scope (clean), display-only behaviour, and chip-offset readiness.
Archive: `Docs/completed_tasks/M0-T16_act2_static_span_annotation_ui.md`. Next: M0-T17 (Act 2 chip
selection + type assignment interaction).

### 2026-06-22 — M0-T17: Act 2 chip selection + type assignment — Completed
Codex run 001 added `Act2EntityExtractionInteractionController` (owns one `EntityExtractionSession` +
selected chip + chipKey→type map; select / assign / untag route through `AddSpan`/`RemoveSpan`;
`StateChanged` event; no validation) and wired the presenter: chips + palette clickable, selected
highlight + System/Custom type badge, Validate left disabled/unwired. The user regenerated
`Act2EntityExtractionPrototype.unity` via the builder and verified in the Editor ("功能測試正確").
Run log honest ("Not run" for Unity in-session). Claude reviewed scope (clean — no Act 2 pure logic /
Act 1 / Shell / asmdef / ProjectSettings / Build Settings edits), session routing, and the disabled
Validate. Archive: `Docs/completed_tasks/M0-T17_act2_chip_selection_assignment.md`. Next: M0-T18 (Act 2
validation feedback — wire the validator into the UI).

### 2026-06-22 — M0-T18: Act 2 validation feedback — Completed
Codex run 001 wired the Act 2 Validate button to the deterministic validator: the controller gained
`ValidateCurrentState()` (calls `EntityExtractionSession.ValidateCurrentState()`, builds feedback from
`IsCorrect`/`Errors.Count`, raises `FeedbackChanged`), and the presenter enabled Validate, routed the
click through the controller, and renders green/red feedback. Correctness stays deterministic (no LLM).
The user regenerated the Act 2 scene and verified correct/incorrect feedback in the Editor ("完成").
Run log honest ("Not run" for Unity in-session). Claude reviewed scope (clean) + deterministic
correctness. Archive: `Docs/completed_tasks/M0-T18_act2_validation_feedback.md`. Next: M0-T19 (integrate
Act 2 into the Game Shell). With M0-T18, Act 2's core puzzle loop (core → session → static UI →
selection/assignment → validation) is feature-complete; M0-T19 makes it reachable from the shell.

### 2026-06-22 — M0-T19: Act 2 Game Shell integration — Completed (Act 2 / Phase B done)
Codex run 001 wired Act 2 into the shell: `ShellSceneNames` Act2 name/path; `GameShellPresenter`
`act2Button` + `StartAct2()`; `ShellReturnToHubOverlay` now Act 1 or Act 2; `ShellDialogueData` hub
line mentions both; `GameShellSceneBuilder` (refactored `CreateActCard`) builds the Act 2 hub card and
registers shell + Act 1 + Act 2 in Build Settings. The user ran `Ghost > Build Game Shell Scene` and
verified the full flow ("完成"). `EditorBuildSettings.asset` diff adds only the Act 2 entry (approved
exception). Run log honest ("Not run" for Unity in-session). Claude reviewed scope (clean — no Act 2
puzzle logic/UI or Act 1 mechanics changed). **This completes ROADMAP Phase B (Act 2): core → session
→ static UI → selection/assignment → validation → shell integration.** Archive:
`Docs/completed_tasks/M0-T19_act2_shell_integration.md`. Next: M0-T20 (Act 3 node-graph design +
learning-content mapping, Phase C).

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
