# Ghost Backend

Local Node.js + TypeScript REST service for the Ghost vertical-slice backend/database foundation.

## What This Is

- Express REST API.
- SQLite database via `better-sqlite3`.
- Local pseudonymous profiles.
- Progress and attempt-log storage.
- Seeded Act 1-3 reference content mirrored from the Unity sample data.
- LLM-backed Lily hints and Ghost response text through local Ollama + Granite, with static fallback.

The backend does **not** score puzzle submissions. Unity's deterministic validators and graph simulator remain authoritative. Stored answer keys are reference/analytics data only.
The LLM also does **not** score puzzle submissions. It only generates natural-language hints/responses.

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

## LLM Setup: Ollama + IBM Granite

M0-T29 uses this backend as the local proxy between Unity and Ollama. The game still works if Ollama is
not installed or not running: `/hints` and `/responses` return static fallback text instead of failing
the gameplay path.

1. Install Ollama from:

```text
https://ollama.com/download
```

2. Start Ollama. On most systems it starts automatically after installation. If needed, run:

```powershell
ollama serve
```

3. Pull the default IBM Granite model used by this backend:

```powershell
ollama pull granite3.1-dense:2b
```

4. Check whether the backend can see Ollama, the model, and complete one timed test generation:

```powershell
npm run check:ollama
```

Expected success output includes the model check and generation latency:

```text
OK: model 'granite3.1-dense:2b' is available.
Running a timed test generation...
OK: test generation succeeded in 12345 ms.
```

The first generation after starting Ollama can be slow because the Granite model is being loaded into
memory. The backend default generation timeout is 60 seconds; increase `OLLAMA_TIMEOUT_MS` if your
machine needs longer.

5. Run the backend:

```powershell
npm run dev
```

### LLM Configuration

Environment variables:

```powershell
$env:OLLAMA_URL = "http://localhost:11434"
$env:OLLAMA_MODEL = "granite3.1-dense:2b"
$env:OLLAMA_TIMEOUT_MS = "60000"
$env:OLLAMA_CHECK_TIMEOUT_MS = "5000"
npm run dev
```

Defaults:

- `OLLAMA_URL`: `http://localhost:11434`
- `OLLAMA_MODEL`: `granite3.1-dense:2b`
- `OLLAMA_TIMEOUT_MS`: `60000` for `/api/generate`
- `OLLAMA_CHECK_TIMEOUT_MS`: `5000` for quick `/api/tags` availability checks

### Try `/hints`

PowerShell:

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:3000/hints" `
  -ContentType "application/json" `
  -Body '{"actId":"act1","level":"1","trigger":"ask_lily_button","state":{"summary":"player asked for help"}}'
```

curl:

```bash
curl -X POST http://localhost:3000/hints \
  -H "Content-Type: application/json" \
  -d '{"actId":"act1","level":"1","trigger":"ask_lily_button","state":{"summary":"player asked for help"}}'
```

Response shape:

```json
{ "hint": "Um...", "source": "llm" }
```

If Ollama is unavailable, the backend still returns HTTP 200:

```json
{ "hint": "Um... maybe don't look at the exact words first...", "source": "static" }
```

Hints are logged to the `hint_logs` table for analytics, including the trigger (for example
`ask_lily_button` or `after_incorrect_validate`) and the supplied non-spoiler state summary. The prompt
uses only act learning metadata and player-facing state summaries; it intentionally does not include
puzzle answer keys.

## Endpoints

- `GET /health` -> `{ ok: true }`
- `GET /content` -> seeded acts, levels, and puzzle content
- `POST /profiles` -> create a pseudonymous local profile
- `GET /progress/:profileId` -> read progress
- `PUT /progress/:profileId` -> upsert progress
- `POST /attempts` -> insert an attempt log
- `POST /hints` -> Lily hint text, `{ hint, source: "llm" | "static" }`
- `POST /responses` -> Ghost response text, `{ text, source: "llm" | "static" }`

There is intentionally no scoring endpoint. Deterministic correctness remains in the Unity client.
