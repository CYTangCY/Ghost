# CURRENT_TASK.md

## ID

M0-T28

## Goal

Wire the Unity client to the M0-T27 backend for pseudonymous profile + player-progress persistence +
attempt logging across Acts 1–3, behind **graceful degradation** (NFR5): if the backend is unavailable,
the game keeps working on the current local/in-memory behaviour with no broken play. Deterministic
puzzle correctness stays client-side — the backend only stores.

## Context

M0-T27 stood up the backend (content/profiles/progress/attempts; `/hints`+`/responses` are M0-T29
stubs). This connects Unity to it so progress survives across sessions (today `GhostNarrativeState` is
in-memory only) and puzzle attempts are logged for analytics. Per `VERTICAL_SLICE_PLAN.md` §B and the
graceful-degradation rule. Backend base URL is local (configurable).

## Scope

- A small Unity API client (UnityWebRequest, WebGL-safe) for the backend endpoints (`POST /profiles`,
  `GET/PUT /progress/:id`, `POST /attempts`; `GET /content` optional this task), with timeouts and
  best-effort failure handling.
- Pseudonymous profile: create once and persist the id locally (e.g. PlayerPrefs); reuse thereafter.
- Progress: load on shell start and save on act completion / narrative-state change (extend
  `GhostNarrativeState` to sync to the backend without changing its in-memory fallback).
- Attempt logging: when a puzzle's Validate runs in any of Acts 1–3, best-effort `POST /attempts`
  (act id + result + brief details) from the existing validation path — WITHOUT changing the
  deterministic validators or puzzle rules.
- Graceful degradation (NFR5): every backend call is best-effort and time-bounded; on failure/offline,
  the game uses the current local behaviour. Backend base URL configurable; gameplay never blocks on the
  network.
- Update CODE_WALKTHROUGH.md + UNITY_TEST_CHECKLIST.md; create a Codex run log.

## Out of Scope

- LLM (M0-T29); replacing local sample-data with backend-served content (optional/later);
  authentication; deployment/hosting; moving any correctness to the backend.
- Do not change the deterministic validators/sessions or puzzle rules.

## Acceptance Criteria

- With the backend running: a pseudonymous profile is created/reused; progress persists across sessions
  (saved then reloaded); attempts are logged (visible in the backend DB / attempts).
- With the backend stopped: the game still plays fully (graceful degradation) with no blocking errors.
- Deterministic validators and puzzle rules unchanged; correctness remains client-side.
- No Console errors in normal play; CODE_WALKTHROUGH.md + UNITY_TEST_CHECKLIST.md updated; a Codex run
  log created.
