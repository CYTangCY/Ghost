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

### User-Approved Story / Teaching Structure Override (2026-07-15)

The playable structure is now explicit:

- **Chapter 0 - Opening Story.** A narrative prologue only. It introduces the late-night lab, Lily,
  the player as Lily's junior, Ghost's garbled voice, and the reason they begin helping. It is not a
  chatbot lesson and has no puzzle score.
- **Chapters 1-6 - Teaching Chapters.** Intent classification, entity extraction, dialogue
  management, confidence/fallback, testing/debugging, then backend action/response generation.
  Every numbered teaching chapter has deterministic gameplay and visible consequences.
- **Final Chapter - Repair Ghost's Voice.** The five-component pipeline is the final integration
  interaction. It reuses every lesson, then owns Ghost's restored voice, player-name thank-you, Lily's
  closing beat, credits, and title return.

This 2026-07-15 user decision supersedes the temporary 2026-07-04 ship-plan wording that treated the
capstone and ending as Chapter 6. The optional Game Shell 'Ghost's Voice Basics' remains a reference
overview; it is not Chapter 0. The former internal Act6Pipeline class/file names may remain temporarily
to preserve imported Unity asset identities, but their player-facing role is the Final Chapter.

- **Chapter 1 - Intent Classification.** Player groups messages by purpose.
- **Chapter 2 - Entity Extraction.** Player marks the important details inside requests.
- **Chapter 3 - Dialogue Management.** Player builds the reply map and follow-up flow.
- **Chapter 4 - Confidence and Fallback.** Player tunes confidence and safe fallback/handoff routes.
- **Chapter 5 - Testing and Debugging.** Player previews, tests, repairs, and reruns the reply map.
- **Chapter 6 - Backend Action and Response Generation.** Player connects a stored-data source,
  chooses the action that retrieves the needed deterministic fact, and places the response template
  that turns that fact into a complete reply.
- **Final Chapter - Full-System Integration.** Player reconnects UI input, NLP engine, dialogue
  management, response generation, and UI output, with backend integration as a side link, then sees
  the authored ending story.

M0-T35 implementation note (2026-06-27): the Game Shell now includes a compact playable
"Ghost's Voice Basics" overview that covers chatbot definition, NLP/ML pillars, rule-based vs
AI-enabled contrast, benefits, the five-component overview, and the four challenges through
Ghost problem -> Lily explanation -> small player action -> visible Ghost consequence.

### IBM Course → Act Teaching Coverage (2026-06-25)

Per the M0-T34 coverage map (`Docs/IBM_COURSE_CONTENT.md`) and the user-corrected goal (the game must
TEACH the course content), the existing 8-Act structure is the vehicle that teaches the whole course —
modify the existing acts; do not invent a parallel structure. The five chatbot components AND the NLP
subtasks are woven in as below; teaching stays playable (problem → Lily explanation → player action →
visible consequence; no lecture/quiz dump).

- **Fundamentals (Game Shell intro — M0-T35):** chatbot definition; NLP & ML pillars; rule-based vs
  AI-enabled; benefits; five-components OVERVIEW; four-challenges overview. (Orientation only; depth is
  in the acts.) M0-T35 implements this as a playable Shell sequence, not a lecture or multiple-choice quiz.
- **Act 1 — Intent:** NLP engine (intent classification); ties to "identify key topics / user purpose."
- **Act 2 — Entity:** NLP engine (NER) + **tokenisation** (word chips, made explicit); system vs custom
  entities; synonyms.
- **Act 3 — Dialogue management:** dialogue-management component; rule-based / decision-tree flow; slots
  (entities) + **contextual-awareness** challenge; response nodes (→ response generation); preview/test.
- **Act 4 — Confidence & Fallback:** confidence scoring; fallback / disambiguation; **handoff /
  escalation**; **sentiment analysis** feeding routing/escalation; misunderstanding + unstructured-input
  challenges.
- **Act 5 — Testing & Debugging:** the course's preview / test / revise workflow.
- **Act 6 — Integration / Backend / Response generation:** backend-integration + response-generation
  components.
- **Act 7 — NLP Pipeline Lab:** NLP subtasks breadth — **POS tagging, sentiment analysis** (in depth),
  tokenisation / NER recap, **machine translation** (light / optional), NLU/NLG/speech recognition
  (conceptual).
- **Act 8 — Capstone "Repair Ghost's Voice":** reconnect the five components (UI → NLP engine →
  Dialogue management → Response generation → UI, + Backend integration) as one pipeline — the **UI
  component** and full-system integration are taught here.
- **Planning** (goal / starting channel / key topics / handoff): a short planning mini-level (M0-T39),
  or folded into the shell / Act 1.

Gap order (fundamentals first): in-game fundamentals (M0-T35) → explicit concept teaching added to
Acts 1–3 (M0-T36–T38) → planning (M0-T39) → Acts 4–8 incl. the NLP subtasks in Act 7 and the
UI/component integration in Act 8. Machine translation and speech recognition are light/optional.

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

> M0-T35 update (2026-06-27): the shell-level version is now a short playable overview, while this
> section remains the source for the later full capstone treatment.

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

The player should understand that intent means the speaker's purpose: what the speaker wants, not the
exact words used. Different messages can share one intent when they express the same purpose, and the
differently worded cards inside an intent group are training examples / example utterances a chatbot
could use to learn that intent. Grouping intents also previews the chatbot-planning step of identifying
common visitor requests / key topics.

### Cute Ghost Communication Problem

Ghost reacts to the wrong purpose behind a message.

For example, Ghost sees a message asking for help finding something but reacts as if the person is asking for identity or location.

### Puzzle Mechanic

Drag-and-drop classification.

### Player Action

The player groups message cards by intent — what the speaker wants (their purpose) — not by exact wording.
The differently worded cards gathered into each group are treated as that intent's training examples.

### Success Consequence

Ghost understands the intended purpose and gives a more appropriate cute response. The feedback makes
the learning explicit: each group is one shared purpose, and the varied message wordings in that group
are training examples a chatbot could learn from.

### Failure Consequence

Ghost responds to the wrong purpose in a cute but confused way.

### Lily Hint Style

Lily should guide the player toward purpose-based grouping, varied wording, and training examples
without solving the card placements. She can lightly connect the result to planning by noting that
grouping common requests helps choose a chatbot's key topics.

Example:
"Um... maybe don't look at the exact words first. What does the person want Ghost to do?"

### Implementation Priority

High.

### M0-T36 Teaching Implementation Note

Act 1 now teaches intent classification, not only practices it. The Act 1 presentation starts with a
visually distinct but compact Lily intent-note panel that frames Ghost's exact-word problem and
explains that intent is purpose rather than exact wording. Intent group titles and hints are phrased
as visitor purposes, and correct validation feedback changes into a visible success-teaching state
explaining that the grouped cards are multiple example phrasings / training examples for the intents,
with a small Ghost success reaction and one Lily planning-link line. The teaching UI is kept compact so
the existing card groups, validation controls, and banter remain inside a 1080p Play Mode view.

### M0-T45 Run 001 Teaching-as-Gameplay Redesign Note

The M0-T36 text-layer approach is superseded as the primary Act 1 teaching method. Act 1 is redesigned
as `Train Ghost to Greet Visitors`: the player first watches Ghost fail by matching exact wording, then
free-clusters the nine visitor transcript cards into training piles, assigns purpose labels to the
piles, and presses `Teach Ghost` to test the piles on authored unseen visitor messages. Ghost's reply
is chosen by a deterministic plurality rule over the player's related training cards, so scattered,
unlabelled, or wrongly labelled piles visibly make Ghost confused or wrong; revising the piles and
reteaching changes Ghost's behaviour. Completion still comes from the existing
`IntentClassificationValidator`, while the new demo engine only shows the consequence of the player's
training data. A shared programmatic Ghost face now shows neutral / happy / confused / sad moods for
the teaching-as-gameplay demo phases.

### M0-T46 Run 001 Experience Wrapper Note

Act 1 now opens with a separate Lily onboarding beat before the transcript controls become available.
Her three short lines preview the playable loop: watch Ghost fail, cluster and label training piles,
teach Ghost, and check new visitors. A persistent objective strip then follows the existing phases
without revealing card placements: watch the failure, build and label the piles, then teach and check.
To match the complete Act 2 transition, the onboarding screen also shows Ghost's exact-word problem,
and the level keeps a compact Lily note with `Replay Lily` after onboarding is dismissed.
Its page composition now follows the same Act 2 hierarchy: header/progress, objective, onboarding or
Lily note, Ghost conversation, then the flexible puzzle body.
The intent validator, sample data, generalization demo, and completion rule are unchanged.

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

### M0-T37 Teaching Implementation Note

Act 2 now teaches entity extraction / NER rather than only practicing chip tagging. The Act 2
presentation starts with a compact `Lily's Entity Note` panel that frames Ghost's problem as hearing a
whole sentence but missing the useful details, then explains that entity extraction / NER spots those
details after the message has been split into word tokens. The entity palette is labelled as entity
kinds and defines system entities versus custom lab-specific entities, with the real sample-data
`lab` / `laboratory` room synonym pair surfaced in the custom room entry. Correct validation feedback
now makes the consequence explicit: the tagged spans are the message's key details, synonyms can map
different wordings to one room entity, and the word chips are tokens that become the entity details
inside an Act 1 intent and later slots. The deterministic validator, session, sample data, answer keys,
and span-tagging mechanic remain unchanged.

### M0-T45 Run 002 Teaching-as-Gameplay Redesign Note

The M0-T37 text-layer approach is superseded as the primary Act 2 teaching method. Act 2 is redesigned
as `Ghost's Errand`: Lily first gives a short how-this-level-works beat, then the player watches Ghost
fail an authored errand, presses `Split` to turn the message into word-token chips, drags or clicks
tokens into Ghost's typed action-card slots, and presses `Go, Ghost!` to see the errand succeed or fail
from those slots. Slots are derived from the existing sample message entity types: `object` becomes
WHAT, `room` becomes WHERE, and `time` becomes WHEN. WHAT and WHERE use custom lab vocabulary chrome;
WHEN uses system entity chrome. Each errand outcome is authored static data, but correctness still
comes from the existing `EntityExtractionSession` and `EntityExtractionValidator`. The `lab` /
`laboratory` synonym beat is shown through slot resolution text such as `laboratory -> lab room`.
Ghost's shared face changes mood for intro failures, slot mistakes, and successful errands, so the
learning content is visible as Ghost's behaviour rather than a success lecture.

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

### M0-T46 Run 001 Experience Standard Note

Act 3 now opens with Lily's short reply-map loop before graph interaction. It connects the earlier
Acts in play: an intent routes a branch, an Act 2 detail fills a slot check, and a response card gives
Ghost its next line. A persistent objective strip moves from assembling the map to revising/testing
it. The onboarding pairs Lily's explanation with a visible Ghost reply-order problem, then changes to
a compact replayable Lily note when the graph unlocks. Deterministic validation alone drives Ghost's
face: empty or incomplete submissions are sad,
wrong structures or failed test routes are confused, and a full pass is happy. Failed validation
keeps its detail visible while the graph remains editable and the action becomes `Try again`; success
shows `Complete Act` and returns through the existing Shell debrief path. The graph session,
simulator, validator, sample data, and test cases are unchanged.

The Act 3 page uses the same top-level layout as Act 2: header/progress, objective, onboarding or Lily
note, a persistent Ghost conversation/result panel, then the graph body. Ghost's face and deterministic
test consequence live in that conversation panel rather than competing with the graph Guide column.

---

## Act 4: Confidence and Fallback

### Confirmed Topic

- confidence scoring
- threshold calibration
- fallback / clarification
- human handoff / escalation
- sentiment as an authored routing signal
- fallback design

### Learning Objective

The player should understand that a confidence score represents how certain a chatbot is about the
intent it detected, and that a confidence threshold controls whether it answers or uses a fallback.
A threshold that is too low makes Ghost bluff at uncertain or unstructured messages; one that is too
high makes Ghost reject clear requests. The player should also understand that a fallback asks for a
safer rephrasing, while a complex or upset request may need a planned human handoff. Sentiment is used
only as one authored routing signal for that escalation, never as correctness or scoring.

### Cute Ghost Communication Problem

Ghost's confidence dial is broken. Ghost either answers messages it barely understands with cheerful
but wrong certainty, or freezes and asks everyone to repeat themselves. One frustrated visitor has a
complex problem that Ghost should pass to Lily instead of trying to solve alone.

### Puzzle Mechanic

Slider calibration plus simple node wiring. The player tunes one 0-100 confidence-threshold slider,
attaches a `Fallback` route that asks uncertain visitors to rephrase, attaches a `Handoff` route that
calls Lily for the authored complex/upset case, then chooses `Run the day` to play the visitor queue.

### Player Action

The player balances the confidence threshold, connects both safety routes, and watches six authored
visitor messages travel through Ghost's reply map. Each message has a fixed confidence score and
authored routing expectation. The player revises the dial or wiring after seeing the consequences and
runs the day again.

### Success Consequence

With the threshold inside the authored acceptable range and both routes connected, Ghost answers the
clear requests, asks uncertain or garbled visitors to rephrase, and calls Lily for the hard frustrated
case. Lily handles that visitor, Ghost finishes the day happily, and the deterministic day-run
validator allows the Act to complete.

### Failure Consequence

- Threshold too low: Ghost confidently gives a cute but wrong answer to the garbled message.
- Threshold too high: Ghost asks even clear visitors to rephrase, and the queue becomes annoyed.
- Missing fallback: Ghost has no safe route for uncertain input.
- Missing handoff: Ghost tries the hard frustrated case alone and melts down.
- Any mismatched day outcome keeps the result visible, leaves the controls editable, and offers
  `Try again`.

### Lily Hint Style

Lily stays slightly nervous and practical. She asks the player to compare what happened at the two
ends of the dial and to notice which message needs another person, without stating the acceptable
range or wiring solution. Her handoff line lightly connects to chatbot planning: a good system plans
what to do when a request is too complex.

Example:
"Um... the dial isn't meant to make Ghost brave. It's meant to decide when Ghost knows enough to
answer. Maybe watch who gets a real reply, and who should get a safer way out...?"

### Implementation Priority

High for the six-chapter ship plan; implemented as M0-T47.

---

## Act 5: Testing and Debugging

### Confirmed Topic

- chatbot preview and testing
- test conversations
- expected versus actual responses
- debugging and revision
- regression-style reruns after a graph change

### Learning Objective

The player should understand that a chatbot which looks complete still needs to be previewed and
tested before it can be trusted. A test conversation supplies a known input and expected response;
running several cases exposes different faults that one successful example can miss. The player
should inspect expected-versus-actual results, trace failures back to dialog-graph wiring, revise the
graph, and rerun the full suite to confirm that the fixes did not break another conversation.

### Cute Ghost Communication Problem

Ghost's reply map looks finished, but its rehearsal visitors receive confidently mismatched answers.
One visitor gives a room and is asked for it again, one asks about lab hours and hears an unrelated
reply, and Ghost cannot greet a new visitor at all. Lily asks the player to run every rehearsal before
letting Ghost handle the real queue.

### Puzzle Mechanic

Test-run and graph-repair loop. The level begins with a pre-built Act 3-style dialog graph containing
three authored wiring faults: swapped room-present/room-missing routes, a lab-hours branch connected
to the wrong response, and a missing start connection for the greeting intent. The player chooses
`Run all tests`, reads each failing card's visitor message plus expected and actual reply, then uses
the existing Act 3 wire-drag interaction to reconnect the faulty graph outputs.

### Player Action

The player previews four authored conversations, runs the full deterministic suite, compares red
expected-versus-actual results, and repairs the graph. After each edit the previous results remain a
diagnostic reference until the player reruns all tests. The player repeats the test / inspect / revise
loop until all four cases are green.

### Success Consequence

When `DialogGraphValidator` and `DialogGraphSimulator` produce the expected response for every
authored conversation, Ghost rehearses the complete queue correctly, the conversation panel shows a
happy response, and the player may complete the Act through the Shell debrief path.

### Failure Consequence

- The first run exposes the seeded faults rather than hiding them behind a generic error count.
- Each failed case shows its visitor message, expected reply, and actual reply or `no response`.
- A partial repair can turn some cards green while the remaining failures stay red.
- Rerunning the whole suite catches regressions; the Act cannot complete while any case fails or the
  graph has structural validation errors.

### Lily Hint Style

Lily is practical and a little hesitant. She points the player toward the first mismatch and asks
which wire could have produced that actual reply, without naming the destination node or completing
the repair. She reinforces that previewing once is not enough: after changing the graph, run every
conversation again.

Example:
`Um... the map can look tidy and still send someone to the wrong answer. Could we compare what that
visitor expected with where the wire actually took them, then run all four again...?`

### Implementation Priority

High for the six-chapter ship plan; implemented as M0-T48.

---

## Chapter 0: Opening Story

> User-approved structural mapping added 2026-07-15. This chapter uses the confirmed premise and tone
> from NARRATIVE.md; it does not add a new academic lesson.

### Story Purpose

Introduce the player as Lily's junior during a late shift in the lightly haunted research lab. Ghost
tries to speak but produces garbled fragments. Lily, nervous but capable, asks the player to help one
message at a time. The sequence establishes why Chapters 1-6 exist before any teaching puzzle begins.

### Player Action

Advance through a short authored conversation between Lily and Ghost. The player's entered name is
used in Lily's lines. A visible Skip control reaches the same completed state. Finishing or skipping
marks Chapter 0 seen and opens the Shell lesson selection.

### Boundaries

Chapter 0 contains no quiz, validator, course definition, or puzzle score. It reuses only confirmed
premise facts: late lab, Lily as the player's postdoctoral senior, cute Ghost, garbled voice, and the
shared decision to help.

### Implementation Priority

Required story framing before the six teaching chapters.

---

## Chapter 6: Backend Action and Response Generation

> User-approved teaching mapping added 2026-07-15. This restores the earlier confirmed Act 6 course
> role and separates it from the Final Chapter capstone/ending.

### Confirmed Topic

- backend integration as the connection to stored or external information
- an action as the explicit operation that requests the needed fact
- a deterministic backend result as data, not a finished conversational reply
- response generation as the step that turns the result into natural language
- IBM SkillsBuild section 1.3 component responsibilities

### Learning Objective

The player should distinguish three responsibilities. A backend source stores facts. An action asks
that source for a specific fact. Response generation inserts the returned fact into a suitable reply.
The backend does not decide puzzle correctness and raw data is not yet Ghost's spoken answer.

### Cute Ghost Communication Problem

Ghost's tested dialogue route correctly recognizes a lab-hours request, but then it either asks the
wrong records for data, performs the wrong lookup, or blurts the raw value '8 PM' without a sentence.
The player must finish the route so Ghost can fetch the fact and phrase it clearly.

### Puzzle Mechanic

A drag/click socket board with three stable roles:

1. DATA SOURCE - connect the authored Lab records backend.
2. ACTION - place Fetch lab closing time.
3. RESPONSE - place The lab closes at {closing_time}.

The palette also contains authored distractors for a room-directory source, object-location action,
and mismatched response. Placement remains editable and is checked only by a new deterministic
Act6BackendResponseValidator.

### Player Action

1. Follow the already-tested lab-hours route from Chapter 5.
2. Drag or click one card into each role socket.
3. Run the route.
4. Observe the action request, backend result closing_time=8 PM, and template substitution as separate
   playback stages.
5. Revise the first incorrect role and rerun until Ghost says the complete reply.

### Success Consequence

The Lab records source receives Fetch lab closing time, returns the authored value 8 PM, and response
generation produces 'The lab closes at 8 PM.' Ghost speaks clearly, Chapter 6 completes through the
normal Shell debrief path, and the Final Chapter remains a separate story/capstone destination.

### Failure Consequence

- Wrong source: the requested lab-hours field is unavailable.
- Wrong action: the backend returns a fact for another route.
- Wrong response template: Ghost has the right fact but phrases the wrong answer.
- Empty role: playback stops before the missing responsibility.

The run names the first broken role, shows its consequence, and returns to the editable board.

### Lily Hint Style

Lily asks which system owns the fact, which action requests it, and which sentence should contain it.
She does not state all three card names at once.

Example:
'Um... the records hold the fact, but they still need a precise request. And even after the value
comes back, Ghost needs a sentence around it.'

### Deterministic Correctness

Act6BackendResponseValidator compares the three placed ids with authored expected ids. Backend and LLM
services may log or decorate the run, but they never score or gate completion.

### Implementation Priority

Required sixth teaching chapter.

---

## Final Chapter: Repair Ghost's Voice

> User-approved finale mapping added 2026-07-15. This reclassifies the implemented M0-T49
> five-component pipeline and ending; it is no longer player-facing Chapter 6.

### Confirmed Topic

Full-system integration of all six teaching chapters, followed by the ending story.

### Integration Interaction

The board shows a fixed Visitor message and Ghost reply, five editable main stages, and one backend
side socket. Twelve concise cards remain available at the same time: five learned skills, five
plausible shortcuts, and two backend actions. Each card shows only its name and one short description,
so the player can compare and arrange the whole route without reading a chapter explanation on every
card. Cards can be dragged or selected and placed, and occupied stages can be swapped.

Lily uses the same draggable portrait panel as Chapters 1-3. Her line changes when the player selects,
places, misplaces, or resets a card. The reaction describes the responsibility or risk without marking
the position correct. Ask Lily opens the existing chat window with the current five-stage route,
backend action, and latest visitor-test evidence.

Selecting **Run all 3 tests** sends a greeting, a missing-room request, and the lab-hours request
through the assembled path. Each case shows its expected and actual reply. Different faults affect
different cases, so a partial repair can pass some cards while others remain red. Changing a card
makes the previous results stale, and all three cases must be rerun together.

### Story Consequence

The authored final lab-hours visitor message travels stage by stage. Ghost gives its complete reply,
becomes happy and glowing, thanks GhostNarrativeState.PlayerName, Lily gives her proud closing line,
credits scroll, and the game returns to the title. Full playback and Skip ending reach the same final
state.

### Separation Rule

The Final Chapter may reuse learned concepts but does not replace Chapter 6 teaching. Chapter 0 owns
the opening story, Chapters 1-6 own curriculum, and only the Final Chapter owns the final credits.

### Implementation Priority

Required final story and capstone after the six teaching chapters.
