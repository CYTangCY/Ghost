using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.DialogGraph
{
    public static class DialogGraphValidator
    {
        public static DialogGraphResult Validate(DialogGraph graph, IEnumerable<DialogGraphTestCase> testCases)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (testCases == null)
            {
                throw new ArgumentNullException(nameof(testCases));
            }

            var errors = new List<string>();
            var copiedTestCases = CopyTestCases(testCases, errors);

            ValidateStartNode(graph, errors);
            ValidateTransitionEndpoints(graph, errors);
            ValidateReachability(graph, errors);
            ValidateDeadEnds(graph, errors);
            ValidateTestCaseIntents(graph, copiedTestCases, errors);
            ValidateTestCaseResponses(graph, copiedTestCases, errors);

            return new DialogGraphResult(errors.Count == 0, errors);
        }

        private static List<DialogGraphTestCase> CopyTestCases(
            IEnumerable<DialogGraphTestCase> testCases,
            ICollection<string> errors)
        {
            var copiedTestCases = new List<DialogGraphTestCase>();

            foreach (var testCase in testCases)
            {
                if (testCase == null)
                {
                    errors.Add("Dialog graph test cases contain a null test case.");
                    continue;
                }

                copiedTestCases.Add(testCase);
            }

            if (copiedTestCases.Count == 0)
            {
                errors.Add("No dialog graph test cases were provided.");
            }

            return copiedTestCases;
        }

        private static void ValidateStartNode(DialogGraph graph, ICollection<string> errors)
        {
            if (!graph.ContainsNode(graph.StartNodeId))
            {
                errors.Add($"Start node '{graph.StartNodeId}' does not exist.");
            }
        }

        private static void ValidateTransitionEndpoints(DialogGraph graph, ICollection<string> errors)
        {
            foreach (var transition in graph.Transitions)
            {
                if (!graph.ContainsNode(transition.FromNodeId))
                {
                    errors.Add($"Transition from '{transition.FromNodeId}' starts at an unknown node.");
                }

                if (!graph.ContainsNode(transition.ToNodeId))
                {
                    errors.Add($"Transition to '{transition.ToNodeId}' targets an unknown node.");
                }
            }
        }

        private static void ValidateReachability(DialogGraph graph, ICollection<string> errors)
        {
            var reachableNodeIds = new HashSet<string>(StringComparer.Ordinal);
            var pendingNodeIds = new Queue<string>();

            reachableNodeIds.Add(graph.StartNodeId);
            pendingNodeIds.Enqueue(graph.StartNodeId);

            while (pendingNodeIds.Count > 0)
            {
                var nodeId = pendingNodeIds.Dequeue();

                foreach (var transition in graph.GetOutgoingTransitions(nodeId))
                {
                    if (!graph.ContainsNode(transition.ToNodeId))
                    {
                        continue;
                    }

                    if (reachableNodeIds.Add(transition.ToNodeId))
                    {
                        pendingNodeIds.Enqueue(transition.ToNodeId);
                    }
                }
            }

            foreach (var node in graph.Nodes)
            {
                if (!reachableNodeIds.Contains(node.Id))
                {
                    errors.Add($"Node '{node.Id}' is unreachable from start node '{graph.StartNodeId}'.");
                }
            }
        }

        private static void ValidateDeadEnds(DialogGraph graph, ICollection<string> errors)
        {
            foreach (var node in graph.Nodes)
            {
                switch (node.Type)
                {
                    case DialogNodeType.Start:
                        if (!HasUsableStartTransition(graph, node))
                        {
                            errors.Add($"Start node '{node.Id}' has no usable intent branch transition.");
                        }

                        break;

                    case DialogNodeType.IntentBranch:
                        if (!HasUsableTransition(graph, node.Id, DialogTransitionCondition.Always))
                        {
                            errors.Add($"Intent branch node '{node.Id}' has no usable outgoing transition.");
                        }

                        break;

                    case DialogNodeType.SlotCheck:
                        if (!HasUsableTransition(graph, node.Id, DialogTransitionCondition.SlotPresent))
                        {
                            errors.Add($"Slot check node '{node.Id}' has no usable SlotPresent transition.");
                        }

                        if (!HasUsableTransition(graph, node.Id, DialogTransitionCondition.SlotMissing))
                        {
                            errors.Add($"Slot check node '{node.Id}' has no usable SlotMissing transition.");
                        }

                        break;
                }
            }
        }

        private static bool HasUsableStartTransition(DialogGraph graph, DialogNode startNode)
        {
            foreach (var transition in graph.GetOutgoingTransitions(startNode.Id))
            {
                if (transition.Condition != DialogTransitionCondition.Always)
                {
                    continue;
                }

                var targetNode = graph.GetNode(transition.ToNodeId);
                if (targetNode != null && targetNode.Type == DialogNodeType.IntentBranch)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasUsableTransition(
            DialogGraph graph,
            string nodeId,
            DialogTransitionCondition condition)
        {
            foreach (var transition in graph.GetOutgoingTransitions(nodeId))
            {
                if (transition.Condition == condition && graph.ContainsNode(transition.ToNodeId))
                {
                    return true;
                }
            }

            return false;
        }

        private static void ValidateTestCaseIntents(
            DialogGraph graph,
            IEnumerable<DialogGraphTestCase> testCases,
            ICollection<string> errors)
        {
            var checkedIntentIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var testCase in testCases)
            {
                if (!checkedIntentIds.Add(testCase.Turn.IntentId))
                {
                    continue;
                }

                if (!IsIntentHandled(graph, testCase.Turn.IntentId))
                {
                    errors.Add($"Intent '{testCase.Turn.IntentId}' is not handled from the start node.");
                }
            }
        }

        private static bool IsIntentHandled(DialogGraph graph, string intentId)
        {
            foreach (var transition in graph.GetOutgoingTransitions(graph.StartNodeId))
            {
                if (transition.Condition != DialogTransitionCondition.Always)
                {
                    continue;
                }

                var targetNode = graph.GetNode(transition.ToNodeId);
                if (targetNode == null || targetNode.Type != DialogNodeType.IntentBranch)
                {
                    continue;
                }

                if (string.Equals(targetNode.IntentId, intentId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static void ValidateTestCaseResponses(
            DialogGraph graph,
            IEnumerable<DialogGraphTestCase> testCases,
            ICollection<string> errors)
        {
            foreach (var testCase in testCases)
            {
                var simulationResult = DialogGraphSimulator.Simulate(graph, testCase.Turn, new DialogContext());
                if (simulationResult.StepLimitReached)
                {
                    errors.Add($"Simulation for {FormatTestCase(testCase)} reached the step limit before a response.");
                    continue;
                }

                if (!string.Equals(simulationResult.ResponseId, testCase.ExpectedResponseId, StringComparison.Ordinal))
                {
                    errors.Add(
                        $"Simulation for {FormatTestCase(testCase)} expected response '{testCase.ExpectedResponseId}' but reached '{simulationResult.ResponseId ?? "none"}'.");
                }
            }
        }

        private static string FormatTestCase(DialogGraphTestCase testCase)
        {
            if (!string.IsNullOrWhiteSpace(testCase.Id))
            {
                return $"test case '{testCase.Id}'";
            }

            return $"intent '{testCase.Turn.IntentId}' expecting response '{testCase.ExpectedResponseId}'";
        }
    }

    public sealed class DialogGraphResult
    {
        private readonly string[] errors;

        internal DialogGraphResult(bool isCorrect, IEnumerable<string> errors)
        {
            IsCorrect = isCorrect;
            this.errors = errors == null ? Array.Empty<string>() : new List<string>(errors).ToArray();
        }

        public bool IsCorrect { get; }

        public IReadOnlyList<string> Errors => errors;
    }

    public sealed class DialogGraphTestCase
    {
        public DialogGraphTestCase(ConversationTurn turn, string expectedResponseId)
            : this(string.Empty, turn, expectedResponseId)
        {
        }

        public DialogGraphTestCase(string id, ConversationTurn turn, string expectedResponseId)
        {
            if (string.IsNullOrWhiteSpace(expectedResponseId))
            {
                throw new ArgumentException("Dialog graph test case expected response id cannot be empty.", nameof(expectedResponseId));
            }

            Id = id ?? string.Empty;
            Turn = turn ?? throw new ArgumentNullException(nameof(turn));
            ExpectedResponseId = expectedResponseId;
        }

        public string Id { get; }

        public ConversationTurn Turn { get; }

        public string ExpectedResponseId { get; }
    }
}
