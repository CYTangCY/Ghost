# M0-T30 — Run 003 — Act 3 palette drag, trash, and outcome feedback

## Task ID

M0-T30

## Run Number

003

## Date

2026-06-23

## Original Request / Codex Prompt Summary

User feedback after M0-T30 run 002: the Act 3 graph should support dragging cards from the palette onto the canvas, the right-side goals and bottom run button take too much space, IN/OUT port boxes should become cute coloured dots/icons with a legend, node removal should use a trash/drop area instead of per-card Remove buttons, wires should be removable by Delete or replaced by dragging a new wire, the level still needs clearer play instructions, and running the map should produce visible Ghost success/failure behaviour based on the player's graph.

## Files Created

- `Docs/codex_runs/M0-T30_003_act3_palette_drag_trash_and_outcome.md`

## Files Modified

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphNodeDragView.cs`
- `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Inspected current Act 3 presentation/controller scripts.
- `dotnet build Ghost.Presentation.csproj --no-restore`
- `git diff --check -- Assets\Presentation\Act3DialogGraph\Act3DialogGraphInteractionController.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphStaticPresenter.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphNodeDragView.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphOutputPortView.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphInputPortView.cs Assets\Presentation\Act3DialogGraph\Editor\Act3DialogGraphPrototypeSceneBuilder.cs Docs\CODE_WALKTHROUGH.md Docs\UNITY_TEST_CHECKLIST.md`
- Trailing-whitespace `Select-String` scan for the changed Act 3 presentation scripts and updated docs.
- `git diff -- Assets\Scripts\Puzzles\DialogGraph`
- `rg "EditorBuildSettings|BuildSettings|UnityWebRequest|OpenAI|LLM|backend" Assets\Presentation\Act3DialogGraph`
- Player-facing raw-text scan for old UI strings and raw ids in Act 3 presentation/docs.
- Unity scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `dotnet build Ghost.Presentation.csproj --no-restore`: passed with 5 existing obsolete `FindFirstObjectByType` warnings and 0 errors.
- `git diff --check`: passed; only line-ending warnings were reported.
- Trailing-whitespace scan: passed; no matches.
- Pure Act 3 logic diff check: passed; no changes under `Assets/Scripts/Puzzles/DialogGraph`.
- Act 3 presentation scope scan: passed; no Build Settings, LLM, backend, OpenAI, or network references in `Assets/Presentation/Act3DialogGraph`.
- Player-facing raw-text scan: Act 3 presentation scripts no longer expose old raw id UI labels. Matches remain in older M0-T23/M0-T24 checklist sections or documentation notes that explicitly discuss internal ids.
- Unity scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- An initial build failed because a newly created `Act3DialogGraphPaletteItemDragView.cs` file was not yet included in the Unity-generated `Ghost.Presentation.csproj`.

## Fixes Applied

- Merged `Act3DialogGraphPaletteItemDragView` into the already included `Act3DialogGraphNodeDragView.cs` file and removed the transient standalone file, avoiding a manual `.csproj` edit.
- Added palette drag-to-canvas placement while keeping click-to-place as a fallback.
- Replaced text-box style ports with compact coloured dot ports.
- Added a compact right-side guide/legend plus Ghost reaction panel.
- Added a trash drop zone for node removal and removed visible per-card/per-wire Remove buttons from the graph UI.
- Added wire selection plus Delete/Backspace removal.
- Changed same-output rewiring so dragging a new wire from the same output replaces the old edge.
- Updated validation feedback so the right-side Ghost reaction describes success or common failure types based on validator errors.
- Reduced the bottom validation strip and widened the central reply-map area in the scene builder.
- Updated walkthrough and Unity checklist.

## What Was Intentionally Not Changed

- Act 3 pure logic/session files under `Assets/Scripts/Puzzles/DialogGraph`.
- Act 1 and Act 2 scripts.
- Game Shell scripts or act list.
- ProjectSettings, Packages, Build Settings, `.meta` files, and unrelated scenes.
- Backend, LLM, save/load, scoring persistence, final art, Act 3 Shell integration, and Act 4-6 node graph scope.

## Remaining Risks

- Human Unity Play Mode verification is required for palette drag/drop feel, trash drop detection, line click/Delete behaviour, rewiring replacement, and Ghost reaction readability.
- The user must rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene` to refresh the generated scene.
- The worktree currently includes Unity/editor side effects and previous untracked files; these need careful review before commit.

## Next Recommended Step

Rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene` in Unity, open `Assets/Scenes/Act3DialogGraphPrototype.unity`, and complete the updated M0-T30 Play Mode checklist for palette drag placement, dot-port wiring, trash deletion, wire Delete/rewire behaviour, and Ghost outcome feedback.
