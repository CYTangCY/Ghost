# M0-T49 - Run 020 - Final Chapter Project Path Audit

## Task ID

M0-T49

## Run Number

020

## Date

2026-07-30

## Original Request / Codex Prompt Summary

Confirm which Unity project is actually open, compare it with the Codex worktree, and trace why the
Final Chapter redesign was not visible to the user before making further gameplay changes.

## Files Created

- `Docs/codex_runs/M0-T49_020_final_chapter_project_path_audit.md`

## Files Modified

- None.

## Tests or Checks Run

- Read the required project context, roadmap, active task, requirements, learning-content mapping,
  and architecture documents.
- Inspected the command line of the running Unity Editor process.
- Inspected the Unity Editor log for the active project and recently loaded scenes.
- Compared Git roots, HEAD revisions, branches, and working-tree status for the Codex worktree and
  the Unity-open project.
- Compared SHA-256 hashes for the Final Chapter scripts, generated scene, tests, shell scene names,
  shell builder, and build settings.
- Searched both copies for the redesigned `Run all 3 tests` content and the earlier generic
  UI-input/NLP-engine pipeline content.
- Traced the Final Chapter scene builder and presenter's runtime rendering path.
- Checked the prior Run 018 batchmode log for its project path.
- Tests: Not run - this was a read-only project-path and scene-source audit.

## Test / Check Result

- The running Unity Editor uses `D:\Code\Ghost`.
- The Codex run operated in
  `C:\Users\fcxsw\.codex\worktrees\1479\Ghost`.
- Both copies share commit `e2f3c9a63273717c318d39ea876e545a96a7823a`, but the Codex worktree
  is detached and contains uncommitted Run 018 Final Chapter changes that are absent from
  `D:\Code\Ghost`.
- The Run 018 batchmode log confirms that its scene builder ran against the Codex worktree, not the
  project open in the user's Unity Editor.
- `D:\Code\Ghost` contains the earlier five-component UI input / NLP engine / dialogue management /
  response generation / UI output puzzle and contains no `Run all 3 tests` redesign terms.
- The Codex worktree contains the redesigned learned-responsibility board and three-test loop.
- The Final Chapter scene builder calls `RenderSampleData()` before saving the generated scene, and
  the presenter calls it again at Play Mode start. Therefore the visible Final Chapter is determined
  by the scripts and generated scene inside the project Unity actually opens.
- `Docs/CURRENT_TASK.md` still describes the earlier generic pipeline, while
  `Docs/LEARNING_CONTENT.md` contains the user-approved 2026-07-29 redesign.

## Errors Encountered

- The Windows workspace sandbox returned `helper_unknown_error: apply deny-read ACLs` on initial
  read-only shell calls.

## Fixes Applied

- Re-ran the required read-only checks with approved elevated sandbox access.
- No project files, scenes, metadata, or settings were changed.

## What Was Intentionally Not Changed

- No Final Chapter code was copied or edited.
- No Unity scene was regenerated.
- No `.meta` file, ProjectSettings file, package, build setting, or user-owned change was modified.
- `Docs/CURRENT_TASK.md` was not changed because this run only established the mismatch; the
  planning record should be reconciled before or with the actual Final Chapter implementation.

## Remaining Risks

- Copying only the generated scene or only part of the scripts would create another mixed version.
- Both project copies have substantial uncommitted changes, so a broad directory copy could
  overwrite unrelated user work.
- Run 018 did not include a human Play Mode visual check, so the redesigned board still requires
  direct verification in the canonical Unity project.

## Next Recommended Step

Choose `D:\Code\Ghost` as the user-facing canonical Unity project or deliberately reopen the Codex
worktree in Unity. Then transfer or reapply only the complete Run 018 Final Chapter change set,
preserve Unity asset GUIDs, regenerate the Final Chapter scene in that same project, and perform a
human Play Mode check before reporting the redesign as complete.

## Chinese STAR

- **S 情境：** 使用者在 Unity 中仍然看到舊的 Final Chapter，但先前的 Codex run 已記錄新的六章整合設計。
- **T 任務：** 找出 Unity 實際使用的專案、場景來源，以及新設計沒有出現在畫面上的原因。
- **A 行動：** 比對 Unity process、Editor log、兩份 Git working tree、Final Chapter 檔案雜湊、
  關鍵文字、scene builder 和 Run 018 batchmode log。
- **R 結果：** 確認 Unity 開啟的是 `D:\Code\Ghost`，而新 Final Chapter 只存在 Codex
  worktree；問題是兩份未同步的專案，不是玩家漏看了 runtime 畫面。
