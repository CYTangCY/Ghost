# M0-T49 - Run 012 - Lily Footwear and Plain-English Audit

## Task ID

M0-T49 follow-on art and dissertation support

## Run Number

012

## Date

2026-07-16

## Original Request / Codex Prompt Summary

Keep Lily in the approved Run 007 RPG pixel style. Change only her shoes so the top of each foot and a small part of each ankle are visible. Also check the full LaTeX report for English near IELTS 6.5 to 7, remove AI-like wording and uncommon substitutes, and keep required technical terms understandable.

## Files Created

- `Assets/Presentation/Characters/Editor/LilyPixelSpriteImporter.cs`
- `Assets/Presentation/Characters/Editor/LilyPixelSpriteImporter.cs.meta` (Unity generated)
- `Assets/Presentation/Characters/Editor.meta` (Unity generated)
- `unorganized_data/dissertation/audit_plain_english.py`
- `Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN012.md`
- `Docs/codex_runs/M0-T49_012_lily_footwear_plain_language_audit.md`

## Files Modified

- `Assets/Resources/Characters/LilyPixelFullBody.png`
- `Assets/Resources/Characters/LilyPixelFullBody.png.meta` (Unity generated)
- `Assets/Resources/Characters/LilyPixelPortrait.png.meta` (Unity generated; texture import repair only)
- `Docs/CODE_WALKTHROUGH.md`
- `unorganized_data/dissertation/Ghost_Final_Report_Draft.md`
- `unorganized_data/dissertation/build_latex_report.py`
- generated chapter files under `unorganized_data/dissertation/latex/contents/`
- `unorganized_data/dissertation/latex/contents/appendices.tex`

## Image Result

- The Run 007 full-body sprite is the base image.
- Hair, face, glasses, body size, navy blazer, red KCL lanyard, tablet, charcoal trousers, pose, and pixel style are unchanged.
- Only the ankle and shoe area changed. The new black low-vamp Mary Jane shoes expose the top of each foot and a short ankle line.
- Pixel comparison reports a difference box of x=39..62 and y=110..121, with 189 changed pixels and zero changed pixels above y=110.
- The final PNG is 96x128 RGBA with hard 0/255 alpha and transparent corners.
- `LilyPixelPortrait.png` content is unchanged.

## English Result

The report checker covers all LaTeX prose files. It ignores table layout when measuring sentence length but still scans table text for blocked wording. It checks long sentences, a named list of common AI-style phrases, and uncommon words that have simpler choices. Required subject terms remain when they are needed.

Final measurements:

- 11 prose files;
- 9,554 words;
- 721 sentences;
- 13.3 words per sentence on average;
- estimated Flesch Reading Ease 51.2;
- estimated Flesch-Kincaid grade 9.4;
- zero sentences over 32 words;
- zero listed AI-style terms; and
- zero listed uncommon alternatives.

These figures support clear mid-level academic English, but they are not an official IELTS score.

## Tests or Checks Run

- Pixel-by-pixel comparison against the approved Run 007 source.
- PNG size, mode, alpha, and corner checks.
- Unity 6000.4.11f1 importer repair through `RepairLilyPixelSpriteImports`.
- Focused Unity EditMode `ShellReturnToHubOverlayTests`.
- `audit_plain_english.py`.
- `build_latex_report.py`.
- `check_latex_report.py`.
- Python syntax checks for the report scripts.
- Changed C# non-ASCII scan.
- `git diff --check`.

## Test / Check Result

- Pixel isolation: passed; zero changes above y=110.
- Unity focused tests: 8 passed, 0 failed, 0 skipped.
- Unity log scan: zero `error CS`, exceptions, stale sprite rectangles, or fatal exits in the valid final run.
- Plain-English audit: passed with the figures listed above.
- LaTeX static check: passed with 42 BibTeX entries, 42 used citation keys, 28 labels, 27 references, 7 figure calls, and 10 table captions.
- Expected LaTeX warnings remain for seven screenshot placeholders and five student TODO fields.
- Unity interactive Play Mode: Not run - this environment ran batch-mode resource checks only.
- Full Unity EditMode suite: Not run - this run changed one image, one editor import helper, and dissertation text; the focused resource suite was used.
- TeX compile and PDF inspection: Not run - no TeX compiler is installed in this environment.

## Errors Encountered

1. The Windows sandbox helper continued to fail while applying workspace ACLs. Approved outside-sandbox commands were used for the required local work.
2. The older Lily PNG metadata contained sprite-sheet rectangles outside the current 96px files. The Unity importer helper changed both Lily textures to Single sprite mode.
3. One focused Unity test launch happened before the importer process released the project lock. That launch produced no valid result. The process was allowed to exit, the old XML was removed, and the focused test was rerun successfully.
4. The first language checker treated complete LaTeX tables as single sentences. Table environments were excluded from sentence-length counts while their text remained covered by the blocked-word scan.

## Fixes Applied

- Restored the approved Run 007 full-body art and redrew only the shoe and ankle pixels.
- Added a repeatable Unity texture-import repair instead of hand-editing `.meta` files.
- Split the remaining long prose sentences.
- Replaced avoidable words such as `affective`, `preregistered`, `provisionally`, `authored`, `concurrency`, `anonymisation`, and `authorisation` with plain explanations in the report body.
- Regenerated the LaTeX chapter files from the edited Markdown source.

## What Was Intentionally Not Changed

- Lily's portrait image content.
- Any full-body sprite pixel above y=110.
- Ghost art.
- Game scenes, presenters, validators, sessions, puzzle data, progress rules, backend behaviour, ProjectSettings, or Packages.
- Paper titles and formal source titles in the reference list.
- Research results, test counts, mark estimate, or study limitations.

## Remaining Risks

- Chapter 0 and Final Chapter still need a human 1920x1080 Play Mode check for Lily's size, crop, and footwear readability.
- The language measures are useful checks, not an IELTS assessment. A human proofread is still required for natural flow and consistent student voice.
- Seven figures, five student fields, a full TeX compile, PDF layout review, and field-by-field reference checks remain before submission.

## Next Recommended Step

Open Chapter 0 and the Final Chapter at 1920x1080 and confirm that Lily keeps the old appearance while the low-cut black shoes remain readable. Then run the LaTeX project in Overleaf or TeX Live, fill the student TODOs, add the seven figures, and give the linked Run 012 prompt to Claude for strict review.

## Chinese STAR

- **S 情境：** Lily 前一個修改版本改動了身形與服裝，超出只換鞋的要求；論文也需要確認英文是否簡單、自然。
- **T 任務：** 恢復上一版像素 Lily，只修改鞋子與必要的腳踝像素，並詳細檢查整份 LaTeX 論文的句長和用詞。
- **A 行動：** 以 Run 007 圖片為底重畫 189 個腳部像素，用像素差異確認其他區域沒有變動；修復 Unity Sprite 匯入；加入可重跑的英文檢查器，改寫長句與少用詞，再重新產生 LaTeX。
- **R 結果：** 圖片差異只在 y=110 到 121，Unity 聚焦測試 8/8 通過；論文平均句長 13.3 字，沒有超過 32 字的句子，也沒有命中列出的 AI 套話或少用替代詞。LaTeX 靜態檢查通過。