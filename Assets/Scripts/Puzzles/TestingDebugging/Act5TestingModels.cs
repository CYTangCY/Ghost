using System;
using System.Collections.Generic;
using Ghost.Puzzles.DialogGraph;

namespace Ghost.Puzzles.TestingDebugging
{
    public sealed class Act5TestConversation
    {
        public Act5TestConversation(string id, string visitorMessage, ConversationTurn turn, string expectedResponseId)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Act 5 test conversation id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(visitorMessage))
            {
                throw new ArgumentException("Act 5 visitor message cannot be empty.", nameof(visitorMessage));
            }

            Id = id;
            VisitorMessage = visitorMessage;
            TestCase = new DialogGraphTestCase(id, turn, expectedResponseId);
        }

        public string Id { get; }

        public string VisitorMessage { get; }

        public DialogGraphTestCase TestCase { get; }
    }

    public sealed class Act5TestCaseResult
    {
        internal Act5TestCaseResult(Act5TestConversation conversation, string actualResponseId)
        {
            Conversation = conversation ?? throw new ArgumentNullException(nameof(conversation));
            ActualResponseId = actualResponseId;
            Passed = string.Equals(
                Conversation.TestCase.ExpectedResponseId,
                ActualResponseId,
                StringComparison.Ordinal);
        }

        public Act5TestConversation Conversation { get; }

        public string ActualResponseId { get; }

        public bool Passed { get; }
    }

    public sealed class Act5TestSuiteResult
    {
        private readonly Act5TestCaseResult[] caseResults;
        private readonly string[] validationErrors;

        internal Act5TestSuiteResult(
            IEnumerable<Act5TestCaseResult> caseResults,
            IEnumerable<string> validationErrors,
            bool isCorrect)
        {
            this.caseResults = caseResults == null
                ? Array.Empty<Act5TestCaseResult>()
                : new List<Act5TestCaseResult>(caseResults).ToArray();
            this.validationErrors = validationErrors == null
                ? Array.Empty<string>()
                : new List<string>(validationErrors).ToArray();
            IsCorrect = isCorrect;
        }

        public IReadOnlyList<Act5TestCaseResult> CaseResults => caseResults;

        public IReadOnlyList<string> ValidationErrors => validationErrors;

        public bool IsCorrect { get; }

        public int PassedCount
        {
            get
            {
                var count = 0;

                foreach (var result in caseResults)
                {
                    if (result.Passed)
                    {
                        count++;
                    }
                }

                return count;
            }
        }
    }
}