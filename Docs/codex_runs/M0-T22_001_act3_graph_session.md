# M0-T22 — Run 001 — act3 graph session

## Task ID

M0-T22

## Run Number

001

## Date

2026-06-22

## Original Request / Codex Prompt Summary

Implement only the Act 3 graph session/state layer as pure C# in `Ghost.Runtime`, plus EditMode tests. The session must hold mutable in-progress graph state, generate unique node ids, cascade node removal to transitions, validate incomplete graphs as incorrect without throwing, and delegate complete graph correctness to `DialogGraphValidator`. Keep the task scene-free and UI-free, and do not modify M0-T21 core types, asmdefs, ProjectSettings, Packages, `.meta` files, or `Docs/CURRENT_TASK.md`.

## Files Created

- `Assets/Scripts/Puzzles/DialogGraph/DialogGraphSession.cs`
- `Assets/Tests/EditMode/Act3DialogGraphSessionTests.cs`
- `Docs/codex_runs/M0-T22_001_act3_graph_session.md`

## Files Modified

- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read required docs and existing Act 2/M0-T21 patterns.
- `dotnet build Ghost.Runtime.csproj --no-restore`
- `dotnet build Ghost.EditModeTests.csproj --no-restore`
- `rg "UnityEngine|MonoBehaviour|SceneManager" Assets\Scripts\Puzzles\DialogGraph\DialogGraphSession.cs Assets\Tests\EditMode\Act3DialogGraphSessionTests.cs`
- `git diff --check -- Assets\Scripts\Puzzles\DialogGraph\DialogGraphSession.cs Assets\Tests\EditMode\Act3DialogGraphSessionTests.cs Docs\CODE_WALKTHROUGH.md Docs\UNITY_TEST_CHECKLIST.md Docs\codex_runs\M0-T22_001_act3_graph_session.md`
- `Select-String` trailing-whitespace check over the new session file, new test file, and this run log
- Unity Editor Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity Editor tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `Ghost.Runtime.csproj` build passed with 0 warnings and 0 errors.
- `Ghost.EditModeTests.csproj` build passed with 0 warnings and 0 errors.
- No Unity API references were found in the new M0-T22 runtime/test files.
- Scoped doc diff check passed; only line-ending warnings were reported.
- New M0-T22 C# files and this run log had no trailing-whitespace matches.
- Not run — Unity Editor test runner was not executed in this Codex session.
- Not run — Unity Editor Play Mode was not executed in this Codex session.

## Errors Encountered

- None in the M0-T22 implementation after code was added.

## Fixes Applied

- None.

## What Was Intentionally Not Changed

- No M0-T21 core dialog graph types were modified.
- No scenes, UI, node-graph interaction, Act 4-6 node types, backend, LLM, Act 1 logic, or Act 2 logic were added or changed.
- No `.asmdef`, `Docs/CURRENT_TASK.md`, Packages, Build Settings, ProjectSettings, or `.meta` files were intentionally edited.
- Pre-existing dirty worktree entries outside M0-T22 were left untouched: `Assets/Scenes/Act1IntentClassificationPrototype.unity`, `ProjectSettings/ProjectSettings.asset`, and `ProjectSettings/ShaderGraphSettings.asset`.

## Remaining Risks

- Unity Editor import and EditMode Test Runner still need human verification.
- Play Mode is out of scope for this logic-only slice and was not run.
- `SetStartNode` and `AddTransition` intentionally reject unknown node ids immediately; Claude should confirm this chosen edge behaviour is acceptable for the future UI.

## Next Recommended Step

Open Unity, let scripts import, run the M0-T22 EditMode tests listed in `Docs/UNITY_TEST_CHECKLIST.md`, then have Claude review the session behaviour and close/archive M0-T22 if human verification passes.
