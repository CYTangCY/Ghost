# CURRENT_TASK.md

## ID

M0-T10

## Goal

Perform a light UI/code architecture review of the Act 1 prototype before adding drag-and-drop.

## Context

M0-T04 implemented the pure validator. M0-T05 added sample data. M0-T06 added session/state.
M0-T07 created the static UI scene. M0-T08 added click-to-assign. M0-T09 added assignment editing,
group display capacity handling, and validation feedback. Before moving to drag-and-drop, review
whether the current structure is still clean and whether UI code is properly separated from puzzle
logic.

## Scope

- Review the current Act 1 code and scene structure.
- Check separation between pure logic and Unity presentation code.
- Check whether the presenter is becoming too large.
- Check whether session state, validation, and UI rendering responsibilities are cleanly separated.
- Check whether the scene-builder workflow is still safe.
- Check whether the current UI is safe to extend into drag-and-drop.
- Identify small refactors if needed before drag-and-drop.
- Do not implement code.

## Out of Scope

- Do not implement drag-and-drop.
- Do not edit Unity scenes.
- Do not edit scripts.
- Do not edit ProjectSettings.
- Do not add features.
- Do not rewrite architecture documents unless explicitly approved later.

## Acceptance Criteria

- A review report is produced.
- The report lists architecture strengths.
- The report lists concrete risks.
- The report recommends whether to proceed to drag-and-drop or do a small refactor first.
- No Unity files are modified.
