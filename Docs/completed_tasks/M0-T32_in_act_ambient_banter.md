# M0-T32 — In-Act Ambient Ghost+Lily Banter

## Completion Status

Completed (Codex runs 001 initial, 002 embed+expand lines, 003 per-act text-box sizing). The user
accepted the current state ("先暫時這樣，繼續下一步"). Claude reviewed scope + run logs.

## Date

2026-06-24

## Summary

Added a non-blocking in-act ambient banter area. `AmbientBanterHook` (runtime scene-load hook) spawns
`AmbientBanterPanel` fed from per-act `BanterData`; the panel cycles/loops Lily+Ghost lines, addresses
the player by name, and uses per-act sizing. Lily warms Act 1→Act 3 with a nerdy-joke-then-embarrassed
beat; Ghost stays story-consistent. Frontend, static text; puzzles unchanged.

## Files Created

- `Assets/Presentation/Banter/AmbientBanterHook.cs`, `AmbientBanterPanel.cs`, `BanterData.cs`
- `Docs/codex_runs/M0-T32_001_in_act_ambient_banter.md`, `M0-T32_002_embed_and_expand_ambient_banter.md`,
  `M0-T32_003_banter_text_box_sizing.md`

## Files Modified

- `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`

## Claude Review Notes

- Scope clean: git status + the run log's prohibited-scope diff confirm only the Banter folder + docs
  changed — no puzzle logic/validators/sessions, Act 1/2/3 presenters, scenes, ProjectSettings,
  Packages, Build Settings, or `.meta` (beyond generated).
- Non-blocking flavour layer; deterministic puzzles untouched; static text (no LLM).
- Run logs honest ("Not run" for Unity in-session); all three retained.

## Human Verification Result

The user verified the banter across iterations and accepted the per-act sizing ("先暫時這樣"); the
puzzles remain playable.

## Remaining Risks

- Banter box sizing is resolution/font-dependent and may want later polish.
- Lines are static; future work adds player choices + LLM (deferred).

## Next Task

M0-T27 — Backend + database foundation (ROADMAP Phase D / `VERTICAL_SLICE_PLAN.md` §B): a local
Node.js + TypeScript REST service with SQLite (content / progress / attempts). LLM endpoints deferred
to M0-T29; Unity client integration is M0-T28. Deterministic puzzle correctness stays client-side.
