# Codex Run Logs

This folder stores one markdown log per Codex implementation or debugging run.

## Why this exists

- `Docs/completed_tasks/` stores completed project task records (one per task).
- `Docs/codex_runs/` stores per-run implementation/debugging logs created after each Codex run.
- `Docs/HANDOFF_LOG.md` keeps only a short chronological summary; the detailed per-run records
  live here.

## Convention

1. Every Codex implementation or debugging run creates exactly one markdown log file here.
2. Filename format: `TASKID_RUNNUMBER_short_description.md`. Run numbers are zero-padded to three
   digits and increase per task. Examples:
   - `M0-T04_001_intent_validator_initial_implementation.md`
   - `M0-T04_002_intent_validator_compile_fix.md`
   - `M0-T04_003_intent_validator_test_fix.md`
3. Each run log must include every field in the template below.
4. Do not claim tests passed unless the tests were actually run.
5. If tests were not run, state exactly: `Not run — [reason].`
6. Never delete or overwrite older run logs; each run gets a new file with the next run number.
7. Do not put hidden reasoning or private chain-of-thought in the log. Record actions, decisions,
   results, and evidence only.

## Run log template

```markdown
# TASKID — Run RUNNUMBER — short description

## Task ID

## Run Number

## Date

## Original Request / Codex Prompt Summary

## Files Created

## Files Modified

## Tests or Checks Run

## Test / Check Result
(Use "Not run — [reason]." if no tests were run.)

## Errors Encountered

## Fixes Applied

## What Was Intentionally Not Changed

## Remaining Risks

## Next Recommended Step
```
