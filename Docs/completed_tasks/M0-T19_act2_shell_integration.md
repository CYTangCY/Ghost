# M0-T19 — Act 2 Game Shell Integration

## Completion Status

Completed. Codex run 001 wired Act 2 into the Game Shell; the user ran `Ghost > Build Game Shell
Scene`, verified the flow in the Editor, and reported "完成". Claude reviewed the diffs, the Build
Settings change, docs, and run log.

## Date

2026-06-22

## Summary

Made Act 2 reachable from the shell, mirroring Act 1. `ShellSceneNames` gained the Act 2 scene
name/path; `GameShellPresenter` gained an `act2Button` + `StartAct2()`; `ShellReturnToHubOverlay` now
shows the runtime Return-to-Hub button for Act 1 or Act 2; `ShellDialogueData`'s hub Lily line
acknowledges both acts; and `GameShellSceneBuilder` (refactored to a shared `CreateActCard`) builds the
Act 2 hub card and registers shell + Act 1 + Act 2 in Build Settings. Act 2's puzzle and Act 1 are
unchanged. This completes ROADMAP Phase B (Act 2).

## Files Modified

- `Assets/Presentation/Shell/ShellSceneNames.cs`
- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
- `Assets/Presentation/Shell/ShellDialogueData.cs`
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`
- `Assets/Scenes/GameShellPrototype.unity` (regenerated via the builder)
- `ProjectSettings/EditorBuildSettings.asset` (approved exception: registers the Act 2 scene alongside
  shell + Act 1)

## Files Created

- `Docs/codex_runs/M0-T19_001_act2_shell_integration.md`

## Claude Review Notes

- Scope clean: only the 5 shell files + regenerated shell scene + `EditorBuildSettings` + 2 docs + run
  log. Act 2 puzzle logic/UI and Act 1 mechanics untouched (confirmed via git status + scoped diff).
- `EditorBuildSettings` diff adds only the Act 2 scene entry (enabled) — the approved exception.
- `StartAct2()` mirrors `StartAct1()` (SceneManager); overlay limited to Act 1/Act 2; builder
  registration is idempotent (skips existing shell/Act1/Act2 paths).
- Run log honest: Play Mode / scene generation / Build Settings registration / tests all "Not run" in
  the Codex session.

## Human Verification Result

The user ran `Ghost > Build Game Shell Scene` and verified in the Editor: hub shows Start Act 1 + Start
Act 2; Lily hub line mentions both; Start Act 2 loads Act 2; Act 2 puzzle works; Return to Hub works
from Act 2; Act 1 still launches/returns; Build Settings has shell + Act 1 + Act 2; no Console errors.
Reported "完成". Source of the "works" status.

## Remaining Risks

- Manual-in-Editor verification only; shell scene is a builder artifact (rebuild if stale).
- `ProjectSettings/ShaderGraphSettings.asset` and `ProjectSettings/ProjectSettings.asset` remain
  unwanted working-tree side-effects to keep OUT of commits; the Act 1 scene also remains a
  pre-existing dirty artifact. Only `EditorBuildSettings.asset` should be committed from ProjectSettings.

## Next Task

M0-T20 — Act 3 (Dialog Management via Node Graph, the flagship mechanic): design + learning-content
mapping BEFORE implementation, per CONFIRMED_PROJECT_CONTEXT §17. Planning task (Claude-led, no Unity
code).
