# Act 1 Intent Classification Prototype Setup

M0-T07 could not create the scene from Codex because Unity batch mode exited before project import or scene generation.

Use this manual Unity Editor setup instead:

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile the new presentation scripts.
3. In the Unity menu, select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
4. Confirm Unity creates `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
5. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
6. Confirm the scene shows:
   - the Act 1 title and purpose reminder
   - nine static sample message cards
   - three intent group areas: `find_item`, `ask_location`, and `ask_identity`
7. Enter Play Mode and confirm the Console has no new errors.

Do not add this scene to Build Settings during M0-T07.

## Refreshing after the M0-T07 run 002 display fix

If the generated scene still shows blank message cards, rebuild it from the Unity menu:

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile the updated presentation scripts.
3. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
4. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
5. Confirm all nine message card texts are visible in the left column.
6. Confirm the three intent group areas still show `find_item`, `ask_location`, and `ask_identity`.

The display fix keeps the UI static. It does not add drag-and-drop, validation, scoring, save/load, backend, LLM, or dialogue behaviour.

## Refreshing after the M0-T08 click-to-assign interaction

The prototype now supports a simple click flow:

1. Click a message card to select it.
2. Confirm the selected card changes to the yellow selected highlight.
3. Click `find_item`, `ask_location`, or `ask_identity` to assign the selected card to that intent group.
4. Confirm the assigned card text appears inside the clicked intent group area.
5. Confirm assigned cards change to the green assigned highlight unless they are currently selected.

If the scene does not respond to clicks after Unity imports the updated scripts, rebuild it from the Unity menu:

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile the updated presentation scripts.
3. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
4. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
5. Enter Play Mode and repeat the click-to-assign check above.

M0-T08 keeps the UI placeholder-based. It does not add drag-and-drop, validation, scoring, save/load, animation, backend, LLM, dialogue behaviour, final art, or Build Settings changes.

## Refreshing after the M0-T08 run 002 UI fix

Run 002 fixes two prototype UI behaviours:

1. Clicking the selected card again now deselects it.
2. Assigning a selected card now clears the selection automatically.
3. Assigned-card rows in each intent group use a compact clipped layout so text stays inside the group panel.

If the visible scene still has older layout behaviour after Unity imports the updated scripts, refresh it from the Unity menu:

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile the updated presentation scripts.
3. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
4. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
5. Enter Play Mode.
6. Confirm selecting the same card twice clears the highlight.
7. Confirm assignment clears the selected-card highlight.
8. Confirm assigned message rows stay visually inside the right-side group areas.

Run 002 still does not add drag-and-drop, validation, scoring, save/load, animation, backend, LLM, dialogue behaviour, final art, or Build Settings changes.

## Refreshing after M0-T09 assignment editing and validation feedback

M0-T09 keeps the prototype click-based and adds:

1. Scrollable assigned-card lists in each intent group.
2. Clickable assigned-card rows labelled `Back:` for returning a card to unassigned.
3. A `Validate` button under the intent group column.
4. Basic validation feedback for correct and incorrect groupings.

To refresh the scene if needed:

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile the updated presentation scripts.
3. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
4. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
5. Enter Play Mode.
6. Assign several or all cards to one intent group and confirm the assigned list can be scrolled.
7. Click an assigned `Back:` row and confirm that card returns to the unassigned state in the left list.
8. Correct a wrong assignment by selecting the card again and clicking a different intent group.
9. Click `Validate` and confirm feedback says whether the grouping is correct or incorrect.

M0-T09 still does not add drag-and-drop, scoring, save/load, animation, backend, LLM, dialogue behaviour, final art, or Build Settings changes.

## Refreshing after M0-T12 minimal drag-to-assign

M0-T12 keeps the existing click flow and adds a minimal drag flow:

1. Drag a message card from the left column.
2. Confirm one solid card-like preview follows the pointer.
3. Drop the card anywhere inside `find_item`, `ask_location`, or `ask_identity`.
4. Confirm the card is assigned through the same session/controller flow and appears as a compact `Back:` row in the dropped group's assigned-card list.
5. Drag an assigned `Back:` row to the left message-card list and confirm it returns to unassigned.
6. Drag an assigned `Back:` row to a different intent group and confirm it reassigns through the normal assigned-card list.
7. Drop a card outside valid targets and confirm the UI returns to normal without changing assignment state.
8. After successful and cancelled drops, confirm no stale `Drag Preview` objects remain in the Hierarchy.
9. Confirm click-to-assign, `Back:` unassign, compact scrollable group lists, and `Validate` feedback still work.

The presenter attaches the drag and drop behaviours when it renders sample data, so scene regeneration should not be required after script import. If Unity shows stale serialized UI or missing script references, refresh the scene from the Unity menu:

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile the updated presentation scripts.
3. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
4. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
5. Enter Play Mode and repeat the drag-to-assign and drag-back-to-unassigned checks above.

M0-T12 still does not add scoring, save/load, animation beyond minimal drag feedback, backend, LLM, dialogue behaviour, final art, free placement, group reordering, mobile/touch polish, Build Settings changes, or ProjectSettings changes.

## Refreshing after visual and instruction polish

The presentation polish keeps all M0-T12 mechanics and improves readability:

1. The top instructions explain that messages should be grouped by speaker intent, not exact wording.
2. The instructions mention click assignment, drag assignment, dragging assigned cards back/between groups, and Validate.
3. The left column is labelled `Unassigned Messages`.
4. The right column is labelled `Intent Groups`.
5. Selected cards, assigned cards, valid drop areas, compact assigned rows, and validation feedback have clearer colour/outline cues.

Scene regeneration should not be required because the presenter applies this polish when it renders sample data. If the scene appears stale after script import:

1. Open the Ghost Unity project.
2. Wait for Unity to import and compile the updated presentation scripts.
3. Select `Ghost > Build Act 1 Intent Classification Prototype Scene`.
4. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity`.
5. Enter Play Mode and confirm the polished instructions and visual cues are visible.

This polish does not change puzzle rules, sample data, scoring, save/load, backend, LLM, dialogue behaviour, final art, free placement, group reordering, Build Settings, or ProjectSettings.
