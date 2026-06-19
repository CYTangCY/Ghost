using System;
using System.Collections.Generic;
using Ghost.Puzzles.IntentClassification;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class IntentClassificationSessionTests
    {
        [Test]
        public void Constructor_WhenCreatedFromCards_LeavesAllCardsUnassigned()
        {
            var cards = Act1IntentClassificationSampleData.CreateCards();
            var session = new IntentClassificationSession(cards);

            Assert.That(session.Cards.Count, Is.EqualTo(cards.Count));
            Assert.That(session.UnassignedCardIds.Count, Is.EqualTo(cards.Count));
            Assert.That(session.AssignedGroupIds.Count, Is.EqualTo(0));
            Assert.That(session.CreateSubmittedGroups().Count, Is.EqualTo(0));
        }

        [Test]
        public void CreateFromSampleData_LeavesAllSampleCardsUnassigned()
        {
            var session = IntentClassificationSession.CreateFromSampleData();

            Assert.That(session.Cards.Count, Is.EqualTo(Act1IntentClassificationSampleData.CreateCards().Count));
            Assert.That(session.UnassignedCardIds.Count, Is.EqualTo(session.Cards.Count));
        }

        [Test]
        public void MoveCardToGroup_AssignsCardAndRemovesItFromUnassigned()
        {
            var session = IntentClassificationSession.CreateFromSampleData();

            session.MoveCardToGroup("find-item-lost-key", "group-a");

            Assert.That(session.GetAssignedGroupId("find-item-lost-key"), Is.EqualTo("group-a"));
            Assert.That(session.GetAssignedCardIds("group-a"), Is.EquivalentTo(new[] { "find-item-lost-key" }));
            Assert.That(session.UnassignedCardIds, Does.Not.Contain("find-item-lost-key"));
        }

        [Test]
        public void MoveCardToGroup_WhenCardAlreadyAssigned_MovesCardBetweenGroups()
        {
            var session = IntentClassificationSession.CreateFromSampleData();

            session.MoveCardToGroup("find-item-lost-key", "group-a");
            session.MoveCardToGroup("find-item-lost-key", "group-b");

            Assert.That(session.GetAssignedGroupId("find-item-lost-key"), Is.EqualTo("group-b"));
            Assert.That(session.GetAssignedCardIds("group-a").Count, Is.EqualTo(0));
            Assert.That(session.GetAssignedCardIds("group-b"), Is.EquivalentTo(new[] { "find-item-lost-key" }));
        }

        [Test]
        public void MoveCardToUnassigned_WhenCardWasAssigned_ReturnsCardToUnassigned()
        {
            var session = IntentClassificationSession.CreateFromSampleData();

            session.MoveCardToGroup("find-item-lost-key", "group-a");
            session.MoveCardToUnassigned("find-item-lost-key");

            Assert.That(session.GetAssignedGroupId("find-item-lost-key"), Is.Null);
            Assert.That(session.GetAssignedCardIds("group-a").Count, Is.EqualTo(0));
            Assert.That(session.UnassignedCardIds, Does.Contain("find-item-lost-key"));
        }

        [Test]
        public void ValidateCurrentState_WhenGroupingIsPartial_ReturnsIncorrect()
        {
            var session = IntentClassificationSession.CreateFromSampleData();

            session.MoveCardToGroup("find-item-lost-key", "find-items");
            session.MoveCardToGroup("find-item-seen-notebook", "find-items");

            var result = session.ValidateCurrentState();

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("was not submitted"));
        }

        [Test]
        public void ValidateCurrentState_WhenSampleGroupingIsCorrect_ReturnsCorrect()
        {
            var session = IntentClassificationSession.CreateFromSampleData();
            var correctGroups = Act1IntentClassificationSampleData.CreateCorrectGroups();

            for (var groupIndex = 0; groupIndex < correctGroups.Count; groupIndex++)
            {
                var groupId = "group-" + groupIndex;

                foreach (var cardId in correctGroups[groupIndex])
                {
                    session.MoveCardToGroup(cardId, groupId);
                }
            }

            var result = session.ValidateCurrentState();

            Assert.That(result.IsCorrect, Is.True, string.Join("\n", result.Errors));
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void MoveCardToGroup_WhenCardIdIsUnknown_ThrowsArgumentException()
        {
            var session = IntentClassificationSession.CreateFromSampleData();

            var exception = Assert.Throws<ArgumentException>(
                () => session.MoveCardToGroup("unknown-card-id", "group-a"));

            Assert.That(exception.Message, Does.Contain("Unknown card id"));
        }

        [Test]
        public void MoveCardToUnassigned_WhenCardIdIsUnknown_ThrowsArgumentException()
        {
            var session = IntentClassificationSession.CreateFromSampleData();

            var exception = Assert.Throws<ArgumentException>(
                () => session.MoveCardToUnassigned("unknown-card-id"));

            Assert.That(exception.Message, Does.Contain("Unknown card id"));
        }

        [Test]
        public void CreateSubmittedGroups_ReturnsOnlyAssignedGroups()
        {
            var session = new IntentClassificationSession(CreateSmallCardSet());

            session.MoveCardToGroup("card-a", "group-a");
            session.MoveCardToGroup("card-b", "group-b");

            var groups = session.CreateSubmittedGroups();

            Assert.That(groups.Count, Is.EqualTo(2));
            Assert.That(FlattenGroups(groups), Is.EquivalentTo(new[] { "card-a", "card-b" }));
        }

        private static IReadOnlyList<IntentCard> CreateSmallCardSet()
        {
            return new[]
            {
                new IntentCard("card-a", "Can you help me find it?", "find_item"),
                new IntentCard("card-b", "Where are you?", "ask_location")
            };
        }

        private static List<string> FlattenGroups(IEnumerable<IReadOnlyList<string>> groups)
        {
            var cardIds = new List<string>();

            foreach (var group in groups)
            {
                cardIds.AddRange(group);
            }

            return cardIds;
        }
    }
}
