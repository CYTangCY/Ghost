using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.DialogGraph
{
    public sealed class DialogGraphSession
    {
        private readonly List<DialogNode> currentNodes;
        private readonly List<DialogTransition> currentTransitions;
        private readonly DialogGraphTestCase[] testCases;
        private int nextNodeNumber;
        private string currentStartNodeId;

        public DialogGraphSession(IEnumerable<DialogGraphTestCase> testCases)
        {
            if (testCases == null)
            {
                throw new ArgumentNullException(nameof(testCases));
            }

            this.testCases = CopyTestCases(testCases);
            currentNodes = new List<DialogNode>();
            currentTransitions = new List<DialogTransition>();
            currentStartNodeId = string.Empty;
            nextNodeNumber = 1;
        }

        public IReadOnlyList<DialogNode> CurrentNodes => currentNodes.ToArray();

        public IReadOnlyList<DialogTransition> CurrentTransitions => currentTransitions.ToArray();

        public string CurrentStartNodeId => currentStartNodeId;

        public IReadOnlyList<DialogGraphTestCase> TestCases => testCases;

        public static DialogGraphSession CreateFromSampleData()
        {
            return new DialogGraphSession(Act3DialogGraphSampleData.CreateTestCases());
        }

        /// <summary>
        /// Creates a node with a generated unique id, adds it to the building state, and returns the id.
        /// The DialogNode constructor validates the per-type configuration.
        /// </summary>
        public string AddNode(
            DialogNodeType type,
            string intentId = null,
            string requiredEntityType = null,
            string responseId = null)
        {
            var nodeId = CreateNextNodeId(type);
            var node = new DialogNode(nodeId, type, intentId, requiredEntityType, responseId);
            currentNodes.Add(node);
            return nodeId;
        }

        /// <summary>
        /// Removes a node and every transition that references it. If the removed node was the start
        /// node, the current start id is cleared. Returns false when the node id is absent.
        /// </summary>
        public bool RemoveNode(string nodeId)
        {
            for (var index = 0; index < currentNodes.Count; index++)
            {
                if (!string.Equals(currentNodes[index].Id, nodeId, StringComparison.Ordinal))
                {
                    continue;
                }

                currentNodes.RemoveAt(index);
                RemoveTransitionsReferencingNode(nodeId);

                if (string.Equals(currentStartNodeId, nodeId, StringComparison.Ordinal))
                {
                    currentStartNodeId = string.Empty;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the start node for the in-progress graph. Unknown or empty node ids are rejected so
        /// UI code catches bad wiring early; incomplete graphs still validate incorrect without throwing.
        /// </summary>
        public void SetStartNode(string nodeId)
        {
            if (!ContainsNode(nodeId))
            {
                throw new ArgumentException($"Cannot set unknown node '{nodeId}' as the start node.", nameof(nodeId));
            }

            currentStartNodeId = nodeId;
        }

        /// <summary>
        /// Adds a transition between existing nodes. Unknown source/target ids are rejected by the
        /// session rather than deferred to validation.
        /// </summary>
        public void AddTransition(string fromNodeId, string toNodeId, DialogTransitionCondition condition)
        {
            EnsureKnownNode(fromNodeId, nameof(fromNodeId));
            EnsureKnownNode(toNodeId, nameof(toNodeId));
            currentTransitions.Add(new DialogTransition(fromNodeId, toNodeId, condition));
        }

        public bool RemoveTransition(string fromNodeId, string toNodeId, DialogTransitionCondition condition)
        {
            for (var index = 0; index < currentTransitions.Count; index++)
            {
                var transition = currentTransitions[index];
                if (!IsMatchingTransition(transition, fromNodeId, toNodeId, condition))
                {
                    continue;
                }

                currentTransitions.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates the current building state. Incomplete state returns an incorrect result with
        /// clear errors instead of allowing DialogGraph construction exceptions to escape.
        /// </summary>
        public DialogGraphResult ValidateCurrentState()
        {
            var incompleteErrors = CreateIncompleteGraphErrors();
            if (incompleteErrors.Count > 0)
            {
                return new DialogGraphResult(false, incompleteErrors);
            }

            var graph = new DialogGraph(currentStartNodeId, CurrentNodes, CurrentTransitions);
            return DialogGraphValidator.Validate(graph, TestCases);
        }

        private static DialogGraphTestCase[] CopyTestCases(IEnumerable<DialogGraphTestCase> source)
        {
            var copiedTestCases = new List<DialogGraphTestCase>();

            foreach (var testCase in source)
            {
                if (testCase == null)
                {
                    throw new ArgumentException("Dialog graph session cannot contain a null test case.", nameof(source));
                }

                copiedTestCases.Add(testCase);
            }

            return copiedTestCases.ToArray();
        }

        private string CreateNextNodeId(DialogNodeType type)
        {
            string nodeId;

            do
            {
                nodeId = $"{type.ToString().ToLowerInvariant()}_{nextNodeNumber}";
                nextNodeNumber++;
            }
            while (ContainsNode(nodeId));

            return nodeId;
        }

        private void RemoveTransitionsReferencingNode(string nodeId)
        {
            for (var index = currentTransitions.Count - 1; index >= 0; index--)
            {
                var transition = currentTransitions[index];
                if (string.Equals(transition.FromNodeId, nodeId, StringComparison.Ordinal)
                    || string.Equals(transition.ToNodeId, nodeId, StringComparison.Ordinal))
                {
                    currentTransitions.RemoveAt(index);
                }
            }
        }

        private List<string> CreateIncompleteGraphErrors()
        {
            var errors = new List<string>();

            if (currentNodes.Count == 0)
            {
                errors.Add("Dialog graph has no nodes.");
            }

            if (string.IsNullOrWhiteSpace(currentStartNodeId))
            {
                errors.Add("Dialog graph start node is not set.");
            }
            else if (!ContainsNode(currentStartNodeId))
            {
                errors.Add($"Dialog graph start node '{currentStartNodeId}' does not exist.");
            }

            return errors;
        }

        private void EnsureKnownNode(string nodeId, string parameterName)
        {
            if (!ContainsNode(nodeId))
            {
                throw new ArgumentException($"Unknown dialog graph node id '{nodeId}'.", parameterName);
            }
        }

        private bool ContainsNode(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                return false;
            }

            foreach (var node in currentNodes)
            {
                if (string.Equals(node.Id, nodeId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsMatchingTransition(
            DialogTransition transition,
            string fromNodeId,
            string toNodeId,
            DialogTransitionCondition condition)
        {
            return string.Equals(transition.FromNodeId, fromNodeId, StringComparison.Ordinal)
                && string.Equals(transition.ToNodeId, toNodeId, StringComparison.Ordinal)
                && transition.Condition == condition;
        }
    }
}
