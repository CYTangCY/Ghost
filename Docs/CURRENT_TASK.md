# CURRENT_TASK.md

## ID

M0-T12

## Goal

Add minimal drag-to-assign interaction for the Act 1 prototype, reusing the existing
session/controller flow.

## Context

M0-T04 implemented the pure validator. M0-T05 added sample data. M0-T06 added session/state logic.
M0-T07 created the static UI scene. M0-T08 added click-to-assign. M0-T09 added assignment editing
and validation feedback. M0-T10 reviewed the architecture. M0-T11 refactored presentation logic into
a smaller interaction controller. The next step is the smallest safe drag-and-drop layer.

## Scope

- Add drag interaction for message cards.
- Add drop handling for the three intent group areas.
- On successful drop, assign the card through the existing interaction controller/session flow.
- Keep click-to-assign as a fallback unless removing it is clearly safer.
- Keep Validate button and feedback behaviour unchanged.
- Keep assignment editing / Back behaviour unchanged.
- Keep pure logic unchanged.
- Keep the UI simple and placeholder-based.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md.
- Create a Codex run log during implementation.

## Out of Scope

- Do not implement scoring.
- Do not implement animations beyond minimal drag visual feedback if necessary.
- Do not implement reorder within groups.
- Do not implement mobile/touch polish yet.
- Do not implement final art.
- Do not implement Act 0, Act 2, backend, LLM, dialogue, save/load, or narrative UI.
- Do not add the scene to Build Settings unless explicitly approved.
- Do not change ProjectSettings unless explicitly required and approved.

## Acceptance Criteria

- A message card can be dragged onto an intent group area.
- Dropping a card onto a group assigns it through the existing controller/session path.
- The UI visibly reflects the assignment.
- Existing click-to-assign still works, unless intentionally removed and documented.
- Validate button still works.
- Back/unassign still works.
- The scene enters Play Mode without Console errors.
- Pure logic files are unchanged.
- A Codex run log is created during implementation.
