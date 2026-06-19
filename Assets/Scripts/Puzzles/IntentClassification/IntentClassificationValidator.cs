using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.IntentClassification
{
    public static class IntentClassificationValidator
    {
        public static IntentClassificationResult Validate(
            IEnumerable<IntentCard> cards,
            IEnumerable<IEnumerable<string>> submittedGroups)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards));
            }

            if (submittedGroups == null)
            {
                throw new ArgumentNullException(nameof(submittedGroups));
            }

            var errors = new List<string>();
            var cardsById = BuildCardLookup(cards, errors);
            var seenCardIds = new HashSet<string>(StringComparer.Ordinal);
            var intentToGroupNumber = new Dictionary<string, int>(StringComparer.Ordinal);

            var groupNumber = 0;
            foreach (var group in submittedGroups)
            {
                groupNumber++;
                ValidateSubmittedGroup(
                    group,
                    groupNumber,
                    cardsById,
                    seenCardIds,
                    intentToGroupNumber,
                    errors);
            }

            if (groupNumber == 0)
            {
                errors.Add("No submitted groups were provided.");
            }

            foreach (var cardId in cardsById.Keys)
            {
                if (!seenCardIds.Contains(cardId))
                {
                    errors.Add($"Card '{cardId}' was not submitted.");
                }
            }

            return new IntentClassificationResult(errors.Count == 0, errors);
        }

        private static Dictionary<string, IntentCard> BuildCardLookup(
            IEnumerable<IntentCard> cards,
            ICollection<string> errors)
        {
            var cardsById = new Dictionary<string, IntentCard>(StringComparer.Ordinal);

            foreach (var card in cards)
            {
                if (card == null)
                {
                    errors.Add("Level contains a null intent card.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(card.Id))
                {
                    errors.Add("Level contains an intent card with an empty id.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(card.IntentId))
                {
                    errors.Add($"Card '{card.Id}' has an empty intent id.");
                    continue;
                }

                if (cardsById.ContainsKey(card.Id))
                {
                    errors.Add($"Level contains duplicate card id '{card.Id}'.");
                    continue;
                }

                cardsById.Add(card.Id, card);
            }

            if (cardsById.Count == 0)
            {
                errors.Add("Level contains no intent cards.");
            }

            return cardsById;
        }

        private static void ValidateSubmittedGroup(
            IEnumerable<string> group,
            int groupNumber,
            IReadOnlyDictionary<string, IntentCard> cardsById,
            ISet<string> seenCardIds,
            IDictionary<string, int> intentToGroupNumber,
            ICollection<string> errors)
        {
            if (group == null)
            {
                errors.Add($"Submitted group {groupNumber} is null.");
                return;
            }

            var groupCardIds = new HashSet<string>(StringComparer.Ordinal);
            string groupIntentId = null;
            var hasAnyCard = false;

            foreach (var cardId in group)
            {
                hasAnyCard = true;

                if (string.IsNullOrWhiteSpace(cardId))
                {
                    errors.Add($"Submitted group {groupNumber} contains an empty card id.");
                    continue;
                }

                if (!groupCardIds.Add(cardId))
                {
                    errors.Add($"Submitted group {groupNumber} contains duplicate card id '{cardId}'.");
                    continue;
                }

                if (!seenCardIds.Add(cardId))
                {
                    errors.Add($"Card '{cardId}' appears in more than one submitted position.");
                    continue;
                }

                if (!cardsById.TryGetValue(cardId, out var card))
                {
                    errors.Add($"Submitted group {groupNumber} contains unknown card id '{cardId}'.");
                    continue;
                }

                if (groupIntentId == null)
                {
                    groupIntentId = card.IntentId;
                    continue;
                }

                if (!string.Equals(groupIntentId, card.IntentId, StringComparison.Ordinal))
                {
                    errors.Add(
                        $"Submitted group {groupNumber} mixes intent '{groupIntentId}' with intent '{card.IntentId}'.");
                }
            }

            if (!hasAnyCard)
            {
                errors.Add($"Submitted group {groupNumber} is empty.");
            }

            if (groupIntentId == null)
            {
                return;
            }

            if (intentToGroupNumber.TryGetValue(groupIntentId, out var existingGroupNumber))
            {
                errors.Add(
                    $"Intent '{groupIntentId}' is split across submitted groups {existingGroupNumber} and {groupNumber}.");
                return;
            }

            intentToGroupNumber.Add(groupIntentId, groupNumber);
        }
    }

    public sealed class IntentClassificationResult
    {
        private readonly string[] errors;

        internal IntentClassificationResult(bool isCorrect, IEnumerable<string> errors)
        {
            IsCorrect = isCorrect;
            this.errors = errors == null ? Array.Empty<string>() : new List<string>(errors).ToArray();
        }

        public bool IsCorrect { get; }

        public IReadOnlyList<string> Errors => errors;
    }
}
