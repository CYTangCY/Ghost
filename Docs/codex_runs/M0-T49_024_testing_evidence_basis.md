# M0-T49 - Run 024 - Testing Evidence Basis

## Task ID

M0-T49

## Run Number

024

## Date

2026-07-30

## Original Request / Codex Prompt Summary

Pause game implementation and rebuild the basis for testing and evaluation so that the dissertation
does not treat Unity's passing test count as a general measure of software quality.

## Files Created

- `Docs/TESTING_EVIDENCE_BASIS.md`
- `Docs/TEST_REQUIREMENT_TRACEABILITY_MATRIX.md`
- `Docs/codex_runs/M0-T49_024_testing_evidence_basis.md`

## Files Modified

None.

## Tests or Checks Run

- Read the current evaluation chapter drafts, methodology testing sections, requirements, KCL rubric,
  Unity test result XML, backend route tests, and Granite evaluation evidence.
- Parsed the latest Unity result into fixture-level counts.
- Checked the ten backend route-test names against the claims made in the report.
- Calculated the existing Granite score distributions and recomputed the row totals.
- Checked the testing approach against official ISO/IEC 25010, ISO/IEC/IEEE 29119, ISTQB, and Unity
  Test Framework material.
- Ran `git diff --check` on the new documentation files.
- Software tests: Not run - this run changed only the testing/evaluation evidence plan and did not
  change executable source.
- Unity Play Mode: Not run - this run defines the evidence that future Play Mode scenarios must
  record.

## Test / Check Result

- The current 96 Unity tests were separated into 83 mainly logic-oriented tests, 2 presenter smoke
  tests, 8 shell/navigation guards, and 3 later hint/panel checks.
- The ten backend tests were confirmed to cover local content/data routes and model success/fallback
  paths, but not security, load, concurrency, or public deployment.
- The existing Granite aggregate was found to be internally inconsistent: four criteria use 0-2,
  `voice_format` uses 0-1, and the report nevertheless states a maximum of 10 per output and uses 54
  as the voice denominator. The `60.4%` overall score should not be reused.
- Six evaluation questions, named Play Mode scenarios, three fault-injection scenarios, per-validator
  condition classes, and a seven-chapter LLM evidence scheme were defined.

## Errors Encountered

- The first broad repository search returned more output than was useful because the historical test
  checklist contains many superseded runs.

## Fixes Applied

- Switched to the current evaluation drafts, latest Unity XML, requirement file, backend test names,
  and raw Granite score CSV as the evidence sources.

## What Was Intentionally Not Changed

- No game, backend, database, test, scene, package, or ProjectSettings file was changed.
- The dissertation evaluation chapter was not rewritten yet; the evidence basis must be agreed and
  the missing records collected first.
- No new learning, usability, enjoyment, accessibility, security, or reliability claim was added.
- The existing Granite raw outputs and scores were retained for re-coding rather than overwritten.

## Remaining Risks

- The requirements and current implementation have evolved, so the FR/NFR matrix still needs a final
  consistency review against the dissertation's six stated requirements.
- Most Play Mode evidence remains a developer report without named result rows or retained artifacts.
- The complete LLM prompt bank does not yet cover Chapters 4-6 or the Final Chapter.
- A WebGL browser build and clean deployment record remain absent.

## Next Recommended Step

Freeze the evaluation build and create the evidence record sheet. Then run the named Play Mode and
fault-injection scenarios before rewriting the Results, Analysis and Evaluation chapter.

## Chinese STAR

- **S 情境：** 現有報告把 Unity pass 數、Play Mode、backend 與 Granite 分數混成籠統的 system
  quality，無法清楚說明每項證據支持什麼結論。
- **T 任務：** 重新建立以 requirements、風險與明確 acceptance criteria 為基礎的測試架構。
- **A 行動：** 盤點 96 個 Unity tests、10 個 backend tests、Play Mode 紀錄與 27 個 Granite
  outputs；建立六個 evaluation questions、requirements traceability、named scenarios、fault
  injection 與 LLM gate criteria。
- **R 結果：** 已建立兩份測試依據文件，並找出 Granite 總分分母不一致的問題。下一步是依表格
  收集可重現證據，再改寫 Evaluation，而不是只更新 pass 數字。
