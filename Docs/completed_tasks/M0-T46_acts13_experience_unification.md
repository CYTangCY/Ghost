# M0-T46 — Unify Acts 1 & 3 to the Act 2 Experience Standard (+ Lily portrait baseline)

## Completion Status

Completed. Codex run 001 (Parts A+B, including a user-driven layout-alignment correction) passed the
user's Play Mode verification ("M0-T46已經確認完成了") against the 8-point checklist: Act 1 keeps its
two-column classification body; Act 3 keeps its three-column graph body; Act 3's GhostFaceView +
deterministic result live in the upper conversation panel; the Act 3 Guide holds only instructions,
legend, and test cases; standalone subtitles hidden; onboarding/replay/state
preservation/retry/completion all work; 1920×1080 with no clipping/overlap; Act 2 and protected logic
unchanged.

## Date

2026-07-05

## Summary

Acts 1 and 3 now use Act 2's full top-level page composition, making the three chapters read as one
game: 56px header with right-side phase progress, 48px objective strip, 180px Lily onboarding panel
(dismissable, with a 54px replayable note), 170px persistent Ghost conversation/result panel, and a
flexible puzzle body with 18px column spacing. Act 1 gained the Lily onboarding beat + persistent
objective strip over its M0-T45 phase flow. Act 3 gained the onboarding beat, objective strip,
GhostFaceView driven only by deterministic validate/test outcomes (`MapGhostMood(controller.
CurrentReaction)`), a clear "Try again" retry state, and a "Complete Act" button returning through
`GhostNarrativeState.SetPendingDebriefAct` to the existing Shell debrief.

Part C baseline: the Lily portrait was reworked by Claude (user-authorized) into a 48×48 string pixel
map with large thin-frame round glasses (v8–v9). Further Korean-style iterations continue as side-runs
when the user supplies the reference image / feedback; the preview-PNG workflow for Codex is specified
in the M0-T46 prompt (parse rows from `LilyPixelPortraitFactory.cs`, render to `tmp/lily/`).

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs` + controller
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs` + controller
- `Assets/Presentation/Characters/LilyPixelPortraitFactory.cs` (v8–v9, Claude direct edits)
- Docs: `LEARNING_CONTENT.md`, `CODE_WALKTHROUGH.md`, `UNITY_TEST_CHECKLIST.md`; run log
  `Docs/codex_runs/M0-T46_001_acts13_experience_unification.md`
- Excluded from commits: the regenerated scene files (shelved side-effects)

## Claude Review Notes

- Scope guards empty across every protected path (Ghost.Runtime puzzle logic, Act 2, Fundamentals,
  Backend, ProjectSettings, Packages, GhostAvatar/Common/Banter/Shell internals).
- Unified layout constants verified in both presenters (48f strip / 180f onboarding / 170f
  conversation / 54f note / 18f spacing); Act 3 face driven by deterministic results only; dual-state
  Try again / Complete Act button; debrief wiring intact.
- Validators and Acts 1–3 puzzle behaviour unchanged; run log honest (Unity not run in-session).

## Human Verification Result

Passed — user confirmed completion after Play Mode verification of the 8-point checklist.

## Next Task

M0-T47/T48/T49 — Chapters 4–6 build-out (Act 4 confidence/fallback slider; Act 5 testing & debugging;
final capstone + ending animation). Spec in `Docs/CURRENT_TASK.md`; Codex prompt issued 2026-07-05.
