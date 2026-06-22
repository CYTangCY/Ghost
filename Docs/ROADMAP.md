# ROADMAP.md

> Status note: revised on 2026-06-20 by the user (Chao-Yang) after Act 1 core gameplay reached a
> playable milestone, and updated on 2026-06-22 for the full-system direction (LLM, backend, and
> database are required final-system components — see Phase D). The Act sequencing here matches
> `Docs/CONFIRMED_PROJECT_CONTEXT.md` §5 (reconciled to the 8-Act structure). Project identity and
> learning goals are unchanged.

## Current Status

Act 1 core gameplay is complete enough to be the first playable prototype milestone:

- click-to-assign
- drag-to-assign
- drag an assigned card back to unassigned
- drag an assigned card between groups
- Back / unassign
- Validate
- validation feedback

Decision: do not keep polishing Act 1 now. The next major step is to make the project feel like a
game (a Ghost educational game), not only a mechanics demo.

Progress since the 2026-06-20 revision:
- Phase A (Game Shell / Lily / Act Select) is **complete** — M0-T13 shipped the title screen,
  act-select/hub, reusable Lily dialogue frame, Ghost placeholder, and shell↔Act 1 navigation
  (Editor-verified; see `Docs/completed_tasks/M0-T13_game_shell_prototype.md`).
- **Active task: M0-T14 — Act 2 entity-extraction core (pure C# logic + sample data + EditMode
  tests), the first slice of Phase B.**

## Phase A — Game Shell / Lily / Act Select  ✓ DONE (M0-T13)

- Title screen, act select / hub, and a Lily dialogue frame.
- Lily appears early as the human guide/tutor character.
- Placeholder presence for Lily and Ghost.
- Start an Act from the shell; return to the hub.
- Goal: the prototype reads as one Ghost game, not disconnected puzzle screens.
- Delivered by M0-T13 (Game Shell prototype), Editor-verified; see
  `Docs/completed_tasks/M0-T13_game_shell_prototype.md`.

## Phase B — Act 2: Entity Extraction  ← CURRENT (M0-T14 = first slice)

- Teach entity extraction through phrase/chip tagging (span annotation + entity typing).
- Builds on Act 1: intent says *what the speaker wants*; entity adds *the key details*.
- M0-T14 starts the logic-first slice (entity-span model + validator + sample data + EditMode tests),
  mirroring the Act 1 logic-before-UI pattern.

## Phase C — Act 3: Dialog Management via Node Graph  (flagship mechanic)

- Introduce the central node-graph gameplay for dialog management.
- The node graph is the project's flagship mechanic and is reused/extended in later Acts.
- Sequencing: the node graph comes AFTER Act 2 on purpose — it benefits from both intent (Act 1)
  and entity (Act 2), which become triggers/slots/conditions inside dialog nodes.

## Phase D — Full-System Foundation (backend + database + LLM)

The final project is a full AI-assisted educational game system, not only a Unity puzzle prototype.
These are **required** final-system components, integrated **after the gameplay skeleton (Game Shell
+ Acts 1–3) is stable** so they wrap a proven core rather than being built speculatively:

- Backend API — content delivery, player progress, attempt logs, optional graph simulation/scoring,
  and LLM orchestration.
- Database schema — learning content, puzzle content, player progress, player attempts, and
  (if appropriate) dialogue/hint logs.
- LLM orchestration — Lily hints, Ghost response generation, explanatory feedback, capstone chatbot
  simulation, and optional natural-language variation.

Deterministic-correctness rule (applies to every phase): puzzle correctness comes from deterministic
validators, graph simulation, test cases, or backend scoring logic — **never from the LLM**. The LLM
may hint, explain, or generate natural language, but is never the source of truth for scoring.

## Phase E — Graph Extensions

Extend the same node graph rather than building unrelated puzzle systems, where possible:

- Act 4: Confidence and Fallback (thresholds, disambiguation, fallback nodes).
- Act 5: Testing and Debugging (run test conversations through the graph; find/fix faults).
- Act 6: Integration / Backend Action / Response Generation (backend/action/response nodes) — builds
  on the Phase D backend/LLM foundation.

## Phase F — NLP Pipeline Lab and Capstone (if time allows)

- Act 7: NLP Pipeline Lab (tokenisation, POS tagging, NER, sentiment) — the former optional Act *.
- Act 8: Capstone / "Repair Ghost's Voice" integration demo — reuses the former Act 0 five-component
  pipeline idea as the final integration puzzle.

## Full-System & Deterministic-Correctness Note

LLM, backend, and database are required components of the final system, but they layer on top of a
working gameplay skeleton: build the playable core first (Game Shell + Acts 1–3), then the
full-system foundation (Phase D), then graph extensions (Phase E) and the capstone (Phase F).
Throughout, correctness stays deterministic — validators, graph simulators, test cases, and backend
scoring decide right/wrong; the LLM only supports hints, responses, explanations, and capstone
simulation. Phases are workstreams that can overlap, not a strict serial order.

## Flagship-Mechanic Note

The node graph (Act 3) is the flagship gameplay, but it is built AFTER Act 2 because it reads best
once the player already understands intent and entity — those concepts become the conditions and
slots inside dialog nodes. Acts 4–6 extend the same graph so the player deepens one core system
instead of learning many disconnected ones.

## Where the Former Act 0 (Fundamentals) Went

The earlier Act 0 (chatbot definition, rule-based vs AI-enabled, five components, four challenges)
is preserved, not dropped:

- the conceptual intro (what a chatbot is, components, challenges) is introduced by Lily in the
  Game Shell;
- the "Rebuild Ghost's Voice" five-component pipeline mechanic becomes the Act 8 capstone
  integration demo.

## Preserved Project Identity (unchanged)

- Ghost is a cute ghost character — not literally an AI, chatbot, or robot.
- Lily is the protagonist's postdoctoral senior from the lab: human, nerdy, technically capable,
  pretty/cute, slightly timid/awkward, deferential but knowledgeable, and likable. She guides the
  player through chatbot/NLP ideas.
- The game teaches chatbot/NLP concepts through playable mechanics, not quiz-only explanation.
- "while True: learn()" is only a reference game, not the project title.
