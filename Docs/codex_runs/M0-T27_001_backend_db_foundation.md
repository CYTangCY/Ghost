# M0-T27 — Run 001 — backend db foundation

## Task ID

M0-T27

## Run Number

001

## Date

2026-06-24

## Original Request / Codex Prompt Summary

Implement only the full-system backend/database foundation as a new top-level `Backend/` Node.js + TypeScript REST service with SQLite. Add Express + better-sqlite3, create and seed the database schema, expose content/profile/progress/attempt endpoints, keep `/hints` and `/responses` as M0-T29 stubs, add backend tests, and document that the backend never scores puzzle correctness.

## Files Created

- `Backend/.gitignore`
- `Backend/package-lock.json`
- `Backend/package.json`
- `Backend/README.md`
- `Backend/tsconfig.json`
- `Backend/src/app.ts`
- `Backend/src/database.ts`
- `Backend/src/seedData.ts`
- `Backend/src/server.ts`
- `Backend/tests/app.test.ts`
- `Docs/codex_runs/M0-T27_001_backend_db_foundation.md`

Generated local artifacts:
- `Backend/node_modules/` from `npm install` (ignored by `Backend/.gitignore`)
- `Backend/dist/` from `npm run build` (ignored by `Backend/.gitignore`)

## Files Modified

- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `npm.cmd install` in `Backend/`
- `npm.cmd audit --omit=dev` in `Backend/`
- `npm.cmd run build` in `Backend/`
- `npm.cmd test` in `Backend/`
- Unity Play Mode: Not run — Unity Editor Play Mode was not executed in this Codex session.
- Unity tests: Not run — Unity Editor test runner was not executed in this Codex session.

## Test / Check Result

- `npm.cmd install`: Passed after rerun with local registry/cache access and after exact-pinning Express at `4.22.2`. Final install still reports dev-dependency audit warnings from tooling.
- `npm.cmd audit --omit=dev`: Passed after the Express update and non-force `npm audit fix`; production dependencies reported 0 vulnerabilities.
- `npm.cmd run build`: Passed after rerun with local file-write access. TypeScript compiled successfully.
- `npm.cmd test`: Passed after rerun with local file-write access. Vitest reported 1 test file passed, 4 tests passed.

## Errors Encountered

- `npm.cmd install` first failed inside the sandbox with `EACCES` fetching `@types/express` from the npm registry and writing npm cache logs.
- The first sandbox `npm.cmd run build` failed with `TS5033` / `EPERM` because TypeScript could not create the sandbox `dist` directory.
- The first sandbox `npm.cmd test` executed the 4 tests successfully but exited with an unhandled `EPERM` because Vitest could not create `Backend/node_modules/.vite`.
- An earlier build attempt exposed Express type mismatch from `@types/express` 5.x against Express 4.x.
- `npm.cmd install` reported dependency audit warnings after installing the initial pinned stack.
- `npm.cmd audit --omit=dev` initially reported Express runtime dependency advisories through `path-to-regexp` and `qs`.
- Full `npm.cmd audit` still reports dev-tooling vulnerabilities in the current `tsx`/`vite`/`vitest` graph that require breaking changes to address.

## Fixes Applied

- Pinned `@types/express` to `4.17.21` to match Express 4.
- Updated and exact-pinned Express to `4.22.2`, then ran non-force `npm audit fix` to resolve production dependency advisories.
- Stored `request.params.profileId` in local string variables before database calls.
- Adjusted `tsconfig.json` so `rootDir` is `src` and compiled output matches `npm start` at `dist/server.js`.
- Reran `npm.cmd install`, `npm.cmd run build`, and `npm.cmd test` with local write/network/cache access after sandbox permission failures.

## What Was Intentionally Not Changed

- No Unity `Assets/`, `ProjectSettings/`, `Packages/`, scenes, puzzle logic, validators, sessions, or presentation scripts were edited.
- No LLM calls were added.
- No Unity client wiring was added.
- No authentication or deployment/hosting configuration was added.
- No endpoint that scores puzzle submissions was added.

## Remaining Risks

- Full npm audit still reports dev-tooling vulnerabilities in the current `tsx`/`vite`/`vitest` graph; production-only audit reports 0 vulnerabilities.
- Seeded answer-key JSON is present in the local database as reference/analytics data; API responses intentionally omit it, and future endpoints should continue to avoid backend scoring.
- The service is local-only and has no authentication, rate limiting, migrations framework, or deployment hardening.

## Next Recommended Step

Have Claude review the backend files and docs, then run `npm install`, `npm run build`, and `npm test` from `Backend/` on the developer machine. If accepted, archive the completed task as `M0-T27_backend_db_foundation.md`.
