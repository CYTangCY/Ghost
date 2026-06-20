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