# M0-T52 … M0-T56 — Run 001 — Claude direct implementation

## Task IDs

M0-T52, M0-T53, M0-T54, M0-T55 (partial), M0-T56 (partial)

## Date

2026-08-04

## Context

Codex was retired after M0-T51 (about GBP 20 per run). Claude writes the code directly; the user runs
Unity and reports back. Claude cannot compile or render, so pure logic is verified by executing it in a
standalone .NET console harness before hand-off, and everything visual needs the user's Editor pass.

## Completed

### M0-T52 — global interaction

- `FloatingWindowDragHandle.ClampToParent` ignored the window's anchors, so "Ask Lily" stopped short of
  the top in chapters that used a non-centre anchor. Maths extracted to a pure `Clamp` and verified
  across five anchor configurations; 7 EditMode tests added.
- Return to Hub now lands on chapter select (`GhostNarrativeState.RequestResumeAtHub`).
- A wrong answer highlights Ask Lily instead of forcing the window open, and the window later opens
  with that specific hint. One change point in `AmbientBanterPanel.RequestHint`; six chapters affected.
- The redundant "Basics" screen was removed entirely, including two `Configure` overloads that
  collapsed onto the same signature once its parameters went.
- Act 1 content varied away from a uniform lab theme.

### M0-T53 — Chapters 1 to 3

- **Chapter 1**: training piles became three side-by-side columns (the user's design call), which
  removed the overflow and the need for a scroll container. Pile contents are fully visible again -
  hiding them behind "+N more" had removed the only feedback the player had about their own work. A
  purpose label can now start a pile on its own. Phase-aware "Now:" banner; build machinery hidden
  before the Build phase.
- **Chapter 2**: three harder messages - the same word as a different entity in context, a full
  three-slot errand, and a decoy that looks like a room but names a tune.
- **Chapter 3**: a second intent (opening hours) with its own branch, progressive visitor arrival, and
  the node palette moved out of the presenter into `Act3DialogGraphSampleData` so it cannot drift from
  the graph the validator demands.

### M0-T54 — Chapter 4 redesign

Rebuilt twice. The first version (two handles, four visitors) was still solvable as arithmetic because
the scores were printed on screen and a perfect setting existed. The final design:

- The confidence score is a **proxy, not the truth**: one visitor scores 52 while being perfectly
  clear, another scores 68 while being genuinely ambiguous, so the ordering breaks on purpose.
- **No pair of handles can serve everyone** - proven by exhaustive search over all 5151 settings.
- Pass condition is a **hard floor** (the upset visitor reaches a person, the obvious question is not
  bounced, both bands have an action) plus a **trade-off scoreboard** that separates "made to repeat
  needlessly" from "answered on a guess".
- Scores stay hidden until the evening has been run once, so the first run is a judgement from reading
  the messages rather than arithmetic.

### M0-T55 — Chapter 5

- **Bidirectional wiring**: input ports can now start a drag and output ports can receive one, so a
  wire connects from either end. `UpdateWireDrag` in both presenters picks whichever end is the anchor;
  previously it read `activeOutputPort` unconditionally and the rubber band did not follow at all on a
  reverse drag.
- Board geometry: columns respaced, ports resized, card text shortened after the palette overflowed.

### M0-T56 — partial

- Lily's "I finally made a friend" is now a **murmur**: `Act6EndingBeat.IsMurmur` renders it italic, in
  soft ink, offset to one side, with a "(quietly, to herself)" stage direction.

## Deliberate deviations from the plan

- **Chapter 5 per-visitor buggy maps: not done, and not recommended.** The plan asked for one map per
  visitor with its own seeded fault. The existing single map already contains all three fault classes
  (swapped slot conditions, a branch wired to the wrong response, a missing intent branch) and four
  test conversations expose them individually. One system with several faults that tests localise is
  closer to real debugging than three separate toy graphs. The difficulty requirement - the failing
  list reports expected versus actual without naming the faulty node - was already satisfied.
- **Chapter 4's visitor-dots-on-the-axis widget** was implemented as two standard sliders plus a live
  per-visitor band prediction in the queue. Functionally equivalent; a custom widget was not worth the
  risk before the design settled.

## Verification

Pure logic was executed standalone before hand-off in every case:

| Area | Evidence |
| --- | --- |
| Drag clamp | 5 anchor configurations, all land the window exactly on the parent edge |
| Chapter 2 | 6 messages / 10 spans confirmed each a single whole token, no entity type used twice |
| Chapter 3 | Correct graph validates, 3 test cases route correctly, single-intent graph is rejected naming the missing intent, palette covers every node both ways |
| Chapter 4 | 0 of 5151 settings please everyone; 1830 clear the hard floor; the opening handles do not; every passing setting still mis-serves someone; both postures reachable |

**Not run by Claude:** Unity compilation, scene regeneration, the EditMode suite, and every visual
result. All of that is the user's Editor pass.

## Errors made and corrected during the run

Recorded because the pattern matters more than the individual bugs.

1. **`\n` in a Python-driven edit became a real newline twice**, cutting a C# string literal in half
   (Chapter 3, then Chapter 4). Both were compile errors the user hit. Now standardised on
   `System.Environment.NewLine`, which carries no escape through the tooling.
2. **A string replacement failed silently** because it had no assertion, so the Chapter 4 task text
   stayed stale and kept telling the player to attach everything up front.
3. **Chapter 1 was made worse twice** by deciding what the player needed to see without seeing it.
4. **Two Chapter 2 messages were authored unsolvable** - a multi-word span when the tokeniser splits on
   whitespace, and two same-type entities when a slot holds one token. Rule-level tests passed because
   they fed hand-built spans straight to the validator and never went through the interaction model.
   `EveryAuthoredMessageIsSolvableThroughTheTokenModel` now guards it.
5. **Chapter 3's palette was hand-written in the presenter**, so adding a required node could leave the
   chapter unsolvable. Fixed structurally by deriving the palette from the same data as the graph.

The recurring lesson: **a correct rule is not a reachable solution.** Three separate chapters shipped
puzzles whose passing state the player could not reach, while every rule-level test stayed green.

## Expected EditMode count

**184.** Any deviation should be reported with the failing test names.

## Next

`Docs/M0-T51_T56_EXPERIENCE_POLISH_PLAN.md` section M0-T56 - the Final Chapter integration redesign,
which now has to reflect the changed Chapters 1 to 5. See the "Final Chapter integration" section
appended to that plan.

## Chinese STAR Summary

- **S 情境**：Codex 退場後由 Claude 直接實作 M0-T52 至 M0-T56，使用者負責 Unity 驗證。
- **T 任務**：完成全域互動修正、第 1-3 章深度與版面、第四章重新設計、第五章雙向連線，並記錄最終章整合設計。
- **A 行動**：每一項純邏輯都先在獨立 .NET 環境執行驗證才交付；第四章以窮舉 5151 種設定證明「不存在完美解」；第三章把 palette 移入資料層讓漂移不可能發生；第二章補上可達性防呆測試。
- **R 結果**：T52、T53、T54 完成並經使用者逐項驗收；T55 雙向連線完成；T56 僅完成結局 murmur。過程中犯的五個錯誤已逐一記錄，共同教訓是「規則正確不等於解可達」。
