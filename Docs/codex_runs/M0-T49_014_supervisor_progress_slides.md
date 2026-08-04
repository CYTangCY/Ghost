# M0-T49 - Run 014 - Supervisor Progress Slides

## Task ID

M0-T49 follow-on supervisor progress presentation

## Run Number

014

## Date

2026-07-16

## Meeting Date

2026-07-17

## Original Request / Prompt Summary

Create a repeatable English 16:9 PowerPoint deck for the supervisor progress meeting. Cover game and dissertation progress, three critical literature samples, expected versus obtained results, Granite evaluation, KCL rubric status, submission plan, supervisor questions, and five backup slides. Every content slide should compare alternatives or evidence, expose a conflict, state the author's judgement, and connect it to a Ghost design decision. Use only named repository evidence and do not invent screenshots, data, citations, deadlines, or test results. The user later asked to compare the earlier literature with the recent 2025-2026 evidence as well.

## Files Created

- `unorganized_data/presentation/supervisor_progress_2026-07-17/build_slides.py`
- `unorganized_data/presentation/supervisor_progress_2026-07-17/Ghost_Progress_2026-07-17.pptx`
- `unorganized_data/presentation/supervisor_progress_2026-07-17/granite_scores.png`
- `unorganized_data/dissertation/latex/figures/granite-results.png`
- `Docs/codex_runs/M0-T49_014_supervisor_progress_slides.md`
- `Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN014.md`

## Files Modified

None.

No existing `Assets/**`, `Backend/**`, `ProjectSettings/**`, `Packages/**`, `.meta`, scene, `.tex`, `references.bib`, or existing documentation file was modified.

## Deck Result

- 19 slides in 16:9 format.
- 14 main slides and 5 backup slides.
- Speaker notes on every slide.
- Two labelled `REPLACE:` screenshot boxes on slide 3; no screenshot was invented.
- Comparison structure appears on every content slide through A/B evidence, conflict, judgement, and decision framing.
- Slides 4, 6, and 7 explicitly compare older literature with recent evidence:
  - Sweller 1988; Rowe 2011; Lester 2014; Plass 2015; Mayer 2019;
  - educational-chatbot reviews from 2020-2023; and
  - 15 verified 2025-2026 papers grouped as five papers per argument.
- The title slide uses the requested visible title `Ghost — Progress Report`; the builder remains ASCII-only through a Unicode escape.
- Backup slides cover backend, database, model serving, 15 recent sources, and all 27 Granite cases.

## Chart Result

`granite_scores.png` is generated from `llm_hint_prompt_bank_scores.csv` with matplotlib. It contains five scored areas plus the overall score, a white background, one neutral series colour, and labels in score/denominator and percentage form.

Final chart values:

- relevance 45/54, 83.3%;
- hint-level fit 32/54, 59.3%;
- technical accuracy 38/54, 70.4%;
- answer safety 38/54, 70.4%;
- Lily voice and format 10/54, 18.5%;
- overall 163/270, 60.4% in the chart and 60.3704% in deck text/notes.

The presentation and report copies are byte-identical.

## Implementation Note

The attached task requested `python-pptx`, while the active presentation skill explicitly requires `@oai/artifact-tool` and forbids `python-pptx`. `python-pptx` 1.0.2 was installed but was intentionally not used. `build_slides.py` is the repeatable entry point: it validates source snippets, parses the Granite CSV, generates the matplotlib chart, creates a temporary artifact-tool JavaScript module outside the repository, builds the deck, removes the exporter sidecar, and validates the final OOXML.

## Tests or Checks Run

1. `python -m py_compile build_slides.py`.
2. `python build_slides.py` first full build.
3. `python build_slides.py` second clean overwrite build.
4. Final title-correction rebuild through the same builder.
5. OOXML validation for slide count, note count, content placeholders, and `REPLACE:` boxes.
6. Required-stat assertion list against final slide and note text.
7. Artifact-tool reopen of the final PPTX.
8. Artifact-tool render of all 19 final slides.
9. Artifact-tool layout JSON export for all 19 slides.
10. `slides_test.py` overflow check using the bundled document Python runtime and correct HOME path.
11. Slide-by-slide visual inspection of all 19 rendered slides.
12. Layout JSON scan for overlap, overflow, out-of-bounds, and warning markers.
13. Chart SHA-256 comparison.
14. Output-directory file-list guard.
15. ASCII scan and trailing-whitespace scan for `build_slides.py`.
16. `git diff --check`.

## Test / Check Result

- Python syntax: passed.
- First full build: passed; 19 slides and 19 notes.
- Second clean overwrite: passed; output hash changed and the prior deck was replaced.
- Final title-correction build: passed.
- Final PPTX size: 197,721 bytes.
- Final PPTX SHA-256: `ff84d044f8734f91d81d321506ea3651771b91b8285497ef5572924ec2b7afae`.
- Final chart size: 92,478 bytes.
- Final chart SHA-256: `b2303dd99b045a51ac76ea093d3351b1c18f6b925a7274f16a2cd3f902feff55`.
- Slide count: 19.
- Speaker notes: 19; source markers 19; talk-track markers 19.
- Content placeholders: 0.
- Labelled `REPLACE:` boxes: exactly 2.
- Required-stat assertions: passed.
- Final artifact-tool reopen: passed; 19 slides.
- Final render: passed for all 19 slides.
- `slides_test.py`: passed; no overflow detected.
- Layout warning scan: 0 matches.
- Visual inspection: all 19 slides inspected; no unintended overlap, clipping, or incoherent text occlusion found. Apparent left-title clipping on two small transfer previews was disproved by bordered renders of the original final PNGs.
- Chart copies: byte-identical.
- Output folder contains only `build_slides.py`, the PPTX, and `granite_scores.png`.
- Builder non-ASCII: 0 matches.
- Builder trailing whitespace: 0 matches.
- `git diff --check`: passed; existing line-ending conversion warnings only.
- Python-pptx reopen: Not run — the active presentation skill explicitly forbids python-pptx; artifact-tool reopened and rendered the final PPTX instead.
- Microsoft PowerPoint desktop rehearsal: Not run — requires the user's interactive meeting environment.

## Errors Encountered

1. Initial source assertions used shortened phrases that did not exactly match Run 008 and the Granite evaluation. The assertions were changed to the recorded source wording; no data value changed.
2. The artifact-tool setup helper initially resolved HOME from the repository path and could not find the bundled package. HOME was set to `C:\Users\fcxsw`; the smoke test and full builds then passed.
3. Bundled document Python did not include matplotlib, while system Python had matplotlib 3.10.8. The repeatable Python builder uses the available system matplotlib.
4. Artifact-tool exported a `.pptx.inspect.ndjson` sidecar next to the deck. The builder now removes that sidecar after every build so the output folder stays within the requested file list.
5. `slides_test.py` first failed under system Python because `pdf2image` was missing. Bundled document Python had the dependency. Its first run still needed HOME corrected for artifact-tool discovery; the final run passed.
6. Two reduced JPEG transfer previews appeared to crop the first word of backup titles. Bordered renders of the original final slide PNGs showed the titles were complete. No deck edit was made for this false preview issue.

## What Was Intentionally Not Changed

- No game feature, runtime code, scene, asset, backend route, database, test, validator, or ProjectSettings file.
- No existing report prose, LaTeX, BibTeX, reference entry, or existing documentation.
- No screenshot was generated or substituted for the two rehearsal captures.
- No learner-study, usability, accessibility, Play Mode, WebGL, or mark claim was invented.
- No exact submission deadline was invented; slide 14 labels the weekly sequence as proposed and dates as needing confirmation.

## Remaining Risks

- Slide 3 still requires the Chapter 6 before/after and Final Chapter screenshots during rehearsal.
- The deck has been reopened and rendered with artifact-tool, but it has not been rehearsed in Microsoft PowerPoint on the supervisor-meeting machine.
- Dense backup tables are readable at full slide size but are not intended for normal presentation flow.
- Speaker notes exist but should be reviewed in the presenter's PowerPoint notes view.
- The deck uses the repository's current literature claims; supervisor or Claude source review may still request corrections.

## Next Recommended Step

Give `Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN014.md` to Claude. Then replace the two slide 3 boxes with the rehearsal screenshots, open the deck in PowerPoint, review notes, rehearse the main 14-slide path, and keep slides 15-19 as question-driven backup only.

## Chinese STAR

- **S 情境：** 明天需要向 supervisor 同時報告遊戲、論文、文獻批判與真實評估結果，但資料分散且仍有兩張遊戲截圖未取得。
- **T 任務：** 建立可重建的 19 張英文簡報，每張內容頁都呈現比較、衝突、作者判斷與 Ghost 設計決定，並加入舊文獻和 2025-2026 新文獻的交互比較。
- **A 行動：** 從 Run 008、Run 010、Granite CSV、文獻矩陣與 design.tex 擷取資料；用 matplotlib 產生圖表；用 artifact-tool 建構 PPTX；重建兩次、重新開啟、渲染 19 張、逐頁檢查並執行 overflow、notes、數字與雜湊守門。
- **R 結果：** 19 張投影片與 19 份 notes 全部產生，0 個空白 placeholder、2 個待換截圖框、0 個 overflow 或 layout warning；兩份 Granite 圖表完全相同，最終簡報可交給 Claude 審查與 supervisor rehearsal。