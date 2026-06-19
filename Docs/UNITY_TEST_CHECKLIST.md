# UNITY_TEST_CHECKLIST.md

## M0-T04: Intent Classification Validator

### Automated EditMode Tests

1. Open the Unity project.
2. Open `Window > General > Test Runner`.
3. Select the `EditMode` tab.
4. Confirm the `Ghost.EditModeTests` assembly appears.
5. Run the EditMode tests.
6. Expected tests:
   - `Validate_WhenMessagesWithSamePurposeAreGrouped_ReturnsCorrect`
   - `Validate_WhenGroupMixesDifferentIntents_ReturnsIncorrect`
   - `Validate_WhenIntentIsSplitAcrossGroups_ReturnsIncorrect`
   - `Validate_WhenCardIsMissing_ReturnsIncorrect`
   - `Validate_WhenDuplicateUnknownOrEmptyGroupsAreSubmitted_ReturnsIncorrect`

### Expected Result

All M0-T04 EditMode tests should pass after Unity imports the new scripts and assembly definitions.

### Play Mode Check

This task adds scene-free validator logic only. There is no GameObject, prefab, UI, or scene setup to test in Play Mode yet.

Manual Play Mode check:
1. Open `Assets/Scenes/SampleScene.unity`.
2. Enter Play Mode.
3. Confirm no new errors appear in the Console from the M0-T04 scripts.

### Inspector Setup

No Inspector setup is required for M0-T04. `IntentCard` and `IntentClassificationValidator` are pure C# classes and are not attached to GameObjects.

---

## M0-T05: Act 1 Intent Classification Sample Data

### Automated EditMode Tests

1. Open the Unity project.
2. Open `Window > General > Test Runner`.
3. Select the `EditMode` tab.
4. Confirm the `Ghost.EditModeTests` assembly appears.
5. Run the EditMode tests.
6. Expected M0-T05 tests:
   - `SampleData_WhenCorrectGroupsSubmitted_ValidatesSuccessfully`
   - `SampleData_ContainsThreeIntentGroupsWithMultipleDifferentlyWordedMessages`
   - `SampleData_WhenOneCardMovesToWrongPurpose_ValidatorRejectsIt`

### Expected Result

All M0-T05 EditMode tests should pass after Unity imports the new sample data script and test file.

### Play Mode Check

This task adds scene-free sample puzzle data only. There is no GameObject, prefab, UI, or scene setup to test in Play Mode yet.

Manual Play Mode check:
1. Open `Assets/Scenes/SampleScene.unity`.
2. Enter Play Mode.
3. Confirm no new errors appear in the Console from the M0-T05 scripts.

### Inspector Setup

No Inspector setup is required for M0-T05. `Act1IntentClassificationSampleData` is a pure C# static data provider and is not attached to a GameObject.

---

## M0-T06: Intent Classification Session State

### Automated EditMode Tests

1. Open the Unity project.
2. Open `Window > General > Test Runner`.
3. Select the `EditMode` tab.
4. Confirm the `Ghost.EditModeTests` assembly appears.
5. Run the EditMode tests.
6. Expected M0-T06 tests:
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

### Expected Result

All M0-T06 EditMode tests should pass after Unity imports the new session state script and test file.

### Play Mode Check

This task adds scene-free session state only. There is no GameObject, prefab, UI, or scene setup to test in Play Mode yet.

Manual Play Mode check:
1. Open `Assets/Scenes/SampleScene.unity`.
2. Enter Play Mode.
3. Confirm no new errors appear in the Console from the M0-T06 scripts.

### Inspector Setup

No Inspector setup is required for M0-T06. `IntentClassificationSession` is a pure C# state object and is not attached to a GameObject.
