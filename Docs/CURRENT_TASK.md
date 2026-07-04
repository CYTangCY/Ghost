# CURRENT_TASK.md

## ID

M0-T39

## Goal

A short **chatbot planning mini-level** ("Plan Ghost's helper duty") that teaches the IBM course's
planning step (`IBM_COURSE_CONTENT.md` §1.8, Nash activity) **as gameplay** — the M0-T45 standard
applies: the player's choices must be USED by Ghost in a visible consequence phase; no lecture panels,
no quiz dump. Concepts:

- **define the chatbot goal** (what should Ghost's helper duty be?);
- **choose the starting channel from simple usage data** (where do visitors actually show up?);
- **identify key topics from common requests** (reusing the Act 1 intent vocabulary);
- **plan the human handoff** (what should Ghost do when a request is too complex → hand off to Lily).

## Context

M0-T45 established the teaching-as-gameplay pattern (build phase → Ghost uses the player's work →
authored deterministic consequences + Ghost face reactions) and its onboarding lessons (Lily how-to
beat first, persistent objective strip). M0-T39 is the next ladder step; it also naturally exercises
the confirmed §7 mechanics "multiple choice" / "form configuration" without becoming a quiz (choices
are plan decisions with consequences, not knowledge checks). Timeline: deadline ≈ early August; after
this, M0-T40 (Act 4 slider/fallback) + a WebGL smoke test take priority, then writing.

## Design Direction (Claude will finalize the full spec + Codex prompt at planning time)

1. **Onboarding**: Lily brings the lab's visitor log; one-line how-to + persistent objective strip.
2. **Plan board (build phase)**: the player fills Ghost's "duty plan" form: pick the goal, pick the
   starting channel from a tiny authored usage chart, pick the top key topics (from the Act 1 intent
   purposes), and set the handoff rule for hard cases.
3. **A day at the lab (consequence phase)**: a deterministic authored montage runs a handful of visitor
   requests against the plan: right channel/topics → Ghost helps happily; missing topic → Ghost stumbles
   on a common request; no handoff → Ghost melts down on the hard case, with-handoff → Lily rescue beat.
   The player revises the plan and reruns. Completion via a deterministic plan check (authored expected
   plan or acceptable set), never an LLM.
4. Lives in the Game Shell as its own screen (like the fundamentals overview); no new Build Settings
   entries.

## Out of Scope

Acts 4–8 content, backend/LLM changes, audio, external art, changes to existing acts/validators.

## Acceptance Criteria (draft — finalize with the full spec)

- The player makes the four plan decisions and watches Ghost's day visibly succeed/fail because of
  them; revising the plan changes the outcome.
- Correctness is deterministic (authored plan check + authored montage outcomes).
- Onboarding beat + objective strip present from the start; 1080p fit; no Console errors.
- LEARNING_CONTENT / CODE_WALKTHROUGH / UNITY_TEST_CHECKLIST updated; run log per run.
