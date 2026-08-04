# M0-T49 - Run 021 - Sync Final Chapter to Active Unity Project

## Task ID

M0-T49

## Run Number

021

## Date

2026-07-30

## Original Request / Codex Prompt Summary

Continue after the project-path audit by applying the redesigned Final Chapter to the Unity project
the user actually has open at `D:\Code\Ghost`.

## Files Created

- `D:\Code\Ghost\Assets\Tests\EditMode\Act6PipelineStaticPresenterTests.cs`
- `D:\Code\Ghost\Assets\Tests\EditMode\Act6PipelineStaticPresenterTests.cs.meta`
- `Docs/codex_runs/M0-T49_021_sync_final_chapter_to_active_unity_project.md`
- `tmp/final_chapter_sync_backup_20260730/` containing the replaced Final Chapter files from
  `D:\Code\Ghost`

## Files Modified

- `D:\Code\Ghost\Assets\Scripts\Puzzles\VoicePipeline\Act6PipelineModels.cs`
- `D:\Code\Ghost\Assets\Scripts\Puzzles\VoicePipeline\Act6PipelineData.cs`
- `D:\Code\Ghost\Assets\Scripts\Puzzles\VoicePipeline\Act6PipelineValidator.cs`
- `D:\Code\Ghost\Assets\Presentation\Act6VoicePipeline\Act6PipelineInteractionController.cs`
- `D:\Code\Ghost\Assets\Presentation\Act6VoicePipeline\Act6PipelineStaticPresenter.cs`
- `D:\Code\Ghost\Assets\Tests\EditMode\Act6PipelineValidatorTests.cs`
- `D:\Code\Ghost\Assets\Scenes\Act6VoicePipelinePrototype.unity`

## Tests or Checks Run

- Compared normalized source contents and semantic diff sizes before synchronization.
- Confirmed the support scripts not listed above differed only by line endings and were not copied.
- Backed up every replaced file from `D:\Code\Ghost`.
- Compared SHA-256 hashes for all eight synchronized files.
- Inspected the active Unity Editor log after asset refresh.
- Built `Ghost.Runtime.csproj`.
- Built `Ghost.Presentation.csproj`.
- Built `Ghost.EditModeTests.csproj`.
- Reran `Ghost.Tests.EditMode.Act6PipelineValidatorTests` in the unlocked Codex worktree containing
  the exact synchronized Final Chapter source.
- Reran `Ghost.Tests.EditMode.Act6PipelineStaticPresenterTests` in the unlocked Codex worktree.
- Confirmed the redesigned `Run all 3 tests`, learned responsibility, fixed endpoint, and shortcut
  content exists in `D:\Code\Ghost`.
- Confirmed the generated scene references `Act6PipelineStaticPresenter` through the preserved Unity
  asset GUID.
- Unity Test Runner in `D:\Code\Ghost`: Not run - the project was already open in the user's Unity
  Editor, so a second batchmode Unity process could not safely open the same project.
- Human Play Mode check: Not run - requires the user to view and interact with the active Unity Game
  window.

## Test / Check Result

- All eight synchronized files exactly match the reviewed Run 018 versions.
- Unity refreshed the assets and loaded `Assets/Scenes/Act6VoicePipelinePrototype.unity`; no compiler
  error appeared in the inspected Editor log.
- Runtime assembly build: passed with 0 warnings and 0 errors.
- Presentation assembly build: passed with 4 pre-existing obsolete-API warnings and 0 errors.
- EditMode test assembly build: passed with 0 warnings and 0 errors.
- Focused validator fixture in the matching worktree: 9/9 passed.
- Focused presenter smoke fixture in the matching worktree: 1/1 passed.
- The active project no longer contains the old Final Chapter implementation in the synchronized
  logic, controller, presenter, tests, or generated scene.
- These focused tests verify the synchronized rule and presenter source, but test execution inside
  `D:\Code\Ghost` and visible Play Mode behaviour are not yet claimed.

## Errors Encountered

- Two PowerShell comparison commands initially used an invalid direct pipeline after a `foreach`
  block and returned `An empty pipe element is not allowed`.

## Fixes Applied

- Stored the comparison objects in a variable before formatting them.
- Kept the synchronization list explicit and did not copy whole directories.

## What Was Intentionally Not Changed

- No existing `.meta` file was copied, deleted, renamed, or edited. The new presenter test's
  previously generated `.meta` file was added because no target metadata existed.
- No ProjectSettings, Packages, Build Settings, other chapter, backend, or database file was changed.
- The existing internal `Act6Pipeline` names were retained to preserve Unity asset identity.
- The active Unity process was not stopped or restarted.
- Documentation files in `D:\Code\Ghost` were not broadly overwritten because they contain other
  unsynchronised user work and require a separate reconciliation.

## Remaining Risks

- The redesigned board still needs a human Play Mode check for layout, drag interaction, partial
  failures, stale results, full success, and ending playback.
- Unity Test Runner has not been rerun in `D:\Code\Ghost`.
- The worktree and `D:\Code\Ghost` remain separate dirty working trees; future edits can diverge
  again unless one canonical project is used.
- Final Chapter documentation is current in the Codex worktree but still needs targeted
  reconciliation in `D:\Code\Ghost`.

## Next Recommended Step

In the already-open `D:\Code\Ghost` Unity Editor, open
`Assets/Scenes/Act6VoicePipelinePrototype.unity`, enter Play Mode, dismiss the onboarding, and confirm
the fixed Visitor/Ghost endpoints, learned-responsibility cards, shortcuts, backend action socket,
and three test cards are visible. Run one partial-failure case, revise it, confirm the old results
become stale, then run the correct path and ending. After that evidence, reconcile the Final Chapter
documentation and proceed to Ask Lily for the later chapters.

## Chinese STAR

- **S 情境：** 新 Final Chapter 已在 Codex worktree 完成，但使用者實際開啟的
  `D:\Code\Ghost` 仍是舊五元件關卡。
- **T 任務：** 在不覆蓋其他未提交工作的前提下，把完整的新 Final Chapter 套用到實際
  Unity project。
- **A 行動：** 逐檔比較、備份舊檔、同步 8 個明確檔案、驗證雜湊、確認 Unity 匯入、編譯
  三個相關 assembly，並檢查 scene 的 presenter GUID。
- **R 結果：** `D:\Code\Ghost` 已包含六章整合、三案例測試與 fixed endpoints 的新版
  Final Chapter；三個 assembly 編譯成功，等待 Unity Test Runner 和人工 Play Mode 確認。
