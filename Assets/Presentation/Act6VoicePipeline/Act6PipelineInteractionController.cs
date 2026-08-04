using System;
using System.Collections.Generic;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Banter;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.VoicePipeline;

namespace Ghost.Presentation.Act6VoicePipeline
{
    public sealed class Act6PipelineInteractionController
    {
        private readonly string[] mainSlots = new string[5];
        private readonly IReadOnlyList<Act6PipelineComponent> paletteComponents;
        private readonly IReadOnlyList<Act6PipelineTestCase> testCases;
        private string backendComponentId = string.Empty;

        public Act6PipelineInteractionController()
        {
            paletteComponents = Act6PipelineData.CreatePaletteComponents();
            testCases = Act6PipelineData.CreateTestCases();
            CurrentPhase = Act6PipelinePhase.Onboarding;
            CurrentMood = Act6GhostMood.Neutral;
            CurrentTestIndex = -1;
            CurrentTraceIndex = -1;
            StatusLine = "Ghost has every repaired skill, but shortcuts are tangled into the voice path.";
        }

        public event Action StateChanged;

        public IReadOnlyList<Act6PipelineComponent> PaletteComponents => paletteComponents;

        public IReadOnlyList<Act6PipelineTestCase> TestCases => testCases;

        public Act6PipelinePhase CurrentPhase { get; private set; }

        public Act6GhostMood CurrentMood { get; private set; }

        public Act6PipelineValidationResult LastValidationResult { get; private set; }

        public bool ResultsAreStale { get; private set; }

        public bool BackendAttached => !string.IsNullOrWhiteSpace(backendComponentId);

        public bool IsPipelineReadyToTest
        {
            get
            {
                for (var index = 0; index < mainSlots.Length; index++)
                {
                    if (string.IsNullOrWhiteSpace(mainSlots[index]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool CanEditBackendForCurrentVisitor =>
            CurrentPhase == Act6PipelinePhase.VisitorTesting &&
            ActiveTestCase != null &&
            ActiveTestCase.RequiresBackend;

        public string BackendComponentId => backendComponentId;

        public string SelectedComponentId { get; private set; }

        public int CurrentTestIndex { get; private set; }

        public int CurrentTraceIndex { get; private set; }

        public int CompletedTestCount { get; private set; }

        public bool CurrentVisitorReplyShown { get; private set; }

        public string StatusLine { get; private set; }

        public Act6PipelineTestCase ActiveTestCase =>
            CurrentTestIndex >= 0 && CurrentTestIndex < testCases.Count
                ? testCases[CurrentTestIndex]
                : null;

        public Act6PipelineTestResult ActiveTestResult =>
            LastValidationResult != null &&
            CurrentTestIndex >= 0 &&
            CurrentTestIndex < LastValidationResult.TestResults.Count
                ? LastValidationResult.TestResults[CurrentTestIndex]
                : null;

        public Act6PipelineTraceStep ActiveTraceStep
        {
            get
            {
                var result = ActiveTestResult;
                return result != null &&
                    CurrentTraceIndex >= 0 &&
                    CurrentTraceIndex < result.TraceSteps.Count
                        ? result.TraceSteps[CurrentTraceIndex]
                        : null;
            }
        }

        public void BeginAfterOnboarding()
        {
            if (CurrentPhase != Act6PipelinePhase.Onboarding)
            {
                return;
            }

            CurrentPhase = Act6PipelinePhase.Configure;
            CurrentMood = Act6GhostMood.Confused;
            StatusLine = "Build one voice path. Three visitors will then try it in different ways.";
            ShowLilyReaction("Try one card at a time. I will watch what changes when each visitor reaches it.");
            NotifyStateChanged();
        }

        public void ReplayOnboarding()
        {
            if (CurrentPhase != Act6PipelinePhase.Configure)
            {
                return;
            }

            CurrentPhase = Act6PipelinePhase.Onboarding;
            CurrentMood = Act6GhostMood.Neutral;
            SelectedComponentId = string.Empty;
            StatusLine = "Ghost has every repaired skill, but shortcuts are tangled into the voice path.";
            NotifyStateChanged();
        }

        public string BuildHintContext()
        {
            var stages = new List<string>();
            for (var index = 0; index < mainSlots.Length; index++)
            {
                stages.Add(string.IsNullOrWhiteSpace(mainSlots[index])
                    ? "empty"
                    : Act6PipelineData.GetComponent(mainSlots[index]).Label);
            }

            var summary = "Main stages: " + string.Join(" -> ", stages) +
                ". Backend action: " +
                (string.IsNullOrWhiteSpace(backendComponentId)
                    ? "empty"
                    : Act6PipelineData.GetComponent(backendComponentId).Label) + ".";

            if (CurrentPhase == Act6PipelinePhase.VisitorTesting &&
                ActiveTestCase != null &&
                ActiveTraceStep != null)
            {
                return summary + " Current visitor: " + ActiveTestCase.VisitorMessage +
                    " Current stage result: " + ActiveTraceStep.Line;
            }

            if (LastValidationResult == null)
            {
                return summary + " The three visitors have not tested this route.";
            }

            if (ResultsAreStale)
            {
                return summary + " The board changed after the last visitors, so those results are stale.";
            }

            foreach (var result in LastValidationResult.TestResults)
            {
                if (!result.Passed)
                {
                    return summary + " First failed visitor: " + result.TestCase.VisitorMessage +
                        " Expected: " + result.TestCase.ExpectedReply +
                        " Actual: " + result.ActualReply + ".";
                }
            }

            return summary + " All three visitors received the expected reply.";
        }

        public string GetMainSlotComponentId(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < mainSlots.Length
                ? mainSlots[slotIndex]
                : string.Empty;
        }

        public bool IsComponentPlaced(string componentId)
        {
            if (Act6PipelineData.IsBackendComponent(componentId))
            {
                return string.Equals(backendComponentId, componentId, StringComparison.Ordinal);
            }

            return FindMainSlot(componentId) >= 0;
        }

        public bool IsMainSlotCorrect(int slotIndex)
        {
            var expected = Act6PipelineData.CreateMainPipelineOrder();
            return slotIndex >= 0 &&
                slotIndex < mainSlots.Length &&
                string.Equals(mainSlots[slotIndex], expected[slotIndex], StringComparison.Ordinal);
        }

        public bool IsMainSlotVerified(int slotIndex)
        {
            return LastValidationResult != null &&
                CurrentPhase != Act6PipelinePhase.VisitorTesting &&
                !ResultsAreStale &&
                IsMainSlotCorrect(slotIndex);
        }

        public bool IsBackendVerified()
        {
            return LastValidationResult != null &&
                CurrentPhase != Act6PipelinePhase.VisitorTesting &&
                !ResultsAreStale &&
                string.Equals(
                    backendComponentId,
                    Act6PipelineData.BackendActionId,
                    StringComparison.Ordinal);
        }

        public bool IsVisitorTestRunning(int testIndex)
        {
            return CurrentPhase == Act6PipelinePhase.VisitorTesting &&
                CurrentTestIndex == testIndex;
        }

        public bool IsTestResultVisible(int testIndex)
        {
            if (LastValidationResult == null ||
                testIndex < 0 ||
                testIndex >= LastValidationResult.TestResults.Count)
            {
                return false;
            }

            if (ResultsAreStale)
            {
                return true;
            }

            if (CurrentPhase == Act6PipelinePhase.VisitorTesting)
            {
                return testIndex < CompletedTestCount;
            }

            return CompletedTestCount >= LastValidationResult.TestResults.Count;
        }

        public Act6PipelineTraceStep GetVisibleTraceStepForComponent(
            string componentId)
        {
            if (CurrentPhase != Act6PipelinePhase.VisitorTesting ||
                ActiveTestResult == null ||
                string.IsNullOrWhiteSpace(componentId))
            {
                return null;
            }

            var lastVisibleIndex = Math.Min(
                CurrentTraceIndex,
                ActiveTestResult.TraceSteps.Count - 1);
            for (var index = 0; index <= lastVisibleIndex; index++)
            {
                var step = ActiveTestResult.TraceSteps[index];
                if (string.Equals(step.ComponentId, componentId, StringComparison.Ordinal))
                {
                    return step;
                }
            }

            return null;
        }

        public void SelectComponent(string componentId)
        {
            var canSelect = CurrentPhase == Act6PipelinePhase.Configure ||
                (CanEditBackendForCurrentVisitor &&
                    Act6PipelineData.IsBackendComponent(componentId));
            if (!canSelect ||
                string.IsNullOrWhiteSpace(componentId) ||
                !Act6PipelineData.IsKnownComponent(componentId))
            {
                return;
            }

            SelectedComponentId = string.Equals(
                SelectedComponentId,
                componentId,
                StringComparison.Ordinal)
                ? string.Empty
                : componentId;
            StatusLine = string.IsNullOrWhiteSpace(SelectedComponentId)
                ? "Selection cleared. Drag a card, or select one and then select a destination."
                : Act6PipelineData.GetComponent(SelectedComponentId).Label +
                    " selected. Now choose its destination.";
            ShowLilyReaction(string.IsNullOrWhiteSpace(SelectedComponentId)
                ? "No problem. Pick another card when you are ready."
                : GetCardReaction(SelectedComponentId));
            NotifyStateChanged();
        }

        public bool PlaceSelectedInMainSlot(int slotIndex)
        {
            return PlaceInMainSlot(SelectedComponentId, slotIndex);
        }

        public bool PlaceSelectedInBackendSlot()
        {
            return PlaceInBackendSlot(SelectedComponentId);
        }

        public bool PlaceInMainSlot(string componentId, int slotIndex)
        {
            if (CurrentPhase != Act6PipelinePhase.Configure ||
                slotIndex < 0 ||
                slotIndex >= mainSlots.Length)
            {
                return false;
            }

            if (!Act6PipelineData.IsMainPipelineComponent(componentId))
            {
                CurrentMood = Act6GhostMood.Confused;
                StatusLine = Act6PipelineData.IsBackendComponent(componentId)
                    ? "Backend actions belong in the side socket, not between the message and reply."
                    : "Select or drag a main-path card first.";
                ShowLilyReaction("That card belongs on the small backend route, not inside Ghost's main voice path.");
                NotifyStateChanged();
                return false;
            }

            var sourceIndex = FindMainSlot(componentId);
            if (sourceIndex == slotIndex)
            {
                SelectedComponentId = string.Empty;
                ShowLilyReaction("That card is already there. Trace what Ghost knows before and after it.");
                NotifyStateChanged();
                return true;
            }

            var displacedComponentId = mainSlots[slotIndex];
            mainSlots[slotIndex] = componentId;
            if (sourceIndex >= 0)
            {
                mainSlots[sourceIndex] = displacedComponentId;
            }

            MarkPlacementChanged(
                Act6PipelineData.GetComponent(componentId).Label +
                " placed in main stage " + (slotIndex + 1) + ".");
            ShowLilyReaction(
                GetCardReaction(componentId) +
                " You placed it at stage " + (slotIndex + 1) +
                "; check what information reaches it.");
            return true;
        }

        public bool PlaceInBackendSlot(string componentId)
        {
            var editingCurrentVisitor = CanEditBackendForCurrentVisitor;
            if (CurrentPhase != Act6PipelinePhase.Configure &&
                !editingCurrentVisitor)
            {
                return false;
            }

            if (!Act6PipelineData.IsBackendComponent(componentId))
            {
                CurrentMood = Act6GhostMood.Confused;
                StatusLine = "Only a backend action fits the side socket; learned message stages stay on the main path.";
                ShowLilyReaction("The side socket asks for data. Ghost's learned message skills belong on the main path.");
                NotifyStateChanged();
                return false;
            }

            backendComponentId = componentId;
            if (editingCurrentVisitor)
            {
                LastValidationResult = Act6PipelineValidator.Validate(
                    mainSlots,
                    backendComponentId);
                ResultsAreStale = false;
                SelectedComponentId = string.Empty;
                CurrentTraceIndex = 0;
                CurrentVisitorReplyShown = false;
                CompletedTestCount = CurrentTestIndex;
                CurrentMood = Act6GhostMood.Neutral;
                StatusLine = "Visitor 3 will retry with " +
                    Act6PipelineData.GetComponent(componentId).Label + ".";
                ShowLilyReaction("Good, we only changed the stored-data action. The first two visitor results can stay.");
                NotifyStateChanged();
                return true;
            }

            MarkPlacementChanged(
                Act6PipelineData.GetComponent(componentId).Label +
                " attached to the backend action socket.");
            ShowLilyReaction(GetCardReaction(componentId));
            return true;
        }

        public void ResetPipeline()
        {
            if (CurrentPhase != Act6PipelinePhase.Configure)
            {
                return;
            }

            Array.Clear(mainSlots, 0, mainSlots.Length);
            backendComponentId = string.Empty;
            SelectedComponentId = string.Empty;
            LastValidationResult = null;
            ResultsAreStale = false;
            ResetVisitorProgress();
            CurrentMood = Act6GhostMood.Neutral;
            StatusLine = "Board cleared. Rebuild the path before the first visitor enters.";
            ShowLilyReaction("Clean slate. Start with what Ghost must understand when a visitor first speaks.");
            NotifyStateChanged();
        }



        public void RunPipeline()
        {
            if (CurrentPhase != Act6PipelinePhase.Configure)
            {
                return;
            }

            if (!IsPipelineReadyToTest)
            {
                CurrentMood = Act6GhostMood.Confused;
                StatusLine = "Fill all five main stages before visitor 1 enters.";
                ShowLilyReaction("One main stage is still empty. The backend action can wait until a visitor needs stored data.");
                NotifyStateChanged();
                return;
            }

            LastValidationResult = Act6PipelineValidator.Validate(mainSlots, backendComponentId);
            ResultsAreStale = false;
            CurrentPhase = Act6PipelinePhase.VisitorTesting;
            CurrentTestIndex = 0;
            CurrentTraceIndex = 0;
            CompletedTestCount = 0;
            CurrentVisitorReplyShown = false;
            SelectedComponentId = string.Empty;
            CurrentMood = Act6GhostMood.Neutral;
            StatusLine = ActiveTraceStep == null
                ? "Visitor 1 is ready."
                : ActiveTraceStep.Line;
            ShowLilyReaction("The first visitor is here. Follow what each card does to this message.");
            NotifyStateChanged();
        }

        public void AdvanceVisitorTest()
        {
            if (CurrentPhase != Act6PipelinePhase.VisitorTesting ||
                ActiveTestResult == null)
            {
                return;
            }

            if (CurrentTraceIndex < ActiveTestResult.TraceSteps.Count - 1)
            {
                CurrentTraceIndex++;
                var step = ActiveTraceStep;
                var reachedReply = CurrentTraceIndex >= ActiveTestResult.TraceSteps.Count - 1;
                if (reachedReply)
                {
                    CurrentVisitorReplyShown = true;
                    CompletedTestCount = CurrentTestIndex + 1;
                    CurrentMood = ActiveTestResult.Passed
                        ? Act6GhostMood.Happy
                        : Act6GhostMood.Confused;
                }
                else
                {
                    CurrentMood = step != null && step.Succeeded
                        ? Act6GhostMood.Neutral
                        : Act6GhostMood.Confused;
                }

                StatusLine = step == null ? string.Empty : step.Line;
                ShowLilyReaction(reachedReply
                    ? GetVisitorResultReaction(ActiveTestResult)
                    : GetTraceReaction(step));
                NotifyStateChanged();
                return;
            }

            if (CurrentTestIndex < testCases.Count - 1)
            {
                CurrentTestIndex++;
                CurrentTraceIndex = 0;
                CurrentVisitorReplyShown = false;
                CurrentMood = Act6GhostMood.Neutral;
                StatusLine = ActiveTraceStep == null
                    ? "The next visitor is ready."
                    : ActiveTraceStep.Line;
                ShowLilyReaction(
                    "Visitor " + (CurrentTestIndex + 1) +
                    " has a different request. Let us see whether the same route still works.");
                NotifyStateChanged();
                return;
            }

            FinishVisitorTests();
        }

        public void CancelVisitorTests()
        {
            if (CurrentPhase != Act6PipelinePhase.VisitorTesting)
            {
                return;
            }

            CurrentPhase = Act6PipelinePhase.Configure;
            LastValidationResult = null;
            ResultsAreStale = false;
            SelectedComponentId = string.Empty;
            ResetVisitorProgress();
            CurrentMood = Act6GhostMood.Neutral;
            StatusLine = "The route is unlocked. Move the cards, then restart with visitor 1.";
            ShowLilyReaction("We can pause here. Change the route and call the first visitor back when it is ready.");
            NotifyStateChanged();
        }

        public void BeginEnding()
        {
            if (CurrentPhase != Act6PipelinePhase.ReadyForEnding)
            {
                return;
            }

            CurrentPhase = Act6PipelinePhase.Ending;
            CurrentMood = Act6GhostMood.Happy;
            NotifyStateChanged();
        }

        private void FinishVisitorTests()
        {
            if (UnityEngine.Application.isPlaying)
            {
                GhostBackendClient.PostAttempt(
                    GhostNarrativeState.FinalChapterId,
                    GhostBackendClient.CreateAttemptResult(LastValidationResult.IsCorrect),
                    GhostBackendClient.CreateAttemptDetails(
                        "final-integration-visitor-sequence",
                        LastValidationResult.Errors,
                        "backend=" + backendComponentId +
                        "; visitors=" + LastValidationResult.PassedTestCount +
                        "/" + LastValidationResult.TestResults.Count));
            }

            if (LastValidationResult.IsCorrect)
            {
                CurrentPhase = Act6PipelinePhase.ReadyForEnding;
                CurrentMood = Act6GhostMood.Happy;
                StatusLine = "All three visitors received the expected reply. Ghost's voice is stable.";
                ShowLilyReaction("All three worked! The same voice path handled uncertainty, missing details, and stored data.");
                NotifyStateChanged();
                return;
            }

            CurrentPhase = Act6PipelinePhase.Configure;
            CurrentMood = LastValidationResult.PassedTestCount > 0
                ? Act6GhostMood.Confused
                : Act6GhostMood.Sad;
            var firstFailed = FindFirstFailedResult();
            StatusLine = "Visitors helped: " +
                LastValidationResult.PassedTestCount + "/" +
                LastValidationResult.TestResults.Count + ". " +
                (firstFailed == null
                    ? "The path still has a structural problem."
                    : firstFailed.ActualReply) +
                " Revise the route and start again with visitor 1.";
            ShowLilyReaction(
                "The first failed visitor shows where the same route stopped working. Compare that reply with the expected one.");
            AmbientBanterPanel.RequestHint(
                GhostNarrativeState.FinalChapterId,
                "after_failed_pipeline_suite",
                BuildHintContext());
            NotifyStateChanged();
        }

        private Act6PipelineTestResult FindFirstFailedResult()
        {
            if (LastValidationResult == null)
            {
                return null;
            }

            foreach (var result in LastValidationResult.TestResults)
            {
                if (!result.Passed)
                {
                    return result;
                }
            }

            return null;
        }

        private static void ShowLilyReaction(string line)
        {
            AmbientBanterPanel.ShowReaction(
                GhostNarrativeState.FinalChapterId,
                line);
        }

        private static string GetCardReaction(string componentId)
        {
            switch (componentId)
            {
                case Act6PipelineData.IntentClassificationId:
                    return "This compares the visitor's purpose. Does Ghost need that before it uses the details?";
                case Act6PipelineData.EntityExtractionId:
                    return "WHAT, WHERE, and WHEN are useful only after Ghost knows what kind of request it heard.";
                case Act6PipelineData.ConfidenceFallbackId:
                    return "This is Ghost's pause before answering. It should happen before Ghost commits to a reply.";
                case Act6PipelineData.DialogueManagementId:
                    return "The reply map needs the purpose and details before it can choose a branch.";
                case Act6PipelineData.ResponseGenerationId:
                    return "This makes the final sentence, so it needs a result from the earlier route.";
                case Act6PipelineData.BackendActionId:
                    return "This asks for lab closing time. Only the hours visitor should need that side route.";
                case Act6PipelineData.KeywordGuessId:
                    return "One keyword feels fast, but a differently worded visitor might expose the guess.";
                case Act6PipelineData.SkipDetailsId:
                    return "If Ghost skips the details, imagine what happens when an object, room, or time matters.";
                case Act6PipelineData.AlwaysAnswerId:
                    return "Always answering sounds confident. What happens when Ghost is actually unsure?";
                case Act6PipelineData.FirstReplyId:
                    return "The first reply is quick, but it may ignore the branch built and tested earlier.";
                case Act6PipelineData.RawDataReplyId:
                    return "A raw value is useful inside the route, but a visitor still needs a complete reply.";
                case Act6PipelineData.ObjectRoomBackendId:
                    return "That action finds an object's room. One visitor needs a closing time instead.";
                case Act6PipelineData.ExactWordingId:
                    return "Exact wording may miss the same purpose written differently. Try it against a new phrase.";
                case Act6PipelineData.NounsOnlyId:
                    return "Keeping only objects can lose WHERE or WHEN. Which visitors still need those details?";
                case Act6PipelineData.RejectAllId:
                    return "Asking everyone again is safe, but it also blocks clear high-confidence requests.";
                case Act6PipelineData.FixedRouteId:
                    return "One route cannot handle fallback, missing details, and lab hours in the same way.";
                case Act6PipelineData.FixedSentenceId:
                    return "A fixed sentence ignores the result that the earlier stages worked out.";
                case Act6PipelineData.VisitorProfileBackendId:
                    return "This returns a visitor name, but the current route asks for a closing time.";
                default:
                    return "Trace what this card receives and what it should pass to the next stage.";
            }
        }

        private static string GetTraceReaction(Act6PipelineTraceStep step)
        {
            if (step == null)
            {
                return "The visitor is still moving through the route.";
            }

            if (!step.Succeeded)
            {
                return "That is where this visitor's route stopped. Check what this card expected to receive.";
            }

            switch (step.ComponentId)
            {
                case Act6PipelineData.VisitorMessageEndpointId:
                    return "A new visitor is here. Start by finding the purpose of this message.";
                case Act6PipelineData.IntentClassificationId:
                case Act6PipelineData.KeywordGuessId:
                case Act6PipelineData.ExactWordingId:
                    return "Ghost has chosen a purpose and confidence value. See what the next card does with it.";
                case Act6PipelineData.EntityExtractionId:
                case Act6PipelineData.SkipDetailsId:
                case Act6PipelineData.NounsOnlyId:
                    return "Now we can see which details will reach the decision and reply map.";
                case Act6PipelineData.ConfidenceFallbackId:
                case Act6PipelineData.AlwaysAnswerId:
                case Act6PipelineData.RejectAllId:
                    return "This decision controls whether Ghost continues or asks the visitor again.";
                case Act6PipelineData.DialogueManagementId:
                case Act6PipelineData.FirstReplyId:
                case Act6PipelineData.FixedRouteId:
                    return "The selected branch now decides what kind of reply Ghost will make.";
                case Act6PipelineData.BackendActionId:
                case Act6PipelineData.ObjectRoomBackendId:
                case Act6PipelineData.VisitorProfileBackendId:
                    return "The side route returned a fact. Check whether it is the fact this visitor needs.";
                case Act6PipelineData.ResponseGenerationId:
                case Act6PipelineData.RawDataReplyId:
                case Act6PipelineData.FixedSentenceId:
                    return "Ghost has formed a reply. Compare it with what this visitor needed.";
                default:
                    return "The visitor is still moving through the route.";
            }
        }

        private static string GetVisitorResultReaction(Act6PipelineTestResult result)
        {
            return result != null && result.Passed
                ? "That visitor received the expected reply. Now test whether the same route handles the next situation."
                : "Ghost replied, but it does not match what this visitor needed. Keep this result for the repair.";
        }

        private int FindMainSlot(string componentId)
        {
            for (var index = 0; index < mainSlots.Length; index++)
            {
                if (string.Equals(mainSlots[index], componentId, StringComparison.Ordinal))
                {
                    return index;
                }
            }

            return -1;
        }

        private void MarkPlacementChanged(string status)
        {
            SelectedComponentId = string.Empty;
            ResultsAreStale = LastValidationResult != null;
            ResetVisitorProgress();
            CurrentMood = Act6GhostMood.Neutral;
            StatusLine = ResultsAreStale
                ? status + " The old visitor cards describe the previous route; start again with visitor 1."
                : status;
            NotifyStateChanged();
        }

        private void ResetVisitorProgress()
        {
            CurrentTestIndex = -1;
            CurrentTraceIndex = -1;
            CompletedTestCount = 0;
            CurrentVisitorReplyShown = false;
        }

        private void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }
    }

    public enum Act6PipelinePhase
    {
        Onboarding,
        Configure,
        VisitorTesting,
        ReadyForEnding,
        Ending
    }

    public enum Act6GhostMood
    {
        Neutral,
        Happy,
        Confused,
        Sad
    }
}
