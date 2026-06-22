# PROJECT_BRIEF.md

## Working Title

Ghost

## One-Sentence Pitch

Ghost is a cute ghost-themed narrative puzzle game that teaches selected IBM SkillsBuild chatbot and NLP concepts by turning Ghost's communication problems into playable puzzle mechanics.

## Project Goal

The player helps a cute ghost communicate more clearly with humans. Ghost is not literally an AI system. Instead, Ghost's misunderstandings are used as playful analogies for chatbot and NLP concepts.

The player learns by solving puzzles that repair Ghost's communication problems and by seeing Ghost's response change.

## Reference

while True: learn() is only a reference for how abstract AI concepts can become puzzle interactions.

This project must not copy its title, story, or structure.

## Main Design Principle

Each level should connect:
- a cute Ghost communication problem
- a chatbot / NLP concept
- a puzzle mechanic
- a visible change in Ghost's response

The game should not become a static lecture or a simple multiple-choice quiz.

## Main Characters

### Ghost

Ghost is a cute supernatural character. Ghost is confused, expressive, playful, and gradually becomes easier to understand.

Ghost should not be written as a literal AI assistant or chatbot.

### Lily

Lily is the protagonist's postdoctoral senior from the lab.

She is nerdy, technically capable, pretty/cute, slightly timid, and a little deferential. She helps interpret Ghost's communication problems, but she should not sound like a generic tutor.

## Confirmed Curriculum Structure

Revised 8-Act structure (user-confirmed 2026-06-20; canonical sequencing lives in `Docs/ROADMAP.md`
and `Docs/LEARNING_CONTENT.md`):

- Act 1: Intent classification
- Act 2: Entity extraction
- Act 3: Dialog management via node graph  (flagship mechanic; built after Act 2)
- Act 4: Confidence and fallback           (extends the Act 3 graph)
- Act 5: Testing and debugging             (extends the Act 3 graph)
- Act 6: Integration / backend action / response generation  (extends the Act 3 graph)
- Act 7: NLP pipeline lab                   (tokenisation, POS, NER, sentiment; former Act *)
- Act 8: Capstone — "Repair Ghost's Voice"  (reconnect the five chatbot components; former Act 0 mechanic)

The former Act 0 (chatbot fundamentals: definition, rule-based vs AI-enabled, five components, four
challenges) is preserved: its concepts are introduced by Lily in the Game Shell, and its "Rebuild
Ghost's Voice" pipeline becomes the Act 8 capstone.

## Prototype Goal

The full design contains 24 levels.

The prototype target is one complete level per Act, 8 levels total, unless scope is later reduced.

## First Development Priority

Current status (2026-06-22): Act 1 (intent) core gameplay and the Game Shell (title / act select /
Lily dialogue frame) are complete; the active task is Act 2 (entity extraction). The node graph
(Act 3) is the flagship mechanic and is built after Act 2 so intent and entity can act as triggers
and slots inside dialog nodes. See `Docs/ROADMAP.md` for the phased plan.

## Full-System Direction

The final project is a full AI-assisted educational game system, not only a Unity puzzle prototype.
LLM, backend, and database are required final-system components (player progress, attempt logs,
content delivery, LLM orchestration, Lily hints, Ghost responses, and the capstone chatbot
simulation). Puzzle correctness stays deterministic (validators / graph simulation / test cases /
backend scoring); the LLM never decides scoring. These systems are integrated after the gameplay
skeleton is stable. See `Docs/ROADMAP.md` Phase D and `Docs/ARCHITECTURE.md`.

## How This Project Is Run

Two-agent workflow with the user as final decision maker: Claude is the repo-aware project commander
and reviewer; Codex is the implementation agent. ChatGPT is not part of the official workflow. The
canonical process is `Docs/AI_COLLABORATION_PROTOCOL.md`.