# Dissertation Format Requirements

## Source Material

This checklist records the report-format material supplied by the project owner on 2026-07-16:

- `C:/Users/fcxsw/Downloads/MSc Final Report Cover Sheet.doc`
- `C:/Users/fcxsw/Desktop/Final Report Latex Template (7CCSMPRJ)/`
- `C:/Users/fcxsw/.codex/attachments/7997fde4-68e5-4318-8504-2fccd849e3a9/pasted-text.txt`

The `__MACOSX` directory contains macOS archive metadata only and is not part of the report template.

The supplied `Readme.md` says that the LaTeX template is unofficial and derived from an Imperial College template. It is still the project owner's supplied format, but the final student must compare it with current KEATS instructions before submission.

The legacy `.doc` cover sheet could not be opened in the automated run because Microsoft Word Trust Center blocked that old file type. No Office security setting was changed. The supplied `kclthesis.cls` contains the same visible metadata and release fields and is used as the working cover implementation. The `.doc` remains an item for final human comparison.

## Required Document Order

The report must remain one document, including appendices.

1. Official-style cover page.
2. Second title page from the supplied LaTeX class.
3. Abstract, normally no more than one page.
4. Acknowledgements.
5. Nomenclature in alphabetic order.
6. Table of contents with page numbers.
7. List of figures.
8. List of tables.
9. Introduction.
10. Background and Literature Review, including background theories.
11. Objectives, Specifications and Design.
12. Methodology and Implementation.
13. Results, Analysis and Evaluation.
14. Legal, Social, Ethical and Professional Issues.
15. Conclusion and Future Work, normally one or two pages.
16. References in one consistent style.
17. Labelled appendices that are all referred to in the main text.

## Main Writing Rules

- Keep the report below 15,000 words unless current KEATS instructions state a different limit.
- Aim for about 70% of the report to describe and evaluate the student's own design, implementation, evidence, and decisions.
- Use the student's own words. Do not copy course or paper wording.
- Do not use contractions.
- Use simple and direct English.
- Number sections and subsections consistently.
- Number pages consecutively. Front matter may use Roman numbers and the main text Arabic numbers.
- Give every figure and table a caption, number, label, and in-text reference.
- Give important equations numbers and punctuation when equations are used.
- Define every abbreviation in the nomenclature.
- Define variables and units when equations or measured variables are used.
- Show floating-point values to four decimal places.
- Keep scalar, vector, matrix, and equation font choices consistent when mathematical notation is used.
- Use one reference style consistently. The supplied template uses `ieeetr` and BibTeX.
- Include full source details where available: authors, title, venue or publisher, date, pages, DOI or official URL.
- Back up the report source and figures.

## Chapter Expectations

### Introduction

State the project background, problem, motivation, aims, objectives, techniques, reasons for those techniques, contribution, main result, and report structure.

### Background and Literature Review

Give the background theories. Compare relevant work rather than listing papers. State each study's method, useful evidence, limit, agreement or conflict with other work, and the final Ghost design decision. Keep the project gap narrow and defensible.

### Objectives, Specifications and Design

Turn the aim into requirements and explain how the design addresses them. Compare alternatives at the frontend, backend, database, model-serving, scoring, and deployment levels before giving the final choice.

### Methodology and Implementation

Explain and justify the development method and implementation steps. Demonstrate the student's contribution. State strengths and limits rather than only listing features.

### Results, Analysis and Evaluation

Explain how each result was obtained. Separate automated evidence, internal review, provisional human review, and missing evidence. Compare expected theory with actual behaviour. Do not claim learning gain, usability success, accessibility conformance, or effective tutoring without participant evidence.

### Legal, Social, Ethical and Professional Issues

Apply the BCS Code of Conduct and IET Rules of Conduct to project decisions. Discuss public interest, privacy, security, inclusion, accessibility, software trust, LLM error, intellectual property, licences, environmental cost, professional competence, and honest reporting.

### Conclusion

Answer the research question, summarise the contribution and results, state the main limits, and give specific future work. Do not add new evidence in the conclusion.

## LaTeX Project

The submission-format source is stored at:

- `unorganized_data/dissertation/latex/Ghost_Final_Report.tex`
- `unorganized_data/dissertation/latex/kclthesis.cls`
- `unorganized_data/dissertation/latex/contents/`
- `unorganized_data/dissertation/latex/figures/`

The KCL logo was copied from the supplied template. The sample signature was not copied. If `figures/signature.png` is absent, the cover shows a blank signature line.

The report uses figure placeholders that compile without screenshots. Adding a PNG with the expected filename replaces the blank box automatically. See `unorganized_data/dissertation/latex/figures/README.md` and Appendix C of the report.

## Metadata Still Required from the Student

- Student number.
- Exact MSc programme title.
- Supervisor name.
- Submission date.
- Release or non-release choice.
- Final word count after compilation.
- Final acknowledgement text.
- Personal signature, if required.

## Final Gate

Before submission:

1. Compile with `pdflatex`, `bibtex`, `pdflatex`, `pdflatex` or the equivalent Overleaf build.
2. Resolve all missing citation, label, overfull box, and undefined control-sequence warnings.
3. Replace every report figure placeholder.
4. Check cover details against the supplied legacy `.doc` manually.
5. Inspect every PDF page at 100% zoom.
6. Check that contents, figure list, table list, references, and appendix page numbers are correct.
7. Run the final WebGL/browser and human Play Mode evidence check.
8. Recalculate the final word count and update the cover.
