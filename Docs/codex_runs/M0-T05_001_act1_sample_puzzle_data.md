# M0-T05 - Run 001 - act1 sample puzzle data

## Task ID

M0-T05

## Run Number

001

## Date

2026-06-19

## Original Request / Codex Prompt Summary

Define Act 1 sample puzzle data for the intent-classification puzzle without creating UI or scenes. Reuse the M0-T04 `IntentCard` and `IntentClassificationValidator` where possible, add sample messages grouped by purpose, add EditMode tests for the sample data, update documentation, and create this run log.

## Files Created

- `Assets/Scripts/Puzzles/IntentClassification/Act1IntentClassificationSampleData.cs`
- `Assets/Tests/EditMode/Act1IntentClassificationSampleDataTests.cs`
- `Docs/codex_runs/M0-T05_001_act1_sample_puzzle_data.md`

## Files Modified

- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read required project context, task, walkthrough, checklist, and M0-T04 run-log files before implementation.
- Checked repository status before implementation.
- Added EditMode tests for the Act 1 sample data.
- Checked final file/status output for accidental scene, ProjectSettings, Packages, and `.meta` changes.

## Test / Check Result

Not run — Unity Editor test runner was not executed in this Codex session.

Unity EditMode tests were not run because this session avoided opening/importing the Unity project after the prior run showed Unity could generate `.meta` files and ProjectSettings side effects. The added tests are ready to run in Unity's EditMode Test Runner.

## Errors Encountered

- None during file editing.

## Fixes Applied

- Not applicable.

## What Was Intentionally Not Changed

- No scenes were edited.
- No ProjectSettings files were edited.
- No Packages files were edited.
- No `.meta` files were manually edited.
- No UI, prefab, art asset, drag-and-drop UI, scene setup, Act 0, Act 2, LLM/backend integration, dialogue system, or save system work was implemented.
- `Docs/CURRENT_TASK.md`, `Docs/LEARNING_CONTENT.md`, and `Docs/completed_tasks/` were not edited.

## Remaining Risks

- Unity has not yet imported or compiled the new M0-T05 sample data and tests in a verified Editor test run.
- Unity may generate `.meta` files automatically when the project is opened or when tests are run.

## Next Recommended Step

Open the project in Unity, let it import the new files, then run the EditMode tests in `Ghost.EditModeTests`.
