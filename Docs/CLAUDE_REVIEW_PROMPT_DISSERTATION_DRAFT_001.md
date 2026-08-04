# Claude Review Prompt - M0-T49 Provisional Closure and Dissertation Draft 001

Read the repository before judging. Do not rely on this prompt alone.

## Part A: M0-T49 provisional closure review

Read first:

- `Docs/CURRENT_TASK.md`
- `Docs/LEARNING_CONTENT.md`
- `Docs/codex_runs/M0-T49_005_remediation_pass.md`
- `Docs/codex_runs/M0-T49_006_character_pixel_style_unification.md`
- `Docs/codex_runs/M0-T49_007_lily_colour_correction.md`
- `Docs/codex_runs/M0-T49_008_batchmode_verification.md`
- `Docs/codex_runs/M0-T49_009_user_provisional_playmode_acceptance.md`
- `Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN009.md`
- the single current checklist in `Docs/UNITY_TEST_CHECKLIST.md`

Confirmed automated evidence from Run 008:

- Four required builders completed successfully in the required final order.
- Full Unity EditMode suite: 87 discovered, 87 passed, 0 failed, 0 skipped.
- Focused suites: Shell 8/8, Act 6 8/8, Act 5 1/1.
- Four scene guards: exactly one Main Camera, Canvas, and EventSystem; zero missing scripts.
- Backend build passed and backend route suite passed 10/10 during the dissertation evidence run.

The project owner has asked that the remaining 1920 by 1080 Play Mode checklist be treated as provisionally complete for review. This is not an independent Codex observation and has no screenshot record. Decide whether this is enough to close M0-T49 under the collaboration protocol. If it is enough, perform Claude's normal closure/archive/handoff/current-task work. If it is not enough, state the exact minimum evidence still required. Do not describe provisional acceptance as an independently verified Play Mode pass.

## Part B: dissertation review

Read:

- `unorganized_data/7CCSMPRJ_Rubric.pdf`
- `unorganized_data/dissertation/Ghost_Final_Report_Draft.md`
- `unorganized_data/dissertation/Ghost_Final_Report_Draft.docx`
- `unorganized_data/dissertation/LITERATURE_MATRIX_2025_2026.md`
- `unorganized_data/dissertation/evidence/LLM_HINT_EVALUATION.md`
- `unorganized_data/dissertation/evidence/llm_hint_prompt_bank_raw.json`
- `unorganized_data/dissertation/evidence/llm_hint_prompt_bank_scores.csv`
- `unorganized_data/dissertation/MARK_ESTIMATE.md`
- `Docs/CONFIRMED_PROJECT_CONTEXT.md`
- `Docs/REQUIREMENTS.md`
- `Docs/ARCHITECTURE.md`

Review the draft as a strict KCL MSc marker and repo-aware technical reviewer.

### Required checks

1. **Literature review first priority.** Check all three arguments. Each should compare at least five recent 2025-2026 papers, including support, limits, and useful conflict. Confirm that each comparison leads to a stated Ghost design choice. Flag any paper claim that is stronger than its method or sample allows.
2. **Gap claim.** Confirm that the report does not falsely claim educational chatbots or direct chatbot-design teaching are absent. The intended narrow gap is connected narrative game actions for intent, entity, dialogue, confidence, testing, and backend response responsibilities.
3. **Source audit.** Check every DOI, title, author list, year, venue, sample claim, and official tool-document claim. List any source that cannot be verified. Do not accept invented or secondary-only support when the primary source is available.
4. **Critical writing.** Check whether paragraphs actually compare sources or options, state limits, give the project's view, and explain the final decision. Flag summary-only paragraphs.
5. **Plain English.** Keep the wording simple and natural. Flag vague, inflated, AI-sounding, or needlessly difficult wording. Do not make the prose more ornate.
6. **Tool choices.** Review the comparisons for Unity/Godot/Phaser/Unreal; Node.js/FastAPI/ASP.NET/serverless; SQLite/PostgreSQL/MongoDB/JSON; and Ollama Granite/hosted API/Transformers/static text. Confirm that strengths, costs, scale limits, and the final project reason are all present.
7. **Implementation accuracy.** Compare Chapters 0, 1-6, Final Chapter, completion rules, backend, database, validators, fallback, and LLM scope against the actual repository. Flag stale eight-Act wording or features claimed but not present.
8. **Results and evaluation.** Confirm the test counts and 27-output Granite result against raw evidence. The report must not claim learner gain, usability success, or effective tutoring. Check that 60.4% is described as an internal developer score, not a validated measure.
9. **Legal, social, ethical, and professional work.** Check privacy, weak prototype accounts, CORS, LLM errors, accessibility, licence records, energy/hardware access, and honest evidence reporting. Suggest missing evidence rather than generic ethics text.
10. **Rubric mark.** Give an independent score for all seven rubric sections, a total, likely band, and the three highest-value changes. Compare your result with `MARK_ESTIMATE.md`; do not simply accept its 73-76 estimate.
11. **Document status.** The DOCX opens in Microsoft Word, has 32 pages and 7 tables, and its static contents page matches measured Heading 1 pages. OOXML parsing and structural audits passed with 0 high and 0 medium accessibility findings. LibreOffice was not installed, and both Word PDF export methods stalled even for a one-page smoke file, so PNG/PDF visual QA was not completed. Treat that as an explicit remaining presentation risk.

### Response format

Return:

1. Findings first, ordered P1/P2/P3, with exact file and section references.
2. Citation verification problems.
3. Rubric score table and likely final band.
4. M0-T49 closure decision.
5. A short edit plan for the next dissertation pass. Do not rewrite the whole report unless the project owner asks.
6. Chinese STAR: S situation, T task, A action, R result.
