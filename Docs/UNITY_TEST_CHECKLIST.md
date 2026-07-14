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

---

## M0-T07: Static Act 1 UI Prototype Scene

### Scene Creation Check

Codex could not create the scene automatically because Unity batch mode exited before project import and scene generation. Use the manual Unity Editor path:

1. Open the Ghost Unity project.
2. Wait for scripts to import and compile.
3. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
4. Confirm `Assets/Scenes/Act1IntentClassificationPrototype.unity` is created.
5. Do not add the scene to Build Settings during M0-T07.

### Static UI Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
2. Confirm the scene displays the Act 1 title.
3. Confirm the scene displays nine sample message cards.
4. Confirm the scene displays three intent group areas:
   - `find_item`
   - `ask_location`
   - `ask_identity`
5. Confirm there is no drag-and-drop behaviour.
6. Confirm there is no validation button, scoring, save/load, animation, backend, LLM, or dialogue behaviour.

### M0-T07 Run 002 Display Fix Check

If the left-side message cards appear as blank pale rectangles:

1. Select `Ghost > Build Act 1 Intent Classification Prototype Scene` again.
2. Reopen `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
3. Confirm all nine message card texts are visible in the left column.
4. Confirm the three right-side intent group areas still show:
   - `find_item`
   - `ask_location`
   - `ask_identity`
5. Enter Play Mode and confirm all nine message texts remain visible.

### Play Mode Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
2. Enter Play Mode.
3. Confirm no new Console errors appear.
4. Confirm the static cards and intent group areas remain visible.

### Inspector Setup

If the scene is created through the menu builder, no manual Inspector setup should be required. The builder wires `Act1IntentClassificationStaticPresenter` to its card list root, intent group list root, card template, and intent group template.

---

## M0-T08: Click-to-Assign Act 1 Prototype Interaction

### Scene Refresh Check

If the existing prototype scene was generated before M0-T08:

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile the updated presentation scripts.
3. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
4. Confirm `Assets/Scenes/Act1IntentClassificationPrototype.unity` is refreshed.
5. Do not add the scene to Build Settings during M0-T08.

### Play Mode Interaction Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
2. Enter Play Mode.
3. Confirm all nine sample message cards are visible in the left column.
4. Confirm the three intent group areas are visible in the right column:
   - `find_item`
   - `ask_location`
   - `ask_identity`
5. Click a message card.
6. Confirm the clicked card changes to the selected highlight.
7. Click one of the three intent group areas.
8. Confirm the selected card text appears inside that intent group area.
9. Confirm the assigned card changes to the assigned highlight when it is no longer the selected card.
10. Click another message card and confirm the selected highlight moves to the newly clicked card.
11. Assign that card to a different intent group and confirm the visible assigned-card list updates.
12. Confirm there is no drag-and-drop behaviour.
13. Confirm there is no validation button, scoring, save/load, animation, backend, LLM, dialogue behaviour, or final art pass.
14. Confirm no new Console errors appear.

### Inspector Setup

If the scene is created or refreshed through the menu builder, no manual Inspector setup should be required. The builder wires `Act1IntentClassificationStaticPresenter` to its card list root, intent group list root, card template, and intent group template, and creates an `EventSystem` for UI clicks.

### M0-T08 Run 002 UI Fix Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
2. Enter Play Mode.
3. Click a message card and confirm it receives the selected highlight.
4. Click the same message card again and confirm the selected highlight disappears.
5. Click another message card, then click an intent group area.
6. Confirm the card is assigned and no card remains selected afterward.
7. Assign multiple cards to the same intent group area.
8. Confirm the assigned message rows stay visually inside the group panel.
9. Confirm the three intent group areas remain visible:
   - `find_item`
   - `ask_location`
   - `ask_identity`
10. Confirm there is still no drag-and-drop behaviour.
11. Confirm there is still no validation button, scoring, save/load, animation, backend, LLM, dialogue behaviour, or final art pass.
12. Confirm no new Console errors appear.

---

## M0-T09: Assignment Editing, Group Capacity, and Validation Feedback

### Scene Refresh Check

If the existing prototype scene was generated before M0-T09:

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile the updated presentation scripts.
3. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
4. Confirm `Assets/Scenes/Act1IntentClassificationPrototype.unity` is refreshed.
5. Do not add the scene to Build Settings during M0-T09.

### Play Mode Interaction Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
2. Enter Play Mode.
3. Confirm all nine sample message cards are visible in the left column.
4. Confirm the three intent group areas are visible:
   - `find_item`
   - `ask_location`
   - `ask_identity`
5. Confirm a `Validate` button and feedback text are visible under the intent group column.
6. Select a message card, then click an intent group area to assign it.
7. Confirm the assigned card appears as a `Back:` row inside that group.
8. Click the assigned `Back:` row.
9. Confirm the card returns to the unassigned/default visual state in the left list.
10. Assign a card to the wrong group, then select it again from the left list and click the correct group.
11. Confirm the card moves from the wrong group to the correct group without restarting Play Mode.
12. Assign many or all cards to one group.
13. Confirm the assigned list can be scrolled and cards do not silently disappear.
14. Click `Validate` with an incomplete or incorrect grouping.
15. Confirm feedback reports an incorrect grouping and shows an issue count.
16. Assign all cards to their correct intent groups.
17. Click `Validate`.
18. Confirm feedback reports a correct grouping.
19. Confirm there is no drag-and-drop behaviour.
20. Confirm there is no scoring, save/load, animation, backend, LLM, dialogue behaviour, or final art pass.
21. Confirm no new Console errors appear.

### Inspector Setup

If the scene is created or refreshed through the menu builder, no manual Inspector setup should be required. The presenter creates the scrollable assigned-card areas, the `Back:` row buttons, the Validate button, and the validation feedback text from the generated UI roots and templates.

---

## M0-T11: Presentation Refactor Regression Check

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile the new presentation assembly definitions:
   - `Assets/Presentation/Ghost.Presentation.asmdef`
   - `Assets/Presentation/Act1IntentClassification/Editor/Ghost.Presentation.Editor.asmdef`
3. Confirm there are no Console compile errors.
4. Confirm the existing EditMode test assembly still appears in `Window > General > Test Runner`.

### Scene Refresh Check

M0-T11 should not require scene regeneration because the same presenter script remains attached. If Unity reports missing script references or the prototype scene appears stale:

1. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
2. Confirm `Assets/Scenes/Act1IntentClassificationPrototype.unity` is refreshed.
3. Do not add the scene to Build Settings during M0-T11.

### Play Mode Regression Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
2. Enter Play Mode.
3. Confirm all nine sample message cards are visible.
4. Click a message card and confirm it receives the selected highlight.
5. Click the same card again and confirm it deselects.
6. Select a card, then click an intent group area to assign it.
7. Confirm the assigned card appears as a `Back:` row inside that group.
8. Click the assigned `Back:` row and confirm the card returns to the unassigned/default visual state.
9. Assign many or all cards to one group and confirm the assigned list remains scrollable.
10. Correct a wrong assignment by selecting the card again and clicking a different group.
11. Click `Validate` with an incomplete or incorrect grouping and confirm incorrect feedback appears.
12. Assign all cards to their correct groups, click `Validate`, and confirm correct feedback appears.
13. Confirm there is no drag-and-drop behaviour.
14. Confirm there is no scoring, save/load, animation, backend, LLM, dialogue behaviour, or final art pass.
15. Confirm no new Console errors appear.

### Automated EditMode Tests

Run the existing EditMode tests without modifying them. M0-T11 is a presentation refactor, so the pure logic tests should still pass after Unity imports the new assembly definitions.

---

## M0-T12: Minimal Drag-to-Assign Act 1 Prototype

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationDraggableCard.cs`
   - `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationDropTarget.cs`
3. Confirm there are no Console compile errors.

### Scene Refresh Check

M0-T12 should work at Play Mode startup because `Act1IntentClassificationStaticPresenter` attaches the drag and drop behaviours while rendering sample data. If the open scene appears stale:

1. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
2. Confirm `Assets/Scenes/Act1IntentClassificationPrototype.unity` is refreshed.
3. Do not add the scene to Build Settings during M0-T12.

### Play Mode Interaction Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
2. Enter Play Mode.
3. Confirm all nine sample message cards are visible.
4. Click a message card and click an intent group to confirm click-to-assign still works.
5. Click an assigned `Back:` row and confirm the card returns to unassigned.
6. Click `Validate` with an incomplete or incorrect grouping and confirm incorrect feedback still appears.
7. Drag a message card and confirm a solid card-like preview follows the pointer.
8. Drop the card anywhere inside `find_item`, `ask_location`, or `ask_identity`, including the background/scroll area rather than only the assigned-card rows.
9. Confirm the card appears as a compact `Back:` row in the dropped group's normal assigned-card list, not as a free-placed object.
10. Drag that assigned `Back:` row back to the left message-card list and confirm the card returns to unassigned.
11. Drag an assigned row from one intent group to a different intent group and confirm it moves through the normal assigned-card list.
12. Drag another card outside all valid target areas and confirm the UI state does not change.
13. After each successful or cancelled drop, confirm no stale `Drag Preview` objects remain in the Hierarchy.
14. Assign many cards to one group and confirm the compact assigned rows remain readable and the assigned list remains scrollable.
15. Assign all cards to their correct groups, click `Validate`, and confirm correct feedback still appears.
16. Confirm there is no scoring, save/load, animation, backend, LLM, dialogue behaviour, final art pass, free placement, or group reordering.
17. Confirm no new Console errors appear.

### Inspector Setup

If the scene is created or refreshed through the menu builder, no manual Inspector setup should be required. The presenter attaches `Act1IntentClassificationDraggableCard` to rendered cards and assigned rows, and attaches `Act1IntentClassificationDropTarget` to rendered intent group areas, their scroll viewports, and the left message-card list at render time.

---

## M0-T13: Game Shell Prototype

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Shell/ShellSceneNames.cs`
   - `Assets/Presentation/Shell/ShellDialogueData.cs`
   - `Assets/Presentation/Shell/LilyDialogueFrame.cs`
   - `Assets/Presentation/Shell/ShellSceneNavigationButton.cs`
   - `Assets/Presentation/Shell/GameShellPresenter.cs`
   - `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
   - `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
3. Confirm there are no Console compile errors.

### Scene Build / Build Settings Check

M0-T13 creates the shell scene through a Unity editor builder, not by hand-editing scene YAML.

1. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
2. Select `Ghost > Build Game Shell Scene`.
3. Confirm `Assets/Scenes/GameShellPrototype.unity` exists.
4. Open `File > Build Profiles` or the Unity 6 Build Settings view.
5. Confirm `Assets/Scenes/GameShellPrototype.unity` and `Assets/Scenes/Act1IntentClassificationPrototype.unity` are enabled in Build Settings.
6. Confirm no other ProjectSettings files were intentionally changed.

### Play Mode Shell Check

1. Open `Assets/Scenes/GameShellPrototype.unity`.
2. Enter Play Mode.
3. Confirm the title screen shows the project title `Ghost`.
4. Confirm Ghost has a visible placeholder presence.
5. Confirm Lily has a visible placeholder presence and a reusable dialogue-frame panel.
6. Confirm Lily's dialogue text appears from `ShellDialogueData`, not from separate hardcoded per-screen presenter text.
7. Click `Start / Continue`.
8. Confirm the act select / hub screen appears.
9. Confirm Lily's dialogue updates to the hub guidance line.
10. Confirm Act 1 is visible as a selectable prototype act.
11. Click `Start Act 1`.
12. Confirm Unity loads `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
13. In Act 1, confirm the existing click assignment, drag assignment, bidirectional reassignment, Back/unassign, Validate, and validation feedback still work.
14. Confirm a `Return to Hub` button appears in Act 1.
15. Click `Return to Hub`.
16. Confirm Unity loads `Assets/Scenes/GameShellPrototype.unity`.
17. Confirm no new Console errors appear.
18. Confirm there is no Act 2 implementation, node graph, save/load, backend, LLM, full visual-novel dialogue system, scoring, final art pass, coordinate-based free placement, or group reordering added by this task.

### Inspector Setup

If the scene is created through `Ghost > Build Game Shell Scene`, no manual Inspector setup should be required. The builder wires `GameShellPresenter`, `LilyDialogueFrame`, the title screen, the act hub screen, and the shell buttons. The Act 1 return button is added at runtime by `ShellReturnToHubOverlay` when the Act 1 scene loads.

---

## M0-T14: Act 2 Entity Extraction Core

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Scripts/Puzzles/EntityExtraction/EntityType.cs`
   - `Assets/Scripts/Puzzles/EntityExtraction/EntitySpan.cs`
   - `Assets/Scripts/Puzzles/EntityExtraction/EntityExtractionValidator.cs`
   - `Assets/Scripts/Puzzles/EntityExtraction/Act2EntityExtractionSampleData.cs`
   - `Assets/Tests/EditMode/Act2EntityExtractionValidatorTests.cs`
   - `Assets/Tests/EditMode/Act2EntityExtractionSampleDataTests.cs`
3. Confirm there are no Console compile errors.

### Automated EditMode Tests

1. Open `Window > General > Test Runner`.
2. Select the `EditMode` tab.
3. Confirm the `Ghost.EditModeTests` assembly appears.
4. Run the EditMode tests.
5. Expected M0-T14 tests:
   - `Validate_WhenSubmittedSpansExactlyMatch_ReturnsCorrect`
   - `Validate_WhenExpectedSpanIsMissing_ReturnsIncorrect`
   - `Validate_WhenBoundaryMatchesButTypeIsWrong_ReturnsIncorrect`
   - `Validate_WhenTypeMatchesButBoundaryIsWrong_ReturnsIncorrect`
   - `Validate_WhenSubmittedSpanIsExtra_ReturnsIncorrect`
   - `Validate_WhenSubmittedSpanIsDuplicated_ReturnsIncorrect`
   - `SampleData_WhenCorrectSpansSubmitted_ValidatesSuccessfully`
   - `SampleData_ContainsSystemAndCustomEntityTypes`
   - `SampleData_ContainsRoomSynonymPair`

### Expected Result

All M0-T14 EditMode tests should pass after Unity imports the new runtime scripts and test files.

### Play Mode Check

No Play Mode behaviour, logic only.

Optional Console sanity check:
1. Open any existing scene.
2. Enter Play Mode.
3. Confirm no new Console errors appear from the M0-T14 scripts.

### Inspector Setup

No Inspector setup is required for M0-T14. The entity-extraction model, validator, sample data, and tests are pure C# logic and are not attached to GameObjects.

---

## M0-T15: Act 2 Entity Extraction Session State

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Scripts/Puzzles/EntityExtraction/EntityExtractionSession.cs`
   - `Assets/Tests/EditMode/Act2EntityExtractionSessionTests.cs`
3. Confirm there are no Console compile errors.

### Automated EditMode Tests

1. Open `Window > General > Test Runner`.
2. Select the `EditMode` tab.
3. Confirm the `Ghost.EditModeTests` assembly appears.
4. Run the EditMode tests.
5. Expected M0-T15 tests:
   - `CreateFromSampleMessage_StartsWithNoCurrentSpansAndValidatesIncorrect`
   - `ValidateCurrentState_WhenAllCorrectSpansAdded_ReturnsCorrect`
   - `RemoveSpan_WhenSpanWasAdded_RemovesItAndStateBecomesIncorrect`
   - `AddSpan_WhenSpanExtendsPastMessageBounds_Throws`
   - `AddSpan_WhenExactDuplicateIsAdded_LeavesCurrentSpanCountUnchanged`
   - `RemoveSpan_WhenSpanWasNeverAdded_ReturnsFalse`

### Expected Result

All M0-T15 EditMode tests should pass after Unity imports the new session script and test file.

### Play Mode Check

No Play Mode behaviour, logic only.

Optional Console sanity check:
1. Open any existing scene.
2. Enter Play Mode.
3. Confirm no new Console errors appear from the M0-T15 scripts.

### Inspector Setup

No Inspector setup is required for M0-T15. `EntityExtractionSession` is a pure C# state object and is not attached to a GameObject.

---

## M0-T16: Display-Only Act 2 Span-Annotation UI Prototype

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityChipView.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
   - `Assets/Presentation/Act2EntityExtraction/Editor/Ghost.Presentation.Act2.Editor.asmdef`
   - `Assets/Presentation/Act2EntityExtraction/Editor/Act2EntityExtractionPrototypeSceneBuilder.cs`
3. Confirm there are no Console compile errors.

### Scene Builder Check

1. Select `Ghost > Build Act 2 Entity Extraction Prototype Scene`.
2. Confirm `Assets/Scenes/Act2EntityExtractionPrototype.unity` is created.
3. Do not add the scene to Build Settings during M0-T16.

### Static UI Check

1. Open `Assets/Scenes/Act2EntityExtractionPrototype.unity`.
2. Confirm the scene displays the Act 2 title.
3. Confirm the sample message is rendered as word chips.
4. Inspect rendered chip GameObjects and confirm each has `Act2EntityChipView` with `Start`, `Length`, and `Text`.
5. Confirm the `lab` and `9pm` chips align to their exact word text and character spans.
6. Confirm the entity-type palette/legend displays:
   - `time` / `System`
   - `room` / `Custom`
   - `object` / `Custom`
7. Confirm a placeholder `Validate spans` button and placeholder feedback text are visible.
8. Confirm there is no chip selection, no type assignment, no working validation, no scoring, no save/load, no backend, no LLM, no dialogue, no node graph, and no later-Act behaviour.

### Play Mode Check

1. Open `Assets/Scenes/Act2EntityExtractionPrototype.unity`.
2. Enter Play Mode.
3. Confirm the word chips, entity-type palette/legend, placeholder Validate button, and feedback text remain visible.
4. Confirm clicking the placeholder Validate button does not validate, score, or change puzzle state.
5. Confirm no new Console errors appear.

### Build Settings Check

1. Open `File > Build Profiles` or the Unity 6 Build Settings view.
2. Confirm `Assets/Scenes/Act2EntityExtractionPrototype.unity` is not added to Build Settings by M0-T16.

### Inspector Setup

If the scene is created through `Ghost > Build Act 2 Entity Extraction Prototype Scene`, no manual Inspector setup should be required. The builder wires `Act2EntityExtractionStaticPresenter` to its chip root, entity palette root, validation controls root, chip template, and entity type template.

---

## M0-T17: Act 2 Chip Selection and Entity Assignment

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
3. Confirm there are no Console compile errors.

### Scene Refresh Check

M0-T17 should work at Play Mode startup because `Act2EntityExtractionStaticPresenter` rebuilds the chips and palette when the scene starts. If the open scene preview looks stale:

1. Select `Ghost > Build Act 2 Entity Extraction Prototype Scene`.
2. Confirm `Assets/Scenes/Act2EntityExtractionPrototype.unity` is refreshed.
3. Do not add the scene to Build Settings during M0-T17.

### Play Mode Interaction Check

1. Open `Assets/Scenes/Act2EntityExtractionPrototype.unity`.
2. Enter Play Mode.
3. Confirm the sample message word chips and entity-type palette render.
4. Click an untagged chip and confirm it receives the selected highlight.
5. Click a different untagged chip and confirm the selection moves.
6. Click the selected chip again and confirm the selection clears.
7. Select the `lab` chip, then click the `room` entity type.
8. Confirm the `lab` chip becomes tagged with a small `room` badge and Custom-style color.
9. Select the `9pm` chip, then click the `time` entity type.
10. Confirm the `9pm` chip becomes tagged with a small `time` badge and System-style color while the `lab` tag remains.
11. Click a tagged chip and confirm it untags and returns to the plain untagged visual state.
12. Confirm multiple chips can be tagged one at a time.
13. Confirm the `Validate spans` button remains disabled/placeholder and does not validate, score, or change feedback.
14. Confirm there is no working validation feedback, save/load, backend, LLM, dialogue, node graph, later-Act behaviour, or final art pass.
15. Confirm no new Console errors appear.

### Build Settings Check

1. Open `File > Build Profiles` or the Unity 6 Build Settings view.
2. Confirm `Assets/Scenes/Act2EntityExtractionPrototype.unity` is not added to Build Settings by M0-T17.

### Inspector Setup

If the scene is created through `Ghost > Build Act 2 Entity Extraction Prototype Scene`, no manual Inspector setup should be required. The presenter attaches chip buttons, palette buttons, and chip badges at render time, and `Act2EntityExtractionInteractionController` is created in code rather than attached to a GameObject.

---

## M0-T18: Act 2 Validation Feedback

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
3. Confirm there are no Console compile errors.

### Scene Refresh Check

M0-T18 should work at Play Mode startup because `Act2EntityExtractionStaticPresenter` rebuilds the validation controls when the scene starts. If the open scene preview looks stale:

1. Select `Ghost > Build Act 2 Entity Extraction Prototype Scene`.
2. Confirm `Assets/Scenes/Act2EntityExtractionPrototype.unity` is refreshed.
3. Do not add the scene to Build Settings during M0-T18.

### Play Mode Interaction Check

1. Open `Assets/Scenes/Act2EntityExtractionPrototype.unity`.
2. Enter Play Mode.
3. Confirm the sample message word chips, entity-type palette, feedback text, and `Validate spans` button render.
4. Confirm the `Validate spans` button is enabled.
5. Click `Validate spans` before tagging anything.
6. Confirm incorrect feedback appears.
7. Tag only `lab` as `room`, then click `Validate spans`.
8. Confirm incorrect feedback still appears because the answer is partial.
9. Tag `9pm` as `time`, then click `Validate spans`.
10. Confirm correct feedback appears.
11. Untag one correct chip or tag an extra/wrong chip, then click `Validate spans`.
12. Confirm incorrect feedback appears.
13. Fix the tags back to the exact answer and click `Validate spans` again.
14. Confirm the feedback updates back to correct.
15. Confirm M0-T17 selection, assignment, untagging, and multi-tag behaviour still works.
16. Confirm there is no scoring persistence, save/load, backend, LLM, dialogue, node graph, multi-chip spans, later-Act behaviour, or final art pass.
17. Confirm no new Console errors appear.

### Build Settings Check

1. Open `File > Build Profiles` or the Unity 6 Build Settings view.
2. Confirm `Assets/Scenes/Act2EntityExtractionPrototype.unity` is not added to Build Settings by M0-T18.

### Inspector Setup

If the scene is created through `Ghost > Build Act 2 Entity Extraction Prototype Scene`, no manual Inspector setup should be required. The presenter creates the enabled Validate button and feedback text at render time, and the controller validates through `EntityExtractionSession.ValidateCurrentState()`.

---

## M0-T19: Act 2 Game Shell Integration

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Shell/ShellSceneNames.cs`
   - `Assets/Presentation/Shell/ShellDialogueData.cs`
   - `Assets/Presentation/Shell/GameShellPresenter.cs`
   - `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
   - `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
3. Confirm there are no Console compile errors.

### Scene Build / Build Settings Check

M0-T19 updates the shell scene through the existing Unity editor builder, not by hand-editing scene YAML.

1. Select `Ghost > Build Game Shell Scene`.
2. Confirm `Assets/Scenes/GameShellPrototype.unity` is refreshed.
3. Open `File > Build Profiles` or the Unity 6 Build Settings view.
4. Confirm all three scenes are enabled in Build Settings:
   - `Assets/Scenes/GameShellPrototype.unity`
   - `Assets/Scenes/Act1IntentClassificationPrototype.unity`
   - `Assets/Scenes/Act2EntityExtractionPrototype.unity`
5. Confirm no other ProjectSettings files were intentionally changed.

### Play Mode Shell Check

1. Open `Assets/Scenes/GameShellPrototype.unity`.
2. Enter Play Mode.
3. Click `Start / Continue`.
4. Confirm the act hub shows both `Start Act 1` and `Start Act 2`.
5. Confirm Lily's hub dialogue mentions Act 1 and Act 2.
6. Click `Start Act 2`.
7. Confirm Unity loads `Assets/Scenes/Act2EntityExtractionPrototype.unity`.
8. Confirm the Act 2 puzzle UI still works: tag chips, validate correct/incorrect feedback, and no new Act 2 puzzle behaviour changed.
9. Confirm a `Return to Hub` button appears in Act 2.
10. Click `Return to Hub`.
11. Confirm Unity loads `Assets/Scenes/GameShellPrototype.unity`.
12. From the hub, click `Start Act 1`.
13. Confirm Unity loads `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
14. Confirm existing Act 1 mechanics still work and a `Return to Hub` button still appears.
15. Click `Return to Hub` from Act 1 and confirm the shell loads.
16. Confirm there is no node graph, backend, LLM, save/load, full visual-novel dialogue, scoring persistence, final art pass, or Act 2 puzzle-rule change added by M0-T19.
17. Confirm no new Console errors appear.

### Inspector Setup

If the scene is created through `Ghost > Build Game Shell Scene`, no manual Inspector setup should be required. The builder wires `GameShellPresenter` to the title screen, hub screen, Lily dialogue frame, Start/Continue button, Start Act 1 button, Start Act 2 button, and Back to Title button. The runtime `ShellReturnToHubOverlay` adds `Return to Hub` in Act 1 and Act 2.

---

## M0-T21: Act 3 Node Graph Core

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Scripts/Puzzles/DialogGraph/DialogNodeType.cs`
   - `Assets/Scripts/Puzzles/DialogGraph/DialogNode.cs`
   - `Assets/Scripts/Puzzles/DialogGraph/DialogTransition.cs`
   - `Assets/Scripts/Puzzles/DialogGraph/DialogGraph.cs`
   - `Assets/Scripts/Puzzles/DialogGraph/ConversationTurn.cs`
   - `Assets/Scripts/Puzzles/DialogGraph/DialogContext.cs`
   - `Assets/Scripts/Puzzles/DialogGraph/DialogGraphSimulator.cs`
   - `Assets/Scripts/Puzzles/DialogGraph/DialogGraphValidator.cs`
   - `Assets/Scripts/Puzzles/DialogGraph/Act3DialogGraphSampleData.cs`
   - `Assets/Tests/EditMode/Act3DialogGraphSimulatorTests.cs`
   - `Assets/Tests/EditMode/Act3DialogGraphValidatorTests.cs`
3. Confirm there are no Console compile errors.

### Automated EditMode Tests

1. Open `Window > General > Test Runner`.
2. Select the `EditMode` tab.
3. Confirm the `Ghost.EditModeTests` assembly appears.
4. Run the EditMode tests.
5. Expected M0-T21 tests:
   - `Simulate_WhenSlotPresent_ReachesAnswerResponseAndStoresSlot`
   - `Simulate_WhenSlotMissing_ReachesAskForRoomResponse`
   - `Simulate_WhenSlotAlreadyExistsInContext_ReachesAnswerResponse`
   - `Simulate_WhenGraphCycles_StopsAtStepCap`
   - `Validate_WhenSampleGraphIsCorrect_ReturnsCorrect`
   - `Validate_WhenIntentBranchIsWiredToWrongIntent_ReturnsIncorrect`
   - `Validate_WhenSlotCheckIsMissing_ReturnsIncorrect`
   - `Validate_WhenResponseIdIsWrong_ReturnsIncorrect`
   - `Validate_WhenGraphHasUnreachableNodeAndDeadEnd_ReturnsIncorrect`

### Expected Result

All M0-T21 EditMode tests should pass after Unity imports the new runtime scripts and test files.

### Play Mode Check

No Play Mode behaviour, logic only.

Optional Console sanity check:
1. Open any existing scene.
2. Enter Play Mode.
3. Confirm no new Console errors appear from the M0-T21 scripts.

### Inspector Setup

No Inspector setup is required for M0-T21. The Act 3 dialog graph model, simulator, validator, sample data, and tests are pure C# logic and are not attached to GameObjects.

---

## M0-T22: Act 3 Graph Session State

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Scripts/Puzzles/DialogGraph/DialogGraphSession.cs`
   - `Assets/Tests/EditMode/Act3DialogGraphSessionTests.cs`
3. Confirm there are no Console compile errors.

### Automated EditMode Tests

1. Open `Window > General > Test Runner`.
2. Select the `EditMode` tab.
3. Confirm the `Ghost.EditModeTests` assembly appears.
4. Run the EditMode tests.
5. Expected M0-T22 tests:
   - `ValidateCurrentState_WhenSessionIsEmpty_ReturnsIncorrectWithoutThrowing`
   - `ValidateCurrentState_WhenCorrectGraphBuiltThroughSession_ReturnsCorrect`
   - `ValidateCurrentState_WhenSlotMissingTransitionIsMissing_ReturnsIncorrect`
   - `RemoveNode_WhenNodeHasTransitions_RemovesNodeAndReferencingTransitions`
   - `AddTransitionAndRemoveTransition_AreReflectedInCurrentTransitions`

### Expected Result

All M0-T22 EditMode tests should pass after Unity imports the new session script and test file.

### Play Mode Check

No Play Mode behaviour, logic only.

Optional Console sanity check:
1. Open any existing scene.
2. Enter Play Mode.
3. Confirm no new Console errors appear from the M0-T22 scripts.

### Inspector Setup

No Inspector setup is required for M0-T22. `DialogGraphSession` is a pure C# state object and is not attached to a GameObject.

---

## M0-T23: Display-Only Act 3 Node Graph UI Prototype

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
   - `Assets/Presentation/Act3DialogGraph/Editor/Ghost.Presentation.Act3.Editor.asmdef`
   - `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
3. Confirm there are no Console compile errors.

### Scene Generation Check

M0-T23 ships the builder and does not hand-write scene YAML.

1. Select `Ghost > Build Act 3 Dialog Graph Prototype Scene`.
2. Confirm `Assets/Scenes/Act3DialogGraphPrototype.unity` is created or refreshed.
3. Do not add the scene to Build Settings during M0-T23.

### Static Scene Check

1. Open `Assets/Scenes/Act3DialogGraphPrototype.unity`.
2. Confirm the node-type palette renders:
   - `Start`
   - `IntentBranch`
   - `SlotCheck`
   - `Response`
3. Confirm the level vocabulary renders:
   - `find_object`
   - `room`
   - `answer_object_location`
   - `ask_for_room`
4. Confirm the palette/vocabulary content is visible and stays inside the Palette panel without clipping past the bottom edge.
5. Confirm an empty graph canvas region renders.
6. Confirm the goal/test panel shows the sample conversations:
   - `find_object + room=lab -> answer_object_location`
   - `find_object (no room) -> ask_for_room`
7. Confirm the goal/test content is visible and stays inside its panel without clipping past the bottom edge.
8. Confirm the `Validate graph` button is present, disabled, and not wired to validation.
9. Confirm placeholder feedback text renders.
10. Confirm there is no node placement, edge drawing, scoring, save/load, backend, LLM, dialogue, Act 4-6 node type, or Game Shell integration added by M0-T23.

### Play Mode Check

1. Enter Play Mode in `Assets/Scenes/Act3DialogGraphPrototype.unity`.
2. Confirm the palette, empty canvas, goal/test panel, disabled Validate button, and placeholder feedback still render.
3. Confirm there are no new Console errors.
4. Confirm there is no gameplay interaction beyond the disabled placeholder controls.

### Build Settings Check

1. Open `File > Build Profiles` or the Unity 6 Build Settings view.
2. Confirm `Assets/Scenes/Act3DialogGraphPrototype.unity` is not added to Build Settings by M0-T23.

### Inspector Setup

If the scene is created through `Ghost > Build Act 3 Dialog Graph Prototype Scene`, no manual Inspector setup should be required. The builder wires `Act3DialogGraphStaticPresenter` to its palette root, graph canvas root, goal/test root, validation controls root, palette item template, and test-case template.

---

## M0-T24: Act 3 Node Placement and Connection Interaction

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
   - `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
   - `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
3. Confirm there are no Console compile errors.

### Scene Generation Check

M0-T24 reuses the existing Act 3 builder and does not hand-write scene YAML.

1. If the saved scene looks stale, select `Ghost > Build Act 3 Dialog Graph Prototype Scene`.
2. Open `Assets/Scenes/Act3DialogGraphPrototype.unity`.
3. Do not add the scene to Build Settings during M0-T24.

### Play Mode Interaction Check

1. Enter Play Mode in `Assets/Scenes/Act3DialogGraphPrototype.unity`.
2. Click each placement row and confirm a configured node card appears:
   - `Start`
   - `IntentBranch` with `find_object`
   - `SlotCheck` with `room`
   - `Response` with `answer_object_location`
   - `Response` with `ask_for_room`
3. Click node cards and confirm selection highlight toggles/replaces correctly.
4. Select the Start node and click `Set Start`; confirm the card is marked `[Start]`.
5. Build the full intended graph:
   - `Start -> IntentBranch(find_object)` with `Always`
   - `IntentBranch(find_object) -> SlotCheck(room)` with `Always`
   - `SlotCheck(room) -> Response(answer_object_location)` with `SlotPresent`
   - `SlotCheck(room) -> Response(ask_for_room)` with `SlotMissing`
6. Confirm each transition appears in the transition list with its condition.
7. Remove one transition with its `Remove` button and confirm it disappears.
8. Remove a node that has transitions and confirm referenced transitions disappear with it.
9. Confirm the `Validate graph` button remains present, disabled, and not wired to validation feedback.
10. Confirm there are no new Console errors.
11. Confirm there is no scoring, save/load, backend, LLM, dialogue, Act 4-6 node type, Game Shell integration, or Build Settings change added by M0-T24.

### Build Settings Check

1. Open `File > Build Profiles` or the Unity 6 Build Settings view.
2. Confirm `Assets/Scenes/Act3DialogGraphPrototype.unity` is not added to Build Settings by M0-T24.

### Inspector Setup

If the scene is created through `Ghost > Build Act 3 Dialog Graph Prototype Scene`, no manual Inspector setup should be required. The builder wires the presenter roots/templates; the presenter creates the interaction controller and runtime node/transition controls.

---

## M0-T30: Act 3 Node Graph UX Redesign and Validation Feedback

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
   - `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
   - `Assets/Presentation/Act3DialogGraph/Act3DialogGraphNodeDragView.cs`
   - `Assets/Presentation/Act3DialogGraph/Act3DialogGraphOutputPortView.cs`
   - `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInputPortView.cs`
   - `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
3. Confirm there are no Console compile errors.

### Scene Generation Check

M0-T30 reuses the existing Act 3 builder and does not hand-write scene YAML.

1. Select `Ghost > Build Act 3 Dialog Graph Prototype Scene` to refresh the generated scene.
2. Open `Assets/Scenes/Act3DialogGraphPrototype.unity`.
3. Confirm the scene is not added to Build Settings.

### Play Mode Interaction Check

1. Enter Play Mode in `Assets/Scenes/Act3DialogGraphPrototype.unity`.
2. Confirm the title and objective use player-facing language, not raw ids like `find_object`, `answer_object_location`, or `ask_for_room`.
3. Confirm the palette is roughly half its previous width, the middle reply-map board receives the reclaimed space, the right guide keeps a readable width/text size, and the bottom `Test Ghost's map` strip is roughly half its earlier height.
   - Re-enter Play Mode or rerun the builder once and confirm the palette/guide widths stay stable instead of expanding unpredictably.
4. Confirm the palette is categorized:
   - Flow: `Start here`, `Recognize request`
   - Check: `Check room`
   - Reply: `Answer location`, `Ask which room`
5. Drag each palette card into the reply-map board and confirm it creates a configured node at the drop position.
6. Click a palette card and confirm click-to-place still works as a fallback.
7. Confirm each card has a short, readable purpose and uses small coloured ports placed on the card edges instead of text boxes labelled IN/OUT.
8. Confirm the right guide explains the port colours in readable text:
   - blue = next step
   - green = room is known
   - orange = room is missing
   - top dot = wire drop target
9. Drag placed node cards freely around the graph board and slightly outside the board toward the bottom trash zone; confirm they stay where dropped during the current session unless dropped on trash.
10. Confirm placing a Start node automatically marks it as the start node.
11. Confirm `Start here` has no top input dot and only has its bottom blue output dot.
12. Drag from `Start here`'s blue output dot to the `Recognize request` top input dot and confirm a straight wire appears.
13. Drag from `Recognize request`'s blue output dot to the `Check room` top input dot and confirm a straight wire appears.
14. Drag from `Check room`'s green output dot to the `Answer location` input dot and confirm a straight wire appears.
15. Drag from `Check room`'s orange output dot to the `Ask which room` input dot and confirm a straight wire appears.
16. Confirm dragging a new wire from the same output dot to another input replaces the previous wire from that dot.
17. Click a wire, press Delete or Backspace, and confirm the wire disappears.
18. Click a node card, press Delete or Backspace, and confirm the selected node disappears.
19. Confirm self-loop drops are rejected/ignored.
20. Confirm duplicate exact wire drops are rejected/ignored.
21. Confirm drops outside valid input dots are rejected/ignored.
22. Confirm reply cards have no output dots and cannot create outgoing wires.
23. Move a connected card and confirm existing straight wires stay attached to the moved dots.
24. Drag a node card over the bottom-bar `X drop card` trash zone to the right of `Test Ghost's map`; confirm the trash zone highlights while hovering.
25. Drop the node whenever the trash zone is highlighted and confirm the node disappears; there should be no state where the trash highlights but the card survives the drop.
26. Confirm removing a node that has wires also removes referenced wires.
27. Press `Test Ghost's map` on a partial/wrong graph and confirm incorrect red feedback appears with an issue count plus a Ghost reaction describing the bad route.
28. Confirm different wrong routes produce different Ghost reactions, for example:
   - no start/first step -> Ghost cannot begin
   - request skips the room check -> Ghost jumps to a reply too early
   - green room-known dot goes to `Ask which room` -> Ghost asks despite knowing the room
   - orange room-missing dot goes to `Answer location` -> Ghost guesses instead of asking
29. Rebuild the correct graph and press `Test Ghost's map`; confirm correct green feedback appears plus a Ghost reaction describing the successful route.
30. Re-validate after fixing a wrong graph and confirm the feedback updates.
31. Confirm there are no new Console errors.
32. Confirm there is no backend, LLM, save/load, scoring persistence, Act 3 Shell integration, Act 4-6 node graph, or Build Settings change added by M0-T30.

### Build Settings Check

1. Open `File > Build Profiles` or the Unity 6 Build Settings view.
2. Confirm `Assets/Scenes/Act3DialogGraphPrototype.unity` is not added to Build Settings by M0-T30.

### Inspector Setup

If the scene is created through `Ghost > Build Act 3 Dialog Graph Prototype Scene`, no manual Inspector setup should be required. The builder wires the presenter roots/templates; the presenter creates the interaction controller plus runtime palette/node drag views, input/output dot views, wire objects, and the trash drop zone.

---

## M0-T31: Act 3 Game Shell Integration

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Shell/ShellSceneNames.cs`
   - `Assets/Presentation/Shell/GameShellPresenter.cs`
   - `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
   - `Assets/Presentation/Shell/ShellDialogueData.cs`
   - `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
3. Confirm there are no Console compile errors.

### Scene Generation / Build Settings Check

M0-T31 uses the Game Shell builder and does not hand-write scene YAML.

1. Select `Ghost > Build Game Shell Scene`.
2. Open `Assets/Scenes/GameShellPrototype.unity`.
3. Open `File > Build Profiles` or the Unity 6 Build Settings view.
4. Confirm these scenes are enabled in Build Settings:
   - `Assets/Scenes/GameShellPrototype.unity`
   - `Assets/Scenes/Act1IntentClassificationPrototype.unity`
   - `Assets/Scenes/Act2EntityExtractionPrototype.unity`
   - `Assets/Scenes/Act3DialogGraphPrototype.unity`

### Play Mode Navigation Check

1. Enter Play Mode in `Assets/Scenes/GameShellPrototype.unity`.
2. Click `Start / Continue` to open the hub.
3. Confirm the hub shows:
   - `Start Act 1`
   - `Start Act 2`
   - `Start Act 3`
4. Click `Start Act 1`, confirm Act 1 loads, then click `Return to Hub` and confirm the shell loads again.
5. Click `Start Act 2`, confirm Act 2 loads, then click `Return to Hub` and confirm the shell loads again.
6. Click `Start Act 3`, confirm `Assets/Scenes/Act3DialogGraphPrototype.unity` loads.
7. Confirm the `Return to Hub` overlay appears in Act 3 above the Act 3 UI and loads the shell when clicked.
8. If the Act 3 return button is absent, inspect the scene hierarchy for `Shell Return To Hub Overlay Canvas`.
9. Confirm the Act 3 puzzle still renders and its M0-T30 interaction is not changed by shell integration.
10. Confirm there are no new Console errors.
11. Confirm no backend, LLM, save/load, full visual-novel dialogue, final art, or non-shell Act 3 puzzle changes were added by M0-T31.

### Inspector Setup

If the scene is created through `Ghost > Build Game Shell Scene`, no manual Inspector setup should be required. The builder wires `GameShellPresenter` with the title screen, hub screen, Lily dialogue frame, Start button, Act 1 button, Act 2 button, Act 3 button, and Back-to-title button.

---

## M0-T26: Acts 1-3 Narrative Shell Integration

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Shell/GhostNarrativeState.cs`
   - `Assets/Presentation/Shell/ShellDialogueData.cs`
   - `Assets/Presentation/Shell/LilyDialogueFrame.cs`
   - `Assets/Presentation/Shell/GameShellPresenter.cs`
   - `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
   - `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
3. Confirm there are no Console compile errors.

### Scene Generation Check

M0-T26 uses the Game Shell builder and does not hand-write scene YAML.

1. Select `Ghost > Build Game Shell Scene`.
2. Open `Assets/Scenes/GameShellPrototype.unity`.
3. Confirm the generated shell has:
   - a title screen;
   - a name-entry screen with an input field and confirm button;
   - a hub screen with Act 1, Act 2, and Act 3 cards;
   - a dialogue frame with a speaker portrait placeholder slot;
   - a narrative continue button that is hidden until an intro or queued closing beat needs it.

### Play Mode Narrative Flow Check

1. Enter Play Mode in `Assets/Scenes/GameShellPrototype.unity`.
2. Confirm the title screen shows `Ghost` and a Lily dialogue line.
3. Click `Start / Continue`.
4. Confirm the name-entry screen appears and Lily asks what Ghost should call the player.
5. Enter a test name, then click `Help Ghost`.
6. Confirm the hub opens and Lily's line includes the entered name.
7. Confirm the three act cards are laid out in a row and the Lily dialogue frame remains fully inside the viewport.
8. Click `Start Act 1`.
9. Confirm Act 1 does not load immediately; Lily's Act 1 intro beat appears first.
10. Click `Continue to Act 1`; confirm Act 1 loads.
11. Click `Return to Hub`; confirm the shell opens and Lily's Act 1 debrief appears.
12. Repeat the intro/load/return/debrief flow for Act 2.
13. Repeat the intro/load/return/debrief flow for Act 3.
14. After the Act 3 debrief, click `Continue`.
15. Confirm the speaker switches to Ghost and the closing line appears.
16. Confirm the portrait placeholder switches between Lily and Ghost when the speaker changes.
17. Confirm leaving the name blank uses the fallback name `Junior`.
18. Confirm Acts 1, 2, and 3 puzzle mechanics are unchanged.
19. Confirm there are no new Console errors.
20. Confirm no backend, LLM, save/load, scoring, database, or puzzle-validator changes were added by M0-T26.

### Inspector Setup

If the scene is created through `Ghost > Build Game Shell Scene`, no manual Inspector setup should be required. The builder wires `GameShellPresenter` with the title screen, name-entry screen, hub screen, Lily dialogue frame, Start button, name input, name confirmation button, Act 1/2/3 buttons, narrative continue button, and Back-to-title button. The builder wires `LilyDialogueFrame` with speaker text, dialogue text, portrait Image, and portrait placeholder Text; Lily/Ghost portrait Sprite fields can remain empty.

---

## M0-T32: In-Act Ambient Ghost and Lily Banter

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Banter/BanterData.cs`
   - `Assets/Presentation/Banter/AmbientBanterPanel.cs`
   - `Assets/Presentation/Banter/AmbientBanterHook.cs`
3. Confirm there are no Console compile errors.

### Scene Setup Check

M0-T32 uses a runtime scene-load hook and does not require scene YAML edits or scene regeneration.

1. Confirm no Inspector setup is required.
2. Confirm the authored Act 1, Act 2, and Act 3 scenes do not need banter GameObjects added manually.
3. In Play Mode, inspect the hierarchy after entering an act and confirm runtime objects appear:
   - `Ambient Banter Panel`
   - a temporary `Ambient Banter Bootstrapper` may appear briefly after scene load, then destroy itself
4. Confirm the panel is embedded into existing act UI where possible:
   - Act 1: inside `Validation Controls`, using the taller Act 1 banter style
   - Act 2: inside `Validation Controls`, using the slimmer Act 2 banter style
   - Act 3: inside `Goal Test List`, using the taller Act 3 guide style
5. Confirm `Ambient Banter Canvas` is not created unless the expected act UI host cannot be found.

### Play Mode Banter Check

1. Enter Play Mode from `Assets/Scenes/GameShellPrototype.unity`.
2. Enter a player name, then open the act hub.
3. Launch Act 1.
4. Confirm a compact ambient banter panel appears in the bottom validation area, not as a floating overlay covering cards or drop targets.
5. Confirm Act 1 lines cycle and loop automatically.
6. Confirm the `Ask Lily` button opens a dedicated Lily chat window without blocking the puzzle.
7. Confirm Act 1 includes nervous Lily lines and garbled Ghost lines, with at least 15 Lily lines and 15 Ghost lines available in the loop.
8. Confirm at least one line addresses the player by the entered name.
9. Confirm Act 1 banter text is not vertically cut off.
10. Confirm Act 1 puzzle controls remain fully playable with the panel present.
11. Return to the hub and launch Act 2.
12. Confirm Act 2 banter appears in the bottom validation area and cycles/loops without covering chips or palette controls.
13. Confirm the Act 2 banter box is visibly slimmer than the earlier oversized version while still readable and not overlapping fixed validation feedback.
14. Confirm Lily is warmer, Ghost catches details, and the first joke/backpedal beat appears, with at least 15 Lily lines and 15 Ghost lines available in the loop.
15. Confirm Act 2 puzzle controls remain fully playable.
16. Return to the hub and launch Act 3.
17. Confirm Act 3 banter appears in the right-side guide/test area, not over the graph board, palette, wires, trash zone, or Test Ghost's map controls.
18. Confirm Act 3 banter cycles/loops.
19. Confirm Act 3 banter text is not vertically cut off.
20. Confirm Lily is more comfortable/jokier, including a nerdy-joke-then-embarrassed beat, with at least 15 Lily lines available in the loop.
21. Confirm Ghost lines are clearer and ask-like, matching the Act 3 stage, with at least 15 Ghost lines available in the loop.
22. Confirm Act 3 puzzle controls remain fully playable.
23. Confirm the return-to-hub overlay still appears and works in all three acts.
24. Confirm there are no new Console errors.
25. Confirm no LLM, backend, save/load, player-choice branching, final art, or puzzle logic changes were added by M0-T32.

### Inspector Setup

No manual Inspector setup is required. `AmbientBanterHook` waits for each Act 1, Act 2, or Act 3 presenter to render, then embeds the runtime panel into existing UI layout hosts. Portrait Sprite fields are empty runtime placeholders for now; future art can replace the labelled boxes in a later task.

---

## M0-T27: Backend / Database Foundation

### Backend Setup and Automated Tests

1. Open a terminal in `Backend/`.
2. Run `npm install`.
3. Run `npm run build`.
4. Run `npm test`.
5. Expected backend tests:
   - `GET /content returns seeded acts without scoring answer keys`
   - `profile progress can be created, updated, and read back`
   - `POST /attempts stores an attempt for an existing profile`
   - `POST /hints falls back to a static hint and logs when Ollama is unavailable`
   - `POST /responses falls back to static Ghost text when Ollama is unavailable`

### Expected Result

The backend install, TypeScript build, and test suite should complete successfully. The server remains local-only and does not score puzzle submissions.

### Unity Play Mode Check

M0-T27 adds a top-level `Backend/` service only. There is no Unity Play Mode behaviour, scene setup, Inspector setup, or Unity client wiring in this task.

### Inspector Setup

No Inspector setup is required. Do not add backend objects to scenes for M0-T27.

---

## M0-T28: Unity Client Backend Integration

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Backend/GhostBackendConfig.cs`
   - `Assets/Presentation/Backend/GhostBackendClient.cs`
   - `Assets/Presentation/Backend/BackendSync.cs`
   - `Assets/Presentation/Shell/GhostNarrativeState.cs`
   - `Assets/Presentation/Shell/GameShellPresenter.cs`
   - the Act 1, Act 2, and Act 3 interaction controllers
3. Confirm there are no Console compile errors.

### Backend-Up Play Mode Check

1. In a terminal, open `Backend/`.
2. Run `npm install` if dependencies are missing.
3. Run `npm run dev`.
4. Open `Assets/Scenes/GameShellPrototype.unity`.
5. Enter Play Mode.
6. Confirm the client creates or reuses a PlayerPrefs profile id.
7. Enter a player name and reach the hub.
8. Return from at least one act so it becomes completed.
9. Stop and restart Play Mode with the backend still running.
10. Confirm progress is fetched and applied from the backend where available.
11. In Act 1, click Validate and confirm an attempt row is inserted in the backend database.
12. Repeat Validate in Act 2 and Act 3 and confirm attempts are inserted with `act1`, `act2`, and `act3` ids and `correct`/`incorrect` result strings.
13. Confirm the backend stores only analytics/progress; correctness feedback still comes from the existing Unity validators.

### Backend-Down Graceful Degradation Check

1. Stop the backend server.
2. Enter Play Mode again.
3. Confirm the shell still opens and the player can enter a name.
4. Confirm Act 1, Act 2, and Act 3 are fully playable.
5. Confirm Validate in each act still produces the same deterministic feedback as before.
6. Confirm backend failures produce at most warning logs and no gameplay-blocking Console errors.
7. Confirm there is no hang longer than the configured short request timeout.

### Validator / Puzzle Rule Regression Check

1. Confirm no files under `Assets/Scripts/Puzzles/` changed for M0-T28.
2. Run the existing EditMode tests if desired; M0-T28 does not change validators or sessions.
3. Confirm no LLM, backend scoring endpoint, or backend-served puzzle content replacement was added.

### Inspector Setup

No manual Inspector setup is required. `BackendSync` starts from runtime hooks, `GameShellPresenter` ensures sync once, and `GhostBackendClient` creates its hidden coroutine runner automatically. To use a non-default backend URL, set `GhostBackendConfig.BaseUrl` or the `Ghost.Backend.BaseUrl` PlayerPrefs value.

---

## M0-T29: LLM Orchestration for Lily Hints and Ghost Responses

### Backend Automated Check

1. Open a terminal in `Backend/`.
2. Run `npm install`.
3. Run `npm run build`.
4. Run `npm test`.
5. Expected fallback/logging tests:
   - `/hints` returns HTTP 200 with `source: "static"` when Ollama is unavailable.
   - `/hints` inserts a `hint_logs` row with trigger and non-spoiler state summary.
   - `/hints` can return mocked `source: "llm"` and log trigger/state without requiring live Ollama.
   - `/responses` returns HTTP 200 with static Ghost text when Ollama is unavailable.
6. Run `npm run check:ollama` to verify whether local Ollama and the configured Granite model are available, and to see timed test generation latency.

### Ollama-Up Play Mode Check

1. Install and start Ollama.
2. In `Backend/`, run `ollama pull granite3.1-dense:2b`.
3. Run `npm run check:ollama` and confirm the model is available.
4. Run `npm run dev`.
5. Open `Assets/Scenes/GameShellPrototype.unity` and enter Play Mode.
6. Enter a player name and launch Act 1.
7. Click `Ask Lily`.
8. Confirm M0-T33 supersedes the M0-T29 one-shot hint UI: `Ask Lily` now opens a dedicated chat window rather than replacing the banter line.
9. Confirm closing the chat resumes the ambient banter loop, and the puzzle remains playable while a longer local Granite request is in flight.
10. Validate an incorrect Act 1 grouping and confirm Lily opens/uses the chat window after the deterministic incorrect feedback.
11. Repeat `Ask Lily` and incorrect Validate in Act 2 and Act 3.
12. Confirm the backend writes `hint_logs` rows with `trigger` values such as `ask_lily_button` and `after_incorrect_validate`, plus a non-spoiler state summary.
13. Confirm puzzle correctness and progression still come only from the existing validators.

### Ollama-Down / Backend-Down Fallback Check

1. Stop Ollama but keep the backend running.
2. Click `Ask Lily` in each act.
3. Confirm static Lily support appears through the M0-T33 chat window and gameplay continues.
4. Stop the backend.
5. Click `Ask Lily` and validate incorrectly in each act.
6. Confirm the Unity client shows local static Lily chat lines, with Close resuming ambient banter, and never blocks play.
7. Confirm network failures produce at most warning logs.

### Validator / Scope Regression Check

1. Confirm no files under `Assets/Scripts/Puzzles/` changed.
2. Confirm there is still no backend scoring endpoint.
3. Confirm `/hints` and `/responses` generate/display text only.
4. Confirm no ProjectSettings, Packages, Build Settings, scenes, or `.meta` files were edited for M0-T29.

### Inspector Setup

No manual Inspector setup is required. `AmbientBanterHook` creates the `Ask Lily` affordance at runtime, `GhostBackendClient` hosts backend requests on its hidden runner, and `BanterData.GetStaticHint(...)` supplies local fallback hints.

---

## M0-T28 Run 002: No-Password Account Recovery

### Backend Automated Check

1. Open a terminal in `Backend/`.
2. Run `npm run build`.
3. Run `npm test`.
4. Expected account tests:
   - `POST /accounts links a username to an existing profile and lookup restores progress`
   - `POST /accounts rejects duplicate usernames`
5. Confirm existing content/profile/progress/attempt/hint/chat tests still pass.

### CMD / REST Manual Check

1. Start the backend with `npm run dev`.
2. Create a no-password account:
   ```powershell
   Invoke-RestMethod `
     -Method Post `
     -Uri "http://localhost:3000/accounts" `
     -ContentType "application/json" `
     -Body '{"userName":"chao_test","displayName":"Chao"}'
   ```
3. Copy the returned `accountId` and `profileId`.
4. Recover by username:
   ```powershell
   Invoke-RestMethod `
     -Method Post `
     -Uri "http://localhost:3000/accounts/lookup" `
     -ContentType "application/json" `
     -Body '{"identifier":"chao_test"}'
   ```
5. Recover by account id:
   ```powershell
   Invoke-RestMethod `
     -Method Post `
     -Uri "http://localhost:3000/accounts/lookup" `
     -ContentType "application/json" `
     -Body '{"identifier":"account_REPLACE_WITH_ID"}'
   ```
6. Read progress with `GET /progress/:profileId` and confirm it is the same profile id.

### Unity UI Play Mode Check

1. Run `Ghost > Build Game Shell Scene` so the name-entry UI includes account controls.
2. Start the backend with `npm run dev`.
3. Open `Assets/Scenes/GameShellPrototype.unity` and enter Play Mode.
4. Click `Start / Continue`.
5. Confirm the name-entry screen shows `Continue as Guest`, `Create Account`, and `Use Account`.
6. Enter a player display name.
7. Enter a username, for example `chao_test`.
8. Click `Create Account`.
9. Confirm the shell reaches the hub and the status reports the username/account id.
10. Complete/return from at least one act so progress is saved.
11. Restart Play Mode.
12. Click `Start / Continue`, enter the same username, and click `Use Account`.
13. Confirm the restored profile/name/completed acts appear.
14. On the same local profile, enter a new unused username and click `Create Account`; confirm it creates a second account/profile instead of overwriting the old account.
15. Use SQLite or `/accounts/lookup` to confirm the old username and new username both still exist.
16. Try a username that belongs to another profile and confirm Unity shows a clear duplicate-name message.
17. Stop the backend and confirm `Continue as Guest` still reaches the hub.

### Security / Scope Check

1. Confirm this is no-password prototype recovery only, not secure authentication.
2. Confirm no puzzle validators/sessions/rules changed.
3. Confirm no ProjectSettings, Packages, Build Settings, scenes, or `.meta` files were intentionally edited.

### Inspector Setup

No manual Inspector setup is required after running `Ghost > Build Game Shell Scene`. The builder wires the player-name input, account identifier input, `Create Account`, `Use Account`, status text, and guest continue button into `GameShellPresenter`.

---

## M0-T33: Constrained Lily Chat Window

### Backend Automated Check

1. Open a terminal in `Backend/`.
2. Run `npm run build`.
3. Run `npm test`.
4. Expected tests:
   - Existing content/profile/progress/attempt tests still pass.
   - `/hints` fallback and mocked LLM tests still pass.
   - `/responses` fallback still passes.
   - `/chat` returns HTTP 200 with `source: "static"` when Ollama is unavailable.
   - `/chat` inserts a `hint_logs` row with `kind:"chat"`, `trigger:"chat_message"`, and the player message/topic.

### Chat Window Play Mode Check

1. Start Ollama and the backend, then enter Play Mode from `Assets/Scenes/GameShellPrototype.unity`.
2. Enter a player name and launch Act 1.
3. Click `Ask Lily`.
4. Confirm a dedicated `Lily Chat Window` opens with a scrollable message list, text input, Send button, and Close button.
5. Confirm the ambient banter strip pauses while the chat window is open.
6. Type an on-topic question about the current act and send it.
7. Confirm Lily replies in one short, hesitant, in-character sentence and does not reveal the exact answer.
8. Ask a private-life question and confirm Lily gives a flustered/annoyed deflection.
9. Ask an off-topic question and confirm Lily redirects toward helping Ghost.
10. Close the chat window and confirm ambient banter resumes.
11. Repeat Ask Lily in Act 2 and Act 3.
12. Trigger an incorrect Validate in each act and confirm it opens/uses the chat window with a Lily opening line.
13. Confirm `hint_logs` contains `kind:"chat"` rows.

### Fallback / Layout Check

1. Stop Ollama but keep the backend running.
2. Send a Lily chat message and confirm the backend returns/static displays an in-character fallback line.
3. Stop the backend.
4. Send a Lily chat message and confirm the Unity client appends a local static Lily line.
5. Confirm puzzle controls remain playable and correctness still comes only from deterministic validators.
6. Confirm the Act 2 ambient banter box is slimmer, readable, and does not overlap fixed validation feedback.
7. Confirm no ProjectSettings, Packages, Build Settings, scenes, `.meta`, or deterministic puzzle logic files were edited for M0-T33.

### Inspector Setup

No manual Inspector setup is required. `AmbientBanterHook` creates the banter affordance at runtime, `LilyChatWindow` creates its own runtime canvas/window when Ask Lily is opened, and `GhostBackendClient.PostChat(...)` sends best-effort chat requests through UnityWebRequest.

---

## M0-T35: Game Shell Chatbot Fundamentals Sequence

### Scene Generation

1. Open the Unity project and wait for scripts to compile.
2. Run `Ghost > Build Game Shell Scene`.
3. Open `Assets/Scenes/GameShellPrototype.unity`.
4. Confirm the scene was generated through the builder, not by hand-editing scene YAML.

### Play Mode Check

1. Enter Play Mode from `Assets/Scenes/GameShellPrototype.unity`.
2. Click `Start / Continue`, enter a display name or continue as guest, and reach the act hub.
3. Confirm the hub shows a `Ghost's Voice Basics` / `Start Basics` entry alongside Acts 1-3.
4. Open `Ghost's Voice Basics`.
5. Confirm the six fundamentals appear in order:
   - chatbot definition
   - NLP and ML pillars
   - rule-based vs AI-enabled contrast
   - benefits / repetitive tasks
   - five-component overview with backend side link
   - four chatbot challenges
6. For each beat, confirm there is a Ghost problem, a short Lily explanation, a player action, and a visible Ghost consequence.
7. Confirm `Next` does not advance until the current action has been tried.
8. In the five-component beat, arrange the path as `UI input -> NLP engine -> Dialogue management -> Response generation -> UI output`, attach the backend side link, and confirm Ghost's voice connects.
9. In the challenge beat, trigger all four failure modes and confirm each produces a different Ghost reaction.
10. Confirm `Skip overview` returns to the act hub.
11. Finish the sequence and confirm it returns to the act hub.
12. Launch Acts 1, 2, and 3 from the hub and confirm their existing puzzle mechanics still open.
13. Confirm no Console errors appear.

### Scope Check

1. Confirm this is a playable overview, not a wall-of-text lecture or multiple-choice quiz.
2. Confirm Acts 1-3 validators, sessions, and puzzle rules are unchanged.
3. Confirm no ProjectSettings, Packages, Build Settings, `.meta` files, or unrelated scenes were intentionally edited.

### Inspector Setup

No manual Inspector setup is required after running `Ghost > Build Game Shell Scene`. The builder wires the fundamentals screen, presenter, dynamic button roots, navigation buttons, shared Lily dialogue frame, and hub entry button into `GameShellPresenter`.

---

## M0-T36: Act 1 Intent Teaching Layer

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
   - `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
3. Confirm there are no Console compile errors.

### Scene Refresh Check

M0-T36 updates the existing Act 1 presenter and should apply at Play Mode startup. If the saved scene
preview looks stale:

1. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
2. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
3. Confirm the scene was generated through the builder, not by hand-editing scene YAML.

### Play Mode Teaching Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity` directly, or enter it from the Game Shell.
2. Enter Play Mode.
3. Confirm a visually distinct `Lily's Intent Note` panel appears under the subtitle, with a warm background and outline rather than plain grey subtitle text.
4. Confirm the Lily note explains Ghost's exact-word problem, intent as what the visitor wants, and varied phrasings as training examples.
5. Confirm the full Act 1 layout fits inside the game view at 1920x1080: the bottom validation / banter area is visible without scrolling or resizing the window.
6. Confirm all nine unassigned message cards fit in the left panel at the start of Play Mode.
7. Confirm all three intent groups plus the validation controls fit in the right panel at the start of Play Mode.
8. Confirm the three group titles are player-facing purpose labels, not raw intent ids:
   - `Purpose: find something`
   - `Purpose: locate Ghost`
   - `Purpose: identify Ghost`
9. Confirm the three group hints are phrased as visitor purposes:
   - visitors want Ghost to help find something
   - visitors want to know where Ghost is
   - visitors want to know who Ghost is or what to call Ghost
10. Confirm click-to-assign, drag-to-assign, drag back to unassigned, drag between groups, `Back:` row unassign, and `Validate` still work.
11. Click `Validate` with an incomplete or wrong grouping.
12. Confirm the incorrect-path feedback and Lily hint behaviour are unchanged, and the validation panel uses an incorrect-state colour.
13. Assign all cards to the correct groups and click `Validate`.
14. Confirm the validation panel changes into a green success-teaching state.
15. Confirm the correct feedback shows a small happy Ghost reaction.
16. Confirm the correct feedback explains that differently worded cards in a group share one intent / purpose.
17. Confirm the correct feedback explains that the varied message cards are training examples / example phrasings for a chatbot to learn the intents.
18. Confirm the correct feedback includes one Lily line connecting grouped intents and training examples to spotting common visitor requests before planning a chatbot.
19. Confirm the ambient banter panel, if present, does not cover the teaching panel or make the success feedback unreadable.
20. Confirm there are no Console errors.

### Scope Check

1. Confirm `IntentClassificationValidator`, `IntentClassificationSession`, `Act1IntentClassificationSampleData`, card wording, answer keys, and validation rules are unchanged.
2. Confirm there is no quiz, new planning mechanic, Act 2/Act 3 change, backend scoring change, ProjectSettings edit, Packages edit, Build Settings edit, or `.meta` edit.

### Inspector Setup

No manual Inspector setup is required. The existing `Act1IntentClassificationStaticPresenter` continues
to create and wire instruction text, intent groups, card lists, drag/drop affordances, Validate, and
feedback from its existing serialized roots/templates.

---

## M0-T37: Act 2 Entity Extraction Teaching Layer

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
3. Confirm there are no Console compile errors.

### Scene Refresh Check

M0-T37 updates the existing Act 2 presenter and should apply at Play Mode startup. If the saved scene
preview looks stale:

1. Select `Ghost > Build Act 2 Entity Extraction Prototype Scene`.
2. Open `Assets/Scenes/Act2EntityExtractionPrototype.unity`.
3. Confirm the scene was generated through the builder, not by hand-editing scene YAML.

### Play Mode Teaching Check

1. Open `Assets/Scenes/Act2EntityExtractionPrototype.unity` directly, or enter it from the Game Shell.
2. Enter Play Mode with the Game view at 1920x1080.
3. Confirm a visually distinct `Lily's Entity Note` panel appears under the subtitle with a warm background and outline.
4. Confirm the Lily note explains Ghost hearing the whole sentence but missing useful details.
5. Confirm the Lily note defines entity extraction / NER as spotting useful details and lightly says the chips are word tokens.
6. Confirm the message area title reads `Message Word Tokens`.
7. Confirm the palette title reads `Entity Kinds`.
8. Confirm the `time` entry explains a System entity as a broadly usable kind, like time.
9. Confirm the `room` entry explains a Custom entity as lab-specific room words and includes the real `lab` / `laboratory` synonym pair from sample data.
10. Confirm the `object` entry explains a Custom entity as this lab's own object words.
11. Confirm the chips, entity legend, validation controls, and ambient banter panel remain visible inside the 1920x1080 Game view without cropping.
12. Select `lab`, assign `room`, select `9pm`, assign `time`, then click `Validate spans`.
13. Confirm correct feedback appears as a compact teaching beat with:
    - a cute Ghost reaction to noticing details
    - an NER line about key details a chatbot must act on
    - a synonym line using `lab` / `laboratory`
    - a tokenization / Act 1 intent / Act 3 slots bridge
14. Click `Validate spans` with no tags, partial tags, or a wrong/extra tag.
15. Confirm the incorrect-path feedback remains the existing non-spoiler wording with the issue count and still requests a Lily hint.
16. Confirm chip select, assign, untag, reassign, and multiple tagged chips still work.
17. Confirm attempt logging still fires through the existing backend client path if that client is available.
18. Confirm there are no Console errors.

### Scope Check

1. Confirm `EntityExtractionValidator`, `EntityExtractionSession`, `Act2EntityExtractionSampleData`, answer keys, span boundaries, and validation rules are unchanged.
2. Confirm the Act 2 Editor scene builder, Act 1, Act 3, backend scoring, ProjectSettings, Packages, Build Settings, and `.meta` files are unchanged.
3. Confirm there is no quiz, new tokenization mini-game, new Act structure, LLM scoring, save/load, or puzzle-rule change.

### Inspector Setup

No manual Inspector setup is required. The existing `Act2EntityExtractionStaticPresenter` creates the
teaching note, token labels, entity-kind subtitles, chip buttons, palette buttons, Validate button, and
feedback from its existing serialized roots/templates.

---

## M0-T45 Run 001: Act 1 Teaching-as-Gameplay + Ghost Face

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Scripts/Puzzles/IntentClassification/Act1TeachingDemoData.cs`
   - `Assets/Scripts/Puzzles/IntentClassification/Act1GhostGeneralizationEngine.cs`
   - `Assets/Tests/EditMode/Act1GhostGeneralizationEngineTests.cs`
   - `Assets/Presentation/GhostAvatar/GhostMood.cs`
   - `Assets/Presentation/GhostAvatar/GhostFaceView.cs`
   - `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationLabelDragView.cs`
   - `Assets/Presentation/Act1IntentClassification/Act1IntentTeachingDropTarget.cs`
   - `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
   - `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
   - `Assets/Presentation/Act1IntentClassification/Editor/Act1IntentClassificationPrototypeSceneBuilder.cs`
3. Confirm there are no Console compile errors.

### EditMode Test Check

1. Open Unity Test Runner.
2. Run all existing EditMode tests plus `Act1GhostGeneralizationEngineTests`.
3. Confirm correct grouping makes all unseen test visitors correct.
4. Confirm scattered, tied, and unlabelled pile cases produce the expected wrong/confused results.

### Scene Refresh Check

1. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
2. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
3. Confirm the scene was generated through the builder, not hand-edited scene YAML.

### Play Mode Teaching Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity` directly, or enter it from the Game Shell.
2. Enter Play Mode with the Game view at 1920x1080.
3. Confirm the top conversation panel shows the shared Ghost face.
4. Step through the intro failures with `Next`; confirm Ghost gives wrong scripted replies and the face is confused.
5. Confirm Lily's short line explains that Ghost memorizes sentences but does not understand purpose.
6. Click `Help Ghost` and confirm the build phase appears with transcript cards, purpose-label chips, a new-pile drop zone, and an empty pile area.
7. Drag a transcript card to the new-pile zone and confirm a new training pile appears.
8. Drag more transcript cards onto existing piles and confirm they join that pile.
9. Drag a piled card back to the transcript list and confirm it becomes unpiled.
10. Click a card, then click/drop it onto a pile or the new-pile zone and confirm click assignment also works.
11. Drag or click a purpose-label chip (`find something`, `where is Ghost`, `who is Ghost`) onto a pile and confirm the label socket updates.
12. Move a label from one pile to another and confirm the previous pile loses that label.
13. Press `Teach Ghost` with incomplete, scattered, unlabelled, or wrongly labelled piles.
14. Confirm unseen visitor messages play one at a time in the conversation panel.
15. Confirm Ghost replies according to the current piles, not the answer key directly.
16. Confirm wrong/confused demo outcomes show a confused Ghost face and highlight misleading training cards in the build area.
17. Click `Revise piles`, fix the piles/labels, and press `Teach Ghost` again.
18. Confirm fixing the piles changes Ghost's unseen-message replies.
19. Build the three correct labelled piles and teach Ghost.
20. Confirm every unseen visitor is answered correctly and Ghost reaches the completion state.
21. Confirm completion only occurs when the existing validator-correct pile structure is also demo-correct.
22. Confirm there is no success lecture wall replacing the behaviour demo.
23. Confirm the full Act 1 screen fits inside 1920x1080: title, Lily note, conversation panel, cards, piles, Teach/Revise controls, and feedback are visible without cropping.
24. Confirm there are no Console errors.

### Scope Check

1. Confirm existing `IntentClassificationValidator`, `IntentClassificationSession`, `Act1IntentClassificationSampleData`, answer keys, and existing Act 1 tests are unchanged.
2. Confirm Act 2, Act 3, Fundamentals, Shell, Banter, Backend, ProjectSettings, Packages, Build Settings, and existing `.meta` files are unchanged by this run.
3. Confirm there is no quiz, LLM scoring, backend scoring change, external art asset, audio, or Act 2 rebuild in Run 001.

### Inspector Setup

No manual Inspector setup is required if the scene is generated through
`Ghost > Build Act 1 Intent Classification Prototype Scene`. The builder still wires the existing
`Act1IntentClassificationStaticPresenter` roots/templates, and the presenter creates the conversation
panel, shared Ghost face, label chips, free piles, drop targets, Teach/Revise buttons, and feedback at
runtime.

---

## M0-T45 Run 002: Act 2 Ghost's Errand Redesign

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Scripts/Puzzles/EntityExtraction/Act2ErrandDemoData.cs`
   - `Assets/Scripts/Puzzles/EntityExtraction/Act2ErrandOutcomeEngine.cs`
   - `Assets/Tests/EditMode/Act2ErrandOutcomeEngineTests.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityTokenDragView.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntitySlotDropTarget.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityTokenReturnDropTarget.cs`
   - `Assets/Presentation/Act2EntityExtraction/Editor/Act2EntityExtractionPrototypeSceneBuilder.cs`
3. Confirm there are no Console compile errors.

### EditMode Test Check

1. Open Unity Test Runner.
2. Run all existing EditMode tests plus `Act2ErrandOutcomeEngineTests`.
3. Confirm all-correct errand spans succeed.
4. Confirm missing WHEN and wrong WHAT return their authored failure outcomes.
5. Confirm `laboratory` tagged as room succeeds and displays `laboratory -> lab room` resolution data.

### Scene Refresh Check

1. Select `Ghost > Build Act 2 Entity Extraction Prototype Scene`.
2. Open `Assets/Scenes/Act2EntityExtractionPrototype.unity`.
3. Confirm the scene was generated through the builder, not by hand-editing scene YAML.

### Play Mode Teaching Check

1. Open `Assets/Scenes/Act2EntityExtractionPrototype.unity` directly, or enter it from the Game Shell.
2. Set the Game view to 1920x1080 and enter Play Mode.
3. Confirm the first visible interaction is Lily's short how-this-level-works beat with one
   `Watch Ghost fail` button.
4. Confirm the persistent objective strip is always visible and updates across onboarding, intro
   failure, fill, run, and completion phases.
5. Click `Watch Ghost fail` and confirm the conversation panel shows the current visitor note,
   Ghost's authored wrong errand outcome, and a sad Ghost face.
6. Confirm the message first appears as one solid sentence.
7. Click `Split` and confirm the sentence becomes word-token chips.
8. Confirm only slots required by the current errand appear on Ghost's action card.
9. Confirm custom slots (WHAT / WHERE) and the system slot (WHEN) have visibly different slot chrome.
10. Drag a token chip into a slot and confirm the slot fills, the token remains traceable, and the
    underlying span is created through the existing session.
11. Click a token then click a slot and confirm click assignment also works.
12. Drag an assigned slot token back to the token area, or re-drop the same token into the same slot,
    and confirm the slot clears.
13. Press `Go, Ghost!` with missing or wrong slots and confirm Ghost shows the authored cute failure,
    slot states mark Correct / Missing / Wrong, and the face becomes sad or confused.
14. Click `Revise card`, fix the slots, and press `Go, Ghost!` again.
15. Confirm a correct errand shows the authored success outcome and happy Ghost face.
16. Continue to the `laboratory` errand, drop `laboratory` into WHERE, and confirm the slot shows
    `laboratory -> lab room`.
17. Complete all errands and confirm completion appears only after all errands validate through the
    existing `EntityExtractionValidator`.
18. Confirm the full Act 2 screen fits inside 1920x1080: title, objective strip, Lily note,
    conversation panel, token area, action card, and bottom buttons are visible without cropping.
19. Confirm there is no M0-T37 four-line success-teaching text replacing the errand outcome.
20. Confirm there are no Console errors.

### Scope Check

1. Confirm existing `EntityExtractionValidator`, `EntityExtractionSession`,
   `Act2EntityExtractionSampleData`, answer keys, span boundaries, and existing tests are unchanged.
2. Confirm Act 1, Act 3, Fundamentals, Shell, Banter, Backend, ProjectSettings, Packages,
   Build Settings, and existing `.meta` files are unchanged by this run.
3. Confirm there is no quiz, LLM scoring, backend scoring change, external art asset, audio, or new
   Act structure.

### Inspector Setup

No manual Inspector setup is required if the scene is generated through
`Ghost > Build Act 2 Entity Extraction Prototype Scene`. The builder creates the canvas and presenter
root; the presenter creates the onboarding panel, objective strip, shared Ghost face, token chips,
drop targets, action-card slots, `Go, Ghost!`, revise, next-errand, and completion controls at
runtime.

---

## M0-T45 Run 003: Play Mode Feedback Fixes

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Common/FloatingWindowDragHandle.cs`
   - `Assets/Presentation/GhostAvatar/GhostFaceView.cs`
   - `Assets/Presentation/Banter/LilyChatWindow.cs`
   - `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
3. Confirm there are no Console compile errors.

### Ghost Face Console Check

1. Open Act 1 or Act 2 in Play Mode.
2. Confirm the shared Ghost face renders normally.
3. Confirm the Console no longer logs missing `UI/Skin/UISprite.psd` or `UI/Skin/Knob.psd` resource errors.

### Floating Lily Chat Check

1. Enter any act that shows the ambient banter `Ask Lily` button.
2. Click `Ask Lily`.
3. Confirm the Lily chat opens near the right side of the Game view.
4. Drag the chat header to another area of the screen.
5. Confirm the window stays inside the Game view and no longer permanently blocks the puzzle area.
6. Close the chat and confirm ambient banter resumes.

### Act 1 Completion Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity` directly, or enter Act 1 from the Game Shell.
2. Complete the level by building the three correct labelled training piles and teaching Ghost.
3. Confirm the completion state shows a visible `Complete Act` button.
4. Click `Complete Act`.
5. Confirm the Game Shell loads and plays/handles the Act 1 debrief through the existing Shell flow.

### Scope Check

1. Confirm existing intent/entity validators, sessions, sample data, and answer keys are unchanged.
2. Confirm ProjectSettings, Packages, Backend, Fundamentals, Act 3, and Build Settings are unchanged by this run.
3. Confirm no quiz, LLM scoring, backend scoring change, external art asset, audio, or new Act structure was added.

---

## M0-T45 Run 004: Retry / Floating Banter / Lily Pixel Portrait

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Characters/LilyPixelPortraitFactory.cs`
   - `Assets/Presentation/Banter/AmbientBanterHook.cs`
   - `Assets/Presentation/Banter/AmbientBanterPanel.cs`
   - `Assets/Presentation/Shell/LilyDialogueFrame.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
3. Confirm there are no Console compile errors.

### Act 2 Retry Check

1. Open Act 2 in Play Mode and reach the token/slot fill phase.
2. Put a wrong token into WHAT, WHERE, or WHEN, or leave a required slot empty.
3. Press `Go, Ghost!`.
4. Confirm Ghost shows the authored failure outcome and the relevant slot result remains visible.
5. Confirm the slots and token chips are immediately editable again without needing a separate `Revise card` button.
6. Confirm the action button reads `Try again`.
7. Fix the slot and press `Try again`.
8. Confirm a correct errand advances with `Next errand` / `Complete` as before.

### Floating Ambient Banter Check

1. Enter Act 1, Act 2, or Act 3 in Play Mode.
2. Confirm the normal Lily/Ghost ambient banter panel appears as a floating window near the bottom or side.
3. Drag the panel by its background/portrait/text area and confirm it moves inside the Game view.
4. Confirm the panel no longer permanently covers bottom puzzle controls.
5. Click `Ask Lily` and confirm the separate Lily chat still opens.
6. Close Lily chat and confirm ambient banter resumes.

### Lily Pixel Portrait Check

1. Open the Game Shell and trigger a Lily dialogue line.
2. Confirm the portrait area shows Lily's generated pixel portrait instead of the text-only `Lily` placeholder.
3. Enter any act with ambient banter and wait for a Lily line.
4. Confirm the ambient banter portrait also shows the generated Lily pixel portrait.

### Scope Check

1. Confirm existing intent/entity validators, sessions, sample data, and answer keys are unchanged.
2. Confirm ProjectSettings, Packages, Backend, Fundamentals, Act 3 logic, and Build Settings are unchanged by this run.
3. Confirm the Lily portrait is an original generated pixel sprite and no external art asset was imported.

---

## M0-T45 Run 005: Drag Preview Cleanup / Lily Style Correction

### Import / Compile Check

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile:
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityTokenDragView.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntitySlotDropTarget.cs`
   - `Assets/Presentation/Act2EntityExtraction/Act2EntityTokenReturnDropTarget.cs`
   - `Assets/Presentation/Characters/LilyPixelPortraitFactory.cs`
3. Confirm there are no Console compile errors.

### Act 2 Drag Preview Cleanup Check

1. Open Act 2 in Play Mode and reach the token/slot fill phase.
2. Drag a token over WHAT / WHERE / WHEN, then drop it into a slot.
3. Confirm the temporary yellow drag preview disappears immediately after drop.
4. Drag several different tokens quickly across the action card.
5. Confirm no old preview boxes remain stuck in Ghost's action card or token area.
6. Drag an assigned token back to the token area and confirm the preview disappears there too.

### Lily Style Check

1. Open the Game Shell and show a Lily dialogue line.
2. Confirm Lily's generated pixel portrait has gold short hair and glasses.
3. Confirm the visible outfit reads as blue suit jacket, white shirt, black long pants, and black shoes.
4. Enter any act with ambient banter and confirm the same Lily portrait appears there.

### Scope Check

1. Confirm existing validators, sessions, sample data, and answer keys are unchanged.
2. Confirm no external art asset was imported.
3. Confirm ProjectSettings, Packages, Backend, Fundamentals, Act 3 logic, and Build Settings are unchanged by this run.

---

## M0-T46 Run 001: Acts 1 and 3 Experience Unification

### Import / Compile Check

1. Open the Ghost Unity project and wait for script compilation.
2. Confirm these four modified scripts import without errors:
   - `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
   - `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
   - `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
   - `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
3. Confirm no Console errors appear.

### Act 1 Onboarding / Objective Check

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity` or enter Act 1 from the Shell.
2. Set the Game view to 1920x1080 and enter Play Mode.
3. Confirm the top-level order matches Act 2: header with right-side progress, objective strip,
   onboarding/Lily note, Ghost conversation, then puzzle body.
4. Confirm the first actionable screen is `Lily's quick training loop` with three short Lily lines and
   one `Watch Ghost fail` button.
5. Confirm the Ghost conversation panel remains visible below onboarding and previews the exact-word
   problem, but has no active advance control yet.
6. Confirm transcript cards, piles, and teaching controls cannot be used before dismissing onboarding.
7. Confirm the persistent strip reads the setup objective and does not reveal card placements.
8. Click `Watch Ghost fail`; confirm the existing intro failure begins.
9. Confirm onboarding changes into the compact Lily note strip with a `Replay Lily` button.
10. Advance the intro, create one training pile, then click `Replay Lily`.
11. Confirm the full onboarding and Ghost problem preview return; dismiss it again and confirm the
    existing pile is preserved.
12. Confirm the strip moves through:
   - `1/3 Watch Ghost fail...`
   - `2/3 Build + label training piles...`
   - `3/3 Teach Ghost and check...`
13. Build the three correct labelled piles, run the unseen visitors, and confirm the completion objective
   and existing `Complete Act` flow still work.
14. Confirm the full screen remains visible at 1920x1080 with no clipped bottom controls.

### Act 3 Onboarding / Objective Check

1. Open `Assets/Scenes/Act3DialogGraphPrototype.unity` or enter Act 3 from the Shell.
2. Set the Game view to 1920x1080 and enter Play Mode.
3. Confirm the top-level order matches Act 2: header with right-side progress, objective strip,
   onboarding/Lily note, Ghost conversation, then graph body.
4. Confirm Lily's onboarding appears before the graph can be edited, with three short lines explaining
   intent branch, Act 2 detail/slot check, response, and test steps.
5. Confirm the panel has one `Build the map` dismiss button.
6. Confirm the Ghost conversation panel below Lily explains that Ghost recognizes the request and room
   detail but replies before checking what it knows.
7. Confirm the setup objective is visible and does not reveal the finished wiring.
8. Click `Build the map`; confirm onboarding changes into a compact Lily note with `Replay Lily` while
   the Ghost conversation panel remains in the same position.
9. Add one graph card, click `Replay Lily`, and confirm the same onboarding/conversation return.
10. Dismiss it and confirm the graph card is preserved.
11. Confirm palette, reply-map board, guide, validation row, and persistent build
   objective appear without cropping.
12. Confirm Ghost's face and deterministic test outcome appear in the upper conversation panel, not
    inside or on top of the right Guide column.

### Act 3 Face / Retry Check

1. With the graph empty, click `Test Ghost's map`.
2. Confirm the deterministic validation failure remains visible, the Ghost face becomes Sad, the button
   becomes `Try again`, and the graph remains editable.
3. Add all five card types but wire at least one route incorrectly; click `Try again`.
4. Confirm the face becomes Confused for wrong structure/failed test, and the new failure detail remains
   visible while moving, adding, deleting, or reconnecting graph items.
5. Confirm clicking `Ask Lily` only requests a hint and never changes validation or face scoring.

### Act 3 Success / Debrief Check

1. Build the passing map using the existing sample case:
   - Start next -> Recognize request
   - Recognize request next -> Check room
   - Check room `room yes` -> Answer location
   - Check room `room no` -> Ask which room
2. Click `Try again` / `Test Ghost's map`.
3. Confirm both deterministic test cases pass, the face becomes Happy, and the objective shows completion.
4. Confirm the primary button reads `Complete Act`.
5. Click `Complete Act`; confirm `GameShellPrototype` loads through the existing pending-debrief path and
   the Act 3 debrief is shown.

### Acts 1-3 Consistency / Floating Windows

1. Enter Acts 1, 2, and 3 and compare onboarding panel style, persistent objective strip, and Ghost face
   language.
2. In Act 3, confirm the ambient banter panel appears as the existing floating window.
3. Drag ambient banter within the Game view and confirm it remains movable and clamped on screen.
4. Click `Ask Lily`, drag the Lily chat by its header, close it, and confirm ambient banter resumes.
5. Confirm neither floating panel permanently blocks graph editing or the validation/completion button.
6. Confirm the Act 3 guide column, Ghost face, test cases, and validation detail all fit at 1920x1080.
7. Confirm no Console errors appear throughout the Act 1-3 checks.

### Scope / Determinism Check

1. Confirm all intent/entity/dialog validators, sessions, sample data, demo engines, and existing tests
   are unchanged.
2. Confirm Act 2, Fundamentals, Shell flow, Banter/Common/GhostAvatar components, Backend,
   ProjectSettings, Packages, Build Settings, scenes, and existing `.meta` files are unchanged by Run 001.
3. Confirm no Lily portrait file or `tmp/lily` preview was changed; portrait work starts in Run 002.

### Inspector Setup

No new Inspector setup is required. Use the existing Act 1 and Act 3 scene presenter wiring. The new
onboarding panels, objective strips, Act 3 Ghost face, and action-button states are created at runtime;
the existing scene-load hooks create floating banter/chat affordances.
