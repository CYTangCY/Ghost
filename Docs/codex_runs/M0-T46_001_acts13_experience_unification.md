# M0-T46 - Run 001 - Acts 1 and 3 experience unification

## Task ID

M0-T46

## Run Number

001

## Date

2026-07-11

## Original Request / Codex Prompt Summary

Use Act 2 as the presentation reference without modifying it. Add a pre-interaction Lily onboarding
beat and persistent phase objective strip to Act 1. Add the same wrapper to Act 3, plus a deterministic
Ghost face, clear failed-validation retry, visible completion into the existing Shell debrief, and
confirmation that the existing floating banter/chat components work in Act 3. Run 001 was restricted
to Parts A and B; Lily portrait work was explicitly deferred to Run 002.

## Files Created

- `Docs/codex_runs/M0-T46_001_acts13_experience_unification.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
  - Added the initial onboarding phase and one transition into the existing intro failures.
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
  - Added the runtime onboarding panel, persistent phase objective strip, and onboarding visibility lock.
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
  - Added presentation phases, retained validation-attempt state, deterministic Ghost reactions, and
    retry/success invalidation behaviour around the unchanged graph session.
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
  - Added onboarding/objective presentation, Ghost face, Try again/Complete Act states, Shell debrief
    return, and compact 1080p guide-column layout.
- `Docs/LEARNING_CONTENT.md`
  - Recorded the Act 1 wrapper and Act 3 experience-standard learning flow.
- `Docs/CODE_WALKTHROUGH.md`
  - Documented all four modified C# scripts and their Unity responsibilities.
- `Docs/UNITY_TEST_CHECKLIST.md`
  - Added the Run 001 human checks for onboarding, objectives, reactions, retry, completion, floating
    windows, deterministic scope, 1080p fit, and Console state.

## Tests or Checks Run

- `dotnet build Ghost.Presentation.csproj`
- `dotnet build Ghost.EditModeTests.csproj`
- `git diff --check`
- Non-ASCII scan of the four changed C# files.
- Before/after SHA-256 tree fingerprints for Act 2, Runtime puzzle code, tests, Fundamentals, Shell,
  Banter/Common/GhostAvatar, Backend, ProjectSettings, Packages, all scene YAML, and all existing `.meta`
  files.
- Git status/diff guards for Lily portrait paths and protected areas.
- Source inspection confirming `AmbientBanterHook` recognizes the Act 3 scene and that its `Ask Lily`
  action opens the existing draggable `LilyChatWindow`.
- Unity Play Mode: Not run — Unity Editor was not launched in this Codex session.
- Unity Test Runner: Not run — Unity Editor was not launched in this Codex session.

## Test / Check Result

- `Ghost.Presentation.csproj`: build succeeded with 0 errors and 2 CS0618 warnings for existing
  `FindFirstObjectByType` calls (`ShellReturnToHubOverlay` and Act 3 `EnsureEventSystem`).
- `Ghost.EditModeTests.csproj`: build succeeded with 0 warnings and 0 errors. This compiled the test
  assembly; it did not execute Unity Test Runner tests.
- `git diff --check`: no whitespace errors; Git reported only line-ending conversion warnings.
- Non-ASCII scan: no non-ASCII characters in changed C# files.
- Act 2 code, Runtime, tests, Fundamentals, Shell, shared presentation components, Backend,
  ProjectSettings, Packages, and existing `.meta` fingerprints matched the pre-edit baseline exactly.
- The scene fingerprint no longer matched during the 2026-07-12 follow-up: three Unity Editor
  processes were open and Act 1/2/3 scene files were externally serialized at approximately 01:46.
  Codex did not run Unity, a scene builder, or edit scene YAML. Those external scene changes were
  preserved and are not claimed as part of this implementation.
- No changes were reported under `Assets/Presentation/Characters` or `tmp/lily`.

## Errors Encountered

- The first `dotnet build --no-restore` could not find
  `Temp/obj/Ghost.Presentation/project.assets.json`.
- A normal sandboxed restore could not read the user-level NuGet config at
  `C:/Users/fcxsw/AppData/Roaming/NuGet/NuGet.Config`.
- During the user follow-up, the protected-scene guard detected concurrent Unity serialization in
  Act 1/2/3 scenes. The scene diff was not reverted because it was not created by Codex.

## Fixes Applied

- Re-ran the requested builds with approved elevated access so `dotnet` could read the existing
  NuGet configuration and generate local restore assets. Both final builds succeeded.
- Kept the Act 3 guide content within the calculated 1920x1080 vertical budget by shortening the
  play-step copy and using a compact face/outcome region.

## User Follow-up Refinement (2026-07-12)

The user clarified that parity means the complete Act 2 onboarding transition, not only matching the
warm panel and objective strip. Run 001 therefore also:

- keeps an Act 1 Ghost-problem conversation preview visible during onboarding;
- changes the Act 1 post-onboarding teaching panel into a compact Lily note with `Replay Lily`;
- adds an Act 3 Ghost reply-order problem preview beneath onboarding;
- changes that preview into a compact replayable Lily note when the graph unlocks;
- preserves graph/pile data when onboarding is replayed; and
- compacts the Act 3 guide labels/copy so the added note strip remains within the calculated 1080p
  vertical budget.

This is a refinement of the still-open Parts A/B Run 001. Part C portrait work remains separate and
was not started.

### Layout-alignment correction (2026-07-12)

The user then clarified that Act 1 and Act 3 must also share Act 2's page composition, not only its
interaction sequence. The presenters now normalize both roots to:

`56px header + progress -> 48px objective -> 180px onboarding / 54px Lily note -> 170px Ghost
conversation -> flexible puzzle body`.

Act 1 keeps its two-column training body; Act 3 keeps its graph-specific three-column body. Act 3's
Ghost face and deterministic test outcome moved from the Guide column into the shared conversation
panel, leaving the Guide for instructions, route legend, and test cases. The old standalone subtitles
are hidden because Lily's onboarding/note and the objective strip now carry that information.

## What Was Intentionally Not Changed

- All `Ghost.Runtime` puzzle validators, sessions, sample data, demo engines, and existing tests.
- All Act 2 files.
- Fundamentals, Shell flow logic, Backend, ProjectSettings, Packages, and Build Settings.
- Banter, Common, GhostAvatar, and Character/Lily portrait components.
- No scene YAML was hand-edited or generated by Codex. Concurrent Unity-generated scene changes were
  left untouched. Existing `.meta` files remained unchanged.
- Part C portrait refinement and `tmp/lily` preview workflow; these begin in Run 002.

## Remaining Risks

- Unity Play Mode must still confirm dynamic layout, raycast blocking during onboarding, deterministic
  face transitions, retry detail persistence, and Complete Act -> Act 3 debrief at runtime.
- The calculated 1920x1080 fit has not been visually inspected in the Unity Game view.
- Unity Console and EditMode Test Runner results remain unverified until the user runs the human
  checklist.
- The concurrent Act 1/2/3 scene serialization must be reviewed separately with the user's Unity
  results before commit; its large YAML diff is outside Codex's Run 001 code edits.
- The two CS0618 warnings are pre-existing API deprecation warnings and were not changed because their
  files/behaviour are outside this run's scope.

## Next Recommended Step

Run the M0-T46 Run 001 section of `Docs/UNITY_TEST_CHECKLIST.md` in Unity at 1920x1080, then send the
implementation and real Editor results to Claude for review/closure. Start Run 002 portrait work only
after Run 001 is accepted and the user provides or confirms the visual reference direction.
