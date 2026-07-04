using System;
using System.Collections.Generic;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Banter;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.EntityExtraction;

namespace Ghost.Presentation.Act2EntityExtraction
{
    public sealed class Act2EntityExtractionInteractionController
    {
        private const char ChipKeySeparator = ':';

        private readonly IReadOnlyList<Act2ErrandDemoData.ErrandDefinition> errands;
        private readonly Dictionary<string, TokenInfo> tokensByChipKey;
        private readonly Dictionary<Act2ErrandSlotId, SlotAssignment> assignmentsBySlotId;
        private readonly Dictionary<string, Act2ErrandSlotId> assignedSlotByChipKey;

        private EntityExtractionSession session;
        private int currentErrandIndex;

        public Act2EntityExtractionInteractionController()
        {
            errands = Act2ErrandDemoData.CreateErrands();
            tokensByChipKey = new Dictionary<string, TokenInfo>(StringComparer.Ordinal);
            assignmentsBySlotId = new Dictionary<Act2ErrandSlotId, SlotAssignment>();
            assignedSlotByChipKey = new Dictionary<string, Act2ErrandSlotId>(StringComparer.Ordinal);
            CurrentPhase = Act2ErrandPhase.Onboarding;
            CurrentMood = Act2ErrandGhostMood.Neutral;
            LoadCurrentErrand();
        }

        public event Action StateChanged;

        public Act2ErrandPhase CurrentPhase { get; private set; }

        public Act2ErrandGhostMood CurrentMood { get; private set; }

        public Act2ErrandDemoData.ErrandDefinition CurrentErrand => errands[currentErrandIndex];

        public int CurrentErrandNumber => currentErrandIndex + 1;

        public int ErrandCount => errands.Count;

        public string MessageText => CurrentErrand.Message.MessageText;

        public string OutcomeLine { get; private set; }

        public string SelectedChipKey { get; private set; }

        public bool HasSplitCurrentMessage { get; private set; }

        public Act2ErrandOutcome LastOutcome { get; private set; }

        public IReadOnlyList<TokenInfo> Tokens
        {
            get
            {
                var tokens = new List<TokenInfo>(tokensByChipKey.Values);
                tokens.Sort((left, right) => left.Start.CompareTo(right.Start));
                return tokens;
            }
        }

        public IReadOnlyList<EntitySpan> CurrentSpans => session.CurrentSpans;

        public static string CreateChipKey(int start, int length)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), "Chip start cannot be negative.");
            }

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Chip length must be greater than zero.");
            }

            return start + ChipKeySeparator.ToString() + length;
        }

        public void BeginAfterOnboarding()
        {
            if (CurrentPhase != Act2ErrandPhase.Onboarding)
            {
                return;
            }

            CurrentPhase = Act2ErrandPhase.IntroFail;
            CurrentMood = Act2ErrandGhostMood.Sad;
            OutcomeLine = CurrentErrand.IntroFailureOutcomeLine;
            NotifyStateChanged();
        }

        public void ReplayOnboarding()
        {
            CurrentPhase = Act2ErrandPhase.Onboarding;
            CurrentMood = Act2ErrandGhostMood.Neutral;
            OutcomeLine = string.Empty;
            NotifyStateChanged();
        }

        public void SplitMessage()
        {
            if (CurrentPhase != Act2ErrandPhase.IntroFail)
            {
                return;
            }

            HasSplitCurrentMessage = true;
            CurrentPhase = Act2ErrandPhase.Fill;
            CurrentMood = Act2ErrandGhostMood.Neutral;
            OutcomeLine = "The sentence breaks into tokens. Now Ghost's action card can catch the useful details.";
            NotifyStateChanged();
        }

        public void SelectToken(string chipKey)
        {
            EnsureKnownToken(chipKey);

            if (CurrentPhase != Act2ErrandPhase.Fill)
            {
                return;
            }

            SelectedChipKey = string.Equals(SelectedChipKey, chipKey, StringComparison.Ordinal)
                ? null
                : chipKey;
            NotifyStateChanged();
        }

        public void AssignSelectedTokenToSlot(Act2ErrandSlotId slotId)
        {
            if (string.IsNullOrWhiteSpace(SelectedChipKey))
            {
                return;
            }

            AssignTokenToSlot(SelectedChipKey, slotId);
        }

        public void AssignTokenToSlot(string chipKey, Act2ErrandSlotId slotId)
        {
            EnsureKnownToken(chipKey);
            var slot = FindCurrentSlot(slotId);
            if (slot == null || CurrentPhase != Act2ErrandPhase.Fill)
            {
                return;
            }

            if (assignmentsBySlotId.TryGetValue(slotId, out var existingForSlot) &&
                string.Equals(existingForSlot.ChipKey, chipKey, StringComparison.Ordinal))
            {
                RemoveSlotAssignment(slotId);
                SelectedChipKey = null;
                NotifyStateChanged();
                return;
            }

            if (assignedSlotByChipKey.TryGetValue(chipKey, out var previousSlotId))
            {
                RemoveSlotAssignment(previousSlotId);
            }

            if (assignmentsBySlotId.ContainsKey(slotId))
            {
                RemoveSlotAssignment(slotId);
            }

            var token = tokensByChipKey[chipKey];
            var span = new EntitySpan(token.Start, token.Length, slot.EntityType);
            session.AddSpan(span);
            assignmentsBySlotId[slotId] = new SlotAssignment(slotId, chipKey, token.Text, span);
            assignedSlotByChipKey[chipKey] = slotId;
            SelectedChipKey = null;
            LastOutcome = null;
            OutcomeLine = "Ghost writes " + token.Text + " into " + slot.DisplayName + ".";
            CurrentMood = Act2ErrandGhostMood.Neutral;
            NotifyStateChanged();
        }

        public void RemoveTokenAssignment(string chipKey)
        {
            EnsureKnownToken(chipKey);
            if (!assignedSlotByChipKey.TryGetValue(chipKey, out var slotId))
            {
                return;
            }

            RemoveSlotAssignment(slotId);
            SelectedChipKey = null;
            LastOutcome = null;
            OutcomeLine = "Ghost erases that slot and waits for the right detail.";
            CurrentMood = Act2ErrandGhostMood.Neutral;
            NotifyStateChanged();
        }

        public void RemoveSlotAssignment(Act2ErrandSlotId slotId)
        {
            if (!assignmentsBySlotId.TryGetValue(slotId, out var assignment))
            {
                return;
            }

            session.RemoveSpan(assignment.Span);
            assignmentsBySlotId.Remove(slotId);
            assignedSlotByChipKey.Remove(assignment.ChipKey);
        }

        public void RunErrand()
        {
            if (CurrentPhase != Act2ErrandPhase.Fill)
            {
                return;
            }

            LastOutcome = Act2ErrandOutcomeEngine.Evaluate(CurrentErrand, session.CurrentSpans);
            OutcomeLine = LastOutcome.OutcomeLine;
            CurrentMood = LastOutcome.Mood;
            CurrentPhase = LastOutcome.IsSuccess ? Act2ErrandPhase.Run : Act2ErrandPhase.Fill;

            GhostBackendClient.PostAttempt(
                GhostNarrativeState.Act2Id,
                GhostBackendClient.CreateAttemptResult(LastOutcome.IsSuccess),
                GhostBackendClient.CreateAttemptDetails(
                    "act2-ghost-errand",
                    LastOutcome.ValidatorResult.Errors,
                    CurrentErrand.ErrandId + " errand run"));

            if (!LastOutcome.IsSuccess)
            {
                AmbientBanterPanel.RequestHint(
                    GhostNarrativeState.Act2Id,
                    "after_incorrect_errand",
                    "The player ran an Act 2 errand with incorrect entity slots. Give a non-spoiler hint about finding the useful detail tokens.");
            }

            NotifyStateChanged();
        }

        public void ReviseCurrentErrand()
        {
            if (CurrentPhase != Act2ErrandPhase.Run || LastOutcome == null || LastOutcome.IsSuccess)
            {
                return;
            }

            CurrentPhase = Act2ErrandPhase.Fill;
            CurrentMood = Act2ErrandGhostMood.Neutral;
            NotifyStateChanged();
        }

        public void ContinueAfterSuccess()
        {
            if (CurrentPhase != Act2ErrandPhase.Run || LastOutcome == null || !LastOutcome.IsSuccess)
            {
                return;
            }

            if (currentErrandIndex >= errands.Count - 1)
            {
                CurrentPhase = Act2ErrandPhase.Complete;
                CurrentMood = Act2ErrandGhostMood.Happy;
                OutcomeLine = "Ghost finishes every errand: tokens became details, and the details made the action work.";
                NotifyStateChanged();
                return;
            }

            currentErrandIndex++;
            LoadCurrentErrand();
            CurrentPhase = Act2ErrandPhase.IntroFail;
            CurrentMood = Act2ErrandGhostMood.Sad;
            OutcomeLine = CurrentErrand.IntroFailureOutcomeLine;
            NotifyStateChanged();
        }

        public SlotAssignment GetAssignment(Act2ErrandSlotId slotId)
        {
            assignmentsBySlotId.TryGetValue(slotId, out var assignment);
            return assignment;
        }

        public Act2ErrandSlotId? GetAssignedSlot(string chipKey)
        {
            EnsureKnownToken(chipKey);
            return assignedSlotByChipKey.TryGetValue(chipKey, out var slotId)
                ? slotId
                : (Act2ErrandSlotId?)null;
        }

        public bool IsSelected(string chipKey)
        {
            EnsureKnownToken(chipKey);
            return string.Equals(SelectedChipKey, chipKey, StringComparison.Ordinal);
        }

        public Act2ErrandSlotResult GetSlotResult(Act2ErrandSlotId slotId)
        {
            if (LastOutcome == null)
            {
                return null;
            }

            foreach (var slotResult in LastOutcome.SlotResults)
            {
                if (slotResult.SlotId == slotId)
                {
                    return slotResult;
                }
            }

            return null;
        }

        private void LoadCurrentErrand()
        {
            session = EntityExtractionSession.CreateFromSampleMessage(CurrentErrand.Message);
            tokensByChipKey.Clear();
            assignmentsBySlotId.Clear();
            assignedSlotByChipKey.Clear();
            SelectedChipKey = null;
            HasSplitCurrentMessage = false;
            LastOutcome = null;
            OutcomeLine = string.Empty;

            foreach (var token in CreateWordTokens(MessageText))
            {
                tokensByChipKey.Add(token.ChipKey, token);
            }
        }

        private Act2ErrandDemoData.ErrandSlot FindCurrentSlot(Act2ErrandSlotId slotId)
        {
            foreach (var slot in CurrentErrand.Slots)
            {
                if (slot.SlotId == slotId)
                {
                    return slot;
                }
            }

            return null;
        }

        private void EnsureKnownToken(string chipKey)
        {
            ParseChipKey(chipKey, out _, out _);
            if (!tokensByChipKey.ContainsKey(chipKey))
            {
                throw new ArgumentException("Unknown token chip key: " + chipKey, nameof(chipKey));
            }
        }

        private static void ParseChipKey(string chipKey, out int start, out int length)
        {
            if (string.IsNullOrWhiteSpace(chipKey))
            {
                throw new ArgumentException("Chip key cannot be empty.", nameof(chipKey));
            }

            var separatorIndex = chipKey.IndexOf(ChipKeySeparator);
            if (separatorIndex <= 0 || separatorIndex >= chipKey.Length - 1)
            {
                throw new ArgumentException("Chip key must use the format Start:Length.", nameof(chipKey));
            }

            var startText = chipKey.Substring(0, separatorIndex);
            var lengthText = chipKey.Substring(separatorIndex + 1);
            if (!int.TryParse(startText, out start) || start < 0)
            {
                throw new ArgumentException("Chip key start must be a non-negative integer.", nameof(chipKey));
            }

            if (!int.TryParse(lengthText, out length) || length <= 0)
            {
                throw new ArgumentException("Chip key length must be a positive integer.", nameof(chipKey));
            }
        }

        private static IEnumerable<TokenInfo> CreateWordTokens(string messageText)
        {
            if (string.IsNullOrEmpty(messageText))
            {
                yield break;
            }

            var index = 0;
            while (index < messageText.Length)
            {
                while (index < messageText.Length && char.IsWhiteSpace(messageText[index]))
                {
                    index++;
                }

                if (index >= messageText.Length)
                {
                    yield break;
                }

                var rawStart = index;
                while (index < messageText.Length && !char.IsWhiteSpace(messageText[index]))
                {
                    index++;
                }

                var rawEnd = index - 1;
                var start = rawStart;
                var end = rawEnd;

                while (start <= end && !char.IsLetterOrDigit(messageText[start]))
                {
                    start++;
                }

                while (end >= start && !char.IsLetterOrDigit(messageText[end]))
                {
                    end--;
                }

                if (start > end)
                {
                    continue;
                }

                var length = end - start + 1;
                var chipKey = CreateChipKey(start, length);
                yield return new TokenInfo(chipKey, start, length, messageText.Substring(start, length));
            }
        }

        private void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }

        public sealed class TokenInfo
        {
            public TokenInfo(string chipKey, int start, int length, string text)
            {
                ChipKey = chipKey ?? string.Empty;
                Start = start;
                Length = length;
                Text = text ?? string.Empty;
            }

            public string ChipKey { get; }

            public int Start { get; }

            public int Length { get; }

            public string Text { get; }
        }

        public sealed class SlotAssignment
        {
            public SlotAssignment(
                Act2ErrandSlotId slotId,
                string chipKey,
                string tokenText,
                EntitySpan span)
            {
                SlotId = slotId;
                ChipKey = chipKey ?? string.Empty;
                TokenText = tokenText ?? string.Empty;
                Span = span ?? throw new ArgumentNullException(nameof(span));
            }

            public Act2ErrandSlotId SlotId { get; }

            public string ChipKey { get; }

            public string TokenText { get; }

            public EntitySpan Span { get; }
        }
    }

    public enum Act2ErrandPhase
    {
        Onboarding,
        IntroFail,
        Fill,
        Run,
        Complete
    }
}
