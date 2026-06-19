# M0-T10 — Act 1 UI/Code Architecture Review

## Completion Status

Completed (Claude review task — no code, scenes, or settings changed; no Codex run). The review
report was produced and its recommendation accepted: do a small behaviour-preserving presentation
refactor (M0-T11) before drag-and-drop.

## Date

2026-06-19

## Summary

Light architecture review of the Act 1 intent-classification prototype before adding drag-and-drop.
The actual source was read (the four logic classes, the ~824-line `Act1IntentClassificationStaticPresenter`,
the Editor scene builder, and the assembly setup). Conclusion: the pure logic foundation is in very
good shape and is DnD-ready; the accumulated debt is concentrated entirely in the presentation layer.

## Architecture Strengths

- The pure logic layer is genuinely pure and **compiler-separated** from Unity: `Ghost.Runtime.asmdef`
  sets `"noEngineReferences": true`, so the logic cannot reference `UnityEngine`.
- Each logic class has a single clean responsibility (see breakdown). The validator is thorough and
  side-effect-free; the session returns defensive copies of all exposed collections.
- Validation is reached only through the session (`session.ValidateCurrentState()` → validator); the
  UI never re-implements grouping rules. Assignment/unassignment also go straight through the session.
- The scene builder uses official `EditorSceneManager` APIs behind a menu item (no hand-written
  `.unity` YAML).
- The new Input System is wired (`InputSystemUIInputModule` on the EventSystem).

## Concrete Risks

- **R1** — The presenter is overloaded (~824 lines, ~4 responsibilities).
- **R2** — UI construction is duplicated across the presenter and the scene builder (two sources of
  truth; e.g. `ConfigureAssignmentScrollView` exists in both). They must be kept in sync by hand.
- **R3** — The saved scene is largely throwaway: the builder renders sample data at edit time, then
  `Start()` destroys and rebuilds it at runtime.
- **R4** — The builder injects the presenter's private `[SerializeField]` fields via reflection
  (`SetPrivateField`), which breaks silently on a field rename and runs against NFR2.
- **R5** — Presentation has no assembly definition; it compiles into the default `Assembly-CSharp`
  (works only because `Ghost.Runtime` is auto-referenced), so there is no explicit presentation
  boundary as Act 2+ arrives.

These are structural risks, not correctness bugs (the prototype runs and was human-verified).

## Responsibility Breakdown

- `IntentCard` — immutable card value object (validates id/intentId). Single, clean.
- `IntentClassificationValidator` (+`IntentClassificationResult`) — pure grouping validation + error
  list. Single, clean, no Unity dependency.
- `Act1IntentClassificationSampleData` — static sample cards + correct groups. Single, clean.
- `IntentClassificationSession` — mutable assignment state; delegates validation. Single, clean.
- `Act1IntentClassificationStaticPresenter` — owns the session, handles interaction
  (select/assign/unassign/validate), updates visual state, AND procedurally builds the UGUI
  hierarchy. Four responsibilities — too large.
- `Act1IntentClassificationPrototypeSceneBuilder` — Editor scene authoring. Useful, but duplicates
  the presenter's UI construction (R2) and uses reflection injection (R4).

## Presenter Risk

`Act1IntentClassificationStaticPresenter` is over the line: ~824 lines mixing rendering, session
ownership, interaction handling, visual-state/colour updates, assignment editing, and validation
feedback. The bulk is procedural UI construction, which is also the part duplicated with the builder.
Adding drag-and-drop on top would enlarge this class further and worsen the duplication.

## Scene-Builder Risk

Mostly safe (official scene APIs, no YAML, menu-driven), but moderate caveats: it duplicates
UI-construction with the presenter (R2), injects fields via reflection (R4), bakes content that
runtime immediately rebuilds (R3), and regeneration overwrites any manual scene edits. Reliable for
this prototype but fragile to extend.

## Recommendation

Do a **small, behaviour-preserving presentation refactor (M0-T11) before drag-and-drop.** The logic
layer is excellent and DnD-ready; the only obstacle to clean DnD is the overloaded presenter and the
presenter/builder duplication. The smallest safe refactor: add a presentation assembly boundary and
extract interaction/state orchestration out of the presenter into a small
`Act1IntentClassificationInteractionController`, with no behaviour or visual change. Deferred (noted
as later optional follow-ups, not bundled into M0-T11): unifying the presenter/builder UI
construction into one shared factory (R2) and removing the reflection injection (R4).

## Next Task

M0-T11 — Refactor the Act 1 presentation layer before drag-and-drop, without changing visible
behaviour.
