# M0-T49 Run 015 - Supervisor Progress Deck Restructure

## Run Identity

- Task ID: M0-T49
- Run number: 015
- Date: 2026-07-17
- Scope: presentation and documentation only

## Original Request Summary

Rework the supervisor progress presentation after the user found the Run 014 deck unfocused, dense,
and unsuitable for a progress meeting. Explain the purpose of disputed slides before changing them.
After discussion, add:
- one detailed implemented-system architecture diagram; and
- one next-few-days Future Work slide for Chapter 7 that does not falsely claim full IBM-course coverage.

## Files Created

- `unorganized_data/presentation/supervisor_progress_2026-07-17/Ghost_Progress_2026-07-17_rebuilt.pptx`
- `unorganized_data/presentation/supervisor_progress_2026-07-17/Ghost_Progress_2026-07-17_revised.pptx`
- `Docs/codex_runs/M0-T49_015_supervisor_deck_restructure.md`
- `Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN015.md`

## Files Modified

- `unorganized_data/presentation/supervisor_progress_2026-07-17/build_slides.py`
  - Rebuilt the meeting deck around a single sequence: progress, playable route, demo, research
    boundary, literature, design response, architecture, evidence, dissertation status, future work,
    and supervisor decisions.
  - Reduced the main deck from 19 slides to 13 slides.
  - Added a detailed architecture slide showing the deterministic Unity gameplay path separately
    from optional REST, SQLite, and Granite services.
  - Replaced the generic work-priority slide with a four-day Chapter 7 Future Work plan.
  - Kept the Future Work claim honest: Chapter 7 adds NLP breadth but does not complete the entire IBM
    SkillsBuild course.
  - Added machine checks that reject mark estimates, KCL rubric bands, missing notes, and unexpected
    placeholders.
- `unorganized_data/presentation/supervisor_progress_2026-07-17/granite_scores.png`
  - Regenerated from the unchanged prompt-bank CSV.
- `unorganized_data/dissertation/latex/figures/granite-results.png`
  - Refreshed as the byte-identical report chart copy.

## Final Deck Structure

1. Meeting purpose
2. Progress since the previous meeting
3. Playable chapter route
4. Product demo placeholders
5. Research question and boundary
6. Older literature versus recent evidence
7. Three design rules
8. Detailed implemented-system architecture
9. Verified software evidence and claim limits
10. Granite negative result
11. Dissertation completion status
12. Chapter 7 Future Work for the next four days
13. Three supervisor decisions

## Architecture Slide Content

- Unity 6 / WebGL presentation layer
- deterministic validators, sessions, authored data, and DialogGraphSimulator
- GhostBackendClient, BackendSync, local validation, and static hints
- Node.js + TypeScript + Express REST routes and services
- SQLite tables
- Ollama + IBM Granite uses
- explicit guard: no scoring endpoint and `/content` omits answer keys
- separate gameplay and optional-service paths

## Future Work Slide Content

- Day 1: confirm Chapter 7 scope and IBM terminology
- Day 2: deterministic data, validator/session logic, and tests
- Day 3: playable presentation, Ghost consequence, retry, and Shell completion
- Day 4: Play Mode/WebGL evidence and report mapping
- playable target: POS tagging, sentiment analysis, and machine translation
- conceptual target: NLU, NLG, and speech recognition
- explicitly still outside full coverage: watsonx Assistant setup, platform selection,
  starting-channel planning, and detailed ML personalisation

## Tests and Checks Run

- Python syntax compile for `build_slides.py`: Passed.
- Builder source non-ASCII scan: 0 non-ASCII bytes.
- Builder run after restructure: Passed.
- Second clean rebuild: Passed; output hash changed, proving overwrite.
- OOXML validation: 13 slides, 13 note slides, 0 content placeholders, exactly 2 labelled screenshot
  replacement boxes.
- Artifact-tool reopen: 13 slides and 13 note records.
- Bundled `slides_test.py`: Passed - no overflow detected.
- Artifact layout JSON warning scan: 0 overlap/overflow/out-of-bounds/warning matches.
- Visual QA: all retained slide layouts were inspected during the restructure; the new architecture
  and Future Work slides were rendered and inspected individually.
- Chart copy comparison: byte-identical; SHA-256
  `bbabd5c1fe00a9c174fcc0e8979ac6395e75cf928126c511aa0e187b04d6186f`.
- Final revised PPTX: 136,769 bytes; SHA-256
  `0E2307A5EF6A76DA9C01675EECC6D3AD94BAA764728DBF18AF7717C576506`.
- `git diff --check`: Passed. Existing line-ending warnings were reported for unrelated dirty files.

## Errors Encountered and Fixes Applied

1. The Windows sandbox helper repeatedly failed with ACL errors. The standard `apply_patch` wrapper
   also returned access denied. Changes were made with bounded elevated PowerShell writes and then
   compile/diff checked.
2. The original Run 014 PPTX was open and locked. It was preserved; revised outputs use new filenames.
3. The first simplified deck validation expected chart values in editable text although they were
   inside a raster chart. The values were added to speaker notes for machine-verifiable traceability.
4. A PowerShell replacement initially wrote literal newline markers into two Python lists. This was
   corrected before deck generation, and Python compilation then passed.
5. An interrupted slide-QA command left no stale Python, Node, or LibreOffice process.

## Intentionally Not Changed

- `Assets/**`
- `Backend/**`
- `ProjectSettings/**`
- `Packages/**`
- Unity scenes or `.meta` files
- dissertation `.tex` files
- `references.bib`
- existing run logs
- the two PowerPoint lock files created by the user's open PowerPoint sessions

## Unity Inspector Setup

Not applicable - presentation-only work.

## Play Mode Verification

Not run - presentation-only work; Chapter 7 is a proposed future task and was not implemented.

## Remaining Risks

- Slide 4 still contains two explicit screenshot replacement frames.
- Chapter 7 has not been mapped or implemented; the slide is a proposal, not completed work.
- The full IBM SkillsBuild course remains outside the current game scope even after the proposed
  Chapter 7.
- Microsoft PowerPoint desktop rehearsal was not run in this session.

## Next Recommended Step

Replace the two demo frames with final 1920x1080 captures, rehearse the 13-slide sequence in Microsoft
PowerPoint, and give the Run 015 review prompt to Claude.

## Chinese STAR

- **S 情境：** Run 014 的 19 頁簡報資訊過多，主線不清楚，也把成績推估、工具比較與大量附錄內容
  放進進度報告。
- **T 任務：** 重整成適合 supervisor meeting 的簡報，並加入詳細系統架構與誠實的 Chapter 7
  Future Work。
- **A 行動：** 將簡報重建為 13 頁單一路線，分開 deterministic gameplay 與 optional services，
  說明 Chapter 7 能補 NLP 但不能完成整門 IBM 課程，並完成重建、重開、溢位與視覺檢查。
- **R 結果：** 產出 13 頁 revised PPTX；13/13 notes、0 placeholders、2 個明確 demo 替換框，
  artifact-tool 重開成功，overflow 與 layout 檢查通過。

