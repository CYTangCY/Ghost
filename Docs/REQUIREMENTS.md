# REQUIREMENTS.md

## Functional Requirements

### FR1: Cute Ghost Communication Problem

Each level must start from a Ghost communication problem.

Ghost's problem should be cute, expressive, and understandable, not horror-first.

### FR2: AI Chatbot / NLP Concept Mapping

Each main puzzle must map to a specific AI chatbot or NLP concept.

Examples:
- intent classification
- entity extraction
- dialog state
- confidence threshold
- fallback handling
- response selection
- testing and debugging
- integration

### FR3: Puzzle-Based Learning

The player should learn by solving a puzzle, not only by reading dialogue or answering a quiz.

### FR4: Visible Ghost Response Change

The player's action must change Ghost's response.

Correct action:
Ghost responds more appropriately.

Incorrect action:
Ghost responds in a cute but wrong/confused way.

### FR5: Lily Hint Support

Lily should provide hints or interpretation without directly solving the puzzle.

Lily is the protagonist's postdoctoral senior from the lab. Her style must be nerdy, technically capable, pretty/cute, slightly timid, and a little deferential — human, not a generic tutor or AI assistant.

### FR6: Playable Prototype

Each implemented level must be playable in Unity and manually testable.

### FR7: Full-System Components (added 2026-06-22)

The final system must include, as required components (see `Docs/ROADMAP.md` Phase D and
`Docs/ARCHITECTURE.md`):
- a backend (content delivery, player progress, attempt logs, LLM orchestration, and optional
  graph simulation/scoring);
- a database (learning content, puzzle content, player progress, player attempts, and — if
  appropriate — dialogue/hint logs);
- an LLM layer (Lily hints, Ghost response generation, explanatory feedback, capstone chatbot
  simulation, and optional natural-language variation).

These are integrated after the gameplay skeleton (Game Shell + Acts 1–3) is stable.

### FR8: Deterministic Correctness

Puzzle correctness must be decided by deterministic logic — validators, graph simulation, authored
test cases, or backend scoring. The LLM must not decide correctness or scoring; it may only hint,
explain, or generate natural language.

## Non-Functional Requirements

### NFR1: Explainability

Every major script must have a clear responsibility and be explainable in the final project.

### NFR2: Simplicity

Avoid unnecessary complex architecture.

Do not use:
- complex event bus
- dependency injection framework
- reflection-heavy systems
- unnecessary threading
- over-generic frameworks

### NFR3: WebGL Compatibility

Avoid APIs or plugins that are unsuitable for Unity WebGL unless explicitly approved.

### NFR4: Documentation

Every implemented system must update:
- CODE_WALKTHROUGH.md
- UNITY_TEST_CHECKLIST.md
- HANDOFF_LOG.md

LLM/backend/database plans must also be reflected in ROADMAP.md and ARCHITECTURE.md before those
components are implemented.

### NFR5: Graceful Degradation (added 2026-06-22)

If the backend or LLM is unavailable, the game must keep working using local validation and static
hints, without breaking puzzle play. NFR3 (WebGL compatibility) applies to the Unity client; the
backend and LLM are server-side.

## Hard Constraints

Do not:
- write Ghost as a literal AI assistant
- write Lily as a generic tutor
- copy while True: learn()
- invent new Act structure
- invent academic references
- turn every level into multiple choice
- modify ProjectSettings without approval
- delete or rename Unity .meta files