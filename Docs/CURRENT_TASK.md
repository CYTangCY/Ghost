# CURRENT_TASK.md

## ID

M0-T07

## Goal

Create a static Act 1 intent-classification UI prototype scene, without drag-and-drop interaction
yet.

## Context

M0-T04 implemented the pure intent-classification validator. M0-T05 added Act 1 sample puzzle data.
M0-T06 added the pure session/state layer. The next step is to create a minimal static Unity UI
layer that can display the sample cards and intent groups, but does not yet implement drag-and-drop.

## Scope

- Create or prepare a dedicated Act 1 prototype scene.
- Display the Act 1 sample message cards as static UI elements.
- Display the target intent groups / drop zones as static UI elements.
- Add a simple presenter/view script only if needed to render the sample data.
- Keep the interaction non-draggable for this task.
- Use UGUI or another Unity UI approach consistent with the project setup.
- Keep the UI simple and placeholder-based.
- Do not implement validation button behaviour yet unless it is trivial and scene-safe.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md.
- Create a Codex run log during implementation.

## Out of Scope

- Do not implement drag-and-drop.
- Do not implement scoring.
- Do not implement animations.
- Do not implement save/load.
- Do not implement Act 0, Act 2, or later Acts.
- Do not implement backend, LLM, or dialogue system.
- Do not create final art assets.
- Do not change ProjectSettings unless explicitly required and approved.

## Acceptance Criteria

- A static Act 1 prototype scene exists.
- The scene displays the sample message cards.
- The scene displays the intent group areas.
- The scene can enter Play Mode without Console errors.
- No drag-and-drop interaction is implemented yet.
- No unrelated ProjectSettings, Packages, or scene files are modified.
- A Codex run log is created during implementation.
