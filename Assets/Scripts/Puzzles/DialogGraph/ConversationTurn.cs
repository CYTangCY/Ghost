using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.DialogGraph
{
    public sealed class ConversationTurn
    {
        private readonly Dictionary<string, string> entities;

        public ConversationTurn(string intentId, IReadOnlyDictionary<string, string> entities)
        {
            if (string.IsNullOrWhiteSpace(intentId))
            {
                throw new ArgumentException("Conversation turn intent id cannot be empty.", nameof(intentId));
            }

            IntentId = intentId;
            this.entities = CopyEntities(entities);
        }

        public string IntentId { get; }

        public IReadOnlyDictionary<string, string> Entities => entities;

        public bool TryGetEntityValue(string entityTypeId, out string value)
        {
            if (entityTypeId == null)
            {
                value = null;
                return false;
            }

            return entities.TryGetValue(entityTypeId, out value);
        }

        private static Dictionary<string, string> CopyEntities(IReadOnlyDictionary<string, string> source)
        {
            var copiedEntities = new Dictionary<string, string>(StringComparer.Ordinal);

            if (source == null)
            {
                return copiedEntities;
            }

            foreach (var entity in source)
            {
                if (string.IsNullOrWhiteSpace(entity.Key))
                {
                    throw new ArgumentException("Conversation turn cannot contain an empty entity type id.", nameof(source));
                }

                copiedEntities.Add(entity.Key, entity.Value ?? string.Empty);
            }

            return copiedEntities;
        }
    }
}
