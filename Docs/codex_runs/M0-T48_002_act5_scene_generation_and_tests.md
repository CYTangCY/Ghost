# M0-T48 - Run 002 - Act 5 scene generation and tests

## Task ID

M0-T48

## Run Number

002

## Date

2026-07-15

## Original Request / Codex Prompt Summary

Continue Act 5 after the Unity project lock and Windows sandbox helper ACL failure interrupted run 001. Generate the Act 5 and Shell scenes, verify Build Settings and serialized Shell wiring, run focused and full Unity EditMode tests, and record the remaining human Play Mode checks.

## Files Created

- Assets/Scenes/Act5TestingDebuggingPrototype.unity
- Assets/Scenes/Act5TestingDebuggingPrototype.unity.meta
- Unity-generated meta files for the Act 5 runtime, presentation, editor, test, and shared wire-host files created in run 001
- Docs/codex_runs/M0-T48_002_act5_scene_generation_and_tests.md

## Files Modified

- Assets/Scenes/GameShellPrototype.unity
- ProjectSettings/EditorBuildSettings.asset
- Docs/UNITY_TEST_CHECKLIST.md

Pre-existing uncommitted Act 4 and Act 1-3 scene changes were preserved and not reverted.

## Tests or Checks Run

- Confirmed no Unity process or Temp/UnityLockfile remained before batch operations.
- Verified Unity-generated csproj files include the Act 5 runtime, presentation, editor, and shared wire-host sources.
- Ran Act5TestingPrototypeSceneBuilder in Unity 6000.4.11f1 batch mode.
- Ran GameShellSceneBuilder in Unity 6000.4.11f1 batch mode.
- Verified GameShellPrototype serializes act5Button, Act 5: Testing, and Start Act 5.
- Verified EditorBuildSettings enables Act5TestingDebuggingPrototype after Act4ConfidenceFallbackPrototype.
- Ran Ghost.Tests.EditMode.Act5TestSuiteRunnerTests through the Unity Test Runner.
- Ran the complete Unity EditMode suite through the Unity Test Runner.
- Ran git diff --check excluding Unity-generated scene YAML.
- Unity Play Mode: Not run — batch mode cannot perform the required visual, drag-wire, Shell navigation, and 1920x1080 human checks.

## Test / Check Result

- Act 5 scene generation completed and Unity exited batch mode successfully.
- Shell scene regeneration completed with the Act 5 card and serialized button reference.
- Focused Act 5 EditMode tests passed: 4/4, 0 failed.
- Full EditMode regression passed: 64/64, 0 failed.
- Non-scene git diff --check passed with line-ending conversion warnings only.
- Full git diff --check reports trailing spaces in Unity-generated empty m_Name and m_Text fields in pre-existing regenerated Act 1-3 and Shell scene YAML. These generated scenes were not hand-edited.

## Errors Encountered

- Normal sandboxed operations remained unavailable because the Windows sandbox helper returned helper_unknown_error while applying deny-read ACLs.
- The first focused-test result read occurred before the detached Unity child finished writing its XML, even though the launcher had returned exit code 0.
- The initial Windows rg command used a shell glob that rg does not accept on Windows.

## Fixes Applied

- Used approved escalated PowerShell operations after the known sandbox helper failure.
- Re-ran the source inclusion search with rg -g.
- Read the focused result after Unity logged test completion and confirmed the XML was present.
- Used Start-Process -Wait for the full regression so result parsing could not race Unity shutdown.
- Kept scene verification read-only and did not hand-edit generated YAML.

## What Was Intentionally Not Changed

- Did not modify DialogGraph, DialogGraphSimulator, DialogGraphValidator, DialogGraphSession, Act3DialogGraphSampleData, or their tests.
- Did not modify Acts 1-2 pure logic, Act 4 implementation, Backend, Packages, or unrelated ProjectSettings.
- Did not repair Unity-generated scene whitespace by hand.
- Did not advance CURRENT_TASK, update HANDOFF_LOG, or archive the task; Claude handles review and closure.

## Remaining Risks

- Human Play Mode verification remains required for the Act 5 onboarding flow, graph/card fit, draggable ports, wire replacement, stale-result feedback, completion, Shell debrief, and Console cleanliness at 1920x1080.
- Act 3 Play Mode drag-wire regression remains a human Editor check.
- Five Act cards share one Shell row and need a visual fit check.

## Next Recommended Step

Perform the documented M0-T48 Play Mode and Act 3 regression checks, then have Claude review the diff, record human verification, archive M0-T48, update HANDOFF_LOG, and advance CURRENT_TASK.
