# M0-T45 Run 005: Drag Preview Cleanup / Lily Style Correction

## Task ID

M0-T45

## Run Number

005

## Date

2026-07-04

## Original Request / Prompt Summary

Respond to user Play Mode feedback: dragging Act 2 tokens left multiple preview boxes stuck in the
action card, and Lily's generated pixel portrait should be corrected to gold short hair, glasses, blue
suit jacket, white shirt, black long pants, and black high heels.

## Files Created

- `Docs/codex_runs/M0-T45_005_drag_preview_lily_style.md`

## Files Modified

- `Assets/Presentation/Act2EntityExtraction/Act2EntityTokenDragView.cs`
- `Assets/Presentation/Act2EntityExtraction/Act2EntitySlotDropTarget.cs`
- `Assets/Presentation/Act2EntityExtraction/Act2EntityTokenReturnDropTarget.cs`
- `Assets/Presentation/Characters/LilyPixelPortraitFactory.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `dotnet build Ghost.Presentation.csproj --no-restore`
- `dotnet build Ghost.Presentation.csproj`
- `dotnet build Ghost.EditModeTests.csproj --no-restore`
- `dotnet build Ghost.EditModeTests.csproj`

## Test / Check Result

- `dotnet build Ghost.Presentation.csproj --no-restore`: Failed before compilation because
  `Temp/obj/Ghost.Presentation/project.assets.json` was missing.
- `dotnet build Ghost.Presentation.csproj`: Failed in the sandbox because dotnet could not read the
  user NuGet config under `C:\Users\fcxsw\AppData\Roaming\NuGet\NuGet.Config`.
- `dotnet build Ghost.Presentation.csproj` with approved escalation: Passed, 2 warnings, 0 errors.
  Remaining warnings are existing obsolete Unity API warnings in Shell overlay and Act 3 files.
- `dotnet build Ghost.EditModeTests.csproj --no-restore`: Failed before compilation because
  `Temp/obj/Ghost.EditModeTests/project.assets.json` was missing.
- `dotnet build Ghost.EditModeTests.csproj` with approved escalation: Passed, 0 warnings, 0 errors.
- Unity Play Mode: Not run — Unity Editor was not available in this Codex session.
- Unity Test Runner: Not run — Unity Editor Test Runner was not available in this Codex session.

## Errors Encountered

- User reported that token dragging left multiple preview boxes stuck inside the Act 2 action card.
- The initial `dotnet build --no-restore` checks could not run because generated project asset files
  were missing.
- Non-escalated dotnet restore/build could not read the user NuGet config due sandbox access.

## Fixes Applied

- Added global active-preview tracking to `Act2EntityTokenDragView`.
- Cleared previews when a new drag starts, when a drag ends, when the source token is disabled or
  destroyed, and when slot/return drop targets receive a token.
- Updated Lily's generated pixel portrait colours and silhouette to match the user-corrected design:
  gold short hair, glasses, blue suit jacket, white shirt, black long pants, and black high heels.
- Updated walkthrough and manual checklist with Run 005 verification steps.

## What Was Intentionally Not Changed

- Existing validators, sessions, sample data, answer keys, and scoring logic were not modified.
- Backend, ProjectSettings, Packages, Build Settings, Fundamentals, and Act 3 logic were not modified.
- No external art asset was imported; Lily remains a runtime-generated original pixel sprite.
- Existing generated scene YAML changes already present in the worktree were not reverted.

## Remaining Risks

- Unity Play Mode must verify that preview cleanup covers quick repeated drag/drop actions in the
  actual EventSystem timing.
- Unity Play Mode must verify the corrected Lily sprite reads clearly at the portrait sizes used by
  Shell and ambient banter.

## Next Recommended Step

Ask Claude to review Run 005 against the user feedback, then run Unity Editor verification using the
M0-T45 Run 005 checklist before closure.
