using System.Collections.Generic;

namespace Ghost.Puzzles.DialogGraph
{
    public static class Act3DialogGraphSampleData
    {
        public const string FindObjectIntentId = "find_object";
        public const string RoomEntityTypeId = "room";
        public const string AnswerObjectLocationResponseId = "answer_object_location";
        public const string AskForRoomResponseId = "ask_for_room";

        public const string StartNodeId = "start";
        public const string FindObjectBranchNodeId = "intent_find_object";
        public const string RoomSlotCheckNodeId = "check_room";
        public const string AnswerResponseNodeId = "response_answer_object_location";
        public const string AskForRoomResponseNodeId = "response_ask_for_room";

        public static DialogGraph CreateCorrectGraph()
        {
            var nodes = new[]
            {
                new DialogNode(StartNodeId, DialogNodeType.Start),
                new DialogNode(FindObjectBranchNodeId, DialogNodeType.IntentBranch, intentId: FindObjectIntentId),
                new DialogNode(RoomSlotCheckNodeId, DialogNodeType.SlotCheck, requiredEntityType: RoomEntityTypeId),
                new DialogNode(AnswerResponseNodeId, DialogNodeType.Response, responseId: AnswerObjectLocationResponseId),
                new DialogNode(AskForRoomResponseNodeId, DialogNodeType.Response, responseId: AskForRoomResponseId)
            };

            var transitions = new[]
            {
                new DialogTransition(StartNodeId, FindObjectBranchNodeId, DialogTransitionCondition.Always),
                new DialogTransition(FindObjectBranchNodeId, RoomSlotCheckNodeId, DialogTransitionCondition.Always),
                new DialogTransition(RoomSlotCheckNodeId, AnswerResponseNodeId, DialogTransitionCondition.SlotPresent),
                new DialogTransition(RoomSlotCheckNodeId, AskForRoomResponseNodeId, DialogTransitionCondition.SlotMissing)
            };

            return new DialogGraph(StartNodeId, nodes, transitions);
        }

        public static IReadOnlyList<DialogGraphTestCase> CreateTestCases()
        {
            return new[]
            {
                new DialogGraphTestCase(
                    "find-object-with-room",
                    CreateFindObjectTurnWithRoom("lab"),
                    AnswerObjectLocationResponseId),
                new DialogGraphTestCase(
                    "find-object-missing-room",
                    CreateFindObjectTurnWithoutRoom(),
                    AskForRoomResponseId)
            };
        }

        public static ConversationTurn CreateFindObjectTurnWithRoom(string roomValue)
        {
            return new ConversationTurn(
                FindObjectIntentId,
                new Dictionary<string, string>
                {
                    { RoomEntityTypeId, roomValue }
                });
        }

        public static ConversationTurn CreateFindObjectTurnWithoutRoom()
        {
            return new ConversationTurn(FindObjectIntentId, new Dictionary<string, string>());
        }
    }
}
