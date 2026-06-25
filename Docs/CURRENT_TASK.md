# CURRENT_TASK.md

## ID

M0-T34

## Goal

Produce the IBM course **content coverage map**: extract the course's actual teaching points from
`unorganized_data/Course_IBM_chatbot.pdf`, and map each to where the game currently teaches it
(introduced AND practiced) vs what is MISSING — so we know exactly what in-game teaching to build so the
GAME teaches the course content. Planning/analysis task; no gameplay code.

## Context

User-corrected goal (2026-06-25): the game must TEACH the IBM course's content — players learn the
curriculum by playing — kept playable (`CONFIRMED_PROJECT_CONTEXT.md` §2: no lecture/quiz dump). The
vertical slice (Acts 1–3 + backend/DB + LLM Lily chat) is built and verified. Before building more
teaching, we need a precise map of what the course teaches vs what the game teaches. Claude cannot render
the image-based PDF in its environment; Codex can.

## Scope

- Codex renders `unorganized_data/Course_IBM_chatbot.pdf` and extracts a faithful, structured outline of
  the course's TEACHING CONTENT (modules/sections + the key points each teaches) into a new
  `Docs/IBM_COURSE_CONTENT.md`, citing page ranges; no invented content.
- A coverage map (in that doc): for each course teaching point → where the game currently teaches it
  (which Act / Game Shell, and whether it is INTRODUCED/explained as well as PRACTICED) → status:
  `taught` / `partial (practiced but not explained)` / `missing`.
- A prioritized gap list of in-game teaching to add (fundamentals first — chatbot definition, rule-based
  vs AI-enabled, five components, four challenges; then strengthen Acts 1–3 teaching; then breadth via
  Acts 4–7), framed as proposed follow-up tasks (do NOT implement them here).
- No Unity/gameplay/puzzle changes. Update HANDOFF; this feeds the next implementation tasks.

## Out of Scope

- Implementing the teaching content (later tasks); changing puzzle rules/validators; dissertation/slide
  wording (a separate, minor concern).

## Acceptance Criteria

- `Docs/IBM_COURSE_CONTENT.md` exists with a faithful, page-cited outline of the course's teaching
  content + a coverage map (taught / partial / missing per concept) + a prioritized gap list.
- It accurately reflects the PDF (Codex actually rendered it); no invented content.
- No gameplay/code changes; a run log is created.
