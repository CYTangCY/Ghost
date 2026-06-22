# M0-T18 — Run 001 — Act 2 validation feedback

## Task ID

M0-T18

## Run Number

001

## Date

2026-06-22

## Original Request / Codex Prompt Summary

Implement only the Act 2 validation-feedback slice: enable the Act 2 `Validate spans` button, route its click through `Act2EntityExtractionInteractionController` to `EntityExtractionSession.ValidateCurrentState()`, and display correct/incorrect feedback in the presenter. Keep M0-T17 chip selection/tagging/untagging intact and do not add scoring persistence, backend, LLM, dialogue, node graph, multi-chip spans, later Act work, or final art.

## Files Created

- `Docs/codex_runs/M0-T18_001_act2_validation_feedback.md`

## Files Modified

- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `git diff --check -- Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `rg "ValidateCurrentState|EntityExtractionValidator|Validate\(" Assets/Presentation/Act2EntityExtraction`
- `rg "disabled placeholder|correctness feedback is not wired|validation is not wired|placeholder only" Assets/Presentation/Act2EntityExtraction Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Scene generation: Not run — Unity Editor menu builder was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- M0-T18 edited files had no whitespace errors; Git emitted CRLF normalization warnings only.
- Validation-call search found only the presenter calling `controller.ValidateCurrentState()` and the controller calling `session.ValidateCurrentState()`.
- Stale placeholder-copy search returned no matches for the old disabled/unwired validation wording.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Scene generation: Not run — Unity Editor menu builder was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

None during this Codex run.

## Fixes Applied

- Added `FeedbackChanged(string message, bool isCorrect)` to `Act2EntityExtractionInteractionController`.
- Added `ValidateCurrentState()` to the controller. It calls `EntityExtractionSession.ValidateCurrentState()`, builds feedback from `IsCorrect` and `Errors.Count`, raises `FeedbackChanged`, and returns the raw result.
- Updated `Act2EntityExtractionStaticPresenter` to subscribe/unsubscribe feedback events, enable the Validate button, call the controller on click, and color feedback green for correct or warm red for incorrect.
- Updated Act 2 instruction/feedback copy so validation is no longer described as unwired.
- Updated CODE_WALKTHROUGH and UNITY_TEST_CHECKLIST for M0-T18.

## What Was Intentionally Not Changed

- No Act 2 pure logic files were modified.
- No Act 1 scripts, Game Shell scripts, existing scenes, ProjectSettings, Packages, Build Settings, asmdefs, or `.meta` files were modified by this task.
- No scoring persistence, save/load, backend, LLM, dialogue, node graph, multi-chip spans, later-Act behaviour, or final art was added.
- Existing unrelated dirty scene and ProjectSettings files in the worktree were not reverted or edited.

## Remaining Risks

- The validation interaction needs manual Unity Play Mode verification because Unity was not run in this shell session.
- If the saved Act 2 scene preview looks stale, the user must rerun `Ghost > Build Act 2 Entity Extraction Prototype Scene`.
- The feedback text and colors are prototype UI and may need polish in a later visual pass.

## Next Recommended Step

Open Unity, let scripts compile, rerun `Ghost > Build Act 2 Entity Extraction Prototype Scene` if the scene looks stale, then verify in Play Mode: Validate enabled, empty/partial/wrong tags show incorrect feedback, `lab`→`room` plus `9pm`→`time` shows correct feedback, fixing after an incorrect state updates feedback, no Console errors, and the Act 2 scene is not in Build Settings.
