using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.DialogGraph
{
    public sealed class DialogGraph
    {
        private readonly DialogNode[] nodes;
        private readonly DialogTransition[] transitions;
        private readonly Dictionary<string, DialogNode> nodesById;
        private readonly Dictionary<string, List<DialogTransition>> outgoingTransitionsByNodeId;

        public DialogGraph(
            string startNodeId,
            IEnumerable<DialogNode> nodes,
            IEnumerable<DialogTransition> transitions)
        {
            if (string.IsNullOrWhiteSpace(startNodeId))
            {
                throw new ArgumentException("Dialog graph start node id cannot be empty.", nameof(startNodeId));
            }

            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (transitions == null)
            {
                throw new ArgumentNullException(nameof(transitions));
            }

            StartNodeId = startNodeId;
            nodesById = new Dictionary<string, DialogNode>(StringComparer.Ordinal);
            outgoingTransitionsByNodeId = new Dictionary<string, List<DialogTransition>>(StringComparer.Ordinal);
            this.nodes = CopyNodes(nodes);

            if (!nodesById.ContainsKey(StartNodeId))
            {
                throw new ArgumentException($"Dialog graph start node '{StartNodeId}' does not exist.", nameof(startNodeId));
            }

            this.transitions = CopyTransitions(transitions);
        }

        public string StartNodeId { get; }

        public IReadOnlyList<DialogNode> Nodes => nodes;

        public IReadOnlyList<DialogTransition> Transitions => transitions;

        public DialogNode GetNode(string nodeId)
        {
            if (nodeId == null)
            {
                return null;
            }

            nodesById.TryGetValue(nodeId, out var node);
            return node;
        }

        public bool ContainsNode(string nodeId)
        {
            return nodeId != null && nodesById.ContainsKey(nodeId);
        }

        public IReadOnlyList<DialogTransition> GetOutgoingTransitions(string nodeId)
        {
            if (nodeId == null || !outgoingTransitionsByNodeId.TryGetValue(nodeId, out var outgoingTransitions))
            {
                return Array.Empty<DialogTransition>();
            }

            return outgoingTransitions.ToArray();
        }

        private DialogNode[] CopyNodes(IEnumerable<DialogNode> source)
        {
            var copiedNodes = new List<DialogNode>();

            foreach (var node in source)
            {
                if (node == null)
                {
                    throw new ArgumentException("Dialog graph cannot contain a null node.", nameof(source));
                }

                if (nodesById.ContainsKey(node.Id))
                {
                    throw new ArgumentException($"Dialog graph contains duplicate node id '{node.Id}'.", nameof(source));
                }

                copiedNodes.Add(node);
                nodesById.Add(node.Id, node);
            }

            if (copiedNodes.Count == 0)
            {
                throw new ArgumentException("Dialog graph must contain at least one node.", nameof(source));
            }

            return copiedNodes.ToArray();
        }

        private DialogTransition[] CopyTransitions(IEnumerable<DialogTransition> source)
        {
            var copiedTransitions = new List<DialogTransition>();

            foreach (var transition in source)
            {
                if (transition == null)
                {
                    throw new ArgumentException("Dialog graph cannot contain a null transition.", nameof(source));
                }

                copiedTransitions.Add(transition);

                if (!outgoingTransitionsByNodeId.TryGetValue(transition.FromNodeId, out var outgoingTransitions))
                {
                    outgoingTransitions = new List<DialogTransition>();
                    outgoingTransitionsByNodeId.Add(transition.FromNodeId, outgoingTransitions);
                }

                outgoingTransitions.Add(transition);
            }

            return copiedTransitions.ToArray();
        }
    }
}
