# CURRENT_TASK.md

## ID

M0-T31

## Goal

Integrate Act 3 into the Game Shell so it is launchable from the hub and can return to the hub —
mirroring Act 2's M0-T19. Adds a "Start Act 3" hub entry + `StartAct3()`, a Return-to-Hub overlay for
the Act 3 scene, and registers the Act 3 scene in Build Settings.

## Context

Vertical-slice milestone (Docs/VERTICAL_SLICE_PLAN.md). Act 3's playable loop is done: M0-T21 core,
M0-T22 session, M0-T23 static UI, M0-T30 redesigned interaction + wired Validate. Act 3 currently has no
entry point from the shell. This connects it the same way Act 1 and Act 2 are connected (M0-T13 / M0-T19),
so all three acts are reachable from one place. Shell scripts (`ShellSceneNames`, `GameShellPresenter`,
`ShellReturnToHubOverlay`, `GameShellSceneBuilder`, optionally `ShellDialogueData`) are in scope.
Registering the Act 3 scene in Build Settings is the approved exception (same as M0-T13/M0-T19), applied
by the user running the shell scene builder.

## Scope

- `ShellSceneNames`: add the Act 3 scene name + asset path (`Act3DialogGraphPrototype`).
- `GameShellPresenter`: add an `act3Button` + `StartAct3()` (loads the Act 3 scene via `SceneManager`),
  wired in `Configure`/`Start` like the Act 1/Act 2 buttons.
- `ShellReturnToHubOverlay`: also show the runtime Return-to-Hub button when the Act 3 scene is active
  (Act 1 or Act 2 or Act 3), without touching Act 3 puzzle rules.
- `GameShellSceneBuilder`: add the Act 3 act-select card to the hub and register the Act 3 scene in
  Build Settings alongside shell + Act 1 + Act 2.
- Optionally extend `ShellDialogueData` so the hub Lily line acknowledges Act 3.
- Keep Act 3's puzzle behaviour and Acts 1/2 unchanged.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md; create a Codex run log. The user re-runs
  `Ghost > Build Game Shell Scene` to regenerate the shell and apply Build Settings.

## Out of Scope

- Do not change Act 3 puzzle logic/UI (M0-T21/M0-T22/M0-T30) or Act 1/2 mechanics.
- No full visual-novel dialogue, save/load, node graph for other acts, backend, LLM, or final art (the
  narrative pass is M0-T26).
- Do not edit ProjectSettings beyond the approved Build Settings registration of the Act 3 scene; do not
  edit Packages or `.meta`; do not hand-write scene YAML (use the builder).

## Acceptance Criteria

- From the shell hub, Start Act 1 / Act 2 / Act 3 are all available; clicking Start Act 3 loads the Act 3
  scene.
- The Act 3 scene shows a Return-to-Hub button that loads the shell.
- Act 3's puzzle still works after launching from the shell; Acts 1/2 still launch and return.
- Shell + Act 1 + Act 2 + Act 3 scenes are enabled in Build Settings (the approved exception); no other
  ProjectSettings changes are intended.
- No Console errors in Play Mode; Act 3 puzzle logic and Acts 1/2 are unchanged.
- CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md updated; a Codex run log created.
