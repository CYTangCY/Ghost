using System;

namespace Ghost.Puzzles.DialogGraph
{
    [Serializable]
    public sealed class DialogNode
    {
        public DialogNode(
            string id,
            DialogNodeType type,
            string intentId = null,
            string requiredEntityType = null,
            string responseId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Dialog node id cannot be empty.", nameof(id));
            }

            Id = id;
            Type = type;
            IntentId = intentId ?? string.Empty;
            RequiredEntityType = requiredEntityType ?? string.Empty;
            ResponseId = responseId ?? string.Empty;

            ValidateRequiredConfig();
        }

        public string Id { get; }

        public DialogNodeType Type { get; }

        public string IntentId { get; }

        public string RequiredEntityType { get; }

        public string ResponseId { get; }

        private void ValidateRequiredConfig()
        {
            switch (Type)
            {
                case DialogNodeType.IntentBranch:
                    if (string.IsNullOrWhiteSpace(IntentId))
                    {
                        throw new ArgumentException("IntentBranch nodes require a non-empty intent id.");
                    }

                    break;

                case DialogNodeType.SlotCheck:
                    if (string.IsNullOrWhiteSpace(RequiredEntityType))
                    {
                        throw new ArgumentException("SlotCheck nodes require a non-empty entity type id.");
                    }

                    break;

                case DialogNodeType.Response:
                    if (string.IsNullOrWhiteSpace(ResponseId))
                    {
                        throw new ArgumentException("Response nodes require a non-empty response id.");
                    }

                    break;
            }
        }
    }
}
