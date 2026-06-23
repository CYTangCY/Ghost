# CURRENT_TASK.md

## ID

M0-T24

## Goal

Add node placement + connection interaction to the Act 3 node-graph scene: the player places nodes from
the palette, configures each node's field (intent / entity-slot / response), connects nodes with a
transition condition, and sets the start node — all routed through `DialogGraphSession`. No validation
feedback wiring yet.

## Context

ROADMAP Phase C = Act 3 (flagship). M0-T21 core, M0-T22 session, and M0-T23 static UI are done and
Editor-verified. Act 1/2 added interaction incrementally (placement/assignment, then validation
feedback). This is the Act 3 equivalent of M0-T17: wire the graph-editing interaction through
`DialogGraphSession` (M0-T22). Validation feedback is the next task (M0-T25); Validate stays a
placeholder here.

## Scope

- Add an Act 3 interaction controller in `Assets/Presentation/Act3DialogGraph/` (mirror Act 2's
  interaction controller), wrapping a `DialogGraphSession`, exposing the current nodes/transitions/start
  for rendering and a `StateChanged` event.
- Extend `Act3DialogGraphStaticPresenter` (or a new interactive presenter) so the player can:
  - place a node by choosing a palette node type (Start/IntentBranch/SlotCheck/Response) → `AddNode`;
    for typed nodes, set the required field from the level vocabulary (IntentBranch→intent id,
    SlotCheck→entity-type id, Response→response id);
  - select a node and mark it as the start node → `SetStartNode`;
  - connect two placed nodes with a chosen condition (Always / SlotPresent / SlotMissing) → `AddTransition`;
  - remove a node (cascades transitions) and remove a transition.
- Render the placed nodes and the existing transitions readably. A robust minimal presentation is
  acceptable: nodes as placed cards/rows and transitions as a clear list (e.g. "start → intent_find_object
  [Always]") and/or simple connector visuals — do NOT over-engineer bezier edge drawing.
- All placement/config/connection/removal MUST route through `DialogGraphSession`; correctness stays in
  the validator (wired in M0-T25). Keep the M0-T23 palette/goal/canvas layout; ensure an `EventSystem`.
- Regenerate the scene via the menu builder if needed (do NOT add it to Build Settings).
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md; create a Codex run log.

## Out of Scope

- Do not wire the Validate button / show correct-incorrect feedback yet (M0-T25); it stays a placeholder.
- No Act 4–6 node types, backend, LLM, save/load, scoring, or final art; no Game Shell change.
- Do not add the scene to Build Settings.
- Do not edit ProjectSettings, Packages, `.meta`, existing non-Act-3 asmdefs, the Act 3 pure logic
  (M0-T21/M0-T22), or Act 1 / Act 2 / Game Shell scripts.

## Acceptance Criteria

- The player can place each node type from the palette and set its field from the level vocabulary; the
  node appears in the canvas.
- The player can mark a start node and connect two nodes with a chosen condition; placed nodes and
  transitions are visible/readable.
- A node can be removed (its transitions go with it) and a transition can be removed.
- All edits route through `DialogGraphSession` (the Act 3 pure logic is unchanged); the Validate button
  remains a placeholder.
- No Console errors in Play Mode; the scene is not in Build Settings; Act 1/2 and the Game Shell are
  unchanged.
- CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md are updated; a Codex run log is created.
