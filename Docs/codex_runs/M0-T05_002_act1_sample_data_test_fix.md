# M0-T05 - Run 002 - act1 sample data test fix

## Task ID

M0-T05

## Run Number

002

## Date

2026-06-19

## Original Request / Codex Prompt Summary

Fix the failing M0-T05 EditMode test `SampleData_ContainsThreeIntentGroupsWithMultipleDifferentlyWordedMessages`, which failed because NUnit could not find a `Count` property on the runtime array returned by the sample data method.

## Files Created

- `Docs/codex_runs/M0-T05_002_act1_sample_data_test_fix.md`

## Files Modified

- `Assets/Tests/EditMode/Act1IntentClassificationSampleDataTests.cs`

## Tests or Checks Run

- Reviewed the user-provided Unity EditMode test failure trace.
- Inspected `Act1IntentClassificationSampleDataTests.cs`.
- Checked repository status before editing.

## Test / Check Result

Not run — Unity Editor test runner was not executed in this Codex session.

The user-provided Unity test result showed `SampleData_ContainsThreeIntentGroupsWithMultipleDifferentlyWordedMessages` failed before this fix. The test was not rerun by Codex after the assertion change.

## Errors Encountered

- NUnit's `Has.Count` constraint tried to resolve a `Count` property on the runtime array object returned by `CreateCorrectGroups()`, causing `System.ArgumentException: Property Count was not found`.

## Fixes Applied

- Replaced `Assert.That(groups, Has.Count.EqualTo(3))` with `Assert.That(groups.Count, Is.EqualTo(3))`, using the strongly typed `IReadOnlyList.Count` property directly.

## What Was Intentionally Not Changed

- No runtime sample data was changed.
- No validator logic was changed.
- No scenes were edited by Codex.
- No ProjectSettings files were edited by Codex.
- No Packages files were edited by Codex.
- No `.meta` files were manually edited by Codex.
- No UI, prefab, art asset, drag-and-drop UI, scene setup, Act 0, Act 2, LLM/backend integration, dialogue system, or save system work was implemented.

## Remaining Risks

- Unity EditMode tests still need to be rerun after this assertion fix.
- The working tree currently includes Unity-generated `.meta` files and a `ProjectSettings/ShaderGraphSettings.asset` modification from the external Unity test/import run; Codex did not modify them in this debugging run.

## Next Recommended Step

Rerun the EditMode tests in Unity's Test Runner and confirm the M0-T05 sample data tests pass.
