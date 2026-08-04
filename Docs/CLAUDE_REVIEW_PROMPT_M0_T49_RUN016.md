# Claude Review Prompt - M0-T49 Run 016

Please review M0-T49 Run 016 as a presentation and curriculum-scope review only.

Read first:

- `Docs/CONFIRMED_PROJECT_CONTEXT.md`
- `Docs/ROADMAP.md`
- `Docs/CURRENT_TASK.md`
- `Docs/LEARNING_CONTENT.md`
- `Docs/IBM_COURSE_CONTENT.md`
- `Docs/codex_runs/M0-T49_015_supervisor_deck_restructure.md`
- `Docs/codex_runs/M0-T49_016_ibm_coverage_future_work_slides.md`

Inspect:

- `unorganized_data/presentation/supervisor_progress_2026-07-17/Ghost_Progress_2026-07-17_expanded.pptx`
- `unorganized_data/presentation/supervisor_progress_2026-07-17/build_slides.py`

Review questions:

1. Does slide 12 state a realistic four-day Chapter 7 plan without implying that work is complete?
2. Does slide 13 clearly explain that Chapter 7 adds NLP breadth but does not complete the IBM course?
3. Does slide 14 accurately identify and explain the four remaining conceptual areas:
   rule-based vs AI-enabled comparison, ML behaviour, chatbot planning, and platform choice?
4. Is it fair to treat hands-on watsonx Assistant setup as intentionally excluded practical training?
5. Is slide 15's three-part recommendation the smallest reasonable scope for the claim:
   "Ghost covers most conceptual topics from the selected IBM course, excluding hands-on watsonx
   setup"?
6. Are the new pages suitable for a supervisor progress meeting, with one clear purpose per slide and
   no irrelevant grade prediction or hidden implementation claim?
7. Does any wording overstate IBM alignment, implementation status, learning effectiveness, or test
   evidence?

Verification evidence recorded by Codex:

- 16 slides and 16 notes.
- 0 content placeholders.
- 2 labelled Unity screenshot replacement frames.
- Slides 12-16 visually inspected.
- Layout warning scan returned no matches.
- Bundled `slides_test.py` passed with no overflow.
- `git diff --check` passed.
- Chapter 7 and the recommended planning/Voice Basics additions are not implemented.

Please return:

- Findings first, ordered by severity.
- Exact slide numbers and suggested replacement wording for any issue.
- A clear judgement on whether the proposed IBM coverage claim is defensible.
- Any remaining evidence or supervisor decision needed before implementation.
- Chinese STAR summary: S 情境 / T 任務 / A 行動 / R 結果.

Do not alter Unity code, scenes, project settings, `.meta` files, or dissertation content during this
review.