# M0-T26 — Run 002 — shell hub layout fix

## Task ID
M0-T26

## Run Number
002

## Date
2026-06-23

## Original Request / Codex Prompt Summary
The user reported that when entering the act-selection hub from the home/title flow, Lily's dialogue box extends beyond the viewport. Fix the Game Shell frontend layout without changing puzzle logic, validators, Act 1/2/3 mechanics, backend, LLM, save/load, ProjectSettings, Packages, `.meta` files, scene YAML, or Build Settings.

## Files Created
- `Docs/codex_runs/M0-T26_002_shell_hub_layout_fix.md`

## Files Modified
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run
- `git diff -- Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `git diff --check -- Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `git status --short -- Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md Assets/Scripts/Puzzles Assets/Presentation/Act1IntentClassification Assets/Presentation/Act2EntityExtraction Assets/Presentation/Act3DialogGraph ProjectSettings Packages Assets/Scenes`

## Test / Check Result
- Diff confirmed the hub act cards now render inside a horizontal `Act Card Row` instead of stacking vertically, reducing hub height so the dialogue frame should remain in the viewport.
- `git diff --check` completed without whitespace errors; Git reported line-ending normalization warnings only.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.
- Scene generation: Not run — Unity Editor menu builder was not executed in this Codex session.

## Errors Encountered
- User-reported UI layout overflow: Lily dialogue box exceeded the viewport after entering the act-selection hub.
- No local text/diff check errors encountered.

## Fixes Applied
- Added `CreateActCardRow(...)` to the shell builder.
- Moved the Act 1, Act 2, and Act 3 hub cards into one horizontal row.
- Gave the row and cards stable heights and flexible widths so hub content is shorter while preserving readable card text.
- Updated walkthrough and checklist documentation to verify the dialogue frame stays inside the viewport.

## What Was Intentionally Not Changed
- Act 1, Act 2, or Act 3 puzzle logic, validators, sessions, and puzzle UI scripts.
- Backend, database, LLM, save/load, scoring, analytics, or persistence.
- Existing `.unity` scene YAML.
- `ProjectSettings`, `Packages`, Build Settings asset, and `.meta` files.
- Act structure, future Acts, final art, or full visual-novel dialogue.

## Remaining Risks
- The fix needs human Unity Play Mode verification because Codex did not run the Unity Editor.
- The user must run `Ghost > Build Game Shell Scene` for the generated shell scene to receive the horizontal hub-card layout.
- Existing scene files in the working tree may contain prior Unity-generated changes; this Codex run did not hand-edit scene YAML.

## Next Recommended Step
Run `Ghost > Build Game Shell Scene`, enter Play Mode, go from title to name entry to the act hub, and confirm the three act cards sit in one row and the Lily dialogue frame remains fully inside the viewport.
