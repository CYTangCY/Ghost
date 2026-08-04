# Claude Review Prompt: M0-T47 through M0-T49

請把以下內容當成 repo-aware project commander / reviewer 的正式審查任務。請先讀實際 repo、run logs、diff 與場景，不要只相信這份摘要，也不要直接關閉任務。

## 審查目標

完整審查 Codex 從 Act 4 到 Act 6、Chapter 0 開場故事、Final Chapter 終章所做的所有未提交工作，確認：

1. 教學內容與玩家操作是否清楚、可玩，而不是文字牆或假互動。
2. correctness 是否只由 deterministic validator / simulator 決定，LLM 與 UI 不得 score 或 gate。
3. Shell 導覽、章節完成、debrief、回程、Build Settings 與場景序列化是否正確。
4. Act 3 共用 wire 元件的修改是否沒有破壞 Act 3。
5. 已知 UI / state 問題是否必須先修，再決定能否 closure。
6. `CURRENT_TASK`、`ROADMAP`、`LEARNING_CONTENT`、`HANDOFF_LOG` 與 completed-task archives 應如何依使用者最後決策同步。

## 必讀檔案

先讀 repository instructions 與以下文件：

- `AGENTS.md`
- `Docs/CONFIRMED_PROJECT_CONTEXT.md`
- `Docs/ROADMAP.md`
- `Docs/CURRENT_TASK.md`
- `Docs/REQUIREMENTS.md`
- `Docs/LEARNING_CONTENT.md`
- `Docs/ARCHITECTURE.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`
- `Docs/AI_COLLABORATION_PROTOCOL.md`

再讀全部 implementation/debug logs：

- `Docs/codex_runs/M0-T47_001_act4_confidence_fallback_slice.md`
- `Docs/codex_runs/M0-T47_002_act4_instruction_clarity.md`
- `Docs/codex_runs/M0-T48_001_act5_testing_debugging_implementation.md`
- `Docs/codex_runs/M0-T48_002_act5_scene_generation_and_tests.md`
- `Docs/codex_runs/M0-T48_003_act5_wire_interaction_usability_fix.md`
- `Docs/codex_runs/M0-T49_001_act6_voice_pipeline_and_ending.md`
- `Docs/codex_runs/M0-T49_002_unity_scene_generation_and_verification.md`
- `Docs/codex_runs/M0-T49_003_chapter_structure_split.md`
- `Docs/codex_runs/M0-T49_004_detailed_chapter_split_review.md`

## 重要文件衝突與最終使用者決策

目前 `Docs/CURRENT_TASK.md` 仍停在 M0-T47，並保留舊的六章 ship plan，把 voice-pipeline capstone + ending 寫成 Chapter 6。這已不是使用者最後決策。

使用者在 2026-07-15 明確決定並批准：

- Chapter 0 = 開場故事，不是教學關、沒有 validator 或 score。
- Chapters 1-6 = 六個教學章。
- Chapter 6 = Backend Action and Response Generation 教學。
- Final Chapter = 五元件 voice pipeline capstone + ending / credits。
- Shell 的 optional `Ghost's Voice Basics` 仍是 reference overview，不是 Chapter 0。

這個 override 已寫入 `Docs/LEARNING_CONTENT.md`。請把它視為最新使用者決策，檢查 `ROADMAP` 與 `CURRENT_TASK` 是否需要由 Claude 正式修正；不要因 CURRENT_TASK 尚未更新而把 Chapter 0 / Chapter 6 split 當成應回退的變更。

保留的 `Act6VoicePipeline` 類別、資料夾與 `Act6VoicePipelinePrototype.unity` 名稱，是為了避免 rename/delete `.meta` 與破壞 Unity asset identity；玩家所見身份與 completion state 已改成 Final Chapter / `FinalChapterId`。

## 實作過程與審查範圍

### M0-T47 Act 4: Confidence and Fallback

初始需求是做 threshold-slider 教學：Ghost 會過度自信亂答，或 threshold 太高對所有人 fallback；一個困難／生氣案例必須 handoff 給 Lily。

Codex 新增：

- `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceModels.cs`
- `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceDemoData.cs`
- `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceValidator.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceInteractionController.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceStaticPresenter.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Editor/Act4ConfidencePrototypeSceneBuilder.cs`
- `Assets/Tests/EditMode/Act4ConfidenceValidatorTests.cs`
- `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity`

並修改 Shell scene names、narrative state、dialogue、Return overlay、hub presenter/builder、Build Settings、walkthrough 與 checklist。

Act 4 correctness：threshold 必須落在 authored acceptable range，fallback 與 handoff 必須接上，visitor outcomes 全部符合 authored expected route。LLM 不參與 scoring。

Run 001 驗證：

- Act 4 focused EditMode：5/5 passed。
- 當時 full EditMode：60/60 passed。
- Act 4 與 Shell scene builder 成功。
- Build Settings 只新增 Act 4 scene。
- Codex 沒有執行互動式 Play Mode。

使用者之後表示「看不懂這關在玩什麼」，因此 Run 002 只改善教學清楚度，沒有改 validator / authored scoring：

- 加入 persistent goal。
- 明寫 confidence score 與 threshold 比較規則。
- 加入三步操作指示。
- 加入 slider trade-off labels。
- queue 顯示 confidence labels。
- playback 顯示 score-versus-threshold route comparison。
- 用 30% threshold / 62% vague message 做具體例子。
- `Ghost.Presentation.csproj` build 0 errors。
- 因 Unity Editor lock，該 run 沒有 Test Runner 或 Play Mode 結果。

請特別審查：新增文字是否真的能在 1920x1080 放下、slider 是否有清楚因果、fallback/handoff wiring 是否真的可操作、Complete Act/debrief 是否只能在 validator success 後發生。

### M0-T48 Act 5: Testing and Debugging

目標是讓玩家對一個 pre-built faulty Act 3 dialog graph 執行完整 test suite，看 expected-versus-actual，修線，再 rerun 到全綠；不得重新發明 scoring。

Codex 新增：

- `Assets/Scripts/Puzzles/TestingDebugging/Act5TestingModels.cs`
- `Assets/Scripts/Puzzles/TestingDebugging/Act5BuggyGraphData.cs`
- `Assets/Scripts/Puzzles/TestingDebugging/Act5TestSuiteRunner.cs`
- `Assets/Presentation/Act3DialogGraph/IDialogGraphWireInteractionHost.cs`
- `Assets/Presentation/Act5TestingDebugging/Act5TestingInteractionController.cs`
- `Assets/Presentation/Act5TestingDebugging/Act5TestingStaticPresenter.cs`
- `Assets/Presentation/Act5TestingDebugging/Editor/Act5TestingPrototypeSceneBuilder.cs`
- `Assets/Tests/EditMode/Act5TestSuiteRunnerTests.cs`
- `Assets/Tests/EditMode/Act5TestingStaticPresenterTests.cs`
- `Assets/Scenes/Act5TestingDebuggingPrototype.unity`

為重用 Act 3 port/wire interaction，Codex 修改：

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInputPortView.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphOutputPortView.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`

只把 host typing 抽成 `IDialogGraphWireInteractionHost`；不應改 Act 3 pure logic、validator、simulator 或 gameplay semantics。請做 Act 3 regression review。

Act 5 pure smoke：

- seeded buggy graph = 0/4, `IsCorrect=false`。
- reference fixed graph = 4/4, `IsCorrect=true`。

Run 002 Unity 驗證：

- Act 5 focused EditMode：4/4 passed。
- 當時 full EditMode：64/64 passed。
- Act 5 scene 與 Shell scene 成功生成。
- Build Settings 正確加入 Act 5。

使用者上傳的 screenshot 顯示 committed wires 全部偏到 graph board 左下角，而且不知道怎麼玩。Run 003 找到 root cause：

- `GetPortLocalCenter` 回傳 wire-layer center coordinates。
- `DrawLine` 卻把 RectTransform anchor 放在 bottom-left。
- 所有線因此被多加 board pivot offset。

Codex 修正：

- DrawLine 改用 center anchors、source-side pivot、source anchored position、length 與 local rotation，對齊 Act 3 geometry。
- committed-wire rebuild 前強制更新 Canvas/root layout。
- socket hit target 20px -> 26px。
- 第一次測試前 output sockets muted，表示 editing locked。
- 加 persistent `TEST -> REPAIR -> RERUN` 指示。
- node 加 `LEFT input / RIGHT output`。
- primary actions 改為 `1. Run all 4 tests` 與 `3. Rerun all 4 tests`。
- 加 `Act5TestingStaticPresenterTests` 保護 centered wire-layer coordinate contract。

該修正當下因使用者仍在 Play Mode，只有 external compile 0 errors，沒有立即跑 Unity Test Runner。之後 M0-T49 的 full 77/77 已包含目前 test assembly，但仍請確認新 static presenter test 確實被發現並執行。使用者之後說「完成，開始建構第六章」，可視為方向接受，但 run log 沒有記錄正式逐項 Play Mode checklist 結果。

請特別審查：wire endpoints 是否真的貼 socket、temporary/committed wire 坐標一致、filled graph 是否可編輯、test results stale state 是否正確、Act 3 wire regression 是否安全。

### M0-T49 Runs 001-002: Voice Pipeline Capstone and Ending

Codex最初依舊版 CURRENT_TASK，把五元件 capstone 當成 Chapter 6：

- main path：UI input -> NLP engine -> dialogue management -> response generation -> UI output。
- side link：backend integration。
- deterministic message playback。
- Ghost full reply。
- personalized player-name ending、Lily closing、credits、skip 與 title return。

新增：

- `Assets/Scripts/Puzzles/VoicePipeline/Act6PipelineModels.cs`
- `Assets/Scripts/Puzzles/VoicePipeline/Act6PipelineData.cs`
- `Assets/Scripts/Puzzles/VoicePipeline/Act6PipelineValidator.cs`
- `Assets/Presentation/Act6VoicePipeline/IAct6PipelineInteractionHost.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6PipelineInteractionController.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6PipelinePartDragView.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6PipelineSlotDropView.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6PipelineStaticPresenter.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6EndingSequence.cs`
- `Assets/Presentation/Act6VoicePipeline/Editor/Act6VoicePipelinePrototypeSceneBuilder.cs`
- `Assets/Tests/EditMode/Act6PipelineValidatorTests.cs`
- `Assets/Scenes/Act6VoicePipelinePrototype.unity`

互動包含 drag/drop 與 click-select fallback、五個 stable main slots、backend side socket、reset/retry、first-broken-stage feedback、六步 playback、ending full/skip 共用完成路徑。

Run 002 Unity 驗證：

- pipeline focused tests：6/6 passed。
- 當時 full EditMode：71/71 passed。
- scene 與 Shell builder 成功，無 compiler errors。
- 沒有 Codex interactive Play Mode 視覺／pointer 驗證。

### M0-T49 Run 003: User-approved Chapter Split

使用者指出上述結構不對：Chapter 6 應是教學，另外要有 Chapter 0 開場與 Final Chapter 結局。使用者批准 Chapter 6 = Backend Action / Response Generation，舊 voice pipeline 移到 Final Chapter。

Codex先更新 `Docs/LEARNING_CONTENT.md` mapping，再實作。

#### Chapter 0 Opening Story

新增：

- `Assets/Presentation/Story/Chapter0StoryData.cs`
- `Assets/Presentation/Story/Chapter0StoryPresenter.cs`
- `Assets/Presentation/Story/Editor/Chapter0StorySceneBuilder.cs`
- `Assets/Scenes/Chapter0OpeningStory.unity`

內容為六個 authored story beats：late lab、Lily 介紹 Ghost、Ghost 語言混亂、玩家答應一起逐條訊息修復。沒有 validator 或 score；有 Continue、Skip opening、Enter the lab、speaker highlight、Ghost mood 與 player-name personalization。

Shell first-run naming / new account / empty account progress 會先進 Chapter 0；完成後用 pending-debrief 回 Shell。Hub 有 Replay Chapter 0。

#### Chapter 6 Backend Action and Response Generation

新增 pure logic：

- `Assets/Scripts/Puzzles/BackendResponse/Act6BackendResponseModels.cs`
- `Assets/Scripts/Puzzles/BackendResponse/Act6BackendResponseData.cs`
- `Assets/Scripts/Puzzles/BackendResponse/Act6BackendResponseValidator.cs`

新增 presentation：

- `Assets/Presentation/Act6BackendResponse/IAct6BackendInteractionHost.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendInteractionController.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendCardDragView.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendSlotDropView.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendStaticPresenter.cs`
- `Assets/Presentation/Act6BackendResponse/Editor/Act6BackendResponseSceneBuilder.cs`
- `Assets/Tests/EditMode/Act6BackendResponseValidatorTests.cs`
- `Assets/Scenes/Act6BackendResponsePrototype.unity`

教學鏈：

- DATA SOURCE = `Lab records`。
- ACTION = `Fetch lab closing time`。
- RESPONSE = `The lab closes at {closing_time}.`。
- backend result = `closing_time = 8 PM`。
- final reply = `The lab closes at 8 PM.`。

Palette 另有 room-directory / object-location distractors。玩家可 drag 或 click-select，Run 後 deterministic validator 檢查三個 placed IDs，成功後五步 playback，再經 Shell debrief 完成 Chapter 6。

#### Final Chapter Reclassification

修改 voice pipeline presentation / ending / Shell，使玩家所見 title、attempt/hint act ID、completion ID、menu builder、scene mapping 都改成 Final Chapter / `FinalChapterId`。內部 `Act6...` 名稱保留以維持 meta identity。

#### Shell and Build Settings

修改：

- `Assets/Presentation/Shell/ShellSceneNames.cs`
- `Assets/Presentation/Shell/GhostNarrativeState.cs`
- `Assets/Presentation/Shell/ShellDialogueData.cs`
- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Assets/Scenes/GameShellPrototype.unity`
- `ProjectSettings/EditorBuildSettings.asset`

預期 canonical scene order：

1. GameShellPrototype
2. Chapter0OpeningStory
3. Act1IntentClassificationPrototype
4. Act2EntityExtractionPrototype
5. Act3DialogGraphPrototype
6. Act4ConfidenceFallbackPrototype
7. Act5TestingDebuggingPrototype
8. Act6BackendResponsePrototype
9. Act6VoicePipelinePrototype (player-facing Final Chapter)
10. pre-existing SampleScene

Run 003 Unity 驗證：

- Chapter 0、Chapter 6、Final Chapter、Shell 四個 builders 成功。
- Chapter 6 backend focused：6/6 passed。
- Final pipeline focused：6/6 passed。
- full EditMode：77/77 passed。
- 0 compiler errors。
- 四個 affected scenes 都已生成並序列化 non-null chapter buttons。
- Codex 沒有執行 interactive Play Mode 視覺／pointer 驗證。

實作時遇到並修復的問題：

- Windows sandbox helper ACL 持續失敗，`apply_patch` 與普通 sandbox read 曾不可用；Codex使用 approved, narrowly scoped exact PowerShell writes。
- 一次大型 presenter write 超過 Windows command-length limit，改為分段寫入。
- Chapter 6 data/controller 有兩處 malformed quote，由 Unity compiler 抓到後修正。
- Shell exact replacement 遇到 mixed line endings，先 normalize LF 再改。
- 初次 Chapter 6 focused test 5/6：舊 NUnit 無法對 `IReadOnlyList` 使用 `Has.Count`；改為 `result.Errors.Count` 後 6/6。

## M0-T49 Run 004 詳細檢查：目前不可 closure 的已知問題

Codex 重新做 line-level review、scene serialization checks、layout calculation，並再次跑 full EditMode 77/77。以下問題尚未修：

### P1: Return to Hub 可把未完成章節標為完成

- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs` 在 Return button click 先呼叫 `SetPendingDebriefForActiveScene`。
- 返回 Shell 後 `GameShellPresenter.PlayPendingDebrief()` 會 consume pending ID 並 `MarkActCompleted(actId)`。
- 結果：玩家進入 Act 1-6 後可立刻 Return，跳過 puzzle 並完成章節。
- Final Chapter 因目前沒有 scene-to-act mapping，所以不受這個錯誤影響。

請設計正確 completion semantics。不能簡單假設所有舊章都由 presenter 設 pending；Act 2 目前可能依賴 Return path，需先讀實際 completion flow 再修。

### P1: Chapter 0 Lily portrait 完全透明

- `Chapter0StoryPresenter.CreateLily()` 用 `Color.clear` 建立 portrait Image。
- 設 sprite 後沒有把 Image tint alpha 恢復成 1。
- Lily sprite 生成成功但畫面不可見。

### P1: Shell hub 在 1920x1080 固定高度溢出

計算：

- Shell body 可用高度 = 664px。
- 現有 active hub content + screen padding = 704px，超出 40px。
- 顯示 chapter intro 的 Continue button 後 = 756px，超出 92px。
- bottom controls 因此可能壓進 Lily dialogue frame。

請重新安排高度或改成適當 scroll / compact layout；不要只隱藏內容。

### P2: Chapter 6 在 Run 前就揭露正解

- `Act6BackendInteractionController.IsRoleCorrect()` 直接比較 expected ID。
- `Act6BackendStaticPresenter.CreateRoleSocket()` 每次 render 都呼叫它。
- 正確卡放入後立即變 SuccessColor 並顯示 `VERIFIED`；錯誤卡立即 FailureColor。
- `LEARNING_CONTENT` 明定 placement 只由 `Act6BackendResponseValidator` 在 Run 時檢查。
- 目前玩家可逐張試放，在 Run 前取得答案，且 UI 自行判定 correctness。

### P2: Filled Chapter 6 socket 有雙 click-handler 風險

- filled socket 同時掛 `Act6BackendSlotDropView` 與 `Act6BackendCardDragView`。
- 兩者都實作 `IPointerClickHandler`。
- click filled socket 時，placement 與 card selection 可能在同一 event 依序執行。
- placement 又同步 `Notify -> RenderState -> Destroy/rebuild children`，可能讓 stale handler 繼續選取舊 card，造成 swap / deselect 不穩定。

請以 Play Mode 或 focused interaction test 證實並修正，不要只靠 validator tests。

### P2: 文件仍保留互相衝突的舊 checklist

`Docs/UNITY_TEST_CHECKLIST.md` 的 M0-T49 Run 001 區塊仍要求：

- Chapter 6 title = Repair Ghost's Voice。
- Chapter 6 load `Act6VoicePipelinePrototype`。
- ending 完成 Chapter 6。

Run 003 雖寫 supersedes，但兩套完整流程並存，容易讓人工驗收照錯。請將舊區塊清楚標成 historical / superseded，或重新整理成唯一 current checklist，不能刪除 run log 歷史。

## 自動驗證現況

最新確認：

- Full Unity EditMode：77/77 passed，0 failed，0 skipped。
- Chapter 6 BackendResponse focused：6/6 passed。
- Final Chapter VoicePipeline focused：6/6 passed。
- 四個 affected scene builder logs：0 compiler/exception matches，clean batchmode exit。
- Chapter0OpeningStory、Act6BackendResponsePrototype、Act6VoicePipelinePrototype、GameShellPrototype 各有 exactly one Main Camera、Canvas、EventSystem。
- 四個場景 Missing Script serialization entries = 0。
- 非 scene source diff check 沒有 whitespace errors；只有 Git line-ending conversion warnings。

這些結果不代表 UI 已可玩。Repo 目前沒有 PlayMode test assembly 或 screenshot/smoke harness；Codex 沒有執行 interactive Game view verification。

## 工作樹與保護規則

目前所有 Act 4-6、Chapter 0、Final Chapter 與部分 Act 3/scene changes 都仍是未提交狀態。請先讀 `git status` 與 `git diff`，不要 revert、rename、delete、stage、commit 或 push。

特別保護：

- 不要回退使用者／先前 agent 的 Act 1-3 scene changes。
- 不要 rename/delete Unity `.meta`。
- 不要手改 scene YAML。
- 不要修改 existing pure validators/sessions/sample data，除非查明是必要 bug fix且先說明。
- 不要讓 LLM 決定 scoring 或 completion。
- Build Settings 只能保留已批准的新增 scene entries，不得改其他 ProjectSettings。

## Claude 請執行的審查工作

1. 先以 code-review 形式列 findings，按 P0/P1/P2/P3 排序，每項附 file/line evidence、玩家影響、是否已有測試覆蓋。
2. 驗證上面四個 confirmed findings 與 filled-socket click risk，不要只重述 Codex 結論。
3. 審查 Act 4 slider/fallback/handoff、Act 5 test-repair-rerun、Chapter 6 backend-response、Final pipeline + ending 是否符合 LEARNING_CONTENT 與 deterministic rules。
4. 審查 Act 3 wire-host interface 修改是否無 regression。
5. 審查 Shell first-run Chapter 0、account restore、chapter debrief、Final completion、title return 與 scene order。
6. 決定是否還有漏掉的 gameplay/UI bugs，尤其 1920x1080 text clipping、pointer hit targets、deferred Destroy + synchronous render、return navigation、completion persistence。
7. 在問題修好前不要 archive/close M0-T47/M0-T48/M0-T49，也不要把 77/77 當成 Play Mode 通過。
8. 產出一份精確的 Codex remediation prompt，範圍至少包含四個 confirmed findings、必要 regression tests、scene regeneration 與 Play Mode checklist。
9. 修復經人類 Play Mode 驗證後，再由 Claude：
   - 更新 `ROADMAP` / `CURRENT_TASK`，正式反映 Chapter 0 / Chapters 1-6 / Final Chapter。
   - 將 M0-T47、M0-T48、M0-T49 依實際 closure 狀態歸檔到 `Docs/completed_tasks/`。
   - 更新 `Docs/HANDOFF_LOG.md`。
   - 清理 current checklist 的矛盾，但保留所有 `Docs/codex_runs/` 歷史。
   - 決定下一個 active task。

## Claude 回覆格式

請依序回覆：

1. **Findings**：嚴重度排序，file/line、影響、證據、建議修法。
2. **Verified Good**：已確認正確且不需改的部分。
3. **Test Gaps**：EditMode / PlayMode / manual checks 缺口。
4. **Documentation Reconciliation**：CURRENT_TASK / ROADMAP / LEARNING_CONTENT / checklist 怎麼同步。
5. **Codex Remediation Prompt**：可直接貼回 Codex 的完整修復 prompt。
6. **Closure Decision**：`DO NOT CLOSE` 或可 closure；若不可 closure，列出 blocking items。
7. **中文 STAR**：S 情境 / T 任務 / A 行動 / R 結果。

不要在沒有實際證據時宣稱 Play Mode passed，也不要替使用者自動 commit/push。
