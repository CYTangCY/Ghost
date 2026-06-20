# ROADMAP.md

> Status note: revised on 2026-06-20 by the user (Chao-Yang) after Act 1 core gameplay reached a
> playable milestone. This roadmap refines the Act sequencing in
> `Docs/CONFIRMED_PROJECT_CONTEXT.md` §5 (which still shows the earlier report structure and should
> be reconciled there in a follow-up). Project identity and learning goals are unchanged.

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

## Phase A — Game Shell / Lily / Act Select  ← NEXT

- Title screen, act select / hub, and a Lily dialogue frame.
- Lily appears early as the human guide/tutor character.
- Placeholder presence for Lily and Ghost.
- Start an Act from the shell; return to the hub.
- Goal: the prototype reads as one Ghost game, not disconnected puzzle screens.
- First task of this phase = the revised M0-T13.

## Phase B — Act 2: Entity Extraction

- Teach entity extraction through phrase/chip tagging (span annotation + entity typing).
- Builds on Act 1: intent says *what the speaker wants*; entity adds *the key details*.

## Phase C — Act 3: Dialog Management via Node Graph  (flagship mechanic)

- Introduce the central node-graph gameplay for dialog management.
- The node graph is the project's flagship mechanic and is reused/extended in later Acts.
- Sequencing: the node graph comes AFTER Act 2 on purpose — it benefits from both intent (Act 1)
  and entity (Act 2), which become triggers/slots/conditions inside dialog nodes.

## Phase D — Graph Extensions

Extend the same node graph rather than building unrelated puzzle systems, where possible:

- Act 4: Confidence and Fallback (thresholds, disambiguation, fallback nodes).
- Act 5: Testing and Debugging (run test conversations through the graph; find/fix faults).
- Act 6: Integration / Backend Action / Response Generation (backend/action/response nodes).

## Phase E — NLP Pipeline Lab and Capstone (if time allows)

- Act 7: NLP Pipeline Lab (tokenisation, POS tagging, NER, sentiment) — the former optional Act *.
- Act 8: Capstone / "Repair Ghost's Voice" integration demo — reuses the former Act 0 five-component
  pipeline idea as the final integration puzzle.

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
