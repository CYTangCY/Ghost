using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.VoicePipeline
{
    public static class Act6PipelineValidator
    {
        public static Act6PipelineValidationResult Validate(
            IReadOnlyList<string> mainSlots,
            string backendComponentId)
        {
            var errors = new List<string>();
            var expectedOrder = Act6PipelineData.CreateMainPipelineOrder();
            var seenIds = new HashSet<string>(StringComparer.Ordinal);
            var firstBrokenComponentId = string.Empty;

            for (var slotIndex = 0; slotIndex < expectedOrder.Count; slotIndex++)
            {
                var expectedId = expectedOrder[slotIndex];
                var actualId = GetSlotValue(mainSlots, slotIndex);

                if (string.IsNullOrWhiteSpace(actualId))
                {
                    errors.Add("Main stage " + (slotIndex + 1) + " is empty.");
                    SetFirstBroken(ref firstBrokenComponentId, expectedId);
                    continue;
                }

                if (!Act6PipelineData.IsMainPipelineComponent(actualId))
                {
                    errors.Add("Main stage " + (slotIndex + 1) + " contains a part that does not belong on the main path.");
                    SetFirstBroken(ref firstBrokenComponentId, expectedId);
                    continue;
                }

                if (!seenIds.Add(actualId))
                {
                    errors.Add("The same voice part appears more than once on the main path.");
                    SetFirstBroken(ref firstBrokenComponentId, expectedId);
                }

                if (!string.Equals(actualId, expectedId, StringComparison.Ordinal))
                {
                    errors.Add("Main stage " + (slotIndex + 1) + " does not continue the visitor message correctly.");
                    SetFirstBroken(ref firstBrokenComponentId, expectedId);
                }
            }

            if (mainSlots != null && mainSlots.Count > expectedOrder.Count)
            {
                errors.Add("The main voice path has more than five editable stages.");
                SetFirstBroken(ref firstBrokenComponentId, Act6PipelineData.ResponseGenerationId);
            }

            if (string.IsNullOrWhiteSpace(backendComponentId))
            {
                errors.Add("The backend action socket is empty.");
                SetFirstBroken(ref firstBrokenComponentId, Act6PipelineData.BackendActionId);
            }
            else if (!Act6PipelineData.IsBackendComponent(backendComponentId))
            {
                errors.Add("The side socket contains a part that is not a backend action.");
                SetFirstBroken(ref firstBrokenComponentId, Act6PipelineData.BackendActionId);
            }
            else if (!string.Equals(
                         backendComponentId,
                         Act6PipelineData.BackendActionId,
                         StringComparison.Ordinal))
            {
                errors.Add("The selected backend action returns the wrong type of fact.");
                SetFirstBroken(ref firstBrokenComponentId, Act6PipelineData.BackendActionId);
            }

            var testResults = RunTestCases(mainSlots, backendComponentId, expectedOrder);
            return new Act6PipelineValidationResult(errors, firstBrokenComponentId, testResults);
        }

        private static IReadOnlyList<Act6PipelineTestResult> RunTestCases(
            IReadOnlyList<string> mainSlots,
            string backendComponentId,
            IReadOnlyList<string> expectedOrder)
        {
            var results = new List<Act6PipelineTestResult>();
            foreach (var testCase in Act6PipelineData.CreateTestCases())
            {
                results.Add(SimulateVisitor(
                    testCase,
                    mainSlots,
                    backendComponentId,
                    expectedOrder));
            }

            return results;
        }

        private static Act6PipelineTestResult SimulateVisitor(
            Act6PipelineTestCase testCase,
            IReadOnlyList<string> mainSlots,
            string backendComponentId,
            IReadOnlyList<string> expectedOrder)
        {
            var trace = new List<Act6PipelineTraceStep>
            {
                new Act6PipelineTraceStep(
                    Act6PipelineData.VisitorMessageEndpointId,
                    "Visitor message enters",
                    "Visitor: \"" + testCase.VisitorMessage + "\"",
                    true)
            };
            var state = new PipelineState();
            var firstAlternativeComponentId = string.Empty;

            for (var slotIndex = 0; slotIndex < expectedOrder.Count; slotIndex++)
            {
                if (slotIndex == expectedOrder.Count - 1 &&
                    string.Equals(
                        state.DialogueRouteId,
                        Act6PipelineData.LabHoursRouteId,
                        StringComparison.Ordinal) &&
                    !state.BackendAttempted)
                {
                    var backendSucceeded = ExecuteBackend(
                        backendComponentId,
                        state,
                        out var backendLine);
                    var backendTraceId = string.IsNullOrWhiteSpace(backendComponentId)
                        ? Act6PipelineData.BackendActionId
                        : backendComponentId;
                    trace.Add(new Act6PipelineTraceStep(
                        backendTraceId,
                        "Backend action",
                        backendLine,
                        backendSucceeded));

                    if (!backendSucceeded)
                    {
                        return new Act6PipelineTestResult(
                            testCase,
                            Act6PipelineData.GetComponent(Act6PipelineData.BackendActionId).FailureLine,
                            Act6PipelineData.BackendActionId,
                            trace);
                    }

                    if (!string.Equals(
                            backendComponentId,
                            Act6PipelineData.BackendActionId,
                            StringComparison.Ordinal) &&
                        string.IsNullOrWhiteSpace(firstAlternativeComponentId))
                    {
                        firstAlternativeComponentId = backendTraceId;
                    }
                }

                var actualId = GetSlotValue(mainSlots, slotIndex);
                var expectedId = expectedOrder[slotIndex];
                if (string.IsNullOrWhiteSpace(actualId))
                {
                    var emptyStageLine = "Stage " + (slotIndex + 1) + " is empty. " +
                        Act6PipelineData.GetComponent(expectedId).FailureLine;
                    trace.Add(new Act6PipelineTraceStep(
                        expectedId,
                        "Empty stage " + (slotIndex + 1),
                        emptyStageLine,
                        false));
                    return new Act6PipelineTestResult(
                        testCase,
                        Act6PipelineData.GetComponent(expectedId).FailureLine,
                        expectedId,
                        trace);
                }

                if (!Act6PipelineData.IsMainPipelineComponent(actualId))
                {
                    trace.Add(new Act6PipelineTraceStep(
                        expectedId,
                        "Wrong card type",
                        "This card cannot process a visitor message on the main path.",
                        false));
                    return new Act6PipelineTestResult(
                        testCase,
                        Act6PipelineData.GetComponent(expectedId).FailureLine,
                        expectedId,
                        trace);
                }

                if (!string.Equals(actualId, expectedId, StringComparison.Ordinal) &&
                    string.IsNullOrWhiteSpace(firstAlternativeComponentId))
                {
                    firstAlternativeComponentId = actualId;
                }

                var succeeded = ExecuteMainStage(
                    actualId,
                    testCase,
                    state,
                    out var line,
                    out var failureReply);
                trace.Add(new Act6PipelineTraceStep(
                    actualId,
                    Act6PipelineData.GetComponent(actualId).Label,
                    line,
                    succeeded));

                if (!succeeded)
                {
                    return new Act6PipelineTestResult(
                        testCase,
                        failureReply,
                        actualId,
                        trace);
                }
            }

            var replyMatches = string.Equals(
                state.Reply,
                testCase.ExpectedReply,
                StringComparison.Ordinal);
            trace.Add(new Act6PipelineTraceStep(
                Act6PipelineData.GhostReplyEndpointId,
                replyMatches ? "Ghost gives the expected reply" : "Ghost gives a different reply",
                string.IsNullOrWhiteSpace(state.Reply)
                    ? "Ghost has no complete reply."
                    : state.Reply,
                replyMatches));

            var brokenComponentId = replyMatches
                ? string.Empty
                : string.IsNullOrWhiteSpace(firstAlternativeComponentId)
                    ? Act6PipelineData.ResponseGenerationId
                    : firstAlternativeComponentId;
            return new Act6PipelineTestResult(
                testCase,
                state.Reply,
                brokenComponentId,
                trace);
        }

        private static bool ExecuteMainStage(
            string componentId,
            Act6PipelineTestCase testCase,
            PipelineState state,
            out string line,
            out string failureReply)
        {
            failureReply = Act6PipelineData.GetComponent(componentId).FailureLine;
            switch (componentId)
            {
                case Act6PipelineData.IntentClassificationId:
                    state.IntentReady = true;
                    state.IntentId = testCase.IntentId;
                    state.ConfidencePercent = testCase.ConfidencePercent;
                    line = "Intent=" + state.IntentId +
                        "; confidence=" + state.ConfidencePercent + "%.";
                    return true;

                case Act6PipelineData.KeywordGuessId:
                    state.IntentReady = true;
                    GuessFromKeyword(testCase.VisitorMessage, state);
                    line = "One keyword guessed intent=" + state.IntentId +
                        "; confidence=" + state.ConfidencePercent + "%.";
                    return true;

                case Act6PipelineData.ExactWordingId:
                    state.IntentReady = true;
                    state.IntentId = Act6PipelineData.UnclearRequestIntentId;
                    state.ConfidencePercent = 20;
                    line = "No memorised phrase matched; intent=unclear_request; confidence=20%.";
                    return true;

                case Act6PipelineData.EntityExtractionId:
                    if (!state.IntentReady)
                    {
                        line = "There is no identified purpose to attach WHAT, WHERE, or WHEN to.";
                        return false;
                    }

                    state.EntitiesReady = true;
                    state.WhatValue = testCase.WhatValue;
                    state.WhereValue = testCase.WhereValue;
                    state.WhenValue = testCase.WhenValue;
                    line = FormatEntities(state);
                    return true;

                case Act6PipelineData.SkipDetailsId:
                    if (!state.IntentReady)
                    {
                        line = "The shortcut skipped details before Ghost understood the request.";
                        return false;
                    }

                    state.EntitiesReady = true;
                    state.WhatValue = string.Empty;
                    state.WhereValue = string.Empty;
                    state.WhenValue = string.Empty;
                    line = "WHAT, WHERE, and WHEN were discarded.";
                    return true;

                case Act6PipelineData.NounsOnlyId:
                    if (!state.IntentReady)
                    {
                        line = "Nouns were collected before Ghost understood the request.";
                        return false;
                    }

                    state.EntitiesReady = true;
                    state.WhatValue = testCase.WhatValue;
                    state.WhereValue = string.Empty;
                    state.WhenValue = string.Empty;
                    line = "WHAT=" + FormatValue(state.WhatValue) + "; WHERE=missing; WHEN=missing.";
                    return true;

                case Act6PipelineData.ConfidenceFallbackId:
                    if (!state.IntentReady || !state.EntitiesReady)
                    {
                        line = "Confidence cannot be checked before purpose and details are ready.";
                        return false;
                    }

                    state.DecisionReady = true;
                    state.DecisionId = state.ConfidencePercent < Act6PipelineData.ConfidenceThreshold
                        ? Act6PipelineData.FallbackDecisionId
                        : Act6PipelineData.ContinueDecisionId;
                    line = "Decision=" + state.DecisionId + " because " +
                        state.ConfidencePercent + "% is " +
                        (state.ConfidencePercent < Act6PipelineData.ConfidenceThreshold ? "below " : "at or above ") +
                        Act6PipelineData.ConfidenceThreshold + "%.";
                    return true;

                case Act6PipelineData.AlwaysAnswerId:
                    if (!state.IntentReady || !state.EntitiesReady)
                    {
                        line = "The shortcut tried to answer before the request was ready.";
                        return false;
                    }

                    state.DecisionReady = true;
                    state.DecisionId = Act6PipelineData.ContinueDecisionId;
                    line = "Decision=continue, even at " + state.ConfidencePercent + "% confidence.";
                    return true;

                case Act6PipelineData.RejectAllId:
                    if (!state.IntentReady || !state.EntitiesReady)
                    {
                        line = "The fallback ran before purpose and details were ready.";
                        return false;
                    }

                    state.DecisionReady = true;
                    state.DecisionId = Act6PipelineData.FallbackDecisionId;
                    line = "Decision=fallback for every confidence score.";
                    return true;

                case Act6PipelineData.DialogueManagementId:
                    if (!state.DecisionReady)
                    {
                        line = "The reply map has no confidence decision to follow.";
                        return false;
                    }

                    state.DialogueReady = true;
                    state.DialogueRouteId = ChooseDialogueRoute(state);
                    line = GetDialogueTraceLine(state);
                    return true;

                case Act6PipelineData.FirstReplyId:
                    state.DialogueReady = true;
                    state.DialogueRouteId = Act6PipelineData.FirstReplyRouteId;
                    line = "The first reply was selected without checking the route.";
                    return true;

                case Act6PipelineData.FixedRouteId:
                    state.DialogueReady = true;
                    state.DialogueRouteId = Act6PipelineData.GenericAnswerRouteId;
                    line = "Route=generic_answer for every visitor.";
                    return true;

                case Act6PipelineData.ResponseGenerationId:
                    if (!state.DialogueReady)
                    {
                        line = "There is no dialogue result to turn into a reply.";
                        return false;
                    }

                    if (!TryGenerateReply(state, out state.Reply))
                    {
                        line = "The selected route needs a backend value that is not available.";
                        failureReply = Act6PipelineData.GetComponent(
                            Act6PipelineData.BackendActionId).FailureLine;
                        return false;
                    }

                    line = "Complete reply created from route=" + state.DialogueRouteId + ".";
                    return true;

                case Act6PipelineData.RawDataReplyId:
                    if (!state.DialogueReady)
                    {
                        line = "There is no result to read aloud.";
                        return false;
                    }

                    var rawValue = !string.IsNullOrWhiteSpace(state.BackendValue)
                        ? state.BackendValue
                        : !string.IsNullOrWhiteSpace(state.WhatValue)
                            ? state.WhatValue
                            : state.DialogueRouteId;
                    state.Reply = "Ghost: " + rawValue;
                    line = "Raw result spoken: " + rawValue + ".";
                    return true;

                case Act6PipelineData.FixedSentenceId:
                    if (!state.DialogueReady)
                    {
                        line = "There is no dialogue result before the fixed sentence.";
                        return false;
                    }

                    state.Reply = "Ghost: I can help with that.";
                    line = "The same fixed sentence was used for every route.";
                    return true;

                default:
                    line = "This card has no main-path behaviour.";
                    return false;
            }
        }

        private static void GuessFromKeyword(string visitorMessage, PipelineState state)
        {
            var message = (visitorMessage ?? string.Empty).ToLowerInvariant();
            if (message.Contains("key"))
            {
                state.IntentId = Act6PipelineData.FindItemIntentId;
                state.ConfidencePercent = 77;
                return;
            }

            if (message.Contains("lab") || message.Contains("close"))
            {
                state.IntentId = Act6PipelineData.WrongLabKeywordIntentId;
                state.ConfidencePercent = 79;
                return;
            }

            state.IntentId = Act6PipelineData.UnclearRequestIntentId;
            state.ConfidencePercent = 35;
        }

        private static string ChooseDialogueRoute(PipelineState state)
        {
            if (string.Equals(
                    state.DecisionId,
                    Act6PipelineData.FallbackDecisionId,
                    StringComparison.Ordinal))
            {
                return Act6PipelineData.FallbackRouteId;
            }

            if (string.Equals(
                    state.IntentId,
                    Act6PipelineData.FindItemIntentId,
                    StringComparison.Ordinal))
            {
                if (string.IsNullOrWhiteSpace(state.WhatValue))
                {
                    return Act6PipelineData.AskObjectRouteId;
                }

                return string.IsNullOrWhiteSpace(state.WhereValue)
                    ? Act6PipelineData.AskRoomRouteId
                    : Act6PipelineData.GenericAnswerRouteId;
            }

            if (string.Equals(
                    state.IntentId,
                    Act6PipelineData.LabHoursIntentId,
                    StringComparison.Ordinal))
            {
                return string.IsNullOrWhiteSpace(state.WhenValue)
                    ? Act6PipelineData.AskTimeRouteId
                    : Act6PipelineData.LabHoursRouteId;
            }

            return Act6PipelineData.GenericAnswerRouteId;
        }

        private static string GetDialogueTraceLine(PipelineState state)
        {
            switch (state.DialogueRouteId)
            {
                case Act6PipelineData.FallbackRouteId:
                    return "Route=fallback because confidence is too low.";
                case Act6PipelineData.AskObjectRouteId:
                    return "Route=ask_object because WHAT is missing.";
                case Act6PipelineData.AskRoomRouteId:
                    return "Route=ask_room because WHERE is missing.";
                case Act6PipelineData.AskTimeRouteId:
                    return "Route=ask_time because WHEN is missing.";
                case Act6PipelineData.LabHoursRouteId:
                    return "Route=lab_hours; a closing-time fact is needed.";
                default:
                    return "Route=generic_answer.";
            }
        }

        private static bool ExecuteBackend(
            string backendComponentId,
            PipelineState state,
            out string line)
        {
            state.BackendAttempted = true;
            if (string.IsNullOrWhiteSpace(backendComponentId) ||
                !Act6PipelineData.IsBackendComponent(backendComponentId))
            {
                line = "The lab-hours route reached an empty backend action socket.";
                return false;
            }

            if (string.Equals(
                    backendComponentId,
                    Act6PipelineData.BackendActionId,
                    StringComparison.Ordinal))
            {
                state.BackendFieldId = "closing_time";
                state.BackendValue = "8 PM";
                line = "Fetch lab closing time returned closing_time=8 PM.";
                return true;
            }

            if (string.Equals(
                    backendComponentId,
                    Act6PipelineData.ObjectRoomBackendId,
                    StringComparison.Ordinal))
            {
                state.BackendFieldId = "object_room";
                state.BackendValue = "archive room";
                line = "Find object room returned object_room=archive room.";
                return true;
            }

            state.BackendFieldId = "visitor_name";
            state.BackendValue = "Ada";
            line = "Fetch visitor profile returned visitor_name=Ada.";
            return true;
        }

        private static bool TryGenerateReply(PipelineState state, out string reply)
        {
            switch (state.DialogueRouteId)
            {
                case Act6PipelineData.FallbackRouteId:
                    reply = "Ghost: Could you rephrase what you need?";
                    return true;
                case Act6PipelineData.AskObjectRouteId:
                    reply = "Ghost: What should I help you find?";
                    return true;
                case Act6PipelineData.AskRoomRouteId:
                    reply = "Ghost: Which room should I search for the " + state.WhatValue + "?";
                    return true;
                case Act6PipelineData.AskTimeRouteId:
                    reply = "Ghost: Which time do you mean?";
                    return true;
                case Act6PipelineData.LabHoursRouteId:
                    if (!state.BackendAttempted ||
                        string.IsNullOrWhiteSpace(state.BackendValue))
                    {
                        reply = string.Empty;
                        return false;
                    }

                    reply = "Ghost: The lab closes at " + state.BackendValue +
                        ". I can show you the way.";
                    return true;
                case Act6PipelineData.FirstReplyRouteId:
                    reply = "Ghost: Hello!";
                    return true;
                default:
                    reply = "Ghost: I will try that.";
                    return true;
            }
        }

        private static string FormatEntities(PipelineState state)
        {
            return "Entities: WHAT=" + FormatValue(state.WhatValue) +
                ", WHERE=" + FormatValue(state.WhereValue) +
                ", WHEN=" + FormatValue(state.WhenValue) + ".";
        }

        private static string FormatValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "missing" : value;
        }

        private static string GetSlotValue(IReadOnlyList<string> mainSlots, int slotIndex)
        {
            return mainSlots != null && slotIndex >= 0 && slotIndex < mainSlots.Count
                ? mainSlots[slotIndex]
                : string.Empty;
        }

        private static void SetFirstBroken(ref string currentId, string componentId)
        {
            if (string.IsNullOrWhiteSpace(currentId))
            {
                currentId = componentId ?? string.Empty;
            }
        }

        private sealed class PipelineState
        {
            public bool IntentReady;
            public string IntentId = string.Empty;
            public int ConfidencePercent;
            public bool EntitiesReady;
            public string WhatValue = string.Empty;
            public string WhereValue = string.Empty;
            public string WhenValue = string.Empty;
            public bool DecisionReady;
            public string DecisionId = string.Empty;
            public bool DialogueReady;
            public string DialogueRouteId = string.Empty;
            public bool BackendAttempted;
            public string BackendFieldId = string.Empty;
            public string BackendValue = string.Empty;
            public string Reply = string.Empty;
        }
    }
}
