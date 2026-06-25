# CURRENT_TASK.md

## ID

M0-T33

## Goal

Turn "Ask Lily" into a constrained **chat**: a dedicated chat window (not the ambient reply box) where
the player can TYPE questions and Lily replies in one short, in-character sentence via the LLM, staying
on-topic (the current act's chatbot/NLP learning + the Ghost story), with persona guardrails. Plus fix
the ambient banter UI (Act 2 box too large; fixed-reply position).

## Context

M0-T29 made the LLM work (one-shot hints; `source:"llm"` verified) but it has no free-text input and
writes into the ambient reply box. The user wants a real chat: type a question → short in-character Lily
reply, in a separate window, with topic guardrails. Builds on the M0-T29 backend (Ollama client +
orchestration) and the M0-T28 client pattern. Deterministic puzzles unchanged; the LLM never scores;
static in-character fallback stays.

## Scope

- Backend: add `POST /chat { actId, message, history?, profileId? }` → `{ reply, source }` that sends the
  PLAYER'S typed `message` plus a persona/guardrail system prompt to Ollama; returns ONE short sentence;
  logs to `hint_logs` (`kind:"chat"`, `trigger`, a non-spoiler note of the player's topic). Reuse the
  Ollama client; static in-character fallback if unavailable. No answer-key in the prompt; no scoring.
  - **Lily persona + guardrails (system prompt):** human postdoctoral senior, nerdy/timid/warm, not an
    AI; reply in ONE short sentence, in character, addressing {playerName}; ONLY discuss the current
    act's chatbot/NLP concept and the Ghost story/situation; if asked about Lily's private life → react
    a bit flustered/annoyed and deflect (in character); if asked anything off-topic/unrelated → redirect
    ("let's focus on helping Ghost right now"); never reveal puzzle solutions/answers; never decide
    correctness or progression.
- Client (Unity): a dedicated Lily chat window/panel that OPENS on Ask Lily (instead of writing into the
  ambient banter): a scrollable message list + a text input field + send + close; it PAUSES the ambient
  banter while open and RESUMES on close. WebGL-safe + graceful (LLM/backend down → a static
  in-character line). The after-incorrect-validate hint may open/append to this chat window.
- Tighten reply length across hints/responses/chat to ~one short sentence (≤ ~25 words) in the prompts.
- Ambient banter UI fixes: resize the Act 2 box (too large) and fix the fixed-reply position so it reads
  cleanly and does not overlap.
- Keep deterministic validators/sessions/puzzle rules unchanged. Update CODE_WALKTHROUGH.md +
  UNITY_TEST_CHECKLIST.md; create a Codex run log.

## Out of Scope

- Persistent chat history across sessions; multiple LLMs; voice; the Act 8 capstone; any LLM scoring.

## Acceptance Criteria

- Pressing Ask Lily opens a separate chat window (not the ambient box); the player can type a question
  and gets a short, in-character Lily reply from the LLM (`source:"llm"` when Ollama is up; static
  in-character fallback otherwise).
- Lily stays on-topic: off-topic → redirect to the task; private-life → flustered deflection; never
  reveals answers; never scores.
- Replies are one short sentence and match the persona.
- The ambient banter pauses while chatting and resumes on close; the Act 2 banter box is sized
  correctly; nothing overlaps.
- Deterministic puzzles unchanged; graceful when backend/LLM is down; `hint_logs` records chat turns
  (`kind:"chat"` + a topic note). Docs + a Codex run log updated.
