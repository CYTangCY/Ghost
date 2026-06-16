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

TBD. The player should understand the basic components and limitations of chatbots.

### Cute Ghost Communication Problem

TBD.

Possible direction:
Ghost tries to communicate through several broken channels, but each channel represents one chatbot component.

### Puzzle Mechanic

TBD.

Possible mechanics:
- flow diagram construction
- ordered list
- multiple choice only if used carefully as a minor mechanic

### Player Action

TBD.

### Success Consequence

TBD.

### Failure Consequence

TBD.

### Lily Hint Style

Lily should nervously explain the system components without sounding like a lecturer.

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

Likely:
drag-and-drop classification

Must be confirmed before implementation.

### Player Action

The player groups message cards by what the speaker wants, not by exact wording.

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

The player should understand that a chatbot needs to identify important details such as names, locations, times, and objects.

### Cute Ghost Communication Problem

Ghost understands the general purpose but misses key details.

For example, Ghost knows someone wants help, but misses which room, object, name, or time matters.

### Puzzle Mechanic

Likely:
span annotation

Must be confirmed before implementation.

### Player Action

The player marks important words or fragments in a message.

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