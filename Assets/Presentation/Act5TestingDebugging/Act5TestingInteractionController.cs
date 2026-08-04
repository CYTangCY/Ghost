using System;
using System.Collections.Generic;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Banter;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.DialogGraph;
using Ghost.Puzzles.TestingDebugging;

namespace Ghost.Presentation.Act5TestingDebugging
{
    public sealed class Act5TestingInteractionController
    {
        private readonly IReadOnlyList<DialogNode> nodes;
        private readonly IReadOnlyList<Act5TestConversation> conversations;
        private readonly List<DialogTransition> transitions;

        public Act5TestingInteractionController()
        {
            nodes = Act5BuggyGraphData.CreateNodes();
            conversations = Act5BuggyGraphData.CreateTestConversations();
            transitions = new List<DialogTransition>(Act5BuggyGraphData.CreateBuggyTransitions());
            CurrentPhase = Act5TestingPhase.Onboarding;
            CurrentMood = Act5GhostMood.Neutral;
            StatusLine = "Ghost's reply map looks tidy, but its rehearsal answers do not match the visitors.";
        }

        public event Action StateChanged;

        public IReadOnlyList<DialogNode> Nodes => nodes;

        public IReadOnlyList<DialogTransition> Transitions => transitions.ToArray();

        public IReadOnlyList<Act5TestConversation> Conversations => conversations;

        public Act5TestingPhase CurrentPhase { get; private set; }

        public Act5GhostMood CurrentMood { get; private set; }

        public Act5TestSuiteResult LastSuiteResult { get; private set; }

        public bool ResultsAreStale { get; private set; }

        public string StatusLine { get; private set; }

        public bool HasRunTests => LastSuiteResult != null;

        public bool HasFailedRun => HasRunTests && !LastSuiteResult.IsCorrect;

        public void BeginAfterOnboarding()
        {
            if (CurrentPhase != Act5TestingPhase.Onboarding)
            {
                return;
            }

            CurrentPhase = Act5TestingPhase.Configure;
            CurrentMood = Act5GhostMood.Confused;
            StatusLine = "Run all four rehearsals. A red card will show what Ghost said instead of the expected reply.";
            NotifyStateChanged();
        }

        public void ReplayOnboarding()
        {
            if (CurrentPhase == Act5TestingPhase.Complete)
            {
                return;
            }

            CurrentPhase = Act5TestingPhase.Onboarding;
            CurrentMood = Act5GhostMood.Neutral;
            StatusLine = "Ghost's reply map looks tidy, but its rehearsal answers do not match the visitors.";
            NotifyStateChanged();
        }

        public void RunAllTests()
        {
            if (CurrentPhase == Act5TestingPhase.Onboarding)
            {
                return;
            }

            LastSuiteResult = Act5TestSuiteRunner.Run(BuildCurrentGraph(), conversations);
            ResultsAreStale = false;

            if (LastSuiteResult.IsCorrect)
            {
                CurrentPhase = Act5TestingPhase.Complete;
                CurrentMood = Act5GhostMood.Happy;
                StatusLine = "All 4 rehearsals pass. Ghost now gives each visitor the expected reply.";
            }
            else
            {
                CurrentPhase = Act5TestingPhase.Configure;
                CurrentMood = Act5GhostMood.Confused;
                StatusLine = LastSuiteResult.PassedCount + "/4 tests pass. Compare the first red card, repair its route, then rerun every test.";

                AmbientBanterPanel.RequestHint(
                    GhostNarrativeState.Act5Id,
                    "after_failed_test_suite",
                    "The player ran Act 5's deterministic dialog-graph tests and still has failures. Give a non-spoiler hint about comparing expected versus actual and tracing the relevant wire.");
            }

            GhostBackendClient.PostAttempt(
                GhostNarrativeState.Act5Id,
                GhostBackendClient.CreateAttemptResult(LastSuiteResult.IsCorrect),
                GhostBackendClient.CreateAttemptDetails(
                    "act5-dialog-test-suite",
                    LastSuiteResult.ValidationErrors,
                    "passed=" + LastSuiteResult.PassedCount + "/" + LastSuiteResult.CaseResults.Count));

            NotifyStateChanged();
        }

        public bool ConnectNodes(
            string fromNodeId,
            string toNodeId,
            DialogTransitionCondition condition)
        {
            if (CurrentPhase == Act5TestingPhase.Onboarding ||
                CurrentPhase == Act5TestingPhase.Complete ||
                !HasRunTests ||
                !CanConnectNodes(fromNodeId, toNodeId, condition))
            {
                return false;
            }

            var source = FindNode(fromNodeId);
            if (source.Type != DialogNodeType.Start)
            {
                RemoveExistingTransitionFromOutput(fromNodeId, condition);
            }

            transitions.Add(new DialogTransition(fromNodeId, toNodeId, condition));
            ResultsAreStale = LastSuiteResult != null;
            CurrentMood = Act5GhostMood.Neutral;
            StatusLine = ResultsAreStale
                ? "Graph changed. The visible results describe the previous wiring; rerun all four tests."
                : "Route reconnected. Run all four tests to see whether the repair works.";
            NotifyStateChanged();
            return true;
        }

        public Act5TestCaseResult FindLastResult(string conversationId)
        {
            if (LastSuiteResult == null || string.IsNullOrWhiteSpace(conversationId))
            {
                return null;
            }

            foreach (var result in LastSuiteResult.CaseResults)
            {
                if (string.Equals(result.Conversation.Id, conversationId, StringComparison.Ordinal))
                {
                    return result;
                }
            }

            return null;
        }

        public Act5TestCaseResult FindFirstFailure()
        {
            if (LastSuiteResult == null)
            {
                return null;
            }

            foreach (var result in LastSuiteResult.CaseResults)
            {
                if (!result.Passed)
                {
                    return result;
                }
            }

            return null;
        }

        public string BuildHintContext()
        {
            if (LastSuiteResult == null)
            {
                return "The four rehearsal conversations have not been run yet.";
            }

            var firstFailure = FindFirstFailure();
            if (firstFailure == null)
            {
                return "All four rehearsal conversations pass.";
            }

            return "Passed=" + LastSuiteResult.PassedCount + "/4; stale=" +
                ResultsAreStale + "; first failed visitor=" +
                firstFailure.Conversation.VisitorMessage + "; expected response id=" +
                firstFailure.Conversation.TestCase.ExpectedResponseId +
                "; actual response id=" +
                (firstFailure.ActualResponseId ?? "no response") + ".";
        }

        private DialogGraph BuildCurrentGraph()
        {
            return new DialogGraph(Act5BuggyGraphData.StartNodeId, nodes, transitions);
        }

        private bool CanConnectNodes(
            string fromNodeId,
            string toNodeId,
            DialogTransitionCondition condition)
        {
            if (string.IsNullOrWhiteSpace(fromNodeId) ||
                string.IsNullOrWhiteSpace(toNodeId) ||
                string.Equals(fromNodeId, toNodeId, StringComparison.Ordinal))
            {
                return false;
            }

            var source = FindNode(fromNodeId);
            var target = FindNode(toNodeId);
            if (source == null || target == null || !IsConditionAllowed(source, condition))
            {
                return false;
            }

            if (!IsTargetAllowed(source, target))
            {
                return false;
            }

            foreach (var transition in transitions)
            {
                if (string.Equals(transition.FromNodeId, fromNodeId, StringComparison.Ordinal) &&
                    string.Equals(transition.ToNodeId, toNodeId, StringComparison.Ordinal) &&
                    transition.Condition == condition)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsConditionAllowed(
            DialogNode source,
            DialogTransitionCondition condition)
        {
            switch (source.Type)
            {
                case DialogNodeType.Start:
                case DialogNodeType.IntentBranch:
                    return condition == DialogTransitionCondition.Always;
                case DialogNodeType.SlotCheck:
                    return condition == DialogTransitionCondition.SlotPresent ||
                        condition == DialogTransitionCondition.SlotMissing;
                default:
                    return false;
            }
        }

        private static bool IsTargetAllowed(DialogNode source, DialogNode target)
        {
            switch (source.Type)
            {
                case DialogNodeType.Start:
                    return target.Type == DialogNodeType.IntentBranch;
                case DialogNodeType.IntentBranch:
                    return target.Type == DialogNodeType.SlotCheck ||
                        target.Type == DialogNodeType.Response;
                case DialogNodeType.SlotCheck:
                    return target.Type == DialogNodeType.Response;
                default:
                    return false;
            }
        }

        private void RemoveExistingTransitionFromOutput(
            string fromNodeId,
            DialogTransitionCondition condition)
        {
            for (var index = transitions.Count - 1; index >= 0; index--)
            {
                var transition = transitions[index];
                if (string.Equals(transition.FromNodeId, fromNodeId, StringComparison.Ordinal) &&
                    transition.Condition == condition)
                {
                    transitions.RemoveAt(index);
                }
            }
        }

        private DialogNode FindNode(string nodeId)
        {
            foreach (var node in nodes)
            {
                if (string.Equals(node.Id, nodeId, StringComparison.Ordinal))
                {
                    return node;
                }
            }

            return null;
        }

        private void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }
    }

    public enum Act5TestingPhase
    {
        Onboarding,
        Configure,
        Complete
    }

    public enum Act5GhostMood
    {
        Neutral,
        Happy,
        Confused,
        Sad
    }
}