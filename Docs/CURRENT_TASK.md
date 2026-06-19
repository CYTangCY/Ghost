# CURRENT_TASK.md

## ID

M0-T06

## Goal

Implement the Act 1 intent-classification puzzle session state as pure C# logic, without creating
UI or scenes.

## Context

M0-T04 implemented the pure intent-classification validator. M0-T05 added Act 1 sample puzzle data.
The next step is to create a small scene-free session/state layer that represents the player's
current grouping before later UI work.

## Scope

- Create a pure C# session/state class for Act 1 intent classification.
- It should initialize from Act1IntentClassificationSampleData or a list of IntentCard objects.
- It should track which card IDs are currently assigned to which player group.
- It should support moving a card between groups.
- It should support unassigned cards.
- It should expose the current grouping in the format needed by IntentClassificationValidator.
- It should allow validating the current state using IntentClassificationValidator.
- Add EditMode tests for initialization, moving cards, unassigned state, incorrect grouping, and
  validating a correct grouping.
- Keep everything scene-free, UI-free, and WebGL-safe.
- Create a Codex run log during implementation.

## Out of Scope

- Do not create scenes.
- Do not create drag-and-drop UI.
- Do not create prefabs.
- Do not create art assets.
- Do not modify ProjectSettings.
- Do not implement Act 0, Act 2, or later Acts.
- Do not implement backend, LLM, save system, or dialogue system.

## Acceptance Criteria

- A pure C# Act 1 session/state layer exists.
- The state can represent unassigned and grouped cards.
- Moving cards between groups is testable.
- The current grouping can be passed into IntentClassificationValidator.
- EditMode tests cover the core state behavior.
- No Unity scene or ProjectSettings files are modified.
- A Codex run log is created during implementation.
