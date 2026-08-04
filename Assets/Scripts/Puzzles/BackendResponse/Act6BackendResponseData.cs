using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.BackendResponse
{
    public static class Act6BackendResponseData
    {
        public const string DataSourceRoleId = "data_source";
        public const string ActionRoleId = "action";
        public const string ResponseRoleId = "response";

        public const string LabRecordsId = "lab_records";
        public const string RoomDirectoryId = "room_directory";
        public const string FetchLabClosingTimeId = "fetch_lab_closing_time";
        public const string FindObjectRoomId = "find_object_room";
        public const string LabHoursResponseId = "lab_hours_response";
        public const string ObjectRoomResponseId = "object_room_response";

        public const string VisitorMessage = "When does the lab close?";
        public const string BackendResult = "closing_time = 8 PM";
        public const string FinalReply = "The lab closes at 8 PM.";

        private static readonly string[] PaletteOrder =
        {
            ObjectRoomResponseId,
            LabRecordsId,
            FindObjectRoomId,
            RoomDirectoryId,
            LabHoursResponseId,
            FetchLabClosingTimeId
        };

        public static IReadOnlyList<string> CreateRoleOrder()
        {
            return new[]
            {
                DataSourceRoleId,
                ActionRoleId,
                ResponseRoleId
            };
        }

        public static IReadOnlyList<Act6BackendCard> CreatePalette()
        {
            var cards = new List<Act6BackendCard>();
            foreach (var cardId in PaletteOrder)
            {
                cards.Add(GetCard(cardId));
            }

            return cards;
        }

        public static IReadOnlyList<Act6BackendPlaybackStep> CreatePlaybackSteps()
        {
            return new[]
            {
                new Act6BackendPlaybackStep(
                    string.Empty,
                    "Tested route chooses lab hours",
                    "Chapters 1-5 already understood the request and selected the safe lab-hours route."),
                new Act6BackendPlaybackStep(
                    DataSourceRoleId,
                    "1. Backend integration reaches Lab records",
                    "The route connects to the system that owns the closing-time fact."),
                new Act6BackendPlaybackStep(
                    ActionRoleId,
                    "2. Action requests one precise fact",
                    "Fetch lab closing time asks Lab records for closing_time."),
                new Act6BackendPlaybackStep(
                    ActionRoleId,
                    "3. Backend returns data",
                    "Backend result: " + BackendResult + ". This is useful data, but not a visitor reply yet."),
                new Act6BackendPlaybackStep(
                    ResponseRoleId,
                    "4. Response generation forms the sentence",
                    "The response template inserts 8 PM and produces: \"" + FinalReply + "\"")
            };
        }

        public static string GetExpectedCardId(string roleId)
        {
            switch (roleId)
            {
                case DataSourceRoleId:
                    return LabRecordsId;
                case ActionRoleId:
                    return FetchLabClosingTimeId;
                case ResponseRoleId:
                    return LabHoursResponseId;
                default:
                    throw new ArgumentException("Unknown backend-response role id.", nameof(roleId));
            }
        }

        public static string GetRoleLabel(string roleId)
        {
            switch (roleId)
            {
                case DataSourceRoleId:
                    return "DATA SOURCE";
                case ActionRoleId:
                    return "ACTION";
                case ResponseRoleId:
                    return "RESPONSE";
                default:
                    return "UNKNOWN ROLE";
            }
        }

        public static Act6BackendCard GetCard(string cardId)
        {
            switch (cardId)
            {
                case LabRecordsId:
                    return new Act6BackendCard(
                        LabRecordsId,
                        "Lab records",
                        DataSourceRoleId,
                        "Stores the lab's authored closing-time field.",
                        "This source does not own the lab closing-time field.");
                case RoomDirectoryId:
                    return new Act6BackendCard(
                        RoomDirectoryId,
                        "Room directory",
                        DataSourceRoleId,
                        "Stores room names and object locations.",
                        "The room directory has locations, not the lab closing time.");
                case FetchLabClosingTimeId:
                    return new Act6BackendCard(
                        FetchLabClosingTimeId,
                        "Fetch lab closing time",
                        ActionRoleId,
                        "Requests closing_time from the connected backend.",
                        "This action asks for a different fact than the visitor needs.");
                case FindObjectRoomId:
                    return new Act6BackendCard(
                        FindObjectRoomId,
                        "Find object room",
                        ActionRoleId,
                        "Requests an object's room from the directory.",
                        "Finding an object's room cannot answer a lab-hours request.");
                case LabHoursResponseId:
                    return new Act6BackendCard(
                        LabHoursResponseId,
                        "The lab closes at {closing_time}.",
                        ResponseRoleId,
                        "Turns closing_time into a complete visitor-facing reply.",
                        "This template phrases a different answer than lab closing time.");
                case ObjectRoomResponseId:
                    return new Act6BackendCard(
                        ObjectRoomResponseId,
                        "The {object} is in {room}.",
                        ResponseRoleId,
                        "Turns object and room fields into a location reply.",
                        "An object-location sentence does not answer when the lab closes.");
                default:
                    throw new ArgumentException("Unknown backend-response card id.", nameof(cardId));
            }
        }

        public static bool IsKnownRole(string roleId)
        {
            return roleId == DataSourceRoleId ||
                roleId == ActionRoleId ||
                roleId == ResponseRoleId;
        }
    }
}
