using Ghost.Puzzles.DialogGraph;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act3DialogGraphValidatorTests
    {
        [Test]
        public void Validate_WhenSampleGraphIsCorrect_ReturnsCorrect()
        {
            var graph = Act3DialogGraphSampleData.CreateCorrectGraph();
            var testCases = Act3DialogGraphSampleData.CreateTestCases();

            var result = DialogGraphValidator.Validate(graph, testCases);

            Assert.That(result.IsCorrect, Is.True, string.Join("\n", result.Errors));
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void Validate_WhenIntentBranchIsWiredToWrongIntent_ReturnsIncorrect()
        {
            var graph = CreateWrongIntentGraph();
            var testCases = Act3DialogGraphSampleData.CreateTestCases();

            var result = DialogGraphValidator.Validate(graph, testCases);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("not handled"));
        }

        [Test]
        public void Validate_WhenSlotCheckIsMissing_ReturnsIncorrect()
        {
            var graph = CreateMissingSlotCheckGraph();
            var testCases = Act3DialogGraphSampleData.CreateTestCases();

            var result = DialogGraphValidator.Validate(graph, testCases);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains(Act3DialogGraphSampleData.AskForRoomResponseId));
        }

        [Test]
        public void Validate_WhenResponseIdIsWrong_ReturnsIncorrect()
        {
            var graph = CreateWrongResponseGraph();
            var testCases = Act3DialogGraphSampleData.CreateTestCases();

            var result = DialogGraphValidator.Validate(graph, testCases);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains(Act3DialogGraphSampleData.AnswerObjectLocationResponseId));
        }

        [Test]
        public void Validate_WhenGraphHasUnreachableNodeAndDeadEnd_ReturnsIncorrect()
        {
            var graph = CreateUnreachableAndDeadEndGraph();
            var testCases = Act3DialogGraphSampleData.CreateTestCases();

            var result = DialogGraphValidator.Validate(graph, testCases);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("unreachable"));
            Assert.That(result.Errors, Has.Some.Contains("SlotMissing"));
        }

        private static DialogGraph CreateWrongIntentGraph()
        {
            var nodes = new[]
            {
                new DialogNode(Act3DialogGraphSampleData.StartNodeId, DialogNodeType.Start),
                new DialogNode(
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    DialogNodeType.IntentBranch,
                    intentId: "ask_identity"),
                new DialogNode(
                    Act3DialogGraphSampleData.RoomSlotCheckNodeId,
                    DialogNodeType.SlotCheck,
                    requiredEntityType: Act3DialogGraphSampleData.RoomEntityTypeId),
                new DialogNode(
                    Act3DialogGraphSampleData.AnswerResponseNodeId,
                    DialogNodeType.Response,
                    responseId: Act3DialogGraphSampleData.AnswerObjectLocationResponseId),
                new DialogNode(
                    Act3DialogGraphSampleData.AskForRoomResponseNodeId,
                    DialogNodeType.Response,
                    responseId: Act3DialogGraphSampleData.AskForRoomResponseId)
            };

            return CreateStandardGraph(nodes);
        }

        private static DialogGraph CreateMissingSlotCheckGraph()
        {
            var nodes = new[]
            {
                new DialogNode(Act3DialogGraphSampleData.StartNodeId, DialogNodeType.Start),
                new DialogNode(
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    DialogNodeType.IntentBranch,
                    intentId: Act3DialogGraphSampleData.FindObjectIntentId),
                new DialogNode(
                    Act3DialogGraphSampleData.AnswerResponseNodeId,
                    DialogNodeType.Response,
                    responseId: Act3DialogGraphSampleData.AnswerObjectLocationResponseId)
            };

            var transitions = new[]
            {
                new DialogTransition(
                    Act3DialogGraphSampleData.StartNodeId,
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    DialogTransitionCondition.Always),
                new DialogTransition(
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    Act3DialogGraphSampleData.AnswerResponseNodeId,
                    DialogTransitionCondition.Always)
            };

            return new DialogGraph(Act3DialogGraphSampleData.StartNodeId, nodes, transitions);
        }

        private static DialogGraph CreateWrongResponseGraph()
        {
            var nodes = new[]
            {
                new DialogNode(Act3DialogGraphSampleData.StartNodeId, DialogNodeType.Start),
                new DialogNode(
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    DialogNodeType.IntentBranch,
                    intentId: Act3DialogGraphSampleData.FindObjectIntentId),
                new DialogNode(
                    Act3DialogGraphSampleData.RoomSlotCheckNodeId,
                    DialogNodeType.SlotCheck,
                    requiredEntityType: Act3DialogGraphSampleData.RoomEntityTypeId),
                new DialogNode(
                    Act3DialogGraphSampleData.AnswerResponseNodeId,
                    DialogNodeType.Response,
                    responseId: "wrong_answer"),
                new DialogNode(
                    Act3DialogGraphSampleData.AskForRoomResponseNodeId,
                    DialogNodeType.Response,
                    responseId: Act3DialogGraphSampleData.AskForRoomResponseId)
            };

            return CreateStandardGraph(nodes);
        }

        private static DialogGraph CreateUnreachableAndDeadEndGraph()
        {
            var nodes = new[]
            {
                new DialogNode(Act3DialogGraphSampleData.StartNodeId, DialogNodeType.Start),
                new DialogNode(
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    DialogNodeType.IntentBranch,
                    intentId: Act3DialogGraphSampleData.FindObjectIntentId),
                new DialogNode(
                    Act3DialogGraphSampleData.RoomSlotCheckNodeId,
                    DialogNodeType.SlotCheck,
                    requiredEntityType: Act3DialogGraphSampleData.RoomEntityTypeId),
                new DialogNode(
                    Act3DialogGraphSampleData.AnswerResponseNodeId,
                    DialogNodeType.Response,
                    responseId: Act3DialogGraphSampleData.AnswerObjectLocationResponseId),
                new DialogNode(
                    Act3DialogGraphSampleData.AskForRoomResponseNodeId,
                    DialogNodeType.Response,
                    responseId: Act3DialogGraphSampleData.AskForRoomResponseId),
                new DialogNode("unreachable_response", DialogNodeType.Response, responseId: "unused_response")
            };

            var transitions = new[]
            {
                new DialogTransition(
                    Act3DialogGraphSampleData.StartNodeId,
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    DialogTransitionCondition.Always),
                new DialogTransition(
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    Act3DialogGraphSampleData.RoomSlotCheckNodeId,
                    DialogTransitionCondition.Always),
                new DialogTransition(
                    Act3DialogGraphSampleData.RoomSlotCheckNodeId,
                    Act3DialogGraphSampleData.AnswerResponseNodeId,
                    DialogTransitionCondition.SlotPresent)
            };

            return new DialogGraph(Act3DialogGraphSampleData.StartNodeId, nodes, transitions);
        }

        private static DialogGraph CreateStandardGraph(DialogNode[] nodes)
        {
            var transitions = new[]
            {
                new DialogTransition(
                    Act3DialogGraphSampleData.StartNodeId,
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    DialogTransitionCondition.Always),
                new DialogTransition(
                    Act3DialogGraphSampleData.FindObjectBranchNodeId,
                    Act3DialogGraphSampleData.RoomSlotCheckNodeId,
                    DialogTransitionCondition.Always),
                new DialogTransition(
                    Act3DialogGraphSampleData.RoomSlotCheckNodeId,
                    Act3DialogGraphSampleData.AnswerResponseNodeId,
                    DialogTransitionCondition.SlotPresent),
                new DialogTransition(
                    Act3DialogGraphSampleData.RoomSlotCheckNodeId,
                    Act3DialogGraphSampleData.AskForRoomResponseNodeId,
                    DialogTransitionCondition.SlotMissing)
            };

            return new DialogGraph(Act3DialogGraphSampleData.StartNodeId, nodes, transitions);
        }
    }
}
