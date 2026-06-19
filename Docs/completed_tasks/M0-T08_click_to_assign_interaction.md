# M0-T08 — Act 1 Click-to-Assign Interaction

## Completion Status

Completed. M0-T08 acceptance criteria met; implementation committed and pushed by the user.
One known UI capacity issue is deferred to M0-T09 (see below).

## Date

2026-06-19

## Summary

Added the first interaction to the Act 1 intent-classification prototype: click a message card to
select it, then click one of the three intent group areas to assign it. The interaction is driven
by the existing pure `IntentClassificationSession`; no drag-and-drop was implemented. Run 001 added
the core click flow and a new-Input-System `EventSystem`; run 002 added deselection, auto-clearing
of selection after assignment, and clipping/compacting so assigned text stays inside the group
panels.

## Files Created

- `Docs/codex_runs/M0-T08_001_click_to_assign_interaction.md`
- `Docs/codex_runs/M0-T08_002_click_assignment_ui_fix.md`

(No new C# files. Both runs modified the existing Act 1 presentation scripts.)

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/Editor/Act1IntentClassificationPrototypeSceneBuilder.cs`
- `Assets/Presentation/Act1IntentClassification/README.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

(Presentation `.cs` files verified present on disk during this closure. The prototype scene must be
refreshed via `Ghost > Build Act 1 Intent Classification Prototype Scene` to pick up the new
EventSystem, assignment-list template, and clipping.)

## Click-to-Select Behaviour

- The presenter creates a fresh `IntentClassificationSession` for the scene.
- Clicking an unselected message card selects it (visually highlighted).
- Clicking the currently selected card again clears the selection (deselect — added in run 002).
- UI clicks are routed through an `EventSystem` using `InputSystemUIInputModule` (new Input System),
  created by the scene builder.

## Click-to-Assign Behaviour

- Clicking an intent group area assigns the selected card via `IntentClassificationSession.MoveCardToGroup(...)`.
- The assigned card is highlighted and its message text is listed inside the target group area.
- After assignment, the selected-card state is cleared automatically (run 002).
- No drag-and-drop; assignment is click-only.

## Run 002 Fix Summary

- Deselection: clicking the selected card again clears the selection.
- Selection auto-clears immediately after a successful assignment.
- Added `RectMask2D` clipping to the group and assignment-list roots so assigned message text stays
  visually inside the right-side intent group panels (fixed the text-overflow issue).
- Reduced assigned-card row height and font size for a compact display that fits the placeholder
  group areas.
- Updated the scene builder so regenerated scenes produce the same clipped/compact layout.
- Note: run 002 recorded that the working tree already showed a modified
  `Assets/Scenes/Act1IntentClassificationPrototype.unity`, a modified
  `ProjectSettings/ShaderGraphSettings.asset`, and an untracked
  `ProjectSettings/SceneTemplateSettings.json` before its edits — these were Editor-generated side
  effects, not Codex edits.

## Codex Run Logs

- `Docs/codex_runs/M0-T08_001_click_to_assign_interaction.md` (run 001, 2026-06-19)
- `Docs/codex_runs/M0-T08_002_click_assignment_ui_fix.md` (run 002, 2026-06-19)

Both logs honestly record: **"Not run — Unity Editor Play Mode was not executed in this Codex
session."** and **"Not run — Unity Editor test runner was not executed in this Codex session."**
Codex's automated checks were limited to source review, out-of-scope keyword search, and
`git diff --check` (clean). This closure does NOT restate Codex as having run Play Mode or tests.

## Human Verification Result

Performed by the user in the Unity Editor after the Codex runs:
- Clicking a message card selects it.
- Clicking the same selected card again deselects it.
- Clicking an intent group assigns the selected card.
- Assignment clears the selected state.
- Right-side assigned text no longer overflows outside the group panel.
- Play Mode runs without Console errors.
- No drag-and-drop was implemented.
- Implementation was committed and pushed.

This human Editor verification is the source of the "works / no Console errors" status — not the
Codex sessions.

## Known Remaining UI Issue

If many or all message cards are assigned to the same right-side group area, some assigned messages
can disappear: the group area now clips overflow (`RectMask2D`) but does not yet provide scrolling,
sufficiently compact display, or an overflow indicator. This is explicitly deferred to M0-T09.

## Remaining Risks

- Verification is manual-in-Editor; there is no automated CI/Play Mode test run.
- The saved prototype scene may need regeneration via `Ghost > Build Act 1 Intent Classification
  Prototype Scene` to reflect the run 001/002 changes.
- Editor side effects (modified `.unity` scene, `ProjectSettings/ShaderGraphSettings.asset`, untracked
  `ProjectSettings/SceneTemplateSettings.json`) can appear in the working tree on Editor open/use —
  check `git status` and avoid committing unintended ProjectSettings changes.
- The known group-capacity / disappearing-cards issue remains until M0-T09.
- The presenter rebuilds a fresh session per render; there is no persistence yet (acceptable for the
  prototype).

## Next Task

M0-T09 — Improve Act 1 assignment editing and group display capacity, then add basic validation
feedback, without drag-and-drop.
