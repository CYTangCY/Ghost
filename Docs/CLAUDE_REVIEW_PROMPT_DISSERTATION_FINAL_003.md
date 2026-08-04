# Claude Final Dissertation Review and KCL Marking Prompt

## Role

Act as a strict KCL Informatics MSc project marker and dissertation reviewer. Review the current repository, not the claims in this prompt. Do not implement new game features. Do not improve the mark by assuming unfinished work will be completed.

## Read Order

Read these files before giving findings or a score:

1. `Docs/DISSERTATION_USER_BRIEF_CONSOLIDATED.md`
2. `Docs/DISSERTATION_WORK_COMPLETED_SUMMARY.md`
3. `Docs/dissertation_review_sources/README.md`
4. `Docs/dissertation_review_sources/supplied/7CCSMPRJ Rubric extracted text.txt`
5. `Docs/dissertation_review_sources/supplied/Suggested Report Chapters and Requirements.txt`
6. `Docs/dissertation_review_sources/MSc_Final_Report_Cover_Sheet_readable.txt`
7. `Docs/dissertation_review_sources/supplied/Final Report Latex Template (7CCSMPRJ)/Readme.md`
8. `Docs/dissertation_review_sources/supplied/Final Report Latex Template (7CCSMPRJ)/Thesis.tex`
9. `Docs/dissertation_review_sources/supplied/Final Report Latex Template (7CCSMPRJ)/kclthesis.cls`
10. `Docs/DISSERTATION_FORMAT_REQUIREMENTS.md`
11. `unorganized_data/dissertation/Ghost_Final_Report_Draft.md`
12. `unorganized_data/dissertation/LITERATURE_MATRIX_2025_2026.md`
13. `unorganized_data/dissertation/latex/Ghost_Final_Report.tex`
14. every file under `unorganized_data/dissertation/latex/contents/`
15. `unorganized_data/dissertation/MARK_ESTIMATE.md`
16. the three evidence files under `unorganized_data/dissertation/evidence/`
17. `Docs/codex_runs/M0-T49_010_dissertation_literature_results_evaluation.md`
18. `Docs/codex_runs/M0-T49_011_kcl_latex_report_restructure.md`
19. `Docs/codex_runs/M0-T49_012_lily_footwear_plain_language_audit.md`
20. `Docs/codex_runs/M0-T49_013_lily_shoe_coverage_dissertation_handoff.md`
21. current project context: `Docs/CONFIRMED_PROJECT_CONTEXT.md`, `Docs/LEARNING_CONTENT.md`, `Docs/ARCHITECTURE.md`, and `Docs/CURRENT_TASK.md`.

The original cover `.doc`, full LaTeX template, `__MACOSX` archive metadata, earlier project report extract, and file hashes are preserved under `Docs/dissertation_review_sources/`. The `__MACOSX` files are not report content. The supplied template says it is unofficial, so check it rather than trusting it automatically.

## Review Priority 1: Literature

Review the literature chapter first and most strictly.

For each of the three arguments, report:

- the exact claim;
- the papers used;
- whether at least four or five relevant studies interact in the discussion;
- which papers support each other;
- which papers conflict or limit the claim;
- whether the report states its own reasoned view;
- whether that view leads clearly to a design choice;
- missing recent work that is important enough to change the argument; and
- any citation that does not support the sentence attached to it.

The three arguments are:

1. learners should apply the concept through a task;
2. educational chatbots are often tools, while direct design teaching is growing; and
3. an LLM tutor needs limits.

Verify all 2025 and 2026 papers through primary publisher pages, official proceedings, or the paper itself if browsing is available. Check authors, title, year, venue, DOI, participant count, design, result, and whether the report overstates the paper. Mark any source that cannot be verified. Do not accept a citation merely because it appears in BibTeX.

Judge whether the review reaches the KCL rubric's Merit, Low Distinction, Mid Distinction, or High Distinction description. A non-systematic search may still be critical, but it must not be called systematic.

## Review Priority 2: Results and Evaluation

Check whether method, result, and interpretation are separate and traceable.

Verify these recorded claims against repository evidence:

- 87/87 Unity EditMode tests from Run 008;
- 10/10 backend tests;
- scene guards for the four rebuilt scenes;
- 27 real Granite outputs;
- internal Granite result of 163/270, or 60.3704%;
- Lily voice score of 10/54;
- two exact-answer failures in Chapter 2; and
- the provisional status of the human Play Mode checklist.

Check that the report does not turn software tests into learning evidence. Check whether the evaluation compares expected results from theory with actual results, because this is central to the KCL 20% Evaluation criterion.

State how the lack of participants, pre/post measures, comparison group, later-recall test, independent Play Mode evidence, and independent LLM raters limits the score. Do not invent a learner study or treat the project-review acceptance as independent verification.

## Review Priority 3: Design and Technical Work

Inspect the actual code and architecture enough to judge the report's claims. Review the curriculum mapping, fixed validators, scene builders, backend, SQLite data model, Ollama/Granite path, fallback, chapter flow, and test approach.

Judge whether the report compares options fairly:

- Unity versus Godot, Phaser, and Unreal;
- Node.js/TypeScript versus FastAPI, ASP.NET Core, and serverless;
- SQLite versus PostgreSQL, MongoDB, and JSON;
- local Granite/Ollama versus hosted APIs, direct Hugging Face serving, and static hints; and
- fixed scoring versus model-based scoring.

Flag shallow comparisons, false current technical claims, weak sources, or choices explained only after the fact. Check whether the literature actually leads to the design.

## Review Priority 4: English and Student Voice

The user wants simple natural academic English near IELTS 6.5 to 7. This is not a request for weak analysis.

Check the full prose for:

- stock AI-style wording;
- rare words used where a common word is clearer;
- unnatural transitions or repeated patterns;
- claims with vague subjects;
- paragraphs that only list papers;
- technical terms that are not explained;
- overlong sentences;
- contractions;
- grammar and punctuation; and
- places where simplification removed needed accuracy.

Run these if available:

```text
python unorganized_data/dissertation/audit_plain_english.py
python unorganized_data/dissertation/check_latex_report.py
```

The current recorded language result is 9,554 words, 721 sentences, 13.3 words per sentence, zero sentences over 32 words, zero listed AI-style phrases, and zero listed uncommon substitutes. Treat this only as a guard. Give a human judgement of naturalness and likely readability. Do not call it an official IELTS score.

## Review Priority 5: KCL Format and Scholarship

Compare the current LaTeX project with the supplied chapter guidance, cover fields, template, and rubric.

Check:

- required sections and order;
- whether about 70% of report content concerns the student's contribution;
- nomenclature order;
- lists of figures and tables;
- captions, labels, and in-text references;
- one-document appendix structure;
- four-decimal presentation of measured values where required;
- reference consistency and missing bibliographic fields;
- current KCL cover details;
- visible TODOs;
- placeholder figures;
- likely LaTeX compile or layout risks; and
- BCS, IET, privacy, security, accessibility, IP, licences, environment, and software trustworthiness.

Do not claim the PDF is correct. No TeX compiler or final PDF visual check was available in the Codex environment.

## Required KCL Mark

Use this exact weighted report rubric:

- Introduction: 10%
- Literature Review: 10%
- Specification and Design: 15%
- Implementation and Technical Achievement: 25%
- Evaluation: 20%
- General Scholarship: 10%
- Legal, Social, Ethical and Professional Issues: 10%

For each area provide:

- raw mark out of 100 for that criterion;
- weighted contribution;
- matching rubric band and wording;
- evidence that earns the mark;
- evidence missing for the next band; and
- any cap or serious risk.

Then provide:

1. exact weighted total;
2. realistic current range;
3. KCL classification using the supplied bands;
4. confidence level in the estimate;
5. a separate conditional range after specific open items are completed;
6. why the work deserves the current mark;
7. why it does not yet deserve 70+, 75+, 80+, or 90+ where relevant; and
8. the five changes with the highest likely mark gain.

The existing estimate is 64-69 with a central estimate of 67, and 69-74 only after required completion. Challenge it. Raise or lower it only with evidence. Do not reward seven missing figures, five student TODOs, uncompiled LaTeX, unverified reference fields, unrecorded Play Mode checks, or a learner study that does not exist.

Grade the written report and technical project. Discuss the separate presentation criterion only as an ungraded risk unless presentation evidence is present.

## Output Format

Return in this order:

1. findings first, ordered by severity, with exact file and line references;
2. source-verification problems;
3. literature review assessment for all three arguments;
4. results and evaluation assessment;
5. design and implementation assessment;
6. English and formatting assessment;
7. weighted KCL score table;
8. current and conditional mark ranges;
9. a direct answer to `Why does this deserve that mark?`;
10. a prioritised fix list;
11. a short final verdict; and
12. Chinese STAR summary using `S situation / T task / A action / R result`.

If no issue is found in an area, say so and state the remaining risk. Keep praise tied to evidence and criticism tied to an actionable fix.