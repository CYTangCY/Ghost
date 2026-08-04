using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.VoicePipeline
{
    public static class Act6PipelineData
    {
        public const string VisitorMessageEndpointId = "visitor_message";
        public const string IntentClassificationId = "intent_classification";
        public const string EntityExtractionId = "entity_extraction";
        public const string ConfidenceFallbackId = "confidence_fallback";
        public const string DialogueManagementId = "dialogue_management";
        public const string ResponseGenerationId = "response_generation";
        public const string GhostReplyEndpointId = "ghost_reply";
        public const string BackendActionId = "backend_action";

        public const string KeywordGuessId = "keyword_guess";
        public const string SkipDetailsId = "skip_details";
        public const string AlwaysAnswerId = "always_answer";
        public const string FirstReplyId = "first_reply";
        public const string RawDataReplyId = "raw_data_reply";
        public const string ObjectRoomBackendId = "object_room_backend";
        public const string ExactWordingId = "exact_wording";
        public const string NounsOnlyId = "nouns_only";
        public const string RejectAllId = "reject_all";
        public const string FixedRouteId = "fixed_route";
        public const string FixedSentenceId = "fixed_sentence";
        public const string VisitorProfileBackendId = "visitor_profile_backend";

        public const string UnclearRequestIntentId = "unclear_request";
        public const string FindItemIntentId = "find_item";
        public const string LabHoursIntentId = "lab_hours";
        public const string WrongLabKeywordIntentId = "lab_location";

        public const string ContinueDecisionId = "continue";
        public const string FallbackDecisionId = "fallback";

        public const string FallbackRouteId = "fallback";
        public const string AskObjectRouteId = "ask_object";
        public const string AskRoomRouteId = "ask_room";
        public const string AskTimeRouteId = "ask_time";
        public const string LabHoursRouteId = "lab_hours";
        public const string GenericAnswerRouteId = "generic_answer";
        public const string FirstReplyRouteId = "first_reply";

        public const int ConfidenceThreshold = 65;
        public const string FinalVisitorMessage = "When does the lab close tonight?";
        public const string FinalGhostReply = "The lab closes at 8 PM. I can show you the way.";

        private static readonly string[] MainPipelineOrder =
        {
            IntentClassificationId,
            EntityExtractionId,
            ConfidenceFallbackId,
            DialogueManagementId,
            ResponseGenerationId
        };

        private static readonly string[] MainCandidateIds =
        {
            IntentClassificationId,
            EntityExtractionId,
            ConfidenceFallbackId,
            DialogueManagementId,
            ResponseGenerationId,
            KeywordGuessId,
            SkipDetailsId,
            AlwaysAnswerId,
            FirstReplyId,
            RawDataReplyId,
            ExactWordingId,
            NounsOnlyId,
            RejectAllId,
            FixedRouteId,
            FixedSentenceId
        };

        private static readonly string[] BackendCandidateIds =
        {
            BackendActionId,
            ObjectRoomBackendId,
            VisitorProfileBackendId
        };

        private static readonly string[] PaletteOrder =
        {
            IntentClassificationId,
            KeywordGuessId,
            ExactWordingId,
            EntityExtractionId,
            SkipDetailsId,
            NounsOnlyId,
            ConfidenceFallbackId,
            AlwaysAnswerId,
            RejectAllId,
            DialogueManagementId,
            FirstReplyId,
            FixedRouteId,
            ResponseGenerationId,
            RawDataReplyId,
            FixedSentenceId,
            BackendActionId,
            ObjectRoomBackendId,
            VisitorProfileBackendId
        };

        public static IReadOnlyList<string> CreateMainPipelineOrder()
        {
            return (string[])MainPipelineOrder.Clone();
        }

        public static IReadOnlyList<Act6PipelineComponent> CreatePaletteComponents()
        {
            var components = new List<Act6PipelineComponent>();
            foreach (var componentId in PaletteOrder)
            {
                components.Add(GetComponent(componentId));
            }

            return components;
        }

        public static IReadOnlyList<Act6PipelineTestCase> CreateTestCases()
        {
            return new[]
            {
                new Act6PipelineTestCase(
                    "unclear-request",
                    "Could you do the thing from before?",
                    "Ghost: Could you rephrase what you need?",
                    UnclearRequestIntentId,
                    42,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    FallbackDecisionId,
                    FallbackRouteId,
                    false),
                new Act6PipelineTestCase(
                    "find-missing-room",
                    "Can you help me find my brass key?",
                    "Ghost: Which room should I search for the brass key?",
                    FindItemIntentId,
                    91,
                    "brass key",
                    string.Empty,
                    string.Empty,
                    ContinueDecisionId,
                    AskRoomRouteId,
                    false),
                new Act6PipelineTestCase(
                    "check-hours",
                    FinalVisitorMessage,
                    "Ghost: " + FinalGhostReply,
                    LabHoursIntentId,
                    94,
                    string.Empty,
                    string.Empty,
                    "tonight",
                    ContinueDecisionId,
                    LabHoursRouteId,
                    true)
            };
        }

        public static Act6PipelineComponent GetComponent(string componentId)
        {
            switch (componentId)
            {
                case VisitorMessageEndpointId:
                    return new Act6PipelineComponent(
                        VisitorMessageEndpointId,
                        "Visitor message",
                        "Fixed start of the voice path.",
                        "The visitor supplies the request.",
                        "Ghost: I did not receive the visitor's message.",
                        false);
                case IntentClassificationId:
                    return new Act6PipelineComponent(
                        IntentClassificationId,
                        "Compare the visitor's purpose",
                        "Intent: group different wording by purpose.",
                        "Chapter 1: intent classification uses your labelled message groups.",
                        "Ghost: I cannot tell what kind of help the visitor wants.",
                        false);
                case EntityExtractionId:
                    return new Act6PipelineComponent(
                        EntityExtractionId,
                        "Keep WHAT / WHERE / WHEN",
                        "Entities: keep the object, place, and time.",
                        "Chapter 2: entity extraction supplies the object, place, and time details.",
                        "Ghost: I lost WHAT, WHERE, or WHEN.",
                        false);
                case ConfidenceFallbackId:
                    return new Act6PipelineComponent(
                        ConfidenceFallbackId,
                        "Check confidence first",
                        "Answer or ask the visitor to rephrase.",
                        "Chapter 4: confidence decides whether Ghost continues or uses fallback.",
                        "Ghost: I am not sure whether I should answer or ask again.",
                        false);
                case DialogueManagementId:
                    return new Act6PipelineComponent(
                        DialogueManagementId,
                        "Follow the reply map",
                        "Dialogue: choose the tested branch.",
                        "Chapter 3 built the route; Chapter 5 proved it with test conversations.",
                        "Ghost: I followed the wrong branch and reached the wrong reply.",
                        false);
                case ResponseGenerationId:
                    return new Act6PipelineComponent(
                        ResponseGenerationId,
                        "Turn the result into a sentence",
                        "Response: make a complete reply.",
                        "Chapter 6 turns a route or backend value into Ghost's reply.",
                        "Ghost: I have a result, but I cannot say it clearly.",
                        false);
                case GhostReplyEndpointId:
                    return new Act6PipelineComponent(
                        GhostReplyEndpointId,
                        "Ghost reply",
                        "Fixed end of the voice path.",
                        "The visitor receives Ghost's complete response.",
                        "Ghost: My reply is ready, but the visitor cannot hear it.",
                        false);
                case BackendActionId:
                    return new Act6PipelineComponent(
                        BackendActionId,
                        "Fetch lab closing time",
                        "Backend: get the lab closing time.",
                        "Chapter 6: this action returns closing_time=8 PM.",
                        "Ghost: I cannot fetch the lab closing time.",
                        true);
                case KeywordGuessId:
                    return new Act6PipelineComponent(
                        KeywordGuessId,
                        "Guess from one keyword",
                        "One word decides the visitor's purpose.",
                        "Shortcut: this ignores the intent groups built in Chapter 1.",
                        "Ghost: I matched one word and guessed the wrong request.",
                        false);
                case SkipDetailsId:
                    return new Act6PipelineComponent(
                        SkipDetailsId,
                        "Skip the details",
                        "Ignores WHAT, WHERE, and WHEN.",
                        "Shortcut: this ignores the entity work from Chapter 2.",
                        "Ghost: I lost the detail needed by the reply map.",
                        false);
                case AlwaysAnswerId:
                    return new Act6PipelineComponent(
                        AlwaysAnswerId,
                        "Always answer",
                        "Continues even when confidence is low.",
                        "Shortcut: this removes the fallback from Chapter 4.",
                        "Ghost: I answered even though I was unsure.",
                        false);
                case FirstReplyId:
                    return new Act6PipelineComponent(
                        FirstReplyId,
                        "Take the first reply",
                        "Skips the tested reply map.",
                        "Shortcut: this ignores Chapters 3 and 5.",
                        "Ghost: Hello!",
                        false);
                case RawDataReplyId:
                    return new Act6PipelineComponent(
                        RawDataReplyId,
                        "Read the raw result",
                        "Says a value without a full sentence.",
                        "Shortcut: this skips response generation from Chapter 6.",
                        "Ghost: 8 PM",
                        false);
                case ObjectRoomBackendId:
                    return new Act6PipelineComponent(
                        ObjectRoomBackendId,
                        "Find object room",
                        "Backend: get an object's room.",
                        "Wrong action: it returns object_room, not closing_time.",
                        "Ghost: I fetched a room instead of a closing time.",
                        true);
                case ExactWordingId:
                    return new Act6PipelineComponent(
                        ExactWordingId,
                        "Match exact wording",
                        "Intent: accepts only a memorised phrase.",
                        "Shortcut: new wording may express the same purpose.",
                        "Ghost: That sentence does not match one I remember.",
                        false);
                case NounsOnlyId:
                    return new Act6PipelineComponent(
                        NounsOnlyId,
                        "Keep nouns only",
                        "Entities: drops places and times.",
                        "Shortcut: WHAT remains, but WHERE and WHEN are lost.",
                        "Ghost: I kept an object but lost its place or time.",
                        false);
                case RejectAllId:
                    return new Act6PipelineComponent(
                        RejectAllId,
                        "Ask everyone again",
                        "Fallback: rejects every request.",
                        "Shortcut: safe, but clear requests never continue.",
                        "Ghost: Could everyone repeat everything?",
                        false);
                case FixedRouteId:
                    return new Act6PipelineComponent(
                        FixedRouteId,
                        "Use one fixed route",
                        "Dialogue: sends every request one way.",
                        "Shortcut: different requests cannot branch.",
                        "Ghost: I sent every visitor to the same reply.",
                        false);
                case FixedSentenceId:
                    return new Act6PipelineComponent(
                        FixedSentenceId,
                        "Use one fixed sentence",
                        "Response: ignores the route result.",
                        "Shortcut: the reply never uses the processed result.",
                        "Ghost: I can help with that.",
                        false);
                case VisitorProfileBackendId:
                    return new Act6PipelineComponent(
                        VisitorProfileBackendId,
                        "Fetch visitor profile",
                        "Backend: gets a name, not closing time.",
                        "Wrong action: it returns visitor_name.",
                        "Ghost: I fetched a visitor name instead of a time.",
                        true);
                default:
                    throw new ArgumentException("Unknown pipeline component id.", nameof(componentId));
            }
        }

        public static bool IsMainPipelineComponent(string componentId)
        {
            foreach (var candidateId in MainCandidateIds)
            {
                if (string.Equals(candidateId, componentId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsBackendComponent(string componentId)
        {
            foreach (var candidateId in BackendCandidateIds)
            {
                if (string.Equals(candidateId, componentId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsKnownComponent(string componentId)
        {
            return string.Equals(componentId, VisitorMessageEndpointId, StringComparison.Ordinal) ||
                string.Equals(componentId, GhostReplyEndpointId, StringComparison.Ordinal) ||
                IsMainPipelineComponent(componentId) ||
                IsBackendComponent(componentId);
        }
    }
}
