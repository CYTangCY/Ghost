using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.EntityExtraction
{
    public sealed class EntityExtractionSession
    {
        private readonly EntitySpan[] expectedSpans;
        private readonly List<EntitySpan> currentSpans;

        public EntityExtractionSession(string messageText, IEnumerable<EntitySpan> expectedSpans)
        {
            if (expectedSpans == null)
            {
                throw new ArgumentNullException(nameof(expectedSpans));
            }

            MessageText = messageText ?? string.Empty;
            this.expectedSpans = CopyExpectedSpans(expectedSpans);
            currentSpans = new List<EntitySpan>();
        }

        public string MessageText { get; }

        public IReadOnlyList<EntitySpan> CurrentSpans => currentSpans.ToArray();

        public static EntityExtractionSession CreateFromSampleMessage(Act2EntityExtractionSampleData.SampleMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return new EntityExtractionSession(message.MessageText, message.CorrectSpans);
        }

        /// <summary>
        /// Adds a player span to the current state. Exact duplicates are ignored, so adding the same
        /// start, length, and type twice leaves the current span count unchanged.
        /// </summary>
        public void AddSpan(EntitySpan span)
        {
            if (span == null)
            {
                throw new ArgumentNullException(nameof(span));
            }

            EnsureSpanFitsMessage(span);

            if (ContainsSpan(currentSpans, span))
            {
                return;
            }

            currentSpans.Add(span);
        }

        /// <summary>
        /// Creates and adds a player span. The span must fit inside MessageText; a boundary that
        /// extends past the message throws ArgumentOutOfRangeException.
        /// </summary>
        public void AddSpan(int start, int length, EntityType type)
        {
            AddSpan(new EntitySpan(start, length, type));
        }

        /// <summary>
        /// Removes a player span from the current state. If the span is absent, including null, the
        /// method returns false and leaves the state unchanged.
        /// </summary>
        public bool RemoveSpan(EntitySpan span)
        {
            if (span == null)
            {
                return false;
            }

            for (var index = 0; index < currentSpans.Count; index++)
            {
                if (currentSpans[index] == span)
                {
                    currentSpans.RemoveAt(index);
                    return true;
                }
            }

            return false;
        }

        public EntityExtractionResult ValidateCurrentState()
        {
            return EntityExtractionValidator.Validate(expectedSpans, CurrentSpans);
        }

        private static EntitySpan[] CopyExpectedSpans(IEnumerable<EntitySpan> source)
        {
            var spans = new List<EntitySpan>();

            foreach (var span in source)
            {
                if (span == null)
                {
                    throw new ArgumentException("Entity extraction session cannot contain a null expected span.", nameof(source));
                }

                spans.Add(span);
            }

            return spans.ToArray();
        }

        private void EnsureSpanFitsMessage(EntitySpan span)
        {
            if (span.Start > MessageText.Length || span.Length > MessageText.Length - span.Start)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(span),
                    "Entity span must fit inside the session message text.");
            }
        }

        private static bool ContainsSpan(IEnumerable<EntitySpan> spans, EntitySpan target)
        {
            foreach (var span in spans)
            {
                if (span == target)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
