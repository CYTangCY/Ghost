# CURRENT_TASK.md

## ID

M0-T02

## Goal

Complete the confirmed learning-content mapping for Acts 0, 1, and 2 in
Docs/LEARNING_CONTENT.md, and confirm the puzzle mechanic assigned to each of these Acts.

## Context

The core planning documents (PROJECT_BRIEF, REQUIREMENTS, LEARNING_CONTENT, DESIGN_RATIONALE,
ARCHITECTURE, AGENTS, CLAUDE) were populated in M0-T01 and are consistent with
Docs/CONFIRMED_PROJECT_CONTEXT.md.

Docs/LEARNING_CONTENT.md is the canonical mapping file (it replaces the
LEARNING_CONTENT_MAPPING.md referenced in earlier drafts).

Acts 0, 1, and 2 have implementation priority because they introduce reused mechanic types.
Their mapping must be fixed before any Unity script is written.

## Scope

In Docs/LEARNING_CONTENT.md, fill the TBD fields for:
- Act 0: learning objective, cute Ghost communication problem, puzzle mechanic, player action,
  success consequence, failure consequence.
- Act 1: confirm the puzzle mechanic (currently "likely drag-and-drop classification").
- Act 2: confirm the puzzle mechanic (currently "likely span annotation").

## Out of Scope

Do not implement Unity scripts.
Do not create Unity scenes.
Do not invent academic references.
Do not invent new Act structure.
Do not invent Act details without user confirmation.
Do not reduce the design to dialogue plus multiple-choice quiz.

## Acceptance Criteria

- Act 0, Act 1, and Act 2 each have no remaining TBD in: learning objective, Ghost problem,
  puzzle mechanic, player action, success consequence, failure consequence, Lily hint style.
- Each confirmed mechanic comes from the confirmed mechanic set (CONFIRMED_PROJECT_CONTEXT §7).
- Each mapping shows a visible change in Ghost's response between success and failure.
- No new Act is introduced and no mechanic is reduced to multiple choice.
