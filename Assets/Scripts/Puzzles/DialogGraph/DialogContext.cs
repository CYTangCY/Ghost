using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.DialogGraph
{
    public sealed class DialogContext
    {
        private readonly Dictionary<string, string> filledSlots;

        public DialogContext()
        {
            filledSlots = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        public DialogContext(IReadOnlyDictionary<string, string> filledSlots)
            : this()
        {
            if (filledSlots == null)
            {
                return;
            }

            foreach (var slot in filledSlots)
            {
                SetSlot(slot.Key, slot.Value);
            }
        }

        public IReadOnlyDictionary<string, string> FilledSlots => new Dictionary<string, string>(filledSlots, StringComparer.Ordinal);

        public bool TryGetSlot(string entityTypeId, out string value)
        {
            if (entityTypeId == null)
            {
                value = null;
                return false;
            }

            return filledSlots.TryGetValue(entityTypeId, out value);
        }

        public bool ContainsSlot(string entityTypeId)
        {
            return entityTypeId != null && filledSlots.ContainsKey(entityTypeId);
        }

        public void SetSlot(string entityTypeId, string value)
        {
            if (string.IsNullOrWhiteSpace(entityTypeId))
            {
                throw new ArgumentException("Dialog context slot entity type id cannot be empty.", nameof(entityTypeId));
            }

            filledSlots[entityTypeId] = value ?? string.Empty;
        }
    }
}
