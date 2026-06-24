# M0-T32 — Run 002 — embed and expand ambient banter

## Task ID

M0-T32

## Run Number

002

## Date

2026-06-24

## Original Request / Codex Prompt Summary

Fix the in-act ambient banter panel because it was covering parts of the puzzle UI. It should be embedded in spare UI space instead of floating over the scene. Also expand the authored banter so each act has more interesting dialogue, with at least 15 lines for each character in each act.

## Files Created

- `Docs/codex_runs/M0-T32_002_embed_and_expand_ambient_banter.md`

## Files Modified

- `Assets/Presentation/Banter/BanterData.cs`
- `Assets/Presentation/Banter/AmbientBanterHook.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Counted authored beat tags with `rg` to confirm Act 1/2/3 each have 16 Lily lines and 16 Ghost lines.
- `git diff --check -- Assets/Presentation/Banter/BanterData.cs Assets/Presentation/Banter/AmbientBanterHook.cs Assets/Presentation/Banter/AmbientBanterPanel.cs`
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- Beat counts were confirmed as 16 Lily lines and 16 Ghost lines for each of Act 1, Act 2, and Act 3.
- `git diff --check` completed with no whitespace errors. Git reported line-ending warnings for edited C# files.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Errors Encountered

- The previous M0-T32 implementation created a high-sorting runtime overlay canvas (`sortingOrder` 30000), so the banter panel sat above the puzzle UI instead of participating in the act layout.
- No Unity Editor runtime errors were observed in this Codex session because Play Mode was not run.

## Fixes Applied

- Changed `AmbientBanterHook` to schedule a short runtime bootstrapper after scene load, wait for act presenters to finish rendering, then embed the banter panel into existing UI layout hosts.
- Act 1 and Act 2 now prefer the `Validation Controls` row.
- Act 3 now prefers the right-side `Goal Test List`.
- Kept a low-sorting fallback canvas only for cases where expected layout hosts are unavailable.
- Expanded `BanterData` so each act contains 32 beats total: 16 Lily lines and 16 Ghost lines.
- Updated documentation and checklist to describe embedded placement and the larger dialogue loops.

## What Was Intentionally Not Changed

- No puzzle logic, validators, sessions, or Act 1/2/3 puzzle mechanics were changed.
- No Act scene YAML was edited or regenerated.
- No ProjectSettings, Packages, Build Settings, `.meta` files, backend, LLM, save/load, player-choice branching, or final art were changed.
- `AmbientBanterPanel` was not changed because its timer/Next/speaker-display behavior still matched the task.

## Remaining Risks

- The embedded positions require human Unity Play Mode verification in each act to confirm the panel fits the existing layout and does not crowd validation feedback, chips, cards, palette controls, graph guide content, or the Act 3 board.
- Long Lily lines in the compact Act 1/2 validation row may truncate depending on resolution; Play Mode should confirm whether the row needs a later layout pass.
- Because Play Mode was not run in this session, compile/runtime behavior still needs Unity verification.

## Next Recommended Step

Open Unity, let scripts compile, enter Act 1/2/3 through the shell, and verify the banter panel is embedded into spare UI space, cycles the expanded lines, keeps puzzle controls usable, and produces no Console errors.
