# M0-T29 — Run 002 — llm timeout ui logging fix

## Task ID

M0-T29

## Run Number

002

## Date

2026-06-25

## Original Request / Codex Prompt Summary

Fix the M0-T29 LLM hint path so local Ollama/Granite does not always fall back to static text, update Ask Lily so hints replace the ambient banter in the same frame rather than overlapping it, and enrich `hint_logs` with trigger/question context and non-spoiler client state.

## Files Created

- `Docs/codex_runs/M0-T29_002_llm_timeout_ui_logging_fix.md`

## Files Modified

- `Backend/src/ollamaClient.ts`
- `Backend/src/llmOrchestration.ts`
- `Backend/src/checkOllama.ts`
- `Backend/src/database.ts`
- `Backend/tests/app.test.ts`
- `Backend/README.md`
- `Assets/Presentation/Backend/GhostBackendClient.cs`
- `Assets/Presentation/Banter/AmbientBanterPanel.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `npm.cmd run build` in `Backend/`
- `npm.cmd test` in `Backend/`
- `npm.cmd run check:ollama` in `Backend/`
- Static call-site scan for `PostHint(...)` and `RequestHint(...)`
- Scoped diff checks for pure puzzle logic, ProjectSettings, Packages, scenes, `.meta`, CURRENT_TASK, HANDOFF_LOG, and completed task files
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `npm.cmd run build` passed.
- `npm.cmd test` passed: 1 test file passed, 6 tests passed. The expected fallback tests emitted backend `console.warn` lines showing the configured Ollama URL/model and test error.
- `npm.cmd run check:ollama` passed on the final run: model `granite3.1-dense:2b` was available and timed test generation succeeded in 6956 ms.
- An earlier `npm.cmd run check:ollama` attempt found the model but generation aborted after the 60000 ms timeout; after improving timeout diagnostics and rerunning with the warmed model, generation succeeded.
- Static call-site scan found the updated `PostHint(...)` and `RequestHint(...)` signatures aligned.
- Scoped diff checks showed existing unrelated dirty files under `Assets/Scenes/GameShellPrototype.unity`, `Docs/CURRENT_TASK.md`, and `Docs/HANDOFF_LOG.md`; they were not edited in this run.

## Errors Encountered

- The first live `check:ollama` generation attempt timed out with `This operation was aborted`, even though `/api/tags` reported the Granite model was available.
- The previous `check:ollama` output could mislead developers by suggesting a model pull even after the model had already been found.
- The Unity client had a short default backend request timeout, which could make long local LLM generations fail client-side even after the backend timeout was raised.

## Fixes Applied

- Split Ollama timeout configuration into a 60000 ms default `/api/generate` timeout and a 5000 ms default `/api/tags` check timeout.
- Converted aborts into clear timeout errors, and added explicit model-not-found messaging with the exact `ollama pull <model>` command.
- Updated `npm run check:ollama` to list/check the model and then run a timed generation probe.
- Added backend fallback warnings that include the Ollama URL, model, and actual error without exposing answer keys.
- Added `trigger` to `/hints` payload handling and logged `trigger` plus summarized non-spoiler state in `hint_logs`.
- Added a database test helper for reading the latest hint-log payload.
- Expanded backend tests to cover static fallback logging and a mocked successful LLM hint path with trigger/state logging.
- Updated Unity `GhostBackendClient.PostHint(...)` to send `trigger` and `state.summary`; LLM hint/response requests use a longer non-blocking timeout.
- Updated `AmbientBanterPanel` so Ask Lily pauses the ambient loop, shows an in-flight "Asking Lily..." state, replaces the current line with the hint in the same frame, and uses `Back` to resume the loop.
- Updated Act 1, Act 2, and Act 3 presentation controllers to request hints after incorrect Validate with the `after_incorrect_validate` trigger and an error-count summary.
- Updated README, code walkthrough, and Unity checklist with the timeout, check command, no-overlap Ask Lily UX, and richer hint logging.

## What Was Intentionally Not Changed

- No deterministic validators, sessions, or puzzle rules were changed.
- No files under `Assets/Scripts/Puzzles/` were changed.
- No ProjectSettings, Packages, Build Settings, scenes, `.meta` files, or asmdefs were edited in this run.
- No backend scoring endpoint was added.
- The LLM still never decides correctness, gates progression, or receives answer keys.
- No cloud hosting, fine-tuning, save/load changes, or Act 8 capstone work was added.

## Remaining Risks

- Unity compile and Play Mode behaviour still require human Editor verification.
- Live Ollama succeeded after the model was warmed; first generation on a slower/cold machine may still need `OLLAMA_TIMEOUT_MS` above 60000.
- The new single-frame Ask Lily flow should be visually checked in all three acts to confirm no layout overflow and that `Back` resumes the intended banter line.
- Existing unrelated dirty files remain in the worktree from earlier tasks/runs.

## Next Recommended Step

Run the backend with Ollama up, enter each act in Unity Play Mode, click Ask Lily and trigger incorrect Validate, then inspect `hint_logs` for `ask_lily_button` and `after_incorrect_validate` rows with non-spoiler state summaries. Repeat with Ollama down and backend down to confirm static fallback and uninterrupted gameplay.
