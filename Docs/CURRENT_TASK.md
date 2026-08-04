# CURRENT_TASK.md

## ID

M0-T56 (Final Chapter integration redesign) — implemented, awaiting Editor verification

## Status

M0-T51 through M0-T55 are closed. M0-T56 was implemented on 2026-08-04 by Claude directly; see
`Docs/codex_runs/M0-T56_001_final_chapter_integration.md`.

**Workflow (user, 2026-08-03):** Codex is no longer used — a single run cost about £20. Claude writes
the code; the user runs Unity (scene builders, EditMode, Play Mode, screenshots) and reports back.
Claude cannot compile or render, so every change needs the user's Editor pass.

## What is waiting on the user

1. **Unity compile.** The Final Chapter's model, data, validator, controller and presenter were all
   replaced in one pass, deliberately, so the repo never sat in a half-migrated state. Claude cannot
   compile UnityEngine code; brace balance and removed-symbol sweeps were the only mechanical checks.
2. **EditMode suite.** 144 `[Test]` + 26 `[TestCase]` = **170 discrete cases**; the Final Chapter
   fixture contributes 20. The 184 figure in the M0-T52–T56 run log could not be reconciled against an
   attribute count and is not the expected number.
   - The one to watch is `ThePresenterRendersEveryStageOfEveryVisitorWithoutThrowing`: it builds the
     presenter, renders every stage of every visitor and solves all three. It is the only automated
     check on the new UI.
3. **Play Mode pass on the Final Chapter.** Expect a layout repair round — the new stage UI's heights
   were derived by adding up their contents, not by looking at them, and every chapter so far has
   needed one pass after that.

## Layout fix applied the same day (all seven chapters)

Each chapter's `Header` is a fixed 44px row whose `LayoutElement` never pinned `flexibleHeight`, while
its inner `HorizontalLayoutGroup` sets `childForceExpandHeight = true` — so the header reported
flexible height upward and swallowed the page's spare space. The blue conversation panel had the same
defect. Both are now pinned; every chapter's root layout centres its blocks with 14px between them.

## Then

- Archives for M0-T47, M0-T48, M0-T49 are still missing from `Docs/completed_tasks/`.
- M0-T56 closure archive once the user verifies.

## Acceptance Criteria

- Nothing renders below `GhostUITheme.TinySize` (14); no text overlaps, clips mid-word, or escapes its
  panel; no header collisions.
- Deterministic correctness unchanged; the LLM stays out of every scoring path.
- EditMode green; backend 10/10.
- Verified from in-level 1920x1080 screenshots — setup-screen captures are not acceptance evidence.
- The Final Chapter is solvable end to end in Play Mode, all three visitors, through to the ending.
