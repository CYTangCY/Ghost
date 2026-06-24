# Ghost Backend

Local Node.js + TypeScript REST service for the Ghost vertical-slice backend/database foundation.

## What This Is

- Express REST API.
- SQLite database via `better-sqlite3`.
- Local pseudonymous profiles.
- Progress and attempt-log storage.
- Seeded Act 1-3 reference content mirrored from the Unity sample data.

The backend does **not** score puzzle submissions. Unity's deterministic validators and graph simulator remain authoritative. Stored answer keys are reference/analytics data only.

## Setup

```powershell
cd Backend
npm install
```

## Run Locally

```powershell
npm run dev
```

By default the service listens on `http://localhost:3000` and stores SQLite data in:

```text
Backend/data/ghost.sqlite
```

Override with environment variables:

```powershell
$env:PORT = "3100"
$env:GHOST_DB_PATH = "data/dev.sqlite"
npm run dev
```

## Build

```powershell
npm run build
```

## Test

```powershell
npm test
```

## Endpoints

- `GET /health` -> `{ ok: true }`
- `GET /content` -> seeded acts, levels, and puzzle content
- `POST /profiles` -> create a pseudonymous local profile
- `GET /progress/:profileId` -> read progress
- `PUT /progress/:profileId` -> upsert progress
- `POST /attempts` -> insert an attempt log
- `POST /hints` -> `501 not implemented (M0-T29)`
- `POST /responses` -> `501 not implemented (M0-T29)`

There is intentionally no scoring endpoint in M0-T27.
