# M0-T33 — Lily Chat (constrained, free-text)

## Completion Status

Completed (Codex run 001). Claude verified the backend/LLM path LIVE; the Unity chat window was reviewed
by reading. The user indicated the chat is done.

## Date

2026-06-25

## Summary

A dedicated Lily chat window with free-text input, replacing the overlap behaviour. `LilyChatWindow`
(separate screen-space overlay, NOT the ambient banter box): input field + Send + Close + scrollable
history (≤10 turns); opens on Ask Lily, calls `AmbientBanterPanel.PauseForChat()` on open and
`ResumeAfterChat()` on Close. Backend `POST /chat` sends the player's typed message + short history + a
Lily persona/guardrail system prompt to Ollama, returns one short in-character sentence, logs
`kind:"chat"` (with `trigger` + the player's `message`), and falls back to a static in-character line.
Persona: stammers ("Um…", "I—I think…"), addresses `{playerName}`; guardrails: only the act/story topic,
private-life → flustered deflection, off-topic → refocus, never reveals answers, never scores. The
hint/response prompts were also tightened to one short sentence + the stammer.

## Files Created / Modified

- Created `Assets/Presentation/Banter/LilyChatWindow.cs`
- Modified `Assets/Presentation/Backend/GhostBackendClient.cs` (PostChat), `Assets/Presentation/Banter/`
  `AmbientBanterHook.cs` / `AmbientBanterPanel.cs` (pause/resume + Act 2 sizing) / `BanterData.cs`
  (static chat reply), `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
- Backend: `src/app.ts` (`/chat`), `src/llmOrchestration.ts` (chat system prompt + guardrails + logging),
  `tests/app.test.ts`, `README.md`
- Docs: `CODE_WALKTHROUGH.md`, `UNITY_TEST_CHECKLIST.md`; run log `M0-T33_001_lily_chat.md`

## Claude Review / Live Verification

- Ran `POST /chat {actId:act1, message:"what is your favorite food?", playerName:"Alex"}` against the
  running backend → `source:"llm"`, reply: "Um... I, I don't have a favorite food, Alex. My focus is on
  intent classification for our current project." → stammer + name + off-topic deflection all working.
- `hint_logs` (ids 13–15) show `kind:"chat"`, `trigger:"chat_message"`, and the actual player `message`
  (fixes the earlier "question type not recorded" gap).
- `npm test` → 7/7 (was 4; +3 for /chat); `npm run build` clean.
- No scoring; no answer-key in prompts (the chat system prompt forbids revealing solutions). Unity
  `LilyChatWindow` is a separate canvas/window that pauses/resumes the ambient banter.

## Human Verification Result

The user indicated the chat is complete. Residual Editor-only checks: the chat window opens as a
separate window (no overlap), the ambient banter pauses on open and resumes on Close, and the Act 2
ambient banter box is sized correctly.

## Remaining Risks

- Chat-window visual + Act 2 banter sizing need a final Editor glance.
- First LLM call is slow (model load). Stray working-tree files (`outputs/`, `tmp/`, the meeting
  slides) must NOT be committed.

## Next Task

M0-T34 — IBM course CONTENT COVERAGE map: extract the course's teaching points from the PDF and map game
coverage vs gaps (so the game can be made to teach the course content). Planning/analysis; no gameplay
code.
