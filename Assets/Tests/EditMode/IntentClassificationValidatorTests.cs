using System.Collections.Generic;
using Ghost.Puzzles.IntentClassification;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class IntentClassificationValidatorTests
    {
        [Test]
        public void Validate_WhenMessagesWithSamePurposeAreGrouped_ReturnsCorrect()
        {
            var cards = CreateSampleCards();
            var submittedGroups = new[]
            {
                new[] { "find-item-2", "find-item-1" },
                new[] { "ask-location-1" },
                new[] { "ask-identity-1" }
            };

            var result = IntentClassificationValidator.Validate(cards, submittedGroups);

            Assert.That(result.IsCorrect, Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void Validate_WhenGroupMixesDifferentIntents_ReturnsIncorrect()
        {
            var cards = CreateSampleCards();
            var submittedGroups = new[]
            {
                new[] { "find-item-1", "ask-location-1" },
                new[] { "find-item-2" },
                new[] { "ask-identity-1" }
            };

            var result = IntentClassificationValidator.Validate(cards, submittedGroups);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("mixes intent"));
        }

        [Test]
        public void Validate_WhenIntentIsSplitAcrossGroups_ReturnsIncorrect()
        {
            var cards = CreateSampleCards();
            var submittedGroups = new[]
            {
                new[] { "find-item-1" },
                new[] { "find-item-2" },
                new[] { "ask-location-1" },
                new[] { "ask-identity-1" }
            };

            var result = IntentClassificationValidator.Validate(cards, submittedGroups);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("split across submitted groups"));
        }

        [Test]
        public void Validate_WhenCardIsMissing_ReturnsIncorrect()
        {
            var cards = CreateSampleCards();
            var submittedGroups = new[]
            {
                new[] { "find-item-1", "find-item-2" },
                new[] { "ask-location-1" }
            };

            var result = IntentClassificationValidator.Validate(cards, submittedGroups);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("was not submitted"));
        }

        [Test]
        public void Validate_WhenDuplicateUnknownOrEmptyGroupsAreSubmitted_ReturnsIncorrect()
        {
            var cards = CreateSampleCards();
            var submittedGroups = new[]
            {
                new[] { "find-item-1", "find-item-1" },
                new[] { "unknown-card-id" },
                new string[0],
                new[] { "find-item-2", "ask-location-1", "ask-identity-1" }
            };

            var result = IntentClassificationValidator.Validate(cards, submittedGroups);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("duplicate card id"));
            Assert.That(result.Errors, Has.Some.Contains("unknown card id"));
            Assert.That(result.Errors, Has.Some.Contains("is empty"));
        }

        private static IReadOnlyList<IntentCard> CreateSampleCards()
        {
            return new[]
            {
                new IntentCard("find-item-1", "I lost my lab key.", "find_item"),
                new IntentCard("find-item-2", "Can Ghost help me look for the key?", "find_item"),
                new IntentCard("ask-location-1", "Where are you floating right now?", "ask_location"),
                new IntentCard("ask-identity-1", "Who are you?", "ask_identity")
            };
        }
    }
}
