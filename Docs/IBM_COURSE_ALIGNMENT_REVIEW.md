# IBM Course Alignment Review

Date: 2026-06-25

Source reviewed:
- `unorganized_data/Course_IBM_chatbot.pdf`

Note: the PDF is image-based, so this review is based on rendered page inspection rather than direct text extraction.

## Short Verdict

The current Ghost direction is still defensible, but it needs a clear alignment pass.

The prototype currently works best as:

> a narrative puzzle-game translation of selected IBM chatbot/NLP concepts

It should not be presented as:

> a complete playable reconstruction of the full IBM chatbot course

The main risk is not that Acts 1-3 are wrong. The risk is that the IBM course concepts are not visible enough in the game UI, narrative framing, or dissertation argument.

## What The IBM Course Emphasizes

From the PDF overview and key-concept pages, the course emphasizes these areas:

1. What a chatbot is.
2. Rule-based chatbots versus AI-enabled chatbots.
3. NLP as the way computers understand human language.
4. Machine learning as learning patterns from data.
5. Benefits of chatbots, such as efficiency and handling repetitive tasks.
6. Key chatbot components:
   - User interface
   - NLP engine
   - Dialogue management system
   - Response generation module
   - Backend integrations
7. Chatbot challenges:
   - Misinterpreting ambiguous or complex language
   - Maintaining context
   - Handling unstructured or unexpected input
   - Making conversation natural and less scripted
8. NLP subtasks:
   - Tokenization
   - Part-of-speech tagging
   - Named entity recognition
   - Sentiment analysis
   - Machine translation
9. Watsonx Assistant workflow:
   - planning a chatbot goal
   - identifying key topics
   - selecting a starting channel
   - using prebuilt/custom conversations
   - previewing/testing the chatbot

## Current Ghost Coverage

| IBM Course Concept | Current Ghost Coverage | Status |
|---|---|---|
| Chatbot definition | Implied by Ghost communication problems and Lily dialogue | Weak / needs explicit intro |
| Rule-based vs AI-enabled chatbots | Not currently represented as gameplay | Missing |
| NLP concept | Represented indirectly by Acts 1-2 | Present but should be labelled more clearly |
| Machine learning from data | Not strongly represented | Missing / possible future framing |
| Chatbot benefits | Not core to gameplay | Can be dissertation framing, not necessary as a mechanic |
| User interface component | Player UI exists, but not framed as a chatbot component | Weak |
| NLP engine component | Act 1 intent + Act 2 entity extraction map well here | Strong, but needs explicit labels |
| Dialogue management component | Act 3 dialog graph maps well here | Strong |
| Response generation component | Ghost responses + LLM response endpoint partially map here | Present but under-explained |
| Backend integration component | Backend/progress/attempt/LLM service exists technically | Present technically, weak as learning content |
| Misunderstanding queries | Act 1/2 failure states map well | Strong |
| Maintaining context | Act 3 context/slot logic maps partially | Medium |
| Unexpected/unstructured input | Lily chat and fallback can illustrate this later | Medium / needs framing |
| Natural conversation challenge | Lily chat and Ghost response generation map well | Medium / needs framing |
| Tokenization | Act 2 word chips imply tokenization | Present but not labelled |
| POS tagging | Not represented | Missing / future Act 7 |
| NER | Act 2 entity spans map well | Strong |
| Sentiment analysis | Not represented | Missing / future Act 7 or Act 4/5 extension |
| Machine translation | Not represented | Out of current prototype scope |
| Watsonx Assistant key topics | Act 1 intents roughly map to topics/purpose | Medium |
| Prebuilt/custom conversations | Act 3 node graph loosely maps to custom conversation flow | Medium |
| Handoff | Not represented | Missing / future fallback/escalation task |
| Preview/test chatbot | Act 3 "Test Ghost's map" maps well | Strong |

## Main Alignment Problems

### 1. The playable slice starts too deep

Acts 1-3 begin with intent, entity, and dialog flow. Those are valid chatbot/NLP ideas, but the IBM course first teaches what chatbots are, the difference between rule-based and AI-enabled chatbots, and the five chatbot components.

Current fix:
- Add a short in-game "Ghost's voice parts" intro before or inside the Game Shell.
- Do not turn it into a lecture. Make it a compact visual map:
  `UI -> NLP Engine -> Dialogue Manager -> Response Generator -> UI`, with backend integration as an optional side connection.

### 2. The IBM vocabulary is not visible enough

The current UI says things like "Reply Map", "Entity Types", and "Ask Lily", which is player-friendly. That is good. However, the dissertation and some in-game guide text should still reveal the course mapping.

Current fix:
- Keep player-facing cute labels.
- Add small subtitle/guide labels:
  - Act 1: "Concept: intent / user purpose"
  - Act 2: "Concept: entity extraction / NER"
  - Act 3: "Concept: dialogue management / conversation flow"

### 3. Rule-based vs AI-enabled is missing

The IBM course spends meaningful time on this contrast. The current prototype has deterministic validators plus LLM hints, but the player may not understand this as rule-based versus AI-enabled.

Current fix:
- Add a short Shell beat or optional mini-card:
  - "Some bots follow fixed rules."
  - "Some use NLP/ML to interpret language."
  - "Ghost's puzzles show the parts behind both."
- In dissertation: explain deterministic validators as the game scoring layer, while Lily chat/LLM is natural-language support only.

### 4. Watsonx Assistant workflow is underrepresented

The PDF includes a large practical section on setting up a chatbot in IBM watsonx Assistant. Ghost currently teaches conceptual mechanics, not product steps.

Current fix:
- Do not claim the prototype teaches the full watsonx UI workflow.
- Claim it teaches selected concepts behind chatbot design.
- Optionally frame Act 3 as "custom conversation flow" and Act 3 testing as "preview/test chatbot".

### 5. NLP subtasks are incomplete

Act 2 maps strongly to named entity recognition, but tokenization, POS tagging, sentiment analysis, and machine translation are not covered.

Current fix:
- Explicitly mark those as future/optional Act 7 "NLP Pipeline Lab".
- Do not imply the current vertical slice fully covers NLP.

## Recommended Reframing For The Dissertation

Use this phrasing:

> This project does not reproduce the IBM course as a linear tutorial. Instead, it selects core chatbot/NLP concepts from the course and translates them into playable puzzle mechanics.

Avoid:

> The game covers the IBM chatbot course.

Better:

> The vertical slice demonstrates how selected IBM chatbot/NLP concepts - intent, entity extraction, dialogue management, backend-supported logging, and LLM-based conversational scaffolding - can be represented as mechanics in a narrative puzzle game.

## Recommended Game Changes

### High Priority Before Next Demo

1. Add a "course concept" label to each existing act.
   - Act 1: Intent / user purpose
   - Act 2: Entity extraction / NER
   - Act 3: Dialogue management / conversation flow

2. Add a short Shell intro panel or Lily line about the five chatbot components.
   - Keep it short and visual.
   - Use Ghost's broken communication as the metaphor.

3. Update Act 3 guide text to explicitly say it represents dialogue management.

4. Update slides/dissertation wording to say "selected IBM concepts", not "the whole IBM course".

### Medium Priority

5. Add a small rule-based vs AI-enabled comparison in the Game Shell.
   - Could be a static two-column visual.
   - Do not make it a quiz unless a future task asks for it.

6. Add a pipeline/capstone preview:
   - UI
   - NLP engine
   - Dialogue manager
   - Response generation
   - Backend integration

7. Make backend/LLM visible as learning content:
   - Backend = stores progress/logs and can fetch/support data.
   - LLM = natural language support, not correctness.

### Future Work / Dissertation Scope

8. Keep Act 7 as NLP Pipeline Lab:
   - tokenization
   - POS tagging
   - named entity recognition
   - sentiment analysis

9. Keep Act 8 as capstone:
   - reconnect the five chatbot components.

## Proposed Next Implementation Tasks

### M0-T34: IBM Course Alignment UI Pass

Goal:
Make the existing Acts 1-3 visibly map to the IBM course without changing puzzle rules.

Scope:
- Add course-concept labels to Act 1/2/3 UI.
- Add a short Shell "Ghost's voice parts" explanation using the five components.
- Update Lily dialogue lines to mention concepts naturally.
- Update docs and checklist.

Out of scope:
- No new puzzle mechanics.
- No changes to validators or sessions.
- No watsonx UI tutorial.

### M0-T35: Rule-Based vs AI-Enabled Framing

Goal:
Add a small narrative/visual explanation of rule-based vs AI-enabled chatbots.

Scope:
- Shell-level explanatory panel or short Lily dialogue.
- Use current deterministic validators and Lily LLM chat as a concrete contrast.

Out of scope:
- No new scoring logic.
- No model comparison.

### M0-T36: Dissertation/Slides Alignment Update

Goal:
Update presentation/dissertation framing so claims match the source course.

Scope:
- Replace "covers IBM course" language with "selected IBM chatbot/NLP concepts".
- Add a mapping table slide.
- Add a limitations/future work note for POS tagging, sentiment, machine translation, and watsonx product workflow.

## Final Recommendation

Keep the current Ghost prototype, but re-anchor it.

The strongest defense is:

> Ghost is a design-and-implementation prototype that turns selected IBM chatbot/NLP concepts into playable mechanics, rather than a direct replica of the IBM watsonx Assistant course.

This makes the current work valid while honestly acknowledging that some IBM topics remain future work or framing material.
