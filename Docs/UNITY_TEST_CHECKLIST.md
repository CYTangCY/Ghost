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

---

## M0-T47 Run 001: Act 4 Confidence and Fallback

### Import / Compile Check

1. Open the Ghost Unity project and wait for script compilation.
2. Confirm these new Act 4 scripts import without Console compile errors:
   - `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceModels.cs`
   - `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceDemoData.cs`
   - `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceValidator.cs`
   - `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceInteractionController.cs`
   - `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceStaticPresenter.cs`
   - `Assets/Presentation/Act4ConfidenceFallback/Editor/Act4ConfidencePrototypeSceneBuilder.cs`
3. Confirm the updated Shell scripts import without errors and still load Acts 1-3.

### EditMode Test Check

1. Open `Window > General > Test Runner`.
2. Run the full EditMode suite.
3. Confirm `Act4ConfidenceValidatorTests` passes:
   - reference threshold plus fallback/handoff passes
   - threshold outside the authored range fails
   - missing fallback/handoff fails
   - very low and very high thresholds create the intended wrong outcomes
4. Expected automated result from Codex run: 60/60 EditMode tests passed, including 5/5 Act 4 tests.

### Scene / Build Settings Check

1. Select `Ghost > Build Act 4 Confidence and Fallback Scene` if the scene needs refreshing.
2. Confirm `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity` exists.
3. Select `Ghost > Build Game Shell Scene` if the hub needs refreshing.
4. Open Unity Build Settings / Build Profiles.
5. Confirm `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity` is enabled in Build Settings.
6. Confirm no ProjectSettings changes were made except the approved Act 4 Build Settings scene entry.

### Shell Hub Check

1. Open `Assets/Scenes/GameShellPrototype.unity`.
2. Enter Play Mode.
3. Click `Start / Continue` and reach the Act Hub.
4. Confirm the hub shows `Act 4: Confidence` with a `Start Act 4` button.
5. Click `Start Act 4`.
6. Confirm Lily plays the Act 4 intro beat, then the continue button loads `Act4ConfidenceFallbackPrototype`.
7. Confirm the runtime `Return to Hub` overlay appears in Act 4.

### Act 4 Play Mode Check

1. Open `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity` directly or enter Act 4 from the Shell.
2. Set the Game view to 1920x1080 and enter Play Mode.
3. Confirm the page order is header, objective strip, Lily onboarding, Ghost conversation panel, then puzzle body after onboarding.
4. Confirm the first screen says the goal is to stop Ghost bluffing, defines confidence score, explains the answer threshold, and names both safe routes.
5. Confirm the conversation example explicitly shows that a vague 62% guess passes the starting 30% threshold.
6. Click `Show me the controls` and confirm the controls unlock with threshold 30%, fallback missing, and handoff missing.
7. Confirm `Reply Safety Map` shows the `score >= threshold` answer rule and a three-step `Your task` guide.
8. Confirm the visitor queue explains what its percentages mean and each ordinary row labels the value as confidence.
9. Drag the confidence slider and confirm the live sentence reads `Answer only when confidence is N% or higher` from 0 to 100.
10. Confirm the slider labels explain the low-threshold bluffing risk and high-threshold rejection risk without overlapping other controls.
11. Attach and detach `Fallback`; confirm its route card explains that it handles scores below the threshold.
12. Attach and detach `Handoff`; confirm its route card explains that it handles upset / complex requests.
13. With threshold very low, fallback attached, and handoff attached, click `Run the day` and step through visitors.
14. Confirm each ordinary visitor displays its exact comparison, such as `62% >= 30% threshold -> Intent reply`.
15. Confirm the garbled/ambiguous visitors can produce wrong intent replies and the failed run returns to editable controls with `Try again`.
16. With threshold very high, fallback attached, and handoff attached, run the day again.
17. Confirm clear visitors are asked to rephrase and the failed run returns to editable controls.
18. With threshold around 70 and fallback missing, run the day and confirm uncertain visitors have no safe route.
19. With threshold around 70 and handoff missing, run the day and confirm the upset/complex archive case displays its special rule and melts down.
20. Set threshold to 70, attach both fallback and handoff, and run the day.
21. Confirm clear requests receive intent replies, uncertain/garbled requests ask for rephrasing, and the upset/complex case calls Lily.
22. Finish the day and confirm the completion state shows `Complete Act`.
23. Click `Complete Act` and confirm the Shell loads and plays the Act 4 debrief through the pending-debrief path.
24. Confirm all text fits at 1920x1080 and no Console errors appear throughout the Act 4 run.

### Scope / Determinism Check

1. Confirm correctness comes only from `Act4ConfidenceValidator` and authored demo data.
2. Confirm the LLM does not score, gate, or decide any Act 4 outcome.
3. Confirm existing Acts 1-3 validators, sessions, sample data, and demo engines are unchanged.
4. Confirm Act 4 is a lean first version: one threshold slider, fallback attach, handoff attach, day-run playback, retry, and Shell completion.

### Inspector Setup

No manual Inspector setup is required if scenes are generated through the menu builders. `Act4ConfidencePrototypeSceneBuilder` creates the canvas, EventSystem, root object, and `Act4ConfidenceStaticPresenter`; the presenter creates the onboarding panel, objective strip, Ghost conversation/result panel, visitor queue, threshold slider, route controls, retry, and completion button at runtime. `GameShellSceneBuilder` wires the Act 4 hub button into `GameShellPresenter`.
## M0-T48 Run 001: Act 5 Testing and Debugging

### Import / Compile Check

1. Open the Ghost Unity project and wait for script compilation and meta generation.
2. Confirm the new TestingDebugging runtime, Act5TestingDebugging presentation, shared dialog-graph wire host, and Act 5 editor builder compile without Console errors.
3. Confirm Act 3 still compiles after its input/output port presenter field changes to IDialogGraphWireInteractionHost.

### EditMode Test Check

1. Open Window > General > Test Runner.
2. Run Ghost.Tests.EditMode.Act5TestSuiteRunnerTests.
3. Confirm the seeded buggy graph is incorrect with 0/4 authored conversations passing.
4. Confirm the reference fixed graph is correct with 4/4 passing and no validator errors.
5. Confirm the room test reports expected answer_object_location versus actual ask_for_room.
6. Confirm the greeting test reports no response while its start branch is missing.
7. Run the full EditMode suite and confirm Acts 1-4 remain green.
8. Expected automated result from Codex run 002: 4/4 focused Act 5 tests and 64/64 full EditMode tests passed.

### Scene / Build Settings Check

1. Select Ghost > Build Act 5 Testing and Debugging Scene.
2. Confirm Assets/Scenes/Act5TestingDebuggingPrototype.unity exists.
3. Select Ghost > Build Game Shell Scene.
4. Open Build Settings / Build Profiles.
5. Confirm Act5TestingDebuggingPrototype is enabled after the existing Act 4 scene.
6. Confirm no ProjectSettings changes exist except the approved Act 4/Act 5 scene entries.

### Shell Hub Check

1. Open Assets/Scenes/GameShellPrototype.unity and enter Play Mode.
2. Reach the Act Hub and confirm an Act 5: Testing card and Start Act 5 button appear.
3. Click Start Act 5 and confirm Lily's testing/debugging intro appears.
4. Continue and confirm Act5TestingDebuggingPrototype loads.
5. Confirm the Return to Hub overlay appears in Act 5.

### Act 5 Play Mode Check

1. Set Game view to 1920x1080 and enter Act 5 Play Mode.
2. Confirm the first screen explains why a tidy graph still needs testing, what red expected/actual cards mean, how to reconnect a wire, and why every test must be rerun.
3. Confirm the conversation example shows the lab-hours visitor receiving an ask-for-room reply.
4. Click Open the test bench.
5. Confirm the page shows a pre-built graph on the left and four NOT RUN test cards on the right.
6. Confirm graph nodes and wires do not overlap incoherently, all nine node titles fit, and blue/green/orange port meanings are readable.
7. Click Run all tests.
8. Confirm the seeded graph reports 0/4 and all four cards show visitor, expected, actual, and FAIL.
9. Confirm the Ghost conversation panel focuses the first failed case instead of only showing a generic error count.
10. Drag the green room-known output to Reply: search in room.
11. Confirm the prior result cards become yellow/stale and the objective asks for a full rerun.
12. Click Rerun all tests and confirm the room-known test turns green while remaining faults stay red.
13. Drag the orange room-missing output to Reply: ask for room.
14. Drag the Intent: lab hours blue output to Reply: lab closes at 8.
15. Drag the Start blue output to the left input of Intent: greeting; confirm existing start branches remain connected.
16. Rerun all tests and confirm 4/4 pass simultaneously.
17. Confirm Ghost is happy, the objective says Complete, and the primary button becomes Complete Act.
18. Click Complete Act and confirm the Shell returns and plays the Act 5 debrief.
19. Confirm no Console errors occur throughout.

### Act 3 Regression Check

1. Open Assets/Scenes/Act3DialogGraphPrototype.unity.
2. Enter Play Mode and confirm palette placement, node dragging, output-port wire dragging, input-port drops, wire replacement, validation, and completion still behave as before.
3. This verifies the shared wire-host interface changed only presenter typing, not Act 3 behaviour.

### Scope / Determinism Check

1. Confirm Act 5 calls the existing DialogGraphValidator and DialogGraphSimulator.
2. Confirm no LLM, backend response, or presentation state decides pass/fail.
3. Confirm the existing DialogGraph pure logic, sessions, sample data, and tests are unchanged.
4. Confirm the player repairs a pre-built graph rather than rebuilding Act 3 from an empty canvas.

### Inspector Setup

No manual Inspector setup is required when using the menu builders. Act5TestingPrototypeSceneBuilder creates the canvas, EventSystem, full-screen root, and Act5TestingStaticPresenter. GameShellSceneBuilder creates and serializes the Act 5 hub button into GameShellPresenter. The presenter creates all graph nodes, ports, wires, test cards, Ghost face, and buttons at runtime.
## M0-T48 Run 003: Act 5 Wire and Usability Repair

### Import / Compile Check

1. Exit the currently running Play Mode and wait for Unity to import Act5TestingStaticPresenterTests.cs and its meta file.
2. Confirm Act5TestingStaticPresenter.cs, Act5TestingStaticPresenterTests.cs, and the updated Ghost.EditModeTests asmdef compile without Console errors.
3. Confirm no Act 3 source or presentation behaviour changed in this run.

### EditMode Test Check

1. Run Ghost.Tests.EditMode.Act5TestingStaticPresenterTests.
2. Confirm DrawLine_UsesCenteredWireLayerCoordinatesWithoutBoardOffset passes.
3. Run Ghost.Tests.EditMode.Act5TestSuiteRunnerTests and confirm the existing 4/4 focused tests remain green.
4. Run the full EditMode suite and confirm all tests pass.

### Act 5 Play Mode Check

1. Re-enter Assets/Scenes/Act5TestingDebuggingPrototype.unity at 1920x1080.
2. Confirm every existing wire begins and ends on a visible socket; no line crosses from outside the board or floats in the lower-left corner.
3. Open the test bench and confirm Step 1 points to the right-side Run all 4 tests button.
4. Before the first run, confirm right-side output sockets look muted and dragging is intentionally locked.
5. Click 1. Run all 4 tests and confirm 0/4 plus red expected-versus-actual cards appear.
6. Confirm node labels explain LEFT input and RIGHT output, and the colored output sockets become fully saturated.
7. Drag a colored RIGHT socket to the expected reply's blue LEFT socket; confirm the temporary wire stays attached to the cursor and the committed wire touches both sockets after drop.
8. Confirm edited results become stale and the graph guide plus button clearly show Step 3: rerun all four tests.
9. Repair all four routes, rerun, and confirm 4/4 plus Complete Act.
10. Open Act 3 and confirm its drag/drop wires still behave as before.

### Inspector Setup

No manual Inspector setup is required. The change is runtime presenter geometry and text; the existing generated Act 5 scene remains valid.


## M0-T49 Run 001: [SUPERSEDED by Run 003 chapter split - historical record] Chapter 6 Repair Ghost's Voice and Ending

### Import / Compile Check

1. Exit Play Mode, return focus to Unity, and wait for script compilation and meta generation.
2. Confirm the new VoicePipeline runtime, Act6VoicePipeline presentation, Act 6 editor builder, and Act6PipelineValidatorTests compile without Console errors.
3. Confirm the updated Shell scripts and GameShell scene builder compile, and Acts 1-5 still load.
4. Confirm all Chapter 6 C# remains ASCII-only and no existing pure puzzle logic was modified.

### EditMode Test Check

1. Open Window > General > Test Runner.
2. Run Ghost.Tests.EditMode.Act6PipelineValidatorTests.
3. Confirm all 6 focused tests pass:
   - the five canonical stages plus backend side link pass
   - swapped opening stages fail first at UI input
   - a partial path identifies response generation as the first missing stage
   - a correct main path without backend still fails
   - backend cannot replace a main-path stage
   - duplicate main components fail deterministically
4. Run the full EditMode suite and confirm Acts 1-5 remain green.
5. External Codex verification before Unity import: runtime/presentation/test/editor projects compiled with 0 errors; a standalone validator smoke test reported correct=True, errors=0 and missingBackend=False, firstBroken=backend_integration.
6. Automated Unity result from M0-T49 verification run 002: 6/6 focused Chapter 6 tests and 71/71 full EditMode tests passed.

### Scene / Build Settings Check

1. Select Ghost > Build Chapter 6 Repair Ghost's Voice Scene.
2. Confirm Assets/Scenes/Act6VoicePipelinePrototype.unity exists.
3. Select Ghost > Build Game Shell Scene.
4. Open Build Settings / Build Profiles.
5. Confirm Act6VoicePipelinePrototype is enabled after the existing Act 5 scene.
6. Confirm the only new ProjectSettings change is the approved Chapter 6 Build Settings scene append.
7. Open the generated scene and confirm it contains one camera, one 1920x1080-scaled canvas, one Input System EventSystem, and an Act6PipelineStaticPresenter root.

### Shell Hub Check

1. Open Assets/Scenes/GameShellPrototype.unity and enter Play Mode.
2. Reach Chapter Select and confirm six chapter cards fit in a stable 3-by-2 grid.
3. Confirm Chapter 6 is titled Repair Ghost's Voice and has a Start Chapter 6 button.
4. With one or more of Acts 1-5 incomplete, start Chapter 6 and confirm Lily gently suggests finishing earlier repairs but still allows Continue.
5. With Acts 1-5 complete, start Chapter 6 and confirm Lily uses the standard final-repair intro without the incomplete-work suggestion.
6. Continue and confirm Act6VoicePipelinePrototype loads.
7. Confirm the Return to Hub overlay recognizes Chapter 6.

### Chapter 6 Onboarding Check

1. Set Game view to 1920x1080 and enter Chapter 6 Play Mode.
2. Confirm the first screen clearly says the purpose is to reconnect every repair so one visitor message can enter, be understood, choose a route, become a reply, and return.
3. Confirm Lily explicitly says to drag five main components into slots 1-5.
4. Confirm Lily explicitly says Backend integration belongs in the separate side socket because it fetches data rather than replacing a main stage.
5. Confirm the onboarding explains that Run the voice path stops at the first broken job on failure and carries the visitor message end-to-end on success.
6. Click Open the repair board.
7. Confirm a persistent Lily note repeats the direction of travel and backend exception.
8. Confirm Replay Lily returns to onboarding without losing the ability to resume configuration.

### Placement / Failure Check

1. Confirm the shuffled palette contains UI input, NLP engine, Dialogue management, Response generation, UI output, and Backend integration.
2. Confirm the main board has five numbered stable slots with visible arrows from 1 to 5 and a visually separate backend side socket.
3. Confirm the palette instruction says cards can be dragged, or clicked and followed by a destination click.
4. Drag a main component into a numbered slot and confirm it stays placed without moving or resizing the slot.
5. Click-select another component, then click a numbered slot and confirm the fallback placement works.
6. Move a placed card to an occupied slot and confirm the two main components swap predictably.
7. Try putting Backend integration in a numbered slot and confirm the status says it belongs in the separate side data socket.
8. Try putting a main component in the backend socket and confirm the status says only Backend integration fits there.
9. Build an incorrect order, attach backend, and click Run the voice path.
10. Confirm Ghost becomes confused and feedback names the first broken stage's visible consequence.
11. Build the correct five-stage main path but leave backend unattached; run and confirm feedback says the lab closing time cannot be fetched.
12. Click Reset and confirm all six components return to the palette and prior validation clears.

### Correct Path / Playback Check

1. Assemble this exact main order: UI input -> NLP engine -> Dialogue management -> Response generation -> UI output.
2. Attach Backend integration to the separate side socket.
3. Confirm every correctly placed component reveals its job and authored prior-work line:
   - NLP engine names the Act 1 intent piles and Act 2 entity slots
   - Dialogue management names the Act 3 reply map and Act 4 confidence/fallback/handoff
   - Response generation names the Act 5 tested route
4. Click Run the voice path.
5. Confirm configuration locks into playback and the active component is highlighted.
6. Advance through all six authored beats:
   - UI input receives Hi Ghost, when does the lab close?
   - NLP engine identifies lab hours and the lab detail
   - Dialogue management selects the tested, confidence-safe route
   - Backend integration fetches 8 PM through the side link
   - Response generation forms the full reply
   - UI output returns it to the visitor
7. Confirm Ghost's first complete answer appears exactly as: The lab closes at 8 PM. I can show you the way.
8. Confirm no LLM or presentation state decides pass/fail; only Act6PipelineValidator does.

### Ending / Return Check

1. Click Hear Ghost speak.
2. Confirm the ending overlay fades in and Ghost appears happy, glowing, floating, and gently pulsing.
3. Confirm Ghost thanks the current GhostNarrativeState.PlayerName.
4. Confirm Lily's closing line remains proud, slightly hesitant, and in character.
5. Confirm Ghost and the heading clear before the credits scroll, so text does not overlap.
6. Confirm credits show GHOST, the game description, credits, the player's name, and a thank-you line.
7. Let the full sequence finish and confirm Chapter 6 is marked complete and GameShellPrototype returns to the title screen.
8. Replay Chapter 6, start the ending, click Skip ending immediately, and confirm the same completion/title state is reached.
9. Confirm the ending uses unscaled time and no Console errors occur.

### 1080p / Regression Check

1. Confirm all onboarding text, palette cards, five slots, arrows, backend socket, feedback, Ghost panel, buttons, and playback text fit at 1920x1080 without overlap or clipping.
2. Confirm placed/selected/highlighted states do not resize the board.
3. Confirm the Shell six-card grid fits without nested cards or text overflow.
4. Run or spot-check Acts 1-5, especially Act 3 and Act 5 drag interactions, and confirm Chapter 6 did not change their puzzle logic or presentation.
5. Confirm Ambient Banter and Return to Hub overlays do not permanently block Chapter 6 interaction.

### Inspector Setup

No manual Inspector setup is required when using the menu builders. Act6VoicePipelinePrototypeSceneBuilder creates the camera, scaled canvas, EventSystem, root, and Act6PipelineStaticPresenter. The presenter creates all cards, drag/drop views, slots, Ghost panel, playback state, and ending references at runtime. GameShellSceneBuilder creates the two-row chapter grid and serializes the Chapter 6 button into GameShellPresenter.

## M0-T49 Run 003: [SUPERSEDED by Run 005 remediation checklist - historical record] Chapter 0 / Chapter 6 Teaching / Final Chapter Split

This checklist supersedes the Run 001 assumption that the voice-pipeline capstone is Chapter 6.

### Automated Import and Test Result

1. Unity 6000.4.11f1 batchmode compiled the new Story and BackendResponse assemblies without C# errors.
2. Chapter0StorySceneBuilder, Act6BackendResponseSceneBuilder, Act6VoicePipelinePrototypeSceneBuilder, and GameShellSceneBuilder all completed successfully.
3. Ghost.Tests.EditMode.Act6BackendResponseValidatorTests passed 6/6.
4. Ghost.Tests.EditMode.Act6PipelineValidatorTests passed 6/6 as the Final Chapter validator suite.
5. The complete EditMode suite passed 77/77 with 0 failed and 0 skipped.
6. Automated Play Mode visual/interaction verification was not run; complete the checks below in the Unity Game view.

### Scene and Build Settings Check

1. Select Ghost > Build Chapter 0 Opening Story Scene.
2. Select Ghost > Build Chapter 6 Backend Action and Response Scene.
3. Select Ghost > Build Final Chapter Repair Ghost's Voice Scene.
4. Select Ghost > Build Game Shell Scene last.
5. Open Build Settings / Build Profiles.
6. Confirm enabled scene order is GameShellPrototype, Chapter0OpeningStory, Chapters 1-5, Act6BackendResponsePrototype, then Act6VoicePipelinePrototype.
7. Confirm SampleScene remains after the Ghost route and no existing scene entry was deleted.
8. Confirm no Console errors occur.

### Shell Route Check

1. Open Assets/Scenes/GameShellPrototype.unity at 1920x1080 and enter Play Mode.
2. Start a fresh in-memory session, enter a player name, and confirm Chapter0OpeningStory loads before Chapter Select.
3. Finish or skip Chapter 0 and confirm the Shell shows Lily's one-time opening debrief, then Chapter Select.
4. Confirm the page states that Chapter 0 is story, Chapters 1-6 are lessons, and Final Chapter combines the repairs.
5. Confirm Replay Chapter 0 and Final Chapter are separate story-route buttons.
6. Confirm six teaching cards remain in a stable 3-by-2 grid.
7. Confirm Chapter 6 is Backend Reply, not Repair Ghost's Voice.
8. Confirm Chapter 6 opens Act6BackendResponsePrototype.
9. Confirm Final Chapter opens Act6VoicePipelinePrototype after Lily's separate capstone intro.
10. Confirm all buttons fit and remain clickable without overlapping Lily's dialogue frame.

### Chapter 0 Opening Check

1. Open Assets/Scenes/Chapter0OpeningStory.unity and enter Play Mode.
2. Confirm the header reads Chapter 0: The Late Shift and progress starts at Opening story 1/6.
3. Confirm the lab backdrop, Lily portrait, and Ghost face are visible in the first frame.
4. Advance through all six beats and confirm the speaking character receives the stronger frame highlight.
5. Confirm the entered player name appears in Lily's first line.
6. Confirm Ghost's text is brief and tangled while Lily explains the story premise without teaching a lesson.
7. On the last beat, confirm the action label changes to Enter the lab.
8. Replay and use Skip opening; confirm both paths return to the Shell without score or validator UI.
9. Confirm no text overlaps the character frames, dialogue action, header, or progress label at 1920x1080.

### Chapter 6 Purpose and Task Check

1. Open Assets/Scenes/Act6BackendResponsePrototype.unity and enter Play Mode.
2. Confirm the title reads Chapter 6: Backend Action and Response.
3. Confirm onboarding explains the purpose: Ghost needs a real fact before it can answer When does the lab close?
4. Confirm Lily explains all three jobs before interaction:
   - DATA SOURCE stores the needed fact
   - ACTION fetches the matching fact
   - RESPONSE turns the raw result into a visitor-facing sentence
5. Confirm the action to open the workbench is visible and clickable.
6. Confirm the board has exactly three stable sockets connected left-to-right by arrows.
7. Confirm the palette has six cards: three lab-hours cards and three object-room distractors.
8. Confirm the persistent task text says cards may be dragged or click-selected and then placed in a matching socket.
9. Confirm Ghost's status panel is readable and does not cover the palette or sockets.

### Chapter 6 Interaction and Failure Check

1. Drag a card into its matching role socket and confirm it stays inside the fixed socket.
2. Click another card, then click its matching socket; confirm the fallback placement works.
3. Drop an ACTION card on DATA SOURCE and confirm explicit wrong-role feedback appears.
4. Fill the three sockets with room-directory distractors and run the reply.
5. Confirm Ghost becomes confused and feedback identifies the first broken role.
6. Replace only DATA SOURCE with Lab records and rerun; confirm ACTION is now the first broken role.
7. Replace ACTION with Fetch lab closing time and rerun; confirm RESPONSE is now the first broken role.
8. Click Reset and confirm every card returns to the palette, playback clears, and socket dimensions do not shift.

### Chapter 6 Correct Playback and Completion Check

1. Build this exact chain: Lab records -> Fetch lab closing time -> Lab-hours response.
2. Click Run reply and confirm configuration locks into playback.
3. Advance through all five steps: visitor request, selected data source, backend action, raw closing_time = 8 PM result, and final delivered response.
4. Confirm the final reply is exactly The lab closes at 8 PM.
5. Confirm Ghost becomes happy only after the deterministic validator passes.
6. Complete the chapter and confirm the Shell plays the Chapter 6 backend/response debrief.
7. Confirm Chapter 6 is marked complete independently from Final Chapter.
8. Use Return to Hub before completion and confirm it does not mark Final Chapter complete.

### Final Chapter Regression Check

1. Open Assets/Scenes/Act6VoicePipelinePrototype.unity and enter Play Mode.
2. Confirm the player-facing title reads Final Chapter: Repair Ghost's Voice.
3. Confirm Chapter 6 is referenced as prior backend/response work inside the capstone.
4. Complete the existing five-stage path plus backend side link and run all playback beats.
5. Confirm the ending still plays and Skip ending reaches the same finish path.
6. Confirm completion uses FinalChapterId, not Act6Id.
7. Confirm Return to Hub is available but does not mark the Final Chapter complete.
8. Confirm the final credits and Lily/Ghost text do not overlap at 1920x1080.

### Inspector Setup

No manual Inspector setup is required when scenes are regenerated with the menu builders. For manual inspection only:

1. Chapter0OpeningStory root must contain Chapter0StoryPresenter with Render On Start enabled.
2. Act6BackendResponsePrototype root must contain Act6BackendStaticPresenter with Render On Start enabled.
3. Act6VoicePipelinePrototype root must contain Act6PipelineStaticPresenter with Render On Start enabled.
4. GameShellPresenter must have non-null Chapter 0, Chapters 1-6, Final Chapter, narrative Continue, and Back to Title button references.
5. ShellReturnToHubOverlay must recognize Chapters 1-6 and Final Chapter; Chapter 0 uses its own Skip/Continue controls.
## M0-T49 Run 005: [CURRENT END-TO-END CHECKLIST] Chapter Build-Out Remediation

This is the only current end-to-end checklist for the Chapter 0 / Chapters 1-6 / Final Chapter build. Run 001 and Run 003 above remain historical records.

### Import, Builders, and Automated Tests

1. Let Unity finish importing all six `Assets/Resources/Characters/LilyPixel*` / `GhostPixel*` PNGs and compiling.
2. Run `Ghost > Build Chapter 0 Opening Story Scene`.
3. Run `Ghost > Build Chapter 6 Backend Action and Response Scene`.
4. Run `Ghost > Build Final Chapter Repair Ghost's Voice Scene`.
5. Run `Ghost > Build Game Shell Scene` last.
6. Run the complete EditMode suite. Confirm the Return overlay source guard, Chapter 6 per-role validation test, and filled-role return test pass.
7. Confirm no builder changed unrelated ProjectSettings or removed/renamed any `.meta` file.

### Current End-to-End Play Mode Check

1. Set the Game view to 1920x1080 and start `GameShellPrototype` with fresh in-memory progress.
2. Enter a player name and confirm Chapter 0 opens before the hub.
3. Confirm Lily is a crisp low-resolution RPG sprite, not smooth or semi-realistic: high blonde ponytail, deep navy-blue blazer, red KCL lanyard, and black leather Oxford shoes. Confirm Ghost uses the same chunky pixel scale, stronger outlined body, large dark eyes, small arms, and wavy tail.
4. Finish Chapter 0 and confirm its story finish, not a puzzle validator, opens the Shell debrief and hub.
5. Confirm the hub fits inside the 664px body, including the chapter-intro `Continue to Chapter` button, Back to Title, and Lily dialogue frame; no overlap or hidden content is allowed.
6. Enter each of Chapters 1-6 one at a time and immediately press `Return to Hub`. Confirm none becomes complete and no success debrief plays.
7. Complete Chapter 1 through its validated success path and confirm its explicit `Complete Act` button returns to the correct debrief.
8. Complete all authored Act 2 errands. Confirm the new `Complete Act` button appears only after the final successful validator result, returns to the Act 2 debrief, and marks Chapter 2 complete.
9. In Chapter 3, drag at least one wire between sockets and confirm temporary and committed lines still land on their ports. Complete only through the validated success path.
10. In Chapters 4 and 5, confirm success/debrief remains gated by their deterministic validator/test-suite success paths.
11. In Chapter 6, place cards without running. Confirm every filled socket remains neutral and says `PLACED - run the route to test this responsibility.`
12. Click a filled Chapter 6 socket once. Confirm exactly one action occurs: the card returns to the palette, no stale selection appears, and the socket becomes empty.
13. Test drag/drop and palette click-select into an empty socket. Run an incorrect route and confirm per-slot repair states and first-broken-stage feedback appear only after Run.
14. Repair Chapter 6, run the correct route, advance all playback steps, and complete through its explicit validated completion action.
15. Enter the Final Chapter and immediately press `Return to Hub`. Confirm Final remains incomplete.
16. Complete the five-stage voice pipeline plus backend side link. Confirm the ending shows the happy Ghost sprite, then the matching low-resolution Lily sprite during Lily's line, then credits without overlap. No duplicate programmatic eyes or text mouth may appear over Ghost.
17. Test both full ending and Skip ending. Confirm both mark only `FinalChapterId`, return to Shell, and allow Back to Title.

### Inspector Setup

No manual Inspector wiring is required after running the four builders. Confirm each generated scene has exactly one Camera, Canvas, and EventSystem; presenter `Render On Start` remains enabled; and GameShellPresenter has non-null Chapter 0, Chapters 1-6, Final Chapter, narrative Continue, and Back to Title references.

## M0-T49 Run 022: Guided Final Chapter and Later Ask Lily Check

This section supersedes the Run 018 Final Chapter board and visual checks. The validator and three
visitor cases are unchanged; only the way the player builds and repairs the path is different.

### Automated checks

1. Run the complete EditMode suite and confirm all 94 tests pass.
2. Run `Ghost.Tests.EditMode.Act6PipelineStaticPresenterTests` and confirm the board creates six
   progress controls and two choice buttons, with no `Component Palette`.
3. Run `Ghost.Tests.EditMode.Act6PipelineValidatorTests` and confirm the six guided candidate pairs
   and existing deterministic results pass.
4. Run `Ghost.Tests.EditMode.LaterChapterHintContextTests` and confirm Chapter 4, Chapter 5,
   Chapter 6, and Final Chapter state summaries are available without internal answer identifiers.
5. From `Backend/`, run `npm test` and confirm all 10 route tests pass.

### Final Chapter guided board

1. Open `Assets/Scenes/Act6VoicePipelinePrototype.unity` and enter Play Mode.
2. Open the repair board and confirm it shows step 1 of 6, a short question, and exactly two concrete
   choices. The old twelve-card palette and six empty sockets must not appear.
3. Choose an option and confirm the board advances to the next incomplete step.
4. Use the six progress buttons to revisit and change an earlier choice.
5. Confirm `Run 3 tests` cannot be used until all six steps have a choice.
6. Make one shortcut choice, finish the other steps, and run the tests. Confirm expected and actual
   Ghost replies appear and the board focuses the first broken step.
7. Change that choice and confirm previous results become stale until the three tests are run again.
8. Choose the learned method at all six steps, run the tests, and confirm all three pass before final
   playback begins.
9. Finish and skip the ending in separate runs; both paths must complete only the Final Chapter.
10. Check the board at the target Game view size and confirm its short prompt, two choices, progress
    row, test cards, feedback, and action button remain readable without overlap.

### Ask Lily in later chapters

1. In Chapter 4, set a very high or low threshold, press `Ask Lily`, and confirm chat opens with
   guidance about the current routing problem.
2. In Chapter 5, run a failing rehearsal, press `Ask Lily`, and confirm the reply refers to comparing
   the failed expected and actual result without naming the correct wire.
3. In Chapter 6, place at least one mismatched component, press `Ask Lily`, and confirm the reply is
   about the selected backend source, action, or response role.
4. In the Final Chapter, select a shortcut or run a failing test, press `Ask Lily`, and confirm the
   reply relates to the focused step or first failed case without stating the correct choice.
5. Repeat one check with the backend stopped and confirm a static Lily reply appears.
6. Confirm opening or closing Lily chat does not change puzzle choices, validation, completion, or
   chapter navigation.

### Inspector setup

No manual Inspector setup is required. The new Final Chapter board and four `Ask Lily` buttons are
created by the existing runtime presenters.


## M0-T49 Run 023: Free-Form Final Chapter and Floating Lily Check

This section supersedes the Run 022 guided Final Chapter checklist.

### Automated checks

1. Run the complete EditMode suite and confirm all 96 tests pass.
2. Confirm `ConfigureBoardRendersFixedEndpointsShortcutsAndThreeTests` passes.
3. Confirm `PaletteCardsUseConcisePlayerFacingText` passes.
4. Confirm `LaterScenesUseTheFloatingPortraitBanterPanel` passes.
5. Confirm `FinalChapterCardChoiceUpdatesFloatingLilyReaction` passes.
6. From `Backend/`, run `npm test` and confirm all 10 route tests pass.

### Final Chapter board

1. Open `Assets/Scenes/Act6VoicePipelinePrototype.unity` and enter Play Mode.
2. Confirm the board shows twelve cards, five main stages, one backend side socket, two fixed
   endpoints, and three visitor tests.
3. Confirm every palette card contains a short name and one short job line. No guided two-choice
   panel or long chapter explanation should appear on a card.
4. Drag a card to a stage, select another card and click a stage, and swap two occupied stages.
5. Confirm Lily's floating line changes after each selection or placement but does not mark the
   position correct.
6. Place a backend action on the main path and a main skill on the backend socket. Confirm Lily gives
   a different wrong-role hint for each case.
7. Reset the board and confirm Lily gives a clean-start hint.
8. Build a partly wrong route and run all three visitor tests. Confirm different cases can pass or
   fail and the expected/actual replies remain visible.
9. Change one card and confirm the old test results become stale.
10. Build the correct route, rerun all three tests, follow playback, and complete the ending.

### Floating Ask Lily

1. Open Chapters 4, 5, 6, and the Final Chapter in separate Play Mode runs.
2. Confirm each scene shows the same draggable floating panel style as Chapters 1-3.
3. Confirm the panel contains a small Lily or Ghost portrait, a short dialogue line, and Ask Lily.
4. Drag the panel and confirm chapter controls remain usable.
5. Change the puzzle state, press Ask Lily, and confirm the chat window uses the current chapter
   state.
6. Close the chat and confirm the floating dialogue resumes.
7. Stop the backend and repeat once; confirm the local static Lily reply appears.

### Inspector setup

No manual Inspector wiring is required. The runtime hook creates the panel and portrait.

## M0-T50 Run 002: Canonical WebGL and installer check

This section records the current deployment result from `D:\Code\Ghost` and supersedes the test
totals in the earlier Final Chapter checklist sections.

### Automated and release checks

1. The complete Unity EditMode suite passed 147 of 147 tests. The XML result is
   `Build/editmode-results.xml`.
2. The backend route suite passed 10 of 10 tests, and the TypeScript production build completed.
3. Unity 6000.4.11f1 built all nine release scenes for WebGL. The new build log contains no
   missing-script warning.
4. The staged release passed with a restricted `PATH`. Its launcher verified WebGL, REST, SQLite,
   packaged Granite discovery, and one model-backed hint.
5. `GhostSetup.exe` installed into a new directory, passed the same launcher self-test, uninstalled,
   and left no application directory behind.
6. A fresh Edge profile loaded the packaged WebGL page. The Unity loading overlay cleared, no
   browser warning appeared, and the 960 by 600 canvas was present.
7. `npm audit --omit=dev` reported zero production vulnerabilities in both the source backend and
   the staged backend.

### Manual check still required

A separate Windows 10/11 x64 computer or virtual machine was not available in this run. Repeat the
installer test there, record the hardware and first-start time, and play through Chapter 3 before
describing the package as clean-machine tested. Unity Editor Play Mode was also not rerun in this
deployment repair; the recorded interaction checks remain in the chapter-specific runs above.