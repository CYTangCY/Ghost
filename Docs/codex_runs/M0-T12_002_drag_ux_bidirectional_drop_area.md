# M0-T12 - Run 002 - drag UX bidirectional drop area

## Task ID

M0-T12

## Run Number

002

## Date

2026-06-20

## Original Request / Codex Prompt Summary

Improve the M0-T12 drag-and-drop UX without changing puzzle logic or adding new gameplay systems. The drag visual should feel like a solid card block rather than a faded preview, intent group drop zones should cover the group area more reliably, assigned cards should be draggable back to the left unassigned/message-card area, assigned cards should be draggable to another group if low-risk, assigned rows should be more compact, and existing click-to-assign, Back/unassign, scrollable lists, and Validate feedback should remain intact.

## Files Created

- `Docs/codex_runs/M0-T12_002_drag_ux_bidirectional_drop_area.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationDraggableCard.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationDropTarget.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/README.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read the requested M0-T12 task/run/documentation context and current presentation scripts.
- Inspected drag/drop wiring with `rg`.
- Ran `git -c safe.directory='C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost' diff --check`.
- Ran `git -c safe.directory='C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost' diff --check -- ...` scoped to the touched presentation and documentation files.
- Checked changed-file scope with `git -c safe.directory='C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost' status --short --untracked-files=all`.

## Test / Check Result

- Source inspection showed drag/drop remains in presentation scripts and assignments/unassignments route through `Act1IntentClassificationInteractionController`.
- Scoped `git diff --check` for touched presentation/documentation files passed.
- Full `git diff --check` reported trailing whitespace in `Assets/Scenes/Act1IntentClassificationPrototype.unity`; that generated scene file was already modified in the working tree and was not manually edited in this Codex run.
- Not run — Unity Editor Play Mode was not executed in this Codex session.
- Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- The working tree already contained modified generated Unity files outside this run's edit scope: `Assets/Scenes/Act1IntentClassificationPrototype.unity`, `ProjectSettings/ShaderGraphSettings.asset`, and Unity-generated `.meta` files for the run 001 scripts.
- Full `git diff --check` failed on trailing whitespace inside the modified generated scene YAML.
- Unity Editor Play Mode and Unity Test Runner were not executed in this Codex session.

## Fixes Applied

- Changed the drag preview to an opaque card-like visual while fading only the source placeholder.
- Extended the drop target component to support both intent-group targets and an unassigned target.
- Attached an unassigned drop target to the left message-card list.
- Attached intent drop targets to group panels and their assigned-card scroll viewports so drops inside the broader group area are accepted.
- Made right-side assigned rows draggable.
- Allowed assigned rows to be dragged back to the left message-card list through the controller/session unassign path.
- Allowed assigned rows to be dragged to a different intent group through the controller/session assignment path.
- Ignored drops onto the card's current group to avoid accidental same-group reordering.
- Reduced assigned row height, padding, and font size for a more compact group list.
- Updated documentation and manual Unity checklist for the run 002 drag UX.

## What Was Intentionally Not Changed

- Pure puzzle logic under `Assets/Scripts/Puzzles/IntentClassification/`.
- `Assets/Scripts/Ghost.Runtime.asmdef`.
- EditMode tests under `Assets/Tests/EditMode/`.
- `ProjectSettings/`; the existing `ProjectSettings/ShaderGraphSettings.asset` working-tree change was left untouched.
- `Packages/`.
- Build Settings.
- `Assets/Scenes/SampleScene.unity`.
- `Assets/Scenes/Act1IntentClassificationPrototype.unity`; the existing scene working-tree change was left untouched and no scene YAML was hand-edited.
- Scoring, save/load, final art, Act 0, Act 2, backend, LLM, dialogue system, mobile/touch polish, position-based free placement, group ordering/reordering, and animation beyond minimal drag visual feedback.

## Remaining Risks

- Unity compile was not verified in the Editor during this session.
- Play Mode drag/drop behaviour was not manually verified in the Editor during this session.
- The generated scene currently has working-tree changes and trailing whitespace from Unity serialization; these were not edited manually.
- The UI relies on an active `EventSystem`, `InputSystemUIInputModule`, and `GraphicRaycaster`; the existing presenter/builder create these, but stale scenes may need a menu-builder refresh.

## Next Recommended Step

Open Unity, wait for import/compile, open `Assets/Scenes/Act1IntentClassificationPrototype.unity`, enter Play Mode, and run the M0-T12 checklist: verify solid drag previews, drop-anywhere group assignment, dragging assigned rows back to the left message-card list, dragging assigned rows to another group, click-to-assign, Back/unassign, Validate feedback, and compact scrollable assigned lists.
