# M0-T13 - Run 002 - game shell prototype

## Task ID

M0-T13

## Run Number

002

## Date

2026-06-20

## Original Request / Codex Prompt Summary

Implement the revised M0-T13 Game Shell prototype from `Docs/CURRENT_TASK.md`: add a placeholder title screen, act-select/hub, reusable Lily dialogue frame with text from data, Ghost placeholder presence, SceneManager-based shell-to-Act-1 navigation, and a way to return from Act 1 to the shell. Use an editor scene builder instead of hand-writing `.unity` YAML. The user approved adding the shell and Act 1 scenes to Build Settings for this task only.

## Files Created

- `Assets/Presentation/Shell/ShellSceneNames.cs`
- `Assets/Presentation/Shell/ShellDialogueData.cs`
- `Assets/Presentation/Shell/LilyDialogueFrame.cs`
- `Assets/Presentation/Shell/ShellSceneNavigationButton.cs`
- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Assets/Presentation/Shell/Editor/Ghost.Presentation.Shell.Editor.asmdef`
- `Docs/codex_runs/M0-T13_002_game_shell_prototype.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/README.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `rg` checks for UnityEditor references, shell scene navigation references, dialogue data references, and M0-T13 documentation references.
- `git -c safe.directory=C:/Users/fcxsw/OneDrive/桌面/KCL_Project/Ghost diff --check -- Assets/Presentation/Shell Assets/Presentation/Act1IntentClassification/README.md Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- Unity batch mode attempt using the installed Unity 6000.4.11f1 editor and `Ghost.Presentation.Shell.Editor.GameShellSceneBuilder.BuildGameShellScene`.
- Unity batch mode retry with a relative project path.
- Unity batch mode retry through a temporary ASCII junction path.

## Test / Check Result

- `rg` checks found the expected shell scripts, SceneManager loading calls, menu builder, return-to-hub button text, and Game Shell documentation entries.
- `git diff --check` completed with no whitespace errors for the files checked. Git reported line-ending normalization warnings only.
- Unity batch mode did not execute the builder successfully. Each attempt exited with code 1 before creating `Assets/Scenes/GameShellPrototype.unity` or updating `ProjectSettings/EditorBuildSettings.asset`.
- Unity Play Mode:
  Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity EditMode tests:
  Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- Unity batch mode initially received the project path with mojibake around the non-ASCII Desktop folder name.
- Retrying with a relative project path still resolved to the mojibake path inside Unity.
- Retrying through a temporary ASCII junction avoided the path mojibake but Unity still exited before project import. The log showed licensing client connection refusal and assertion failures before terminating with return code 1.
- The first attempt to remove the temporary junction with `Remove-Item` threw a PowerShell null reference exception after reporting removal. A follow-up check showed the junction still existed.

## Fixes Applied

- Added the shell runtime presentation scripts under `Assets/Presentation/Shell/`.
- Added a shell editor assembly definition and a Unity editor builder under `Assets/Presentation/Shell/Editor/`.
- The builder creates the placeholder shell UI and registers `Assets/Scenes/GameShellPrototype.unity` plus `Assets/Scenes/Act1IntentClassificationPrototype.unity` in Build Settings when it can be run inside Unity.
- Added `ShellReturnToHubOverlay`, which creates a lightweight runtime `Return to Hub` button when the Act 1 scene loads, keeping Act 1 puzzle rules and pure logic unchanged.
- Cleaned up the temporary ASCII junction using `.NET Directory.Delete` after verifying it was a junction under the temp directory.
- Updated `CODE_WALKTHROUGH.md` and `UNITY_TEST_CHECKLIST.md` with shell setup and Play Mode test steps.

## What Was Intentionally Not Changed

- No files under `Assets/Scripts/Puzzles/IntentClassification/` were edited.
- `Assets/Scripts/Ghost.Runtime.asmdef` was not edited.
- `Assets/Tests/EditMode/` was not edited.
- `Assets/Scenes/SampleScene.unity` was not edited.
- No `.unity` scene YAML was hand-written.
- `ProjectSettings/EditorBuildSettings.asset` was not manually edited because the shell scene could not be generated safely in this Codex session.
- No Act 1 puzzle rules, Act 2, node graph, save/load, backend, LLM integration, full visual-novel dialogue system, scoring, or final art were implemented.

## Remaining Risks

- `Assets/Scenes/GameShellPrototype.unity` does not exist yet because Unity batch mode failed before running the builder.
- Build Settings were not updated in this Codex session for the same reason.
- Unity must import and compile the new shell scripts, then the user should run `Ghost > Build Game Shell Scene` in the Unity Editor to create the shell scene and update Build Settings.
- The return-to-hub overlay depends on `GameShellPrototype` being present in Build Settings before Play Mode navigation can succeed.
- There were pre-existing local modifications in the working tree before this run, including docs and the Act 1 scene; this run did not revert them.

## Next Recommended Step

Open the project in Unity, wait for scripts to compile, run `Ghost > Build Game Shell Scene`, then open `Assets/Scenes/GameShellPrototype.unity` and follow the M0-T13 checklist in `Docs/UNITY_TEST_CHECKLIST.md`.
