# M0-T35 — Chatbot Fundamentals Teaching Pass

## Completion Status

Completed (Codex run 001). Human Unity verification passed — the user confirmed in the Editor ("通過")
that the `Ghost's Voice Basics` overview appears in the Act Hub, all six fundamentals beats are playable
(Ghost problem → Lily explanation → player action → visible Ghost consequence), the five-component
ordering + backend side link work, skip/finish returns to the hub, Acts 1–3 still launch, and there are
no Console errors.

## Date

2026-06-27

## Summary

Added a compact, **playable** chatbot-fundamentals overview to the Game Shell — the preserved "former
Act 0" concept layer. From the Act Hub the player can open an optional `Ghost's Voice Basics` card and
walk through six short teaching beats that cover the IBM course fundamentals the prior Acts only practiced
but never explained: (1) chatbot definition, (2) NLP + ML pillars, (3) rule-based vs AI-enabled, (4)
benefits, (5) the five components, (6) the four challenges. Each beat is taught in-fiction (Ghost problem
→ Lily's short stammered explanation → a small player action → a visible Ghost consequence), not as a
lecture wall or multiple-choice quiz. Interaction kinds: single-action tap, toggle/compare, a
five-component ordering puzzle (+ backend side link), and a trigger-and-observe challenge-modes beat.
The sequence is optional and skippable; it does not gate Acts 1–3.

## Files Created / Modified

- Created: `Assets/Presentation/Fundamentals/ChatbotFundamentalsData.cs` (beats / component order /
  challenge-mode data, static text), `Assets/Presentation/Fundamentals/ChatbotFundamentalsPresenter.cs`
  (renders beats, wires dynamic action/component/challenge buttons, deterministic completion gating)
- Modified: `Assets/Presentation/Shell/GameShellPresenter.cs` (fundamentals screen + presenter fields,
  `ShowFundamentals()`, hub `fundamentalsButton`, `Finished → ShowActHub`, backward-compatible
  `Configure(...)` overloads), `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs`
  (builds the fundamentals screen + hub card, +281 lines)
- Docs: `Docs/LEARNING_CONTENT.md`, `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`;
  run log `Docs/codex_runs/M0-T35_001_fundamentals_teaching.md`
- Regenerated (NOT committed — shelved Game Shell scene side-effect per CLAUDE.md):
  `Assets/Scenes/GameShellPrototype.unity`
- New `.meta` for the new folder/files: `Assets/Presentation/Fundamentals.meta` + the folder's
  `.cs.meta` files (required for Unity to recognize the new scripts)

## Claude Review Notes

- **Scope clean.** `git diff --name-only` touched only Shell (2 files) + the scene + 3 docs, plus the 2
  new Fundamentals files. No `Assets/Scripts/Puzzles`, no Act 1/2/3 validators / sessions / sample data /
  rules, no ProjectSettings / Packages / Build Settings, no existing `.meta` edits.
- **Deterministic-correctness upheld.** The only beat with a right/wrong state is the five-component
  ordering, decided by `ChatbotFundamentalsPresenter.IsSelectedComponentOrderCorrect()` against
  `ChatbotFundamentalsData.CreateComponentOrder()` — pure C#, no LLM. Lily's explanations are static text;
  the LLM is not involved in this sequence and never gates progression.
- **Playable, not a quiz.** Each beat = Ghost problem + Lily explanation + small action (tap / compare /
  arrange / trigger) + visible consequence. No ABCD multiple-choice; `Next` only enables after the action
  produces a consequence.
- **Identity preserved.** Ghost stays a cute ghost with failure-mode reactions; Lily keeps the
  timid/stammering nerdy-senior voice via `LilyDialogueFrame`.
- Run log is honest: Unity Play Mode / Test Runner / scene builder all marked `Not run` in the Codex
  session; static rg / non-ASCII checks recorded.

## Human Verification Result

Passed in the Unity Editor ("通過"): hub card visible; all six beats playable with visible Ghost
consequences; component ordering + backend side link work; skip/finish returns to hub; Acts 1–3 still
launch; no Console errors.

## Next Task

M0-T36 — Strengthen Act 1 as Intent / Key-Topic teaching: keep the existing intent-classification
mechanic, but add in-fiction teaching so the player understands that intent = the user's purpose, that
different wording can share one intent, and that intent grouping is the chatbot-planning step of
identifying key topics. No change to the Act 1 validator / session / sample data.
