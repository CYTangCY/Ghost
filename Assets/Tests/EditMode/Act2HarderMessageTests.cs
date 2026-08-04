using System.Collections.Generic;
using System.Linq;
using Ghost.Puzzles.EntityExtraction;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    /// <summary>
    /// The three difficulty cases added to Chapter 2. Each one exists to punish a specific wrong idea
    /// about entities, so each test names that wrong idea rather than just checking a happy path.
    /// </summary>
    public sealed class Act2HarderMessageTests
    {
        private static Act2EntityExtractionSampleData.SampleMessage Message(string id)
        {
            return Act2EntityExtractionSampleData.CreateMessages().First(message => message.Id == id);
        }

        private static EntitySpan Span(string messageText, string surface, EntityType type)
        {
            return new EntitySpan(messageText.IndexOf(surface, System.StringComparison.Ordinal), surface.Length, type);
        }

        private static EntityExtractionResult Validate(
            Act2EntityExtractionSampleData.SampleMessage message,
            IEnumerable<EntitySpan> submitted)
        {
            return EntityExtractionValidator.Validate(message.CorrectSpans, submitted);
        }

        [Test]
        public void LanternIsARoomInOneMessageAndAnObjectInAnother()
        {
            var asObject = Message("lantern-object");
            var asRoom = Message("lantern-room-context");

            Assert.That(
                asObject.CorrectSpans.Single().Type.Id,
                Is.EqualTo(Act2EntityExtractionSampleData.ObjectEntityTypeId));
            Assert.That(
                asRoom.CorrectSpans.Any(span =>
                    span.Type.Id == Act2EntityExtractionSampleData.RoomEntityTypeId &&
                    span.GetText(asRoom.MessageText) == "lantern-room"),
                Is.True,
                "The room message must tag 'lantern-room' as a room, not 'lantern' as an object.");
        }

        [Test]
        public void TaggingLanternAsAnObjectInTheRoomMessageIsRejected()
        {
            var message = Message("lantern-room-context");
            var submitted = new[]
            {
                Span(message.MessageText, "lantern", Act2EntityExtractionSampleData.CreateObjectEntityType()),
                Span(message.MessageText, "8am", Act2EntityExtractionSampleData.CreateTimeEntityType())
            };

            Assert.That(Validate(message, submitted).IsCorrect, Is.False);
        }

        [Test]
        public void TheCarryMessageFillsAllThreeSlots()
        {
            var message = Message("carry-to-library");

            Assert.That(
                message.CorrectSpans.Select(span => span.Type.Id),
                Is.EquivalentTo(new[]
                {
                    Act2EntityExtractionSampleData.ObjectEntityTypeId,
                    Act2EntityExtractionSampleData.RoomEntityTypeId,
                    Act2EntityExtractionSampleData.TimeEntityTypeId
                }));
        }

        [Test]
        public void SwappingTheObjectAndTheRoomIsRejected()
        {
            var message = Message("carry-to-library");
            var submitted = new[]
            {
                Span(message.MessageText, "library", Act2EntityExtractionSampleData.CreateObjectEntityType()),
                Span(message.MessageText, "lantern", Act2EntityExtractionSampleData.CreateRoomEntityType()),
                Span(message.MessageText, "6pm", Act2EntityExtractionSampleData.CreateTimeEntityType())
            };

            Assert.That(Validate(message, submitted).IsCorrect, Is.False);
        }

        [Test]
        public void TheDecoyMessageHasATimeAndNothingElse()
        {
            var message = Message("tune-decoy");

            Assert.That(message.CorrectSpans.Count, Is.EqualTo(1));
            Assert.That(
                message.CorrectSpans.Single().Type.Id,
                Is.EqualTo(Act2EntityExtractionSampleData.TimeEntityTypeId));
        }

        [Test]
        public void TaggingTheTuneNameAsARoomIsRejected()
        {
            var message = Message("tune-decoy");
            var submitted = new[]
            {
                Span(message.MessageText, "Lonely Corridor", Act2EntityExtractionSampleData.CreateRoomEntityType()),
                Span(message.MessageText, "7pm", Act2EntityExtractionSampleData.CreateTimeEntityType())
            };

            Assert.That(
                Validate(message, submitted).IsCorrect,
                Is.False,
                "'Lonely Corridor' looks like a room but names a tune; tagging it must fail.");
        }

        [Test]
        public void EveryHarderMessageHasAnErrandWithFailureLines()
        {
            var errands = Act2ErrandDemoData.CreateErrands();

            foreach (var id in new[] { "lantern-room-context", "carry-to-library", "tune-decoy" })
            {
                var errand = errands.FirstOrDefault(candidate => candidate.Message.Id == id);
                Assert.That(errand, Is.Not.Null, id + " has no errand");
                Assert.That(errand.Slots.Count, Is.GreaterThan(0), id + " produced no slots");
                Assert.That(errand.FailureLines.Count, Is.GreaterThan(0), id + " has no failure lines");
            }
        }
        /// <summary>
        /// The reachability guard. Chapter 2 splits the sentence on whitespace and each action-card
        /// slot holds exactly one token, so a correct answer is only obtainable when every expected
        /// span is exactly one whole token and no entity type is expected twice in one message.
        /// Two authored messages violated these rules and were unsolvable while every rule-level test
        /// still passed - the tests fed hand-built spans straight to the validator and never went
        /// through the tokeniser.
        /// </summary>
        [Test]
        public void EveryAuthoredMessageIsSolvableThroughTheTokenModel()
        {
            foreach (var message in Act2EntityExtractionSampleData.CreateMessages())
            {
                var tokens = Tokenise(message.MessageText);

                foreach (var span in message.CorrectSpans)
                {
                    var surface = span.GetText(message.MessageText);
                    Assert.That(
                        tokens.Any(token => token.Start == span.Start && token.Length == span.Length),
                        Is.True,
                        $"'{message.Id}' expects '{surface}', which is not a single token. " +
                        "The player can never select it.");
                }

                var typesUsedTwice = message.CorrectSpans
                    .GroupBy(span => span.Type.Id)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key);

                Assert.That(
                    typesUsedTwice,
                    Is.Empty,
                    $"'{message.Id}' expects one entity type more than once, but a slot holds only one token.");
            }
        }

        /// <summary>Mirrors the controller: split on whitespace, then trim non-alphanumeric edges.</summary>
        private static List<(int Start, int Length)> Tokenise(string messageText)
        {
            var tokens = new List<(int Start, int Length)>();
            var index = 0;

            while (index < messageText.Length)
            {
                if (char.IsWhiteSpace(messageText[index]))
                {
                    index++;
                    continue;
                }

                var start = index;
                while (index < messageText.Length && !char.IsWhiteSpace(messageText[index]))
                {
                    index++;
                }

                var end = index - 1;
                while (start <= end && !char.IsLetterOrDigit(messageText[start]))
                {
                    start++;
                }

                while (end >= start && !char.IsLetterOrDigit(messageText[end]))
                {
                    end--;
                }

                if (start <= end)
                {
                    tokens.Add((start, end - start + 1));
                }
            }

            return tokens;
        }
    }
}
