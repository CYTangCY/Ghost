# CURRENT_TASK.md

## ID

M0-T27

## Goal

Scaffold the full-system backend + database foundation (ROADMAP Phase D / `VERTICAL_SLICE_PLAN.md` §B):
a local Node.js + TypeScript REST service backed by SQLite, exposing content / progress / attempts
endpoints. No Unity client wiring yet (M0-T28) and no LLM yet (M0-T29). Deterministic puzzle correctness
stays client-side — the backend stores and serves data; it does not decide scoring.

## Context

Vertical-slice milestone. Acts 1–3 + the shell narrative + banter are done; the next workstream is the
server-side foundation. Contracts are already drafted in `VERTICAL_SLICE_PLAN.md` §B and
`ARCHITECTURE.md` (Phase D). This is the project's first non-Unity component: a new top-level backend
project (outside `Assets/`). Stack per `CONFIRMED_PROJECT_CONTEXT.md` §8: Node.js + TypeScript, REST,
SQLite, pseudonymous local profile, attempt-log analytics. Default framework Express + better-sqlite3,
running locally (confirm if a different stack is preferred).

## Scope

- A new top-level backend project (e.g. `Backend/`), separate from the Unity project (NOT under
  `Assets/`): `package.json`, `tsconfig.json`, a small REST server, SQLite, and a schema + seed.
- DB schema: `learning_content`, `puzzles` (per-act content + answer keys as reference/analytics — NOT
  used to override client validation), `profiles` (pseudonymous), `progress`, `attempts`, `hint_logs`
  (schema only; populated in M0-T29).
- Endpoints: `GET /content` (acts/levels metadata + puzzle content), `GET/PUT /progress/:profileId`,
  `POST /attempts`. (Hint/response endpoints may be stubbed; real LLM is M0-T29.)
- Seed the DB with Act 1–3 reference content mirrored from the existing C# sample data (reference only;
  client validators remain authoritative).
- Minimal backend tests (endpoint smoke tests) runnable via `npm test`; a `Backend/README.md` for
  running locally.
- Deterministic-correctness rule: the backend serves content + logs; it does NOT decide puzzle
  correctness.

## Out of Scope

- Unity client integration (M0-T28), LLM endpoints (M0-T29), authentication/accounts, deployment/hosting
  beyond local, and any graph-simulation/scoring service.
- Do NOT modify Unity `Assets/`, `ProjectSettings/`, `Packages/`, puzzle logic, or scenes.

## Acceptance Criteria

- A runnable local Node/TS backend (`npm install` then a documented run command) with SQLite; documented
  in `Backend/README.md`.
- DB schema for content/puzzles/profiles/progress/attempts/hint_logs, seeded with Act 1–3 reference
  content.
- `GET /content`, `GET/PUT /progress/:profileId`, `POST /attempts` work and are smoke-tested.
- The backend does not decide puzzle correctness; no Unity `Assets/`/`ProjectSettings` changes.
- `npm test` (or equivalent) results are honestly reported in the run log (Codex CAN run npm in its
  shell — report real results).
- A backend doc/CODE_WALKTHROUGH note + a Codex run log are created.
