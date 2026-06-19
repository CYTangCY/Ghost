# CURRENT_TASK.md

## ID

M0-T11

## Goal

Refactor the Act 1 presentation layer before drag-and-drop, without changing visible behaviour.

## Context

M0-T04 implemented the pure validator. M0-T05 added sample data. M0-T06 added session/state.
M0-T07 created the static UI. M0-T08 added click-to-assign. M0-T09 added assignment editing and
validation feedback. M0-T10 reviewed the architecture and found that the pure logic layer is clean,
but the presentation layer is becoming too large, especially Act1IntentClassificationStaticPresenter.

## Scope

- Add a presentation assembly definition if appropriate:
  - Ghost.Presentation.asmdef
  - Ghost.Presentation.Editor.asmdef if needed
- Extract interaction/state orchestration from Act1IntentClassificationStaticPresenter into a small
  Act1IntentClassificationInteractionController.
- The controller should own or coordinate:
  - IntentClassificationSession
  - selected card ID
  - assign / unassign
  - validate
  - state-changed notification or equivalent simple callback
- Keep the presenter focused on rendering and visual updates.
- Keep visible UI behaviour unchanged:
  - click card to select
  - click group to assign
  - assigned card can return to unassigned
  - Validate button still works
  - feedback still works
- Keep drag-and-drop out of scope.
- Keep pure logic files unchanged.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md.
- Create a Codex run log during implementation.

## Out of Scope

- Do not implement drag-and-drop.
- Do not change the puzzle rules.
- Do not change sample data.
- Do not edit pure logic files unless absolutely necessary.
- Do not implement new UI features.
- Do not implement scoring.
- Do not implement final art.
- Do not implement Act 0, Act 2, backend, LLM, dialogue, save/load, or animation.
- Do not edit ProjectSettings unless explicitly required and approved.
- Do not add the scene to Build Settings.

## Acceptance Criteria

- Visible behaviour is unchanged from M0-T09.
- The presenter is smaller and more focused on rendering/visual state.
- Interaction/session orchestration is moved to a separate controller or equivalent small class.
- Pure logic files remain unchanged.
- EditMode tests still pass.
- The Act 1 scene enters Play Mode without Console errors.
- Click-to-select, assign, unassign, and Validate still work.
- No drag-and-drop is implemented.
- A Codex run log is created during implementation.
