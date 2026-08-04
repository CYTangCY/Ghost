# M0-T49 — Run 006 — Lily and Ghost pixel-style unification

## Task ID

M0-T49

## Run Number

006

## Date

2026-07-16

## Original Request / Codex Prompt Summary

Replace Lily and Ghost with an original low-resolution RPG pixel style inspired only by the broad chunky sprite language of the supplied reference. The current Lily looked too AI-polished and semi-realistic; Ghost looked too simple. Preserve the approved Lily high ponytail / black blazer / red KCL lanyard / brown Oxford shoes, and preserve Ghost's large dark eyes and cute direction while giving Ghost a stronger complete silhouette and mood sprites.

## Files Created

- Assets/Resources/Characters/GhostPixelNeutral.png
- Assets/Resources/Characters/GhostPixelHappy.png
- Assets/Resources/Characters/GhostPixelConfused.png
- Assets/Resources/Characters/GhostPixelSad.png
- Docs/codex_runs/M0-T49_006_character_pixel_style_unification.md
- Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN006.md

## Files Modified

- Assets/Resources/Characters/LilyPixelFullBody.png
- Assets/Resources/Characters/LilyPixelPortrait.png
- Assets/Presentation/GhostAvatar/GhostFaceView.cs
- Assets/Presentation/Characters/LilyPixelPortraitFactory.cs
- Assets/Presentation/Shell/LilyDialogueFrame.cs
- Assets/Presentation/Banter/AmbientBanterPanel.cs
- Assets/Tests/EditMode/ShellReturnToHubOverlayTests.cs
- Docs/CODE_WALKTHROUGH.md
- Docs/UNITY_TEST_CHECKLIST.md

## Tests or Checks Run

- Read CURRENT_TASK, LEARNING_CONTENT, run 005, the current worktree, GhostFaceView, LilyDialogueFrame, and AmbientBanterPanel before editing.
- Generated a new original Lily sprite and a four-mood Ghost sheet with the built-in image tool.
- Removed chroma backgrounds, applied a green-dominance cleanup, cropped, nearest-neighbor resized, and palette-limited the final project PNGs.
- Built Ghost.Presentation.csproj after the final factory/view changes.
- Built Ghost.EditModeTests.csproj after adding resource and mood-switch tests.
- Validated all six final PNGs with Pillow for authored dimensions, alpha extrema, visible bounds, and transparent corners.
- Ran git diff --check for the affected scripts and docs.
- Ran delete/rename, non-ASCII C# source, and resource-wiring rg guards.
- Inspected the open Unity Editor process and current meta/import state.

## Test / Check Result

- Ghost.Presentation.csproj: passed with 0 errors and four pre-existing FindFirstObjectByType deprecation warnings.
- Ghost.EditModeTests.csproj: passed with 0 errors and 0 warnings.
- LilyPixelFullBody.png: 96x128 RGBA; alpha values only 0 and 255; transparent corners.
- LilyPixelPortrait.png and all four Ghost mood PNGs: 96x96 RGBA; alpha values only 0 and 255; transparent corners.
- The four Ghost moods share one consistent outlined body, large dark eyes, side arms, wavy tail, and blue-lavender shadow palette.
- GhostFaceView prefers the mood Sprite and hides old eye/mouth/mood-mark overlays; the original programmatic face remains fallback-only.
- Shell dialogue and ambient banter use neutral Ghost art when no serialized Ghost portrait is assigned.
- Protected-path guard found 0 deleted or renamed files. No scene YAML or ProjectSettings was edited in run 006.
- Unity EditMode Test Runner: Not run — Unity remained open with D:/Code/Ghost, and the four new Ghost PNGs had not yet received Unity-generated meta files.
- Unity interactive Play Mode: Not run — visual confirmation requires Unity to refresh/import the new PNGs and a human Game view check.

## Errors Encountered

- Two image-generation attempts that included the supplied reference were rejected by the image safety classifier. A text-only generic low-resolution RPG specification succeeded without changing the requested character direction.
- apply_patch again failed because the Windows sandbox helper could not apply deny-read ACLs.
- The first compile after adding GhostPixelSpriteFactory.cs failed because Unity had not regenerated Ghost.Presentation.csproj to include the new file; LilyDialogueFrame also needed the GhostAvatar namespace import.
- Unity stayed open on Chapter0OpeningStory and did not auto-refresh the four new Ghost PNGs during this run, so their meta files were not created yet.

## Fixes Applied

- Used the supplied image only to understand the requested broad pixel scale; the successful generation prompt did not copy its characters or identity.
- Replaced the smooth 967x1626 Lily render with a hard-edged 96x128 RPG sprite and replaced its portrait with a matching 96x96 crop.
- Added four 96x96 Ghost mood images rather than drawing a simple white UI block.
- Merged GhostPixelSpriteFactory into the existing GhostFaceView.cs after confirming the temporary new script had no meta, avoiding dependence on Unity project-file regeneration.
- Added the missing GhostAvatar import to LilyDialogueFrame and recompiled successfully.
- Changed both character factories to load Texture2D resources and create full-rect cached runtime Sprites, avoiding stale importer crop rectangles from the previous larger Lily image.
- Added six resource-resolution test cases and one Ghost mood-switch view test.
- Updated walkthrough and the single current M0-T49 checklist with the new pixel-style acceptance checks.

## What Was Intentionally Not Changed

- No puzzle validator, session, authored sample data, scoring, completion, navigation, or chapter structure changed.
- No existing scene was hand-edited; no builder was run while Unity held the project lock.
- No ProjectSettings, Packages, Backend, or LLM code changed.
- No existing `.meta` file was renamed, deleted, or hand-edited.
- The original programmatic Ghost face and code-drawn Lily portrait remain fallback paths for missing resources.

## Remaining Risks

- Unity must refresh or reopen to create meta files for the four new Ghost PNGs and reimport the resized Lily PNGs.
- The newly added resource/mood EditMode tests have compiled but have not run in Unity.
- Human Play Mode must confirm point-filtered crisp edges, correct Lily framing in Chapter 0/Final/Shell, all four Ghost mood changes, no duplicate old eyes or text mouth, and acceptable scale in small portrait frames.
- Run 005 scene builders and full EditMode verification also remain pending while Unity is open.

## Final Image Prompt Set

Built-in image generation was used. Final project PNGs were produced from these successful prompt specifications, followed by local chroma removal, hard-alpha cleanup, cropping, nearest-neighbor resizing, and palette limiting.

### Lily

```text
Create one original low-resolution 16-bit RPG overworld sprite of Lily, a professional university postdoctoral researcher. High platinum-blonde ponytail, small round black academic glasses, quiet intelligent expression, tailored black blazer, pale grey shirt, charcoal trousers, red lanyard with tiny KCL ID, brown British Oxford shoes, and a small dark tablet. Use an approximately 48x64 native-pixel look with chunky square pixels, hard nearest-neighbor edges, strong dark outline, limited palette, two-tone shading, and tiny dot-and-line facial features. One centered full-body figure on perfectly flat #00ff00 chroma green; no floor, shadow, scenery, smooth gradients, realistic texture, 3D, watermark, or green in the subject.
```

### Ghost mood sheet

```text
Create a four-pose 2x2 sprite sheet for one original cute floating sheet ghost in the same low-resolution 16-bit RPG pixel language: rounded white-blue body, tiny side arms, wavy tail, large dark oval eyes with pale highlights, simple mouth, and blue-lavender edge shadows. Neutral, Happy, Confused, and Sad cells must keep identical body design, scale, outline, and palette. Use an approximately 48x48 native-pixel look per pose with hard square pixels and no antialiasing. Perfectly flat #00ff00 background; no borders, floor, shadow, scenery, gradient, high-resolution rendering, or watermark.
```

## Next Recommended Step

In Unity, exit Play Mode if active and run Assets > Refresh or reopen the project. Confirm all four Ghost PNG meta files are generated, run the complete EditMode suite (expected at least 87 discovered tests after run 005 and run 006 additions), then follow the current M0-T49 checklist at 1920x1080. Close Unity afterward if batchmode scene regeneration from run 005 is still required.
