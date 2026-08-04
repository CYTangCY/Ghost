# M0-T49 — Run 007 — Lily blazer and shoe colour correction

## Task ID

M0-T49

## Run Number

007

## Date

2026-07-16

## Original Request / Codex Prompt Summary

Keep the Run 006 low-resolution RPG Lily design, but restore the suit blazer from black to the original blue and restore the shoes from brown to the original black.

## Files Created

- Docs/codex_runs/M0-T49_007_lily_colour_correction.md
- Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN007.md

## Files Modified

- Assets/Resources/Characters/LilyPixelFullBody.png
- Assets/Resources/Characters/LilyPixelPortrait.png
- Docs/CODE_WALKTHROUGH.md
- Docs/UNITY_TEST_CHECKLIST.md

## Tests or Checks Run

- Used the built-in image editing tool with the recent Run 006 Lily image.
- Removed chroma green, cropped the subject, nearest-neighbor resized, and palette-limited the replacement full-body and portrait PNGs.
- Validated both PNG dimensions, alpha values, visible bounds, and transparent corners with Pillow.
- Built Ghost.Presentation.csproj.
- Inspected the lower shoe region for brown-like versus dark pixels.
- Ran git diff --check and protected delete/rename guards for the affected paths.

## Test / Check Result

- LilyPixelFullBody.png remains 96x128; LilyPixelPortrait.png remains 96x96.
- Both images have only alpha 0 and 255, with transparent corners.
- The full-body sprite contains 371 deep-blue pixels for the restored navy blazer.
- The lower shoe region contains 290 visible pixels: 258 dark pixels and only 1 brown-like antialias/palette pixel, confirming the shoes read as black.
- Ghost.Presentation.csproj passed with 0 warnings and 0 errors.
- Runtime code, Ghost assets, gameplay, scenes, ProjectSettings, and Inspector wiring were unchanged.
- Unity interactive Play Mode: Not run — this is a PNG-only colour correction and final visual confirmation remains a human Game view check after Unity refresh.

## Errors Encountered

- No implementation errors. Pillow emitted only a deprecation warning for Image.getdata.

## Fixes Applied

- Replaced only the blazer palette with deep navy blue.
- Replaced only the shoe palette with black.
- Preserved the high blonde ponytail, glasses, expression, red KCL lanyard, grey shirt, charcoal trousers, tablet, pose, silhouette, low-resolution proportions, and hard-edged pixel style.
- Updated current walkthrough/checklist wording to deep navy-blue blazer and black Oxford shoes.

## Final Image Prompt

```text
Edit only the most recent low-resolution RPG Lily sprite. Change the black suit blazer to the original deep navy-blue suit blazer, preserving shape, lapels, sleeves, outline, shading, and fit. Change the brown Oxford shoes to black leather Oxford shoes, preserving shape, laces, outline, and shading. Preserve the high platinum-blonde ponytail, round black glasses, expression, pale grey shirt, red lanyard and KCL ID, charcoal trousers, tablet, pose, proportions, framing, chunky pixels, hard edges, limited palette, and flat #00ff00 chroma background. Do not redesign or increase detail.
```

## What Was Intentionally Not Changed

- No C# runtime logic or tests changed.
- No Ghost asset changed.
- No scene, scene YAML, ProjectSettings, Packages, validator, scoring, navigation, Backend, or LLM code changed.
- No `.meta` file was renamed, deleted, or hand-edited.

## Remaining Risks

- Unity must refresh the two modified PNGs before the new colours appear in Game view.
- Human Play Mode should confirm the deep navy blazer remains distinct from charcoal trousers and the black shoes remain readable against the scene background.

## Next Recommended Step

Run Assets > Refresh in Unity and inspect Lily in Chapter 0, the Shell portrait, ambient banter, and Final Chapter at 1920x1080. Continue the pending run 005 / run 006 full Unity verification afterward.
