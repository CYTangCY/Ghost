# M0-T15 — Run 002 — Act 2 entity extraction session test fix

## Task ID

M0-T15

## Run Number

002

## Date

2026-06-22

## Original Request / Codex Prompt Summary

Fix the user-reported Unity EditMode test failure in `Act2EntityExtractionSessionTests.AddSpan_WhenExactDuplicateIsAdded_LeavesCurrentSpanCountUnchanged`. The failure was `System.ArgumentException : Property Count was not found`, caused by using NUnit's `Has.Count` constraint on the `CurrentSpans` array snapshot. Keep the fix scoped to M0-T15 and do not expand into UI, scenes, asmdefs, ProjectSettings, backend, LLM, or unrelated files.

## Files Created

- `Docs/codex_runs/M0-T15_002_act2_entity_extraction_session_test_fix.md`

## Files Modified

- `Assets/Tests/EditMode/Act2EntityExtractionSessionTests.cs`

## Tests or Checks Run

- Read required task/context docs before implementation: `AGENTS.md`, `Docs/CURRENT_TASK.md`, `Docs/CONFIRMED_PROJECT_CONTEXT.md`, `Docs/AI_COLLABORATION_PROTOCOL.md`, `Docs/ROADMAP.md`, `Docs/REQUIREMENTS.md`, `Docs/LEARNING_CONTENT.md`, and `Docs/ARCHITECTURE.md`.
- Reviewed the user-reported Unity Test Runner failure: 32 tests passed, 1 failed before this fix.
- `rg "Has\.Count" Assets\Tests\EditMode\Act2EntityExtractionSessionTests.cs`
- `rg "UnityEngine|MonoBehaviour|SceneManager" Assets\Scripts\Puzzles\EntityExtraction\EntityExtractionSession.cs Assets\Tests\EditMode\Act2EntityExtractionSessionTests.cs`
- `rg "[ \t]+$" Assets\Tests\EditMode\Act2EntityExtractionSessionTests.cs`
- `Get-Command Unity -ErrorAction SilentlyContinue`
- `Get-Command Unity.exe -ErrorAction SilentlyContinue`

## Test / Check Result

- `Has.Count` scan found no remaining matches in `Act2EntityExtractionSessionTests.cs` after the fix.
- Runtime Unity dependency scan found no `UnityEngine`, `MonoBehaviour`, or `SceneManager` references in the M0-T15 runtime/test files.
- Trailing-whitespace scan found no matches in the modified test file.
- Unity command lookup found no `Unity` or `Unity.exe` command on PATH.
- Unity EditMode tests: Not run — Unity Editor/Test Runner is not available from this Codex shell session.
- Unity Play Mode: Not run — M0-T15 is scene-free logic only and Unity Editor is not available from this Codex shell session.

## Errors Encountered

- User-reported Unity EditMode failure before this fix: `System.ArgumentException : Property Count was not found` in `AddSpan_WhenExactDuplicateIsAdded_LeavesCurrentSpanCountUnchanged`.
- Unity command-line execution was unavailable from PATH, so Codex could not rerun Unity EditMode tests or Play Mode checks in this session.

## Fixes Applied

- Replaced `Assert.That(session.CurrentSpans, Has.Count.EqualTo(1));` with `Assert.That(session.CurrentSpans.Count, Is.EqualTo(1));` so the assertion uses the `IReadOnlyList<EntitySpan>.Count` value directly instead of NUnit's reflected `Count` property constraint.

## What Was Intentionally Not Changed

- No M0-T14 runtime files were edited.
- No runtime session logic was changed.
- No docs were changed in this debug run because the expected test name and documented behaviour are unchanged.
- No `.asmdef` files, ProjectSettings, Packages, Build Settings, scenes, or `.meta` files were edited.
- `Docs/CURRENT_TASK.md`, Act 1, Game Shell, backend, LLM, node graph, and later Acts were not changed.

## Remaining Risks

- The user needs to rerun the Unity EditMode tests to confirm the assertion fix resolves the reported failure.
- Unity Play Mode remains irrelevant for this scene-free logic task but can still be used as an optional Console sanity check.

## Next Recommended Step

Rerun the M0-T15 EditMode tests in the Unity Test Runner. If all tests pass, provide the updated Unity verification result to Claude for review, task archival, `Docs/HANDOFF_LOG.md`, and `Docs/CURRENT_TASK.md` advancement.
