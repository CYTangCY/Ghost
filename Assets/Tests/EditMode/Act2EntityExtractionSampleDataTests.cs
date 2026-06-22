using System.Collections.Generic;
using Ghost.Puzzles.EntityExtraction;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act2EntityExtractionSampleDataTests
    {
        [Test]
        public void SampleData_WhenCorrectSpansSubmitted_ValidatesSuccessfully()
        {
            var messages = Act2EntityExtractionSampleData.CreateMessages();

            foreach (var message in messages)
            {
                var submitted = new List<EntitySpan>(message.CorrectSpans);

                var result = EntityExtractionValidator.Validate(message.CorrectSpans, submitted);

                Assert.That(result.IsCorrect, Is.True, $"{message.Id}: {string.Join("\n", result.Errors)}");
                Assert.That(result.Errors, Is.Empty);
            }
        }

        [Test]
        public void SampleData_ContainsSystemAndCustomEntityTypes()
        {
            var messages = Act2EntityExtractionSampleData.CreateMessages();
            var timeType = Act2EntityExtractionSampleData.CreateTimeEntityType();
            var roomType = Act2EntityExtractionSampleData.CreateRoomEntityType();
            var objectType = Act2EntityExtractionSampleData.CreateObjectEntityType();

            Assert.That(ContainsType(messages, timeType), Is.True);
            Assert.That(ContainsType(messages, roomType), Is.True);
            Assert.That(ContainsType(messages, objectType), Is.True);
            Assert.That(timeType.Category, Is.EqualTo(EntityCategory.System));
            Assert.That(roomType.Category, Is.EqualTo(EntityCategory.Custom));
            Assert.That(objectType.Category, Is.EqualTo(EntityCategory.Custom));
            Assert.That(timeType, Is.Not.EqualTo(new EntityType(Act2EntityExtractionSampleData.TimeEntityTypeId, EntityCategory.Custom)));
        }

        [Test]
        public void SampleData_ContainsRoomSynonymPair()
        {
            var messages = Act2EntityExtractionSampleData.CreateMessages();
            var roomType = Act2EntityExtractionSampleData.CreateRoomEntityType();
            var roomSurfaceTexts = new List<string>();

            foreach (var message in messages)
            {
                foreach (var span in message.CorrectSpans)
                {
                    if (span.Type == roomType)
                    {
                        roomSurfaceTexts.Add(span.GetText(message.MessageText));
                    }
                }
            }

            Assert.That(roomSurfaceTexts, Does.Contain("lab"));
            Assert.That(roomSurfaceTexts, Does.Contain("laboratory"));
            Assert.That(roomSurfaceTexts, Has.Count.GreaterThanOrEqualTo(2));
        }

        private static bool ContainsType(IEnumerable<Act2EntityExtractionSampleData.SampleMessage> messages, EntityType type)
        {
            foreach (var message in messages)
            {
                foreach (var span in message.CorrectSpans)
                {
                    if (span.Type == type)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
