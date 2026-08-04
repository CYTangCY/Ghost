# Claude Review Prompt - M0-T49 Run 014 Supervisor Progress Slides

Review the actual repository state for M0-T49 Run 014. This is a review-only request. Do not modify files until findings are accepted.

## Read First

1. `Docs/codex_runs/M0-T49_014_supervisor_progress_slides.md`
2. `unorganized_data/presentation/supervisor_progress_2026-07-17/build_slides.py`
3. `unorganized_data/presentation/supervisor_progress_2026-07-17/Ghost_Progress_2026-07-17.pptx`
4. `unorganized_data/presentation/supervisor_progress_2026-07-17/granite_scores.png`
5. `unorganized_data/dissertation/latex/figures/granite-results.png`
6. `Docs/codex_runs/M0-T49_008_batchmode_verification.md`
7. `Docs/codex_runs/M0-T49_010_dissertation_literature_results_evaluation.md`
8. `unorganized_data/dissertation/evidence/LLM_HINT_EVALUATION.md`
9. `unorganized_data/dissertation/evidence/llm_hint_prompt_bank_scores.csv`
10. `unorganized_data/dissertation/LITERATURE_MATRIX_2025_2026.md`
11. `unorganized_data/dissertation/latex/contents/literature_review.tex`
12. `unorganized_data/dissertation/latex/contents/design.tex`
13. `unorganized_data/dissertation/MARK_ESTIMATE.md`

## Scope and Required Review

Check the final PPTX slide by slide and compare visible claims and speaker notes with the named source files.

Verify:

- 19 total slides: 14 main plus 5 backup;
- speaker notes on every slide;
- exactly two labelled `REPLACE:` screenshot boxes and no invented screenshot;
- the requested game, dissertation, three critical samples, evaluation, plan, questions, timeline, and backups;
- comparison-driven structure: A versus B, conflict, author judgement, and Ghost design decision;
- older literature is compared with recent evidence on slides 4, 6, and 7;
- exactly 15 recent 2025-2026 papers in the verified-source backup, five per argument;
- every number against Run 008, Run 010, the Granite CSV/evaluation, literature matrix, design chapter, and mark estimate;
- Granite chart labels, five areas plus overall, and byte-identical report copy;
- honest distinction between software verification and learning evidence;
- no overclaim about Play Mode, WebGL, learners, usability, accessibility, tutor effectiveness, or final mark;
- slide 14 uses proposed weeks and does not invent calendar deadlines;
- backup slides are readable at full 16:9 size;
- no unintended overlap, clipping, weak contrast, broken hierarchy, or text that is too small for the supervisor meeting;
- code is repeatable and only writes the approved output paths plus external scratch;
- no prohibited existing file was modified.

The active presentation skill required `@oai/artifact-tool` and prohibited python-pptx. Review whether the documented artifact-tool deviation from the attached task is acceptable. Do not request python-pptx merely for preference if the artifact-tool build, reopen, notes, render, and OOXML checks are sound.

## Important Evidence to Recalculate

- Unity: 87/87 passed, 0 failed, 0 skipped; focused 8/8, 8/8, 1/1; four scene guards.
- Backend: 10/10 route tests and build pass.
- Granite: 27/27 LLM-path returns; 45/54 relevance; 32/54 level fit; 38/54 accuracy; 38/54 answer safety; 10/54 Lily voice; 163/270 = 60.3704% overall.
- Chapter totals: 64/90, 52/90, 47/90.
- Granite issues: 16/27 over 25 words; two exact Act 2 placement leaks.
- Report: 9,554 words; 721 sentences; 13.3-word average; 42 BibTeX entries used; 7 figure placeholders; 5 student TODOs; not compiled.
- Current mark estimate: 64-69, central 67; conditional 69-74 only after named work.

## Expected Build Review

Run if available:

```text
python unorganized_data/presentation/supervisor_progress_2026-07-17/build_slides.py
```

Confirm it overwrites cleanly, leaves only the three requested files in its output folder, and keeps both chart copies byte-identical. Reopen and render the resulting deck with artifact-tool or another method compatible with the active environment.

## Output Format

Return:

1. findings first, ordered by severity;
2. exact slide number and source-file references for every issue;
3. factual and citation audit;
4. visual and presentation audit;
5. builder and repeatability audit;
6. open questions or assumptions;
7. concise verdict: ready for rehearsal, ready after fixes, or not ready;
8. prioritised fixes before the supervisor meeting; and
9. Chinese STAR summary using S situation, T task, A action, and R result.

If no issue is found in an area, say so and state the remaining risk. Do not praise without evidence and do not treat the two screenshot placeholders as completed figures.