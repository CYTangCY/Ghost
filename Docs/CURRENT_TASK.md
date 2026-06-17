# CURRENT_TASK.md

## ID

M0-T04

## Goal

Implement the Act 1 intent-classification core validator as pure C# logic with EditMode tests.

## Context

M0-T03 found a clean Unity 6 Universal 2D / URP project with the new Input System active,
SampleScene only, no existing scripts, no game scenes, and the Unity Test Framework installed.
The first implementation task should be scene-free, input-free, and testable.

## Scope

- Create a small runtime code structure for Act 1 intent classification.
- Implement pure C# validation logic for grouping message cards by intent / purpose.
- Add EditMode tests for correct grouping and incorrect grouping.
- Add runtime and test asmdefs if appropriate.
- Create a Codex run log for the implementation run when Codex performs it.

## Out of Scope

- Do not create scenes.
- Do not create UI.
- Do not implement drag-and-drop yet.
- Do not modify ProjectSettings.
- Do not edit .meta files manually.
- Do not implement Act 0, Act 2, or later Acts.
- Do not add LLM / backend integration.
- Do not create art assets.

## Acceptance Criteria

- Intent-classification validation logic exists as pure C#.
- EditMode tests cover at least one correct grouping and at least one incorrect grouping.
- Code is scene-free and WebGL-safe.
- No Unity scene or ProjectSettings files are modified.
- A Codex run log is created under Docs/codex_runs/ after the implementation run.
- CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md are updated only when implementation actually
  happens, not during this planning closure.
