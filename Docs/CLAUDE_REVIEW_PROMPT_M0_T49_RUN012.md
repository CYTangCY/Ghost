# Claude Review Prompt - M0-T49 Run 012

Please review the repository state for M0-T49 Run 012. Read these files first:

- `Docs/CURRENT_TASK.md`
- `Docs/CONFIRMED_PROJECT_CONTEXT.md`
- `Docs/LEARNING_CONTENT.md`
- `Docs/codex_runs/M0-T49_012_lily_footwear_plain_language_audit.md`
- `Docs/CODE_WALKTHROUGH.md` (Run 012 section)
- `unorganized_data/dissertation/Ghost_Final_Report_Draft.md`
- `unorganized_data/dissertation/audit_plain_english.py`
- `unorganized_data/dissertation/latex/Ghost_Final_Report.tex`

Review these points strictly:

1. Confirm `Assets/Resources/Characters/LilyPixelFullBody.png` keeps the approved Run 007 RPG pixel style and changes only the lower shoe area. Compare it with `tmp/imagegen/run012/LilyPixelFullBody_before.png`. Expected difference box: x=39..62, y=110..121; zero changed pixels above y=110.
2. Confirm the new shoes are black low-vamp Mary Jane flats that show the top of each foot and a small part of each ankle. Hair, glasses, face, proportions, blazer, red KCL lanyard, tablet, trousers, and pose must be unchanged.
3. Confirm `LilyPixelPortrait.png` content was not replaced.
4. Review `LilyPixelSpriteImporter.cs` for a small, safe, Editor-only Unity import repair. Check Single sprite mode, point filtering, no mipmaps, alpha transparency, and no compression. Do not request hand-edited `.meta` files.
5. Review the dissertation for plain, natural academic English near an IELTS 6.5 to 7 writing level. Treat the automated figures as supporting checks, not an official IELTS score.
6. Check that the report avoids generic AI-style phrases and uncommon substitute words, while keeping necessary technical terms and formal paper titles accurate.
7. Check that simpler wording did not weaken the critical comparison of papers, tool choices, evidence limits, or the honest 64-69 present-state mark estimate.
8. Re-run `python unorganized_data/dissertation/audit_plain_english.py` and `python unorganized_data/dissertation/check_latex_report.py` if available.
9. Do not claim Play Mode or PDF layout success. Both still need human or compiler checks.
10. Report findings first, ordered by severity with file and line references. Then give a short verdict and a Chinese STAR summary.

Expected recorded evidence:

- focused Unity test: 8 passed, 0 failed, 0 skipped;
- report: 9,554 words, 721 sentences, 13.3 words per sentence;
- zero sentences over 32 words;
- zero listed AI-style terms and zero listed uncommon alternatives;
- LaTeX static check passes with 42/42 citation keys used;
- expected open items: seven figures, five student TODOs, TeX compile, PDF inspection, and human Play Mode review.