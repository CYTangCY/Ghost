# CURRENT_TASK.md

## ID

M0-T13

## Goal

Polish the Act 1 prototype's visual hierarchy and player-facing instructional clarity, without
changing core mechanics.

## Context

M0-T04 implemented the pure validator. M0-T05 added sample data. M0-T06 added session/state logic.
M0-T07 created the static UI. M0-T08 added click-to-assign. M0-T09 added assignment editing and
validation feedback. M0-T10 reviewed architecture. M0-T11 refactored presentation logic. M0-T12
added drag-to-assign, bidirectional drag, group-wide drop zones, drag visual cleanup, and compact
assigned rows. The next step is small visual/instruction polish before moving to the next Act.

## Scope

- Improve the player-facing instructions for Act 1.
- Make the card/group visual hierarchy clearer and cuter while staying placeholder-based.
- Keep card rows compact and readable.
- Improve feedback text wording if needed.
- Make selected, assigned, dragging, and valid drop-area states easier to understand.
- Keep all current mechanics unchanged:
  - click-to-assign
  - drag-to-assign
  - drag back to unassigned
  - drag between groups
  - Back/unassign
  - Validate
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md.
- Create a Codex run log during implementation.

## Out of Scope

- Do not change puzzle rules.
- Do not change sample data.
- Do not implement scoring.
- Do not implement Act 0 or Act 2.
- Do not implement final art.
- Do not implement backend, LLM, dialogue, save/load, or narrative UI.
- Do not add Build Settings.
- Do not edit ProjectSettings unless explicitly required and approved.
- Do not refactor architecture unless a small UI polish requires it.

## Acceptance Criteria

- The Act 1 UI is clearer to a new player.
- Instructions explain drag/click assignment and validation in simple language.
- Visual states are easier to understand.
- Existing mechanics continue to work.
- Scene enters Play Mode without Console errors.
- Pure logic files remain unchanged.
- A Codex run log is created during implementation.
