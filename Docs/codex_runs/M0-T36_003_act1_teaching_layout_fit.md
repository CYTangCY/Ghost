# M0-T36 — Run 003 — Act 1 teaching layout fit

## Task ID

M0-T36

## Run Number

003

## Date

2026-06-28

## Original Request / Codex Prompt Summary

The user reviewed the updated Act 1 screenshot and reported that the teaching panel pushed the UI
below the game window, hiding lower content. Fix the Act 1 teaching layout so the visible teaching
layer remains compact and the bottom validation / banter area stays inside a 1080p Play Mode view.

## Files Created

- `Docs/codex_runs/M0-T36_003_act1_teaching_layout_fit.md`

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Docs/LEARNING_CONTENT.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `git diff --name-only -- Assets/Scripts/Puzzles/IntentClassification Assets/Presentation/Act1IntentClassification/Editor`
- PowerShell non-ASCII scan for the two changed Act 1 C# files
- `git diff --check -- Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs Docs/LEARNING_CONTENT.md Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `rg -n "CardPreferredHeight = 72f|GroupPreferredHeight = 200f|TeachingPanelPreferredHeight = 112f|ValidationControlsPreferredHeight = 118f|layout\\.spacing = 24f|padding = new RectOffset\\(48" Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `rg -n "CardPreferredHeight = 58f|GroupPreferredHeight = 166f|TeachingPanelPreferredHeight = 76f|ConfigureRootLayout|bottom validation / banter area" Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs Docs/UNITY_TEST_CHECKLIST.md`
- Unity Play Mode: Not run — Unity Editor is unavailable in this Codex session.

## Test / Check Result

- Puzzle logic / Act 1 editor-builder diff check: no files reported.
- Non-ASCII scan: both changed Act 1 C# files reported `ASCII only`.
- `git diff --check`: no whitespace errors reported; Git printed line-ending normalization warnings only.
- Old high-layout constant search: no matches, meaning the previous oversized constants were removed from the presenter.
- Compact-layout search: found the new compact card/group/teaching-panel constants and checklist viewport check.
- Unity Play Mode: Not run — Unity Editor is unavailable in this Codex session.

## Errors Encountered

Unity Play Mode could not be run from this environment, so viewport fit still needs human verification.

## Fixes Applied

- Reduced card, group, assignment viewport, assigned-row, teaching-panel, title, subtitle, and column-label heights.
- Tightened root layout padding/spacing and column panel/list spacing at runtime, so stale generated scenes also receive the compact layout.
- Kept the validation area large enough for success feedback and ambient banter while freeing space from the top and card/group sections.
- Tightened intent group title/hint text sizes and heights so purpose labels do not overrun the smaller groups.
- Updated documentation and Play Mode checklist with explicit 1920x1080 viewport-fit checks.

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

- Human Unity Play Mode verification is still required to confirm the layout fits at the user's exact
  Game view resolution and scale.
- If a lower-than-1080 viewport is used, Act 1 may still need a scrollable body or an even more compact
  variant in a later task.

## Next Recommended Step

Run Act 1 in Play Mode at 1920x1080 and confirm the Lily intent note, all nine cards, all three intent
groups, validation controls, and ambient banter are visible without cropping.
