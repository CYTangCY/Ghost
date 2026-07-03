using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.IntentClassification
{
    public static class Act1GhostGeneralizationEngine
    {
        public static Act1GeneralizationResult Evaluate(
            IReadOnlyDictionary<string, string> cardIdToPileId,
            IReadOnlyDictionary<string, string> pileIdToIntentLabelId,
            Act1TeachingDemoData.TestMessage testMessage,
            IReadOnlyDictionary<string, string> replyLinesByIntentId)
        {
            if (cardIdToPileId == null)
            {
                throw new ArgumentNullException(nameof(cardIdToPileId));
            }

            if (pileIdToIntentLabelId == null)
            {
                throw new ArgumentNullException(nameof(pileIdToIntentLabelId));
            }

            if (testMessage == null)
            {
                throw new ArgumentNullException(nameof(testMessage));
            }

            if (replyLinesByIntentId == null)
            {
                throw new ArgumentNullException(nameof(replyLinesByIntentId));
            }

            var relatedCardsByPileId = CountRelatedCardsByPile(cardIdToPileId, testMessage.RelatedCardIds);
            if (relatedCardsByPileId.Count == 0)
            {
                return Act1GeneralizationResult.Confused(null, testMessage.RelatedCardIds);
            }

            var chosenPileId = ChoosePluralityPile(relatedCardsByPileId, out var hasTie);
            if (hasTie || string.IsNullOrWhiteSpace(chosenPileId))
            {
                return Act1GeneralizationResult.Confused(null, testMessage.RelatedCardIds);
            }

            if (!pileIdToIntentLabelId.TryGetValue(chosenPileId, out var repliedIntentId) ||
                string.IsNullOrWhiteSpace(repliedIntentId) ||
                !replyLinesByIntentId.TryGetValue(repliedIntentId, out var replyLine))
            {
                return Act1GeneralizationResult.Confused(
                    chosenPileId,
                    relatedCardsByPileId[chosenPileId]);
            }

            var isCorrect = string.Equals(repliedIntentId, testMessage.TrueIntentId, StringComparison.Ordinal);
            var misleadingCardIds = isCorrect
                ? CollectRelatedCardsNotInPile(cardIdToPileId, testMessage.RelatedCardIds, chosenPileId)
                : relatedCardsByPileId[chosenPileId];

            return new Act1GeneralizationResult(
                chosenPileId,
                repliedIntentId,
                replyLine,
                false,
                isCorrect,
                misleadingCardIds);
        }

        private static Dictionary<string, List<string>> CountRelatedCardsByPile(
            IReadOnlyDictionary<string, string> cardIdToPileId,
            IEnumerable<string> relatedCardIds)
        {
            var relatedCardsByPileId = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var cardId in relatedCardIds)
            {
                if (!cardIdToPileId.TryGetValue(cardId, out var pileId) || string.IsNullOrWhiteSpace(pileId))
                {
                    continue;
                }

                if (!relatedCardsByPileId.TryGetValue(pileId, out var cards))
                {
                    cards = new List<string>();
                    relatedCardsByPileId.Add(pileId, cards);
                }

                cards.Add(cardId);
            }

            return relatedCardsByPileId;
        }

        private static string ChoosePluralityPile(
            IReadOnlyDictionary<string, List<string>> relatedCardsByPileId,
            out bool hasTie)
        {
            hasTie = false;
            string chosenPileId = null;
            var bestCount = 0;

            foreach (var pair in relatedCardsByPileId)
            {
                var count = pair.Value.Count;
                if (count > bestCount)
                {
                    chosenPileId = pair.Key;
                    bestCount = count;
                    hasTie = false;
                    continue;
                }

                if (count == bestCount)
                {
                    hasTie = true;
                }
            }

            return chosenPileId;
        }

        private static IReadOnlyList<string> CollectRelatedCardsNotInPile(
            IReadOnlyDictionary<string, string> cardIdToPileId,
            IEnumerable<string> relatedCardIds,
            string chosenPileId)
        {
            var cardIds = new List<string>();
            foreach (var cardId in relatedCardIds)
            {
                if (!cardIdToPileId.TryGetValue(cardId, out var pileId) ||
                    !string.Equals(pileId, chosenPileId, StringComparison.Ordinal))
                {
                    cardIds.Add(cardId);
                }
            }

            return cardIds;
        }
    }

    public sealed class Act1GeneralizationResult
    {
        private readonly string[] misleadingCardIds;

        public Act1GeneralizationResult(
            string chosenPileId,
            string repliedIntentId,
            string replyLine,
            bool isConfused,
            bool isCorrect,
            IEnumerable<string> misleadingCardIds)
        {
            ChosenPileId = chosenPileId;
            RepliedIntentId = repliedIntentId;
            ReplyLine = replyLine ?? string.Empty;
            IsConfused = isConfused;
            IsCorrect = isCorrect;
            this.misleadingCardIds = misleadingCardIds == null
                ? Array.Empty<string>()
                : new List<string>(misleadingCardIds).ToArray();
        }

        public string ChosenPileId { get; }

        public string RepliedIntentId { get; }

        public string ReplyLine { get; }

        public bool IsConfused { get; }

        public bool IsCorrect { get; }

        public IReadOnlyList<string> MisleadingCardIds => misleadingCardIds;

        public static Act1GeneralizationResult Confused(
            string chosenPileId,
            IEnumerable<string> misleadingCardIds)
        {
            return new Act1GeneralizationResult(
                chosenPileId,
                null,
                "Ghost: ...I only learned scattered echoes.",
                true,
                false,
                misleadingCardIds);
        }
    }
}
