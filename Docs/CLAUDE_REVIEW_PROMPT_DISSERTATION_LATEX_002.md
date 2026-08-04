# Claude Review Prompt - Dissertation LaTeX 002

Read the repository before reviewing. This is a dissertation-format and evidence review, not a request for new game features.

## Read First

- `Docs/DISSERTATION_FORMAT_REQUIREMENTS.md`
- `unorganized_data/dissertation/latex/Ghost_Final_Report.tex`
- `unorganized_data/dissertation/latex/kclthesis.cls`
- every `.tex` and `.bib` file under `unorganized_data/dissertation/latex/contents/`
- `unorganized_data/dissertation/latex/README.md`
- `unorganized_data/dissertation/latex/figures/README.md`
- `unorganized_data/dissertation/Ghost_Final_Report_Draft.md`
- `unorganized_data/dissertation/LITERATURE_MATRIX_2025_2026.md`
- `unorganized_data/dissertation/MARK_ESTIMATE.md`
- `unorganized_data/dissertation/evidence/LLM_HINT_EVALUATION.md`
- `Docs/codex_runs/M0-T49_008_batchmode_verification.md`
- `Docs/codex_runs/M0-T49_011_kcl_latex_report_restructure.md`
- `unorganized_data/7CCSMPRJ_Rubric.pdf`

The student supplied these external format sources:

- `C:/Users/fcxsw/Downloads/MSc Final Report Cover Sheet.doc`
- `C:/Users/fcxsw/Desktop/Final Report Latex Template (7CCSMPRJ)/`
- `C:/Users/fcxsw/.codex/attachments/7997fde4-68e5-4318-8504-2fccd849e3a9/pasted-text.txt`

The legacy `.doc` was blocked by Microsoft Word Trust Center. Do not ask to weaken Office security. The working cover uses the supplied `kclthesis.cls`; the `.doc` still needs final manual comparison.

## Required Review

1. **Compile the LaTeX project if a TeX compiler is available.** Use `pdflatex`, `bibtex`, `pdflatex`, `pdflatex`. Report every undefined control sequence, missing citation, missing reference, overfull box, page-layout defect, and bibliography problem. If compilation is unavailable, say so exactly.
2. **Check format against the supplied structure.** Confirm the two cover pages, Abstract, Acknowledgements, Nomenclature, Contents, List of Figures, List of Tables, seven main chapters, References, and four labelled appendices are in one document with suitable numbering.
3. **Check remaining TODOs.** There should be only student number, programme, supervisor, submission date, and real acknowledgement text. Release choice, final word count, and personal signature also require student confirmation even if they are not written as `TODO`.
4. **Check figures.** Seven placeholders are intentional. Confirm that each has a caption, label, main-text reference, appendix checklist entry, and exact replacement filename. No placeholder may remain in the submission PDF.
5. **Check tables.** There should be ten captioned tables. Check width, page breaks, repeated longtable headers, readable text, and in-text references.
6. **Check citations.** Static checks found 42 BibTeX entries and 42 used keys. Verify authors, titles, years, venues, page ranges, DOI values, and official URLs against primary sources. Flag any reference that is incomplete or not supported by the cited claim.
7. **Check literature critically.** Each of the three main arguments should compare at least five 2025-2026 studies, state method and limits, include useful support or conflict, and lead to a Ghost design choice. Flag summary-only paragraphs and claims that exceed the evidence.
8. **Check technical comparisons.** Confirm the report compares Unity/Godot/Phaser/Unreal, Node.js/FastAPI/ASP.NET/serverless, SQLite/PostgreSQL/MongoDB/JSON, and Granite/hosted API/Transformers/static text at the correct project scale.
9. **Check results and evaluation.** Confirm 87 Unity tests, 10 backend tests, scene guards, and 27 Granite outputs against repository evidence. Do not allow claims of learning gain, usability success, accessibility conformance, or effective tutoring. The Play Mode record remains provisional.
10. **Check professional issues.** Confirm that BCS and IET principles are applied to real project decisions, not only named. Review privacy, security, accessibility, licence, LLM error, energy, professional competence, and honest reporting.
11. **Check style rules.** No contractions, simple English, consistent section numbering, four decimal places for measured floating values, captions and references for all figures/tables, and all appendices referred to in the main text.
12. **Score independently.** The corrected estimate is 64-69 now, central 67; conditional 69-74 after clean compilation, figures, citation verification, and final browser evidence. Challenge this estimate. Give a rubric score for all seven categories and explain what evidence prevents a higher mark. Do not reward unfinished work in advance.

## Known Static Result

`check_latex_report.py` passed with:

- 12 TeX files checked;
- 42 BibTeX entries and 42 used citation keys;
- 28 labels and 27 references;
- 7 figure placeholders;
- 10 table captions;
- 5 expected student metadata/acknowledgement TODOs.

Warnings are expected until the student supplies seven figures and five TODO values. No TeX compiler was installed in the Codex environment, so no project PDF or visual layout pass was produced.

## Response Format

1. Findings first, ordered P1/P2/P3, with exact file and line or section references.
2. Compilation and layout result.
3. Citation verification problems.
4. Rubric score table and defensible current mark range.
5. Exact remaining student actions before submission.
6. Chinese STAR: S situation, T task, A action, R result.
