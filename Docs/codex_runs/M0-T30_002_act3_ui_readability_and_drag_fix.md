# M0-T30 — Run 002 — Act 3 UI readability and drag fix

## Task ID

M0-T30

## Run Number

002

## Date

2026-06-23

## Original Request / Codex Prompt Summary

User feedback after the first M0-T30 implementation: the Act 3 screen looked too cluttered, node/card information and naming were confusing, palette cards were not categorized, internal ids such as `find_object` looked like file names rather than UI text, node cards could not be freely moved, and the level objective/gameplay was hard to understand and not fun. Implement a scoped M0-T30 UI readability fix without changing Act 3 pure logic, backend/LLM scope, Game Shell, Build Settings, ProjectSettings, Packages, or `.meta` files.

## Files Created

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphNodeDragView.cs`
- `Docs/codex_runs/M0-T30_002_act3_ui_readability_and_drag_fix.md`

## Files Modified

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
- `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Re-read required active-task/project docs and current Act 3 UI files.
- `rg "find_object|answer_object_location|ask_for_room|IntentBranch|SlotCheck|SlotPresent|SlotMissing|Validate graph|Dialog Graph" Assets\Presentation\Act3DialogGraph Docs\UNITY_TEST_CHECKLIST.md Docs\CODE_WALKTHROUGH.md`
- `git diff --check -- Assets\Presentation\Act3DialogGraph\Act3DialogGraphInteractionController.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphStaticPresenter.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphNodeDragView.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphOutputPortView.cs Assets\Presentation\Act3DialogGraph\Act3DialogGraphInputPortView.cs Assets\Presentation\Act3DialogGraph\Editor\Act3DialogGraphPrototypeSceneBuilder.cs Docs\CODE_WALKTHROUGH.md Docs\UNITY_TEST_CHECKLIST.md`
- Trailing-whitespace `Select-String` scan for the changed Act 3 presentation scripts and updated docs.
- `git diff -- Assets\Scripts\Puzzles\DialogGraph`
- `rg "EditorBuildSettings|BuildSettings|UnityWebRequest|OpenAI|LLM|backend" Assets\Presentation\Act3DialogGraph`
- `dotnet build Ghost.Presentation.csproj --no-restore`
- Unity scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- Player-facing presentation-source scan: passed for Act 3 presentation scripts; raw ids are no longer used in visible Act 3 UI strings. Matches remain only in older checklist/history docs, core terminology, enum/type names, or internal code identifiers.
- `git diff --check`: passed; only line-ending warnings were reported.
- Trailing-whitespace scan: passed; no matches.
- Pure Act 3 logic diff check: passed; no changes under `Assets/Scripts/Puzzles/DialogGraph`.
- Act 3 presentation scope scan: passed; no Build Settings, LLM, backend, OpenAI, or network references in `Assets/Presentation/Act3DialogGraph`.
- `dotnet build Ghost.Presentation.csproj --no-restore`: passed with 5 existing obsolete `FindFirstObjectByType` warnings and 0 errors.
- Unity scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- Initial drag-start design selected the card before moving it, which would trigger a full graph refresh and destroy the dragged card. The drag view was adjusted so dragging moves the existing card without selecting/rebuilding it.

## Fixes Applied

- Replaced player-facing raw-id labels with readable names: `Start here`, `Recognize request`, `Check room`, `Answer location`, and `Ask which room`.
- Categorized palette cards into Flow, Check, and Reply.
- Simplified node-card detail text so each card explains its purpose in one short line.
- Added `Act3DialogGraphNodeDragView` so placed node cards can be freely moved on the board.
- Stored node-card positions in `Act3DialogGraphInteractionController` presentation state, separate from `DialogGraphSession`.
- Updated wire/connection labels to readable text such as `next`, `room yes`, `room no`, `room known`, and `room missing`.
- Updated the objective, subtitle, test-case panel, validate button label, feedback copy, walkthrough, and checklist.

## What Was Intentionally Not Changed

- Act 3 pure logic/session files under `Assets/Scripts/Puzzles/DialogGraph`.
- Act 1 and Act 2 scripts.
- Game Shell scripts or act list.
- ProjectSettings, Packages, Build Settings, `.meta` files, and unrelated scenes.
- Backend, LLM, save/load, scoring persistence, final art, Act 3 Shell integration, and Act 4-6 node graph scope.

## Remaining Risks

- Human Unity Play Mode verification is still required for actual drag feel, node layout, line redrawing, and readability.
- The user must rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene` to refresh the generated scene if it looks stale.
- Unity may have generated untracked `.meta` files for new scripts and scene/project side effects while the Editor was open; these should be reviewed and scoped carefully before commit.

## Next Recommended Step

Rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene` in Unity, open `Assets/Scenes/Act3DialogGraphPrototype.unity`, and verify the updated M0-T30 checklist for categorized palette labels, free node dragging, readable cards, wire creation/removal, and deterministic validation feedback.
