using System;
using System.Collections.Generic;
using Ghost.Puzzles.DialogGraph;
using DialogGraphModel = Ghost.Puzzles.DialogGraph.DialogGraph;

namespace Ghost.Puzzles.TestingDebugging
{
    public static class Act5BuggyGraphData
    {
        public const string FindObjectIntentId = "find_object";
        public const string CheckHoursIntentId = "check_hours";
        public const string GreetingIntentId = "greeting";
        public const string RoomEntityTypeId = "room";

        public const string AnswerObjectLocationResponseId = "answer_object_location";
        public const string AskForRoomResponseId = "ask_for_room";
        public const string AnswerLabHoursResponseId = "answer_lab_hours";
        public const string FriendlyGreetingResponseId = "friendly_greeting";

        public const string StartNodeId = "start";
        public const string FindObjectBranchNodeId = "intent_find_object";
        public const string CheckRoomNodeId = "check_room";
        public const string CheckHoursBranchNodeId = "intent_check_hours";
        public const string GreetingBranchNodeId = "intent_greeting";
        public const string AnswerLocationNodeId = "response_answer_location";
        public const string AskForRoomNodeId = "response_ask_for_room";
        public const string AnswerLabHoursNodeId = "response_answer_lab_hours";
        public const string FriendlyGreetingNodeId = "response_friendly_greeting";

        public static DialogGraphModel CreateBuggyGraph()
        {
            return new DialogGraphModel(StartNodeId, CreateNodes(), CreateBuggyTransitions());
        }

        public static DialogGraphModel CreateFixedGraph()
        {
            return new DialogGraphModel(StartNodeId, CreateNodes(), CreateFixedTransitions());
        }

        public static IReadOnlyList<DialogNode> CreateNodes()
        {
            return new[]
            {
                new DialogNode(StartNodeId, DialogNodeType.Start),
                new DialogNode(FindObjectBranchNodeId, DialogNodeType.IntentBranch, intentId: FindObjectIntentId),
                new DialogNode(CheckRoomNodeId, DialogNodeType.SlotCheck, requiredEntityType: RoomEntityTypeId),
                new DialogNode(CheckHoursBranchNodeId, DialogNodeType.IntentBranch, intentId: CheckHoursIntentId),
                new DialogNode(GreetingBranchNodeId, DialogNodeType.IntentBranch, intentId: GreetingIntentId),
                new DialogNode(AnswerLocationNodeId, DialogNodeType.Response, responseId: AnswerObjectLocationResponseId),
                new DialogNode(AskForRoomNodeId, DialogNodeType.Response, responseId: AskForRoomResponseId),
                new DialogNode(AnswerLabHoursNodeId, DialogNodeType.Response, responseId: AnswerLabHoursResponseId),
                new DialogNode(FriendlyGreetingNodeId, DialogNodeType.Response, responseId: FriendlyGreetingResponseId)
            };
        }

        public static IReadOnlyList<DialogTransition> CreateBuggyTransitions()
        {
            return new[]
            {
                new DialogTransition(StartNodeId, FindObjectBranchNodeId, DialogTransitionCondition.Always),
                new DialogTransition(StartNodeId, CheckHoursBranchNodeId, DialogTransitionCondition.Always),
                new DialogTransition(FindObjectBranchNodeId, CheckRoomNodeId, DialogTransitionCondition.Always),
                new DialogTransition(CheckRoomNodeId, AskForRoomNodeId, DialogTransitionCondition.SlotPresent),
                new DialogTransition(CheckRoomNodeId, AnswerLocationNodeId, DialogTransitionCondition.SlotMissing),
                new DialogTransition(CheckHoursBranchNodeId, AskForRoomNodeId, DialogTransitionCondition.Always),
                new DialogTransition(GreetingBranchNodeId, FriendlyGreetingNodeId, DialogTransitionCondition.Always)
            };
        }

        public static IReadOnlyList<DialogTransition> CreateFixedTransitions()
        {
            return new[]
            {
                new DialogTransition(StartNodeId, FindObjectBranchNodeId, DialogTransitionCondition.Always),
                new DialogTransition(StartNodeId, CheckHoursBranchNodeId, DialogTransitionCondition.Always),
                new DialogTransition(StartNodeId, GreetingBranchNodeId, DialogTransitionCondition.Always),
                new DialogTransition(FindObjectBranchNodeId, CheckRoomNodeId, DialogTransitionCondition.Always),
                new DialogTransition(CheckRoomNodeId, AnswerLocationNodeId, DialogTransitionCondition.SlotPresent),
                new DialogTransition(CheckRoomNodeId, AskForRoomNodeId, DialogTransitionCondition.SlotMissing),
                new DialogTransition(CheckHoursBranchNodeId, AnswerLabHoursNodeId, DialogTransitionCondition.Always),
                new DialogTransition(GreetingBranchNodeId, FriendlyGreetingNodeId, DialogTransitionCondition.Always)
            };
        }

        public static IReadOnlyList<Act5TestConversation> CreateTestConversations()
        {
            return new[]
            {
                new Act5TestConversation(
                    "find-with-room",
                    "Please find my brass key in the library.",
                    new ConversationTurn(
                        FindObjectIntentId,
                        new Dictionary<string, string>
                        {
                            { RoomEntityTypeId, "library" }
                        }),
                    AnswerObjectLocationResponseId),
                new Act5TestConversation(
                    "find-missing-room",
                    "Can you help me find my brass key?",
                    new ConversationTurn(FindObjectIntentId, new Dictionary<string, string>()),
                    AskForRoomResponseId),
                new Act5TestConversation(
                    "check-hours",
                    "When does the lab close tonight?",
                    new ConversationTurn(CheckHoursIntentId, new Dictionary<string, string>()),
                    AnswerLabHoursResponseId),
                new Act5TestConversation(
                    "greeting",
                    "Hello, Ghost!",
                    new ConversationTurn(GreetingIntentId, new Dictionary<string, string>()),
                    FriendlyGreetingResponseId)
            };
        }

        public static string GetResponseLine(string responseId)
        {
            switch (responseId)
            {
                case AnswerObjectLocationResponseId:
                    return "Ghost: I will search for the brass key in the library.";
                case AskForRoomResponseId:
                    return "Ghost: Which room should I search?";
                case AnswerLabHoursResponseId:
                    return "Ghost: The lab closes at 8 p.m.";
                case FriendlyGreetingResponseId:
                    return "Ghost: Hello! I am ready to help.";
                case null:
                case "":
                    return "No response";
                default:
                    return "Unknown response: " + responseId;
            }
        }

        public static string GetNodeTitle(DialogNode node)
        {
            if (node == null)
            {
                return "Unknown node";
            }

            switch (node.Id)
            {
                case StartNodeId:
                    return "Start";
                case FindObjectBranchNodeId:
                    return "Intent: find object";
                case CheckRoomNodeId:
                    return "Check: room known?";
                case CheckHoursBranchNodeId:
                    return "Intent: lab hours";
                case GreetingBranchNodeId:
                    return "Intent: greeting";
                case AnswerLocationNodeId:
                    return "Reply: search in room";
                case AskForRoomNodeId:
                    return "Reply: ask for room";
                case AnswerLabHoursNodeId:
                    return "Reply: lab closes at 8";
                case FriendlyGreetingNodeId:
                    return "Reply: friendly hello";
                default:
                    return node.Id;
            }
        }
    }
}