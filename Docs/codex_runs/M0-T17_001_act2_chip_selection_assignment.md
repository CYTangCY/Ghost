# M0-T17 — Run 001 — Act 2 chip selection assignment

## Task ID

M0-T17

## Run Number

001

## Date

2026-06-22

## Original Request / Codex Prompt Summary

Implement only the Act 2 span-annotation interaction slice: add single-chip selection, entity-type assignment, untagging, and visual state refresh to the existing Act 2 prototype scene path. Keep validation disabled and unwired, keep all assignment/removal routed through `EntityExtractionSession`, and do not modify Act 2 pure logic, Act 1, Game Shell, scenes, ProjectSettings, Packages, Build Settings, or existing asmdefs.

## Files Created

- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
- `Docs/codex_runs/M0-T17_001_act2_chip_selection_assignment.md`

## Files Modified

- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `git diff --check -- Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `Select-String -Path Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs -Pattern "\s$"`
- `rg "ValidateCurrentState|EntityExtractionValidator|Validate\(" Assets/Presentation/Act2EntityExtraction`
- Unity Play Mode: Not run — Unity Editor is not available from this Codex shell session.
- Scene generation: Not run — Unity Editor menu builders cannot be executed from this Codex shell session.
- Unity tests: Not run — M0-T17 is interactive UI work with no EditMode tests and Unity Test Runner is not available from this Codex shell session.

## Test / Check Result

- M0-T17 edited tracked files had no whitespace errors; Git emitted CRLF normalization warnings only.
- New controller file had no trailing-whitespace matches.
- Act 2 presentation validation search returned no matches, confirming the presenter/controller do not call validation.
- Unity Play Mode: Not run — Unity Editor is not available from this Codex shell session.
- Scene generation: Not run — Unity Editor menu builders cannot be executed from this Codex shell session.
- Unity tests: Not run — M0-T17 is interactive UI work with no EditMode tests and Unity Test Runner is not available from this Codex shell session.

## Errors Encountered

- A full `git diff --check` reported trailing whitespace in unrelated dirty Unity scene files (`Assets/Scenes/Act1IntentClassificationPrototype.unity` and `Assets/Scenes/GameShellPrototype.unity`). Those files were already outside the M0-T17 scope, so they were not edited. A path-scoped check was run for the M0-T17 edited files instead.

## Fixes Applied

- Added `Act2EntityExtractionInteractionController` to own the sample session, selected chip key, assigned entity types, and `StateChanged` notifications.
- Updated `Act2EntityExtractionStaticPresenter` to create the controller, make word chips and palette rows clickable, route tag/untag through `EntityExtractionSession`, add chip type badges, and refresh selected/tagged/untagged visuals from controller state.
- Kept the Validate button disabled with no validation listener.
- Updated project walkthrough and Unity checklist documentation for M0-T17.

## What Was Intentionally Not Changed

- No Act 2 pure logic files were modified.
- No Act 1 scripts, Game Shell scripts, existing scenes, ProjectSettings, Packages, Build Settings, existing asmdefs, or `.meta` files were modified.
- No validation feedback, scoring, save/load, backend, LLM, dialogue, node graph, multi-chip selection, animation, or later-Act behaviour was added.
- Existing unrelated dirty files in the worktree were not reverted or edited.

## Remaining Risks

- The interaction needs manual Unity verification because Play Mode and scene generation were not run in this shell session.
- If the saved Act 2 scene preview looks stale, the user must rerun `Ghost > Build Act 2 Entity Extraction Prototype Scene`.
- The chip badge layout is prototype UGUI and may need visual tuning in a later polish task after Play Mode inspection.

## Next Recommended Step

Open Unity, let scripts compile, rerun `Ghost > Build Act 2 Entity Extraction Prototype Scene` if the scene preview is stale, then verify chip selection, type assignment, untagging, multi-tag behaviour, disabled Validate placeholder, no Console errors, and that the Act 2 scene is not in Build Settings.
