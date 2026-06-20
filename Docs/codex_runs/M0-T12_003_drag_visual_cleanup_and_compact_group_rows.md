# M0-T12 - Run 003 - drag visual cleanup and compact group rows

## Task ID

M0-T12

## Run Number

003

## Date

2026-06-20

## Original Request / Codex Prompt Summary

Fix drag-and-drop visual/state bugs and refine the group-area presentation without changing puzzle logic. The drag interaction should have only one solid card-like preview, clean it reliably on successful and failed drops, keep group-based assignment rather than coordinate-based placement, render right-side assigned cards as compact readable rows/chips, and preserve right-to-left unassign, right-to-right reassign, click-to-assign, Back/unassign, and Validate feedback.

## Files Created

- `Docs/codex_runs/M0-T12_003_drag_visual_cleanup_and_compact_group_rows.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationDraggableCard.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationDropTarget.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/README.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read the requested M0-T12 task/run/documentation context and current presentation scripts.
- Inspected scene wiring context with `rg` against `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
- Inspected drag/drop lifecycle and documentation references with `rg`.
- Ran `git -c safe.directory='C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost' diff --check -- ...` scoped to touched presentation and documentation files.
- Ran full `git -c safe.directory='C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost' diff --check`.
- Checked changed-file scope with `git -c safe.directory='C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost' status --short --untracked-files=all`.

## Test / Check Result

- Source inspection showed drag/drop remains in presentation scripts and assign/unassign/reassign callbacks still route through `Act1IntentClassificationInteractionController`.
- Scoped `git diff --check` for touched presentation/documentation files passed.
- Full `git diff --check` still reported trailing whitespace in `Assets/Scenes/Act1IntentClassificationPrototype.unity`; the generated scene file was already modified in the working tree and was not manually edited in this Codex run.
- Not run — Unity Editor Play Mode was not executed in this Codex session.
- Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- The likely cause of drag afterimages was that successful drops can trigger a UI re-render that destroys the dragged assigned-row source before Unity sends `OnEndDrag`, while preview cleanup was tied primarily to the source's end-drag callback.
- Run 002 also allowed each source to own its preview independently, so a stale preview could survive if the source was destroyed during assignment-state refresh.
- The working tree already contained generated/out-of-scope changes: `Assets/Scenes/Act1IntentClassificationPrototype.unity`, `ProjectSettings/ShaderGraphSettings.asset`, Unity-generated `.meta` files for run 001 scripts, and earlier untracked M0-T12 run logs.
- Full `git diff --check` failed on trailing whitespace inside the modified generated scene YAML.
- Unity Editor Play Mode and Unity Test Runner were not executed in this Codex session.

## Fixes Applied

- Added a single static active drag source and active drag preview to `Act1IntentClassificationDraggableCard`.
- Added `CancelActiveDrag()` so starting a new drag cleans any previous active preview.
- Added `CompleteDragVisuals()` so drop targets can clean the preview before they invoke assignment/unassignment callbacks that may re-render and destroy the source row.
- Added `OnDisable()` cleanup so a source row destroyed during UI re-render still clears its active preview.
- Hid preview objects immediately before scheduling destruction, avoiding visible one-frame leftovers while Unity defers `Destroy`.
- Updated `Act1IntentClassificationDropTarget` to capture the card id, call `CompleteDragVisuals()`, then route the drop to the existing presenter/controller callback.
- Kept right-side group drops group-based: a successful drop calls controller/session assignment and the normal assigned-card list re-renders the row.
- Made right-side assigned rows more compact with shorter height, smaller text, tighter padding, and a soft outline/color treatment.
- Updated documentation and manual Unity checklist to include stale `Drag Preview` hierarchy checks and the compact group-list behaviour.

## What Was Intentionally Not Changed

- Pure puzzle logic under `Assets/Scripts/Puzzles/IntentClassification/`.
- `Assets/Scripts/Ghost.Runtime.asmdef`.
- EditMode tests under `Assets/Tests/EditMode/`.
- `ProjectSettings/`; the existing `ProjectSettings/ShaderGraphSettings.asset` working-tree change was left untouched.
- `Packages/`.
- Build Settings.
- `Assets/Scenes/SampleScene.unity`.
- `Assets/Scenes/Act1IntentClassificationPrototype.unity`; the existing generated scene working-tree change was left untouched and no scene YAML was hand-edited.
- Coordinate-based free placement, group ordering/reordering, scoring, save/load, final art, Act 0, Act 2, backend, LLM, dialogue system, and mobile/touch polish.

## Remaining Risks

- Unity compile was not verified in the Editor during this session.
- Play Mode drag/drop behaviour was not manually verified in the Editor during this session.
- The generated prototype scene currently has working-tree changes and trailing whitespace from Unity serialization; these were not edited manually.
- The UI relies on an active `EventSystem`, `InputSystemUIInputModule`, and `GraphicRaycaster`; the existing presenter/builder create these, but stale scenes may need a menu-builder refresh.

## Next Recommended Step

Open Unity, wait for import/compile, open `Assets/Scenes/Act1IntentClassificationPrototype.unity`, enter Play Mode, and run the M0-T12 checklist: verify one solid preview per drag, no stale `Drag Preview` objects after successful/cancelled drops, group-based assignment into the normal list, compact assigned rows, right-to-left unassign, right-to-right reassign, click-to-assign, Back/unassign, and Validate feedback.
