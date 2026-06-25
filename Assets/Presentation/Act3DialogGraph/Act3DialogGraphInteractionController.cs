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
        }

        public event Action StateChanged;

        public event Action<string, bool, IReadOnlyList<string>> FeedbackChanged;

        public IReadOnlyList<DialogNode> CurrentNodes => session.CurrentNodes;

        public IReadOnlyList<DialogTransition> CurrentTransitions => session.CurrentTransitions;

        public string CurrentStartNodeId => session.CurrentStartNodeId;

        public string SelectedNodeId { get; private set; }

        public IReadOnlyList<DialogGraphTestCase> TestCases => session.TestCases;

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
            NotifyStateChanged();
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
            NotifyStateChanged();
        }

        public bool ConnectNodes(string fromId, string toId, DialogTransitionCondition condition)
        {
            if (!CanConnectNodes(fromId, toId, condition))
            {
                return false;
            }

            RemoveExistingTransitionFromOutput(fromId, condition);
            session.AddTransition(fromId, toId, condition);
            NotifyStateChanged();
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
            NotifyStateChanged();
            return true;
        }

        public bool RemoveTransition(string fromId, string toId, DialogTransitionCondition condition)
        {
            var removed = session.RemoveTransition(fromId, toId, condition);
            if (removed)
            {
                NotifyStateChanged();
            }

            return removed;
        }

        public DialogGraphResult ValidateCurrentState()
        {
            var result = session.ValidateCurrentState();
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

            FeedbackChanged?.Invoke(feedbackMessage, result.IsCorrect, result.Errors);
            if (!result.IsCorrect)
            {
                AmbientBanterPanel.RequestHint(
                    GhostNarrativeState.Act3Id,
                    "after_incorrect_validate",
                    "The player validated an incorrect dialog graph. Error count: " + result.Errors.Count + ". Give a non-spoiler hint about ordering the map and checking slots before answering.");
            }

            return result;
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
}
