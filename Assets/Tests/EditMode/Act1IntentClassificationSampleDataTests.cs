using System.Collections.Generic;
using Ghost.Puzzles.IntentClassification;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act1IntentClassificationSampleDataTests
    {
        [Test]
        public void SampleData_WhenCorrectGroupsSubmitted_ValidatesSuccessfully()
        {
            var cards = Act1IntentClassificationSampleData.CreateCards();
            var groups = Act1IntentClassificationSampleData.CreateCorrectGroups();

            var result = IntentClassificationValidator.Validate(cards, groups);

            Assert.That(result.IsCorrect, Is.True, string.Join("\n", result.Errors));
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void SampleData_ContainsThreeIntentGroupsWithMultipleDifferentlyWordedMessages()
        {
            var cardsById = CreateCardLookup(Act1IntentClassificationSampleData.CreateCards());
            var groups = Act1IntentClassificationSampleData.CreateCorrectGroups();

            Assert.That(groups.Count, Is.EqualTo(3));

            foreach (var group in groups)
            {
                var messagesInGroup = new List<string>();

                foreach (var cardId in group)
                {
                    messagesInGroup.Add(cardsById[cardId].MessageText);
                }

                Assert.That(messagesInGroup, Has.Count.GreaterThanOrEqualTo(2));
                Assert.That(messagesInGroup, Is.Unique);
            }
        }

        [Test]
        public void SampleData_WhenOneCardMovesToWrongPurpose_ValidatorRejectsIt()
        {
            var cards = Act1IntentClassificationSampleData.CreateCards();
            var incorrectGroups = new[]
            {
                new[]
                {
                    "find-item-lost-key",
                    "find-item-seen-notebook",
                    "ask-location-where-floating"
                },
                new[]
                {
                    "ask-location-which-room",
                    "ask-location-near-door"
                },
                new[]
                {
                    "ask-identity-who",
                    "ask-identity-name",
                    "ask-identity-tell-name"
                },
                new[]
                {
                    "find-item-help-look"
                }
            };

            var result = IntentClassificationValidator.Validate(cards, incorrectGroups);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("mixes intent"));
        }

        private static Dictionary<string, IntentCard> CreateCardLookup(IEnumerable<IntentCard> cards)
        {
            var cardsById = new Dictionary<string, IntentCard>();

            foreach (var card in cards)
            {
                cardsById.Add(card.Id, card);
            }

            return cardsById;
        }
    }
}
