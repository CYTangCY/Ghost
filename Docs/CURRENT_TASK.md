# CURRENT_TASK.md

## ID

M0-T26

## Goal

Weave narrative into Acts 1–3 so the three acts read as one cohesive story: a story premise, characters,
short Lily/Ghost beats (intro before each act + debrief after), and scene transitions — frontend-only
and data-driven, reusing `LilyDialogueFrame` + a small narrative state. Two parts: (1) Claude drafts the
narrative content/beats (design), then (2) Codex implements the data + shell beats + act ordering.

## Context

Vertical-slice milestone (Docs/VERTICAL_SLICE_PLAN.md §A), next phase after Acts 1–3 became playable and
shell-linked (M0-T31). The puzzles stay deterministic and unchanged; this task only adds story framing
around them. Gated by the open narrative decisions (setting / protagonist / title) — to be confirmed
before Claude drafts the content.

## Scope

- Narrative content (Claude-drafted): a short premise; per-act Lily intro + debrief beats and Ghost
  reactions that tie Act 1 (intent) → Act 2 (entity) → Act 3 (dialog graph) into one through-line
  (Ghost gradually becomes clearer; Lily warms up).
- A small **narrative state** (which acts are done) to order/gate the beats from the shell hub.
- Implementation (Codex): extend `ShellDialogueData` / the shell so intro/debrief beats play around each
  act via `LilyDialogueFrame`; data-driven (not hardcoded per screen). Frontend-only, static text (no
  LLM yet — that is M0-T29).
- Keep all puzzle rules and Act 1/2/3 mechanics unchanged. Update CODE_WALKTHROUGH.md +
  UNITY_TEST_CHECKLIST.md; create a Codex run log.

## Out of Scope

- No backend / database / LLM (static text only); no full visual-novel dialogue system; no save/load.
- No changes to puzzle logic/validators or the node graph; no final art.
- Do not edit ProjectSettings/Packages/.meta beyond what the shell builder already does.

## Acceptance Criteria

- Decisions confirmed (setting / protagonist / title) and recorded.
- The shell plays a short Lily intro before each act and a debrief after, ordered by narrative state; the
  beats tie the three acts into one story (data-driven, not hardcoded per screen).
- Puzzle behaviour for Acts 1/2/3 is unchanged; no Console errors.
- CODE_WALKTHROUGH.md + UNITY_TEST_CHECKLIST.md updated; a Codex run log created.
