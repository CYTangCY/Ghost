# M0-T49 Run 016 - IBM Coverage Future Work Slides

## Run Identity

- Task ID: M0-T49
- Run number: 016
- Date: 2026-07-17
- Scope: supervisor presentation and review documentation only

## Original Request Summary

Add new slide pages explaining why Chapter 7 alone does not cover the IBM course, which conceptual
topics remain, and the smallest additional scope needed to support a careful claim that Ghost covers
most conceptual topics while excluding the hands-on watsonx Assistant setup exercise.

## Files Created

- `unorganized_data/presentation/supervisor_progress_2026-07-17/Ghost_Progress_2026-07-17_expanded.pptx`
- `Docs/codex_runs/M0-T49_016_ibm_coverage_future_work_slides.md`
- `Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN016.md`

## Files Modified

- `unorganized_data/presentation/supervisor_progress_2026-07-17/build_slides.py`
  - Preserved the previous 13-slide deck as a separate file.
  - Changed the output to a new 16-slide expanded deck.
  - Reworked slide 12 into a focused four-day Chapter 7 plan.
  - Added slide 13 to separate Chapter 7 NLP gains from the remaining IBM topics.
  - Added slide 14 to explain four remaining conceptual gaps and why each matters.
  - Added slide 15 to show the smallest recommended three-part scope.
  - Renumbered the supervisor decision slide to slide 16.
  - Updated slide-count and required-text guards from 13 to 16.

## New Slide Purpose

- Slide 12: show what can be built and checked over four days.
- Slide 13: prevent the false claim that Chapter 7 equals full IBM-course coverage.
- Slide 14: explain rule-based vs AI-enabled systems, ML behaviour, chatbot planning, and platform
  choice in plain terms, including why each topic matters.
- Slide 15: recommend Chapter 7, one short planning interaction, and a small Voice Basics extension.
- Slide 16: retain only the supervisor questions that can change the final project scope.

## Tests and Checks Run

- Python syntax: passed with `python -m py_compile`.
- Artifact-tool deck generation: passed.
- OOXML validation: 16 slides, 16 notes, 0 content placeholders, 2 labelled screenshot replacement
  frames.
- Required deck evidence guard: passed for the new Chapter 7, remaining-gap, and recommended-scope
  statements.
- Visual review: slides 12-16 reviewed from rendered PNG/JPEG previews; no visible overlap, clipping,
  title wrapping, or unclear hierarchy found.
- Layout JSON keyword scan: no overlap, overflow, out-of-bounds, warning, or error matches across all
  16 slides.
- Bundled `slides_test.py`: passed; no overflow detected.
- Builder non-ASCII scan: 0 non-ASCII bytes.
- `git diff --check` on the builder: passed.
- Final deck size: 149,199 bytes.
- Final deck SHA-256:
  `0345B27642DDF9F7EA30CB10BE9677037E1961274BF77B0C7212E598F860D95E`.
- Microsoft PowerPoint live rehearsal: Not run - no live PowerPoint session was used.

## Errors Encountered

1. The Windows sandbox helper repeatedly failed with `helper_unknown_error: apply deny-read ACLs`.
   `apply_patch` failed for both absolute and relative paths.
2. The first generated 16-slide deck was rejected because the old OOXML guard still expected 13
   slides and notes.
3. The next run was rejected because the required-text guard still expected the old Chapter 7 title.
4. A PowerShell replacement wrote literal newline escape text into the Python list and caused a
   syntax error.
5. The first `slides_test.py` attempt used system Python without `pdf2image`.
6. The second attempt found the wrong artifact-tool runtime because `HOME` was not set for the
   Windows subprocess.

## Fixes Applied

- Used a bounded, escalated PowerShell replacement only after `apply_patch` failed twice.
- Updated both slide-count guards and required-text guards to the 16-slide structure.
- Repaired the malformed Python list and reran syntax validation before rebuilding.
- Used the bundled Codex Python runtime and set `HOME=C:\Users\fcxsw` for the final overflow test.
- Kept the old revised deck unchanged and exported a new expanded deck.

## Intentionally Not Changed

- No Unity scenes, scripts, tests, packages, project settings, or `.meta` files.
- No dissertation text or reference entries.
- No IBM course source document.
- No previous run log or previous presentation file.
- Chapter 7 was not implemented.
- The hands-on watsonx Assistant setup workflow remains outside the proposed game scope.

## Remaining Risks

- The two demo screenshot boxes still need real Unity captures.
- The expanded deck has not been rehearsed in Microsoft PowerPoint.
- Slide 15 is a recommendation, not completed implementation.
- The final coverage claim must stay limited to most conceptual topics from the selected IBM course,
  excluding hands-on watsonx setup.

## Next Recommended Step

Use the expanded deck for supervisor review. Ask the supervisor whether the three-part addition is
worth the remaining time before implementing Chapter 7 or changing the Shell teaching content.

## Chinese STAR

- **S 情境：** Chapter 7 可補 NLP 任務，但單獨完成後仍不能說已覆蓋整個 IBM 課程。
- **T 任務：** 把剩餘缺口、原因與最小補充範圍整理成有順序的新簡報頁面。
- **A 行動：** 將未來工作拆成五頁，分別說明四天計畫、Chapter 7 的範圍、四個剩餘概念、
  三項最小補充方案與老師需要決定的問題，並完成 16 頁簡報的版面與溢位檢查。
- **R 結果：** 新的 16 頁簡報成功產生，新增頁面沒有可見重疊或溢位，且保留誠實的課程
  覆蓋界線。