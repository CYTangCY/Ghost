# Dissertation Review Sources

This folder keeps the files that the user supplied for the final-report work, plus readable text extracts needed for review. Claude should use these files together with the current report under `unorganized_data/dissertation/`.

## Read First

1. `supplied/7CCSMPRJ Rubric extracted text.txt` - February 2025 KCL Informatics MSc project marking rubric. Use its weighted report areas for the mark estimate.
2. `supplied/Suggested Report Chapters and Requirements.txt` - supplied chapter order, content rules, formatting rules, and the statement that about 70% of the report should describe the student's work.
3. `MSc_Final_Report_Cover_Sheet_readable.txt` - readable field summary from the supplied legacy Word cover.
4. `supplied/Final Report Latex Template (7CCSMPRJ)/Readme.md` - notes from the supplied LaTeX template.
5. `supplied/Final Report Latex Template (7CCSMPRJ)/Thesis.tex` and `kclthesis.cls` - supplied example structure and class.
6. `supplied/Individual Project First Report extracted text.txt` - earlier project report retained for traceability, not as the final source of truth where the current project docs disagree.

## Preserved Originals

- `supplied/MSc Final Report Cover Sheet.doc` is copied unchanged from the user-supplied legacy Word file.
- `supplied/Final Report Latex Template (7CCSMPRJ)/` is a complete copy of the supplied template folder, including its example source and generated build files.
- `supplied/Suggested Report Chapters and Requirements.txt` is a copy of the attached chapter guidance.
- `supplied/__MACOSX/` is copied only so the supplied archive is complete. It contains macOS resource-fork and Finder metadata files. It is not report content and should not be used for grading.

## Added Text Sources

- `supplied/7CCSMPRJ Rubric extracted text.txt` is the local text extraction of the supplied rubric PDF used in earlier review work.
- `supplied/Individual Project First Report extracted text.txt` is the local text extraction of the earlier project report.
- `MSc_Final_Report_Cover_Sheet_readable.txt` is a best-effort field summary. The original `.doc` remains authoritative for what was actually supplied.
- `FILE_HASHES_SHA256.txt` records SHA-256 hashes for every file in this review-source folder.

## Current Report Files

These are not duplicated here:

- `unorganized_data/dissertation/Ghost_Final_Report_Draft.md` - editable report source.
- `unorganized_data/dissertation/latex/Ghost_Final_Report.tex` - main LaTeX entry point.
- `unorganized_data/dissertation/latex/contents/` - current report sections and BibTeX database.
- `unorganized_data/dissertation/LITERATURE_MATRIX_2025_2026.md` - literature comparison and source notes.
- `unorganized_data/dissertation/MARK_ESTIMATE.md` - current honest mark estimate.
- `unorganized_data/dissertation/evidence/` - Granite output and scoring evidence.

## Important Limits

- The supplied LaTeX README says the template is unofficial and derived from an Imperial College template. Claude must check it against current KCL submission rules rather than treating it as official merely because it was supplied.
- The report has not been compiled in this environment because no TeX compiler is installed.
- The current cover still contains student TODO fields.
- The seven planned report images are placeholders until real diagrams or screenshots are added.