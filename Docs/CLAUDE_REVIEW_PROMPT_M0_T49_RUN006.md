# Claude Review Prompt — M0-T49 Run 006 Character Pixel-Style Unification

請先讀實際 repo 與 diff，再以 project commander / reviewer 身分審查；不要只相信摘要，不要自動 commit/push，也不要在 Unity 資源尚未匯入與 Play Mode 尚未驗證時關閉任務。

## Required Reading

- `AGENTS.md`
- `Docs/CURRENT_TASK.md`
- `Docs/LEARNING_CONTENT.md`（chapter-split override 仍是 authoritative）
- `Docs/codex_runs/M0-T49_005_remediation_pass.md`
- `Docs/codex_runs/M0-T49_006_character_pixel_style_unification.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`
- `git status` 與本次相關 diff

## User Direction

使用者認為 run 005 Lily 過度 AI-polished / 半寫實，而 Ghost 過度簡陋。兩者應統一成低解析、粗輪廓、有限色盤的 RPG 場景小人像素風。參考圖只提供 broad style language，不得複製角色身分或服裝。

Approved result:

- Lily: 96x128 full body + 96x96 portrait；高金髮馬尾、圓眼鏡、黑西裝外套、灰上衣、紅色 KCL 掛繩、炭灰長褲、棕色 Oxford shoes、tablet。
- Ghost: 四張 96x96 mood sprites；一致的白藍 sheet-ghost 身體、大深色眼睛、小手、波浪尾、藍紫陰影。Neutral / Happy / Confused / Sad 應容易辨識。
- 所有圖均為硬邊 alpha，只允許 0 或 255；point filtering；不得呈現光滑半寫實或重複舊眼睛/文字嘴型。

## Review Scope

### Assets

Review:

- `Assets/Resources/Characters/LilyPixelFullBody.png`
- `Assets/Resources/Characters/LilyPixelPortrait.png`
- `Assets/Resources/Characters/GhostPixelNeutral.png`
- `Assets/Resources/Characters/GhostPixelHappy.png`
- `Assets/Resources/Characters/GhostPixelConfused.png`
- `Assets/Resources/Characters/GhostPixelSad.png`

Confirm dimensions, alpha, transparent padding, consistent scale/palette, and that Unity import/meta state is valid after Refresh. The four Ghost PNG meta files were still pending at Codex handoff because Unity was open and had not auto-refreshed.

### Runtime integration

Review `Assets/Presentation/GhostAvatar/GhostFaceView.cs`:

- `GhostPixelSpriteFactory` is intentionally in the existing file to preserve Unity project-file compatibility and avoid a new script/meta dependency.
- It loads Texture2D resources, creates full-rect cached runtime Sprites, and forces point filtering.
- `GhostFaceView.SetMood` selects the proper image, hides old eye/mouth/mood-mark overlays, and restores the original programmatic view only if resources are missing.
- Confirm there is no memory churn from repeated mood switches and no stale overlay state.

Review:

- `LilyPixelPortraitFactory.cs`: full-rect runtime Sprite loading avoids stale crop data from the previous larger Lily PNG.
- `LilyDialogueFrame.cs`: serialized Ghost portrait wins; otherwise neutral Ghost image is used.
- `AmbientBanterPanel.cs`: same neutral Ghost fallback.
- Existing `GhostFaceView` usages across Chapter 0, Chapters 1-6, and Final should pick up the new art without scene YAML edits.

### Tests and docs

Review `ShellReturnToHubOverlayTests.cs` additions:

- six resource-resolution test cases;
- one `GhostFaceUsesPixelSpriteForEveryMood` view test;
- expected Unity full-suite discovery is at least 87 after prior run additions, but verify the actual count.

Confirm `CODE_WALKTHROUGH.md` accurately describes image-first/fallback behavior and that the single current M0-T49 checklist includes Lily/Ghost style checks without creating a second current end-to-end checklist.

## Evidence

- Ghost.Presentation.csproj: 0 errors; four pre-existing deprecation warnings.
- Ghost.EditModeTests.csproj: 0 errors, 0 warnings.
- Pillow check: Lily full body 96x128; the other five images 96x96; all six alpha sets are exactly `[0, 255]`; all corners transparent.
- `git diff --check`, non-ASCII C# scan, delete/rename guard: clean.
- No scene, ProjectSettings, validator, scoring, completion, Backend, or LLM code changed in run 006.

Do not claim:

- Unity EditMode passed: Not run — Unity was open and new Ghost assets had not been imported.
- Play Mode passed: Not run — human Game view verification remains required.
- Run 005 builders passed: still pending from the project lock.

## Required Response

1. **Findings** — P0/P1/P2/P3 with exact file/line evidence and player impact.
2. **Visual/Asset Verdict** — whether Lily and Ghost truly share a low-resolution RPG pixel language and remain readable at actual UI sizes.
3. **Runtime Verdict** — resource load, caching, fallback, overlay hiding, WebGL compatibility.
4. **Test Gaps** — exact Unity Refresh/EditMode/Play Mode steps still required.
5. **Protected-Path Audit** — confirm no unrelated gameplay or project settings changes.
6. **Exact Next Codex Prompt** — a narrowly scoped fix prompt for any real issue, otherwise a verification-only prompt.
7. **Closure Decision** — use `DO NOT CLOSE` until import, full EditMode, run 005 builders, and human visual checklist are complete.
8. **中文 STAR** — S情境 / T任務 / A行動 / R結果。

第一次 review 不要直接修改檔案；先回報 findings。
