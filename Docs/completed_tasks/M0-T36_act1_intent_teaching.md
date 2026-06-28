# M0-T36 — Strengthen Act 1 as Intent teaching (classification + training examples)

## Completion Status

Completed (Codex runs 001 teaching, 002 visual clarity, 003 layout fit). Human Unity verification passed —
the user confirmed in Play Mode ("pass") that the teaching panel, purpose-labelled groups, and the
training-examples success feedback display, that all 9 cards / 3 groups / validation / banter fit at
1920×1080 without cropping, and that there are no Console errors.

## Date

2026-06-28

## Summary

Made Act 1 actually **teach** intent, not just let the player practice it, while leaving the
intent-classification mechanic and deterministic validator untouched. Added a runtime `Lily's Intent Note`
panel (Ghost problem: "matching exact words, so replies miss the purpose" + Lily's short stammered
explanation that intent = what the visitor wants and that varied phrasings become training examples).
Intent group titles changed from raw ids to player-facing **purpose labels** ("Purpose: find something /
locate Ghost / identify Ghost") and group hints rephrased as purposes ("These visitors all want…").
On a correct Validate, the feedback now teaches through the action: Ghost brightens, states "one purpose
per group, even with different words" (classification), shows a **training-examples** count derived from
the real card data, and adds a Lily line linking grouping to "spotting common requests before planning the
chatbot." Two follow-up runs improved visual clarity (002: dedicated warm teaching panel + green
success-teaching state) and compact layout (003: reduced card/group/teaching/validation heights + tighter
runtime root layout so the bottom validation/banter area fits a 1080p Game view).

## Files Created / Modified

- Modified: `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
  (Lily's Intent Note panel, purpose-label group titles/hints, feedback-state styling, compact runtime
  layout), `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationInteractionController.cs`
  (`BuildCorrectFeedbackMessage` / `BuildTrainingExampleSummary` — success teaching from real card data)
- Docs: `Docs/LEARNING_CONTENT.md`, `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`
- Run logs: `Docs/codex_runs/M0-T36_001_act1_intent_teaching.md`,
  `Docs/codex_runs/M0-T36_002_act1_teaching_visual_clarity.md`,
  `Docs/codex_runs/M0-T36_003_act1_teaching_layout_fit.md`
- NOT committed (shelved scene side-effects, dirty in worktree): `Assets/Scenes/Act1IntentClassificationPrototype.unity`,
  `Assets/Scenes/GameShellPrototype.unity`

## Claude Review Notes

- **Scope clean.** `git diff --name-only` over `Assets/Scripts/Puzzles`, `Act1.../Editor`, and
  `Assets/Presentation/Fundamentals` returned empty — no puzzle logic, no Act 1 scene builder, no
  fundamentals touched. Only the 2 Act 1 presentation C# files + 3 docs changed.
- **Deterministic-correctness upheld.** `ValidateCurrentGrouping()` still scores via
  `IntentClassificationSession.ValidateCurrentState()`; only the feedback string changed on the `IsCorrect`
  branch. `BuildTrainingExampleSummary` only reads `card.IntentId` to count examples after a correct solve;
  no validator/session/sample-data/answer-key edits; the LLM never scores.
- **Playable, not a quiz/lecture.** Teaching emerges from the existing grouping action + a short in-fiction
  panel; no multiple-choice. Group purpose labels map the three sample-data intent ids
  (find_item/ask_location/ask_identity) without changing the underlying ids or validation.
- **Identity preserved.** Lily stammers and stays the timid nerdy senior; Ghost is a cute ghost that
  "brightens." No robot/generic-tutor framing.
- Run logs honest (Unity Play Mode "Not run" in every Codex session); all three retained, not overwritten.

## Human Verification Result

Passed in the Unity Editor ("pass"): Lily's Intent Note panel visible; groups show purpose labels; correct
Validate shows the green success-teaching (Ghost reaction + shared-purpose + training-examples + planning
line); incorrect path still gives the non-spoiler hint; all 9 cards / 3 groups / validation / banter fit at
1920×1080 without cropping; no Console errors.

## Next Task

M0-T37 — Strengthen Act 2 as Entity Extraction / NER (+ tokenization) teaching: keep the existing span
tagging mechanic, but add in-fiction teaching that entity extraction / NER means finding the important
details in text (locations, dates/times, objects, people, other key terms), that system vs custom entities
and synonyms matter, and that the word chips connect lightly to tokenization. No change to the Act 2
validator / session / sample data.
