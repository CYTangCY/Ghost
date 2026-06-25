# M0-T29 — Run 001 — llm orchestration

## Task ID

M0-T29

## Run Number

001

## Date

2026-06-25

## Original Request / Codex Prompt Summary

Implement LLM orchestration for Lily hints and Ghost responses through the local Node.js/TypeScript backend using Ollama + an IBM Granite model, while preserving deterministic Unity validators as the only source of puzzle correctness. Add static fallback behaviour, Unity client hint requests, docs, tests, and setup instructions.

## Files Created

- `Backend/src/ollamaClient.ts`
- `Backend/src/llmOrchestration.ts`
- `Backend/src/checkOllama.ts`
- `Docs/codex_runs/M0-T29_001_llm_orchestration.md`

## Files Modified

- `Backend/package.json`
- `Backend/README.md`
- `Backend/src/app.ts`
- `Backend/src/database.ts`
- `Backend/tests/app.test.ts`
- `Assets/Presentation/Backend/GhostBackendClient.cs`
- `Assets/Presentation/Banter/AmbientBanterPanel.cs`
- `Assets/Presentation/Banter/AmbientBanterHook.cs`
- `Assets/Presentation/Banter/BanterData.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `npm.cmd install` in `Backend/`
- `npm.cmd run build` in `Backend/`
- `npm.cmd test` in `Backend/`
- `npm.cmd run check:ollama` in `Backend/`
- Static scan for forbidden Unity client APIs: `System.Net.Http`, `HttpClient`, `Thread`, `Task.Run`
- Scoped diff checks for `Assets/Scripts`, `ProjectSettings`, `Packages`, `Assets/Scenes`, and `.meta` files
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `npm.cmd install` passed after rerun with local write access: packages were already up to date; npm reported 6 existing audit vulnerabilities (4 moderate, 1 high, 1 critical).
- `npm.cmd run build` passed after rerun with local write access.
- `npm.cmd test` passed after rerun with local write access: 1 test file passed, 5 tests passed.
- `npm.cmd run check:ollama` was run and failed because local Ollama was not reachable: `fetch failed`. No live Granite generation call succeeded in this session.
- Static Unity API scan found no `System.Net.Http`, `HttpClient`, `Thread`, or `Task.Run` usage in the scoped M0-T29 Unity files.
- Scoped diff checks found no M0-T29 changes under `Assets/Scripts`, `ProjectSettings`, `Packages`, `Assets/Scenes`, or `.meta` files.

## Errors Encountered

- Initial sandboxed `npm.cmd run build` failed with EPERM when writing `Backend/dist`.
- Initial sandboxed `npm.cmd install` failed with EPERM when writing under `Backend/node_modules`.
- Initial sandboxed `npm.cmd test` passed the tests but exited with EPERM when Vitest wrote `node_modules/.vite/vitest/results.json`.
- `npm.cmd run check:ollama` could not reach local Ollama at `http://localhost:11434`.

## Fixes Applied

- Added a fetch-based Ollama client with env configuration for `OLLAMA_URL`, `OLLAMA_MODEL`, and `OLLAMA_TIMEOUT_MS`.
- Implemented curriculum-aware `/hints` and `/responses` orchestration with non-spoiler prompts and static fallback.
- Added hint logging to `hint_logs`, using an anonymous local profile when no valid profile id is supplied.
- Added `npm run check:ollama`.
- Updated backend tests to cover the no-live-LLM fallback path for `/hints` and `/responses`.
- Extended the Unity backend client with best-effort `PostHint(...)` and optional `PostResponse(...)`.
- Reused the ambient banter panel as the in-act Lily hint display, with an `Ask Lily` button and local static fallback.
- Routed incorrect Validate outcomes in Acts 1-3 to request a non-spoiler Lily hint without changing validation logic.
- Documented Ollama/Granite setup, backend endpoints, Unity test steps, and fallback behaviour.

## What Was Intentionally Not Changed

- No deterministic validators, sessions, or puzzle rules were changed.
- No files under `Assets/Scripts/Puzzles/` were changed.
- No ProjectSettings, Packages, Build Settings, scenes, `.meta` files, or asmdefs were changed.
- No backend scoring endpoint was added.
- The LLM is not used to decide correctness or progression.
- No cloud hosting, model fine-tuning, Act 8 capstone, or backend-served puzzle replacement was added.

## Remaining Risks

- Unity compile and Play Mode behaviour still require human Editor verification.
- The local Ollama/Granite live path was not verified because Ollama was unreachable in this session.
- Existing npm audit vulnerabilities remain; they were not fixed because forced dependency changes are outside M0-T29.
- The `Ask Lily` button is generated at runtime in existing banter panel layouts and should be visually checked in all three acts.

## Next Recommended Step

Install/start Ollama, run `ollama pull granite3.1-dense:2b`, verify with `npm run check:ollama`, then run the backend and test `Ask Lily` in Unity Play Mode with Ollama up, Ollama down, and backend down.
