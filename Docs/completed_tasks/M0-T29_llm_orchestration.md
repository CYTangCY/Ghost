# M0-T29 — LLM Orchestration (Lily hints / Ghost responses)

## Completion Status

Completed (Codex runs 001 initial, 002 timeout/UI/logging fix). The LLM path is verified working by the
user: after raising the Ollama timeout and killing a stale backend, `hint_logs` shows `source:"llm"`,
`trigger:"ask_lily_button"`, `error:null`. Claude reviewed the orchestration (no answer-key, no scoring)
and relied on the user's live DB evidence.

## Date

2026-06-25

## Summary

Backend `/hints` + `/responses` call Ollama (IBM Granite via local Ollama) with a curriculum-aware,
no-spoiler system prompt; static-hint fallback when Ollama/backend is unavailable; `hint_logs` records
kind/source/trigger/level/state/error; the LLM never decides correctness/scoring. Run 002 fixed the root
cause of always-static (the Ollama request timeout was 5s → raised to 60s, configurable), added a
fallback warning (real error + model + URL), a timed `check:ollama`, the Ask-Lily-replace-in-panel UX,
and `trigger` logging. Unity: `GhostBackendClient.PostHint` + an Ask Lily affordance + an
after-incorrect-validate hint, all graceful.

## Files Created / Modified

- Backend: `src/ollamaClient.ts`, `src/llmOrchestration.ts`, `src/checkOllama.ts`, `src/database.ts`,
  `src/app.ts`, `tests/app.test.ts`, `README.md`
- Unity: `Assets/Presentation/Backend/GhostBackendClient.cs`, `Assets/Presentation/Banter/AmbientBanterPanel.cs`,
  Act 1/2/3 interaction controllers
- Docs: `CODE_WALKTHROUGH.md`, `UNITY_TEST_CHECKLIST.md`; run logs `M0-T29_001…`, `M0-T29_002…`

## Claude Review Notes

- Deterministic rule upheld: system prompt forbids revealing answers/solutions and forbids deciding
  correctness; `getContent`/`getLearningContentSummary` omit answer keys; validators unchanged; static
  fallback always present.
- Root cause of "always static" (5s timeout aborting Granite generation) fixed → 60s; verified live
  (`source:"llm"`). Operational note: a stale backend on `:3000` must be killed before restart for new
  settings to take effect (this was masking the fix during testing).

## Human Verification Result

The user confirmed via the live DB that hints now come from the LLM (`source:"llm"`, `trigger` logged,
`error:null`) after killing the stale server and raising the timeout.

## Superseded / Follow-up (→ M0-T33)

The run-002 "Ask Lily replaces the ambient panel" UX is **superseded** by a new requirement: a dedicated
Lily **chat window** with free-text input, short in-character replies, and topic guardrails. Plus the
ambient banter UI needs sizing fixes (Act 2 box too large; fixed-reply position). Captured in M0-T33.

## Next Task

M0-T33 — Lily chat: free-text input in a dedicated chat window, one-sentence in-character replies via
the LLM, persona/topic guardrails (on-topic only; private-life → flustered deflection; off-topic →
redirect to the task; never reveal answers/score), plus ambient banter UI fixes.
