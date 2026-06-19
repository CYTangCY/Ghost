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
