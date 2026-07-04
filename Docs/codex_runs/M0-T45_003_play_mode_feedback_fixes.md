# M0-T45 Run 003: Play Mode Feedback Fixes

## Task ID

M0-T45

## Run Number

003

## Date

2026-07-04

## Original Request / Prompt Summary

Respond to user Play Mode feedback after the M0-T45 teaching-as-gameplay redesign: remove repeated
missing `UI/Skin/*.psd` Console errors from the shared Ghost face, make Lily chat/hint-style support UI
movable as a floating window pattern, and add a clear Act 1 completion button after correct
classification/training.

## Files Created

- `Assets/Presentation/Common.meta`
- `Assets/Presentation/Common/FloatingWindowDragHandle.cs`
- `Assets/Presentation/Common/FloatingWindowDragHandle.cs.meta`
- `Docs/codex_runs/M0-T45_003_play_mode_feedback_fixes.md`

## Files Modified

- `Assets/Presentation/GhostAvatar/GhostFaceView.cs`
- `Assets/Presentation/Banter/LilyChatWindow.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`
- `Ghost.Presentation.csproj`

## Tests or Checks Run

- `dotnet build Ghost.Presentation.csproj --no-restore`
- `dotnet build Ghost.EditModeTests.csproj --no-restore`
- Scope guard: `git diff --name-only` against protected validator/sample-data files
- Scope guard: `git diff --name-only` against ProjectSettings, Packages, Backend, Act 3, and Fundamentals
- Search guard: `rg "UI/Skin/UISprite|UI/Skin/Knob" Assets/Presentation -g '*.cs'`

## Test / Check Result

- `dotnet build Ghost.Presentation.csproj --no-restore`: Passed, 5 warnings, 0 errors. Remaining
  warnings are obsolete Unity API warnings in existing Banter, Shell, and Act 3 files.
- `dotnet build Ghost.EditModeTests.csproj --no-restore`: Passed, 0 warnings, 0 errors.
- Protected validator/sample-data scope guard: Passed, no diffs reported.
- ProjectSettings/Packages/Backend/Act3/Fundamentals scope guard: Passed, no diffs reported.
- `UI/Skin` search guard: Passed, no remaining presentation C# references.
- Unity Play Mode: Not run — Unity Editor was not available in this Codex session.
- Unity Test Runner: Not run — Unity Editor Test Runner was not available in this Codex session.

## Errors Encountered

- User reported repeated Unity Console warnings/errors for missing built-in resources:
  `UI/Skin/Knob.psd` and `UI/Skin/UISprite.psd`.
- User reported the support/chat window could block the game view.
- User reported that Act 1 completion had no visible completion button after correct classification.
- The new `Assets/Presentation/Common/` folder initially had no `.meta` files until they were added in
  this run.

## Fixes Applied

- Replaced `GhostFaceView`'s old built-in UI skin sprite lookup with a cached runtime-generated
  1x1 white sprite.
- Added reusable `FloatingWindowDragHandle` for draggable UGUI floating windows with parent-canvas
  clamping.
- Changed `LilyChatWindow` from a fixed right-side overlay to a draggable floating window by attaching
  the drag handle to its header.
- Added `Complete Act` to the Act 1 completion state; clicking it sets the existing pending Act 1
  debrief and loads the Game Shell so the existing Shell flow marks completion.
- Updated documentation and Unity manual-test checklist for the new Play Mode feedback checks.

## What Was Intentionally Not Changed

- Existing intent/entity validators, sessions, sample data, answer keys, and existing tests were not
  modified.
- ProjectSettings, Packages, Backend, Fundamentals, Act 3, and Build Settings were not modified.
- Existing generated scene YAML changes already present in the worktree were not reverted.
- No quiz, LLM scoring, backend scoring, audio, external art asset, or new Act structure was added.
- Future hint panels were not separately rebuilt; the new floating-window drag handle is the reusable
  pattern for them.

## Remaining Risks

- Unity Editor Play Mode must verify that the Console no longer reports `UI/Skin` missing resources.
- Unity Editor Play Mode must verify exact pointer behaviour for dragging the Lily chat header across
  desktop Game views.
- The floating-window pattern is now implemented for Lily chat only; future hint windows need to attach
  `FloatingWindowDragHandle` when they are created.
- Act 1 completion uses the existing Shell pending-debrief path; Editor verification should confirm it
  matches the desired pacing from the hub.

## Next Recommended Step

Ask Claude to review Run 003 against the user Play Mode feedback, then run Unity Editor verification
using the M0-T45 Run 003 checklist before closure.
