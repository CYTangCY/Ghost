# ROADMAP.md

> Status note: revised on 2026-06-20 by the user (Chao-Yang) after Act 1 core gameplay reached a
> playable milestone, and updated on 2026-06-22 for the full-system direction (LLM, backend, and
> database are required final-system components — see Phase D). The Act sequencing here matches
> `Docs/CONFIRMED_PROJECT_CONTEXT.md` §5 (reconciled to the 8-Act structure). Project identity and
> learning goals are unchanged.

## Current Status

> **As of 2026-07-03 (post M0-T36):** the vertical-slice milestone is **DONE** — Act 3 UX redesign
> (M0-T30) + shell integration (M0-T31), narrative into Acts 1–3 (M0-T26) + ambient banter (M0-T32),
> backend + database (M0-T27), client↔backend sync (M0-T28) + no-password accounts (M0-T28 follow-on),
> LLM orchestration with static fallback (M0-T29), and the Lily chat window (M0-T33). The active
> workstream is **IBM course content coverage** (section below): coverage map (M0-T34 ✓) →
> fundamentals overview in the shell (M0-T35 ✓) → per-act teaching. **Direction revision
> (2026-07-03, user):** the M0-T36/T37 text-layer teaching passes were rejected as the primary
> method ("teaching must BECOME the game") — superseded by **M0-T45: Acts 1–2 teaching-as-gameplay
> redesign — DONE 2026-07-04** (free-cluster + label piles + Ghost generalization demo; token →
> action-card slots + errand consequences; shared Ghost expression face; floating Lily windows;
> generated Lily portrait; validators unchanged; user-accepted after iterative Play Mode runs
> 001–005). M0-T38 (Act 3 teaching text) is reduced/absorbed — Act 3 already teaches via test-case
> consequences. **ACTIVE: M0-T39 planning mini-level (teaching-as-gameplay).** Then M0-T40 (Act 4
> confidence/fallback — delivers the guaranteed-minimum slider mechanic) + an early WebGL build smoke
> test; Acts 5–8 ship as design mappings if the early-August writing window arrives first.
>
> **Delivery constraint (user, 2026-07-03): dissertation deadline ≈ early August 2026.** Reserve the
> final 1.5–2 weeks for writing; re-assess remaining scope after M0-T38. If time runs short, Acts 5–7
> ship as design mappings (the M0-T34 coverage map already provides them) and implementation priority
> goes to Act 4 — which also delivers the guaranteed-minimum **slider calibration** mechanic
> (`CONFIRMED_PROJECT_CONTEXT.md` §6) — plus a possible Ghost-expressions + SFX presentation pass
> (FR1/FR4 demo polish; the 2026-07-03 project review flagged Ghost's lack of visual presence as the
> largest experience gap).

Progress (as of 2026-07-03):
- Phase A — Game Shell: complete (M0-T13); extended with narrative/name entry (M0-T26), accounts
  (M0-T28 follow-on), and the fundamentals overview (M0-T35).
- Phase B — Act 2 (Entity Extraction): complete and shell-integrated (M0-T14…M0-T19).
- Phase C — Act 3 (Dialog Node Graph): complete — prototype (M0-T20…M0-T24), UX redesign + wired
  Validate (M0-T30), shell integration (M0-T31). Deterministic throughout.
- Vertical slice (narrative / banter / backend / DB / client sync / accounts / LLM / Lily chat):
  complete (M0-T26–M0-T33). See `Docs/VERTICAL_SLICE_PLAN.md` (now marked delivered).
- IBM course content coverage: **in progress** — M0-T34 ✓ map, M0-T35 ✓ fundamentals, M0-T36 ✓ Act 1
  teaching; M0-T37 (Act 2 teaching) active; then M0-T38, M0-T39, M0-T40–T44.

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

### IBM Course Content Coverage (ACTIVE workstream — goal corrected 2026-06-25)

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

Plan and status (2026-07-03):
- **Curriculum coverage map — DONE (M0-T34):** `Docs/IBM_COURSE_CONTENT.md` maps every course teaching
  point (page-cited) → where the game teaches it → gaps, and defines the task ladder M0-T35…T44.
- **Build the missing in-game teaching — IN PROGRESS:** fundamentals overview in the shell done
  (M0-T35, "Ghost's Voice Basics"); per-act teaching passes: Act 1 done (M0-T36, intent classification +
  training examples), Act 2 active (M0-T37, entity/NER + system-custom + synonyms + tokenization link),
  Act 3 next (M0-T38, dialogue management + rule-based flow). Then the planning mini-level (M0-T39) and
  Acts 4–8 (M0-T40–T44), scope-checked against the early-August deadline after M0-T38.

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

- Act 4: Confidence and Fallback (thresholds, disambiguation, fallback nodes, sentiment-based
  routing/escalation — sentiment as a routing signal, not a scoring signal).
- Act 5: Testing and Debugging (run test conversations through the graph; find/fix faults).
- Act 6: Integration / Backend Action / Response Generation (backend/action/response nodes) — builds
  on the Phase D backend/LLM foundation.

## Phase F — NLP Pipeline Lab and Capstone (if time allows)

- Act 7: NLP Pipeline Lab (POS tagging, sentiment, machine translation) — the former optional Act *;
  tokenisation and NER are taught in Act 2, so Act 7 covers the remaining subtasks.
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
