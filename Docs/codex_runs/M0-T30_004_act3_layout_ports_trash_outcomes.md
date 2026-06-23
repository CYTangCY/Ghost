# M0-T30 — Run 004 — Act 3 layout, ports, trash, and outcome refinements

## Task ID

M0-T30

## Run Number

004

## Date

2026-06-23

## Original Request / Codex Prompt Summary

User feedback after the M0-T30 UI redesign: the right Guide panel is too wide, card connection dots should sit on card edges, dragging a card to the trash zone does not remove it, the bottom Test Ghost's map area is too large, and failed validation needs different Ghost reactions based on different wrong links instead of one generic failure.

## Files Created

- `Docs/codex_runs/M0-T30_004_act3_layout_ports_trash_outcomes.md`

## Files Modified

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphNodeDragView.cs`
- `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Reviewed active task and project context docs required by the workflow.
- Inspected current Act 3 presenter, controller, node drag view, editor scene builder, and M0-T30 docs.
- `dotnet build Ghost.Presentation.csproj --no-restore`
- `git diff --check -- Assets\Presentation\Act3DialogGraph\Act3DialogGraphStaticPresenter.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphNodeDragView.cs Assets\Presentation\Act3DialogGraph\Editor\Act3DialogGraphPrototypeSceneBuilder.cs Docs\CODE_WALKTHROUGH.md Docs\UNITY_TEST_CHECKLIST.md Docs\codex_runs\M0-T30_004_act3_layout_ports_trash_outcomes.md`
- Trailing-whitespace `Select-String` scan for the changed Act 3 UI files, docs, and run log.
- `git diff -- Assets\Scripts\Puzzles\DialogGraph`
- `rg "EditorBuildSettings|BuildSettings|UnityWebRequest|OpenAI|LLM|backend" Assets\Presentation\Act3DialogGraph`
- Unity scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `dotnet build Ghost.Presentation.csproj --no-restore`: passed with 5 existing obsolete `FindFirstObjectByType` warnings and 0 errors.
- Final `dotnet build Ghost.Presentation.csproj --no-restore`: passed with 0 warnings and 0 errors.
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

- Changed the Act 3 editor builder to use fixed narrow palette/guide columns and a flexible wide graph-board column.
- Reduced the generated bottom validation strip height and shortened the validate button label to `Test map`.
- Moved node input/output ports to anchored edge dots on card borders instead of layout rows inside the card body.
- Made trash deletion less fragile by checking both pointer-over-trash and card-overlaps-trash when node drag ends.
- Enlarged the trash drop zone slightly.
- Shortened right-side guide/test/reaction panel text blocks to fit the narrower helper column.
- Added route-specific Ghost outcome text for common wrong links: no start, missing request/check/response cards, skipping the room check, green room-known wire going to ask, orange room-missing wire going to answer, and missing green/orange routes.
- Updated CODE_WALKTHROUGH and UNITY_TEST_CHECKLIST for the refined M0-T30 UI behaviours.

## What Was Intentionally Not Changed

- Act 3 pure logic/session files under `Assets/Scripts/Puzzles/DialogGraph`.
- Act 1 and Act 2 scripts.
- Game Shell scripts or act list.
- ProjectSettings, Packages, Build Settings, `.meta` files, and unrelated scenes.
- Backend, LLM, save/load, scoring persistence, final art, Act 3 Shell integration, and Act 4-6 node graph scope.

## Remaining Risks

- Human Unity Play Mode verification is required for the new layout proportions, edge-port hit targets, trash deletion feel, and route-specific Ghost reaction readability.
- The user must rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene` to regenerate `Assets/Scenes/Act3DialogGraphPrototype.unity` with the updated builder proportions.
- The worktree already contains Unity/editor side effects and prior untracked M0-T30 files; review `git status` carefully before committing.

## Next Recommended Step

Rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene` in Unity, open `Assets/Scenes/Act3DialogGraphPrototype.unity`, and complete the updated M0-T30 Play Mode checklist focusing on guide width, edge ports, trash deletion, compact `Test map` strip, and distinct failure reactions.
