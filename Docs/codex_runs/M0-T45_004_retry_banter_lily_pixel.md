# M0-T45 Run 004: Retry / Floating Banter / Lily Pixel Portrait

## Task ID

M0-T45

## Run Number

004

## Date

2026-07-04

## Original Request / Prompt Summary

Respond to user Play Mode feedback: after one failed assignment/run the player could not retry; the
normal bottom dialogue/banter window also needed to be draggable; and Lily should receive an initial
pixel portrait with the same cute project style as Ghost while only broadly referencing a timid young
researcher mood.

## Files Created

- `Assets/Presentation/Characters.meta`
- `Assets/Presentation/Characters/LilyPixelPortraitFactory.cs`
- `Assets/Presentation/Characters/LilyPixelPortraitFactory.cs.meta`
- `Docs/codex_runs/M0-T45_004_retry_banter_lily_pixel.md`

## Files Modified

- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
- `Assets/Presentation/Banter/AmbientBanterHook.cs`
- `Assets/Presentation/Banter/AmbientBanterPanel.cs`
- `Assets/Presentation/Shell/LilyDialogueFrame.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`
- `Ghost.Presentation.csproj`

## Tests or Checks Run

- `dotnet build Ghost.Presentation.csproj --no-restore`
- `dotnet build Ghost.EditModeTests.csproj --no-restore`

## Test / Check Result

- `dotnet build Ghost.Presentation.csproj --no-restore`: Passed, 2 warnings, 0 errors. Remaining
  warnings are obsolete Unity API warnings in existing Shell overlay and Act 3 files.
- `dotnet build Ghost.EditModeTests.csproj --no-restore`: Passed, 0 warnings, 0 errors.
- Unity Play Mode: Not run — Unity Editor was not available in this Codex session.
- Unity Test Runner: Not run — Unity Editor Test Runner was not available in this Codex session.

## Errors Encountered

- User reported that after one failed assignment/run, the level could not be retried.
- User reported that the normal bottom dialogue/banter window blocked the game and needed to be
  draggable.
- User requested an initial Lily pixel portrait reference direction.

## Fixes Applied

- Changed Act 2 failure flow so an incorrect errand keeps the failure outcome and slot result visible
  but immediately returns to editable Fill phase; the action button changes to `Try again`.
- Changed ambient banter creation from embedded layout placement to a canvas-level floating panel with
  `FloatingWindowDragHandle`.
- Added `LilyPixelPortraitFactory`, a runtime-generated original 32x32 point-filtered Lily pixel
  portrait with no external art asset.
- Wired the generated Lily portrait into `AmbientBanterPanel` and `LilyDialogueFrame` as the fallback
  when no serialized Lily portrait is assigned.
- Updated documentation and manual Unity checklist for retry, floating banter, and Lily portrait checks.

## What Was Intentionally Not Changed

- Existing validators, sessions, sample data, answer keys, and scoring logic were not modified.
- Backend, ProjectSettings, Packages, Build Settings, Fundamentals, and Act 3 logic were not modified.
- No external art asset was imported.
- The Lily portrait is not a direct copy of a Resident Evil character; it is an original Ghost-project
  pixel portrait using only broad timid researcher cues.
- Existing generated scene YAML changes already present in the worktree were not reverted.

## Remaining Risks

- Unity Play Mode must verify that the ambient floating panel drag feel works with the current canvas
  hierarchy in Act 1, Act 2, and Act 3.
- Unity Play Mode must verify the Act 2 retry flow after wrong WHAT/WHERE/WHEN slots and missing slots.
- The generated Lily portrait is an initial programmatic placeholder; later art direction may replace
  it with a hand-authored sprite.

## Next Recommended Step

Ask Claude to review Run 004 against the user feedback, then run Unity Editor verification using the
M0-T45 Run 004 checklist before task closure.
