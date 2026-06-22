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
