# M0-T15 — Act 2 Entity Extraction Session / State Layer

## Completion Status

Completed. Codex run 001 implemented the session; run 002 fixed a failing NUnit assertion
(`Has.Count` → `CurrentSpans.Count`). The user ran the EditMode suite in the Unity Editor and
reported success ("測試成功"); Claude reviewed the code, tests, docs, and both run logs.

## Date

2026-06-22

## Summary

Added the Act 2 entity-extraction session/state layer, mirroring Act 1's `IntentClassificationSession`
(M0-T06). `EntityExtractionSession` (pure C#, `Ghost.Runtime`) owns one message's text + expected
spans + the player's **distinct** current spans, supports add/remove, rejects spans that fall outside
the message, and delegates correctness to `EntityExtractionValidator`. Scene-free, UI-free. The M0-T14
types were not changed.

## Files Created

- `Assets/Scripts/Puzzles/EntityExtraction/EntityExtractionSession.cs`
- `Assets/Tests/EditMode/Act2EntityExtractionSessionTests.cs`
- `Docs/codex_runs/M0-T15_001_act2_entity_extraction_session.md`
- `Docs/codex_runs/M0-T15_002_act2_entity_extraction_session_test_fix.md`

## Files Modified

- `Docs/CODE_WALKTHROUGH.md` — entries for the session + session-test scripts.
- `Docs/UNITY_TEST_CHECKLIST.md` — M0-T15 EditMode checklist with the six test names.

## Design / Behaviour

- The session owns state only; `EntityExtractionValidator` remains the single correctness authority
  (`ValidateCurrentState()` calls `Validate(expectedSpans, CurrentSpans)`).
- Current spans are kept distinct by `EntitySpan` value equality; `CurrentSpans` returns a snapshot
  copy; out-of-message spans throw `ArgumentOutOfRangeException`; duplicate-add is a no-op; removing
  an absent span returns false.
- Reuses the M0-T14 types and existing assemblies; no new asmdef.

## Claude Review Notes

- Scope: clean — only the session + test + two run logs + two doc updates; no asmdef / ProjectSettings
  / Packages / scenes / `CURRENT_TASK.md` changes; only Unity-generated `.meta` for the new scripts.
- Correctness: validator delegation correct; bounds/duplicate/remove semantics match the documented
  behaviour and the tests.
- Tests: 6 EditMode cases; run 002 fixed the `Has.Count` reflection failure by asserting
  `session.CurrentSpans.Count` directly (same gotcha previously seen in M0-T05 run 002).
- Run logs: honest "Not run" for Unity in both sessions; template-compliant; no chain-of-thought.
- Minor doc nit (cosmetic): `CODE_WALKTHROUGH.md` now has a duplicated
  `## Act 2 Entity Extraction EditMode Tests` header around the `SampleDataTests` entry.

## Codex Run Logs

- `Docs/codex_runs/M0-T15_001_act2_entity_extraction_session.md` — implementation.
- `Docs/codex_runs/M0-T15_002_act2_entity_extraction_session_test_fix.md` — `Has.Count` → `.Count`
  assertion fix. Both recorded Unity EditMode/Play Mode as **"Not run — Unity Editor/Test Runner is
  not available from this Codex shell session."**

## Human Verification Result

Performed by the user in the Unity Editor after run 002: the EditMode suite passed ("測試成功"). This
human Editor verification is the source of the "tests pass" status — not the Codex session.

## Remaining Risks

- Manual-in-Editor verification only; no CI/automated run.
- The low-severity M0-T14 note still applies: if a single message ever holds two entities of the same
  type, validator error **wording** could misattribute (`IsCorrect` unaffected).
- Unity-generated `.meta` files for the new scripts should be committed alongside them.

## Next Task

M0-T16 — Act 2 static span-annotation UI prototype scene (display-only), built via an Editor menu
scene builder and wired to the sample data / session, mirroring Act 1's M0-T07. Span-selection
interaction and validation feedback come in later sub-tasks.
