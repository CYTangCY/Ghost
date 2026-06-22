# AGENTS.md

This repository is for the Ghost project.

Before implementation, always read:
- Docs/CONFIRMED_PROJECT_CONTEXT.md
- Docs/ROADMAP.md
- Docs/CURRENT_TASK.md
- Docs/REQUIREMENTS.md
- Docs/LEARNING_CONTENT.md
- Docs/ARCHITECTURE.md

## Project Identity

Ghost is a cute ghost-themed narrative puzzle game for teaching selected IBM SkillsBuild chatbot and NLP concepts.

Ghost is a cute ghost, not literally an AI assistant, chatbot, or robot.

while True: learn() is a separate reference game, not this project's title.

Lily is the protagonist's nerdy, slightly timid, pretty/cute postdoctoral senior from the lab.

## Implementation Rules

1. Implement only the current task.
2. Do not invent new Act structures.
3. Do not invent academic references.
4. Do not invent story content beyond the current task.
5. Do not write Ghost as a literal AI assistant.
6. Do not write Lily as a generic tutor.
7. Do not reduce gameplay to multiple-choice unless the task explicitly says so.
8. Keep Unity C# simple and readable.
9. Keep code compatible with WebGL.
10. Do not edit ProjectSettings unless explicitly approved.
11. Do not delete or rename Unity .meta files.
12. Separate UI display from puzzle logic.
13. Update CODE_WALKTHROUGH.md for every implemented C# script.
14. Provide Unity Inspector setup steps after editing.
15. Provide Play Mode test steps after editing.

## Required Output After Each Coding Task

After editing, report:
- changed files
- purpose of each changed file
- how to set up the feature in Unity Inspector
- how to test it in Play Mode
- assumptions made
- risks or limitations
- a run log saved in `Docs/codex_runs/` (see Codex Run Log Convention below)

## Task Archiving Convention

- `Docs/CURRENT_TASK.md` always contains only the active task.
- When a task is completed, copy or summarize its final state into `Docs/completed_tasks/`.
- Archive filename format: `TASKID_short_description.md` (e.g. `M0-T01_initialize_planning_docs.md`).
- Each archived task file includes: completion status, date, summary, changed files, test / verification result, and next task.
- `Docs/HANDOFF_LOG.md` keeps a short chronological summary; `Docs/completed_tasks/` stores the fuller task records.
- Never delete or rename previous task files in `Docs/completed_tasks/`.

## Codex Run Log Convention

After every implementation or debugging run, Codex must create one run log in `Docs/codex_runs/`.

- Filename: `TASKID_RUNNUMBER_short_description.md` (e.g. `M0-T04_001_intent_validator_initial_implementation.md`); run numbers are zero-padded and increase per task.
- The log must include: Task ID, run number, date, original request / prompt summary, files created, files modified, tests or checks run, test/check result, errors encountered, fixes applied, what was intentionally not changed, remaining risks, and next recommended step.
- Do not claim tests passed unless they were actually run. If tests were not run, write exactly: `Not run — [reason].`
- Never delete or overwrite older run logs; each run gets a new file with the next run number.
- Do not put hidden reasoning or private chain-of-thought in the log — record actions, decisions, results, and evidence only.
- `Docs/HANDOFF_LOG.md` keeps only a short chronological summary; `Docs/codex_runs/` stores the detailed per-run records.
- See `Docs/codex_runs/README.md` for the full template.

## AI Collaboration Workflow

This project uses a two-agent workflow with the user as the final decision maker. The canonical,
detailed version is `Docs/AI_COLLABORATION_PROTOCOL.md`; this section is the short form.

- **User:** final decision maker. Copies prompts between tools, runs Unity Editor verification,
  checks `git status` / `git diff` before commits, and approves commits/pushes.
- **Claude:** the repo-aware **project commander and reviewer**. Reads the actual repo before
  planning; maintains ROADMAP/CURRENT_TASK/HANDOFF_LOG/ARCHITECTURE/LEARNING_CONTENT/DESIGN_RATIONALE;
  writes precise Codex implementation prompts; reviews Codex work; performs task closure and writes
  completed-task archives; advances `Docs/CURRENT_TASK.md` only after implementation is verified.
  Claude does not silently change the confirmed direction.
- **Codex:** the **implementation agent**. Implements ONLY the active `Docs/CURRENT_TASK.md` scope,
  writes a run log in `Docs/codex_runs/`, and returns a Claude review/closure prompt after
  implementation.
- **ChatGPT:** **no longer part of the official workflow.** It may be used only as external, ad-hoc
  strategy support if the user asks. Repo docs — not chat memory — are the source of truth.

Both agents (Claude and Codex) must include a Chinese STAR summary (S情境 / T任務 / A行動 / R結果)
in their final reports.

Rules for Codex in this workflow:
- Implement only what CURRENT_TASK.md defines; do not silently expand scope.
- Do not intentionally edit ProjectSettings, Packages, Build Settings, or unrelated scenes.
- Run logs must state honestly whether Unity Play Mode or tests were actually run
  (use `Not run — [reason].` when they were not).
- Human Editor verification is recorded separately from Codex in-session checks — Codex must not
  claim Editor/Play Mode results it did not produce.
- Follow the current Act structure in `Docs/ROADMAP.md` and `Docs/LEARNING_CONTENT.md`; do not
  invent new Acts.
- Full-system components (LLM, backend, database) are required for the final system, but their plans
  must already be reflected in `Docs/ROADMAP.md` and `Docs/ARCHITECTURE.md` before implementation
  begins. Deterministic validators / graph simulation / test cases / backend scoring remain the
  source of truth for puzzle correctness — the LLM must never decide scoring.
- After implementing, return a Claude review/closure prompt and include a Chinese STAR summary
  (S情境 / T任務 / A行動 / R結果) in the final report. Claude (not Codex) reviews, archives, updates
  HANDOFF_LOG.md, and advances CURRENT_TASK.md.