# CURRENT_TASK.md

## ID

M0-T47

## Status Note

M0-T46 run 001 is code-reviewed (scope clean) and awaits the user's Play Mode verification before
closure; its Part C (Lily portrait iterations) continues as side-runs under M0-T46 ids when the user
gives portrait feedback. Per the user's direction (2026-07-05, "build Acts 4/5/6 and the ending first;
I will revise after"), M0-T47 is now the active implementation task.

## Goal

Build the remaining three chapters of the 6-chapter ship plan (ROADMAP 2026-07-04 v2), one slice at a
time, to the M0-T46 experience standard: **Chapter 4 = Act 4 Confidence & Fallback (threshold slider)**,
**Chapter 5 = Act 5 Testing & Debugging**, **Chapter 6 = final capstone "Repair Ghost's Voice" + ending
animation and credits**. Lean first versions — the user will iterate afterwards. Everything follows the
teaching-as-gameplay standard (player's work is USED by Ghost with visible consequences; no lecture
walls, no quiz dumps) and the deterministic-correctness rule (validators only; the LLM never scores or
gates).

## Global Requirements (all slices)

- **Mapping before implementation**: per `Docs/LEARNING_CONTENT.md` rules, write that slice's Act
  mapping (topic / objective / Ghost problem / mechanic / action / success / failure / Lily hint
  style) into LEARNING_CONTENT BEFORE implementing the level. Content basis:
  `Docs/IBM_COURSE_CONTENT.md` §1.2, §1.8, §1.10, §1.3 and ROADMAP Phase E/F notes.
- M0-T46 page composition: 56px header with phase progress, 48px objective strip, onboarding panel
  (Lily how-to, dismiss + replayable note), 170px Ghost conversation/result panel with `GhostFaceView`
  (deterministic mood mapping), flexible puzzle body, retry ("Try again") and "Complete Act" via the
  existing Shell pending-debrief path.
- New pure logic + authored data as NEW files in `Ghost.Runtime` beside the existing puzzle folders,
  with EditMode tests. Existing validators/sessions/sample data/demo engines are NOT modified.
- Shell integration per act (the M0-T19/M0-T31 pattern): scene builder + generated scene,
  `ShellSceneNames`, hub card in `GameShellSceneBuilder`, `ShellReturnToHubOverlay`,
  `ShellDialogueData` intro/debrief beats, `GhostNarrativeState` act id. Build Settings additions for
  the new scenes are an APPROVED exception (only add the new scene entries; change nothing else).
- Reuse shared components (`GhostAvatar`, `Common/FloatingWindowDragHandle`, banter hooks). ASCII-only
  C#; 1920×1080 fit; no Console errors; honest run log per slice; Chinese STAR per run.

## Slice A — M0-T47 run 001: Act 4 Confidence & Fallback (IBM §1.2, §1.8; slider mechanic)

- Fiction: Ghost either bluffs answers to messages it barely understands, or freezes on everything —
  its confidence dial is broken, and one angry/complex visitor needs a human (Lily) handoff.
- Data (new, authored, deterministic — e.g. `Act4ConfidenceDemoData`): a small pre-built reply map
  summary; ~6 visitor messages each with an authored per-intent confidence score, an authored
  expected outcome for any configuration, including one clear-intent message, ambiguous messages, one
  garbled message, and one hard/frustrated case that must be handed off.
- Mechanic: ONE confidence-threshold slider (0–100) + wiring a Fallback node ("ask to rephrase") and a
  Handoff node ("call Lily") — wiring may reuse Act 3 port/wire components or simple attach buttons.
- Consequence: "Run the day" — messages play through the conversation panel one by one: score >=
  threshold → intent reply; below → fallback; handoff case → Lily rescue beat. Authored cute failures:
  threshold too LOW → Ghost confidently answers the garbled message wrongly; too HIGH → Ghost asks
  everyone to rephrase (annoyed visitors); missing handoff → meltdown on the hard case. Ghost face per
  outcome.
- Completion (new pure validator, e.g. `Act4ConfidenceValidator` + tests): threshold within the
  authored acceptable range AND fallback+handoff wired AND the day-run outcomes all match expected.
- Optional one-line sentiment note: the hard case is routed partly because it sounds upset (sentiment
  as a routing signal, never scoring) — keep to one authored beat.

## Slice B — M0-T48 run 001: Act 5 Testing & Debugging (IBM §1.10; reuses Act 3 infrastructure)

- Fiction: Ghost's reply map "looks done" but visitors keep getting wrong answers; Lily suggests
  running the test suite before trusting it.
- Data (new, authored): `Act5BuggyGraphData` — a pre-built Act 3-style dialog graph with 2–3 seeded
  faults (e.g. a swapped transition condition, a wrong response node, a missing intent branch) + an
  authored test-conversation suite with expected responses.
- Mechanic: run the test suite (EXISTING `DialogGraphSimulator`/`DialogGraphValidator` — no new
  scoring); failing cases list shows expected vs actual; the player edits the graph using the existing
  Act 3 editing interactions to fix the faults; re-run until green.
- Completion: all authored test cases pass via the existing validator. EditMode tests must prove the
  seeded buggy graph FAILS the suite and a reference fixed graph PASSES it.
- Teaching beat: the objective strip frames it as the course's preview → test → revise loop.

## Slice C — M0-T49 run 001: Final chapter — capstone "Repair Ghost's Voice" + ending (IBM §1.3)

- Fiction: everything the player trained is ready; reconnect Ghost's voice pipeline and bring its
  voice back for good.
- Mechanic (capstone, depth version of the fundamentals overview beat): assemble the five components
  in order — UI input → NLP engine → dialogue management → response generation → UI output — plus the
  backend side link, via drag/ordered placement. Each correctly placed component shows an authored
  line naming what the player built there (NLP engine ← your Act 1 intent piles + Act 2 entities;
  dialogue management ← your Act 3 reply map + Act 4 threshold; tested in Act 5...). New pure data +
  order validator (e.g. `Act6PipelineData`/`Act6PipelineValidator`) + tests — do NOT modify the
  Fundamentals files.
- Consequence: one final visitor message travels the assembled pipeline stage by stage (authored,
  deterministic), and Ghost answers in full natural speech for the first time.
- Ending animation (programmatic, no external assets): staged sequence — Ghost glowing/happy (face +
  simple color/position tweens), a thank-you line addressing `GhostNarrativeState.PlayerName`, a Lily
  closing beat (in character: proud, still a bit stammery), a simple credits scroll, then return to
  the title screen. Skippable with one button.
- Shell: final chapter hub card; if earlier acts are incomplete, show a gentle Lily suggestion line
  but do not hard-gate.

## Files Codex must NOT modify

Existing `Ghost.Runtime` pure logic files and their tests; Acts 1–3 presentation (just shipped in
M0-T46 — bug fixes only if a slice truly requires wiring, recorded in the run log); Fundamentals;
Banter/Common/GhostAvatar internals (attach, don't rewrite); Backend; ProjectSettings; Packages;
existing `.meta`; hand-edited scene YAML. Build Settings: ONLY append the new Act 4/5/final scene
entries (approved exception).

## Acceptance Criteria (per slice)

- The slice's LEARNING_CONTENT mapping exists before its implementation; the level is playable
  end-to-end (onboarding → build/tune → consequence playback → retry → Complete Act → hub debrief).
- Correctness is deterministic (new/existing validators; LLM absent from the loop); EditMode tests
  pass for the new pure logic; Acts 1–3 and Act 2 behaviour unchanged.
- Shell hub shows the new chapter card; scene registered in Build Settings; 1080p fit; no Console
  errors; docs (LEARNING_CONTENT / CODE_WALKTHROUGH / UNITY_TEST_CHECKLIST) + run log updated.
