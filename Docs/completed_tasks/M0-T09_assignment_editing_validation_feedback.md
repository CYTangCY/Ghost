# M0-T09 — Act 1 Assignment Editing, Group Capacity & Validation Feedback

## Completion Status

Completed. M0-T09 acceptance criteria met; implementation committed and pushed by the user.

## Date

2026-06-19

## Summary

Extended the Act 1 click-based prototype so the player can edit assignments, the right-side group
areas handle many assigned cards via scrolling, and a Validate button reports correct/incorrect
grouping. All behaviour is driven by the existing pure `IntentClassificationSession` /
`IntentClassificationValidator`; the interaction stays click-based and no drag-and-drop was added.
Delivered in a single Codex run (001).

## Files Created

- `Docs/codex_runs/M0-T09_001_assignment_editing_validation_feedback.md`

(No new C# files; the run modified the existing Act 1 presentation scripts.)

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/Editor/Act1IntentClassificationPrototypeSceneBuilder.cs`
- `Assets/Presentation/Act1IntentClassification/README.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

(Presentation `.cs` files verified present on disk during this closure. The prototype scene must be
refreshed via `Ghost > Build Act 1 Intent Classification Prototype Scene` to pick up the scrollable
assignment templates and Validate button.)

## Assignment Editing Behaviour

- Each assigned card now has a clickable "Back:" row that calls
  `IntentClassificationSession.MoveCardToUnassigned(...)` to return the card to the unassigned pile.
- Correction without restart: an assigned card can be selected again from the left list and moved to
  a different group via `MoveCardToGroup(...)`.

## Group Display Capacity Fix

- Replaced the fixed clipped assignment area with a vertical `ScrollRect` content area per intent
  group, so assigning many cards to one group no longer hides them — they remain inspectable by
  scrolling.
- Compact assigned-card row display keeps the scroll content readable.
- The Editor scene builder was updated to generate matching scrollable assignment templates.

## Validate Button Behaviour

- A generated Validate button (with feedback text) sits under the intent group column.
- Validate is wired to `IntentClassificationSession.ValidateCurrentState()`, which uses the existing
  pure `IntentClassificationValidator` — no validation logic was duplicated in the UI.

## Validation Feedback Behaviour

- The feedback text reports whether the current grouping is correct or incorrect, shown in the UI
  after the player presses Validate.

## Codex Run Log

- `Docs/codex_runs/M0-T09_001_assignment_editing_validation_feedback.md` (run 001, 2026-06-19).

Test/check result recorded honestly: **"Not run — Unity Editor Play Mode was not executed in this
Codex session."** and **"Not run — Unity Editor test runner was not executed in this Codex session."**
Codex's automated checks were limited to source review, out-of-scope keyword search,
`git diff --check` (clean), and confirming pure logic / ProjectSettings / Packages / SampleScene /
the prototype scene were not in the diff. This closure does NOT restate Codex as having run Play
Mode or tests.

## Human Verification Result

Performed by the user in the Unity Editor after the Codex run:
- The project compiled successfully and the Act 1 prototype scene opened.
- Play Mode entered without Console errors.
- Clicking cards and groups worked.
- Assigned cards could be removed / returned to unassigned.
- Wrong assignments could be corrected without restarting Play Mode.
- The right-side group display did not silently lose cards when many were assigned.
- Validate showed correct / incorrect feedback.
- No drag-and-drop was present.
- No ProjectSettings, Packages, Build Settings, or SampleScene changes were intentionally committed.

This human Editor verification is the source of the "works / no Console errors" status — not the
Codex session.

## Remaining Risks

- Verification is manual-in-Editor; there is no automated CI/Play Mode test run.
- The saved prototype scene may need regeneration via `Ghost > Build Act 1 Intent Classification
  Prototype Scene` to reflect the M0-T09 scroll/Validate templates.
- The presenter (`Act1IntentClassificationStaticPresenter`) now handles rendering, selection,
  assign, unassign, scroll lists, and validation feedback — its responsibilities are accumulating.
  This is exactly what the M0-T10 architecture review should examine before drag-and-drop.
- Editor side effects can still appear in the working tree (e.g. `ProjectSettings/ShaderGraphSettings.asset`,
  untracked `ProjectSettings/SceneTemplateSettings.json`); check `git status` before committing.
- No persistence; a fresh session is built per render (acceptable for the prototype).

## Next Task

M0-T10 — Light UI/code architecture review of the Act 1 prototype before adding drag-and-drop
(Claude review task; no implementation).
