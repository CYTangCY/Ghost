# M0-T20 — Act 3 Node Graph Design + Learning-Content Mapping (planning)

## Completion Status

Completed. Claude-led planning task (docs only, no Unity code). The user confirmed the design on
2026-06-22.

## Date

2026-06-22

## Summary

Defined Act 3 (Dialog Management via Node Graph, the flagship mechanic) before implementation, per
`CONFIRMED_PROJECT_CONTEXT.md` §17. Completed the Act 3 learning-content mapping and added a concrete,
minimal, deterministic node-graph data model + simulation/validation design and a logic-first
build-slice breakdown.

## Files Modified

- `Docs/LEARNING_CONTENT.md` — Act 3 section fully drafted (objective, cute Ghost problem,
  node-assembly mechanic, player action, success/failure, Lily hint, connection to Acts 1–2,
  deterministic correctness).
- `Docs/ARCHITECTURE.md` — "Act 3 minimal concrete design" (DialogNode/DialogTransition/DialogGraph;
  Start/IntentBranch/SlotCheck/Response; ConversationTurn + DialogContext; DialogGraphSimulator +
  DialogGraphValidator) and the M0-T21…M0-T26 build slices.

## Design Decisions (user-confirmed)

- Minimal node set: Start, IntentBranch, SlotCheck, Response (Acts 4–6 add node types, not new
  systems).
- Intent (Act 1) routes branches; entity (Act 2) fills slots; context remembers collected details.
- Correctness is deterministic: `DialogGraphSimulator` runs each test conversation through the
  assembled graph and `DialogGraphValidator` compares expected responses + structural checks. The LLM
  never decides correctness.

## Codex Run Logs

None — this was a Claude-led planning task, not a Codex implementation run.

## Human Verification Result

The user reviewed the design summary and confirmed ("確認", 2026-06-22). No Unity verification needed
(docs only).

## Next Task

M0-T21 — Act 3 node-graph core: pure C# data model + `DialogGraphSimulator` + `DialogGraphValidator` +
sample data + EditMode tests (scene-free), mirroring M0-T04/M0-T14.
