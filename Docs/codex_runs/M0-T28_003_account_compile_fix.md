# M0-T28 — Run 003 — account compile fix

## Task ID

M0-T28

## Run Number

003

## Date

2026-06-26

## Original Request / Codex Prompt Summary

Fix the Unity compile error reported after the no-password account recovery implementation. Unity reported CS0411 in `Assets/Presentation/Backend/GhostBackendClient.cs` because the type arguments for `GhostBackendClient.SendRequest<T>(...)` could not be inferred.

## Files Created

- `Docs/codex_runs/M0-T28_003_account_compile_fix.md`

## Files Modified

- `Assets/Presentation/Backend/GhostBackendClient.cs`

## Tests or Checks Run

- Inspected the reported line in `GhostBackendClient.cs`.
- Searched `GhostBackendClient.cs` for `SendRequest(...)` call sites.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

The two account-related `SendRequest(...)` calls now explicitly specify `SendRequest<AccountResponse>(...)`, so C# no longer needs to infer the generic response type from the lambda.

## Errors Encountered

- Unity compile error: CS0411, generic type argument inference failed for `SendRequest<T>` in `LookupAccount(...)`.

## Fixes Applied

- Added explicit `<AccountResponse>` generic type arguments to both account request wrappers:
  - `CreateAccount(...)`
  - `LookupAccount(...)`

## What Was Intentionally Not Changed

- No puzzle logic, validators, sessions, or gameplay rules.
- No backend endpoint behaviour.
- No ProjectSettings, Packages, Build Settings, scenes, or `.meta` files.
- Existing CS0618 warnings about `Object.FindFirstObjectByType<T>()` were not changed because they are warnings and unrelated to this compile blocker.

## Remaining Risks

- Unity Editor must recompile to confirm no further C# compile errors appear.
- Play Mode account UI still needs human verification after the shell scene is regenerated.

## Next Recommended Step

Return to Unity, wait for scripts to recompile, then run `Ghost > Build Game Shell Scene` and test the no-password account flow with the backend running.
