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

Owns the Act 1 presentation interaction state for click and drag assignment. It coordinates the pure `IntentClassificationSession`, selected card id, assignment, unassignment, validation, and simple state/feedback notifications. The feedback strings are player-facing UI copy, not puzzle-rule logic.

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
- `ValidateCurrentGrouping()`: validates through `IntentClassificationSession.ValidateCurrentState()` and sends player-facing correct/incorrect feedback.
- `GetAssignedGroupId(string cardId)`: exposes assignment state for card highlighting.
- `GetAssignedCardIds(string groupId)`: exposes group contents for rendering.
- `StateChanged`: event used by the presenter to refresh visuals.
- `FeedbackChanged`: event used by the presenter to refresh validation feedback text.

### Input

Plain C# method calls from the presenter in response to UI clicks and drop-target events.

### Output

Updated interaction/session state plus simple callbacks. It does not create UI objects directly. Feedback wording explains that the player is grouping by speaker intent, confirms when all messages are grouped by intent, and suggests comparing what the speaker wants when the grouping is incorrect.

### Failure Cases

- Invalid card ids or group ids still fail through `IntentClassificationSession`.
- If no card is selected, assigning to a group is ignored.

### Unity Test

Manual Act 1 scene check. Confirm click assignment, drag assignment, drag-back-to-unassigned, `Back:` row clicks, and validation feedback still route through the controller.

---

### Script Name

Act1IntentClassificationStaticPresenter.cs

### Purpose

Renders the Act 1 sample intent-classification UI and connects UI events to `Act1IntentClassificationInteractionController`. It remains a placeholder presentation script with click assignment, minimal drag-to-assign, validation feedback, clearer placeholder instruction/visual hierarchy polish, and no scoring, save/load, or dialogue behaviour.

### Attached GameObject

Attach to the root UI object in `Assets/Scenes/Act1IntentClassificationPrototype.unity`, once that scene is created through the Unity menu builder.

### Runtime Role

On `Start`, it optionally refreshes the card and intent group UI from `Act1IntentClassificationSampleData`, creates an `Act1IntentClassificationInteractionController`, applies the current player-facing instruction labels, renders the UI, and wires card/group/assigned-row/validation clicks plus drag/drop behaviours to that controller. The generated prototype scene also calls the same render method in the Editor before saving, so the scene can show the layout when opened.

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
- `EnsureInstructionText()`: updates the title, instruction copy, column headings, and soft panel surfaces so the prototype explains grouping by intent, click/drag assignment, correction by dragging back or between groups, and Validate.
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
- `ConfigureExistingLabel(...)`: updates existing generated text labels without requiring scene YAML edits.
- `ConfigurePanelSurface(...)`: softens the left and right panel backgrounds.
- `SetOutline(...)`: applies selected, assigned, ready-drop, and panel outline cues.
- `EnsureEventSystem()`: creates an `EventSystem` with `InputSystemUIInputModule` at runtime if the scene does not already contain one.

### Input

- Sample cards and intent ids from `Act1IntentClassificationSampleData`.
- Pointer clicks on rendered message cards, intent group areas, assigned-card rows, and the Validate button.
- Pointer drag events from rendered message cards and assigned-card rows.
- Drop events from intent group areas, intent group scroll viewports, and the left message-card list.

### Output

UGUI objects showing:
- title and instructions explaining that cards should be grouped by speaker intent rather than exact wording
- labelled `Unassigned Messages` and `Intent Groups` columns
- nine sample message cards
- three intent group areas
- short intent-purpose descriptions
- selected-card highlight using a warmer fill and stronger outline
- compact assigned-card rows listed inside each intent group area
- assigned-card row/chip styling distinct from left-side unassigned cards
- scrollable assigned-card lists so assigning many cards to one group does not silently hide them
- a single opaque temporary drag preview while a message card or assigned-card row is being dragged
- basic validation feedback for correct or incorrect grouping inside a small feedback panel

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
- If the title, instruction copy, or column labels still show older wording after presentation changes, enter Play Mode so `RenderSampleData()` applies the updated labels, or rerun the Unity menu builder if a refreshed saved scene preview is needed.

### Unity Test

Manual Act 1 scene check. Open the prototype scene after running the menu builder if needed, enter Play Mode, and repeat the M0-T12 behaviour checks. Also confirm the instruction text explains intent grouping, click/drag assignment, mistake correction, and Validate, and that unassigned cards, intent group panels, assigned chips, selected state, drop-ready group state, and validation feedback are visually distinct.

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

---

## Game Shell Prototype

### Script Name

ShellSceneNames.cs

### Purpose

Stores the shared scene names and scene asset paths used by the Game Shell and Act 1 / Act 2 / Act 3 navigation.

### Attached GameObject

None. This is a static constants class.

### Runtime Role

Used by shell navigation scripts when loading the shell scene, Act 1 scene, Act 2 scene, or Act 3 scene with `SceneManager`.

### Important Fields

- `GameShellSceneName`: scene name used to load the shell.
- `Act1SceneName`: scene name used to load the Act 1 prototype.
- `Act2SceneName`: scene name used to load the Act 2 prototype.
- `Act3SceneName`: scene name used to load the Act 3 prototype.
- `GameShellScenePath`: asset path used by the editor builder.
- `Act1ScenePath`: asset path used by the editor builder and Build Settings registration.
- `Act2ScenePath`: asset path used by the editor builder and Build Settings registration.
- `Act3ScenePath`: asset path used by the editor builder and Build Settings registration.

### Important Methods

None.

### Input

None.

### Output

Scene-name and scene-path constants.

### Failure Cases

If scene asset names are changed later, these constants must be updated to keep SceneManager loading and Build Settings registration aligned.

### Unity Test

Run `Ghost > Build Game Shell Scene`, then confirm the generated shell can load Act 1, Act 2, and Act 3, and that each act's return button can load the shell.

---

### Script Name

GhostNarrativeState.cs

### Purpose

Stores the shell's lightweight in-memory narrative progress for the Acts 1-3 vertical slice: player name, completed act ids, and the act that should debrief when the player returns to the hub.

### Attached GameObject

None. This is a static C# state holder used by shell presentation scripts.

### Runtime Role

Persists across `SceneManager` scene loads during one app session. It resets naturally when the app restarts; it does not save to disk or call any backend.

### Important Fields

- `PlayerName`: display name used for `{playerName}` substitutions, falling back to `Junior`.
- `Act1Id`, `Act2Id`, `Act3Id`: shell narrative ids for the three playable acts.
- Completed act set: tracks which act debriefs have already been consumed.
- `PendingDebriefActId`: the act id set by the return overlay before the shell reloads.

### Important Methods

- `SetPlayerName(...)`: stores a trimmed player name, or the fallback name if blank.
- `SetPendingDebriefAct(...)`: records the act the player is returning from.
- `ConsumePendingDebriefAct()`: reads and clears the pending debrief act id.
- `MarkActCompleted(...)` / `IsActCompleted(...)`: update and query the completed-act set.

### Input

Name-entry text, act ids, and return-to-hub scene context.

### Output

In-memory state for `GameShellPresenter`, `LilyDialogueFrame`, and `ShellReturnToHubOverlay`.

### Failure Cases

Blank or missing names fall back to `Junior`. Completed acts skip repeated debriefs but still return to the hub. The state is intentionally not persistent across app restarts.

### Unity Test

Enter a name in the shell, launch an act, return to the hub, and confirm the shell remembers the name and plays the correct act debrief within the same Play Mode session.

---

### Script Name

ShellDialogueData.cs

### Purpose

Provides data-backed shell dialogue: title, name-entry, hub, per-act intro beats, per-act debrief beats, and the post-Act-3 Ghost closing line. The presenter requests lines by id/act/phase instead of hardcoding narrative text.

### Attached GameObject

None. This is plain C# data used by shell presentation scripts.

### Runtime Role

When the shell changes screens or narrative phases, `GameShellPresenter` requests the matching `ShellDialogueLine` or act beat and passes it to `LilyDialogueFrame`.

### Important Fields

- `TitleScreenId`: id for the title-screen Lily line.
- `NameEntryScreenId`: id for the player-name prompt line.
- `ActHubScreenId`: id for the act-select / hub Lily line.
- `ShellDialogueLine`: immutable speaker/text data.
- `ShellDialogueBeat`: immutable act/phase/speaker/text data.
- `IntroPhaseId`, `DebriefPhaseId`, `ClosingPhaseId`: phases used by `GetBeat(...)`.
- `LilySpeakerName`, `GhostSpeakerName`: speaker ids used by the portrait frame.

### Important Methods

- `GetLine(string screenId)`: returns the Lily line for a known shell screen id.
- `GetBeat(string actId, string phase)`: returns the act-aware intro, debrief, or closing line.
- `GetActTitle(string actId)`: returns a short label for continue-button copy.

### Input

A shell screen id, or an act id plus beat phase.

### Output

A `ShellDialogueLine` containing speaker name and dialogue text. Some text contains `{playerName}` for `LilyDialogueFrame` to substitute.

### Failure Cases

Unknown screen ids or act beats throw `ArgumentException`, which should make missing dialogue wiring obvious during testing.

### Unity Test

Enter Play Mode in the shell scene, enter a name, click each act card, and confirm the dialogue frame shows that act's intro before loading. Return from each act and confirm the matching debrief appears; after Act 3, continue once more and confirm Ghost says the closing line.

---

### Script Name

LilyDialogueFrame.cs

### Purpose

Reusable UI frame for displaying shell narrative lines from `ShellDialogueData`, including Lily and Ghost speaker names, dialogue text, player-name substitution, and a portrait/placeholder slot.

### Attached GameObject

Attached to the `Lily Dialogue Frame` GameObject created by `GameShellSceneBuilder`.

### Runtime Role

Receives `ShellDialogueLine` values, replaces `{playerName}` with `GhostNarrativeState.PlayerName`, writes the speaker name and dialogue text into UGUI `Text` components, and switches the portrait slot based on the current speaker.

### Important Fields

- `speakerNameText`: text component for the current speaker name.
- `dialogueText`: text component for the current narrative line.
- `speakerPortraitImage`: sized Image slot for Lily/Ghost portrait art.
- `portraitPlaceholderText`: placeholder label shown when no speaker sprite is assigned.
- `lilyPortrait`, `ghostPortrait`: optional serialized sprites, intentionally empty until art exists.

### Important Methods

- `Configure(...)`: used by the editor builder to assign the text references.
- `Show(ShellDialogueLine line)`: updates the visible dialogue frame.
- `UpdatePortrait(...)`: selects the Lily/Ghost sprite or labelled placeholder.

### Input

Dialogue data from `ShellDialogueData` and the current player name from `GhostNarrativeState`.

### Output

Visible Lily/Ghost speaker name, portrait placeholder or sprite, and dialogue text in the shell UI.

### Failure Cases

Missing text or portrait references leave that part of the frame unchanged. Empty portrait sprites show the labelled placeholder box.

### Unity Test

Open the shell scene in Play Mode and confirm the frame appears on title, name-entry, hub, intro, debrief, and Act 3 closing beats. Confirm Lily lines show the Lily placeholder and the Act 3 closing line switches to the Ghost placeholder.

---

### Script Name

GameShellPresenter.cs

### Purpose

Controls the shell scene narrative flow: title screen, player-name entry, act-select/hub screen, act intro beats, post-act debrief beats, the Act 3 closing line, and starting Act 1, Act 2, or Act 3.

### Attached GameObject

Attached to the `Game Shell Root` GameObject created by `GameShellSceneBuilder`.

### Runtime Role

On `Start`, it wires the shell buttons. A fresh session shows the title screen; `Start / Continue` opens the name-entry screen, then the hub. Act card clicks show that act's intro line first; the narrative continue button then loads the selected act. If `GhostNarrativeState` has a pending debrief act on shell load, the presenter opens the hub, plays that act's debrief, and queues the Ghost closing line after Act 3.

### Important Fields

- `titleScreen`: root GameObject for the title screen.
- `nameEntryScreen`: root GameObject for the player-name entry step.
- `actHubScreen`: root GameObject for the act-select/hub screen.
- `lilyDialogueFrame`: reusable Lily dialogue UI.
- `startButton`: title-screen button that opens the hub.
- `playerNameInput`: input field used to store the player's name in `GhostNarrativeState`.
- `confirmNameButton`: name-entry button that confirms the player name and opens the hub.
- `act1Button`: hub button that loads Act 1.
- `act2Button`: hub button that loads Act 2.
- `act3Button`: hub button that loads Act 3.
- `narrativeContinueButton`: button used to continue from an intro into an act, or from the Act 3 debrief into the Ghost closing line.
- `backToTitleButton`: hub button that returns to the title screen.

### Important Methods

- `Configure(...)`: used by the editor builder to assign all scene references.
- `ShowTitle()`: shows the title screen and title Lily line.
- `ShowNameEntry()`: shows the player-name prompt screen and name-entry Lily line.
- `ShowActHub()`: shows the act hub and hub Lily line.
- `ConfirmPlayerNameAndShowHub()`: stores the name and opens the hub.
- `ShowActIntro(...)`: shows the selected act's intro beat and arms the continue button.
- `StartAct1()`: loads `Act1IntentClassificationPrototype` through `SceneManager`.
- `StartAct2()`: loads `Act2EntityExtractionPrototype` through `SceneManager`.
- `StartAct3()`: loads `Act3DialogGraphPrototype` through `SceneManager`.
- `PlayPendingDebrief()`: consumes `GhostNarrativeState` pending debrief state and shows post-act narrative.

### Input

Button clicks and player-name input from the shell UI.

### Output

Screen visibility changes, data-driven dialogue-frame text changes, in-memory narrative state updates, and SceneManager loading of Act 1, Act 2, or Act 3.

### Failure Cases

- Missing screen references prevent that screen from being shown or hidden.
- Missing button references mean that button will not be wired.
- Missing name input falls back to `Junior`.
- If Act 1 is not in Build Settings, `StartAct1()` can fail to load the scene.
- If Act 2 is not in Build Settings, `StartAct2()` can fail to load the scene.
- If Act 3 is not in Build Settings, `StartAct3()` can fail to load the scene.

### Unity Test

Open `Assets/Scenes/GameShellPrototype.unity`, enter Play Mode, click `Start / Continue`, enter a player name, and confirm the hub line uses that name. Click each act card, confirm an intro appears first, then click the continue button to load the act. Return from each act and confirm the debrief appears; after Act 3, click continue again and confirm Ghost's closing line appears.

---

### Script Name

ShellSceneNavigationButton.cs

### Purpose

Reusable button helper that loads a configured scene name with `SceneManager`.

### Attached GameObject

Attached to any UGUI Button that should load a scene. M0-T13 uses it for the runtime Act 1 `Return to Hub` button.

### Runtime Role

On `Awake`, it wires the host `Button` to call `LoadTargetScene()`.

### Important Fields

- `targetSceneName`: scene name to load when clicked.

### Important Methods

- `Configure(string sceneName)`: sets the target scene name.
- `LoadTargetScene()`: loads the configured scene if the name is not empty.

### Input

Button click from Unity UI.

### Output

SceneManager loads the configured scene.

### Failure Cases

If `targetSceneName` is empty or the target scene is missing from Build Settings, the button cannot navigate successfully.

### Unity Test

Load Act 1 from the shell, click `Return to Hub`, and confirm the shell scene loads.

---

### Script Name

ShellReturnToHubOverlay.cs

### Purpose

Adds a lightweight return-to-hub UI overlay when the Act 1, Act 2, or Act 3 prototype scene is loaded. This keeps act puzzle rules and pure logic unchanged while still providing shell navigation.

### Attached GameObject

None in the scene. The static runtime hook creates a dedicated `Shell Return To Hub Overlay Canvas` plus a `Shell Return To Hub Overlay` button when the active scene is `Act1IntentClassificationPrototype`, `Act2EntityExtractionPrototype`, or `Act3DialogGraphPrototype`.

### Runtime Role

After scene load, it checks the active scene name. If Act 1, Act 2, or Act 3 is active and no return overlay exists, it creates an EventSystem if needed, then creates a dedicated high-sorting overlay Canvas and adds a top-right `Return to Hub` button wired with `ShellSceneNavigationButton`. Before the button loads the shell, it records the active act in `GhostNarrativeState` so the hub can play the matching debrief.

### Important Fields

No Inspector fields.

### Important Methods

- `RegisterSceneHook()`: registers the scene-loaded callback.
- `CreateForScene(...)`: creates the overlay only for supported act scenes.
- `ShouldShowOverlay(...)`: returns true for Act 1, Act 2, and Act 3 scene names.
- `CreateOverlayCanvas(...)`: builds a separate top-layer Canvas so act prototype UI canvases cannot cover the return button.
- `CreateReturnButton(...)`: builds the placeholder UGUI return button.
- `SetPendingDebriefForActiveScene()`: maps the current act scene to an act id before returning to the shell.

### Input

Unity scene-load events and return button clicks.

### Output

A small `Return to Hub` button in Act 1, Act 2, and Act 3 that marks the pending debrief act and loads `GameShellPrototype`.

### Failure Cases

- If the shell scene is not in Build Settings, the return button cannot load it.
- If another system later creates a button with the same overlay name, this script will treat it as already present.

### Unity Test

Start from the shell, enter Act 1, Act 2, and Act 3, confirm the `Return to Hub` button appears above each act's own UI, and click it to return to the shell. Confirm the hub dialogue changes to the returned act's debrief.

---

### Script Name

GameShellSceneBuilder.cs

### Purpose

Editor-only helper that creates the placeholder Game Shell scene through Unity-supported scene serialization. It builds the title, name-entry, act hub, companion placeholders, dialogue portrait frame, and shell navigation wiring. The existing builder still registers the shell, Act 1, Act 2, and Act 3 scenes when the menu is run.

### Attached GameObject

None. This script lives under an `Editor` folder and runs from Unity Editor menu items.

### Runtime Role

No runtime role. It is excluded from player builds by `Assets/Presentation/Shell/Editor/Ghost.Presentation.Shell.Editor.asmdef`.

### Important Fields

No Inspector fields.

### Important Methods

- `BuildGameShellScene()`: creates `Assets/Scenes/GameShellPrototype.unity`, builds the placeholder UGUI title/name-entry/hub/companion/dialogue layout, wires `GameShellPresenter`, and uses the existing shell registration flow.
- `RegisterGameShellBuildSettings()`: updates Build Settings without rebuilding the shell scene.
- `CreateNameEntryScreen(...)`: builds the name-entry screen with an `InputField` and confirm button.
- `CreateLilyDialogueFrame(...)`: builds the dialogue frame with a speaker portrait Image slot plus placeholder label.
- `CreateActCardRow(...)`: lays out the three act cards horizontally so the act hub does not push the Lily dialogue frame outside the viewport.
- `CreateActCard(...)`: creates reusable act-select cards for Act 1, Act 2, and Act 3.

### Input

Manual Unity Editor menu actions:
- `Ghost > Build Game Shell Scene`
- `Ghost > Register Game Shell Build Settings`

### Output

- `Assets/Scenes/GameShellPrototype.unity`, when Unity can execute the builder successfully.
- `ProjectSettings/EditorBuildSettings.asset` updated to include the shell, Act 1, Act 2, and Act 3 scenes when the Unity menu builder/register action is run.

### Failure Cases

- If Unity cannot execute the editor builder, Codex cannot safely create the scene asset because hand-writing `.unity` YAML is out of scope.
- If compile errors exist, the menu item may not be available until they are fixed.
- If Build Settings are not updated, SceneManager scene loading can fail.

### Unity Test

Run `Ghost > Build Game Shell Scene`, open `Assets/Scenes/GameShellPrototype.unity`, enter Play Mode, confirm name entry appears after `Start / Continue`, confirm the act hub keeps the Lily dialogue frame fully inside the viewport, confirm the dialogue portrait placeholder switches between Lily and Ghost, confirm `Start Act 1`, `Start Act 2`, and `Start Act 3` show intro beats before loading, launch all three acts, and confirm `Return to Hub` plays the correct debrief from each.

---

## Act 2 Entity Extraction Runtime

### Script Name

EntityType.cs

### Purpose

Defines the value object for an Act 2 entity type. Each entity type has:
- an `Id` used by validators and sample data
- an `EntityCategory` showing whether the type is a built-in `System` entity or a game-specific `Custom` entity

### Attached GameObject

None. This is pure C# data and should not be attached to a GameObject.

### Runtime Role

Created by sample data, tests, or future puzzle data before validation. It does not run by itself.

### Important Fields

No serialized Unity fields. The constructor receives `id` and `category`.

### Important Methods

- `EntityType(string id, EntityCategory category)`: creates an entity type and rejects empty ids.
- `Equals(...)`, `GetHashCode()`, `==`, and `!=`: compare entity types by `Id` and `Category`.

### Input

Plain C# constructor values.

### Output

An immutable entity-type value object that can distinguish, for example, a system `time` entity from a custom game entity.

### Failure Cases

- Empty entity type id throws an `ArgumentException`.
- Entity types with the same id but different categories are not equal.

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/Act2EntityExtractionSampleDataTests.cs` and `Assets/Tests/EditMode/Act2EntityExtractionValidatorTests.cs`. This script has no Play Mode behaviour.

---

### Script Name

EntitySpan.cs

### Purpose

Defines one Act 2 span annotation over a message. Each span stores:
- `Start`: zero-based character index
- `Length`: number of characters in the span
- `Type`: the `EntityType` assigned to the span

### Attached GameObject

None. This is pure C# data and should not be attached to a GameObject.

### Runtime Role

Created by sample data, tests, or future player-submission code before validation. It does not run by itself.

### Important Fields

No serialized Unity fields. The constructor receives `start`, `length`, and `type`.

### Important Methods

- `EntitySpan(int start, int length, EntityType type)`: creates a span and rejects invalid boundaries or null type.
- `GetText(string message)`: returns the substring covered by the span.
- `Equals(...)`, `GetHashCode()`, `==`, and `!=`: compare spans by start, length, and entity type.

### Input

Plain C# constructor values and, optionally, a message string for `GetText(...)`.

### Output

An immutable span annotation that the validator can compare against expected answers.

### Failure Cases

- Negative start throws an `ArgumentOutOfRangeException`.
- Zero or negative length throws an `ArgumentOutOfRangeException`.
- Null type throws an `ArgumentNullException`.
- `GetText(...)` throws if the message is null or the span falls outside the message.

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/Act2EntityExtractionSampleDataTests.cs` and `Assets/Tests/EditMode/Act2EntityExtractionValidatorTests.cs`. This script has no Play Mode behaviour.

---

### Script Name

EntityExtractionValidator.cs

### Purpose

Validates whether submitted Act 2 entity spans exactly match the expected span/type answer key. This mirrors Act 1's deterministic validator pattern: correctness is decided by pure C# rules, not by UI, scene state, backend, or LLM output.

### Attached GameObject

None. This is pure C# puzzle logic and should not be attached to a GameObject.

### Runtime Role

Future Act 2 UI or puzzle controller code can call `EntityExtractionValidator.Validate(...)` after the player highlights message spans and assigns entity types.

### Important Fields

No serialized Unity fields.

### Important Methods

- `Validate(IEnumerable<EntitySpan> expected, IEnumerable<EntitySpan> submitted)`: compares submitted annotations against the expected annotations.
- `EntityExtractionResult.IsCorrect`: true only when submitted spans exactly match expected spans.
- `EntityExtractionResult.Errors`: validation messages for missing spans, wrong type, wrong boundary, extra spans, duplicate submitted spans, and null spans.

### Input

- Expected `EntitySpan` values representing the authored answer key.
- Submitted `EntitySpan` values representing the player's annotation attempt.

### Output

An `EntityExtractionResult` with a boolean correctness flag and error details for future UI feedback or tests.

### Failure Cases

The validator returns incorrect results with errors for:
- missing expected spans
- correct boundary but wrong entity type/category
- correct entity type/category but wrong boundary
- extra submitted spans
- duplicate submitted spans
- null spans inside either input list

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/Act2EntityExtractionValidatorTests.cs`. This script has no Play Mode behaviour.

---

### Script Name

Act2EntityExtractionSampleData.cs

### Purpose

Provides reusable sample data for the Act 2 entity-extraction puzzle. The data demonstrates span annotation, system vs custom entities, and a synonym case where `lab` and `laboratory` are different surface words for the same custom `room` entity type.

### Attached GameObject

None. This is pure C# sample data and should not be attached to a GameObject.

### Runtime Role

Future UI or puzzle controller code can call this class to get sample messages and their correct entity spans. The class does not run by itself.

### Important Fields

No serialized Unity fields.

Constants:
- `TimeEntityTypeId`
- `RoomEntityTypeId`
- `ObjectEntityTypeId`

### Important Methods

- `CreateTimeEntityType()`: returns the system `time` entity type.
- `CreateRoomEntityType()`: returns the custom `room` entity type.
- `CreateObjectEntityType()`: returns the custom `object` entity type.
- `CreateMessages()`: returns sample messages with correct spans.
- `SampleMessage`: immutable message text plus correct span list.

### Input

None. The sample data is created by method calls.

### Output

Three short Ghost-themed messages:
- one message with a custom room span and a system time span
- one message with the synonym surface word `laboratory` mapped to the custom room entity type
- one message with a custom object span

### Failure Cases

- If a sample surface phrase is edited and no longer appears in the message, sample creation throws an `InvalidOperationException`.
- If sample ids or spans are changed later, the sample data tests should be updated to preserve the system/custom and synonym coverage.

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/Act2EntityExtractionSampleDataTests.cs`. This script has no Play Mode behaviour.

---

## Act 2 Entity Extraction EditMode Tests

### Script Name

Act2EntityExtractionValidatorTests.cs

### Purpose

Tests the pure Act 2 entity-extraction validator.

### Attached GameObject

None. This is an EditMode test script and should not be attached to a GameObject.

### Runtime Role

Runs in Unity's EditMode Test Runner only.

### Important Fields

No serialized Unity fields.

### Important Methods

NUnit tests cover:
- exact correct span/type submission
- missing span
- wrong type
- wrong boundary
- extra span
- duplicate submitted span

### Input

Test-created `EntitySpan` and `EntityType` values.

### Output

NUnit pass/fail results.

### Failure Cases

- Compile errors indicate the runtime model or validator signature changed.
- Failed assertions indicate the validator no longer reports one of the required deterministic error categories.

### Unity Test

Run the EditMode tests in Unity Test Runner. This script has no Play Mode behaviour.

---

## M0-T35 Chatbot Fundamentals Shell Sequence

### Script Name

ChatbotFundamentalsData.cs

### Purpose

Data source for the compact Game Shell fundamentals sequence. It defines the six IBM-course overview
beats: chatbot definition, NLP/ML pillars, rule-based vs AI-enabled contrast, benefits,
five-component overview, and four common chatbot challenges.

### Attached GameObject

None. Static presentation data only.

### Runtime Role

Provides short problem/explanation/action/consequence text plus component and challenge labels to
`ChatbotFundamentalsPresenter`.

### Important Fields

No serialized Unity fields. Static factory methods return copied read-only lists.

### Important Methods

- `CreateBeats()` returns the six teaching beats.
- `CreateComponentOrder()` and `CreateComponentPaletteOrder()` provide the five-component ordering
  mini-interaction data.
- `CreateChallengeModes()` provides the four challenge failure modes.

### Input

No runtime input.

### Output

In-memory data consumed by the presenter.

### Failure Cases

- If a beat is missing or misordered, the Shell overview no longer covers the required M0-T35
  fundamentals.

### Unity Test

Run the Shell scene in Play Mode after rebuilding it with `Ghost > Build Game Shell Scene`.

---

### Script Name

ChatbotFundamentalsPresenter.cs

### Purpose

Runs the playable fundamentals overview in the Game Shell. Each beat requires a small action before
the player can continue, then shows a visible Ghost consequence.

### Attached GameObject

Attached by `GameShellSceneBuilder` to the generated `Chatbot Fundamentals Screen`.

### Runtime Role

Shows the current beat, creates runtime action buttons, updates Ghost/Lily/consequence text, handles
the component-order mini-interaction, and returns to the act hub when finished or skipped.

### Important Fields

- Text fields for progress, title, Ghost problem, Lily explanation, action prompt, consequence, Ghost
  status, component path, backend side link, and feedback.
- Dynamic button roots for simple actions, component ordering, and challenge modes.
- Previous / Next / Skip buttons.
- Optional `LilyDialogueFrame` reference so Lily's explanation also appears in the shared portrait
  dialogue frame.

### Important Methods

- `Begin()` resets the sequence and renders the first beat.
- `ShowNextBeat()` only advances after the current action has produced a consequence.
- `RenderComponentOrderBeat(...)` lets the player arrange the overview path and attach backend
  integration as a side link.
- `RenderChallengeModesBeat(...)` makes the player trigger the four challenge failure modes.

### Input

Player button clicks in the Shell overview.

### Output

Updated Shell UI text, Ghost reaction text, and the `Finished` event used by `GameShellPresenter` to
return to the act hub.

### Failure Cases

- Wrong component order resets the mini-interaction and shows a wrong-order Ghost consequence.
- Pressing Next before an action shows a feedback prompt instead of advancing.

### Unity Test

Run Play Mode after rebuilding the shell; verify all six beats require action and can finish or skip
back to the hub.

---

### Script Name

GameShellPresenter.cs

### Purpose

Adds a Shell-level entry point for the fundamentals overview while preserving existing title,
name/account, narrative, and Act 1-3 launch flows.

### Attached GameObject

Generated on the `Game Shell Root` GameObject by `GameShellSceneBuilder`.

### Runtime Role

Wires the `Ghost's Voice Basics` hub button, shows/hides the fundamentals screen, starts
`ChatbotFundamentalsPresenter.Begin()`, and returns to the hub when the overview finishes.

### Important Fields

`fundamentalsScreen`, `fundamentalsPresenter`, and `fundamentalsButton`.

### Important Methods

- `ShowFundamentals()` switches from hub/name/title screens into the overview.
- Existing `ShowTitle`, `ShowNameEntry`, `ShowActHub`, and `ShowActIntro` now hide the fundamentals
  screen.

### Input

The fundamentals hub button and the presenter's `Finished` event.

### Output

Screen transitions inside the Game Shell.

### Failure Cases

- If the presenter reference is missing, `ShowFundamentals()` safely returns to the hub.

### Unity Test

Run `Ghost > Build Game Shell Scene`, enter Play Mode, open `Ghost's Voice Basics`, finish or skip it,
and confirm Act 1-3 buttons still work.

---

### Script Name

GameShellSceneBuilder.cs

### Purpose

Builds the Game Shell UI with the new `Chatbot Fundamentals Screen` and a `Ghost's Voice Basics`
entry card in the act hub.

### Attached GameObject

Editor-only scene builder. It creates and wires GameObjects in `Assets/Scenes/GameShellPrototype.unity`
when the menu item is run.

### Runtime Role

No runtime role after scene generation.

### Important Fields

No serialized fields.

### Important Methods

- `CreateFundamentalsScreen(...)` creates the overview UI and configures
  `ChatbotFundamentalsPresenter`.
- `CreateFundamentalsHubCard(...)` adds the hub entry button.
- `CreateShellUi(...)` wires the fundamentals screen, presenter, and hub button into
  `GameShellPresenter`.

### Input

Editor menu action `Ghost > Build Game Shell Scene`.

### Output

Regenerated Game Shell scene containing the fundamentals overview UI.

### Failure Cases

- If the menu builder is not rerun, the existing scene may not show the new overview entry.

### Unity Test

Run the builder, then test the shell in Play Mode.

---

## M0-T28 Unity Client Backend Integration

### Script Name

GhostBackendConfig.cs

### Purpose

Stores the Unity client's backend base URL and request timeout. The default URL is `http://localhost:3000`, and the base URL can be overridden at runtime or through PlayerPrefs.

### Attached GameObject

None. This is a static presentation helper.

### Runtime Role

`GhostBackendClient` reads this config before each UnityWebRequest.

### Important Fields

- `DefaultBaseUrl`: local backend URL.
- `BaseUrlPlayerPrefsKey`: PlayerPrefs key for overriding the URL.
- `RequestTimeoutSeconds`: clamped short timeout for graceful degradation.

### Important Methods

- `BuildUrl(...)`: combines the configured base URL with an endpoint path.

### Failure Cases

- Empty or whitespace base URLs fall back to the local default.

### Unity Test

In Play Mode, override `GhostBackendConfig.BaseUrl` from a debug console or script if a non-default backend port is needed, then verify backend calls still degrade gracefully if the URL is wrong.

---

### Script Name

GhostBackendClient.cs

### Purpose

Provides a WebGL-safe, best-effort UnityWebRequest API client for the M0-T27 backend. It creates/reuses a pseudonymous profile, reads/writes progress, and logs puzzle attempts.

### Attached GameObject

None manually. The client creates a hidden persistent runner GameObject named `Ghost Backend Client Runner` to host coroutines.

### Runtime Role

Initialized before scene load. Public methods start coroutines and invoke callbacks with success/failure results. Network failures, timeouts, or offline backend states log warnings only and do not throw or block gameplay.

### Important Methods

- `EnsureProfile(...)`: reuses the PlayerPrefs profile id or POSTs `/profiles` and stores the returned id.
- `GetProgress(...)`: GETs `/progress/:profileId`.
- `PutProgress(...)`: PUTs completed acts/levels and narrative state to `/progress/:profileId`.
- `PostAttempt(...)`: POSTs act id, correct/incorrect result, and brief details to `/attempts`.
- `PostHint(...)`: POSTs an act id, trigger, and player-facing state summary to `/hints`; it uses a longer non-blocking LLM timeout so local Granite has time to respond, and failures are callback-only.
- `PostResponse(...)`: POSTs an act id and state summary to `/responses` for optional generated Ghost text, using the same longer non-blocking LLM timeout.
- `PostChat(...)`: POSTs an act id, typed player message, short chat history, and player name to `/chat`; failures are callback-only so the Lily chat window can show local static fallback text.
- `CreateAttemptDetails(...)`: packages validator error count and messages as analytics details.

### Input

Backend endpoint paths, profile id, progress snapshots, and attempt details from presentation controllers.

### Output

Best-effort backend writes/reads plus callback result objects. It never decides correctness. Hint/response text is display-only natural language.

### Failure Cases

- Backend down, timeout, parse failure, stale profile id, or HTTP error returns a failed response and logs a warning.
- If no profile can be created, progress/attempt/hint calls are skipped through failed callbacks; local static hints remain available through the banter panel.

### Unity Test

Run the game with the backend up and confirm profile/progress/attempt/hint requests are visible in the backend. Then stop the backend and confirm the same puzzles remain fully playable with warning-only degradation and static hints.

---

### Script Name

BackendSync.cs

### Purpose

Coordinates narrative progress sync between `GhostNarrativeState` and the backend. It starts once, ensures a profile, loads backend progress if available, and pushes progress whenever the local narrative state changes.

### Attached GameObject

None. This is a static presentation coordinator.

### Runtime Role

Starts from a runtime initialize hook and is also explicitly ensured by `GameShellPresenter.Start()`. It subscribes to `GhostNarrativeState.StateChanged`.

### Important Methods

- `EnsureStarted()`: starts sync once.
- `PushProgress()`: sends the current player name and completed acts through `GhostBackendClient.PutProgress(...)`.

### Input

Local narrative state and backend progress responses.

### Output

Applies fetched backend player name/completed acts into `GhostNarrativeState` if fetch succeeds, and best-effort saves local progress back to the backend.

### Failure Cases

- If the backend is unavailable, sync silently keeps local in-memory behaviour except for warning logs from the client.

### Unity Test

With backend running, enter a player name, complete/return from acts, restart Play Mode, and confirm progress reloads. With backend stopped, confirm the shell and acts still work.

---

### Script Name

GhostNarrativeState.cs (M0-T28 sync additions)

### Purpose

Keeps the existing in-memory narrative fallback while exposing backend sync hooks.

### Runtime Role

Stores player name, completed act ids, pending debrief id, and the persisted backend profile id.

### Important Methods

- `SetBackendProfileId(...)`: stores or clears the pseudonymous backend profile id in PlayerPrefs.
- `GetCompletedActIds()`: returns a sorted snapshot for progress sync.
- `ApplyBackendProgress(...)`: merges backend player name/completed acts into local narrative state.
- `StateChanged`: event raised when player name or completed acts change.

### Unity Test

Confirm name entry and act completion still work offline, then confirm backend sync writes the same state when the backend is available.

---

### Script Name

Act 1/2/3 interaction controllers (M0-T28 attempt logging)

### Purpose

After each existing deterministic Validate call, the presentation controller sends a best-effort attempt log to the backend.

### Runtime Role

The controllers still call their existing sessions/validators for correctness. The backend receives only the result string (`correct` or `incorrect`) plus brief analytics details; it does not score the attempt.

### Important Methods

- `Act1IntentClassificationInteractionController.ValidateCurrentGrouping()`
- `Act2EntityExtractionInteractionController.ValidateCurrentState()`
- `Act3DialogGraphInteractionController.ValidateCurrentState()`

### Failure Cases

- Backend failures do not affect validation feedback or puzzle state.

### Unity Test

Run Validate in each act with backend up and confirm attempts are inserted. Stop the backend and confirm Validate feedback still appears normally.

---

### Component Name

Backend CORS Middleware (`Backend/src/app.ts`, M0-T28)

### Purpose

Adds minimal permissive local-development CORS headers so Unity WebGL/browser builds can call the local M0-T27 backend.

### Runtime Role

Runs before JSON parsing in the Express app. It allows `GET`, `POST`, `PUT`, and `OPTIONS`, and returns `204` for preflight requests.

### Failure Cases

- This is intentionally broad for local prototype development and should be revisited before any hosted deployment.

### Test

Run `npm run build` and `npm test` from `Backend/`. In browser/WebGL verification, confirm backend calls are not blocked by CORS.

---

## M0-T29 LLM Orchestration

### Component Name

Backend Ollama + Granite orchestration (`Backend/src/ollamaClient.ts`, `Backend/src/llmOrchestration.ts`, `/hints`, `/responses`, `/chat`)

### Purpose

Adds local LLM-backed natural-language support for Lily hints, constrained Lily chat, and Ghost response text. The LLM never decides correctness, never receives puzzle answer keys, and never gates progression.

### Runtime Role

The backend reads act learning metadata from `learning_content`, builds a curriculum-aware prompt, and calls Ollama's local `/api/generate` endpoint. Generation uses a longer default timeout because local Granite cold starts can be slow. If Ollama is unavailable, errors, or times out, the backend logs the Ollama URL/model/error and returns HTTP 200 with static fallback text and `source: "static"`.

### Important Files

- `Backend/src/ollamaClient.ts`: fetch-based Ollama client, env config (`OLLAMA_URL`, `OLLAMA_MODEL`, `OLLAMA_TIMEOUT_MS`, `OLLAMA_CHECK_TIMEOUT_MS`), a 60-second default generate timeout, and a short model-list helper timeout.
- `Backend/src/llmOrchestration.ts`: Lily/Ghost system prompts, constrained Lily chat prompt, static hints/responses/chat fallback, prompt sanitisation, fallback warnings, and hint logging with trigger/state context.
- `Backend/src/checkOllama.ts`: `npm run check:ollama` command for local setup checks plus one timed test generation.
- `Backend/src/database.ts`: `getLearningContentSummary(...)`, `insertHintLog(...)`, `getHintLogCount()`, and `getLatestHintLogPayload()` test helper.

### Client Flow

- `GhostBackendClient.PostHint(...)` calls `/hints` best-effort through UnityWebRequest and sends a non-spoiler `trigger` plus `state.summary`.
- `GhostBackendClient.PostChat(...)` calls `/chat` best-effort through UnityWebRequest and sends typed player text plus a short in-memory history.
- `AmbientBanterPanel` exposes an `Ask Lily` button and a static `RequestHint(...)` helper for incorrect validation events. Both open the dedicated Lily chat window instead of writing hint text into the ambient banter strip.
- `LilyChatWindow` pauses ambient banter while open, sends chat turns to `/chat`, appends Lily replies to the scrollable chat list, and shows local static fallback if the backend or LLM fails.
- Act 1, Act 2, and Act 3 presentation interaction controllers open Lily chat after an incorrect deterministic Validate result.
- `BanterData.GetStaticHint(...)` and `BanterData.GetStaticChatReply(...)` provide local fallbacks used when the backend or LLM is unavailable.

### Failure Cases

- Ollama unavailable: backend returns static fallback; tests cover this path.
- Backend unavailable from Unity: `GhostBackendClient` fails callback; `LilyChatWindow` displays local static Lily chat fallback.
- Unity panel missing: incorrect Validate still works; the hint request no-ops.

### Test

From `Backend/`, run `npm install`, `npm run build`, and `npm test`. Use `npm run check:ollama` to verify a live local Ollama + Granite setup and see timed generation latency. In Unity Play Mode, enter each act, click `Ask Lily`, confirm the dedicated chat window opens and ambient banter pauses, type a question, then close the chat and confirm banter resumes. Repeat with backend/Ollama stopped and confirm static fallback.

---

## M0-T28 Run 002 No-Password Account Recovery

### Component Name

Prototype account recovery (`Backend/src/database.ts`, `Backend/src/app.ts`, `GhostBackendClient.cs`, `GameShellPresenter.cs`)

### Purpose

Adds an optional no-password account layer on top of the existing pseudonymous profile/progress system. A tester can create a readable username or later enter either that username or the generated `account_...` id to recover the same backend profile and progress. This is prototype progress recovery, not secure authentication.

### Runtime Role

The backend stores an `accounts` row that links one `userName` and generated `accountId` to one existing or newly created `profileId`. Unity keeps the existing guest path, but the shell name-entry screen can now create or use an account before entering the act hub. When an account is found, Unity stores the returned profile id, loads `/progress/:profileId`, and applies the restored player name/completed acts. If the current local profile already has an account, `Create Account` creates a separate new profile/account when the requested username is not already used by another profile, so multiple prototype accounts can coexist.

### Important Files

- `Backend/src/database.ts`: creates the `accounts` table and implements `createAccount(...)` / `findAccount(...)`.
- `Backend/src/app.ts`: exposes `POST /accounts` and `POST /accounts/lookup`.
- `Backend/tests/app.test.ts`: covers account creation, lookup by username/account id, progress recovery, and duplicate username rejection.
- `Assets/Presentation/Backend/GhostBackendClient.cs`: adds `CreateAccount(...)` and `LookupAccount(...)` UnityWebRequest wrappers.
- `Assets/Presentation/Shell/GhostNarrativeState.cs`: stores optional backend account id / username in PlayerPrefs and supports replacing local progress when restoring an account.
- `Assets/Presentation/Shell/GameShellPresenter.cs`: handles create/use account button clicks and progress loading.
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`: regenerates the shell name-entry UI with a compact two-column name/account layout, the account input, and the create/use buttons.

### Failure Cases

- Backend unavailable: account create/use fails with a shell status message; `Continue as Guest` still works.
- Duplicate username owned by another profile: backend returns `409`; Unity asks the player to use that account or choose a different username.
- Creating a new username while already on an account switches Unity to a new profile/account instead of overwriting the old account.
- No password: anyone with the username or account id can load that prototype progress on the same backend. This is intentional until password/auth design is approved.

### Unity Test

Run `Ghost > Build Game Shell Scene` after code changes so the scene contains the current generated account controls. Start the backend, enter Play Mode in the shell, type a player name and username, click `Create Account`, complete/return from an act, then restart and use the same username via `Use Account`. Confirm the hub restores the name/completed-act state. Stop the backend and confirm `Continue as Guest` still enters the hub.

---

## M0-T33 Constrained Lily Chat

### Component Name

Constrained Lily chat (`POST /chat`, `GhostBackendClient.PostChat(...)`, `LilyChatWindow.cs`)

### Purpose

Turns Ask Lily into a dedicated free-text chat window while keeping the LLM constrained to one short in-character sentence about the current act's chatbot/NLP concept and Ghost story situation.

### Runtime Role

The backend `/chat` endpoint receives the current act id, typed player message, short recent history, player name, optional profile id, and level. It builds a Lily persona and guardrail prompt with act learning metadata only. It never includes puzzle answer keys, never decides scoring, and logs each chat turn to `hint_logs` as `kind:"chat"` / `trigger:"chat_message"`.

### Important Files

- `Backend/src/llmOrchestration.ts`: `createLilyChatReply(...)`, Lily chat system prompt, short-history prompt construction, and static chat fallback.
- `Backend/src/app.ts`: exposes `POST /chat`.
- `Backend/tests/app.test.ts`: covers `/chat` static fallback and `hint_logs` chat payload.
- `Assets/Presentation/Backend/GhostBackendClient.cs`: `PostChat(...)` coroutine wrapper and JSON payload/response classes.
- `Assets/Presentation/Banter/LilyChatWindow.cs`: runtime-created UGUI chat window with scrollable message list, text input, Send, and Close.
- `Assets/Presentation/Banter/AmbientBanterPanel.cs`: Ask Lily and incorrect Validate now open chat, pause ambient banter, and resume it on close.
- `Assets/Presentation/Banter/AmbientBanterHook.cs`: Act 2 banter style adjusted to a slimmer, readable validation-area strip.

### Failure Cases

- Ollama unavailable: backend returns HTTP 200 with `source:"static"` and logs the chat turn.
- Backend unavailable from Unity: the chat window appends a local static Lily line and gameplay continues.
- Off-topic/private-life handling is enforced by the LLM prompt and must be manually checked with live Ollama.

### Unity Test

No Inspector setup is required. In Play Mode, enter any act, click `Ask Lily`, type an on-topic question, and confirm Lily replies in one short sentence. Ask an off-topic question and a private-life question to confirm redirect/deflection. Stop the backend and confirm a local static line appears. Close the chat and confirm ambient banter resumes.

---

## M0-T27 Backend / Database Foundation

### Component Name

Backend REST Service (`Backend/src/server.ts`, `Backend/src/app.ts`)

### Purpose

Creates the first local server-side component for Ghost: a small Node.js + TypeScript + Express REST API. It serves seeded reference learning/puzzle content, creates pseudonymous local profiles, stores progress, and logs attempts.

### Runtime Role

Run locally from `Backend/` with `npm run dev` during development or `npm run build` then `npm start` for compiled output. This service is outside Unity and is not wired to the Unity client yet.

### Important Files

- `Backend/src/server.ts`: starts the local Express server and owns the SQLite connection lifetime.
- `Backend/src/app.ts`: defines the REST endpoints and request/response handling, including `/hints` and `/responses`.
- `Backend/src/database.ts`: creates the SQLite schema, seeds reference content, and performs profile/progress/attempt/hint-log operations.
- `Backend/src/ollamaClient.ts`: small fetch-based Ollama client plus `check:ollama` support.
- `Backend/src/llmOrchestration.ts`: curriculum-aware Lily/Ghost prompts and static fallbacks.
- `Backend/src/checkOllama.ts`: human-readable local Ollama/model availability check plus a timed test generation.
- `Backend/src/seedData.ts`: contains Act 1-3 reference content mirrored from the C# sample data.
- `Backend/tests/app.test.ts`: covers seeded content, profile/progress round trip, attempts logging, and the no-live-LLM static fallback path.

### Important Endpoints

- `GET /health`: returns `{ ok: true }`.
- `GET /content`: returns seeded act/level metadata and puzzle content.
- `POST /profiles`: creates a pseudonymous profile id.
- `GET /progress/:profileId`: reads progress for a profile.
- `PUT /progress/:profileId`: upserts completed acts/levels and narrative state JSON.
- `POST /hints`: returns `{ hint, source }`, using Ollama/Granite when available and static hints otherwise; logs kind/source/level/trigger/state/error to `hint_logs`.
- `POST /responses`: returns `{ text, source }`, using Ollama/Granite when available and static Ghost response text otherwise.
- `POST /chat`: returns `{ reply, source }`, using constrained Lily chat when available and static Lily chat fallback otherwise.
- `POST /attempts`: stores attempt-log analytics data.

### Deterministic Correctness Rule

The backend does not score puzzle submissions and does not expose a scoring endpoint. Seeded answer-key JSON is stored only as reference/analytics data and is not included in LLM prompts. Unity-side deterministic validators remain authoritative for correctness.

### Failure Cases

- Missing profiles return `404` for progress or attempt insertion.
- Missing `profileId`, `actId`, or `result` on `POST /attempts` returns `400`.
- SQLite data files are local runtime artifacts and are ignored by git.
- Ollama errors/timeouts on `/hints`, `/responses`, and `/chat` return HTTP 200 with `source: "static"` so gameplay can continue; fallback warnings include the configured Ollama URL, model, and actual error.

### Test

From `Backend/`, run:

1. `npm install`
2. `npm run build`
3. `npm test`
4. `npm run check:ollama` if local Ollama availability needs to be checked.

These are backend checks, not Unity Play Mode tests.

---

## In-Act Ambient Banter

### Script Name

BanterData.cs

### Purpose

Stores static, data-driven ambient Ghost/Lily banter loops for Act 1, Act 2, and Act 3.

### Attached GameObject

None. This is plain C# data used by `AmbientBanterHook` and `AmbientBanterPanel`.

### Runtime Role

Provides per-act lists of `AmbientBanterBeat` values. Each beat has a speaker, text, optional tag, beat kind, and a future-choice placeholder list so later player-choice or LLM extensions can be added without replacing the data shape.

### Important Fields

- `AmbientBanterBeat`: immutable speaker/text/tag/kind/future-choice data.
- `AmbientBanterBeatKind`: currently only `Line`; exists to keep future beat types explicit.
- Act 1 beats: 16 nervous Lily lines plus 16 garbled Ghost lines.
- Act 2 beats: 16 warmer Lily lines, including the first joke/backpedal beat, plus 16 Ghost-catching-details lines.
- Act 3 beats: 16 jokier Lily lines, including nerdy-joke-then-embarrassed beats, plus 16 clearer Ghost lines.

### Important Methods

- `GetBeats(string actId)`: returns the ambient loop for `act1`, `act2`, or `act3`.
- `GetStaticHint(string actId)`: returns a local non-spoiler Lily hint for use when the backend or Ollama is unavailable.
- `GetStaticChatReply(string actId)`: returns a short in-character Lily chat fallback for offline chat.

### Input

An act id from `GhostNarrativeState`.

### Output

A read-only list of authored ambient beats for that act.

### Failure Cases

Unknown act ids return an empty beat list, so the runtime hook will not spawn an empty panel.

### Unity Test

Enter each act in Play Mode and confirm the banter panel uses act-appropriate Lily/Ghost lines from this data, with at least 15 Lily lines and 15 Ghost lines available per act.

---

### Script Name

AmbientBanterPanel.cs

### Purpose

Displays a compact, non-blocking UGUI banter panel with the current speaker, dialogue text, and a portrait placeholder. It cycles through the current act's beats on a timer and includes an `Ask Lily` button that opens the dedicated Lily chat window.

### Attached GameObject

Attached at runtime to the `Ambient Banter Panel` GameObject created by `AmbientBanterHook`.

### Runtime Role

Receives an act's banter beats, shows the first line, substitutes `{playerName}` from `GhostNarrativeState`, swaps Lily/Ghost portrait placeholders by speaker, advances after a few seconds, and loops back to the beginning. M0-T33 makes the button open `LilyChatWindow`; the ambient loop pauses while chat is open and resumes when chat closes.

### Important Fields

- `speakerNameText`: visible speaker label.
- `dialogueText`: visible banter line.
- `speakerPortraitImage`: sized placeholder Image for future Lily/Ghost sprites.
- `portraitPlaceholderText`: label shown when no sprite is assigned.
- `nextButton`: runtime button labelled `Ask Lily`; it opens the dedicated Lily chat window instead of deciding correctness.
- `cycleSeconds`: timer interval for automatic cycling.
- `lilyPortrait`, `ghostPortrait`: optional sprites left empty for placeholder art.

### Important Methods

- `Configure(...)`: assigns runtime-created UI references, cycle timing, and current act id.
- `Initialize(...)`: stores the act beat list, wires the Ask Lily button, and shows the first beat.
- `RequestHint(...)`: static helper used by act controllers after incorrect validation; opens `LilyChatWindow` with an opening Lily line.
- `PauseForChat()` / `ResumeAfterChat()`: used by `LilyChatWindow` to pause and resume ambient cycling.
- `Update()`: advances the loop when the timer elapses, but stays paused while chat is open.

### Input

Ambient beats from `BanterData`; chat opens through `LilyChatWindow`.

### Output

A cycling, visible ambient banter strip plus an entry point into Lily chat. It does not decide puzzle correctness.

### Failure Cases

Empty or missing beat lists show no text. Missing portrait sprites intentionally show labelled placeholders. Chat/backend/Ollama failures are handled by `LilyChatWindow`, not by the ambient strip.

### Unity Test

Enter an act in Play Mode, watch the panel cycle, click `Ask Lily`, and confirm a separate chat window opens and pauses ambient cycling. Close the chat and confirm the panel resumes.

---

### Script Name

AmbientBanterHook.cs

### Purpose

Runtime scene-load hook that spawns the ambient banter panel in Act 1, Act 2, and Act 3 scenes without editing scene YAML.

### Attached GameObject

None in authored scenes. The static hook creates a temporary `Ambient Banter Bootstrapper` after scene load, then creates an `Ambient Banter Panel` after the act presenter has rendered. It uses an `Ambient Banter Canvas` only as a fallback if no suitable act UI host can be found.

### Runtime Role

On scene load, maps the active scene name to an act id using `ShellSceneNames`, gets that act's banter beats, creates an EventSystem if needed, waits briefly for the act presenter to finish layout, then embeds the banter panel into existing act UI space. Act 1 and Act 2 use the `Validation Controls` row; Act 3 uses the right-side `Goal Test List`. Each act has its own panel style: Act 1 uses a taller validation-row panel to avoid clipped text, Act 2 uses a slimmer validation-row panel, and Act 3 uses a taller guide-panel card for wrapped lines. A low-sorting fallback Canvas is created only if those hosts are unavailable.

### Important Fields

- `FallbackCanvasName`: runtime fallback canvas name, used only when no act layout host can be found.
- `BootstrapperName`: temporary runtime object that waits for act UI layout before creating the panel.
- `PanelName`: runtime duplicate guard for the banter panel.
- `FallbackSortingOrder`: keeps fallback banter visible without forcing it above all act UI.
- `CycleSeconds`: default timer interval.
- `BanterPanelStyle`: per-act runtime sizing for the panel, portrait, text column, and Next button.

### Important Methods

- `RegisterSceneHook()`: registers the `SceneManager.sceneLoaded` callback.
- `ScheduleForScene(...)`: starts a temporary bootstrapper only for Act 1, Act 2, or Act 3 scenes.
- `CreateForSceneAfterActLayout(...)`: creates the panel after the act presenter has had time to render its runtime UI.
- `GetActIdForScene(...)`: maps scene names to `GhostNarrativeState` act ids.
- `ResolvePlacement(...)`: prefers existing act UI hosts over fallback overlay placement.
- `CreatePanel(...)`: builds the non-blocking UGUI panel and wires `AmbientBanterPanel`.
- `BanterPanelStyle.Act2Validation()`: keeps the Act 2 validation-area banter strip slimmer while preserving readable text and button spacing.

### Input

Unity scene-load events and act scene names.

### Output

A runtime ambient banter panel embedded into each act scene's existing UI layout where possible.

### Failure Cases

If no beats exist for an act, no panel is spawned. Missing or unknown scene names do nothing. Duplicate panels are ignored. If expected layout hosts are missing, the hook falls back to a low-sorting runtime canvas. The panel background/text do not block raycasts; only the `Next` button is interactive.

### Unity Test

Enter Act 1, Act 2, and Act 3 from the shell. Confirm the panel appears embedded in existing spare UI space rather than floating over puzzle content, cycles/loops, can advance with `Next`, uses the player name token, and does not prevent puzzle interaction.

---

## Act 3 Dialog Graph UI Prototype

### Script Name

Act3DialogGraphInteractionController.cs

### Purpose

Presentation-layer controller for Act 3 node placement, port-to-port connection editing, node/wire removal, and deterministic validation feedback. It wraps one `DialogGraphSession` and keeps UI state out of the pure graph logic.

### Attached GameObject

None. `Act3DialogGraphStaticPresenter` creates and owns the controller at runtime.

### Runtime Role

Receives UI requests from the presenter, routes graph edits through `DialogGraphSession`, raises `StateChanged` so the presenter can refresh node cards/wires, and raises `FeedbackChanged` after validation.

### Important Fields

No serialized Unity fields.

Internal runtime state:
- one `DialogGraphSession` from `DialogGraphSession.CreateFromSampleData()`
- `SelectedNodeId`
- per-node normalized presentation positions for free movement on the graph board

### Important Methods

- `PlaceNode(...)`: calls `DialogGraphSession.AddNode(...)`, auto-sets newly placed Start nodes as the graph start, selects the new node, and raises `StateChanged`.
- `GetNodePosition(...)` / `SetNodePosition(...)`: keep draggable node-card positions in presentation state without modifying `DialogGraphSession`.
- `SelectNode(...)`: toggles/replaces selected-node state and raises `StateChanged`.
- `ClearSelection()`: clears selected-node state when the presenter switches selection to a wire.
- `SetSelectedAsStart()` / `SetStartNode(...)`: routes start-node changes through `DialogGraphSession.SetStartNode(...)`.
- `ConnectNodes(...)`: rejects self-loops, duplicate exact edges, unknown endpoints, Response-node sources, and source-node/condition mismatches before routing transition creation through `DialogGraphSession.AddTransition(...)`; when the same output dot is rewired to a new target, it removes the previous transition first.
- `RemoveNode(...)`: routes node removal through `DialogGraphSession.RemoveNode(...)`; the session cascades referenced transitions.
- `RemoveTransition(...)`: routes transition removal through `DialogGraphSession.RemoveTransition(...)`.
- `ValidateCurrentState()`: calls `DialogGraphSession.ValidateCurrentState()`, builds player-facing feedback from `DialogGraphResult.IsCorrect` / `Errors.Count`, and raises `FeedbackChanged` with the validator errors for presentation-only Ghost reaction text.

### Input

Node placement, node movement, selection, port-to-port connection, removal, and validation requests from the Act 3 presenter.

### Output

Snapshots of current nodes, transitions, start node id, selected node id, node positions, level test cases for rendering, and validation feedback messages.

### Failure Cases

- Unknown node ids passed directly to session methods can throw from `DialogGraphSession`; `ConnectNodes(...)` guards UI-originated connection attempts before they reach the session.
- Validation correctness is not reimplemented here. The controller only reads `DialogGraphResult` from the session/validator.

### Unity Test

Use the Act 3 prototype scene in Play Mode. Place nodes, drag node cards freely, drag wires from output ports to input ports, remove nodes/wires, validate correct/incorrect graphs, and confirm the UI refreshes after every action.

---

### Script Name

Act3DialogGraphStaticPresenter.cs

### Purpose

Renders the Act 3 node-graph prototype UI and wires node placement, drag-a-wire connection, removal, and deterministic validation feedback through `Act3DialogGraphInteractionController`.

### Attached GameObject

Attached to the root UI object created by `Act3DialogGraphPrototypeSceneBuilder`.

### Runtime Role

On `Start`, when `renderOnStart` is true, it creates an `Act3DialogGraphInteractionController`, renders categorized placement palette entries with player-facing names, a clear objective, draggable graph node cards with coloured edge ports, straight-line wires, a readable side guide/legend, a compact bottom validation strip with a trash drop zone, and Ghost reaction text. The Editor scene builder also calls `RenderSampleData()` before saving the generated scene.

### Important Fields

- `nodePaletteRoot`: parent `RectTransform` for fully configured placement rows.
- `graphCanvasRoot`: graph editing region containing the objective, node board, node cards, straight-line wires, and transition rows.
- `goalTestRoot`: parent `RectTransform` for the compact guide, port legend, target checks, and Ghost reaction panel.
- `validationControlsRoot`: parent `RectTransform` for enabled Validate controls.
- `paletteItemTemplate`: inactive template for palette/vocabulary rows.
- `testCaseTemplate`: inactive template for test-case rows.
- `renderOnStart`: when true, rebuilds the display at Play Mode start.

Internal runtime state:
- one `Act3DialogGraphInteractionController`
- rendered input/output port lookups used for line drawing
- an active temporary wire while dragging from an output port
- validation feedback text subscribed to `FeedbackChanged`
- selected wire endpoint/condition state for Delete/Backspace removal
- bottom-bar trash drop-zone state and highlight image while dragging cards over the trash target
- player-facing label helpers that translate internal ids such as `find_object` into readable UI text

### Important Methods

- `Configure(...)`: wires generated UI roots/templates from the builder.
- `RenderSampleData()`: clears prior display children, creates the controller, renders palette, graph editor, goal/test cases, and enabled validation controls.
- `RenderNodePalette()`: renders categorized clickable and draggable placement rows: Flow (`Start here`, `Recognize request`), Check (`Check room`), and Reply (`Answer location`, `Ask which room`).
- `TryPlacePaletteNodeAtPointer(...)`: places a dragged palette card onto the graph board at the drop position.
- `ConfigureGeneratedColumnLayout()`: reapplies the intended palette/graph/guide column widths at render time and disables the parent body's forced width expansion, so old generated scenes do not stretch fixed-width columns unpredictably.
- `RefreshGraphCanvas()`: redraws the objective panel, graph board, placed nodes, coloured edge-dot ports, and straight-line wires after controller state changes.
- `MoveNodeToPointer(...)`: lets `Act3DialogGraphNodeDragView` move a placed card freely on the graph board, including slightly outside the board so cards can be dragged onto the bottom trash zone, and redraws straight-line wires against the new port positions.
- `CompleteNodeDrag(...)`: removes a node through the controller when the player drops the card on, overlaps it with, or has already highlighted the bottom-bar trash zone.
- `RemoveSelectedGraphItem()`: removes a selected wire first, otherwise removes the selected node, when the player presses Delete/Backspace.
- `BeginWireDrag(...)` / `UpdateWireDrag(...)` / `EndWireDrag(...)`: manage the temporary straight-line wire during output-port drag.
- `CompleteWireDrop(...)`: asks the controller to connect the dragged output port to the dropped input port.
- `RenderSidePanel()`: formats the how-to-play copy, port legend, compact target checks, and Ghost reaction text.
- `RenderValidationControls()`: creates a short enabled Validate bar with feedback text plus the right-side trash drop zone.
- `ApplyValidationFeedback(...)`: colours validation feedback green/red and updates Ghost reaction text from validator errors plus the current player wiring, so different wrong routes produce different Ghost outcomes.
- `EnsureEventSystem()`: creates an `EventSystem` with `InputSystemUIInputModule` if missing.

### Input

Sample vocabulary/test data from `Act3DialogGraphSampleData`, with all graph edits routed through `DialogGraphSession` via the interaction controller.

### Output

UGUI objects showing categorized narrow palette rows, objective text, draggable placed node cards with coloured edge-dot ports, straight-line graph wires, a bottom-bar trash drop zone, readable guide/legend/target checks, and validation/Ghost reaction feedback.

### Failure Cases

- Missing roots/templates cause `RenderSampleData()` to return without rendering.
- Invalid drops, duplicate exact wires, self-loops, Response-node output attempts, and source-node/condition mismatches are ignored by the controller.
- Wire removal requires selecting a wire and pressing Delete/Backspace; node removal can also use selected-node Delete/Backspace.
- Rewiring the same source dot replaces its previous edge.
- Trash deletion checks both pointer-over-trash and card-overlaps-trash, and it also uses the cached highlight state at drop time so a highlighted trash zone always accepts the card.
- If the guide or palette column width appears to drift, verify the generated scene has the body `HorizontalLayoutGroup.childForceExpandWidth` disabled; the presenter also reapplies this at render time for stale scenes.
- Start nodes intentionally have no input port because they are only conversation entry points.
- Straight-line wires depend on refreshed port positions; rerun the scene builder if an older generated scene looks stale.

### Unity Test

Run `Ghost > Build Act 3 Dialog Graph Prototype Scene` if the saved scene looks stale, open `Assets/Scenes/Act3DialogGraphPrototype.unity`, and enter Play Mode. Confirm the palette is categorized and draggable, raw ids are hidden from player-facing text, cards can be dropped onto the board, node cards can be dragged freely and slightly outside the board toward the trash zone, Start nodes auto-mark the start and have no input port, edge dots sit on card borders, dragging coloured output dots to top input dots creates straight wires, rewiring replaces old wires from the same dot, selected wires and selected nodes delete with Delete/Backspace, dragging a card over the bottom-bar trash zone highlights it and removes the node on drop, `Test Ghost's map` shows correct/incorrect feedback plus route-specific Ghost reaction text, and no Console errors appear.

---

### Script Name

Act3DialogGraphNodeDragView.cs

### Purpose

Presentation-only drag handlers for moving an already placed Act 3 node card around the graph board and for dragging palette cards onto the board.

### Attached GameObject

`Act3DialogGraphNodeDragView` is attached at runtime to each rendered node card. `Act3DialogGraphPaletteItemDragView` is attached at runtime to each palette item.

### Runtime Role

`Act3DialogGraphNodeDragView` asks the presenter to convert the pointer position into a normalized board position, store that position in the interaction controller, move the card, redraw straight-line wires, and remove the node if the card is dropped on or overlaps the trash zone. `Act3DialogGraphPaletteItemDragView` lets a palette card be dragged onto the board and asks the presenter to place a configured node at that drop point.

### Important Fields

No serialized Unity fields.

Runtime state:
- presenter reference
- node id
- cached `RectTransform`
- palette node configuration (`DialogNodeType`, intent id, required entity type, response id) for palette drags
- temporary `CanvasGroup` alpha/raycast state while dragging palette items

### Important Methods

- `Act3DialogGraphNodeDragView.Initialize(...)`: stores the presenter, node id, and cached rect.
- `Act3DialogGraphNodeDragView.OnBeginDrag(...)`: immediately moves the card toward the pointer without triggering a full graph refresh.
- `Act3DialogGraphNodeDragView.OnDrag(...)`: keeps moving the card while the pointer moves.
- `Act3DialogGraphNodeDragView.OnEndDrag(...)`: completes the drag and lets the presenter remove the node when dropped on the trash zone.
- `Act3DialogGraphPaletteItemDragView.Initialize(...)`: stores the presenter and configured node data for a palette item.
- `Act3DialogGraphPaletteItemDragView.OnBeginDrag(...)` / `OnEndDrag(...)`: dims the palette card while dragging and places a configured node when released over the board.

### Input

Pointer drag events from Unity's EventSystem.

### Output

Presenter movement/placement calls. This script does not mutate puzzle graph structure or validation state directly.

### Failure Cases

- If no presenter is assigned, drag events do nothing.
- Node positions are presentation-only and are not persisted outside the current scene/session.
- Palette drops outside the board are ignored.

### Unity Test

In the Act 3 prototype scene, drag palette cards onto the board, drag placed node cards around the board, drag a card to the trash zone, and confirm wires stay attached to their ports until the node is removed.

---

### Script Name

Act3DialogGraphOutputPortView.cs

### Purpose

Presentation-only drag source for an Act 3 node output port.

### Attached GameObject

Attached at runtime to each rendered output port on a node card.

### Runtime Role

Implements Unity pointer drag callbacks. It asks the presenter to begin a temporary wire, update that wire while dragging, and end the drag when released.

### Important Fields

No serialized Unity fields.

Runtime state:
- presenter reference
- source node id
- implied `DialogTransitionCondition`
- cached `RectTransform`

### Important Methods

- `Initialize(...)`: stores the presenter, node id, condition, and cached rect.
- `OnBeginDrag(...)`: lets pointer raycasts pass through the output port and starts the presenter's temporary wire.
- `OnDrag(...)`: updates the presenter's temporary wire to the cursor.
- `OnEndDrag(...)`: restores raycasts and asks the presenter to clear the temporary wire when no valid drop consumed it.

### Input

Pointer drag events from Unity's EventSystem.

### Output

Presenter drag lifecycle calls. This script does not create transitions directly.

### Failure Cases

- If no presenter is assigned, drag events do nothing.
- Drops are validated by the presenter/controller/input port path, not by this component.

### Unity Test

In the Act 3 prototype scene, drag from each output port and confirm a temporary straight wire follows the cursor.

---

### Script Name

Act3DialogGraphInputPortView.cs

### Purpose

Presentation-only drop target for an Act 3 node input port.

### Attached GameObject

Attached at runtime to each rendered input port on a node card.

### Runtime Role

Implements Unity drop callbacks. When a dragged output port is dropped on this input port, it asks the presenter to complete the wire drop.

### Important Fields

No serialized Unity fields.

Runtime state:
- presenter reference
- target node id
- cached `RectTransform`

### Important Methods

- `Initialize(...)`: stores the presenter, node id, and cached rect.
- `OnDrop(...)`: extracts the dragged `Act3DialogGraphOutputPortView` and calls `Act3DialogGraphStaticPresenter.CompleteWireDrop(...)`.

### Input

Pointer drop events from Unity's EventSystem.

### Output

Presenter drop-completion calls. This script does not create transitions directly.

### Failure Cases

- Non-output-port drops are ignored.
- Self-loops, duplicates, invalid endpoints, and Response-source attempts are rejected by the controller after the drop reaches the presenter.

### Unity Test

In the Act 3 prototype scene, drop an output port onto another node's input port and confirm a committed straight wire appears when the controller accepts the transition.

---

### Script Name

Act3DialogGraphPrototypeSceneBuilder.cs

### Purpose

Editor-only helper that creates the Act 3 node-graph prototype scene through Unity-supported scene serialization. It avoids hand-writing `.unity` YAML.

### Attached GameObject

None. This script lives under an `Editor` folder and runs from a Unity Editor menu item.

### Runtime Role

No runtime role. It is excluded from player builds by the Act 3 editor asmdef and the `Editor` folder.

### Important Fields

No Inspector fields.

### Important Methods

- `BuildAct3DialogGraphPrototypeScene()`: creates a new scene, builds a UGUI canvas with a half-width palette column, a readable fixed-width guide column, a flexible large graph board, and a half-height bottom validation/trash strip; disables forced width expansion on the body layout so only the graph column receives spare width; adds an EventSystem, wires `Act3DialogGraphStaticPresenter`, renders sample data/interactions, and saves `Assets/Scenes/Act3DialogGraphPrototype.unity`.
- `CreateListRoot(...)`: creates compact vertical list regions for palette/vocabulary and goal/test content.

### Input

Manual Unity Editor menu action:
`Ghost > Build Act 3 Dialog Graph Prototype Scene`

### Output

`Assets/Scenes/Act3DialogGraphPrototype.unity`, when the user runs the menu builder in Unity.

### Failure Cases

- If Unity has compile errors, the menu item may not be available until they are fixed.
- Codex does not generate the `.unity` scene automatically in this task.
- If palette, guide, graph-board, bottom validation, or trash content appears clipped or uses older proportions, rerun the builder so the fixed-width columns, compact validation strip, and row templates are regenerated.
- The builder intentionally does not add the generated scene to Build Settings.

### Unity Test

Run the menu builder in Unity, open the generated Act 3 scene, enter Play Mode, and confirm there are no Console errors. Confirm the M0-T30 objective, half-width palette, large middle graph board, readable right guide, compact bottom `Test Ghost's map` strip with right-side trash, node placement, edge-port drag/drop wires, selected-node/selected-wire removal, trash highlight, and enabled deterministic validation feedback render correctly.

---

## Act 3 Dialog Graph Session State

### Script Name

DialogGraphSession.cs

### Purpose

Tracks the player's in-progress Act 3 dialog graph before UI exists. It owns mutable graph-building state and delegates correctness to `DialogGraphValidator`.

### Attached GameObject

None. This is pure C# session state and should not be attached to a GameObject.

### Runtime Role

Future Act 3 UI/controller code can create a session, add configured nodes, connect or disconnect transitions, set the start node, and validate the current graph without constructing a `DialogGraph` until the state is complete enough.

### Important Fields

No serialized Unity fields.

Internal state:
- current node list
- current transition list
- current start node id
- copied level `DialogGraphTestCase` array
- generated node-id counter

### Important Methods

- `CreateFromSampleData()`: creates an empty building session using `Act3DialogGraphSampleData.CreateTestCases()`.
- `AddNode(...)`: creates a `DialogNode` with a generated unique id and returns that id.
- `RemoveNode(string nodeId)`: removes a node, removes all transitions that reference it, and clears the start node if needed.
- `SetStartNode(string nodeId)`: sets the start node; unknown node ids throw `ArgumentException`.
- `AddTransition(...)`: adds a transition between existing nodes; unknown node ids throw `ArgumentException`.
- `RemoveTransition(...)`: removes the first exact matching transition and returns whether one was removed.
- `ValidateCurrentState()`: returns incorrect `DialogGraphResult` errors for incomplete graphs; otherwise builds a `DialogGraph` snapshot and calls `DialogGraphValidator.Validate(...)`.

### Input

Dialog graph test cases at construction, then method calls from future UI/session tests.

### Output

Snapshots of current nodes/transitions/start id/test cases and a `DialogGraphResult` from current-state validation.

### Failure Cases

- Null test-case collections throw `ArgumentNullException`.
- Null test cases throw `ArgumentException`.
- `DialogNode` constructor validation still rejects missing required per-type config.
- Unknown start node ids and transition endpoint ids throw `ArgumentException`.
- Empty/incomplete graph state validates incorrect without throwing.

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/Act3DialogGraphSessionTests.cs`. This script has no Play Mode behaviour.

---

### Script Name

Act3DialogGraphSessionTests.cs

### Purpose

Tests the pure Act 3 graph session/state layer.

### Attached GameObject

None. This is an EditMode test script and should not be attached to a GameObject.

### Runtime Role

Runs in Unity's EditMode Test Runner only.

### Important Fields

No serialized Unity fields.

### Important Methods

NUnit tests cover:
- empty sessions validate incorrect without throwing
- building the sample-correct graph through the session API validates correct
- a missing `SlotMissing` transition validates incorrect
- removing a node removes every referencing transition and makes the state incorrect
- adding/removing a transition is reflected in `CurrentTransitions`

### Input

Session state built from `Act3DialogGraphSampleData` constants and test cases.

### Output

NUnit pass/fail results.

### Failure Cases

- Failed assertions indicate the session no longer safely owns incomplete state, cascades node removal incorrectly, or stops delegating validation to the graph validator.

### Unity Test

Run the EditMode tests in Unity Test Runner. This script has no Play Mode behaviour.

---

## Act 3 Dialog Graph Core

### Script Name

DialogNodeType.cs

### Purpose

Defines the minimal Act 3 dialog node categories: `Start`, `IntentBranch`, `SlotCheck`, and `Response`.

### Attached GameObject

None. This is pure C# data and should not be attached to a GameObject.

### Runtime Role

Used by dialog graph nodes, the simulator, validator, and sample data to keep the Act 3 node palette small and deterministic.

### Important Fields

No serialized Unity fields.

### Important Methods

No methods. This file contains the `DialogNodeType` enum only.

### Input

None.

### Output

Named node-type values for Act 3 graph logic.

### Failure Cases

None directly. Invalid per-type node configuration is checked by `DialogNode`.

### Unity Test

Run the M0-T21 EditMode tests. This script has no Play Mode behaviour.

---

### Script Name

DialogNode.cs

### Purpose

Represents one immutable Act 3 dialog graph node with a non-empty id, a node type, and the required per-type configuration.

### Attached GameObject

None. This is pure C# puzzle data and should not be attached to a GameObject.

### Runtime Role

`DialogGraph` stores these nodes; `DialogGraphSimulator` reads their type/config to route a conversation turn.

### Important Fields

No serialized Unity fields.

Constructor-set properties:
- `Id`
- `Type`
- `IntentId` for `IntentBranch`
- `RequiredEntityType` for `SlotCheck`
- `ResponseId` for `Response`

### Important Methods

- `DialogNode(...)`: validates the id and required config for the selected node type.

### Input

String ids and node type at construction.

### Output

An immutable node object.

### Failure Cases

- Empty node id throws an `ArgumentException`.
- `IntentBranch` without an intent id throws an `ArgumentException`.
- `SlotCheck` without a required entity type throws an `ArgumentException`.
- `Response` without a response id throws an `ArgumentException`.

### Unity Test

Run the M0-T21 EditMode tests. This script has no Play Mode behaviour.

---

### Script Name

DialogTransition.cs

### Purpose

Represents one directed edge between dialog graph nodes, with the minimal Act 3 conditions: `Always`, `SlotPresent`, and `SlotMissing`.

### Attached GameObject

None. This is pure C# puzzle data and should not be attached to a GameObject.

### Runtime Role

The simulator follows transitions to move from `Start` to an intent branch, from a branch onward, and from a slot check to the correct response path.

### Important Fields

No serialized Unity fields.

Constructor-set properties:
- `FromNodeId`
- `ToNodeId`
- `Condition`

### Important Methods

- `DialogTransition(...)`: validates non-empty source and target node ids.

### Input

Source node id, target node id, and condition.

### Output

An immutable transition object.

### Failure Cases

Empty source or target node ids throw an `ArgumentException`.

### Unity Test

Run the M0-T21 EditMode tests. This script has no Play Mode behaviour.

---

### Script Name

DialogGraph.cs

### Purpose

Stores an Act 3 dialog graph's nodes, transitions, start node id, and lookup helpers.

### Attached GameObject

None. This is pure C# puzzle logic/data and should not be attached to a GameObject.

### Runtime Role

`DialogGraphSimulator` and `DialogGraphValidator` use this as the authored graph under test.

### Important Fields

No serialized Unity fields.

Internal state:
- copied node array
- copied transition array
- node lookup by id
- outgoing-transition lookup by source node id

### Important Methods

- `DialogGraph(...)`: copies nodes/transitions, rejects duplicate/null nodes, and rejects an unknown start node.
- `GetNode(string nodeId)`: returns a node by id or null.
- `ContainsNode(string nodeId)`: checks node existence.
- `GetOutgoingTransitions(string nodeId)`: returns outgoing transitions for a node.

### Input

Start node id, node collection, and transition collection.

### Output

A graph object with deterministic lookups.

### Failure Cases

- Empty start node id throws an `ArgumentException`.
- Null node/transition collections throw `ArgumentNullException`.
- Null nodes/transitions, duplicate node ids, no nodes, or unknown start node throw `ArgumentException`.

### Unity Test

Run the M0-T21 EditMode tests. This script has no Play Mode behaviour.

---

### Script Name

ConversationTurn.cs

### Purpose

Represents one already-interpreted user turn for Act 3: a detected intent id plus detected entity values keyed by entity-type id.

### Attached GameObject

None. This is pure C# data and should not be attached to a GameObject.

### Runtime Role

The simulator consumes a `ConversationTurn` instead of re-running Act 1/Act 2 logic. Act 3 only manages the dialog flow.

### Important Fields

No serialized Unity fields.

Internal state:
- `IntentId`
- copied `Entities` dictionary

### Important Methods

- `ConversationTurn(...)`: copies entity values and rejects an empty intent id.
- `TryGetEntityValue(...)`: checks whether the current turn contains a value for an entity type.

### Input

Intent id and optional entity-value dictionary.

### Output

An immutable turn object.

### Failure Cases

- Empty intent id throws an `ArgumentException`.
- Empty entity-type keys throw an `ArgumentException`.

### Unity Test

Run the M0-T21 EditMode tests. This script has no Play Mode behaviour.

---

### Script Name

DialogContext.cs

### Purpose

Tracks mutable filled slots across Act 3 conversation turns.

### Attached GameObject

None. This is pure C# state and should not be attached to a GameObject.

### Runtime Role

The simulator updates context when a turn provides a required entity and can use existing context when a later turn omits that entity.

### Important Fields

No serialized Unity fields.

Internal state:
- filled slot dictionary (`entity type id -> value`)

### Important Methods

- `TryGetSlot(...)`: checks for a stored slot value.
- `ContainsSlot(...)`: checks whether a slot has been filled.
- `SetSlot(...)`: stores or updates a slot value.
- `FilledSlots`: returns a snapshot of current slots.

### Input

Optional initial slot dictionary and simulator slot updates.

### Output

Mutable dialog context for simulator results.

### Failure Cases

Empty slot entity-type ids throw an `ArgumentException`.

### Unity Test

Run the M0-T21 EditMode tests. This script has no Play Mode behaviour.

---

### Script Name

DialogGraphSimulator.cs

### Purpose

Deterministically runs one conversation turn through an Act 3 dialog graph and returns the reached response id plus updated context.

### Attached GameObject

None. This is pure C# puzzle logic and should not be attached to a GameObject.

### Runtime Role

The validator uses the simulator for authored test cases. Later UI/session code can use it to preview graph behaviour, but M0-T21 remains scene-free.

### Important Fields

No serialized Unity fields.

Result properties:
- `DialogSimulationResult.ResponseId`
- `DialogSimulationResult.UpdatedContext`
- `DialogSimulationResult.StepLimitReached`

### Important Methods

- `Simulate(DialogGraph graph, ConversationTurn turn, DialogContext context)`: walks from the start node, routes by intent branch, checks slot presence/missing, stores provided slots in context, stops at response nodes, and enforces a step cap.

### Input

A `DialogGraph`, one `ConversationTurn`, and a mutable `DialogContext`.

### Output

`DialogSimulationResult` with the reached response id or null if no response is reached.

### Failure Cases

- Null graph or turn throws `ArgumentNullException`.
- Missing transitions or unknown targets produce a null response result.
- Cycles stop via `StepLimitReached`.

### Unity Test

Run `Act3DialogGraphSimulatorTests.cs`. This script has no Play Mode behaviour.

---

### Script Name

DialogGraphValidator.cs

### Purpose

Validates an Act 3 dialog graph through deterministic simulation test cases plus structural checks.

### Attached GameObject

None. This is pure C# puzzle logic and should not be attached to a GameObject.

### Runtime Role

Future Act 3 session/UI code can call `DialogGraphValidator.Validate(...)` after the player assembles a graph. Correctness is based only on graph simulation and structural rules, never on an LLM.

### Important Fields

No serialized Unity fields.

Result/test-case types:
- `DialogGraphResult.IsCorrect`
- `DialogGraphResult.Errors`
- `DialogGraphTestCase.Turn`
- `DialogGraphTestCase.ExpectedResponseId`

### Important Methods

- `Validate(DialogGraph graph, IEnumerable<DialogGraphTestCase> testCases)`: checks start, transition endpoints, reachability, non-response dead ends, handled intents, and expected simulator responses.

### Input

An authored graph and a list of conversation-turn test cases with expected response ids.

### Output

`DialogGraphResult` with a boolean correctness flag and human-readable errors.

### Failure Cases

Returns incorrect with errors for null test cases, no test cases, unknown transition endpoints, unreachable nodes, dead ends, unhandled intents, wrong responses, and step-cap termination.

### Unity Test

Run `Act3DialogGraphValidatorTests.cs`. This script has no Play Mode behaviour.

---

### Script Name

Act3DialogGraphSampleData.cs

### Purpose

Provides one minimal Act 3 sample level: vocabulary constants, the correct target graph, and test conversations for slot-present and slot-missing routing.

### Attached GameObject

None. This is pure C# sample data and should not be attached to a GameObject.

### Runtime Role

Tests and future Act 3 UI/session code can create the target graph and test cases from this static factory class.

### Important Fields

No serialized Unity fields.

Constants include:
- `FindObjectIntentId`
- `RoomEntityTypeId`
- `AnswerObjectLocationResponseId`
- `AskForRoomResponseId`
- node id constants for the sample graph

### Important Methods

- `CreateCorrectGraph()`: creates the target Start -> IntentBranch -> SlotCheck -> Response/Ask graph.
- `CreateTestCases()`: returns a room-present case and a room-missing case.
- `CreateFindObjectTurnWithRoom(...)`: creates a slot-present turn.
- `CreateFindObjectTurnWithoutRoom()`: creates a slot-missing turn.

### Input

None, except the optional room value passed to `CreateFindObjectTurnWithRoom(...)`.

### Output

Fresh graph/test-case/turn objects for Act 3 core tests and future UI.

### Failure Cases

Factory methods should not fail unless constructor validation changes.

### Unity Test

Run the M0-T21 EditMode tests. This script has no Play Mode behaviour.

---

### Script Name

Act3DialogGraphSimulatorTests.cs

### Purpose

Tests the deterministic Act 3 simulator routing and cycle safety.

### Attached GameObject

None. This is an EditMode test script and should not be attached to a GameObject.

### Runtime Role

Runs in Unity's EditMode Test Runner only.

### Important Fields

No serialized Unity fields.

### Important Methods

NUnit tests cover:
- slot-present routing to the answer response and context slot storage
- slot-missing routing to the ask-for-room response
- context-filled slot routing
- cycle termination through the simulator step cap

### Input

Sample graph data from `Act3DialogGraphSampleData` and a small cyclic test graph.

### Output

NUnit pass/fail results.

### Failure Cases

Failed assertions indicate the simulator is no longer routing by intent/slot state or guarding cycles correctly.

### Unity Test

Run the EditMode tests in Unity Test Runner. This script has no Play Mode behaviour.

---

### Script Name

Act3DialogGraphValidatorTests.cs

### Purpose

Tests the Act 3 graph validator against the correct sample graph and several deliberately broken graphs.

### Attached GameObject

None. This is an EditMode test script and should not be attached to a GameObject.

### Runtime Role

Runs in Unity's EditMode Test Runner only.

### Important Fields

No serialized Unity fields.

### Important Methods

NUnit tests cover:
- sample graph validates successfully
- wrong-intent-wired graph validates incorrectly
- missing slot-check graph validates incorrectly
- wrong response id validates incorrectly
- unreachable/dead-end graph validates incorrectly

### Input

Sample test cases from `Act3DialogGraphSampleData` plus local broken graph factories.

### Output

NUnit pass/fail results.

### Failure Cases

Failed assertions indicate the validator is no longer catching simulation mismatches or structural graph problems.

### Unity Test

Run the EditMode tests in Unity Test Runner. This script has no Play Mode behaviour.

---

## Act 2 Static Span-Annotation UI Prototype

### Script Name

Act2EntityChipView.cs

### Purpose

Stores the display metadata for one rendered Act 2 word chip. Each chip records the trimmed word's character `Start`, `Length`, and displayed `Text` from the source message.

### Attached GameObject

Attached by `Act2EntityExtractionStaticPresenter` to each rendered word chip.

### Runtime Role

During Play Mode, the presenter creates one component per chip while rendering the display-only sample message. The component does not listen for input or validate anything.

### Important Fields

- `Start`: zero-based character index into the message text.
- `Length`: number of characters covered by the chip.
- `Text`: displayed chip text.

### Important Methods

- `Configure(int start, int length, string text)`: stores the chip metadata after the presenter tokenizes the message.

### Input

Character-offset data from the presenter.

### Output

Inspectable chip metadata for future span-selection work.

### Failure Cases

- This component does not validate offsets. The presenter is responsible for assigning offsets that match the message text.

### Unity Test

After running `Ghost > Build Act 2 Entity Extraction Prototype Scene`, open the generated scene and inspect rendered chip GameObjects. Each should have `Act2EntityChipView` with the chip's `Start`, `Length`, and `Text`. This script has no Play Mode interaction.

---

### Script Name

Act2EntityExtractionInteractionController.cs

### Purpose

Owns the Act 2 prototype interaction state for chip selection, entity-type assignment, and deterministic validation feedback. It coordinates one `EntityExtractionSession`, the currently selected chip key, and the assigned type for each tagged chip. It delegates correctness to the session/validator and does not create UI objects.

### Attached GameObject

None. This is a plain C# presentation controller created by `Act2EntityExtractionStaticPresenter` at runtime.

### Runtime Role

When the presenter renders the sample Act 2 message, it creates one controller for that UI session. Chip clicks, palette clicks, and Validate clicks are forwarded into this controller. The controller raises `StateChanged` when the presenter should refresh chip visuals and `FeedbackChanged` when validation feedback should update.

### Important Fields

No serialized Unity fields.

Internal state:
- one `EntityExtractionSession` created from `Act2EntityExtractionSampleData.CreateMessages()[0]`
- selected chip key in `Start:Length` format
- assigned entity type by chip key

### Important Methods

- `SelectChip(string chipKey)`: selects or deselects an untagged chip. Selecting a different untagged chip clears the previous selection.
- `AssignSelectedChipToType(EntityType type)`: parses the selected chip key, creates the matching span through `EntityExtractionSession.AddSpan(...)`, records the assigned type, clears selection, and raises `StateChanged`.
- `UntagChip(string chipKey)`: removes the assigned span through `EntityExtractionSession.RemoveSpan(...)`, clears the chip assignment, clears selection if needed, and raises `StateChanged`.
- `ValidateCurrentState()`: calls `EntityExtractionSession.ValidateCurrentState()`, builds a short player-facing feedback message from `IsCorrect` and `Errors.Count`, raises `FeedbackChanged`, and returns the raw `EntityExtractionResult`.
- `GetAssignedType(string chipKey)`: exposes a chip's assigned type for rendering.
- `IsSelected(string chipKey)`: exposes selected-chip state for rendering.
- `CreateChipKey(int start, int length)`: creates the stable `Start:Length` key used by the presenter.

### Input

Plain C# method calls from the presenter in response to rendered chip clicks, entity-palette clicks, and the Validate button.

### Output

Updated presentation/session state plus `StateChanged` and `FeedbackChanged` callbacks. The controller mutates player spans only through `EntityExtractionSession`, and validation correctness comes only from `EntityExtractionSession.ValidateCurrentState()`.

### Failure Cases

- Invalid chip keys throw an `ArgumentException`.
- Null entity types passed to assignment throw an `ArgumentNullException`.
- If no chip is selected, assigning a palette type is ignored.
- Clicking an already tagged chip should route to `UntagChip(...)` from the presenter rather than selecting it.
- If the current spans are partial, wrong, or extra, validation returns incorrect feedback through the deterministic validator.

### Unity Test

Manual Act 2 scene check. Select an untagged chip, click an entity type, confirm the chip shows a type badge and System/Custom color, click the tagged chip again to untag it, and confirm Validate reports incorrect feedback for partial/wrong tagging and correct feedback for the exact answer.

---

### Script Name

Act2EntityExtractionStaticPresenter.cs

### Purpose

Renders the Act 2 span-annotation prototype and connects UI objects to `Act2EntityExtractionInteractionController` for chip selection, entity-type assignment, untagging, and deterministic validation feedback.

### Attached GameObject

Attached to the root UI object created by `Act2EntityExtractionPrototypeSceneBuilder`.

### Runtime Role

On `Start`, when `renderOnStart` is true, it rebuilds the prototype UI from sample data, creates an interaction controller, wires chip/entity-type/Validate clicks, and refreshes chip visuals plus feedback from controller state. The Editor scene builder also calls `RenderSampleData()` before saving the generated scene.

### Important Fields

- `messageChipRoot`: parent `RectTransform` for word chips.
- `entityPaletteRoot`: parent `RectTransform` for entity-type legend items.
- `validationControlsRoot`: parent `RectTransform` for placeholder Validate/feedback UI.
- `chipTemplate`: inactive template for word chips.
- `entityTypeTemplate`: inactive template for entity-type legend rows.
- `renderOnStart`: when true, rebuilds the display at Play Mode start.

Internal runtime state:
- rendered chip images, outlines, and badge labels by `Start:Length` chip key
- one `Act2EntityExtractionInteractionController`
- one validation feedback text view

### Important Methods

- `Configure(...)`: wires generated UI roots/templates without using reflection.
- `RenderSampleData()`: clears prior rendered UI, creates a fresh interaction controller, renders chips, renders entity types, wires click handlers, and renders validation controls.
- `ConfigureChipButton(...)`: forwards untagged chip clicks to selection and tagged chip clicks to untagging.
- `ConfigureEntityTypeButton(...)`: forwards palette clicks to assignment through the selected chip.
- `UpdateVisualState()`: reads controller state to apply selected-chip highlights, tagged-chip colors, and type badges.
- `RenderValidationControls()`: creates an enabled Validate button and feedback text, then routes clicks to the controller.
- `ApplyValidationFeedback(...)`: displays the controller's feedback message and colors it green for correct or warm red for incorrect.
- `EnsureChipBadge(...)`: creates the small per-chip type label used when a chip is tagged.
- `CreateWordTokens(...)`: splits message text into whitespace-delimited word chips and trims surrounding punctuation so chip offsets match word characters.
- `EnsureEventSystem()`: creates an `EventSystem` plus `InputSystemUIInputModule` when one is missing.

### Input

Sample data from `Act2EntityExtractionSampleData`, plus pointer clicks on rendered word chips, entity-type palette items, and the Validate button.

### Output

UGUI objects showing:
- one sample message rendered as word chips
- entity types `time`, `room`, and `object` with System/Custom categories
- selected-chip visual highlighting
- tagged-chip System/Custom coloring and small type badges
- an enabled Validate button
- correct/incorrect validation feedback text from the deterministic validator path

### Failure Cases

- Missing roots or templates cause `RenderSampleData()` to return without rendering.
- If sample entity spans later become multi-word, the display will still render word chips but later interaction work may need phrase grouping.
- If an older generated scene looks stale, rerun the Act 2 scene builder so the saved scene preview is refreshed. Play Mode startup also rebuilds the rendered chips from the current presenter.
- If the controller is missing, UI click callbacks return without changing state.
- The presenter does not inspect expected spans or decide correctness; it only displays feedback raised by the controller after the session validates.

### Unity Test

Run `Ghost > Build Act 2 Entity Extraction Prototype Scene` if the saved scene looks stale, open `Assets/Scenes/Act2EntityExtractionPrototype.unity`, and enter Play Mode. Confirm chip selection, type assignment, untagging, multiple tagged chips, incorrect feedback for partial/wrong tagging, correct feedback after tagging `lab` as `room` and `9pm` as `time`, feedback update after fixing mistakes, and no Console errors.

---

### Script Name

Act2EntityExtractionPrototypeSceneBuilder.cs

### Purpose

Editor-only helper that creates the display-only Act 2 prototype scene through Unity-supported scene serialization. It avoids hand-writing `.unity` YAML.

### Attached GameObject

None. This script lives under an `Editor` folder and runs from a Unity Editor menu item.

### Runtime Role

No runtime role. It is excluded from player builds by the new Act 2 editor asmdef and the `Editor` folder.

### Important Fields

No Inspector fields.

### Important Methods

- `BuildAct2EntityExtractionPrototypeScene()`: creates a new scene, builds a placeholder UGUI canvas, adds an EventSystem, wires `Act2EntityExtractionStaticPresenter`, renders the sample data, and saves `Assets/Scenes/Act2EntityExtractionPrototype.unity`.

### Input

Manual Unity Editor menu action:
`Ghost > Build Act 2 Entity Extraction Prototype Scene`

### Output

`Assets/Scenes/Act2EntityExtractionPrototype.unity`, when the user runs the menu builder in Unity.

### Failure Cases

- If Unity has compile errors, the menu item may not be available until they are fixed.
- If the scene is not generated through the menu item, Codex does not create the `.unity` file automatically.
- M0-T16 intentionally does not add the generated scene to Build Settings.

### Unity Test

Run the menu builder in Unity, open the generated Act 2 scene, enter Play Mode, and confirm there are no Console errors. Confirm the scene remains display-only.

---

## Act 2 Entity Extraction Session State

### Script Name

EntityExtractionSession.cs

### Purpose

Tracks the player's current Act 2 entity-span annotations for one message. It owns only session state: message text, expected spans, and the distinct current submitted spans. It delegates correctness to `EntityExtractionValidator`.

### Attached GameObject

None. This is pure C# session state and should not be attached to a GameObject.

### Runtime Role

Future Act 2 UI or puzzle controller code can create a session when a message starts, add or remove player-selected spans as the player annotates text, and validate the current state.

### Important Fields

No serialized Unity fields.

Internal state:
- message text
- copied expected/correct span array
- current player span list, kept distinct by `EntitySpan` value equality

### Important Methods

- `EntityExtractionSession(string messageText, IEnumerable<EntitySpan> expectedSpans)`: initializes a session from message text and correct spans; null message text becomes an empty string.
- `CreateFromSampleMessage(...)`: initializes a session from `Act2EntityExtractionSampleData.SampleMessage`.
- `AddSpan(EntitySpan span)`: adds a player span if it fits the message; exact duplicates are ignored.
- `AddSpan(int start, int length, EntityType type)`: creates and adds a player span from boundary values and type.
- `RemoveSpan(EntitySpan span)`: removes a current span and returns whether it was present; absent or null spans return false.
- `CurrentSpans`: returns a snapshot of submitted spans.
- `ValidateCurrentState()`: calls `EntityExtractionValidator.Validate(expectedSpans, CurrentSpans)`.

### Input

- Message text and expected spans at construction.
- Player span additions/removals through method calls.

### Output

- Snapshot of current submitted spans.
- `EntityExtractionResult` from the validator when validating current state.

### Failure Cases

- Null expected span collection throws an `ArgumentNullException`.
- Null expected span elements throw an `ArgumentException`.
- Null span passed to `AddSpan(...)` throws an `ArgumentNullException`.
- Span boundaries that extend past `MessageText` throw an `ArgumentOutOfRangeException`.
- Exact duplicate span additions are no-ops.
- Removing a span that was never added returns false and leaves state unchanged.

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/Act2EntityExtractionSessionTests.cs`. This script has no Play Mode behaviour.

---

### Script Name

Act2EntityExtractionSessionTests.cs

### Purpose

Tests the pure Act 2 entity-extraction session/state layer.

### Attached GameObject

None. This is an EditMode test script and should not be attached to a GameObject.

### Runtime Role

Runs in Unity's EditMode Test Runner only.

### Important Fields

No serialized Unity fields.

### Important Methods

NUnit tests cover:
- creating from a sample message starts with no current spans and validates incorrect
- adding all correct spans validates correct
- adding then removing a span clears it and validates incorrect again
- adding a span outside the message bounds throws
- adding an exact duplicate leaves the current span count unchanged
- removing a never-added span returns false

### Input

Sample messages and entity spans from `Act2EntityExtractionSampleData`.

### Output

NUnit pass/fail results.

### Failure Cases

- Failed assertions indicate the session is no longer preserving distinct current spans, rejecting out-of-range spans, or delegating validation correctly.

### Unity Test

Run the EditMode tests in Unity Test Runner. This script has no Play Mode behaviour.

---

## Act 2 Entity Extraction EditMode Tests

### Script Name

Act2EntityExtractionSampleDataTests.cs

### Purpose

Tests the Act 2 sample data against the validator and checks that the authored data includes required learning coverage.

### Attached GameObject

None. This is an EditMode test script and should not be attached to a GameObject.

### Runtime Role

Runs in Unity's EditMode Test Runner only.

### Important Fields

No serialized Unity fields.

### Important Methods

NUnit tests cover:
- every sample message validates successfully with its correct spans
- the sample set contains both system and custom entity types
- the sample set contains the `lab` / `laboratory` synonym pair for the custom room entity type

### Input

Sample messages from `Act2EntityExtractionSampleData`.

### Output

NUnit pass/fail results.

### Failure Cases

- Failed validation means a sample span boundary, length, or type no longer matches the message text.
- Missing system/custom/synonym assertions mean the sample data no longer covers the Act 2 learning requirements.

### Unity Test

Run the EditMode tests in Unity Test Runner. This script has no Play Mode behaviour.
