# Test Requirement Traceability Matrix

## Status meanings

- **Covered:** the current evidence directly checks the stated requirement.
- **Partial:** relevant evidence exists, but one important layer or condition is missing.
- **Missing:** no adequate evidence record currently exists.
- **Review only:** this is assessed through design/source review rather than a dynamic software test.

## Functional and system requirements

| Requirement | Test basis and acceptance criterion | Current evidence | Status | Required next evidence |
| --- | --- | --- | --- | --- |
| FR1: Each chapter starts from a Ghost communication problem | Every teaching chapter and the Final Chapter must introduce a concrete Ghost problem before the main interaction becomes available | Story and chapter content exist; no single traceability record checks every current chapter opening | Partial | `CR-01`: review each chapter opening and record the Ghost problem, screenshot, and transition into play |
| FR2: Each main puzzle maps to a chatbot/NLP concept | The required player operation, validator rule, and visible result must correspond to the stated concept | Internal curriculum map plus validator and simulator source | Partial | `CR-02`: concept-action-validator-feedback table for Chapters 1-6, reviewed against IBM course content |
| FR3: Learning occurs through a puzzle rather than only dialogue or quiz | A chapter cannot complete by reading text or selecting an unrelated prepared answer; the accepted state must require the represented operation | Validator tests show accepted states; interaction form has been reviewed by the developer | Partial | Add the completion route and the easiest possible shortcut to `CR-02`; fail the review if the concept can be bypassed |
| FR4: Player actions visibly change Ghost's response | At least one correct and one meaningful wrong state in every teaching chapter must produce different Ghost output before completion | Controllers contain authored reactions; existing developer Play Mode claim is not recorded by chapter | Partial | `PV-C1` to `PV-C6`: capture one wrong and one correct Ghost response in each chapter |
| FR5: Lily hints without directly solving the puzzle | Static and generated hints must remain relevant, technically correct, and must not state the exact required placement, wire, threshold, or card order | Static fallback exists; 27 Granite outputs reveal two exact-answer faults; later context tests cover state transfer | Failed for current Granite sample; partial for static path | Re-code the 27 outputs as fault counts; run the seven-chapter prompt bank; record any leak as a safety failure |
| FR6: The prototype is playable and manually testable | A new profile must reach Chapter 0, the hub, Chapters 1-6, the Final Chapter, and the ending through visible controls | Developer reports a completed Play Mode route; no structured evidence set | Partial | Run `PV-01` to `PV-08` using the frozen evaluation build |
| FR7: Backend, database, and LLM layers are integrated | Content, progress, attempts, hints/chat, static fallback, and stored records must travel through the implemented routes | 10 backend route tests cover the local service and temporary SQLite database | Partial | Add one Unity-to-backend integration scenario and record the database/log result; separate live-model and fallback runs |
| FR8: Deterministic rules decide puzzle correctness | Correct and incorrect states must be decided by validators/simulators; model output must not alter the result | 83 logic-oriented Unity tests, no backend scoring route, and current controller structure | Covered for tested rules | Complete the per-validator condition matrix so `83` is traceable to rules rather than presented as a quality score |

## Non-functional requirements

| Requirement | Test basis and acceptance criterion | Current evidence | Status | Required next evidence |
| --- | --- | --- | --- | --- |
| NFR1: Explainability | Every major script has one stated responsibility and its role can be followed from input to result | `CODE_WALKTHROUGH.md` | Review only | Sample the scripts used in one complete chapter and one service path; record missing or outdated explanations |
| NFR2: Simplicity | The system avoids the prohibited architecture mechanisms and keeps puzzle rules separate from presentation | Architecture and source review | Review only | Use a short architecture checklist; do not create a numerical simplicity score |
| NFR3: WebGL compatibility | The selected evaluation build completes a WebGL build and the main route works in a named browser | Web-compatible coding constraint only; no current browser-build evidence | Missing | `WEB-01`: build WebGL; `WEB-02`: run opening, one puzzle, fallback, hub return, and Final route in the chosen browser |
| NFR4: Documentation | Required implementation and test records exist and match the frozen evaluation build | Walkthrough, checklist, run logs, architecture documents | Partial | Freeze the build first, then audit documentation against that identifier |
| NFR5: Graceful degradation | Backend/Ollama failure must not block local puzzle play or change deterministic correctness | Backend route fallback tests; no structured Unity fault-injection record | Partial | Run `FT-01`, `FT-02`, and `FT-03` below |

## Automated puzzle-rule condition matrix

The current Unity result has 96 passing tests. The number is reported only as execution evidence. The
following condition classes define whether the puzzle-rule coverage is meaningful.

| Area | Existing fixtures | Required condition classes | Current assessment |
| --- | --- | --- | --- |
| Chapter 1 intent | `IntentClassificationValidatorTests`, `IntentClassificationSessionTests`, sample-data and generalisation fixtures | correct grouping; missing message; duplicate use; mixed intent; unknown label; unseen wording | Covered by current tests, pending line-by-line traceability confirmation |
| Chapter 2 entities | validator, session, sample-data, and outcome fixtures | correct span/type; missing entity; wrong type; overlap; duplicate span; boundary token positions; synonym/custom entity | Covered by current tests, pending line-by-line traceability confirmation |
| Chapter 3 dialogue | validator, simulator, and session fixtures | complete route; missing node/edge; wrong branch; cycle/invalid connection where prohibited; known and missing entity simulation | Covered by current tests, pending line-by-line traceability confirmation |
| Chapter 4 confidence | `Act4ConfidenceValidatorTests` | below, at, and above threshold; fallback; handoff; missing route | Five tests exist; exact boundary-value coverage must be confirmed |
| Chapter 5 repair/testing | `Act5TestSuiteRunnerTests` and presenter smoke test | known failing graph; partial repair; correct repair; stale rerun; visible board construction | Rule cases covered; stale-result and visible interaction need Play Mode evidence |
| Chapter 6 backend response | `Act6BackendResponseValidatorTests` | correct source/action/response; empty role; each wrong role; neutral before Run; rerun after change | Eight rule tests exist; drag/click interaction needs Play Mode evidence |
| Final Chapter | `Act6PipelineValidatorTests` and presenter smoke test | correct complete route; empty/duplicate/swapped stages; each shortcut's affected cases; missing/wrong backend action; stale results | Eleven tests exist; free-form interaction and Lily response need Play Mode evidence |
| Shell/progress | `ShellReturnToHubOverlayTests` | supported scene set; Return to Hub never completes; final completion route remains separate | Source/navigation guards covered; end-to-end persistence needs Play Mode evidence |

## Named Play Mode acceptance scenarios

| ID | Initial state and action | Acceptance criterion | Evidence to retain |
| --- | --- | --- | --- |
| PV-01 | Start with a new profile and finish Chapter 0 | Opening precedes hub; Chapter 0 completes only at the story end | Result row plus opening/hub screenshots |
| PV-02 | Enter every unfinished teaching chapter and use Return to Hub | Hub loads; chapter remains incomplete; no success debrief occurs | One result row per scene or a continuous recording |
| PV-C1 | Submit one wrong and one correct Chapter 1 grouping | Ghost responses differ; only the correct grouping enables completion | Wrong/correct screenshots |
| PV-C2 | Submit one wrong and one correct Chapter 2 entity mapping | Wrong detail produces a different outcome; correct authored cases complete | Wrong/correct screenshots |
| PV-C3 | Run one wrong and one correct Chapter 3 graph | Simulation follows the connected route; completion requires the accepted graph | Graph plus output screenshots |
| PV-C4 | Exercise threshold/fallback boundary states in Chapter 4 | Clear, fallback, and handoff cases follow the defined routes | Result table or recording |
| PV-C5 | Run the faulty, partly repaired, and fully repaired Chapter 5 graph | Expected/actual results change with the real graph and become current only after rerun | Three result screenshots |
| PV-C6 | Run wrong-role and correct Chapter 6 backend chains | Roles remain neutral before Run; wrong roles fail; correct chain produces the authored reply | Before/after result screenshots |
| PV-FINAL | Run a partly wrong and correct Final Chapter route | Visitor cases fail selectively; changed results become stale; all pass before ending | Test cards plus ending evidence |
| PV-END | Complete and separately skip the ending | Both paths complete only the Final Chapter and return to the title route | Two result rows |

## Fault-injection scenarios

| ID | Injected fault | Acceptance criterion |
| --- | --- | --- |
| FT-01 | Backend is unavailable before game start | New local play remains possible; validators and hub navigation work; failed sync is logged |
| FT-02 | Backend becomes unavailable after puzzle state changes | Current puzzle state and validator result remain usable; player can continue or return to hub |
| FT-03 | Backend is available but Ollama is unavailable | Ask Lily returns the authored static fallback; the chat request is logged; puzzle state is unchanged |

## LLM evidence identifiers

Use `LLM-{CHAPTER}-{LEVEL}-R{REPEAT}`, for example `LLM-C4-L2-R3`.

The minimum complete prompt bank is:

- Chapters 1-6 and Final Chapter;
- one known incorrect puzzle state per chapter;
- hint levels 1, 2, and 3;
- three repeated generations per state and level;
- 63 raw outputs in total.

Each row records these independent results:

| Criterion | Result rule |
| --- | --- |
| Relevant to current state | Pass only if the advice uses the supplied chapter/state rather than a generic or invented case |
| Technically correct | Pass only if every technical claim is consistent with the authored concept and puzzle |
| No exact answer | Pass only if no required card, slot, wire, threshold, or complete order is stated |
| Hint level | Pass if the strength matches the predefined level description |
| Length | Pass if the final displayed text meets the stated word limit |
| Lily voice | Pass if Lily speaks directly to the player in the defined character voice |
| No invented fact | Pass if no unsupported project, story, or visitor detail is introduced |

Relevance, technical correctness, no exact answer, and no invented fact are mandatory. Report each
criterion as `passes / total` and list the fault examples. Do not combine the criteria into one total
percentage.
