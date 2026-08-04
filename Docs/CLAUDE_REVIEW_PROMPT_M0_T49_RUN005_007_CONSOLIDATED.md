# Claude Review Prompt — M0-T49 Runs 005-007 Consolidated Review

請把以下內容當成 repo-aware project commander / reviewer 的正式 code review 任務。先讀實際 repo、git status、git diff、run logs、程式與角色資產，不要只相信摘要。第一次 review 只回報 findings，不要直接修改、stage、commit、push、rename/delete `.meta`，也不要手改 scene YAML。

## Review Goal

完整審查 M0-T49 run 005-007：

1. run 005 的六項 gameplay/UI remediation 是否真正修好，而且沒有破壞 Chapters 1-6、Chapter 0、Final Chapter 或 Act 3 共用 wire interaction。
2. run 006 的 Lily/Ghost 低解析 RPG 像素風是否正確接入所有共用顯示位置。
3. run 007 是否只把 Lily 外套改為深藍色、鞋子改為黑色，沒有造成角色或 runtime regression。
4. 哪些項目已由 build/static evidence 證實，哪些仍必須經 Unity builders、EditMode 與人工 Play Mode 才能 closure。

## Required Reading

- `AGENTS.md`
- `Docs/CONFIRMED_PROJECT_CONTEXT.md`
- `Docs/ROADMAP.md`
- `Docs/CURRENT_TASK.md`
- `Docs/REQUIREMENTS.md`
- `Docs/LEARNING_CONTENT.md`
- `Docs/ARCHITECTURE.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`
- `Docs/codex_runs/M0-T49_004_detailed_chapter_split_review.md`
- `Docs/codex_runs/M0-T49_005_remediation_pass.md`
- `Docs/codex_runs/M0-T49_006_character_pixel_style_unification.md`
- `Docs/codex_runs/M0-T49_007_lily_colour_correction.md`

Also inspect all uncommitted changes. The worktree contains earlier user/agent work from Acts 1-6, Chapter 0, Final Chapter, scenes, tests, docs, and Build Settings. Do not revert or overwrite unrelated work.

## Authoritative Structure and Latest Visual Override

`Docs/CURRENT_TASK.md` still contains older wording. Where it conflicts, the 2026-07-15 chapter split in `Docs/LEARNING_CONTENT.md` is authoritative:

- Chapter 0 = opening story, no validator/score.
- Chapters 1-6 = teaching chapters.
- Chapter 6 = backend action and response generation.
- Final Chapter = five-component voice pipeline capstone plus ending/credits.

Latest Lily specification from run 007 supersedes older run 005/run 006 colour wording:

- high platinum-blonde ponytail;
- small round black glasses;
- deep navy-blue suit blazer;
- pale grey shirt;
- bright red KCL lanyard;
- charcoal trousers;
- black leather Oxford shoes;
- dark tablet;
- low-resolution 16-bit/RPG sprite style.

## Run 005 — Chapter Build-Out Remediation

### Completion semantics

Files:

- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
- `Assets/Tests/EditMode/ShellReturnToHubOverlayTests.cs`

Implemented:

- `Return to Hub` is pure navigation. The overlay no longer calls `SetPendingDebriefAct`, no longer creates a pending debrief, and no longer owns scene-to-act completion mapping.
- Act 2 now shows `Complete Act` only in `Act2ErrandPhase.Complete`. Clicking it sets the Act 2 pending debrief and loads the Shell.
- Added a source-convention EditMode guard asserting the Return overlay does not reference `SetPendingDebriefAct` or the removed helper.

Claude must independently audit every completion trigger:

- Chapters 1-6: only presenter success/Complete state reached from deterministic validation/testing.
- Chapter 0: authored story finish/skip path by design.
- Final Chapter: `Act6EndingSequence` marks only `FinalChapterId`.
- Immediate `Return to Hub` from any chapter must not complete it or play a success debrief.

### Chapter 0 Lily visibility

File:

- `Assets/Presentation/Story/Chapter0StoryPresenter.cs`

Implemented:

- Lily uses `LilyPixelPortraitFactory.GetFullBody()`.
- After assigning the Sprite, `portrait.color = Color.white` restores visible alpha tint.
- `preserveAspect` remains enabled.

Confirm no other `Color.clear` Image later receives a Sprite without restoring visible alpha.

### Shell 1920x1080 fit

File:

- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`

Implemented a compact, non-hidden hub layout. Authored active height budget:

```text
24 vertical padding
+ 36 spacing
+ 44 heading
+ 40 copy
+ 72 fundamentals
+ 52 story route
+ 240 lesson grid
+ 40 narrative Continue
+ 40 Back to Title
= 588px
```

Shell body is 664px, leaving 76px headroom.

Internal lesson-card calculation:

```text
16 padding + 26 title + 32 description + 34 button + 8 spacing = 116px
2 x 116 rows + 8 grid spacing = 240px
```

Verify actual Unity layout and runtime presenter behavior do not invalidate this arithmetic or clip text.

### Chapter 6 validator-only feedback

Files:

- `Assets/Scripts/Puzzles/BackendResponse/Act6BackendResponseModels.cs`
- `Assets/Scripts/Puzzles/BackendResponse/Act6BackendResponseValidator.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendInteractionController.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendStaticPresenter.cs`
- `Assets/Tests/EditMode/Act6BackendResponseValidatorTests.cs`

Implemented:

- Existing deterministic validator output was extended with invalid role ids / `IsRoleCorrect(roleId)`.
- Authored cards, expected answers, error messages, first-broken role, and overall `IsCorrect` scoring were intended to remain unchanged.
- Removed controller/UI-side comparison against expected card ids.
- Filled but untested sockets remain neutral and show `PLACED - run the route to test this responsibility.`
- Per-slot `VERIFIED` / `NEEDS REPAIR` appears only when `LastValidation` exists and comes from `Act6BackendValidationResult`.
- Palette distractors and first-broken-stage feedback remain.

Confirm the pure validator change only exposes validator-owned state and does not silently alter correctness.

### Chapter 6 one-click contract

Files:

- `Assets/Presentation/Act6BackendResponse/IAct6BackendInteractionHost.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendCardDragView.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendSlotDropView.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendInteractionController.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendStaticPresenter.cs`

Chosen behavior:

- Palette card click = select/deselect.
- Empty socket click with selected palette card = place card.
- Filled socket click = return exactly that card to palette.
- Placed `Act6BackendCardDragView` has click-selection disabled.
- Click/drop handlers consume `PointerEventData`.
- Placement edits clear stale validation.

Review synchronous `Notify -> RenderState -> Destroy/rebuild` behavior and confirm no stale handler performs a second action.

### Final Chapter Lily art

Files:

- `Assets/Presentation/Act6VoicePipeline/Act6PipelineStaticPresenter.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6EndingSequence.cs`

Implemented:

- Ending overlay creates a Lily Image.
- Lily appears only during Lily's closing line.
- Dialogue is repositioned to avoid Lily overlap.
- Lily hides before credits.
- Full ending and Skip ending should still share `FinalChapterId` completion behavior.

### Checklist reconciliation

`Docs/UNITY_TEST_CHECKLIST.md` should contain exactly one current M0-T49 end-to-end checklist:

- Run 001 marked historical/superseded by the chapter split.
- Run 003 marked historical/superseded by run 005 remediation.
- Run 005 marked `[CURRENT END-TO-END CHECKLIST]`.

Do not delete historical run logs.

## Run 006 — Lily/Ghost Pixel-Style Unification

### Final assets

- `Assets/Resources/Characters/LilyPixelFullBody.png` — 96x128.
- `Assets/Resources/Characters/LilyPixelPortrait.png` — 96x96.
- `Assets/Resources/Characters/GhostPixelNeutral.png` — 96x96.
- `Assets/Resources/Characters/GhostPixelHappy.png` — 96x96.
- `Assets/Resources/Characters/GhostPixelConfused.png` — 96x96.
- `Assets/Resources/Characters/GhostPixelSad.png` — 96x96.

All six currently have Unity-generated `.meta` files. Local image checks reported alpha values exactly `[0, 255]` and transparent corners.

Ghost direction:

- same chunky low-resolution RPG pixel language as Lily;
- consistent white-blue sheet-ghost body;
- preserves large dark eyes and cute expression style;
- adds small side arms, wavy tail, stronger dark outline, and blue-lavender shadow pixels;
- four moods should remain visually distinct.

### Runtime integration

Files:

- `Assets/Presentation/GhostAvatar/GhostFaceView.cs`
- `Assets/Presentation/Characters/LilyPixelPortraitFactory.cs`
- `Assets/Presentation/Shell/LilyDialogueFrame.cs`
- `Assets/Presentation/Banter/AmbientBanterPanel.cs`

Implemented:

- `GhostPixelSpriteFactory` lives in existing `GhostFaceView.cs` to avoid a new Unity script/meta/project-file dependency.
- It loads `Texture2D` resources, creates full-rect cached runtime Sprites, and forces `FilterMode.Point` / clamp wrapping.
- `GhostFaceView.SetMood` chooses the matching Ghost Sprite and hides old programmatic eyes, text mouth, and mood mark.
- If a Ghost resource is unavailable, the old programmatic Ghost view is restored as fallback.
- Shell dialogue and ambient banter use neutral Ghost art when no serialized Ghost portrait is assigned.
- `LilyPixelPortraitFactory` loads the 96px Lily textures and creates cached full-rect runtime Sprites; the old code-drawn Lily remains missing-resource fallback only.

Review caching, WebGL compatibility, texture/sprite lifetime, fallback restoration, and whether repeated mood switches can leak or leave stale overlays.

### Added test coverage

`ShellReturnToHubOverlayTests.cs` now also includes:

- six resource-resolution test cases;
- `GhostFaceUsesPixelSpriteForEveryMood` covering all moods, point filtering, Sprite selection, and old-eye hiding.

## Run 007 — Latest Lily Colour Correction

Only these project assets were changed in run 007:

- `Assets/Resources/Characters/LilyPixelFullBody.png`
- `Assets/Resources/Characters/LilyPixelPortrait.png`
- walkthrough/checklist wording

Final colour changes:

- black blazer -> deep navy-blue blazer;
- brown Oxford shoes -> black Oxford shoes.

Preserved:

- dimensions;
- silhouette and pose;
- high ponytail, glasses, expression;
- grey shirt, red KCL lanyard, charcoal trousers, tablet;
- chunky pixels, hard alpha, limited palette, transparent padding.

Run 007 did not change C#, Ghost assets, gameplay, scenes, ProjectSettings, validators, or `.meta` files.

## Verification Evidence

Actually run and passed:

- run 005: `Ghost.Runtime.csproj`, `Ghost.Presentation.csproj`, `Ghost.EditModeTests.csproj`, Shell editor, Chapter 6 Backend editor, Final Chapter editor, and Story editor builds — 0 errors.
- run 006 final: `Ghost.Presentation.csproj` and `Ghost.EditModeTests.csproj` — 0 errors.
- run 007: `Ghost.Presentation.csproj` — 0 warnings, 0 errors.
- `git diff --check` on affected files.
- delete/rename guard: 0.
- non-ASCII C# scan: clean.
- current checklist count: exactly 1.
- final Lily PNGs retain 96x128 / 96x96 dimensions and hard alpha.
- run 007 shoe-region check: 258 dark pixels, 1 brown-like palette pixel.
- all six role PNGs now have Unity-generated `.meta` files.

Do not claim these passed:

- Run 005 affected scene builders after remediation: Not run — previous batchmode attempt was blocked by the then-open Unity project.
- Complete Unity EditMode suite after run 005/run 006 tests: Not run.
- Interactive Unity Play Mode / 1920x1080 visual and pointer verification: Not run.

Unity currently appears closed, so the next implementation/verification pass can run batchmode.

Expected full EditMode discovery is at least 87 tests:

- prior verified suite: 77;
- run 005 additions: 3;
- run 006 additions: 7.

Verify actual discovered count rather than trusting this expectation.

## Required Unity Verification Before Closure

1. Run Chapter 0 builder.
2. Run Chapter 6 Backend Response builder.
3. Run Final Chapter builder.
4. Run Game Shell builder last.
5. Inspect builder logs for compiler errors, exceptions, failed execute methods, and abnormal exits.
6. Run focused Return/Chapter 6/character tests.
7. Run the complete EditMode suite; report actual total/pass/fail/skip.
8. Inspect affected scenes for exactly one Camera, Canvas, EventSystem and no missing scripts.
9. Complete the single current human Play Mode checklist at 1920x1080.

Human checks must include:

- immediate Return from every chapter does not complete it;
- Act 2 validated completion works;
- Chapter 0 Lily is visible;
- Shell hub including chapter intro Continue fits;
- Chapter 6 sockets remain neutral before Run;
- a filled socket click performs exactly one return-to-palette action;
- Act 3 wire drag still works;
- Lily/Ghost share the same crisp low-resolution style;
- all four Ghost moods switch without old eyes/text mouth overlay;
- Lily's final deep navy blazer is distinct from charcoal trousers;
- Lily's black shoes remain readable;
- Final full/skip ending and credits remain correct.

## Protected-Path Rules

- Do not revert user/agent scene changes.
- Do not rename/delete or hand-edit existing `.meta` files.
- Do not hand-edit scene YAML; use builders only.
- Do not change unrelated ProjectSettings, Packages, Backend, LLM, or Acts 1-3 logic.
- Do not change deterministic scoring except for a demonstrated bug and explicit approval.
- Do not stage, commit, or push during review.

## Required Claude Response

Return these sections in order:

1. **Findings** — P0/P1/P2/P3, exact file/line evidence, player impact, test coverage, recommended fix.
2. **Verified Good** — independently confirmed correct behavior.
3. **Visual/Asset Verdict** — Lily/Ghost style consistency, dimensions, alpha, current colours, runtime readability.
4. **Runtime Verdict** — completion, Chapter 6 source of truth, pointer contract, resource loading/caching/fallback, WebGL risk.
5. **Test Gaps** — builders, focused/full EditMode, scene serialization, human Play Mode.
6. **Documentation Reconciliation** — stale CURRENT_TASK/ROADMAP versus authoritative LEARNING_CONTENT and current checklist.
7. **Protected-Path Audit** — confirm no prohibited changes in runs 005-007.
8. **Exact Next Codex Prompt** — narrowly scoped fixes if findings exist; otherwise a verification-only prompt for builders/tests/scene guards.
9. **Closure Decision** — use `DO NOT CLOSE` until all blockers and required verification are resolved.
10. **中文 STAR** — S情境 / T任務 / A行動 / R結果。

