# Claude Review Prompt - M0-T49 Run 015

Review only. Do not edit files.

Read these files first:

1. `Docs/codex_runs/M0-T49_015_supervisor_deck_restructure.md`
2. `unorganized_data/presentation/supervisor_progress_2026-07-17/Ghost_Progress_2026-07-17_revised.pptx`
3. `unorganized_data/presentation/supervisor_progress_2026-07-17/build_slides.py`
4. `Docs/ARCHITECTURE.md`
5. `Docs/REQUIREMENTS.md`
6. `Docs/IBM_COURSE_CONTENT.md`
7. `Docs/LEARNING_CONTENT.md`
8. `Docs/codex_runs/M0-T49_008_batchmode_verification.md`
9. `unorganized_data/dissertation/evidence/LLM_HINT_EVALUATION.md`

Do not review the older `Ghost_Progress_2026-07-17.pptx` or
`Ghost_Progress_2026-07-17_rebuilt.pptx` as the final deliverable. They are preserved comparison
versions. The review target is `Ghost_Progress_2026-07-17_revised.pptx`.

## Review Questions

1. Does the 13-slide sequence work as a supervisor progress meeting rather than a dissertation defence
   or mark-prediction exercise?
2. Does every slide have one clear purpose, with a logical order from progress to design, architecture,
   evidence, future work, and decisions?
3. Is slide 8's architecture diagram accurate to the implemented repository?
   - Unity deterministic validators/sessions are the scoring source.
   - Backend and LLM services are optional to gameplay correctness.
   - The REST route list, SQLite tables, static fallback, and no-scoring-endpoint guard are accurate.
   - The diagram does not imply Granite decides completion.
4. Is slide 12 honest about curriculum coverage?
   - Chapter 7 adds POS, sentiment, translation, and conceptual NLP breadth.
   - It does not claim that Chapter 7 completes the whole IBM course.
   - watsonx setup, platform selection, starting-channel planning, and detailed ML personalisation remain
     outside full coverage.
5. Are the literature claims on slide 6 traceable to the report and matrix, including the comparison
   with older literature?
6. Are test numbers and Granite scores accurate to the named evidence?
7. Is any slide still too dense, visually confusing, repetitive, or unsuitable for oral presentation?
8. Are the two screenshot placeholders the only unresolved visible items?
9. Does the deck avoid expected marks, KCL band predictions, and unsupported learning claims?
10. Is the plain English appropriate for the student's intended IELTS 6.5-7 level?

## Required Output

- Findings first, ordered by severity.
- Give slide numbers and exact text when identifying a problem.
- Separate factual errors from presentation/clarity concerns.
- State clearly if there are no blocking findings.
- End with a Chinese STAR summary: S 情境 / T 任務 / A 行動 / R 結果.

