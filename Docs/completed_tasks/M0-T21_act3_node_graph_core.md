# M0-T21 — Act 3 Node Graph Core (model + simulator + validator + sample data + tests)

## Completion Status

Completed. Codex run 001 implemented the pure C# Act 3 core; the user ran the EditMode tests in the
Unity Editor and reported all correct ("全部都正確並測試成功"). Claude reviewed the simulator,
validator, sample data, scope, and run log.

## Date

2026-06-22

## Summary

First Act 3 logic-first slice (mirrors M0-T04/M0-T14), scene-free. Pure C# in
`Ghost.Runtime` (`Ghost.Puzzles.DialogGraph`): immutable `DialogNode`/`DialogTransition`/`DialogGraph`,
`DialogNodeType` (Start/IntentBranch/SlotCheck/Response), `ConversationTurn` + `DialogContext`, a
deterministic `DialogGraphSimulator` (step-capped), a `DialogGraphValidator` (test-case + structural
checks), and `Act3DialogGraphSampleData` (one level). EditMode tests for the simulator and validator.
Correctness is deterministic; the LLM is not involved.

## Files Created

- `Assets/Scripts/Puzzles/DialogGraph/` — `DialogNodeType.cs`, `DialogNode.cs`, `DialogTransition.cs`,
  `DialogGraph.cs`, `ConversationTurn.cs`, `DialogContext.cs`, `DialogGraphSimulator.cs`,
  `DialogGraphValidator.cs`, `Act3DialogGraphSampleData.cs`
- `Assets/Tests/EditMode/Act3DialogGraphSimulatorTests.cs`, `Act3DialogGraphValidatorTests.cs`
- `Docs/codex_runs/M0-T21_001_act3_node_graph_core.md`

## Files Modified

- `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`

## Claude Review Notes

- Deterministic: simulator terminates (step cap = Nodes.Count+1); validator decides correctness via
  simulation + structural checks; no UnityEngine, no LLM.
- Intent routing realized as: Start → per-intent `IntentBranch` nodes via `Always` transitions, routing
  by the branch node's `IntentId` (validator's reachability/dead-end/intent-handled checks are
  consistent). This is a valid realization of the M0-T20 design ("intent routes branches").
- Sample level covers slot-present (answer) and slot-missing (ask_for_room) — the two required cases.
- Scope clean: only the DialogGraph folder + 2 tests + run log + 2 docs (+ Unity-generated `.meta`); no
  Act 1/2 logic, asmdefs, ProjectSettings, or Packages edited.
- Run log honest: dotnet build (Codex's own check) passed; Unity Play Mode/tests "Not run" in-session;
  transparently noted the NuGet-restore elevation and the flagged routing convention.

## Human Verification Result

The user ran the M0-T21 EditMode tests in the Unity Editor and reported all passing
("全部都正確並測試成功"). Source of the "tests pass" status — not the Codex session.

## Remaining Risks

- Manual-in-Editor verification only.
- For the UI slices, the routing convention (Start fans out to per-intent IntentBranch nodes) must be
  reflected in how the player wires the graph.
- Unity-generated `.meta` files accompany the new scripts; commit them with the scripts.

## Next Task

M0-T22 — Act 3 graph session/state: a pure C# layer the player UI will use to build/edit a dialog
graph (add/remove nodes, connect/disconnect transitions, set node config) and validate the current
state via `DialogGraphValidator`. Scene-free, EditMode tests. Mirrors Act 2's M0-T15.
