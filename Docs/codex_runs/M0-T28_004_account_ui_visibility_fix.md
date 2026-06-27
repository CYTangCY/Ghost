# M0-T28 — Run 004 — account UI visibility fix

## Task ID

M0-T28

## Run Number

004

## Date

2026-06-26

## Original Request / Codex Prompt Summary

Fix the no-password account UI because the user reported that there was no way to create an account and no option buttons were visible in the shell.

## Files Created

- `Docs/codex_runs/M0-T28_004_account_ui_visibility_fix.md`

## Files Modified

- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- Inspected the shell scene builder name-entry layout.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.
- Backend tests: Not run — backend behaviour was not changed in this UI layout fix.

## Test / Check Result

Not run — Unity Editor Play Mode was not executed in this Codex session.

## Errors Encountered

- The account controls were added below the existing name-entry content in a single vertical layout. On the shell screen this could push `Create Account` / `Use Account` outside the visible area, or make the old scene appear unchanged until the builder is rerun.

## Fixes Applied

- Changed `GameShellSceneBuilder.CreateNameEntryScreen(...)` to generate a compact two-column layout:
  - left column: display name + `Continue as Guest`;
  - right column: account identifier + `Create Account` / `Use Account` + status text.
- Updated docs/checklist to explicitly require rerunning `Ghost > Build Game Shell Scene` and confirming all three buttons are visible.

## What Was Intentionally Not Changed

- No backend endpoint behaviour.
- No puzzle logic, validators, sessions, or gameplay rules.
- No ProjectSettings, Packages, Build Settings, `.meta` files, or hand-written scene YAML.
- Password/authentication remains out of scope.

## Remaining Risks

- The existing `Assets/Scenes/GameShellPrototype.unity` will not show the new generated controls until the user runs `Ghost > Build Game Shell Scene`.
- Unity layout still needs human Editor verification at the target resolution.

## Next Recommended Step

In Unity, run `Ghost > Build Game Shell Scene`, open `Assets/Scenes/GameShellPrototype.unity`, enter Play Mode, and confirm the name-entry screen shows `Continue as Guest`, `Create Account`, and `Use Account`.
