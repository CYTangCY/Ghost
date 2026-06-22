# M0-T21 — Run 001 — act3 node graph core

## Task ID

M0-T21

## Run Number

001

## Date

2026-06-22

## Original Request / Codex Prompt Summary

Implement only the Act 3 node-graph core as pure C# logic in `Ghost.Runtime`, plus sample data and EditMode tests. Keep the slice scene-free and UI-free. Mirror the Act 1/Act 2 deterministic validator pattern. Do not edit asmdefs, ProjectSettings, Packages, `.meta` files, `Docs/CURRENT_TASK.md`, or Act 1/Act 2 logic.

## Files Created

- `Assets/Scripts/Puzzles/DialogGraph/DialogNodeType.cs`
- `Assets/Scripts/Puzzles/DialogGraph/DialogNode.cs`
- `Assets/Scripts/Puzzles/DialogGraph/DialogTransition.cs`
- `Assets/Scripts/Puzzles/DialogGraph/DialogGraph.cs`
- `Assets/Scripts/Puzzles/DialogGraph/ConversationTurn.cs`
- `Assets/Scripts/Puzzles/DialogGraph/DialogContext.cs`
- `Assets/Scripts/Puzzles/DialogGraph/DialogGraphSimulator.cs`
- `Assets/Scripts/Puzzles/DialogGraph/DialogGraphValidator.cs`
- `Assets/Scripts/Puzzles/DialogGraph/Act3DialogGraphSampleData.cs`
- `Assets/Tests/EditMode/Act3DialogGraphSimulatorTests.cs`
- `Assets/Tests/EditMode/Act3DialogGraphValidatorTests.cs`
- `Docs/codex_runs/M0-T21_001_act3_node_graph_core.md`

Observed Unity-generated untracked `.meta` side effects while the Editor/project was active, not manually authored or edited by Codex:
- `Assets/Scripts/Puzzles/DialogGraph.meta`
- `Assets/Scripts/Puzzles/DialogGraph/ConversationTurn.cs.meta`
- `Assets/Scripts/Puzzles/DialogGraph/DialogContext.cs.meta`
- `Assets/Scripts/Puzzles/DialogGraph/DialogGraph.cs.meta`
- `Assets/Scripts/Puzzles/DialogGraph/DialogNode.cs.meta`
- `Assets/Scripts/Puzzles/DialogGraph/DialogNodeType.cs.meta`
- `Assets/Scripts/Puzzles/DialogGraph/DialogTransition.cs.meta`

## Files Modified

- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read required docs and existing Act 1/Act 2 patterns.
- `rg "UnityEngine" Assets\Scripts\Puzzles\DialogGraph Assets\Tests\EditMode\Act3DialogGraphSimulatorTests.cs Assets\Tests\EditMode\Act3DialogGraphValidatorTests.cs`
- `dotnet restore Ghost.Runtime.csproj`
- `dotnet build Ghost.Runtime.csproj --no-restore`
- `dotnet restore Ghost.EditModeTests.csproj`
- `dotnet build Ghost.EditModeTests.csproj --no-restore`
- `git diff --check -- Docs\CODE_WALKTHROUGH.md Docs\UNITY_TEST_CHECKLIST.md`
- `Select-String` trailing-whitespace check over new Act 3 C# files, new Act 3 test files, and this run log
- Unity Editor Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity Editor tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- No `UnityEngine` references were found in the new Act 3 runtime/test files.
- `Ghost.Runtime.csproj` build passed with 0 warnings and 0 errors.
- `Ghost.EditModeTests.csproj` build passed with 0 warnings and 0 errors.
- Scoped doc diff check passed; only line-ending warnings were reported.
- New Act 3 C# files, new Act 3 test files, and this run log had no trailing-whitespace matches.
- A full `git diff --check` also reported trailing whitespace in a pre-existing dirty Act 1 scene file; that file was outside M0-T21 and was not edited by this task.
- Not run — Unity Editor test runner was not executed in this Codex session.
- Not run — Unity Editor Play Mode was not executed in this Codex session.

## Errors Encountered

- Initial `dotnet build --no-restore` failed because Unity-generated `project.assets.json` files were missing.
- Initial `dotnet restore` attempts were blocked by sandboxed access to the user NuGet config at `C:\Users\fcxsw\AppData\Roaming\NuGet\NuGet.Config`.

## Fixes Applied

- Re-ran the required `dotnet restore` commands with user-approved elevated access so the local generated-project compile checks could run.
- Adjusted validator mismatch errors to include both expected and reached response ids, making failures clearer and testable.

## What Was Intentionally Not Changed

- No scenes, UI, node-graph interaction, session/state layer, backend, LLM, Act 4-6 node types, or Game Shell integration were added.
- No Act 1 or Act 2 puzzle logic was edited.
- No `.asmdef`, `Docs/CURRENT_TASK.md`, Packages, Build Settings, or ProjectSettings files were intentionally edited.
- Pre-existing dirty worktree entries outside M0-T21 were left untouched: `Assets/Scenes/Act1IntentClassificationPrototype.unity`, `ProjectSettings/ProjectSettings.asset`, and `ProjectSettings/ShaderGraphSettings.asset`.
- Unity-generated `.meta` side effects were observed and left untouched.

## Remaining Risks

- Unity Editor import and EditMode Test Runner still need human verification.
- Play Mode is out of scope for this logic-only slice and was not run.
- The minimal Act 3 intent-routing convention uses `Start -> IntentBranch` Always transitions where the target branch node's `IntentId` matches the turn intent; Claude should review this against the M0-T20 design wording because the transition enum intentionally does not include an intent-specific condition.
- Unity may generate additional `.meta` files for new scripts when the project imports them.

## Next Recommended Step

Open Unity, let scripts import, run the M0-T21 EditMode tests listed in `Docs/UNITY_TEST_CHECKLIST.md`, and have Claude review this run log plus the changed files before task closure.
