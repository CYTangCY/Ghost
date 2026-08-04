# M0-T50 — Local Release Packaging (WebGL + Installer)

## Completion Status

**CLOSED — delivered and verified, then superseded by decision.**

The implementation was completed and independently verified by Claude in the canonical repository. The
user then retired the WebGL delivery path (2026-08-03), so the shipped artifact this task produced is
not the one that will be submitted. The task is closed on its verified result rather than left open
against a deliverable that is being replaced.

## Date

2026-08-03 (Run 001 in the C-drive Codex worktree; Run 002 synced to `D:\Code\Ghost` and reviewed)

## Summary

Built the local release path for Ghost: a Unity WebGL client, a self-contained .NET launcher, a Node
backend and Ollama bundle, and an Inno Setup installer, plus scripted release, installer, and
isolated-environment tests. Run 001 was created in the wrong location (a Codex worktree on `C:`); Run
002 moved the implementation into the canonical repository, rebuilt it there, and repaired five
Chapter 3 missing-script warnings found by the canonical build.

## Files Created

- `Assets/Editor/GhostWebGLReleaseBuilder.cs` (+ `.meta`, `Assets/Editor.meta`)
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphPaletteItemDragView.cs` (+ `.meta`)
- `Assets/Tests/EditMode/ChatbotFundamentalsDataTests.cs` (+ `.meta`)
- `Deployment/Launcher/GhostLauncher.csproj`, `Deployment/Launcher/Program.cs`
- `Deployment/Installer/Ghost.iss`
- `Deployment/build-release.ps1`, `build-installer.ps1`, `test-clean-environment.ps1`,
  `test-installer.ps1`, `README.md`
- `Docs/codex_runs/M0-T50_001_local_webgl_installer.md`, `M0-T50_002_sync_to_canonical_repo.md`

## Files Modified

- `.gitignore` — added `[Bb]in/` plus a `!Deployment/Launcher/GhostLauncher.csproj` negation so the
  launcher project stays tracked despite the blanket `*.csproj` rule.
- `Backend/src/server.ts` — 41 additive lines serving the WebGL client with Brotli/MIME headers when
  `GHOST_WEB_ROOT` is set. **Now dead code**; scheduled for removal in M0-T51.
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphNodeDragView.cs` — reduced to its own class.
- `Assets/Scenes/Act3DialogGraphPrototype.unity` — regenerated; five palette items repointed to the
  separated palette drag component.
- `Assets/Presentation/Fundamentals/ChatbotFundamentalsData.cs` / `ChatbotFundamentalsPresenter.cs` —
  component-order beat removed (reserved for the final chapter), locked by a new test.
- `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`

## Test / Verification Result

Claude verified Run 002 against artifacts in `D:\Code\Ghost` on 2026-08-03, re-running what was cheap
to re-run rather than trusting the run log's summary:

| Check | Result |
|---|---|
| Unity EditMode | 147/147 passed. XML header matches 147 actual `<test-case>` elements, all `Passed`; 22 fixtures sum to exactly 147; every line of the run log's fixture breakdown reconciles. |
| Chapter 3 missing scripts | Resolved. Class now lives in a filename-matching file; five scene references repointed (all `+` lines in the diff); zero null script refs across all ten scenes; zero missing-script hits in the final build log. |
| Backend | 10/10 passed — **re-run independently by Claude**, not taken from the log. |
| Installer | SHA-256 recomputed on the 1.55 GiB binary matches `E49F5D3A…04C50`; size 1,667,664,159 bytes exact; install / launch / uninstall all exit 0; no residue. |
| WebGL | 17 files / 13,043,856 bytes exact; all nine release scenes present in the build log. Browser: title `Unity Web Player | Ghost`, no loading or warning state, canvas 960x600. |
| Scoped changes | No deletions or renames; `Packages/` untouched; `EditorBuildSettings.asset` mtime still `2026-07-15 18:10`, so the build did not rewrite it; all new `.cs` have `.meta`; zero non-ASCII in new/changed C#. |
| `bin`/`obj` exclusion | Confirmed via `git check-ignore`: launcher `bin/` and `obj/` ignored, `GhostLauncher.csproj` and `Program.cs` still tracked. |

**Unity Editor Play Mode: Not run** for this task — Run 002 exercised the browser release rather than
the Editor. Earlier Play Mode evidence remains separate.

**Separate physical clean-machine install: Not run — and now not applicable.** The executed check used
a sanitised `PATH`, an isolated install root, and isolated user data on the development machine. That
is genuine package evidence but was never an external-machine result, and the run log said so
honestly. Because the WebGL artifact is being replaced (see below), this check is closed as no longer
applicable to a shipped deliverable rather than recorded as an outstanding gap.

## Documentation Gaps Recorded at Review

Noted for the record; none blocked closure.

1. Backend results (`npm test`, `npm run build`, production audits) had no stored artifact under
   `Build/`, unlike every other claim. Claude re-ran the tests to confirm 10/10; the audit claim
   remains unevidenced.
2. The run log described the Act 3 scene change as "five palette items repointed" when the diff is a
   full scene regeneration. (Partly attributable to pre-existing uncommitted state.)
3. The removal of the Fundamentals component-order beat — a curriculum-visible change — was listed as
   a file modification without stating that a teaching beat was deleted.

## Decision at Closure (2026-08-03, user)

The WebGL delivery path is retired. The browser build still required the local Node backend and Ollama
for hints, responses, and Lily chat, so it never delivered a zero-install demo — its only advantage —
while the browser's scaling layer actively worsened the project's most-reported UI problem (text too
small; the verified canvas was 960x600).

**What survives:** the `Deployment/` scaffolding — launcher, Inno Setup script, release/installer/
isolation test scripts — and the `.gitignore` and Chapter 3 fixes.

**What is retired:** the WebGL build target and the `GHOST_WEB_ROOT` serving code in
`Backend/src/server.ts`.

M0-T51 retargets the builder to `StandaloneWindows64`, updates the deployment scripts to stage the
standalone player, and deletes the dead WebGL block from `server.ts`.

## Next Task

**M0-T51** — Global visual system + desktop build, the first of six slices specified in
`Docs/M0-T51_T56_EXPERIENCE_POLISH_PLAN.md`.

## Outstanding Archive Debt

M0-T47, M0-T48, and M0-T49 were implemented and are covered by the verified 147-test suite, but have
no completed-task archives. Recorded here so the gap is visible; not resolved by this closure.
