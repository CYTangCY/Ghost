# CONFIRMED_PROJECT_CONTEXT.md

> Purpose: This file is the single source of truth for the Ghost project before Claude Code, Codex, or Unity implementation work starts.
>
> Rule: Do not continue design, coding, or scene building unless the task is consistent with this file.
>
> Status labels:
> - **Confirmed from report**: stated in the preliminary project report.
> - **Confirmed from supervisor feedback**: stated in the report revision summary.
> - **Confirmed by user**: corrected or specified by Chao-Yang in conversation.
> - **Unconfirmed**: do not invent; must be checked before implementation.

---

## 1. Project Identity

### 1.1 Working project name

**Confirmed from supervisor feedback**

Use:

```text
Ghost
```

or, in report title form:

```text
Ghost: A Narrative Puzzle Game for Teaching Chatbot and NLP Concepts
```

Do not use the previous title as the main framing:

```text
Ghost Chatbot
```

Reason: the learning subject is already chatbot/NLP, so calling the game "Ghost Chatbot" can confuse the game character/system with the learning topic.

### 1.2 Reference game

**Confirmed by user**

```text
while True: learn()
```

is **not** the project title.

It is a separate reference game. It may be used only as a high-level reference for turning abstract AI-related ideas into puzzle interactions.

Do not copy its title, story, structure, or identity.

### 1.3 Current project framing

**Confirmed from supervisor feedback + corrected by user**

This project is a **cute ghost-themed narrative puzzle game** for teaching selected IBM SkillsBuild chatbot and NLP concepts.

Ghost is a cute ghost character. Ghost is **not literally an AI assistant, chatbot, or robot**.

The AI/chatbot/NLP layer is the learning analogy: Ghost's communication problems are used to help the player understand chatbot and NLP concepts.

The player learns by repairing Ghost's misunderstandings through puzzle mechanics and seeing Ghost's response change.

---

## 2. Core Design Principle

### 2.1 Hard principle

Every major gameplay step must map to an AI chatbot / NLP learning concept.

The project must not become:

```text
dialogue + multiple-choice quiz
```

unless the current Act explicitly uses multiple choice as its intended mechanic.

### 2.2 Correct learning pattern

Each level should contain:

```text
1. Ghost has a cute communication problem.
2. Lily notices or explains the problem nervously but intelligently.
3. The player performs a puzzle action.
4. The puzzle action maps to a chatbot/NLP concept.
5. Ghost's response visibly changes based on the player's action.
6. The player learns the concept through the consequence.
```

### 2.3 Concrete consequence rule

Avoid abstract phrases like:

```text
design decisions with observable consequences
```

Use concrete behaviour:

```text
players make choices and immediately see how Ghost's response changes
```

Examples:

```text
If the player assigns the wrong intent, Ghost chooses the wrong type of reply.
If the player marks the wrong entity, Ghost cannot extract the needed information.
If the confidence threshold is too low, Ghost answers when it should ask for clarification.
If the fallback design is poor, Ghost repeats irrelevant answers.
```

---

## 3. Characters

## 3.1 Ghost

**Confirmed by user**

Ghost is a cute ghost.

Ghost should feel:

```text
cute
expressive
playful
confused
slightly strange
gradually easier to understand
```

Ghost should **not** be written as:

```text
a literal AI assistant
a robot
a chatbot character
a horror-first monster
a generic software agent
```

Design function:

```text
Ghost provides the visible result of the player's learning.
```

When the player solves a puzzle correctly, Ghost's response becomes clearer, cuter, or more appropriate.

When the player makes an incorrect mapping, Ghost responds in a cute but wrong/confused way.

## 3.2 Lily

**Confirmed from report + corrected by user**

From the preliminary report, Lily is a postdoctoral researcher character and is connected to IBM Granite via Ollama for tiered scaffolded dialogue in prototype contexts.

User correction: Lily should be the protagonist's postdoctoral senior from the lab.

Personality:

```text
nerdy
technically capable
pretty and cute
slightly timid
somewhat deferential
a bit awkward
likable
not overconfident
```

Lily should not sound like:

```text
a generic teacher
a generic AI tutor
a confident lecturer
a solution-dispenser
```

Lily's function:

```text
1. She helps interpret Ghost's communication problem.
2. She gives hints without directly solving the puzzle.
3. She makes the AI/NLP concept understandable through the situation.
4. She keeps a human, slightly nervous tone.
```

Example Lily tone:

```text
"Um... I think Ghost isn't ignoring us. It might be reacting to the wrong purpose."

"Sorry, that sounded vague. I mean... maybe we should look at what the message wants, not just the words."

"I don't think Ghost is ready to answer yet. The confidence looks too high for something this uncertain."
```

---

## 4. Confirmed Curriculum Scope

**Confirmed from preliminary report**

The project is scoped to selected IBM SkillsBuild chatbot/NLP concepts.

Confirmed course topics include:

```text
Intent classification
Entity extraction
Dialog Node configuration
Confidence scoring
Fallback design
System integration
```

The project should not become a general AI literacy game unless explicitly redesigned.

---

## 5. Confirmed Act Structure

**Confirmed from preliminary report; Act sequence revised and confirmed by user (Chao-Yang) on 2026-06-20.**

Do not invent a new Act structure. Follow the revised structure below (mirrored in `Docs/ROADMAP.md`
and `Docs/LEARNING_CONTENT.md`).

### Revised Act structure (user-confirmed 2026-06-20)

```text
Act 1: Intent classification
Act 2: Entity extraction
Act 3: Dialog management via node graph   (flagship mechanic; built after Act 2)
Act 4: Confidence and fallback            (extends the Act 3 graph)
Act 5: Testing and debugging              (extends the Act 3 graph)
Act 6: Integration / backend action / response generation  (extends the Act 3 graph)
Act 7: NLP pipeline lab                    (tokenisation, POS tagging, NER, sentiment; former Act *)
Act 8: Capstone — "Repair Ghost's Voice"   (reconnect the five chatbot components; former Act 0 mechanic)
```

Notes on the revision:
- Chatbot fundamentals (definitions, rule-based vs AI-enabled, five components, four challenges) —
  the former Act 0 — are now introduced by Lily in the Game Shell, and the "Rebuild Ghost's Voice"
  five-component pipeline mechanic becomes the Act 8 capstone.
- The node graph (Act 3) is the flagship mechanic and is reused/extended by Acts 4–6.
- A Game Shell / Lily / Act Select layer precedes Act 2 work (see ROADMAP Phase A).

### Original report structure (retained for traceability)

```text
Act 0: Chatbot fundamentals
- definitions
- rule-based vs AI-enabled chatbot
- five components
- four challenges

Act 1: Intent
- classification
- training examples

Act 2: Entity
- extraction
- synonyms
- system entities vs custom entities

Act *: Supplementary NLP pipeline
- tokenisation
- POS tagging
- named entity recognition
- sentiment analysis
- implemented in the prototype if needed
- excluded from the primary Evaluation 2 and Evaluation 3 scope

Act 3: Dialog
- nodes
- branching
- slots
- response types
- context variables

Act 4: Confidence and fallback
- scoring
- threshold calibration
- disambiguation

Act 5: Testing and debugging

Act 6: Integration and deployment
- represented as a deployment design puzzle
- not a production chatbot deployment
```

Important: UI naming can be cuter/narrative-facing, but the curriculum must remain traceable to
these Acts (the revised structure above; the original report structure is retained for lineage).

---

## 6. Prototype Scope

**Confirmed from preliminary report**

Full design:

```text
24-level curriculum fully specified in the GDD.
```

Playable prototype target:

```text
one complete level per Act
8 levels total
```

Guaranteed minimum from preliminary report:

```text
five structurally distinct mechanics:
- drag-and-drop classification
- slider calibration
- node assembly
- span annotation
- multiple choice
```

Priority from preliminary report:

```text
Acts 0, 1, and 2 first
```

because these introduce commonly reused mechanic types.

---

## 7. Confirmed Puzzle Mechanic Set

**Confirmed from preliminary report**

The reported mechanic set is:

```text
1. drag-and-drop classification
2. span annotation
3. node assembly
4. slider calibration
5. flow diagram construction
6. multiple choice
7. form configuration
8. ordered list
```

Do not add new mechanic types unless the user explicitly approves.

Do not reduce all mechanics to multiple choice.

---

## 8. Technical Scope

**Confirmed from preliminary report**

Client:

```text
Unity 6
C#
WebGL build target
single-player
```

AI tutor / Lily:

```text
IBM Granite via Ollama
local Node.js proxy
curriculum-specific system prompt
no direct puzzle solution disclosure
```

Backend:

```text
Node.js with TypeScript
REST API for progress synchronisation and attempt logging
SQLite for prototype development and local evaluation
local pseudonymous profile for test session progress and attempt logs
analytics: puzzle attempt logs and hint triggers
```

Fallback rule:

```text
If LLM integration is delayed, static hints can be used as a gameplay fallback.
The LLM scaffolding evaluation should then be reported as a limitation or deferred component.
```

Repository / actual implementation status:

```text
Unconfirmed in current repo.
Must inspect actual Unity project before assuming what scripts, scenes, or prefabs already exist.
```

---

## 9. Evaluation Scope

**Confirmed from preliminary report + supervisor feedback**

No user study is conducted for the preliminary/final project scope unless explicitly changed.

Evaluation should focus on:

```text
1. curriculum-to-puzzle mapping
2. prototype implementation check
3. LLM tutor response check
```

The user-based learning outcome evaluation is outside the current project scope.

The project should be framed as a software artefact / prototype and design mapping, not as a large empirical learning study.

---

## 10. Research Question and Framing

**Confirmed from supervisor feedback**

Use one main research question:

```text
How can selected IBM SkillsBuild chatbot and NLP concepts be translated into playable puzzle mechanics in a narrative game prototype?
```

Do not use the old overcomplicated version:

```text
To what extent does a multi-game-type narrative game design operationalise selected IBM SkillsBuild chatbot/NLP concepts through intrinsically integrated puzzle mechanics within a playable prototype?
```

Avoid over-polished words such as:

```text
operationalise
intrinsically
theoretical mechanisms
system-level correctness
narrative-pedagogical coupling
observable consequences
```

Prefer simpler wording:

```text
turn into
build into
directly linked
whether the system works as designed
link between story and learning
visible changes in Ghost's behaviour
```

---

## 11. Literature / Gap Framing

**Confirmed from supervisor feedback**

Do not claim:

```text
chatbot education is scarce
```

because chatbot education literature is broad.

Use a narrower gap:

```text
Many studies discuss chatbots in education, but most treat chatbots as tutors, assistants, or delivery tools.
Fewer studies focus on teaching learners how to design chatbots.
Fewer still turn chatbot and NLP concepts into playable game mechanics.
```

Relevant literature groups to maintain:

```text
1. Teaching chatbot and NLP concepts
2. Chatbots in education: broad field, but different focus
3. Game-based learning and puzzle mechanics
4. Narrative games for learning
5. Pedagogical AI conversational agents and Lily
6. LLM support in game-based learning
7. Summary: what literature supports and what remains unsolved
```

Important rule:

```text
Do not invent citations.
Only use references already supplied or explicitly verified.
```

---

## 12. Current Design Corrections from User

These are the latest user corrections and override older report phrasing when they conflict.

```text
1. while True: learn() is a separate reference game, not this project's title.
2. Ghost should be a cute ghost.
3. Ghost should not literally be an AI.
4. Ghost can be used as an analogy for AI/chatbot/NLP concepts.
5. Every gameplay step must still clearly correspond to AI chatbot/NLP learning.
6. Lily should resemble a nerdy, timid, deferential, pretty/cute postdoctoral senior from the protagonist's lab.
7. Lily should be likable because she is competent but not overconfident.
8. Lily should not sound like a generic tutor or generic AI assistant.
```

---

## 13. Conflicts Between Old Report and Current Direction

### Conflict 1: Ghost as AI entity

Old report says:

```text
The player trains an AI entity, Ghost, trapped in a smart home system.
```

Current user correction says:

```text
Ghost is a cute ghost, not literally AI.
```

Resolution for future work:

```text
Write Ghost as a cute ghost character.
Use AI/chatbot/NLP as the learning analogy behind the gameplay mechanic.
Do not write Ghost as a normal AI assistant.
```

### Conflict 2: Title

Old report title:

```text
Ghost Chatbot
```

Supervisor feedback:

```text
Use Ghost instead.
```

Current direction:

```text
Do not use while True: learn() as title.
Working title remains Ghost or TBD.
```

### Conflict 3: Lily as generic LLM tutor

Old report may frame Lily functionally as LLM tutor / assistant.

Current user correction adds personality:

```text
Lily is a nerdy, slightly timid, pretty/cute postdoctoral senior.
```

Resolution:

```text
Keep Lily's technical role as LLM-assisted scaffold provider.
Write her character voice as a human lab senior, not generic tutor.
```

---

## 14. Unconfirmed Items

Do not invent these. Confirm before implementation.

```text
1. Final game title.
2. Exact player character identity.
3. Exact setting: lab, haunted lab, smart home, cute haunted interface, or another setting.
4. Exact cute ghost art style.
5. Exact Act 0 prototype level.
6. Exact representative level for each Act.
7. Exact mechanic assigned to each Act.
8. Exact 24-level GDD contents.
9. Actual current Unity repository structure.
10. Which scripts, prefabs, and scenes already exist.
11. Current status of DraggableNode.
12. Current status of backend / Ollama / IBM Granite.
13. Whether LLM integration remains required for final implementation or can be deferred.
14. Whether Act * should be implemented in the final prototype.
15. Whether the final prototype target is still 8 levels or has been reduced.
```

---

## 15. Rules for Claude Code

Claude must read this file before planning.

Claude should be used for:

```text
planning
architecture review
scope checking
explainability review
debugging support
Codex prompt generation
```

Claude must not:

```text
invent new Act structures
rename the project without approval
write Ghost as a robot/chatbot/AI assistant
turn levels into simple quiz screens
copy while True: learn()
ignore the IBM SkillsBuild chatbot/NLP mapping
invent references or academic claims
```

Claude must check each task against:

```text
1. cute Ghost narrative
2. AI chatbot/NLP learning mapping
3. Lily's correct character voice
4. visible change in Ghost's response
5. prototype scope and Act structure
6. implementation explainability
```

---

## 16. Rules for Codex

Codex must read this file before implementation.

Codex should be used for:

```text
small Unity C# tasks
UI components
puzzle mechanics
testable scripts
documentation updates
compile error fixes
```

Codex must not:

```text
invent story content beyond the current task
invent learning objectives
invent references
edit ProjectSettings without approval
delete or rename .meta files
change Act structure
turn every level into multiple choice
write Ghost as a literal AI assistant
write Lily as a generic tutor
```

Every Codex task must update or produce:

```text
1. changed file list
2. Unity Inspector setup
3. Play Mode test steps
4. CODE_WALKTHROUGH.md update
5. any assumptions made
```

---

## 17. Canonical Mapping File

The canonical learning-content mapping lives in:

```text
Docs/LEARNING_CONTENT.md
```

(This file replaces the previously planned Docs/LEARNING_CONTENT_MAPPING.md.)

Purpose:

```text
Confirm each Act's:
- IBM SkillsBuild concept
- learning objective
- target puzzle mechanic
- Ghost cute communication problem
- player action
- success consequence
- failure consequence
- Lily hint style
- implementation priority
```

No Unity implementation should start until this mapping is at least drafted for Act 0, Act 1, and Act 2.

---

## 18. Immediate Next Step

Do not start Unity coding yet.

Next task:

```text
M0-T02: Complete the confirmed learning-content mapping for Acts 0, 1, and 2 in
Docs/LEARNING_CONTENT.md, and confirm the mechanic assigned to each.
```

Reason:

```text
Acts 0, 1, and 2 have implementation priority and introduce reused mechanic types.
The exact mechanic-to-concept mapping must be fixed before Codex writes scripts.
```
