# M0-T26 — Narrative Integration into Acts 1–3

## Completion Status

Completed (Codex runs 001 narrative + 002 hub layout fix). The user verified the full flow ("功能完成").
Claude reviewed scope and the run logs.

## Date

2026-06-24 (closure; runs dated 2026-06-23)

## Summary

Wove the narrative frame into the Game Shell (frontend, static, data-driven). `GhostNarrativeState`
(in-memory player name + acts done, survives scene loads), a name-entry step at Start, act-aware
`ShellDialogueData` (intro/debrief per act + speaker + `{playerName}`), a `LilyDialogueFrame` portrait
frame (empty placeholder Sprite slots), the shell narrative flow (title → name → hub greeting → act
intro → puzzle → debrief → Act 3 Ghost closing), and the return overlay setting the pending debrief.
Run 002 placed the three hub act cards in one row so Lily's dialogue box stays inside the viewport.
Puzzle logic unchanged.

## Files Created

- `Assets/Presentation/Shell/GhostNarrativeState.cs`
- `Docs/codex_runs/M0-T26_001_narrative_integration.md`, `M0-T26_002_shell_hub_layout_fix.md`

## Files Modified

- `Assets/Presentation/Shell/ShellDialogueData.cs`, `LilyDialogueFrame.cs`, `GameShellPresenter.cs`,
  `ShellReturnToHubOverlay.cs`, `Editor/GameShellSceneBuilder.cs`
- `Assets/Scenes/GameShellPrototype.unity` (regenerated)
- `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`, `Docs/NARRATIVE.md`

## Claude Review Notes

- Scope clean (git status): Shell/frontend only; no puzzle logic/validators/sessions, no ProjectSettings
  /Packages/Build Settings, no `.meta` beyond the Unity-generated one for the new script.
- `Assets/Scenes/Act2EntityExtractionPrototype.unity` showing modified is an **unrelated Editor
  side-effect** (not intentionally edited; run log confirms no scene YAML edits) — left unstaged.
- Run logs honest ("Not run" for Unity in-session); both retained.

## Human Verification Result

The user verified: title → name entry → hub greeting by name → per-act Lily intro + debrief → Act 3
Ghost closing line; three hub cards in one row; Lily dialogue box inside the viewport; Acts 1/2/3
puzzles unchanged; no Console errors. Reported "功能完成".

## Remaining Risks

- Manual-in-Editor verification only; shell scene is a builder artifact.
- Character portrait frames are empty placeholders (art later).

## Next Task

M0-T32 — In-act ambient Ghost+Lily banter: fixed per-act dialogue loops in each act scene's spare space
(data-driven, fun, plenty), Lily warming up across acts + a recurring nerdy-joke-then-embarrassed beat,
story-consistent Ghost lines; structured for future player choices + LLM. See `Docs/NARRATIVE.md`.
