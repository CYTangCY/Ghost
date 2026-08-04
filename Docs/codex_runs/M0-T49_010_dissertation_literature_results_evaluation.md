# M0-T49 - Run 010 - Dissertation Literature, Results, and Evaluation Draft

## Task ID

M0-T49 follow-on writing and review support

## Run Number

010

## Date

2026-07-16

## Original Request / Prompt Summary

Treat the remaining human Unity checklist as provisionally complete for Claude review, then complete the dissertation literature review, results, and evaluation in simple English. Give the literature review first priority; add and compare recent 2025-2026 papers across three arguments; state conflicts, project views, and design decisions; compare frontend, backend, database, and model-serving options before justifying Unity, Node.js, SQLite, Ollama, and IBM Granite; and estimate the likely rubric mark.

## Files Created

- `Docs/codex_runs/M0-T49_009_user_provisional_playmode_acceptance.md`
- `Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN009.md`
- `Docs/CLAUDE_REVIEW_PROMPT_DISSERTATION_DRAFT_001.md`
- `unorganized_data/dissertation/Ghost_Final_Report_Draft.md`
- `unorganized_data/dissertation/Ghost_Final_Report_Draft.docx`
- `unorganized_data/dissertation/LITERATURE_MATRIX_2025_2026.md`
- `unorganized_data/dissertation/MARK_ESTIMATE.md`
- `unorganized_data/dissertation/build_final_report_docx.py`
- `unorganized_data/dissertation/run_llm_hint_prompt_bank.ts`
- `unorganized_data/dissertation/evidence/LLM_HINT_EVALUATION.md`
- `unorganized_data/dissertation/evidence/llm_hint_prompt_bank_raw.json`
- `unorganized_data/dissertation/evidence/llm_hint_prompt_bank_scores.csv`
- `Docs/codex_runs/M0-T49_010_dissertation_literature_results_evaluation.md`

Temporary evaluation, render, and intermediate DOCX files were written under `tmp/`.

## Files Modified

- None of the existing game runtime, puzzle, scene, test, ProjectSettings, Package, or `.meta` files were changed during this writing run.
- The new dissertation Markdown was revised during the run to replace workflow-specific wording with formal project-review wording.

## Research and Writing Result

- Final Markdown length: 10,621 words and 508 lines.
- Final Word document: 32 A4 pages, 7 tables, and a static contents page matched to measured Word Heading 1 page numbers.
- Literature review has three main arguments. Each argument compares five recent 2025-2026 papers, their evidence, limits, support or conflict, and the resulting Ghost design choice.
- The report uses a narrow gap claim. It does not claim that educational chatbots or direct chatbot-design teaching are absent.
- The design chapter compares Unity, Godot, Phaser, and Unreal; Node.js, FastAPI, ASP.NET Core, and serverless; SQLite, PostgreSQL, MongoDB, and JSON; and local Granite, hosted APIs, direct Transformers, and static text.
- Results and evaluation include real Unity and backend test evidence, curriculum mapping, and a new 27-output Granite prompt-bank result.
- The report explicitly does not claim learning gain, usability success, or effective tutoring because no participant study was run.
- The rubric estimate gives a likely current range of 73-76, with a central estimate of 74.

## Sources Checked

Recent research claims were checked against publisher, DOI, university repository, or official proceedings pages. The research matrix records the citation, evidence, limit, project use, and comparison for each paper. Official documentation was used for Unity, Godot, Phaser, Unreal, Node.js, Express, FastAPI, ASP.NET Core, SQLite, PostgreSQL, MongoDB, Ollama, IBM Granite, ICO data protection guidance, and WCAG 2.2.

The final report contains 29 reference entries dated 2025 or 2026, including current technical documentation. Every core literature argument uses five research papers from 2025-2026.

## Tests and Checks Run

### Backend and model evidence

- `npm.cmd test` in `Backend`: passed, 1 test file and 10/10 tests.
- `npm.cmd run build` in `Backend`: passed.
- `npm.cmd run check:ollama`: `granite3.1-dense:2b` was available; a cold timed generation completed in about 46.4 seconds.
- Real prompt bank through production `createLilyHint`: 27/27 requests returned from the LLM path.
- Warm prompt-bank latency: mean 764 ms, median 581 ms, minimum 453 ms, maximum 5,653 ms.
- Internal score: 163/270, or 60.4%. Two hints gave an exact Act 2 placement; 16/27 exceeded the requested 25-word guide.

### Unity evidence used

- Unity was not rerun during this writing run.
- The report uses the actual M0-T49 Run 008 results: full EditMode 87/87, focused Shell 8/8, Act 6 8/8, Act 5 1/1, four successful final builder runs, and four passing scene guards.
- Interactive Play Mode: Not run - the project owner requested provisional acceptance for Claude review, and no independent observer or screenshot record was created.

### Dissertation and DOCX checks

- Microsoft Word read-only open: passed.
- Word pagination: 32 pages and 7 tables.
- Measured major pages: Contents 2, Abstract 3, Introduction 4, Literature 6, Design 10, Implementation 17, Evaluation Method 20, Results 22, Discussion 24, Legal/Ethical 27, Conclusion 29, References 30. Static contents entries match these pages.
- `python-docx` reopen: passed, 261 paragraphs and 7 tables.
- All DOCX XML parts parsed successfully; direct `fldChar` or `instrText` children under paragraphs: 0.
- DOCX table geometry: 7 tables, 0 problems; every table width and grid total is 9,411 DXA with a 120 DXA table indent.
- Accessibility audit: 0 high, 0 medium, 29 low findings. All low findings were raw DOI or official-document URLs in the reference list.
- Heading audit: 12 Heading 1 and 42 Heading 2 paragraphs. Numbered-list warnings were false positives for real list paragraphs.
- Section audit: one A4 portrait section, 0.87-inch margins, different first-page header/footer.
- Python builder `py_compile`: passed.
- Plain-language scan for common inflated wording: no flagged wording.
- Non-ASCII scan across the English report, research notes, builder, evidence script, and review prompts: no matches. The required Chinese STAR block was added afterwards and is intentionally non-ASCII.
- `git diff --check`: passed.

## Errors Encountered

1. The Windows sandbox helper repeatedly failed with `helper_unknown_error: apply deny-read ACLs`.
2. The normal `apply_patch` tool could not read the workspace through that helper. The external Codex `apply_patch` wrapper was also denied by WindowsApps permissions.
3. `git apply` worked for new and Markdown files, but failed on later changes to the untracked CRLF Python builder. A verified temporary copy was used only for the builder after the patch paths failed; the diff was inspected before replacement.
4. The first DOCX field XML placed `fldChar` and `instrText` directly under a paragraph. Python could parse it, but Word rejected it. The fields were repaired by placing them inside runs.
5. The automatic dirty TOC opened in Word but stalled during field update/save. It was replaced by a static contents page using measured Word page numbers.
6. The required `render_docx.py` path could not run because LibreOffice was not installed.
7. Microsoft Word `ExportAsFixedFormat` and `SaveAs2` both stalled. The same failure occurred with a one-page smoke DOCX, so the PDF export failure was environmental rather than report-size-specific. Hidden Word processes started by these tests were identified by PID and closed.

## Fixes Applied

- Corrected custom numbering XML order.
- Corrected Word field XML run structure.
- Replaced the stalling automatic TOC with a stable one-page static contents list.
- Measured all major section pages through Word and corrected five initial TOC estimates.
- Rebuilt the DOCX after replacing collaboration-tool wording with formal project-review wording.
- Promoted the verified final file to `Ghost_Final_Report_Draft.docx`; generated intermediate versions were moved under `tmp/dissertation_docx_versions/`.

## What Was Intentionally Not Changed

- No new game features were added.
- No existing runtime code, validators, sessions, scenes, sample puzzle data, `.meta` files, ProjectSettings, or Packages were changed in this writing run.
- `Docs/CURRENT_TASK.md`, `Docs/HANDOFF_LOG.md`, and completed-task archives were not advanced. Closure remains Claude's responsibility.
- No learning-gain, usability, accessibility-conformance, or effective-tutor claim was added.
- No PDF was delivered because visual rendering could not be completed honestly.

## Remaining Risks

- PNG/PDF visual QA was not completed because LibreOffice was unavailable and Word PDF export failed even for a one-page control file. Word open, pagination, structural, style, geometry, and accessibility checks passed, but they do not replace page-image inspection.
- The final report still needs supervisor/Claude review, student verification of every citation, final KCL formatting, figures or screenshots, and a final asset/licence table.
- The static contents page must be remeasured if later edits change pagination.
- The Play Mode checklist remains provisional and has no independent evidence record.
- There is no participant study. The evaluation cannot support a learning-gain claim.
- The Granite hint score was an internal single-reviewer score and covers only Chapters 1-3.

## Next Recommended Step

Give `Docs/CLAUDE_REVIEW_PROMPT_DISSERTATION_DRAFT_001.md` to Claude. Claude should first decide M0-T49 closure, then verify citations and implementation claims, score the report independently against the KCL rubric, and return a short priority edit plan. Before submission, perform a final visual check on a machine with a working Word or LibreOffice PDF export path.

## Chinese STAR

- **S ???** ????? Unity ????????????????????????????????????????????????????????????
- **T ???** ??????? Claude ??????????????????? 2025 ? 2026 ????????????????
- **A ???** ????????????????????????? Granite ??????? 27 ??????????? A4 DOCX???????????? Claude review prompt?
- **R ???** ??? 10,621 ??????? 32 ? DOCX???????????????????????????????????? 73 ? 76 ??????????????????? QA?
