using System;
using System.Collections.Generic;
using Ghost.Presentation.Banter;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.DialogGraph;
using UnityEngine;

namespace Ghost.Presentation.Act3DialogGraph
{
    public sealed class Act3DialogGraphInteractionController
    {
        private readonly DialogGraphSession session;
        private readonly Dictionary<string, Vector2> nodePositions;

        public Act3DialogGraphInteractionController()
        {
            session = DialogGraphSession.CreateFromSampleData();
            nodePositions = new Dictionary<string, Vector2>(StringComparer.Ordinal);
            SelectedNodeId = string.Empty;
            CurrentPhase = Act3ExperiencePhase.Onboarding;
            CurrentReaction = Act3GhostReaction.Neutral;
            LastValidationErrors = Array.Empty<string>();
            LastFeedbackMessage = string.Empty;
        }

        public event Action StateChanged;

        public event Action<string, bool, IReadOnlyList<string>> FeedbackChanged;

        public IReadOnlyList<DialogNode> CurrentNodes => session.CurrentNodes;

        public IReadOnlyList<DialogTransition> CurrentTransitions => session.CurrentTransitions;

        public string CurrentStartNodeId => session.CurrentStartNodeId;

        public string SelectedNodeId { get; private set; }

        public IReadOnlyList<DialogGraphTestCase> TestCases => session.TestCases;

        public Act3ExperiencePhase CurrentPhase { get; private set; }

        public Act3GhostReaction CurrentReaction { get; private set; }

        public bool HasValidationAttempt { get; private set; }

        public bool LastValidationWasCorrect { get; private set; }

        private int visitorIndex;

        /// <summary>How many visitors have walked in so far. Starts at one and grows on each success.</summary>
        public int RevealedVisitorCount { get; private set; } = 1;

        /// <summary>The visitor currently at the desk during the build phase.</summary>
        public Act3DialogGraphSampleData.VisitorScript ArrivedVisitor
        {
            get
            {
                var scripts = Act3DialogGraphSampleData.CreateVisitorScripts();
                var index = RevealedVisitorCount - 1;
                return index >= 0 && index < scripts.Count ? scripts[index] : null;
            }
        }

        /// <summary>Which visitor is at the desk during playback, 1-based for display.</summary>
        public int CurrentVisitorNumber => CurrentPhase == Act3ExperiencePhase.Playback ? visitorIndex + 1 : 0;

        public int VisitorCount => Act3DialogGraphSampleData.CreateVisitorScripts().Count;

        public Act3DialogGraphSampleData.VisitorScript CurrentVisitor
        {
            get
            {
                if (CurrentPhase != Act3ExperiencePhase.Playback)
                {
                    return null;
                }

                var scripts = Act3DialogGraphSampleData.CreateVisitorScripts();
                return visitorIndex >= 0 && visitorIndex < scripts.Count ? scripts[visitorIndex] : null;
            }
        }

        public bool HasMoreVisitors =>
            RevealedVisitorCount < Act3DialogGraphSampleData.CreateVisitorScripts().Count;

        /// <summary>
        /// Ends the current visitor's turn. If anyone is still waiting outside, the next one walks in
        /// and the player goes back to editing with a new requirement; otherwise the act is done.
        /// </summary>
        public void AdvanceVisitor()
        {
            if (CurrentPhase != Act3ExperiencePhase.Playback)
            {
                return;
            }

            var total = Act3DialogGraphSampleData.CreateVisitorScripts().Count;
            if (RevealedVisitorCount < total)
            {
                RevealedVisitorCount++;
                CurrentPhase = Act3ExperiencePhase.Build;
                HasValidationAttempt = false;
                LastValidationWasCorrect = false;
                LastValidationErrors = Array.Empty<string>();
                CurrentReaction = Act3GhostReaction.Neutral;

                var next = ArrivedVisitor;
                LastFeedbackMessage = next != null
                    ? "Another visitor walks in: \"" + next.VisitorLine + "\" Extend the map for this one."
                    : "Another visitor walks in.";
            }
            else
            {
                CurrentPhase = Act3ExperiencePhase.Complete;
                CurrentReaction = Act3GhostReaction.Happy;
                LastFeedbackMessage = "Every visitor got the reply their message deserved.";
            }

            StateChanged?.Invoke();
        }

        public bool HasFailedValidation => HasValidationAttempt && !LastValidationWasCorrect;

        public string LastFeedbackMessage { get; private set; }

        public IReadOnlyList<string> LastValidationErrors { get; private set; }

        public void BeginAfterOnboarding()
        {
            if (CurrentPhase != Act3ExperiencePhase.Onboarding)
            {
                return;
            }

            CurrentPhase = Act3ExperiencePhase.Build;
            NotifyStateChanged();
        }

        public void ReplayOnboarding()
        {
            if (CurrentPhase == Act3ExperiencePhase.Onboarding || CurrentPhase == Act3ExperiencePhase.Complete)
            {
                return;
            }

            CurrentPhase = Act3ExperiencePhase.Onboarding;
            NotifyStateChanged();
        }

        public string PlaceNode(
            DialogNodeType type,
            string intentId = null,
            string requiredEntityType = null,
            string responseId = null,
            Vector2? normalizedPosition = null)
        {
            var typePlacementIndex = CountNodesOfType(type);
            var nodeId = session.AddNode(type, intentId, requiredEntityType, responseId);
            nodePositions[nodeId] = ClampNodePosition(normalizedPosition ?? CreateDefaultNodePosition(type, responseId, typePlacementIndex));

            if (type == DialogNodeType.Start)
            {
                session.SetStartNode(nodeId);
            }

            SelectedNodeId = nodeId;
            NotifyGraphChanged();
            return nodeId;
        }

        public Vector2 GetNodePosition(DialogNode node)
        {
            if (node == null)
            {
                return new Vector2(0.5f, 0.5f);
            }

            if (nodePositions.TryGetValue(node.Id, out var position))
            {
                return position;
            }

            return CreateDefaultNodePosition(node.Type, node.ResponseId, 0);
        }

        public void SetNodePosition(string nodeId, Vector2 normalizedPosition)
        {
            if (FindNode(nodeId) == null)
            {
                return;
            }

            nodePositions[nodeId] = ClampNodePosition(normalizedPosition);
        }

        public void SelectNode(string nodeId)
        {
            var nextSelection = string.Equals(SelectedNodeId, nodeId, StringComparison.Ordinal)
                ? string.Empty
                : nodeId ?? string.Empty;

            if (string.Equals(SelectedNodeId, nextSelection, StringComparison.Ordinal))
            {
                return;
            }

            SelectedNodeId = nextSelection;
            NotifyStateChanged();
        }

        public void ClearSelection()
        {
            if (string.IsNullOrWhiteSpace(SelectedNodeId))
            {
                return;
            }

            SelectedNodeId = string.Empty;
            NotifyStateChanged();
        }

        public void SetSelectedAsStart()
        {
            if (string.IsNullOrWhiteSpace(SelectedNodeId))
            {
                return;
            }

            SetStartNode(SelectedNodeId);
        }

        public void SetStartNode(string nodeId)
        {
            session.SetStartNode(nodeId);
            NotifyGraphChanged();
        }

        public bool ConnectNodes(string fromId, string toId, DialogTransitionCondition condition)
        {
            if (!CanConnectNodes(fromId, toId, condition))
            {
                return false;
            }

            // One wire per output port is right for an intent branch or a slot check, but the start
            // node has to fan out to every intent it handles - it picks a branch by matching the
            // visitor's intent, so a second Always edge is required, not a replacement.
            if (!IsStartNode(fromId))
            {
                RemoveExistingTransitionFromOutput(fromId, condition);
            }

            session.AddTransition(fromId, toId, condition);
            NotifyGraphChanged();
            return true;
        }

        public bool RemoveNode(string nodeId)
        {
            var removed = session.RemoveNode(nodeId);
            if (!removed)
            {
                return false;
            }

            if (string.Equals(SelectedNodeId, nodeId, StringComparison.Ordinal))
            {
                SelectedNodeId = string.Empty;
            }

            nodePositions.Remove(nodeId);
            NotifyGraphChanged();
            return true;
        }

        public bool RemoveTransition(string fromId, string toId, DialogTransitionCondition condition)
        {
            var removed = session.RemoveTransition(fromId, toId, condition);
            if (removed)
            {
                NotifyGraphChanged();
            }

            return removed;
        }

        public DialogGraphResult ValidateCurrentState()
        {
            var result = session.ValidateCurrentState(RevealedVisitorCount);
            GhostBackendClient.PostAttempt(
                GhostNarrativeState.Act3Id,
                GhostBackendClient.CreateAttemptResult(result.IsCorrect),
                GhostBackendClient.CreateAttemptDetails(
                    "act3-dialog-graph",
                    result.Errors,
                    "Dialog graph validation"));

            var feedbackMessage = result.IsCorrect
                ? "Nice. Ghost answers when the room is known and asks when it is missing."
                : CreateIncorrectFeedbackMessage(result.Errors.Count);

            HasValidationAttempt = true;
            LastValidationWasCorrect = result.IsCorrect;
            LastFeedbackMessage = feedbackMessage;
            LastValidationErrors = new List<string>(result.Errors);
            if (result.IsCorrect)
            {
                visitorIndex = RevealedVisitorCount - 1;
                CurrentPhase = Act3ExperiencePhase.Playback;
            }
            else
            {
                CurrentPhase = Act3ExperiencePhase.Build;
            }
            CurrentReaction = result.IsCorrect
                ? Act3GhostReaction.Happy
                : IsEmptyOrIncompleteGraph()
                    ? Act3GhostReaction.Sad
                    : Act3GhostReaction.Confused;

            FeedbackChanged?.Invoke(feedbackMessage, result.IsCorrect, result.Errors);
            if (!result.IsCorrect)
            {
                AmbientBanterPanel.RequestHint(
                    GhostNarrativeState.Act3Id,
                    "after_incorrect_validate",
                    "The player validated an incorrect dialog graph. Error count: " + result.Errors.Count + ". Give a non-spoiler hint about ordering the map and checking slots before answering.");
            }

            NotifyStateChanged();
            return result;
        }

        private bool IsEmptyOrIncompleteGraph()
        {
            if (session.CurrentNodes.Count == 0
                || string.IsNullOrWhiteSpace(session.CurrentStartNodeId)
                || session.CurrentTransitions.Count == 0)
            {
                return true;
            }

            var hasStart = false;
            var hasIntent = false;
            var hasSlotCheck = false;
            var hasAnswer = false;
            var hasAsk = false;

            foreach (var node in session.CurrentNodes)
            {
                switch (node.Type)
                {
                    case DialogNodeType.Start:
                        hasStart = true;
                        break;
                    case DialogNodeType.IntentBranch:
                        hasIntent = true;
                        break;
                    case DialogNodeType.SlotCheck:
                        hasSlotCheck = true;
                        break;
                    case DialogNodeType.Response:
                        hasAnswer |= string.Equals(
                            node.ResponseId,
                            Act3DialogGraphSampleData.AnswerObjectLocationResponseId,
                            StringComparison.Ordinal);
                        hasAsk |= string.Equals(
                            node.ResponseId,
                            Act3DialogGraphSampleData.AskForRoomResponseId,
                            StringComparison.Ordinal);
                        break;
                }
            }

            return !hasStart || !hasIntent || !hasSlotCheck || !hasAnswer || !hasAsk;
        }

        private bool CanConnectNodes(string fromId, string toId, DialogTransitionCondition condition)
        {
            if (string.IsNullOrWhiteSpace(fromId) || string.IsNullOrWhiteSpace(toId))
            {
                return false;
            }

            if (string.Equals(fromId, toId, StringComparison.Ordinal))
            {
                return false;
            }

            var fromNode = FindNode(fromId);
            if (fromNode == null || !IsConditionAllowedForSourceNode(fromNode, condition))
            {
                return false;
            }

            if (FindNode(toId) == null)
            {
                return false;
            }

            foreach (var transition in session.CurrentTransitions)
            {
                if (string.Equals(transition.FromNodeId, fromId, StringComparison.Ordinal)
                    && string.Equals(transition.ToNodeId, toId, StringComparison.Ordinal)
                    && transition.Condition == condition)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsStartNode(string nodeId)
        {
            var node = FindNode(nodeId);
            return node != null && node.Type == DialogNodeType.Start;
        }

        private void RemoveExistingTransitionFromOutput(string fromId, DialogTransitionCondition condition)
        {
            var transitions = session.CurrentTransitions;

            foreach (var transition in transitions)
            {
                if (string.Equals(transition.FromNodeId, fromId, StringComparison.Ordinal)
                    && transition.Condition == condition)
                {
                    session.RemoveTransition(transition.FromNodeId, transition.ToNodeId, transition.Condition);
                }
            }
        }

        private static bool IsConditionAllowedForSourceNode(DialogNode node, DialogTransitionCondition condition)
        {
            switch (node.Type)
            {
                case DialogNodeType.Start:
                case DialogNodeType.IntentBranch:
                    return condition == DialogTransitionCondition.Always;

                case DialogNodeType.SlotCheck:
                    return condition == DialogTransitionCondition.SlotPresent
                        || condition == DialogTransitionCondition.SlotMissing;

                default:
                    return false;
            }
        }

        private DialogNode FindNode(string nodeId)
        {
            foreach (var node in session.CurrentNodes)
            {
                if (string.Equals(node.Id, nodeId, StringComparison.Ordinal))
                {
                    return node;
                }
            }

            return null;
        }

        private int CountNodesOfType(DialogNodeType type)
        {
            var count = 0;

            foreach (var node in session.CurrentNodes)
            {
                if (node.Type == type)
                {
                    count++;
                }
            }

            return count;
        }

        private static Vector2 CreateDefaultNodePosition(DialogNodeType type, string responseId, int typePlacementIndex)
        {
            var yOffset = Mathf.Clamp(typePlacementIndex, 0, 3) * -0.11f;

            switch (type)
            {
                case DialogNodeType.Start:
                    return new Vector2(0.12f, 0.74f + yOffset);

                case DialogNodeType.IntentBranch:
                    return new Vector2(0.34f, 0.74f + yOffset);

                case DialogNodeType.SlotCheck:
                    return new Vector2(0.56f, 0.74f + yOffset);

                case DialogNodeType.Response:
                    if (string.Equals(responseId, Act3DialogGraphSampleData.AskForRoomResponseId, StringComparison.Ordinal))
                    {
                        return new Vector2(0.82f, 0.44f + yOffset);
                    }

                    return new Vector2(0.82f, 0.76f + yOffset);

                default:
                    return new Vector2(0.5f, 0.5f);
            }
        }

        private static Vector2 ClampNodePosition(Vector2 normalizedPosition)
        {
            return new Vector2(
                Mathf.Clamp(normalizedPosition.x, -0.12f, 1.12f),
                Mathf.Clamp(normalizedPosition.y, -0.28f, 1.08f));
        }

        private void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }

        private void NotifyGraphChanged()
        {
            if (LastValidationWasCorrect)
            {
                HasValidationAttempt = false;
                LastValidationWasCorrect = false;
                LastFeedbackMessage = string.Empty;
                LastValidationErrors = Array.Empty<string>();
                CurrentReaction = Act3GhostReaction.Neutral;
                CurrentPhase = Act3ExperiencePhase.Build;
            }

            NotifyStateChanged();
        }

        private static string CreateIncorrectFeedbackMessage(int issueCount)
        {
            if (issueCount <= 0)
            {
                return "Not yet. Ghost's map still needs a fix.";
            }

            if (issueCount == 1)
            {
                return "Not yet. Ghost's map still has 1 issue.";
            }

            return $"Not yet. Ghost's map still has {issueCount} issues.";
        }
    }

    public enum Act3ExperiencePhase
    {
        Onboarding,
        Build,
        Playback,
        Complete
    }

    public enum Act3GhostReaction
    {
        Neutral,
        Happy,
        Confused,
        Sad
    }
}
