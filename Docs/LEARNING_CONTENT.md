# LEARNING_CONTENT.md

## Purpose

This file maps each confirmed Act to:
- IBM SkillsBuild chatbot / NLP topic
- learning objective
- cute Ghost communication problem
- puzzle mechanic
- player action
- success consequence
- failure consequence
- Lily hint style

Do not implement a level before its mapping is written here.

---

## Intended Act Structure (revised roadmap — 2026-06-20; full-system note 2026-06-22)

This is the current working Act sequence (user-directed revision; see `Docs/ROADMAP.md`). It matches
the structure in `Docs/CONFIRMED_PROJECT_CONTEXT.md` §5 (reconciled to the 8-Act structure). For each
Act: what the player does → the chatbot/NLP concept it teaches → how it connects to earlier Acts.

Full-system note: backend, database, and LLM are required final-system components that later support
hints, Ghost responses, progress, attempt logging, and the capstone simulation — but every Act's
puzzle correctness stays deterministic (validators, graph simulation, test cases, or backend scoring),
never decided by the LLM. The per-Act "Systems later" notes below say how each Act touches those
systems.

- **Act 1 — Intent Classification.** Player groups message cards by purpose. Teaches that different
  wording can share one intent. (Foundation.)
- **Act 2 — Entity Extraction.** Player tags phrases/chips and assigns entity types. Teaches that a
  chatbot must pull key details (names/places/times/objects), synonyms, and system vs custom
  entities. Connects to Act 1: intent = what is wanted; entity = the details inside that request.
- **Act 3 — Dialog Management via Node Graph (flagship).** Player assembles a dialog node graph
  (nodes, branching, slots, responses, context). Teaches how a conversation is structured. Connects
  to Acts 1–2: intents trigger nodes and entities fill slots/conditions. The node graph is the
  flagship mechanic, reused/extended in Acts 4–6.
- **Act 4 — Confidence and Fallback.** Player tunes thresholds and adds fallback/disambiguation
  nodes in the graph. Teaches confidence scoring and graceful failure. Extends the Act 3 graph.
- **Act 5 — Testing and Debugging.** Player runs test conversations through the graph and fixes
  faults. Teaches chatbot testing/debugging. Extends the Act 3 graph.
- **Act 6 — Integration / Backend Action / Response Generation.** Player adds backend/action and
  response-generation nodes. Teaches how a bot fetches data and forms a reply. Extends the Act 3
  graph.
- **Act 7 — NLP Pipeline Lab.** Tokenisation, POS tagging, NER, sentiment (the former optional
  Act *). Supplementary lab that supports the concepts behind earlier Acts.
- **Act 8 — Capstone / "Repair Ghost's Voice".** Player reconnects the five chatbot components into
  a working pipeline (the former Act 0 mechanic). Teaches how the whole system fits together; the
  integration demo that ties every prior Act's concept into one playable whole.

Fundamentals note: the former Act 0 (chatbot definition, rule-based vs AI-enabled, five components,
four challenges) is preserved — its concepts are introduced by Lily in the Game Shell, and its
"Rebuild Ghost's Voice" pipeline mechanic becomes the Act 8 capstone. The detailed per-Act sections
below still use the earlier numbering and are being migrated to this structure.

### Per-Act Backend / Database / LLM Interaction (added 2026-06-22)

High-level only; see `Docs/ARCHITECTURE.md` Phase D layers. In every Act the correctness check is
deterministic; the LLM only adds hints/responses/explanations.

- **Act 1 — Intent.** Correctness: deterministic intent validator. Later systems: backend logs
  attempts; database stores progress; LLM provides Lily hints / Ghost responses.
- **Act 2 — Entity.** Correctness: deterministic span/type validator. Later systems: same as Act 1;
  LLM may also explain why a span/type was wrong.
- **Act 3 — Dialog Node Graph.** Correctness: deterministic graph simulation. Later systems: backend
  may host graph simulation/scoring; LLM generates natural-language responses for graph outputs.
- **Act 4 — Confidence and Fallback.** Correctness: deterministic threshold/fallback checks on the
  graph. Later systems: backend scoring; LLM explains threshold trade-offs.
- **Act 5 — Testing and Debugging.** Correctness: deterministic authored test cases run through the
  graph. Later systems: backend runs the test suite and returns pass/fail; LLM explains failures.
- **Act 6 — Integration / Backend Action / Response Generation.** Correctness: deterministic checks
  on expected backend-action results and response selection. Later systems: backend action nodes call
  services; LLM does response generation around those deterministic results.
- **Act 7 — NLP Pipeline Lab.** Correctness: deterministic per-step pipeline checks. Later systems:
  LLM optional (illustrative outputs only).
- **Act 8 — Capstone "Repair Ghost's Voice".** Correctness: deterministic end-to-end pipeline
  validation. Later systems: backend orchestrates; LLM drives the chatbot simulation; database/logs
  capture the capstone run.

---

## Act 0: Chatbot Fundamentals

> Revised mapping (2026-06-20): the fundamentals concepts here are now introduced by Lily in the
> Game Shell, and the "Rebuild Ghost's Voice" pipeline mechanic below is re-planned as the **Act 8
> capstone**. This section is retained for that reuse.

### Confirmed Topic

- chatbot definition
- rule-based vs AI-enabled chatbot
- five components
- four challenges

### Learning Objective

The player should understand what a chatbot is, tell a rule-based chatbot apart from an AI-enabled one, name the five chatbot components, and recognise the four common chatbot challenges.

The five components (IBM SkillsBuild wording):
1. User interface (UI): Facilitates interaction between the user and the chatbot
2. NLP engine: Interprets and processes the user's input
3. Dialogue management system: Decides on the appropriate response
4. Response generation module: Generates appropriate responses
5. Backend integration: Allows the chatbot to fetch additional data to provide accurate information

The four challenges (IBM SkillsBuild wording):
1. Handling unstructured data
2. Misunderstanding queries
3. Providing human-like interaction
4. Contextual awareness

### Cute Ghost Communication Problem

Ghost is a cute ghost whose communication process has become disconnected. The pieces Ghost needs to understand a message and reply have come apart, so Ghost's messages come out broken or jumbled. Each disconnected piece maps to one of the five chatbot components, and the four chatbot challenges appear as cute failure cases when parts are missing, misordered, or poorly connected.

### Puzzle Mechanic

Flow diagram construction. (Act 0 design: Option A — "Rebuild Ghost's voice".)

### Player Action

The player arranges and connects the five components into a working communication pipeline:

UI → NLP engine → Dialogue management system → Response generation module → UI

with Backend integration connected where extra information is needed.

### Success Consequence

When the pipeline is connected correctly, Ghost produces one clear, cute response because the communication process works end to end.

### Failure Consequence

Ghost's response breaks at the wrong or missing stage, in a cute but confused way:
- UI missing: Ghost cannot receive or show the message clearly.
- NLP engine wrong: Ghost hears the words but does not interpret them.
- Dialogue management system wrong: Ghost understands the input but chooses the wrong next step.
- Response generation module wrong: Ghost knows what to say but says it awkwardly.
- Backend integration missing: Ghost cannot fetch the extra information it needs.

These broken pipelines are how the four chatbot challenges show up in play — handling unstructured data, misunderstanding queries, providing human-like interaction, and contextual awareness — so the player feels each challenge as a concrete, cute failure rather than reading it as a definition.

### Lily Hint Style

Lily sounds nervous, nerdy, technically capable, and slightly deferential. She does not lecture. She helps the player notice that Ghost's communication parts are disconnected, without naming the exact fix.

Example:
"Um... I don't think Ghost is broken-broken. It's more like... the parts that let it understand and reply got unplugged from each other? Maybe we line them up in the order a message would actually travel...?"

### Implementation Priority

High.

---

## Act 1: Intent

### Confirmed Topic

- intent classification
- training examples

### Learning Objective

The player should understand that different messages can share the same intent if they express the same purpose.

### Cute Ghost Communication Problem

Ghost reacts to the wrong purpose behind a message.

For example, Ghost sees a message asking for help finding something but reacts as if the person is asking for identity or location.

### Puzzle Mechanic

Drag-and-drop classification.

### Player Action

The player groups message cards by intent — what the speaker wants (their purpose) — not by exact wording.

### Success Consequence

Ghost understands the intended purpose and gives a more appropriate cute response.

### Failure Consequence

Ghost responds to the wrong purpose in a cute but confused way.

### Lily Hint Style

Lily should guide the player toward purpose-based grouping.

Example:
"Um... maybe don't look at the exact words first. What does the person want Ghost to do?"

### Implementation Priority

High.

---

## Act 2: Entity

### Confirmed Topic

- entity extraction
- synonyms
- system entities vs custom entities

### Learning Objective

The player should understand that a chatbot needs to identify important details (entity extraction) such as names, locations, times, and objects; that different words or spellings can refer to the same entity (synonyms); and that some entities are built-in system entities (for example time) while others are custom entities defined for this situation (for example a specific room or object).

### Cute Ghost Communication Problem

Ghost understands the general purpose but misses key details.

For example, Ghost knows someone wants help, but misses which room, object, name, or time matters.

### Puzzle Mechanic

Span annotation with entity typing.

### Player Action

The player highlights the important span(s) in a message and assigns each span an entity type, such as:
- system entity
- custom entity
- location / room
- object
- time
- name

This covers entity extraction (finding the detail), synonyms (different words that map to the same entity), and the difference between system entities (built-in types such as time) and custom entities (game-specific types such as a particular room or object).

### Success Consequence

Ghost uses the correct detail and responds more clearly.

### Failure Consequence

Ghost gives an incomplete or wrong response because it missed the required detail.

### Lily Hint Style

Lily should point out that Ghost understood the broad meaning but missed the useful detail.

Example:
"I think Ghost knows what kind of request this is... but it lost the important part. Maybe the name or place matters here."

### Implementation Priority

High.

---

## Act *: Supplementary NLP Pipeline (revised: Act 7 — NLP Pipeline Lab)

### Confirmed Topic

- tokenisation
- POS tagging
- named entity recognition
- sentiment analysis

### Status

Supplementary.

May be implemented in prototype if needed, but excluded from primary Evaluation 2 and Evaluation 3 scope.

### Learning Objective

TBD.

### Cute Ghost Communication Problem

TBD.

### Puzzle Mechanic

TBD.

### Implementation Priority

Medium / optional.

---

## Act 3: Dialog (Dialog Management via Node Graph — flagship mechanic)

> Revised mapping (2026-06-20): Act 3 is the flagship **node graph** gameplay, built after Act 2 so
> intents and entities can act as triggers/slots inside dialog nodes; Acts 4–6 extend this graph.

### Confirmed Topic

- dialog nodes
- branching
- slots
- response types
- context variables

### Learning Objective

The player should understand dialog management: a chatbot decides its next reply by following a
structured conversation flow. The detected intent routes the conversation (branching); required
entities act as slots that must be filled before the bot can answer; context variables remember the
details already collected; and the right response type is produced at the right step. Different inputs
follow different paths.

### Cute Ghost Communication Problem

Ghost can now tell what people want (intent, Act 1) and catch the key details (entities, Act 2) — but
Ghost's replies come out in the wrong order. Ghost answers before it has the information it needs, or
responds to the wrong step (says goodbye to a greeting; gives an answer before learning which room).
Ghost's conversation has no map. The player builds Ghost a small conversation map (a dialog node
graph) so Ghost follows the right steps: work out what is wanted, check it has the needed detail, ask
if something is missing, then reply.

### Puzzle Mechanic

Node assembly (flow / graph construction): the player assembles and configures a small dialog node
graph from a node palette.

### Player Action

Given a target conversation (a few test messages, each with its intent + entities already detected,
and the expected Ghost behaviour), the player:
- places and connects dialog nodes from a palette (start, intent branch, slot check / ask, response);
- sets each branch node's triggering intent, each slot node's required entity type, and each response
  node's reply;
- so that simulating each test message through the graph makes Ghost reach the expected response — and
  ask for a missing slot when the entity is absent — using context to remember collected details.

### Success Consequence

When the graph is correct, the simulation drives Ghost to respond appropriately: it follows the right
branch for each intent, asks for a missing detail instead of guessing, remembers it, and gives the
right reply. Ghost's conversation becomes coherent and in order.

### Failure Consequence

A wrong graph (wrong intent wired, missing slot check, wrong/duplicated response, or a dead-end /
unreachable node) makes Ghost answer out of order, ignore missing info, or reply with the wrong type —
cute but broken. The simulator reports which test message produced the wrong result.

### Lily Hint Style

Nervous, nerdy, competent, and non-spoiling. Example:
"Um... Ghost knows what they want and even caught the details, but it's... replying before it actually
has everything? Maybe there should be a step that checks the room is known before Ghost answers...?"

### Connection to Earlier Acts

Act 1 intents become the triggers that pick which branch fires; Act 2 entities become the slots a node
requires and fills (context remembers them). Act 3 is where intent + entity combine into a flow. The
same graph is extended later: Act 4 adds confidence thresholds + fallback nodes, Act 5 runs more test
conversations to debug the graph, and Act 6 adds backend-action + response-generation nodes.

### Deterministic Correctness

Correctness comes from a deterministic graph simulator/validator: each test conversation is run through
the assembled graph and checked against expected responses, plus structural checks (reachability, no
dead ends, every expected intent handled). The LLM never decides correctness; later it may only voice
Ghost's responses or Lily's hints. See `Docs/ARCHITECTURE.md` (Node Graph System) for the data model.

### Implementation Priority

High (flagship; ROADMAP Phase C).

---

## Act 4: Confidence and Fallback

### Confirmed Topic

- scoring
- threshold calibration
- disambiguation
- fallback design

### Learning Objective

TBD.

### Cute Ghost Communication Problem

TBD.

### Puzzle Mechanic

Likely:
slider calibration and fallback selection.

### Implementation Priority

Medium.

---

## Act 5: Testing and Debugging

### Confirmed Topic

chatbot testing and debugging.

### Learning Objective

TBD.

### Cute Ghost Communication Problem

TBD.

### Puzzle Mechanic

Likely:
ordered list, log review, or debugging puzzle.

### Implementation Priority

Medium.

---

## Act 6: Integration and Deployment

### Confirmed Topic

- integration
- deployment design
- configuration

### Learning Objective

TBD.

### Cute Ghost Communication Problem

TBD.

### Puzzle Mechanic

Likely:
form configuration or deployment design puzzle.

### Implementation Priority

Medium / late.

---

## Act 8: Capstone — "Repair Ghost's Voice" (revised roadmap)

### Confirmed Topic

Integration of all prior Acts: the five chatbot components working together as one pipeline.

### Player Action

The player reconnects the five components (UI → NLP engine → Dialogue management system → Response
generation module → UI, with Backend integration) into a working pipeline — reusing the former
Act 0 "Rebuild Ghost's Voice" mechanic as the final integration demo.

### Connection to Earlier Acts

Ties together intent (Act 1), entity (Act 2), dialog node graph (Act 3), confidence/fallback
(Act 4), testing (Act 5), and backend/response (Act 6): the player sees the whole system Ghost
needs to communicate clearly.

### Implementation Priority

Late / if time allows.