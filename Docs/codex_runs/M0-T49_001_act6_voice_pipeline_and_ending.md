# M0-T49 - Run 001 - Chapter 6 voice pipeline and ending

## Task ID

M0-T49

## Run Number

001

## Date

2026-07-15

## Original Request / Codex Prompt Summary

The user confirmed the prior chapter work and asked to begin Chapter 6. CURRENT_TASK defines the final six-chapter capstone: assemble UI input, NLP engine, dialogue management, response generation, and UI output in order, attach backend integration as a side link, follow one deterministic final visitor message through the repaired path, restore Ghost's full voice, and play a personalized skippable ending before returning to the title.

## Files Created

- Assets/Scripts/Puzzles/VoicePipeline/Act6PipelineModels.cs
- Assets/Scripts/Puzzles/VoicePipeline/Act6PipelineData.cs
- Assets/Scripts/Puzzles/VoicePipeline/Act6PipelineValidator.cs
- Assets/Presentation/Act6VoicePipeline/IAct6PipelineInteractionHost.cs
- Assets/Presentation/Act6VoicePipeline/Act6PipelineInteractionController.cs
- Assets/Presentation/Act6VoicePipeline/Act6PipelinePartDragView.cs
- Assets/Presentation/Act6VoicePipeline/Act6PipelineSlotDropView.cs
- Assets/Presentation/Act6VoicePipeline/Act6PipelineStaticPresenter.cs
- Assets/Presentation/Act6VoicePipeline/Act6EndingSequence.cs
- Assets/Presentation/Act6VoicePipeline/Editor/Act6VoicePipelinePrototypeSceneBuilder.cs
- Assets/Presentation/Act6VoicePipeline/Editor/Ghost.Presentation.Act6.Editor.asmdef
- Assets/Tests/EditMode/Act6PipelineValidatorTests.cs
- Docs/codex_runs/M0-T49_001_act6_voice_pipeline_and_ending.md

Unity-generated meta files and the generated Chapter 6 scene are pending Editor import after the open project lock is released.

## Files Modified

- Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs
- Assets/Presentation/Shell/GameShellPresenter.cs
- Assets/Presentation/Shell/GhostNarrativeState.cs
- Assets/Presentation/Shell/ShellDialogueData.cs
- Assets/Presentation/Shell/ShellReturnToHubOverlay.cs
- Assets/Presentation/Shell/ShellSceneNames.cs
- Docs/LEARNING_CONTENT.md
- Docs/CODE_WALKTHROUGH.md
- Docs/UNITY_TEST_CHECKLIST.md

All pre-existing uncommitted Act 3, Act 4, Act 5, scene, Build Settings, and documentation work was preserved.

## Tests or Checks Run

- Read every repository document required by AGENTS.md before implementation.
- Added the Chapter 6 learning mapping before writing Chapter 6 code.
- Built Ghost.EditModeTests.csproj externally with an ignored Temp MSBuild include that compiles the new runtime, presentation, and test sources.
- Built the Chapter 6 scene builder externally through the temporary editor-project include.
- Built Ghost.Presentation.Shell.Editor.csproj externally after the six-card hub changes.
- Ran a standalone .NET validator smoke test for a correct pipeline and a missing-backend pipeline.
- Scanned all new and modified Chapter 6 C# for non-ASCII characters.
- Ran focused diff/whitespace checks on the Chapter 6 Shell and documentation edits.
- Unity Test Runner: Not run — the Unity Editor still holds the Ghost project lock and has not imported the new Chapter 6 files.
- Unity Play Mode: Not run — the Chapter 6 scene cannot be generated safely until the open Editor imports the scripts or releases the project lock.
- Scene builder execution: Not run — a second Unity process cannot acquire the currently open project.

## Test / Check Result

- Ghost.Runtime, Ghost.Presentation, and Ghost.EditModeTests compiled with 0 errors. Two existing obsolete FindFirstObjectByType warnings remain in ShellReturnToHubOverlay and Act3DialogGraphStaticPresenter.
- The Chapter 6 editor builder compiled with 0 errors and 0 warnings.
- The updated Shell editor builder compiled with 0 errors and 0 warnings.
- Validator smoke output: correct=True, errors=0; missingBackend=False, firstBroken=backend_integration.
- Six authored EditMode tests compile, but formal NUnit execution is pending Unity import.
- New/modified Chapter 6 C# passed the ASCII scan.
- Focused Chapter 6 tracked-file diff check found no whitespace errors. Pre-existing generated Act 1-3 and Shell scene diffs contain Unity YAML trailing spaces and were intentionally left unchanged.

## Errors Encountered

- The Windows sandbox helper repeatedly failed with helper_unknown_error while applying deny-read ACLs, including apply_patch and ordinary read commands.
- A first controller/interface write attempt used the reserved PowerShell variable name host; the controller succeeded and the interface write failed.
- Early combined Shell-builder exact-replacement attempts failed before writing because of a Replace overload mismatch and a PowerShell here-string parser error.
- The initial standalone smoke project targeted .NET 8, but this machine has .NET 6 and .NET 10 runtimes.
- Unity remained open on the Ghost project, leaving Temp/UnityLockfile in place and delaying meta generation, scene generation, and formal Editor tests.

## Fixes Applied

- Used narrowly scoped, approved PowerShell reads and exact writes outside the broken sandbox helper after apply_patch failed.
- Retried the interaction interface write with a non-reserved variable name.
- Split the Shell edits into small exact replacements and rebuilt all affected assemblies.
- Retargeted only the ignored temporary smoke project to the installed .NET 10 runtime.
- Unified the Chapter 6 action language to Open the repair board, Run the voice path, Try the voice path again, and Hear Ghost speak.
- Kept backend integration visibly and logically separate from the canonical five-stage main path.
- Added click-select placement alongside drag/drop so the puzzle remains usable if dragging is awkward.
- Added focused first-broken-stage consequences, prior-chapter callbacks on correct placements, reset/retry, six-step deterministic playback, and one completion path shared by full ending and skip.

## What Was Intentionally Not Changed

- Did not modify existing Ghost.Runtime validators, sessions, sample data, demo engines, Fundamentals, Backend, Banter, Common, GhostAvatar internals, Packages, or existing meta files.
- Did not hand-edit or create Unity scene YAML.
- Did not manually create Unity meta files.
- Did not add the Chapter 6 Build Settings entry before its scene exists; the approved append remains in the scene builders.
- Did not terminate, automate, or otherwise control the user's open Unity Editor.
- Did not advance CURRENT_TASK, update HANDOFF_LOG, archive M0-T47/M0-T49, stage, commit, or push.

## Remaining Risks

- Unity must import the new folders and generate meta files.
- Act6VoicePipelinePrototype.unity must be generated, the Shell scene regenerated, and the approved Chapter 6 Build Settings entry verified.
- The 6/6 focused tests and full EditMode suite need formal Unity Test Runner execution.
- The 1920x1080 layout, both placement modes, wrong-order feedback, playback highlights, full ending, skip path, title return, and Acts 1-5 regression need Play Mode verification.
- The open Editor may surface Unity-only serialization or layout issues that external C# compilation cannot detect.

## Next Recommended Step

Close the Unity Editor or release the project lock, then run the Chapter 6 and Shell scene builders, execute focused and full EditMode tests, and complete the M0-T49 Play Mode checklist before Claude closure.
