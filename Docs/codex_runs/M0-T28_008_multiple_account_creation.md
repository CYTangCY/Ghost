# M0-T28 — Run 008 — multiple account creation

## Task ID

M0-T28

## Run Number

008

## Date

2026-06-26

## Original Request / Codex Prompt Summary

Change the no-password account prototype so multiple accounts can coexist. Creating a new account should not overwrite or rename the account already linked to the current local profile.

## Files Created

- `Docs/codex_runs/M0-T28_008_multiple_account_creation.md`

## Files Modified

- `Backend/src/database.ts`
- `Backend/tests/app.test.ts`
- `Backend/README.md`
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
- Backend test now confirms that creating a new username while the current profile already has an account creates a separate account/profile and leaves the original account lookup intact.

## Errors Encountered

- Previous run 007 allowed a current profile's account to be renamed. That avoided a 409 but did not match the user's expectation that multiple accounts should coexist.

## Fixes Applied

- Updated `GhostDatabase.createAccount(...)`:
  - if the requested username already belongs to the current profile, return that existing account;
  - if the requested username belongs to another profile, return `account_exists`;
  - if the current profile already has a different account and the requested username is unused, create a new profile and new account instead of overwriting the old account.
- Updated tests and docs to describe multiple-account coexistence.

## What Was Intentionally Not Changed

- No password authentication, password hashing, token/session security, or account deletion UI.
- No puzzle logic, validators, sessions, or gameplay rules.
- No ProjectSettings, Packages, Build Settings, scenes, or `.meta` files.
- No backend scoring endpoint.

## Remaining Risks

- This remains no-password prototype account recovery. Anyone with a username/account id can recover that profile from the same backend.
- Unity Play Mode still needs human verification.
- Creating a new account while already on another account switches Unity to the new account and then pushes the current local state to that new profile. This preserves demo continuity but is not a final multi-user account design.

## Next Recommended Step

Restart the backend, create two different usernames from Unity or REST, then verify in SQLite that both rows still exist and have different profile ids.
