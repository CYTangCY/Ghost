# Claude Review Prompt — M0-T49 Run 005 Remediation Pass

請把以下內容當成 repo-aware project commander / reviewer 的正式 code review 任務。先讀實際 repo、git diff、run log 與程式，不要只相信摘要，不要自動 commit/push，也不要在 Unity 驗證缺失時關閉任務。

## Authority and Required Reading

1. Read `AGENTS.md`.
2. Read `Docs/CURRENT_TASK.md`, but treat the chapter-split override in `Docs/LEARNING_CONTENT.md` as authoritative where they conflict.
3. Read `Docs/codex_runs/M0-T49_004_detailed_chapter_split_review.md`.
4. Read `Docs/codex_runs/M0-T49_005_remediation_pass.md`.
5. Read the current `Docs/CODE_WALKTHROUGH.md` and `Docs/UNITY_TEST_CHECKLIST.md`.
6. Inspect `git status` and all relevant diffs. The worktree contains user/agent changes from Acts 1-6, Chapter 0, and Final Chapter; do not revert, stage, commit, push, rename/delete `.meta`, or hand-edit scene YAML.

Authoritative chapter structure:

- Chapter 0: opening story, no validator/score.
- Chapters 1-6: teaching chapters.
- Chapter 6: backend action and response generation.
- Final Chapter: five-part voice pipeline capstone plus ending/credits.

## Review Scope

Review M0-T49 run 005 only as a remediation/art pass. No new gameplay mechanic was intended.

### 1. Completion semantics

- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs` must be pure navigation and must never set pending debrief or completion.
- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs` now shows `Complete Act` only in `Act2ErrandPhase.Complete`; verify the phase is reachable only after the final authored errand has passed the existing deterministic entity validator.
- Audit all completion call sites. Chapters 1-6 should set pending debrief only from validated presenter success/Complete state; Chapter 0 should finish from authored story completion; Final should call `MarkActCompleted(FinalChapterId)` only from the ending sequence.
- Review `Assets/Tests/EditMode/ShellReturnToHubOverlayTests.cs` as a compile/source convention guard and identify any stronger practical test that is necessary.

### 2. Chapter 0 Lily visibility and replacement art

- `Chapter0StoryPresenter.CreateLily()` now uses `LilyPixelPortraitFactory.GetFullBody()`, sets `portrait.color = Color.white`, and preserves aspect.
- Verify there is no other Chapter 0 `Color.clear` Image that later receives a sprite without restoring visible alpha.
- Review the project assets `Assets/Resources/Characters/LilyPixelFullBody.png` and `LilyPixelPortrait.png` plus their Unity import/meta state.
- The approved original Lily design is an adult postdoctoral lab senior with a high blonde ponytail, smaller round glasses, black blazer, red KCL lanyard, charcoal trousers, and brown British-style leather shoes. It should preserve only the calm/nerdy/slightly timid mood of the references, not their identity, uniform, logo, or setting.
- `LilyPixelPortraitFactory` should load Unity Sprite sub-assets reliably in Editor and WebGL, use point filtering, and retain the old code-drawn portrait only as a missing-resource fallback.

### 3. Shell 1920x1080 fit

- Review `GameShellSceneBuilder.ConfigureHubScreenLayout` and the compact hub blocks.
- Recalculate the authored active budget: 24 vertical padding + 36 spacing + 44 heading + 40 copy + 72 fundamentals + 52 story route + 240 lesson grid + 40 narrative Continue + 40 Back to Title = 588px, leaving 76px inside the 664px body.
- Verify internal card budgets also fit: lesson card = 16 padding + 26 title + 32 description + 34 button + 8 spacing = 116px; two rows plus 8 spacing = 240px. Confirm no runtime presenter changes invalidate this arithmetic.
- Do not accept hidden content as a fix. Check likely text clipping at 1920x1080.

### 4. Chapter 6 validator-only feedback

- Review `Act6BackendResponseModels.cs` and `Act6BackendResponseValidator.cs`. The only allowed pure-logic change is exposing validator-owned invalid role ids/per-role status. Authored cards, expected answers, errors, first-broken role, and overall scoring must remain unchanged.
- Confirm `Act6BackendInteractionController` no longer compares placed ids against expected ids for UI rendering.
- Confirm `Act6BackendStaticPresenter` renders filled, untested sockets as neutral with `PLACED - run the route to test this responsibility.`
- Per-slot VERIFIED/NEEDS REPAIR state must appear only when `LastValidation` exists and must be derived from `Act6BackendValidationResult`.
- Keep palette distractors and first-broken-stage feedback.

### 5. Chapter 6 one-click contract

- The chosen behavior is: palette cards can click-select; clicking a filled socket once returns its card to the palette.
- `Act6BackendCardDragView` on placed cards receives click-selection disabled.
- `Act6BackendSlotDropView` owns the filled-socket click action and uses `eventData.Use()`; drop also consumes the event.
- Review synchronous `Notify -> RenderState -> Destroy/rebuild` ordering and confirm a stale placed-card handler cannot select or move a second card.
- Review the new focused controller test and state whether an actual UGUI pointer test is still required.

### 6. Final Chapter Lily art without gameplay regression

- `Act6PipelineStaticPresenter` creates the Lily Image in the ending overlay.
- `Act6EndingSequence` shows it only during the Lily closing line, moves the dialogue to avoid overlap, hides Lily before credits, and leaves `FinalChapterId` completion/full-vs-skip behavior unchanged.
- Check Ghost, Lily, text, skip, and credits framing at 1920x1080.

### 7. Checklist reconciliation

- `Docs/UNITY_TEST_CHECKLIST.md` must mark Run 001 as `[SUPERSEDED by Run 003 chapter split - historical record]`.
- Run 003 is also historical after remediation.
- There must be exactly one current M0-T49 end-to-end checklist: Run 005, covering first-run Shell -> Chapter 0 -> hub -> Chapters 1-6 -> Final -> ending/credits -> title, plus every remediation reproduction step.
- Run logs must remain preserved.

## Verification Evidence and Gap

Codex actually ran and passed these builds with 0 errors:

- Ghost.Runtime.csproj
- Ghost.Presentation.csproj
- Ghost.EditModeTests.csproj
- Ghost.Presentation.Shell.Editor.csproj
- Ghost.Presentation.Act6Backend.Editor.csproj
- Ghost.Presentation.Act6.Editor.csproj
- Ghost.Presentation.Story.Editor.csproj

Static completion/forbidden-reference/non-ASCII/delete-rename/whitespace guards were clean. Both PNGs passed RGBA/transparent-corner/bounding-box checks.

Do not claim the following passed:

- Unity scene builders: Not run — a batchmode attempt aborted because another Unity instance had `D:/Code/Ghost` open.
- Unity EditMode tests: Not run — same project lock.
- Interactive Play Mode: Not run — human verification still required.

The expected next automated result after closing Unity is at least 80 EditMode tests (previous 77 plus three new tests), then successful Chapter 0, Chapter 6, Final, and Shell builders. Verify actual discovered counts rather than trusting the expectation.

## Required Claude Response

Return these sections in order:

1. **Findings** — P0/P1/P2/P3 order, with precise file/line evidence, player impact, and recommended fix.
2. **Verified Good** — behavior independently confirmed from code/data.
3. **Test Gaps** — especially project-lock-blocked builders/EditMode and required human Play Mode checks.
4. **Protected-Path Audit** — whether scenes, ProjectSettings, pure validators, and `.meta` handling stayed within scope.
5. **Exact Next Codex Prompt** — either a narrowly scoped fix prompt for real findings, or a verification-only prompt to close Unity, run all four builders, run focused/full EditMode, inspect logs/scenes, and preserve user changes.
6. **Closure Decision** — use `DO NOT CLOSE` while any code bug, scene regeneration, full EditMode run, or required human checklist remains unresolved.
7. **中文 STAR** — S情境 / T任務 / A行動 / R結果。

Do not edit files during the first review pass unless the user explicitly asks Claude to implement. Do not commit or push.
