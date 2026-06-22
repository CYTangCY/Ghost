# CURRENT_TASK.md

## ID

M0-T18

## Goal

Wire the deterministic validator into the Act 2 scene: enable the Validate button so it calls
`EntityExtractionSession.ValidateCurrentState()` and shows correct/incorrect feedback. This is the
Act 2 equivalent of Act 1's M0-T09 (validation feedback).

## Context

ROADMAP Phase B = Act 2 Entity Extraction. M0-T14 (core), M0-T15 (session), M0-T16 (static UI), and
M0-T17 (chip selection + entity-type assignment) are done and Editor-verified. The Validate button is
currently a disabled placeholder. This task finally connects the puzzle to its deterministic validator
(`EntityExtractionValidator`, via the session) so the player gets correct/incorrect feedback — the
LLM is never involved in scoring. Confirmed mechanic (Docs/LEARNING_CONTENT.md, Act 2): span
annotation with entity typing.

## Scope

- Enable the Validate button in `Act2EntityExtractionStaticPresenter` and route its click through
  `Act2EntityExtractionInteractionController` to `EntityExtractionSession.ValidateCurrentState()`.
- Add a controller method (e.g. `ValidateCurrentState()`) that returns the `EntityExtractionResult`
  and raises a `FeedbackChanged` event (mirror Act 1's interaction controller), so the presenter
  renders feedback without owning puzzle rules.
- Show player-facing feedback in the existing feedback text: a clear "correct" message when all spans
  match, and an "incorrect" message (e.g. an issue/again hint, optionally an issue count) otherwise.
- Keep M0-T17 selection/tagging/untagging behaviour intact; keep all correctness in the validator
  (the presenter/controller must not re-implement matching).
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md; create a Codex run log. Regenerate the scene
  via the menu builder if needed (do NOT add it to Build Settings).

## Out of Scope

- Do not let the LLM or any non-deterministic source decide correctness.
- Do not implement per-span inline error markers beyond simple feedback text (a later polish task),
  the node graph, Act 3+, backend, LLM, dialogue, save/load, scoring persistence, multi-chip spans,
  or final art.
- Do not add the scene to Build Settings; do not change the Game Shell or its act list.
- Do not edit ProjectSettings, Packages, `.meta`, non-Act-2 asmdefs, the Act 2 pure logic
  (M0-T14/M0-T15), or Act 1 / Game Shell scripts.

## Acceptance Criteria

- The Validate button is enabled; clicking it with all chips correctly tagged shows a clear "correct"
  message; clicking it with a partial/incorrect set shows an "incorrect" message.
- Validation result comes from `EntityExtractionSession.ValidateCurrentState()` /
  `EntityExtractionValidator` (deterministic) — not from the UI or any LLM.
- M0-T17 selection/tagging/untagging still works; feedback updates as the player changes tags and
  re-validates.
- No Console errors in Play Mode; the scene is not in Build Settings; Act 1, the Game Shell, and the
  Act 2 pure logic are unchanged.
- CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md are updated; a Codex run log is created.
