using System.Collections.Generic;

namespace Ghost.Puzzles.DialogGraph
{
    public static class Act3DialogGraphSampleData
    {
        public const string FindObjectIntentId = "find_object";
        public const string OpeningHoursIntentId = "opening_hours";
        public const string RoomEntityTypeId = "room";
        public const string AnswerObjectLocationResponseId = "answer_object_location";
        public const string AskForRoomResponseId = "ask_for_room";
        public const string AnswerOpeningHoursResponseId = "answer_opening_hours";

        public const string StartNodeId = "start";
        public const string FindObjectBranchNodeId = "intent_find_object";
        public const string RoomSlotCheckNodeId = "check_room";
        public const string AnswerResponseNodeId = "response_answer_object_location";
        public const string AskForRoomResponseNodeId = "response_ask_for_room";
        public const string OpeningHoursBranchNodeId = "intent_opening_hours";
        public const string AnswerOpeningHoursNodeId = "response_answer_opening_hours";

        public static DialogGraph CreateCorrectGraph()
        {
            var nodes = new[]
            {
                new DialogNode(StartNodeId, DialogNodeType.Start),
                new DialogNode(FindObjectBranchNodeId, DialogNodeType.IntentBranch, intentId: FindObjectIntentId),
                new DialogNode(RoomSlotCheckNodeId, DialogNodeType.SlotCheck, requiredEntityType: RoomEntityTypeId),
                new DialogNode(AnswerResponseNodeId, DialogNodeType.Response, responseId: AnswerObjectLocationResponseId),
                new DialogNode(AskForRoomResponseNodeId, DialogNodeType.Response, responseId: AskForRoomResponseId),

                // Second intent. The visitor asking about closing time needs no room, so this branch
                // answers directly - the point being that not every request needs a slot check.
                new DialogNode(OpeningHoursBranchNodeId, DialogNodeType.IntentBranch, intentId: OpeningHoursIntentId),
                new DialogNode(AnswerOpeningHoursNodeId, DialogNodeType.Response, responseId: AnswerOpeningHoursResponseId)
            };

            var transitions = new[]
            {
                new DialogTransition(StartNodeId, FindObjectBranchNodeId, DialogTransitionCondition.Always),
                new DialogTransition(FindObjectBranchNodeId, RoomSlotCheckNodeId, DialogTransitionCondition.Always),
                new DialogTransition(RoomSlotCheckNodeId, AnswerResponseNodeId, DialogTransitionCondition.SlotPresent),
                new DialogTransition(RoomSlotCheckNodeId, AskForRoomResponseNodeId, DialogTransitionCondition.SlotMissing),
                new DialogTransition(StartNodeId, OpeningHoursBranchNodeId, DialogTransitionCondition.Always),
                new DialogTransition(OpeningHoursBranchNodeId, AnswerOpeningHoursNodeId, DialogTransitionCondition.Always)
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
                    AskForRoomResponseId),
                new DialogGraphTestCase(
                    "opening-hours",
                    CreateOpeningHoursTurn(),
                    AnswerOpeningHoursResponseId)
            };
        }

        /// <summary>
        /// The cards Chapter 3 offers in its node palette. This lives beside the correct graph on
        /// purpose: the palette used to be hand-written in the presenter, so adding a node to the
        /// correct graph could silently make the chapter unsolvable. PaletteCoversCorrectGraph proves
        /// the two stay in step.
        /// </summary>
        public static IReadOnlyList<PaletteEntry> CreatePaletteEntries()
        {
            return new[]
            {
                new PaletteEntry("Start here", "Where Ghost begins.", DialogNodeType.Start),
                new PaletteEntry(
                    "Recognize request",
                    "Visitor wants help finding something.",
                    DialogNodeType.IntentBranch,
                    intentId: FindObjectIntentId),
                new PaletteEntry(
                    "Recognize hours",
                    "Visitor is asking when the lab shuts.",
                    DialogNodeType.IntentBranch,
                    intentId: OpeningHoursIntentId),
                new PaletteEntry(
                    "Check room",
                    "Does Ghost know which room?",
                    DialogNodeType.SlotCheck,
                    requiredEntityType: RoomEntityTypeId),
                new PaletteEntry(
                    "Answer location",
                    "Use this when the room is known.",
                    DialogNodeType.Response,
                    responseId: AnswerObjectLocationResponseId),
                new PaletteEntry(
                    "Ask which room",
                    "Use this when the room is missing.",
                    DialogNodeType.Response,
                    responseId: AskForRoomResponseId),
                new PaletteEntry(
                    "Answer hours",
                    "Use this for the closing-time question.",
                    DialogNodeType.Response,
                    responseId: AnswerOpeningHoursResponseId)
            };
        }

        public sealed class PaletteEntry
        {
            public PaletteEntry(
                string title,
                string detail,
                DialogNodeType type,
                string intentId = null,
                string requiredEntityType = null,
                string responseId = null)
            {
                Title = title;
                Detail = detail;
                Type = type;
                IntentId = intentId ?? string.Empty;
                RequiredEntityType = requiredEntityType ?? string.Empty;
                ResponseId = responseId ?? string.Empty;
            }

            public string Title { get; }

            public string Detail { get; }

            public DialogNodeType Type { get; }

            public string IntentId { get; }

            public string RequiredEntityType { get; }

            public string ResponseId { get; }
        }

        /// <summary>
        /// What each test case looks like as an actual person at the desk. Kept beside the test cases
        /// rather than inside DialogGraphTestCase, because Chapter 5 shares that type and does not
        /// want the presentation text.
        /// </summary>
        public static IReadOnlyList<VisitorScript> CreateVisitorScripts()
        {
            return new[]
            {
                new VisitorScript(
                    "find-object-with-room",
                    "I left my notebook in the lab. Could you have a look?",
                    "Ghost: I will search the lab for the notebook.",
                    "The room was in the message, so Ghost never had to ask."),
                new VisitorScript(
                    "find-object-missing-room",
                    "I have lost my notebook somewhere in this building.",
                    "Ghost: Which room should I search?",
                    "No room in the message, so the check sends Ghost down the ask branch."),
                new VisitorScript(
                    "opening-hours",
                    "When does the lab close tonight?",
                    "Ghost: The lab closes at 8pm.",
                    "This one needs no room at all, so it answers straight from its own branch.")
            };
        }

        public sealed class VisitorScript
        {
            public VisitorScript(string testCaseId, string visitorLine, string ghostReply, string note)
            {
                TestCaseId = testCaseId;
                VisitorLine = visitorLine;
                GhostReply = ghostReply;
                Note = note;
            }

            public string TestCaseId { get; }

            public string VisitorLine { get; }

            public string GhostReply { get; }

            public string Note { get; }
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

        public static ConversationTurn CreateOpeningHoursTurn()
        {
            return new ConversationTurn(OpeningHoursIntentId, new Dictionary<string, string>());
        }
    }
}
