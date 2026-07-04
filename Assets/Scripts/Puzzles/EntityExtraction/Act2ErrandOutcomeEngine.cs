using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.EntityExtraction
{
    public static class Act2ErrandOutcomeEngine
    {
        public static Act2ErrandOutcome Evaluate(
            Act2ErrandDemoData.ErrandDefinition errand,
            IEnumerable<EntitySpan> playerSpans)
        {
            if (errand == null)
            {
                throw new ArgumentNullException(nameof(errand));
            }

            if (playerSpans == null)
            {
                throw new ArgumentNullException(nameof(playerSpans));
            }

            var submittedSpans = CopySpans(playerSpans);
            var slotResults = CreateSlotResults(errand, submittedSpans);
            var validatorResult = EntityExtractionValidator.Validate(errand.Message.CorrectSpans, submittedSpans);
            var isSuccess = validatorResult.IsCorrect && AllSlotsCorrect(slotResults);
            var outcomeLine = isSuccess
                ? errand.SuccessOutcomeLine
                : ChooseFailureLine(errand, slotResults);

            return new Act2ErrandOutcome(
                isSuccess,
                outcomeLine,
                isSuccess ? Act2ErrandGhostMood.Happy : ChooseFailureMood(slotResults),
                slotResults,
                validatorResult);
        }

        private static EntitySpan[] CopySpans(IEnumerable<EntitySpan> source)
        {
            var copied = new List<EntitySpan>();
            foreach (var span in source)
            {
                if (span == null)
                {
                    throw new ArgumentException("Player spans cannot contain null.", nameof(source));
                }

                copied.Add(span);
            }

            return copied.ToArray();
        }

        private static Act2ErrandSlotResult[] CreateSlotResults(
            Act2ErrandDemoData.ErrandDefinition errand,
            IReadOnlyList<EntitySpan> submittedSpans)
        {
            var results = new List<Act2ErrandSlotResult>();
            foreach (var slot in errand.Slots)
            {
                var expectedSpans = FindExpectedSpansForSlot(errand.Message.CorrectSpans, slot.EntityType);
                var submittedForSlot = FindSubmittedSpansForSlot(submittedSpans, slot.EntityType);
                var state = EvaluateSlotState(expectedSpans, submittedForSlot);
                var expectedSurface = expectedSpans.Count == 0
                    ? string.Empty
                    : expectedSpans[0].GetText(errand.Message.MessageText);
                var submittedSurface = submittedForSlot.Count == 0
                    ? string.Empty
                    : submittedForSlot[0].GetText(errand.Message.MessageText);
                var resolutionText = state == Act2ErrandSlotState.Correct
                    ? CreateResolutionText(slot.EntityType, expectedSurface)
                    : string.Empty;

                results.Add(new Act2ErrandSlotResult(
                    slot.SlotId,
                    slot.EntityType,
                    state,
                    expectedSurface,
                    submittedSurface,
                    resolutionText));
            }

            return results.ToArray();
        }

        private static List<EntitySpan> FindExpectedSpansForSlot(
            IEnumerable<EntitySpan> expectedSpans,
            EntityType slotType)
        {
            var spans = new List<EntitySpan>();
            foreach (var span in expectedSpans)
            {
                if (span.Type == slotType)
                {
                    spans.Add(span);
                }
            }

            return spans;
        }

        private static List<EntitySpan> FindSubmittedSpansForSlot(
            IEnumerable<EntitySpan> submittedSpans,
            EntityType slotType)
        {
            var spans = new List<EntitySpan>();
            foreach (var span in submittedSpans)
            {
                if (span.Type == slotType)
                {
                    spans.Add(span);
                }
            }

            return spans;
        }

        private static Act2ErrandSlotState EvaluateSlotState(
            IReadOnlyList<EntitySpan> expectedSpans,
            IReadOnlyList<EntitySpan> submittedSpans)
        {
            if (submittedSpans.Count == 0)
            {
                return Act2ErrandSlotState.Missing;
            }

            var result = EntityExtractionValidator.Validate(expectedSpans, submittedSpans);
            return result.IsCorrect
                ? Act2ErrandSlotState.Correct
                : Act2ErrandSlotState.Wrong;
        }

        private static bool AllSlotsCorrect(IEnumerable<Act2ErrandSlotResult> slotResults)
        {
            foreach (var slotResult in slotResults)
            {
                if (slotResult.State != Act2ErrandSlotState.Correct)
                {
                    return false;
                }
            }

            return true;
        }

        private static string ChooseFailureLine(
            Act2ErrandDemoData.ErrandDefinition errand,
            IEnumerable<Act2ErrandSlotResult> slotResults)
        {
            foreach (var slotResult in slotResults)
            {
                if (slotResult.State == Act2ErrandSlotState.Correct)
                {
                    continue;
                }

                return errand.GetFailureLine(slotResult.SlotId, slotResult.State);
            }

            return "Ghost has every visible slot filled, but one extra detail is still making the errand wobble.";
        }

        private static Act2ErrandGhostMood ChooseFailureMood(IEnumerable<Act2ErrandSlotResult> slotResults)
        {
            foreach (var slotResult in slotResults)
            {
                if (slotResult.State == Act2ErrandSlotState.Wrong)
                {
                    return Act2ErrandGhostMood.Confused;
                }
            }

            return Act2ErrandGhostMood.Sad;
        }

        private static string CreateResolutionText(EntityType entityType, string surfaceText)
        {
            foreach (var resolution in Act2ErrandDemoData.CreateSynonymResolutions())
            {
                if (resolution.Matches(entityType.Id, surfaceText))
                {
                    return surfaceText + " -> " + resolution.CanonicalLabel;
                }
            }

            return string.Empty;
        }
    }

    public sealed class Act2ErrandOutcome
    {
        private readonly Act2ErrandSlotResult[] slotResults;

        public Act2ErrandOutcome(
            bool isSuccess,
            string outcomeLine,
            Act2ErrandGhostMood mood,
            IEnumerable<Act2ErrandSlotResult> slotResults,
            EntityExtractionResult validatorResult)
        {
            if (slotResults == null)
            {
                throw new ArgumentNullException(nameof(slotResults));
            }

            IsSuccess = isSuccess;
            OutcomeLine = outcomeLine ?? string.Empty;
            Mood = mood;
            ValidatorResult = validatorResult ?? throw new ArgumentNullException(nameof(validatorResult));
            this.slotResults = CopySlotResults(slotResults);
        }

        public bool IsSuccess { get; }

        public string OutcomeLine { get; }

        public Act2ErrandGhostMood Mood { get; }

        public IReadOnlyList<Act2ErrandSlotResult> SlotResults => slotResults;

        public EntityExtractionResult ValidatorResult { get; }

        private static Act2ErrandSlotResult[] CopySlotResults(IEnumerable<Act2ErrandSlotResult> source)
        {
            var copied = new List<Act2ErrandSlotResult>();
            foreach (var slotResult in source)
            {
                if (slotResult == null)
                {
                    throw new ArgumentException("Slot results cannot contain null.", nameof(source));
                }

                copied.Add(slotResult);
            }

            return copied.ToArray();
        }
    }

    public sealed class Act2ErrandSlotResult
    {
        public Act2ErrandSlotResult(
            Act2ErrandSlotId slotId,
            EntityType entityType,
            Act2ErrandSlotState state,
            string expectedSurfaceText,
            string submittedSurfaceText,
            string resolutionText)
        {
            SlotId = slotId;
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            State = state;
            ExpectedSurfaceText = expectedSurfaceText ?? string.Empty;
            SubmittedSurfaceText = submittedSurfaceText ?? string.Empty;
            ResolutionText = resolutionText ?? string.Empty;
        }

        public Act2ErrandSlotId SlotId { get; }

        public EntityType EntityType { get; }

        public Act2ErrandSlotState State { get; }

        public string ExpectedSurfaceText { get; }

        public string SubmittedSurfaceText { get; }

        public string ResolutionText { get; }
    }
}
