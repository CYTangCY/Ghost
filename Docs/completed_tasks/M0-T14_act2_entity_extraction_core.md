# M0-T14 — Act 2 Entity Extraction Core (model + validator + sample data + tests)

## Completion Status

Completed. Implemented by Codex run 001 (pure C# logic, scene-free). The user ran the EditMode tests
in the Unity Editor and reported all tests passed ("測試都通過"); Claude reviewed the code, sample
data, tests, docs, and run log for scope and deterministic correctness.

## Date

2026-06-22

## Summary

Added the Act 2 entity-extraction logic core, mirroring the Act 1 logic-first pattern, with no
scene/UI. Pure C# in `Ghost.Runtime`: an `EntityType` value object (System/Custom category), an
immutable `EntitySpan` (start + length + type), a deterministic `EntityExtractionValidator`
(exact-match; missing / wrong-type / wrong-boundary / extra / duplicate / null errors), and
`Act2EntityExtractionSampleData` (system `time` + custom `room`/`object`, plus a `lab`/`laboratory`
synonym pair). EditMode tests cover the validator and the sample data. Act 1 logic, the Game Shell,
and all non-Act-2 code are unchanged.

## Files Created

- `Assets/Scripts/Puzzles/EntityExtraction/EntityType.cs` (+ `EntityCategory` enum)
- `Assets/Scripts/Puzzles/EntityExtraction/EntitySpan.cs`
- `Assets/Scripts/Puzzles/EntityExtraction/EntityExtractionValidator.cs` (+ `EntityExtractionResult`)
- `Assets/Scripts/Puzzles/EntityExtraction/Act2EntityExtractionSampleData.cs` (+ `SampleMessage`)
- `Assets/Tests/EditMode/Act2EntityExtractionValidatorTests.cs`
- `Assets/Tests/EditMode/Act2EntityExtractionSampleDataTests.cs`
- `Docs/codex_runs/M0-T14_001_act2_entity_extraction_core.md`

## Files Modified

- `Docs/CODE_WALKTHROUGH.md` — entries for all six new scripts.
- `Docs/UNITY_TEST_CHECKLIST.md` — M0-T14 EditMode checklist with the nine test names.

## Design / Behaviour

- Deterministic correctness: `EntityExtractionValidator` decides right/wrong; no UnityEngine
  dependency (stays in the `noEngineReferences` `Ghost.Runtime` assembly). Consistent with the
  project's deterministic-correctness rule.
- Span boundaries by start index + length (exact); types carry a System/Custom category. Synonyms are
  taught via sample data (different surface words → same type); system-vs-custom via `time` vs
  `room`/`object`.
- Reuses existing assemblies: no new asmdef (`Ghost.Runtime` covers `Assets/Scripts/**`,
  `Ghost.EditModeTests` covers `Assets/Tests/EditMode/**`).

## Claude Review Notes

- Scope: clean — only the 7 new files + 2 doc updates; no asmdef / ProjectSettings / Packages /
  scenes / `.meta` / Act 1 / Shell / `CURRENT_TASK.md` changes (confirmed via `git status` + reading
  the diffs).
- Correctness: traced correct / missing / wrong-type / wrong-boundary / extra / duplicate — `IsCorrect`
  is right in each.
- Tests: 9 EditMode tests; names recorded in `UNITY_TEST_CHECKLIST.md`; user-verified passing.
- Run log: honest "Not run" for Unity tests in the Codex session, with reasons; template-compliant;
  no chain-of-thought.

## Codex Run Logs

- `Docs/codex_runs/M0-T14_001_act2_entity_extraction_core.md` — recorded Unity EditMode/Play Mode as
  **"Not run — Unity Editor/Test Runner is not available from this Codex shell session."** This
  closure does not restate Codex as having run them.

## Human Verification Result

Performed by the user in the Unity Editor after the Codex run: the project imported/compiled and the
M0-T14 EditMode tests passed ("測試都通過"). This human Editor verification is the source of the
"tests pass" status — not the Codex session.

## Remaining Risks

- Manual-in-Editor verification only; no CI / automated Play Mode run.
- Validator error wording is logic-level; friendly player-facing copy is a future UI concern.
- Minor: if a single message later contains two entities of the same type, `FindSpanWithSameType`
  could misattribute a wrong-boundary / missing error **message** (`IsCorrect` stays correct). Current
  sample data has unique types per message; revisit when Act 2 data grows or the session/UI lands.
- Unity generates `.meta` files for the new scripts on import (not created by Codex); include them
  when committing.

## Next Task

M0-T15 — Act 2 entity-extraction session/state layer (pure C#, scene-free, EditMode tests), mirroring
the Act 1 M0-T06 session, before any span-annotation UI.
