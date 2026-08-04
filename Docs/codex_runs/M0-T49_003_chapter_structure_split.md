# M0-T49 - Run 003 - Chapter 0, Chapter 6 teaching, and Final Chapter split

## Task ID

M0-T49

## Run Number

003

## Date

2026-07-15

## Original Request / Codex Prompt Summary

The user reported that the gameplay UI was buggy, unclear, and not playable, then clarified the intended story/teaching structure: Chapter 6 must be a teaching chapter, Chapter 0 must be a separate opening story, and a separate Final Chapter must contain the capstone and ending. The user approved Chapter 6 as Backend Action / Response Generation and approved moving the existing full voice-pipeline work to Final Chapter.

## Files Created

- Assets/Scripts/Puzzles/BackendResponse/Act6BackendResponseModels.cs
- Assets/Scripts/Puzzles/BackendResponse/Act6BackendResponseData.cs
- Assets/Scripts/Puzzles/BackendResponse/Act6BackendResponseValidator.cs
- Assets/Presentation/Act6BackendResponse/IAct6BackendInteractionHost.cs
- Assets/Presentation/Act6BackendResponse/Act6BackendInteractionController.cs
- Assets/Presentation/Act6BackendResponse/Act6BackendCardDragView.cs
- Assets/Presentation/Act6BackendResponse/Act6BackendSlotDropView.cs
- Assets/Presentation/Act6BackendResponse/Act6BackendStaticPresenter.cs
- Assets/Presentation/Act6BackendResponse/Editor/Act6BackendResponseSceneBuilder.cs
- Assets/Presentation/Act6BackendResponse/Editor/Ghost.Presentation.Act6Backend.Editor.asmdef
- Assets/Tests/EditMode/Act6BackendResponseValidatorTests.cs
- Assets/Presentation/Story/Chapter0StoryData.cs
- Assets/Presentation/Story/Chapter0StoryPresenter.cs
- Assets/Presentation/Story/Editor/Chapter0StorySceneBuilder.cs
- Assets/Presentation/Story/Editor/Ghost.Presentation.Story.Editor.asmdef
- Assets/Scenes/Chapter0OpeningStory.unity
- Assets/Scenes/Act6BackendResponsePrototype.unity
- Unity-generated .meta files for all new folders, scripts, assembly definitions, tests, and scenes above
- Docs/codex_runs/M0-T49_003_chapter_structure_split.md

## Files Modified

- Docs/LEARNING_CONTENT.md
- Assets/Presentation/Shell/ShellSceneNames.cs
- Assets/Presentation/Shell/GhostNarrativeState.cs
- Assets/Presentation/Shell/ShellDialogueData.cs
- Assets/Presentation/Shell/GameShellPresenter.cs
- Assets/Presentation/Shell/ShellReturnToHubOverlay.cs
- Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs
- Assets/Presentation/Act6VoicePipeline/Act6PipelineInteractionController.cs
- Assets/Presentation/Act6VoicePipeline/Act6EndingSequence.cs
- Assets/Presentation/Act6VoicePipeline/Act6PipelineStaticPresenter.cs
- Assets/Presentation/Act6VoicePipeline/Act6PipelinePartDragView.cs
- Assets/Presentation/Act6VoicePipeline/Editor/Act6VoicePipelinePrototypeSceneBuilder.cs
- Assets/Scripts/Puzzles/VoicePipeline/Act6PipelineData.cs
- Assets/Scenes/Act6VoicePipelinePrototype.unity
- Assets/Scenes/GameShellPrototype.unity
- ProjectSettings/EditorBuildSettings.asset
- Docs/CODE_WALKTHROUGH.md
- Docs/UNITY_TEST_CHECKLIST.md

## Tests or Checks Run

- Updated Docs/LEARNING_CONTENT.md with the user-approved Chapter 0 / Chapters 1-6 / Final Chapter mapping before implementation.
- Ran Chapter0StorySceneBuilder in Unity 6000.4.11f1 batchmode.
- Ran Act6BackendResponseSceneBuilder in Unity batchmode.
- Ran the reclassified Final Chapter Act6VoicePipelinePrototypeSceneBuilder in Unity batchmode.
- Ran GameShellSceneBuilder in Unity batchmode after all chapter scenes.
- Inspected all builder logs for C# compiler errors, exceptions, failed execute methods, and abnormal exits.
- Verified Chapter0OpeningStory, Act6BackendResponsePrototype, Act6VoicePipelinePrototype, and GameShellPrototype were regenerated.
- Verified GameShellPrototype serializes non-null Chapter 0, Chapter 6, and Final Chapter buttons.
- Verified player-facing scene titles identify Chapter 0, Chapter 6 Backend Action and Response, and Final Chapter Repair Ghost's Voice.
- Verified EditorBuildSettings canonical order is Shell, Chapter 0, Chapters 1-6, Final Chapter, then the pre-existing SampleScene.
- Ran Ghost.Tests.EditMode.Act6BackendResponseValidatorTests.
- Ran Ghost.Tests.EditMode.Act6PipelineValidatorTests for the reclassified Final Chapter.
- Ran the complete Unity EditMode suite.
- Unity Play Mode visual/interaction test: Not run — automated batchmode verified compilation, generated scenes, serialized references, deterministic tests, and scene order; final pointer ergonomics and 1920x1080 visual judgment require the interactive Game view.

## Test / Check Result

- All four Unity scene builders completed successfully with return code 0.
- No C# compilation errors or builder exceptions were reported.
- Chapter 6 focused tests passed: 6/6, 0 failed, 0 skipped.
- Final Chapter pipeline tests passed: 6/6, 0 failed, 0 skipped.
- Full EditMode regression passed: 77/77, 0 failed, 0 skipped.
- Shell scenes and Build Settings contain the approved three-part structure.

## Errors Encountered

- The workspace sandbox helper continued failing with helper_unknown_error while applying deny-read ACLs. Direct apply_patch calls could not read workspace files.
- One attempted large PowerShell write exceeded the Windows command-length limit while creating the large Chapter 6 presenter.
- Initial generated Chapter 6 data/controller strings had two malformed quote expressions, caught by Unity compilation.
- One exact text replacement failed because the source file used mixed line endings.
- The first Chapter 6 focused test run passed 5/6 but EmptyBoardFailsAtDataSource errored because the NUnit version could not apply Has.Count to IReadOnlyList.

## Fixes Applied

- Used narrowly scoped, approved PowerShell exact reads/writes only after apply_patch failed from the known sandbox helper ACL error.
- Split the large presenter write into smaller exact chunks.
- Corrected both malformed quote expressions and reran Unity scene generation.
- Normalized the edited Shell source to LF before exact replacements.
- Changed the failing assertion to check result.Errors.Count directly, then reran focused and full test suites.
- Rebuilt GameShellPrototype last so all button references and canonical Build Settings order were serialized together.

## What Was Intentionally Not Changed

- Did not edit Docs/CURRENT_TASK.md, Docs/ROADMAP.md, Docs/HANDOFF_LOG.md, or Docs/completed_tasks; Claude remains responsible for review, closure, archiving, and task advancement.
- Did not rename or delete the existing Act6VoicePipeline folder, classes, scene, or .meta files; retained internal names preserve Unity asset identities while player-facing behavior is Final Chapter.
- Did not hand-edit scene YAML or .meta files.
- Did not change Packages, unrelated ProjectSettings, deterministic logic for Chapters 1-5, LLM/backend scoring rules, or pre-existing uncommitted user/agent work.
- Did not stage, commit, push, or revert unrelated changes.

## Remaining Risks

- Human Play Mode verification is still required for first-time Chapter 0 routing, all six story beats, skip, Shell debrief, Chapter 6 drag/drop and click placement, status readability, five-step playback, Final Chapter ending/skip, and 1920x1080 overlap checks.
- GameShell's existing in-memory narrative state means a clean first-run test may require restarting Play Mode/domain state.
- The retained Act6 internal names are intentionally different from their new player-facing Final Chapter identity and may need a later migration task if the project chooses to rename assets safely.
- Existing obsolete FindFirstObjectByType warnings remain warnings only and do not block compilation or tests.

## Next Recommended Step

Run the M0-T49 Run 003 Play Mode checklist at 1920x1080, then give the results and this run log to Claude for review, documentation closure, task archiving, HANDOFF update, and CURRENT_TASK advancement.