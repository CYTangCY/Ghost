# Testing and Evaluation Evidence Basis

## Purpose

This document defines what each project test is intended to show. It prevents a passing test count
from being treated as a general measure of software quality or learning effectiveness.

The approach is requirements-based and risk-based. ISO/IEC 25010:2023 is used only to select relevant
quality areas, while the ISO/IEC/IEEE 29119 series provides the principle that test processes,
test-design techniques, and test records should be connected. Unity's Test Framework documentation
is used to distinguish tests that run in Edit Mode from behaviour exercised in Play Mode.

Primary sources:

- ISO/IEC 25010:2023 product quality model:
  https://www.iso.org/standard/78176.html
- ISO/IEC/IEEE 29119 software-testing series:
  https://committee.iso.org/sites/jtc1sc7/home/projects/flagship-standards/isoiecieee-29119-series.html
- Unity Test Framework, Edit Mode and Play Mode:
  https://docs.unity3d.com/Packages/com.unity.test-framework@2.0/manual/edit-mode-vs-play-mode-tests.html

The complete standards are not being claimed as fully implemented. Their relevant principles are
adapted to the size of this prototype.

## What the current evidence can and cannot show

A successful C# build shows that the selected assemblies compile and link. It does not show that a
puzzle accepts the correct state or rejects a wrong one.

An EditMode test shows that one stated assertion holds for its prepared input in the tested build. A
failed test does not always stop the game from launching; it can reveal a wrong validator result,
lost state, or broken navigation rule in an otherwise executable build. Therefore, the value of the
test comes from the requirement or risk represented by the assertion, not from Unity displaying a
green result.

A Play Mode scenario can show that several implemented parts work together through the visible
interface. It does not by itself show that the interface is generally usable or enjoyable.

A participant study would be needed to measure learning, enjoyment, or accessibility for the target
players. Those claims are outside the current evidence.

## Evaluation questions

| ID | Evaluation question | Test basis | Main evidence | Claim allowed |
| --- | --- | --- | --- | --- |
| EQ1 | Do the puzzles enforce the intended chatbot and NLP operations? | FR2, FR3, FR8 and the chapter learning map | Validator, simulator, session, and authored-case tests | The implemented rules require the represented operation in the tested cases |
| EQ2 | Does the complete prototype follow the required chapter and completion flow? | FR1, FR4, FR6 and the chapter route | Structured Play Mode acceptance scenarios and navigation tests | The tested build completes the defined end-to-end scenarios |
| EQ3 | Does the system continue to work when optional services fail? | NFR5 | Backend/Ollama fault-injection scenarios and fallback route tests | The tested failure does not block puzzle play and produces the defined fallback |
| EQ4 | Does the local service store and return the required prototype data correctly? | FR7 and the REST/database design | API route tests using a temporary SQLite database | The tested routes preserve their stated request/response and persistence behaviour |
| EQ5 | Does generated Lily text remain inside the defined support limits? | FR5 and the authored hint policy | Repeated prompt-bank constraint checks | The model met or violated each stated output constraint at the reported rate |
| EQ6 | Is each course concept meaningfully connected to a puzzle action and visible Ghost result? | Research question, FR2-FR4, literature arguments | Curriculum-to-puzzle traceability review | The design contains the intended concept-action-feedback connection; no learning gain is claimed |

## Selected software-quality areas

Only quality areas that are both relevant and testable are retained.

### Functional suitability

This covers the correctness and completeness of the implemented prototype. The evidence comes from
requirements traceability, puzzle-rule tests, backend route tests, and end-to-end acceptance
scenarios.

### Reliability and fault tolerance

This is limited to the project's explicit graceful-degradation requirement. The relevant faults are
the backend being unavailable, Ollama being unavailable, and a request failing during play. General
long-term reliability cannot be claimed without endurance and deployment testing.

### Interaction quality

Developer Play Mode checks can identify visible faults such as an unreachable button, unreadable
text, or a drag target that cannot be used. A short cognitive walkthrough can examine whether the
goal, action, and feedback are discoverable. Neither method establishes general usability or
enjoyment without other users.

Maintainability is discussed through the separation of puzzle rules, presenters, and service
clients. It is not converted into an unsupported numerical quality score.

## Test-design methods

### Puzzle rules

Use equivalence partitions and boundary cases derived from each validator:

- a complete correct solution;
- an incomplete or empty solution;
- a wrong-role or wrong-category solution;
- duplicated, overlapping, or disconnected data where relevant;
- threshold values immediately below, at, and above a routing boundary;
- a solution changed after a previous result;
- an unknown or malformed identifier where the public method accepts external input.

The acceptance criterion is not simply that the suite passes. Every validator rule stated in the
chapter design must have at least one positive and one negative test condition, with boundary cases
where ordering, range, or span limits exist.

### End-to-end prototype flow

Use named Play Mode scenarios rather than one long generic checklist:

1. start a new profile, complete Chapter 0, and reach the hub;
2. enter and leave an unfinished teaching chapter without completing it;
3. complete each teaching chapter through its accepted route;
4. submit one meaningful wrong state in each chapter and observe Ghost and Lily's response;
5. complete the Final Chapter and reach the ending;
6. skip the ending and confirm the same completion state;
7. repeat the service-dependent path with the backend unavailable;
8. repeat Ask Lily with Ollama unavailable.

Each scenario record must contain the tested build, environment, initial state, actions, expected
result, actual result, pass/fail result, and evidence filename. The display resolution is part of the
environment record, not a quality measure by itself.

### Graceful degradation

Use controlled fault injection:

- backend unavailable before the game starts;
- backend becomes unavailable after local puzzle state exists;
- backend available while Ollama is unavailable.

For each case, the acceptance criteria are:

- local puzzle interaction and validation remain available;
- a service error does not alter the validator result;
- Ask Lily uses the authored fallback when generation is unavailable;
- the failure is visible in a log without exposing an answer key;
- the player can continue or return to the hub.

### Backend and database

Route tests use a temporary database and verify response status, response shape, stored state, and
the next read. Missing or duplicate data must be tested where the route defines that condition.
These tests do not establish security, concurrency, or public-scale performance.

### LLM output constraints

The previous five-area 0-2 aggregate should not be reused as one overall percentage. In the existing
CSV, `voice_format` is scored 0/1 while the report describes every area as 0-2. Each item therefore
has a maximum of 9, not 10, and the `10/54` voice percentage uses the wrong denominator. More
importantly, adding ordinal judgements into one percentage hides safety faults.

The replacement reports separate criteria:

- state and course relevance;
- technical correctness;
- no exact solution or answer leakage;
- appropriate strength for the requested hint level;
- maximum word length;
- Lily speaks in character and directly to the player;
- no invented project fact.

Technical correctness, state relevance, and no answer leakage are mandatory gates. One leaking or
technically wrong output is recorded as a safety fault rather than being cancelled out by a strong
voice score. Hint-level fit, length, and character voice are reported as separate conformance rates.
Raw outputs and the reason for every failure must remain available.

The next prompt bank should cover Chapters 1-6 and the Final Chapter. At minimum, it should use one
known failure state per chapter, all three hint levels, and three repeated generations. This produces
63 outputs and keeps the earlier repeat structure while covering the complete game.

## Current evidence audit

### Unity

The latest complete run (2026-08-03, Unity Test Runner) contains **153 passing EditMode tests** across
22 fixtures:

| Area | Tests | What it evidences |
| --- | ---: | --- |
| Validators, simulators, sessions, authored data | ~66 | EQ1 — the puzzle rules require the represented NLP operation |
| Chapter flow, completion, navigation, Return-to-Hub | ~41 | EQ2 — the route and completion rules hold |
| Interaction controllers (state transitions) | ~23 | Chapter state machines behave as specified |
| Engines and authored outcomes | ~14 | The authored cases resolve as written |
| Presenter construction | ~9 | Structural regression only — see the caveat below |

`153/153` is **not** a coverage figure. No code-coverage result is recorded, and most visible
interaction is not executed by these tests.

### What the assertions actually encode

These tests were written for this project; nothing about the framework dictates their content. Most do
not check "does the function return the right value" — they encode **a design rule, expressed as a
specific way a player can be wrong**. That is what makes them usable as evidence for the research
question, which asks how course concepts can be turned into playable mechanics.

Representative examples:

| Assertion | Player misconception it pins down |
| --- | --- |
| `Validate_WhenIntentIsSplitAcrossGroups_ReturnsIncorrect` | Grouping too finely — one purpose spread over two piles |
| `Validate_WhenBoundaryMatchesButTypeIsWrong_ReturnsIncorrect` / `WhenTypeMatchesButBoundaryIsWrong` | Selecting the right words but the wrong entity type, and vice versa — deliberately separated, because they are different errors |
| `Simulate_WhenGraphCycles_StopsAtStepCap` | Wiring an infinite loop |
| `Evaluate_WhenRelatedCardsMajorityInWrongLabelledPile_ReturnsWrongReplyAndMisleadingCards` | A mis-sorted pile produces a visibly wrong Ghost reply, and the offending cards are named |
| `AlwaysAnswerFailsOnlyTheLowConfidenceVisitor`, `KeywordGuessFailsTheLabHoursWording` | Degenerate strategies fail on specific visitors, so the chapter distinguishes understanding from guessing |
| `CorrectOptionPositionsVaryAcrossVisitorsAndCategories` | Answering from remembered position rather than comprehension |

Two patterns are stronger evidence than a passing count and should be cited as such:

1. **Discrimination pairs.** `BuggyGraphFailsEveryAuthoredConversation` together with
   `FixedGraphPassesEveryAuthoredConversation` shows the validator separates a correct artefact from a
   faulty one. A test that only shows "no crash" cannot support that claim.
2. **Design properties encoded as tests.** `EveryVisitorFlips` (Act 4) asserts that no visitor's
   outcome can stay constant anywhere in the control's range — i.e. that the control is not decorative.
   It exists because the previous Act 4 had an acceptable band of 65-80 with **no visitor score inside
   it**, so the dial could not change any outcome while every test stayed green.

### Known gaps in this suite

Reporting these is more credible than claiming breadth.

- **Act 4 has only 6 validator tests** despite being the most conceptually complex chapter.
- **The Act 1 generalisation engine has 4 tests**, though it is the core teaching mechanism.
- **Presenter tests are weak evidence.** `PresenterUsesCompactSliderHandleAndExplainsThresholdReason`
  asserts a handle is at most 12px; that says nothing about whether the layout is usable. Treat these
  as structural regression guards, not interface-quality evidence.
- No participant data, so no claim about learning, enjoyment, or accessibility.

### Automated verification passed while the artefact was unusable

Recorded because it bounds what this evidence can claim.

During M0-T51, a verification run reported every automated gate green — 147/147 EditMode, a
nine-scene Windows build, and a clean installer install/launch/uninstall — while the running game was
not usable: Chapter 3's node palette printed its titles on top of their own descriptions, and Chapter
5's wires could not be grabbed. The defects were invisible to the suite because they were layout and
hit-testing faults, and the screenshot evidence that should have caught them was captured on the
onboarding screen and omitted the two affected chapters.

The evidence chain is in the repository: the run log claiming all gates passed, the user's in-level
screenshots showing the breakage, and the subsequent repair runs.

**Consequence for this project's evaluation:** rule-level automated tests and human interface
acceptance are complementary, not substitutable. Automated results bound what can be claimed about
rules; they say nothing about whether the interface can be operated. Manual acceptance evidence must
therefore be captured *inside* each level and must cover every scene, and it should be recorded with
the same discipline as the automated runs (see "Evidence record format").

### Backend

The 10 route tests cover content, progress, account/profile behaviour, attempt logs, model success,
and static fallback paths. They do not test security hardening, concurrent writers, load, or a public
deployment.

### Play Mode

The current report records a developer-completed end-to-end check. The evidence needs to be rebuilt
as named scenarios with individual expected and actual results. A screenshot or short recording
should be attached to the most important scenarios, especially wrong feedback, graceful degradation,
Final Chapter completion, and the ending.

### Granite

The existing 27-output sample covers only Chapters 1-3 and was scored by one developer. It already
shows answer leakage, inaccurate content, weak level separation, excessive length, and out-of-character
text. The raw outputs remain useful, but the aggregate `60.4%` should be removed. The data should be
re-coded as criterion-level pass/fail or fault counts before being used in the dissertation.

## Evidence record format

Every new manual or automated evidence item should use these fields:

| Field | Meaning |
| --- | --- |
| Evidence ID | Stable identifier such as `PV-04` or `LLM-FINAL-L2-R3` |
| Claim / requirement | The exact requirement or risk being checked |
| Test condition | The state or equivalence class represented |
| Build and environment | Commit/hash, Unity/backend version, OS, resolution where relevant |
| Input / actions | Reproducible steps |
| Expected result | Defined before reading the actual result |
| Actual result | What occurred |
| Result | Pass, fail, or blocked |
| Artifact | XML, log, screenshot, video, CSV row, or database record |
| Limitation | What the evidence does not establish |

## Smallest useful additional evidence

1. Freeze one evaluation build and record its identifier and environment.
2. Create the requirement-to-evidence matrix for FR1-FR8 and NFR5.
3. Replace the generic Play Mode checklist result with the eight named acceptance scenarios.
4. Run the three graceful-degradation fault scenarios.
5. Re-code the existing 27 Granite outputs without the aggregate percentage.
6. Extend the prompt bank to Chapters 4-6 and the Final Chapter.
7. If time remains, carry out a cognitive walkthrough of Chapter 1, Chapter 3, and the Final Chapter.

The participant study remains future work unless suitable approval, recruitment, consent, and study
measures are available. It should not be simulated through developer testing.
