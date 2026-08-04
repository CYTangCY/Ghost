# Claude Review Prompt - M0-T49 Run 009

Review M0-T49 for closure using the repository and real evidence. Read:

- `Docs/CURRENT_TASK.md`
- `Docs/LEARNING_CONTENT.md` (chapter-split override is authoritative)
- `Docs/codex_runs/M0-T49_005_remediation_pass.md`
- `Docs/codex_runs/M0-T49_006_character_pixel_style_unification.md`
- `Docs/codex_runs/M0-T49_007_lily_colour_correction.md`
- `Docs/codex_runs/M0-T49_008_batchmode_verification.md`
- `Docs/codex_runs/M0-T49_009_user_provisional_playmode_acceptance.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

Inspect `git status`, relevant diffs, `tmp/run008/` builder logs, and Unity test XML files.

Run 008 established these automated results:

- Full Unity EditMode: 87 discovered, 87 passed, 0 failed, 0 skipped.
- Focused suites: Shell Return 8/8; Act 6 Backend 8/8; Act 5 presenter 1/1.
- Chapter 0, Chapter 6 Backend, Final Chapter, and Game Shell builders completed successfully in order, with Game Shell last.
- Each regenerated scene has exactly one Main Camera, one Canvas, one EventSystem, and zero missing scripts.
- Final static guards passed.

For run 009, the user explicitly asked to treat the remaining human 1920x1080 Play Mode checklist as complete for review. Record this as **user provisional acceptance**, not as a Codex-observed test result. No new Unity run occurred.

Please:

1. Review findings first, ordered by severity with file/line references.
2. Decide whether runs 005-009 and the user's provisional acceptance are enough to close M0-T49.
3. If closing, archive the task, update `HANDOFF_LOG.md`, and advance `CURRENT_TASK.md` to the dissertation/writing phase without changing the confirmed chapter structure.
4. Keep the final live build/demo smoke check as a submission risk if it has not been independently observed.
5. Include a Chinese STAR summary.
