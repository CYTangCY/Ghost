# M0-T31 — Run 001 — Act 3 shell integration

## Task ID
M0-T31

## Run Number
001

## Date
2026-06-23

## Original Request / Codex Prompt Summary
Integrate the existing Act 3 dialog graph prototype scene into the Game Shell so the hub can launch Act 3 and Act 3 can return to the hub, mirroring the earlier Act 2 shell integration. Do not modify Act 3 puzzle logic/UI, Act 1/Act 2 mechanics, unrelated scenes, ProjectSettings, Packages, or `.meta` files.

## Files Created
- `Docs/codex_runs/M0-T31_001_act3_shell_integration.md`

## Files Modified
- `Assets/Presentation/Shell/ShellSceneNames.cs`
- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Assets/Presentation/Shell/ShellDialogueData.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run
- `rg "Act 3|Act3|StartAct3|act3Button|Act3Scene" Assets/Presentation/Shell Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `rg "Configure\\(" Assets/Presentation/Shell -n`
- `git diff --check -- Assets/Presentation/Shell/ShellSceneNames.cs Assets/Presentation/Shell/GameShellPresenter.cs Assets/Presentation/Shell/ShellReturnToHubOverlay.cs Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs Assets/Presentation/Shell/ShellDialogueData.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `git diff --stat -- Assets/Presentation/Shell/ShellSceneNames.cs Assets/Presentation/Shell/GameShellPresenter.cs Assets/Presentation/Shell/ShellReturnToHubOverlay.cs Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs Assets/Presentation/Shell/ShellDialogueData.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`

## Test / Check Result
- Shell/doc search confirmed Act 3 scene constants, presenter wiring, return overlay handling, builder Build Settings registration, and documentation checklist entries are present.
- `Configure(...)` search confirmed the only `GameShellPresenter.Configure(...)` caller is the shell scene builder and it now passes the Act 3 button.
- `git diff --check` completed without whitespace errors; Git reported line-ending normalization warnings only.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.
- Scene generation: Not run — Unity Editor menu builder was not executed in this Codex session.
- Build Settings: Not run — Unity Editor Build Settings were not modified in this Codex session.

## Errors Encountered
- No code errors encountered during this run.
- Git emitted line-ending normalization warnings for the touched files during diff checks.

## Fixes Applied
- Added Act 3 scene name/path constants to shell scene names.
- Added `act3Button` and `StartAct3()` to the shell presenter and wired them through `Configure(...)` and `Start()`.
- Added an Act 3 hub card in the shell scene builder using the existing `CreateActCard(...)` helper.
- Extended shell Build Settings registration logic so the builder registers shell + Act 1 + Act 2 + Act 3 when the Unity menu item is run.
- Extended the return-to-hub overlay scene check so Act 3 also receives the runtime `Return to Hub` button.
- Updated the hub Lily line to acknowledge Act 3.
- Updated walkthrough and Unity checklist documentation for M0-T31.

## What Was Intentionally Not Changed
- Act 3 puzzle logic/session/UI files under `Assets/Scripts/Puzzles/DialogGraph/` and `Assets/Presentation/Act3DialogGraph/`.
- Act 1 and Act 2 puzzle mechanics.
- Existing `.unity` scene YAML.
- `ProjectSettings`, `Packages`, Build Settings asset, and `.meta` files.
- Backend, LLM, save/load, full visual-novel dialogue, and final art.

## Remaining Risks
- The user must run `Ghost > Build Game Shell Scene` in Unity to regenerate `Assets/Scenes/GameShellPrototype.unity` and apply the Build Settings registration.
- Act 3 launch and return-to-hub navigation still require human Unity Play Mode verification.
- The compact hub card layout should be visually checked after the builder regenerates the shell scene.

## Next Recommended Step
Run `Ghost > Build Game Shell Scene` in Unity, confirm Build Settings include `GameShellPrototype`, Act 1, Act 2, and Act 3, then verify Play Mode navigation from the hub to all three acts and back.
