# VERTICAL_SLICE_PLAN.md

> Status: blueprint for the vertical-slice milestone (2026-06-22, user-directed after the Act 3
> prototype). Defines (A) the narrative frame for Acts 1–3, (B) the full-system architecture + data
> contracts, (C) the Act 3 node-graph UX redesign direction, and (D) the task breakdown. The
> deterministic-correctness rule holds throughout (validators / graph simulation / test cases decide
> scoring; the LLM never does). Items marked ⚠️ need user confirmation before implementation
> (`CONFIRMED_PROJECT_CONTEXT.md` §14).

## A. Narrative frame (Acts 1–3)

Proposed premise (consistent with confirmed identity; the sample data already says "lab"/"laboratory"):
a cute **Ghost** has appeared in a research **lab** and can't make itself understood. The **protagonist**
(a junior lab member) helps Ghost learn to communicate, guided by **Lily** (the protagonist's nerdy,
timid-but-capable postdoctoral senior). Each Act = Ghost learning one piece of understanding people:

- **Act 1 — Intent:** Ghost reacts to the wrong purpose → the player teaches it to read what people *want*.
- **Act 2 — Entity:** Ghost misses key details → the player teaches it to catch the *specifics*.
- **Act 3 — Dialog graph:** Ghost replies out of order → the player builds Ghost a conversation map so it
  asks for what's missing and answers in the right order.

Through-line: Ghost gradually becomes clearer/cuter as the player progresses; Lily warms up. Delivered as
short **Lily dialogue beats in the Game Shell** (an intro before each act, a debrief after), reusing
`LilyDialogueFrame` + a small narrative-state record (which acts are done) to order them.

⚠️ **Confirm:** exact setting (plain lab vs lightly haunted lab vs other), protagonist identity/name,
Ghost backstory depth, and the game title (all "unconfirmed" in `CONFIRMED_PROJECT_CONTEXT.md` §14).

## B. Full-system architecture + data contracts (ROADMAP Phase D)

Builds on `ARCHITECTURE.md` Phase D layers. Concrete enough to scaffold; keep minimal (NFR1/NFR2).

- **Backend** (Node.js + TypeScript REST, per `CONFIRMED §8`). Draft endpoints:
  - `GET /content` — act/level metadata + puzzle data.
  - `GET/PUT /progress/:profileId` — acts/levels complete + narrative state.
  - `POST /attempts` — log a puzzle attempt (act, result, errors) for analytics.
  - `POST /hints` and `POST /responses` — LLM-backed Lily hints / Ghost responses (see LLM).
- **Database** (SQLite prototype): `learning_content`, `puzzles` (per-act data + answer keys),
  `profiles` (pseudonymous), `progress`, `attempts`, `hint_logs`.
- **LLM** (IBM Granite via Ollama + local proxy, per `CONFIRMED §8`): orchestrated by the backend for
  Lily hints, Ghost responses, explanatory feedback — **never scoring**. Deterministic validators stay
  authoritative.
- **Client** (Unity WebGL): thin API client + **graceful degradation** (NFR5) — if backend/LLM are
  down, fall back to the current local validators + static hints, no broken play.

⚠️ **Confirm:** is the LLM in-scope for this slice, or static-hints-first (LLM deferred within the
slice)? And the prototype backend runtime/host (local Node assumed).

## C. Act 3 node-graph UX redesign direction

Fixes the M0-T24 "not fun / unclear / fiddly" debt — interaction/visual only; the M0-T21 validator and
M0-T22 session stay the source of truth.

- **Clear objective:** frame the level in-story ("Help Ghost answer the visitor looking for something")
  and state the goal plainly: Ghost should *answer* when it knows the room and *ask which room* when it
  doesn't.
- **Free-form connecting:** drag a wire from a node's **output port** to another node's **input port**
  (SlotCheck has two labelled outputs: present / missing); reject self-loops and invalid endpoints.
- **Placement:** drag nodes from the palette onto the canvas; nodes show ports.
- Keep deterministic validation unchanged.

## D. Task breakdown (vertical slice) — proposed, sequence flexible

- **M0-T26** — Narrative content + flow for Acts 1–3: Game Shell intro/debrief beats, Lily/Ghost lines,
  narrative state, act ordering (frontend, data-driven, reuses `LilyDialogueFrame`).
- **M0-T27** — Backend + DB foundation: Node/TS REST + SQLite schema + content/progress/attempts
  endpoints (local); client API client with graceful degradation. (Design → scaffold.)
- **M0-T28** — Client ↔ backend integration for progress + attempt logging across Acts 1–3 (behind
  graceful degradation).
- **M0-T29** — LLM orchestration (Lily hints / Ghost responses) via the backend, with static-hint
  fallback. (Only if LLM is in-scope for the slice.)
- **M0-T30** — Act 3 node-graph UX redesign (ports + wire-drag + clear objective).

(Numbers/sequence are flexible; narrative (M0-T26) and the backend design (M0-T27) can proceed in
parallel. Acts 4–8 resume after the slice reaches a certain completeness.)

## Decisions + open confirmations (updated 2026-06-22)

Decided (user):
- **LLM:** static-hints-first; LLM integration deferred to later in the slice (M0-T29). Deterministic
  validators stay authoritative throughout.
- **Execution order:** **M0-T30** Act 3 node-graph UX redesign (Section C — done; wired Validate) →
  **M0-T31** Act 3 Game Shell integration → narrative (M0-T26) → backend+DB (M0-T27) → client
  integration (M0-T28) → LLM (M0-T29).

Still open (settle when the narrative task M0-T26 starts):
- Narrative setting (lab vs lightly haunted lab), protagonist identity/name, and the game title.
- Backend runtime/host for the prototype (local Node assumed).
