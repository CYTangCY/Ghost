# CURRENT_TASK.md

## ID

M0-T36

## Goal

Strengthen **Act 1 (Intent Classification)** so it actually **teaches** its concept in-game, not just lets
the player practice it. Keep the existing intent-classification mechanic (group message cards by purpose)
unchanged, and add a thin in-fiction teaching layer so the player understands:

- **intent = the user's purpose** (what the speaker wants), not the exact words;
- **different wording can share one intent** (the reason cards group together);
- **training examples**: the differently-worded messages inside one intent group ARE that intent's
  example utterances — a chatbot learns an intent from many example phrasings (the original Act 1 facet in
  `CONFIRMED_PROJECT_CONTEXT.md` §5 "Act 1: Intent — classification + training examples"; IBM §1.6 NLU);
- **grouping intents = the chatbot-planning step of identifying key topics / common requests**
  (`IBM_COURSE_CONTENT.md` §1.8, §1.6 NLU).

Taught the playable way (`CONFIRMED_PROJECT_CONTEXT.md` §2): Ghost's problem + Lily's short in-fiction
explanation + the existing player action + visible consequence — NOT a lecture wall or multiple-choice
quiz.

## Context

The M0-T34 coverage map marks Act 1's intent concept as **partial**: the player *does* intent grouping but
the game never *explains* what intent is or why different wording groups together. M0-T35 added the
fundamentals overview (chatbot definition, pillars, rule vs AI, components, challenges); M0-T36 is the
first of the "strengthen Acts 1–3 teaching" trio (M0-T36 Act 1, M0-T37 Act 2, M0-T38 Act 3). Reuse the
existing Act 1 presenter / `LilyDialogueFrame` / banter; this is a teaching-text + light-consequence pass,
not a mechanic redesign.

## Scope

- Add a short in-fiction intent explanation to Act 1 (before/at the start of the puzzle, via Lily and/or
  Ghost), in the existing Act 1 presentation layer — data-driven static text where possible.
- Make the teaching visible through the existing mechanic: when the player groups differently-worded cards
  under one intent, surface a brief in-fiction line that this is one intent / purpose AND that those varied
  wordings are the intent's training examples (e.g. on a correct group, or as a Lily aside) so the *why* is
  shown, not just pass/fail.
- Optionally connect grouping to "identifying key topics / common user requests" in one Lily line, lightly
  referencing the chatbot-planning idea (no new planning mechanic — that is M0-T39).
- Update `Docs/LEARNING_CONTENT.md` (Act 1 now teaches, not just practices), `CODE_WALKTHROUGH.md`,
  `UNITY_TEST_CHECKLIST.md`; create a Codex run log.

## Files Codex may modify

- Act 1 presentation layer (`Assets/Presentation/...` Act 1 presenter / interaction controller / any Act 1
  static-text/dialogue source) and the Act 1 scene builder, for teaching text + light consequence only.
- `Docs/LEARNING_CONTENT.md`, `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`; new run log.

## Files Codex must NOT modify

- Act 1 puzzle logic: `IntentClassificationValidator`, `IntentClassificationSession`, the Act 1 sample
  data / answer key, and the validation rules (correctness stays deterministic and unchanged).
- Act 2 / Act 3 puzzles, the M0-T35 fundamentals files, backend code, ProjectSettings, Packages, Build
  Settings, and `.meta` files. Do not regenerate or hand-edit scene YAML beyond what the Act 1 scene
  builder requires (the Game Shell scene is a shelved side-effect, excluded from commits).

## Out of Scope

- The chatbot-planning mini-level (M0-T39), Act 2/3 strengthening (M0-T37/T38), Acts 4–8, any quiz, and any
  change to how correctness is decided.

## Acceptance Criteria

- Starting Act 1, the player sees a short in-fiction explanation of what intent is (purpose, not exact
  wording) before/while grouping, in Lily's/Ghost's voice — not a lecture or quiz.
- Grouping differently-worded cards under one intent produces a visible in-fiction beat that conveys "these
  share one intent/purpose, and these varied wordings are that intent's training examples," so the concept
  is taught through the action.
- The Act 1 validator / session / sample data / correctness behaviour are unchanged; no Console errors.
- `Docs/LEARNING_CONTENT.md` + `CODE_WALKTHROUGH.md` + `UNITY_TEST_CHECKLIST.md` and a Codex run log are
  updated.
