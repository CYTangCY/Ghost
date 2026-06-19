# M0-T06 — Act 1 Intent-Classification Session State

## Completion Status

Completed. M0-T06 acceptance criteria met; implementation committed and pushed by the user.

## Date

2026-06-19

## Summary

Codex implemented the Act 1 intent-classification puzzle session/state layer as pure, scene-free,
input-free, WebGL-safe C# logic, plus EditMode tests. The session owns the player's current
grouping (unassigned + grouped cards), supports moving cards, exposes the grouping in the shape the
M0-T04 `IntentClassificationValidator` expects, and can validate its own current state. It builds
on M0-T04 (validator) and M0-T05 (sample data). Documentation was updated.

## Files Created

- `Assets/Scripts/Puzzles/IntentClassification/IntentClassificationSession.cs`
- `Assets/Tests/EditMode/IntentClassificationSessionTests.cs`
- `Docs/codex_runs/M0-T06_001_intent_classification_session_state.md`

(Both `.cs` files verified present on disk during this closure.)

## Files Modified

- `Docs/CODE_WALKTHROUGH.md` (added the IntentClassificationSession entry)
- `Docs/UNITY_TEST_CHECKLIST.md` (added the M0-T06 section)

## Session / State Behaviour

`IntentClassificationSession` — pure C# session state (not a MonoBehaviour, no serialized fields).

Internal state: source `IntentCard` list; unassigned card ids; assigned card ids by player group
id; current group id by card id.

Methods:
- `IntentClassificationSession(IEnumerable<IntentCard> cards)` — initialize from any card list.
- `CreateFromSampleData()` — initialize from `Act1IntentClassificationSampleData`.
- `MoveCardToGroup(string cardId, string groupId)` — assign or move a known card into a player group.
- `MoveCardToUnassigned(string cardId)` — return a known card to the unassigned pile.
- `GetAssignedGroupId(string cardId)` — current group id for a card, or null if unassigned.
- `GetAssignedCardIds(string groupId)` — assigned card ids for a player group.
- `CreateSubmittedGroups()` — current non-empty groups in the format expected by the validator.
- `ValidateCurrentState()` — validates the current grouping via `IntentClassificationValidator`.

Failure handling: null card list → `ArgumentNullException`; empty card list, null cards, duplicate
card ids, empty/unknown card ids, and empty group ids → `ArgumentException`. Partial groupings
validate as incorrect because unassigned cards are missing from the submitted groups. Player group
enumeration order is intentionally not part of the session contract.

## Tests Added

EditMode tests in `Assets/Tests/EditMode/IntentClassificationSessionTests.cs`:
- `Constructor_WhenCreatedFromCards_LeavesAllCardsUnassigned`
- `CreateFromSampleData_LeavesAllSampleCardsUnassigned`
- `MoveCardToGroup_AssignsCardAndRemovesItFromUnassigned`
- `MoveCardToGroup_WhenCardAlreadyAssigned_MovesCardBetweenGroups`
- `MoveCardToUnassigned_WhenCardWasAssigned_ReturnsCardToUnassigned`
- `ValidateCurrentState_WhenGroupingIsPartial_ReturnsIncorrect`
- `ValidateCurrentState_WhenSampleGroupingIsCorrect_ReturnsCorrect`
- `MoveCardToGroup_WhenCardIdIsUnknown_ThrowsArgumentException`
- `MoveCardToUnassigned_WhenCardIdIsUnknown_ThrowsArgumentException`
- `CreateSubmittedGroups_ReturnsOnlyAssignedGroups`

Covers initialization, moving cards, unassigned state, partial/incorrect grouping, and validating a
correct grouping (criteria met).

## Codex Run Log

`Docs/codex_runs/M0-T06_001_intent_classification_session_state.md` (run 001, 2026-06-19).

Test/check result recorded honestly in that log: **"Not run — Unity Editor test runner was not
executed in this Codex session."** Notable test-design decisions: avoided order-sensitive
assertions for submitted groups (enumeration order is not part of the session contract) and avoided
NUnit `Has.Count` on interface-returned arrays (carrying forward the M0-T05 compatibility lesson),
asserting explicit `.Count` values instead. This closure does NOT restate Codex as having run the
tests.

## Human Verification Result

Performed by the user in the Unity Editor after the Codex run:
- The Unity project compiled successfully.
- EditMode tests passed: `IntentClassificationValidatorTests`,
  `Act1IntentClassificationSampleDataTests`, and `IntentClassificationSessionTests`.
- No scene, ProjectSettings, Packages, or unrelated files were intentionally committed.

This human Editor verification is the source of the "tests passed" status — not the Codex session.

## Remaining Risks

- Verification is manual (human-in-Editor); there is no automated CI test run yet.
- Unity may regenerate `.meta` files on next open / test run; keep them tracked, do not hand-edit.
- No UI or scene yet, so Act 1 is still not playable (a static UI prototype scene is M0-T07 — the
  first task that introduces scene/UI work).
- Consumers of the session must not depend on player-group enumeration order.

## Next Task

M0-T07 — Create a static Act 1 intent-classification UI prototype scene, without drag-and-drop
interaction yet.
