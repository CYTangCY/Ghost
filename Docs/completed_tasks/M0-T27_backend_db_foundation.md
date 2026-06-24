# M0-T27 — Backend + Database Foundation

## Completion Status

Completed. Codex implemented and ran npm build/test; Claude independently ran `npm run build` and
`npm test` (4/4 pass) and reviewed the routes + schema. First full-system (Phase D) component.

## Date

2026-06-24

## Summary

New top-level `Backend/` (Node.js + TypeScript + Express + better-sqlite3): a local REST service +
SQLite. Endpoints: `GET /health`, `GET /content`, `POST /profiles`, `GET/PUT /progress/:profileId`,
`POST /attempts`; `/hints` and `/responses` return 501 (deferred to M0-T29). SQLite schema
(`learning_content`, `puzzles`, `profiles`, `progress`, `attempts`, `hint_logs`) seeded with Act 1–3
reference content. The backend stores/serves only — it does NOT score puzzles.

## Files Created

- `Backend/.gitignore`, `package.json`, `package-lock.json`, `tsconfig.json`, `README.md`
- `Backend/src/app.ts`, `src/database.ts`, `src/seedData.ts`, `src/server.ts`, `tests/app.test.ts`
- `Docs/codex_runs/M0-T27_001_backend_db_foundation.md`

## Files Modified

- `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`

## Claude Review Notes

- **Deterministic rule upheld:** no scoring endpoint; `getContent()` selects only learning-content
  metadata + puzzle content, NOT `answer_key_json` (answer keys are seeded as reference but never
  exposed via the API); `/hints` + `/responses` = 501.
- **Independently verified by Claude:** `npm run build` (tsc) succeeded; `npm test` → 4/4 vitest tests
  pass (run locally from `Backend/`).
- Scope clean: only `Backend/` + the 2 docs; no Unity `Assets/`/`ProjectSettings/`/`Packages/`/puzzle
  changes.
- Run log honest and detailed (Codex actually ran npm install/build/test; dev-tooling audit advisories
  noted — prototype-acceptable; production audit 0 vulnerabilities).

## Human Verification Result

Backend command verification is reproducible (`npm install` → `npm run build` → `npm test` pass);
manual endpoint smoke is documented in `Backend/README.md`. No Unity verification needed (backend-only).

## Remaining Risks

- Dev-tooling (tsx/vite/vitest) audit advisories (dev-only; production audit clean).
- Local-only: no auth, rate limiting, migrations framework, or deployment hardening.
- Seeded answer keys live in the DB (not exposed); future endpoints must continue to not score.
- WebGL client → localhost backend will need CORS/config handling at integration (M0-T28).

## Next Task

M0-T28 — Unity client ↔ backend integration: pseudonymous profile + progress persistence + attempt
logging across Acts 1–3, behind graceful degradation (if the backend is down, the game keeps working
on local/in-memory behaviour). Deterministic validators unchanged.
