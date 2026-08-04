# M0-T50 Run 001: Local WebGL installer and isolated deployment test

- Task ID: M0-T50
- Run number: 001
- Date: 2026-08-02
- Status: Implemented and verified with an isolated install on the development computer

## Original request / prompt summary

Create the one-file Windows installer discussed with the user, build the Unity game for WebGL, and
test that a new installation can start without relying on separately installed Node.js or Ollama.
Keep the packaged local Granite model and make the player open only the game.

`Docs/CURRENT_TASK.md` still names M0-T47, but the user explicitly started the deployment work
already listed as M0-T50 in `Docs/ROADMAP.md`. This run did not advance or rewrite CURRENT_TASK;
that closure remains with Claude under the repository workflow.

## Files created

- `Assets/Editor/GhostWebGLReleaseBuilder.cs`
- `Assets/Editor/GhostWebGLReleaseBuilder.cs.meta`
- `Assets/Editor.meta`
- `Deployment/Launcher/GhostLauncher.csproj`
- `Deployment/Launcher/Program.cs`
- `Deployment/Installer/Ghost.iss`
- `Deployment/build-release.ps1`
- `Deployment/build-installer.ps1`
- `Deployment/test-clean-environment.ps1`
- `Deployment/test-installer.ps1`
- `Deployment/README.md`
- `Docs/codex_runs/M0-T50_001_local_webgl_installer.md`

Generated, ignored evidence under `Build/` includes the staged release, WebGL build, installer,
Unity log, browser screenshots, clean-environment results, installer results, and launcher logs.

## Files modified

- `.gitignore`: keeps the launcher project file in source control while excluding its generated
  `bin` and `obj` output.
- `Backend/src/server.ts`: optionally serves the WebGL directory and sends the Brotli headers
  required by Unity's compressed WebGL output.
- `Backend/package-lock.json`: production dependency repair updated body-parser; the packaged
  production dependency audit now reports zero vulnerabilities.
- `Docs/CODE_WALKTHROUGH.md`: documents the release builder, launcher, service layout, and tests.

## Implementation summary

The release builder passes an explicit list of nine game scenes to Unity's WebGL build pipeline and
leaves the sample scene out without changing Build Settings. The Node service hosts the generated
browser files on the same localhost origin as the REST API.

The self-contained launcher starts packaged Node.js and Ollama executables by absolute path. It
uses a separate Ollama port (11435), disables Ollama cloud access, points Ollama at the packaged
Granite model, and stores SQLite data and logs in local application data. Normal play retains the
static hint fallback. The self-test requires a real model-backed hint.

The release script stages the WebGL client, backend, production node_modules, portable Node.js,
the Ollama CPU and Vulkan runtime, Granite 3.1 Dense 2B, and licence notices. CUDA and ROCm
libraries are not packaged. Inno Setup creates a per-user Windows installer with Start menu and
optional desktop shortcuts.

## Tests and checks run

- Backend: `npm ci`, `npm test`, and `npm run build`.
  - Result: 10 Vitest tests passed and TypeScript compilation passed.
- Production dependencies: `npm audit --omit=dev` in the source backend and staged backend.
  - Result: zero production vulnerabilities.
  - The development-only tree still reports seven findings; development packages are not shipped.
- Launcher: `dotnet build Deployment/Launcher/GhostLauncher.csproj`.
  - Result: passed with zero warnings and zero errors.
- PowerShell parser check for all four deployment scripts.
  - Result: passed.
- Unity 6000.4.11f1 batch WebGL build.
  - Result: `Build Finished, Result: Success`; 17 files, 13,134,275 bytes.
- Live HTTP header check for Unity's `.data.br`, `.framework.js.br`, and `.wasm.br` files.
  - Result: each returned HTTP 200 with Brotli content encoding and the expected MIME type.
- Browser smoke test in a fresh Microsoft Edge profile.
  - Result: Unity loading overlay cleared, no browser warning was shown, and the 960 by 600 canvas
    was present. Evidence: `Build/webgl-browser-ready.png`.
- Staged-package isolated test with `PATH` restricted to Windows system directories and new
  temporary user data.
  - Result: exit 0. WebGL, REST health, SQLite startup, packaged Granite discovery, and a
    model-backed hint passed. Temporary data was removed.
- Inno Setup build.
  - Result: `GhostSetup.exe`, 1,667,776,583 bytes.
  - SHA-256: `B313270B30C890126512D28BBB30C5BA4AAE1E3BA2B805594B02C54EA0989274`.
- Installer isolated-environment test on drive D.
  - Result: installer exit 0; installed launcher self-test exit 0; uninstaller exit 0; application
    directory absent after uninstall. The sanitized PATH was
    `C:\WINDOWS\System32;C:\WINDOWS`. Total duration was 139.06 seconds.

Unity Editor Play Mode: Not run — this deployment run used the batch WebGL build, a real browser
smoke test, and installed-package service tests. Existing gameplay Play Mode results belong to
earlier runs.

Separate physical clean machine: Not run — Windows Sandbox is unavailable on this Windows edition,
and no second Windows computer or virtual machine was available in-session. The isolated tests
remove system Node.js and Ollama from PATH and use new install/data directories, but they are not
described as a separate-machine result.

## Errors encountered and fixes applied

- The workspace `apply_patch` helper failed with a Windows sandbox ACL error. Exact, asserted
  PowerShell file writes were used instead; this limitation did not change task scope.
- The first launcher self-test expected hint source `ollama`, while the backend contract returns
  `llm`. The self-test was corrected to the existing API contract and rerun successfully.
- The first installer harness looked for `Ghost.exe`; the installer correctly uses
  `GhostLauncher.exe`. The repeatable test uses the real filename.
- The first saved evidence path omitted the `logs` directory. Both test scripts now copy
  `<user-data>\logs\launcher.log`.
- A trailing slash in `-TestBase D:\` caused the safety prefix to contain two separators. Cleanup
  deliberately stopped. The prefix normalization was corrected, validated against the exact
  generated path, and the verified temporary directory was removed.
- The in-app browser tool could not create its kernel assets. A fresh Edge headless profile and
  Chrome DevTools Protocol smoke script were used instead.
- Unity enabled its cloud-connect setting during the first import. That unintended one-line
  ProjectSettings change was restored; other user-owned ProjectSettings edits were left untouched.

## What was intentionally not changed

- No gameplay rules, chapter content, scenes, or player-facing story were edited.
- No Build Settings or other ProjectSettings were intentionally edited.
- Existing unrelated modified and untracked files were preserved.
- Generated launcher `bin` and `obj` directories were removed from the source handoff; they are
  recreated by the release build.
- CUDA and ROCm Ollama libraries were excluded to keep the single installer smaller. CPU and Vulkan
  remain available.
- CURRENT_TASK, HANDOFF_LOG, and completed-task archives were not advanced because Claude owns task
  review and closure in the documented workflow.

## Remaining risks and limitations

- Unity reported five missing-script components on place-node objects in
  `Assets/Scenes/Act3DialogGraphPrototype.unity`. The WebGL build succeeded and the browser runtime
  started, but Chapter 3 should be rebuilt or inspected before final submission.
- The clean-environment evidence comes from isolated directories on the development computer, not a
  separate physical or virtual clean Windows installation.
- The installer targets Windows x64. It does not provide macOS or Linux packages.
- The release is about 1.81 GiB before installer compression; the installer is about 1.55 GiB.
- Systems without usable Vulkan acceleration can fall back to CPU and may load Granite more slowly.
- The installer is not code-signed, so Windows may show an unknown-publisher warning.
- The browser smoke test proves startup and rendering, not a complete WebGL playthrough of every
  chapter.

## Next recommended step

Run the final installer on a separate Windows 10/11 x64 machine with no Node.js, Ollama, or Granite
installation, record its hardware and first-launch time, and play through Chapter 3. Resolve the five
missing-script warnings before treating the WebGL build as submission-ready. After that evidence is
available, update the dissertation methodology and evaluation with the installer workflow and the
measured result.
