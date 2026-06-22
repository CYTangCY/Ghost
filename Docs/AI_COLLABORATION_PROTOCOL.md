# AI_COLLABORATION_PROTOCOL.md

> Status: canonical workflow for the Ghost project. Established 2026-06-22.
> This file is the single source of truth for *how the project is run*. `AGENTS.md` and `CLAUDE.md`
> carry short forms of these rules; if they ever disagree, this file and the repo docs win.

The project now uses a **two-agent workflow** with the **user as the final decision maker**:
Claude is the repo-aware **project commander and reviewer**, and Codex is the **implementation
agent**. ChatGPT is no longer part of the official workflow.

---

## 1. Roles

### User
- Final decision maker.
- Copies prompts between tools (Claude ↔ Codex).
- Runs Unity Editor verification (Play Mode, Test Runner).
- Checks `git status` / `git diff` before commits.
- Approves commits and pushes.

### Claude — project commander and reviewer
- Reads the actual repo files before planning (does not rely on memory).
- Maintains the planning/architecture docs: `Docs/ROADMAP.md`, `Docs/CURRENT_TASK.md`,
  `Docs/HANDOFF_LOG.md`, `Docs/ARCHITECTURE.md`, `Docs/LEARNING_CONTENT.md`,
  `Docs/DESIGN_RATIONALE.md`.
- Produces precise Codex implementation prompts scoped to the active task.
- Reviews Codex work (scope, explainability, concept-to-mechanic alignment, deterministic correctness).
- Performs task closure and writes completed-task archives in `Docs/completed_tasks/`.
- Advances `Docs/CURRENT_TASK.md` to the next task **only after implementation is verified** (by Codex
  output **and** the user's Unity Editor check).
- Does not silently change the confirmed direction; does not invent Acts, references, or scope.

### Codex — implementation agent
- Implements **only** the `Docs/CURRENT_TASK.md` scope.
- Does not change the roadmap or project direction.
- Writes a run log in `Docs/codex_runs/` (see `Docs/codex_runs/README.md`).
- Reports the actual files changed.
- Honestly reports whether Unity Play Mode and tests were run (see §5).
- Produces a Claude review/closure prompt after implementation.

### ChatGPT — not part of the official workflow
- May be used only as external, ad-hoc strategy support if the user asks.
- Repo docs, not ChatGPT memory, are the source of truth.

---

## 2. Decision hierarchy

1. **User** has final approval.
2. **Repo docs** are the source of truth.
3. **Claude** coordinates based on the actual repo state.
4. **Codex** implements only the active task.

If chat memory conflicts with the repo docs, the **repo docs win** — unless the user explicitly
updates the docs.

---

## 3. Required task cycle

1. Claude reads the repo and checks the active task (`Docs/CURRENT_TASK.md`).
2. Claude creates a Codex implementation prompt.
3. User gives the prompt to Codex.
4. Codex implements only the active task.
5. Codex creates a run log in `Docs/codex_runs/`.
6. Codex returns:
   - implementation summary
   - changed files
   - verification status (what was actually run vs. not run)
   - risks
   - **Chinese STAR summary** (see §7)
   - a **Claude review/closure prompt**
7. User verifies in the Unity Editor (Play Mode and/or Test Runner).
8. User checks `git status` and `git diff` (see §4).
9. User gives the Codex output **and** the human verification result to Claude.
10. Claude reviews, archives the task, updates `Docs/HANDOFF_LOG.md`, and advances
    `Docs/CURRENT_TASK.md`.
11. Claude returns a **Chinese STAR summary**.
12. User commits and pushes.

---

## 4. Git hygiene rules

Before every commit, run:

```bash
git status --short
git diff --name-only
```

Do **not** intentionally commit:
- ProjectSettings side effects;
- Packages changes unless approved;
- unrelated scene changes;
- `SampleScene` changes;
- generated `.unity` YAML rewrites unless required and reviewed.

Common cleanup (only when the change is an unwanted side effect):

```bash
git restore -- ProjectSettings/ShaderGraphSettings.asset
```

```powershell
Remove-Item ProjectSettings/SceneTemplateSettings.json -ErrorAction SilentlyContinue
```

Note: the working tree may carry Editor-generated changes (e.g. regenerated `.unity` scenes,
`ProjectSettings/ShaderGraphSettings.asset`). Scope each commit to the files the task intended.

---

## 5. Verification honesty

Codex must **not** claim any of the following unless it actually ran them in that session:
- Unity Play Mode passed;
- Unity tests passed;
- the scene works.

When not run, the run log must say exactly: `Not run — [reason].`

**Human Unity verification is recorded separately** from Codex in-session verification. Claude must
never rewrite a run log's honest "Not run" as if Codex ran tests, and must record the user's Editor
result as the source of any "works" status.

---

## 6. Documentation map (what goes where)

- Long-term plan: `Docs/ROADMAP.md`
- Active task only: `Docs/CURRENT_TASK.md`
- Completed task archives: `Docs/completed_tasks/`
- Codex implementation logs: `Docs/codex_runs/`
- Architecture decisions: `Docs/ARCHITECTURE.md` and `Docs/DESIGN_RATIONALE.md`
- Learning concept mapping: `Docs/LEARNING_CONTENT.md`
- Current engineering notes: `Docs/CODE_WALKTHROUGH.md` and `Docs/UNITY_TEST_CHECKLIST.md`
- Confirmed project ground truth: `Docs/CONFIRMED_PROJECT_CONTEXT.md`
- This workflow: `Docs/AI_COLLABORATION_PROTOCOL.md`

---

## 7. Chinese STAR summary requirement

Both Codex and Claude final reports **must** include a Chinese STAR summary:

- **S（情境）**: what state the project/task was in.
- **T（任務）**: what the agent was asked to do.
- **A（行動）**: what files / actions / verification steps were performed.
- **R（結果）**: what changed, what passed, what remains risky, and what should happen next.

---

## 8. Full-system requirement

The final project must include planned **LLM, backend, and database** systems. However:

- Puzzle correctness must remain **deterministic and validator-driven**.
- The LLM may generate Lily hints, Ghost responses, explanations, natural-language feedback, or the
  capstone chatbot simulation.
- The LLM must **not** be the source of truth for scoring.
- Backend / database / LLM work must be reflected in `Docs/ROADMAP.md` and `Docs/ARCHITECTURE.md`
  **before** implementation (see ROADMAP Phase D and the ARCHITECTURE backend/database/LLM layers).
