# M0-T02 — Learning-Content Mapping (Acts 0–2)

## Completion Status

Completed. M0-T02 acceptance criteria met for Acts 0, 1, and 2.

## Date

2026-06-17

## Summary

Completed the confirmed learning-content mapping in `Docs/LEARNING_CONTENT.md` for the three
priority Acts. Act 0 was designed from the user-selected option; Act 1 and Act 2 mechanics were
confirmed and the "likely / must be confirmed" wording removed. The five chatbot components and
four chatbot challenges were sourced from the IBM SkillsBuild course material and used verbatim.

## Changed Files

- `Docs/LEARNING_CONTENT.md` — Act 0 fully mapped; Act 1 and Act 2 mechanics confirmed and clarified.

Workflow/archival files changed in the same session (separate from the mapping deliverable):
`Docs/completed_tasks/` created; `M0-T01` and `M0-T02` archives added; `Docs/HANDOFF_LOG.md`,
`CLAUDE.md`, and `AGENTS.md` updated with the task archiving convention.

## IBM Source Used

`unorganized_data/Course_IBM_chatbot.pdf`:
- Five components — p.5 "Components of a chatbot" (reinforced p.6 "Interaction between components"
  and p.39 "Key concepts", item 6).
- Four challenges — p.3 "Challenges in using chatbots" (reinforced p.39 "Key concepts", item 7).
- Exact wording preserved: "Dialogue management system", "Backend integration".

## Act 0 Decision

- Option A — "Rebuild Ghost's voice".
- Mechanic: flow diagram construction (from the confirmed mechanic set, §7).
- Concept: the five components assembled into a working pipeline
  (UI → NLP engine → Dialogue management system → Response generation module → UI, with Backend
  integration where extra information is needed).
- The four challenges appear as cute failure flavours when parts are missing, misordered, or
  poorly connected.

## Act 1 Mechanic Confirmation

Drag-and-drop classification — the player groups message cards by intent (purpose), not by exact
wording.

## Act 2 Mechanic Confirmation

Span annotation with entity typing — the player highlights a span and assigns an entity type
(system entity, custom entity, location/room, object, time, name), covering entity extraction,
synonyms, and system vs custom entities.

## Remaining Limitations

- The four challenges are represented as failure "flavours" across the pipeline, not as a strict
  one-to-one component-to-challenge map.
- Acts *, 3, 4, 5, and 6 mappings remain TBD (out of M0-T02 scope; medium / optional priority).
- No Unity implementation exists yet; the mechanics are design-level only.
- Unconfirmed per `CONFIRMED_PROJECT_CONTEXT.md` §14: exact setting, art style, player-character
  identity, and final level content are still open.

## Next Task

M0-T03 — Inventory the actual Unity repository state before the first implementation task.
