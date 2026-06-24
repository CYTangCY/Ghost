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
6. Confirm the `Next` button advances to the next line.
7. Confirm Act 1 includes nervous Lily lines and garbled Ghost lines, with at least 15 Lily lines and 15 Ghost lines available in the loop.
8. Confirm at least one line addresses the player by the entered name.
9. Confirm Act 1 banter text is not vertically cut off.
10. Confirm Act 1 puzzle controls remain fully playable with the panel present.
11. Return to the hub and launch Act 2.
12. Confirm Act 2 banter appears in the bottom validation area and cycles/loops without covering chips or palette controls.
13. Confirm the Act 2 banter box is visibly slimmer than the earlier oversized version while still readable.
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
   - `future LLM endpoints remain explicit 501 stubs`

### Expected Result

The backend install, TypeScript build, and test suite should complete successfully. The server remains local-only and does not score puzzle submissions.

### Unity Play Mode Check

M0-T27 adds a top-level `Backend/` service only. There is no Unity Play Mode behaviour, scene setup, Inspector setup, or Unity client wiring in this task.

### Inspector Setup

No Inspector setup is required. Do not add backend objects to scenes for M0-T27.
