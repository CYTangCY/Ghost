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

## Act 0: Chatbot Fundamentals

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

## Act *: Supplementary NLP Pipeline

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

## Act 3: Dialog

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