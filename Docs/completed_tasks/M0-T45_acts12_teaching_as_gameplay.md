# M0-T45 — Acts 1–2 Teaching-as-Gameplay Redesign + Shared Ghost Face

## Completion Status

Completed (Codex runs 001–005 + one user-authorized direct Claude edit for the Lily portrait polish).
Human Unity verification: the user play-tested iteratively across runs 001–005 (each round of Play Mode
feedback drove the next run) and accepted the final state ("這章也不錯"). Unity Play Mode / Test Runner
were never run inside Codex sessions (honestly recorded per run); Codex evidence is dotnet builds +
scope guards; behavioural evidence is the user's Editor testing.

## Date

2026-07-04

## Summary

Replaced the rejected M0-T36/T37 "unchanged mechanic + teaching text" approach with teaching-as-gameplay
for both acts, per the user's direction ("the teaching content should BECOME the game").

- **Act 1 — "Train Ghost to greet visitors"** (run 001): intro conversation where Ghost visibly fails by
  word-matching → the player free-clusters the 9 visitor messages into training piles and assigns
  purpose labels (deriving the intents themselves) → "Teach Ghost" replays UNSEEN test messages, with
  Ghost classifying via the deterministic plurality-of-related-training-cards rule
  (`Act1GhostGeneralizationEngine` + `Act1TeachingDemoData`, 4 EditMode tests); wrong answers highlight
  the misleading cards; revise-and-reteach loop; completion requires `IntentClassificationValidator`
  correct AND all demo messages correct.
- **Act 2 — "Ghost's errand"** (run 002): Lily onboarding beat + persistent objective strip (run-001
  lessons) → watch Ghost fail an errand → "Split" the sentence into word tokens → drag tokens into typed
  WHAT/WHERE/WHEN action-card slots (same `EntitySpan` model) → "Go, Ghost!" runs the errand with
  authored success/cute-failure outcomes per slot state (`Act2ErrandOutcomeEngine` +
  `Act2ErrandDemoData`, 4 EditMode tests; slot states and success both derive from
  `EntityExtractionValidator`) → lab/laboratory synonym resolution beat.
- **Shared Ghost face**: programmatic `GhostFaceView` (neutral/happy/confused/sad), no external art.
- **Play Mode feedback fixes** (runs 003–005): removed missing `UI/Skin/*.psd` sprite errors
  (runtime-generated white sprite); reusable `FloatingWindowDragHandle` — Lily chat AND ambient banter
  now draggable floating windows; clear `Complete Act` button ending Act 1 via the existing Shell
  debrief path; Act 2 failed errands return to an editable Fill phase with `Try again`; stuck drag
  previews fixed via global active-preview tracking; initial Lily pixel portrait
  (`LilyPixelPortraitFactory`) wired into `LilyDialogueFrame` + `AmbientBanterPanel` as fallback.
- **Lily portrait polish (Claude direct edit, explicitly user-authorized as an exception)**: rebuilt the
  portrait from 32×32 FillRects to a 48×48 string-pixel map, iterated visually before porting: gold bob
  with highlights, black-framed glasses with lens tint + visible eyes, blush, blue suit jacket with
  lapels/white shirt/buttons, slim black pants, black high heels. Original art (broad style cues only,
  no copyrighted character copied). `dotnet build Ghost.Presentation.csproj` clean (0 errors).

## Files Created / Modified (high level)

- New pure logic + tests: `Act1TeachingDemoData`, `Act1GhostGeneralizationEngine`,
  `Act1GhostGeneralizationEngineTests`; `Act2ErrandDemoData`, `Act2ErrandOutcomeEngine`,
  `Act2ErrandOutcomeEngineTests` (existing validators/sessions/sample data untouched — verified by
  git guards each run).
- New presentation: `GhostAvatar/` (GhostMood, GhostFaceView), `Common/FloatingWindowDragHandle`,
  `Characters/LilyPixelPortraitFactory`, Act 1 `LabelDragView`/`TeachingDropTarget`, Act 2
  `TokenDragView`/`SlotDropTarget`/`TokenReturnDropTarget` (+ new `.meta` files).
- Rebuilt: Act 1 + Act 2 presenters/controllers + both Editor scene builders; Banter hook/panel +
  LilyChatWindow (floating); LilyDialogueFrame (portrait fallback).
- Docs: LEARNING_CONTENT, CODE_WALKTHROUGH, UNITY_TEST_CHECKLIST; run logs
  `M0-T45_001…005` (all retained).
- Excluded from commits (shelved side-effects): the three dirty generated scenes.

## Claude Review Notes

- Deterministic rule intact end-to-end: Act 1 completion = existing validator + demo engine (same
  grouping source); Act 2 success = `EntityExtractionValidator.IsCorrect` AND per-slot states computed
  via the same validator on per-type subsets. All demo replies/outcomes are authored static data; the
  LLM never touches the loop.
- Run 001 critical check: demo `relatedCardIds` verified to exactly match
  `Act1IntentClassificationSampleData` card ids.
- Scope guards (validators/sessions/sample data, Act 3, Fundamentals, Backend, ProjectSettings,
  Packages) returned empty on every run.
- Run logs honest throughout (dotnet-only evidence; Unity steps "Not run" with reasons).

## Human Verification Result

Iterative Play Mode testing by the user across runs 001–005 drove fixes (completion path, retry,
draggable windows, sprite errors, drag previews, portrait style) and the final state was accepted:
"這章也不錯". Remaining known cosmetic item: the Lily portrait was then polished by Claude (above) and
should be glanced at in the Editor (Shell dialogue frame + in-act banter panel) at next Play Mode run.

## Next Task

M0-T39 — Chatbot planning mini-level (define Ghost's helper goal, choose the starting channel from
simple usage data, pick key topics, plan the human-handoff rule), taught as gameplay with visible
consequences — not text panels. Timeline note: deadline ≈ early August; after M0-T39, priority is
M0-T40 (Act 4 confidence/fallback — also delivers the guaranteed-minimum slider mechanic) and an early
WebGL build smoke test, then dissertation writing.
