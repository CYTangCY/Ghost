# M0-T48 - Run 001 - Act 5 testing and debugging implementation

## Task ID

M0-T48

## Run Number

001

## Date

2026-07-14

## Original Request / Codex Prompt Summary

Continue with Act 5. Implement the current-task Testing and Debugging slice: a pre-built faulty Act 3-style dialog graph, authored test conversations, expected-versus-actual results, graph repair through the existing wire interaction, full-suite reruns, Shell integration, tests, documentation, and scene generation.

## Files Created

- Assets/Scripts/Puzzles/TestingDebugging/Act5TestingModels.cs
- Assets/Scripts/Puzzles/TestingDebugging/Act5BuggyGraphData.cs
- Assets/Scripts/Puzzles/TestingDebugging/Act5TestSuiteRunner.cs
- Assets/Presentation/Act3DialogGraph/IDialogGraphWireInteractionHost.cs
- Assets/Presentation/Act5TestingDebugging/Act5TestingInteractionController.cs
- Assets/Presentation/Act5TestingDebugging/Act5TestingStaticPresenter.cs
- Assets/Presentation/Act5TestingDebugging/Editor/Act5TestingPrototypeSceneBuilder.cs
- Assets/Presentation/Act5TestingDebugging/Editor/Ghost.Presentation.Act5.Editor.asmdef
- Assets/Tests/EditMode/Act5TestSuiteRunnerTests.cs
- Docs/codex_runs/M0-T48_001_act5_testing_debugging_implementation.md

Unity meta files and the generated Act 5 scene were not created in this run because the Unity project lock remained active.

## Files Modified

- Assets/Presentation/Act3DialogGraph/Act3DialogGraphInputPortView.cs
- Assets/Presentation/Act3DialogGraph/Act3DialogGraphOutputPortView.cs
- Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs
- Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs
- Assets/Presentation/Shell/GameShellPresenter.cs
- Assets/Presentation/Shell/GhostNarrativeState.cs
- Assets/Presentation/Shell/ShellDialogueData.cs
- Assets/Presentation/Shell/ShellReturnToHubOverlay.cs
- Assets/Presentation/Shell/ShellSceneNames.cs
- Docs/LEARNING_CONTENT.md
- Docs/CODE_WALKTHROUGH.md
- Docs/UNITY_TEST_CHECKLIST.md

Observed pre-existing uncommitted Act 4 and Act 1-3 scene changes were preserved and not reverted.

## Tests or Checks Run

- Read Docs/CURRENT_TASK.md, the detailed Act 5 LEARNING_CONTENT section, IBM course section 1.10 summary, DialogGraph architecture, and the existing Act 3 logic/presentation APIs.
- Completed the detailed Act 5 mapping before writing implementation code.
- Built Ghost.EditModeTests.csproj with temporary MSBuild includes for the new runtime and test files.
- Built Ghost.Presentation.csproj with temporary includes for the new interface/controller/presenter.
- Built the Act 5 scene builder through Ghost.Presentation.Act4.Editor.csproj with a temporary include.
- Built Ghost.Presentation.Shell.Editor.csproj after adding the Act 5 hub card and Configure wiring.
- Ran a standalone pure-logic smoke runner against the compiled Ghost.Runtime assembly.
- Ran non-scene git diff --check.
- Monitored Temp/UnityLockfile for two minutes without terminating or manipulating the open Editor.
- Unity EditMode Test Runner: Not run — the Ghost project remained open in Unity Editor and batchmode could not safely acquire the project lock.
- Unity scene generation: Not run — the Ghost project remained open in Unity Editor and hand-editing Unity scene YAML is forbidden.
- Unity Play Mode: Not run — the generated Act 5 scene does not exist until the builder can run after the Editor releases the lock.

## Test / Check Result

- New runtime and EditMode test code compiled with 0 errors.
- New presentation code and shared Act 3 wire-host interface compiled with 0 errors; only two existing obsolete API warnings appeared in unrelated existing methods.
- Act 5 scene builder compiled with 0 errors.
- Updated GameShellSceneBuilder compiled with 0 errors.
- Pure-logic smoke result: seeded buggy graph = 0/4 passed and IsCorrect false; reference fixed graph = 4/4 passed and IsCorrect true.
- Non-scene git diff --check passed with line-ending conversion warnings only.
- The first test locks graph editing until Run all tests has exposed expected-versus-actual mismatches.
- Formal Unity test, scene, meta, and 1920x1080 layout results remain pending.

## Errors Encountered

- Normal sandboxed reads and apply_patch failed with windows sandbox helper_unknown_error while applying deny-read ACLs.
- The first exact-block LEARNING_CONTENT replacement did not match; a section-scoped regex replacement succeeded.
- A single long presenter write exceeded the Windows command-line length limit with OS error 206.
- Initial runtime compilation found DialogGraph namespace/type ambiguity in the new Ghost.Puzzles.TestingDebugging namespace.
- Several generated Unity csproj builds initially lacked local project.assets.json files.
- Unity held Temp/UnityLockfile throughout the scene-generation/test phase.

## Fixes Applied

- Used approved escalated operations after the sandbox helper failure and recorded the fallback.
- Split the long presenter creation into four bounded append operations.
- Added a DialogGraphModel alias for the existing DialogGraph class.
- Restored generated csproj assets locally with failed network sources ignored.
- Used a temporary MSBuild targets file under ignored Temp so generated csproj files did not need modification.
- Added IDialogGraphWireInteractionHost so Act 5 reuses the existing Act 3 input/output port components while Act 3 behaviour stays unchanged.
- Added a first-run edit lock to preserve the preview / test / revise teaching order.

## What Was Intentionally Not Changed

- Did not modify DialogGraph, DialogGraphSimulator, DialogGraphValidator, DialogGraphSession, Act3DialogGraphSampleData, or their tests.
- Did not modify Acts 1-2 presentation, Fundamentals, Backend, Banter, Common, GhostAvatar internals, Packages, or hand-edit scene YAML.
- Did not change existing scene files or ProjectSettings in this run.
- Did not terminate the user's Unity Editor or bypass its project lock.
- Did not advance CURRENT_TASK, update HANDOFF_LOG, or archive the task; Claude handles review and closure.

## Remaining Risks

- Act 5 scene and meta files still need Unity generation.
- GameShellPrototype still needs regeneration so the serialized Act 5 button appears.
- The full Unity EditMode suite and Act 3 Play Mode regression check remain required.
- Human 1920x1080 Play Mode verification is required for node/card fit, draggable ports, wire geometry, stale-result feedback, completion, and Shell debrief.
- Five Act cards now share one Shell row; the generated hub needs a visual fit check.

## Next Recommended Step

Close the Ghost Unity Editor normally, then resume Codex. Run Act5TestingPrototypeSceneBuilder, rebuild GameShellPrototype, run the filtered Act 5 tests and full EditMode suite, inspect generated Build Settings changes, create M0-T48 run 002, and perform the documented Play Mode checks.