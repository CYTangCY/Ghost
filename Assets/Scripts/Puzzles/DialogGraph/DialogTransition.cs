using System;

namespace Ghost.Puzzles.DialogGraph
{
    public enum DialogTransitionCondition
    {
        Always,
        SlotPresent,
        SlotMissing
    }

    [Serializable]
    public sealed class DialogTransition
    {
        public DialogTransition(string fromNodeId, string toNodeId, DialogTransitionCondition condition)
        {
            if (string.IsNullOrWhiteSpace(fromNodeId))
            {
                throw new ArgumentException("Transition source node id cannot be empty.", nameof(fromNodeId));
            }

            if (string.IsNullOrWhiteSpace(toNodeId))
            {
                throw new ArgumentException("Transition target node id cannot be empty.", nameof(toNodeId));
            }

            FromNodeId = fromNodeId;
            ToNodeId = toNodeId;
            Condition = condition;
        }

        public string FromNodeId { get; }

        public string ToNodeId { get; }

        public DialogTransitionCondition Condition { get; }
    }
}
