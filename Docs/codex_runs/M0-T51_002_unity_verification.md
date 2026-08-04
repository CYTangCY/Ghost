# M0-T51 — Run 002 — Unity verification

## Task ID

M0-T51

## Run Number

002

## Date

2026-08-03

## Original Request / Codex Prompt Summary

Verify the M0-T51 visual-system and Windows desktop rollout after Claude restored the coherent Act 4 sources. Add no features. Repair only genuine M0-T51 regressions, regenerate all nine scenes, audit post-regeneration scene invariants and layout proportions, run the full EditMode suite, build the Windows player, save four 1920x1080 chapter screenshots including Chapters 1 and 4, run staged and installed clean-environment tests, and report Chapter 1/4 Console evidence. Leave the legacy `GhostWebGLReleaseBuilder.cs` filename and the three existing layout-helper names unchanged.

## Files Created

- `Docs/codex_runs/M0-T51_002_unity_verification.md` — this evidence log.
- `Build/M0-T51-002-EditModeResults.xml` — Unity EditMode result XML.
- `Build/M0-T51-002-EditMode.log` — Unity EditMode log.
- `Build/M0-T51-002-builders/*.log` — one log for each of the nine scene-builder runs.
- `Build/M0-T51-002-desktop-build.log` — direct `GhostDesktopReleaseBuilder` build log.
- `Build/M0-T51-002-Chapter1-1920x1080.png` — 1920x1080 native-player capture, 99,481 bytes.
- `Build/M0-T51-002-Chapter2-1920x1080.png` — 1920x1080 native-player capture, 98,291 bytes.
- `Build/M0-T51-002-Chapter4-1920x1080.png` — 1920x1080 native-player capture, 107,895 bytes.
- `Build/M0-T51-002-Chapter6Backend-1920x1080.png` — 1920x1080 native-player capture, 112,297 bytes.
- `Build/M0-T51-002-*-visual-build.log` and `Build/M0-T51-002-*-player-dpiaware.log` — auxiliary native-player build and capture evidence.
- `Build/M0-T51-002-build-release-transcript.log`, `Build/M0-T51-002-build-installer-transcript.log`, `Build/M0-T51-002-clean-environment-rerun-transcript.log`, and `Build/M0-T51-002-installer-test-transcript.log` — deployment command transcripts.
- Fresh release outputs under `Build/Windows/`, `Build/Release/Ghost/`, and `Build/Installer/GhostSetup.exe`.

Temporary per-chapter native players and preview images were removed after the four final screenshots were saved. The canonical `Build/Windows/Ghost.exe`, release package, installer, screenshots, logs, and XML were retained.

## Files Modified

- `Assets/Presentation/Act1IntentClassification/Editor/Act1IntentClassificationPrototypeSceneBuilder.cs` — changed the invalid seven-argument `GhostUITheme.DropZone` call to the existing name/parent/fill overload, then reapplied the original anchors and zero offsets to its `RectTransform`.
- `Assets/Scenes/Chapter0OpeningStory.unity` — regenerated through its builder.
- `Assets/Scenes/Act1IntentClassificationPrototype.unity` — regenerated through its builder.
- `Assets/Scenes/Act2EntityExtractionPrototype.unity` — regenerated through its builder.
- `Assets/Scenes/Act3DialogGraphPrototype.unity` — regenerated through its builder.
- `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity` — regenerated through its builder.
- `Assets/Scenes/Act5TestingDebuggingPrototype.unity` — regenerated through its builder.
- `Assets/Scenes/Act6BackendResponsePrototype.unity` — regenerated through its builder.
- `Assets/Scenes/Act6VoicePipelinePrototype.unity` — regenerated through its builder.
- `Assets/Scenes/GameShellPrototype.unity` — regenerated through its builder.
- `Docs/CODE_WALKTHROUGH.md` — added the required Run 002 compile-repair, Inspector, and Play Mode guidance.

The repository was already broadly dirty before this run. Existing unrelated and earlier-task changes were preserved. No pre-existing change was reset or attributed to Run 002.

## Tests or Checks Run

### Act 1 compile regression

- `git diff --check` on `Act1IntentClassificationPrototypeSceneBuilder.cs`.
- Unity compilation during the Chapter 0 and Chapter 1 builder runs.

### All nine scene builders

The following existing methods ran in Unity 6000.4.11f1 and each log ended with `Batchmode quit successfully invoked` and `Exiting batchmode successfully now`:

1. Chapter 0 Opening Story.
2. Act 1 Intent Classification.
3. Act 2 Entity Extraction.
4. Act 3 Dialog Graph.
5. Act 4 Confidence Fallback.
6. Act 5 Testing and Debugging.
7. Act 6 Backend Action and Response.
8. Act 6 Voice Pipeline / Final Chapter.
9. Game Shell.

### Post-regeneration scene invariants

| Scene | Main Camera | Canvas | EventSystem | `m_Script: {fileID: 0}` |
|---|---:|---:|---:|---:|
| Chapter0OpeningStory | 1 | 1 | 1 | 0 |
| Act1IntentClassificationPrototype | 1 | 1 | 1 | 0 |
| Act2EntityExtractionPrototype | 1 | 1 | 1 | 0 |
| Act3DialogGraphPrototype | 1 | 1 | 1 | 0 |
| Act4ConfidenceFallbackPrototype | 1 | 1 | 1 | 0 |
| Act5TestingDebuggingPrototype | 1 | 1 | 1 | 0 |
| Act6BackendResponsePrototype | 1 | 1 | 1 | 0 |
| Act6VoicePipelinePrototype | 1 | 1 | 1 | 0 |
| GameShellPrototype | 1 | 1 | 1 | 0 |

### Layout proportions and rendering

- Parsed regenerated scene YAML by GameObject and `LayoutElement` reference.
- Confirmed authored 44 px headers and 40 px objective strips in every teaching/final gameplay scene.
- Confirmed 170 px conversation elements in every teaching/final gameplay scene.
- Confirmed the information/body rule is a 96 px minimum with flexible height, matching `Docs/CURRENT_TASK.md`; flexible blocks may render taller when spare vertical space is distributed.
- Act 1 serialized evidence includes `Header` 44/44, `Objective Strip` 40/40, `Lily Intent Teaching Panel` 96/96, and `Ghost Conversation Demo` 170/170.
- Act 3 serialized evidence includes `Header` 44/44, `Objective Strip` 40/40, `Lily Note Strip` 96/96, and `Conversation Panel` 170/170.
- Chapter 1, Chapter 2, Chapter 4, and Chapter 6 Backend were rendered by isolated native players. A per-monitor-DPI-aware capture process launched or sized the player at 1920x1080 and asserted the HWND capture dimensions before saving each PNG. The four final frames show the full hierarchy rather than the cropped preview produced by the first DPI-virtualized attempt.
- `ConfigureLayoutElement`, `SetHeight`, and `AddHeight` were not renamed or unified.

### Unity EditMode suite

Command used `-batchmode -nographics -runTests -testPlatform EditMode -testResults ...` without `-quit`.

XML result:

- Total: 147
- Passed: 147
- Failed: 0
- Skipped: 0
- Inconclusive: 0
- Result: Passed
- XML duration: 2.0990681 seconds
- Unity process exit: 0

### Windows standalone player

`Ghost.Editor.Deployment.GhostDesktopReleaseBuilder.BuildDesktopRelease` ran directly.

- Process exit: 0
- Output: `Build/Windows/Ghost.exe`
- Executable size: 667,648 bytes
- Build report total size: 105,765,151 bytes
- All nine scene paths appear as `Opening scene` and `Loaded scene` entries in `Build/M0-T51-002-desktop-build.log`.
- A later full `Deployment/build-release.ps1` rebuild also succeeded; its build report total size was 105,765,949 bytes.

### Backend and release staging

`Deployment/build-release.ps1` ran without skip switches.

- Backend Vitest: 10 passed, 0 failed.
- TypeScript build: succeeded.
- Launcher publish: succeeded.
- Production backend dependency install: 0 vulnerabilities.
- Staged package: `Build/Release/Ghost`.
- Staged release size: 2,032,422,636 bytes.
- Native player location: `Build/Release/Ghost/app/player/Ghost.exe`.

The development dependency audit printed 7 findings: 4 moderate, 2 high, and 1 critical. This run did not change dependency versions because M0-T51 Run 002 is verification-only.

### Staged clean-environment test

The first invocation returned exit 0 but its launcher log showed `EADDRINUSE` on ports 3000 and 11435 from a previously installed Ghost launcher. That result was rejected as non-isolated evidence. The exact pre-existing Ghost package processes were identified by executable path and stopped; the system Ollama process on port 11434 was left unchanged. Ports 3000 and 11435 were confirmed free before rerun.

Rerun result:

- Script exit: 0
- Sanitised `PATH`: `C:\WINDOWS\System32;C:\WINDOWS`
- Temporary user data: used and removed by the script.
- Duration: 33.4 seconds.
- Bundled Ollama listened on 127.0.0.1:11435 with cloud disabled.
- Bundled backend listened on localhost:3000.
- Player evidence: `Windows player payload found at D:\Code\Ghost\Build\Release\Ghost\app\player\Ghost.exe`.
- Final evidence: `Self-test passed: Windows player payload, REST backend, SQLite startup, Ollama model discovery, and model-backed hint.`
- No WebGL page or browser launch was requested. The model runner command included `--no-webui --offline`.

### Installer build and isolated installer test

`Deployment/build-installer.ps1` used Inno Setup 6.7.3.

- Compile result: success.
- Installer: `Build/Installer/GhostSetup.exe`.
- Installer size: 1,688,221,541 bytes.
- SHA-256: `702DCADF3097B55E88E9971CFCCB05C335F5428C73610FEF624AED5950E41A0B`.

`Deployment/test-installer.ps1` result:

- Installer exit: 0
- Installed launcher self-test exit: 0
- Uninstaller exit: 0
- Install root remains after uninstall: False
- Duration: 122.98 seconds
- Installed player evidence: `App/app/player/Ghost.exe` under the temporary install root.
- Model runner command included `--no-webui --offline`.
- No WebGL/browser route was exercised.

### Chapter 1 and Chapter 4 Console/log check

The Chapter 1 and Chapter 4 builder runs opened and saved those scenes in an Editor process. Their Editor logs and the corresponding native-player logs were scanned for `warning CS`, `error CS`, `NullReferenceException`, `MissingReferenceException`, `Unhandled Exception`, `Exception:`, and `Assertion failed`.

Per log:

- Chapter 1 Editor builder log: 0 compiler warnings, 0 compiler errors, 0 runtime exceptions, 0 assertions.
- Chapter 4 Editor builder log: 0 compiler warnings, 0 compiler errors, 0 runtime exceptions, 0 assertions.
- Chapter 1 player log: 0 compiler warnings, 0 compiler errors, 0 runtime exceptions, 0 assertions.
- Chapter 4 player log: 0 compiler warnings, 0 compiler errors, 0 runtime exceptions, 0 assertions.

The pre-run Editor output supplied with the request contained nine non-blocking CS0618 warnings that remain intentionally unchanged:

- Five uses of `Object.FindFirstObjectByType<T>()`: `FindFirstObjectByType has been deprecated because it relies on instance ID ordering. Use FindAnyObjectByType instead, which does not depend on ordering.` The paths are Chapter 0 Story, two Shell Return calls, Act 6 Backend, and Act 3 Dialog Graph.
- Two `FindObjectsSortMode` warnings in the Act 4 and Final Chapter tests: `FindObjectsSortMode has been deprecated. Use the FindObjectsByType overloads that do not take a FindObjectsSortMode parameter.`
- Two `Object.FindObjectsByType<T>(FindObjectsSortMode)` warnings in the same test files: `FindObjectsByType with FindObjectsSortMode parameter has been deprecated. Use FindObjectsByType<T>() or FindObjectsByType<T>(FindObjectsInactive) instead. InstanceID will be replaced in the future with EntityId and previous sort order cannot be maintained.`

The clean-environment and installer launcher logs also contain one Ollama warning: `AMD driver is too old. Update your AMD driver to enable GPU inference.` Ollama then selected the NVIDIA GeForce RTX 4050 Laptop GPU and completed the model-backed hint.

### Interactive Play Mode

Not run — this verification run captured real native-player rendering and inspected Chapter 1/4 Editor-open logs, but it did not execute the chapters' full drag, click, validation, reset, completion, or Shell return flows interactively.

## Test / Check Result

Passed for the requested automated M0-T51 gates after one compile-only M0-T51 repair and one clean-environment rerun:

- 9/9 scene builders completed.
- 9/9 regenerated scenes passed all four invariants.
- EditMode: 147/147 passed with 0 failed and 0 skipped.
- Backend: 10/10 passed.
- Direct desktop build: succeeded with all nine scenes.
- Four final PNGs: each exactly 1920x1080, including Chapters 1 and 4.
- Staged clean-environment test: exit 0 from newly started bundled services.
- Installer test: install 0, self-test 0, uninstall 0, no residue.
- Chapter 1/4 new-run logs: no compiler error/warning, runtime exception, or assertion.

## Errors Encountered

1. The request's initial compile failed at Act 1 scene-builder line 238 because no `DropZone` overload accepted seven arguments.
2. The workspace `apply_patch` helper could not read the Windows worktree because the sandbox ACL helper returned `apply deny-read ACLs`. Guarded exact-match PowerShell file updates were used instead.
3. The first attempt to run several Unity builders overlapped because the Unity launcher detached before the real process completed. The affected builders were rerun with explicit process-handle waits.
4. Unity left Roslyn `VBCSCompiler` child processes alive after several successful builds, which held PowerShell's process-tree wait open. Each compiler server was stopped only after the corresponding Unity log showed the desktop build success and successful batch exit.
5. The first screenshot capture path was DPI-virtualized and showed a cropped effective view. Those images were rejected and replaced with per-monitor-DPI-aware 1920x1080 captures; the rejected previews were removed.
6. The first staged self-test reused ports owned by a previous Ghost installation. That result was rejected; only the rerun with ports confirmed free is reported as the clean result.
7. Non-blocking warnings remain: the supplied CS0618 warnings, the Ollama AMD-driver warning for the unused integrated GPU, and the development npm audit findings.

## Fixes Applied

- Replaced the invalid Act 1 `DropZone` call with the existing name/parent/fill overload.
- Restored the same `anchorMin`, `anchorMax`, `offsetMin`, and `offsetMax` values directly on the returned `RectTransform`.
- Added no overload and changed no gameplay or puzzle state.
- Regenerated all nine scenes through their existing builders.
- Repeated rejected verification attempts until the evidence was isolated and correctly scaled.

## What Was Intentionally Not Changed

- No gameplay, validator, simulator, session, authored puzzle data, or Act 4 controller/presenter API.
- No `Packages/` changes.
- No hand edit to scene YAML; scene changes came from the builders.
- No `.meta` deletion or rename.
- No intentional `ProjectSettings` edit. Existing dirty ProjectSettings files were preserved.
- `Assets/Editor/GhostWebGLReleaseBuilder.cs` retains its legacy filename while the class remains `GhostDesktopReleaseBuilder`.
- `ConfigureLayoutElement`, `SetHeight`, and `AddHeight` remain separate helper names.
- The CS0618 modernization was not folded into this verification-only run.
- Existing unrelated dirty-worktree changes were not reset, cleaned, staged, or committed.

## Unity Inspector Setup

No manual Inspector setup is required. The nine scenes were regenerated and saved by their existing builders. Keep each presenter's existing render-on-start configuration.

## Play Mode Test Steps

1. Open `Assets/Scenes/Act1IntentClassificationPrototype.unity` and set Game view to 1920x1080.
2. Enter Play Mode and confirm the header/progress, objective strip, flexible teaching blocks, 170 px conversation panel, piles/cards, lower validation controls, and Return to Hub overlay remain visible without overlap.
3. Exercise Act 1 drag/drop, teaching, revise, validation, completion, and immediate Return to Hub paths.
4. Repeat in `Assets/Scenes/Act4ConfidenceFallbackPrototype.unity`, covering onboarding, threshold control, both routes, six-visitor playback, retry, completion, and Return to Hub.
5. Confirm the Console adds no errors; record any warning text separately from the known CS0618 compiler warnings.

## Assumptions Made

- The restored Act 4 sources supplied by Claude are the approved M0-T51 baseline.
- The four screenshots are visual-review evidence of the native initial chapter render, not evidence that every interaction path was exercised.
- A sanitised-PATH temporary-user-data self-test on this machine is the requested automated clean-environment test; it is not a separate physical clean Windows machine.

## Remaining Risks

- Full interactive Play Mode gameplay remains a human check.
- Claude still needs to review the four 1920x1080 screenshots for type scale, contrast, rounded surfaces, and visual clipping.
- The supplied CS0618 warnings remain technical debt.
- The packaged Ollama runtime warns that the AMD integrated-GPU driver is old, although it selected the RTX 4050 and completed inference.
- The backend development dependency tree reports 7 audit findings; production staging reported 0 vulnerabilities.
- The repository remains broadly dirty from earlier work, so the user should review `git status` and `git diff` before any commit.

## Chinese STAR Summary

- **S 情境：** M0-T51 Run 001 已完成視覺系統與桌面版路徑，但 Act 4 API 不一致阻擋 Unity 編譯與後續驗證；Claude 已恢復一致的 Act 4 原始碼。
- **T 任務：** 不新增功能，只修復 M0-T51 自身的編譯回歸，完成九個場景重建、場景不變量、147 項 EditMode、桌面建置、四張 1920x1080 截圖、乾淨環境與安裝器驗證。
- **A 行動：** 修正 Act 1 的 `DropZone` 呼叫並保留原錨點；重建九個場景；從 XML、Unity 日誌、PNG 尺寸、launcher 日誌與安裝器結果讀取實際證據；拒絕 DPI 錯誤截圖與埠衝突的第一次乾淨環境結果後重新執行。
- **R 結果：** 九個場景全部通過 1/1/1/0 不變量；EditMode 147/147、Backend 10/10；九場景 Windows player 建置成功；四張最終截圖皆為 1920x1080；乾淨環境 exit 0；安裝、自測、解除安裝皆為 0 且無殘留。完整互動 Play Mode 與 Claude 視覺審查仍待人工完成。

## Next Recommended Step

Give this log, the four `Build/M0-T51-002-*-1920x1080.png` files, `Build/M0-T51-002-EditModeResults.xml`, the direct desktop-build log, and the clean/installer result files to Claude for scope review and M0-T51 closure. Claude should confirm whether the screenshots are visually acceptable, preserve the known warning debt for a later approved task, archive M0-T51, update the handoff, and advance `Docs/CURRENT_TASK.md` only after review.