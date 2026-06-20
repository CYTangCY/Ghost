# M0-T13 - Run 001 - visual instruction polish

## Task ID

M0-T13

## Run Number

001

## Date

2026-06-20

## Original Request / Codex Prompt Summary

Polish the Act 1 intent-classification prototype's visual hierarchy and player-facing instructional clarity without changing the existing mechanics, puzzle rules, sample data, validation logic, pure logic files, tests, scene build settings, or final-art scope.

## Files Created

- `Docs/codex_runs/M0-T13_001_visual_instruction_polish.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/README.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `rg` checks over the Act 1 presentation files and docs for the new M0-T13 instruction, label, and validation feedback text.
- `git -c safe.directory=C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost diff --check -- Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs Assets/Presentation/Act1IntentClassification/README.md Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `git -c safe.directory=C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost status --short --untracked-files=all`

## Test / Check Result

- `rg` checks found the expected updated instruction labels, validation feedback wording, and M0-T13 documentation entries.
- `git diff --check` completed with no whitespace errors. Git reported line-ending normalization warnings only.
- `git status` showed only the intended modified files and the new run log after it was created.
- Unity Play Mode:
  Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity EditMode tests:
  Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- A plain `git status` initially failed because Git detected dubious ownership for the workspace path under the current Windows user.

## Fixes Applied

- Re-ran Git checks with a per-command `safe.directory` override. No global Git configuration was changed.
- Updated player-facing feedback text to more clearly explain intent grouping and correction.
- Added runtime presenter polish for title/subtitle copy, column labels, softer panel/card/group colors, outlines, selected-card cues, ready group cues, validation panel styling, and clearer Validate button wording.
- Updated the presentation README, code walkthrough, and Unity test checklist for M0-T13.

## What Was Intentionally Not Changed

- No pure puzzle logic files under `Assets/Scripts/Puzzles/IntentClassification/` were edited.
- No EditMode tests were edited.
- No scene asset was regenerated or manually edited.
- No ProjectSettings, Packages, Build Settings, SampleScene, `.meta` files, UI prefab, art asset, final art, scoring, Act 0, Act 2, backend, LLM, dialogue, save/load, coordinate placement, or group ordering/reordering changes were made.
- Existing mechanics were intentionally preserved: click-to-assign, drag-to-assign, drag assigned card back to unassigned, drag assigned card between groups, Back/unassign, Validate, validation feedback, and group-wide drop semantics.

## Remaining Risks

- Unity compile and Play Mode behaviour still need to be verified in the Unity Editor.
- Because the scene was not regenerated, the saved scene preview may show older labels until Play Mode runs the presenter. If a refreshed saved preview is desired, use the existing Unity menu builder.
- Some UI construction remains duplicated between the presenter and scene builder from earlier prototype work, so future visual changes may still need care to avoid drift.

## Next Recommended Step

Open `Assets/Scenes/Act1IntentClassificationPrototype.unity` in Unity, enter Play Mode, and run the M0-T13 checklist in `Docs/UNITY_TEST_CHECKLIST.md`.
