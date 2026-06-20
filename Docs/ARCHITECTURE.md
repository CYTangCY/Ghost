# ARCHITECTURE.md

## Status

Initial target architecture. This file should be updated after actual Unity implementation.

## Main Principle

Separate:
- level data
- puzzle logic
- Ghost response display
- Lily hint display
- UI view
- progress tracking

Do not put all logic into one large GameManager.

## Target Runtime Flow

1. Level scene loads.
2. LevelController loads the current level data.
3. Ghost presents a cute communication problem.
4. Lily gives initial interpretation or hint.
5. Player performs the puzzle action.
6. PuzzleController validates the action.
7. GhostResponseView shows success or failure response.
8. LilyHintView may show a hint after failure.
9. LevelProgressController marks the level complete when solved.

## Target Systems

### Level System

Responsibility:
Controls which level is active and when it is complete.

Possible scripts:
- LevelController
- LevelProgressController
- LevelDefinition

### Puzzle System

Responsibility:
Controls the puzzle mechanic for the current level.

Possible scripts:
- IntentPuzzleController
- EntityPuzzleController
- PuzzleValidator

### Ghost Response System

Responsibility:
Displays Ghost's response based on the player's action.

Possible scripts:
- GhostResponseView

### Lily Hint System

Responsibility:
Displays Lily's hint or interpretation.

Possible scripts:
- LilyHintView
- HintController

### UI System

Responsibility:
Displays panels, cards, buttons, and feedback.

UI scripts should not decide puzzle correctness.

### Data System

Responsibility:
Stores level content and puzzle data.

Possible approach:
- ScriptableObject for Unity-editable prototype data
- JSON later if needed

## Architecture Rules

- UI scripts only display data and forward player input.
- Puzzle controllers validate puzzle actions.
- Level controllers manage progression.
- Ghost and Lily text should come from data, not hardcoded everywhere.
- Avoid complex event bus unless truly needed.

## Future-Facing Layers (roadmap, not yet implemented)

> Forward-looking notes added with the 2026-06-20 roadmap revision (see `Docs/ROADMAP.md`). They are
> intentionally high-level; do not over-specify until each is the active task.

### Game Shell Layer (Phase A — next)

Responsibility: frame the prototype as one Ghost game and route the player between screens.

Conceptual pieces:
- Title screen.
- Act select / hub.
- Lily dialogue frame (a reusable panel that shows short tutorial/instruction text in Lily's voice).
- Ghost placeholder/presence.
- Start-Act and return-to-hub flow.

Notes: keep the shell a thin routing/presentation layer; Acts stay self-contained scenes/screens.
The Lily dialogue frame should read its text from data, not hardcode it per screen.

### Node Graph System (Phase C — flagship, later)

Responsibility: represent and play dialog management as an editable graph; reused/extended in
Acts 4–6.

Conceptual pieces:
- Graph data model (pure C#, Unity-independent where possible — like the existing puzzle logic).
- Node UI (presentation layer: nodes, ports/connections).
- Graph validation/simulation (run a message/conversation through the graph and report the result,
  analogous to how `IntentClassificationValidator` validates a grouping).
- Extension node types for confidence/fallback (Act 4), testing/debugging (Act 5), and
  backend-action/response-generation (Act 6).

Notes: follow the Act 1 pattern that worked well — keep the graph rules in a pure, testable logic
assembly (no UnityEngine dependency), with presentation and an interaction controller on top.