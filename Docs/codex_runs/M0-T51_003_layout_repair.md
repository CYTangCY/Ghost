# M0-T51 — Run 003 — layout repair

## Task ID

M0-T51

## Run Number

003

## Date

2026-08-03

## Original Request / Codex Prompt Summary

Repair only the layout regressions exposed by the M0-T51 type scale. Preserve Claude's lowered
`GhostUITheme` scale and `VerticalWrapMode.Truncate`, change no gameplay or authored data, regenerate
all nine scenes, verify EditMode 147/147, build the nine-scene Windows player, and run backend 10/10.
The user later elected to capture and review the required nine in-level screenshots personally.
Deployment and installer testing were explicitly outside this run.

## Files Created

- `Docs/codex_runs/M0-T51_003_layout_repair.md` — this evidence log.
- `Build/M0-T51-003-EditModeResults.xml` — EditMode NUnit result.
- `Build/M0-T51-003-editmode.log` — Unity EditMode log.
- `Build/M0-T51-003-desktop-build.log` — Windows build log.
- `Build/M0-T51-003-builder-*.log` — one log for each of the nine scene builders.

## Files Modified

- `Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs` — content-sized buttons, taller chapter
  cards, corrected story-route/fundamentals rows, and sufficient Companions text space.
- `Assets/Presentation/Story/Chapter0StoryPresenter.cs` — moved progress and Skip left of the global
  Return-to-Hub overlay.
- `Assets/Presentation/Act1IntentClassification/Act1IntentClassificationStaticPresenter.cs`
- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
- `Assets/Presentation/Act4ConfidenceFallback/Act4ConfidenceStaticPresenter.cs`
- `Assets/Presentation/Act5TestingDebugging/Act5TestingStaticPresenter.cs`
- `Assets/Presentation/Act6BackendResponse/Act6BackendStaticPresenter.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6PipelineStaticPresenter.cs`
- `Assets/Presentation/Act6VoicePipeline/FinalChapterConversationPresenter.cs`
  — header guards, fixed objective height, and scene-specific text/card repairs.
- `Assets/Scenes/GameShellPrototype.unity`
- `Assets/Scenes/Chapter0OpeningStory.unity`
- `Assets/Scenes/Act1IntentClassificationPrototype.unity`
- `Assets/Scenes/Act2EntityExtractionPrototype.unity`
- `Assets/Scenes/Act3DialogGraphPrototype.unity`
- `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity`
- `Assets/Scenes/Act5TestingDebuggingPrototype.unity`
- `Assets/Scenes/Act6BackendResponsePrototype.unity`
- `Assets/Scenes/Act6VoicePipelinePrototype.unity`
  — regenerated through their existing Editor builders; some may be byte-identical to their prior state.
- `Docs/CODE_WALKTHROUGH.md` — Run 003 presentation-layout responsibilities.

## Constants and Constraints Changed

### Shell

- Story route height: `52 -> 60` so its 7px top/bottom padding can contain 44px buttons.
- Fundamentals card: `72 -> 88`; text column `54 -> 72`; title row `24 -> 32`; copy row `26 -> 36`.
  Reason: 26px title and wrapped 15px copy require separate non-truncated rows.
- Act grid: `240 -> 288`; each row/card: `116 -> 140`; title row `26 -> 32`; description row
  `32 -> 36`. Reason: description and 44px button must not share vertical space.
- Hub spacing: `6 -> 5` to keep the expanded content inside the available 1080p shell column.
- Companions title row: `42 -> 36`; description row: `72 -> 78`. Reason: preserve the 150px card
  while giving the wrapped description its required space.
- All shell buttons now use `max(requestedWidth, label.preferredWidth + 32)` and
  `max(requestedHeight, 44)`; button rows also use a 44px minimum.

### Shared chapter chrome

- Header right padding: `220` in Chapters 1-6 and both Final Chapter presenters. Reason: reserve the
  global 210px Return-to-Hub button plus separation.
- Objective `flexibleHeight`: explicitly `0` in Chapters 2-6 and both Final Chapter presenters; Act 1
  already used zero. Reason: hold the objective at 40px during setup as well as in-level.
- Chapter 6 progress: added `minWidth 220` to its existing `preferredWidth 220`.
- legacy Final pipeline progress: added `minWidth 180` to its existing `preferredWidth 180`.
- Chapter 0 progress anchors: `0.69-0.86 -> 0.55-0.72`; Skip anchors:
  `0.875-0.98 -> 0.735-0.855`. Reason: leave the global overlay's top-right region clear.

### Scene-specific repairs

- Act 4 queue title: implicit height -> fixed `36`; guide: `28 -> 30`. Reason: prevent the heading
  and subtitle from sharing painted space.
- Act 5 response cards: `194x70 -> 210x88`; other cards: `188x78 -> 210x96`; title row:
  `22 -> 24`; response detail: `22 -> 34`; other detail: `32 -> 42`. Reason: fit 14-15px wrapped port
  instructions without truncation. Port offsets were not changed: dots remain anchored to card edges,
  and wires still derive endpoints from transformed port centres.
- Final Chapter Ghost face anchor: `0.58 -> 0.66`; size: `105 -> 94`; status region upper anchor:
  `0.38 -> 0.30`. Reason: separate the instruction/status text from the sprite.
- Final Chapter no-backend label: fixed `preferredWidth 300` -> `flexibleWidth 1`; the populated label
  keeps fixed `min/preferredWidth 86`. Reason: use the available route width rather than clip the sentence.

Claude's Act 3 estimates were not changed: `PaletteColumnWidth 190`, `PaletteItemPreferredHeight 96`,
`TestCasePreferredHeight 74`, `ValidationControlsPreferredHeight 40`. Retained because no reliable
post-entry Chapter 3 visual review was completed after the user took over screenshot capture.

## Tests or Checks Run

1. Ran every existing scene builder in Unity 6000.4.11f1, sequentially:
   Chapter 0, Chapters 1-5, Chapter 6 Backend, Final Chapter, and Game Shell.
2. Audited each regenerated scene YAML for Main Camera, Canvas, EventSystem, and missing scripts.
3. Scanned direct `fontSize = N` assignments under `Assets/Presentation` for values below 14.
4. Ran the complete EditMode suite in batch mode without `-quit`; read counts from
   `Build/M0-T51-003-EditModeResults.xml`.
5. Built `Build/Windows/Ghost.exe` with `GhostDesktopReleaseBuilder` and scanned the build log for all
   nine scene names.
6. Ran `npm.cmd test` in `Backend`.
7. Ran `git diff --check -- Assets/Presentation` before documentation updates.
8. Native-player spot checks reached the shell title/name/hub plus in-level Chapters 1 and 2 before
   the user elected to own screenshot capture. Those incomplete captures were deleted so they cannot
   be mistaken for the required nine-scene evidence set.

## Test / Check Result

- Nine scene builders: 9/9 exit 0; each log records successful batch-mode exit; compiler errors: 0.
- Scene invariants: all nine are exactly Main Camera `1`, Canvas `1`, EventSystem `1`, missing script `0`.
- Presentation font-floor scan: no direct numeric `fontSize` assignment below 14.
- EditMode XML: result `Passed`; total `147`; passed `147`; failed `0`; skipped `0`;
  inconclusive `0`; duration `1.9082493` seconds; process exit `0`.
- Windows build: success; `Build/Windows/Ghost.exe` is 667,648 bytes; the build log contains all nine
  scene names and no compiler errors.
- Backend: 10/10 tests passed. The three Ollama-unavailable messages are expected fallback-path test
  evidence, not failures.
- Required all-nine 1920x1080 in-level screenshots and their visual assertions:
  Not run — user elected to capture and review all nine in-level screenshots personally.
- Deployment/installer retest: Not run — explicitly not required by the Run 003 prompt.
- Full end-to-end Play Mode puzzle interaction: Not run — this run changed presentation layout only;
  automated validators remained covered by EditMode, and the user took over the visual/Play Mode pass.

## Errors Encountered

- `apply_patch` could not read repository files because the Windows sandbox helper returned
  `apply deny-read ACLs`. Guarded exact-string replacements were used; every replacement asserted its
  old source text before writing.
- The first desktop-build wrapper inspected the log before detached Unity had finished. The existing
  Unity process was waited to completion; its final log records build success and all nine scenes.
- Native screenshot automation became unnecessary when the user chose to capture the evidence. All
  run-owned partial/debug PNGs were deleted and the run-owned native player was stopped.

## Fixes Applied

- Content-sized shell buttons and expanded chapter-card rows.
- Header title/progress/Return-to-Hub separation across chapters.
- Fixed 40px objective strip in every phase.
- Dedicated Act 4 title/guide heights.
- Larger Act 5 node/card text regions without changing port or wire maths.
- Final Chapter Ghost/status separation and flexible no-backend label.
- Chapter 0 skip/progress separation from Return-to-Hub.

## What Was Intentionally Not Changed

- `GhostUITheme` token values and both `Truncate` settings supplied by Claude.
- Gameplay, validators, simulators, sessions, authored data, and the Act 4 controller/presenter API.
- Act 5 port/wire calculations.
- Claude's four Act 3 estimated layout constants.
- `Packages/`, `ProjectSettings/`, Build Settings, existing `.meta` files or GUIDs.
- Deployment and installer scripts.

## Unity Inspector Setup

None. The affected hierarchy and layout components are generated by the existing scene builders, and
all nine scenes were regenerated during this run.

## Play Mode / Screenshot Steps

1. Open each regenerated scene at 1920x1080.
2. For teaching chapters, press the onboarding action until the puzzle body is present; do not capture
   the setup-only page. For Game Shell use Chapter Select, and for Chapter 0 use an active story beat.
3. Capture Game Shell, Chapter 0, Chapters 1-6, and Final Chapter under `Build/`.
4. Confirm every capture has no text overlap, no mid-word clipping, no element outside its panel, and
   no header collision. Pay particular attention to Act 3's four retained estimates and Act 5 wire
   endpoints after the larger cards.
5. Exercise the primary interaction and inspect Console errors/warnings before closure.

## Assumptions

- 1920x1080 remains the acceptance resolution.
- The global Return-to-Hub overlay remains 210x48 at the existing top-right offset.
- The user's screenshots will be the final visual evidence and may trigger one further layout-only
  correction before M0-T51 closes.

## Remaining Risks

- M0-T51 cannot be visually closed until the user supplies/reviews all nine true in-level screenshots.
- Act 3's `190/96/74/40` estimates have compiled and regenerated but still need the user's on-screen
  confirmation.
- Act 5 uses larger cards with unchanged edge-derived ports; screenshot review must confirm the visual
  wire joins at 1080p.
- Existing CS0618 deprecation-warning debt remains outside this layout-only scope.

## Chinese STAR Summary

- **S 情境：** Run 002 的自動化檢查通過，但 setup 畫面沒有揭露放大字級造成的關卡重疊與裁切。
- **T 任務：** 不改 gameplay、validator 或資料，只修復 M0-T51 的容器尺寸、header、objective 與文字區域。
- **A 行動：** 擴充 shell/card/button 空間、替所有關卡 header 預留返回按鈕、固定 objective 40px，並修復 Act 4、Act 5、Final Chapter 的特定容器；重建九場景並執行測試與 Windows build。
- **R 結果：** 九場景重建及 1/1/1/0 不變量通過，EditMode 147/147、backend 10/10、九場景 Windows build 成功；九張真正 in-level 截圖依使用者決定改由使用者完成，因此視覺 closure 仍待確認。

## Next Recommended Step

User captures the nine required 1920x1080 in-level screenshots and checks the four visual invariants.
Claude reviews those images and this run log. If no defect remains, Claude archives M0-T51, updates
`Docs/HANDOFF_LOG.md`, and advances `Docs/CURRENT_TASK.md`; otherwise Claude issues a narrowly scoped
layout-only correction prompt naming the scene and container.