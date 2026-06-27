# M0-T28 — Run 006 — account button layout fix

## Task ID

M0-T28

## Run Number

006

## Date

2026-06-26

## Original Request / Codex Prompt Summary

Fix the account UI because the user could see the optional account input field but not the `Create Account` / `Use Account` buttons; only clipped text such as `CU` appeared.

## Files Created

- `Docs/codex_runs/M0-T28_006_account_button_layout_fix.md`

## Files Modified

- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`

## Tests or Checks Run

- Inspected `GameShellSceneBuilder.CreateNameEntryScreen(...)` and `CreateButtonRow(...)`.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.
- Backend tests: Not run — backend behaviour was not changed in this UI layout fix.

## Test / Check Result

Not run — Unity Editor Play Mode was not executed in this Codex session.

## Errors Encountered

- The account button row used `HorizontalLayoutGroup.childControlWidth = false`, so Unity did not apply the child buttons' `LayoutElement` preferred widths. The buttons collapsed to near-zero width and only clipped label fragments were visible.

## Fixes Applied

- Changed `CreateButtonRow(...)` to use `childControlWidth = true`, allowing the horizontal layout group to size the `Create Account` and `Use Account` buttons from their layout elements.

## What Was Intentionally Not Changed

- No backend endpoint behaviour.
- No puzzle logic, validators, sessions, or gameplay rules.
- No ProjectSettings, Packages, Build Settings, scenes, or `.meta` files.
- Password/authentication remains out of scope.

## Remaining Risks

- The shell scene must be regenerated with `Ghost > Build Game Shell Scene` before the fixed generated layout appears.
- Unity Play Mode still needs human verification.

## Next Recommended Step

Run `Ghost > Build Game Shell Scene`, enter Play Mode, click `Start / Continue`, and confirm the optional account panel shows both `Create Account` and `Use Account` as full buttons.
