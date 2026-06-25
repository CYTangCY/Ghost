# M0-T28 — Run 001 — client backend integration

## Task ID

M0-T28

## Run Number

001

## Date

2026-06-24

## Original Request / Codex Prompt Summary

Wire the Unity client to the M0-T27 local backend for pseudonymous profile creation/reuse, backend progress persistence, and attempt logging across Acts 1-3. Keep all calls WebGL-safe through UnityWebRequest coroutines, time-bounded, and best-effort so the game remains fully playable when the backend is unavailable. Do not change deterministic validators, sessions, puzzle rules, LLM, backend scoring, or Unity client content loading.

## Files Created

- `Assets/Presentation/Backend/GhostBackendConfig.cs`
- `Assets/Presentation/Backend/GhostBackendClient.cs`
- `Assets/Presentation/Backend/BackendSync.cs`
- `Docs/codex_runs/M0-T28_001_client_backend_integration.md`

## Files Modified

- `Assets/Presentation/Shell/GhostNarrativeState.cs`
- `Assets/Presentation/Shell/GameShellPresenter.cs`
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
- `Backend/src/app.ts`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `npm.cmd run build` in `Backend/`
- `npm.cmd test` in `Backend/`
- Static scan for forbidden client networking/threading APIs in the new/modified Unity presentation files.
- Scoped diff check for `Assets/Scripts`, `ProjectSettings`, and `Packages`.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- First sandbox `npm.cmd run build` failed with EPERM writing `Backend/dist`; rerun with local file-write access passed.
- First sandbox `npm.cmd test` ran 4 tests successfully but exited with EPERM writing Vitest cache; rerun with local file-write access passed: 1 test file passed, 4 tests passed.
- Static scan found no `System.Net.Http`, `HttpClient`, `Thread`, or `Task.Run` usage in the M0-T28 client integration files.
- Scoped diff check showed no M0-T28 changes under `Assets/Scripts`, `ProjectSettings`, or `Packages`.

## Errors Encountered

- Sandbox write restrictions prevented TypeScript build output and Vitest cache writes until rerun with local write access.
- Unity Play Mode and Unity Test Runner could not be run from this Codex session.

## Fixes Applied

- Added `GhostBackendConfig` for configurable backend URL and short request timeout.
- Added `GhostBackendClient` with UnityWebRequest coroutine methods for profile, progress, and attempt endpoints.
- Added `BackendSync` to ensure a profile on shell/game start, fetch progress, and push progress after narrative state changes.
- Extended `GhostNarrativeState` with PlayerPrefs-backed backend profile id, completed-act snapshots, backend progress merge, and a state-change event.
- Started backend sync from `GameShellPresenter.Start()`.
- Added best-effort attempt logging after deterministic validation in Act 1, Act 2, and Act 3 interaction controllers.
- Added a minimal permissive local-dev CORS middleware to `Backend/src/app.ts` so WebGL/browser builds can call the local backend without an extra package.

## What Was Intentionally Not Changed

- No deterministic validators, sessions, or puzzle rules were changed.
- No files under `Assets/Scripts/Puzzles/` were changed.
- No ProjectSettings, Packages, Build Settings, scenes, `.meta` files, or asmdefs were changed.
- No LLM calls were added.
- No backend scoring endpoint was added.
- The Unity client still uses local sample data; backend content loading is intentionally deferred.

## Remaining Risks

- Unity compile and Play Mode behaviour still require human Editor verification.
- Backend warnings will appear when the server is down; this is intentional graceful degradation, but the user should confirm it does not feel noisy.
- A stale PlayerPrefs profile id from a deleted backend database can cause best-effort progress/attempt calls to fail until the profile id is cleared.
- Progress sync is intentionally minimal: it persists player name and completed acts/derived level ids, not full save/load state.

## Next Recommended Step

Run the backend locally with `npm run dev`, then verify in Unity Play Mode that profile creation/reuse, progress reload, and attempt logging work with the backend up, and that Acts 1-3 remain fully playable with the backend down. If accepted, ask Claude to review and archive as `M0-T28_client_backend_integration.md`.
