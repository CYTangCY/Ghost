# M0-T36 — Run 002 — Act 1 teaching visual clarity

## Task ID

M0-T36

## Run Number

002

## Date

2026-06-28

## Original Request / Codex Prompt Summary

The user reviewed the Act 1 screenshot and said it looked almost the same. Improve visual clarity so
the M0-T36 teaching layer reads as an obvious in-game teaching beat, while keeping the existing
intent-classification mechanic and deterministic puzzle logic unchanged.

## Files Created

- `Docs/codex_runs/M0-T36_002_act1_teaching_visual_clarity.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
- `Docs/LEARNING_CONTENT.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `git diff --name-only -- Assets/Scripts/Puzzles/IntentClassification`
- `git diff --name-only -- Assets/Presentation/Act1IntentClassification/Editor`
- PowerShell non-ASCII scan for the two changed Act 1 C# files
- `git diff --check -- Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs Docs/LEARNING_CONTENT.md Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `rg -n 'IntentTitleText\", intentId|All messages are grouped by intent|Group messages by speaker intent, not exact wording|Lily: Um\\.\\.\\. Ghost is chasing exact words' Assets/Presentation/Act1IntentClassification Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- Unity Play Mode: Not run — Unity Editor is unavailable in this Codex session.

## Test / Check Result

- Puzzle logic diff check: no files under `Assets/Scripts/Puzzles/IntentClassification` changed.
- Act 1 editor scene-builder diff check: no files under `Assets/Presentation/Act1IntentClassification/Editor` changed.
- Non-ASCII scan: both changed Act 1 C# files reported `ASCII only`.
- `git diff --check`: no whitespace errors reported; Git printed line-ending normalization warnings only.
- Old-copy search: no matches for raw-id title assignment or replaced old Act 1 feedback/instruction wording.
- Unity Play Mode: Not run — Unity Editor is unavailable in this Codex session.

## Errors Encountered

- One initial `rg` old-copy search command failed because of a PowerShell quote terminator mistake. The check was rerun with single-quoted pattern syntax and returned no matches.
- Unity Play Mode could not be run from this environment.

## Fixes Applied

- Replaced the subtle subtitle-only teaching text with a runtime-created `Lily Intent Teaching Panel`
  using a warm background and outline.
- Kept the subtitle as compact action guidance.
- Changed intent group display titles from raw intent ids to player-facing purpose labels while leaving
  underlying intent ids and validation unchanged.
- Restyled the validation feedback strip by feedback state, including a green success-teaching state.
- Shortened correct success feedback and labelled the training-examples line explicitly.
- Updated learning-content mapping, code walkthrough, and Unity test checklist to require visual
  clarity checks.

## What Was Intentionally Not Changed

- `IntentClassificationValidator`
- `IntentClassificationSession`
- `Act1IntentClassificationSampleData`
- Act 1 card wording, answer key, and validation rules
- Act 1 scene builder
- Act 2 / Act 3 code
- Fundamentals files
- Backend code
- ProjectSettings, Packages, Build Settings, `.meta` files, and scene YAML
- Existing dirty scene changes in `Assets/Scenes/Act1IntentClassificationPrototype.unity` and
  `Assets/Scenes/GameShellPrototype.unity`

## Remaining Risks

- Human Unity Play Mode verification is still required to confirm the teaching panel is visible at the
  target resolution and does not crowd the puzzle body.
- The ambient banter panel should be checked with the green success-teaching state to ensure feedback
  remains readable.

## Next Recommended Step

Run the updated M0-T36 checklist in `Docs/UNITY_TEST_CHECKLIST.md`, especially the visual checks for the
Lily intent-note panel, purpose labels, and green success-teaching validation state.
