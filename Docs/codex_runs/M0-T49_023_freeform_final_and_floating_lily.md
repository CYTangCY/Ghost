# M0-T49 - Run 023 - Free-Form Final Chapter and Floating Lily

## Task ID

M0-T49

## Run Number

023

## Date

2026-07-30

## Original Request / Codex Prompt Summary

Restore the more playful free-form Final Chapter instead of the Run 022 guided two-choice interface.
Make the answer cards clearer and shorter. Let Lily react frequently to the player's choices, ideally
with a different response for each action. Make Ask Lily in Chapters 4-6 and the Final Chapter use the
same floating window and small portrait as Chapters 1-3.

## Files Created

- `Docs/codex_runs/M0-T49_023_freeform_final_and_floating_lily.md`
- `tmp/freeform_final_floating_lily_sync_backup_20260730_1725/`, containing the replaced files from
  `D:\Code\Ghost`

## Files Modified

- `Assets/Scripts/Puzzles/VoicePipeline/Act6PipelineData.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6PipelineInteractionController.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6PipelineStaticPresenter.cs`
- `Assets/Presentation/Banter/AmbientBanterHook.cs`
- `Assets/Presentation/Banter/AmbientBanterPanel.cs`
- `Assets/Presentation/Banter/BanterData.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceStaticPresenter.cs`
- `Assets/Presentation/Act5TestingDebugging/Act5TestingStaticPresenter.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendStaticPresenter.cs`
- `Assets/Tests/EditMode/Act6PipelineStaticPresenterTests.cs`
- `Assets/Tests/EditMode/Act6PipelineValidatorTests.cs`
- `Assets/Tests/EditMode/LaterChapterHintContextTests.cs`
- `Backend/src/llmOrchestration.ts`
- `Backend/src/seedData.ts`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/LEARNING_CONTENT.md`
- `Docs/UNITY_TEST_CHECKLIST.md`
- The same implementation files and targeted documentation sections in `D:\Code\Ghost`

## Tests or Checks Run

- Built `Ghost.Runtime.csproj`, `Ghost.Presentation.csproj`, and `Ghost.EditModeTests.csproj` in the
  Codex worktree.
- Ran the complete Unity EditMode suite after the final reaction test was added.
- Confirmed the exact free-form board, concise-card, later floating-panel, and Final Chapter
  action-reaction tests appear in the Unity result XML.
- Ran `npm test` from `Backend/`.
- Ran `git diff --check` on the touched tracked source and documentation.
- Searched for discarded guided-step logic and embedded later-chapter Ask Lily buttons.
- Backed up and synchronized 14 explicit implementation/test files to `D:\Code\Ghost`, then compared
  SHA-256 hashes.
- Inspected the active Unity Editor log after its assembly reload.
- Built all three C# projects in `D:\Code\Ghost`.
- Human Play Mode check: Not run - the user needs to judge the visible layout, portrait placement,
  card clarity, Lily reaction timing, and puzzle enjoyment in the active Game window.
- Unity Test Runner in `D:\Code\Ghost`: Not run - the active project remained open in the user's Unity
  Editor, so the complete suite was run in the matching Codex worktree.
- Live Ollama response check: Not run - backend tests used the controlled unavailable-model fallback.

## Test / Check Result

- Unity EditMode: 96/96 passed.
- Backend routes: 10/10 passed.
- `ConfigureBoardRendersFixedEndpointsShortcutsAndThreeTests`: passed.
- `PaletteCardsUseConcisePlayerFacingText`: passed.
- `LaterScenesUseTheFloatingPortraitBanterPanel`: passed.
- `FinalChapterCardChoiceUpdatesFloatingLilyReaction`: passed.
- Runtime builds in both projects: passed with 0 warnings and 0 errors.
- Presentation builds in both projects: passed with 4 pre-existing obsolete-API warnings and 0
  errors.
- EditMode test assembly builds in both projects: passed with 0 warnings and 0 errors.
- All 14 synchronized implementation/test files matched by SHA-256.
- The active Unity Editor reloaded the scripts and showed no new C# compilation error in the
  inspected log.

## Errors Encountered

- The first restoration pass stopped because the backed-up Final presenter used
  `ClearChildren(transform)` rather than the expected helper signature.
- Mixed CRLF/LF line endings in backed-up and current source prevented two exact temporary-script
  replacements.
- The restored Final presenter initially lacked the Banter and Shell namespace imports required by
  the new current-state registration.
- The Unity command-line launcher returned before the background test process wrote its result XML.

## Fixes Applied

- Corrected the temporary restoration anchors and normalized touched source to LF before applying
  exact transformations.
- Added `Ghost.Presentation.Banter` and `Ghost.Presentation.Shell` imports to the Final presenter.
- Rebuilt the presentation and test assemblies after the import fix.
- Waited for the Unity result file and parsed the completed 96-test run rather than treating the
  launcher exit as the result.

## What Was Intentionally Not Changed

- The deterministic Final Chapter validator and its three visitor cases were retained.
- The free-form board still uses the fixed Visitor/Ghost endpoints, five main stages, and one backend
  side socket.
- Existing `Act6Pipeline` names were retained to preserve Unity asset identity.
- No ProjectSettings, Packages, Build Settings, or scene file was changed.
- No `.meta` file was edited, renamed, or deleted.
- No Chapter 1-3 Lily behaviour was replaced; their existing floating panel was extended to the later
  scenes.
- No dependency upgrade or forced audit fix was performed.
- The active Unity process was not stopped or restarted.

## Remaining Risks

- The floating 560x96 panel overlays the chapter canvas and is draggable, but its starting position
  still needs a human check against each later chapter at the target Game view size.
- The restored twelve-card board still needs a human readability check; automated tests only protect
  card count and copy length.
- Frequent Lily reactions may feel helpful or too frequent depending on play pace. This needs actual
  Play Mode judgement.
- Live Granite wording can vary and was not exercised in this run.
- The backend dependency audit findings reported in Run 022 remain open.
- The Codex worktree and `D:\Code\Ghost` remain separate dirty working trees.

## Next Recommended Step

Stop and re-enter Play Mode in `D:\Code\Ghost`. Open the Final Chapter, move several different cards,
run one partly wrong route, ask Lily, then repair the route. Confirm the floating portrait panel does
not cover important controls and that Lily's reactions help without revealing the exact order.
Repeat the floating-panel and Ask Lily check once in Chapters 4, 5, and 6.

## Chinese STAR

- **S 情境：** Run 022 的二選一介面較清楚，但失去自由排列的樂趣；後期 Ask Lily 也不像前三章
  的懸浮大頭貼視窗。
- **T 任務：** 恢復原本的完整排列玩法，同時縮短卡片文字，讓 Lily 隨玩家操作給不同提示，並
  統一七個教學／最終場景的 Ask Lily 外觀。
- **A 行動：** 還原十二張卡與完整路徑，重寫短標籤和一句工作說明；加入選卡、放卡、錯誤角色、
  重設、測試和播放反應；把 Chapters 4-6 與 Final 接到原有 Ambient Banter 懸浮視窗及狀態化
  Ask Lily。
- **R 結果：** 自由排列玩法已恢復，後四個場景會建立小大頭貼懸浮窗，Lily 能依操作更新台詞。
  Unity EditMode 96/96、backend 10/10，兩個專案均編譯成功，等待人工確認視覺與遊玩感受。
