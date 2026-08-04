# M0-T50 - Run 002 - Sync to canonical repository and rebuild release

## Task ID

M0-T50

## Run Number

002

## Date

2026-08-03

## Original Request / Codex Prompt Summary

Run 001 had been created in the Codex worktree under `C:` instead of the canonical repository at
`D:\Code\Ghost`. The user authorised moving the implementation to the canonical repository,
rebuilding it there, resolving the five Chapter 3 build warnings, and recording evidence that
Claude can review in the correct location.

`Docs/CURRENT_TASK.md` still identifies M0-T47. M0-T50 is already defined in `Docs/ROADMAP.md`, and
the user explicitly requested this implementation. CURRENT_TASK, HANDOFF_LOG, and the completed-task
archive were not advanced because closure remains Claude's responsibility.

## Files Created

- `Assets/Editor.meta` and `Assets/Editor/GhostWebGLReleaseBuilder.cs` with its `.meta`.
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphPaletteItemDragView.cs` with its `.meta`.
- `Assets/Tests/EditMode/ChatbotFundamentalsDataTests.cs` with its `.meta`.
- `Deployment/Launcher/GhostLauncher.csproj` and `Program.cs`.
- `Deployment/Installer/Ghost.iss`.
- `Deployment/build-release.ps1`, `build-installer.ps1`, `test-clean-environment.ps1`,
  `test-installer.ps1`, and `README.md`.
- `Docs/codex_runs/M0-T50_001_local_webgl_installer.md` was copied as the historical Run 001 record.
- `Docs/codex_runs/M0-T50_002_sync_to_canonical_repo.md`.

Generated evidence under ignored `Build/` includes the WebGL client, staged release, installer,
Unity logs, EditMode XML, launcher logs, clean-environment results, browser result, and screenshot.

## Files Modified

- `.gitignore`: added the .NET `bin` exclusion while keeping the launcher project trackable.
- `Backend/src/server.ts`: serves WebGL and the required Brotli/MIME headers when `GHOST_WEB_ROOT`
  is present.
- `Backend/package-lock.json`: contains the dependency resolution used by the packaged backend.
- `Assets/Presentation/Fundamentals/ChatbotFundamentalsData.cs` and
  `ChatbotFundamentalsPresenter.cs`.
- `Assets/Scenes/Act6BackendResponsePrototype.unity`, `Act6VoicePipelinePrototype.unity`, and
  `GameShellPrototype.unity`. These were synchronised because the C worktree held the latest
  submission state used to create the release.
- `Assets/Presentation/Act3DialogGraph/Act3DialogGraphNodeDragView.cs`: now contains only its matching
  node-drag component.
- `Assets/Scenes/Act3DialogGraphPrototype.unity`: five palette items now refer to the separated
  palette drag component by GUID.
- `Deployment/build-release.ps1`: waits for Unity and checks its real exit code.
- `Docs/CODE_WALKTHROUGH.md` and `Docs/UNITY_TEST_CHECKLIST.md`: document Run 002 and current results.

## Tests or Checks Run

- Canonical Unity EditMode suite using Unity 6000.4.11f1 in batch mode.
- Backend `npm ci`, `npm test`, and `npm run build` through the release script.
- Production dependency audit in the source and staged backend.
- Full canonical WebGL build with all nine release scenes and a missing-script log search.
- Staged-package self-test with a restricted `PATH` and temporary user data.
- Inno Setup build, silent install, installed launcher self-test, uninstall, and residue check.
- WebGL startup in a fresh Microsoft Edge profile.
- Checks that ports 3000 and 11435 were released after testing.

EditMode fixture breakdown: Act 1 data/rules 7; Act 2 data/session/validator/outcomes 19; Act 3
session/simulator/validator 14; Chapter 4 validator 6; Chapter 5 runner/presenter 5; Chapter 6 backend
validator 8; Final pipeline controller/presenter/validator 28; Final Chapter conversation 24;
fundamentals 1; intent session/validator 15; later hint context 3; shell navigation/completion 17.

## Test / Check Result

- Unity EditMode: 147 passed, 0 failed, 0 skipped. XML duration: 1.8764977 seconds.
- Backend: 10 tests passed; TypeScript build passed.
- Production audits: zero vulnerabilities in the source and staged production trees.
- WebGL: success; 17 files; 13,043,856 bytes; no missing-script warning in the final log.
- Staged package: exit 0 in 21.99 seconds; WebGL, REST, SQLite, Granite discovery, and a model-backed
  hint passed.
- Installer: `D:\Code\Ghost\Build\Installer\GhostSetup.exe`, 1,667,664,159 bytes.
- Installer SHA-256: `E49F5D3A15FB3515F97AFE373320C15BE5862C31A2356A895BA65AB587004C50`.
- Installed test: installer exit 0, launcher exit 0, uninstaller exit 0, and no application directory
  remained. Duration: 126.74 seconds.
- Browser: title `Unity Web Player | Ghost`; loading absent; warning empty; canvas 960 by 600.
- Ports 3000 and 11435: no listener remained.

Unity Editor Play Mode: Not run - this run exercised the browser release rather than opening the
Editor interactively. Earlier Play Mode evidence remains separate.

Separate physical clean machine: Not run - the available check used isolated install/data
directories and a sanitised PATH on the development computer. It is useful package evidence, but it
is not an external-machine result.

## Errors Encountered

- Run 001 existed only in the C-drive worktree, so Claude could not find it in the canonical repo.
- The first D-drive command checked for WebGL output before GUI Unity had finished.
- The first canonical build reproduced five missing-script warnings on Chapter 3 palette items.
- The first EditMode command used `-quit` and produced no XML.
- The in-app browser runtime failed because its Windows sandbox ACL helper could not initialise.
- The `apply_patch` helper later failed with the same ACL error, including inside the writable
  worktree; documentation was therefore written with exact guarded PowerShell replacements.
- Unity build/import temporarily changed render-pipeline and ProjectSettings files.

## Fixes Applied

- Relevant source was copied into `D:\Code\Ghost` without overwriting unrelated user changes.
- After D-drive verification, 4.04 GB of generated Build output was removed from the C-drive
  worktree. The Git worktree itself was retained.
- `build-release.ps1` now uses `Start-Process -Wait` and checks Unity's exit code.
- `Act3DialogGraphPaletteItemDragView` was moved to a matching file and five scene references were
  repaired. The final build log has no missing-script warning.
- The EditMode suite was rerun without `-quit`; its XML was produced and parsed.
- A fresh Edge headless profile was used after the in-app browser helper failed.
- Only build-generated render-pipeline and ProjectSettings changes were restored. The existing
  user-owned `ProjectSettings/EditorBuildSettings.asset` change was preserved.

## What Was Intentionally Not Changed

- Unrelated modified and untracked files in the D-drive repository were preserved.
- `ProjectSettings/EditorBuildSettings.asset` was not reset or edited.
- No Build Settings were intentionally changed.
- CURRENT_TASK, HANDOFF_LOG, ROADMAP, and completed-task archives were not advanced.
- Run 001 was retained as history; the C-drive Codex worktree was not deleted.
- CUDA and ROCm Ollama libraries remain excluded; CPU and Vulkan support are packaged.

## Remaining Risks

- A separate Windows 10/11 x64 computer or virtual machine has not run the installer.
- The installer is unsigned and may show an unknown-publisher warning.
- It is about 1.55 GiB; the staged release is about 1.81 GiB.
- Hardware without Vulkan acceleration will use CPU inference and may start more slowly.
- The browser check confirms startup and rendering, not a complete WebGL playthrough.
- The canonical repo has many user-owned uncommitted files; review scoped changes before committing.

## Next Recommended Step

Claude should review Run 002 and scoped changes in `D:\Code\Ghost`. Then install
`Build\Installer\GhostSetup.exe` on a separate Windows 10/11 x64 machine and record its hardware,
first-launch time, and a Chapter 3 playthrough before closing M0-T50 as clean-machine verified.