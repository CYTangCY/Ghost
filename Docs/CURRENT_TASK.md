# CURRENT_TASK.md

## ID

M0-T21

## Goal

Implement the Act 3 node-graph CORE as pure C# logic — graph data model + deterministic simulator +
validator — plus sample data and EditMode tests. Scene-free and UI-free, mirroring the Act 1/Act 2
logic-first pattern (M0-T04 / M0-T14).

## Context

ROADMAP Phase C = Act 3 (flagship node graph). M0-T20 fixed the design: see `Docs/LEARNING_CONTENT.md`
(Act 3) and `Docs/ARCHITECTURE.md` ("Act 3 minimal concrete design"). This is the first logic-first
slice. Correctness is deterministic (graph simulation), never the LLM. Intent (Act 1) routes branches;
entity (Act 2) fills slots; context remembers collected details.

## Scope

- Pure C# in the `Ghost.Runtime` assembly, new folder `Assets/Scripts/Puzzles/DialogGraph/`
  (namespace `Ghost.Puzzles.DialogGraph`), no UnityEngine dependency:
  - `DialogNodeType` enum: `Start`, `IntentBranch`, `SlotCheck`, `Response`.
  - `DialogNode` — immutable: `Id` + `Type` + the per-type config it needs (`IntentId` for
    `IntentBranch`, `RequiredEntityType` for `SlotCheck`, `ResponseId` for `Response`); use string ids
    for intent/entity-type/response (decoupled, like Act 1's string `intentId`); reject empty `Id`.
  - `DialogTransition` — `FromNodeId`, `ToNodeId`, `Condition` enum `{ Always, SlotPresent, SlotMissing }`.
  - `DialogGraph` — nodes + transitions + `StartNodeId`, with lookups.
  - `ConversationTurn` — `IntentId` (string) + entities (`type→value` string map, from the Act 2
    concept); `DialogContext` — filled slots (`type→value`), carried across turns.
  - `DialogGraphSimulator.Simulate(graph, turn, context)` → result with the reached `ResponseId` (+
    updated context). Walk from `Start`; `IntentBranch` follows the transition matching `turn.IntentId`;
    `SlotCheck` follows `SlotPresent`/`SlotMissing` by whether the required entity is in turn/context
    (fill context when present); stop at a `Response`. Deterministic, with a step cap to guard cycles.
  - `DialogGraphValidator.Validate(graph, testCases)` → `DialogGraphResult { bool IsCorrect;
    IReadOnlyList<string> Errors }`: per test case (`turn` → expected `ResponseId`, e.g. an answer id
    or an `ask_for_<entity>` id) run the simulator and compare; plus structural checks (Start present,
    no unreachable nodes, no dead ends, each test's intent handled).
- `Act3DialogGraphSampleData` — one level: the intents/entities/response vocabulary + a correct/target
  graph (or the test cases + expected responses), covering at least a slot-present answer and a
  slot-missing `ask_for_<entity>` case.
- EditMode tests (`Ghost.EditModeTests`): correct graph validates true; wrong-intent-wired, missing
  slot-check, wrong response, and unreachable/dead-end cases validate false; simulator slot-present vs
  slot-missing routing; cycle/step-cap termination safety.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md; create a Codex run log.

## Out of Scope

- No scenes, UI, or node-graph interaction (later slices M0-T23/M0-T24).
- No graph session/state layer yet (that is M0-T22).
- No Act 4–6 node types, backend, LLM, or Act 1/2 changes.
- Do not edit asmdefs, ProjectSettings, Packages, `.meta` files, or `Docs/CURRENT_TASK.md`.

## Acceptance Criteria

- Pure C# Act 3 graph model + `DialogGraphSimulator` + `DialogGraphValidator` exist with no UnityEngine
  dependency (like the Act 1/2 validators).
- Sample data has one level covering a slot-present answer and a slot-missing "ask" case.
- The simulator deterministically routes by intent + slot presence and always terminates (step cap).
- The validator returns `IsCorrect` + `Errors` for a correct graph and for several
  incorrect/structural cases.
- EditMode tests cover correct + incorrect + simulator routing + termination.
- Code is scene-free and WebGL-safe; no scene or ProjectSettings files are modified; a Codex run log is
  created.
