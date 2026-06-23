# M0-T31 — Run 002 — Act 3 return overlay canvas fix

## Task ID
M0-T31

## Run Number
002

## Date
2026-06-23

## Original Request / Codex Prompt Summary
The user reported that Act 3 has no button to return to the hub. Investigate and fix the shell return overlay without modifying Act 3 puzzle logic/UI or Act 1/Act 2 mechanics.

## Files Created
- `Docs/codex_runs/M0-T31_002_act3_return_overlay_canvas_fix.md`

## Files Modified
- `Assets/Presentation/Shell/ShellReturnToHubOverlay.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run
- `rg "ShellReturnToHubOverlay|Return to Hub|ShouldShowOverlay|RuntimeInitializeOnLoadMethod|sceneLoaded" Assets Docs -n`
- `rg "Destroy|Clear|RenderSampleData|Start\\(|Canvas|SetSibling|transform\\.SetParent|GetComponentsInChildren" Assets\\Presentation\\Act3DialogGraph -n`
- `git diff --check -- Assets/Presentation/Shell/ShellReturnToHubOverlay.cs Docs/CODE_WALKTHROUGH.md Docs/UNITY_TEST_CHECKLIST.md`
- `rg "OverlayCanvasName|CreateOverlayCanvas|OverlaySortingOrder|Shell Return To Hub Overlay Canvas" Assets\\Presentation\\Shell\\ShellReturnToHubOverlay.cs Docs\\CODE_WALKTHROUGH.md Docs\\UNITY_TEST_CHECKLIST.md`

## Test / Check Result
- Search confirmed the shell overlay hook already included Act 3 scene-name handling.
- Review showed the old overlay was parented under the first scene Canvas, which could allow Act prototype UI canvases or full-screen UI hierarchy to cover the return button.
- `git diff --check` completed without whitespace errors; Git reported line-ending normalization warnings only.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.
- Scene generation: Not run — Unity Editor menu builder was not executed in this Codex session.

## Errors Encountered
- Act 3 return button was reported absent by the user.
- No code errors were encountered during the local text/diff checks.

## Fixes Applied
- Changed `ShellReturnToHubOverlay` to create a dedicated `Shell Return To Hub Overlay Canvas` instead of reusing the first Canvas found in the active act scene.
- Set the overlay Canvas to `overrideSorting = true` with a high sorting order so Act 3 UI layers cannot cover the return button.
- Kept the button name `Shell Return To Hub Overlay` for duplicate detection.
- Updated walkthrough and checklist notes to verify the dedicated overlay Canvas if the Act 3 return button is absent.

## What Was Intentionally Not Changed
- Act 3 puzzle logic/session/UI files under `Assets/Scripts/Puzzles/DialogGraph/` and `Assets/Presentation/Act3DialogGraph/`.
- Act 1 and Act 2 puzzle mechanics.
- Existing `.unity` scene YAML.
- `ProjectSettings`, `Packages`, Build Settings asset, and `.meta` files.
- Backend, LLM, save/load, full visual-novel dialogue, and final art.

## Remaining Risks
- The fix needs human Unity Play Mode verification because Codex did not run the Unity Editor.
- If Unity script compilation has not refreshed, the user may need to let Unity recompile before retesting.
- Build Settings still need to include the shell scene for the button click to load the hub successfully.

## Next Recommended Step
Let Unity compile, run from the shell into Act 3, and verify a top-right `Return to Hub` button appears above the Act 3 UI and loads `GameShellPrototype` when clicked.
