# M0-T03 — Unity Repository Inventory

## Completion Status

Completed. Read-only inventory; no files were modified.

## Date

2026-06-17

## Summary

Inventoried the actual Unity repository before any implementation. The repo is a clean Unity 6
Universal 2D / URP project with the new Input System active, SampleScene only, no existing
scripts, no game scenes, and the Unity Test Framework installed. The safest first implementation
task is a scene-free Act 1 intent-classification validator with EditMode tests.

## Existing Scenes

- `Assets/Scenes/SampleScene.unity` — default template scene; the only scene in the build list
  (`ProjectSettings/EditorBuildSettings.asset`).
- `Assets/Settings/Scenes/URP2DSceneTemplate.unity` + `Assets/Settings/Lit2DSceneTemplate.scenetemplate`
  — URP 2D scene templates (scaffolding for "New Scene", not game levels).
- No Ghost / Act level scenes exist.

## Existing Scripts

- None. `Assets/**/*.cs` returns no files. No `Assets/Scripts/` folder and no `.asmdef`.
- Note: `DraggableNode` (CONFIRMED_PROJECT_CONTEXT §14 #11) does not exist yet.
- Without asmdefs, future code compiles into the default `Assembly-CSharp`.

## Package Setup (`Packages/manifest.json`)

- Render: `com.unity.render-pipelines.universal` 17.4.0
- Input: `com.unity.inputsystem` 1.19.0
- UI: `com.unity.ugui` 2.0.0 (UI Toolkit / uielements module also available)
- 2D suite: animation, aseprite, psdimporter, sprite, spriteshape, tilemap (+extras), tooling
- Testing: `com.unity.test-framework` 1.6.0 (EditMode/PlayMode tests available)
- Also: timeline, visualscripting, collab-proxy, Rider/VS IDE packages, standard modules.
- No third-party / networking / LLM packages.

## Render Pipeline Status

- URP with 2D Renderer, active. `QualitySettings.asset` binds `customRenderPipeline`
  (`Assets/Settings/UniversalRP.asset`, guid `681886c5…`) across all six quality levels.
- `GraphicsSettings.m_CustomRenderPipeline` is `0` (normal for Unity 6 — assigned per quality
  level); SRP default settings reference `UniversalRenderPipeline`.
- Supporting assets: `Renderer2D.asset`, `UniversalRenderPipelineGlobalSettings.asset`,
  `DefaultVolumeProfile.asset`. Use 2D/URP-compatible shaders and `SpriteRenderer`.

## Input System Status

- New Input System only. `ProjectSettings.asset → activeInputHandler: 1` (0=old, 1=new, 2=both).
- Project-wide actions asset `Assets/InputSystem_Actions.inputactions` wired via
  `com.unity.input.settings.actions` in `EditorBuildSettings`.
- UGUI must use `InputSystemUIInputModule`; input code must use `UnityEngine.InputSystem`
  (legacy `UnityEngine.Input.*` is disabled).

## Relevant Project Settings

- Unity 6000.4.11f1 (`ProjectSettings/ProjectVersion.txt`).
- `productName: Ghost` (already set); `companyName: DefaultCompany` (placeholder).
- Build scenes: SampleScene only. No asmdefs. URP active; new Input System active.

## Risks / Constraints Before Implementation

1. New Input System only — use `UnityEngine.InputSystem`; UGUI EventSystem needs
   `InputSystemUIInputModule`; legacy `Input.*` will not work.
2. URP-2D — use 2D/URP shaders and sprites; no Built-in RP materials.
3. WebGL target (scope) not verifiable from repo (lives in editor/Library). Confirm the WebGL
   module is installed and the target is switched in-editor before WebGL-specific work; obey NFR3
   (no threads, no reflection-heavy / non-WebGL APIs).
4. No level scenes, and scene creation is out of scope here — the first task must be scene-free
   (or testable in SampleScene).
5. No asmdefs — the first task should add a runtime asmdef + a test asmdef so tests can reference
   game code.
6. Cosmetic: `companyName` still `DefaultCompany` (set before any WebGL release; not blocking).
7. `.meta` files are Unity-managed — must not be hand-edited.

## Safest First Implementation Task

Act 1 "intent grouping" core logic as a pure C# class + an EditMode unit test. Given message
cards each carrying a true intent label and a player grouping, the validator returns whether the
grouping is correct. Scene-free, art-free, input-free, fully testable via the Test Framework, and
maps directly to the confirmed Act 1 mechanic and ARCHITECTURE's `PuzzleValidator` + Data System.

## Files Likely Needed for the First Implementation Task

- `Assets/Scripts/Ghost.Runtime.asmdef` — runtime assembly definition
- `Assets/Scripts/Puzzles/IntentClassification/IntentCard.cs` — small data type
- `Assets/Scripts/Puzzles/IntentClassification/IntentClassificationValidator.cs` — pure logic
- `Assets/Tests/EditMode/Ghost.EditModeTests.asmdef` — test assembly
- `Assets/Tests/EditMode/IntentClassificationValidatorTests.cs` — unit tests
- Unity auto-generates the matching `.meta` files (not hand-edited).

## Verification Result

Read-only inventory verified directly from repo files: `manifest.json`, `ProjectVersion.txt`,
`QualitySettings.asset`, `GraphicsSettings.asset`, `ProjectSettings.asset`,
`EditorBuildSettings.asset`, `Assets/**` and `ProjectSettings/**` globs, and `Assets/**/*.cs`
glob (= none). No Unity files were modified.

## Next Task

M0-T04 — Implement the Act 1 intent-classification core validator as pure C# logic with EditMode tests.
