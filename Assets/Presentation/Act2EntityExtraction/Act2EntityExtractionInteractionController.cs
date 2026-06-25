using System;
using System.Collections.Generic;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.EntityExtraction;

namespace Ghost.Presentation.Act2EntityExtraction
{
    public sealed class Act2EntityExtractionInteractionController
    {
        private const char ChipKeySeparator = ':';

        private readonly EntityExtractionSession session;
        private readonly Dictionary<string, EntityType> assignedTypesByChipKey;

        public Act2EntityExtractionInteractionController()
        {
            var sampleMessage = Act2EntityExtractionSampleData.CreateMessages()[0];
            session = EntityExtractionSession.CreateFromSampleMessage(sampleMessage);
            assignedTypesByChipKey = new Dictionary<string, EntityType>(StringComparer.Ordinal);
        }

        public event Action StateChanged;

        public event Action<string, bool> FeedbackChanged;

        public string MessageText => session.MessageText;

        public IReadOnlyList<EntitySpan> CurrentSpans => session.CurrentSpans;

        public string SelectedChipKey { get; private set; }

        public bool HasSelectedChip => !string.IsNullOrEmpty(SelectedChipKey);

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

        public void SelectChip(string chipKey)
        {
            EnsureValidChipKey(chipKey);

            if (assignedTypesByChipKey.ContainsKey(chipKey))
            {
                ClearSelectionIfNeeded();
                return;
            }

            var nextSelection = string.Equals(SelectedChipKey, chipKey, StringComparison.Ordinal)
                ? null
                : chipKey;

            if (string.Equals(SelectedChipKey, nextSelection, StringComparison.Ordinal))
            {
                return;
            }

            SelectedChipKey = nextSelection;
            NotifyStateChanged();
        }

        public void AssignSelectedChipToType(EntityType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!HasSelectedChip)
            {
                return;
            }

            var chipKey = SelectedChipKey;
            ParseChipKey(chipKey, out var start, out var length);

            session.AddSpan(start, length, type);
            assignedTypesByChipKey[chipKey] = type;
            SelectedChipKey = null;
            NotifyStateChanged();
        }

        public void UntagChip(string chipKey)
        {
            EnsureValidChipKey(chipKey);

            var changed = false;
            if (assignedTypesByChipKey.TryGetValue(chipKey, out var assignedType))
            {
                ParseChipKey(chipKey, out var start, out var length);
                session.RemoveSpan(new EntitySpan(start, length, assignedType));
                assignedTypesByChipKey.Remove(chipKey);
                changed = true;
            }

            if (string.Equals(SelectedChipKey, chipKey, StringComparison.Ordinal))
            {
                SelectedChipKey = null;
                changed = true;
            }

            if (changed)
            {
                NotifyStateChanged();
            }
        }

        public EntityType GetAssignedType(string chipKey)
        {
            EnsureValidChipKey(chipKey);
            assignedTypesByChipKey.TryGetValue(chipKey, out var assignedType);
            return assignedType;
        }

        public bool IsSelected(string chipKey)
        {
            EnsureValidChipKey(chipKey);
            return string.Equals(SelectedChipKey, chipKey, StringComparison.Ordinal);
        }

        public EntityExtractionResult ValidateCurrentState()
        {
            var result = session.ValidateCurrentState();
            GhostBackendClient.PostAttempt(
                GhostNarrativeState.Act2Id,
                GhostBackendClient.CreateAttemptResult(result.IsCorrect),
                GhostBackendClient.CreateAttemptDetails(
                    "act2-entity-extraction",
                    result.Errors,
                    "Entity extraction validation"));

            var feedbackMessage = result.IsCorrect
                ? "All key details are tagged with the right entity types."
                : CreateIncorrectFeedbackMessage(result.Errors.Count);

            FeedbackChanged?.Invoke(feedbackMessage, result.IsCorrect);
            return result;
        }

        private void ClearSelectionIfNeeded()
        {
            if (!HasSelectedChip)
            {
                return;
            }

            SelectedChipKey = null;
            NotifyStateChanged();
        }

        private static void EnsureValidChipKey(string chipKey)
        {
            ParseChipKey(chipKey, out _, out _);
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

        private void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }

        private static string CreateIncorrectFeedbackMessage(int issueCount)
        {
            if (issueCount <= 0)
            {
                return "Not yet. Ghost is still missing or mistagging a detail.";
            }

            if (issueCount == 1)
            {
                return "Not yet. Ghost is still missing or mistagging a detail. 1 issue found.";
            }

            return $"Not yet. Ghost is still missing or mistagging a detail. {issueCount} issues found.";
        }
    }
}
