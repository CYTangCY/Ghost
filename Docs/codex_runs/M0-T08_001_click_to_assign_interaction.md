# M0-T08 - Run 001 - click-to-assign interaction

## Task ID

M0-T08

## Run Number

001

## Date

2026-06-19

## Original Request / Codex Prompt Summary

Implement basic click-to-assign interaction for the Act 1 intent-classification prototype scene without drag-and-drop. The required flow is: click a sample message card to select it, click one of the three intent group areas to assign it, and visibly reflect both selection and assignment. Keep the pure puzzle logic unchanged and do not edit ProjectSettings, Packages, Build Settings, SampleScene, or unrelated systems.

## Files Created

- `Docs/codex_runs/M0-T08_001_click_to_assign_interaction.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/Editor/Act1IntentClassificationPrototypeSceneBuilder.cs`
- `Assets/Presentation/Act1IntentClassification/README.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Searched the Act 1 presentation code for drag-and-drop, validation, scoring, save/load, and related out-of-scope terms.
- Reviewed the modified presenter and scene builder source files.
- Checked relevant git status with an explicit safe-directory override.
- Ran `git diff --check` on the modified M0-T08 files.

## Test / Check Result

- The presentation code contains click button wiring for cards and intent group areas, and no drag-and-drop implementation was added.
- The scene builder now creates an `EventSystem` with `InputSystemUIInputModule` for UI clicks.
- `git diff --check` reported no whitespace errors. Git printed line-ending normalization warnings for existing Windows workspace behavior.
- Not run — Unity Editor Play Mode was not executed in this Codex session.
- Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- A plain `git status` command failed because Git reported dubious repository ownership for this workspace path.
- Unity Play Mode and Unity Test Runner were not executed from Codex.

## Fixes Applied

- Used `git -c safe.directory='C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost' ...` for the relevant status check without changing global Git configuration.
- Extended `Act1IntentClassificationStaticPresenter` to create a fresh `IntentClassificationSession`, select cards on click, assign selected cards through `MoveCardToGroup(...)`, highlight selected and assigned cards, and list assigned card text inside each intent group.
- Extended the scene builder to create an `EventSystem` with `InputSystemUIInputModule` and to include an assignment-list root in intent group templates.
- Updated documentation and manual test checklist for the M0-T08 click-to-assign flow.

## What Was Intentionally Not Changed

- No drag-and-drop interaction was implemented.
- No validation button, scoring, save/load, animation, backend, LLM, dialogue system, Act 0, Act 2, or final art work was implemented.
- No pure logic files under `Assets/Scripts/Puzzles/IntentClassification/` were edited.
- No ProjectSettings, Packages, Build Settings, SampleScene, prefabs, art assets, or `.meta` files were manually edited.
- `Assets/Scenes/Act1IntentClassificationPrototype.unity` was not hand-edited in Codex.

## Remaining Risks

- The updated scripts still need Unity import/compile verification.
- The existing generated scene may need to be refreshed with `Ghost > Build Act 1 Intent Classification Prototype Scene` so the saved scene contains the M0-T08 EventSystem and assignment-list template.
- Play Mode must still be run in Unity to confirm clicks, highlights, assigned-card lists, and Console cleanliness.

## Next Recommended Step

Open Unity, allow scripts to compile, run `Ghost > Build Act 1 Intent Classification Prototype Scene` if the existing scene does not respond to clicks, then enter Play Mode and follow the M0-T08 checklist in `Docs/UNITY_TEST_CHECKLIST.md`.
