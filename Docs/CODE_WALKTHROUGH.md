# CODE_WALKTHROUGH.md

## Purpose

This file explains the implemented code in a way that can be used for report writing, debugging, and final project explanation.

Do not describe scripts that do not exist yet.

Each implemented C# file should use the format below.

---

## Script Template

### Script Name

TBD

### Purpose

What does this script do?

### Attached GameObject

Which Unity GameObject should this script be attached to?

### Runtime Role

When does this script run during Play Mode?

### Important Fields

List important serialized fields and what should be dragged into them in Inspector.

### Important Methods

Explain the key methods.

### Input

What player input, Unity event, or data does it receive?

### Output

What UI, state, Ghost response, or progress does it change?

### Failure Cases

What can go wrong? For example:
- missing Inspector reference
- empty data
- wrong tag
- invalid puzzle state

### Unity Test

How to test this script manually in Play Mode.

---

## Intent Classification Runtime

### Script Name

IntentCard.cs

### Purpose

Defines one message card for the Act 1 intent-classification puzzle. Each card has:
- an `Id` used by puzzle submissions
- `MessageText` shown later by UI
- an `IntentId` used as the correct purpose / intent group

### Attached GameObject

None. This is pure C# data and should not be attached to a GameObject.

### Runtime Role

Created by future level data or tests before validation. It does not run by itself.

### Important Fields

No serialized Unity fields. The constructor receives `id`, `messageText`, and `intentId`.

### Important Methods

- `IntentCard(string id, string messageText, string intentId)`: creates a card and rejects empty card ids or intent ids.

### Input

Plain C# constructor values.

### Output

An immutable card object that the validator can read.

### Failure Cases

- Empty card id throws an `ArgumentException`.
- Empty intent id throws an `ArgumentException`.
- Null message text is converted to an empty string.

### Unity Test

Use EditMode tests in `IntentClassificationValidatorTests.cs`. This script has no Play Mode behaviour.

---

### Script Name

IntentClassificationValidator.cs

### Purpose

Validates whether submitted groups of message card ids match the intended Act 1 learning concept: messages should be grouped by shared purpose / intent, not by exact wording.

### Attached GameObject

None. This is pure C# puzzle logic and should not be attached to a GameObject.

### Runtime Role

Future UI or puzzle controller code can call `IntentClassificationValidator.Validate(...)` after the player arranges cards into groups.

### Important Fields

No serialized Unity fields.

### Important Methods

- `Validate(IEnumerable<IntentCard> cards, IEnumerable<IEnumerable<string>> submittedGroups)`: compares the submitted card-id groups against the card intent ids.
- `IntentClassificationResult.IsCorrect`: true only when every known card appears exactly once and each intent appears in exactly one pure group.
- `IntentClassificationResult.Errors`: validation messages for incorrect or invalid submissions.

### Input

- A list of `IntentCard` objects representing the puzzle answer key.
- A list of submitted groups, where each group contains card ids.

### Output

An `IntentClassificationResult` with a boolean correctness flag and error details for UI feedback or tests.

### Failure Cases

The validator returns incorrect results with errors for:
- empty level card data
- duplicate card ids in level data
- null cards
- empty groups
- duplicate submitted cards
- unknown submitted card ids
- missing known cards
- groups that mix different intents
- one intent split across multiple groups

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/IntentClassificationValidatorTests.cs`.

---

### Script Name

Act1IntentClassificationSampleData.cs

### Purpose

Provides reusable sample data for the Act 1 intent-classification puzzle. The data demonstrates the learning concept that different wording can still share the same purpose / intent.

### Attached GameObject

None. This is pure C# sample data and should not be attached to a GameObject.

### Runtime Role

Future UI or puzzle controller code can call this class to get sample message cards and the correct intent groups. The class does not run by itself.

### Important Fields

No serialized Unity fields.

Constants:
- `FindItemIntentId`
- `AskLocationIntentId`
- `AskIdentityIntentId`

### Important Methods

- `CreateCards()`: returns fresh `IntentCard` objects for the Act 1 sample puzzle.
- `CreateCorrectGroups()`: returns the correct grouping by card id, ready to pass into `IntentClassificationValidator.Validate(...)`.

### Input

None. The sample data is created by method calls.

### Output

- Three intent groups.
- Nine message cards total.
- Three differently worded messages per intent.

Sample intent groups:
- `find_item`: messages about finding a missing key, notebook, or lantern.
- `ask_location`: messages asking where Ghost is.
- `ask_identity`: messages asking who Ghost is or what to call the little ghost.

### Failure Cases

- If card ids are edited later, `CreateCorrectGroups()` must be updated to match.
- If sample cards are moved to data files later, tests should continue validating that every card appears in exactly one correct group.

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/Act1IntentClassificationSampleDataTests.cs`.

---

### Script Name

IntentClassificationSession.cs

### Purpose

Tracks the player's current Act 1 intent-classification grouping before UI exists. It keeps puzzle state separate from display code, so later drag-and-drop UI can move card ids while this class owns the grouping state.

### Attached GameObject

None. This is pure C# session state and should not be attached to a GameObject.

### Runtime Role

Future puzzle controller or UI code can create a session at the start of an Act 1 puzzle, move cards between player groups as the player acts, and ask the session to validate the current grouping.

### Important Fields

No serialized Unity fields.

Internal state:
- source `IntentCard` list
- unassigned card ids
- assigned card ids by player group id
- current group id by card id

### Important Methods

- `IntentClassificationSession(IEnumerable<IntentCard> cards)`: initializes a session from any card list.
- `CreateFromSampleData()`: initializes a session from `Act1IntentClassificationSampleData`.
- `MoveCardToGroup(string cardId, string groupId)`: assigns or moves a known card into a player group.
- `MoveCardToUnassigned(string cardId)`: returns a known card to the unassigned pile.
- `GetAssignedGroupId(string cardId)`: returns the current group id for a card, or null if unassigned.
- `GetAssignedCardIds(string groupId)`: returns the assigned card ids for a player group.
- `CreateSubmittedGroups()`: returns the current non-empty groups in the format expected by `IntentClassificationValidator`.
- `ValidateCurrentState()`: validates the current grouping with `IntentClassificationValidator`.

### Input

- `IntentCard` data at construction time.
- Card ids and player group ids when cards are moved.

### Output

- Snapshots of cards, unassigned card ids, assigned group ids, and submitted groups.
- An `IntentClassificationResult` when the session validates its current state.

### Failure Cases

- Null card list throws an `ArgumentNullException`.
- Empty card list throws an `ArgumentException`.
- Null cards or duplicate card ids throw an `ArgumentException`.
- Empty or unknown card ids throw an `ArgumentException`.
- Empty group ids throw an `ArgumentException`.
- Partial groupings validate as incorrect because unassigned cards are missing from the submitted groups.

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/IntentClassificationSessionTests.cs`.

---

## Act 1 Static UI Prototype

## Presentation Assembly Boundary

- `Assets/Presentation/Ghost.Presentation.asmdef` compiles Unity-facing presentation scripts into `Ghost.Presentation`.
- `Ghost.Presentation` references `Ghost.Runtime`, `UnityEngine.UI`, and `Unity.InputSystem`.
- `Assets/Presentation/Act1IntentClassification/Editor/Ghost.Presentation.Editor.asmdef` keeps editor-only scene builder code in `Ghost.Presentation.Editor`.
- The pure puzzle logic remains in `Ghost.Runtime` with `noEngineReferences` enabled.

---

### Script Name

Act1IntentClassificationInteractionController.cs

### Purpose

Owns the Act 1 presentation interaction state for click and drag assignment. It coordinates the pure `IntentClassificationSession`, selected card id, assignment, unassignment, validation, and simple state/feedback notifications.

### Attached GameObject

None. This is a plain C# presentation controller created by `Act1IntentClassificationStaticPresenter` at runtime.

### Runtime Role

When the presenter renders sample data, it creates one controller for that UI session. UI clicks and valid drag-drop assignments are forwarded from the presenter into this controller, and the controller notifies the presenter when visual state or validation feedback should refresh.

### Important Fields

No serialized Unity fields.

Internal state:
- source `IntentCard` list
- one `IntentClassificationSession`
- selected card id
- current validation feedback message and kind

### Important Methods

- `SelectCard(string cardId)`: selects an unselected card or deselects the currently selected card.
- `AssignSelectedCardToIntent(string intentId)`: assigns the selected card to the clicked intent group, clears selection, and sends neutral feedback.
- `AssignCardToIntent(string cardId, string intentId)`: assigns a specific dragged card to the dropped intent group through the same session assignment path.
- `MoveAssignedCardToUnassigned(string cardId)`: moves an assigned card back to unassigned and sends neutral feedback. The presenter uses this for both `Back:` row clicks and assigned-card drops onto the left message-card area.
- `ValidateCurrentGrouping()`: validates through `IntentClassificationSession.ValidateCurrentState()` and sends correct/incorrect feedback.
- `GetAssignedGroupId(string cardId)`: exposes assignment state for card highlighting.
- `GetAssignedCardIds(string groupId)`: exposes group contents for rendering.
- `StateChanged`: event used by the presenter to refresh visuals.
- `FeedbackChanged`: event used by the presenter to refresh validation feedback text.

### Input

Plain C# method calls from the presenter in response to UI clicks and drop-target events.

### Output

Updated interaction/session state plus simple callbacks. It does not create UI objects directly.

### Failure Cases

- Invalid card ids or group ids still fail through `IntentClassificationSession`.
- If no card is selected, assigning to a group is ignored.

### Unity Test

Manual scene check for M0-T12. Use the M0-T12 Play Mode checklist and confirm click assignment, drag assignment, drag-back-to-unassigned, `Back:` row clicks, and validation feedback all route through the controller.

---

### Script Name

Act1IntentClassificationStaticPresenter.cs

### Purpose

Renders the Act 1 sample intent-classification UI and connects UI events to `Act1IntentClassificationInteractionController`. It remains a placeholder presentation script with click assignment, minimal drag-to-assign, validation feedback, and no scoring, save/load, or dialogue behaviour.

### Attached GameObject

Attach to the root UI object in `Assets/Scenes/Act1IntentClassificationPrototype.unity`, once that scene is created through the Unity menu builder.

### Runtime Role

On `Start`, it optionally refreshes the card and intent group UI from `Act1IntentClassificationSampleData`, creates an `Act1IntentClassificationInteractionController`, renders the UI, and wires card/group/assigned-row/validation clicks plus drag/drop behaviours to that controller. The generated prototype scene also calls the same render method in the Editor before saving, so the scene can show the layout when opened.

### Important Fields

- `cardListRoot`: parent `RectTransform` for sample message cards.
- `intentGroupListRoot`: parent `RectTransform` for intent group areas.
- `cardTemplate`: inactive in-scene template used to render a card.
- `intentGroupTemplate`: inactive in-scene template used to render an intent group area.
- `renderOnStart`: when true, rebuilds the prototype UI at Play Mode start.

Internal runtime state:
- rendered card views by card id
- rendered intent group views by intent id
- scrollable assignment list content roots by intent id
- one `Act1IntentClassificationInteractionController`
- validation feedback text

### Important Methods

- `RenderSampleData()`: clears existing rendered children, creates a fresh interaction controller from sample data, displays the nine sample cards, and displays the `find_item`, `ask_location`, and `ask_identity` group areas.
- `UpdateVisualState()`: reads controller state to update card colors, group colors, and assigned-card text lists.
- `ConfigureCardDrag(...)`: attaches `Act1IntentClassificationDraggableCard` to left-side message cards and right-side assigned-card rows.
- `ConfigureIntentGroupDropTarget(...)`: attaches `Act1IntentClassificationDropTarget` to each intent group area and its assigned-card scroll viewport so dropping anywhere in the group panel is more forgiving.
- `ConfigureUnassignedDropTarget(...)`: attaches `Act1IntentClassificationDropTarget` to the left message-card list so assigned cards can be dragged back to unassigned.
- `AssignDraggedCardToIntent(...)`: forwards the dragged card id and target intent id to the controller so drag and click assignment share the same session flow. Drops onto the card's current group are ignored to avoid accidental reordering.
- `MoveDraggedCardToUnassigned(...)`: forwards assigned-card drops on the left message-card list to the controller/session unassign path.
- `EnsureAssignmentRoot(...)`: builds or upgrades each intent group's assigned-card area into a vertical `ScrollRect`.
- `CreateAssignedCardRow(...)`: renders assigned cards as compact draggable rows/chips in the group list, not as free-positioned objects.
- `EnsureValidationControls()`: creates or reuses the Validate button and feedback text under the intent group column.
- `ApplyValidationFeedback(...)`: displays feedback produced by the controller.
- `EnsureEventSystem()`: creates an `EventSystem` with `InputSystemUIInputModule` at runtime if the scene does not already contain one.

### Input

- Sample cards and intent ids from `Act1IntentClassificationSampleData`.
- Pointer clicks on rendered message cards, intent group areas, assigned-card rows, and the Validate button.
- Pointer drag events from rendered message cards and assigned-card rows.
- Drop events from intent group areas, intent group scroll viewports, and the left message-card list.

### Output

UGUI objects showing:
- nine sample message cards
- three intent group areas
- short intent-purpose descriptions
- selected-card highlight
- compact assigned-card rows listed inside each intent group area
- scrollable assigned-card lists so assigning many cards to one group does not silently hide them
- a single opaque temporary drag preview while a message card or assigned-card row is being dragged
- basic validation feedback for correct or incorrect grouping

### Failure Cases

- Missing template or root references leave the UI unchanged.
- If sample card ids or intent ids change later, the displayed labels will change because the presenter reads from sample data.
- If the controller is missing, the presenter can still render placeholder UI but click actions will not update state.
- If cards appear as blank pale rectangles, regenerate the scene with the Unity menu builder so the compact card template is saved, or enter Play Mode so `RenderSampleData()` rebuilds the card views from the updated presenter.
- If group areas do not respond to clicks in an older generated scene, rerun the Unity menu builder so the saved scene includes the generated `EventSystem` and updated group templates. The presenter also attempts to create a runtime `EventSystem` if one is missing.
- If assigned-card text still appears outside a group after script import, rerun the Unity menu builder so the saved scene includes the M0-T08 run 002 clipped assignment-list template. Play Mode startup also reapplies the compact/clipped runtime layout.
- If assigned-card rows are not scrollable or the Validate button is missing in an older generated scene, rerun the Unity menu builder so the saved scene includes the M0-T09 scrollable assignment areas and validation controls. Play Mode startup also rebuilds these controls.
- After M0-T11, Unity must import the new `Ghost.Presentation` assembly definitions before the presentation scripts compile in their explicit assembly boundary.
- If drag-to-assign or drag-back-to-unassigned does not work after importing M0-T12, confirm the scene has an `EventSystem` with `InputSystemUIInputModule`, then rerun the menu builder or enter Play Mode so the presenter attaches the draggable card and drop target behaviours.
- If drag previews remain visible after a drop, confirm the imported `Act1IntentClassificationDropTarget` calls `CompleteDragVisuals()` before invoking assignment callbacks.

### Unity Test

Manual scene check for M0-T12. Open the prototype scene after running the menu builder if needed, enter Play Mode, and repeat the M0-T09 behaviour checks plus drag a message card onto each intent group area, drag an assigned row back to the left message-card list, and drag an assigned row to a different intent group.

---

### Script Name

Act1IntentClassificationDraggableCard.cs

### Purpose

Adds minimal Unity UI drag behaviour to a rendered Act 1 message card or assigned-card row. It is presentation-only and does not validate or assign cards itself.

### Attached GameObject

Attached by `Act1IntentClassificationStaticPresenter` to each rendered left-side message card GameObject and each right-side assigned-card row.

### Runtime Role

During Play Mode, Unity UI pointer events call this component when the player begins dragging, moves the pointer, and ends the drag. The component keeps one static active drag preview, creates an opaque temporary visual preview under the root canvas, fades the original source view as a placeholder, and restores the original source view when the drag ends.

### Important Fields

No Inspector fields. The presenter initializes:
- `cardId`: the message card id represented by this view.
- `rootCanvas`: the canvas used for the temporary drag preview.

### Important Methods

- `Initialize(...)`: stores the card id and canvas, and ensures the card has a `CanvasGroup`.
- `OnBeginDrag(...)`: creates the solid card-like preview and disables raycast blocking on the source card so drop targets can receive the drop.
- `OnDrag(...)`: moves the preview by the pointer delta.
- `OnEndDrag(...)`: destroys the preview and restores source-card raycast blocking and alpha.
- `OnDisable()`: cleans up the active preview if the source row is destroyed by a successful drop re-render before Unity sends end-drag.
- `CompleteDragVisuals()`: explicitly ends the visual drag state and is called by drop targets before they trigger assignment/unassignment callbacks.
- `CancelActiveDrag()`: ensures a new drag cannot leave an older preview behind.

### Input

Unity UI drag events from the EventSystem.

### Output

Visual drag feedback only. Assignment and unassignment are handled by `Act1IntentClassificationDropTarget` and the presenter/controller flow.

### Failure Cases

- If no root canvas is available, the drag does not start.
- If the scene has no working EventSystem or UI input module, Unity will not send drag events.
- If UI re-rendering destroys the dragged assigned row during a successful drop, `OnDisable()` and `CompleteDragVisuals()` clean the preview so no stale afterimage remains.

### Unity Test

Enter Play Mode, drag a message card, confirm a single solid card-like preview follows the pointer, then release outside a valid target and confirm the original card returns to its normal visual state with no `Drag Preview` objects left in the hierarchy.

---

### Script Name

Act1IntentClassificationDropTarget.cs

### Purpose

Adds minimal Unity UI drop handling to an Act 1 intent group area or the left message-card list. It detects dropped cards and forwards the card id plus either the target intent id or unassigned target to the presenter.

### Attached GameObject

Attached by `Act1IntentClassificationStaticPresenter` to each rendered intent group GameObject, each intent group scroll viewport, and the left message-card list.

### Runtime Role

During Play Mode, Unity UI calls `OnDrop` when a draggable message card or assigned-card row is released over a group area or the left message-card list.

### Important Fields

No Inspector fields. The presenter initializes:
- `intentId`: the target intent group id for intent-group targets.
- `cardDroppedOnIntent`: callback used to route group drops into the existing controller/session assignment flow.
- `cardDroppedOnUnassigned`: callback used to route left-column drops into the existing controller/session unassignment flow.

### Important Methods

- `InitializeIntentGroup(...)`: stores the intent id and assignment callback.
- `InitializeUnassigned(...)`: stores the unassignment callback.
- `OnDrop(...)`: reads the dragged card component from `PointerEventData.pointerDrag`, tells the draggable source to clean up its preview, and invokes the correct callback for the target.

### Input

Unity UI drop events from the EventSystem.

### Output

Calls back into `Act1IntentClassificationStaticPresenter`, which assigns or unassigns through `Act1IntentClassificationInteractionController`.

### Failure Cases

- Drops from unknown objects are ignored.
- Empty card ids are ignored.
- A target without a callback does nothing.
- If the dragged card keeps blocking raycasts, the group may not receive the drop; the draggable card component disables source-card raycast blocking while dragging.

### Unity Test

Enter Play Mode, drag a message card onto `find_item`, `ask_location`, or `ask_identity`, and confirm the card appears as a `Back:` row in that group. Then drag that assigned row back to the left message-card list and confirm it returns to unassigned.

---

### Script Name

Act1IntentClassificationPrototypeSceneBuilder.cs

### Purpose

Editor-only helper that creates the Act 1 prototype scene through Unity-supported scene serialization. It avoids hand-writing `.unity` YAML.

### Attached GameObject

None. This script lives under an `Editor` folder and runs from the Unity Editor menu.

### Runtime Role

No runtime role. It is excluded from player builds by the `Editor` folder.

### Important Fields

No Inspector fields.

### Important Methods

- `BuildAct1IntentClassificationPrototypeScene()`: creates a new scene, builds a placeholder UGUI canvas, adds an EventSystem for UI clicks, wires the presenter, renders the sample data with scrollable assignment-list areas and validation controls, and saves `Assets/Scenes/Act1IntentClassificationPrototype.unity`.

### Input

Manual Unity Editor menu action:
`Ghost > Build Act 1 Intent Classification Prototype Scene`

### Output

`Assets/Scenes/Act1IntentClassificationPrototype.unity`, when Unity can execute the builder successfully.

### Failure Cases

- If Unity cannot run batch mode or cannot import the project, Codex cannot create the scene automatically.
- If the menu builder fails in the Editor, check the Console for compile errors before rerunning it.
- If the left-side cards show blank text after an older scene generation, rerun `Ghost > Build Act 1 Intent Classification Prototype Scene` to rebuild the scene with the compact card template from M0-T07 run 002.
- If an older generated scene does not show assigned-card lists or does not respond to clicks, rerun `Ghost > Build Act 1 Intent Classification Prototype Scene` to rebuild the scene with the M0-T08 EventSystem and assignment-list template.
- If an older generated scene still lets assigned-card text overflow, rerun `Ghost > Build Act 1 Intent Classification Prototype Scene` to rebuild the scene with the M0-T08 run 002 clipped group template.
- If an older generated scene does not show scrollable assignment lists or validation feedback, rerun `Ghost > Build Act 1 Intent Classification Prototype Scene` to rebuild the scene with the M0-T09 presenter output.
- M0-T11 does not require scene regeneration for behaviour because the presenter component remains the same script asset, but Unity must import the new presentation/editor assembly definitions. If the scene shows stale serialized layout after import, rerun the builder.

### Unity Test

Run the menu builder in Unity, open the generated scene, and enter Play Mode. Confirm there are no Console errors.
