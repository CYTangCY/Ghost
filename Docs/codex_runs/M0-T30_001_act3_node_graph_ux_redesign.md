# M0-T30 — Run 001 — Act 3 node graph UX redesign

## Task ID

M0-T30

## Run Number

001

## Date

2026-06-23

## Original Request / Codex Prompt Summary

Implement only M0-T30: redesign the Act 3 dialog graph prototype with a clearer objective, node input/output ports, drag-a-wire connection interaction, node/wire removal, and deterministic Validate feedback through the existing `DialogGraphSession`. Keep Act 3 pure logic unchanged. Do not add the scene to Build Settings or touch backend/LLM/Game Shell scope.

## Files Created

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphOutputPortView.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInputPortView.cs`
- `Docs/codex_runs/M0-T30_001_act3_node_graph_ux_redesign.md`

## Files Modified

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
- `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read required project/task docs before implementation.
- `git diff --check -- Assets\Presentation\Act3DialogGraph\Act3DialogGraphInteractionController.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphStaticPresenter.cs Assets\Presentation\Act3DialogGraph\Editor\Act3DialogGraphPrototypeSceneBuilder.cs Docs\CODE_WALKTHROUGH.md Docs\UNITY_TEST_CHECKLIST.md`
- `Select-String` trailing-whitespace scan for the Act 3 presentation scripts and updated docs.
- `git diff -- Assets\Scripts\Puzzles\DialogGraph`
- `rg "EditorBuildSettings|BuildSettings|UnityWebRequest|OpenAI|LLM" Assets\Presentation\Act3DialogGraph`
- `dotnet build Ghost.Presentation.csproj --no-restore`
- Unity scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `git diff --check`: passed; only line-ending warnings were reported.
- Trailing-whitespace scan: passed; no matches.
- Pure Act 3 logic diff check: passed; no changes under `Assets/Scripts/Puzzles/DialogGraph`.
- Act 3 presentation scope scan: passed; no Build Settings, LLM, backend, OpenAI, or network references in `Assets/Presentation/Act3DialogGraph`.
- `dotnet build Ghost.Presentation.csproj --no-restore`: failed because the Unity-generated `Ghost.Presentation.csproj` currently includes `Act3DialogGraphStaticPresenter.cs` and `Act3DialogGraphInteractionController.cs`, but does not yet include the newly added `Act3DialogGraphOutputPortView.cs` and `Act3DialogGraphInputPortView.cs`. Unity should regenerate project files/import these scripts because they are under the existing `Ghost.Presentation` asmdef. The `.csproj` was not hand-edited.
- Unity scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- External `dotnet build` could not compile the presentation project because the generated `.csproj` is stale and does not list the two new port view scripts.

## Fixes Applied

- Added dedicated output-port and input-port view components for Unity EventSystem drag/drop.
- Updated the Act 3 interaction controller to auto-set Start nodes, reject self-loops, reject duplicate exact edges, reject unknown endpoints, reject Response-source edges, reject source-node/condition mismatches, and expose deterministic validation feedback.
- Updated the Act 3 presenter to render an objective panel, node cards with ports, temporary drag wires, committed straight-line wires, wire removal rows, an enabled Validate button, and green/red feedback.
- Updated the scene builder subtitle to describe the new drag-wire/validate UX.
- Updated walkthrough and test checklist documentation for M0-T30.

## What Was Intentionally Not Changed

- Act 3 pure logic/session files under `Assets/Scripts/Puzzles/DialogGraph`.
- Act 1 and Act 2 scripts.
- Game Shell scripts or act list.
- ProjectSettings, Packages, Build Settings, `.meta` files, and existing scenes.
- Backend, LLM, save/load, scoring persistence, final art, Act 3 Shell integration, and Act 4-6 node graph scope.

## Remaining Risks

- Unity Editor import is required to regenerate project files and include the two new port scripts in the presentation assembly.
- The generated scene may look stale until the user reruns `Ghost > Build Act 3 Dialog Graph Prototype Scene`.
- Drag/drop behaviour, line placement, validation feedback colours, and layout need human Play Mode verification in Unity.

## Next Recommended Step

Open Unity, let scripts import, rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene`, then complete the M0-T30 Play Mode checklist in `Docs/UNITY_TEST_CHECKLIST.md`.
