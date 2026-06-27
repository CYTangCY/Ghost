# M0-T28 — Run 002 — account name recovery

## Task ID

M0-T28

## Run Number

002

## Date

2026-06-26

## Original Request / Codex Prompt Summary

Add a first prototype account-recovery version without passwords. The user wants an account name / username-only flow first, with password authentication deferred, plus Claude review guidance and instructions for using it from both CMD/API and Unity UI. Scope was kept to backend profile recovery and shell UI wiring; deterministic puzzle validators and gameplay rules were not changed.

## Files Created

- `Docs/codex_runs/M0-T28_002_account_name_recovery.md`

## Files Modified

- `Backend/src/database.ts`
- `Backend/src/app.ts`
- `Backend/tests/app.test.ts`
- `Backend/README.md`
- `Assets/Presentation/Backend/GhostBackendClient.cs`
- `Assets/Presentation/Shell/GhostNarrativeState.cs`
- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `npm run build` from `Backend/` attempted through PowerShell `npm`; blocked by Windows execution policy for `npm.ps1`.
- `npm test` from `Backend/` attempted through PowerShell `npm`; blocked by Windows execution policy for `npm.ps1`.
- `npm.cmd run build` from `Backend/` attempted in sandbox; TypeScript ran but failed to write `Backend/dist/*.js` due sandbox EPERM.
- `npm.cmd test` from `Backend/` attempted in sandbox; Vitest executed tests but failed to write its results cache due sandbox EPERM.
- `npm.cmd run build` from `Backend/` rerun with escalated permissions.
- `npm.cmd test` from `Backend/` rerun with escalated permissions.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `npm.cmd run build`: passed.
- `npm.cmd test`: passed, 1 test file, 9 tests.
- New backend tests cover:
  - account creation linked to an existing profile;
  - lookup by username;
  - lookup by generated account id;
  - progress recovery through the looked-up profile id;
  - duplicate username rejection.
- Unity compile / Play Mode verification still requires the Unity Editor.

## Errors Encountered

- PowerShell blocked `npm.ps1` due execution policy.
- Sandboxed backend build could not write `Backend/dist/*.js`.
- Sandboxed Vitest could not write `Backend/node_modules/.vite/vitest/results.json`.

## Fixes Applied

- Used `npm.cmd` instead of PowerShell's `npm.ps1` shim.
- Reran backend build/tests with escalated permissions so TypeScript output and Vitest cache could be written.
- Kept password authentication out of scope and documented the security limitation.

## What Was Intentionally Not Changed

- No deterministic puzzle validators, sessions, or rules.
- No `Assets/Scripts/Puzzles/` files.
- No ProjectSettings, Packages, Build Settings, `.meta` files, or hand-written scene YAML.
- No password storage, password hashing, token authentication, or secure login flow.
- No backend scoring endpoint; correctness remains client-side deterministic.
- `Assets/Scenes/GameShellPrototype.unity` was already modified in the working tree before this run and was not hand-edited by this task.

## Remaining Risks

- This is not secure authentication. Anyone who knows the username or generated account id can recover that prototype profile on the same backend.
- Unity UI must be regenerated with `Ghost > Build Game Shell Scene` before the new account controls appear in the shell scene.
- Unity Play Mode has not been run in this Codex session, so UI wiring and layout need human Editor verification.
- Switching accounts during one live session replaces local completed-act state; this is intended for recovery but should be tested carefully.

## Next Recommended Step

Run `Ghost > Build Game Shell Scene`, start the backend, verify create/use account from the Unity shell UI, and then ask Claude to review whether this should be accepted as a prototype-only no-password recovery feature or advanced into a later password/auth task.
