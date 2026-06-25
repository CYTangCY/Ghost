# ROADMAP.md

> Status note: revised on 2026-06-20 by the user (Chao-Yang) after Act 1 core gameplay reached a
> playable milestone, and updated on 2026-06-22 for the full-system direction (LLM, backend, and
> database are required final-system components — see Phase D). The Act sequencing here matches
> `Docs/CONFIRMED_PROJECT_CONTEXT.md` §5 (reconciled to the 8-Act structure). Project identity and
> learning goals are unchanged.

## Current Status

> **Latest direction (2026-06-22, post Act 3 prototype):** Acts 1–3 prototypes are in place (intent,
> entity, dialog node graph — all deterministic, Editor-verified). Per the user, the next milestone is
> a **vertical slice / cohesive build**, not more new acts:
> 1. **Full-system foundation** — backend + database + LLM (ROADMAP Phase D), wrapping the existing acts.
> 2. **Narrative integration** — weave story, characters, Lily, and scene transitions into Acts 1–3.
> 3. **Act 3 node-graph UX redesign** — make it fun with a clear objective and free-form connecting
>    (the M0-T24 build is functional but mechanical: fiddly From/To selection, allows self-loops,
>    unclear goal).
>
> Acts 4–8 (graph extensions / NLP lab / capstone) resume only AFTER this vertical slice reaches a
> certain completeness ("整個成形"). See `Docs/completed_tasks/M0-T24_act3_node_placement_connection.md`
> for the Act 3 UX debt.

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

Progress (as of 2026-06-22):
- Phase A — Game Shell: complete (M0-T13).
- Phase B — Act 2 (Entity Extraction): complete and shell-integrated (M0-T14…M0-T19).
- Phase C — Act 3 (Dialog Node Graph): prototype complete (M0-T20 design, M0-T21 core, M0-T22 session,
  M0-T23 static UI, M0-T24 placement/connection [functional, UX debt]). Deterministic throughout.
- **Active: the vertical-slice milestone (see note above). Next task: M0-T30 — Act 3 node-graph UX
  redesign + wire Validate.** Then narrative (M0-T26), backend+DB (M0-T27), client integration
  (M0-T28), LLM (M0-T29, static-hints-first). Acts 4–8 resume after the slice. See
  `Docs/VERTICAL_SLICE_PLAN.md`.

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

## Vertical Slice (current milestone, 2026-06-22)

After the Acts 1–3 prototypes, the active milestone is a cohesive **vertical slice** rather than new
acts — see `Docs/VERTICAL_SLICE_PLAN.md`. It bundles three workstreams: the Act 3 node-graph UX
redesign, narrative integration into Acts 1–3, and the Phase D full-system foundation (backend / DB /
LLM). Decisions (2026-06-22): the LLM is **static-hints-first** (LLM deferred within the slice);
execution order is **M0-T30** Act 3 UX redesign → **M0-T26** narrative → **M0-T27** backend+DB →
**M0-T28** client↔backend → **M0-T29** LLM. Acts 4–8 (the phases below) resume after the slice reaches a
certain completeness. Deterministic-correctness still holds throughout.

### IBM Course Content Coverage (queued — after M0-T33; goal corrected 2026-06-25)

**Goal (user-corrected): the game must TEACH the IBM course's content — players learn the course's
chatbot/NLP curriculum by playing it.** This is pedagogical CONTENT COVERAGE, not the dissertation or
architecture "mirroring" the course, and not merely adding concept labels. Teaching stays playable
(`CONFIRMED_PROJECT_CONTEXT.md` §2: no lecture/quiz dump): each course concept should be introduced
in-fiction (Lily, via Ghost's problem), practiced through a mechanic, and shown in consequence.

Gap: the current Acts let the player DO intent / entity / dialog, but the game does not yet clearly
TEACH those concepts, nor the course's fundamentals (what a chatbot is; rule-based vs AI-enabled; the
five components; the four challenges), nor the rest of the curriculum (confidence/fallback, testing,
integration, NLP subtasks: tokenisation / POS / NER / sentiment). Those are course content the game must
deliver — so Acts 4–8 are part of coverage, not optional extras.

Plan (after M0-T33):
- **First — Curriculum coverage map:** extract the IBM course's actual teaching points from
  `unorganized_data/Course_IBM_chatbot.pdf` (Codex/the user can render the image PDF; Claude cannot in
  this environment) and map each teaching point → where the game currently teaches it (intro + practice)
  → what is MISSING.
- **Then — build the missing in-game teaching** as playable learning, prioritised by the map: the
  fundamentals first (chatbot definition, rule-based vs AI-enabled, five components, four challenges,
  taught in-game — the deferred "Act 0"), strengthen Acts 1–3 so they genuinely teach their concept (not
  just run a puzzle), then cover the remaining curriculum (Acts 4–7) for breadth.

(This supersedes the earlier "alignment UI labels / dissertation framing" plan from
`Docs/IBM_COURSE_ALIGNMENT_REVIEW.md`, which treated alignment as labelling/wording and missed the real
goal — the game teaching the course content. Labels/dissertation wording are at most a minor by-product.)

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
