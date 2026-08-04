# Codex Prompt — M0-T51 (Global visual system rollout + desktop build)

> Revised 2026-08-03 for the new division of labour. Claude has already written
> `Assets/Presentation/Common/GhostUITheme.cs`. Codex consumes it — do not redesign it.

Copy everything below the line into Codex.

---

You are implementing **M0-T51**, the first of six slices in
`Docs/M0-T51_T56_EXPERIENCE_POLISH_PLAN.md`. Read `Docs/CURRENT_TASK.md` and the plan's sections 1, 2,
3 and 4 (M0-T51) before you start. Implement **only M0-T51**. Do not start M0-T52 or later.

Also read first, as `CLAUDE.md` requires: `Docs/CONFIRMED_PROJECT_CONTEXT.md`, `Docs/ROADMAP.md`,
`Docs/LEARNING_CONTENT.md`, `Docs/ARCHITECTURE.md`.

## Context

Body text sits at 13-14px in low-contrast greys, every panel is a bare rectangle with no sprite, and
the chapter header crowds out the information blocks below it. The cause is that each presenter
carries its own private copy of the same text/panel helpers, so there was never a single place to fix
any of it.

**`Assets/Presentation/Common/GhostUITheme.cs` already exists** — Claude wrote it. It owns the font
sizes, colours, corner radii, a cached runtime 9-sliced rounded-rect sprite generator, and the widget
factories `Panel`, `Card`, `Chip`, `DropZone`, `PushButton`, `Label`, `Heading`, `Stretch`.

**Your job is the rollout, not the design.** Do not change the token values, add tokens, or rewrite
the factories. If a factory genuinely cannot express an existing call site, say so in the run log and
add the smallest possible overload — do not fork a local helper.

## Part 1 — Migrate every presenter onto the theme

Move these onto `GhostUITheme` and **delete their private helpers**: `CreateSmallText`,
`CreateFillText`, `CreateLabel`, `ConfigureOrCreateLabel`, `CreatePanel`, `CreateText`, the duplicated
`ResolveFont`/`LegacyRuntime.ttf` → `Arial.ttf` fallback blocks, and the per-file `TextColor` /
`SubtleTextColor` fields.

`Act1IntentClassificationStaticPresenter`, `Act2EntityExtractionStaticPresenter`,
`Act3DialogGraphStaticPresenter`, `Act4ConfidenceStaticPresenter`, `Act5TestingStaticPresenter`,
`Act6BackendStaticPresenter`, `Act6PipelineStaticPresenter`, `FinalChapterConversationPresenter`,
`Chapter0StoryPresenter`, `AmbientBanterPanel`, `LilyChatWindow`, `GhostFaceView`, `LilyDialogueFrame`,
`Act2EntityTokenDragView`, `Act6PipelinePartDragView`, `Act6BackendCardDragView`, and the four
`Editor/` scene builders.

Mapping rules — apply mechanically, do not improvise:

| Old | New |
|---|---|
| `fontSize` 20 or higher | `GhostUITheme.TitleSize` |
| `fontSize` 18-19 | `GhostUITheme.HeadingSize` |
| `fontSize` 15-17 | `GhostUITheme.BodySize` |
| `fontSize` 13-14 | `GhostUITheme.SmallSize` |
| any `SubtleTextColor` | `GhostUITheme.InkSoft` |
| any primary text colour | `GhostUITheme.Ink` |
| success / failure colours | `GhostUITheme.Good` / `GhostUITheme.Bad` |

Every `Image` used as a panel, card, chip, button, or drop zone must go through the matching factory
so it gets a rounded sprite. **No bare rectangular `Image` may remain in gameplay UI.** Grep for
`AddComponent<Image>()` when you think you are done and justify every remaining direct use in the run
log.

## Part 2 — Re-proportion the chapter page

For every chapter scene builder:

- Header: 56px → **44px**, title at `TitleSize`, phase progress right-aligned, trimmed padding.
- Objective strip: 48px → **40px**, at `HeadingSize`.
- The three information blocks: **minimum 96px each**, `BodySize` text, 12px internal padding, and a
  `flexibleHeight` so they absorb the space reclaimed from the header. They must expand, never clip.
- Ghost conversation panel stays 170px.

## Part 3 — WebGL out, Windows standalone in

M0-T50 is closed and the WebGL delivery path is retired: the browser build still needed the local
backend and Ollama, so it never gave a zero-install demo, and the browser's scaling layer was part of
why text looked small.

- Convert `Assets/Editor/GhostWebGLReleaseBuilder.cs` to `GhostDesktopReleaseBuilder.cs` targeting
  `BuildTarget.StandaloneWindows64`. Keep the same nine-scene list, the same scene-existence check, and
  the same throw-on-failure contract.
- Update `Deployment/build-release.ps1`, `Deployment/Installer/Ghost.iss` and `Deployment/README.md` to
  stage and install the standalone player instead of the WebGL folder.
- **Delete the dead WebGL block from `Backend/src/server.ts`** — the `GHOST_WEB_ROOT` resolution, the
  `express.static` mount, and the Brotli/MIME helper (the 41 lines added in M0-T50). Backend tests must
  still pass 10/10.
- Drop the WebGL assertions from `Deployment/test-installer.ps1`; keep install, launch, uninstall and
  the residue check.

## Part 4 — Code-quality cleanup (every file you touch)

Required, and it must not change behaviour.

- Delete comments that restate the code. Keep comments that explain a *reason* or a non-obvious
  constraint.
- Shorten ceremonial names to idiomatic ones — `intentGroupListRoot` → `pileList`,
  `ConfigureOrCreateLabel` → `SetLabel`. **Short and natural, never careless or random**; deliberately
  sloppy naming is not what is being asked for.
- Remove dead code and unused fields as you meet them.
- The duplicated font-resolution and text/panel helpers are the single largest real duplication in this
  codebase. Collapsing them into the theme is the point of this slice.

## Constraints

- ASCII-only C#. 1920x1080 fit. No Console errors.
- **Do not touch any validator, simulator, session, or authored puzzle data.** This is a presentation
  and build slice only. Puzzle correctness stays deterministic and untouched.
- Do not modify `Packages/`, `ProjectSettings/`, or existing `.meta` GUIDs. Do not hand-edit scene
  YAML — regenerate through the builders.
- Ghost stays a cute ghost; Lily stays the timid, nerdy, kind postdoc.
- `GhostUITheme.cs` needs a `.meta`; let Unity generate it on import and keep it.

## Required verification

Report real numbers from real runs. If something was not run, write `Not run - [reason]`.

1. Regenerate every chapter scene with the existing scene builders.
2. Confirm each regenerated scene has exactly one Main Camera, one Canvas, one EventSystem, and zero
   missing scripts.
3. Run the Unity EditMode suite in batch mode **without `-quit`** so the results XML is written, and
   report pass/fail counts read from the XML. It was 147/147 at M0-T50 and must not regress.
4. `npm test` in `Backend/` — expect 10/10.
5. Build the Windows standalone player; confirm success with all nine scenes.
6. **Capture 1920x1080 screenshots of at least four chapters** (including Chapter 1 and Chapter 4) and
   save them under `Build/`. Claude will review these — they are the only way the visual result gets
   checked before the user's Play Mode pass.
7. Grep for remaining `AddComponent<Image>()` without a rounded sprite and list them.

## Deliverable

Write an honest run log at `Docs/codex_runs/M0-T51_001_global_visual_system.md` per
`Docs/codex_runs/README.md`: actions, decisions, results and evidence only, no chain-of-thought. Never
claim a test passed that you did not execute. Include a Chinese STAR summary (S情境 / T任務 / A行動 /
R結果).

Then return a Claude review/closure prompt.
