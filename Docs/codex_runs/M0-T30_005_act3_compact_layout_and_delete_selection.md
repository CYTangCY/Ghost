# M0-T30 — Run 005 — compact layout and delete selection refinements

## Task ID

M0-T30

## Run Number

005

## Date

2026-06-23

## Original Request / Codex Prompt Summary

User feedback after M0-T30 run 004: shrink the right Guide width by another half, shrink the bottom Test Ghost's map height by another half, give the space back to the canvas, fix node deletion because dragging boxes to trash still does not remove them, add Delete/Backspace removal for selected wires and selected boxes, highlight the trash target while dragging a box over it, move the trash target outside the canvas into the right side of the bottom test bar, remove the Start card's input port, and re-check Ghost reaction text.

## Files Created

- `Docs/codex_runs/M0-T30_005_act3_compact_layout_and_delete_selection.md`

## Files Modified

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
- `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Reviewed active task and required project context docs.
- Inspected current Act 3 presenter, controller, editor scene builder, walkthrough, and checklist.
- `dotnet build Ghost.Presentation.csproj --no-restore`
- `git diff --check -- Assets\Presentation\Act3DialogGraph\Act3DialogGraphStaticPresenter.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphInteractionController.cs Assets\Presentation\Act3DialogGraph\Editor\Act3DialogGraphPrototypeSceneBuilder.cs Docs\CODE_WALKTHROUGH.md Docs\UNITY_TEST_CHECKLIST.md`
- Trailing-whitespace `Select-String` scan for changed files.
- `git diff -- Assets\Scripts\Puzzles\DialogGraph`
- `rg "EditorBuildSettings|BuildSettings|UnityWebRequest|OpenAI|LLM|backend" Assets\Presentation\Act3DialogGraph`
- Unity scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `dotnet build Ghost.Presentation.csproj --no-restore`: passed with 0 warnings and 0 errors.
- `git diff --check`: passed; only CRLF conversion warnings were reported.
- Trailing-whitespace scan: passed; no matches.
- Pure Act 3 logic diff check: passed; no changes under `Assets/Scripts/Puzzles/DialogGraph`.
- Act 3 presentation scope scan: passed; no Build Settings, LLM, backend, OpenAI, or network references in `Assets/Presentation/Act3DialogGraph`.
- Unity scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- No compile errors were encountered in this run.

## Fixes Applied

- Reduced the Act 3 Guide column to a much narrower fixed width and gave the extra space to the graph panel.
- Reduced the bottom validation/test strip height and compressed its button/feedback text.
- Moved the trash drop target out of the graph canvas into the right side of the bottom validation/test strip.
- Added trash highlight while a dragged node is over the trash target.
- Added selected-node Delete/Backspace removal in addition to selected-wire Delete/Backspace removal.
- Added controller `ClearSelection()` so selecting a wire can clear selected-node state without bypassing the controller.
- Removed the input port from Start nodes.
- Shortened the right-side guide/test text for the narrow column.
- Reworked Ghost reaction copy into shorter route-specific outcomes that are readable in the narrow guide and tied to common wrong connections.
- Updated walkthrough and Unity checklist for the compact layout, bottom-bar trash, Start-without-input, selected-node deletion, and trash highlight behaviours.

## What Was Intentionally Not Changed

- Act 3 pure logic/session files under `Assets/Scripts/Puzzles/DialogGraph`.
- Act 1 and Act 2 scripts.
- Game Shell scripts or act list.
- ProjectSettings, Packages, Build Settings, `.meta` files, and unrelated scenes.
- Backend, LLM, save/load, scoring persistence, final art, Act 3 Shell integration, and Act 4-6 node graph scope.

## Remaining Risks

- Human Unity Play Mode verification is required for the new guide width, bottom-bar height, bottom-bar trash hit target, selected-node Delete/Backspace removal, Start-without-input readability, and Ghost reaction wording.
- The user must rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene` to regenerate `Assets/Scenes/Act3DialogGraphPrototype.unity` with the updated builder proportions.
- The worktree already contains Unity/editor side effects and prior untracked M0-T30 files; review `git status` carefully before committing.

## Next Recommended Step

Rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene` in Unity, open `Assets/Scenes/Act3DialogGraphPrototype.unity`, and complete the updated M0-T30 Play Mode checklist focusing on the compact Guide, compact bottom `Test map` strip, bottom-bar trash highlight/removal, selected-node Delete/Backspace removal, Start node with no input port, and route-specific Ghost reactions.
