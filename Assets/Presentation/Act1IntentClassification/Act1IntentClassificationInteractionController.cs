using System;
using System.Collections.Generic;
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
                "Group all cards, then click Validate.");
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
                "Assignment changed. Click Validate when ready."));
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
                "Card returned to unassigned. Click Validate when ready."));
            NotifyStateChanged();
        }

        public void ValidateCurrentGrouping()
        {
            var result = session.ValidateCurrentState();
            if (result.IsCorrect)
            {
                SetFeedback(Act1IntentClassificationFeedback.Correct(
                    "Correct grouping. Ghost can react to the right purpose now."));
                return;
            }

            SetFeedback(Act1IntentClassificationFeedback.Incorrect(
                "Incorrect grouping. " + result.Errors.Count + " issue(s) to fix."));
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
