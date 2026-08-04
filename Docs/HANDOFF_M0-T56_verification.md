# Handoff — M0-T56 Final Chapter, blocked on one compile error

Written 2026-08-04 at the end of the implementation session, for whoever picks this up next.

## State in one line

M0-T56 (Final Chapter integration redesign) is fully implemented and its pure logic is verified, but
**Unity will not compile** because of one botched text edit in the presenter. Nothing else is known to
be wrong; nothing past the compile has been checked yet.

## 1. Fix this first — it is the only thing blocking the Editor

`Assets/Presentation/Act6VoicePipeline/FinalChapterConversationPresenter.cs`, lines **384–393**.

Unity reports:

```
FinalChapterConversationPresenter.cs(385,23): error CS1010: Newline in constant
FinalChapterConversationPresenter.cs(385,80): error CS1003: Syntax error, ',' expected
FinalChapterConversationPresenter.cs(387,1):  error CS1010: Newline in constant
FinalChapterConversationPresenter.cs(389,23): error CS1010: Newline in constant
FinalChapterConversationPresenter.cs(389,94): error CS1003: Syntax error, ',' expected
FinalChapterConversationPresenter.cs(391,1):  error CS1010: Newline in constant
```

### Cause

The last edit of the session rewrote Lily's onboarding paragraph through a Python script in a bash
heredoc. The intent was to emit the two-character C# escape `\n\n` inside a string literal. It arrived
as **real newlines**, splitting two string literals across three lines each.

The file currently reads:

```
384    "Um... everything we repaired is in Ghost now. But nobody is going to tell you which "
385        + "part a conversation needs. That is the whole difference.
386
387 "
388        + "Three visitors. The first only has to be understood. The second names two of the "
389        + "same thing, so a route has to be built for it. The third one... is me.
390
391 "
392        + "Work through the steps along the top, then run the conversation once and read the "
393        + "trace before you trust it. Each visitor is separate - if one breaks, only that one goes back.",
```

Lines 385 and 389 must each end with a closing quote on the same line, with the paragraph break
expressed as an escape rather than a literal newline. Lines 386–387 and 390–391 then collapse away.

### Do not fix it with a Python/heredoc script

**This is the third time this exact bug has shipped in this project** — Chapter 3 and Chapter 4 hit it
before, both surfacing to the user as compile errors, and it is recorded as a known lesson in
`Docs/codex_runs/M0-T52_T56_001_claude_direct_implementation.md`. It was then repeated anyway, on a
purely cosmetic wording change, after ~3000 lines of new logic had gone in cleanly.

Use the `Edit` tool directly on the file, or `System.Environment.NewLine` concatenation. Do not route
C# string escapes through a shell heredoc into Python.

After fixing, check for the same damage elsewhere before compiling again:

```bash
grep -rn '"[^"]*$' Assets/Presentation/Act6VoicePipeline/FinalChapterConversationPresenter.cs | grep -v '+ *$\|",$\|");$\|" *$'
```

## 2. What M0-T56 actually is

Full detail: `Docs/codex_runs/M0-T56_001_final_chapter_integration.md`. Summary:

The Final Chapter was rebuilt because it still used a confidence **threshold slider** with a per-visitor
`[min, max]` band — a number on screen with a perfect answer, which is the exact design Chapter 4 had to
be rebuilt twice to remove. The capstone was contradicting the chapter before it.

Now: **three visitors, escalating**, each stage running the mechanic its chapter teaches.

| Visitor | Stages | Point |
| --- | --- | --- |
| Courier | Intent, Details | Establishes the loop. Only has to be understood. |
| Student | Intent, Details, Dialogue | Names two machines of the same type; a route must be built. |
| Lily | Intent, Details, Backend, Confidence, Response | The whole machine at once. |

Two deliberate traps, both covered by tests:

1. **Lily's confidence is 78%** — reading the number says "answer now". The correct call is "ask one
   more question", because a required slot has two candidates in her message. The score measures how
   clear the wording is, not whether the request is decided.
2. **Lily has a slot nothing can fill** (`slot_which_job`). Leaving it empty is correct. So
   `CanRunTest` is keyed on stages *visited*, not slots *filled* — otherwise a greyed-out run button
   would give the answer away.

The confidence slider is gone, replaced by Chapter 4's three-way call with each option's cost stated
on the card.

## 3. What is verified, and what is not

**Verified — standalone .NET 10 harness.** `Ghost.Puzzles.VoicePipeline` has no UnityEngine dependency,
so the data/models/validator were compiled and run outside Unity. Ten sections, all passing. The one
that matters most is **reachability**: every visitor's reference solution actually passes and produces
the authored reply. Chapters 2 and 3 both shipped unsolvable puzzles while their rule-level tests
stayed green, so this check exists specifically to stop a fourth.

Also confirmed: correct-intent indices are 1/3/0 (position cannot be memorised); every wrong intent has
its own failure reply; every declared stage can actually fail; the trace marks exactly one broken stage.

**Not verified — anything that needs Unity.** No compile, no EditMode run, no Play Mode, no screenshots.

## 4. Order of work for the next session

1. Fix the string literals above. Compile.
2. Run EditMode. Expected: **144 `[Test]` + 26 `[TestCase]` = 170 discrete cases**, Final Chapter
   fixture contributes 20. (The figure "184" in the earlier run log could not be reconciled against an
   attribute count — do not use it.)
   - Watch `ThePresenterRendersEveryStageOfEveryVisitorWithoutThrowing`. It builds the presenter,
     renders every stage of every visitor and solves all three. It is the only automated check on the
     new UI.
3. Play Mode on the Final Chapter, all three visitors through to the ending.
4. **Expect a layout repair round.** Every height in the new stage UI (chip 40px, slot 60px, action
   card 64px, onboarding 362px) was derived by adding up its contents, not by looking at it. All six
   earlier chapters needed a repair pass after exactly this.

## 5. Layout fix landed the same day, across all seven chapters

Worth knowing because it will look like unrelated churn in the diff.

Each chapter's `Header` is a fixed 44px row whose `LayoutElement` never pinned `flexibleHeight`, while
its inner `HorizontalLayoutGroup` sets `childForceExpandHeight = true`. A layout group that force-expands
height reports flexible height to *its* parent, so the header claimed a share of the page's spare space
and pushed everything down — the "too much white at the top" the user reported. The blue conversation
panel had the identical defect at 170px.

Both are now pinned to `flexibleHeight = 0f` in all seven presenters; each root layout uses
`childAlignment = MiddleCenter` with 14px spacing; conversation panels went 170 → 178 (they hold a
150px Ghost face plus 12px padding, so 170 was clipping it).

**Do not "tidy" these pins away.** The user rejected one over-correction already: pinning
`flexibleHeight = 0f` in bulk across every panel broke the Final Chapter's layout badly. Only the fixed
header row, the objective strip and the conversation panel are pinned. The main body keeps
`flexibleHeight = 1f` and is supposed to absorb the slack.

## 6. Files changed in M0-T56

Rewritten:
- `Assets/Scripts/Puzzles/VoicePipeline/FinalChapterConversationModels.cs`
- `Assets/Scripts/Puzzles/VoicePipeline/FinalChapterConversationData.cs`
- `Assets/Scripts/Puzzles/VoicePipeline/FinalChapterConversationValidator.cs`
- `Assets/Presentation/Act6VoicePipeline/FinalChapterConversationController.cs`
- `Assets/Tests/EditMode/FinalChapterConversationTests.cs` (17 tests → 20)

Partly rewritten — host methods, objective text, conversation panel and the whole body; header,
onboarding, ending overlay and helpers kept:
- `Assets/Presentation/Act6VoicePipeline/FinalChapterConversationPresenter.cs`

Untouched and must stay that way: `Act6EndingSequence.cs` (Lily's "I finally made a friend" murmur),
every other chapter's validator and authored data, `Packages/`, `ProjectSettings/`.

No `.meta` files were created or renamed — every file already existed.

## 7. Still outstanding after this

- `Docs/completed_tasks/` archives for M0-T47, M0-T48, M0-T49 were never written.
- M0-T56 closure archive, once the user verifies.
- The working tree has ~220 uncommitted entries spanning M0-T52 through M0-T56. Nothing has been
  committed this session. Scoped starting point, which deliberately excludes `ProjectSettings/`,
  `Packages/`, `Backend/`, `Deployment/` and `Assets/Scenes/*.unity`:

```bash
git add Assets/Presentation Assets/Scripts Assets/Tests Docs
```

## 8. Working agreement to carry over

- Codex is out of the workflow (a run cost ~£20). Claude writes the code; the user runs Unity and
  reports back. Claude cannot compile or render, so every change needs the user's Editor pass.
- The delivery constraint in `Docs/ROADMAP.md:39` is a dissertation deadline of **early August 2026**.
  It is now 2026-08-04. Prefer repairs over redesigns unless the user explicitly asks otherwise.
- The user is cost-sensitive about tokens and has said so directly. Batch independent greps into one
  call; avoid whole-file `Write` on large files (it echoes the file back into context); never dump
  debug output into the transcript when a file will do.
- Puzzle correctness stays deterministic — validators only. The LLM never decides scoring.
