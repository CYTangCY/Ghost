# Parked — M0-T54 Act 4 two-handle redesign (NOT live code)

These four `.cs` files are **not compiled**. They sit outside `Assets/` on purpose, so Unity ignores
them. Do not copy them into `Assets/` until M0-T54 actually runs.

## Why they are here

Claude wrote and verified this Act 4 redesign on 2026-08-03 while **M0-T51 was still in flight**, and
dropped it straight into `Assets/Scripts/Puzzles/ConfidenceFallback/`. That replaced the live model API
(`Act4ConfidenceConfiguration`, `ExpectedOutcome`, `ActualOutcome`, `StartingThreshold`, …) that
`Act4ConfidenceInteractionController` and `Act4ConfidenceStaticPresenter` still consume. Unity stopped
compiling, which blocked every Unity-side check M0-T51 needed — scene regeneration, the EditMode suite,
the Windows player build, and screenshots.

That was Claude's error, not Codex's. M0-T51 Run 001 correctly refused to "fix" it, because repairing
the mismatch means rewriting the Act 4 controller and presenter — which is M0-T54 gameplay work and
explicitly out of M0-T51's scope.

On 2026-08-03 the original Act 4 sources were restored into `Assets/` from the Codex worktree at
`C:/Users/fcxsw/.codex/worktrees/4e1b/Ghost` (byte-identical originals; `.cs` only — the existing
`.meta` files in `D:` were left untouched, and their GUIDs were confirmed identical anyway).

## What is parked here

| File | Contents |
|---|---|
| `Act4ConfidenceModels.cs` | `Act4Zone`, `Act4Posture`, `Act4VisitorLines`, `Act4ZoneConfiguration` (two handles + per-band wiring), `Act4ShiftTally`; `Act4VisitorMessage` carries **accepted outcomes**, plural |
| `Act4ConfidenceDemoData.cs` | Four authored visitors (88 / 63 / 34-upset / 71-second-pass) with per-band lines, plus `DescribePosture` |
| `Act4ConfidenceValidator.cs` | Three-band routing, the upset-visitor meltdown rule, per-visitor failure explanations, posture reading, scoreboard tally |
| `Act4ConfidenceValidatorTests.cs` | 15 tests including `EveryVisitorFlips`, the permanent guard against a decorative dial |

## Verification status

The pure logic was **executed** in a standalone .NET console harness (it has no UnityEngine
dependency): **44 assertions, all passing**, plus a full sweep of the handle space measuring the
solution set at **667 / 5151 configurations (12.9%)** — Lily handle `35..63`, answer handle `35..71`,
435 Bold / 232 Cautious.

It has **never been run under Unity's NUnit runner.** That is still required when M0-T54 lands.

## Restoring this for M0-T54

1. Copy the three puzzle files back over `Assets/Scripts/Puzzles/ConfidenceFallback/` — `.cs` only,
   leave the existing `.meta` files alone.
2. Copy `Act4ConfidenceValidatorTests.cs` over `Assets/Tests/EditMode/`.
3. Rewrite `Act4ConfidenceInteractionController` and `Act4ConfidenceStaticPresenter` against the new
   API **in the same run** — the two must move together or Unity breaks again.
4. The old presenter test `PresenterUsesCompactSliderHandleAndExplainsThresholdReason` asserts the
   single-handle UI and must be replaced, not kept.

Full specification: `Docs/M0-T51_T56_EXPERIENCE_POLISH_PLAN.md`, section M0-T54.
