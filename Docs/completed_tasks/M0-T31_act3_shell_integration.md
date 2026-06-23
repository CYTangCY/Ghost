# M0-T31 — Act 3 Game Shell Integration

## Completion Status

Completed (Codex runs 001 + 002; 002 = a Return-to-Hub overlay canvas fix). The user ran the shell
builder and verified the flow ("完成"). Claude reviewed scope, the Build Settings diff, and run logs.

## Date

2026-06-23

## Summary

Act 3 is now launchable from the hub and returns to the hub, mirroring Act 2's M0-T19. `ShellSceneNames`
gained the Act 3 name/path; `GameShellPresenter` an `act3Button` + `StartAct3()`;
`ShellReturnToHubOverlay` now covers Act 1/2/3; `GameShellSceneBuilder` adds the Act 3 hub card and
registers shell + Act 1 + Act 2 + Act 3 in Build Settings; the hub Lily line acknowledges Act 3. Act 3's
puzzle and Acts 1/2 are unchanged. This completes shell wiring for all three prototype acts.

## Files Modified

- `Assets/Presentation/Shell/ShellSceneNames.cs`, `GameShellPresenter.cs`,
  `ShellReturnToHubOverlay.cs`, `ShellDialogueData.cs`, `Editor/GameShellSceneBuilder.cs`
- `Assets/Scenes/GameShellPrototype.unity` (regenerated via the builder)
- `ProjectSettings/EditorBuildSettings.asset` (approved exception: registers the Act 3 scene)
- `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`

## Files Created

- `Docs/codex_runs/M0-T31_001_act3_shell_integration.md`,
  `Docs/codex_runs/M0-T31_002_act3_return_overlay_canvas_fix.md`

## Claude Review Notes

- Scope clean (git status): only the 5 shell scripts + regenerated shell scene + `EditorBuildSettings` +
  2 docs + 2 run logs. No Act 3 logic/UI (`Assets/Scripts/Puzzles/DialogGraph/`,
  `Assets/Presentation/Act3DialogGraph/`), no Act 1/2, no other ProjectSettings.
- `EditorBuildSettings` diff adds only the Act 3 scene entry (enabled) — the approved exception.
- Run logs honest ("Not run" for Unity in-session); both retained.

## Human Verification Result

The user ran `Ghost > Build Game Shell Scene` and verified: hub shows Start Act 1/2/3, Start Act 3 loads
the Act 3 scene, Return to Hub works from Act 3, Acts 1/2 still launch/return. Reported "完成".

## Remaining Risks

- Manual-in-Editor verification only; shell scene is a builder artifact.
- Hub card layout (now three acts + placeholder) should be visually checked after regeneration.

## Next Task

M0-T26 — Narrative integration into Acts 1–3 (story premise, characters, Lily/Ghost lines, scene
transitions; data-driven, frontend-only). Needs the setting / protagonist / title decisions first;
Claude drafts the narrative content, then Codex implements the data + shell beats.
