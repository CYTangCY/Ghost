# CURRENT_TASK.md

## ID

M0-T35

## Goal

Add a compact, **playable** chatbot-fundamentals teaching layer so the player actually LEARNS the IBM
course fundamentals in-game: what a chatbot is; NLP & ML as the two pillars; rule-based vs AI-enabled
chatbots; benefits (simple level); the five components; the four challenges. Taught via Ghost's problem
+ Lily's short in-fiction explanation + a small player action + visible consequence — NOT a lecture or
quiz dump (`CONFIRMED_PROJECT_CONTEXT.md` §2).

## Context

The M0-T34 coverage map (`Docs/IBM_COURSE_CONTENT.md` §1.1–1.4, §3) shows the game practices
intent/entity/dialog but does not teach the course fundamentals, which are currently missing. The course
starts there and the user-corrected goal is that the game teaches the course content. This is the
"fundamentals first" priority. Builds on the Game Shell + `LilyDialogueFrame` + the deferred "Rebuild
Ghost's Voice / five components" idea (LEARNING_CONTENT Act 0 / Act 8) + the existing Lily portrait/chat
UI.

## Scope

- A short, playable fundamentals sequence (in the Game Shell, e.g. an early "Ghost's voice is
  disconnected" intro before/at the act hub), data-driven, reusing the Lily dialogue frame / portrait,
  covering:
  - **Chatbot definition** (a program that simulates conversation) — via Ghost.
  - **NLP & ML pillars** (NLP understands language/intent; ML learns from data) — simple.
  - **Rule-based vs AI-enabled** — use Ghost's deterministic validators vs Lily's LLM chat as the
    concrete in-game contrast.
  - **Benefits** (efficiency / repetitive tasks) — brief.
  - **Five components** (UI → NLP engine → dialogue management → response generation → UI, + backend
    integration) — a small visual "Ghost's voice parts" map.
  - **Four challenges** (unstructured input, misunderstanding, human-like interaction, contextual
    awareness) — shown as Ghost's cute failure modes.
- Each beat = Ghost problem → Lily short explanation → a small player action (tap / arrange / pick) →
  visible consequence. Playable + visual; NOT a lecture wall or multiple-choice quiz.
- Frontend, data-driven, static text (the existing Lily LLM chat may be reused but is not required). Do
  NOT change puzzle validators/sessions/rules or Acts 1–3 mechanics.
- Update `Docs/LEARNING_CONTENT.md` (fundamentals now taught in-game), CODE_WALKTHROUGH.md,
  UNITY_TEST_CHECKLIST.md; create a Codex run log.

## Out of Scope

- The watsonx product UI walkthrough; the per-Act teaching strengthening (M0-T36–T38) and Acts 4–8
  (later); quizzes; any change to puzzle rules/validators.

## Acceptance Criteria

- From the shell, the player can go through a short playable fundamentals sequence that teaches the six
  fundamentals above.
- It teaches via Ghost problem + Lily explanation + player action + consequence (playable/visual), not a
  lecture/quiz dump.
- Acts 1–3 puzzles and validators are unchanged; no Console errors.
- `Docs/LEARNING_CONTENT.md` + CODE_WALKTHROUGH.md + UNITY_TEST_CHECKLIST.md and a Codex run log are
  updated.
