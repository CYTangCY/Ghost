# M0-T11 — Act 1 Presentation Refactor

## Completion Status

Completed. Behaviour-preserving refactor; M0-T11 acceptance criteria met; committed and pushed by
the user.

## Date

2026-06-19

## Summary

Refactored the Act 1 presentation layer ahead of drag-and-drop, with no visible behaviour change.
Added an explicit presentation assembly boundary and extracted session/interaction orchestration out
of the overloaded `Act1IntentClassificationStaticPresenter` into a new small controller. This directly
addresses the M0-T10 review's primary risk (R1, the overloaded presenter) and R5 (no presentation
assembly). Delivered in a single Codex run (001).

## Files Created

- `Assets/Presentation/Ghost.Presentation.asmdef`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
- `Assets/Presentation/Act1IntentClassification/Editor/Ghost.Presentation.Editor.asmdef`
- `Docs/codex_runs/M0-T11_001_presentation_refactor.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

(All new presentation files verified present on disk during this closure; pure logic files and
EditMode tests confirmed unchanged.)

## What Moved From Presenter Into Controller

Moved out of `Act1IntentClassificationStaticPresenter` into the new
`Act1IntentClassificationInteractionController`:
- ownership of `IntentClassificationSession`
- the selected card id, plus select / deselect (toggle)
- assign and unassign operations
- validate
- state-changed and feedback callbacks (neutral / correct / incorrect feedback value types live in
  the controller file)

The presenter now creates the controller, subscribes to its state/feedback callbacks, renders from
controller state, and forwards UI click events to the controller — so it is focused on rendering,
visual state, and event wiring.

## Presentation Asmdef Changes

- `Ghost.Presentation.asmdef` (runtime) references `Ghost.Runtime`, `UnityEngine.UI`, and
  `Unity.InputSystem`.
- `Ghost.Presentation.Editor.asmdef` (Editor folder, for the scene builder) references
  `Ghost.Presentation`, `UnityEngine.UI`, and `Unity.InputSystem`.
- Presentation now compiles in its own assemblies rather than the default `Assembly-CSharp`.

## Behaviour-Preserving Verification

- Codex's diff review confirmed the presenter no longer owns the session, selected-card mutation,
  assignment, unassignment, or validation, and that the controller now owns/coordinates them.
- The scene builder did not need a code change: the same presenter script stays attached and creates
  the controller internally.
- No visible M0-T09 behaviour was intentionally changed; pure logic files and EditMode tests were
  not edited.

## Codex Run Log

- `Docs/codex_runs/M0-T11_001_presentation_refactor.md` (run 001, 2026-06-19).

Test/check result recorded honestly: **"Not run — Unity Editor Play Mode was not executed in this
Codex session."** and **"Not run — Unity Editor test runner was not executed in this Codex session."**
Codex's automated checks were source/diff review, an out-of-scope keyword search, presenter/controller
line-count checks, scoped `git status`, and `git diff --check` (clean). This closure does NOT restate
Codex as having run Play Mode or tests.

## Human Verification Result

Performed by the user in the Unity Editor after the Codex run:
- The project compiled successfully and EditMode tests passed.
- The Act 1 prototype scene opened and Play Mode entered without Console errors.
- All existing behaviour still worked: click card to select, click selected card again to deselect,
  click group to assign, assigned card can return to unassigned (Back), Validate button works, and
  validation feedback works.
- No drag-and-drop was present.
- No pure logic files, tests, ProjectSettings, Packages, Build Settings, or SampleScene changes were
  intentionally committed.

This human Editor verification is the source of the "compiles / tests pass / behaviour unchanged"
status — not the Codex session.

## Remaining Risks

- Verification is manual-in-Editor; there is no automated CI/Play Mode test run.
- New asmdefs trigger an assembly recompile on first open; if Unity shows stale scene/script
  references, the scene may need refreshing via `Ghost > Build Act 1 Intent Classification Prototype
  Scene`.
- Deferred from the M0-T10 review and still open (relevant once M0-T12 changes templates):
  - R2 — the presenter and scene builder still duplicate UI construction; drag/drop components may
    need adding in both places.
  - R4 — the scene builder still injects the presenter's private fields via reflection.
- Editor side effects can still appear in the working tree (e.g. `ProjectSettings/ShaderGraphSettings.asset`,
  untracked `ProjectSettings/SceneTemplateSettings.json`); check `git status` before committing.

## Next Task

M0-T12 — Add minimal drag-to-assign interaction for the Act 1 prototype, reusing the existing
session/controller flow.
