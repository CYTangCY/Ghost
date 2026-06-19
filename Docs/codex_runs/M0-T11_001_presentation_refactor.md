# M0-T11 - Run 001 - presentation refactor

## Task ID

M0-T11

## Run Number

001

## Date

2026-06-19

## Original Request / Codex Prompt Summary

Refactor the Act 1 presentation layer before drag-and-drop without changing visible M0-T09 behaviour. Add an explicit presentation assembly boundary, extract session/interaction orchestration from `Act1IntentClassificationStaticPresenter` into a small controller, keep the presenter focused on rendering and UI event wiring, update documentation, and create this run log. Do not edit pure logic files, tests, ProjectSettings, Packages, Build Settings, SampleScene, or completed task records.

## Files Created

- `Assets/Presentation/Ghost.Presentation.asmdef`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
- `Assets/Presentation/Act1IntentClassification/Editor/Ghost.Presentation.Editor.asmdef`
- `Docs/codex_runs/M0-T11_001_presentation_refactor.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read the required project context, current task, architecture, learning-content, walkthrough, checklist, run-log convention, M0-T10 completed review, current presentation files, existing asmdefs, and package assembly names.
- Checked package assembly names for `UnityEngine.UI` and `Unity.InputSystem` before adding presentation asmdef references.
- Searched the presentation files and docs for interaction/state ownership, drag-and-drop, scoring, save/load, backend, LLM, and dialogue terms.
- Reviewed the presenter diff to confirm session ownership, selected-card state, assignment, unassignment, and validation moved out of the presenter.
- Checked line counts for the presenter and new controller.
- Checked scoped git status for allowed and disallowed paths.
- Ran `git diff --check` on the M0-T11 files.

## Test / Check Result

- `Act1IntentClassificationStaticPresenter` no longer owns `IntentClassificationSession`, selected-card mutation, assignment, unassignment, or validation.
- `Act1IntentClassificationInteractionController` owns/coordinaties the session, selected card id, select/deselect, assign, unassign, validate, and state/feedback callbacks.
- `Ghost.Presentation.asmdef` and `Ghost.Presentation.Editor.asmdef` were added.
- The scene builder did not need a code change because the same presenter script remains attached and creates the controller internally.
- `git diff --check` reported no whitespace errors. Git printed line-ending normalization warnings for existing Windows workspace behavior.
- Not run — Unity Editor Play Mode was not executed in this Codex session.
- Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- No file-editing errors were encountered.
- Unity Play Mode and Unity Test Runner were not executed from Codex.

## Fixes Applied

- Added `Act1IntentClassificationInteractionController` as a plain C# presentation controller.
- Added neutral/correct/incorrect feedback value types in the controller file.
- Updated the presenter to create the controller, subscribe to state/feedback callbacks, render from controller state, and forward UI clicks to the controller.
- Added a runtime presentation asmdef referencing `Ghost.Runtime`, `UnityEngine.UI`, and `Unity.InputSystem`.
- Added an Editor presentation asmdef for the scene builder referencing `Ghost.Presentation`, `UnityEngine.UI`, and `Unity.InputSystem`.
- Updated `Docs/CODE_WALKTHROUGH.md` with the controller and assembly boundary.
- Updated `Docs/UNITY_TEST_CHECKLIST.md` with M0-T11 import, Play Mode regression, and EditMode test checklist steps.

## What Was Intentionally Not Changed

- No drag-and-drop interaction was implemented.
- No visible M0-T09 behaviour was intentionally changed.
- No new gameplay features, puzzle rules, sample data, validation logic, final visual design, scoring, animations, Act 0, Act 2, backend, LLM, dialogue system, or save/load work was implemented.
- No pure logic files under `Assets/Scripts/Puzzles/IntentClassification/` were edited.
- `Assets/Scripts/Ghost.Runtime.asmdef` was not edited.
- No EditMode tests were edited.
- No ProjectSettings, Packages, Build Settings, SampleScene, `.meta` files, or completed task records were manually edited.
- `Assets/Scenes/Act1IntentClassificationPrototype.unity` was not hand-edited or regenerated in this Codex session.

## Remaining Risks

- Unity still needs to import and compile the new presentation assembly definitions.
- Existing EditMode tests still need to be run in Unity to verify the new assembly boundary did not disturb test compilation.
- Play Mode still needs to verify that M0-T09 behaviour remains unchanged.
- If Unity shows stale scene layout or script references after import, the scene may need to be refreshed with `Ghost > Build Act 1 Intent Classification Prototype Scene`.

## Next Recommended Step

Open Unity, wait for the new presentation asmdefs to import and compile, run the existing EditMode tests, then open `Assets/Scenes/Act1IntentClassificationPrototype.unity` and follow the M0-T11 regression checklist in `Docs/UNITY_TEST_CHECKLIST.md`.
