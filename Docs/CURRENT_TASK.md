# CURRENT_TASK.md

## ID

M0-T20

## Goal

Design Act 3 (Dialog Management via Node Graph, the flagship mechanic) BEFORE any implementation:
complete the Act 3 learning-content mapping and define a concrete, minimal, deterministic node-graph
data model + simulation/validation approach, plus a logic-first implementation breakdown. Planning
task — Claude-led, no Unity code.

## Context

ROADMAP Phase B (Act 2) is complete and shell-integrated (M0-T14 … M0-T19). Phase C is Act 3 — the
flagship node-graph mechanic, reused/extended by Acts 4–6. Per `Docs/CONFIRMED_PROJECT_CONTEXT.md` §17,
the learning-content mapping must be drafted before Unity implementation, and Act 3's mapping is
currently TBD in `Docs/LEARNING_CONTENT.md`. The node graph should benefit from intent (Act 1) and
entity (Act 2) acting as triggers/slots inside dialog nodes, and correctness must stay deterministic
(graph simulation / test cases), never decided by the LLM.

## Scope

- Complete the Act 3 section of `Docs/LEARNING_CONTENT.md`: learning objective, cute Ghost
  communication problem, mechanic specifics (nodes, branching, slots, response types, context
  variables), player action, success consequence, failure consequence, Lily hint style, and the
  connection to Acts 1–2.
- Define, in `Docs/ARCHITECTURE.md` and/or `Docs/DESIGN_RATIONALE.md`, a concrete MINIMAL node-graph
  design: the graph data model (node types, edges/transitions, triggers/conditions, slots), the
  deterministic simulation (run a message/conversation through the graph → a checkable result), how
  intents/entities feed triggers/slots, and how correctness stays deterministic.
- Produce a logic-first Act 3 sub-task breakdown (e.g. model → simulation/validator → sample data →
  EditMode tests → static UI → interaction → validation feedback → shell integration), mirroring
  Acts 1–2.
- Keep the scope minimal and explainable (NFR1/NFR2); do not over-design.

## Out of Scope

- Do not write any Unity C#, scenes, or asmdefs (this is a design/planning task).
- Do not implement the node graph or any Act 3 code yet.
- Do not invent a new Act structure or change Acts 1–2.
- Do not design backend/LLM specifics beyond noting where they later attach (per ROADMAP Phase D).

## Acceptance Criteria

- The Act 3 section of `Docs/LEARNING_CONTENT.md` is fully drafted (no TBDs for objective, Ghost
  problem, mechanic, player action, success/failure, Lily hint, connection to Acts 1–2).
- `Docs/ARCHITECTURE.md`/`Docs/DESIGN_RATIONALE.md` contain a concrete minimal node-graph data model +
  deterministic simulation/validation design.
- An Act 3 logic-first sub-task breakdown exists (ready to become the next CURRENT_TASK).
- Deterministic-correctness is preserved (graph simulation/test cases decide correctness; never the
  LLM). No Unity files are modified.
