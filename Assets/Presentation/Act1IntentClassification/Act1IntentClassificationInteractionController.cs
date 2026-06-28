using System;
using System.Collections.Generic;
using Ghost.Presentation.Banter;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.IntentClassification;

namespace Ghost.Presentation.Act1IntentClassification
{
    public sealed class Act1IntentClassificationInteractionController
    {
        private readonly List<IntentCard> cards;
        private readonly IntentClassificationSession session;

        public Act1IntentClassificationInteractionController(IEnumerable<IntentCard> cards)
        {
            this.cards = new List<IntentCard>(cards);
            session = new IntentClassificationSession(this.cards);
            CurrentFeedback = Act1IntentClassificationFeedback.Neutral(
                "Group by intent: what the speaker wants. Click or drag cards, then Validate.");
        }

        public event Action StateChanged;

        public event Action<Act1IntentClassificationFeedback> FeedbackChanged;

        public IReadOnlyList<IntentCard> Cards => cards.ToArray();

        public string SelectedCardId { get; private set; }

        public bool HasSelectedCard => !string.IsNullOrEmpty(SelectedCardId);

        public Act1IntentClassificationFeedback CurrentFeedback { get; private set; }

        public string GetAssignedGroupId(string cardId)
        {
            return session.GetAssignedGroupId(cardId);
        }

        public IReadOnlyList<string> GetAssignedCardIds(string groupId)
        {
            return session.GetAssignedCardIds(groupId);
        }

        public void SelectCard(string cardId)
        {
            SelectedCardId = SelectedCardId == cardId ? null : cardId;
            NotifyStateChanged();
        }

        public void AssignSelectedCardToIntent(string intentId)
        {
            if (!HasSelectedCard)
            {
                return;
            }

            session.MoveCardToGroup(SelectedCardId, intentId);
            SelectedCardId = null;
            SetFeedback(Act1IntentClassificationFeedback.Neutral(
                "Assignment updated. Keep grouping by purpose, then Validate."));
            NotifyStateChanged();
        }

        public void AssignCardToIntent(string cardId, string intentId)
        {
            SelectedCardId = cardId;
            AssignSelectedCardToIntent(intentId);
        }

        public void MoveAssignedCardToUnassigned(string cardId)
        {
            session.MoveCardToUnassigned(cardId);

            if (SelectedCardId == cardId)
            {
                SelectedCardId = null;
            }

            SetFeedback(Act1IntentClassificationFeedback.Neutral(
                "Card moved back to unassigned. Try another intent group when ready."));
            NotifyStateChanged();
        }

        public void ValidateCurrentGrouping()
        {
            var result = session.ValidateCurrentState();
            GhostBackendClient.PostAttempt(
                GhostNarrativeState.Act1Id,
                GhostBackendClient.CreateAttemptResult(result.IsCorrect),
                GhostBackendClient.CreateAttemptDetails(
                    "act1-intent-classification",
                    result.Errors,
                    "Intent grouping validation"));

            if (result.IsCorrect)
            {
                SetFeedback(Act1IntentClassificationFeedback.Correct(BuildCorrectFeedbackMessage()));
                return;
            }

            SetFeedback(Act1IntentClassificationFeedback.Incorrect(
                "Some messages are in the wrong group. Compare what the speaker wants."));
            AmbientBanterPanel.RequestHint(
                GhostNarrativeState.Act1Id,
                "after_incorrect_validate",
                "The player validated an incorrect intent grouping. Error count: " + result.Errors.Count + ". Give a non-spoiler hint about grouping by purpose.");
        }

        private string BuildCorrectFeedbackMessage()
        {
            return "Ghost brightens: one purpose per group, even with different words.\n" +
                BuildTrainingExampleSummary() + "\n" +
                "Lily: Um... this is how we spot common requests before planning the chatbot.";
        }

        private string BuildTrainingExampleSummary()
        {
            var exampleCountsByIntent = new Dictionary<string, int>(StringComparer.Ordinal);
            var totalExampleCount = 0;

            foreach (var card in cards)
            {
                if (card == null ||
                    string.IsNullOrWhiteSpace(card.IntentId) ||
                    string.IsNullOrWhiteSpace(card.MessageText))
                {
                    continue;
                }

                if (!exampleCountsByIntent.ContainsKey(card.IntentId))
                {
                    exampleCountsByIntent.Add(card.IntentId, 0);
                }

                exampleCountsByIntent[card.IntentId]++;
                totalExampleCount++;
            }

            if (exampleCountsByIntent.Count == 0 || totalExampleCount == 0)
            {
                return "Each intent group now has varied message phrasings, so those cards are the chatbot's training examples.";
            }

            var intentLabel = exampleCountsByIntent.Count == 1 ? "intent" : "intents";
            var exampleLabel = totalExampleCount == 1 ? "example phrasing" : "example phrasings";
            return "Training examples: these " + totalExampleCount + " " + exampleLabel +
                " can teach a chatbot " + exampleCountsByIntent.Count + " " + intentLabel + ".";
        }

        private void SetFeedback(Act1IntentClassificationFeedback feedback)
        {
            CurrentFeedback = feedback;
            FeedbackChanged?.Invoke(CurrentFeedback);
        }

        private void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }
    }

    public sealed class Act1IntentClassificationFeedback
    {
        private Act1IntentClassificationFeedback(Act1IntentClassificationFeedbackKind kind, string message)
        {
            Kind = kind;
            Message = message ?? string.Empty;
        }

        public Act1IntentClassificationFeedbackKind Kind { get; }

        public string Message { get; }

        public static Act1IntentClassificationFeedback Neutral(string message)
        {
            return new Act1IntentClassificationFeedback(Act1IntentClassificationFeedbackKind.Neutral, message);
        }

        public static Act1IntentClassificationFeedback Correct(string message)
        {
            return new Act1IntentClassificationFeedback(Act1IntentClassificationFeedbackKind.Correct, message);
        }

        public static Act1IntentClassificationFeedback Incorrect(string message)
        {
            return new Act1IntentClassificationFeedback(Act1IntentClassificationFeedbackKind.Incorrect, message);
        }
    }

    public enum Act1IntentClassificationFeedbackKind
    {
        Neutral,
        Correct,
        Incorrect
    }
}
