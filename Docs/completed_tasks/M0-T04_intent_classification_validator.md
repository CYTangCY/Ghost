# M0-T04 — Act 1 Intent-Classification Validator

## Completion Status

Completed. M0-T04 acceptance criteria met; implementation committed and pushed by the user.

## Date

2026-06-19

## Summary

Codex implemented the Act 1 intent-classification core as pure, scene-free, input-free C# logic
plus EditMode tests, and created runtime and test assembly definitions. The validator checks the
Act 1 learning concept: messages must be grouped by shared purpose / intent, not by exact wording.
Documentation (CODE_WALKTHROUGH.md, UNITY_TEST_CHECKLIST.md) and a Codex run log were produced.

## Files Created

- `Assets/Scripts/Ghost.Runtime.asmdef`
- `Assets/Scripts/Puzzles/IntentClassification/IntentCard.cs`
- `Assets/Scripts/Puzzles/IntentClassification/IntentClassificationValidator.cs`
- `Assets/Tests/EditMode/Ghost.EditModeTests.asmdef`
- `Assets/Tests/EditMode/IntentClassificationValidatorTests.cs`
- `Docs/codex_runs/M0-T04_001_intent_validator_initial_implementation.md`

(All five Unity files verified present on disk during this closure. Unity-generated `.meta` files
for the new scripts/asmdefs are required by Unity and are the expected part of the commit.)

## Files Modified

- `Docs/CODE_WALKTHROUGH.md` (added Intent Classification Runtime entries)
- `Docs/UNITY_TEST_CHECKLIST.md` (added the M0-T04 EditMode test checklist)

## Validator Behaviour

- `IntentCard`: immutable pure-C# data — `Id`, `MessageText`, `IntentId`. The constructor rejects
  an empty card id or empty intent id (throws `ArgumentException`); null message text becomes "".
- `IntentClassificationValidator.Validate(cards, submittedGroups)` returns an
  `IntentClassificationResult` with `IsCorrect` and `Errors`.
- `IsCorrect` is true only when every known card appears exactly once and each intent appears in
  exactly one pure (single-intent) group.
- Returns incorrect with error details for: empty level card data, duplicate card ids, null cards,
  empty groups, duplicate submitted cards, unknown submitted card ids, missing known cards, groups
  that mix different intents, and one intent split across multiple groups.

## Tests Added

EditMode tests in `Assets/Tests/EditMode/IntentClassificationValidatorTests.cs`:
- `Validate_WhenMessagesWithSamePurposeAreGrouped_ReturnsCorrect` (correct grouping)
- `Validate_WhenGroupMixesDifferentIntents_ReturnsIncorrect`
- `Validate_WhenIntentIsSplitAcrossGroups_ReturnsIncorrect`
- `Validate_WhenCardIsMissing_ReturnsIncorrect`
- `Validate_WhenDuplicateUnknownOrEmptyGroupsAreSubmitted_ReturnsIncorrect`

Covers at least one correct grouping and several incorrect groupings (criteria met).

## Codex Run Log

`Docs/codex_runs/M0-T04_001_intent_validator_initial_implementation.md` (run 001, 2026-06-19).

Test/check result recorded honestly in that log: **"Not run — Unity Editor test runner was not
executed in this Codex session."** Codex's Unity batch-mode attempt produced no result XML/log, and
a plain C# compiler check could not run because the local environment had no .NET SDK. Codex also
reverted its import side effects (auto-generated `.meta` files and a brief touch to
`ProjectSettings/ShaderGraphSettings.asset`) so its working tree stayed within the allowed files.
This closure does NOT restate Codex as having run the tests.

## Human Verification Result

Performed by the user in the Unity Editor after the Codex run:
- The Unity project compiled successfully.
- The `IntentClassificationValidatorTests` EditMode tests were run and **passed**.
- No scene, ProjectSettings, Packages, or unrelated files were intentionally committed (the new
  scripts'/asmdefs' required `.meta` files are the expected exception).
- The implementation was committed and pushed.

This human Editor verification is the source of the "tests passed" status — not the Codex session.

## Remaining Risks

- Verification is manual (human-in-Editor); there is no automated CI test run yet.
- No Act 1 sample puzzle data exists yet, so the validator is exercised only by hand-built test
  fixtures, not by reusable level content (addressed in M0-T05).
- No UI / scene / drag-and-drop yet; the player-facing Act 1 loop is not playable.
- Unity may regenerate `.meta` files on next open; keep them tracked and do not hand-edit them.

## Next Task

M0-T05 — Define the Act 1 sample puzzle data structure and sample content for the
intent-classification puzzle, without creating UI or scenes.
