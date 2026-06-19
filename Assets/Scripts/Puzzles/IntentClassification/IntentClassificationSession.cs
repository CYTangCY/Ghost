using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.IntentClassification
{
    public sealed class IntentClassificationSession
    {
        private readonly List<IntentCard> cards;
        private readonly Dictionary<string, IntentCard> cardsById;
        private readonly List<string> unassignedCardIds;
        private readonly Dictionary<string, List<string>> cardIdsByGroupId;
        private readonly Dictionary<string, string> groupIdByCardId;

        public IntentClassificationSession(IEnumerable<IntentCard> cards)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards));
            }

            this.cards = new List<IntentCard>();
            cardsById = new Dictionary<string, IntentCard>(StringComparer.Ordinal);
            unassignedCardIds = new List<string>();
            cardIdsByGroupId = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            groupIdByCardId = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var card in cards)
            {
                if (card == null)
                {
                    throw new ArgumentException("Intent classification session cannot contain a null card.", nameof(cards));
                }

                if (cardsById.ContainsKey(card.Id))
                {
                    throw new ArgumentException($"Duplicate card id '{card.Id}' cannot be added to a session.", nameof(cards));
                }

                this.cards.Add(card);
                cardsById.Add(card.Id, card);
                unassignedCardIds.Add(card.Id);
            }

            if (this.cards.Count == 0)
            {
                throw new ArgumentException("Intent classification session requires at least one card.", nameof(cards));
            }
        }

        public IReadOnlyList<IntentCard> Cards => cards.ToArray();

        public IReadOnlyList<string> UnassignedCardIds => unassignedCardIds.ToArray();

        public IReadOnlyList<string> AssignedGroupIds
        {
            get
            {
                var groupIds = new List<string>(cardIdsByGroupId.Keys);
                return groupIds.ToArray();
            }
        }

        public static IntentClassificationSession CreateFromSampleData()
        {
            return new IntentClassificationSession(Act1IntentClassificationSampleData.CreateCards());
        }

        public void MoveCardToGroup(string cardId, string groupId)
        {
            EnsureKnownCardId(cardId);
            EnsureValidGroupId(groupId);
            RemoveCardFromCurrentLocation(cardId);

            if (!cardIdsByGroupId.TryGetValue(groupId, out var groupCardIds))
            {
                groupCardIds = new List<string>();
                cardIdsByGroupId.Add(groupId, groupCardIds);
            }

            groupCardIds.Add(cardId);
            groupIdByCardId[cardId] = groupId;
        }

        public void MoveCardToUnassigned(string cardId)
        {
            EnsureKnownCardId(cardId);

            if (!groupIdByCardId.ContainsKey(cardId))
            {
                return;
            }

            RemoveCardFromCurrentLocation(cardId);
            unassignedCardIds.Add(cardId);
        }

        public string GetAssignedGroupId(string cardId)
        {
            EnsureKnownCardId(cardId);
            groupIdByCardId.TryGetValue(cardId, out var groupId);
            return groupId;
        }

        public IReadOnlyList<string> GetAssignedCardIds(string groupId)
        {
            EnsureValidGroupId(groupId);

            if (!cardIdsByGroupId.TryGetValue(groupId, out var groupCardIds))
            {
                return Array.Empty<string>();
            }

            return groupCardIds.ToArray();
        }

        public IReadOnlyList<IReadOnlyList<string>> CreateSubmittedGroups()
        {
            var groups = new List<IReadOnlyList<string>>();

            foreach (var groupCardIds in cardIdsByGroupId.Values)
            {
                if (groupCardIds.Count > 0)
                {
                    groups.Add(groupCardIds.ToArray());
                }
            }

            return groups.ToArray();
        }

        public IntentClassificationResult ValidateCurrentState()
        {
            return IntentClassificationValidator.Validate(cards, CreateSubmittedGroups());
        }

        private void RemoveCardFromCurrentLocation(string cardId)
        {
            unassignedCardIds.Remove(cardId);

            if (!groupIdByCardId.TryGetValue(cardId, out var currentGroupId))
            {
                return;
            }

            var currentGroupCardIds = cardIdsByGroupId[currentGroupId];
            currentGroupCardIds.Remove(cardId);
            groupIdByCardId.Remove(cardId);

            if (currentGroupCardIds.Count == 0)
            {
                cardIdsByGroupId.Remove(currentGroupId);
            }
        }

        private void EnsureKnownCardId(string cardId)
        {
            if (string.IsNullOrWhiteSpace(cardId))
            {
                throw new ArgumentException("Card id cannot be empty.", nameof(cardId));
            }

            if (!cardsById.ContainsKey(cardId))
            {
                throw new ArgumentException($"Unknown card id '{cardId}'.", nameof(cardId));
            }
        }

        private static void EnsureValidGroupId(string groupId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                throw new ArgumentException("Group id cannot be empty.", nameof(groupId));
            }
        }
    }
}
