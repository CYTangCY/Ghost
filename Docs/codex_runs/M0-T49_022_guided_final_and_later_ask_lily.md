# M0-T49 - Run 022 - Guided Final Chapter and Later Ask Lily

## Task ID

M0-T49

## Run Number

022

## Date

2026-07-30

## Original Request / Codex Prompt Summary

Simplify the Final Chapter because the previous board was too crowded, lacked guidance, used vague
answer cards, and displayed too much text. Also complete the Ask Lily interaction for Chapters 4-6
and the Final Chapter.

## Files Created

- `Assets/Tests/EditMode/LaterChapterHintContextTests.cs`
- `Assets/Tests/EditMode/LaterChapterHintContextTests.cs.meta`
- `Docs/codex_runs/M0-T49_022_guided_final_and_later_ask_lily.md`
- `tmp/guided_lily_sync_backup_20260730_1700/`, containing the replaced files from
  `D:\Code\Ghost`

## Files Modified

- `Assets/Scripts/Puzzles/VoicePipeline/Act6PipelineData.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6PipelineInteractionController.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6PipelineStaticPresenter.cs`
- `Assets/Tests/EditMode/Act6PipelineStaticPresenterTests.cs`
- `Assets/Tests/EditMode/Act6PipelineValidatorTests.cs`
- `Assets/Presentation/Banter/AmbientBanterPanel.cs`
- `Assets/Presentation/Banter/LilyChatWindow.cs`
- `Assets/Presentation/Banter/BanterData.cs`
- `Assets/Presentation/Backend/GhostBackendClient.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceInteractionController.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceStaticPresenter.cs`
- `Assets/Presentation/Act5TestingDebugging/Act5TestingInteractionController.cs`
- `Assets/Presentation/Act5TestingDebugging/Act5TestingStaticPresenter.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendInteractionController.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendStaticPresenter.cs`
- `Backend/src/llmOrchestration.ts`
- `Backend/src/seedData.ts`
- `Backend/tests/app.test.ts`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/LEARNING_CONTENT.md`
- `Docs/UNITY_TEST_CHECKLIST.md`
- The same implementation files and targeted documentation sections in `D:\Code\Ghost`

## Tests or Checks Run

- Built `Ghost.Runtime.csproj`, `Ghost.Presentation.csproj`, and `Ghost.EditModeTests.csproj` in the
  Codex worktree.
- Ran the complete Unity EditMode suite in the Codex worktree.
- Confirmed Unity imported `LaterChapterHintContextTests.cs` and included it in the test assembly.
- Ran `npm test` from `Backend/` after installing the existing package dependencies.
- Compared every synchronized implementation file against `D:\Code\Ghost`, backed up each replaced
  target, copied only the explicit file list, and compared SHA-256 hashes after the copy.
- Inspected the active Unity Editor log after the synchronized source caused an assembly reload.
- Built `Ghost.Runtime.csproj`, `Ghost.Presentation.csproj`, and `Ghost.EditModeTests.csproj` in
  `D:\Code\Ghost`.
- Ran `git diff --check` on the changed tracked source and documentation.
- Human Play Mode check: Not run - the user needs to judge the visible Final Chapter layout,
  guidance, difficulty, Ghost reactions, and Lily conversations in the active Unity Game window.
- Unity Test Runner in `D:\Code\Ghost`: Not run - the project was already open in the user's Unity
  Editor, so the complete suite was run in the matching Codex worktree instead.
- Live Ollama reply check: Not run - no live model request was required for the deterministic source
  and fallback-path verification.

## Test / Check Result

- Unity EditMode: 94/94 passed, including the new later-chapter hint-context fixture and updated Final
  Chapter presenter/validator fixtures.
- Backend routes: 10/10 passed.
- Worktree runtime build: passed with 0 warnings and 0 errors.
- Worktree presentation build: passed with 4 pre-existing obsolete-API warnings and 0 errors.
- Worktree EditMode test assembly build: passed with 0 warnings and 0 errors.
- Active-project runtime build: passed with 0 warnings and 0 errors.
- Active-project presentation build: passed with the same 4 pre-existing obsolete-API warnings and
  0 errors.
- Active-project EditMode test assembly build: passed with 0 warnings and 0 errors.
- The inspected active Unity Editor log showed the assembly reload and no C# compilation error.
- All 20 synchronized implementation/test files matched by SHA-256 after copying.

## Errors Encountered

- The backend initially had no local `node_modules`, so the first `npm test` command could not start.
- After dependency installation, one route test still expected only the original three learning
  content records even though four later chapters had been added.
- The Unity batch process completed successfully, but the result XML was checked before Unity had
  finished writing it.
- The first documentation patch command used the wrong temporary-script path.
- `npm install` reported 8 dependency audit findings: 1 low, 4 moderate, 2 high, and 1 critical.

## Fixes Applied

- Installed the backend packages from the existing lock file and reran the tests.
- Updated the learning-content route assertion from three to seven records.
- Rechecked the completed Unity log and parsed the result XML after it was written.
- Located the documentation patch script, applied it, and removed the temporary script.
- Updated the active project's documentation by replacing/appending only the Run 022 sections rather
  than copying the three dirty documentation files wholesale.

## What Was Intentionally Not Changed

- The deterministic Final Chapter validator and three authored visitor test cases remain the source
  of success and failure.
- The existing `Act6Pipeline` internal names were retained to preserve Unity asset identity.
- No ProjectSettings, Packages, Build Settings, or scene file was changed.
- Existing `.meta` files were not edited, renamed, or deleted. Only the new test metadata generated by
  Unity was added and synchronized.
- No earlier chapter puzzle rule was changed.
- The dependency audit findings were not automatically fixed because `npm audit fix --force` could
  introduce breaking dependency changes outside this task.
- The active Unity process was not stopped or restarted.

## Remaining Risks

- The new Final Chapter needs a human Play Mode check for text fit, visual balance, Ghost reactions,
  test-result readability, ending flow, and whether two choices per step give a suitable challenge.
- Ask Lily needs a Play Mode check in all four later chapters with both the backend running and stopped.
- Model wording can vary when Ollama is available; the static fallback is deterministic, but a live
  Granite response was not exercised in this run.
- The active project's generated IDE test project did not immediately list the new test source,
  although Unity imported and ran it in the matching worktree. Regenerating project files from Unity
  may be needed for IDE discovery.
- The backend dependency audit findings remain open.
- The Codex worktree and `D:\Code\Ghost` are separate dirty working trees and can diverge again.

## Next Recommended Step

In the already-open `D:\Code\Ghost` Unity Editor, open the Final Chapter and verify the six-step board
at the target Game view size. Make one shortcut choice, run the three visitor cases, confirm the board
returns to the broken step, then repair it and finish the ending. After that, open Ask Lily once in
each of Chapters 4, 5, 6, and the Final Chapter and check both a live backend reply and the static
fallback.

## Chinese STAR

- **S 情境：** Final Chapter 同時顯示十二張卡、六個位置與三個測試結果，資訊量太大；Chapter
  4-6 和 Final Chapter 的 Ask Lily 也沒有真正取得目前關卡狀態。
- **T 任務：** 在保留六章整合與三案例測試的前提下簡化最後章介面，並完成四個後期章節的
  Ask Lily。
- **A 行動：** 把修復拆成六個短步驟，每步只顯示兩個具體選項；錯誤測試後回到第一個有問題
  的步驟；讓四個 controller 建立狀態摘要，經 Lily chat 傳到 backend prompt，並補上本地
  fallback、測試、文件與主專案同步。
- **R 結果：** 最後章不再同時顯示大型卡片盤；Ask Lily 已可在 Chapter 4、5、6 和 Final
  Chapter 開啟並使用目前狀態。Unity EditMode 94/94、backend 10/10，兩個專案的 C# assembly
  均編譯成功，等待人工 Play Mode 確認實際好玩程度與視覺效果。
