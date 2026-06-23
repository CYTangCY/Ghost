# M0-T24 — Act 3 Node Placement + Connection Interaction (functional prototype; UX debt)

## Completion Status

Completed functionally (Codex run 001); the user confirmed it works in the Editor ("做完了", with a
screenshot). Closed as a **functional prototype**, not a final design — the Act 3 node-graph UX is
flagged for redesign (see Known Issues) and deferred into the upcoming vertical-slice / narrative pass.

## Date

2026-06-22

## Summary

Added `Act3DialogGraphInteractionController` (wraps `DialogGraphSession`) and made the Act 3 presenter
interactive: place configured nodes from the palette, select a node card, set a start node, connect
nodes via From/To selection + condition buttons (Always/SlotPresent/SlotMissing), a transitions list
with per-row remove, and remove-node. All placement/config/connection/removal route through
`DialogGraphSession`; the Validate button remains a placeholder.

## Files Created / Modified

- Created `Assets/Presentation/Act3DialogGraph/Act3DialogGraphInteractionController.cs`
- Modified `Assets/Presentation/Act3DialogGraph/Act3DialogGraphStaticPresenter.cs`
- Modified `Docs/CODE_WALKTHROUGH.md`, `Docs/UNITY_TEST_CHECKLIST.md`
- Created `Docs/codex_runs/M0-T24_001_act3_node_placement_connection.md`
- Regenerated `Assets/Scenes/Act3DialogGraphPrototype.unity` (not in Build Settings)

## Claude Review / Status

- Mechanically meets the M0-T24 criteria: place each node type, select, set start, connect with each
  condition, remove node (cascade) + remove transition — all via `DialogGraphSession`; Validate stays a
  placeholder; no Build Settings, Act 3 pure logic, Act 1/2, or Game Shell edits.

## Known Issues / UX Debt (user feedback, 2026-06-22)

- Not fun; the level's objective is unclear ("不知道這關要做什麼").
- Connection UX is mechanical and error-prone: separate From/To selection + condition buttons instead
  of a free-form "drag a wire between nodes" feel; it allows invalid self-loops (the screenshot showed
  `slotcheck_2 → slotcheck_2 [Always]`).
- → Deferred: an **Act 3 node-graph UX redesign** (clear in-story objective, free-form connecting,
  reject self-loops / invalid edges), to be done as part of the narrative + vertical-slice pass.

## Human Verification Result

The user confirmed the interaction works in the Unity Editor (screenshot) but raised the design
critique above. Functional status comes from the user's Editor check.

## Next

Direction pivot (see `Docs/ROADMAP.md` Current Status, 2026-06-22): move to a vertical slice / cohesive
build — full-system foundation (backend/DB/LLM) + narrative integration into Acts 1–3 + the Act 3 UX
redesign — before resuming Acts 4–8. The specific next task is being chosen with the user.
