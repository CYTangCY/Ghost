# Claude Review Prompt — M0-T49 Run 007 Lily Colour Correction

請先讀 `AGENTS.md`、`Docs/codex_runs/M0-T49_006_character_pixel_style_unification.md`、`Docs/codex_runs/M0-T49_007_lily_colour_correction.md`、實際 PNG 與 git diff。

Review scope is deliberately narrow:

1. Confirm `LilyPixelFullBody.png` remains 96x128 and `LilyPixelPortrait.png` remains 96x96.
2. Confirm the Run 006 low-resolution RPG silhouette, high blonde ponytail, glasses, red KCL lanyard, grey shirt, charcoal trousers, tablet, pose, and hard pixel edges are preserved.
3. Confirm the blazer is now deep navy blue rather than black.
4. Confirm the Oxford shoes are now black rather than brown.
5. Confirm both files retain transparent corners and hard alpha only.
6. Confirm no runtime C#, Ghost asset, gameplay, scene, ProjectSettings, validator, or `.meta` file changed in this colour-only run.
7. Confirm `CODE_WALKTHROUGH.md` and the single current `UNITY_TEST_CHECKLIST.md` use the updated colour description.

Evidence:

- Ghost.Presentation.csproj passed with 0 warnings and 0 errors.
- Both PNGs retained authored dimensions and alpha values `[0, 255]`.
- Shoe-region scan: 258 dark pixels, 1 brown-like pixel.
- Interactive Play Mode was not run; Unity Refresh and visual inspection are still required.

Return findings first with file evidence, then Verified Good, remaining visual test gap, closure decision, an exact next Codex prompt if needed, and Chinese STAR. Do not modify, commit, or push during the first review.
