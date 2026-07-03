# M0-T37 - Run 001 - Act 2 entity teaching

## Task ID

M0-T37

## Run Number

001

## Date

2026-07-03

## Original Request / Codex Prompt Summary

Strengthen Act 2 (Entity Extraction) so the existing chip tagging / entity typing mechanic teaches
entity extraction / NER, entity kinds, system vs custom entities, synonyms, and the word-token
connection to tokenization. Keep the deterministic Act 2 validator, session, sample data, answer keys,
span boundaries, and scene builder unchanged. Update learning/code/test docs and return a Claude
review / closure prompt.

## Files Created

- `Docs/codex_runs/M0-T37_001_act2_entity_teaching.md`

## Files Modified

- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs`
- `Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionInteractionController.cs`
- `Docs/LEARNING_CONTENT.md`
- `Docs/CODE_WALKTHROUGH.md`
- `Docs/UNITY_TEST_CHECKLIST.md`

## Tests or Checks Run

- `dotnet build Ghost.Presentation.csproj --no-restore`
- `git diff --check -- Assets\Presentation\Act2EntityExtraction\Act2EntityExtractionStaticPresenter.cs Assets\Presentation\Act2EntityExtraction\Act2EntityExtractionInteractionController.cs Docs\LEARNING_CONTENT.md Docs\CODE_WALKTHROUGH.md Docs\UNITY_TEST_CHECKLIST.md Docs\codex_runs\M0-T37_001_act2_entity_teaching.md`
- `git diff --name-only -- Assets\Scripts\Puzzles\EntityExtraction Assets\Presentation\Act2EntityExtraction\Editor`
- `rg -n "[^\x00-\x7F]" Assets\Presentation\Act2EntityExtraction\Act2EntityExtractionStaticPresenter.cs Assets\Presentation\Act2EntityExtraction\Act2EntityExtractionInteractionController.cs`

## Test / Check Result

- `dotnet build Ghost.Presentation.csproj --no-restore` succeeded with 0 errors and 8 warnings. The warnings were existing Unity API deprecation warnings (`CS0618`) around `FindFirstObjectByType` / `FindObjectsByType` usage, including the existing Act 2 event-system helper pattern.
- `git diff --check` reported no whitespace errors. Git printed CRLF conversion warnings for edited files.
- The prohibited-path diff check returned no files for `Assets\Scripts\Puzzles\EntityExtraction` or `Assets\Presentation\Act2EntityExtraction\Editor`.
- The non-ASCII scan of changed Act 2 C# files returned no matches.
- Unity Play Mode: Not run — Unity Editor is not available in this Codex session.

## Errors Encountered

- No implementation errors encountered.
- The working tree contains concurrent / pre-existing modified files outside this run's scope, including shelved scene side effects and Claude planning/doc sync files. They were observed and left untouched.

## Fixes Applied

- Added a compact runtime-created `Lily's Entity Note` panel to the Act 2 presenter.
- Changed Act 2 wording from generic chips/types to word tokens and entity kinds.
- Extended entity-kind legend subtitles so `time` teaches System entity, `room` and `object` teach Custom entities, and `room` surfaces the real sample-data `lab` / `laboratory` synonym pair.
- Replaced only the correct validation message with a compact teaching beat about Ghost noticing details, NER, synonyms, tokenization, Act 1 intent, and Act 3 slots.
- Kept incorrect validation wording, hint request behavior, attempt logging, chip selection, assignment, untagging, and validation logic unchanged.
- Updated learning content, code walkthrough, and Unity checklist for the new Act 2 teaching layer.

## What Was Intentionally Not Changed

- `EntityExtractionValidator`, `EntityExtractionSession`, `Act2EntityExtractionSampleData`, entity spans, answer keys, and validation rules.
- `Assets/Presentation/Act2EntityExtraction/Editor/Act2EntityExtractionPrototypeSceneBuilder.cs`.
- Act 1, Act 3, fundamentals, backend scoring, ProjectSettings, Packages, Build Settings, `.meta` files, and scene YAML.
- Concurrent modified docs / shelved scene side effects not related to M0-T37.

## Remaining Risks

- Human Play Mode verification is still required to confirm the teaching panel, legend, validation feedback, validation strip, and ambient banter all fit a 1920x1080 Game view without cropping.
- The correct feedback is compact and uses a smaller font, but final readability should be checked in Unity.
- The `lab` / `laboratory` synonym wording is derived from sample data surfaces for the custom `room` type; if future sample data represents multiple custom room concepts, this teaching shortcut may need a richer synonym model.

## Next Recommended Step

Send the Claude review / closure prompt, then have the user or Claude perform the Unity Play Mode
checklist for M0-T37. If verified, Claude should close/archive M0-T37 and advance `Docs/CURRENT_TASK.md`
to M0-T38.
