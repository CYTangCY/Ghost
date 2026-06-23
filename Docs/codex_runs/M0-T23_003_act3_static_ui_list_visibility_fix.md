# M0-T23 — Run 003 — act3 static ui list visibility fix

## Task ID

M0-T23

## Run Number

003

## Date

2026-06-23

## Original Request / Prompt Summary

Fix the display-only Act 3 node-graph prototype after the previous scroll-view layout caused the Palette and Goal/Test panel contents to disappear. Keep the task within M0-T23 display-only scope: no node placement, no edge drawing, no working validation, no Build Settings registration, and no changes to Act 3 pure logic.

## Files Created

- `Docs/codex_runs/M0-T23_003_act3_static_ui_list_visibility_fix.md`

## Files Modified

- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
- `Assets/Presentation/Act3DialogGraph/Editor/Act3DialogGraphPrototypeSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Read required project docs and confirmed active task remains M0-T23.
- `Select-String` check that the builder uses compact `CreateListRoot(...)` list roots and no longer uses `ScrollRect`, `Mask`, `ContentSizeFitter`, or `CreateScrollableListRoot`.
- `rg "DialogGraphValidator|ValidateCurrentState|AddNode|AddTransition|RemoveNode|RemoveTransition|DialogGraphSession\.Validate" Assets\Presentation\Act3DialogGraph`
- `rg "EditorBuildSettings|RegisterBuildSettings|BuildSettings" Assets\Presentation\Act3DialogGraph`
- `Select-String` check that current M0-T23 docs no longer require scroll-view verification for the Act 3 lists.
- `dotnet build Ghost.Presentation.csproj --no-restore`
- `dotnet restore Ghost.Presentation.Act3.Editor.csproj`
- `dotnet build Ghost.Presentation.Act3.Editor.csproj --no-restore`
- Scene generation: Not run — Unity Editor scene builder was not executed in this Codex session.
- Unity Editor Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity Editor tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- The presenter now uses compact row heights and tighter item padding/section labels for the fixed M0-T23 sample content.
- The builder now uses plain compact vertical list roots again and compact template label sizes, avoiding the saved-scene `ScrollRect`/content sizing issue that hid list contents.
- No validation/session-mutation calls were found in the Act 3 presentation folder.
- No Build Settings registration calls were found in the Act 3 presentation folder.
- Current M0-T23 docs no longer ask for scroll-view verification for the Act 3 lists.
- `Ghost.Presentation.csproj` build passed with 5 obsolete API warnings for existing `FindFirstObjectByType` usage, including the Act 3 presenter's `EnsureEventSystem` pattern.
- `Ghost.Presentation.Act3.Editor.csproj` restore passed after rerunning with approved elevated access because sandboxed restore could not read the user NuGet config.
- `Ghost.Presentation.Act3.Editor.csproj` build passed with 0 warnings and 0 errors.
- Not run — Unity Editor scene builder was not executed in this Codex session.
- Not run — Unity Editor Play Mode was not executed in this Codex session.
- Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- Manual Unity verification showed the Palette and Goal/Test list contents disappeared after the Run 002 scroll-view change.
- `dotnet build Ghost.Presentation.Act3.Editor.csproj --no-restore` initially failed because the generated editor project did not yet have a `project.assets.json`.
- Sandboxed `dotnet restore Ghost.Presentation.Act3.Editor.csproj` failed because it could not read the user NuGet config.

## Fixes Applied

- Replaced the Run 002 scroll-view list roots with compact plain vertical list roots.
- Reduced palette/test row preferred heights, item padding, section-label height, and template label sizes so the fixed sample content fits in the available panels.
- Updated documentation and checklist from the temporary scroll-view expectation to compact-list visibility checks.
- Restored the Act 3 editor generated project with approved elevated access and rebuilt it successfully.

## What Was Intentionally Not Changed

- No `.unity` scene YAML was hand-edited.
- No Build Settings registration was added.
- No node placement, edge drawing, working validation, scoring, save/load, animation, backend, LLM, dialogue, final art, or Act 4-6 node types were added.
- No Act 3 pure logic files from M0-T21/M0-T22 were edited.
- No Act 1, Act 2, Game Shell, ProjectSettings, Packages, existing asmdef, existing scenes, or `.meta` files were intentionally edited.

## Remaining Risks

- The generated Act 3 scene will remain stale until the user reruns `Ghost > Build Act 3 Dialog Graph Prototype Scene`.
- Unity Editor Play Mode verification was not run in this Codex session, so the fixed visual layout still needs human Unity confirmation.
- The user's working tree currently includes Unity-generated scene/meta and ProjectSettings changes outside Codex's hand edits; those should be reviewed before commit.

## Next Recommended Step

Open Unity, rerun `Ghost > Build Act 3 Dialog Graph Prototype Scene`, open `Assets/Scenes/Act3DialogGraphPrototype.unity`, and confirm Palette and Goal/Test contents are visible, stay inside their panels, and the scene remains display-only.
