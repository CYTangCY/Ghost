# M0-T56 run 002 — dialogue as connect-the-lines, Lily staged as a conversation

Date: 2026-08-04
Agent: Claude (implementation; Codex is out of the workflow)
Scope: user feedback on the Final Chapter after run 001, plus the compile fix run 001 left behind.

## 0. Compile fix carried over from run 001

`FinalChapterConversationPresenter.cs` lines 384-393 held two string literals split across real
newlines (CS1010/CS1003 x6). Repaired with the `Edit` tool — **not** a heredoc/Python script, which is
how this bug shipped three times. A repo-wide sweep for unterminated literals across
`Assets/Presentation`, `Assets/Scripts` and `Assets/Tests` came back clean.

## 1. What the user asked for

1. Visitors 2 and 3 build their dialogue by connect-the-lines, like Chapter 3.
2. Visitor 3 should read as a Lily/Ghost conversation — portraits facing each other, dialogue beside each.
3. Lily's correct answer being a *blank* slot is odd; there should be a "don't know" option instead.
4. The chapter should still contain a dialogue stage.
5. Lily's ending murmur should carry a bracketed stage direction: a hidden fist-pump behind her back.

## 2. What changed

### Dialogue is now a drawn graph (1, 4)

`FinalChapterConfiguration` carries `FinalChapterLink` wires instead of an ordered `List<string>`.
The controller holds the wires; `RouteStepIds` is now **derived** by walking them from the fixed start.

`CheckDialogue` walks start -> ... -> reply and compares the cards it passes against the visitor's
authored `ExpectedRouteStepIds` — so a chain still reduces to an ordered list and the same authored
answer decides correctness. What the walk adds is failures a list could not express: a branch, a loop,
a dead end, and cards wired to each other off the path. Each has its own message.

Two fixed endpoints (`route_start`, `route_end`) are never in a palette — where a conversation begins
and where it must arrive are not choices.

**Lily gained a Dialogue stage** (now 6: Intent, Details, Dialogue, Stored data, Confidence call,
Response). Her route is `detect_intent -> look_up -> ask_which`. The lookup deliberately precedes the
question: Ghost does not know the request is open until the job queue hands back two rows. The
ambiguity is *discovered*, not read off the 78%.

### Lily's stretch is staged as a conversation (2)

`CreateConversationPanel` branches on `IsLilyVisitor`: her pixel portrait on the left with what she
said, Ghost's face on the right with what he is making of it, the two speech columns between them
aligned outward so the pair face each other. Panel 178 -> 198px for that variant only.

### "They never said" is a card, not a blank (3)

New `FinalChapterFragment.IsUnknownMarker`. `slot_which_job` now *expects* `frag_not_stated` — leaving
it blank fails, and so does guessing. The card is offered to **all three** visitors (a decoy for the
first two), otherwise its presence alone would announce which visitor left something open.

### Ending murmur (5)

`Act6EndingSequence.cs`: "R-really? I... I finally made a friend. (behind her back, out of sight, one
hand closes into a small, triumphant fist)". Rendering was already italic + "(quietly, to herself)".

## 3. Files changed

- `Assets/Scripts/Puzzles/VoicePipeline/FinalChapterConversationModels.cs`
- `Assets/Scripts/Puzzles/VoicePipeline/FinalChapterConversationData.cs`
- `Assets/Scripts/Puzzles/VoicePipeline/FinalChapterConversationValidator.cs`
- `Assets/Presentation/Act6VoicePipeline/FinalChapterConversationController.cs`
- `Assets/Presentation/Act6VoicePipeline/FinalChapterConversationPresenter.cs`
- `Assets/Presentation/Act6VoicePipeline/Act6EndingSequence.cs` (murmur line only)
- `Assets/Tests/EditMode/FinalChapterConversationTests.cs` (20 -> 26 tests)
- New: `Assets/Presentation/Act6VoicePipeline/FinalChapterRoutePortView.cs`
- New: `Assets/Presentation/Act6VoicePipeline/IFinalChapterWireHost.cs`

The two new files need `.meta` files, which Unity generates on import. Every other file already existed.

Untouched: every other chapter's validator and authored data, `Packages/`, `ProjectSettings/`.

## 4. Test results — real, not claimed

**Run: standalone .NET 10 harness, 117 checks, all passing.** `Ghost.Puzzles.VoicePipeline` has no
UnityEngine dependency, so models/data/validator were compiled and executed outside Unity. Covered:

- reachability — every visitor's reference solution passes and produces the authored reply (this is the
  check that exists because Chapters 2 and 3 shipped unsolvable puzzles with green rule-level tests);
- every stage a visitor declares can fail, and blames itself, including Lily's new Dialogue stage;
- the trace still marks exactly one broken stage plus the reply;
- the not-stated card: guessing fails, blank fails, the card passes, and it is rejected on every slot
  the message does answer, for all three visitors;
- route failures: branch, dead end, off-path wires, loop — all rejected and blamed on Dialogue;
- unchanged invariants: intent indices 1/3/0, distinct failure replies, the 78% still points the wrong
  way, escalation 2 -> 3 -> 6 stages.

**Not run — anything needing Unity.** No compile, no EditMode run, no Play Mode, no screenshots. Claude
cannot open the Editor; the user's pass is what confirms these.

EditMode attribute count after this run: **150 `[Test]` + 26 `[TestCase]` = 176 discrete cases**, of
which the Final Chapter fixture contributes 26 `[Test]`. (Run 001's handoff predicted 170; the increase
is the 6 tests added here.)

## 5. Expected on the user's Editor pass

1. Compile clean.
2. EditMode 176/176. Watch `ThePresenterRendersEveryStageOfEveryVisitorWithoutThrowing` — it is still
   the only automated check on the new UI, and it now renders the wire board too.
3. Play Mode: visitor 2 and visitor 3 both show the route board; wires drag from a bottom dot to a top
   dot, either direction; clicking a bottom dot takes its wire down.
4. **Expect a layout repair round.** Every height in the wire board (card 54px, endpoint 44px, port 20px,
   stack spacing 20px, Lily panel 198px) was derived by adding up contents, not by looking at it. Every
   chapter so far has needed exactly this pass.

## 6. Explainability

Puzzle correctness stays deterministic. The wires are player state; `FinalChapterConversationValidator`
alone decides, from authored data. No language model touches scoring. The route read-out beside the
board states in plain English what the drawing currently says, so the player is never guessing what the
graph means.

## 7. Follow-up in the same session — user feedback on the first attempt

The user played it and reported two things.

### The TEST PASS button was eating clicks

`ShellReturnToHubOverlay.CreateCheatButton` anchored the debug button to the **bottom-left corner** at
(18, 18). Every chapter puts its palette column there, and the overlay canvas draws at sorting order
32767, so it sat on top of the palette and swallowed pointer events meant for it.

Moved to the top right — anchored (1,1), position (-28, -80), 104x26 — which lands under the
"Return to Hub" button and over the **empty right end of the objective strip**. The strip is a label in
every chapter, so covering its far end costs nothing. This is a shell file: it affects all seven chapters.

### The route board was not Chapter 3's mechanic

The first attempt pre-placed every step as a fixed card in a vertical stack and only asked the player to
join them. That is not what Chapter 3 does, and the user was explicit with a screenshot: in Chapter 3
you pick a card from the **palette**, it appears on a free **canvas**, and *then* you connect the dots.

Rebuilt on Act 3's actual model:

- **Palette (left)** — the steps this visitor could use. A step leaves the palette once it is on the map.
  `FinalChapterRoutePaletteDragView` releases onto the board; dropped anywhere else, nothing happens.
- **Map (centre)** — a free canvas with a wire layer under a node layer, cards positioned in 0..1 of the
  board rect exactly as `Act3DialogGraphStaticPresenter` does it. Cards move freely
  (`FinalChapterRouteCardDragView`), which is how a tangled route gets untangled. Dragging follows the
  pointer directly and redraws wires live — `MoveRouteStep` deliberately does **not** raise
  `StateChanged`, because re-rendering mid-drag would destroy the card under the player's finger.
- **Bin** — a strip under the map, highlighting while a card is held over it. Removing a card removes
  every wire touching it; a wire to a card that is gone is a leftover, not a route.
- **Read-out (right)** — the wires read back as a numbered route, plus "Clear the map".

The two ends (`route_start`, `route_end`) are seeded onto the map and can be moved but never binned.
A wire is refused unless both cards are actually on the map.

The validator did not change: it still walks the wires and compares against the authored
`ExpectedRouteStepIds`. Only how the player produces those wires changed.

New files this round: `FinalChapterRoutePaletteDragView.cs`, `FinalChapterRouteCardDragView.cs`.
`IFinalChapterWireHost` gained the three card methods.

### Test results after the follow-up

**Run: the same standalone harness, 117 checks, still all passing** — the puzzle layer is untouched by
this round, so this is a regression check rather than new coverage. Four controller-level tests were
added for the board (a step is only on the map once placed; binning a card takes its wires; the two ends
cannot be binned; redrawing a wire moves it rather than forking).

EditMode attribute count is now **153 `[Test]` + 26 `[TestCase]` = 179 discrete cases**.

Still not run: compile, EditMode, Play Mode. The layout repair round is now more likely, not less — the
map's card size (196x70), seeded positions and clamp bounds are all guesses about a board whose real
pixel size Claude has never seen.

## 8. A stale test surfaced by the first EditMode run in several sessions

`LaterChapterHintContextTests.LaterChaptersProvideCurrentStateWithoutAnswerKeys` failed:

```
Expected: String containing "Threshold=30"
But was:  "Lily band below 10; answer band from 90; rephrase attached=False; Lily attached=False.
           The evening has not been run."
```

**Not caused by this task.** Chapter 4 stopped being a single threshold slider when it was rebuilt into
two handles the player positions (`HandoffEdge`, `AnswerEdge`); `Act4ConfidenceInteractionController`
was updated, the test was not. It went unnoticed because EditMode had not been run since — run 001
ended blocked on a compile error.

Fixed by asserting the contract instead of the number: the context has to report where the two handles
currently sit, read off the controller rather than hard-coded, so retuning the opening positions cannot
fail it the same way again. The band edges are the player's current settings, not an answer key, so the
test's stated purpose still holds.

The other three assertions in that test were checked by hand against fresh controllers and still pass
(`act5` "not been run", `act6` "Data source=empty", `finalChapter` "Main stages: empty"). A scan of
`Assets/Tests` for the same rebuilt-away vocabulary (Threshold / slider / band) found nothing else.
`ShellReturnToHubOverlayTests` was re-read after the cheat-button move — it asserts on source text and
scene mapping, not on the button's position, so it is unaffected.

Test count is unchanged at 179; this edited assertions inside an existing test rather than adding one.

## STAR 摘要

- **S 情境**：M0-T56 首次實作後仍留一個編譯錯誤，且使用者對終章提出五點回饋。
- **T 任務**：修好編譯、把訪客二三的對話改成第三章的連連看、讓 Lily 那關像真的對話、用「沒說」卡片取代留白答案、在結局 murmur 加上握拳的括號描述。
- **A 行動**：把路線狀態從有序清單改為可走訪的連線（分支／死路／迴圈／離線連線各有錯誤訊息），Lily 新增對話關卡並改為左右對望的對話版面，新增 `IsUnknownMarker` 卡片並對三位訪客都提供。使用者試玩後回報兩點，同一輪再修：TEST PASS 按鈕從左下角（各章的 palette 欄位）移到右上角，路線關卡改成第三章真正的做法——左邊 palette 拖到中間自由畫布，卡片可任意移動、連點成線、拖到垃圾桶移除。
- **R 結果**：獨立 .NET harness 117 項檢查全數通過（含最重要的可解性檢查）；EditMode 測試由 20 增至 30，全專案 179 個案例。Unity 端（編譯／EditMode／Play Mode）尚未執行，待使用者驗證。
