# M0-T49 Run 017 - Chapter Flow Figure

## Run Identity

- Task ID: M0-T49
- Run number: 017
- Date: 2026-07-24
- Scope: dissertation figure only

## Original Request Summary

Generate the report figure captioned "Planned chapter flow from the opening story through six
teaching chapters to the final integration chapter." Include both Chapter 0 and the Foundation
overview because Chapter 0 provides the narrative basis for the game.

## Files Created

- `unorganized_data/dissertation/latex/figures/chapter-flow.svg`
- `unorganized_data/dissertation/latex/figures/chapter-flow.png`
- `Docs/codex_runs/M0-T49_017_chapter_flow_figure.md`
- `Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN017.md`

## Files Modified

- None.

## Figure Structure

- Chapter 0: opening story and motivation.
- Foundation: Ghost's Voice Basics overview.
- Chapters 1-6: Intent, Entities, Dialogue, Confidence, Testing, and Backend.
- Final Chapter: full-system integration and ending.
- Directional arrows show the complete order from Chapter 0 to the Final Chapter.

## Tests and Checks Run

- SVG XML parse: passed.
- PNG render through bundled Sharp: passed.
- PNG metadata: 6400 x 3600, 192 DPI, sRGB, RGBA.
- Visual preview: inspected after rendering.
- First visual fix: moved the teaching-area background below the connector layer.
- Second visual fix: reduced arrowhead size and kept arrow tips outside chapter cards.
- LaTeX path check: `contents/design.tex` already references `figures/chapter-flow.png`.
- Appendix path check: `contents/appendices.tex` already lists the same PNG.
- `git diff --check` for the SVG: passed.
- LaTeX compilation: Not run - `pdflatex` is not installed in the current environment.

## Errors Encountered

- `apply_patch` failed during two follow-up SVG edits because the Windows sandbox helper returned
  `helper_unknown_error: apply deny-read ACLs`.
- The first compact preview exceeded the tool output limit.

## Fixes Applied

- Used bounded PowerShell text replacements only after `apply_patch` failed.
- Generated a smaller temporary JPEG for visual inspection.
- Corrected SVG layer order and arrow spacing, then regenerated the final PNG.

## Intentionally Not Changed

- No LaTeX chapter text or caption.
- No Unity files, scenes, scripts, packages, settings, or `.meta` files.
- No previous report figures or run logs.

## Remaining Risks

- The complete report could not be compiled in this environment.
- The current caption does not mention the Foundation overview by name, although the figure includes
  it between Chapter 0 and Chapter 1.

## Next Recommended Step

Compile the LaTeX report and confirm that the labels remain readable at the final figure width.

## Chinese STAR

- **S 情境：** 論文已有 chapter-flow 圖片位置，但原本只有 placeholder。
- **T 任務：** 產生包含 Chapter 0、Foundation、六個教學章節和 Final Chapter 的完整流程圖。
- **A 行動：** 建立可編輯 SVG、輸出高解析 PNG，並修正箭頭圖層、大小和卡片間距。
- **R 結果：** 產生 6400 x 3600 的論文流程圖，路徑與現有 LaTeX 引用一致。
