# M0-T30 — Run 007 — act3 stable layout and trash drop

## Task ID

M0-T30

## Run Number

007

## Date

2026-06-23

## Original Request / Codex Prompt Summary

Fix two Act 3 UX regressions reported from Unity verification: the Guide column width kept changing between iterations, and the trash drop behavior had a mismatch where highlighted trash did not always remove the dragged card.

## Files Created

- `Docs/codex_runs/M0-T30_007_act3_stable_layout_and_trash_drop.md`

## Files Modified

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
- `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `git diff --check -- Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`

## Test / Check Result

- `git diff --check`: Passed with only Git LF/CRLF working-copy warnings.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.
- Scene generation: Not run — Unity Editor menu builder was not executed in this Codex session.

## Errors Encountered

- The root cause of the Guide width drift was the generated body `HorizontalLayoutGroup.childForceExpandWidth = true`, which made fixed-width columns participate in spare-width expansion.
- Trash hover/drop behavior used equivalent-looking checks, but the drop path did not use the cached highlighted state, so the user's visible highlight could disagree with the final drop result.

## Fixes Applied

- Set the Act 3 scene builder body layout `childForceExpandWidth` to `false`, so only the graph column's `flexibleWidth = 1` receives spare width.
- Added runtime presenter layout repair via `ConfigureGeneratedColumnLayout()`, which reapplies the palette, graph, and guide column layout values and disables body forced width expansion for stale generated scenes.
- Updated node drag completion to delete when `isDraggingNodeOverTrash` is true, guaranteeing that a highlighted trash zone accepts the card on drop.
- Updated CODE_WALKTHROUGH and UNITY_TEST_CHECKLIST to document the forced-width-expansion cause and the highlight-equals-droppable trash rule.

## What Was Intentionally Not Changed

- Act 3 pure logic/session files under `Assets/Scripts/Puzzles/DialogGraph/`.
- Act 1, Act 2, Game Shell, backend, LLM, save/load, scoring persistence, Build Settings, ProjectSettings, Packages, `.meta` files, and `Docs/CURRENT_TASK.md`.
- Existing generated scenes were not hand-edited; the user should rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene` if the saved scene is stale.

## Remaining Risks

- Unity Play Mode was not run in this session, so the exact visual result and trash hover/drop behavior still require human Editor verification.
- The working tree contains pre-existing unrelated scene, ProjectSettings, and documentation changes that should be reviewed separately before staging.

## Next Recommended Step

Rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene`, open `Assets/Scenes/Act3DialogGraphPrototype.unity`, enter Play Mode, confirm palette/guide widths remain stable after rerender/rebuild, and confirm every highlighted `X drop card` hover removes the node on drop.
