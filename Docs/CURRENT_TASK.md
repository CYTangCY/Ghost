# CURRENT_TASK.md

## ID

M0-T19

## Goal

Finish Act 2 by integrating it into the Game Shell: add an act-select entry that starts the Act 2
scene, add a Return-to-Hub overlay for the Act 2 scene, and register the Act 2 scene in Build Settings
— mirroring how Act 1 is wired so Act 2 is reachable from the shell home.

## Context

ROADMAP Phase A built the Game Shell (title → act-select/hub → start Act 1 → Return to Hub). Phase B
built Act 2's full puzzle loop: M0-T14 core, M0-T15 session, M0-T16 static UI, M0-T17
selection/assignment, M0-T18 validation feedback — all Editor-verified. Act 2 currently has no entry
point from the shell. This task connects it the same way Act 1 is connected, so the prototype reads as
one Ghost game. The shell scripts (`ShellSceneNames`, `GameShellPresenter`, `ShellReturnToHubOverlay`,
`GameShellSceneBuilder`, optionally `ShellDialogueData`) are in scope for this task. Registering the
Act 2 scene in Build Settings is the approved exception (the same exception used in M0-T13 for the
shell + Act 1), applied by the user running the shell scene builder.

## Scope

- `ShellSceneNames`: add the Act 2 scene name + asset path constants.
- `GameShellPresenter`: add a "Start Act 2" act-select button + `StartAct2()` that loads the Act 2
  scene via `SceneManager`, wired in `Configure`/`Start` like the Act 1 button.
- `ShellReturnToHubOverlay`: also create the runtime "Return to Hub" button when the Act 2 scene is
  active (generalise the current Act-1-only check to Act 1 or Act 2), without touching Act 2 puzzle
  rules.
- `GameShellSceneBuilder` (Editor): add the Act 2 act-select button to the hub UI and register the
  Act 2 scene in Build Settings alongside the shell + Act 1.
- Optionally extend `ShellDialogueData` so the hub Lily line acknowledges both acts.
- Keep Act 2's puzzle behaviour and Act 1 unchanged.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md; create a Codex run log. The user re-runs
  `Ghost > Build Game Shell Scene` to regenerate the shell and apply Build Settings.

## Out of Scope

- Do not change Act 2 puzzle logic/UI behaviour (M0-T14–M0-T18) or Act 1 mechanics.
- Do not implement a full visual-novel dialogue system, save/load, the node graph, Act 3+, backend,
  LLM, or final art.
- Do not edit ProjectSettings beyond the approved Build Settings registration of the Act 2 scene; do
  not edit Packages or `.meta` files; do not hand-write scene YAML (use the builder).

## Acceptance Criteria

- From the shell hub, both "Start Act 1" and "Start Act 2" are available; clicking "Start Act 2" loads
  the Act 2 scene.
- The Act 2 scene shows a "Return to Hub" button that loads the shell scene.
- Act 2's puzzle (tag + Validate) still works after launching from the shell; Act 1 still launches and
  returns.
- Both `Act1IntentClassificationPrototype.unity` and `Act2EntityExtractionPrototype.unity` are enabled
  in Build Settings (the approved exception); no other ProjectSettings changes are intended.
- No Console errors in Play Mode; Act 2 puzzle logic and Act 1 are unchanged.
- CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md are updated; a Codex run log is created.
