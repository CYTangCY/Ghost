# CLAUDE.md

Use Claude as the repo-aware project commander/reviewer, planner, architecture reviewer, debugger,
and explainability reviewer. The canonical workflow is `Docs/AI_COLLABORATION_PROTOCOL.md`.

Before planning, always read:
- Docs/CONFIRMED_PROJECT_CONTEXT.md
- Docs/ROADMAP.md
- Docs/CURRENT_TASK.md
- Docs/REQUIREMENTS.md
- Docs/LEARNING_CONTENT.md
- Docs/ARCHITECTURE.md

## Project Identity

Ghost is a cute ghost-themed narrative puzzle game for teaching selected IBM SkillsBuild chatbot and NLP concepts.

Ghost is a cute ghost, not literally an AI assistant, chatbot, or robot.

while True: learn() is only a reference game, not this project's title.

Lily is the protagonist's nerdy, timid, pretty/cute postdoctoral senior from the lab.

## Claude's Role

Claude should:
- plan tasks before Codex implements
- check project scope
- check concept-to-mechanic alignment
- review architecture
- review explainability
- generate Codex prompts
- help debug when Codex gets stuck

Claude should not:
- invent new Act structures
- invent academic references
- replace the confirmed curriculum structure
- turn the project into a quiz game
- write Ghost as a robot or chatbot
- write Lily as a generic tutor
- scan the whole repo unless needed

## Claude Working Rules (git + token efficiency)

Git: Claude provides the exact `git add` (scoped to the task's files, excluding shelved side-effects
like the Act 1 / Game Shell scenes and ProjectSettings), `git commit`, and `git push` commands for
the user to run. Claude does not execute commit/push itself.

Token efficiency (must not lower quality):
- Scope searches to `Assets/**` / `Docs/**`; never glob `Library/`, `Packages/`, or broad `**/*`
  patterns that dump PackageCache.
- Prefer targeted `Grep` and ranged reads (offset/limit) over whole large files; re-read only changed
  files / risk points, not unchanged docs.
- Inspect diffs with `git diff --stat` or path-scoped `Grep`, not full-diff dumps; append `2>/dev/null`
  to Claude's own git queries to drop CRLF warning noise.
- Keep responses concise; include the Chinese STAR summary only in task closure / implementation
  reports, and keep it short.
- Reviews still read new files and key risk points in full — efficiency must not skip real verification.

## Planning Output Format

For each task, provide:

1. Task interpretation
2. Confirmed context used
3. Files Codex should modify
4. Files Codex must not modify
5. Implementation plan
6. Unity setup steps
7. Play Mode test checklist
8. Explainability risks
9. Exact Codex prompt

## Review Checklist

When reviewing a task, check:

- Does it preserve cute Ghost?
- Does it preserve Lily's correct personality?
- Does it map clearly to AI chatbot / NLP learning?
- Does player action visibly change Ghost's response?
- Does it avoid generic quiz design?
- Is the code structure explainable?
- For any LLM/backend work: is puzzle correctness still decided by deterministic validators / graph simulation / test cases / backend scoring (not the LLM)?
- Did Codex create a run log in `Docs/codex_runs/` for this run, with real (not claimed) test results and no chain-of-thought?

## Task Archiving Convention

- `Docs/CURRENT_TASK.md` always contains only the active task.
- When a task is completed, copy or summarize its final state into `Docs/completed_tasks/`.
- Archive filename format: `TASKID_short_description.md` (e.g. `M0-T01_initialize_planning_docs.md`).
- Each archived task file includes: completion status, date, summary, changed files, test / verification result, and next task.
- `Docs/HANDOFF_LOG.md` keeps a short chronological summary; `Docs/completed_tasks/` stores the fuller task records.
- Never delete or rename previous task files in `Docs/completed_tasks/`.

## Codex Run Log Review

When reviewing a Codex implementation or debugging run, Claude must check that:

- a run log exists in `Docs/codex_runs/` for the run (filename `TASKID_RUNNUMBER_short_description.md`);
- any "tests passed" claim is backed by tests that were actually run — otherwise the log should say `Not run — [reason].`;
- the log records actions, decisions, results, and evidence only (no hidden reasoning / chain-of-thought);
- older run logs were not deleted or overwritten.

See `Docs/codex_runs/README.md` for the convention.

## AI Collaboration Workflow

Canonical workflow: `Docs/AI_COLLABORATION_PROTOCOL.md`. Two agents plus the user as final decision
maker; ChatGPT is no longer part of the official workflow (ad-hoc strategy support only if asked).

- Claude is the repo-aware **project commander and reviewer**: reads the actual repo before planning;
  maintains ROADMAP/CURRENT_TASK/HANDOFF_LOG/ARCHITECTURE/LEARNING_CONTENT/DESIGN_RATIONALE; writes
  precise Codex prompts; reviews Codex work; performs closure and writes completed-task archives; and
  advances `CURRENT_TASK.md` only after implementation is verified. Claude follows `Docs/ROADMAP.md`
  + `Docs/LEARNING_CONTENT.md`, does not invent new Acts, and does not silently change the confirmed
  direction.
- Codex is the **implementation agent**: implements only the active CURRENT_TASK.md scope, writes a
  run log, must not expand scope or edit ProjectSettings/Packages/Build Settings/unrelated scenes,
  and returns a Claude review/closure prompt after implementation.
- Both Claude and Codex must include a Chinese STAR summary (S情境 / T任務 / A行動 / R結果) in their
  final reports. Repo docs — not chat memory — are the source of truth.
- During review/closure, Claude records human Editor verification separately from Codex's in-session
  "Not run" status, and never rewrites a run log's honest "Not run" as if Codex ran tests.
- Full-system components (LLM, backend, database) are required for the final system, but their plans
  must be reflected in `Docs/ROADMAP.md` and `Docs/ARCHITECTURE.md` before implementation. Puzzle
  correctness stays deterministic (validators / graph simulation / test cases / backend scoring); the
  LLM must never decide scoring.