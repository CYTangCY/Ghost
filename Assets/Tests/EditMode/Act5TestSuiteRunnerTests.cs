using System.Linq;
using Ghost.Puzzles.TestingDebugging;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act5TestSuiteRunnerTests
    {
        [Test]
        public void BuggyGraphFailsEveryAuthoredConversation()
        {
            var result = Act5TestSuiteRunner.Run(
                Act5BuggyGraphData.CreateBuggyGraph(),
                Act5BuggyGraphData.CreateTestConversations());

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.PassedCount, Is.EqualTo(0));
            Assert.That(result.CaseResults.Count, Is.EqualTo(4));
            Assert.That(result.ValidationErrors, Is.Not.Empty);
        }

        [Test]
        public void FixedGraphPassesEveryAuthoredConversation()
        {
            var result = Act5TestSuiteRunner.Run(
                Act5BuggyGraphData.CreateFixedGraph(),
                Act5BuggyGraphData.CreateTestConversations());

            Assert.That(result.IsCorrect, Is.True);
            Assert.That(result.PassedCount, Is.EqualTo(4));
            Assert.That(result.ValidationErrors, Is.Empty);
        }

        [Test]
        public void BuggyGraphReportsExpectedAndActualResponseForRoomCase()
        {
            var result = Act5TestSuiteRunner.Run(
                Act5BuggyGraphData.CreateBuggyGraph(),
                Act5BuggyGraphData.CreateTestConversations());
            var roomCase = result.CaseResults.Single(
                item => item.Conversation.Id == "find-with-room");

            Assert.That(
                roomCase.Conversation.TestCase.ExpectedResponseId,
                Is.EqualTo(Act5BuggyGraphData.AnswerObjectLocationResponseId));
            Assert.That(
                roomCase.ActualResponseId,
                Is.EqualTo(Act5BuggyGraphData.AskForRoomResponseId));
            Assert.That(roomCase.Passed, Is.False);
        }

        [Test]
        public void BuggyGraphReportsNoResponseForMissingGreetingBranch()
        {
            var result = Act5TestSuiteRunner.Run(
                Act5BuggyGraphData.CreateBuggyGraph(),
                Act5BuggyGraphData.CreateTestConversations());
            var greetingCase = result.CaseResults.Single(
                item => item.Conversation.Id == "greeting");

            Assert.That(greetingCase.ActualResponseId, Is.Null);
            Assert.That(greetingCase.Passed, Is.False);
        }
    }
}