# M0-T28 — Unity ↔ Backend Integration

## Completion Status

Completed. The user verified in the Editor (backend on: profile + progress persist across Play
sessions, attempts logged; backend off: full play, graceful degradation) — "過了". Claude confirmed
each feature's wiring by reading the code and ran the backend build/test (4/4) after the CORS change.

## Date

2026-06-24

## Summary

Wired the Unity client to the M0-T27 backend. `GhostBackendClient` (UnityWebRequest coroutines,
best-effort, timeouts), `BackendSync` (EnsureProfile → GetProgress → apply to `GhostNarrativeState`;
save on `StateChanged`), pseudonymous profile id in PlayerPrefs, best-effort attempt logging from the
Act 1/2/3 validation paths, and graceful degradation (offline → in-memory fallback, warning-only). The
backend `app.ts` gained dev-only CORS (with `OPTIONS`→204). Deterministic correctness unchanged.

## Files Created / Modified

- Created `Assets/Presentation/Backend/GhostBackendConfig.cs`, `GhostBackendClient.cs`, `BackendSync.cs`
- Modified `Assets/Presentation/Shell/GhostNarrativeState.cs`, `GameShellPresenter.cs`, and the Act
  1/2/3 interaction controllers (best-effort `PostAttempt`)
- Modified `Backend/src/app.ts` (dev CORS + OPTIONS 204)
- Modified `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`; created the run log

## Claude Review Notes (each feature confirmed)

- Profile create/reuse ✓ (`EnsureProfile` + PlayerPrefs); progress load ✓ and save-on-change ✓ (with
  an `isApplyingBackendProgress` echo-loop guard); attempt logging in all three acts ✓ (Act1/Act2/Act3
  controllers); graceful degradation ✓ (timeouts + `Debug.LogWarning` + callback, no throw/block);
  WebGL-safe ✓ (UnityWebRequest + coroutines); CORS ✓ (verified build+test+OPTIONS 204 by running it).
- Correctness stays client-side: attempt logging is fire-and-forget after validation; validators/
  sessions/puzzle rules unchanged.
- Scope clean: Backend client + shell/controllers + `app.ts` CORS + 2 docs; no
  ProjectSettings/Packages/Build Settings/scenes/.meta.

## Human Verification Result

The user verified backend-on (profile + progress persist across Play sessions; attempts logged) and
backend-off (game fully playable, no hangs) in the Editor — "過了".

## Remaining Risks

- Profile id is per-device PlayerPrefs (no accounts/auth); attempts have no GET endpoint (verify via the
  SQLite DB); cross-Play-session persistence relies on Editor domain reload resetting statics.
- LLM not yet wired (`/hints`, `/responses` still 501) — M0-T29.

## Next Task

M0-T29 — LLM orchestration: implement `/hints` and `/responses` via the backend (IBM Granite via
Ollama, local), with a curriculum-aware prompt that never reveals the solution, hint logging, and a
static-hint fallback. The LLM only generates language — it never decides puzzle correctness.
