# M0-T05 — Act 1 Sample Puzzle Data

## Completion Status

Completed across two Codex runs (001 implementation, 002 test fix); committed and pushed by the user.

## Date

2026-06-19

## Summary

Codex added reusable Act 1 intent-classification sample puzzle data plus EditMode tests, reusing
the M0-T04 `IntentCard` / `IntentClassificationValidator`. The data demonstrates the Act 1 concept
that differently worded messages can share the same purpose / intent. Run 002 fixed one failing
EditMode test. Documentation (CODE_WALKTHROUGH.md, UNITY_TEST_CHECKLIST.md) was updated. The work
stayed scene-free and UI-free.

## Files Created

- `Assets/Scripts/Puzzles/IntentClassification/Act1IntentClassificationSampleData.cs` (run 001)
- `Assets/Tests/EditMode/Act1IntentClassificationSampleDataTests.cs` (run 001)
- `Docs/codex_runs/M0-T05_001_act1_sample_puzzle_data.md` (run 001)
- `Docs/codex_runs/M0-T05_002_act1_sample_data_test_fix.md` (run 002)

(Sample data and test `.cs` files verified present on disk during this closure.)

## Files Modified

- `Docs/CODE_WALKTHROUGH.md` (run 001 — added the Act1IntentClassificationSampleData entry)
- `Docs/UNITY_TEST_CHECKLIST.md` (run 001 — added the M0-T05 section)
- `Assets/Tests/EditMode/Act1IntentClassificationSampleDataTests.cs` (run 002 — assertion fix only)

## Sample Data Structure

`Act1IntentClassificationSampleData` — a pure C# static data provider (no serialized fields, not a
MonoBehaviour):
- Intent-id constants: `FindItemIntentId`, `AskLocationIntentId`, `AskIdentityIntentId`.
- `CreateCards()` — returns fresh `IntentCard` objects for the sample puzzle.
- `CreateCorrectGroups()` — returns the correct grouping by card id as an `IReadOnlyList`, ready to
  pass into `IntentClassificationValidator.Validate(...)`.
- Output: 3 intent groups, 9 message cards total, 3 differently worded messages per intent.

## Sample Intent Groups

- `find_item` — messages about finding a missing key, notebook, or lantern.
- `ask_location` — messages asking where Ghost is.
- `ask_identity` — messages asking who Ghost is / what to call the little ghost.

(Each group has three differently worded messages, demonstrating "same purpose, different wording".)

## Tests Added

EditMode tests in `Assets/Tests/EditMode/Act1IntentClassificationSampleDataTests.cs`:
- `SampleData_WhenCorrectGroupsSubmitted_ValidatesSuccessfully`
- `SampleData_ContainsThreeIntentGroupsWithMultipleDifferentlyWordedMessages`
- `SampleData_WhenOneCardMovesToWrongPurpose_ValidatorRejectsIt`

## Codex Run Logs

- `Docs/codex_runs/M0-T05_001_act1_sample_puzzle_data.md` (run 001)
- `Docs/codex_runs/M0-T05_002_act1_sample_data_test_fix.md` (run 002)

Both logs honestly record the test/check result as **"Not run — Unity Editor test runner was not
executed in this Codex session."** This closure does NOT restate Codex as having run the tests.

## Run 002 Test Fix Summary

- Failure: `SampleData_ContainsThreeIntentGroupsWithMultipleDifferentlyWordedMessages` failed
  because NUnit's `Has.Count` constraint tried to resolve a `Count` property on the runtime array
  returned by `CreateCorrectGroups()`, raising `System.ArgumentException: Property Count was not
  found`.
- Fix: replaced `Assert.That(groups, Has.Count.EqualTo(3))` with
  `Assert.That(groups.Count, Is.EqualTo(3))`, using the strongly typed `IReadOnlyList.Count` directly.
- Scope of the fix: only the test file changed; runtime sample data and validator logic untouched.

## Human Verification Result

Performed by the user in the Unity Editor after run 002:
- The Unity project compiled successfully.
- EditMode tests passed: `IntentClassificationValidatorTests` and
  `Act1IntentClassificationSampleDataTests`.
- No scene, ProjectSettings, Packages, or unrelated files were intentionally committed.

This human Editor verification is the source of the "tests passed" status — not the Codex sessions.

## Remaining Risks

- Verification is manual (human-in-Editor); there is no automated CI test run yet.
- During M0-T05 the working tree temporarily included Unity import side effects (auto-generated
  `.meta` files and a `ProjectSettings/ShaderGraphSettings.asset` change) that Codex did not author;
  the human verification confirms they were not intentionally committed. Keep checking the working
  tree before each commit.
- Sample card ids and `CreateCorrectGroups()` must stay in sync if the cards are edited later.
- No session/state layer, UI, or scene yet, so Act 1 is not playable (session/state is M0-T06).

## Next Task

M0-T06 — Implement the Act 1 intent-classification puzzle session state as pure C# logic, without
creating UI or scenes.
