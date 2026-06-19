# CURRENT_TASK.md

## ID

M0-T08

## Goal

Implement basic click-to-assign interaction for the Act 1 intent-classification prototype, without
drag-and-drop.

## Context

M0-T04 implemented the pure validator. M0-T05 added sample data. M0-T06 added session/state logic.
M0-T07 created a static UI scene showing the sample cards and intent group areas. The next step is
to add a simple low-risk interaction before full drag-and-drop: select a card, then click an intent
group to assign it.

## Scope

- Add simple click-to-select behaviour for message cards.
- Add click-to-assign behaviour for intent group areas.
- Update the visual state so assigned cards visibly move or are duplicated into the selected group
  area.
- Use the existing IntentClassificationSession where possible.
- Keep the existing static scene as the target scene.
- Keep this simpler than drag-and-drop.
- Add or update Play Mode/manual test checklist steps.
- Create a Codex run log during implementation.

## Out of Scope

- Do not implement drag-and-drop yet.
- Do not implement scoring.
- Do not implement final art.
- Do not implement save/load.
- Do not implement Act 0, Act 2, or later Acts.
- Do not implement backend, LLM, or dialogue system.
- Do not add the scene to Build Settings unless explicitly approved.
- Do not change ProjectSettings unless explicitly required and approved.

## Acceptance Criteria

- The user can click a message card to select it.
- The user can click an intent group area to assign the selected card.
- The UI visibly reflects the assignment.
- The interaction uses or updates the existing session/state layer.
- The scene enters Play Mode without Console errors.
- No drag-and-drop is implemented yet.
- A Codex run log is created during implementation.
