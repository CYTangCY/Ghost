# M0-T35 — Run 001 — fundamentals teaching

## Task ID

M0-T35

## Run Number

001

## Date

2026-06-27

## Original Request / Codex Prompt Summary

Implement a compact, playable chatbot-fundamentals teaching sequence in the Game Shell. The sequence must teach IBM course fundamentals through Ghost problems, Lily's short in-fiction explanations, small player actions, and visible consequences. It must not change Acts 1-3 puzzle validators, sessions, or rules.

## Files Created

- `Assets/Presentation/Fundamentals/ChatbotFundamentalsData.cs`
- `Assets/Presentation/Fundamentals/ChatbotFundamentalsPresenter.cs`
- `Docs/codex_runs/M0-T35_001_fundamentals_teaching.md`

## Files Modified

- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Docs/LEARNING_CONTENT.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read required task/context docs before implementation.
- Ran `git diff --name-only` to review tracked modified files.
- Ran `rg` checks to confirm the new/modified presentation files do not reference puzzle logic folders.
- Ran `Select-String -Pattern '[^\x00-\x7F]'` on the new fundamentals C# files and modified Shell C# files to check for accidental non-ASCII characters.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity Test Runner: Not run — Unity Editor test runner was not executed in this Codex session.
- Scene generation / menu builder: Not run — Unity Editor menu builder was not executed in this Codex session.

## Test / Check Result

- Static file-scope checks found no references from the M0-T35 presentation changes into `Assets/Scripts/Puzzles`.
- Non-ASCII check on the new/modified C# files returned no matches.
- Not run — Unity Editor Play Mode was not executed in this Codex session.
- Not run — Unity Editor test runner was not executed in this Codex session.
- Not run — Unity Editor menu builder was not executed in this Codex session.

## Errors Encountered

- A first docs patch against `Docs/LEARNING_CONTENT.md` failed because the existing file contains mojibake characters around old arrow/dash text.
- One exploratory PowerShell regex command for non-ASCII checking had a malformed string literal.

## Fixes Applied

- Updated `Docs/LEARNING_CONTENT.md` with additive M0-T35 clarification notes instead of rewriting the mojibake-heavy legacy paragraph.
- Re-ran the non-ASCII check with a safer `Select-String` command.
- Added backward-compatible `GameShellPresenter.Configure(...)` overload forwarding so older builder-style calls still compile.

## What Was Intentionally Not Changed

- Did not change any Act 1, Act 2, or Act 3 puzzle validators, sessions, data, or rules.
- Did not edit `Assets/Scripts/Puzzles/`.
- Did not edit ProjectSettings, Packages, Build Settings, `.meta` files, backend code, or scene YAML.
- Did not implement Act 8 capstone depth, watsonx product walkthrough content, quizzes, save/load changes, or new backend/LLM behaviour.

## Remaining Risks

- Unity compilation and Play Mode layout must be verified in the Editor.
- The generated shell scene will not show the new fundamentals entry until `Ghost > Build Game Shell Scene` is run.
- The compact fundamentals screen may still need small visual tuning after real 16:9 and smaller-window Play Mode checks.

## Next Recommended Step

Run `Ghost > Build Game Shell Scene` in Unity, enter Play Mode, verify the `Ghost's Voice Basics` sequence covers all six fundamentals, and confirm Acts 1-3 still launch unchanged.
