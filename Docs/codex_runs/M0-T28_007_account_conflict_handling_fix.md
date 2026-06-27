# M0-T28 — Run 007 — account conflict handling fix

## Task ID

M0-T28

## Run Number

007

## Date

2026-06-26

## Original Request / Codex Prompt Summary

Fix the no-password account creation flow after the user showed that `Create Account` returned `409 Conflict`, preventing account creation from the Unity shell even though the account UI was visible.

## Files Created

- `Docs/codex_runs/M0-T28_007_account_conflict_handling_fix.md`

## Files Modified

- `Backend/src/database.ts`
- `Backend/tests/app.test.ts`
- `Backend/README.md`
- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `npm.cmd run build` from `Backend/`.
- `npm.cmd test` from `Backend/`.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `npm.cmd run build`: passed.
- `npm.cmd test`: passed, 1 test file, 10 tests.
- New backend test confirms a profile that already has an account can rename/update its account username when the new username is unused.

## Errors Encountered

- The backend returned `409 Conflict` both when a username was genuinely taken and when the current profile already had an account. This made Unity's `Create Account` path feel broken after a tester had already created or partially linked an account.

## Fixes Applied

- Updated `GhostDatabase.createAccount(...)`:
  - if the requested username already belongs to the same profile, return that account;
  - if the current profile already has an account and the requested username is unused, update/rename the existing account;
  - if the requested username belongs to another profile, keep returning `409`.
- Added backend test coverage for account renaming.
- Improved Unity account creation error text for duplicate usernames.
- Updated README, code walkthrough, and test checklist.

## What Was Intentionally Not Changed

- No password authentication, password hashing, or token/session security.
- No puzzle logic, validators, sessions, or gameplay rules.
- No ProjectSettings, Packages, Build Settings, scenes, or `.meta` files.
- No backend scoring endpoint; correctness remains deterministic in Unity.

## Remaining Risks

- This remains no-password prototype account recovery. A username/account id is enough to recover the profile on the same backend.
- Unity Play Mode still needs human verification with the backend running.
- If a username is already taken by another profile, the user must click `Use Account` for that username or choose a different username.

## Next Recommended Step

Restart the backend, rerun `Ghost > Build Game Shell Scene` if needed, and test `Create Account` again. If the username already exists for another profile, use `Use Account` or pick a different username.
