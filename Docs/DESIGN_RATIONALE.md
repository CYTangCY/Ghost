# DESIGN_RATIONALE.md

## Why a Cute Ghost?

Ghost is a cute ghost rather than a literal AI system because the project needs a character-driven game identity.

The ghost theme makes communication failure more playful and memorable. It also gives visible narrative consequences when the player repairs a misunderstanding.

The AI chatbot and NLP concepts remain the learning structure behind the gameplay.

## Why Not a Quiz Game?

A quiz can test recall, but it does not show how chatbot and NLP concepts work inside a communication system.

This project aims to turn concepts into puzzle mechanics. The player should manipulate a problem and observe how Ghost's response changes.

## Why Lily Exists

Lily provides human guidance.

She is not a generic tutor. She is the protagonist's nerdy and slightly timid postdoctoral senior from the lab.

Her role is to interpret Ghost's communication problems, provide hints, and connect the puzzle situation to the learning concept without directly giving the answer.

## Why Use Puzzle Mechanics?

Puzzle mechanics allow each concept to become an action.

Examples:
- intent classification becomes grouping messages by purpose
- entity extraction becomes marking important fragments
- dialog state becomes arranging conversation nodes
- confidence threshold becomes adjusting when Ghost should answer
- fallback becomes choosing what Ghost should do when confused

## Why Keep the Architecture Simple?

This is an MSc final project prototype, not a commercial game.

The code must be explainable, testable, and maintainable by the student.

The implementation should avoid overengineering.

## Why Build the Game Shell and Lily Before More Acts

Act 1 proved the core mechanic loop works, but a sequence of isolated puzzle scenes does not read as
a game. A small game shell (title, act select/hub, Lily dialogue frame, Ghost presence) gives the
prototype a frame: the player meets Lily, understands why they are helping Ghost, and enters Acts
from one place. Introducing Lily early also establishes the human guide voice that every later Act's
hints depend on. This is cheaper to add now than to retrofit after several Acts exist.

## Why Treat Act 1 as a Milestone Now

Act 1 already covers click-assign, drag-assign, bidirectional reassignment, unassign, Validate, and
feedback — enough to demonstrate the intent-classification concept end to end. Polishing it further
has diminishing returns compared with proving the game can grow (shell, second concept, flagship
mechanic). Act 1 visual polish is deferred, not abandoned.

## Why Act 2 Comes Before the Node Graph

The node graph (dialog management) is most learnable once the player already understands intent
(Act 1) and entity (Act 2), because intents and entities become the triggers, slots, and conditions
inside dialog nodes. Teaching the graph first would force those concepts to be explained abstractly
instead of reused.

## Why the Node Graph Is the Core / Flagship Mechanic

Dialog management is the heart of how a chatbot decides what to do, and a node graph makes that
structure directly manipulable and visible. It is also the most reusable mechanic: confidence,
fallback, testing, and backend/response can be expressed as additional node types rather than
separate mini-games.

## Why Acts 4–6 Should Extend the Graph

Reusing one deepening system (the node graph) keeps the player learning a coherent model instead of
many disconnected puzzle systems, and keeps the codebase smaller and more explainable (NFR1/NFR2).
Where a concept genuinely does not fit the graph, a separate small mechanic is acceptable, but the
graph is the default home.

## Why LLM, Backend, and Database Are Required (Not Optional)

The final artefact is a full AI-assisted educational game system, not only a Unity puzzle prototype.
The LLM gives Lily her scaffolded hints and drives the Act 8 capstone chatbot simulation; the backend
delivers content and orchestrates the LLM; the database holds learning/puzzle content, player
progress, and attempt logs. These are part of the system being built and assessed, so they are
required final-system components rather than optional extras. They are integrated after the gameplay
skeleton is stable (see the next note), and the older "LLM can be deferred" wording is now treated as
risk mitigation (graceful degradation to static hints), not permission to drop the component.

## Why the LLM Must Not Decide Correctness

Correctness must be reproducible, fair, and explainable (NFR1). LLM output is non-deterministic and
can hallucinate, so if it decided scoring the same submission could pass once and fail the next time,
and the result could not be justified to a marker. Therefore puzzle correctness comes only from
deterministic logic — validators, graph simulation, authored test cases, or backend scoring — and the
LLM is confined to hints, Ghost responses, explanatory feedback, and natural-language generation. This
also lets the game keep working (local validation + static hints) when the LLM/backend are
unavailable.

## Why Build the Gameplay Skeleton Before the Full System

Building the playable core first (Game Shell + Acts 1–3) de-risks the learning design — the part the
project is really about — before adding infrastructure. The backend, database, and LLM then wrap a
proven core instead of being built speculatively against gameplay that might still change. This keeps
early work small and explainable and avoids rework if a mechanic is revised.

## Why a Vertical Slice Before More Acts (2026-06-22; delivered by M0-T26–T33)

Once the Act 1–3 prototypes existed, continuing to add acts would have produced more thin, disconnected
screens. The priority shifted to making the first three cohere as a game (see
`Docs/VERTICAL_SLICE_PLAN.md`): weaving narrative (story, characters, Lily, scene transitions) through
Acts 1–3, standing up the full-system foundation (backend/database/LLM), and redesigning the Act 3
node-graph UX. A cohesive, reasonably complete slice is easier to demo, evaluate, and reason about than
eight half-built acts. The LLM was integrated static-hints-first (deterministic validators stay
authoritative), so the slice is fully playable without it. Delivered by M0-T30/T31 (Act 3 redesign +
shell link) and M0-T26–M0-T33 (narrative, banter, backend+DB, client sync + accounts, LLM hints, Lily
chat).

## Why the Act 3 Node-Graph UX Was Redesigned (M0-T30)

The first Act 3 interaction (M0-T24) worked mechanically but was not fun: connecting nodes via separate
From/To selection + condition buttons felt clerical, allowed nonsense self-loops, and the level gave no
in-story reason to build the graph. The redesign — drag-a-wire between node ports, a clear story
objective, and a wired deterministic Validate — makes the flagship mechanic feel like assembling a
conversation. This matters most for Act 3 because the node graph is reused and extended by Acts 4–6.
Correctness stayed deterministic (the validator/session are unchanged).

## Why the Game Teaches the IBM Course Content (2026-06-25)

The user-corrected goal is that players LEARN the IBM SkillsBuild curriculum by playing — not that the
dissertation merely "mirrors" the course, and not that mechanics get concept labels. The M0-T34
coverage map (`Docs/IBM_COURSE_CONTENT.md`) made the gap measurable: Acts 1–3 let players DO
intent/entity/dialog, but the game did not EXPLAIN them, and the course's fundamentals were absent.
Coverage work therefore follows the course's own order — fundamentals first, then strengthening the
acts that already exist, then breadth (Acts 4–8) — and every teaching addition keeps the playable
pattern: Ghost problem → Lily explanation → player action → visible consequence.

## Why a Fundamentals Overview Lives in the Game Shell (M0-T35)

The IBM course starts with fundamentals (chatbot definition, NLP+ML pillars, rule-based vs AI-enabled,
benefits, five components, four challenges), so the game needs them BEFORE the acts practice the
details. The shell hosts them as a short optional "Ghost's Voice Basics" sequence — six beats, each
with a small player action and a visible Ghost consequence — rather than a lecture or a quiz. It is
skippable so it never gates play, and it deliberately splits the former Act 0: the overview plants the
five-component concept early, while Act 8's capstone makes the same pipeline playable at depth.

## Why Acts Teach Through Their Own Feedback (M0-T36–T38)

The per-act teaching passes attach explanation to the player's own action: a compact in-fiction note
before the puzzle (Lily frames the concept) plus success feedback that explains WHY the solution is
right (e.g. Act 1: differently-worded messages share one intent, and those wordings are the intent's
training examples). Explaining at the moment of success lands better than front-loading a lecture,
keeps the no-quiz rule intact, and costs no mechanic changes — validators and sessions stay untouched,
so the deterministic-correctness guarantee is preserved.