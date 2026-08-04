# M0-T48 - Run 003 - Act 5 wire interaction usability fix

## Task ID

M0-T48

## Run Number

003

## Date

2026-07-15

## Original Request / Codex Prompt Summary

The user reported that the Act 5 play interface contained many bugs, gave no clear way to play, and appeared unplayable. The supplied screenshot showed committed wires shifted into the lower-left of the graph board instead of connecting node sockets.

## Files Created

- Assets/Tests/EditMode/Act5TestingStaticPresenterTests.cs
- Docs/codex_runs/M0-T48_003_act5_wire_interaction_usability_fix.md

The new test meta file remains Unity-generated and is pending import when the active Play Mode stops.

## Files Modified

- Assets/Presentation/Act5TestingDebugging/Act5TestingStaticPresenter.cs
- Assets/Tests/EditMode/Ghost.EditModeTests.asmdef
- Docs/CODE_WALKTHROUGH.md
- Docs/UNITY_TEST_CHECKLIST.md

All pre-existing uncommitted work was preserved.

## Tests or Checks Run

- Compared the broken screenshot with Act 5 and working Act 3 wire geometry.
- Built Ghost.Presentation.csproj externally after a local restore.
- Built Ghost.EditModeTests.csproj externally with an ignored Temp MSBuild include for the new test and presentation reference.
- Unity Test Runner: Not run — the user currently has the Ghost project open in Play Mode, so batchmode cannot acquire the project lock and the Editor has not imported the new test yet.
- Unity Play Mode verification: Not run — the currently running player contains the pre-fix assembly and must be stopped before Unity imports the change.

## Test / Check Result

- Root cause confirmed: GetPortLocalCenter returns wire-layer coordinates around the layer centre, but DrawLine previously anchored its RectTransform at the bottom-left. Every line was therefore displaced by the board pivot offset.
- Act 5 presentation build passed with 0 errors and two pre-existing obsolete API warnings.
- The EditMode test assembly including Act5TestingStaticPresenterTests compiled with 0 errors and the same two pre-existing warnings.
- Formal Unity test execution and post-fix visual verification remain pending.

## Errors Encountered

- view_image could not read the supplied screenshot because the Windows sandbox helper failed while applying deny-read ACLs; the screenshot embedded in the conversation remained visible for diagnosis.
- apply_patch failed with the same sandbox helper_unknown_error.
- The first temporary test-project restore created a circular project reference because the target applied to all referenced projects.
- Unity remained open with an active project lock throughout this run.

## Fixes Applied

- Matched Act 3 wire geometry: centre anchors, source-side pivot, source anchored position, computed length, and local rotation.
- Forced Canvas and root layout updates before rebuilding committed wires.
- Increased socket hit targets from 20 to 26 pixels.
- Muted output sockets before the first test to show that editing is locked.
- Added persistent numbered TEST, REPAIR, and RERUN instructions and explicit LEFT input / RIGHT output node labels.
- Renamed the primary actions to 1. Run all 4 tests and 3. Rerun all 4 tests.
- Added an EditMode regression test for the wire coordinate contract.
- Scoped the temporary MSBuild include to Ghost.EditModeTests to remove the restore cycle.

## What Was Intentionally Not Changed

- Did not modify DialogGraph pure logic, simulator, validator, authored Act 5 conversations, scoring, Act 3 code, scenes, ProjectSettings, Packages, or existing meta files.
- Did not terminate or control the user's open Unity Editor.
- Did not hand-edit Unity scene YAML or create the test meta file manually.
- Did not advance CURRENT_TASK, update HANDOFF_LOG, or archive the task.

## Remaining Risks

- The user must stop Play Mode so Unity can import the fix and generate the new test meta file.
- Formal Unity EditMode tests and 1920x1080 visual/drag-drop verification are still required.
- Node instruction wrapping and the test-panel button position need visual confirmation in the refreshed player.

## Next Recommended Step

Stop Play Mode, wait for Unity compilation, rerun the focused and full EditMode suites, then re-enter Act 5 and execute the M0-T48 Run 003 checklist before Claude closure.
