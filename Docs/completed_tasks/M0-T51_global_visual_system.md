# M0-T51 — Global Visual System + Windows Desktop Build

## Completion Status

**CLOSED** — user-verified on screen 2026-08-03 across Chapter 0, Chapters 1-6, Final Chapter and the
Game Shell.

## Date

2026-08-03 (Runs 001-003 by Codex, plus four Claude repair passes)

## Summary

Replaced per-presenter UI code with one shared theme, re-proportioned every chapter page, and retired
the WebGL delivery path in favour of a Windows standalone player.

The visual half took four attempts. The type scale alone was not the hard part — raising it exposed
that every container in the game had been sized around 13-14px text, and two defects in Claude's own
theme code.

## Files Created

- `Assets/Presentation/Common/GhostUITheme.cs` — font/colour/radius tokens, a cached runtime 9-sliced
  rounded-rect sprite generator, and the widget factories every presenter now calls.
- `Assets/Tests/EditMode/FloatingWindowDragHandleTests.cs` — 7 tests.
- `Docs/M0-T51_T56_EXPERIENCE_POLISH_PLAN.md` — the six-slice plan behind this and M0-T52…T56.

## Files Removed

- `Assets/Presentation/Fundamentals/` (both scripts + metas), its hub card, its shell screen, and
  `ChatbotFundamentalsDataTests.cs`. The "Ghost's Voice Basics" block duplicated Chapter 6 and did
  nothing for the story.
- The WebGL build target and the `GHOST_WEB_ROOT` serving block in `Backend/src/server.ts`.

## Root Causes Found (the useful record)

| Symptom | Cause |
|---|---|
| Text small and faint everywhere | No shared tokens. Font sizes were integer literals across 20 files; each presenter carried its own text/panel helpers. Body copy had drifted to 13px. |
| UI looked like stacked boxes | Only 6 files in the whole presentation tree ever set `.sprite`; everything else was a bare rectangular `Image`. |
| "Ask Lily" would not drag to the top, **but only in some chapters** | `FloatingWindowDragHandle.ClampToParent` ignored the window's `anchorMin`/`anchorMax`. `anchoredPosition` is measured from the anchor rect, so the bounds were only correct for a centre-anchored window. |
| Act 3 palette printed titles on top of their own descriptions | `GhostUITheme.Label` set `verticalOverflow = Overflow`, so wrapped text painted outside its own rect and over its neighbour. **Claude's own bug**, introduced with the theme. |
| Chapter 1 onboarding heading vanished | Fixing the above to `Truncate` clipped every label whose box equalled its font size (a 26px title in a 26f box). Fixed centrally with `AtLeastOneLine`. |
| Lily flattened in Chapter 0 | `GhostUITheme.Panel` sets `type = Sliced`, and a sliced `Image` ignores `preserveAspect`. Added `GhostUITheme.Picture`. |
| Act 5 wires unusable, ports unclickable | Ports straddle the card edge, so each consumes half its size from the ~16px gap between columns. Enlarging them to 34px made facing ports overlap completely; the top one swallowed every drag. |
| Chapter 1 illegible | Two Claude errors: hiding pile contents behind "+N more" removed the player's only feedback about their own work, and in-pile cards were shrunk to 34px while the card template renders text at TitleSize 26. |

## Final Layout Decisions

- Type scale `Title 26 / Heading 21 / Body 17 / Small 15 / Tiny 14`. The first attempt (30/24/19/17/15)
  scaled the title +50% and broke containers everywhere.
- Chapter page: header 44px, objective strip 40px (`flexibleHeight = 0`), information blocks min 96px,
  conversation panel 170px.
- Act 3 palette column 190px, item 96px. Act 5 board 520px tall, cards 170x72, columns at
  0.10 / 0.33 / 0.60 / 0.86, ports 26px.
- Chapter 1 training piles are **three side-by-side columns**, one per purpose (user's design call) —
  which removed the need for a scroll container entirely.

## Test / Verification Result

- EditMode: **not re-run since the Fundamentals removal and the drag-handle tests were added.**
  Expected count is **153** (147 − 1 removed Fundamentals test + 7 new clamp tests). This must be
  confirmed at the start of M0-T52/T53 work.
- `FloatingWindowDragHandle.Clamp` was verified by executing the maths standalone across five anchor
  configurations (centre, top-left, top-stretch, full-stretch, bottom-right); all land the window's
  top edge exactly on the parent's top edge.
- Backend: 10/10, re-run by Claude.
- Run 002 (Codex) recorded 147/147 EditMode, nine-scene Windows build, installer install/launch/
  uninstall clean with no residue, SHA-256 and byte size matching.
- **Visual acceptance is the user's**, from in-level screenshots of all nine scenes.

## Process Note

Run 002's automated gates all passed while the game was visually broken, because its four screenshots
were captured on the onboarding screen and omitted Chapters 3 and 5. Screenshot evidence for a visual
task must be captured *inside* the level and must cover every scene. That failure was in the
verification prompt Claude wrote.

## Next Task

**M0-T52 / M0-T53 (in progress)** — see `Docs/CURRENT_TASK.md`.

## Outstanding Archive Debt

M0-T47, M0-T48 and M0-T49 still have no completed-task archives.
