# M0-T47 - Run 002 - Act 4 instruction clarity

## Task ID

M0-T47

## Run Number

002

## Date

2026-07-14

## Original Request / Codex Prompt Summary

The player could not understand Act 4's purpose or what actions the level expected. Clarify the learning goal, routing rule, task sequence, and feedback without changing the authored mechanic or deterministic scoring.

## Files Created

- `Docs/codex_runs/M0-T47_002_act4_instruction_clarity.md`

## Files Modified

- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceStaticPresenter.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceInteractionController.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Inspected the existing uncommitted M0-T47 changes and current Act 4 presentation/controller text.
- Ran `dotnet restore Ghost.Presentation.csproj --ignore-failed-sources --nologo` to create the missing local assets file.
- Ran `dotnet build Ghost.Presentation.csproj --no-restore --nologo`.
- Reviewed the edited Act 4 strings and updated walkthrough/checklist content.
- Ran `git diff --check -- . ':(exclude)Assets/Scenes/*.unity'`.
- Unity Test Framework: Not run — the Ghost project was open in Unity Editor and a second batchmode process would conflict with the project lock.
- Unity Play Mode: Not run — the open Editor session was not controlled by Codex, so the revised 1920x1080 layout still needs human verification.

## Test / Check Result

- `Ghost.Presentation.csproj` build passed with 0 errors.
- The build reported two pre-existing obsolete API warnings in `ShellReturnToHubOverlay.cs` and `Act3DialogGraphStaticPresenter.cs`.
- The revised UI now explains the confidence score, threshold rule, fallback and handoff purposes, three player actions, slider trade-off, and exact per-visitor route comparison.
- The Act 4 validator and authored scoring data were not changed.
- Non-scene `git diff --check` passed; Git only reported line-ending conversion warnings.

## Errors Encountered

- Normal sandboxed reads failed again with `windows sandbox: helper_unknown_error: apply deny-read ACLs`.
- `apply_patch` failed with the same ACL error.
- The first `dotnet build --no-restore` failed because `Temp/obj/Ghost.Presentation/project.assets.json` did not exist.

## Fixes Applied

- Used approved escalated read commands after the sandbox helper failure.
- Used exact-block PowerShell replacements only after `apply_patch` could not read the workspace files.
- Ran a local restore with failed network sources ignored, then reran the presentation build successfully.
- Replaced abstract tutorial wording with a concrete 30% threshold versus 62% vague-message example.
- Added a persistent goal, explicit routing formula, three-step task guide, slider trade-off labels, confidence labels in the queue, and playback comparison text.

## What Was Intentionally Not Changed

- Did not change `Act4ConfidenceValidator`, `Act4ConfidenceDemoData`, threshold acceptance range, visitor outcomes, or puzzle completion rules.
- Did not change scenes, ProjectSettings, Packages, Build Settings, Shell flow, or Acts 1-3.
- Did not regenerate the Act 4 scene because the presenter builds this UI at runtime and the scene component wiring did not change.
- Did not advance `Docs/CURRENT_TASK.md`, archive M0-T47, or update `Docs/HANDOFF_LOG.md`; Claude handles review and closure.

## Remaining Risks

- Human Play Mode verification is required to confirm all added copy fits at 1920x1080 and remains readable while failed-run feedback is visible.
- The English copy is intentionally direct and instructional; final narrative voice polish can follow after the user confirms the mechanic is now understandable.

## Next Recommended Step

Exit and re-enter Play Mode after Unity imports the scripts, then follow the updated Act 4 Play Mode checklist. Ask the user whether the purpose and three actions are understandable before Claude closes M0-T47.