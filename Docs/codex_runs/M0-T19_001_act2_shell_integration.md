# M0-T19 — Run 001 — Act 2 shell integration

## Task ID

M0-T19

## Run Number

001

## Date

2026-06-22

## Original Request / Codex Prompt Summary

Implement only the Act 2 Game Shell integration: add Act 2 scene constants, add a Start Act 2 hub button and presenter route, make the runtime Return-to-Hub overlay appear in Act 2 as well as Act 1, update the shell builder so it generates the Act 2 hub card and registers shell + Act 1 + Act 2 in Build Settings, optionally update the hub Lily line, and update docs. Do not change Act 2 puzzle logic/UI or Act 1 mechanics.

## Files Created

- `Docs/codex_runs/M0-T19_001_act2_shell_integration.md`

## Files Modified

- `Assets/Presentation/Shell/ShellSceneNames.cs`
- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
- `Assets/Presentation/Shell/ShellDialogueData.cs`
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `git diff --check -- Assets/Presentation/Shell/ShellSceneNames.cs Assets/Presentation/Shell/GameShellPresenter.cs Assets/Presentation/Shell/ShellReturnToHubOverlay.cs Assets/Presentation/Shell/ShellDialogueData.cs Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `rg "Act2SceneName|Act2ScenePath|StartAct2|act2Button|Start Act 2|Act 2" Assets/Presentation/Shell`
- `git diff --name-only -- Assets/Scripts/Puzzles/EntityExtraction Assets/Presentation/Act2EntityExtraction Assets/Presentation/Act1IntentClassification`
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Scene generation: Not run — Unity Editor menu builder was not executed in this Codex session.
- Build Settings registration: Not run — Build Settings registration was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- M0-T19 edited files had no whitespace errors; Git emitted CRLF normalization warnings only.
- Act 2 shell constants/routes/button references were found in the expected shell files.
- No diffs appeared under Act 2 puzzle logic/UI folders or Act 1 presentation mechanics.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Scene generation: Not run — Unity Editor menu builder was not executed in this Codex session.
- Build Settings registration: Not run — Build Settings registration was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

None during this Codex run.

## Fixes Applied

- Added `Act2SceneName` and `Act2ScenePath` to `ShellSceneNames`.
- Added an `act2Button` field and `StartAct2()` route to `GameShellPresenter`.
- Generalized `ShellReturnToHubOverlay` so the return button appears in Act 1 and Act 2.
- Updated `ShellDialogueData` so Lily's hub line acknowledges both Act 1 and Act 2.
- Updated `GameShellSceneBuilder` to create an Act 2 hub card, pass the Act 2 button into the presenter, and register Act 2 in Build Settings when the user runs the builder.
- Updated CODE_WALKTHROUGH and UNITY_TEST_CHECKLIST for M0-T19.

## What Was Intentionally Not Changed

- No Act 2 puzzle logic files were modified.
- No Act 2 presenter/controller files were modified.
- No Act 1 mechanics were modified.
- No existing scenes, ProjectSettings, Packages, asmdefs, `.meta` files, node graph, backend, LLM, save/load, scoring persistence, full visual-novel dialogue, or final art were modified by this task.
- Existing unrelated dirty scene and ProjectSettings files in the worktree were not reverted or edited.

## Remaining Risks

- The shell scene and Build Settings still need manual Unity builder execution because scene generation and Build Settings registration were not run in this Codex shell session.
- Existing dirty `Assets/Scenes/Act1IntentClassificationPrototype.unity`, `Assets/Scenes/GameShellPrototype.unity`, and ProjectSettings files were present in the worktree and should be reviewed separately before commit.
- The generated hub layout needs Play Mode visual verification after `Ghost > Build Game Shell Scene`, especially the two act cards plus future placeholder in the available screen space.

## Next Recommended Step

Open Unity, let scripts compile, run `Ghost > Build Game Shell Scene` to regenerate the shell and apply Build Settings, then verify: hub shows Start Act 1 and Start Act 2, Start Act 2 loads Act 2, Return to Hub works from Act 2, Act 1 still launches/returns, Build Settings contains shell + Act 1 + Act 2, Act 2 puzzle behaviour is unchanged, and no Console errors appear.
