using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.VoicePipeline
{
    public sealed class Act6PipelineComponent
    {
        public Act6PipelineComponent(
            string id,
            string label,
            string jobLine,
            string priorWorkLine,
            string failureLine,
            bool isBackend)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Component id is required.", nameof(id));
            }

            Id = id.Trim();
            Label = label ?? string.Empty;
            JobLine = jobLine ?? string.Empty;
            PriorWorkLine = priorWorkLine ?? string.Empty;
            FailureLine = failureLine ?? string.Empty;
            IsBackend = isBackend;
        }

        public string Id { get; }

        public string Label { get; }

        public string JobLine { get; }

        public string PriorWorkLine { get; }

        public string FailureLine { get; }

        public bool IsBackend { get; }
    }

    public sealed class Act6PipelinePlaybackStep
    {
        public Act6PipelinePlaybackStep(string componentId, string title, string line)
        {
            ComponentId = componentId ?? string.Empty;
            Title = title ?? string.Empty;
            Line = line ?? string.Empty;
        }

        public string ComponentId { get; }

        public string Title { get; }

        public string Line { get; }
    }

    public sealed class Act6PipelineTestCase
    {
        public Act6PipelineTestCase(
            string id,
            string visitorMessage,
            string expectedReply,
            string intentId,
            int confidencePercent,
            string whatValue,
            string whereValue,
            string whenValue,
            string expectedDecisionId,
            string expectedDialogueRouteId,
            bool requiresBackend)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Test case id is required.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(visitorMessage))
            {
                throw new ArgumentException("Visitor message is required.", nameof(visitorMessage));
            }

            Id = id.Trim();
            VisitorMessage = visitorMessage;
            ExpectedReply = expectedReply ?? string.Empty;
            IntentId = intentId ?? string.Empty;
            ConfidencePercent = confidencePercent;
            WhatValue = whatValue ?? string.Empty;
            WhereValue = whereValue ?? string.Empty;
            WhenValue = whenValue ?? string.Empty;
            ExpectedDecisionId = expectedDecisionId ?? string.Empty;
            ExpectedDialogueRouteId = expectedDialogueRouteId ?? string.Empty;
            RequiresBackend = requiresBackend;
        }

        public string Id { get; }

        public string VisitorMessage { get; }

        public string ExpectedReply { get; }

        public string IntentId { get; }

        public int ConfidencePercent { get; }

        public string WhatValue { get; }

        public string WhereValue { get; }

        public string WhenValue { get; }

        public string ExpectedDecisionId { get; }

        public string ExpectedDialogueRouteId { get; }

        public bool RequiresBackend { get; }
    }

    public sealed class Act6PipelineTraceStep
    {
        public Act6PipelineTraceStep(
            string componentId,
            string title,
            string line,
            bool succeeded)
        {
            ComponentId = componentId ?? string.Empty;
            Title = title ?? string.Empty;
            Line = line ?? string.Empty;
            Succeeded = succeeded;
        }

        public string ComponentId { get; }

        public string Title { get; }

        public string Line { get; }

        public bool Succeeded { get; }
    }

    public sealed class Act6PipelineTestResult
    {
        private readonly Act6PipelineTraceStep[] traceSteps;

        public Act6PipelineTestResult(
            Act6PipelineTestCase testCase,
            string actualReply,
            string firstBrokenComponentId,
            IEnumerable<Act6PipelineTraceStep> trace)
        {
            TestCase = testCase ?? throw new ArgumentNullException(nameof(testCase));
            ActualReply = actualReply ?? string.Empty;
            FirstBrokenComponentId = firstBrokenComponentId ?? string.Empty;
            traceSteps = trace == null
                ? Array.Empty<Act6PipelineTraceStep>()
                : new List<Act6PipelineTraceStep>(trace).ToArray();
            Passed = string.IsNullOrWhiteSpace(FirstBrokenComponentId) &&
                string.Equals(TestCase.ExpectedReply, ActualReply, StringComparison.Ordinal);
        }

        public Act6PipelineTestCase TestCase { get; }

        public string ActualReply { get; }

        public string FirstBrokenComponentId { get; }

        public IReadOnlyList<Act6PipelineTraceStep> TraceSteps => traceSteps;

        public bool Passed { get; }
    }

    public sealed class Act6PipelineValidationResult
    {
        private readonly string[] errors;
        private readonly Act6PipelineTestResult[] testResults;

        public Act6PipelineValidationResult(
            IEnumerable<string> validationErrors,
            string firstBrokenComponentId,
            IEnumerable<Act6PipelineTestResult> results = null)
        {
            var errorCopy = new List<string>();
            if (validationErrors != null)
            {
                foreach (var error in validationErrors)
                {
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        errorCopy.Add(error);
                    }
                }
            }

            errors = errorCopy.ToArray();
            testResults = results == null
                ? Array.Empty<Act6PipelineTestResult>()
                : new List<Act6PipelineTestResult>(results).ToArray();
            FirstBrokenComponentId = firstBrokenComponentId ?? string.Empty;
        }

        public IReadOnlyList<string> Errors => errors;

        public IReadOnlyList<Act6PipelineTestResult> TestResults => testResults;

        public string FirstBrokenComponentId { get; }

        public int PassedTestCount
        {
            get
            {
                var count = 0;
                foreach (var result in testResults)
                {
                    if (result.Passed)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public bool IsCorrect
        {
            get
            {
                if (errors.Length > 0)
                {
                    return false;
                }

                foreach (var result in testResults)
                {
                    if (!result.Passed)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
