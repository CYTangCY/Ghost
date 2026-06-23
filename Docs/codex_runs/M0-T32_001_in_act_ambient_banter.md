# M0-T32 — Run 001 — in-act ambient banter

## Task ID

M0-T32

## Run Number

001

## Date

2026-06-24

## Original Request / Codex Prompt Summary

Implement only M0-T32: add a non-blocking, runtime-spawned ambient Ghost and Lily banter area to each Act 1/2/3 scene using static, data-driven per-act dialogue loops seeded from `Docs/NARRATIVE.md`. Reuse the shell scene-load hook pattern and `GhostNarrativeState` player-name token. Do not change puzzle logic, validators, sessions, puzzle mechanics, scenes, ProjectSettings, Build Settings, backend, LLM, or save/load.

## Files Created

- `Assets/Presentation/Banter/BanterData.cs`
- `Assets/Presentation/Banter/AmbientBanterPanel.cs`
- `Assets/Presentation/Banter/AmbientBanterHook.cs`
- `Docs/codex_runs/M0-T32_001_in_act_ambient_banter.md`

## Files Modified

- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `git diff --check -- Assets/Presentation/Banter/BanterData.cs Assets/Presentation/Banter/AmbientBanterPanel.cs Assets/Presentation/Banter/AmbientBanterHook.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `rg "lily_joke_backpedal|lily_nerdy_joke_backpedal|AmbientBanterBeat|M0-T32" Assets/Presentation/Banter Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md -n`
- `git diff --name-only -- Assets/Scripts/Puzzles Assets/Presentation/Act1IntentClassification Assets/Presentation/Act2EntityExtraction Assets/Presentation/Act3DialogGraph ProjectSettings Packages Assets/Scenes`
- `git status --short -- Assets/Presentation/Banter Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md Docs/codex_runs`
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `git diff --check` completed with no whitespace errors. Git reported line-ending warnings for the edited docs.
- `rg` confirmed the new banter data/scripts and M0-T32 docs entries exist.
- The scoped prohibited-area diff showed existing dirty scene files (`Assets/Scenes/Act1IntentClassificationPrototype.unity`, `Assets/Scenes/Act2EntityExtractionPrototype.unity`, `Assets/Scenes/Act3DialogGraphPrototype.unity`). These were not edited by this run.
- Scoped status shows the new `Assets/Presentation/Banter/` folder and the two documentation files changed for this task.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- No implementation errors were encountered.
- The working tree already had dirty Act 1/2/3 scene files visible during scoped checks; they were left untouched.

## Fixes Applied

- Added `BanterData` with per-act static ambient beats for Act 1, Act 2, and Act 3, including player-name substitution text and future-choice placeholder data shape.
- Added `AmbientBanterPanel`, a small UGUI panel that shows speaker, dialogue text, portrait placeholder, timer cycling, looping, and an optional Next button.
- Added `AmbientBanterHook`, a runtime scene-load hook that detects Act 1/2/3 scenes, creates an overlay canvas/event system if needed, and spawns the banter panel without editing scene YAML.
- Split the scripted joke/backpedal moments into separate beats to preserve the `Docs/NARRATIVE.md` timing.
- Updated code walkthrough and Unity test checklist for M0-T32.

## What Was Intentionally Not Changed

- No puzzle logic, validators, sessions, or Act 1/2/3 puzzle mechanics were changed.
- No Act 1/2/3 scene YAML was edited or regenerated.
- No ProjectSettings, Packages, Build Settings, `.meta` files, backend, LLM, save/load, player-choice branching, or final art were changed.
- No new asmdef was added; the scripts use the existing `Ghost.Presentation` assembly.

## Remaining Risks

- The runtime panel position and size need human Play Mode verification in each act to confirm it occupies spare space and does not cover important puzzle controls.
- Because Unity Editor Play Mode was not run in this session, compile and runtime behavior still require Unity verification.
- The optional Next button intentionally consumes input in its small area; the rest of the panel is configured to avoid blocking puzzle input.

## Next Recommended Step

Open Unity, let scripts compile, enter each Act 1/2/3 scene through the shell, and verify the ambient banter panel appears, cycles/loops, substitutes the player name, keeps puzzle input playable, and produces no Console errors.
