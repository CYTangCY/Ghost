using System.Collections.Generic;
using Ghost.Puzzles.IntentClassification;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act1GhostGeneralizationEngineTests
    {
        [Test]
        public void Evaluate_WhenGroupingAndLabelsAreCorrect_AllTestMessagesAreCorrect()
        {
            var cardToPile = BuildCorrectCardToPileMap();
            var pileLabels = new Dictionary<string, string>
            {
                { Act1IntentClassificationSampleData.FindItemIntentId, Act1IntentClassificationSampleData.FindItemIntentId },
                { Act1IntentClassificationSampleData.AskLocationIntentId, Act1IntentClassificationSampleData.AskLocationIntentId },
                { Act1IntentClassificationSampleData.AskIdentityIntentId, Act1IntentClassificationSampleData.AskIdentityIntentId }
            };

            foreach (var testMessage in Act1TeachingDemoData.CreateTestMessages())
            {
                var result = Act1GhostGeneralizationEngine.Evaluate(
                    cardToPile,
                    pileLabels,
                    testMessage,
                    Act1TeachingDemoData.CreateReplyLines());

                Assert.That(result.IsCorrect, Is.True, testMessage.Id);
                Assert.That(result.IsConfused, Is.False, testMessage.Id);
                Assert.That(result.RepliedIntentId, Is.EqualTo(testMessage.TrueIntentId), testMessage.Id);
                Assert.That(result.MisleadingCardIds, Is.Empty, testMessage.Id);
            }
        }

        [Test]
        public void Evaluate_WhenRelatedCardsMajorityInWrongLabelledPile_ReturnsWrongReplyAndMisleadingCards()
        {
            var cardToPile = new Dictionary<string, string>
            {
                { "find-item-lost-key", "wrong-pile" },
                { "find-item-seen-notebook", "wrong-pile" },
                { "find-item-help-look", "find-pile" }
            };
            var pileLabels = new Dictionary<string, string>
            {
                { "wrong-pile", Act1IntentClassificationSampleData.AskIdentityIntentId },
                { "find-pile", Act1IntentClassificationSampleData.FindItemIntentId }
            };
            var testMessage = Act1TeachingDemoData.CreateTestMessages()[0];

            var result = Act1GhostGeneralizationEngine.Evaluate(
                cardToPile,
                pileLabels,
                testMessage,
                Act1TeachingDemoData.CreateReplyLines());

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.IsConfused, Is.False);
            Assert.That(result.ChosenPileId, Is.EqualTo("wrong-pile"));
            Assert.That(result.RepliedIntentId, Is.EqualTo(Act1IntentClassificationSampleData.AskIdentityIntentId));
            Assert.That(result.MisleadingCardIds, Is.EquivalentTo(new[] { "find-item-lost-key", "find-item-seen-notebook" }));
        }

        [Test]
        public void Evaluate_WhenRelatedCardsTieBetweenPiles_ReturnsConfused()
        {
            var cardToPile = new Dictionary<string, string>
            {
                { "find-item-lost-key", "pile-a" },
                { "find-item-seen-notebook", "pile-b" }
            };
            var pileLabels = new Dictionary<string, string>
            {
                { "pile-a", Act1IntentClassificationSampleData.FindItemIntentId },
                { "pile-b", Act1IntentClassificationSampleData.FindItemIntentId }
            };
            var testMessage = Act1TeachingDemoData.CreateTestMessages()[0];

            var result = Act1GhostGeneralizationEngine.Evaluate(
                cardToPile,
                pileLabels,
                testMessage,
                Act1TeachingDemoData.CreateReplyLines());

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.IsConfused, Is.True);
            Assert.That(result.ChosenPileId, Is.Null);
            Assert.That(result.MisleadingCardIds, Is.EquivalentTo(testMessage.RelatedCardIds));
        }

        [Test]
        public void Evaluate_WhenChosenPileIsUnlabelled_ReturnsConfused()
        {
            var cardToPile = new Dictionary<string, string>
            {
                { "ask-location-where-floating", "where-pile" },
                { "ask-location-which-room", "where-pile" },
                { "ask-location-near-door", "where-pile" }
            };
            var pileLabels = new Dictionary<string, string>();
            var testMessage = Act1TeachingDemoData.CreateTestMessages()[2];

            var result = Act1GhostGeneralizationEngine.Evaluate(
                cardToPile,
                pileLabels,
                testMessage,
                Act1TeachingDemoData.CreateReplyLines());

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.IsConfused, Is.True);
            Assert.That(result.ChosenPileId, Is.EqualTo("where-pile"));
            Assert.That(result.MisleadingCardIds, Is.EquivalentTo(testMessage.RelatedCardIds));
        }

        private static Dictionary<string, string> BuildCorrectCardToPileMap()
        {
            var cardToPile = new Dictionary<string, string>();
            foreach (var card in Act1IntentClassificationSampleData.CreateCards())
            {
                cardToPile.Add(card.Id, card.IntentId);
            }

            return cardToPile;
        }
    }
}
