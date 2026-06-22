# CURRENT_TASK.md

## ID

M0-T22

## Goal

Implement the Act 3 graph session/state layer as pure C#: the layer the player UI will use to build and
edit a dialog graph (add/remove nodes, connect/disconnect transitions, set node config, set the start
node) and validate the current state via `DialogGraphValidator`. Scene-free and UI-free, mirroring
Act 2's `EntityExtractionSession` (M0-T15).

## Context

ROADMAP Phase C = Act 3 (flagship). M0-T21 added the Act 3 core (model + `DialogGraphSimulator` +
`DialogGraphValidator` + `Act3DialogGraphSampleData`), Editor-verified. This task adds the session/state
layer so a later node-graph UI (M0-T23/M0-T24) can mutate the player's in-progress graph while this
class owns the building state and delegates correctness to the validator. Correctness stays
deterministic; the LLM is not involved.

## Scope

- Create `DialogGraphSession` in `Ghost.Runtime` (`Ghost.Puzzles.DialogGraph`,
  `Assets/Scripts/Puzzles/DialogGraph/`), no UnityEngine dependency:
  - holds the player's in-progress graph (mutable building state) plus the level's test cases;
  - a convenience to create the level from `Act3DialogGraphSampleData` (its test cases + the available
    intent/entity/response/node vocabulary), starting from an empty graph (or a seed Start node);
  - mutators: add a node (by type + the config it needs, returning its id) / remove a node;
    add a transition (from, to, condition) / remove a transition; set the start node;
  - a snapshot of the current `DialogGraph` and the level's test cases;
  - `ValidateCurrentState()` → `DialogGraphValidator.Validate(currentGraph, testCases)` returning the
    `DialogGraphResult`.
- Keep mutations safe and documented (e.g. removing a node also removes its transitions; adding a
  transition to/from unknown nodes is rejected or surfaced by validation — choose and document).
- Reuse the M0-T21 types unchanged (do not modify them unless a small behaviour-preserving addition is
  needed; note it in the run log).
- EditMode tests (`Ghost.EditModeTests`): empty/partial graph validates incorrect; building the correct
  graph validates correct; add/remove node and transition behave as documented; ValidateCurrentState
  delegates to the validator.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md; create a Codex run log.

## Out of Scope

- No scenes, UI, or node-graph interaction (M0-T23/M0-T24).
- No Act 4–6 node types, backend, LLM, or Act 1/2 changes.
- Do not edit asmdefs, ProjectSettings, Packages, `.meta` files, or `Docs/CURRENT_TASK.md`.

## Acceptance Criteria

- `DialogGraphSession` exists as pure C# (no UnityEngine), owning the player's in-progress graph and
  delegating correctness to `DialogGraphValidator`.
- The session can be created for the sample level (test cases + vocabulary) and supports
  add/remove node, add/remove transition, and set start node, with documented edge behaviour.
- An empty/partial graph validates incorrect; assembling the correct graph validates correct.
- EditMode tests cover the above.
- Code is scene-free and WebGL-safe; no scene or ProjectSettings files are modified; a Codex run log is
  created.
