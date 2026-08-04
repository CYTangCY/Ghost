# M0-T49 — Run 005 — chapter build-out remediation pass

## Task ID

M0-T49

## Run Number

005

## Date

2026-07-16

## Original Request / Codex Prompt Summary

Fix the confirmed chapter build-out findings without adding gameplay features: make Return to Hub pure navigation, add the missing validated Act 2 completion action, make Chapter 0 Lily visible, fit the Shell hub inside the 1920x1080 body, make Chapter 6 correctness appear only after deterministic validation, remove the filled-socket double-click action, and reconcile the current Unity checklist. The user additionally approved replacing the old Lily drawing in Chapter 0 and Final Chapter with an original pixel character based only on the mood of four references, then refined the design to a high blonde ponytail, black blazer, red KCL lanyard, and brown British-style leather shoes.

## Files Created

- Assets/Resources.meta (Unity-generated)
- Assets/Resources/Characters.meta (Unity-generated)
- Assets/Resources/Characters/LilyPixelFullBody.png
- Assets/Resources/Characters/LilyPixelFullBody.png.meta (Unity-generated)
- Assets/Resources/Characters/LilyPixelPortrait.png
- Assets/Tests/EditMode/ShellReturnToHubOverlayTests.cs
- Docs/codex_runs/M0-T49_005_remediation_pass.md
- Docs/CLAUDE_REVIEW_PROMPT_M0_T49_RUN005.md (created after verification summary)

## Files Modified

- Assets/Presentation/Shell/ShellReturnToHubOverlay.cs
- Assets/Presentation/Act2EntityExtraction/Act2EntityExtractionStaticPresenter.cs
- Assets/Presentation/Shell/Editor/GameShellSceneBuilder.cs
- Assets/Presentation/Characters/LilyPixelPortraitFactory.cs
- Assets/Presentation/Story/Chapter0StoryPresenter.cs
- Assets/Scripts/Puzzles/BackendResponse/Act6BackendResponseModels.cs
- Assets/Scripts/Puzzles/BackendResponse/Act6BackendResponseValidator.cs
- Assets/Presentation/Act6BackendResponse/IAct6BackendInteractionHost.cs
- Assets/Presentation/Act6BackendResponse/Act6BackendInteractionController.cs
- Assets/Presentation/Act6BackendResponse/Act6BackendCardDragView.cs
- Assets/Presentation/Act6BackendResponse/Act6BackendSlotDropView.cs
- Assets/Presentation/Act6BackendResponse/Act6BackendStaticPresenter.cs
- Assets/Presentation/Act6VoicePipeline/Act6PipelineStaticPresenter.cs
- Assets/Presentation/Act6VoicePipeline/Act6EndingSequence.cs
- Assets/Tests/EditMode/Act6BackendResponseValidatorTests.cs
- Docs/CODE_WALKTHROUGH.md
- Docs/UNITY_TEST_CHECKLIST.md

## Tests or Checks Run

- Read CURRENT_TASK, LEARNING_CONTENT, the required project context documents, and M0-T49 run 004 before editing.
- Built Ghost.Runtime.csproj with dotnet build.
- Built Ghost.Presentation.csproj with dotnet build after all presentation and Lily loader changes.
- Built Ghost.EditModeTests.csproj with dotnet build after adding the new tests.
- Built Ghost.Presentation.Shell.Editor.csproj, Ghost.Presentation.Act6Backend.Editor.csproj, Ghost.Presentation.Act6.Editor.csproj, and Ghost.Presentation.Story.Editor.csproj.
- Ran rg guards for all SetPendingDebriefAct / MarkActCompleted call sites, forbidden Return-overlay completion references, Chapter 0 Color.clear plus sprite assignment, and Chapter 6 UI-side expected-id access.
- Ran git diff --check on the affected source and documentation paths.
- Ran git status guards for delete/rename entries; none were reported.
- Scanned the affected C# files for non-ASCII characters; none were reported.
- Verified Docs/UNITY_TEST_CHECKLIST.md has exactly one current end-to-end checklist and marks Run 001 and Run 003 as historical/superseded.
- Validated LilyPixelFullBody.png with Pillow: 967x1626 RGBA, alpha extrema 0-255, transparent four corners, opaque bounding box (294, 87, 652, 1530), 1,253,403 transparent pixels, 7,491 partial-alpha pixels, and 311,448 opaque pixels.
- Validated LilyPixelPortrait.png with Pillow: 640x640 RGBA, alpha extrema 0-255, transparent four corners, and opaque bounding box (130, 72, 488, 635).
- Attempted GameShellSceneBuilder through Unity 6000.4.11f1 batchmode.

## Test / Check Result

- All seven dotnet build targets completed with 0 errors. Ghost.Presentation reported four pre-existing FindFirstObjectByType deprecation warnings; the other builds were clean.
- Completion audit: Chapters 1-6 set pending debrief only from their presenter Complete/success state; Chapter 0 does so from story finish; Final Chapter marks only FinalChapterId from Act6EndingSequence.
- ShellReturnToHubOverlay contains no SetPendingDebriefAct or scene-to-act completion helper.
- Shell 1920x1080 active hub budget is 588px inside the 664px body: 24px vertical padding + 36px spacing + 44px heading + 40px copy + 72px fundamentals + 52px story row + 240px two-row lesson grid + 40px narrative Continue + 40px Back to Title. Remaining headroom: 76px.
- Chapter 6 untested slots are neutral. Per-role success/failure is read from Act6BackendValidationResult only after RunRoute.
- Filled Chapter 6 socket click contract: one click returns the card to the palette; palette cards alone perform click-selection; pointer events are consumed.
- Image generation used the built-in image tool. The final project asset was derived from the approved second generated variant and chroma-key removal; a separate portrait crop was made from that same asset.
- Unity scene regeneration: Not run — Unity batchmode aborted because another Unity instance had D:/Code/Ghost open.
- Unity EditMode Test Runner: Not run — the open Unity Editor held the project lock, so a batchmode test run could not start.
- Unity interactive Play Mode: Not run — interactive verification remains a human Game view check and is documented in the single Run 005 checklist.

## Errors Encountered

- The Windows sandbox helper repeatedly failed with helper_unknown_error while applying deny-read ACLs. apply_patch, local image reference reads, and view_image were affected.
- The first dotnet build used --no-restore and failed because Unity had removed Temp/obj/Ghost.Runtime/project.assets.json.
- Built-in image generation could not read local reference paths through the ACL helper; using the four recent conversation images succeeded.
- The first generated Lily variant remained too close to the visual references. The user requested a more distinct high-ponytail design and approved the revised direction.
- The Unity batchmode Shell builder aborted because the project was already open in Unity.

## Fixes Applied

- Used exact-match, fail-fast PowerShell replacements only after apply_patch failed from the known ACL helper problem; no broad filesystem rewrites were used.
- Re-ran dotnet builds with normal local restore; all affected assemblies compiled.
- Removed the Return overlay debrief listener and deleted its now-unused scene-to-act mapping methods.
- Added an Act 2 Complete Act button visible only in Act2ErrandPhase.Complete.
- Set the Chapter 0 Lily Image tint to Color.white and switched it to the approved full-body Sprite.
- Reworked the Shell hub into a compact 588px budget without hiding content.
- Extended the existing Chapter 6 validator result with invalid role ids while preserving authored data, accepted answers, errors, first-broken role, and scoring.
- Removed the controller UI-side expected-card comparison; presenter correctness now comes from LastValidation only.
- Made a filled Chapter 6 socket click return exactly one card to the palette and consumed click/drop events.
- Added source-convention and focused controller/validator regression tests.
- Generated an original Lily pixel sprite, removed the chroma background, created a portrait crop, and loaded Unity Sprite sub-assets through LilyPixelPortraitFactory.
- Added Lily to the Final Chapter only during her closing line; Final completion logic was not changed.
- Marked older M0-T49 checklists historical and added one current end-to-end checklist.

## What Was Intentionally Not Changed

- No existing Act 1-3 scene or user/agent work was reverted.
- No .meta file was renamed, deleted, or hand-edited.
- No scene YAML was hand-edited.
- No ProjectSettings file was edited during run 005.
- No existing puzzle answer, sample data, session, or scoring rule was changed. The Chapter 6 validator output was only extended to expose validator-owned per-role status.
- No LLM path was allowed to score or gate completion.
- CURRENT_TASK, ROADMAP, HANDOFF_LOG, and completed-task archives were not advanced; Claude remains responsible for review and closure.

## Remaining Risks

- The Shell, Chapter 0, Chapter 6, and Final scenes still need regeneration after the open Unity Editor is closed.
- The new ShellReturnToHubOverlayTests.cs and LilyPixelPortrait.png meta files will be generated on the next Unity import; they were not hand-authored.
- The complete Unity EditMode suite, expected to be at least 80 tests, has not run in this session because of the project lock.
- Pointer ordering, Chapter 0/Final Lily visual framing, 1920x1080 fit, Act 2 completion navigation, Act 3 wire regression, all immediate Return cases, and Final full/skip ending still need the human Play Mode checklist.

## Next Recommended Step

Save and close the currently open Unity Editor. Then run the Chapter 0, Chapter 6 Backend Response, Final Chapter, and Game Shell builders in that order; run the focused Chapter 6/Return tests and complete EditMode suite; inspect builder/test logs; then complete the single M0-T49 Run 005 human Play Mode checklist before Claude decides closure.
