# M0-T34 — IBM Course Content Coverage Map

## Completion Status

Completed (Codex run 001; docs/analysis). Claude reviewed and accepted.

## Date

2026-06-25

## Summary

Codex rendered all 44 pages of `unorganized_data/Course_IBM_chatbot.pdf` (Poppler) and produced
`Docs/IBM_COURSE_CONTENT.md`: a page-cited course teaching outline (11 sections), a coverage map that
separates "introduced/explained in-game" from "practiced via mechanic" (most concepts land as
partial/missing), a prioritized gap list (M0-T35…M0-T44: fundamentals → strengthen Acts 1–3 teaching →
breadth via Acts 4–8), reasonable out-of-scope items, and a design implication (each concept = Ghost
problem + Lily explanation + player action + visible consequence; playable, not lecture/quiz).

## Files Created

- `Docs/IBM_COURSE_CONTENT.md`
- `Docs/codex_runs/M0-T34_001_ibm_course_coverage_map.md`

(No code/gameplay/backend/scene changes.)

## Claude Review Notes

- Faithful to the PDF per Codex's render (Claude cannot render the image-based PDF in this environment)
  and consistent with `CONFIRMED_PROJECT_CONTEXT.md` §4/§5 — five components, four challenges,
  intent/entity/dialog, and the NLP subtasks all match; the doc adds course detail (rule-based vs
  AI-enabled pros/cons/use-cases, NLG/NLU/speech recognition, watsonx setup/planning/creation workflow,
  platforms) without contradicting the confirmed set.
- Coverage map is honest: intent/entity/dialog are "partial" (practiced but not explained); the
  fundamentals, the five-components-as-a-system, several challenges, ML, and most NLP subtasks are
  "missing".
- Gap list aligns with the user-corrected goal (the game teaches the course content) and §2 (playable,
  not lecture/quiz).
- Scope clean: only the 2 docs; the rendered-PDF artifacts live under `tmp/` (gitignored).

## Human Verification Result

None required (docs/analysis). The user requested closure.

## Key Takeaway

The game currently PRACTICES intent/entity/dialog but does not yet TEACH the concepts, and it lacks the
course fundamentals and breadth. The next work is a teaching pass, fundamentals first.

## Next Task

M0-T35 — Chatbot Fundamentals Teaching Pass: a compact, playable in-game fundamentals layer (chatbot
definition; NLP & ML pillars; rule-based vs AI-enabled; benefits; five components; four challenges),
taught via Ghost problem + Lily explanation + player action + consequence — per
`Docs/IBM_COURSE_CONTENT.md` §1.1–1.4 and §3.
