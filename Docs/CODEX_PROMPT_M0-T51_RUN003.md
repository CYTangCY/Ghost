# Codex Prompt — M0-T51 Run 003 (layout repair)

> Run 002 passed every automated gate while the game was visually broken, because its four screenshots
> were captured on the **onboarding/setup screen** and omitted Chapters 3 and 5. User-supplied in-level
> screenshots then showed overlapping and clipped text across nearly every scene.

Copy everything below the line into Codex.

---

You are running **M0-T51 Run 003**. Add no features. Repair layout regressions introduced by the
M0-T51 type scale, then re-verify.

## What Claude already changed (do not redo or revert)

1. `GhostUITheme` type scale lowered: `Title 30→26`, `Heading 24→21`, `Body 19→17`, `Small 17→15`,
   `Tiny 15→14`. The first scale raised the title +50% while every container was still drawn for
   13-14px text.
2. **`GhostUITheme.Label` now sets `verticalOverflow = Truncate` instead of `Overflow`** (both
   overloads). This was the root cause of most of the damage: wrapped text was painting outside its own
   rect and over whatever sat below it. Expect a lot of the overlap to disappear from this alone — and
   expect some text to now be visibly *clipped* where a container is genuinely too small. Clipping is
   the signal telling you which container to fix.
3. `Act3DialogGraphStaticPresenter`: `PaletteColumnWidth 125→190`,
   `PaletteItemPreferredHeight 70→96`, `TestCasePreferredHeight 58→74`,
   `ValidationControlsPreferredHeight 28→40`.

Claude cannot render, so those four numbers are estimates from font metrics. **Verify them on screen
and adjust if still wrong** — report any value you change and why.

## Defects to repair (all observed in user screenshots)

1. **Chapter Select** — "Start Basics", "Replay Chapter 0" and "Final Chapter" button captions spill
   outside their buttons; the "Start Chapter N" buttons overlap the chapter descriptions above them;
   the Companions panel text is clipped mid-sentence. Make these buttons size to their content
   (min-height 44) and give each chapter card enough height for description + button without overlap.
2. **Title screen** — "Create Account" and "Use Account" captions overflow their buttons.
3. **Act 2 header** — "Errand 1/3" collides with "Return to Hub". Give the title `flexibleWidth` and
   the progress label a fixed `minWidth`/`preferredWidth` so they can never overlap. Apply the same
   guard to every chapter header.
4. **Act 4** — "Today's Visitor Queue" heading paints over its subtitle. Same class of fix.
5. **Act 5** — node card text is clipped, and the wires appear detached from their port dots.
   **Hypothesis to test first:** ports sit at hard-coded `anchoredPosition` offsets on fixed-size
   cards, and the old `Overflow` mode made the text spill past the card so the *visible* card edge no
   longer matched its real rect. The Truncate fix may resolve the apparent detachment on its own.
   Confirm before changing any port maths; if it is genuinely wrong, derive the port offset from the
   card's actual size rather than a constant.
6. **Final Chapter** — Ghost's instruction text is drawn over the Ghost sprite, and "Backend: not
   needed for this conversation" is clipped.
7. **Setup phase** — the objective strip balloons to roughly 230px when the puzzle body has not been
   built yet, because spare vertical space is distributed into it. It collapses correctly to 40px
   in-level. Set its `flexibleHeight` to 0 so it holds 40px in every phase.

## Constraints

- No gameplay, validator, simulator, session, authored data, or Act 4 controller/presenter API changes.
- Do not change the `GhostUITheme` token values or the Truncate setting.
- Do not modify `Packages/`, `ProjectSettings/`, or existing `.meta` GUIDs.
- Nothing may render below `GhostUITheme.TinySize` (14).

## Required verification — this is the part Run 002 got wrong

1. Regenerate all nine scenes.
2. **Capture 1920x1080 screenshots of all nine scenes, each one *inside* the level** — past the
   onboarding button, with the puzzle body actually built. A setup-screen capture is not acceptable
   evidence. Chapters 3 and 5 are mandatory.
3. In every screenshot, confirm: no text overlaps another element, no text is clipped mid-word, no
   element escapes its panel, and nothing collides in the header row.
4. EditMode suite without `-quit`, counts read from the XML. Baseline 147/147.
5. Windows standalone build succeeds with all nine scenes.
6. Backend `npm test` — expect 10/10.

Deployment/installer re-testing is **not** required this run; Run 002 already proved that chain and
nothing here touches it.

## Deliverable

`Docs/codex_runs/M0-T51_003_layout_repair.md` — actions, decisions, results, evidence only. Use
`Not run - [reason]` where applicable. List every constant you changed and the observed reason.
Include a Chinese STAR summary. Then return a Claude review prompt.
