# M0-T12 - Run 001 - minimal drag to assign

## Task ID

M0-T12

## Run Number

001

## Date

2026-06-20

## Original Request / Codex Prompt Summary

Add the smallest safe drag-to-assign layer for the Act 1 intent-classification prototype. The drag/drop UI must reuse the existing presentation controller and pure `IntentClassificationSession` flow, keep click-to-assign, Back/unassign, and Validate feedback working, avoid drag/drop changes in pure logic, and avoid ProjectSettings, Packages, Build Settings, SampleScene, tests, and final-art changes.

## Files Created

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationDraggableCard.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationDropTarget.cs`
- `Docs/codex_runs/M0-T12_001_minimal_drag_to_assign.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/README.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read the required project/task context and existing presentation scripts.
- Inspected drag/drop wiring with `rg`.
- Checked changed-file scope with `git -c safe.directory='C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost' status --short --untracked-files=all`.

## Test / Check Result

- Source inspection showed the drag scripts are presentation-only and the presenter routes drops through `Act1IntentClassificationInteractionController`.
- Changed-file scope was limited to allowed presentation files and documentation before this run log was added.
- Not run — Unity Editor Play Mode was not executed in this Codex session.
- Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- A plain `git status` command initially failed because Git reported the repository as having dubious ownership for the current OS user.
- Unity Editor Play Mode and Unity Test Runner were not executed in this Codex session.

## Fixes Applied

- Used a one-off `git -c safe.directory=... status` command for repository status inspection without changing global Git configuration.
- Added `Act1IntentClassificationDraggableCard` to create a temporary drag preview, fade the source card, and disable source-card raycast blocking during drag.
- Added `Act1IntentClassificationDropTarget` to detect dropped cards on intent group areas and forward card id plus intent id to the presenter.
- Added `AssignCardToIntent` to the presentation interaction controller so drag assignment can assign a specific card through the same controller/session path without toggling selection.
- Updated the presenter to attach draggable behaviours to rendered cards and drop targets to rendered group areas.
- Updated documentation and manual Unity checklist for M0-T12.

## What Was Intentionally Not Changed

- Pure puzzle logic under `Assets/Scripts/Puzzles/IntentClassification/`.
- `Assets/Scripts/Ghost.Runtime.asmdef`.
- EditMode tests under `Assets/Tests/EditMode/`.
- `ProjectSettings/`.
- `Packages/`.
- Build Settings.
- `Assets/Scenes/SampleScene.unity`.
- `Assets/Scenes/Act1IntentClassificationPrototype.unity`; the scene was not regenerated in this Codex session.
- Drag reordering inside groups, scoring, save/load, final art, Act 0, Act 2, backend, LLM, dialogue system, mobile/touch polish, and animation beyond minimal drag visual feedback.

## Remaining Risks

- Unity compile was not verified in the Editor during this session.
- Play Mode drag/drop behaviour was not manually verified in the Editor during this session.
- The UI relies on an active `EventSystem`, `InputSystemUIInputModule`, and `GraphicRaycaster`; the existing presenter/builder create these, but stale scenes may need a menu-builder refresh.

## Next Recommended Step

Open Unity, wait for import/compile, open `Assets/Scenes/Act1IntentClassificationPrototype.unity`, enter Play Mode, and run the M0-T12 checklist: verify click-to-assign, Back/unassign, Validate feedback, dragging cards onto each intent group, dropping outside groups, and scrollable assigned lists.
