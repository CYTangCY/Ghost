# M0-T49 - Run 004 - Detailed chapter split review

## Task ID

M0-T49

## Run Number

004

## Date

2026-07-15

## Original Request / Codex Prompt Summary

The user asked for a detailed inspection after the Chapter 0 / Chapter 6 teaching / Final Chapter split. This run used a code-review stance and did not intentionally change gameplay code.

## Files Created

- Docs/codex_runs/M0-T49_004_detailed_chapter_split_review.md

## Files Modified

- None.

## Tests or Checks Run

- Reviewed Chapter0StoryPresenter, Chapter 6 controller/presenter/pointer views, Shell navigation/debrief flow, Final Chapter completion IDs, scene builders, and backend progress compatibility with line-level inspection.
- Calculated the Shell 1920x1080 vertical layout from authored fixed heights, padding, and spacing.
- Verified all four affected scenes contain exactly one Main Camera, Canvas, and EventSystem and no missing-script serialization entries.
- Verified all four scene-builder logs have zero compiler/exception matches and clean batchmode exits.
- Reran the complete Unity EditMode suite in Unity 6000.4.11f1.
- Unity Play Mode visual/interaction test: Not run — the project has no PlayMode test assembly or reusable screenshot/smoke harness, and interactive Game view control is outside this batch review.

## Test / Check Result

- Full EditMode suite passed: 77/77, 0 failed, 0 skipped.
- Scene structure and builder logs passed their static checks.
- Four actionable gameplay/UI findings remain: Return to Hub can falsely complete chapters; Chapter 0 Lily is rendered fully transparent; the 1080p Shell hub overflows its available height; and Chapter 6 reveals placement correctness before Run instead of relying only on its validator.
- A secondary click-handler conflict risk exists on filled Chapter 6 sockets, and the old Run 001 checklist still contradicts the Run 003 chapter mapping.

## Errors Encountered

- The workspace sandbox helper still fails while applying deny-read ACLs, so apply_patch could not read workspace files.
- No Unity compiler or test-run errors occurred.

## Fixes Applied

- None. This was an inspection-only run.

## What Was Intentionally Not Changed

- Did not edit gameplay, presentation, scenes, ProjectSettings, CURRENT_TASK, ROADMAP, HANDOFF_LOG, or completed-task archives.
- Did not stage, commit, push, revert, or overwrite existing uncommitted work.

## Remaining Risks

- Existing EditMode coverage proves deterministic validators but does not cover Chapter 0 visibility, Shell layout, Return-to-Hub completion semantics, or pointer interaction ordering.
- Human Play Mode verification remains required after the findings are fixed.

## Next Recommended Step

Fix the four confirmed gameplay/UI findings, add focused controller/state and UI geometry regression coverage, regenerate Chapter 0 and Shell scenes, rerun 77 EditMode tests, and complete the Run 003 Play Mode checklist at 1920x1080.
