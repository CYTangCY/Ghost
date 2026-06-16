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