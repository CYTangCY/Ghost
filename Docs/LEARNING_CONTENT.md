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

## Intended Act Structure (revised roadmap — 2026-06-20)

This is the current working Act sequence (user-directed revision; see `Docs/ROADMAP.md`). It refines
the structure in `Docs/CONFIRMED_PROJECT_CONTEXT.md` §5 (to be reconciled there). For each Act: what
the player does → the chatbot/NLP concept it teaches → how it connects to earlier Acts.

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

TBD.

### Cute Ghost Communication Problem

TBD.

### Puzzle Mechanic

Likely:
node assembly or flow diagram construction.

### Implementation Priority

Medium.

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