# M0-T33 — Run 001 — lily chat

## Task ID

M0-T33

## Run Number

001

## Date

2026-06-25

## Original Request / Codex Prompt Summary

Implement a constrained Lily free-text chat window on top of the existing M0-T29 LLM backend, with `/chat` backend orchestration, static fallback, chat logging, Unity WebGL-safe client calls, a runtime-created dedicated chat UI, and ambient banter UI fixes. Preserve deterministic puzzle correctness and avoid scene/ProjectSettings/package/meta edits.

## Files Created

- `Assets/Presentation/Banter/LilyChatWindow.cs`
- `Docs/codex_runs/M0-T33_001_lily_chat.md`

## Files Modified

- `Backend/src/llmOrchestration.ts`
- `Backend/src/app.ts`
- `Backend/tests/app.test.ts`
- `Backend/README.md`
- `Assets/Presentation/Backend/GhostBackendClient.cs`
- `Assets/Presentation/Banter/AmbientBanterPanel.cs`
- `Assets/Presentation/Banter/AmbientBanterHook.cs`
- `Assets/Presentation/Banter/BanterData.cs`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `npm.cmd run build` in `Backend/`
- `npm.cmd test` in `Backend/`
- Static search for `PostChat(...)`, `/chat`, and `chat_message` call/logging paths
- Static scan for disallowed Unity client APIs in the touched backend/banter presentation folders: `System.Net.Http`, `HttpClient`, `Thread`, `Task.Run`
- Scoped diff check for pure puzzle logic, ProjectSettings, Packages, scenes, and `.meta`
- Live Ollama `/chat` call: Not run — live Ollama chat verification must be run by the user with the backend and Unity Editor.
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `npm.cmd run build` passed.
- `npm.cmd test` passed: 1 test file passed, 7 tests passed.
- Static `/chat` path search found backend route/test/docs and Unity `GhostBackendClient.PostChat(...)` call from `LilyChatWindow`.
- Static WebGL API scan found no `System.Net.Http`, `HttpClient`, `Thread`, or `Task.Run` in the touched Unity presentation folders.
- Scoped diff check showed an existing dirty `Assets/Scenes/GameShellPrototype.unity`; it was not edited by this run.
- Working tree also had pre-existing dirty `Docs/CURRENT_TASK.md` and unrelated output/unorganized-data artifacts; they were not edited by this run.

## Errors Encountered

- An initial static search command for `/chat` had a quoting/regex parse error; it was rerun with simpler `rg` queries.
- Unity compilation and Play Mode could not be verified in this Codex session.

## Fixes Applied

- Added `POST /chat` to the backend with act id, typed message, short history, player name, optional profile id, and level.
- Added Lily chat orchestration with persona/guardrails: human lab senior, timid/nerdy/stammering, one short sentence, on-topic only, private-life deflection, off-topic redirect, no solutions/answer keys/scoring/progression.
- Tightened existing hint/response prompts to one short sentence and added Lily stammering to hint voice.
- Logged chat turns into `hint_logs` with `kind:"chat"`, `trigger:"chat_message"`, source, level, player message summary, history count, and error if fallback occurred.
- Added `/chat` fallback test covering HTTP 200 static reply and `hint_logs` chat payload.
- Added `GhostBackendClient.PostChat(...)` using UnityWebRequest coroutine, long LLM timeout, profile reuse/creation, and callback-only failure.
- Added runtime `LilyChatWindow` with scrollable message list, input field, Send, Close, short in-memory history, backend chat call, and local static fallback.
- Changed `AmbientBanterPanel` so Ask Lily and incorrect Validate open the dedicated chat window instead of writing hints into the ambient banter box.
- Added pause/resume methods so ambient banter stops while the chat window is open and resumes on close.
- Adjusted the Act 2 ambient banter style to be slimmer/readable in the validation area and less likely to overlap validation feedback.
- Updated backend README, code walkthrough, and Unity checklist for `/chat`, Lily chat UI, fallback behavior, and manual verification.

## What Was Intentionally Not Changed

- No deterministic validators, sessions, puzzle rules, or files under `Assets/Scripts/Puzzles/` were changed.
- No ProjectSettings, Packages, Build Settings, scenes, `.meta` files, or asmdefs were edited in this run.
- No persistent cross-session chat history was added.
- No LLM scoring, progression gating, answer-key prompting, voice, multiple-model orchestration, or Act 8 capstone work was added.
- Existing one-shot `/hints` and `/responses` endpoints remain available, but the in-act Ask Lily UI now opens chat.

## Remaining Risks

- Unity compile and Play Mode behavior require human Editor verification.
- Live Ollama `/chat` persona/guardrail behavior was not verified in this session; the user should test on-topic, off-topic, private-life, and solution-seeking prompts.
- The runtime-created chat window may need visual tuning after checking actual Act 1/2/3 layouts in the Unity Editor.
- The Act 2 banter size fix is code-side only and should be confirmed visually.
- Existing unrelated dirty scene/doc/output files remain in the worktree.

## Next Recommended Step

Run the backend with Ollama available, enter each act in Unity Play Mode, click Ask Lily, test on-topic/off-topic/private-life/solution-seeking messages, inspect `hint_logs` for `kind:"chat"` rows, then repeat with Ollama down and backend down to confirm static fallback and uninterrupted puzzle play.
