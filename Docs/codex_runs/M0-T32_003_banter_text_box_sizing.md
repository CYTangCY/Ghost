# M0-T32 — Run 003 — banter text box sizing

## Task ID

M0-T32

## Run Number

003

## Date

2026-06-24

## Original Request / Codex Prompt Summary

Fix the ambient banter layout after human visual review: Act 1 and Act 3 banter text was being cut off because their text boxes were too small, while Act 2's banter box was too large. Keep the change scoped to the M0-T32 ambient banter presentation layer.

## Files Created

- `Docs/codex_runs/M0-T32_003_banter_text_box_sizing.md`

## Files Modified

- `Assets/Presentation/Banter/AmbientBanterHook.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `git diff --check -- Assets/Presentation/Banter/AmbientBanterHook.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `rg "Act1Validation|Act2Validation|Act3Guide|BanterPanelStyle|vertically cut|slimmer" Assets/Presentation/Banter/AmbientBanterHook.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md -n`
- `git diff --name-only -- ProjectSettings Packages Assets/Scenes Assets/Scripts/Puzzles Assets/Presentation/Act1IntentClassification Assets/Presentation/Act2EntityExtraction Assets/Presentation/Act3DialogGraph`
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `git diff --check` completed with no whitespace errors. Git reported line-ending warnings for edited files.
- `rg` confirmed the new per-act banter styles and checklist verification language.
- The prohibited-scope diff returned no files, confirming this run did not change ProjectSettings, Packages, scenes, puzzle logic, or Act 1/2/3 puzzle presenters.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- Human visual review found Act 1 and Act 3 banter text was clipped.
- Human visual review found Act 2's banter text box was too large.

## Fixes Applied

- Added per-act `BanterPanelStyle` sizing in `AmbientBanterHook`.
- Act 1 now uses a taller validation-row panel with more dialogue text height.
- Act 2 now uses a slimmer validation-row panel with smaller portrait/text/button sizing.
- Act 3 now uses a taller guide-panel card with a larger dialogue text area for wrapped lines.
- Updated walkthrough and test checklist to describe and verify the per-act sizing.

## What Was Intentionally Not Changed

- No puzzle logic, validators, sessions, or Act 1/2/3 puzzle mechanics were changed.
- No Act scene YAML was edited or regenerated.
- No ProjectSettings, Packages, Build Settings, `.meta` files, backend, LLM, save/load, player-choice branching, or final art were changed.
- Banter dialogue data was not changed in this run.

## Remaining Risks

- Unity Play Mode visual verification is still required to confirm Act 1 and Act 3 text no longer clips and Act 2 is not too small after slimming.
- Long lines can still depend on exact resolution, font metrics, and Unity layout timing.

## Next Recommended Step

Open Unity, let scripts compile, enter Act 1/2/3 through the shell, and verify Act 1/3 banter text is not cut off, Act 2 banter is visibly smaller but readable, puzzle controls remain usable, and there are no Console errors.
