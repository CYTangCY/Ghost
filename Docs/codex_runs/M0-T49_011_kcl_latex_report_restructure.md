# M0-T49 - Run 011 - KCL LaTeX Report Restructure

## Task ID

M0-T49 follow-on dissertation formatting support

## Run Number

011

## Date

2026-07-16

## Original Request / Prompt Summary

Challenge the earlier high mark estimate, use the supplied MSc cover sheet, KCL LaTeX template, and chapter instructions, store the format requirements in the project documentation, rewrite the report in LaTeX, and provide real or blank figures that the student can replace with screenshots.

## Files Created

- `Docs/DISSERTATION_FORMAT_REQUIREMENTS.md`
- `Docs/CLAUDE_REVIEW_PROMPT_DISSERTATION_LATEX_002.md`
- `Docs/codex_runs/M0-T49_011_kcl_latex_report_restructure.md`
- `unorganized_data/dissertation/build_latex_report.py`
- `unorganized_data/dissertation/check_latex_report.py`
- `unorganized_data/dissertation/latex/Ghost_Final_Report.tex`
- `unorganized_data/dissertation/latex/kclthesis.cls`
- `unorganized_data/dissertation/latex/kcl.png`
- `unorganized_data/dissertation/latex/README.md`
- `unorganized_data/dissertation/latex/contents/abstract.tex`
- `unorganized_data/dissertation/latex/contents/acknowledgements.tex`
- `unorganized_data/dissertation/latex/contents/nomenclature.tex`
- `unorganized_data/dissertation/latex/contents/introduction.tex`
- `unorganized_data/dissertation/latex/contents/literature_review.tex`
- `unorganized_data/dissertation/latex/contents/design.tex`
- `unorganized_data/dissertation/latex/contents/implementation.tex`
- `unorganized_data/dissertation/latex/contents/evaluation.tex`
- `unorganized_data/dissertation/latex/contents/professional_issues.tex`
- `unorganized_data/dissertation/latex/contents/conclusion.tex`
- `unorganized_data/dissertation/latex/contents/appendices.tex`
- `unorganized_data/dissertation/latex/contents/references.bib`
- `unorganized_data/dissertation/latex/figures/README.md`

## Files Modified

- `unorganized_data/dissertation/MARK_ESTIMATE.md` - replaced the optimistic 73-76 estimate with a present-state range of 64-69, central 67, and a conditional completed range of 69-74.

No existing game runtime, validator, session, scene, test, ProjectSettings, Package, or `.meta` file was changed in this run.

## Supplied Format Review

- Read the supplied chapter and formatting instructions from the attached text file.
- Read the supplied `Thesis.tex`, `kclthesis.cls`, `Readme.md`, example chapter files, BibTeX example, and compiled template assets.
- The supplied README states that the template is unofficial and derived from an Imperial College template. This caveat is recorded in the project documentation.
- The `__MACOSX` folder was treated as archive metadata and was not copied.
- Microsoft Word blocked the legacy cover-sheet `.doc` through Trust Center File Block. No Office security setting was changed.
- The supplied LaTeX class contains the working cover fields and was copied into the report project. Its sample signature was not copied. The local class shows a blank signature line when the student has not supplied `figures/signature.png`.
- The supplied KCL logo was copied unchanged.

## Report Structure Result

The LaTeX project contains:

1. official-style cover and second title page;
2. Abstract;
3. Acknowledgements;
4. alphabetic Nomenclature;
5. Contents;
6. List of Figures;
7. List of Tables;
8. Introduction;
9. Background and Literature Review with Background Theories;
10. Objectives, Specifications and Design;
11. Methodology and Implementation;
12. Results, Analysis and Evaluation;
13. Legal, Social, Ethical and Professional Issues;
14. Conclusion and Future Work;
15. BibTeX References; and
16. four labelled appendices in the same document.

The professional chapter now applies the BCS Code of Conduct and IET Rules of Conduct to deterministic scoring, local data handling, honest evidence reporting, professional competence, and unfinished accessibility work.

## Figure Result

Seven `reportfigure` calls were added:

- chapter flow;
- architecture;
- Shell hub;
- Chapter 3 dialogue graph;
- Chapter 6 backend sockets;
- Final Chapter pipeline; and
- Granite score chart.

Each call has a caption, label, main-text reference, expected PNG name, and appendix checklist entry. Missing files render as labelled blank boxes. No gameplay screenshot was invented.

## Citation Result

- 42 BibTeX entries.
- 42 used citation keys.
- Zero missing and zero unused BibTeX keys in the static check.
- Added official BCS, IET, ICO, WCAG, engine, backend, database, and model-serving documentation.
- The report still requires final student or Claude verification of every bibliographic field against the primary source.

## Tests and Checks Run

- `build_latex_report.py` Python syntax check: passed.
- `check_latex_report.py` Python syntax check: passed.
- LaTeX static checker: passed.
- Static checker evidence: 12 TeX files, 42 BibTeX entries, 42 used keys, 28 labels, 27 references, 7 report figures, and 10 table captions.
- Expected warnings: seven missing student-supplied figure files and five student metadata/acknowledgement TODOs.
- Author-year remnants in generated chapter files: zero.
- Markdown remnants in generated chapter files: zero.
- Contractions: zero.
- Measured floating values without four decimal places: zero.
- Non-ASCII scan across new `.tex`, `.bib`, `.md`, `.py`, and copied class sources: zero.
- `git diff --check`: passed.
- TeX compilation: Not run - no `pdflatex`, XeLaTeX, LuaLaTeX, latexmk, or other TeX compiler was installed in the environment.
- Final PDF visual inspection: Not run - the project could not be compiled in this environment.
- Unity and backend tests: Not run - this run changed dissertation files only and used the already recorded evidence.

## Errors Encountered

1. Microsoft Word refused the supplied legacy `.doc` because the file type is blocked by Trust Center File Block.
2. No legacy Word text extractor was installed.
3. No TeX compiler was installed.
4. The bundled `pdftoppm.cmd` wrapper had a bad path. The underlying executable could rasterise the supplied template PDF, but the image-view helper then hit the known Windows sandbox ACL error. The source template and class were still inspected directly.
5. One multi-hunk `git apply` patch was rejected as corrupt. Narrow verified edits were then applied directly to the new converter file.
6. The first MongoDB citation insertion missed a newline and caused a Python syntax error. The newline was fixed, the script was recompiled, chapters were regenerated, and the static checker passed.

## Fixes Applied

- Corrected the mark estimate so unfinished work is not rewarded in advance.
- Added the supplied KCL structure and cover class.
- Added the safe blank-signature fallback.
- Converted author-year prose to BibTeX citations.
- Narrowed longtable column widths and removed invalid centering wrappers.
- Added four-decimal formatting for measured floating values.
- Added BCS and IET project-specific discussion.
- Added a checker that fails on missing or unused citations, unresolved labels, duplicate labels, unreferenced figures/tables/appendices, unbalanced braces/environments, contractions, Markdown remnants, wrong precision, and unexpected TODOs.

## What Was Intentionally Not Changed

- No game feature or code was added.
- The old DOCX and Markdown drafts were retained as working sources.
- The sample signature in the supplied template was not copied.
- No Office Trust Center setting was changed.
- No fake screenshots or fake user-study evidence were created.
- No claim of learning gain, usability success, accessibility conformance, or effective tutoring was added.
- `CURRENT_TASK.md`, task archives, and `HANDOFF_LOG.md` were not advanced; closure remains Claude's responsibility.

## Remaining Risks

- The LaTeX project has not been compiled. Package, overflow, bibliography-layout, page-break, and cover-layout errors may remain.
- Seven figure placeholders must be replaced and checked at 100% PDF zoom.
- Student number, programme, supervisor, submission date, release choice, signature, acknowledgement, and final word count require student input.
- The legacy `.doc` cover must be compared manually against the compiled cover.
- All 42 references need final field-by-field source checking.
- The Play Mode evidence remains provisional.
- No participant study exists. The current defensible mark estimate is 64-69; 69-74 is conditional on completing the remaining format and evidence work.

## Next Recommended Step

Open `unorganized_data/dissertation/latex/Ghost_Final_Report.tex` in Overleaf or a TeX Live/MiKTeX environment. Fill the personal TODOs, compile through BibTeX, fix all warnings and overflow, add the seven figures, and inspect every page. Then give `Docs/CLAUDE_REVIEW_PROMPT_DISSERTATION_LATEX_002.md` to Claude for strict content, citation, format, and rubric review.

## Chinese STAR

- **S ???** ???????????????????????????? 73 ? 76 ?????????????????? KCL LaTeX ???
- **T ???** ????????LaTeX template ??????????????????????????????????????
- **A ???** ???? KCL LaTeX ???42 ? BibTeX??????????????????????????????? BCS/IET ????????
- **R ???** LaTeX ???????????????????????? 64 ? 69 ???????????????????????? 69 ? 74 ??
