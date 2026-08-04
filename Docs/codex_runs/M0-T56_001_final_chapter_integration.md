# M0-T56 Run 001 — Final Chapter integration redesign

## Task ID

M0-T56 (Final Chapter capstone rebuild)

## Date

2026-08-04

## Implemented by

Claude, directly. Codex is no longer in the workflow.

## Context

The Final Chapter was written before Chapters 1–5 changed. It asked the player to pick one option in
each of six categories for five visitors, all of them lab-themed, and it settled confidence with a
threshold slider bounded by a per-visitor `[ThresholdMinimum, ThresholdMaximum]` band.

That slider is the design Chapter 4 was rebuilt twice to remove: the screen shows a number, a perfect
answer exists, and the player can solve it by arithmetic without reading the request. The capstone was
reproducing the exact mistake the chapter before it now teaches against.

The user was told the risk of rebuilding the capstone this close to the delivery constraint recorded
in `Docs/ROADMAP.md:39` (deadline ≈ early August 2026) and chose the full redesign per specification.

## What changed

### Structure: five visitors to three, escalating

| Visitor | Stages | What it is for |
| --- | --- | --- |
| Courier | Intent, Details | Establishes the loop. Only has to be understood. |
| Student | Intent, Details, Dialogue | Names two machines of the same type, so a route has to be built. |
| Lily | Intent, Details, Backend, Confidence, Response | The whole machine at once. |

Escalation is asserted, not assumed: `TheThreeVisitorsEscalateRatherThanRepeat` fails if a later
visitor does not ask for strictly more than the one before it.

### Each stage now runs the mechanic its chapter teaches

- **Intent** — four plausible options: a near-miss, a too-broad one, and one belonging to another
  visitor. No filler; every wrong pick has its own failure reply.
- **Details** — Chapter 2's token rule. Sentence fragments drag into named slots, one fragment per
  slot, with decoys that are specifically rather than generically wrong ("reception" is one of the two
  options the courier is offering, not where the parcel goes).
- **Dialogue** — an ordered route assembled from a palette derived from the same data as the puzzle,
  so the Chapter 3 drift failure cannot recur.
- **Confidence** — replaced the threshold slider with Chapter 4's three-way call: answer now, ask one
  more question, hand over. Each option states its cost on screen.
- **Backend** — attach only what the answer depends on. Attaching Lily's direct messages is rejected.
- **Response** — Chapter 6's three responsibilities, three slots, reversible placement.
- **Run** — Chapter 5's habit. The player runs the conversation once and steps through the trace,
  which stops at the first fault rather than cascading.

### The two designed traps

1. **Lily's confidence is 78%** — high enough that reading the number says "just answer". The correct
   call is to ask again, because a required slot has two candidates in her message. The number is a
   proxy for wording, not for whether the request is decided. `ReadingTheConfidenceNumberGivesTheWrongCall`
   fails if the score ever points at the right answer.
2. **Lily has a slot nothing can fill.** `slot_which_job` has no expected fragment; leaving it empty is
   the correct answer, and dropping "the thing I left running" into it is rejected with the reason.
   `CanRunTest` is keyed on stages *visited*, not slots *filled*, so the run button does not give this
   away by staying greyed out.

## Files changed

- `Assets/Scripts/Puzzles/VoicePipeline/FinalChapterConversationModels.cs` — rewritten
- `Assets/Scripts/Puzzles/VoicePipeline/FinalChapterConversationData.cs` — rewritten
- `Assets/Scripts/Puzzles/VoicePipeline/FinalChapterConversationValidator.cs` — rewritten
- `Assets/Presentation/Act6VoicePipeline/FinalChapterConversationController.cs` — rewritten
- `Assets/Presentation/Act6VoicePipeline/FinalChapterConversationPresenter.cs` — host methods,
  objective text, conversation panel and the whole body replaced; header, onboarding, ending overlay
  and helpers kept
- `Assets/Tests/EditMode/FinalChapterConversationTests.cs` — rewritten, 17 tests to 20

No `.meta` files were created or renamed: every file already existed. No validators outside the Final
Chapter, no authored data for Chapters 1–6, no `Packages`, no `ProjectSettings` were touched.

`Act6EndingSequence` is untouched. Lily's "I finally made a friend" murmur is unchanged.

## Verification

**Ran — standalone .NET 10 harness** (`Ghost.Puzzles.VoicePipeline` has no UnityEngine dependency, so
the three puzzle files compile and run outside Unity). Ten sections, all passing:

1. Reachability — every visitor's reference solution passes and produces the authored reply.
2. Every required answer exists among the things the player can actually pick (intent, fragments,
   route steps, backend sources, response parts, and each part's declared role).
3. One fragment per slot; every visitor has a decoy; every decoy explains itself.
4. Correct-intent indices are **1, 3, 0** — position cannot be memorised.
5. Every wrong intent is rejected, has its own reply, and no two fail identically.
6. Every stage a visitor declares can actually fail them — no decorative stage.
7. Lily: score 78% suggests AnswerNow, correct call is AskAgain. All three costs are stated.
8. Stage counts escalate 2 → 3 → 5, Lily last.
9. The trace marks exactly one broken stage plus the reply line, and reports exactly one error.
10. Guessing which job she meant is rejected at Details; attaching her direct messages is rejected at
    Backend; right intent with a wrong detail stops on Details rather than Intent.

**Not run — Unity EditMode suite.** Claude cannot open the Editor. The suite now contains
144 `[Test]` plus 26 `[TestCase]` methods = **170 discrete cases**, of which the Final Chapter fixture
contributes 20. The earlier run log's figure of 184 could not be reconciled against an attribute count
and should not be carried forward as the expected number.

**Not run — Unity compile.** The presenter is UnityEngine code and cannot be compiled here. Brace
balance was checked mechanically (101/101) and every removed API name was grepped out of `Assets/`, but
the compile itself is the user's pass. The rewritten test file includes
`ThePresenterRendersEveryStageOfEveryVisitorWithoutThrowing`, which builds the presenter, renders every
stage of every visitor and solves all three — that is the automated check for null references in the
new UI, and it only runs in the Editor.

**Not run — Play Mode.** No screenshots taken; no visual acceptance claimed.

## Known risk carried into the user's Editor pass

Layout numbers in the new stage UI (chip heights, slot heights, the 362px onboarding panel) were
derived by adding up their contents, not by looking at them. Chapters 1–6 needed a layout repair pass
after exactly this, so expect the same here.

## Chinese STAR Summary

- **S 情境**：最終章寫於前五章改版之前，仍用「六類別各選一個」加信心門檻滑桿，而滑桿正是第四章重建兩次才拿掉的「有完美解」設計。
- **T 任務**：依規格書重做整合關卡，使每一段真正考前面章節現在教的東西。
- **A 行動**：先重寫純 C# 的資料／模型／validator，在獨立 .NET 環境跑十組檢查（含可達性）通過後，才動 controller 與 presenter；信心改為三選一並各自標明代價；Lily 設計一個「填了才是錯」的空欄位，且執行按鈕以「是否走過該段」而非「是否填滿」判定，避免洩題。
- **R 結果**：純邏輯十組檢查全過，正確答案位置為 1、3、0，78% 分數刻意指向錯誤選項。Unity 編譯、EditMode、Play Mode 均未執行，須由使用者驗收；版面數值為推算而非目視，預期仍需一次修版。
