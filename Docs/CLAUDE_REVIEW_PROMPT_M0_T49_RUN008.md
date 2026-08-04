# Claude Review Prompt - M0-T49 Run 008

Review M0-T49 run 008 as a verification-only remediation pass. Read the repository rather than relying on this prompt alone.

Read first:

- `Docs/CURRENT_TASK.md`
- `Docs/LEARNING_CONTENT.md` (the chapter-split override remains authoritative)
- `Docs/codex_runs/M0-T49_005_remediation_pass.md`
- `Docs/codex_runs/M0-T49_006_character_pixel_style_unification.md`
- `Docs/codex_runs/M0-T49_007_lily_colour_correction.md`
- `Docs/codex_runs/M0-T49_008_batchmode_verification.md`
- `Docs/UNITY_TEST_CHECKLIST.md`
- `Docs/CODE_WALKTHROUGH.md`

Then inspect `git status`, the relevant diffs, and the saved evidence under `tmp/run008/`.

## Scope Implemented

- Regenerated, in order, Chapter 0, Chapter 6 Backend Response, Final Chapter, and Game Shell last.
- Ran the full Unity EditMode suite and the three requested focused suites.
- Ran serialized guards on the four regenerated scenes.
- Ran protected-path, `.meta`, delete/rename, changed-C# non-ASCII, current-checklist-count, and `git diff --check` guards.
- Made no production-code or gameplay change.

## Actual Results to Verify

- Initial full suite: 87 discovered, 85 passed, 2 failed, 0 skipped.
- The two failures were test-harness setup problems:
  - `Act6BackendResponseValidatorTests` invoked a Play Mode-only backend runner from EditMode.
  - `ShellReturnToHubOverlayTests` looked up generated UI children before initializing `GhostFaceView`.
- After minimal test-only corrections: full suite 87/87.
- Focused suites:
  - `ShellReturnToHubOverlayTests`: 8/8.
  - `Act6BackendResponseValidatorTests`: 8/8.
  - `Act5TestingStaticPresenterTests`: 1/1.
- Each regenerated scene has exactly one Main Camera, Canvas, and EventSystem, with zero missing scripts.
- Final `git diff --check` exits 0 after adding `*.unity whitespace=-trailing-space` because Unity serializes empty scalar values with a trailing space.
- Changed C# non-ASCII count: 0 across 55 files; delete/rename count: 0; `.meta` delete/rename count: 0.
- Exactly one `[CURRENT END-TO-END CHECKLIST]` remains.

## Review Questions

1. Confirm the two C# test edits are test-harness-only, preserve the intended assertions, and do not conceal runtime defects.
2. Confirm `.gitattributes` is narrowly scoped to Unity scene YAML and does not disable whitespace checks for C# or docs.
3. Confirm all four builder logs show successful final runs and Game Shell is last; retain the first failed Final namespace attempt as honest evidence.
4. Confirm the XML files support the reported full and focused test counts.
5. Confirm the serialized scene guard method and results are adequate for duplicate Camera/Canvas/EventSystem and missing scripts.
6. Confirm no protected path, `.meta`, scene-YAML, validator, session, sample-data, ProjectSettings, Packages, or unrelated dirty-worktree content was improperly changed during run 008.
7. Confirm `Docs/UNITY_TEST_CHECKLIST.md` contains exactly one current end-to-end checklist and that automated evidence does not claim the human Play Mode gate passed.

Report findings first, ordered by severity with file/line references. If no findings remain, say so clearly and state that the only remaining gate is human execution of the single current 1920x1080 Play Mode checklist. Do not archive or advance `CURRENT_TASK.md` until that human result is supplied.
