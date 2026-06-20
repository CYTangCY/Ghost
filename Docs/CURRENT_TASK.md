# CURRENT_TASK.md

## ID

M0-T14

## Goal

Implement the Act 2 entity-extraction core as pure C# logic (entity-span model + validator) plus
sample data and EditMode tests — scene-free and UI-free.

## Context

ROADMAP Phase B = Act 2 Entity Extraction. Act 1 was built logic-first (M0-T04 validator → M0-T05
sample data → M0-T06 session) before any UI, and that pattern worked well; M0-T13 added the Game
Shell. This task is the first scene-free slice of Act 2, mirroring the proven Act 1 logic-first
pattern. Confirmed mechanic (Docs/LEARNING_CONTENT.md, Act 2): span annotation with entity typing,
covering entity extraction, synonyms, and system vs custom entities.

## Scope

- Create a pure C# model for an entity span: the target message text, a span (e.g. start index +
  length, or matched substring), and an entity type.
- Create a pure C# validator that checks a player's submitted spans/types against the correct answer
  for a message (correct spans found, correct types; handles synonyms and the system-vs-custom
  entity distinction).
- Add Act 2 sample data: messages with their correct entity spans/types, covering entity extraction,
  synonyms, and system vs custom entities.
- Add EditMode tests for at least one correct annotation and several incorrect cases (missing span,
  wrong type, wrong boundary, extra span).
- Put runtime code in the existing `Ghost.Runtime` assembly (namespace under it), reusing the Act 1
  logic-layer conventions (no UnityEngine dependency).
- Keep everything scene-free, UI-free, and WebGL-safe.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md.
- Create a Codex run log during implementation.

## Out of Scope

- Do not create scenes, UI, or span-annotation interaction (a later Act 2 sub-task).
- Do not implement the node graph, Act 3+, or change Act 0/1.
- Do not implement backend, LLM, dialogue, or save/load.
- Do not edit ProjectSettings, Packages, Build Settings, or .meta files.
- Do not change Act 1 pure logic or the Game Shell.

## Acceptance Criteria

- Act 2 entity-extraction validation logic exists as pure C# (no UnityEngine dependency, like the
  Act 1 validator).
- Act 2 sample data exists with multiple messages and correct spans/types, including a synonym case
  and a system-vs-custom-entity distinction.
- EditMode tests cover at least one correct annotation and several incorrect cases.
- Code is scene-free and WebGL-safe.
- No Unity scene or ProjectSettings files are modified.
- A Codex run log is created under Docs/codex_runs/ after the implementation run.
