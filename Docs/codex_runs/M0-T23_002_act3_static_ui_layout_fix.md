# M0-T23 — Run 002 — act3 static ui layout fix

## Task ID

M0-T23

## Run Number

002

## Date

2026-06-23

## Original Request / Prompt Summary

Fix the display-only Act 3 node-graph prototype UI after manual Unity verification showed the bottom of the Palette content extending past its panel. Keep the M0-T23 scope unchanged: display-only UI, no node placement, no connections, no working validation, no Build Settings registration, and no changes to Act 3 pure logic.

## Files Created

- `Docs/codex_runs/M0-T23_002_act3_static_ui_layout_fix.md`

## Files Modified

- `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read required project docs and confirmed active task remains M0-T23.
- `Select-String` check for the new scroll-list structure in `Act3DialogGraphPrototypeSceneBuilder.cs`.
- `rg "DialogGraphValidator|ValidateCurrentState|AddNode|AddTransition|RemoveNode|RemoveTransition|DialogGraphSession\.Validate" Assets\Presentation\Act3DialogGraph`
- `rg "EditorBuildSettings|RegisterBuildSettings|BuildSettings" Assets\Presentation\Act3DialogGraph`
- `Select-String` trailing-whitespace check over the Act 3 presentation files and updated docs.
- Scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Editor Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity Editor tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- The builder now creates masked scrollable list regions for the Palette and Goal/Test panels.
- No validation/session-mutation calls were found in the Act 3 presentation folder.
- No Build Settings registration calls were found in the Act 3 presentation folder.
- The checked files had no trailing-whitespace matches.
- Not run — Unity Editor scene builder was not executed in this Codex session.
- Not run — Unity Editor Play Mode was not executed in this Codex session.
- Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- Manual Unity verification showed Palette content clipping past the bottom of its panel in a shorter Game view.

## Fixes Applied

- Replaced the builder's plain vertical list roots with `ScrollRect` + masked viewport + content roots for Palette and Goal/Test lists.
- Kept `Act3DialogGraphStaticPresenter` unchanged; it still renders display-only rows into the configured content roots.
- Updated documentation and test checklist to include the no-clipping / scroll-inside-panel verification.

## What Was Intentionally Not Changed

- No `.unity` scene YAML was hand-edited.
- No Build Settings registration was added.
- No node placement, edge drawing, working validation, scoring, save/load, animation, backend, LLM, dialogue, final art, or Act 4-6 node types were added.
- No Act 3 pure logic files from M0-T21/M0-T22 were edited.
- No Act 1, Act 2, Game Shell, ProjectSettings, Packages, existing asmdef, existing scenes, or `.meta` files were intentionally edited.

## Remaining Risks

- The existing generated Act 3 scene will remain stale until the user reruns `Ghost > Build Act 3 Dialog Graph Prototype Scene`.
- Unity Editor import/compile and Play Mode verification were not run in this Codex session.
- Generated `.csproj` files may still be stale until Unity imports the new Act 3 editor asmdef.

## Next Recommended Step

Open Unity, rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene`, open `Assets/Scenes/Act3DialogGraphPrototype.unity`, and confirm the Palette and Goal/Test content stay inside their panels, scrolling within the panel when the Game view is short.
