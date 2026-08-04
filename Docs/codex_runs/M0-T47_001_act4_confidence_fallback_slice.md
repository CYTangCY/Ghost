# M0-T47 - Run 001 - Act 4 confidence fallback slice

## Task ID

M0-T47

## Run Number

001

## Date

2026-07-14

## Original Request / Codex Prompt Summary

Continue M0-T47 after the previous task was interrupted by a Windows sandbox helper ACL error. First read `Docs/CURRENT_TASK.md` and existing uncommitted changes, then continue Slice A: Act 4 Confidence & Fallback.

## Files Created

- `Assets/Scripts/Puzzles/ConfidenceFallback.meta`
- `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceModels.cs`
- `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceModels.cs.meta`
- `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceDemoData.cs`
- `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceDemoData.cs.meta`
- `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceValidator.cs`
- `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceValidator.cs.meta`
- `Assets/Presentation/Act4ConfidenceFallback.meta`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceInteractionController.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceInteractionController.cs.meta`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceStaticPresenter.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceStaticPresenter.cs.meta`
- `Assets/Presentation/Act4ConfidenceFallback/Editor.meta`
- `Assets/Presentation/Act4ConfidenceFallback/Editor/Act4ConfidencePrototypeSceneBuilder.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Editor/Act4ConfidencePrototypeSceneBuilder.cs.meta`
- `Assets/Presentation/Act4ConfidenceFallback/Editor/Ghost.Presentation.Act4.Editor.asmdef`
- `Assets/Presentation/Act4ConfidenceFallback/Editor/Ghost.Presentation.Act4.Editor.asmdef.meta`
- `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity`
- `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity.meta`
- `Assets/Tests/EditMode/Act4ConfidenceValidatorTests.cs`
- `Assets/Tests/EditMode/Act4ConfidenceValidatorTests.cs.meta`
- `Docs/codex_runs/M0-T47_001_act4_confidence_fallback_slice.md`

## Files Modified

- `Assets/Presentation/Shell/ShellSceneNames.cs`
- `Assets/Presentation/Shell/GhostNarrativeState.cs`
- `Assets/Presentation/Shell/ShellDialogueData.cs`
- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Assets/Scenes/GameShellPrototype.unity`
- `ProjectSettings/EditorBuildSettings.asset`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

Observed pre-existing uncommitted changes at run start, not intentionally edited by this continuation:

- `Assets/Scenes/Act1IntentClassificationPrototype.unity`
- `Assets/Scenes/Act2EntityExtractionPrototype.unity`
- `Assets/Scenes/Act3DialogGraphPrototype.unity`
- `Docs/LEARNING_CONTENT.md` (already contained the Act 4 mapping required before implementation)

## Tests or Checks Run

- Read required project docs: `Docs/CURRENT_TASK.md`, `Docs/CONFIRMED_PROJECT_CONTEXT.md`, `Docs/ROADMAP.md`, `Docs/REQUIREMENTS.md`, `Docs/LEARNING_CONTENT.md`, and `Docs/ARCHITECTURE.md`.
- Checked initial `git status --short`, `git diff --stat`, and `git diff --name-status`.
- Ran Unity batchmode scene generation:
  - `Ghost.Presentation.Act4.Editor.Act4ConfidencePrototypeSceneBuilder.BuildAct4ConfidencePrototypeScene`
  - `Ghost.Presentation.Shell.Editor.GameShellSceneBuilder.BuildGameShellScene`
- Confirmed `Assets/Scenes/GameShellPrototype.unity` contains `Act 4: Confidence`, `Start Act 4`, and serialized `act4Button` references.
- Ran filtered Unity EditMode tests for `Ghost.Tests.EditMode.Act4ConfidenceValidatorTests`.
- Ran full Unity EditMode test suite.
- Ran `git diff --check` and then reran it excluding Unity scene YAML files.

## Test / Check Result

- Unity script compilation succeeded during batchmode scene generation.
- Act 4 scene generated successfully and was imported by Unity.
- Game Shell scene regenerated successfully and now includes the Act 4 hub card/button.
- `ProjectSettings/EditorBuildSettings.asset` changed only by appending `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity`.
- Filtered Act 4 EditMode tests passed: 5/5.
- Full EditMode suite passed: 60/60.
- Unity batchmode logs included existing/benign warnings:
  - obsolete `Object.FindFirstObjectByType<T>()` warnings in existing presentation scripts
  - Unity licensing access-token update warning while license validation still succeeded
  - Direct3D info queue warning
- Unity Play Mode was not run interactively in this Codex session.
- Full `git diff --check` reported Unity YAML trailing whitespace in generated/pre-existing scene diffs. `git diff --check -- . ':(exclude)Assets/Scenes/*.unity'` passed for non-scene files.

## Errors Encountered

- Normal sandboxed PowerShell reads failed with `windows sandbox: helper_unknown_error: apply deny-read ACLs`.
- `apply_patch` also failed with the same sandbox helper ACL issue.
- The first Unity test command used `-quit` and exited without creating a test results XML; it was not counted as a valid test run.

## Fixes Applied

- Used approved escalated PowerShell commands for read/write operations after sandbox helper failures.
- Used minimal PowerShell text edits as a fallback because `apply_patch` could not read files under the broken sandbox helper.
- Reran Unity tests without `-quit` and with an Act 4 filter; this generated a valid XML result.
- Ran the full EditMode suite afterward.

## What Was Intentionally Not Changed

- Did not modify existing pure logic validators/sessions/sample data for Acts 1-3.
- Did not modify Fundamentals, Backend, Packages, or non-Build-Settings ProjectSettings.
- Did not delete, rename, or revert existing `.meta` files.
- Did not revert pre-existing uncommitted scene changes in Acts 1-3.
- Did not advance `Docs/CURRENT_TASK.md`, archive the task, or update `Docs/HANDOFF_LOG.md`; per the repository workflow, Claude handles review/closure/handoff updates.
- Did not add LLM/backend scoring. Act 4 correctness remains deterministic.

## Remaining Risks

- Human Play Mode verification is still required for 1920x1080 layout fit, slider feel, Shell hub flow, return overlay, and Act 4 completion/debrief flow.
- The Act 4 UI is a lean generated UGUI first version and may need user polish after Play Mode review.
- The working tree still contains pre-existing large Act 1-3 scene diffs from before this continuation.

## Next Recommended Step

Have Claude review the M0-T47 implementation, then run Unity Play Mode checks for the Shell hub and Act 4 scene using `Docs/UNITY_TEST_CHECKLIST.md`.