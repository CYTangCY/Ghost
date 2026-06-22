using System.Collections.Generic;
using Ghost.Puzzles.DialogGraph;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act3DialogGraphSimulatorTests
    {
        [Test]
        public void Simulate_WhenSlotPresent_ReachesAnswerResponseAndStoresSlot()
        {
            var graph = Act3DialogGraphSampleData.CreateCorrectGraph();
            var context = new DialogContext();
            var turn = Act3DialogGraphSampleData.CreateFindObjectTurnWithRoom("lab");

            var result = DialogGraphSimulator.Simulate(graph, turn, context);

            Assert.That(result.ResponseId, Is.EqualTo(Act3DialogGraphSampleData.AnswerObjectLocationResponseId));
            Assert.That(result.StepLimitReached, Is.False);
            Assert.That(result.UpdatedContext.TryGetSlot(Act3DialogGraphSampleData.RoomEntityTypeId, out var roomValue), Is.True);
            Assert.That(roomValue, Is.EqualTo("lab"));
        }

        [Test]
        public void Simulate_WhenSlotMissing_ReachesAskForRoomResponse()
        {
            var graph = Act3DialogGraphSampleData.CreateCorrectGraph();
            var context = new DialogContext();
            var turn = Act3DialogGraphSampleData.CreateFindObjectTurnWithoutRoom();

            var result = DialogGraphSimulator.Simulate(graph, turn, context);

            Assert.That(result.ResponseId, Is.EqualTo(Act3DialogGraphSampleData.AskForRoomResponseId));
            Assert.That(result.StepLimitReached, Is.False);
            Assert.That(result.UpdatedContext.ContainsSlot(Act3DialogGraphSampleData.RoomEntityTypeId), Is.False);
        }

        [Test]
        public void Simulate_WhenSlotAlreadyExistsInContext_ReachesAnswerResponse()
        {
            var graph = Act3DialogGraphSampleData.CreateCorrectGraph();
            var context = new DialogContext(
                new Dictionary<string, string>
                {
                    { Act3DialogGraphSampleData.RoomEntityTypeId, "library" }
                });
            var turn = Act3DialogGraphSampleData.CreateFindObjectTurnWithoutRoom();

            var result = DialogGraphSimulator.Simulate(graph, turn, context);

            Assert.That(result.ResponseId, Is.EqualTo(Act3DialogGraphSampleData.AnswerObjectLocationResponseId));
            Assert.That(result.StepLimitReached, Is.False);
        }

        [Test]
        public void Simulate_WhenGraphCycles_StopsAtStepCap()
        {
            var graph = CreateCyclicGraph();
            var turn = Act3DialogGraphSampleData.CreateFindObjectTurnWithoutRoom();

            var result = DialogGraphSimulator.Simulate(graph, turn, new DialogContext());

            Assert.That(result.ResponseId, Is.Null);
            Assert.That(result.StepLimitReached, Is.True);
        }

        private static DialogGraph CreateCyclicGraph()
        {
            var nodes = new[]
            {
                new DialogNode(Act3DialogGraphSampleData.StartNodeId, DialogNodeType.Start),
                new DialogNode(
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    DialogNodeType.IntentBranch,
                    intentId: Act3DialogGraphSampleData.FindObjectIntentId)
            };

            var transitions = new[]
            {
                new DialogTransition(
                    Act3DialogGraphSampleData.StartNodeId,
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    DialogTransitionCondition.Always),
                new DialogTransition(
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    Act3DialogGraphSampleData.StartNodeId,
                    DialogTransitionCondition.Always)
            };

            return new DialogGraph(Act3DialogGraphSampleData.StartNodeId, nodes, transitions);
        }
    }
}
