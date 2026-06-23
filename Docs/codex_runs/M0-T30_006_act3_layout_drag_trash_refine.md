# M0-T30 — Run 006 — act3 layout drag trash refine

## Task ID

M0-T30

## Run Number

006

## Date

2026-06-23

## Original Request / Codex Prompt Summary

Refine the Act 3 node-graph UX after user Play Mode review: let node cards visibly move outside the graph board toward the trash zone, shrink the left palette column by half, keep the right guide readable at its restored width/text size, reduce the bottom Test Ghost's map strip height, restore bottom/guide copy sizes, and keep existing deletion/validation behavior intact.

## Files Created

- `Docs/codex_runs/M0-T30_006_act3_layout_drag_trash_refine.md`

## Files Modified

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
- `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `git diff --check -- Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `Select-String` checks for stale compact UI copy and stale documentation references.

## Test / Check Result

- `git diff --check`: Passed with only Git LF/CRLF working-copy warnings.
- Stale compact UI/documentation references checked and updated where needed.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.
- Scene generation: Not run — Unity Editor menu builder was not executed in this Codex session.

## Errors Encountered

- Initial patch context did not match one presenter method, so the change was split into smaller targeted patches.
- User review showed the prior compact layout over-shrunk right-side text and still kept placed nodes visually constrained inside the graph board.

## Fixes Applied

- Expanded presentation controller node-position bounds so placed cards can move slightly outside the graph board, including down toward the bottom trash zone.
- Added a nested canvas and graphic raycaster to the node layer so dragged cards render above the bottom control strip while remaining interactive.
- Forced the validation controls root to a compact fixed height during presenter render, so old generated scenes are corrected at runtime.
- Changed the builder to use a half-width palette column, a larger flexible graph column, and a restored readable guide column.
- Restored guide/legend/goal/Ghost reaction text sizes and fuller copy, relying on wrapping rather than tiny text.
- Restored the bottom button label to `Test Ghost's map`, kept the bottom strip compact, and widened the trash label to `X drop card`.
- Updated the walkthrough and checklist to match the latest layout and interaction expectations.

## What Was Intentionally Not Changed

- Act 3 pure logic/session files under `Assets/Scripts/Puzzles/DialogGraph/`.
- Act 1, Act 2, Game Shell, backend, LLM, save/load, scoring persistence, Build Settings, ProjectSettings, Packages, `.meta` files, and `Docs/CURRENT_TASK.md`.
- Existing generated scenes were not hand-edited; the user should rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene` if the saved scene is stale.

## Remaining Risks

- Unity Play Mode was not run in this session, so the exact visual proportions, node drag outside-board feel, nested-canvas raycast behavior, and trash hover behavior still need human Editor verification.
- The working tree contains pre-existing unrelated scene, ProjectSettings, and documentation changes; those should be reviewed separately before commit staging.

## Next Recommended Step

Rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene`, open `Assets/Scenes/Act3DialogGraphPrototype.unity`, verify the palette/guide/bottom proportions, drag a node outside the board onto `X drop card`, test Delete/Backspace removal for nodes and wires, and press `Test Ghost's map` on wrong and correct graphs.
