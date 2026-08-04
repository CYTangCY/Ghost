# M0-T51 … M0-T56 — Experience Polish and Depth Plan

> Source: user feedback session 2026-08-03 (post M0-T50 Run 002 review). Claude verified every item
> against the repo before writing this plan; root causes with `file:line` are recorded in section 3.
> This document is the planning source for six Codex slices. Codex implements ONE slice per run.

---

## 1. Decisions taken before planning

### 1.1 Drop WebGL from the shipped product — AGREED

The user asked whether WebGL is still worth keeping now that there is an installer. It is not.

- The WebGL client **still requires the local Node backend and Ollama** for hints, responses, and Lily
  chat (`Backend/src/server.ts` serves it, `GHOST_WEB_ROOT`). So it never delivers a real "open a link
  and play" demo. That was WebGL's only advantage, and it does not exist here.
- The browser adds a DPI/scaling layer that **actively worsens the single most repeated complaint in
  this feedback round (text too small)**. The verified smoke evidence shows a 960x600 canvas.
- A Windows standalone player removes the browser dependency, renders text crisply at native
  resolution, supports proper fullscreen, and **shrinks the installer** by dropping the WebGL payload.
- WebGL also carries Unity limitations (no threads, slow first load, IL2CPP quirks) that are pure risk
  during a supervisor demo or viva.

**Actions (M0-T51):**
- Convert `Assets/Editor/GhostWebGLReleaseBuilder.cs` to `GhostDesktopReleaseBuilder.cs` targeting
  `BuildTarget.StandaloneWindows64`. Keep the same nine-scene list and the same fail-on-error contract.
- Update `Deployment/build-release.ps1`, `Ghost.iss`, and `Deployment/README.md` to stage and install
  the standalone player instead of the WebGL folder.
- **Remove the now-dead WebGL serving code from `Backend/src/server.ts`** (the `GHOST_WEB_ROOT` /
  Brotli-header block, 41 lines added in M0-T50). Dead code removal is explicitly in scope this round.
- Retire `Deployment/test-installer.ps1`'s WebGL assertions; keep install/launch/uninstall/residue.

### 1.2 On the "make the code look human, not AI" instruction

One flag, stated once, then the plan proceeds.

This repo **openly documents AI collaboration** — `CLAUDE.md`, `Docs/AI_COLLABORATION_PROTOCOL.md`,
and 80+ run logs in `Docs/codex_runs/`. Restyling the source specifically so it is not *identified* as
AI-written is inconsistent with that documented process, and the user should keep whatever they submit
consistent with their department's AI-use disclosure policy. That is the user's call to make, not a
blocker on this work.

What is being specified instead is **genuine code-quality cleanup**, which delivers most of what the
user actually wants and is defensible on its own merits:

- Delete comments that restate the code (`// set the font size` above `fontSize = 18`). Keep comments
  that explain a *reason* or a non-obvious constraint.
- Shorten ceremonial names to idiomatic ones (`intentGroupListRoot` -> `pileList`,
  `ConfigureOrCreateLabel` -> `SetLabel`). **Not random names** — deliberately careless naming would
  cost marks on code quality. Short, natural, consistent.
- Merge the duplicated per-presenter UI helpers (`CreateSmallText`, `CreateFillText`, `CreateLabel`,
  `CreatePanel` appear in near-identical form across 8+ presenters) into the shared theme module from
  M0-T51. This is the single largest real duplication in the codebase.
- Remove dead code and unused fields as encountered.

**Behaviour must not change.** Every EditMode test must still pass, unchanged, after the cleanup.

**This cleanup is folded into every slice**, per the user's instruction to do it while fixing the
listed problems — it is NOT a separate task at the end.

---

## 2. Standing rules for all six slices

1. **Deterministic correctness is unchanged.** Validators, simulators, and test suites decide
   pass/fail. The LLM never scores or gates. Any new puzzle logic is a new pure class in
   `Ghost.Runtime` with EditMode tests.
2. **Ghost stays a cute ghost. Lily stays the timid, nerdy, kind postdoc.** No robot/chatbot framing,
   no generic-tutor voice.
3. **Programmatic assets only** — no external art imports. Rounded corners come from a runtime-
   generated sprite (section 4.1).
4. ASCII-only C#. 1920x1080 fit. No Console errors.
5. Honest run log per slice in `Docs/codex_runs/`, with a Chinese STAR summary. Never claim a test ran
   that did not.
6. Code-quality cleanup (section 1.2) applies to every file the slice touches.
7. Do not modify: `Packages/`, `ProjectSettings/` (except the already-approved
   `EditorBuildSettings.asset` scene entries), existing `.meta` GUIDs, or unrelated hand-edited scenes.

---

## 3. Verified root causes

Claude confirmed each of these in the repo. Codex should fix the cause, not the symptom.

| # | User report | Verified root cause |
|---|---|---|
| R1 | Text too small and too faint, everywhere | **No shared design tokens.** Font sizes are per-call integer literals scattered across 20 files (`fontSize = 13/14/15/17/18/20`), and each presenter has its own private text helper. Body copy sits at 13px. There is no single place to change it. |
| R2 | UI too square, not cute | Panels are plain `Image` components with **no sprite assigned** — only 6 files in the whole `Assets/Presentation` tree ever set `.sprite`. Everything else renders as a hard rectangle. |
| R3 | "Ask Lily" cannot be dragged above a certain height, only in some chapters | `Common/FloatingWindowDragHandle.cs:72-85` — `ClampToParent` computes bounds from `parentRect.rect.size` and `targetWindow.rect.size` but **ignores the window's `anchorMin`/`anchorMax`**. `anchoredPosition` is relative to the anchor, so the clamp is only correct for centre-anchored windows. Chapters whose Lily window uses a different anchor clamp early. This is exactly why it is chapter-dependent. |
| R4 | Return to Hub goes to the login screen | `Shell/ShellReturnToHubOverlay.cs` loads `ShellSceneNames.GameShellSceneName`, and `GameShellPrototype` boots at its title/account entry state rather than resuming the act-select hub. The overlay needs to signal "resume at hub" through `GhostNarrativeState`. |
| R5 | Ch.4 slider crushes the text above and below it | `Act4ConfidenceStaticPresenter.cs:464-501` — the "Confidence Dial" section is pinned to `preferredHeight = 164f`, but its children already demand 28 + 34 + 24 + 18 padding + 15 spacing = 119px **before the slider**, and the slider root is created with **no `LayoutElement`** (`CreateSlider`, line 548). The slider takes the remainder and squeezes the labels. |
| R6 | Ch.4: the first five visitors are correct no matter what | `Act4ConfidenceDemoData.cs:7-8` — the acceptable band is `65..80`, but the visitors' authored confidence scores are not clustered near that boundary, so most of the queue resolves identically across a wide range of thresholds. **The dial does not bite.** Only the edge cases discriminate. |
| R7 | Ch.5: cannot drag a wire out of the Start node; cannot drag backwards from the end | `Act3DialogGraphOutputPortView.cs:8` implements `IBeginDragHandler/IDragHandler/IEndDragHandler`, but `Act3DialogGraphInputPortView.cs:7` implements **only `IDropHandler`**. Wiring is therefore strictly output -> input and one-directional by construction. Reverse dragging is not a bug, it was never implemented. The Start node's output port also needs verifying that a port view is actually attached. |
| R8 | Ch.6: a box dragged to the right cannot be removed or put back | `Act6VoicePipeline/Act6PipelinePartDragView.cs` has drag-preview lifecycle only (`ClearOwnPreview`, `ClearActivePreviews`) and **no detach/return path**. Placement is one-way. |
| R9 | The "basics" block is redundant | `Assets/Presentation/Fundamentals/` (2 files) plus the hub button wiring in `GameShellPresenter.cs:207,277` and the whole `CreateFundamentalsScreen` block in `GameShellSceneBuilder.cs:308-370`. M0-T50 already stripped its interactive component-order beat and moved that teaching to the final chapter, so what remains is a text-only screen that duplicates Chapter 6. |

---

## 4. Slice plan

### M0-T51 — Global visual system + desktop build (foundation slice)

**Everything else depends on this. It must land first.**

#### 4.1 New shared module: `Assets/Presentation/Common/GhostUITheme.cs`

A single static class owning all visual tokens, plus the factory methods that every presenter will
call. This replaces the duplicated per-presenter helpers.

Font scale — **every size below is the new value; all are increases**:

| Token | Old (typical) | New | Use |
|---|---|---|---|
| `TitleSize` | 20 | **30** | Chapter title in the header |
| `HeadingSize` | 18 | **24** | Panel headings, objective strip |
| `BodySize` | 14 | **19** | Default body copy, dialogue, card text |
| `SmallSize` | 13 | **17** | Captions, hints, secondary labels |
| `TinySize` | — | **15** | Absolute minimum; use sparingly |

Nothing in the game may render below `TinySize`.

Text colours — raise contrast (the "too faint" complaint). Body text must reach at least **7:1**
against its panel background:

| Token | Value | Use |
|---|---|---|
| `Ink` | `#241C2E` | Primary body text |
| `InkSoft` | `#4A4159` | Secondary text — replaces every current "subtle" colour |
| `InkOnDark` | `#F4F0FA` | Text on dark panels |
| `Accent` | `#6D5BD0` | Interactive/emphasis |
| `Good` / `Bad` | `#1E7F63` / `#B3402F` | Result states |

`InkSoft` replaces all existing `SubtleTextColor` fields. Delete those fields.

Rounded, cuter shapes:

- `GhostUITheme.RoundedSprite(int radius)` — generates a 9-sliced rounded-rect `Sprite` at runtime
  via `Texture2D` (no external asset), cached per radius in a static dictionary.
- `PanelRadius = 18`, `CardRadius = 14`, `ChipRadius = 999` (full pill), `ButtonRadius = 16`.
- All panels, cards, chips, buttons, and drop zones use it. **No bare rectangular `Image` may remain
  in gameplay UI.**
- Add a soft 2px outline in `Accent` at 25% alpha on panels, and a 1px lighter inner edge on cards, so
  shapes read as soft rather than flat.

Factory methods (these are what presenters call — the per-presenter helpers get deleted):
`Panel(...)`, `Card(...)`, `Chip(...)`, `Button(...)`, `Label(...)`, `Heading(...)`, `DropZone(...)`.

#### 4.2 Chapter page composition — re-proportion

Current: 56px header + 48px objective strip, with the three information blocks below squeezed.
User report: the header block is too tall, the three blocks below are too small and crushed.

New composition for all chapter scenes:

- Header: **44px**, title at `TitleSize`, phase progress right-aligned. Trim the vertical padding.
- Objective strip: **40px**, `HeadingSize`.
- The three information blocks: **minimum 96px each**, `BodySize` text, 12px internal padding, and
  they must expand rather than clip. Give the row a `flexibleHeight` so it takes the space reclaimed
  from the header.
- Ghost conversation panel stays 170px.

#### 4.3 Desktop build conversion

Per section 1.1: `GhostDesktopReleaseBuilder.cs`, updated deployment scripts, and removal of the dead
WebGL block from `Backend/src/server.ts`.

#### 4.4 Migration requirement

Every presenter listed below must be migrated to `GhostUITheme` in this slice, and its private text /
panel helpers deleted:

`Act1IntentClassificationStaticPresenter`, `Act2EntityExtractionStaticPresenter`,
`Act3DialogGraphStaticPresenter`, `Act4ConfidenceStaticPresenter`, `Act5TestingStaticPresenter`,
`Act6BackendStaticPresenter`, `Act6PipelineStaticPresenter`, `FinalChapterConversationPresenter`,
`Chapter0StoryPresenter`, `AmbientBanterPanel`, `LilyChatWindow`, `GhostFaceView`, `LilyDialogueFrame`,
and the four `Editor/` scene builders.

**Scenes must be regenerated** by the existing scene builders after this change, and the regenerated
scenes verified for exactly one Main Camera / Canvas / EventSystem and zero missing scripts.

---

### M0-T52 — Global interaction fixes

1. **Fix `FloatingWindowDragHandle.ClampToParent` (R3).** Compute the clamp in the window's own anchor
   space: derive the anchor rect from `anchorMin`/`anchorMax` against the parent size, then bound
   `anchoredPosition` so the window stays fully inside the parent regardless of anchor configuration.
   Add EditMode tests covering centre, top-left, top-stretch, and full-stretch anchors.
2. **Return to Hub goes to act-select (R4).** Add `GhostNarrativeState.ResumeAtHub` (bool). The
   overlay sets it before loading `GameShellPrototype`; `GameShellPresenter` reads and clears it on
   boot, skipping title/account entry and opening the act-select hub directly. EditMode test for the
   set/clear round trip.
3. **Ask Lily highlights on a wrong answer, never auto-opens.** On a failed validation, pulse the Ask
   Lily button (scale 1.0 -> 1.08 -> 1.0, and an `Accent` glow) for ~2s and show an unread dot. Do not
   open the window. Applies to every chapter. Opening stays user-initiated.
4. **Delete the "basics" screen entirely (R9).** Remove `Assets/Presentation/Fundamentals/` (both files
   plus metas), the hub button and `ShowFundamentals` in `GameShellPresenter`, the
   `CreateFundamentalsScreen` block in `GameShellSceneBuilder`, and
   `Assets/Tests/EditMode/ChatbotFundamentalsDataTests.cs`. Regenerate `GameShellPrototype`. Confirm
   no dangling serialized references remain in the scene.
5. **Question variety.** Rewrite authored puzzle content so it is not uniformly lab-themed. Ghost is a
   ghost haunting a building — visitors should include a lost delivery courier, someone hunting a
   vending machine, a student looking for a room, a cat owner, a person asking about opening hours.
   Keep two or three lab-flavoured items so Lily's world still reads. This touches authored data only,
   never validator logic, and **existing tests must still pass** — if a test asserts on specific
   authored strings, update the test alongside it and say so in the run log.

---

### M0-T53 — Chapters 1-3: layout, depth, and visitor coupling

**Chapter 1 (Intent Classification)** — the worst layout offender.

- The right-hand pile column collapses once more than two piles exist, and the player cannot see how
  many transcripts are in a pile. Rebuild it as a **2-column grid of pile cards** inside a vertical
  `ScrollRect`, each card showing `intent name` + a **count badge** + the first two transcript chips,
  with the rest collapsed behind a "+N more" chip that expands on click.
- The training block is oversized and covers the visitor canvas. Move training to a **48px bottom
  bar** with the button and a one-line status. It must not overlap the visitor area.
- The visitor scene at the top is nearly inert. Make each transcript **originate from a visible
  visitor**: when the player picks up a transcript chip, the corresponding visitor at the top
  highlights and speaks the line; when a pile is trained, the visitors whose messages it covers react.
- Target: the player can comfortably work with **at least 5 piles and 12 transcripts** on screen.

**Chapter 2 (Entity Extraction)** — mechanically fine, too easy, weak visitor coupling.

- Raise difficulty: add messages where the **same word is a different entity depending on context**,
  one message with **two entities of the same type** (pick-up and drop-off), and one with a decoy
  phrase that looks like an entity but is not.
- Visitor coupling: each extracted entity **visibly fills a slot on the visitor's errand card** at the
  top as it is assigned, and a wrong assignment makes that visitor react immediately.

**Chapter 3 (Dialog Graph)** — too easy, weak visitor coupling.

- Raise difficulty: require a graph that handles **two intents plus one missing-entity re-ask branch**,
  rather than a single linear path.
- Visitor coupling: when the player hits Validate, the visitor at the top **walks through the graph
  live**, node by node, with the current node highlighted, so the graph's shape visibly causes the
  conversation.

---

### M0-T54 — Chapter 4 redesign: two handles, three bands

**The pure logic for this slice is already written and verified — see "Delivered" below. M0-T54 is now
a presentation-only slice.**

The user's verdict on the old chapter: purpose unclear, the fallback/handoff toggles show no visible
difference, a correct answer teaches nothing, and the queue never discriminates. Root cause R6, stated
exactly: the old pass band was `65..80` and **not one visitor's confidence score fell inside it**, so
the dial could not change any outcome within the passing range. It was decorative by construction.

**The redesign.** One dial becomes a 0-100 axis with **two draggable handles** cutting three bands:

```
0 ─────────┬──────────────┬───────── 100
  call Lily │ ask to rephrase │  Ghost answers
```

Why this beats one threshold plus two on/off toggles: fallback and handoff stop being abstract
switches and become *places on the same scale*. Each visitor is a dot on that axis, so dragging a
handle visibly moves a specific person between bands — which is precisely the feedback the user says
is missing.

**The three-plus-one visitors** (deliberately not lab-themed):

| Visitor | Score | Role |
|---|---:|---|
| `vending-machine` — "Which floor is the vending machine on?" | **88** | The ceiling. Put the answer handle above 88 and Ghost bounces a question a noticeboard could answer. |
| `courier-vague` — "I'm after that room, the one with the machine in it?" | **63** | **The decision.** Answer *or* bounce — both pass, both cost something. |
| `locked-out` — angry, card won't scan | **34** | Upset. Only a human ends this well. |
| `hurried-parent` — arrives on **pass 2** | **71** | Sits where most players parked the answer handle. Forces one deliberate re-decision. |

**The lesson that makes the third band necessary:** an upset visitor routed to *ask-to-rephrase* melts
down **worse** than one Ghost bluffs at. Telling someone who has been locked out for half an hour to
say it again more nicely is the wrong move. Fallback is not a safe default — that is the teaching
beat, and it emerges from the routing rules rather than being special-cased.

**No perfect setting.** The courier at 63 legitimately goes either way:
- answer handle ≤ 63 → answered → Ghost sends him to the wrong floor; he's back eleven minutes later.
- answer handle ≥ 64 → bounced → he's mildly annoyed but the box is delivered.

Both pass; the debrief names which risk the player chose (`Act4Posture.Bold` / `Cautious`).

#### Delivered (Claude, verified 2026-08-03)

Written and **executed against a standalone .NET run** of the puzzle assembly — 44 assertions, all
passing, plus a full sweep of the handle space:

- `Assets/Scripts/Puzzles/ConfidenceFallback/Act4ConfidenceModels.cs` — `Act4Zone`, `Act4Posture`,
  `Act4VisitorLines` (authored text per band), `Act4VisitorMessage` (now carries **accepted
  outcomes**, plural), `Act4ZoneConfiguration` (two handles + per-band wiring), `Act4ShiftTally`.
- `Act4ConfidenceDemoData.cs` — the four visitors above with authored lines for every band, plus
  `DescribePosture`.
- `Act4ConfidenceValidator.cs` — band routing, the upset-visitor meltdown rule, per-visitor failure
  explanations, posture reading, scoreboard tally.
- `Assets/Tests/EditMode/Act4ConfidenceValidatorTests.cs` — 15 tests including
  **`EveryVisitorFlips`**, a permanent regression guard asserting no visitor's outcome can be constant
  across the handle space. That test exists specifically so the R6 bug cannot return.

Measured solution space: **667 of 5151 handle positions pass (12.9%)** — Lily handle `35..63`, answer
handle `35..71`, of which 435 are Bold and 232 Cautious. Tight enough to require thought, wide enough
that both postures are comfortably reachable.

#### Remaining for Codex (presentation only)

1. **Two-handle axis widget** with the three bands colour-coded, and **each visitor drawn as a dot at
   their score**. Dots recolour live as handles move — the consequence must be visible *before* "Run
   the evening".
2. **Wiring by drag:** the player drags an "ask them to rephrase" card into the middle band and a
   "call Lily" card into the bottom band. An unwired band means Ghost says nothing — already modelled
   as `Act4RouteOutcome.NoSafeRoute`.
3. **Scoreboard** after the run: answered / rephrased / handed off / upset from `Act4ShiftTally`, plus
   `Act4ConfidenceDemoData.DescribePosture(result.Posture)` as Lily's verdict.
4. **Two passes:** run with `ArrivesInPass == 1` visitors, then re-open the handles and run the full
   queue including `hurried-parent`.
5. **Fix the slider layout (R5).** The old bar had no `LayoutElement` inside a section pinned to
   `preferredHeight = 164f`, so it crushed the labels. Give the axis an explicit
   `preferredHeight = 28f` and raise the section to **210**. The bar is **max-width ~420px, centred** —
   the complaint was vertical crushing, not width.
6. Rewrite `Act4ConfidenceInteractionController` and `Act4ConfidenceStaticPresenter` against the new
   model, and add presenter tests to replace the one dropped from the old test file.

---

### M0-T55 — Chapter 5: per-visitor maps and bidirectional wiring

1. **Per-visitor buggy maps.** Currently one shared pre-built graph. Give **each visitor their own
   pre-built map with its own seeded fault**, so the player diagnoses and repairs several distinct
   bugs rather than one. Minimum three visitors, each with a different fault class: a swapped
   transition condition, a wrong response node, and a missing intent branch.
2. **Raise difficulty.** The failing-test list should report *expected vs actual* but **not name the
   faulty node** — the player must read the trace to locate it.
3. **Fix wire dragging (R7).** Make `Act3DialogGraphInputPortView` also implement
   `IBeginDragHandler/IDragHandler/IEndDragHandler`, and `Act3DialogGraphOutputPortView` also
   implement `IDropHandler`, so a wire can be dragged **from either end** and dropped on either. Add
   a shared `IDialogGraphWireInteractionHost` path for both directions (the interface already exists
   at `Assets/Presentation/Act3DialogGraph/IDialogGraphWireInteractionHost.cs`).
4. **Verify the Start node has an output port view attached** and can begin a drag. The user reports
   it cannot. Fix in the scene builder and regenerate.
5. Dragging an existing wire's end off a port and releasing on empty canvas **detaches** it.

Correctness still comes from the existing `DialogGraphSimulator`/`DialogGraphValidator`. EditMode tests
must prove each seeded buggy map FAILS and each reference repair PASSES.

---

### M0-T56 — Chapter 6 and the Final chapter

**Chapter 6**

1. **Placement must be reversible (R8).** A placed part can be dragged back to the palette, or removed
   via a small round "x" on the placed card. Add the detach path to `Act6PipelinePartDragView` and the
   presenter.
2. Raise difficulty: add **two decoy parts** that look plausible but belong to no stage.
3. Text sizes come from `GhostUITheme` (handled by M0-T51).

**Final chapter** — the user wants this to be the real capstone.

1. **Exactly three visitors, escalating**, each requiring a decision:
   - Visitor 1: a simple, unambiguous request.
   - Visitor 2: more complex — two entities and an ambiguous intent.
   - Visitor 3: **Lily herself**, asking something genuinely harder, as the final test.
2. **Intent becomes a real choice.** Present it as a multiple-choice, but **remove the filler
   options**. Every distractor must be *plausible and specifically wrong* — a near-miss intent, a
   too-broad intent, an intent that matches a different visitor. The player should have to think.
3. **Entities become a composition puzzle.** Rather than picking one span, the player assembles the
   correct **combination of sentence fragments** into the entity slots, with plausible wrong
   combinations available.
4. **Dialogue opens a real pipeline window.** Clicking the dialogue stage opens a **floating,
   collapsible/savable window** running the Chapter 3 graph mechanic — build the reply path, then
   collapse or save it and return to the conversation. Reuse `FloatingWindowDragHandle` (fixed in
   M0-T52) and the Act 3 components.
5. **Response selects a backend data source + action.** The left palette holds **many distractor
   options**; the right side has a box with **three pipeline slots**; the player drags the correct
   three in. Removal and drag-back must work (same requirement as Chapter 6 item 1).
6. **Fix the slider (same as R5):** the problem is **vertical**, not width — give it a bounded
   `LayoutElement` height so it stops crushing the text above and below.
7. **Ending beat.** Lily's "I finally made a friend" line must be delivered as a **murmur** — smaller
   than body text is not allowed, so instead: italic, `InkSoft` colour, lower opacity fade-in,
   rendered in a smaller *bubble* offset to the side, with a "(quietly, to herself)" stage direction.
   She should not be looking at the player when she says it. This is the emotional payoff of the whole
   game — it must read as accidental honesty, not a speech.

---

## 5. Feedback traceability (使用者原始意見 -> 對應切片)

| 原始意見 | 切片 |
|---|---|
| 文字太小、顏色太淡 | M0-T51 (§4.1) |
| 既然要安裝，為什麼還要 WebGL | M0-T51 (§1.1) — 同意移除 |
| Return to hub 應回到選關而非登入 | M0-T52 (2) |
| 每章開頭標題佔太大、下方三 block 太小 | M0-T51 (§4.2) |
| Ask Lily 拉不過某高度 | M0-T52 (1) — 根因 R3 |
| 答錯應高亮 Ask Lily 而非直接開啟 | M0-T52 (3) |
| UI 太方塊，要更圓潤可愛 | M0-T51 (§4.1) |
| 問題要多樣化，不要全是 lab | M0-T52 (5) |
| 拿掉 basic 區塊 | M0-T52 (4) |
| 第一章右側擁擠、看不到放了幾個、training block 過大、與訪客互動少 | M0-T53 |
| 第二章太簡單、字小、互動少 | M0-T53 |
| 第三章太簡單、互動不足、字小 | M0-T53 |
| 第四章拉桿過長擠壓文字 | M0-T54 (6) — 根因 R5 |
| 第四章用意模糊、fallback/handoff 無差別、玩法差、前五個訪客怎樣都對 | M0-T54 (1-5) — 根因 R6 |
| 第五章每位顧客應有不同 pre-built map、難度低 | M0-T55 (1-2) |
| 第五章 start 拉不出線、尾端不能反向連 | M0-T55 (3-4) — 根因 R7 |
| 第六章太簡單、字小、拖過去無法取消 | M0-T56 — 根因 R8 |
| Final 拉桿上下太長 | M0-T56 (6) |
| Final dialogue 不應只用選的 | M0-T56 (4) |
| Final 只要三個訪客、要能思考決策 | M0-T56 (1) |
| Final intent 選項要實際且有誘答性 | M0-T56 (2) |
| Final entities 要拆句組合 | M0-T56 (3) |
| Final response 要選 backend datasource action | M0-T56 (5) |
| 結局 Lily 交到朋友應用 murmur | M0-T56 (7) |
| 程式碼自然化、去重、刪無用碼 | 每個切片皆含 (§1.2) |

---

## 6. Acceptance criteria (per slice)

- No text renders below `GhostUITheme.TinySize` (15px); body copy is `BodySize` (19px).
- No bare rectangular `Image` remains in gameplay UI.
- All EditMode tests pass; new pure logic has new tests; the suite total only grows.
- Regenerated scenes: exactly one Main Camera / Canvas / EventSystem, zero missing scripts.
- 1920x1080 fit, no Console errors, no clipped or overlapping panels.
- Deterministic correctness unchanged; the LLM is absent from every scoring path.
- Honest run log with a Chinese STAR summary.

---

## M0-T56 — Final Chapter integration redesign (specification, not yet implemented)

Written 2026-08-04, after Chapters 1-5 changed. The Final Chapter is the capstone, so it has to test
what those chapters now actually teach - not what they taught when it was first written.

### What each chapter now teaches, and what the Final must therefore ask

| Chapter | What it now teaches | What the Final should ask of it |
| --- | --- | --- |
| 1 Intent | Group by **purpose**, not wording. Ghost's reply comes from the player's grouping. | Pick the intent from options that are all plausible - a near-miss, a too-broad one, and one that belongs to a different visitor. No filler. |
| 2 Entities | Boundary and type are separate mistakes. Same word, different entity in context. Decoys that look like entities. | Assemble the entity from **sentence fragments**, with a decoy fragment present. One token per slot, matching the Chapter 2 interaction. |
| 3 Dialogue | Not every request needs a slot check. Two intents, one re-ask branch. Build for who is in front of you. | Open a **floating, collapsible window** running the Chapter 3 graph mechanic. Reuse the palette-from-data pattern so it cannot drift. |
| 4 Confidence | The score is a **proxy**. No threshold pleases everyone; choose a defensible trade-off. | Show the confidence for this visitor and make the player decide answer / ask again / hand over - and live with the cost. |
| 5 Testing | Run the suite before trusting a map. Read expected vs actual to locate a fault. | Before the final reply, run it once and let the player see the trace. |

### Structure

Three visitors, escalating, each requiring a real decision:

1. **Simple** - one clear request. Intent and entity only. Establishes the loop.
2. **Complex** - two entities of the same type and an ambiguous intent. Needs the dialogue window.
3. **Lily** - asks something genuinely harder. This is the only one that needs the backend response
   route, and it is where the confidence trade-off bites.

Response stage: the left palette holds many distractors; the right side has a box with three pipeline
slots; the player drags the correct three in. **Removal and drag-back must work** - the same
requirement as Chapter 6, and the Chapter 2 lesson that placement must be reversible.

### Constraints carried over from what went wrong earlier

These are not optional. Each one is a bug that shipped in this project.

1. **Reachability first.** Before authoring any required answer, confirm the interaction can produce
   it. Chapters 2 and 3 both shipped puzzles whose passing state was unreachable while every
   rule-level test stayed green. Write the reachability guard test *before* the data.
2. **No perfect-answer arithmetic.** If the screen shows the numbers and a perfect answer exists, the
   player solves it without understanding. Chapter 4 had to be rebuilt twice for this.
3. **Distractors must be plausible and specifically wrong.** A near-miss intent, a too-broad intent,
   an intent belonging to another visitor. Filler options teach nothing.
4. **Correct option positions must vary** across visitors and categories, so position cannot be
   memorised. There is already a test asserting this - keep it.
5. **Every authored line needs a reason to exist.** Ghost's reply must visibly follow from what the
   player built, the way Chapter 1's generalisation engine does.

### Ending

Already implemented: Lily's "I finally made a friend" is a murmur - italic, soft ink, offset, with a
"(quietly, to herself)" stage direction, and she is not looking at the player. It is the emotional
payoff of the whole game and must read as accidental honesty, not a speech.

### Order of work

Palette/interaction reachability, then data, then validator plus tests, then presenter. Any step can
then fail as a compile error rather than as a chapter that builds and cannot be played.
