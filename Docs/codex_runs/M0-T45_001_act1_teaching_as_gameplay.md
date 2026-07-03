# M0-T45 - Run 001 - Act 1 teaching as gameplay

## Task ID

M0-T45

## Run Number

001

## Date

2026-07-03

## Original Request / Codex Prompt Summary

Implement M0-T45 Run 001 only: add a shared programmatic Ghost expression face and rebuild Act 1 so
intent classification is taught through gameplay. The player watches Ghost fail, clusters visitor
transcript cards into free training piles, labels each pile by purpose, teaches Ghost, and watches
Ghost answer unseen visitor messages based on the player's piles. Do not touch Act 2 in this run.

## Files Created

- `Assets/Presentation/GhostAvatar/GhostMood.cs`
- `Assets/Presentation/GhostAvatar/GhostFaceView.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationLabelDragView.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentTeachingDropTarget.cs`
- `Assets/Scripts/Puzzles/IntentClassification/Act1TeachingDemoData.cs`
- `Assets/Scripts/Puzzles/IntentClassification/Act1GhostGeneralizationEngine.cs`
- `Assets/Tests/EditMode/Act1GhostGeneralizationEngineTests.cs`
- Unity-generated new `.meta` files for the new folders/scripts above.
- `Docs/codex_runs/M0-T45_001_act1_teaching_as_gameplay.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/Editor/Act1IntentClassificationPrototypeSceneBuilder.cs`
- `Docs/LEARNING_CONTENT.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `dotnet build Ghost.EditModeTests.csproj --no-restore`
- `dotnet build Ghost.Presentation.csproj --no-restore`
- `dotnet test Ghost.EditModeTests.csproj --no-build`
- `dotnet test Ghost.EditModeTests.csproj --no-build --list-tests`
- `git diff --check -- Assets\Presentation\Act1IntentClassification Assets\Presentation\GhostAvatar Assets\Scripts\Puzzles\IntentClassification Assets\Tests\EditMode Docs\LEARNING_CONTENT.md Docs\CODE_WALKTHROUGH.md Docs\UNITY_TEST_CHECKLIST.md`
- `git diff --name-only -- Assets\Presentation\Act2EntityExtraction Assets\Scripts\Puzzles\EntityExtraction Assets\Presentation\Act3DialogGraph Assets\Presentation\Fundamentals Assets\Presentation\Shell Backend ProjectSettings Packages`
- `rg -n "[^\x00-\x7F]" Assets\Presentation\Act1IntentClassification Assets\Presentation\GhostAvatar Assets\Scripts\Puzzles\IntentClassification\Act1TeachingDemoData.cs Assets\Scripts\Puzzles\IntentClassification\Act1GhostGeneralizationEngine.cs Assets\Tests\EditMode\Act1GhostGeneralizationEngineTests.cs`

## Test / Check Result

- `dotnet build Ghost.EditModeTests.csproj --no-restore` succeeded with 0 warnings and 0 errors.
- `dotnet build Ghost.Presentation.csproj --no-restore` succeeded with 0 warnings and 0 errors.
- `dotnet test Ghost.EditModeTests.csproj --no-build` exited 0 but produced no test output.
- `dotnet test Ghost.EditModeTests.csproj --no-build --list-tests` exited 0 but listed no tests, so it was not used as evidence of Unity EditMode test pass/fail.
- `git diff --check` reported no whitespace errors. Git printed CRLF conversion warnings for edited files.
- The prohibited-scope diff guard returned no files for Act 2, entity runtime logic, Act 3, Fundamentals, Shell, Backend, ProjectSettings, or Packages.
- The non-ASCII scan of changed Act 1 / Ghost face C# files returned no matches.
- Unity EditMode Test Runner: Not run — Unity Editor is not available in this Codex session.
- Unity Play Mode: Not run — Unity Editor is not available in this Codex session.

## Errors Encountered

- Initial `dotnet build --no-restore` failed because `Temp/obj/.../project.assets.json` did not exist.
- Regular `dotnet build` inside the sandbox failed because the sandbox could not read `C:\Users\fcxsw\AppData\Roaming\NuGet\NuGet.Config`.
- The elevated `dotnet build` commands restored local assets and succeeded.
- `dotnet test` could not provide useful Unity EditMode results from this generated project setup.
- Pre-existing dirty scene files were present in the worktree and were left untouched.

## Fixes Applied

- Added pure Act 1 teaching demo data and a deterministic generalization engine.
- Added EditMode test coverage for correct grouping, wrong-majority, tie, and unlabelled-pile outcomes.
- Added a reusable programmatic Ghost face with neutral, happy, confused, and sad moods.
- Rebuilt Act 1 presentation around intro failures, free training piles, purpose-label chips, Teach Ghost demo, revise/reteach loop, misleading-card highlights, and completion state.
- Updated the Act 1 scene builder's generated title, subtitle, and column proportions for the new layout.
- Updated learning content, code walkthrough, and Unity checklist for the M0-T45 Run 001 design.

## What Was Intentionally Not Changed

- Existing `IntentClassificationValidator`, `IntentClassificationSession`, `Act1IntentClassificationSampleData`, answer keys, and existing tests.
- Act 2 code, Act 2 runtime puzzle logic, Act 2 scene builder, Act 3, Fundamentals, Shell, Banter, Backend, ProjectSettings, Packages, Build Settings, and existing `.meta` files.
- Scene YAML was not hand-edited and the Act 1 scene builder was not run in this session.
- Act 2 redesign is intentionally deferred to M0-T45 Run 002.

## Remaining Risks

- Unity Play Mode verification is required for drag/drop behaviour, label dragging, Ghost face rendering, misleading-card highlights, revise/reteach loop, and 1920x1080 layout fit.
- Unity EditMode Test Runner should run the new `Act1GhostGeneralizationEngineTests`; `dotnet test` did not list Unity tests.
- New Unity `.meta` files were generated for new scripts/folders and should be reviewed with Unity's import result before commit.
- The generated `.csproj` files were locally synchronized so `dotnet build` would include the new files; they are not git-tracked source files in this worktree.

## Next Recommended Step

Run the M0-T45 Run 001 Unity checklist in the Editor. If accepted, Claude should review/close Run 001
and prepare Run 002 for the Act 2 errand/token-slot redesign.
