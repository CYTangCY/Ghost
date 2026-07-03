using System;
using System.Collections.Generic;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.IntentClassification;

namespace Ghost.Presentation.Act1IntentClassification
{
    public sealed class Act1IntentClassificationInteractionController
    {
        private readonly List<IntentCard> cards;
        private readonly Dictionary<string, IntentCard> cardsById;
        private readonly List<Act1IntentPileState> piles;
        private readonly List<string> unpiledCardIds;
        private readonly Dictionary<string, string> pileIdByCardId;
        private readonly Dictionary<string, string> pileIdByLabelIntentId;
        private readonly IReadOnlyList<Act1TeachingDemoData.IntroFailure> introFailures;
        private readonly IReadOnlyList<Act1TeachingDemoData.TestMessage> testMessages;
        private readonly IReadOnlyDictionary<string, string> replyLinesByIntentId;
        private readonly HashSet<string> highlightedCardIds;

        private int nextPileNumber = 1;
        private int introIndex;
        private int demoIndex;
        private Act1GeneralizationResult currentDemoResult;

        public Act1IntentClassificationInteractionController(IEnumerable<IntentCard> cards)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards));
            }

            this.cards = new List<IntentCard>(cards);
            if (this.cards.Count == 0)
            {
                throw new ArgumentException("Act 1 teaching controller requires cards.", nameof(cards));
            }

            cardsById = new Dictionary<string, IntentCard>(StringComparer.Ordinal);
            unpiledCardIds = new List<string>();
            foreach (var card in this.cards)
            {
                cardsById.Add(card.Id, card);
                unpiledCardIds.Add(card.Id);
            }

            piles = new List<Act1IntentPileState>();
            pileIdByCardId = new Dictionary<string, string>(StringComparer.Ordinal);
            pileIdByLabelIntentId = new Dictionary<string, string>(StringComparer.Ordinal);
            highlightedCardIds = new HashSet<string>(StringComparer.Ordinal);
            introFailures = Act1TeachingDemoData.CreateIntroFailures();
            testMessages = Act1TeachingDemoData.CreateTestMessages();
            replyLinesByIntentId = Act1TeachingDemoData.CreateReplyLines();
            Phase = Act1TeachingPhase.Intro;
            CurrentFeedback = Act1IntentClassificationFeedback.Neutral(
                "Watch Ghost try exact-word replies, then help it learn visitor purposes.");
        }

        public event Action StateChanged;

        public event Action<Act1IntentClassificationFeedback> FeedbackChanged;

        public IReadOnlyList<IntentCard> Cards => cards.ToArray();

        public IReadOnlyList<string> UnpiledCardIds => unpiledCardIds.ToArray();

        public IReadOnlyList<Act1IntentPileState> Piles => piles.ToArray();

        public IReadOnlyList<Act1TeachingDemoData.TestMessage> TestMessages => testMessages;

        public Act1TeachingPhase Phase { get; private set; }

        public string SelectedCardId { get; private set; }

        public string SelectedLabelIntentId { get; private set; }

        public bool HasSelectedCard => !string.IsNullOrEmpty(SelectedCardId);

        public bool HasSelectedLabel => !string.IsNullOrEmpty(SelectedLabelIntentId);

        public int CurrentDemoIndex => demoIndex;

        public Act1GeneralizationResult CurrentDemoResult => currentDemoResult;

        public Act1IntentClassificationFeedback CurrentFeedback { get; private set; }

        public Act1ConversationBeat GetCurrentConversationBeat()
        {
            if (Phase == Act1TeachingPhase.Intro)
            {
                if (introIndex < introFailures.Count)
                {
                    var failure = introFailures[introIndex];
                    return new Act1ConversationBeat(
                        failure.VisitorLine,
                        failure.GhostWrongReply,
                        "Ghost is matching words, not purpose.",
                        true,
                        "Next");
                }

                return new Act1ConversationBeat(
                    "Lily",
                    "Um... it memorizes sentences, but it doesn't understand what the visitor wants.",
                    "Cluster the transcripts, label the purposes, then teach Ghost.",
                    true,
                    "Help Ghost");
            }

            if (Phase == Act1TeachingPhase.Demo && demoIndex < testMessages.Count && currentDemoResult != null)
            {
                return new Act1ConversationBeat(
                    testMessages[demoIndex].Text,
                    currentDemoResult.ReplyLine,
                    currentDemoResult.IsCorrect
                        ? "Ghost used your pile as training examples and answered the unseen visitor."
                        : "Ghost followed the pile that looked most related. Fix the training pile and teach again.",
                    true,
                    demoIndex >= testMessages.Count - 1 ? "Finish demo" : "Next visitor");
            }

            if (Phase == Act1TeachingPhase.Complete)
            {
                return new Act1ConversationBeat(
                    "Visitor: I need help again.",
                    "Ghost: I can tell what kind of help you want now.",
                    "Your piles became intent training examples Ghost can use.",
                    false,
                    string.Empty);
            }

            return new Act1ConversationBeat(
                "Training table",
                "Drag transcript cards into piles, then put one purpose label on each pile.",
                "Press Teach Ghost to test your training on unseen messages.",
                false,
                string.Empty);
        }

        public void AdvanceConversation()
        {
            if (Phase == Act1TeachingPhase.Intro)
            {
                introIndex++;
                if (introIndex > introFailures.Count)
                {
                    Phase = Act1TeachingPhase.Build;
                    SetFeedback(Act1IntentClassificationFeedback.Neutral(
                        "Build training piles first. A pile without a purpose label cannot teach Ghost yet."));
                }

                NotifyStateChanged();
                return;
            }

            if (Phase == Act1TeachingPhase.Demo)
            {
                AdvanceDemo();
            }
        }

        public void SelectCard(string cardId)
        {
            EnsureKnownCard(cardId);
            SelectedLabelIntentId = null;
            SelectedCardId = string.Equals(SelectedCardId, cardId, StringComparison.Ordinal)
                ? null
                : cardId;
            NotifyStateChanged();
        }

        public void SelectLabel(string intentId)
        {
            EnsureKnownIntentLabel(intentId);
            SelectedCardId = null;
            SelectedLabelIntentId = string.Equals(SelectedLabelIntentId, intentId, StringComparison.Ordinal)
                ? null
                : intentId;
            NotifyStateChanged();
        }

        public void MoveSelectedCardToNewPile()
        {
            if (!HasSelectedCard)
            {
                return;
            }

            MoveCardToNewPile(SelectedCardId);
        }

        public void MoveCardToNewPile(string cardId)
        {
            EnsureKnownCard(cardId);
            var pile = new Act1IntentPileState(CreatePileId());
            piles.Add(pile);
            MoveCardIntoPile(cardId, pile);
            SelectedCardId = null;
            BeginBuildAfterEdit("New training pile created. Add more matching transcripts or give it a purpose label.");
        }

        public void MoveSelectedCardToPile(string pileId)
        {
            if (!HasSelectedCard)
            {
                return;
            }

            MoveCardToPile(SelectedCardId, pileId);
        }

        public void MoveCardToPile(string cardId, string pileId)
        {
            EnsureKnownCard(cardId);
            var pile = FindPile(pileId);
            MoveCardIntoPile(cardId, pile);
            SelectedCardId = null;
            BeginBuildAfterEdit("Training pile updated. Teach Ghost when the piles look purposeful.");
        }

        public void MoveCardToUnpiled(string cardId)
        {
            EnsureKnownCard(cardId);
            RemoveCardFromCurrentLocation(cardId);
            if (!unpiledCardIds.Contains(cardId))
            {
                unpiledCardIds.Add(cardId);
            }

            SelectedCardId = null;
            BeginBuildAfterEdit("Transcript moved back out. Ghost will ignore it until it is in a labelled pile.");
        }

        public void AssignSelectedLabelToPile(string pileId)
        {
            if (!HasSelectedLabel)
            {
                return;
            }

            AssignLabelToPile(SelectedLabelIntentId, pileId);
        }

        public void AssignLabelToPile(string intentId, string pileId)
        {
            EnsureKnownIntentLabel(intentId);
            var pile = FindPile(pileId);

            if (pileIdByLabelIntentId.TryGetValue(intentId, out var oldPileId))
            {
                FindPile(oldPileId).SetIntentLabel(null);
                pileIdByLabelIntentId.Remove(intentId);
            }

            if (!string.IsNullOrEmpty(pile.IntentLabelId))
            {
                pileIdByLabelIntentId.Remove(pile.IntentLabelId);
            }

            pile.SetIntentLabel(intentId);
            pileIdByLabelIntentId[intentId] = pile.Id;
            SelectedLabelIntentId = null;
            BeginBuildAfterEdit("Purpose label attached. That pile can now teach Ghost one intent.");
        }

        public void ClearPileLabel(string pileId)
        {
            var pile = FindPile(pileId);
            if (!string.IsNullOrEmpty(pile.IntentLabelId))
            {
                pileIdByLabelIntentId.Remove(pile.IntentLabelId);
                pile.SetIntentLabel(null);
                BeginBuildAfterEdit("Purpose label removed. This pile will not teach Ghost until labelled.");
            }
        }

        public void TeachGhost()
        {
            demoIndex = 0;
            Phase = Act1TeachingPhase.Demo;
            EvaluateCurrentDemoMessage();
            NotifyStateChanged();
        }

        public void ReturnToBuild()
        {
            Phase = Act1TeachingPhase.Build;
            currentDemoResult = null;
            highlightedCardIds.Clear();
            SetFeedback(Act1IntentClassificationFeedback.Neutral(
                "Revise the piles, then teach Ghost again."));
            NotifyStateChanged();
        }

        public bool IsCardHighlighted(string cardId)
        {
            return highlightedCardIds.Contains(cardId);
        }

        public IntentCard GetCard(string cardId)
        {
            EnsureKnownCard(cardId);
            return cardsById[cardId];
        }

        private void AdvanceDemo()
        {
            if (demoIndex < testMessages.Count - 1)
            {
                demoIndex++;
                EvaluateCurrentDemoMessage();
                NotifyStateChanged();
                return;
            }

            CompleteDemoAttempt();
        }

        private void EvaluateCurrentDemoMessage()
        {
            highlightedCardIds.Clear();
            currentDemoResult = Act1GhostGeneralizationEngine.Evaluate(
                BuildCardPileMap(),
                BuildPileLabelMap(),
                testMessages[demoIndex],
                replyLinesByIntentId);

            if (!currentDemoResult.IsCorrect)
            {
                foreach (var cardId in currentDemoResult.MisleadingCardIds)
                {
                    highlightedCardIds.Add(cardId);
                }
            }

            SetFeedback(currentDemoResult.IsCorrect
                ? Act1IntentClassificationFeedback.Correct("Ghost answered this unseen visitor from your training pile.")
                : Act1IntentClassificationFeedback.Incorrect("Ghost is still being misled by the highlighted training examples."));
        }

        private void CompleteDemoAttempt()
        {
            var validation = ValidatePilesWithExistingValidator();
            var allDemoMessagesCorrect = AreAllDemoMessagesCorrect();
            GhostBackendClient.PostAttempt(
                GhostNarrativeState.Act1Id,
                GhostBackendClient.CreateAttemptResult(validation.IsCorrect && allDemoMessagesCorrect),
                GhostBackendClient.CreateAttemptDetails(
                    "act1-teaching-generalization",
                    validation.Errors,
                    "Intent pile training and unseen-message demo"));

            if (validation.IsCorrect && allDemoMessagesCorrect)
            {
                Phase = Act1TeachingPhase.Complete;
                highlightedCardIds.Clear();
                SetFeedback(Act1IntentClassificationFeedback.Correct(
                    "Training works: Ghost can use your piles to answer new visitors."));
                NotifyStateChanged();
                return;
            }

            Phase = Act1TeachingPhase.Build;
            currentDemoResult = null;
            SetFeedback(Act1IntentClassificationFeedback.Incorrect(
                "Not stable yet. Fix the highlighted or split piles, then teach Ghost again."));
            NotifyStateChanged();
        }

        private bool AreAllDemoMessagesCorrect()
        {
            var cardPileMap = BuildCardPileMap();
            var pileLabelMap = BuildPileLabelMap();
            foreach (var testMessage in testMessages)
            {
                var result = Act1GhostGeneralizationEngine.Evaluate(
                    cardPileMap,
                    pileLabelMap,
                    testMessage,
                    replyLinesByIntentId);
                if (!result.IsCorrect)
                {
                    return false;
                }
            }

            return true;
        }

        private IntentClassificationResult ValidatePilesWithExistingValidator()
        {
            var submittedGroups = new List<IReadOnlyList<string>>();
            foreach (var pile in piles)
            {
                if (!string.IsNullOrEmpty(pile.IntentLabelId) && pile.CardIds.Count > 0)
                {
                    submittedGroups.Add(pile.CardIds);
                }
            }

            return IntentClassificationValidator.Validate(cards, submittedGroups);
        }

        private Dictionary<string, string> BuildCardPileMap()
        {
            return new Dictionary<string, string>(pileIdByCardId, StringComparer.Ordinal);
        }

        private Dictionary<string, string> BuildPileLabelMap()
        {
            var labels = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var pile in piles)
            {
                if (!string.IsNullOrEmpty(pile.IntentLabelId))
                {
                    labels[pile.Id] = pile.IntentLabelId;
                }
            }

            return labels;
        }

        private void MoveCardIntoPile(string cardId, Act1IntentPileState pile)
        {
            RemoveCardFromCurrentLocation(cardId);
            pile.AddCard(cardId);
            pileIdByCardId[cardId] = pile.Id;
        }

        private void RemoveCardFromCurrentLocation(string cardId)
        {
            unpiledCardIds.Remove(cardId);
            if (!pileIdByCardId.TryGetValue(cardId, out var oldPileId))
            {
                return;
            }

            var oldPile = FindPile(oldPileId);
            oldPile.RemoveCard(cardId);
            pileIdByCardId.Remove(cardId);

            if (oldPile.CardIds.Count == 0 && string.IsNullOrEmpty(oldPile.IntentLabelId))
            {
                piles.Remove(oldPile);
            }
        }

        private void BeginBuildAfterEdit(string message)
        {
            Phase = Act1TeachingPhase.Build;
            currentDemoResult = null;
            highlightedCardIds.Clear();
            SetFeedback(Act1IntentClassificationFeedback.Neutral(message));
            NotifyStateChanged();
        }

        private string CreatePileId()
        {
            return "pile-" + nextPileNumber++;
        }

        private Act1IntentPileState FindPile(string pileId)
        {
            if (string.IsNullOrWhiteSpace(pileId))
            {
                throw new ArgumentException("Pile id cannot be empty.", nameof(pileId));
            }

            foreach (var pile in piles)
            {
                if (string.Equals(pile.Id, pileId, StringComparison.Ordinal))
                {
                    return pile;
                }
            }

            throw new ArgumentException("Unknown pile id '" + pileId + "'.", nameof(pileId));
        }

        private void EnsureKnownCard(string cardId)
        {
            if (string.IsNullOrWhiteSpace(cardId) || !cardsById.ContainsKey(cardId))
            {
                throw new ArgumentException("Unknown card id '" + cardId + "'.", nameof(cardId));
            }
        }

        private static void EnsureKnownIntentLabel(string intentId)
        {
            if (intentId != Act1IntentClassificationSampleData.FindItemIntentId &&
                intentId != Act1IntentClassificationSampleData.AskLocationIntentId &&
                intentId != Act1IntentClassificationSampleData.AskIdentityIntentId)
            {
                throw new ArgumentException("Unknown purpose label id '" + intentId + "'.", nameof(intentId));
            }
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

    public enum Act1TeachingPhase
    {
        Intro,
        Build,
        Demo,
        Complete
    }

    public sealed class Act1IntentPileState
    {
        private readonly List<string> cardIds = new List<string>();

        public Act1IntentPileState(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Pile id cannot be empty.", nameof(id));
            }

            Id = id;
        }

        public string Id { get; }

        public string IntentLabelId { get; private set; }

        public IReadOnlyList<string> CardIds => cardIds.ToArray();

        public void SetIntentLabel(string intentLabelId)
        {
            IntentLabelId = intentLabelId;
        }

        public void AddCard(string cardId)
        {
            if (!cardIds.Contains(cardId))
            {
                cardIds.Add(cardId);
            }
        }

        public void RemoveCard(string cardId)
        {
            cardIds.Remove(cardId);
        }
    }

    public readonly struct Act1ConversationBeat
    {
        public Act1ConversationBeat(
            string visitorLine,
            string ghostReply,
            string note,
            bool hasAdvanceButton,
            string advanceButtonText)
        {
            VisitorLine = visitorLine ?? string.Empty;
            GhostReply = ghostReply ?? string.Empty;
            Note = note ?? string.Empty;
            HasAdvanceButton = hasAdvanceButton;
            AdvanceButtonText = advanceButtonText ?? string.Empty;
        }

        public string VisitorLine { get; }

        public string GhostReply { get; }

        public string Note { get; }

        public bool HasAdvanceButton { get; }

        public string AdvanceButtonText { get; }
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
