# CURRENT_TASK.md

## ID

M0-T32

## Goal

Add an in-act ambient Ghost+Lily banter area in each act scene's spare space: fixed per-act dialogue
loops (data-driven, plenty, fun) with Lily warming up across acts + a recurring nerdy-joke-then-
embarrassed beat, and story-consistent Ghost lines. Frontend, static text; structured so player choices
+ LLM can extend it later. Continuation of the M0-T26 narrative work (user request).

## Context

Acts 1–3 are playable and the shell narrative (name entry, intro/debrief beats, portraits) is done
(M0-T26). The user wants the spare on-screen space during each act used for real-time Ghost+Lily banter
that makes the lab feel alive. Content is drafted in `Docs/NARRATIVE.md` ("In-Act Ambient Banter").
Puzzles stay deterministic and unchanged; this is presentation/flavour only.

## Scope

- An ambient banter panel placed in each act prototype scene's spare space (Act 1/2/3), non-blocking,
  that cycles per-act dialogue loops (timer and/or a "next" tap) and loops. Reuse the speaker portrait
  placeholder.
- Data-driven ambient beats per act `{ speaker, text (with {playerName}), optional tag }`, seeded from
  `Docs/NARRATIVE.md`; easy to add more lines. Structure leaves room for future player choices / LLM
  (e.g. a beat type or optional choices field) without building them now.
- Lily's arc (Act 1 nervous → Act 3 joking) + the nerdy-joke-then-embarrassed beat; Ghost lines match
  each act's story stage; address the player by name.
- Frontend, static text only (no LLM, no backend, no save/load). Do NOT change puzzle logic, validators,
  sessions, or act mechanics. Regenerate scenes via the existing builders if needed; no Build Settings
  changes beyond existing.
- Update CODE_WALKTHROUGH.md + UNITY_TEST_CHECKLIST.md; create a Codex run log.

## Out of Scope

- LLM, backend, player-choice branching (future), save/load, final art, and any puzzle-rule change.

## Acceptance Criteria

- Each act scene shows a non-blocking ambient Ghost+Lily banter area in spare space that cycles fun
  per-act lines and loops; data-driven; addresses the player by name.
- Lily noticeably warms across acts and has a nerdy-joke-then-embarrassed beat; Ghost lines match each
  act's story stage.
- Puzzle behaviour for Acts 1/2/3 is unchanged; no Console errors.
- The data is structured so player choices / LLM lines can be added later without a rewrite.
- CODE_WALKTHROUGH.md + UNITY_TEST_CHECKLIST.md updated; a Codex run log created.
