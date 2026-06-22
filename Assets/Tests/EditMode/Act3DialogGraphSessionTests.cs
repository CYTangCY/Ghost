using System;
using Ghost.Puzzles.DialogGraph;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act3DialogGraphSessionTests
    {
        [Test]
        public void ValidateCurrentState_WhenSessionIsEmpty_ReturnsIncorrectWithoutThrowing()
        {
            var session = DialogGraphSession.CreateFromSampleData();

            DialogGraphResult result = null;
            Assert.DoesNotThrow(() => result = session.ValidateCurrentState());

            Assert.That(session.CurrentNodes, Is.Empty);
            Assert.That(session.CurrentTransitions, Is.Empty);
            Assert.That(session.CurrentStartNodeId, Is.Empty);
            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("no nodes"));
            Assert.That(result.Errors, Has.Some.Contains("start node is not set"));
        }

        [Test]
        public void ValidateCurrentState_WhenCorrectGraphBuiltThroughSession_ReturnsCorrect()
        {
            var session = DialogGraphSession.CreateFromSampleData();

            BuildCorrectGraph(session);

            var result = session.ValidateCurrentState();

            Assert.That(result.IsCorrect, Is.True, string.Join("\n", result.Errors));
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void ValidateCurrentState_WhenSlotMissingTransitionIsMissing_ReturnsIncorrect()
        {
            var session = DialogGraphSession.CreateFromSampleData();
            var ids = AddCorrectNodes(session);
            session.SetStartNode(ids.StartNodeId);
            session.AddTransition(ids.StartNodeId, ids.IntentBranchNodeId, DialogTransitionCondition.Always);
            session.AddTransition(ids.IntentBranchNodeId, ids.SlotCheckNodeId, DialogTransitionCondition.Always);
            session.AddTransition(ids.SlotCheckNodeId, ids.AnswerResponseNodeId, DialogTransitionCondition.SlotPresent);

            var result = session.ValidateCurrentState();

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("SlotMissing"));
        }

        [Test]
        public void RemoveNode_WhenNodeHasTransitions_RemovesNodeAndReferencingTransitions()
        {
            var session = DialogGraphSession.CreateFromSampleData();
            var ids = BuildCorrectGraph(session);

            var wasRemoved = session.RemoveNode(ids.SlotCheckNodeId);
            var result = session.ValidateCurrentState();

            Assert.That(wasRemoved, Is.True);
            Assert.That(ContainsNode(session, ids.SlotCheckNodeId), Is.False);
            Assert.That(ContainsTransitionReferencing(session, ids.SlotCheckNodeId), Is.False);
            Assert.That(result.IsCorrect, Is.False);
        }

        [Test]
        public void AddTransitionAndRemoveTransition_AreReflectedInCurrentTransitions()
        {
            var session = DialogGraphSession.CreateFromSampleData();
            var startNodeId = session.AddNode(DialogNodeType.Start);
            var branchNodeId = session.AddNode(DialogNodeType.IntentBranch, intentId: Act3DialogGraphSampleData.FindObjectIntentId);

            session.AddTransition(startNodeId, branchNodeId, DialogTransitionCondition.Always);
            Assert.That(session.CurrentTransitions.Count, Is.EqualTo(1));

            var wasRemoved = session.RemoveTransition(startNodeId, branchNodeId, DialogTransitionCondition.Always);
            var wasRemovedAgain = session.RemoveTransition(startNodeId, branchNodeId, DialogTransitionCondition.Always);

            Assert.That(wasRemoved, Is.True);
            Assert.That(wasRemovedAgain, Is.False);
            Assert.That(session.CurrentTransitions, Is.Empty);
        }

        private static CorrectGraphIds BuildCorrectGraph(DialogGraphSession session)
        {
            var ids = AddCorrectNodes(session);

            session.SetStartNode(ids.StartNodeId);
            session.AddTransition(ids.StartNodeId, ids.IntentBranchNodeId, DialogTransitionCondition.Always);
            session.AddTransition(ids.IntentBranchNodeId, ids.SlotCheckNodeId, DialogTransitionCondition.Always);
            session.AddTransition(ids.SlotCheckNodeId, ids.AnswerResponseNodeId, DialogTransitionCondition.SlotPresent);
            session.AddTransition(ids.SlotCheckNodeId, ids.AskForRoomResponseNodeId, DialogTransitionCondition.SlotMissing);

            return ids;
        }

        private static CorrectGraphIds AddCorrectNodes(DialogGraphSession session)
        {
            return new CorrectGraphIds(
                session.AddNode(DialogNodeType.Start),
                session.AddNode(DialogNodeType.IntentBranch, intentId: Act3DialogGraphSampleData.FindObjectIntentId),
                session.AddNode(DialogNodeType.SlotCheck, requiredEntityType: Act3DialogGraphSampleData.RoomEntityTypeId),
                session.AddNode(DialogNodeType.Response, responseId: Act3DialogGraphSampleData.AnswerObjectLocationResponseId),
                session.AddNode(DialogNodeType.Response, responseId: Act3DialogGraphSampleData.AskForRoomResponseId));
        }

        private static bool ContainsNode(DialogGraphSession session, string nodeId)
        {
            foreach (var node in session.CurrentNodes)
            {
                if (string.Equals(node.Id, nodeId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsTransitionReferencing(DialogGraphSession session, string nodeId)
        {
            foreach (var transition in session.CurrentTransitions)
            {
                if (string.Equals(transition.FromNodeId, nodeId, StringComparison.Ordinal)
                    || string.Equals(transition.ToNodeId, nodeId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private sealed class CorrectGraphIds
        {
            public CorrectGraphIds(
                string startNodeId,
                string intentBranchNodeId,
                string slotCheckNodeId,
                string answerResponseNodeId,
                string askForRoomResponseNodeId)
            {
                StartNodeId = startNodeId;
                IntentBranchNodeId = intentBranchNodeId;
                SlotCheckNodeId = slotCheckNodeId;
                AnswerResponseNodeId = answerResponseNodeId;
                AskForRoomResponseNodeId = askForRoomResponseNodeId;
            }

            public string StartNodeId { get; }

            public string IntentBranchNodeId { get; }

            public string SlotCheckNodeId { get; }

            public string AnswerResponseNodeId { get; }

            public string AskForRoomResponseNodeId { get; }
        }
    }
}
