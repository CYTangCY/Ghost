# CURRENT_TASK.md

## ID

M0-T45

## Goal

**Teaching-as-gameplay redesign of Act 1 and Act 2 (both in this one task), plus a shared Ghost
expression face.** The M0-T36/T37 approach (unchanged mechanic + teaching text panels + success-text)
is REJECTED by the user as the primary teaching method: "the teaching content should BECOME the game,
not the original game with a bolted-on paragraph." Redesign both acts so the IBM concepts are learned
by playing — the player's work is USED by Ghost in front of the player, and understanding is
demonstrated through Ghost's visible behaviour, not read from text.

Design constraints that still hold: correctness stays deterministic (existing validators remain the
single source of truth — FR8); interactions stay within the confirmed mechanic set
(`CONFIRMED_PROJECT_CONTEXT.md` §7: drag-and-drop classification, span annotation, form
configuration); cute Ghost / timid nerdy Lily unchanged; no quiz, no lecture walls.

## Context

User verdict (2026-07-03, review of M0-T37): the game content was identical to before with text added;
"nothing is learned"; Act 1 has the same problem; both acts must be redone. Direction confirmed by the
user: full interaction redesign (not just a consequence phase), both acts in one task, Ghost face
included. The M0-T36/T37 text layers (Lily note panels, purpose labels, legend subtitles) remain as
supporting flavour, but the success-text teaching beats are REPLACED by behavioural consequences.
Act 3 already teaches through consequence (test-case simulation) and is NOT in scope here.

## Design (agreed with the user — implement this, do not invent alternatives)

### New Act 1 — "Train Ghost to greet visitors" (intent + training examples + key topics)

1. **Watch Ghost fail (problem intro):** a conversation panel; scripted visitors arrive; Ghost
   word-matches and answers wrongly (e.g. "where's the lantern?" → "I am Ghost!"); Ghost face =
   confused. 2–3 scripted failures; one short Lily line ("it memorizes sentences, it doesn't
   understand the purpose").
2. **Cluster the transcripts (mechanic upgrade):** NO pre-labelled bins. The player free-clusters the
   9 existing visitor message cards into piles (drag to form/extend piles), then assigns each pile a
   purpose label picked from label chips (find something / where is Ghost / who is Ghost). This makes
   "identify key topics" playable — the player derives the 3 intents.
3. **Teach Ghost → watch it generalize (core loop):** press "Teach Ghost"; NEW unseen visitor
   messages arrive (authored test messages, e.g. "I can't find my flask anywhere"). Ghost classifies
   each using the player's piles: each test message has an authored related-training-cards set; Ghost
   picks the pile holding the MAJORITY of those related cards (scattered/tied → confused face +
   fallback wrong reply). Wrong answers highlight which training cards misled Ghost; the player
   revises piles and re-teaches. Replies come from authored per-intent reply lines.
4. **Completion:** deterministic — the level is complete when `IntentClassificationValidator` reports
   the final pile→intent assignment correct (with fully correct piles the majority rule necessarily
   answers all test visitors correctly; the validator stays the single scoring source).

### New Act 2 — "Ghost's errand" (tokenization + NER + entity kinds + system/custom + synonyms)

1. **Watch Ghost fail an errand:** visitor: "Please bring the key to the lab at 3pm." Ghost's action
   card shows three empty slots WHAT / WHERE / WHEN; Ghost improvises a wrong errand (sad face).
2. **Split into tokens (tokenization as an action):** the message starts as one solid sentence; the
   player presses "Split" and it visibly breaks into word-token chips.
3. **Fill Ghost's action card (mechanic upgrade — token → slot):** drag key tokens into typed slots:
   WHAT (custom: the lab's object words), WHERE (custom: the lab's room words), WHEN (system:
   built-in time). A token dropped into a slot = an `EntitySpan` (token start/length + the slot's
   `EntityType`) — the existing Act 2 span model and validator are reused unchanged.
4. **Run the errand (consequence):** press "Go, Ghost!"; correct slots → happy face + authored
   success outcome; each wrong/missing slot maps to an authored cute failure (empty WHEN → Ghost
   arrives at midnight; wrong WHAT → delivers the lantern instead...). Outcomes derive
   deterministically from per-span correctness (existing validator results).
5. **Synonym beat (experienced, not told):** a second errand uses "laboratory"; when it lands in
   WHERE the slot shows the resolution (laboratory → the same lab room) and Ghost succeeds. System vs
   custom is conveyed by slot presentation (WHEN = built-in; WHAT/WHERE = Ghost's lab dictionary).
6. **Completion:** deterministic — all errands' spans validate via `EntityExtractionValidator`.

### Shared: Ghost expression face

A simple programmatic Ghost face (built-in sprites or runtime-generated texture; NO external art
assets): neutral / happy / confused / sad, driven by the demo outcomes in both acts. Lives on the
conversation/consequence panel.

## Scope

- Rebuild the Act 1 and Act 2 interaction/presentation flows per the design above (new panels,
  conversation/demo displays, cluster + label interaction, token/slot interaction, Ghost face).
- NEW pure-logic classes + EditMode tests in `Ghost.Runtime` for the deterministic demo engines
  (Act 1 generalization: related-cards majority rule + authored test messages/replies; Act 2 errand
  outcomes: span-correctness → outcome mapping + authored errand/outcome data). New files only.
- Reuse (do not modify) `IntentClassificationValidator`/`Session`, `EntityExtractionValidator`/
  `Session`, and the existing sample data files; new authored demo data goes in NEW data files.
- Update the Act 1 / Act 2 Editor scene builders as needed for the new layouts.
- Update `Docs/LEARNING_CONTENT.md`, `CODE_WALKTHROUGH.md`, `UNITY_TEST_CHECKLIST.md`; run logs per
  run. Keep every screen inside a 1920×1080 Game view.

## Out of Scope

- Act 3 (already consequence-based), Fundamentals overview, Shell flow, backend/LLM changes, audio,
  external art, Acts 4–8, quizzes. The LLM never participates in the demo loop (all replies/outcomes
  are authored static data).

## Files Codex may modify

- `Assets/Presentation/Act1IntentClassification/**` and `Assets/Presentation/Act2EntityExtraction/**`
  (presenters, controllers, new views/panels, Editor scene builders).
- NEW files in the `Ghost.Runtime` puzzle-logic folders for demo engines + data + their EditMode tests
  (e.g. alongside `Assets/Scripts/Puzzles/IntentClassification/` and `.../EntityExtraction/`).
- `Docs/LEARNING_CONTENT.md`, `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`; new run logs.

## Files Codex must NOT modify

- Existing pure-logic files: `IntentClassificationValidator`, `IntentClassificationSession`,
  `Act1IntentClassificationSampleData`, `EntityExtractionValidator`, `EntityExtractionSession`,
  `Act2EntityExtractionSampleData`, and their existing tests.
- Act 3, Fundamentals, Shell, Banter, Backend, ProjectSettings, Packages, Build Settings, `.meta`
  files; no hand-edited scene YAML (scene changes only via the builders; generated scenes stay
  shelved side-effects excluded from commits).

## Acceptance Criteria

- Act 1: the player watches Ghost fail, forms the intent piles themselves, labels them, teaches
  Ghost, and watches Ghost answer UNSEEN messages correctly/incorrectly based on their piles; fixing
  piles visibly fixes Ghost's replies; completion via the existing validator; no quiz, no
  success-lecture text.
- Act 2: the player splits the sentence into tokens, fills Ghost's WHAT/WHERE/WHEN slots by dragging
  tokens, runs the errand, and sees authored success/cute-failure outcomes per slot correctness; the
  laboratory synonym beat shows resolution to the same room; completion via the existing validator.
- Ghost face reacts (neutral/happy/confused/sad) in both acts' demo phases.
- All EditMode tests pass (existing + new demo-engine tests); no Console errors; 1080p fit.
- Docs + run logs updated; honest "Not run" for anything not run in-session.

## Run Slicing (guidance)

- Run 001: shared Ghost face + conversation/demo panel components + full Act 1 redesign.
- Run 002: full Act 2 redesign (reusing the shared components).
- Run 003+: fixes from human Play Mode feedback.
