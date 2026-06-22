using System;

namespace Ghost.Puzzles.DialogGraph
{
    public static class DialogGraphSimulator
    {
        public static DialogSimulationResult Simulate(DialogGraph graph, ConversationTurn turn, DialogContext context)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (turn == null)
            {
                throw new ArgumentNullException(nameof(turn));
            }

            var updatedContext = context ?? new DialogContext();
            var currentNode = graph.GetNode(graph.StartNodeId);
            var stepLimit = graph.Nodes.Count + 1;

            for (var step = 0; step < stepLimit; step++)
            {
                if (currentNode == null)
                {
                    return new DialogSimulationResult(null, updatedContext, false);
                }

                if (currentNode.Type == DialogNodeType.Response)
                {
                    return new DialogSimulationResult(currentNode.ResponseId, updatedContext, false);
                }

                var nextTransition = FindNextTransition(graph, currentNode, turn, updatedContext);
                if (nextTransition == null)
                {
                    return new DialogSimulationResult(null, updatedContext, false);
                }

                currentNode = graph.GetNode(nextTransition.ToNodeId);
            }

            return new DialogSimulationResult(null, updatedContext, true);
        }

        private static DialogTransition FindNextTransition(
            DialogGraph graph,
            DialogNode currentNode,
            ConversationTurn turn,
            DialogContext context)
        {
            switch (currentNode.Type)
            {
                case DialogNodeType.Start:
                    return FindStartTransition(graph, currentNode, turn.IntentId);

                case DialogNodeType.IntentBranch:
                    if (!string.Equals(currentNode.IntentId, turn.IntentId, StringComparison.Ordinal))
                    {
                        return null;
                    }

                    return FindFirstTransition(graph, currentNode.Id, DialogTransitionCondition.Always);

                case DialogNodeType.SlotCheck:
                    return FindSlotTransition(graph, currentNode, turn, context);

                default:
                    return null;
            }
        }

        private static DialogTransition FindStartTransition(DialogGraph graph, DialogNode currentNode, string intentId)
        {
            foreach (var transition in graph.GetOutgoingTransitions(currentNode.Id))
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
                    return transition;
                }
            }

            return null;
        }

        private static DialogTransition FindSlotTransition(
            DialogGraph graph,
            DialogNode currentNode,
            ConversationTurn turn,
            DialogContext context)
        {
            var requiredEntityType = currentNode.RequiredEntityType;
            var hasSlot = false;

            if (turn.TryGetEntityValue(requiredEntityType, out var turnEntityValue))
            {
                context.SetSlot(requiredEntityType, turnEntityValue);
                hasSlot = true;
            }
            else if (context.ContainsSlot(requiredEntityType))
            {
                hasSlot = true;
            }

            var condition = hasSlot
                ? DialogTransitionCondition.SlotPresent
                : DialogTransitionCondition.SlotMissing;

            return FindFirstTransition(graph, currentNode.Id, condition);
        }

        private static DialogTransition FindFirstTransition(
            DialogGraph graph,
            string nodeId,
            DialogTransitionCondition condition)
        {
            foreach (var transition in graph.GetOutgoingTransitions(nodeId))
            {
                if (transition.Condition == condition)
                {
                    return transition;
                }
            }

            return null;
        }
    }

    public sealed class DialogSimulationResult
    {
        internal DialogSimulationResult(string responseId, DialogContext updatedContext, bool stepLimitReached)
        {
            ResponseId = responseId;
            UpdatedContext = updatedContext ?? new DialogContext();
            StepLimitReached = stepLimitReached;
        }

        public string ResponseId { get; }

        public DialogContext UpdatedContext { get; }

        public bool StepLimitReached { get; }
    }
}
