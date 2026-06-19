# M0-T07 - Run 002 - static ui display fix

## Task ID

M0-T07

## Run Number

002

## Date

2026-06-19

## Original Request / Codex Prompt Summary

Fix the static Act 1 intent-classification prototype UI where the intent group areas display correctly but the left-side sample message cards appear as blank pale rectangles. Keep the UI static and do not implement drag-and-drop, validation, scoring, Build Settings changes, ProjectSettings changes, Packages changes, pure logic changes, or unrelated systems.

## Files Created

- `Docs/codex_runs/M0-T07_002_static_ui_display_fix.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/Editor/Act1IntentClassificationPrototypeSceneBuilder.cs`
- `Assets/Presentation/Act1IntentClassification/README.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read `Docs/CURRENT_TASK.md`.
- Read `Docs/codex_runs/M0-T07_001_static_act1_ui_scene.md`.
- Read the static presenter, scene builder, setup README, and Act 1 sample data files.
- Checked that `Assets/Scenes/Act1IntentClassificationPrototype.unity` exists.
- Inspected repository status.

## Test / Check Result

Not run — Unity Editor Play Mode was not executed in this Codex session.

Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- The generated UI used an over-tall card template for nine cards in one vertical list. The card backgrounds could still render, but the card text rows could be compressed or clipped by Unity layout, making the cards look blank.

## Fixes Applied

- Made the presenter configure each instantiated card with a compact fixed layout height.
- Made the presenter configure the message text with an explicit built-in font, dark contrast color, wrapping, and minimum/preferred layout height.
- Hid the optional card id and intent hint rows on cards so the message text has enough vertical space.
- Made the presenter force a layout rebuild after rendering sample cards and intent group areas.
- Updated the scene builder to generate compact card templates with one visible message text row.
- Updated setup docs and checklist with scene refresh steps.

## What Was Intentionally Not Changed

- No drag-and-drop interaction was implemented.
- No validation button was implemented.
- No scoring was implemented.
- No save/load, animation, Act 0, Act 2, backend, LLM, dialogue system, or final art was implemented.
- No pure logic files under `Assets/Scripts/Puzzles/IntentClassification/` were edited.
- No ProjectSettings files were edited by Codex.
- No Packages files were edited by Codex.
- `Assets/Scenes/SampleScene.unity` was not edited by Codex.
- `Docs/CURRENT_TASK.md`, `Docs/LEARNING_CONTENT.md`, and `Docs/completed_tasks/` were not edited.

## Remaining Risks

- The existing generated scene may need to be rebuilt with `Ghost > Build Act 1 Intent Classification Prototype Scene` so the saved scene uses the compact card template.
- Play Mode has not been run by Codex after this display fix.
- The working tree may include Unity-generated scene, `.meta`, or ProjectSettings files from external Unity Editor generation; Codex did not modify them in this run.

## Next Recommended Step

In Unity, run `Ghost > Build Act 1 Intent Classification Prototype Scene`, open `Assets/Scenes/Act1IntentClassificationPrototype.unity`, and confirm all nine message card texts are visible beside the three intent group areas.
