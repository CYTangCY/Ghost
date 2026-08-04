# Dissertation Work Completed Summary

## Overview

Codex completed three main report passes before this handoff. The detailed records are:

- `Docs/codex_runs/M0-T49_010_dissertation_literature_results_evaluation.md`;
- `Docs/codex_runs/M0-T49_011_kcl_latex_report_restructure.md`; and
- `Docs/codex_runs/M0-T49_012_lily_footwear_plain_language_audit.md`.

This summary lists outputs and evidence. It does not replace the report or the run logs.

## Pass 1: Literature, Results, and Evaluation

Created and expanded:

- `unorganized_data/dissertation/Ghost_Final_Report_Draft.md`;
- `unorganized_data/dissertation/Ghost_Final_Report_Draft.docx`;
- `unorganized_data/dissertation/LITERATURE_MATRIX_2025_2026.md`;
- `unorganized_data/dissertation/MARK_ESTIMATE.md`;
- `unorganized_data/dissertation/build_final_report_docx.py`;
- Granite prompt-bank evidence and scoring files under `unorganized_data/dissertation/evidence/`.

The literature review was organised around three arguments rather than a paper-by-paper list. It compares supporting and conflicting sources, narrows the research gap, and links the final design rules to the evidence.

The results and evaluation report:

- curriculum-to-puzzle mapping for Chapters 1 to 6;
- the recorded Unity result of 87 EditMode tests passed;
- the recorded backend result of 10 tests passed;
- scene and static guards from the implementation runs;
- 27 real Granite hint outputs from Chapters 1 to 3;
- an internal Granite total of 163/270, or 60.3704%;
- weak Lily voice and hint-level separation;
- two answer-revealing Chapter 2 hints; and
- the absence of participant evidence.

## Pass 2: KCL LaTeX Structure

Created a full LaTeX project at:

- `unorganized_data/dissertation/latex/Ghost_Final_Report.tex`;
- `unorganized_data/dissertation/latex/contents/`;
- `unorganized_data/dissertation/latex/figures/`.

The report now includes:

1. cover and second title page;
2. abstract;
3. acknowledgements;
4. nomenclature;
5. contents;
6. lists of figures and tables;
7. introduction;
8. background, literature review, and theory;
9. objectives, specification, and design;
10. methodology and implementation;
11. results, analysis, and evaluation;
12. legal, social, ethical, and professional issues;
13. conclusion and future work;
14. BibTeX references; and
15. labelled appendices.

Seven labelled figure calls were added for chapter flow, architecture, Shell hub, Chapter 3 graph, Chapter 6 backend sockets, Final Chapter pipeline, and Granite results. They remain placeholders until real images are supplied.

The design chapter compares:

- Unity, Godot, Phaser, and Unreal;
- Node.js/TypeScript, FastAPI, ASP.NET Core, and serverless functions;
- SQLite, PostgreSQL, MongoDB, and JSON files; and
- Ollama/Granite, hosted APIs, direct Hugging Face serving, and static hints.

The professional chapter applies BCS and IET principles to actual project choices. It covers privacy, security, LLM learner control, accessibility, copyright, licences, environment, and honest reporting.

## Pass 3: Plain-English Audit

Created `unorganized_data/dissertation/audit_plain_english.py` and revised the Markdown source before regenerating LaTeX.

Latest result:

- 11 prose files checked;
- 9,554 words;
- 721 sentences;
- average sentence length 13.3 words;
- estimated Flesch Reading Ease 51.2;
- estimated Flesch-Kincaid grade 9.4;
- zero sentences over 32 words;
- zero listed AI-style phrases; and
- zero listed uncommon substitutes.

This supports clear mid-level academic English. It is not an official IELTS score and does not replace human proofreading.

## Static LaTeX Result

`unorganized_data/dissertation/check_latex_report.py` reports:

- 12 TeX files;
- 42 BibTeX entries;
- 42 used citation keys;
- 28 labels;
- 27 references;
- 7 report figures; and
- 10 table captions.

The static check passes. It checks citation-key use, labels, references, structure, contractions, Markdown remnants, and selected format rules. It does not compile or visually inspect the PDF.

## Supplied Review Files

The earlier user files are now preserved under `Docs/dissertation_review_sources/`:

- original legacy Word cover sheet;
- readable cover field summary;
- complete supplied LaTeX template folder;
- supplied report chapter and format guidance;
- supplied `__MACOSX` archive metadata for completeness;
- extracted February 2025 KCL marking rubric;
- extracted earlier individual project report; and
- SHA-256 file list.

Read `Docs/dissertation_review_sources/README.md` before using them.

## Current Mark Estimate

`unorganized_data/dissertation/MARK_ESTIMATE.md` records:

- current defensible range: 64-69;
- current central estimate: 67;
- conditional range after required completion: 69-74; and
- conditional central estimate: 71.

The main reason the current estimate is below a secure Distinction is evaluation, not code volume. The project has no learner study, comparison group, later-recall measure, independent Play Mode record, or independent Granite rating.

## Items Still Open

1. Fill student number, degree programme, supervisor, date, release choice, signature, acknowledgements, and final word count.
2. Compile with a real TeX system and fix layout, overflow, package, and bibliography warnings.
3. Replace seven placeholders with real figures or checked diagrams.
4. Verify every reference field against its publisher or official primary page.
5. Record the final 1920x1080 and WebGL/browser checks with screenshots and dates.
6. Add or check an asset and licence record.
7. Complete accessibility evidence or keep the limitation explicit.
8. Obtain independent review of a sample of Lily outputs if time permits.
9. Proofread for student voice after all structural edits.

## Claims That Must Stay Limited

- The project shows that selected concepts were turned into playable actions.
- Automated tests show repeatable software behaviour.
- The internal Granite check shows current prompt strengths and failures.
- No evidence shows that the game improves learning, later recall, enjoyment, usability, or accessibility.
- The supplied LaTeX template is unofficial according to its own README and needs current KCL checking.
- All 42 citation keys are used, but full bibliographic field verification is still open.