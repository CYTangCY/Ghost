# M0-T28 — Run 005 — account entry gate fix

## Task ID

M0-T28

## Run Number

005

## Date

2026-06-26

## Original Request / Codex Prompt Summary

Fix the shell flow because the user reported that the game went directly to the play/hub interface using an old character name, so the account creation/use options were never shown.

## Files Created

- `Docs/codex_runs/M0-T28_005_account_entry_gate_fix.md`

## Files Modified

- `Assets/Presentation/Shell/GameShellPresenter.cs`

## Tests or Checks Run

- Inspected `GameShellPresenter.ShowNameEntryOrHub()` and the account field preparation path.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.
- Backend tests: Not run — backend behaviour was not changed in this shell-flow fix.

## Test / Check Result

Not run — Unity Editor Play Mode was not executed in this Codex session.

## Errors Encountered

- `ShowNameEntryOrHub()` skipped the name/account screen whenever `GhostNarrativeState.HasPlayerName` was true. This was correct before account recovery existed, but now it hides the account controls for returning players with an old saved name.

## Fixes Applied

- Changed `ShowNameEntryOrHub()` so it shows the name/account entry screen whenever that screen exists.
- Kept the direct-to-hub fallback only for old scenes that have no name-entry screen wired.
- Pre-fills the player-name input with `GhostNarrativeState.PlayerName` so returning players still see their old name before choosing guest/account options.

## What Was Intentionally Not Changed

- No backend endpoint behaviour.
- No puzzle logic, validators, sessions, or gameplay rules.
- No ProjectSettings, Packages, Build Settings, scenes, or `.meta` files.
- Password/authentication remains out of scope.

## Remaining Risks

- Unity Editor must recompile and Play Mode must confirm the shell now shows the account controls before entering the hub.
- If the active scene has not been regenerated with `Ghost > Build Game Shell Scene`, the new account controls still will not exist in the scene.

## Next Recommended Step

Return to Unity, wait for compilation, run `Ghost > Build Game Shell Scene`, then enter Play Mode and confirm `Start / Continue` opens the name/account screen even when an old player name already exists.
