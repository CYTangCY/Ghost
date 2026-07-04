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

### 2026-06-22 — M0-T20: Act 3 node-graph design + learning-content mapping — Completed (planning)
Claude-led design (docs only). Completed the Act 3 section of LEARNING_CONTENT (objective, cute Ghost
problem, node-assembly mechanic, player action, success/failure, Lily hint, connection to Acts 1–2,
deterministic correctness), and added to ARCHITECTURE a minimal node-graph model
(DialogNode/DialogTransition/DialogGraph; Start/IntentBranch/SlotCheck/Response; ConversationTurn +
DialogContext), the DialogGraphSimulator/DialogGraphValidator design, and the M0-T21…M0-T26 build
slices. Intent = branch triggers, entity = slots, context remembers; correctness deterministic (LLM
never scores). The user confirmed the design ("確認"). No Unity code. Archive:
`Docs/completed_tasks/M0-T20_act3_node_graph_design.md`. Next: M0-T21 (Act 3 core logic).

### 2026-06-22 — M0-T21: Act 3 node-graph core — Completed
Codex run 001 added the pure C# Act 3 core in `Ghost.Runtime` (`Ghost.Puzzles.DialogGraph`):
`DialogNode`/`DialogTransition`/`DialogGraph` + `DialogNodeType` (Start/IntentBranch/SlotCheck/Response),
`ConversationTurn`/`DialogContext`, a step-capped deterministic `DialogGraphSimulator`, a
`DialogGraphValidator` (test-case + structural checks: start, endpoints, reachability, dead ends,
intents handled), `Act3DialogGraphSampleData` (one level: slot-present answer + slot-missing ask), and
EditMode tests. Intent routing realized as Start → per-intent IntentBranch (Always) matched by the
branch's IntentId. Run log honest (dotnet build passed; Unity tests "Not run" in-session). The user ran
the EditMode tests in the Editor and reported all passing ("全部都正確並測試成功"). Claude reviewed the
simulator/validator/sample data + scope (clean). Archive:
`Docs/completed_tasks/M0-T21_act3_node_graph_core.md`. Next: M0-T22 (Act 3 graph session/state).

### 2026-06-22 — M0-T22: Act 3 graph session/state — Completed
Codex run 001 added `DialogGraphSession` (pure C#, `Ghost.Runtime`): mutable building state (nodes /
transitions / start id) + level test cases; `AddNode` (generated unique ids), `RemoveNode` (cascades to
transitions, clears start), `SetStartNode`/`AddTransition` (reject unknown ids), `RemoveTransition`,
snapshots, and `ValidateCurrentState()` that returns an incorrect result for incomplete graphs WITHOUT
throwing (and otherwise delegates to `DialogGraphValidator`). M0-T21 types unchanged. Run log honest
(dotnet build passed; Unity tests "Not run" in-session); the user ran the EditMode tests in the Editor
and reported success ("測試成功"). Claude reviewed the incomplete-graph safety, cascade behaviour, and
scope (clean). Archive: `Docs/completed_tasks/M0-T22_act3_graph_session.md`. Next: M0-T23 (Act 3 static
node-graph UI scene).

### 2026-06-22 — M0-T23: Act 3 static node-graph UI — Completed
Codex runs 001–003 (001 initial; 002 layout fix; 003 ScrollRect→compact static lists so palette/goal
content stays visible) added the display-only Act 3 scene: `Act3DialogGraphStaticPresenter` (backed by
`DialogGraphSession.CreateFromSampleData()`) renders a node-type palette + level vocabulary, an empty
graph canvas, a Goal/Tests panel, and a disabled placeholder Validate; plus an Editor scene builder
(menu `Ghost > Build Act 3 Dialog Graph Prototype Scene`) + a new Act 3 Editor asmdef. The user
generated the scene and verified rendering (screenshot). Run logs honest ("Not run" for Unity
in-session); all three retained. Claude reviewed scope (clean — no Build Settings, no Act 3 logic /
Act 1 / Act 2 / Shell edits). Archive: `Docs/completed_tasks/M0-T23_act3_static_node_graph_ui.md`.
Next: M0-T24 (Act 3 node placement + connection interaction).

### 2026-06-22 — M0-T24: Act 3 node placement + connection — Completed (functional; UX debt) + direction pivot
Codex run 001 added `Act3DialogGraphInteractionController` (wraps `DialogGraphSession`) and made the
presenter interactive: place configured palette nodes, select a node card, set start, connect via
From/To + condition buttons, transitions list with remove, remove node — all routed through
`DialogGraphSession`; Validate left as a placeholder. The user confirmed it works (screenshot) but
flagged the design: not fun, objective unclear, connection UX fiddly and allowing self-loops
(`slotcheck_2 → slotcheck_2`). Closed as a functional prototype with an **Act 3 node-graph UX redesign
deferred**. **Direction pivot (user-set):** Acts 1–3 prototypes are in place; the next milestone is a
vertical slice / cohesive build — full-system foundation (backend/DB/LLM, Phase D) + narrative
integration into Acts 1–3 + the Act 3 UX redesign — to reach a certain completeness before resuming
Acts 4–8 (recorded in ROADMAP Current Status). Archive:
`Docs/completed_tasks/M0-T24_act3_node_placement_connection.md`. Next: pivot task (sequencing being
decided with the user).

### 2026-06-22 — M0-T25: Vertical-slice blueprint — Completed (planning)
Claude-led blueprint (`Docs/VERTICAL_SLICE_PLAN.md`): narrative frame for Acts 1–3, full-system
architecture + data contracts (Node/TS REST + SQLite + LLM; deterministic correctness preserved), Act 3
node-graph UX redesign direction, and a task breakdown. User decisions: LLM static-hints-first (LLM
deferred to M0-T29); start execution with the Act 3 UX redesign (M0-T30). Narrative
setting/protagonist/title + backend host remain open (settle at M0-T26). Archive:
`Docs/completed_tasks/M0-T25_vertical_slice_blueprint.md`. Next: M0-T30 (Act 3 node-graph UX redesign +
wire Validate to the validator).

### 2026-06-23 — M0-T30: Act 3 node-graph UX redesign — Completed
Codex runs 001–007 redesigned the Act 3 interaction: drag-a-wire port connecting (new input/output port
views + node drag), a clear in-story objective, an X/trash drop zone to delete nodes, stable column
layout (run 007 fixed body `HorizontalLayoutGroup.childForceExpandWidth` stretching fixed columns; trash
drop now keyed on cached highlight state), and wired the Validate button to
`DialogGraphSession.ValidateCurrentState()` (deterministic, no LLM). M0-T21 core + M0-T22 session
unchanged. Run logs honest ("Not run" for Unity in-session); all seven retained. The user verified
across iterations ("經過多版本修復現在完成了"). Claude reviewed scope (clean — no Act 3 pure logic /
Act 1/2 / Shell / ProjectSettings edits) and the deterministic Validate wiring. Archive:
`Docs/completed_tasks/M0-T30_act3_node_graph_ux_redesign.md`. Next: M0-T31 (Act 3 Game Shell
integration).

### 2026-06-23 — M0-T31: Act 3 Game Shell integration — Completed (Acts 1–3 all shell-linked)
Codex runs 001 + 002 (002 = Return-to-Hub overlay canvas fix) wired Act 3 into the shell:
`ShellSceneNames` Act3 name/path; `GameShellPresenter` `act3Button` + `StartAct3()`;
`ShellReturnToHubOverlay` now Act 1/2/3; `GameShellSceneBuilder` Act 3 hub card + Build Settings
(shell + Act 1 + Act 2 + Act 3); hub Lily line acknowledges Act 3. The `EditorBuildSettings` diff adds
only the Act 3 entry (approved exception). Act 3 puzzle + Acts 1/2 unchanged. The user ran the builder
and verified hub→Act 3 + return ("完成"). Run logs honest. Claude reviewed scope (clean). All three
prototype acts are now launchable from the hub. Archive:
`Docs/completed_tasks/M0-T31_act3_shell_integration.md`. Next: M0-T26 (narrative integration into
Acts 1–3).

### 2026-06-24 — M0-T26: Narrative integration into Acts 1–3 — Completed
Codex runs 001 (narrative) + 002 (hub layout fix). Added `GhostNarrativeState` (in-memory player name +
acts done across scene loads), a name-entry step, act-aware `ShellDialogueData` (intro/debrief + speaker
+ `{playerName}`), a `LilyDialogueFrame` portrait frame (empty Sprite placeholders), the shell narrative
flow (title → name → hub greeting → act intro → puzzle → debrief → Act 3 Ghost closing), and
return-overlay pending-debrief; run 002 put the three hub act cards in one row so Lily's box stays in
the viewport. Puzzles unchanged. Run logs honest; the user verified ("功能完成"). Claude reviewed scope
(clean; the Act 2 scene showing modified is an unrelated Editor side-effect, left unstaged). Archive:
`Docs/completed_tasks/M0-T26_narrative_integration.md`. Next: M0-T32 (in-act ambient Ghost+Lily banter).

### 2026-06-24 — M0-T32: In-act ambient Ghost+Lily banter — Completed
Codex runs 001–003 added a runtime ambient banter area per act: `AmbientBanterHook` spawns
`AmbientBanterPanel` from per-act `BanterData`; non-blocking cycling/looping lines that address the
player by name, Lily warming Act 1→Act 3 with a nerdy-joke-then-embarrassed beat, Ghost story-consistent;
run 003 fixed per-act text-box sizing. Frontend/static; puzzles unchanged. Run logs honest ("Not run"
Unity); all 3 retained. User accepted ("先暫時這樣，繼續下一步"). Claude reviewed scope (clean — only
Banter + docs). Archive: `Docs/completed_tasks/M0-T32_in_act_ambient_banter.md`. Next: M0-T27 (backend +
database foundation; full-system Phase D).

### 2026-06-24 — M0-T27: Backend + database foundation — Completed (full-system Phase D begins)
New `Backend/` Node+TS+Express+better-sqlite3 REST service + SQLite. Endpoints: `/health`, `GET /content`,
`POST /profiles`, `GET/PUT /progress/:id`, `POST /attempts`; `/hints`+`/responses` 501 (M0-T29). Schema
(learning_content/puzzles/profiles/progress/attempts/hint_logs) seeded with Act 1–3 reference content.
Backend stores/serves only — `GET /content` omits `answer_key_json`; no scoring endpoint (deterministic
correctness stays client-side). Codex ran npm; **Claude independently ran `npm run build` + `npm test`
(4/4 pass)** and reviewed routes/schema (scope clean: Backend/ + 2 docs; no Unity changes). Run log
honest (dev-tooling audit advisories noted; production audit clean). Archive:
`Docs/completed_tasks/M0-T27_backend_db_foundation.md`. Next: M0-T28 (Unity client↔backend integration,
graceful degradation).

### 2026-06-24 — M0-T28: Unity ↔ backend integration — Completed
Unity client ↔ backend: `GhostBackendClient` (UnityWebRequest coroutines, best-effort + timeouts),
`BackendSync` (profile + load/save progress to `GhostNarrativeState`), profile id in PlayerPrefs,
best-effort attempt logging from all three act validation paths, graceful degradation (offline →
in-memory fallback, warning-only). Backend `app.ts` gained dev CORS (+ OPTIONS 204). Correctness stays
client-side. Claude confirmed each feature's wiring + ran the backend build/test (4/4 after CORS). The
user verified in the Editor (backend on: profile/progress persist across Play sessions + attempts
logged; backend off: full play, no hangs) — "過了". Scope clean (Backend client + shell/controllers +
app.ts CORS + docs). Archive: `Docs/completed_tasks/M0-T28_client_backend_integration.md`. Next: M0-T29
(LLM orchestration; static-hint fallback; LLM never scores).

### 2026-06-25 — M0-T29: LLM orchestration — Completed
Runs 001 (Lily hints / Ghost responses via Ollama + Granite, static fallback, hint_logs, no scoring /
no answer-key) + 002 (fixed the 5s→60s Ollama timeout that caused always-static, added a fallback
warning + a timed `check:ollama`, Ask-Lily-replace-in-panel, and `trigger` logging). The user confirmed
live: `hint_logs` now shows `source:"llm"`, `trigger:"ask_lily_button"`, `error:null` (after killing a
stale `:3000` backend and raising the timeout). LLM never scores/gates; deterministic validators
unchanged. Claude reviewed the orchestration (no answer-key, no scoring) and relied on the live
evidence. Archive: `Docs/completed_tasks/M0-T29_llm_orchestration.md`. Next: M0-T33 (Lily chat:
free-text input, dedicated chat window, one-sentence in-character replies, topic guardrails; + banter UI
sizing — supersedes the run-002 same-panel hint UX).

### 2026-06-25 — IBM course CONTENT COVERAGE direction (goal corrected; planning, no code)
Reviewed Codex's `Docs/IBM_COURSE_ALIGNMENT_REVIEW.md` (Claude could not re-render the image-based PDF
in-session — no pdftoppm — so corroborated its concept list against `CONFIRMED_PROJECT_CONTEXT.md` §4/§5;
consistent). **Goal corrected by the user:** the requirement is that **the GAME teaches the IBM course's
content** (players learn the course curriculum by playing) — NOT the dissertation/architecture
"mirroring" the course, and NOT concept labels or dissertation wording. So the work is pedagogical
CONTENT COVERAGE, kept playable per §2. The review's "alignment = labels/framing" angle missed this and
is superseded. Plan (after the in-flight M0-T33): (1) a curriculum coverage map from the actual PDF
(course teaching point → where the game teaches it → gaps); (2) build the missing in-game teaching —
fundamentals first (chatbot definition, rule vs AI, five components, four challenges), strengthen Acts
1–3 to actually teach their concept, then cover the rest (Acts 4–7); Acts 4–8 are coverage, not extras.
Recorded in ROADMAP ("IBM Course Content Coverage"). CURRENT_TASK left as M0-T33. Next: finish M0-T33,
then the coverage map.

### 2026-06-25 — M0-T33: Lily chat — Completed
Codex run 001 added a dedicated `LilyChatWindow` (separate overlay, free-text input + Send + Close +
scrollable ≤10-turn history) that opens on Ask Lily and pauses/resumes the ambient banter; backend
`POST /chat` sends the player's message + history + a Lily persona/guardrail system prompt to Ollama,
returns one short in-character sentence, logs `kind:"chat"` (+ trigger + the player's message), static
fallback. Persona stammers + addresses {playerName}; guardrails: act/story only, private-life→flustered
deflect, off-topic→refocus, never reveals answers, never scores. Claude verified LIVE: `POST /chat`
returned `source:"llm"` ("Um... I, I don't have a favorite food, Alex. My focus is on intent
classification…") — stammer + name + off-topic deflection working; `hint_logs` now records the actual
question (fixes the prior gap); `npm test` 7/7; build clean. Unity window reviewed by reading
(pause/resume; separate canvas). Residual Editor checks: window visual + Act 2 banter sizing. Archive:
`Docs/completed_tasks/M0-T33_lily_chat.md`. Next: M0-T34 (IBM course content coverage map).

### 2026-06-25 — M0-T34: IBM course content coverage map — Completed (docs/analysis)
Codex rendered all 44 PDF pages (Poppler) and produced `Docs/IBM_COURSE_CONTENT.md`: a page-cited course
outline, a coverage map separating "introduced/explained in-game" vs "practiced via mechanic" (most
concepts partial or missing — the game practices intent/entity/dialog but does not yet TEACH them;
fundamentals + breadth missing), a prioritized gap list (M0-T35 fundamentals → M0-T36/37/38 strengthen
Acts 1–3 teaching → M0-T39 planning → M0-T40–44 Acts 4–8), out-of-scope items, and a playable-teaching
design pattern (Ghost problem + Lily explanation + action + consequence; no lecture/quiz). Claude could
not re-render the image PDF but cross-checked the outline against `CONFIRMED §4/§5` (consistent) and
accepted; scope clean (2 docs only). Archive: `Docs/completed_tasks/M0-T34_ibm_course_coverage_map.md`.
Next: M0-T35 (Chatbot Fundamentals Teaching Pass).

### 2026-06-26 — M0-T28 follow-on: no-password account management — Completed (off-track from M0-T35; accepted)
Codex runs M0-T28_002–008 added a no-password account/profile system on the M0-T28 backend: account
creation, username-based recovery of a profile, an entry gate, account-conflict handling (409
`account_exists`), button/UI layout fixes, and multiple-account coexistence (`createAccount`:
same-profile → return existing; other-profile → `account_exists`; current profile already has a
different account + unused name → new profile). Files: Backend (`database.ts`/`app.ts`/`tests`/README),
Shell (`GhostBackendClient`, `GameShellPresenter`, `GameShellSceneBuilder`, `GhostNarrativeState`),
regenerated `GameShellPrototype.unity`, docs. Claude ran backend build + test → 10/10 pass; scope clean
(no puzzle logic/validators/ProjectSettings; deterministic rule intact — accounts are identity/storage,
not scoring). Run logs honest (Unity "Not run"; risks: no-password recovery, multi-user not a final
design). NOTE: this was OFF the stated `CURRENT_TASK` (M0-T35 fundamentals) — an accepted user-directed
detour; future Codex runs should track `CURRENT_TASK`. Residual: Unity Editor verification of the
account UI (the iterative UI-fix runs imply in-Editor testing). `CURRENT_TASK` remains M0-T35 (next).

### 2026-06-27 — M0-T35: Chatbot Fundamentals Teaching Pass — Completed
Codex run 001 added a compact, playable chatbot-fundamentals overview to the Game Shell (the preserved
"former Act 0"): an optional `Ghost's Voice Basics` hub card walks the player through six teaching beats —
chatbot definition, NLP+ML pillars, rule-based vs AI-enabled, benefits, the five components (an ordering
puzzle + backend side link), and the four challenges — each as Ghost problem → Lily explanation → small
player action → visible Ghost consequence (no lecture/quiz). New `ChatbotFundamentalsData` +
`ChatbotFundamentalsPresenter`; `GameShellPresenter`/`GameShellSceneBuilder` add the screen + hub card +
`Finished → ShowActHub`; the sequence is optional and does not gate Acts 1–3. Correctness stays
deterministic (only the component order has a right/wrong state, via `IsSelectedComponentOrderCorrect()`;
no LLM). Run log honest (Unity Play Mode / Test Runner / scene builder "Not run" in-session). The user
verified in the Editor ("通過"): hub card visible, all six beats playable with visible consequences,
component order + backend link work, skip/finish returns to hub, Acts 1–3 still launch, no Console errors.
Claude reviewed scope (clean — no `Assets/Scripts/Puzzles`, no Act 1–3 validators/sessions/rules, no
ProjectSettings/Packages/Build Settings/existing `.meta`). The regenerated `GameShellPrototype.unity` is a
shelved Game Shell scene side-effect and is excluded from the commit. Archive:
`Docs/completed_tasks/M0-T35_fundamentals_teaching.md`. Next: M0-T36 (strengthen Act 1 intent teaching).

### 2026-06-27 — Planning sync: Act 4 sentiment routing + Act 7 dedup; M0-T36 scope (no code)
User-confirmed refinements recorded across ROADMAP / IBM_COURSE_CONTENT / LEARNING_CONTENT: Act 4
(M0-T40) gains sentiment-based routing/escalation (sentiment as a routing signal, never scoring); Act 7
(M0-T43) deduped to POS / sentiment / machine translation because tokenisation + NER are taught in Act 2
(M0-T37). M0-T36 scope expanded to Act 1's two original facets — classification **and** training examples
(`CONFIRMED_PROJECT_CONTEXT.md` §5 "Act 1: Intent — classification + training examples"). Docs only; the
overall task plan (M0-T35 → T44) and course-content mapping are unchanged otherwise.

### 2026-06-28 — M0-T36: Strengthen Act 1 as Intent teaching — Completed
Codex runs 001 (teaching) + 002 (visual clarity) + 003 (layout fit) made Act 1 TEACH intent without
touching the mechanic or validator. Added a runtime `Lily's Intent Note` panel (Ghost problem + Lily's
stammered explanation that intent = the visitor's purpose and that varied phrasings become training
examples); changed group titles from raw ids to purpose labels ("Purpose: find something/locate
Ghost/identify Ghost") and rephrased hints as purposes; on correct Validate the feedback now teaches
through the action — Ghost brightens, states "one purpose per group, even with different words"
(classification), shows a training-examples count computed from the real card data, and adds a Lily
planning-link line. Run 002 added a dedicated warm teaching panel + green success-teaching state; run 003
reduced card/group/teaching/validation heights + tightened runtime root layout so the bottom
validation/banter area fits 1920×1080. Correctness stays deterministic (`ValidateCurrentState()`; only the
feedback string changed; LLM never scores). Run logs honest (Unity "Not run" each; all 3 retained). The
user verified in Play Mode ("pass"): teaching panel + purpose labels + green success teaching visible, all
9 cards/3 groups/validation/banter fit at 1080p, no Console errors. Claude reviewed scope (clean — empty
diff over `Assets/Scripts/Puzzles`, `Act1.../Editor`, `Fundamentals`). The dirty `Act1...Prototype.unity`
and `GameShellPrototype.unity` scenes are shelved side-effects, excluded from the commit. Archive:
`Docs/completed_tasks/M0-T36_act1_intent_teaching.md`. Next: M0-T37 (strengthen Act 2 entity/NER +
tokenization teaching).

### 2026-07-03 — Project review + docs sync (planning, no code; M0-T37 running in parallel)
Claude ran a whole-project review against the grading basis (`CONFIRMED_PROJECT_CONTEXT.md` §9
evaluation = ① curriculum-to-puzzle mapping ② prototype implementation check ③ LLM tutor response check;
no user study; §10 RQ; 4th group-meeting slides). Verdict: all three evaluation axes already have
evidence (coverage map / playable Acts 1–3 + tests + run logs / live LLM logs); weakest axes are visual
UI/UX and game feel — Ghost has no in-game visual presence (FR1/FR4 currently satisfied via text only)
and there is no audio; guaranteed-minimum mechanics still missing slider calibration (→ Act 4) and
multiple choice (→ can ride M0-T39); WebGL build never smoke-tested (NFR3 risk). User decisions:
deadline ≈ **early August 2026**; **keep the original task order** (T37→T38→T39→T40…); re-assess scope
after M0-T38 (Acts 5–7 may ship as design mappings; Ghost-expressions+SFX pass is a candidate insert).
Docs refreshed to current state: ROADMAP (Current Status rewritten to post-M0-T36 reality + deadline
constraint; coverage section marked ACTIVE), ARCHITECTURE (new Implemented Architecture Snapshot;
per-layer statuses; slice-id traceability note), DESIGN_RATIONALE (deduplicated the two repeated
vertical-slice / Act 3 redesign sections; added rationales for course-content coverage, the shell
fundamentals overview, and teaching-through-feedback), VERTICAL_SLICE_PLAN (marked DELIVERED).
LEARNING_CONTENT / CODE_WALKTHROUGH / UNITY_TEST_CHECKLIST deliberately untouched — Codex is editing
them in the in-flight M0-T37 run (avoid collision); LEARNING_CONTENT legacy-mojibake cleanup remains a
small pending chore after M0-T37 lands. Next: review/close M0-T37 on Codex return + human Play Mode
verification.

### 2026-07-03 — M0-T37 reviewed but NOT closed; teaching approach REJECTED → M0-T45 redesign
Claude reviewed Codex M0-T37 run 001 (Act 2 teaching text layer): scope clean (only the 2 Act 2
presentation files + 3 docs + run log; validators/session/sample data/Editor builder untouched),
deterministic scoring unchanged, synonym line derived from real sample data, run log honest. But the
user rejected the whole M0-T36/T37 teaching METHOD before Play Mode verification: the gameplay is
identical to before with text bolted on — "the teaching content should BECOME the game; nothing is
learned; both acts must be redone." Direction confirmed by the user: full interaction redesign
(mechanics too, not just a consequence phase), BOTH acts in ONE task, plus a programmatic Ghost
expression face (neutral/happy/confused/sad). New task **M0-T45 — Acts 1–2 teaching-as-gameplay
redesign** written to `CURRENT_TASK.md` with the agreed design: Act 1 = watch Ghost fail → free-cluster
the 9 messages into piles + assign purpose labels (key topics derived by the player) → "Teach Ghost" →
Ghost classifies UNSEEN test messages by the majority-of-related-training-cards rule and answers
right/wrong visibly → iterate; Act 2 = watch Ghost fail an errand → "Split" the sentence into tokens →
drag tokens into typed WHAT/WHERE/WHEN action-card slots (same EntitySpan model) → "Go, Ghost!" runs
the errand with authored success/cute-failure outcomes per slot correctness → laboratory/lab synonym
resolution beat. Existing validators/sessions/sample data stay untouched and remain the single scoring
source (new pure demo engines + authored demo data as NEW Ghost.Runtime files + EditMode tests);
interactions stay within the confirmed §7 mechanic set. M0-T36/T37 text panels remain as flavour; the
success-text teaching beats will be replaced. M0-T37 run 001 code stays in the tree as the substrate
for the redesign (commit as an honestly-labelled checkpoint). The old M0-T38 (Act 3 teaching-text
pass) is likely absorbed/reduced — Act 3 already teaches through test-case consequences; reassess
after M0-T45. Next: Codex implements M0-T45 (run 001 = Ghost face + Act 1; run 002 = Act 2).

### 2026-07-04 — M0-T45 Run 001: Ghost face + Act 1 teaching-as-gameplay — Accepted (task continues)
Codex run 001 delivered the redesigned Act 1: pure `Act1TeachingDemoData` (3 scripted intro failures,
per-intent reply lines keyed to real sample-data intent ids, 4 unseen test messages whose
relatedCardIds Claude verified against `Act1IntentClassificationSampleData` — exact match) +
`Act1GhostGeneralizationEngine` (deterministic plurality-of-related-cards rule; tie/unassigned/
unlabelled-pile → Confused; misleading-card reporting) + 4 EditMode tests
(correct/wrong-majority/tie/unlabelled); shared `GhostAvatar` (GhostMood, programmatic GhostFaceView:
neutral/happy/confused/sad, built-in sprites only); Act 1 rebuilt as Intro → Build (free piles +
purpose-label chips, new LabelDragView/TeachingDropTarget) → Teach/Demo (Ghost answers unseen visitors
from the player's piles, wrong answers highlight misleading cards, revise-and-reteach) → Complete.
Completion requires `IntentClassificationValidator` correct AND all demo messages correct (validator
stays the deterministic authority; attempt logging kept). Claude review: scope clean (guards on
validator/session/sample data, Act 2/3, Fundamentals, Shell, Backend, ProjectSettings, Packages all
empty; the 3 dirty scenes are pre-existing shelved side-effects), run log honest (dotnet builds 0
errors; dotnet test cannot see Unity tests — recorded, not claimed). User Play Mode verdict:
"玩法比之前好很多" — ACCEPTED with known UX debt deferred to Run 003: no upfront Lily how-to-play
beat, objective not persistently visible. Run 002 (Act 2 errand/token-slot redesign) must include the
how-to intro + persistent objective strip from the start. Residual: run EditMode Test Runner once in
the Editor for the new engine tests. M0-T45 remains the active task.

### 2026-07-04 — M0-T45: Acts 1–2 teaching-as-gameplay + Ghost face — Completed
Runs 002–005 completed the redesign and the user's iterative Play Mode feedback: run 002 rebuilt Act 2
as the "Ghost's errand" loop (Lily onboarding + persistent objective strip; Split-into-tokens; drag
tokens into typed WHAT/WHERE/WHEN slots via the same EntitySpan model; "Go, Ghost!" plays authored
success/cute-failure outcomes; lab/laboratory synonym resolution; `Act2ErrandOutcomeEngine` +
`Act2ErrandDemoData` + 4 EditMode tests — success AND per-slot states derive from
`EntityExtractionValidator`); run 003 fixed missing `UI/Skin/*.psd` sprite errors, added the reusable
`FloatingWindowDragHandle` (Lily chat draggable) and a clear Act 1 `Complete Act` path; run 004 added
Act 2 retry after failed errands, made ambient banter a draggable floating panel, and added the first
generated Lily pixel portrait; run 005 fixed stuck drag previews (global active-preview tracking) and
corrected the portrait to the user's design (gold short hair, glasses, blue suit jacket, white shirt,
black pants, black heels). Scope guards clean on every run; run logs honest (dotnet builds only; Unity
"Not run" in-session). The user accepted after Play Mode ("這章也不錯") and explicitly authorized
Claude to directly polish the Lily portrait as an exception: Claude rebuilt
`LilyPixelPortraitFactory` from 32×32 FillRects to a 48×48 string-pixel map (iterated visually in a
scratchpad render before porting; original art, black-framed glasses with visible eyes, gold bob,
lapelled blue suit, heels), `dotnet build` clean. Archive:
`Docs/completed_tasks/M0-T45_acts12_teaching_as_gameplay.md`. Next: M0-T39 (planning mini-level,
teaching-as-gameplay), then M0-T40 (Act 4 slider/fallback) + WebGL smoke test before the writing window
(deadline ≈ early August).

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
