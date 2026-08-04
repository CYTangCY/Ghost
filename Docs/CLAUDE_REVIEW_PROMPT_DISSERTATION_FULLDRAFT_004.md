# Reviewer Prompt — Ghost Dissertation, Full Draft (Review 004)

Date issued: 2026-08-01
Submission target: 2026-08-09
Previous review: literature review chapter only (Review 003)

---

## What you are reviewing

The **complete dissertation**, not one chapter. At the time of your last review, seven of the
eight content files were instruction skeletons — bullet lists of directives to the writer such as
`\item Describe the design:` and `\item Report the totals with Table~\ref{...}`. Only
`literature_review.tex` was finished prose, which is why your last review covered only that file.

All chapters are now written. Six chapters have **never been reviewed by anyone**. Those six are
the priority for this pass; the literature review has already had one round and has been revised
against it.

Files, all under `unorganized_data/dissertation/latex/contents/`:

| File | Status | Words |
|---|---|---|
| `abstract.tex` | New prose — never reviewed | 404 |
| `introduction.tex` | New prose — never reviewed | 987 |
| `literature_review.tex` | Revised against Review 003 | 3,129 |
| `design.tex` | New prose — never reviewed | 2,075 |
| `implementation.tex` | New prose — never reviewed | 2,080 |
| `evaluation.tex` | New prose — never reviewed | 3,289 |
| `professional_issues.tex` | New prose — never reviewed | 1,403 |
| `conclusion.tex` | New prose — never reviewed | 679 |
| `references.bib` | 49 entries, 7 added | — |

Main file: `Ghost_Final_Report.tex`. Style: `ieeetr`. Class: `kclthesis`.

---

## Two corrections to Review 003

Please do not repeat these, and please note the method that produced them.

**1. Lin et al. (2025) is accurate.** You flagged "72 university students", "four-week trial",
and "better than a fixed FAQ tool" as three details you could not find, and called this the only
possible factual error. All three are correct: the study is a four-week randomised controlled
trial with 72 university students comparing a GPT-based learning aid against a fixed FAQ tool.
The abstract also reports richer self-assessment and reflective behaviour in the GPT group via
multimodal learning analytics, which has now been added. The sentence has been strengthened, not
corrected.

**2. There are no orphan `.bib` entries.** You listed eighteen keys (`unity2026`, `godot2026`,
`express2026`, and so on) as possibly unused. A full check across all chapters found that every
key was already cited somewhere. All 49 entries are now cited, and none is undefined.

Both errors came from assessing one chapter as if it were the whole document. Please read the
whole document this time before judging whether something is missing.

---

## What Review 003 asked for, and what was done

| # | Your point | Action taken |
|---|---|---|
| 1 | Sweller 1988 misattributed | Added `chandler1991` (split attention) and `sweller1994` (element interactivity); the two misattributed sentences now cite those. `sweller1988` retained only for the working-memory-in-problem-solving claim it actually supports. |
| 2 | Background Theories had zero citations; no SkillsBuild anywhere | Eight technical definitions now cite Jurafsky & Martin (3rd ed., 2026). A new `ibmskillsbuild2026` entry is cited in Background Theories and in the Introduction. **See open question A below.** |
| 3 | Hsu & Hsu sample missing; double standard vs Wang/Gong | Sample added (56 sixth-graders, two schools in northern Taiwan, 28/28, control used a web search engine only). Three explicit weaknesses now stated, and its argumentative weight reduced to "one supporting reason among several". |
| 4 | Lin et al. details unverifiable | Verified correct. See correction 1. |
| 5 | Liu effect sizes unchallenged | Effect sizes now given (0.857 / 0.803), Kraft (2020) added as the benchmark, and publication bias plus small-study effects named. The meta-analysis is now used for its moderator pattern only. The unverifiable "motivation effect reduced over time" claim was removed. |
| 6 | Bastani and Barcaui treated as equal | Now explicitly unequal: Bastani described as pre-registered, ~1,000 students, national academy journal; Barcaui as single-author, broad open-access, with the attrition problem. **See open question B below.** |
| 7 | Yoo is a workshop paper | Now stated in the prose, not only in the `.bib`. |
| 8 | Kim reads causal | Rewritten. 6% response rate and absence of a control group both stated; "improved" replaced with reported pre-post gains that cannot be attributed to the game. |
| 9 | Bastani's guardrails ≠ Lily's guardrails | A dedicated paragraph now explains that Bastani's tutor worked because teachers pre-authored the solutions and hints and the model only delivered them, whereas Lily generates at run time. Only the hint-not-answer principle is claimed as transferred. |
| 10 | Five limits presented as if all literature-derived | Split: one limit attributed to `bastani2025`, four stated as the author's own design decisions with their reasons. |
| 11 | Rule-following ≠ learning gain | Added as a closing paragraph to Argument 3, and repeated in the evaluation chapter's opening bound. |
| 12 | No search strategy | Added. **See open question C below.** |
| 13 | Geographic skew unmentioned | Added to the review approach section. |
| 14 | Two AI-literacy-game papers might collide with the gap claim | Both verified as real and both added. See below. |

**On point 14.** Both papers exist:

- Tu, Smith, Bassanelli, Marconi & Nacke (2026), "Conceptualizing How to Design for AI Literacy
  through Game Artifacts", DIS 2026, pp. 1056–1080, DOI 10.1145/3800645.3812865 — scoping review,
  48 design artifacts from 45 papers.
- Chen & Pu (2026), "Using Games to Learn How Large Language Models Work", arXiv 2603.28374 —
  an early-stage proposal for two games teaching next-token prediction.

The judgement made was that they strengthen the gap claim rather than destroy it, because Ghost's
claim is about chatbot pipeline responsibilities and connected narrative puzzles, while the Tu et
al. corpus is dominated by general AI literacy topics and Chen & Pu work one level below the
chatbot architecture. **Challenge this judgement if you disagree** — it is the single most
load-bearing new argument in the chapter, and it was made without reading the full Tu et al. text,
only its abstract and metadata.

---

## Three open questions the author must settle — check these first

**A. Is `ibmskillsbuild2026` the right course?** The entry points to IBM SkillsBuild's
"Build Your First Chatbot Using IBM watsonx". This was inferred from the author's source files
(`Course_IBM_chatbot.pdf`, `Course_Simulation transcript_Building a simple chatbot.pdf`,
`Course_Video transcript_Chatbots Your efficiency boosters.pdf`), not confirmed against an
enrolment record. The whole curriculum-mapping claim rests on this citation being the course the
author actually took. Flag it if the title, URL, or year is wrong.

**B. The Barcaui attrition sentence is unverified.** The text states that "the degrees of freedom
reported for its main comparison indicate that roughly a quarter" of the 120 randomised students
were not in the final analysis. This derives from your `t(83)` observation and has **not** been
checked against the paper. A `% VERIFY BEFORE SUBMISSION` comment marks it in the source. Either
confirm it from the PDF or tell the author to delete the sentence — the surrounding argument about
unequal evidence strength survives without it.

**C. The search strategy paragraph may not be true.** A search strategy has been written
(Google Scholar and the ACM Digital Library; a listed set of terms; theory sources from any year,
empirical sources 2020 onwards). It is marked with a `% VERIFY BEFORE SUBMISSION` comment because
**it describes a plausible process, not a recorded one**. If it does not match what the author
actually did, it must be rewritten by the author before submission. Treat this as an integrity
issue, not a style issue. Deliberately, no screening counts were invented.

---

## What to review in the six new chapters

Apply the same standard you applied to the literature review: claim → evaluation → connection, and
flag any assertion whose evidence is not visible.

**Highest priority — `evaluation.tex`.** This chapter carries the marks and has never been read by
anyone but its author. Check specifically:

1. Are expected results genuinely defined *before* the obtained results, or has the criterion been
   written backwards from what was found?
2. The chapter reports 18.5% on "Lily voice and format" and two hints that leaked an exact
   Chapter 2 answer. Is that handled as a real negative finding, or is it softened by the
   surrounding argument that it "supports architectural decisions made before the measurement"?
   That sentence is the most likely place for motivated reasoning in the whole document.
3. The 27 outputs are scored 0–2 across five areas, giving 54 per area and 270 overall. Is this
   explained clearly enough that a reader does not mistake `38/54` for 38 of 54 outputs?
4. Every limit is supposed to sit next to the claim it qualifies rather than in a list at the end.
   Verify this actually happens; there is also a consolidated limits list, which risks duplication.
5. Is the "no learning claim" boundary maintained consistently, or does any sentence drift into
   implying learning?

**`design.tex` and `implementation.tex`.** These describe a system you cannot see. Every factual
claim about the build should be checkable against the repository, and any that is not should be
flagged for the author to verify:

- The eleven REST routes and six SQLite tables named in `implementation.tex`.
- The claim that `GET /content` omits answer keys.
- The claim that there is no code path from hint text to a validator — this is the strongest
  architectural claim in the document and appears in four chapters.
- The engine comparison's two "checked facts": that Godot 4 cannot export C# to the web, and that
  Unreal's packaging targets exclude a browser.
- Test counts: 87/87 EditMode, 8/8, 8/8, 1/1 focused, 10/10 backend.
- Latency figures: 764 ms mean, 453 ms min, 5,653 ms max, ~46 s cold start.

**`introduction.tex`, `professional_issues.tex`, `conclusion.tex`, `abstract.tex`.** Check for
consistency of numbers and claims with the evaluation chapter, and check that the conclusion
introduces no evidence not already presented.

**Cross-chapter coherence.** This was invisible in your last review and is now checkable. The
literature review states five limits on Lily; the evaluation measures that two of them were broken.
Does the document handle that honestly across chapter boundaries, or does one chapter promise what
another disproves without acknowledgement?

---

## Known gaps, already identified — do not spend time re-reporting

- Five of seven figures are missing (`architecture.png`, `shell-hub.png`,
  `chapter3-dialogue-graph.png`, `chapter6-backend.png`, `final-pipeline.png`). They render as
  labelled placeholder boxes. Screenshots must be captured by the author.
- `\wordcount` in the main file is a placeholder pending final compilation.
- No learner study exists and none is claimed.
- The human Play Mode checklist has no independent observer or screenshot record; this is stated
  in the evaluation chapter as a limit.

---

## Output format requested

1. **Factual errors** — anything stated that is not true, with the correct value. Highest priority.
2. **Unsupported claims** — assertions with no cited or checkable basis, per chapter.
3. **Cross-chapter contradictions** — where two chapters disagree.
4. **Critical-thinking gaps** — where a source or a result is reported without evaluation, in the
   six new chapters specifically.
5. **Ranked fix list** — ordered by risk to the mark, given eight days to the deadline, and
   distinguishing "must fix before submission" from "would improve".

Please verify against sources rather than inferring from a single chapter, and state explicitly
when you could not verify something rather than treating it as absent.
