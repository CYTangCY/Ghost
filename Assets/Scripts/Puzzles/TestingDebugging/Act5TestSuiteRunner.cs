using System;
using System.Collections.Generic;
using Ghost.Puzzles.DialogGraph;
using DialogGraphModel = Ghost.Puzzles.DialogGraph.DialogGraph;

namespace Ghost.Puzzles.TestingDebugging
{
    public static class Act5TestSuiteRunner
    {
        public static Act5TestSuiteResult Run(
            DialogGraphModel graph,
            IEnumerable<Act5TestConversation> conversations)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (conversations == null)
            {
                throw new ArgumentNullException(nameof(conversations));
            }

            var copiedConversations = CopyConversations(conversations);
            var testCases = new List<DialogGraphTestCase>();

            foreach (var conversation in copiedConversations)
            {
                testCases.Add(conversation.TestCase);
            }

            var validation = DialogGraphValidator.Validate(graph, testCases);
            var caseResults = new List<Act5TestCaseResult>();

            foreach (var conversation in copiedConversations)
            {
                var simulation = DialogGraphSimulator.Simulate(
                    graph,
                    conversation.TestCase.Turn,
                    new DialogContext());
                caseResults.Add(new Act5TestCaseResult(conversation, simulation.ResponseId));
            }

            return new Act5TestSuiteResult(caseResults, validation.Errors, validation.IsCorrect);
        }

        private static List<Act5TestConversation> CopyConversations(
            IEnumerable<Act5TestConversation> conversations)
        {
            var copied = new List<Act5TestConversation>();

            foreach (var conversation in conversations)
            {
                if (conversation == null)
                {
                    throw new ArgumentException(
                        "Act 5 test suite cannot contain a null conversation.",
                        nameof(conversations));
                }

                copied.Add(conversation);
            }

            return copied;
        }
    }
}