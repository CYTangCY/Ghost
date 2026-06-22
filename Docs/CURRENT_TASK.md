# CURRENT_TASK.md

## ID

M0-T15

## Goal

Implement the Act 2 entity-extraction session/state layer as pure C# (track the player's current
spans for a message, add/remove spans, validate the current state) plus EditMode tests — scene-free
and UI-free.

## Context

ROADMAP Phase B = Act 2 Entity Extraction. M0-T14 added the Act 2 logic core (`EntityType`,
`EntitySpan`, `EntityExtractionValidator`, `Act2EntityExtractionSampleData`) with EditMode tests,
verified passing in the Editor. Act 1 was built logic-first (validator → sample data → session → UI)
and that pattern worked well; this task adds the Act 2 session layer (mirroring Act 1's
`IntentClassificationSession` / M0-T06) so a later span-annotation UI can add and remove player spans
while this class owns the state and delegates correctness to the validator. Confirmed mechanic
(Docs/LEARNING_CONTENT.md, Act 2): span annotation with entity typing.

## Scope

- Create a pure C# `EntityExtractionSession` in the existing `Ghost.Runtime` assembly (namespace
  `Ghost.Puzzles.EntityExtraction`, under `Assets/Scripts/Puzzles/EntityExtraction/`) that:
  - is created from a message (message text + expected/correct spans), with a convenience to create
    from an `Act2EntityExtractionSampleData.SampleMessage`;
  - tracks the player's current set of submitted spans for that message;
  - supports adding a span (start + length + `EntityType`, or an `EntitySpan`) and removing a span;
  - exposes a snapshot of the current spans and the message text;
  - validates the current state by calling `EntityExtractionValidator.Validate(expected, current)`
    (e.g. a `ValidateCurrentState()` method) and returning the `EntityExtractionResult`.
- Add EditMode tests covering: empty/partial state is incorrect; adding all correct spans validates
  as correct; adding then removing a span; safe handling of spans whose boundaries fall outside the
  message text; duplicate-add behaviour (documented).
- Reuse the existing `EntityType` / `EntitySpan` / `EntityExtractionValidator` from M0-T14; do not
  change them unless a small, behaviour-preserving addition is required (note it in the run log if so).
- Keep everything scene-free, UI-free, WebGL-safe, with no `UnityEngine` dependency.
- Update CODE_WALKTHROUGH.md and UNITY_TEST_CHECKLIST.md.
- Create a Codex run log during implementation.

## Out of Scope

- Do not create scenes, UI, or span-annotation interaction (a later Act 2 sub-task).
- Do not implement the node graph, Act 3+, or change Act 0/1.
- Do not implement backend, LLM, dialogue, or save/load.
- Do not edit ProjectSettings, Packages, Build Settings, asmdefs, or .meta files.
- Do not change the Game Shell or Act 1 pure logic.

## Acceptance Criteria

- `EntityExtractionSession` exists as pure C# (no `UnityEngine` dependency), owning the player's
  current spans for a message and delegating correctness to `EntityExtractionValidator`.
- The session can be created from an `Act2EntityExtractionSampleData` sample message.
- Adding all correct spans validates as correct; an empty or partial set validates as incorrect.
- Spans can be added and removed; out-of-range spans are handled safely with documented behaviour.
- EditMode tests cover the above (correct, partial/incorrect, add/remove, boundary handling).
- Code is scene-free and WebGL-safe; no scene or ProjectSettings files are modified.
- A Codex run log is created under Docs/codex_runs/ after the implementation run.
