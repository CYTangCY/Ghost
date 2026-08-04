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

Owns the Act 1 presentation interaction state for click and drag assignment. It coordinates the pure `IntentClassificationSession`, selected card id, assignment, unassignment, validation, and simple state/feedback notifications. The feedback strings are player-facing UI copy, not puzzle-rule logic. M0-T36 expands the correct feedback so success teaches why the grouping is right: different wordings can share one purpose, and the grouped cards are training examples for the intents.

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
- `ValidateCurrentGrouping()`: validates through `IntentClassificationSession.ValidateCurrentState()` and sends player-facing correct/incorrect feedback. The correct path builds a teaching message from the existing card data, including the number of example phrasings and intent ids represented, without changing or hardcoding the answer key.
- `BuildCorrectFeedbackMessage()`: creates the successful Ghost reaction plus Lily's planning-link line.
- `BuildTrainingExampleSummary()`: counts the actual card messages and intent ids from the current card list so success feedback explains training examples without duplicating sample-data constants.
- `GetAssignedGroupId(string cardId)`: exposes assignment state for card highlighting.
- `GetAssignedCardIds(string groupId)`: exposes group contents for rendering.
- `StateChanged`: event used by the presenter to refresh visuals.
- `FeedbackChanged`: event used by the presenter to refresh validation feedback text.

### Input

Plain C# method calls from the presenter in response to UI clicks and drop-target events.

### Output

Updated interaction/session state plus simple callbacks. It does not create UI objects directly. Feedback wording explains that the player is grouping by speaker intent, confirms that correct groups are shared purposes, explains that the varied message cards are training examples a chatbot could use to learn the intents, and suggests comparing what the speaker wants when the grouping is incorrect.

### Failure Cases

- Invalid card ids or group ids still fail through `IntentClassificationSession`.
- If no card is selected, assigning to a group is ignored.

### Unity Test

Manual Act 1 scene check. Confirm click assignment, drag assignment, drag-back-to-unassigned, `Back:` row clicks, and validation feedback still route through the controller. For a correct grouping, confirm the feedback shows Ghost's happy reaction, explains shared purpose, and names the grouped cards as training examples.

---

### Script Name

Act1IntentClassificationStaticPresenter.cs

### Purpose

Renders the Act 1 sample intent-classification UI and connects UI events to `Act1IntentClassificationInteractionController`. It remains a placeholder presentation script with click assignment, minimal drag-to-assign, validation feedback, clearer placeholder instruction/visual hierarchy polish, and no scoring, save/load, or dialogue behaviour. M0-T36 adds a visible but compact teaching layer: a highlighted Lily intent-note panel, purpose-based group titles and hints, and a state-changing validation strip for the correct teaching feedback.

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
- `EnsureInstructionText()`: updates the title, compact action instruction, teaching panel, column headings, and soft panel surfaces so the prototype explains that intent means purpose rather than exact wording, plus click/drag assignment, correction by dragging back or between groups, and Validate.
- `ConfigureRootLayout()`, `ConfigureColumnPanelLayout(...)`, and `ConfigureListRoot(...)`: tighten runtime spacing/padding so the added teaching layer does not push the card lists, validation controls, or ambient banter below the 1080p viewport.
- `EnsureTeachingPanel(...)`: creates or updates the highlighted `Lily Intent Teaching Panel` under the subtitle at runtime, so older generated scenes still receive a visibly separate teaching note without hand-editing scene YAML.
- `UpdateVisualState()`: reads controller state to update card colors, group colors, and assigned-card text lists.
- `ConfigureCardDrag(...)`: attaches `Act1IntentClassificationDraggableCard` to left-side message cards and right-side assigned-card rows.
- `ConfigureIntentGroupDropTarget(...)`: attaches `Act1IntentClassificationDropTarget` to each intent group area and its assigned-card scroll viewport so dropping anywhere in the group panel is more forgiving.
- `ConfigureUnassignedDropTarget(...)`: attaches `Act1IntentClassificationDropTarget` to the left message-card list so assigned cards can be dragged back to unassigned.
- `AssignDraggedCardToIntent(...)`: forwards the dragged card id and target intent id to the controller so drag and click assignment share the same session flow. Drops onto the card's current group are ignored to avoid accidental reordering.
- `MoveDraggedCardToUnassigned(...)`: forwards assigned-card drops on the left message-card list to the controller/session unassign path.
- `EnsureAssignmentRoot(...)`: builds or upgrades each intent group's assigned-card area into a vertical `ScrollRect`.
- `CreateAssignedCardRow(...)`: renders assigned cards as compact draggable rows/chips in the group list, not as free-positioned objects.
- `EnsureValidationControls()`: creates or reuses the Validate button and feedback text under the intent group column. The validation strip is tall enough for the M0-T36 correct-feedback teaching lines and stores its Image / Outline so feedback can restyle the panel.
- `ApplyValidationFeedback(...)`: displays feedback produced by the controller and changes the validation strip colour/outline for correct and incorrect states.
- `GetIntentDescription(...)`: maps intent ids to purpose-style group hints, such as visitors wanting Ghost to help find something, know where Ghost is, or know who Ghost is.
- `GetIntentTitle(...)`: maps internal intent ids to player-facing purpose labels while keeping the underlying ids unchanged for drag/drop and validation.
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
- title and compact action instructions
- a highlighted compact Lily intent-note panel explaining Ghost's exact-word problem, intent as purpose, and training examples
- labelled `Unassigned Messages` and `Intent Groups` columns
- nine sample message cards
- three intent group areas with purpose labels instead of raw intent ids
- short intent-purpose descriptions phrased as what the visitors want Ghost to do or answer
- selected-card highlight using a warmer fill and stronger outline
- compact assigned-card rows listed inside each intent group area
- assigned-card row/chip styling distinct from left-side unassigned cards
- scrollable assigned-card lists so assigning many cards to one group does not silently hide them
- a single opaque temporary drag preview while a message card or assigned-card row is being dragged
- validation feedback for correct or incorrect grouping inside a feedback panel; correct feedback changes the panel into a green success-teaching state and teaches shared purpose, training examples, and the planning link
- compact runtime layout spacing so the full Act 1 teaching UI should remain visible in a 1080p Play Mode window

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
- If the title, instruction copy, Lily teaching panel, group-purpose labels/hints, or validation strip still show older wording after presentation changes, enter Play Mode so `RenderSampleData()` applies the updated labels, or rerun the Unity menu builder if a refreshed saved scene preview is needed.

### Unity Test

Manual Act 1 scene check. Open the prototype scene after running the menu builder if needed, enter Play Mode, and repeat the M0-T12 behaviour checks. Also confirm the highlighted Lily teaching panel explains intent as purpose rather than exact wording, group titles/hints read as visitor purposes, correct validation feedback changes into a visible success-teaching panel that teaches shared intent plus training examples, the bottom Validate/banter area remains inside the game view, and unassigned cards, intent group panels, assigned chips, selected state, drop-ready group state, and validation feedback are visually distinct.

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

After scene load, it checks the active scene name. If a teaching chapter or Final Chapter is active and no return overlay exists, it creates an EventSystem if needed, then creates a dedicated high-sorting overlay Canvas and adds a top-right `Return to Hub` button wired with `ShellSceneNavigationButton`. Run 005 makes this button pure navigation: it never sets pending debrief state or marks a chapter complete.

### Important Fields

No Inspector fields.

### Important Methods

- `RegisterSceneHook()`: registers the scene-loaded callback.
- `CreateForScene(...)`: creates the overlay only for supported act scenes.
- `ShouldShowOverlay(...)`: returns true for Chapters 1-6 and the Final Chapter scene names.
- `CreateOverlayCanvas(...)`: builds a separate top-layer Canvas so act prototype UI canvases cannot cover the return button.
- `CreateReturnButton(...)`: builds the placeholder UGUI return button.

### Input

Unity scene-load events and return button clicks.

### Output

A small `Return to Hub` button in Chapters 1-6 and Final Chapter that loads `GameShellPrototype` without changing completion or debrief state.

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

## M0-T46 Run 001: Acts 1 and 3 Experience Unification

### Script Name

Act1IntentClassificationInteractionController.cs

### Purpose

Adds a presentation-only `Onboarding` phase before the existing Act 1 intro/build/demo/complete flow.
The controller starts in onboarding and exposes `BeginAfterOnboarding()` as the single transition into
the authored exact-word failure beats. Existing pile state, generalization demo, validator call, and
completion logic are unchanged.

### Attached GameObject

None. The presenter owns this plain C# controller at runtime.

### Important Methods

- `BeginAfterOnboarding()`: dismisses Lily's loop explanation and enters the existing intro failures.
- `ReplayOnboarding()`: reopens Lily's loop from the in-level note without changing validator data.
- `GetCurrentConversationBeat()`: includes a non-interactive onboarding fallback beat, then preserves
  the existing intro, build, demo, and complete beats.

### Unity Test

Open Act 1 in Play Mode. Confirm only Lily's onboarding panel is actionable at first, then dismiss it
and complete the existing watch/build/teach flow.

### Script Name

Act1IntentClassificationStaticPresenter.cs

### Purpose

Creates the Act 2-style warm onboarding panel and dark persistent objective strip at runtime. During
onboarding it hides the prototype body so transcript cards cannot be touched, while keeping the Ghost
problem preview visible beneath Lily's explanation. After dismissal it restores the existing UI,
changes Lily's panel into a compact note strip with `Replay Lily`, and updates the objective for Intro,
Build, Demo, and Complete.

The runtime root is normalized to Act 2's page skeleton and dimensions: a 56px header with right-side
phase progress, 48px objective, 180px onboarding or 54px Lily note, 170px Ghost conversation, and a
flexible two-column puzzle body with 18px column spacing.
No scene YAML change or Inspector reference is required.

### Important Methods

- `EnsureExperienceChrome()`: creates and orders the objective strip/onboarding panel around the
  existing generated scene hierarchy.
- `EnsurePageHeader()`: moves the generated title into the shared header pattern and creates phase
  progress while hiding the superseded standalone subtitle.
- `UpdateExperienceChrome()`: toggles onboarding versus gameplay visibility and refreshes the strip.
- `EnsureTeachingPanel()`: builds the compact in-level Lily note row used after onboarding.
- `GetObjectiveText()`: returns non-answer phase guidance for the three-step training loop.

### Unity Test

At 1920x1080, confirm the onboarding and every later objective fit without hiding cards, piles,
conversation, controls, feedback, or the floating banter panel.

### Script Name

Act3DialogGraphInteractionController.cs

### Purpose

Adds presentation state around the existing `DialogGraphSession`: onboarding, build/retry, and
complete. Each Validate still delegates to `session.ValidateCurrentState()`. The returned deterministic
result sets a presentation reaction: Happy on pass, Sad when required graph pieces are absent, and
Confused when a populated graph has wrong structure or fails a test route. A failed attempt remains
recorded while edits occur; a successful result is invalidated if the graph structure is edited.

### Important Methods

- `BeginAfterOnboarding()`: unlocks graph building after Lily's one-button onboarding.
- `ReplayOnboarding()`: reopens the same onboarding from the in-level Lily note while preserving the
  current graph and last deterministic result.
- `ValidateCurrentState()`: stores the existing validator result, selects the deterministic Ghost
  reaction, posts the existing best-effort attempt log, and requests the existing non-spoiler hint on
  failure.
- `IsEmptyOrIncompleteGraph()`: presentation classification for Sad versus Confused; it does not score
  correctness.
- `NotifyGraphChanged()`: preserves failed-attempt detail during retry edits and clears a stale success
  if the player changes the graph.

### Unity Test

Validate an empty/incomplete graph, a fully populated but wrong graph, and the correct graph. Confirm
Sad, Confused, and Happy respectively, with no LLM involvement in the result.

### Script Name

Act3DialogGraphStaticPresenter.cs

### Purpose

Adds the Act 3 onboarding panel, a visible Ghost reply-order problem preview, persistent objective
strip, replayable in-level Lily note, compact `GhostFaceView`, retry label, and Shell completion
action. The graph body is hidden until onboarding is dismissed. Failed validation
keeps the existing summary and specific Ghost outcome visible while the graph stays editable and the
button reads `Try again`. Success changes the button to `Complete Act`, sets the pending Act 3 debrief,
and loads the existing Game Shell scene.

The root follows the same Act 2 page skeleton and dimensions as Act 1. The persistent 170px Ghost
conversation panel owns the deterministic face and test outcome; the right Guide column is reserved
for graph instructions, route legend, and authored test cases. The graph-specific three-column body
remains flexible beneath the shared header/objective/note/conversation chrome.

The existing `AmbientBanterHook` already recognizes `Act3DialogGraphPrototype` and creates the floating
banter panel; its `Ask Lily` action opens the existing draggable `LilyChatWindow`. No Banter/Common/
GhostAvatar code is changed by M0-T46 Run 001.

### Important Methods

- `EnsureExperienceChrome()` / `UpdateExperienceChrome()`: create and drive onboarding, objective,
  Ghost-problem preview, replayable note, visibility, face mood, and primary-action text.
- `EnsurePageHeader()`: creates the shared 56px header and phase progress and hides the old subtitle.
- `UpdateConversationPanel()`: maps onboarding, untested, failed, and passed states into the shared
  Ghost conversation/result panel without deciding correctness.
- `RenderSidePanel()`: adds the shared Ghost face within the compact 1080p guide column.
- `ApplyValidationFeedback()`: preserves the deterministic failure/success detail and refreshes the
  presentation state.
- `HandlePrimaryAction()`: validates during build/retry and returns through the Shell pending-debrief
  path after success.

### Inspector Setup

No new serialized fields are required. Keep the existing Act 1 and Act 3 presenter references created
by their scene builders. Runtime presentation code creates the new panels and Ghost face.

### Unity Test

Follow the M0-T46 Run 001 section in `Docs/UNITY_TEST_CHECKLIST.md`, including 1920x1080 fit, floating
window drag checks, retry, completion/debrief, deterministic face reactions, and Console review.

---

## M0-T45 Run 002: Act 2 Ghost's Errand Redesign

This section supersedes the older M0-T37 Act 2 span-teaching presentation notes. The pure
`EntityExtractionSession`, `EntityExtractionValidator`, and `Act2EntityExtractionSampleData` scripts
remain unchanged; the new scripts below turn those same spans into an errand consequence loop.

### Script Name

Act2ErrandDemoData.cs

### Purpose

Defines authored static Act 2 errand data beside the existing entity-extraction sample messages. Each
errand references an existing sample message, derives its needed action-card slots from that message's
expected entity types, and supplies intro failure, success, and per-slot missing/wrong outcome lines.
It also defines the `lab` / `laboratory` synonym resolution display data.

### Attached GameObject

None. This is pure C# runtime data and should not be attached to a GameObject.

### Runtime Role

Created by the Act 2 presentation controller when a new errand loop starts.

### Important Methods

- `CreateErrands()`: returns the three authored errands from the existing Act 2 sample messages.
- `CreateSlotsForMessage(...)`: maps `object` to WHAT, `room` to WHERE, and `time` to WHEN.
- `CreateSynonymResolutions()`: returns the room synonym display mapping to `lab room`.

### Unity Test

Covered by `Act2ErrandOutcomeEngineTests.cs`; also verify manually through the Act 2 scene.

---

### Script Name

Act2ErrandOutcomeEngine.cs

### Purpose

Pure deterministic engine that evaluates Ghost's current errand action-card spans. It compares the
player's `EntitySpan` list against the current errand's expected spans using the existing
`EntityExtractionValidator`, then returns per-slot Correct / Missing / Wrong states, the authored
outcome line, and a pure Ghost mood enum for the presenter to map into `GhostFaceView`.

### Attached GameObject

None. This is pure C# logic and should not be attached to a GameObject.

### Runtime Role

Called by `Act2EntityExtractionInteractionController.RunErrand()` after the player presses
`Go, Ghost!`.

### Important Methods

- `Evaluate(...)`: validates the submitted spans, creates slot results, chooses the outcome line, and
  marks success only when every slot is correct and the existing validator passes.

### Failure Cases

Missing submitted spans return Missing slot states. Wrong boundaries, wrong token choices, duplicate
or extra spans remain incorrect through the validator semantics.

### Unity Test

Run `Act2ErrandOutcomeEngineTests` in Unity EditMode Test Runner.

---

### Script Name

Act2EntityExtractionInteractionController.cs

### Purpose

Owns the current Act 2 errand state machine: onboarding, intro failure, token fill, run outcome, and
completion. It keeps one `EntityExtractionSession` per errand, stores token-to-slot assignments, and
uses only session span additions/removals to mutate the puzzle state.

### Attached GameObject

None. This is a plain C# presentation controller created by
`Act2EntityExtractionStaticPresenter`.

### Runtime Role

Receives UI events from token clicks/drags, slot drops, Split, `Go, Ghost!`, revise, and next-errand
buttons. It raises `StateChanged` so the presenter can redraw the current phase.

### Important Methods

- `BeginAfterOnboarding()`: dismisses Lily's how-to beat and shows Ghost's first authored errand
  failure.
- `SplitMessage()`: changes the solid sentence into token chips.
- `AssignTokenToSlot(...)`: converts a token into an `EntitySpan` with the slot's `EntityType` and
  adds it through `EntityExtractionSession`.
- `RemoveTokenAssignment(...)`: removes an assigned span through the session.
- `RunErrand()`: evaluates via `Act2ErrandOutcomeEngine`, posts a best-effort attempt log, and asks
  Lily for a non-spoiler hint on failure.
- `ContinueAfterSuccess()`: advances to the next errand or the completion state.

### Unity Test

In Play Mode, confirm the onboarding, objective strip, Split, token-to-slot assignment, untagging,
errand outcomes, synonym resolution, and completion flow.

---

### Script Name

Act2EntityExtractionStaticPresenter.cs

### Purpose

Renders the current Act 2 `Ghost's Errand` UI: Lily onboarding, persistent objective strip,
conversation panel with shared Ghost face, solid sentence / token grid, and Ghost's action card with
typed WHAT / WHERE / WHEN slots. It does not decide correctness; it displays controller state and
forwards input.

### Attached GameObject

Attach to the root UI object in `Assets/Scenes/Act2EntityExtractionPrototype.unity`, normally created
through `Ghost > Build Act 2 Entity Extraction Prototype Scene`.

### Runtime Role

On `Start`, creates the controller and redraws the entire UI whenever the controller phase or slot
state changes.

### Important Methods

- `RenderSampleData()`: creates the controller and initial onboarding UI.
- `RenderState()`: rebuilds the UI for the current phase.
- `CreateObjectiveStrip()`: displays the always-visible current objective.
- `CreateMessagePanel()`: shows either the solid sentence or draggable token chips.
- `CreateActionCardPanel()` / `CreateSlotView(...)`: shows typed slots, assignment chips, slot
  state colours, and synonym resolution text.

### Unity Test

Open the Act 2 scene at 1920x1080 and confirm all visible panels fit without cropping, especially the
bottom action controls.

---

### Script Name

Act2EntityTokenDragView.cs

### Purpose

Makes an Act 2 token chip draggable and creates a temporary drag preview.

### Attached GameObject

Added at runtime by `Act2EntityExtractionStaticPresenter` to token chips and assigned slot chips.

### Runtime Role

Forwards drag metadata through Unity's event system so slot and return drop targets can identify the
dragged token's chip key.

### Unity Test

Drag a token chip into a slot and confirm a preview follows the pointer and disappears after drop.

---

### Script Name

Act2EntitySlotDropTarget.cs

### Purpose

Receives a dragged token chip over one action-card slot and forwards the token chip key plus target
slot id to the presenter/controller.

### Attached GameObject

Added at runtime to each slot view.

### Unity Test

Drop a token into WHAT, WHERE, or WHEN and confirm the slot fills and creates the corresponding span.

---

### Script Name

Act2EntityTokenReturnDropTarget.cs

### Purpose

Receives an assigned token dragged back to the message-token area and forwards it for untagging.

### Attached GameObject

Added at runtime to the token grid area.

### Unity Test

After filling a slot, drag its assigned token back into the token area and confirm the slot clears.

---

### Script Name

Act2EntityExtractionPrototypeSceneBuilder.cs

### Purpose

Editor-only scene builder for the redesigned Act 2 errand prototype. It creates the camera, canvas,
event system, and root presenter object, then lets `Act2EntityExtractionStaticPresenter` render the
current layout.

### Attached GameObject

None. This script lives in an `Editor` folder and runs from the Unity Editor menu.

### Runtime Role

No runtime role.

### Unity Test

Run `Ghost > Build Act 2 Entity Extraction Prototype Scene`, open the generated scene, and enter Play
Mode.

---

### Script Name

Act2ErrandOutcomeEngineTests.cs

### Purpose

EditMode tests for the pure Act 2 errand outcome engine.

### Important Methods

NUnit tests cover all-correct success, missing WHEN failure, wrong WHAT failure, and successful
`laboratory -> lab room` synonym resolution.

### Unity Test

Run the EditMode tests in Unity Test Runner.

---

## M0-T45 Run 001 Act 1 Teaching-as-Gameplay Redesign

### Script Name

Act1TeachingDemoData.cs

### Purpose

Provides authored Act 1 teaching demo data: intro failure beats, per-intent Ghost reply lines, and unseen
test visitor messages tied to real card ids from `Act1IntentClassificationSampleData`.

### Runtime Role

Pure `Ghost.Runtime` data source. It does not score the puzzle; it feeds the generalization demo that
shows how the player's training piles affect Ghost.

### Unity Test

Covered through `Act1GhostGeneralizationEngineTests.cs`.

---

### Script Name

Act1GhostGeneralizationEngine.cs

### Purpose

Pure deterministic demo engine for Act 1. Given card-to-pile assignments, pile labels, and an unseen
test message, it chooses the pile containing the plurality of related training cards. Ties, unassigned
related cards, or unlabelled chosen piles produce confused outcomes. A labelled chosen pile produces an
authored reply, and correctness is true only when that label matches the test message's true intent.

### Runtime Role

Used by `Act1IntentClassificationInteractionController` when the player presses `Teach Ghost`. It is a
consequence/demo engine only; final completion still checks the existing `IntentClassificationValidator`.

### Unity Test

Run `Act1GhostGeneralizationEngineTests.cs` in EditMode.

---

### Script Name

GhostMood.cs / GhostFaceView.cs

### Purpose

Defines a shared Ghost avatar with `Neutral`, `Happy`, `Confused`, and `Sad` moods. Run 006 uses
four authored 96x96 low-resolution RPG sprites first; the earlier Unity-UI face remains only as a
missing-resource fallback.

### Attached GameObject

`GhostFaceView` is attached to runtime-created `Ghost Face` UI objects inside the Act 1 and Act 2
conversation/consequence panels. Future acts can reuse it.

### Runtime Role

`SetMood(GhostMood)` asks `GhostPixelSpriteFactory` for the matching low-resolution sprite, hides
the old programmatic eye/mouth layers, and keeps point filtering. If a resource is missing, the same
method restores the former colour/eyes/mouth/mood-mark UI so Ghost remains visible without Console
resource warnings. Shell and ambient banter use the neutral Ghost sprite when no serialized portrait is assigned.

### Unity Test

In Play Mode, confirm the Act 1 conversation panel shows confused Ghost during intro failures, neutral
Ghost while building piles, happy Ghost on correct demo replies, and confused Ghost on wrong/confused
demo replies.

---

### Script Name

Act1IntentClassificationLabelDragView.cs

### Purpose

Adds drag-preview behaviour for the three purpose-label chips in the rebuilt Act 1 UI.

### Runtime Role

The presenter attaches this to each label chip. Dragging a label onto a pile routes through
`Act1IntentTeachingDropTarget`; clicking still selects the label for click-to-assign.

### Unity Test

In Act 1 Play Mode, drag `find something`, `where is Ghost`, or `who is Ghost` onto a training pile and
confirm the pile label socket updates.

---

### Script Name

Act1IntentTeachingDropTarget.cs

### Purpose

Drop target for the redesigned Act 1 training UI. It accepts transcript cards on the new-pile zone,
existing piles, or the unpiled list, and accepts purpose labels on existing piles.

### Runtime Role

The presenter attaches this at runtime to the unpiled transcript list, new-pile zone, and each pile.
It forwards drops to the interaction controller without deciding correctness.

### Unity Test

Drag a card to the new-pile zone, drag another card onto that pile, drag a piled card back to the
transcript list, and drag a purpose label onto a pile.

---

### Script Name

Act1IntentClassificationInteractionController.cs

### Purpose

Owns the rebuilt Act 1 teaching-as-gameplay state: intro phase, free training piles, selected card,
selected purpose label, demo phase, highlighted misleading cards, and completion phase.

### Runtime Role

The controller maps free piles plus labels into the existing validator's submitted groups for final
completion. During `Teach Ghost`, it calls `Act1GhostGeneralizationEngine` on authored unseen messages
and exposes the current conversation beat and highlighted card ids to the presenter.

### Important Methods

- `AdvanceConversation()`: steps through intro failures or demo messages.
- `MoveCardToNewPile(...)`, `MoveCardToPile(...)`, `MoveCardToUnpiled(...)`: update free training piles.
- `AssignLabelToPile(...)`: attaches one purpose label to a pile, moving that label from any previous pile.
- `TeachGhost()`: runs the unseen-message demo from the current piles.
- `ReturnToBuild()`: returns from demo to pile editing.

### Unity Test

Use the M0-T45 checklist: watch intro failures, build piles, label piles, teach Ghost, revise, reteach,
and complete only when the validator-correct pile structure also answers all unseen messages correctly.

---

### Script Name

Act1IntentClassificationStaticPresenter.cs

### Purpose

Renders the new Act 1 teaching-as-gameplay UI. The screen now has a Ghost conversation/demo panel,
transcript cards, purpose-label chips, free training piles, a new-pile drop zone, Teach/Revise controls,
and misleading-card highlights.

### Runtime Role

On start, it creates the controller from existing sample cards, runtime-creates/reuses the compact Lily
note and conversation panel, renders cards/piles/labels from controller state, and refreshes Ghost's face
and feedback after every interaction.

### Unity Test

Open Act 1 at 1920x1080 and confirm the conversation panel, transcript list, label chips, piles, controls,
and Ghost face fit without cropping.

---

### Script Name

Act1IntentClassificationPrototypeSceneBuilder.cs

### Purpose

Editor menu builder for the Act 1 prototype scene. M0-T45 updates its generated title, subtitle, and
column proportions for the teaching-as-gameplay layout. Scene YAML is still generated through Unity, not
hand-edited.

### Unity Test

Run `Ghost > Build Act 1 Intent Classification Prototype Scene`, open the generated scene, enter Play
Mode, and run the M0-T45 checklist.

---

## M0-T45 Run 003 Play Mode Feedback Fixes

### Script Name

FloatingWindowDragHandle.cs

### Purpose

Reusable UGUI drag handle for floating panels such as Lily chat or future hint windows. It keeps the
window inside its parent canvas so the player can move support UI away from the puzzle without losing
it off-screen.

### Runtime Role

Attach this component to a window header and call `Configure(...)` with the target window
`RectTransform`. During drag, it converts screen pointer movement into parent-local movement and clamps
the target window inside the canvas bounds.

### Unity Test

Open Lily chat in Play Mode, drag the chat header, and confirm the window moves freely while staying
inside the Game view.

---

### Script Name

LilyChatWindow.cs

### Purpose

Runtime-created Lily chat window. M0-T45 Run 003 changes it from a fixed right-side overlay into a
floating draggable window by attaching `FloatingWindowDragHandle` to the chat header.

### Runtime Role

The window still pauses ambient banter, sends chat turns to `/chat`, and falls back to static Lily
chat when backend/LLM calls fail. Its initial position is near the right side of the screen, but the
player can now drag the header to uncover puzzle content.

### Unity Test

In any act, click `Ask Lily`, confirm the chat opens near the right side, drag the header to another
part of the screen, type/close as before, and confirm ambient banter resumes.

---

### Script Name

Act1IntentClassificationStaticPresenter.cs

### Purpose

M0-T45 Run 003 adds a visible `Complete Act` control for the Act 1 completion state. This resolves the
Play Mode issue where the player could correctly train Ghost but had no obvious way to finish.

### Runtime Role

When `Act1IntentClassificationInteractionController` reaches `Complete`, the presenter shows
`Complete Act`, hides Teach/Revise, sets the pending Act 1 debrief on `GhostNarrativeState`, and loads
the Game Shell scene. The Shell's existing debrief flow marks Act 1 complete.

### Unity Test

Complete Act 1 with the correct labelled piles, confirm `Complete Act` appears, click it, and confirm
the Game Shell debrief/hub flow starts.

---

## M0-T45 Run 004 Retry / Floating Banter / Lily Pixel Portrait

### Script Name

Act2EntityExtractionInteractionController.cs / Act2EntityExtractionStaticPresenter.cs

### Purpose

M0-T45 Run 004 fixes the Act 2 Play Mode issue where a failed errand felt unretryable. A failed
`Go, Ghost!` run now keeps the authored failure outcome and slot result colours visible, but returns
the level to the editable Fill phase immediately.

### Runtime Role

After an incorrect errand run, slots and token chips remain interactive and the action button changes
to `Try again`. Correct runs still enter the success Run phase and use `Next errand` / `Complete`.
Correctness still comes from `EntityExtractionValidator` through the existing outcome engine.

### Unity Test

In Act 2, intentionally put a wrong or missing token in a slot, press `Go, Ghost!`, confirm the wrong
slot result stays visible, edit the slot immediately without pressing a separate revise button, and
press `Try again`.

---

### Script Name

AmbientBanterHook.cs / AmbientBanterPanel.cs

### Purpose

The normal ambient Lily/Ghost dialogue panel is now a floating draggable window instead of a fixed
layout child of the bottom validation area. This prevents it from permanently blocking act controls.

### Runtime Role

`AmbientBanterHook` creates the banter panel on the scene canvas with `FloatingWindowDragHandle`
attached to the panel root. The panel still cycles through `BanterData`, still opens Lily chat, and
still pauses while chat is open. `AmbientBanterPanel` now uses the generated Lily pixel portrait when
no explicit Lily sprite is assigned.

### Unity Test

Enter an act, confirm the ambient banter panel appears near the bottom, drag the panel away from puzzle
controls, click `Ask Lily`, close chat, and confirm ambient cycling resumes.

---

### Script Name

LilyPixelPortraitFactory.cs

### Purpose

Creates a small original Lily pixel portrait at runtime. Run 004 introduced the generated fallback
portrait; Run 005 updates the visual specification to the current user direction. It is a
Ghost-project original portrait and does not use external art assets.

### Runtime Role

`LilyPixelPortraitFactory.GetPortrait()` lazily builds a 32x32 point-filtered `Texture2D`, converts it
to a Sprite, and caches it. `AmbientBanterPanel` and `LilyDialogueFrame` use it when their serialized
`lilyPortrait` sprite is empty.

### Unity Test

Open the Game Shell and any act with ambient banter; confirm Lily lines show a pixel portrait instead
of the plain `Lily` placeholder label.

---

## M0-T45 Run 005 Drag Preview Cleanup / Lily Style Correction

### Script Name

Act2EntityTokenDragView.cs / Act2EntitySlotDropTarget.cs / Act2EntityTokenReturnDropTarget.cs

### Purpose

Fixes the Act 2 Play Mode issue where token drag preview boxes could remain stuck inside Ghost's
action card after dragging. This happened because dropping a token can immediately re-render the Act 2
presenter, destroying the dragged source object before Unity calls `OnEndDrag`.

### Runtime Role

`Act2EntityTokenDragView` now tracks all active drag previews globally. It clears old previews when a
new drag starts, when a drag ends, when the source token is disabled/destroyed, and when slot/return
drop targets receive a token. This keeps the preview visual separate from the actual slot assignment
state.

### Unity Test

In Act 2 Play Mode, drag several tokens across WHAT/WHERE/WHEN and drop them on slots or back on the
token area. Confirm no yellow preview boxes remain stuck after each drop or failed drag.

---

### Script Name

LilyPixelPortraitFactory.cs

### Purpose

Updates Lily's generated pixel portrait to the current character direction: gold short hair, glasses,
blue suit jacket, white shirt, black long pants, and black high heels. The portrait remains a runtime
generated original asset.

### Runtime Role

`GetPortrait()` still lazily creates and caches a point-filtered 32x32 sprite. The pixel drawing now
uses the corrected colour blocks and full-body chibi silhouette so the same fallback sprite is used by
Shell and ambient banter Lily portraits.

### Unity Test

Open the Shell and an act with ambient banter; confirm Lily appears as a gold-short-haired pixel
character with glasses, blue jacket, white shirt, black pants, and black shoes.

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

Owns the Act 2 prototype interaction state for chip selection, entity-type assignment, and deterministic validation feedback. It coordinates one `EntityExtractionSession`, the currently selected chip key, and the assigned type for each tagged chip. It delegates correctness to the session/validator and does not create UI objects; on correct validation it now builds a compact teaching beat about NER, key details, the sample-data synonym pair, tokenization, and the Act 1 intent / Act 3 slot bridge.

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
- `ValidateCurrentState()`: calls `EntityExtractionSession.ValidateCurrentState()`, keeps the existing non-spoiler incorrect feedback from `Errors.Count`, builds the correct teaching feedback from sample data, raises `FeedbackChanged`, and returns the raw `EntityExtractionResult`.
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

Manual Act 2 scene check. Select an untagged chip, click an entity type, confirm the chip shows a type badge and System/Custom color, click the tagged chip again to untag it, and confirm Validate reports incorrect feedback for partial/wrong tagging and correct teaching feedback for the exact answer. The correct feedback should mention Ghost noticing useful details, NER, the `lab` / `laboratory` synonym pair, word-token chips, intent, and slots.

---

### Script Name

Act2EntityExtractionStaticPresenter.cs

### Purpose

Renders the Act 2 span-annotation prototype and connects UI objects to `Act2EntityExtractionInteractionController` for chip selection, entity-type assignment, untagging, and deterministic validation feedback. It also adds the M0-T37 in-fiction teaching layer: a compact Lily entity note, word-token wording, and entity-kind legend subtitles for system/custom entities and synonyms.

### Attached GameObject

Attached to the root UI object created by `Act2EntityExtractionPrototypeSceneBuilder`.

### Runtime Role

On `Start`, when `renderOnStart` is true, it rebuilds the prototype UI from sample data, creates an interaction controller, wires chip/entity-type/Validate clicks, and refreshes chip visuals plus feedback from controller state. It creates the teaching panel at runtime so existing scenes do not need hand-edited YAML. The Editor scene builder also calls `RenderSampleData()` before saving the generated scene.

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
- `EnsureInstructionText()`: keeps the Act 2 title/subtitle compact, creates `Lily's Entity Note`, labels chips as word tokens, and labels the palette as entity kinds.
- `ConfigureChipButton(...)`: forwards untagged chip clicks to selection and tagged chip clicks to untagging.
- `ConfigureEntityTypeButton(...)`: forwards palette clicks to assignment through the selected chip.
- `UpdateVisualState()`: reads controller state to apply selected-chip highlights, tagged-chip colors, and type badges.
- `RenderValidationControls()`: creates an enabled Validate button and feedback text, then routes clicks to the controller.
- `ApplyValidationFeedback(...)`: displays the controller's feedback message and colors it green for correct or warm red for incorrect.
- `GetEntityTeachingSubtitle(...)`: derives the legend subtitles from the rendered sample entity types, including system/custom definitions and the room synonym pair collected from `Act2EntityExtractionSampleData`.
- `EnsureChipBadge(...)`: creates the small per-chip type label used when a chip is tagged.
- `CreateWordTokens(...)`: splits message text into whitespace-delimited word chips and trims surrounding punctuation so chip offsets match word characters.
- `EnsureEventSystem()`: creates an `EventSystem` plus `InputSystemUIInputModule` when one is missing.

### Input

Sample data from `Act2EntityExtractionSampleData`, plus pointer clicks on rendered word chips, entity-type palette items, and the Validate button.

### Output

UGUI objects showing:
- one sample message rendered as word chips
- a compact `Lily's Entity Note` panel explaining Ghost's key-detail problem, NER, and word tokens
- entity kinds `time`, `room`, and `object` with System/Custom teaching subtitles
- the sample-data room synonym pair displayed in the custom room entry
- selected-chip visual highlighting
- tagged-chip System/Custom coloring and small type badges
- an enabled Validate button
- correct/incorrect validation feedback text from the deterministic validator path, with the correct path teaching key details, synonyms, tokenization, intent, and slots

### Failure Cases

- Missing roots or templates cause `RenderSampleData()` to return without rendering.
- If sample entity spans later become multi-word, the display will still render word chips but later interaction work may need phrase grouping.
- If an older generated scene looks stale, rerun the Act 2 scene builder so the saved scene preview is refreshed. Play Mode startup also rebuilds the rendered chips from the current presenter.
- If the controller is missing, UI click callbacks return without changing state.
- The presenter does not inspect expected spans or decide correctness; it only displays feedback raised by the controller after the session validates.

### Unity Test

Run `Ghost > Build Act 2 Entity Extraction Prototype Scene` if the saved scene looks stale, open `Assets/Scenes/Act2EntityExtractionPrototype.unity`, and enter Play Mode. Confirm the Lily entity note, word-token label, entity-kind legend subtitles, chip selection, type assignment, untagging, multiple tagged chips, unchanged incorrect feedback for partial/wrong tagging, correct teaching feedback after tagging `lab` as `room` and `9pm` as `time`, feedback update after fixing mistakes, 1920x1080 layout fit with validation/banter visible, and no Console errors.

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

---

## Act 4 Confidence and Fallback

### Script Name

Act4ConfidenceModels.cs

### Purpose

Defines the pure C# data model for Act 4 confidence routing: visitor messages, player configuration, route outcomes, per-visitor run results, and validation results.

### Attached GameObject

None. This is pure `Ghost.Runtime` logic and should not be attached to a GameObject.

### Runtime Role

Used by the Act 4 validator, demo data, tests, and presentation controller to describe the deterministic day-run state.

### Important Fields

No serialized Unity fields. Key values include threshold, fallback wiring, handoff wiring, confidence scores, expected route outcomes, and visitor outcome lines.

### Important Methods

- `Act4VisitorMessage(...)`: validates visitor ids, message text, and 0-100 confidence scores.
- `Act4ConfidenceConfiguration(...)`: validates the player threshold and stores fallback/handoff wiring.
- `Act4ConfidenceValidationResult.IsCorrect`: true only when there are no deterministic validation errors.

### Input

Constructor values supplied by authored data or the presentation controller.

### Output

Immutable data objects consumed by `Act4ConfidenceValidator` and the Act 4 UI.

### Failure Cases

Invalid ids, empty visitor text, or out-of-range confidence/threshold values throw exceptions before validation.

### Unity Test

Run `Act4ConfidenceValidatorTests` in EditMode.

---

### Script Name

Act4ConfidenceDemoData.cs

### Purpose

Provides the authored Act 4 visitor queue, starting threshold, and acceptable threshold range. The data includes clear messages, ambiguous messages, a garbled message, and one upset/complex handoff case.

### Attached GameObject

None. This is pure static authored data.

### Runtime Role

The validator tests and Act 4 presentation controller create fresh visitor queues from this class.

### Important Fields

- `AcceptableThresholdMinimum`: 65.
- `AcceptableThresholdMaximum`: 80.
- `StartingThreshold`: 30.
- `CreateVisitorMessages()`: returns the six authored visitor messages and their deterministic expected outcomes.

### Input

None.

### Output

A fresh visitor queue for each validation or UI run.

### Failure Cases

If authored confidence scores or expected outcomes are changed, the validator tests should be updated to keep the intended low/high threshold failures covered.

### Unity Test

Run `Act4ConfidenceValidatorTests` in EditMode and confirm the reference configuration passes while low/high thresholds fail.

---

### Script Name

Act4ConfidenceValidator.cs

### Purpose

Deterministically validates the Act 4 player configuration. It checks the threshold range, fallback wiring, handoff wiring, and every authored visitor route outcome.

### Attached GameObject

None. This is pure C# puzzle logic in `Ghost.Runtime`.

### Runtime Role

Called by `Act4ConfidenceInteractionController.RunDay()` when the player chooses `Run the day`.

### Important Fields

No serialized fields.

### Important Methods

- `Validate(...)`: checks configuration-level requirements and runs every visitor through `RunVisitor(...)`.
- `RunVisitor(...)`: returns `IntentReply`, `Fallback`, `Handoff`, `NoSafeRoute`, or `Meltdown` from authored data and player wiring.

### Input

An `Act4ConfidenceConfiguration` plus the authored visitor queue.

### Output

An `Act4ConfidenceValidationResult` with per-visitor outcomes and error messages for UI feedback.

### Failure Cases

- Threshold below 65 or above 80 fails the range check.
- Missing fallback fails wiring and uncertain-message outcomes.
- Missing handoff makes the upset/complex case melt down.
- Empty visitor queues or null visitors produce validation errors.

### Unity Test

Run the full EditMode suite or `Ghost.Tests.EditMode.Act4ConfidenceValidatorTests`.

---

### Script Name

Act4ConfidenceInteractionController.cs

### Purpose

Owns Act 4 presentation state: the concrete low-threshold bluff example, threshold value, route attachments, day-run playback, retry state, completion state, Ghost mood, backend attempt logging, and ambient Lily hint requests after incorrect runs.

### Attached GameObject

None. This is a plain C# controller created by `Act4ConfidenceStaticPresenter`.

### Runtime Role

The presenter creates one controller per rendered Act 4 session. UI buttons and sliders call into it, and it raises `StateChanged` whenever the presenter should rebuild the visible UI.

### Important Fields

No serialized fields. Runtime state includes current phase, threshold, fallback/handoff wiring, current visitor index, last validation result, status line, and Ghost mood.

### Important Methods

- `BeginAfterOnboarding()`: unlocks configuration after Lily's first beat.
- `ReplayOnboarding()`: replays Lily's onboarding while preserving configuration state.
- `SetThreshold(...)`: clamps the slider value to 0-100.
- `ToggleFallbackWiring()` / `ToggleHandoffWiring()`: attach or detach the two safe routes.
- `RunDay()`: validates the current configuration and starts visitor playback.
- `AdvancePlayback()`: steps through the queue and finishes the run.
- `FinishDayRun()`: enters complete state on success or returns to configuration with retry feedback on failure.

### Input

UI events from the Act 4 presenter: onboarding button, route buttons, threshold slider, `Run the day`, `Next visitor`, `Finish the day`, `Try again`, and `Complete Act`.

### Output

Updated state, Ghost mood, status text, validation feedback, backend attempt logging, and non-spoiler ambient Lily hint requests on failed runs.

### Failure Cases

If validation fails, controls become editable again and the primary action becomes `Try again`. Severe failures set the Ghost face to sad; wrong threshold outcomes set it to confused.

### Unity Test

Open `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity` in Play Mode and run through low threshold, high threshold, missing fallback, missing handoff, and the passing configuration.

---

### Script Name

Act4ConfidenceStaticPresenter.cs

### Purpose

Builds the Act 4 UGUI page at runtime: header, phase progress, persistent objective strip, explicit goal/routing tutorial, three-step task guide, Ghost conversation panel, labelled visitor confidence scores, threshold trade-off labels, fallback/handoff controls, per-visitor rule comparisons, retry feedback, and completion button.

### Attached GameObject

Attached to the root object in `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity` by the scene builder.

### Runtime Role

On `Start`, creates the interaction controller and renders the current state. Each controller state change clears and rebuilds the generated UI.

### Important Fields

- `renderOnStart`: when true, renders the sample Act 4 data in Play Mode.

### Important Methods

- `RenderSampleData()`: ensures an EventSystem, creates a controller, subscribes to state changes, and renders the first state.
- `CreateHeader()`, `CreateObjectiveStrip()`, `CreateOnboardingPanel()`, `CreateConversationPanel()`, `CreateMainBody()`: build the M0-T46 page composition while keeping the goal and current task visible. - `CreateConfidenceControls(...)`: explains the answer/fallback rule and the three player actions before presenting controls. - `CreateThresholdSlider(...)`: builds the 0-100 confidence slider with a live minimum-answer rule and low/high trade-off labels. - `GetConversationLabel()`: shows the exact confidence-versus-threshold comparison during playback, or the upset/complex handoff rule.
- `CreateRouteControl(...)`: builds the fallback and handoff attach buttons.
- `HandlePrimaryAction()`: routes the main button to run/playback/complete behaviour.

### Input

Unity UI button and slider events.

### Output

A playable Act 4 scene with deterministic Ghost face moods and Shell debrief handoff through `GhostNarrativeState.SetPendingDebriefAct(GhostNarrativeState.Act4Id)`.

### Failure Cases

If the scene lacks an EventSystem, the presenter creates one. If the controller is missing, render calls exit without changing UI. If Shell constants are missing, completion cannot return through the debrief path.

### Unity Test

Use the Act 4 Play Mode checklist in `Docs/UNITY_TEST_CHECKLIST.md`.

---

### Script Name

Act4ConfidencePrototypeSceneBuilder.cs

### Purpose

Editor-only scene builder for Act 4. It creates the camera, canvas, EventSystem, Act 4 root object, attaches `Act4ConfidenceStaticPresenter`, saves the generated scene, and appends the scene to Build Settings.

### Attached GameObject

None. This is an Editor menu utility under `Assets/Presentation/Act4ConfidenceFallback/Editor`.

### Runtime Role

No runtime role. It is compiled only in the Editor assembly.

### Important Fields

No serialized fields.

### Important Methods

- `BuildAct4ConfidencePrototypeScene()`: available from `Ghost > Build Act 4 Confidence and Fallback Scene`.
- `AppendSceneToBuildSettings(...)`: adds Act 4 if it is not already present.

### Input

Manual Unity Editor menu action or batchmode `-executeMethod`.

### Output

`Assets/Scenes/Act4ConfidenceFallbackPrototype.unity` and the Act 4 Build Settings entry.

### Failure Cases

If scripts do not compile, the menu item and batchmode method cannot run. If Build Settings already contain the Act 4 scene, no duplicate entry is added.

### Unity Test

Run the menu builder, open the generated scene, and enter Play Mode.

---

### Script Name

Act4ConfidenceValidatorTests.cs

### Purpose

Tests the Act 4 deterministic confidence/fallback validator.

### Attached GameObject

None. This is an EditMode test script.

### Runtime Role

Runs only in Unity EditMode Test Runner.

### Important Fields

No serialized fields.

### Important Methods

NUnit tests cover:
- reference threshold plus both routes passes the day
- threshold outside the authored range fails
- missing fallback and handoff fail wiring and outcomes
- very low and very high thresholds produce the intended wrong authored outcomes

### Input

`Act4ConfidenceDemoData.CreateVisitorMessages()` plus test configurations.

### Output

NUnit pass/fail results.

### Failure Cases

Failed assertions indicate the validator no longer enforces the intended threshold range, safe-route wiring, or low/high threshold consequences.

### Unity Test

Run `Ghost.Tests.EditMode.Act4ConfidenceValidatorTests` or the full EditMode suite.

---

### Shell Integration Note

M0-T47 extends the existing Shell scripts with Act 4 constants, hub card wiring, intro/debrief beats, return-to-hub overlay support, and Build Settings registration. The generated `GameShellPrototype.unity` scene now serializes the Act 4 hub card/button, and `Act4ConfidenceFallbackPrototype.unity` returns through the same pending-debrief path used by Acts 1-3.
## Act 5 Testing and Debugging

### Script Name

Act5TestingModels.cs

### Purpose

Defines the authored test-conversation wrapper, per-conversation expected/actual result, and full suite result used by Act 5.

### Attached GameObject

None. This is pure Ghost.Runtime logic.

### Runtime Role

Carries visitor text beside the existing DialogGraphTestCase and exposes stable case results, validation errors, correctness, and passed count to tests and presentation code.

### Important Methods

- Act5TestConversation(...): validates the authored id/message and creates the existing dialog-graph test case.
- Act5TestCaseResult(...): compares the simulator response id with the authored expected response id.
- Act5TestSuiteResult.PassedCount: counts green cases without introducing a second scoring rule.

### Failure Cases

Empty ids/messages and null conversations are rejected. Correctness still comes from the existing DialogGraphValidator result.

### Unity Test

Run Act5TestSuiteRunnerTests in EditMode.

---

### Script Name

Act5BuggyGraphData.cs

### Purpose

Provides the Act 5 nodes, three seeded wiring faults, reference fixed transitions, four test conversations, player-facing response lines, and graph-node labels.

### Attached GameObject

None. This is pure authored data.

### Runtime Role

Creates a buggy graph with swapped room routes, a wrong lab-hours reply, and a missing greeting branch. CreateFixedGraph supplies the deterministic reference used by tests only.

### Important Methods

- CreateBuggyGraph(): builds the starting graph that fails all four conversations.
- CreateFixedGraph(): builds the reference graph that passes all four.
- CreateNodes(), CreateBuggyTransitions(), CreateFixedTransitions(): provide fresh immutable graph parts.
- CreateTestConversations(): returns the four authored preview conversations.
- GetResponseLine(...), GetNodeTitle(...): translate stable ids into visible Act 5 copy.

### Failure Cases

Changing a node id, response id, or expected route without updating all related transitions/tests can invalidate the seeded-fault contract.

### Unity Test

Confirm the buggy graph reports 0/4 and the fixed graph reports 4/4.

---

### Script Name

Act5TestSuiteRunner.cs

### Purpose

Runs Act 5 through the existing DialogGraphValidator and DialogGraphSimulator, then packages per-case expected/actual results for the UI.

### Attached GameObject

None. This is pure Ghost.Runtime logic.

### Runtime Role

Called whenever the player chooses Run all tests or Rerun all tests.

### Important Methods

- Run(...): validates the graph against every authored test and simulates each conversation with a fresh DialogContext.
- CopyConversations(...): rejects null suite entries before simulation.

### Failure Cases

Null graph/suite arguments throw. Structural graph errors and response mismatches remain visible through Act5TestSuiteResult.ValidationErrors.

### Unity Test

Run Act5TestSuiteRunnerTests in EditMode.

---

### Script Name

IDialogGraphWireInteractionHost.cs

### Purpose

Defines the four pointer callbacks required by the existing Act 3 input/output port views so more than one presenter can reuse the same wire-drag interaction.

### Attached GameObject

None. This is a presentation interface.

### Runtime Role

Act3DialogGraphStaticPresenter and Act5TestingStaticPresenter both implement it. Act 3 input/output port behaviour is unchanged; only the held presenter type was generalized.

### Important Methods

BeginWireDrag(...), UpdateWireDrag(...), EndWireDrag(...), and CompleteWireDrop(...) form the shared drag/drop contract.

### Failure Cases

A host that does not clean up cancelled drag wires can leave a temporary visual line. Both current presenters handle cleanup.

### Unity Test

Play Act 3 once to confirm existing graph wiring still works, then test Act 5 reconnection.

---

### Script Name

Act5TestingInteractionController.cs

### Purpose

Owns Act 5 state, mutable transition wiring, suite runs, stale-result state, Ghost mood, backend attempt logging, and completion.

### Attached GameObject

None. The Act 5 presenter creates this plain C# controller.

### Runtime Role

Starts from Act5BuggyGraphData, allows legal Act 3-style output reconnections, constructs the current DialogGraph, and calls Act5TestSuiteRunner.

### Important Methods

- BeginAfterOnboarding() and ReplayOnboarding(): control the replayable teaching state.
- RunAllTests(): runs the deterministic suite, records 0-4 pass count, requests a non-spoiler Lily hint after failure, and completes only at 4/4.
- ConnectNodes(...): adds missing start-to-intent branches or replaces one non-start output destination.
- FindLastResult(...) and FindFirstFailure(): provide stable feedback selection for result cards and the conversation panel.

### Failure Cases

Illegal self-links, wrong source conditions, incompatible target types, duplicate links, onboarding edits, and post-completion edits are rejected.

### Unity Test

Run the initial suite, repair one route, confirm results become stale, rerun, and continue until 4/4.

---

### Script Name

Act5TestingStaticPresenter.cs

### Purpose

Builds the Act 5 M0-T46-style UGUI page: header/progress, objective strip, onboarding/replay note, Ghost result panel, pre-built dialog graph, shared drag ports/wires, four expected/actual test cards, rerun state, and completion button. The graph keeps a visible test / repair / rerun sequence and labels left input versus right output sockets directly on each node.

### Attached GameObject

Attached to the Act 5 scene root by Act5TestingPrototypeSceneBuilder.

### Runtime Role

Creates the controller on Start, rebuilds the visible state after tests or graph edits, and returns through the Shell pending-debrief path after completion.

### Important Methods

- RenderSampleData() and RenderState(): create and refresh the runtime page.
- CreateGraphPanel(...) and CreateNodeCard(...): display the fixed node set and current authored transitions.
- BeginWireDrag(...), UpdateWireDrag(...), EndWireDrag(...), CompleteWireDrop(...): reuse the Act 3 port components to reconnect routes.
- CreateTestPanel(...) and CreateTestCard(...): keep visitor, expected, actual, pass/fail, and stale state visible.
- GetGraphGuideText() and GetPrimaryActionLabel(): keep the numbered test, repair, and rerun instructions visible after onboarding.
- RebuildWires(): draws the current transitions behind graph nodes after Canvas and nested layouts have resolved.
- DrawLine(...): anchors each line at the wire layer centre and places its pivot at the source port, matching the local coordinates returned by GetPortLocalCenter(...).
- HandlePrimaryAction(): runs/reruns the suite or completes Act 5.

### Failure Cases

Invalid drops are ignored. Missing ports skip only the affected visual wire. A missing EventSystem is created automatically. Output sockets are visibly muted until the first test run unlocks editing. Wire geometry must use centre anchors; treating centre-based local coordinates as bottom-left anchored positions shifts the full graph off the board.

### Unity Test

Use the M0-T48 Play Mode checklist at 1920x1080 and confirm every committed and dragged wire touches its source and destination sockets, then confirm cards, text, and complete/debrief flow.

---

### Script Name

Act5TestingStaticPresenterTests.cs

### Purpose

Protects the Act 5 wire-layer coordinate contract that keeps graph lines attached to their node sockets.

### Attached GameObject

None. This is an EditMode presentation regression test.

### Important Test

- DrawLine_UsesCenteredWireLayerCoordinatesWithoutBoardOffset: confirms centre anchors, source pivot, source position, line length, and thickness.

### Unity Test

Run Ghost.Tests.EditMode.Act5TestingStaticPresenterTests or the full EditMode suite.

---

### Script Name

Act5TestingPrototypeSceneBuilder.cs

### Purpose

Creates the Act 5 camera, 1920x1080 overlay canvas, EventSystem, presenter root, generated scene, and approved Build Settings entry.

### Attached GameObject

None. This is an Editor menu utility in Ghost.Presentation.Act5.Editor.

### Runtime Role

No runtime role.

### Important Methods

- BuildAct5TestingPrototypeScene(): available from Ghost > Build Act 5 Testing and Debugging Scene.
- AppendSceneToBuildSettings(...): adds the Act 5 scene without duplicating existing entries.

### Failure Cases

The builder cannot run until all runtime/presentation scripts compile. Existing scenes and existing Build Settings entries are not edited by hand.

### Unity Test

Run the builder, open Assets/Scenes/Act5TestingDebuggingPrototype.unity, and enter Play Mode.

---

### Script Name

Act5TestSuiteRunnerTests.cs

### Purpose

Proves the authored seeded-fault and reference-fixed graph contracts.

### Attached GameObject

None. This is an EditMode test script.

### Important Tests

- BuggyGraphFailsEveryAuthoredConversation: confirms 0/4 and an incorrect validator result.
- FixedGraphPassesEveryAuthoredConversation: confirms 4/4 and no validation errors.
- BuggyGraphReportsExpectedAndActualResponseForRoomCase: confirms expected/actual mismatch detail.
- BuggyGraphReportsNoResponseForMissingGreetingBranch: confirms the missing start branch is observable.

### Unity Test

Run Ghost.Tests.EditMode.Act5TestSuiteRunnerTests or the full EditMode suite.

---

### Shell Integration Note

M0-T48 adds Act 5 scene/id constants, hub button wiring, intro/debrief beats, Return to Hub support, GameShell builder registration, and the approved Act 5 Build Settings append. Completion uses GhostNarrativeState.Act5Id and the same pending-debrief flow as earlier Acts.


---

## M0-T49 Runs 001-002: Final Chapter Repair Ghost's Voice

### Script Name

Act6PipelineModels.cs

### Purpose

Defines the immutable component, playback-step, and validation-result records shared by the Final Chapter data and deterministic validator.

### Attached GameObject

None. This is pure Ghost.Runtime data.

### Runtime Role

Carries authored labels, component jobs, prior-chapter callbacks, failure lines, playback text, validation errors, and the first broken component id without any UI dependency.

### Failure Cases

Null error collections become an empty array. Unknown component ids are rejected by Act6PipelineData rather than represented by these models.

### Unity Test

Run Act6PipelineValidatorTests in EditMode.

---

### Script Name

Act6PipelineData.cs

### Purpose

Provides the authored six-part palette, canonical five-stage main path, backend side link, final visitor/reply, prior-chapter callbacks, failure lines, and six deterministic playback beats.

### Attached GameObject

None. This is pure Ghost.Runtime authored data.

### Runtime Role

Supplies fresh read-only lists to the validator and presentation while keeping scoring independent from the UI and backend.

### Important Methods

- CreateMainPipelineOrder(): returns UI input, NLP engine, dialogue management, response generation, and UI output.
- CreatePaletteComponents(): returns all six cards in an intentionally shuffled order.
- CreatePlaybackSteps(): follows the final message through five main stages and the backend side fetch.
- GetComponent(...): resolves labels, jobs, prior work, and failure consequences by stable id.

### Failure Cases

GetComponent throws for an unknown id so malformed authored data cannot silently pass.

### Unity Test

Confirm the palette is shuffled, the correct path validates, and the final lab-hours message produces the authored full reply.

---

### Script Name

Act6PipelineValidator.cs

### Purpose

Deterministically validates the exact five-stage order and the required backend side link.

### Attached GameObject

None. This is pure Ghost.Runtime logic.

### Important Method

Validate(...): reports empty, misplaced, unknown, duplicate, excess, and missing-backend errors while preserving the first broken expected component for focused feedback.

### Failure Cases

A null or partial main-slot list fails safely. Backend integration in the main path fails, and a correct main path without the side link still fails.

### Unity Test

Run Ghost.Tests.EditMode.Act6PipelineValidatorTests.

---

### Script Name

Act6PipelineInteractionController.cs

### Purpose

Owns Final Chapter onboarding, placement, selection, swapping, validation, stage playback, Ghost mood, hint request, attempt logging, and transition into the ending.

### Attached GameObject

None. Act6PipelineStaticPresenter creates this plain C# controller.

### Important Methods

- PlaceInMainSlot(...) and PlaceInBackendSlot(...): enforce main-path versus side-link roles and support swapping placed cards.
- ResetPipeline(): clears every placement and result.
- RunPipeline(): calls only Act6PipelineValidator for correctness, records the attempt, and focuses the first broken stage on failure.
- AdvancePlayback(): moves through all six authored message beats before revealing Ghost's complete reply.
- BeginEnding(): exposes the final programmatic ending only after playback finishes.

### Failure Cases

Placement is ignored outside Configure phase. Backend cards cannot enter the main path, main cards cannot enter the backend socket, and failed runs remain editable.

### Unity Test

Exercise both drag/drop and click-select placement, then test wrong order, missing backend, correct playback, and ending entry.

---

### Script Name

IAct6PipelineInteractionHost.cs

### Purpose

Defines the narrow component-selection and destination-placement contract used by Final Chapter pointer views.

### Attached GameObject

None. This is a presentation interface implemented by Act6PipelineStaticPresenter.

### Failure Cases

The pointer views require a configured host; otherwise they ignore interaction safely.

### Unity Test

Confirm cards can be dragged or click-selected into both main and backend destinations.

---

### Script Name

Act6PipelinePartDragView.cs

### Purpose

Makes palette and placed cards draggable, creates a cursor-following preview, and supplies click selection as an accessible fallback.

### Attached GameObject

Added at runtime to every Final Chapter component card.

### Important Methods

- Configure(...): binds component id, display label, root canvas, and interaction host.
- OnBeginDrag(...), OnDrag(...), and OnEndDrag(...): manage raycasts and the temporary preview.
- ClearActivePreviews(): removes stale previews during presenter rebuilds or completed drops.

### Failure Cases

Missing canvas or event data cancels preview movement without changing puzzle state. Disable/destroy restores raycasts and removes owned previews.

### Unity Test

Drag from palette and occupied slots; confirm the preview tracks the pointer and disappears after drop or cancellation.

---

### Script Name

Act6PipelineSlotDropView.cs

### Purpose

Turns each numbered main slot and the backend socket into both drop targets and click-placement targets.

### Attached GameObject

Added at runtime to the five main slots and one backend socket.

### Failure Cases

Drops without an Act6PipelinePartDragView are ignored. Main/backend role errors are delegated to the controller for explicit player feedback.

### Unity Test

Confirm a selected card can be placed by clicking a destination and a dragged card can be dropped on the same destination.

---

### Script Name

Act6PipelineStaticPresenter.cs

### Purpose

Builds the complete Final Chapter UGUI experience: phase header, objective, replayable Lily onboarding, Ghost result panel, shuffled palette, numbered stable pipeline slots, backend side socket, failure feedback, staged playback, and ending overlay.

### Attached GameObject

Attached to the generated Final Chapter scene root by Act6VoicePipelinePrototypeSceneBuilder.

### Important Methods

- RenderSampleData() and RenderState(): create and refresh the runtime page from controller state.
- CreateOnboardingPanel(): explains the purpose, five-stage message route, backend exception, and exact drag/click task before interaction begins.
- CreatePalettePanel(...) and CreatePipelinePanel(...): render the six cards, five main slots, arrows, backend socket, live status, reset, and primary action.
- CreateMainSlot(...) and CreateBackendSlot(...): reveal each correct component's authored job and prior-chapter connection.
- HandlePrimaryAction(): runs validation, advances every playback beat, or starts the ending according to phase.
- CreateEndingOverlay(): assembles the happy Ghost, glow, personalized text, credits, and skip control used by Act6EndingSequence.

### Failure Cases

Unknown/missing component data remains a validator error rather than UI scoring. Layout uses fixed header/objective/conversation heights and flexible body columns for the 1920x1080 target; smaller aspect ratios still require visual regression testing.

### Unity Test

Use the M0-T49 Play Mode checklist at 1920x1080 and verify readable instructions, stable slots, every failure state, six-step playback, ending, skip, and return to title.

---

### Script Name

Act6EndingSequence.cs

### Purpose

Runs the programmatic final animation with unscaled-time fades, Ghost glow/float, player-name thank-you, Lily closing line, credits scroll, skip, completion persistence, and return to the Shell title.

### Attached GameObject

Added at runtime to the Final Chapter presenter root.

### Important Methods

- Configure(...): receives the overlay, Ghost, glow, text, credits, and skip references.
- Play(): resets the visual state and starts one sequence.
- Skip(): completes through the same FinishEnding path as the full sequence.
- FinishEnding(): marks GhostNarrativeState.FinalChapterId complete and loads GameShellPrototype.

### Failure Cases

Repeated Play/Skip calls are idempotent. Missing optional visual references skip that animation piece but still complete and return to the Shell.

### Unity Test

Watch the full ending once, then replay and skip immediately; both paths must mark Final Chapter complete and return to the title without Console errors.

---

### Script Name

Act6VoicePipelinePrototypeSceneBuilder.cs

### Purpose

Creates the Final Chapter camera, 1920x1080 canvas, Input System EventSystem, presenter root, generated scene, and approved Build Settings entry.

### Attached GameObject

None. This is an Editor menu utility in Ghost.Presentation.Act6.Editor.

### Important Method

BuildAct6VoicePipelinePrototypeScene(): available from Ghost > Build Final Chapter Repair Ghost's Voice Scene.

### Failure Cases

The builder requires all Final Chapter and Shell scripts to compile. It appends the scene only when its path is absent.

### Unity Test

Run the builder, open Assets/Scenes/Act6VoicePipelinePrototype.unity, and enter Play Mode.

---

### Script Name

Act6PipelineValidatorTests.cs

### Purpose

Protects the canonical main-path order, backend side-link rule, partial-pipeline diagnosis, role separation, and deterministic duplicate handling.

### Attached GameObject

None. This is an EditMode test script.

### Important Tests

- CorrectMainOrderAndBackendPass: proves the reference build succeeds.
- SwappedOpeningStagesFailAtUiInput and PartialPipelineReportsFirstMissingStage: prove focused first-break feedback.
- MissingBackendFailsAfterCorrectMainPath and BackendCannotReplaceAMainPipelineStage: protect the side-link model.
- DuplicateMainComponentFailsDeterministically: protects stable duplicate/order failure reporting.

### Unity Test

Run Ghost.Tests.EditMode.Act6PipelineValidatorTests or the full EditMode suite.

---

### Shell Integration Note

Run 003 reclassifies this implementation as the Final Chapter. The retained Act6Pipeline class and asset names preserve existing Unity meta identities, but gameplay completion now uses GhostNarrativeState.FinalChapterId. Chapter 6 is the separate backend-action teaching puzzle documented below.

---

## M0-T49 Run 003: Chapter 0, Chapter 6 Teaching, and Final Chapter Split

### Structure Correction

The user-approved route is now explicit:

- Chapter 0 is an opening story with no lesson validator or score.
- Chapters 1-6 are teaching chapters.
- Chapter 6 teaches backend action and response generation.
- Final Chapter uses the existing full voice-pipeline capstone and ending.

The old Act6Pipeline class, folder, and scene filename remain for Unity asset stability. Player-facing text and narrative completion treat that implementation as Final Chapter.

---

### Script Name

Chapter0StoryData.cs

### Purpose

Defines six authored opening-story beats based on the confirmed late-lab premise: Lily introduces Ghost, Ghost's speech is tangled, and the player agrees to help one message at a time.

### Attached GameObject

None. This is presentation-owned authored story data.

### Runtime Role

Creates a fresh ordered beat list personalized with GhostNarrativeState.PlayerName. Each beat carries speaker text and Ghost mood only; it contains no puzzle answer or scoring rule.

### Failure Cases

A blank player name falls back to Junior. The data does not invent a Chapter 0 lesson or validator.

### Unity Test

Play Chapter0OpeningStory from the Shell and confirm all six beats appear in order.

---

### Script Name

Chapter0StoryPresenter.cs

### Purpose

Builds the complete Chapter 0 opening scene at runtime with a late-lab stage, Lily portrait, Ghost face, current-speaker emphasis, dialogue progress, Continue, Skip opening, and Enter the lab.

### Attached GameObject

Attached to the Chapter 0 scene root by Chapter0StorySceneBuilder.

### Important Methods

- BeginStory(): loads the personalized story beats and renders beat 1.
- Advance(): moves to the next beat without scoring.
- FinishStory(): sets Chapter 0's pending debrief and returns to GameShellPrototype.
- CreateLabStage(...): builds the stable lab backdrop and character frames.
- CreateDialogue(...): shows the active speaker, line, progress action, and final Enter the lab action.

### Failure Cases

Advance ignores empty or completed story state. FinishStory is idempotent. Chapter 0 is marked complete by the Shell debrief flow, not before the debrief can play.

### Unity Test

Verify Continue, Skip opening, final Enter the lab, personalized name, speaker highlights, Shell return, and the one-time Chapter 0 debrief.

---

### Script Name

Chapter0StorySceneBuilder.cs

### Purpose

Creates Chapter0OpeningStory.unity with a camera, 1920x1080 scaled UGUI canvas, Input System EventSystem, and Chapter0StoryPresenter root.

### Attached GameObject

None. This is an Editor menu utility in Ghost.Presentation.Story.Editor.

### Important Method

BuildChapter0OpeningStoryScene(): available from Ghost > Build Chapter 0 Opening Story Scene.

### Failure Cases

The builder requires Story and Shell assemblies to compile. It appends the scene only when absent; GameShellSceneBuilder later establishes canonical Build Settings order.

### Unity Test

Run the builder, open Assets/Scenes/Chapter0OpeningStory.unity, and enter Play Mode.

---

### Script Name

Act6BackendResponseModels.cs

### Purpose

Defines the immutable card, playback-step, and validation-result models used by the Chapter 6 backend lesson.

### Attached GameObject

None. This is pure Ghost.Runtime data.

### Runtime Role

Carries stable card roles, job/failure text, five playback steps, validation errors, and the first broken role without any Unity UI dependency.

### Failure Cases

Null strings and error collections are normalized safely.

### Unity Test

Run Act6BackendResponseValidatorTests in EditMode.

---

### Script Name

Act6BackendResponseData.cs

### Purpose

Authors the Chapter 6 visitor request, three socket roles, three correct cards, three role-matched distractors, backend result, complete reply, and five playback beats.

### Attached GameObject

None. This is pure Ghost.Runtime authored data.

### Runtime Role

Teaches this concrete chain: lab records -> fetch lab closing time -> lab-hours response. Distractors use the object-room task so each role can be wrong without violating socket type.

### Important Methods

- CreateCards(): returns all six draggable cards.
- CreatePlaybackSteps(): returns the visitor, data, action, response, and delivered-reply sequence.
- GetExpectedCardId(...): defines the deterministic answer for each role.
- GetCard(...) and GetRoleLabel(...): resolve stable authored ids.

### Failure Cases

Unknown role or card ids throw instead of silently becoming correct.

### Unity Test

Confirm the correct three-card chain produces The lab closes at 8 PM.

---

### Script Name

Act6BackendResponseValidator.cs

### Purpose

Deterministically checks the selected data source, backend action, and response template.

### Attached GameObject

None. This is pure Ghost.Runtime puzzle logic.

### Important Method

Validate(...): validates all three sockets, reports empty, unknown, wrong-role, and wrong-answer errors, and preserves the first broken role for focused feedback.

### Failure Cases

A null or partial board fails safely. A card from another role cannot satisfy a socket. Presentation and backend services never decide correctness.

### Unity Test

Run Ghost.Tests.EditMode.Act6BackendResponseValidatorTests.

---

### Script Name

Act6BackendInteractionController.cs

### Purpose

Owns Chapter 6 onboarding, card selection, socket placement, swaps, reset, deterministic validation, five-step playback, Ghost mood, attempts, hints, and completion state.

### Attached GameObject

None. Act6BackendStaticPresenter creates this plain C# controller.

### Important Methods

- PlaceSelectedCard(...) and PlaceCard(...): enforce role sockets and predictable replacement.
- Reset(): returns every card to the palette.
- Run(): calls only Act6BackendResponseValidator and focuses the first broken role.
- AdvancePlayback(): reveals how raw backend data becomes the final visitor-facing sentence.

### Failure Cases

Placement is ignored outside Configure phase. Wrong-role drops give explicit feedback. Failed runs remain editable.

### Unity Test

Exercise drag/drop, click-select, each distractor, reset, the correct chain, and all playback steps.

---

### Script Name

IAct6BackendInteractionHost.cs

### Purpose

Defines the narrow card-selection and role-socket placement contract used by Chapter 6 pointer views.

### Attached GameObject

None. Implemented by Act6BackendStaticPresenter.

### Failure Cases

Unconfigured pointer views ignore interaction safely.

### Unity Test

Confirm both drag/drop and click-select placement reach the same controller methods.

---

### Script Name

Act6BackendCardDragView.cs

### Purpose

Makes Chapter 6 cards draggable and click-selectable, with a cursor-following preview that does not move the stable source layout.

### Attached GameObject

Added at runtime to palette and placed cards.

### Failure Cases

Missing canvas, host, or event data cancels the preview without changing puzzle state. Disable/destroy removes owned previews.

### Unity Test

Drag from palette and from a filled socket; verify the preview disappears after drop or cancellation.

---

### Script Name

Act6BackendSlotDropView.cs

### Purpose

Turns each DATA SOURCE, ACTION, and RESPONSE socket into both a drop target and a selected-card destination.

### Attached GameObject

Added at runtime to all three role sockets.

### Failure Cases

Drops without an Act6BackendCardDragView are ignored; role feedback comes from the controller.

### Unity Test

Place cards by dragging and by click-selecting a card followed by a socket.

---

### Script Name

Act6BackendStaticPresenter.cs

### Purpose

Builds the complete Chapter 6 teaching UI: clear objective, Lily onboarding, 170-pixel Ghost panel, six-card palette, three fixed role sockets with arrows, status feedback, reset/run controls, and five-step message playback.

### Attached GameObject

Attached to the generated Chapter 6 root by Act6BackendResponseSceneBuilder.

### Important Methods

- RenderState(): refreshes the page from controller state.
- CreateOnboardingPanel(): explains why real data is needed and exactly what the three roles do.
- CreatePalette(...) and CreatePipeline(...): render stable cards, sockets, arrows, state colors, and controls.
- HandlePrimaryAction(): opens the board, validates, advances playback, or completes through the Shell debrief.

### Failure Cases

The presenter does not score. Stable socket dimensions prevent feedback, highlighting, or card labels from shifting the board. 1920x1080 remains the primary authored target.

### Unity Test

Use the Run 003 checklist and verify readable purpose/task text, all wrong states, correct playback, completion, and Return to Hub.

---

### Script Name

Act6BackendResponseSceneBuilder.cs

### Purpose

Creates Act6BackendResponsePrototype.unity with a camera, 1920x1080 canvas, Input System EventSystem, and Act6BackendStaticPresenter root.

### Attached GameObject

None. This is an Editor menu utility in Ghost.Presentation.Act6BackendResponse.Editor.

### Important Method

BuildAct6BackendResponseScene(): available from Ghost > Build Chapter 6 Backend Action and Response Scene.

### Failure Cases

The builder requires the runtime, presentation, and Shell assemblies to compile. It does not hand-edit scene YAML.

### Unity Test

Run the builder, open Assets/Scenes/Act6BackendResponsePrototype.unity, and enter Play Mode.

---

### Script Name

Act6BackendResponseValidatorTests.cs

### Purpose

Protects the Chapter 6 reference chain, empty-board diagnosis, every role-specific distractor, and cross-role rejection.

### Attached GameObject

None. This is an EditMode test script.

### Important Tests

- ReferenceBackendActionAndResponsePass.
- EmptyBoardFailsAtDataSource.
- WrongSourceStopsAtDataSource.
- WrongActionStopsAtAction.
- WrongResponseStopsAtResponse.
- CardFromWrongRoleCannotSatisfySocket.

### Unity Test

Run Ghost.Tests.EditMode.Act6BackendResponseValidatorTests or the full EditMode suite.

---

### Shell Integration Note

ShellSceneNames now maps Chapter 0 to Chapter0OpeningStory, Chapter 6 to Act6BackendResponsePrototype, and Final Chapter to the retained Act6VoicePipelinePrototype asset. GameShellPresenter starts Chapter 0 after first-time naming/account setup, exposes replay and Final Chapter buttons, gives separate Chapter 6/final intros, and tracks Final Chapter with FinalChapterId. GameShellSceneBuilder serializes both story-route buttons and registers the canonical scene order: Shell, Chapter 0, Chapters 1-6, Final Chapter, then unrelated existing scenes.

### Inspector Setup

No manual Inspector wiring is required when using the three menu builders. Regenerate Chapter 0, Chapter 6, Final Chapter, then Game Shell in that order. The builders create and serialize all required cameras, canvases, EventSystems, presenter roots, buttons, and Build Settings entries.

### Automated Verification

Unity 6000.4.11f1 batchmode generated all four affected scenes without compiler errors. Act6BackendResponseValidatorTests passed 6/6, the reclassified Final Chapter Act6PipelineValidatorTests passed 6/6, and the complete EditMode suite passed 77/77.
## M0-T49 Run 005 Remediation Pass

### Completion and Navigation

- `ShellReturnToHubOverlay.cs`: `Return to Hub` is pure scene navigation. It no longer maps scenes to act ids or calls `SetPendingDebriefAct`.
- `Act2EntityExtractionStaticPresenter.cs`: the Complete phase now renders an explicit `Complete Act` button. Only the final successful authored errand can reach that phase; the button sets the Act 2 pending debrief and loads the Shell.
- `ShellReturnToHubOverlayTests.cs`: reads the overlay source and guards against reintroducing `SetPendingDebriefAct` or the removed scene-to-debrief helper.

### Shell Layout

- `GameShellSceneBuilder.cs`: the hub uses a dedicated compact vertical layout. At 1920x1080 the active seven-child budget is 588px: 24px vertical padding + 36px spacing + 44px heading + 40px copy + 72px fundamentals + 52px story route + 240px lesson grid + 40px narrative Continue + 40px Back to Title. This remains 76px inside the 664px Shell body.
- Chapter cards remain a stable 3-by-2 grid with two 116px rows and 8px between them. Content is compacted rather than hidden.

### Chapter 6 Validator-Only Feedback and Click Contract

- `Act6BackendResponseModels.cs` / `Act6BackendResponseValidator.cs`: the existing deterministic validator now records which authored role ids failed while preserving the same accepted cards, errors, first-broken role, and `IsCorrect` rule.
- `Act6BackendInteractionController.cs`: removes the UI-side expected-id comparison and adds `ReturnRoleCardToPalette`. Any placement change clears stale validation.
- `Act6BackendStaticPresenter.cs`: untested placements remain neutral and read `PLACED - run the route to test this responsibility.` Per-slot success/failure appears only from `LastValidation` after Run.
- `IAct6BackendInteractionHost.cs`, `Act6BackendSlotDropView.cs`, and `Act6BackendCardDragView.cs`: palette cards may click-select; placed cards do not click-select. One click on a filled socket returns that card to the palette, and consumed pointer events cannot perform a second stale action during synchronous re-render.
- `Act6BackendResponseValidatorTests.cs`: covers validator-owned per-role states and returning a filled role while clearing stale validation.

### Lily Pixel Art

- `Assets/Resources/Characters/LilyPixelFullBody.png`: transparent original pixel-art Lily with a high blonde ponytail, deep navy-blue blazer, red KCL lanyard, charcoal trousers, and black Oxford shoes.
- `Assets/Resources/Characters/LilyPixelPortrait.png`: transparent upper-body crop from the same approved asset for small Shell and banter portrait slots.
- `LilyPixelPortraitFactory.cs`: loads Unity-imported full-body and portrait Sprite sub-assets with point filtering; the prior code-drawn portrait remains only as a missing-resource fallback.
- `Chapter0StoryPresenter.cs`: displays the new full-body sprite with white tint and preserved aspect, fixing the prior transparent Image tint.
- `Act6PipelineStaticPresenter.cs` / `Act6EndingSequence.cs`: Final Chapter shows the new Lily sprite only during Lily's closing line, then hides it before credits. Final completion logic remains `FinalChapterId`.

### Inspector Setup

No manual wiring is required. Run the Chapter 0, Chapter 6 Backend Response, Final Chapter, and Game Shell builders, with Game Shell last. The Lily PNG must remain under `Assets/Resources/Characters/` so `LilyPixelPortraitFactory` can load it in Editor and WebGL.

### Play Mode Test

Use the single current M0-T49 Run 005 checklist in `Docs/UNITY_TEST_CHECKLIST.md`. In particular, return immediately from every chapter to verify no completion, complete Act 2 through its new button, inspect the 1080p hub, verify Chapter 6 remains neutral before Run and returns a filled card on one click, sanity-check Act 3 wire dragging, and complete/skip the Final ending.

## M0-T49 Run 006 Character Pixel-Style Unification

### Character Assets

- `LilyPixelFullBody.png`: replaced with a transparent 96x128 low-resolution RPG sprite. Lily keeps the approved high blonde ponytail, deep navy-blue blazer, red KCL lanyard, charcoal trousers, tablet, and black Oxford shoes, but uses chunky square pixels, a limited palette, a dark outline, and simplified facial features.
- `LilyPixelPortrait.png`: replaced with a transparent 96x96 upper-body crop in the same pixel language.
- `GhostPixelNeutral.png`, `GhostPixelHappy.png`, `GhostPixelConfused.png`, and `GhostPixelSad.png`: transparent 96x96 sprites with one consistent sheet-ghost body, large established dark eyes, small arms, wavy tail, and blue-lavender shadow pixels.
- All six files use hard 0/255 alpha and contain no smooth semi-transparent edge pixels.

### GhostFaceView.cs / GhostPixelSpriteFactory

- `GhostPixelSpriteFactory.GetSprite(...)` maps `GhostMood` to the four `Resources/Characters/GhostPixel*` textures, creates a full-rect runtime Sprite, caches it, and forces point filtering.
- `GhostFaceView.SetMood(...)` prefers the matching image and disables the old overlaid eyes, mouth, and question mark. If the texture is unavailable, it restores the original programmatic face behavior.
- Because all Chapters and the Final ending already use `GhostFaceView`, the style change applies without changing puzzle presenters or deterministic logic.

### LilyDialogueFrame.cs / AmbientBanterPanel.cs

- Ghost dialogue now falls back to `GhostPixelNeutral` when no explicit serialized Ghost portrait is assigned.
- Lily continues to load the new low-resolution full-body/portrait textures through `LilyPixelPortraitFactory`; its previous code-drawn portrait remains fallback-only.

### Automated Coverage

- `ShellReturnToHubOverlayTests.cs` now also checks all six character textures load at their authored 96px dimensions.
- `GhostFaceUsesPixelSpriteForEveryMood` creates a `GhostFaceView`, switches through every `GhostMood`, confirms a `GhostPixel*` Sprite is selected with point filtering, and confirms the old left-eye overlay is hidden.

### Inspector Setup

No new Inspector references are required. Keep all six PNGs under `Assets/Resources/Characters/`. Existing scenes receive the new art at runtime through `GhostFaceView`, `LilyPixelPortraitFactory`, `LilyDialogueFrame`, and `AmbientBanterPanel`; scene YAML does not need manual editing.

### Play Mode Test

At 1920x1080, check Chapter 0, one teaching Chapter, Shell dialogue/banter, and the Final ending. Lily and Ghost must read as the same chunky low-resolution RPG art style with crisp nearest-neighbor edges. Confirm Ghost visibly changes between neutral, happy, confused, and sad without duplicate old eyes or text-mouth marks.

## M0-T49 Run 007 Lily Colour Correction

- `LilyPixelFullBody.png` and `LilyPixelPortrait.png` keep the Run 006 low-resolution RPG silhouette, high ponytail, glasses, red KCL lanyard, grey shirt, charcoal trousers, and tablet.
- The suit blazer is restored from black to deep navy blue.
- The Oxford shoes are restored from brown to black.
- Both assets remain 96px, point-filtered, hard-alpha images; no runtime code, Inspector wiring, scene, or gameplay behavior changes.

## M0-T49 Run 008 Batchmode Verification

### EditMode Test Setup Corrections

- `Act6BackendResponseValidatorTests.cs`: the filled-role return test now seeds a real deterministic validator result through the private `LastValidation` setter. This preserves the stale-validation precondition without starting the Play Mode-only backend client runner in EditMode.
- `ShellReturnToHubOverlayTests.cs`: the Ghost pixel-sprite test calls `SetMood` before looking up generated child images, so `GhostFaceView` has created its runtime hierarchy before assertions run.
- These are test-harness corrections only. Runtime puzzle, scoring, navigation, backend, and scene-builder behavior did not change.

### Git Guard

- `.gitattributes`: Unity scene YAML is exempt from Git's trailing-space check because Unity serializes empty scalar values as lines such as `m_Name: `. Other files remain covered by the normal `git diff --check` rules.

### Verification Result

- The four required builders regenerated Chapter 0, Chapter 6 Backend Response, Final Chapter, and Game Shell in that order, with Game Shell last.
- The full Unity EditMode suite passed 87/87 after the two test setup corrections. Focused suites passed 8/8 for `ShellReturnToHubOverlayTests`, 8/8 for `Act6BackendResponseValidatorTests`, and 1/1 for `Act5TestingStaticPresenterTests`.
- Serialized guards found exactly one Main Camera, Canvas, and EventSystem and zero missing scripts in each regenerated scene.
- No Inspector changes are required. The only remaining verification is the single current 1920x1080 human Play Mode checklist in `Docs/UNITY_TEST_CHECKLIST.md`.

## Run 012 Lily Footwear and Sprite Import Repair

### LilyPixelFullBody.png

- Keeps the approved Run 007 96x128 RPG pixel sprite, including its proportions, high ponytail, round glasses, navy-blue blazer, red KCL lanyard, tablet, charcoal trousers, pose, and bookish expression.
- Changes only the lower footwear region. Black low-vamp Mary Jane flats show the top of each foot and a short ankle line.
- Pixel comparison against the Run 007 source reports zero changed pixels above y=110; the complete difference box is limited to x=39..62 and y=110..121.
- `LilyPixelPortrait.png` is unchanged because the requested change is below the portrait crop.

### LilyPixelSpriteImporter.cs

- `RepairLilyPixelSpriteImports()` configures the full-body and portrait PNG files as single sprites through Unity's `TextureImporter` API.
- It uses point filtering, 100 pixels per unit, no mipmaps, clamp wrapping, alpha transparency, and no texture compression.
- This removes stale sprite-sheet rectangles left from the older high-resolution art. The repair is run through `-executeMethod`; the `.meta` files are written by Unity rather than edited by hand.

### Inspector Setup

No scene or Inspector reference changes are required. Keep both Lily PNG files under `Assets/Resources/Characters/`.

### Play Mode Test

At 1920x1080, inspect Lily in Chapter 0 and the Final Chapter. Confirm the full sprite is crisp, fully visible, and not cropped; the blazer is navy blue, the lanyard is red, and both Mary Jane shoes remain readable. Open a Shell dialogue to confirm the existing portrait remains unchanged.
## M0-T49 Run 013 Lily Shoe Coverage and Dissertation Review Package

### LilyPixelFullBody.png

- Keeps every approved Run 007 character pixel above y=110 unchanged.
- Raises the black Mary Jane vamp by one pixel row and the thin strap by one row compared with Run 012.
- Leaves three short ankle rows and one narrow instep row visible, so the shoe covers more of the foot while keeping the first-version low-vamp shape.
- The difference from the Run 007 base remains inside x=39..62 and y=110..121: 180 changed pixels, zero changes above y=110, and hard 0/255 alpha.

### Dissertation Review Package

- `Docs/DISSERTATION_USER_BRIEF_CONSOLIDATED.md` records the user's literature, language, tool-comparison, evaluation, format, and grading requirements.
- `Docs/DISSERTATION_WORK_COMPLETED_SUMMARY.md` separates completed report work from open evidence and format tasks.
- `Docs/CLAUDE_REVIEW_PROMPT_DISSERTATION_FINAL_003.md` requires a source-checked literature review and a weighted KCL rubric mark.
- `Docs/dissertation_review_sources/` preserves the supplied cover, LaTeX template, chapter guidance, rubric extract, earlier report extract, macOS archive metadata, readable manifest, and SHA-256 list.

### Inspector Setup

No manual scene or Inspector changes are required. Keep the Lily PNGs under `Assets/Resources/Characters/`; the Editor import helper keeps them in Single sprite mode with point filtering.

### Play Mode Test

At 1920x1080, inspect Lily in Chapter 0 and the Final Chapter. Confirm that the Run 007 appearance is unchanged and only a small ankle and instep strip remains above each black shoe. The portrait must remain unchanged.

## M0-T49 Run 022: Guided Final Chapter and Later Ask Lily

### Final Chapter guided repair

- `Act6PipelineData.cs` defines six repair steps. Each step pairs one learned method with one
  concrete shortcut and provides the short question shown on the board.
- `Act6PipelineInteractionController.cs` stores the focused step and the six choices, advances after
  a choice, returns to the first broken step after a failed run, and builds a hint context from the
  current choice and test evidence.
- `Act6PipelineStaticPresenter.cs` replaces the twelve-card palette with a six-step progress row and
  two choice cards for the current step. The three test cards appear after all six choices are filled.
  Its Lily note includes a direct `Ask Lily` button.
- `Act6PipelineValidatorTests.cs` protects the guided candidate pairs. The presenter smoke test
  checks six progress controls, two visible choices, and the absence of the old palette.

The existing validator and three visitor cases remain the source of the result. The change reduces the
amount shown at one time without changing the correct six-chapter integration.

### Ask Lily in Chapters 4-6 and Final Chapter

- `Act4ConfidenceInteractionController.cs`, `Act5TestingInteractionController.cs`,
  `Act6BackendInteractionController.cs`, and `Act6PipelineInteractionController.cs` each build a
  short state summary from the current puzzle. Their presenters open Lily chat from the chapter note.
- `AmbientBanterPanel.cs` can now open chat even when a later chapter has no ambient banter panel.
- `LilyChatWindow.cs` keeps the chapter state and request trigger for each chat turn.
- `GhostBackendClient.cs` sends `stateSummary` and `trigger` with `POST /chat`.
- `BanterData.cs` provides local fallback replies for Chapter 4, Chapter 5, Chapter 6, and the Final
  Chapter.
- `LaterChapterHintContextTests.cs` checks that all four later contexts are present and do not expose
  internal answer identifiers.

The backend seeds the four later learning contexts and includes the current puzzle state in the
restricted prompt. If the backend or model is unavailable, the matching local chapter line is shown.

### Inspector setup

No manual Inspector wiring is required. The presenters create the new buttons and guided board at
runtime. Keep `Render On Start` enabled on each generated chapter presenter.

### Play Mode test

Open Chapters 4, 5, 6, and the Final Chapter and press `Ask Lily` after changing the puzzle state.
Confirm the chat opens and the reply remains related to the current chapter. In the Final Chapter,
confirm only two choices are visible for the focused step, all six steps can be revisited, the test
button remains unavailable until all six have a choice, and a failed run focuses the first broken
step.


## M0-T49 Run 023: Free-Form Final Chapter and Floating Lily

Run 023 supersedes the guided two-choice Final Chapter from Run 022.

### Final Chapter board

- `Act6PipelineData.cs` again supplies twelve cards for a free-form board: five learned skills, five
  shortcuts, and two backend actions. Player-facing labels and job lines are short and concrete.
- `Act6PipelineInteractionController.cs` again supports selection, drag/drop placement, stage swaps,
  the backend side socket, reset, and the three-case test suite. It also builds the current five-stage
  state for Ask Lily and sends a different Lily reaction for selection, placement, wrong-role
  placement, reset, failed tests, successful tests, and playback.
- `Act6PipelineStaticPresenter.cs` again renders the palette, five main slots, backend socket, and
  three visitor-test cards. Repeated card and slot explanations are shortened.
- `Act6PipelineStaticPresenterTests.cs` confirms the palette has twelve cards, both fixed endpoints,
  and three test cards, with no guided repair panel.
- `Act6PipelineValidatorTests.cs` keeps the deterministic route cases and checks every palette label
  and job line remains within a concise length.

### Floating Lily in Chapters 4-6 and Final Chapter

- `AmbientBanterHook.cs` maps the Chapter 4, Chapter 5, Chapter 6, and Final Chapter scenes to the
  same draggable floating panel used by Chapters 1-3.
- `AmbientBanterPanel.cs` keeps the latest state for each chapter, passes it into Ask Lily, and can
  display an immediate Lily reaction without opening chat. The existing small Lily/Ghost portrait
  remains part of the panel.
- `BanterData.cs` adds short rotating Lily and Ghost lines for the four later scenes.
- The Chapter 4, Chapter 5, Chapter 6, and Final presenters register their current controller state
  with the floating panel. Their temporary embedded Ask Lily buttons are removed.
- `LaterChapterHintContextTests.cs` checks the later scene mappings, banter content, state summaries,
  and a real Final Chapter selection reaction on the floating panel.

### Inspector setup

No manual Inspector setup is required. The floating panel, portrait, drag handle, and Ask Lily button
are created at runtime. Keep `Render On Start` enabled on the four chapter presenters.

### Play Mode test

Re-enter each later chapter after scripts compile. Confirm the same small portrait panel used in
Chapters 1-3 appears and can be dragged. In the Final Chapter, select and place several different
cards and confirm Lily's line changes without showing a correct/incorrect mark. Run one failing route,
compare the three visitor cards, open Ask Lily, then repair the route and complete the ending.

## M0-T50 Run 001: Local WebGL Installer

The release keeps the browser client and local services in one Windows installation. The player
installs `GhostSetup.exe` and opens Ghost from the Start menu or desktop shortcut. The launcher
starts the packaged Node.js service and Ollama process on loopback addresses, then opens
`http://localhost:3000` in the default browser. Progress and logs are written under the player's
local application-data directory instead of the installation directory.

`GhostWebGLReleaseBuilder` defines the nine release scenes explicitly and sends them to Unity's
WebGL build pipeline. This keeps the unused sample scene out of the release without rewriting the
project's Build Settings. `GHOST_WEBGL_OUTPUT` can redirect the build when it is called from
`build-release.ps1`.

The backend reads `GHOST_WEB_ROOT` when it starts. When that directory is present, Express serves
the Unity WebGL files and adds the content-encoding and content-type headers required by Unity's
Brotli-compressed data, JavaScript, and WebAssembly files. The existing REST routes remain on the
same local origin.

`GhostLauncher.exe` is a self-contained Windows launcher. It uses absolute paths inside the
installed package, so it does not depend on a system Node.js or Ollama installation. Ollama listens
on port 11435 with the packaged Granite model and cloud access disabled. Node.js listens on port
3000 and uses a SQLite file under `%LOCALAPPDATA%\Ghost\data`. Normal startup can continue with
static hints if the model runtime is unavailable. The `--self-test` path is stricter: it checks
the REST health route, WebGL page, Granite model discovery, and one model-generated Lily hint before
returning success.

`build-release.ps1` builds and tests the backend, builds WebGL, publishes the launcher, and stages
portable Node.js, the CPU/Vulkan Ollama runtime, Granite 3.1 Dense 2B, production dependencies, and
licence notices. `build-installer.ps1` uses Inno Setup to create one installer.
`test-clean-environment.ps1` runs the staged package with a restricted `PATH` and temporary user
data. `test-installer.ps1` additionally performs a silent install, runs the same self-test, runs
the uninstaller, and checks that the installed application directory is removed.

### Inspector setup

No Inspector setup is required. The release builder reads the existing generated scene files and
does not add serialized scene references.

### Play Mode and release test

This deployment path is checked separately from Unity Play Mode. For an Editor check, use
`Ghost > Build > WebGL Release`. For the distributable check, run the four commands in
`Deployment/README.md`, install `Build/Installer/GhostSetup.exe`, and confirm that Ghost opens in
the default browser. The recorded automated self-test requires the bundled model path to return an
LLM-backed hint; it does not treat a static fallback as a clean-environment pass.

## M0-T50 Run 002: Canonical D-drive build and Chapter 3 repair

Run 002 rebuilt the release from `D:\Code\Ghost`, which is the canonical repository. The release
script now waits for the Unity process to finish and checks its exit code before it looks for WebGL
output. This prevents a successful GUI Unity build from being reported as missing while it is still
running.

The first D-drive WebGL build reproduced five missing-script warnings on Chapter 3 palette items.
`Act3DialogGraphPaletteItemDragView` had been declared inside
`Act3DialogGraphNodeDragView.cs`. Unity could add that component at runtime, but it could not save a
normal script GUID for the second `MonoBehaviour`. Moving the palette component into
`Act3DialogGraphPaletteItemDragView.cs` gives both behaviours a file that matches the class name.
The five scene references now use that script's GUID. The rebuilt WebGL log contains no
missing-script warning.

The canonical release passed 147 EditMode tests, 10 backend route tests, the staged-package
self-test, the installed-package self-test, and a browser startup check. The installed self-test
uses a restricted `PATH` and verifies the WebGL page, REST backend, SQLite startup, packaged Granite
model discovery, and one model-backed hint. These results are recorded in
`Docs/codex_runs/M0-T50_002_sync_to_canonical_repo.md`.

### Inspector setup

No manual Inspector setup is required. The repaired Chapter 3 scene already refers to the separated
palette drag component, and the WebGL release builder reads the existing scene files.

### Play Mode and release test

Unity Editor Play Mode was not rerun during this deployment repair. The generated WebGL client was
opened in a fresh Edge profile, where the loading overlay cleared and the game canvas appeared. The
installer self-test then exercised the packaged services and local model without using a system
Node.js or Ollama installation. A separate physical clean Windows machine is still required for the
final external-machine check.
## M0-T51 Run 001: Global visual system and Windows desktop release

### Shared visual system

`GhostUITheme.cs` is now the sole runtime source for UI font selection, minimum text sizing, colour tokens, rounded sprites, and the standard `Panel`, `Card`, `Chip`, `DropZone`, `PushButton`, and `Label` factories. Small compatibility overloads let existing scene geometry and existing GameObjects use the same factories without local copies.

The following C# presentation scripts now consume the theme directly:

- `Act1IntentClassificationStaticPresenter.cs` uses themed piles, cards, labels, and controls; its serialized `intentGroupListRoot` field is preserved through `FormerlySerializedAs` while the code name is now `pileList`.
- `Act2EntityExtractionStaticPresenter.cs`, `Act3DialogGraphStaticPresenter.cs`, `Act4ConfidenceStaticPresenter.cs`, `Act5TestingStaticPresenter.cs`, `Act6BackendStaticPresenter.cs`, `Act6PipelineStaticPresenter.cs`, and `FinalChapterConversationPresenter.cs` use the shared text tokens and matching rounded surface factories without changing their controllers or validators.
- `Chapter0StoryPresenter.cs`, `AmbientBanterHook.cs`, `AmbientBanterPanel.cs`, `LilyChatWindow.cs`, `GhostFaceView.cs`, and `LilyDialogueFrame.cs` use the same typography and surfaces for story, companion, chat, and ghost UI.
- `Act2EntityTokenDragView.cs`, `Act6PipelinePartDragView.cs`, and `Act6BackendCardDragView.cs` use themed drag previews rather than local font fallbacks or bare rectangular previews.
- `Act1IntentClassificationPrototypeSceneBuilder.cs`, `Act3DialogGraphPrototypeSceneBuilder.cs`, and `GameShellSceneBuilder.cs` use `GhostUITheme` for the hierarchy they author. The other chapter builders only create a Canvas and presenter root, so they have no local UI factories to migrate.
- `ChatbotFundamentalsPresenter.cs` and `ShellReturnToHubOverlay.cs` route their remaining runtime-created gameplay buttons through `PushButton`, closing the global bare-button audit.

All chapter presenters now construct a 44 px header, a 40 px objective strip at `HeadingSize`, a 170 px conversation panel, and a main information/body region with a 96 px minimum and flexible height. Information panels use 12 px internal padding where they are constructed by the presenter.

Direct `Image` construction remains only for non-panel graphics: full-screen backdrops, Ghost/Lily/visitor portraits, confidence slider graphics, graph ports and wires, test-map ports and wires, and ending glow artwork. Rounded panels, cards, chips, buttons, and drop zones are created through `GhostUITheme`.

### Windows desktop delivery

`GhostWebGLReleaseBuilder.cs` now contains `GhostDesktopReleaseBuilder`. The legacy filename is retained so the existing Unity `.meta` file is not renamed; the class builds the same nine scenes to `Build\Windows\Ghost.exe` with `BuildTarget.StandaloneWindows64` and keeps the scene-existence and throw-on-failure checks.

`Deployment/Launcher/Program.cs` starts the packaged backend and Ollama services, validates `app\player\Ghost.exe`, and launches the native player. Its self-test checks the Windows player payload instead of requesting a WebGL page. `Backend/src/server.ts` now starts only the REST API; the retired WebGL static host and Brotli/MIME handling are gone.

`Deployment/build-release.ps1` stages the standalone player under `Build\Release\Ghost\app\player`. The Inno Setup definition uses the player executable for installed icons while shortcuts continue to start `GhostLauncher.exe`, ensuring services are ready before Unity opens. The installer test checks that the native player was installed.

### Inspector setup

No new manual Inspector references are required. Keep each generated chapter presenter's existing `Render On Start` setting. `GhostUITheme.cs.meta` is generated by Unity on import. Regenerate the shell and all eight chapter scenes through their existing `Ghost/Build...` menu commands after the repository's Act 4 model/controller compile mismatch is resolved.

### Play Mode and release test

At 1920x1080, open Chapter 1 and Chapters 2-6 plus the Final Chapter. Confirm the 44 px header and right-aligned progress, 40 px objective strip, three expanding information blocks, 170 px Ghost conversation, readable themed text, and rounded cards/buttons/drop zones. Exercise each chapter's existing drag, click, validation, reset, and Ask Lily paths to confirm presentation changes did not alter puzzle results.

For the native release, run `Deployment\build-release.ps1`, confirm `Build\Windows\Ghost.exe` and its data folder exist, then run the clean-environment and installer tests described in `Deployment\README.md`.
## M0-T51 Run 002: Unity verification and Act 1 builder repair

`Act1IntentClassificationPrototypeSceneBuilder.cs` now creates the intent-group template through the existing three-argument `GhostUITheme.DropZone` overload, then reapplies the template's original anchors and zero offsets to the returned `RectTransform`. This is a compile-only repair for the Run 001 theme migration; it does not change intent grouping, validation, authored data, or runtime puzzle behaviour.

All nine scene builders were rerun after the repair. The regenerated gameplay hierarchy keeps the M0-T51 authored proportions: 44 px header, 40 px objective strip, information blocks with a 96 px minimum and flexible height, and a 170 px Ghost conversation panel. Flexible information blocks may render taller when the vertical layout distributes spare space.

### Inspector setup

No manual Inspector setup is required. The nine generated scenes already contain the rebuilt presenter hierarchies and existing serialized references.

### Play Mode and release test

Open Chapters 1 and 4 at 1920x1080 and confirm the full hierarchy remains visible with no overlap: header and progress, objective strip, flexible teaching/information blocks, conversation panel, and lower controls. The Run 002 automated evidence is recorded in `Docs/codex_runs/M0-T51_002_unity_verification.md`; interactive drag, click, and completion flows remain a separate human Play Mode check.
## M0-T51 Run 003: layout repair

Run 003 keeps the M0-T51 theme tokens and deterministic gameplay unchanged while repairing the
containers exposed by the larger type scale. Shell buttons now use their label's preferred width and
have a 44px minimum height; chapter cards reserve separate title, description, and button space.
Chapter headers reserve the 220px Return-to-Hub overlay area, keep the title flexible, and give progress
indicators fixed widths. Objective strips explicitly use zero flexible height.

Act 4 gives the visitor-queue title and guide their own fixed rows. Act 5 enlarges node cards and their
text rows; ports remain anchored to card edges and wires still use transformed port centres, so no port
math changed. The Final Chapter separates Ghost's status region from the face and lets the no-backend
label fill its row. Chapter 0 moves its progress and skip controls left of the global overlay.