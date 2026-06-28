# M0-T36 — Run 001 — Act 1 intent teaching

## Task ID

M0-T36

## Run Number

001

## Date

2026-06-27

## Original Request / Codex Prompt Summary

Strengthen Act 1 Intent Classification so it teaches intent as speaker purpose, varied wording as one
shared intent, and grouped card wordings as training examples. Keep the existing Act 1 mechanic and
deterministic validator/session/sample data unchanged. Update documentation and provide human Unity
Play Mode verification steps.

## Files Created

- `Docs/codex_runs/M0-T36_001_act1_intent_teaching.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
- `Docs/LEARNING_CONTENT.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `git diff --name-only -- Assets/Scripts/Puzzles/IntentClassification`
- PowerShell non-ASCII scan for the two changed Act 1 C# files
- `git diff --check -- Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs Docs/LEARNING_CONTENT.md Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `rg -n "All messages are grouped by intent|Group messages by speaker intent, not exact wording|Messages asking Ghost" Assets/Presentation/Act1IntentClassification Docs/LEARNING_CONTENT.md Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- Unity Play Mode: Not run — Unity Editor is unavailable in this Codex session.

## Test / Check Result

- Puzzle logic diff check: no files under `Assets/Scripts/Puzzles/IntentClassification` changed.
- Non-ASCII scan: both changed Act 1 C# files reported `ASCII only`.
- `git diff --check`: no whitespace errors reported; Git printed line-ending normalization warnings only.
- Old-copy search: no matches for the replaced old Act 1 instruction / success / group hint wording.
- Unity Play Mode: Not run — Unity Editor is unavailable in this Codex session.

## Errors Encountered

None during the code or documentation edit. Unity Play Mode could not be run from this environment.

## Fixes Applied

- Added Lily-attributed Act 1 instruction text explaining intent as purpose rather than exact wording.
- Rephrased intent group hints as visitor purposes.
- Expanded correct validation feedback to show a happy Ghost reaction, explain shared purpose, summarize training examples from the actual card data, and add one Lily planning-link line.
- Increased the existing validation strip height so the correct teaching feedback has room to display.
- Updated learning-content mapping, code walkthrough, and Unity test checklist.

## What Was Intentionally Not Changed

- `IntentClassificationValidator`
- `IntentClassificationSession`
- `Act1IntentClassificationSampleData`
- Act 1 card wording, answer key, and validation rules
- Act 2 / Act 3 code
- Fundamentals files
- Backend code
- ProjectSettings, Packages, Build Settings, `.meta` files, and scene YAML
- The pre-existing dirty `Assets/Scenes/GameShellPrototype.unity` worktree change

## Remaining Risks

- Unity layout and Console status still need human Editor / Play Mode verification.
- The taller Act 1 validation strip should be checked with the existing ambient banter panel to confirm the text remains readable and does not overlap puzzle controls.

## Next Recommended Step

Run the M0-T36 checklist in `Docs/UNITY_TEST_CHECKLIST.md`, then have Claude review the code/docs, record human verification, and close/archive the task if accepted.
