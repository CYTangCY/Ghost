# CURRENT_TASK.md

## ID

M0-T09

## Goal

Improve Act 1 assignment editing and group display capacity, then add basic validation feedback,
without drag-and-drop.

## Context

M0-T04 implemented the pure validator. M0-T05 added sample data. M0-T06 added session/state logic.
M0-T07 created the static UI scene. M0-T08 added click-to-select and click-to-assign interaction.
The next step is to make assigned cards editable and make the right-side group areas handle many
assigned cards visibly before adding more complex drag-and-drop.

## Scope

- Allow assigned cards in the right-side group areas to be removed or moved back to unassigned.
- Allow the player to correct mistakes without restarting the scene.
- Fix the right-side group display so assigning many cards to one group does not make cards
  disappear.
- Use a simple solution such as ScrollRect, compact card rows/chips, or an explicit overflow
  indicator.
- Add a basic Validate button.
- Use the existing IntentClassificationSession and IntentClassificationValidator.
- Show simple validation feedback:
  - correct grouping
  - incorrect grouping
  - optionally show number of issues or a generic hint
- Keep the interaction click-based.
- Keep this simpler than drag-and-drop.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md.
- Create a Codex run log during implementation.

## Out of Scope

- Do not implement drag-and-drop yet.
- Do not implement scoring.
- Do not implement final art.
- Do not implement save/load.
- Do not implement animations beyond basic UI state changes.
- Do not implement Act 0, Act 2, or later Acts.
- Do not implement backend, LLM, or dialogue system.
- Do not add the scene to Build Settings unless explicitly approved.
- Do not change ProjectSettings unless explicitly required and approved.

## Acceptance Criteria

- Assigned cards can be removed from a group or moved back to unassigned.
- The player can fix a wrong assignment without restarting Play Mode.
- Assigning all cards to one group does not make assigned cards silently disappear.
- The group display uses scrolling, compact display, or another clear capacity solution.
- A Validate button checks the current assignment using the existing validator.
- The UI displays clear basic validation feedback.
- The scene enters Play Mode without Console errors.
- No drag-and-drop is implemented yet.
- A Codex run log is created during implementation.
