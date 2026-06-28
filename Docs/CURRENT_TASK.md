# CURRENT_TASK.md

## ID

M0-T37

## Goal

Strengthen **Act 2 (Entity Extraction)** so it actually **teaches** its concept in-game, not just lets the
player practice span tagging. Keep the existing span-tagging / chip-typing mechanic unchanged, and add a
thin in-fiction teaching layer so the player understands:

- **entity extraction / NER = finding the important details in a message** (not every word — the key
  terms a chatbot must act on);
- **entities come in kinds** — locations, dates/times, objects, people, and other key terms;
- **system vs custom entities** and **synonyms** matter (e.g. the Act 2 `lab` / `laboratory` pair is one
  entity under two wordings);
- **the word chips connect lightly to tokenization** — the message is first split into word tokens, and
  entity tagging marks which tokens/spans are the important details.

Taught the playable way (`CONFIRMED_PROJECT_CONTEXT.md` §2): Ghost's problem + Lily's short in-fiction
explanation + the existing player action (select chip → assign type) + visible consequence — NOT a lecture
wall or multiple-choice quiz. This is the Act 2 sibling of M0-T36 (Act 1) in the "strengthen Acts 1–3
teaching" trio; M0-T38 (Act 3) follows.

## Context

The M0-T34 coverage map marks Act 2's entity/NER, tokenization, and system/custom-entity points as
**partial**: the player tags chips and assigns System/Custom types, but the game never explains what NER
is, why details matter, what system vs custom entities or synonyms are, or how chips relate to
tokenization. M0-T36 set the pattern for Act 1 (a Lily teaching note + purpose labels + teaching-through
correct feedback). Reuse the existing Act 2 presenter / chip views / `LilyDialogueFrame` / banter; this is
a teaching-text + light-consequence pass, not a mechanic redesign. Mirror the M0-T36 approach (runtime
teaching panel, descriptive labels, success feedback that explains the *why*).

## Scope

- Add a short in-fiction NER explanation to Act 2 (before/at the start of the puzzle, via Lily and/or
  Ghost), in the existing Act 2 presentation layer — data-driven static text where possible.
- Make the System/Custom entity-type legend and the chips teach: label them as entity *kinds*; explain
  system vs custom and (using the real `lab`/`laboratory` synonym from the sample data) what a synonym is —
  read the actual sample data, do not hardcode wordings that could drift.
- On a correct Validate, surface an in-fiction beat that conveys "these are the message's important details
  / entities," lightly noting the tokenization connection (the message was split into word chips and you
  marked which are the key entities) — taught through the action, not a quiz.
- Optionally one Lily line connecting entities to Act 1 intent (intent = what is wanted; entities = the
  details inside that request) and forward to slots in Act 3.
- Update `Docs/LEARNING_CONTENT.md` (Act 2 now teaches, not just practices), `CODE_WALKTHROUGH.md`,
  `UNITY_TEST_CHECKLIST.md`; create a Codex run log. Apply the M0-T36 layout lesson: keep the teaching
  panel compact so the chips + legend + validation + banter still fit a 1920×1080 Game view.

## Files Codex may modify

- Act 2 presentation layer (`Assets/Presentation/Act2EntityExtraction/...` — presenter, chip view,
  interaction controller, any Act 2 static-text source) and the Act 2 scene builder only if a new text
  region is truly required (prefer reusing existing labels and runtime-created panels, as in M0-T36).
- `Docs/LEARNING_CONTENT.md`, `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`; new run log.

## Files Codex must NOT modify

- Act 2 puzzle logic: `EntityExtractionValidator`, `EntityExtractionSession`, the Act 2 sample data /
  answer key, and the validation rules (correctness stays deterministic and unchanged).
- Act 1 / Act 3 puzzles, the M0-T35 fundamentals files, backend code, ProjectSettings, Packages, Build
  Settings, and `.meta` files. Do not regenerate or hand-edit scene YAML beyond what the Act 2 scene
  builder requires (Act 2 / Game Shell scenes are shelved side-effects, excluded from commits).

## Out of Scope

- The chatbot-planning mini-level (M0-T39), Act 3 strengthening (M0-T38), Acts 4–8, any quiz, and any
  change to how correctness is decided. Do not add a full tokenization mini-game — tokenization is only a
  light conceptual link here.

## Acceptance Criteria

- Starting Act 2, the player sees a short in-fiction explanation of what entity extraction / NER is
  (finding the important details), in Lily's/Ghost's voice — not a lecture or quiz.
- The System/Custom legend and chips teach entity kinds, system vs custom, and synonyms (using the real
  sample-data synonym pair), and a correct Validate produces a visible in-fiction beat conveying "these are
  the message's key entities," with a light tokenization link.
- The Act 2 validator / session / sample data / correctness behaviour are unchanged; no Console errors; the
  teaching layer fits a 1920×1080 Game view without cropping (per the M0-T36 layout lesson).
- `Docs/LEARNING_CONTENT.md` + `CODE_WALKTHROUGH.md` + `UNITY_TEST_CHECKLIST.md` and a Codex run log are
  updated.
