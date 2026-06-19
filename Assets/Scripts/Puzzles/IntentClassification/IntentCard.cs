using System;

namespace Ghost.Puzzles.IntentClassification
{
    [Serializable]
    public sealed class IntentCard
    {
        public string Id { get; }
        public string MessageText { get; }
        public string IntentId { get; }

        public IntentCard(string id, string messageText, string intentId)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Intent card id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(intentId))
            {
                throw new ArgumentException("Intent card intent id cannot be empty.", nameof(intentId));
            }

            Id = id;
            MessageText = messageText ?? string.Empty;
            IntentId = intentId;
        }
    }
}
