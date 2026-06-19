# CURRENT_TASK.md

## ID

M0-T05

## Goal

Define Act 1 sample puzzle data structure and sample content for the intent-classification puzzle,
without creating UI or scenes.

## Context

M0-T04 implemented the pure Act 1 intent-classification validator and EditMode tests. The next
step is to define a small, reusable Act 1 puzzle data layer that later UI can render and pass into
the validator.

## Scope

- Define a small Act 1 puzzle data structure if the existing M0-T04 model is not enough.
- Add sample Act 1 message cards grouped by intent / purpose.
- Ensure sample data can be validated by the existing IntentClassificationValidator.
- Add or update EditMode tests for the sample data.
- Keep this scene-free and UI-free.
- Create a Codex run log when implementation happens.

## Out of Scope

- Do not create scenes.
- Do not create drag-and-drop UI.
- Do not create prefabs.
- Do not create art assets.
- Do not modify ProjectSettings.
- Do not implement Act 0, Act 2, or later Acts.
- Do not implement backend, LLM, save system, or dialogue system.

## Acceptance Criteria

- Act 1 sample puzzle data exists in a small, testable form.
- The sample data includes multiple messages that share the same intent despite different wording.
- The sample data includes at least two or three intent groups.
- EditMode tests verify the sample data can be validated correctly.
- No Unity scene or ProjectSettings files are modified.
- A Codex run log is created during implementation.
