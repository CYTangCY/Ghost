# M0-T26 — Run 001 — narrative integration

## Task ID
M0-T26

## Run Number
001

## Date
2026-06-23

## Original Request / Codex Prompt Summary
Weave the Acts 1-3 narrative into the Game Shell as frontend-only, static, data-driven text. Add in-memory narrative state, player-name entry, per-act intro/debrief beats, a post-Act-3 Ghost closing line, and a portrait placeholder frame. Do not change puzzle logic, validators, Act 1/2/3 puzzle mechanics, backend, LLM, save/load, ProjectSettings, Packages, `.meta` files, scene YAML, or Build Settings.

## Files Created
- `Assets/Presentation/Shell/GhostNarrativeState.cs`
- `Docs/codex_runs/M0-T26_001_narrative_integration.md`

## Files Modified
- `Assets/Presentation/Shell/ShellDialogueData.cs`
- `Assets/Presentation/Shell/LilyDialogueFrame.cs`
- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run
- `rg "Configure\\(|GhostNarrativeState|NameEntry|narrativeContinue|GetBeat|SetPendingDebrief|CreateOverlayCanvas|CreateInputField|CreateLilyDialogueFrame" Assets\\Presentation\\Shell -n`
- `rg "GhostNarrativeState|NameEntryScreenId|IntroPhaseId|DebriefPhaseId|ClosingPhaseId|SetPendingDebriefAct|PlayPendingDebrief|LilyDialogueFrame|CreateInputField" Assets\\Presentation\\Shell Docs\\CODE_WALKTHROUGH.md Docs\\UNITY_TEST_CHECKLIST.md -n`
- `git diff --check -- Assets/Presentation/Shell/GhostNarrativeState.cs Assets/Presentation/Shell/ShellDialogueData.cs Assets/Presentation/Shell/LilyDialogueFrame.cs Assets/Presentation/Shell/GameShellPresenter.cs Assets/Presentation/Shell/ShellReturnToHubOverlay.cs Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `git diff --name-only -- Assets/Scripts/Puzzles Assets/Presentation/Act1IntentClassification Assets/Presentation/Act2EntityExtraction Assets/Presentation/Act3DialogGraph ProjectSettings Packages Assets/Scenes`

## Test / Check Result
- Search confirmed the new narrative state, name-entry flow, act beats, portrait frame wiring, pending-debrief handling, and builder references are present in Shell-only files and docs.
- `git diff --check` completed without whitespace errors; Git reported line-ending normalization warnings only.
- Scoped diff-name check returned no puzzle logic/UI, ProjectSettings, Packages, or scene files changed by this run.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.
- Scene generation: Not run — Unity Editor menu builder was not executed in this Codex session.

## Errors Encountered
- No code errors encountered during local text/diff checks.

## Fixes Applied
- Added `GhostNarrativeState` for in-memory player name, completed act ids, and pending return/debrief act.
- Extended `ShellDialogueData` with name-entry, hub, per-act intro/debrief, and Act 3 Ghost closing beats from `Docs/NARRATIVE.md`.
- Extended `LilyDialogueFrame` with a speaker portrait Image slot, empty Lily/Ghost sprite fields, placeholder label behavior, and `{playerName}` substitution.
- Extended `GameShellPresenter` with title -> name entry -> hub flow, act intro continue flow, pending debrief playback, and Act 3 closing-line queue.
- Extended `ShellReturnToHubOverlay` so returning from Act 1/2/3 records the active act before loading the shell.
- Updated `GameShellSceneBuilder` to generate the name-entry UI, narrative continue button, and portrait-capable dialogue frame.
- Updated walkthrough and Unity checklist documentation for M0-T26.

## What Was Intentionally Not Changed
- Act 1, Act 2, or Act 3 puzzle logic, validators, sessions, and puzzle UI scripts.
- Backend, database, LLM, save/load, scoring, analytics, or persistence.
- Existing `.unity` scene YAML.
- `ProjectSettings`, `Packages`, Build Settings asset, and `.meta` files.
- Act structure, future Acts, final art, or full visual-novel dialogue.

## Remaining Risks
- Unity compilation and Play Mode behavior still require human Editor verification.
- The user must run `Ghost > Build Game Shell Scene` to regenerate the shell scene with the new name-entry and portrait UI.
- Dialogue layout and portrait placeholder sizing should be visually checked after scene generation.
- The narrative state is intentionally static and in-memory only; it resets on app restart and is not save/load.

## Next Recommended Step
Run `Ghost > Build Game Shell Scene` in Unity, enter Play Mode in the regenerated shell scene, and verify name entry, hub greeting, per-act intro/debrief flow, portrait placeholder speaker switching, Act 3 Ghost closing line, unchanged puzzle mechanics, and no Console errors.
