using System;
using Ghost.Puzzles.EntityExtraction;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act2EntityExtractionSessionTests
    {
        [Test]
        public void CreateFromSampleMessage_StartsWithNoCurrentSpansAndValidatesIncorrect()
        {
            var sample = CreateSampleMessage();

            var session = EntityExtractionSession.CreateFromSampleMessage(sample);
            var result = session.ValidateCurrentState();

            Assert.That(session.MessageText, Is.EqualTo(sample.MessageText));
            Assert.That(session.CurrentSpans, Is.Empty);
            Assert.That(result.IsCorrect, Is.False);
        }

        [Test]
        public void ValidateCurrentState_WhenAllCorrectSpansAdded_ReturnsCorrect()
        {
            var sample = CreateSampleMessage();
            var session = EntityExtractionSession.CreateFromSampleMessage(sample);

            foreach (var span in sample.CorrectSpans)
            {
                session.AddSpan(span.Start, span.Length, span.Type);
            }

            var result = session.ValidateCurrentState();

            Assert.That(result.IsCorrect, Is.True, string.Join("\n", result.Errors));
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void RemoveSpan_WhenSpanWasAdded_RemovesItAndStateBecomesIncorrect()
        {
            var sample = CreateSampleMessage();
            var session = EntityExtractionSession.CreateFromSampleMessage(sample);
            var span = sample.CorrectSpans[0];
            session.AddSpan(span);

            var wasRemoved = session.RemoveSpan(span);
            var result = session.ValidateCurrentState();

            Assert.That(wasRemoved, Is.True);
            Assert.That(session.CurrentSpans, Is.Empty);
            Assert.That(result.IsCorrect, Is.False);
        }

        [Test]
        public void AddSpan_WhenSpanExtendsPastMessageBounds_Throws()
        {
            var sample = CreateSampleMessage();
            var session = EntityExtractionSession.CreateFromSampleMessage(sample);
            var roomType = Act2EntityExtractionSampleData.CreateRoomEntityType();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                session.AddSpan(sample.MessageText.Length, 1, roomType));
        }

        [Test]
        public void AddSpan_WhenExactDuplicateIsAdded_LeavesCurrentSpanCountUnchanged()
        {
            var sample = CreateSampleMessage();
            var session = EntityExtractionSession.CreateFromSampleMessage(sample);
            var span = sample.CorrectSpans[0];

            session.AddSpan(span);
            session.AddSpan(span);

            Assert.That(session.CurrentSpans.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveSpan_WhenSpanWasNeverAdded_ReturnsFalse()
        {
            var sample = CreateSampleMessage();
            var session = EntityExtractionSession.CreateFromSampleMessage(sample);
            var missingSpan = new EntitySpan(0, 5, Act2EntityExtractionSampleData.CreateObjectEntityType());

            var wasRemoved = session.RemoveSpan(missingSpan);

            Assert.That(wasRemoved, Is.False);
            Assert.That(session.CurrentSpans, Is.Empty);
        }

        private static Act2EntityExtractionSampleData.SampleMessage CreateSampleMessage()
        {
            return Act2EntityExtractionSampleData.CreateMessages()[0];
        }
    }
}
