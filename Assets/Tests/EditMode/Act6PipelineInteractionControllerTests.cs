using Ghost.Presentation.Act6VoicePipeline;
using Ghost.Puzzles.VoicePipeline;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act6PipelineInteractionControllerTests
    {
        [Test]
        public void VisitorsAreRevealedOneAtATime()
        {
            var controller = CreateCorrectController();

            controller.RunPipeline();

            Assert.That(controller.CurrentPhase, Is.EqualTo(Act6PipelinePhase.VisitorTesting));
            Assert.That(controller.CurrentTestIndex, Is.EqualTo(0));
            Assert.That(controller.ActiveTestCase.Id, Is.EqualTo("unclear-request"));
            Assert.That(controller.IsTestResultVisible(0), Is.False);
            Assert.That(controller.IsTestResultVisible(1), Is.False);

            AdvanceToCurrentReply(controller);

            Assert.That(controller.IsTestResultVisible(0), Is.True);
            Assert.That(controller.IsTestResultVisible(1), Is.False);
            Assert.That(controller.CurrentPhase, Is.EqualTo(Act6PipelinePhase.VisitorTesting));

            controller.AdvanceVisitorTest();

            Assert.That(controller.CurrentTestIndex, Is.EqualTo(1));
            Assert.That(controller.ActiveTestCase.Id, Is.EqualTo("find-missing-room"));
            Assert.That(controller.ActiveTraceStep.ComponentId,
                Is.EqualTo(Act6PipelineData.VisitorMessageEndpointId));
        }

        [Test]
        public void CompletionWaitsUntilAllThreeVisitorTracesAreShown()
        {
            var controller = CreateCorrectController();
            controller.RunPipeline();

            for (var visitorIndex = 0; visitorIndex < controller.TestCases.Count; visitorIndex++)
            {
                Assert.That(controller.CurrentTestIndex, Is.EqualTo(visitorIndex));
                AdvanceToCurrentReply(controller);
                Assert.That(controller.CompletedTestCount, Is.EqualTo(visitorIndex + 1));
                Assert.That(controller.IsTestResultVisible(visitorIndex), Is.True);

                if (visitorIndex < controller.TestCases.Count - 1)
                {
                    controller.AdvanceVisitorTest();
                }
            }

            Assert.That(controller.CurrentPhase, Is.EqualTo(Act6PipelinePhase.VisitorTesting));
            controller.AdvanceVisitorTest();
            Assert.That(controller.CurrentPhase, Is.EqualTo(Act6PipelinePhase.ReadyForEnding));
        }

        [Test]
        public void FailedSequenceReturnsToEditableBoardWithAllResults()
        {
            var controller = CreateCorrectController();
            controller.PlaceInBackendSlot(Act6PipelineData.ObjectRoomBackendId);
            controller.RunPipeline();

            for (var visitorIndex = 0; visitorIndex < controller.TestCases.Count; visitorIndex++)
            {
                AdvanceToCurrentReply(controller);
                if (visitorIndex < controller.TestCases.Count - 1)
                {
                    controller.AdvanceVisitorTest();
                }
            }

            controller.AdvanceVisitorTest();

            Assert.That(controller.CurrentPhase, Is.EqualTo(Act6PipelinePhase.Configure));
            Assert.That(controller.LastValidationResult.PassedTestCount, Is.EqualTo(2));
            Assert.That(controller.IsTestResultVisible(0), Is.True);
            Assert.That(controller.IsTestResultVisible(1), Is.True);
            Assert.That(controller.IsTestResultVisible(2), Is.True);
        }

        [Test]
        public void IncompleteBoardCannotStartVisitorSequence()
        {
            var controller = new Act6PipelineInteractionController();
            controller.BeginAfterOnboarding();

            controller.RunPipeline();

            Assert.That(controller.CurrentPhase, Is.EqualTo(Act6PipelinePhase.Configure));
            Assert.That(controller.LastValidationResult, Is.Null);
            Assert.That(controller.StatusLine, Does.Contain("Fill all five main stages"));
        }

        [Test]
        public void MainRouteCanStartBeforeBackendIsChosen()
        {
            var controller = new Act6PipelineInteractionController();
            controller.BeginAfterOnboarding();
            var order = Act6PipelineData.CreateMainPipelineOrder();
            for (var index = 0; index < order.Count; index++)
            {
                controller.PlaceInMainSlot(order[index], index);
            }

            Assert.That(controller.BackendAttached, Is.False);
            Assert.That(controller.IsPipelineReadyToTest, Is.True);

            controller.RunPipeline();

            Assert.That(controller.CurrentPhase, Is.EqualTo(Act6PipelinePhase.VisitorTesting));
            Assert.That(controller.CurrentTestIndex, Is.EqualTo(0));
        }

        [Test]
        public void VisibleCardTraceChangesWithTheCurrentVisitor()
        {
            var controller = CreateCorrectController();
            controller.RunPipeline();

            Assert.That(
                controller.GetVisibleTraceStepForComponent(
                    Act6PipelineData.IntentClassificationId),
                Is.Null);

            controller.AdvanceVisitorTest();

            Assert.That(
                controller.GetVisibleTraceStepForComponent(
                    Act6PipelineData.IntentClassificationId).Line,
                Does.Contain("Intent=unclear_request"));

            AdvanceToCurrentReply(controller);
            controller.AdvanceVisitorTest();

            Assert.That(controller.CurrentTestIndex, Is.EqualTo(1));
            Assert.That(
                controller.GetVisibleTraceStepForComponent(
                    Act6PipelineData.IntentClassificationId),
                Is.Null);

            controller.AdvanceVisitorTest();

            Assert.That(
                controller.GetVisibleTraceStepForComponent(
                    Act6PipelineData.IntentClassificationId).Line,
                Does.Contain("Intent=find_item"));
        }

        [Test]
        public void EditRouteStopsVisitorAndKeepsPlacedCards()
        {
            var controller = CreateCorrectController();
            controller.RunPipeline();

            controller.CancelVisitorTests();

            Assert.That(controller.CurrentPhase, Is.EqualTo(Act6PipelinePhase.Configure));
            Assert.That(controller.LastValidationResult, Is.Null);
            Assert.That(controller.CurrentTestIndex, Is.EqualTo(-1));
            Assert.That(
                controller.GetMainSlotComponentId(0),
                Is.EqualTo(Act6PipelineData.IntentClassificationId));
            Assert.That(
                controller.BackendComponentId,
                Is.EqualTo(Act6PipelineData.BackendActionId));
        }

        [Test]
        public void BackendChangeRetriesOnlyVisitorThree()
        {
            var controller = CreateCorrectController();
            controller.PlaceInBackendSlot(Act6PipelineData.ObjectRoomBackendId);
            controller.RunPipeline();

            AdvanceToCurrentReply(controller);
            controller.AdvanceVisitorTest();
            AdvanceToCurrentReply(controller);
            controller.AdvanceVisitorTest();

            Assert.That(controller.CurrentTestIndex, Is.EqualTo(2));
            Assert.That(controller.CompletedTestCount, Is.EqualTo(2));
            Assert.That(controller.CanEditBackendForCurrentVisitor, Is.True);

            Assert.That(
                controller.PlaceInBackendSlot(Act6PipelineData.BackendActionId),
                Is.True);

            Assert.That(controller.CurrentTestIndex, Is.EqualTo(2));
            Assert.That(controller.CompletedTestCount, Is.EqualTo(2));
            Assert.That(controller.CurrentTraceIndex, Is.EqualTo(0));
            Assert.That(controller.IsTestResultVisible(0), Is.True);
            Assert.That(controller.IsTestResultVisible(1), Is.True);
            Assert.That(controller.IsTestResultVisible(2), Is.False);

            AdvanceToCurrentReply(controller);
            controller.AdvanceVisitorTest();

            Assert.That(controller.CurrentPhase, Is.EqualTo(Act6PipelinePhase.ReadyForEnding));
        }

        private static Act6PipelineInteractionController CreateCorrectController()
        {
            var controller = new Act6PipelineInteractionController();
            controller.BeginAfterOnboarding();
            var order = Act6PipelineData.CreateMainPipelineOrder();
            for (var index = 0; index < order.Count; index++)
            {
                Assert.That(controller.PlaceInMainSlot(order[index], index), Is.True);
            }

            Assert.That(
                controller.PlaceInBackendSlot(Act6PipelineData.BackendActionId),
                Is.True);
            return controller;
        }

        private static void AdvanceToCurrentReply(
            Act6PipelineInteractionController controller)
        {
            var guard = 0;
            while (!controller.CurrentVisitorReplyShown && guard < 20)
            {
                controller.AdvanceVisitorTest();
                guard++;
            }

            Assert.That(guard, Is.LessThan(20), "Visitor trace did not reach a reply.");
            Assert.That(controller.CurrentVisitorReplyShown, Is.True);
        }
    }
}
