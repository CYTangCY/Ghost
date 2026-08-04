# Codex Prompt — M0-T51 Run 002 (verification only)

> The Act 4 API mismatch that blocked Run 001 has been reconciled by Claude. Unity should compile now.
> This run adds **no features**. It runs the checks Run 001 could not reach and reports real numbers.

Copy everything below the line into Codex.

---

You are running **M0-T51 Run 002**. Run 001 (`Docs/codex_runs/M0-T51_001_global_visual_system.md`)
completed the theme rollout, the chapter proportions, and the desktop build conversion, but every
Unity-side check was blocked by an Act 4 model/controller API mismatch.

**That mismatch is now fixed.** Claude restored the original Act 4 sources into
`Assets/Scripts/Puzzles/ConfidenceFallback/` (`.cs` only; `.meta` files untouched) and parked the
future two-handle redesign in `Docs/planned/M0-T54/`, which is outside `Assets/` and not compiled.

## Scope — verification only

Implement nothing new. Do not touch gameplay, puzzle logic, authored data, or the Act 4
controller/presenter API. If a check fails, report it; only fix things that are genuinely M0-T51's own
regressions (a bad theme call, a wrong layout number, a missing `using`).

## Required checks — report real numbers, never claim an unrun check

1. **All nine scene builders.** Run every chapter's builder and save the scenes:
   Chapter 0, Acts 1-5, Act 6 Backend, Act 6 Voice Pipeline, Game Shell.
2. **Scene invariants on the regenerated scenes** — exactly one Main Camera, one Canvas, one
   EventSystem, and zero `m_Script: {fileID: 0}` per scene. Run 001 only audited the *pre-existing*
   scene YAML; this must be the post-regeneration state.
3. **EditMode suite** in batch mode **without `-quit`** so the XML is written. Report pass/fail/skip
   read from the XML. Baseline is **147/147** at M0-T50, and the restored Act 4 test file is the
   original six-test version, so **expect 147 with no regression**. Any deviation must be explained.
4. **Windows standalone player build** via `GhostDesktopReleaseBuilder` — confirm success and that all
   nine scenes are in the build log.
5. **Four 1920x1080 screenshots**, which must include **Chapter 1 and Chapter 4**, saved under
   `Build/`. Claude reviews these for the type scale, contrast, and rounded surfaces.
6. **Staged and installed clean-environment tests** — `Deployment/build-release.ps1`, then
   `test-clean-environment.ps1` and `test-installer.ps1`. Confirm the launcher starts the native player
   from `app/player/Ghost.exe` and that no WebGL or browser path is exercised.
7. **Console check** — open at least Chapters 1 and 4 in the Editor and report any Console errors or
   warnings, with text.

## Two known items to confirm, not fix

- `Assets/Editor/GhostWebGLReleaseBuilder.cs` still has the old filename while the class is
  `GhostDesktopReleaseBuilder`. Leave it. A later approved run renames `.cs` and `.meta` together.
- The chapter presenters set the 44/40/96/170 proportions through three differently-named helpers
  (`ConfigureLayoutElement`, `SetHeight`, `AddHeight`). Confirm the **rendered** heights are correct;
  do not unify the helper names in this run.

## Deliverable

`Docs/codex_runs/M0-T51_002_unity_verification.md` — actions, decisions, results, evidence only, no
chain-of-thought. Use `Not run - [reason]` where applicable. Include a Chinese STAR summary. Then
return a Claude review/closure prompt.
