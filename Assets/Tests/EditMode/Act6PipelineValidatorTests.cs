using System.Collections.Generic;
using System.Linq;
using Ghost.Puzzles.VoicePipeline;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act6PipelineValidatorTests
    {
        [Test]
        public void LearnedRouteSimulatesThreeDifferentVisitors()
        {
            var result = Act6PipelineValidator.Validate(
                Act6PipelineData.CreateMainPipelineOrder(),
                Act6PipelineData.BackendActionId);

            Assert.That(result.IsCorrect, Is.True);
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.FirstBrokenComponentId, Is.Empty);
            Assert.That(result.TestResults.Count, Is.EqualTo(3));
            Assert.That(result.PassedTestCount, Is.EqualTo(3));
            Assert.That(result.TestResults.All(test => test.Passed), Is.True);

            Assert.That(
                result.TestResults.Select(test => test.TestCase.ExpectedDialogueRouteId),
                Is.EquivalentTo(new[]
                {
                    Act6PipelineData.FallbackRouteId,
                    Act6PipelineData.AskRoomRouteId,
                    Act6PipelineData.LabHoursRouteId
                }));
            Assert.That(
                result.TestResults[0].TraceSteps.Any(
                    step => step.ComponentId == Act6PipelineData.BackendActionId),
                Is.False);
            Assert.That(
                result.TestResults[1].TraceSteps.Any(
                    step => step.ComponentId == Act6PipelineData.BackendActionId),
                Is.False);
            Assert.That(
                result.TestResults[2].TraceSteps.Any(
                    step => step.ComponentId == Act6PipelineData.BackendActionId),
                Is.True);
        }

        [Test]
        public void EveryCorrectVisitorTraceUsesTheFivePlacedStages()
        {
            var result = Act6PipelineValidator.Validate(
                Act6PipelineData.CreateMainPipelineOrder(),
                Act6PipelineData.BackendActionId);

            foreach (var visitor in result.TestResults)
            {
                var traceIds = visitor.TraceSteps
                    .Select(step => step.ComponentId)
                    .ToArray();
                foreach (var stageId in Act6PipelineData.CreateMainPipelineOrder())
                {
                    Assert.That(traceIds, Does.Contain(stageId), visitor.TestCase.Id);
                }

                Assert.That(
                    visitor.TraceSteps.First().ComponentId,
                    Is.EqualTo(Act6PipelineData.VisitorMessageEndpointId));
                Assert.That(
                    visitor.TraceSteps.Last().ComponentId,
                    Is.EqualTo(Act6PipelineData.GhostReplyEndpointId));
            }
        }

        [Test]
        public void PaletteCardsUseConcisePlayerFacingText()
        {
            foreach (var component in Act6PipelineData.CreatePaletteComponents())
            {
                Assert.That(component.Label.Length, Is.LessThanOrEqualTo(32), component.Id);
                Assert.That(component.JobLine.Length, Is.LessThanOrEqualTo(48), component.Id);
            }
        }

        [Test]
        public void PaletteProvidesThreeChoicesForEachOfSixResponsibilities()
        {
            var palette = Act6PipelineData.CreatePaletteComponents();

            Assert.That(palette.Count, Is.EqualTo(18));
            Assert.That(
                palette.Select(component => component.Id),
                Does.Contain(Act6PipelineData.ExactWordingId));
            Assert.That(
                palette.Select(component => component.Id),
                Does.Contain(Act6PipelineData.NounsOnlyId));
            Assert.That(
                palette.Select(component => component.Id),
                Does.Contain(Act6PipelineData.RejectAllId));
            Assert.That(
                palette.Select(component => component.Id),
                Does.Contain(Act6PipelineData.FixedRouteId));
            Assert.That(
                palette.Select(component => component.Id),
                Does.Contain(Act6PipelineData.FixedSentenceId));
            Assert.That(
                palette.Select(component => component.Id),
                Does.Contain(Act6PipelineData.VisitorProfileBackendId));
        }

        [Test]
        public void FixedEndpointsAreNotPaletteChoices()
        {
            var paletteIds = Act6PipelineData.CreatePaletteComponents()
                .Select(component => component.Id)
                .ToArray();

            Assert.That(paletteIds, Does.Not.Contain(Act6PipelineData.VisitorMessageEndpointId));
            Assert.That(paletteIds, Does.Not.Contain(Act6PipelineData.GhostReplyEndpointId));
            Assert.That(paletteIds, Does.Contain(Act6PipelineData.IntentClassificationId));
            Assert.That(paletteIds, Does.Contain(Act6PipelineData.KeywordGuessId));
        }

        [Test]
        public void SwappedOpeningStagesStopAtTheFirstRealDependency()
        {
            var result = Act6PipelineValidator.Validate(
                new[]
                {
                    Act6PipelineData.EntityExtractionId,
                    Act6PipelineData.IntentClassificationId,
                    Act6PipelineData.ConfidenceFallbackId,
                    Act6PipelineData.DialogueManagementId,
                    Act6PipelineData.ResponseGenerationId
                },
                Act6PipelineData.BackendActionId);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(
                result.FirstBrokenComponentId,
                Is.EqualTo(Act6PipelineData.IntentClassificationId));
            Assert.That(result.PassedTestCount, Is.EqualTo(0));
            Assert.That(
                result.TestResults[0].TraceSteps.Last().ComponentId,
                Is.EqualTo(Act6PipelineData.EntityExtractionId));
            Assert.That(result.TestResults[0].TraceSteps.Last().Succeeded, Is.False);
        }

        [Test]
        public void SkippingEntitiesPassesFallbackButChangesDetailReplies()
        {
            var stages = new List<string>(Act6PipelineData.CreateMainPipelineOrder());
            stages[1] = Act6PipelineData.SkipDetailsId;

            var result = Act6PipelineValidator.Validate(
                stages,
                Act6PipelineData.BackendActionId);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(
                result.FirstBrokenComponentId,
                Is.EqualTo(Act6PipelineData.EntityExtractionId));
            Assert.That(result.PassedTestCount, Is.EqualTo(1));
            Assert.That(result.TestResults[0].Passed, Is.True);
            Assert.That(result.TestResults[1].Passed, Is.False);
            Assert.That(result.TestResults[2].Passed, Is.False);
            Assert.That(
                result.TestResults[1].ActualReply,
                Is.EqualTo("Ghost: What should I help you find?"));
            Assert.That(
                result.TestResults[2].ActualReply,
                Is.EqualTo("Ghost: Which time do you mean?"));
        }

        [Test]
        public void AlwaysAnswerFailsOnlyTheLowConfidenceVisitor()
        {
            var stages = new List<string>(Act6PipelineData.CreateMainPipelineOrder());
            stages[2] = Act6PipelineData.AlwaysAnswerId;

            var result = Act6PipelineValidator.Validate(
                stages,
                Act6PipelineData.BackendActionId);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.PassedTestCount, Is.EqualTo(2));
            Assert.That(result.TestResults[0].Passed, Is.False);
            Assert.That(result.TestResults[1].Passed, Is.True);
            Assert.That(result.TestResults[2].Passed, Is.True);
            Assert.That(
                result.TestResults[0].ActualReply,
                Is.EqualTo("Ghost: I will try that."));
        }

        [Test]
        public void KeywordGuessFailsTheLabHoursWording()
        {
            var stages = new List<string>(Act6PipelineData.CreateMainPipelineOrder());
            stages[0] = Act6PipelineData.KeywordGuessId;

            var result = Act6PipelineValidator.Validate(
                stages,
                Act6PipelineData.BackendActionId);

            Assert.That(result.PassedTestCount, Is.EqualTo(2));
            Assert.That(result.TestResults[0].Passed, Is.True);
            Assert.That(result.TestResults[1].Passed, Is.True);
            Assert.That(result.TestResults[2].Passed, Is.False);
            Assert.That(
                result.TestResults[2].ActualReply,
                Is.EqualTo("Ghost: I will try that."));
        }

        [Test]
        public void WrongBackendRunsOnlyForTheHoursVisitor()
        {
            var result = Act6PipelineValidator.Validate(
                Act6PipelineData.CreateMainPipelineOrder(),
                Act6PipelineData.ObjectRoomBackendId);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.PassedTestCount, Is.EqualTo(2));
            Assert.That(result.TestResults[0].Passed, Is.True);
            Assert.That(result.TestResults[1].Passed, Is.True);
            Assert.That(result.TestResults[2].Passed, Is.False);
            Assert.That(
                result.TestResults[2].ActualReply,
                Is.EqualTo("Ghost: The lab closes at archive room. I can show you the way."));
            Assert.That(
                result.TestResults[2].TraceSteps.Any(
                    step => step.Line.Contains("object_room=archive room")),
                Is.True);
        }

        [TestCase(Act6PipelineData.ExactWordingId, 0)]
        [TestCase(Act6PipelineData.NounsOnlyId, 1)]
        [TestCase(Act6PipelineData.RejectAllId, 2)]
        [TestCase(Act6PipelineData.FixedRouteId, 3)]
        [TestCase(Act6PipelineData.FixedSentenceId, 4)]
        public void NewMainPathChoicesHaveDeterministicConsequences(
            string componentId,
            int slotIndex)
        {
            var stages = new List<string>(Act6PipelineData.CreateMainPipelineOrder());
            stages[slotIndex] = componentId;

            var result = Act6PipelineValidator.Validate(
                stages,
                Act6PipelineData.BackendActionId);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.PassedTestCount, Is.LessThan(3));
        }

        [Test]
        public void VisitorProfileBackendReturnsTheWrongField()
        {
            var result = Act6PipelineValidator.Validate(
                Act6PipelineData.CreateMainPipelineOrder(),
                Act6PipelineData.VisitorProfileBackendId);

            Assert.That(result.PassedTestCount, Is.EqualTo(2));
            Assert.That(result.TestResults[2].Passed, Is.False);
            Assert.That(result.TestResults[2].ActualReply, Does.Contain("Ada"));
        }

        [Test]
        public void MissingBackendStopsTheHoursVisitorBeforeResponseGeneration()
        {
            var result = Act6PipelineValidator.Validate(
                Act6PipelineData.CreateMainPipelineOrder(),
                string.Empty);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.PassedTestCount, Is.EqualTo(2));
            Assert.That(result.TestResults[2].Passed, Is.False);
            Assert.That(
                result.TestResults[2].FirstBrokenComponentId,
                Is.EqualTo(Act6PipelineData.BackendActionId));
            Assert.That(result.TestResults[2].TraceSteps.Last().Succeeded, Is.False);
            Assert.That(result.Errors.Single(), Does.Contain("empty"));
        }

        [Test]
        public void DuplicateLearnedStageFailsDeterministically()
        {
            var result = Act6PipelineValidator.Validate(
                new[]
                {
                    Act6PipelineData.IntentClassificationId,
                    Act6PipelineData.IntentClassificationId,
                    Act6PipelineData.ConfidenceFallbackId,
                    Act6PipelineData.DialogueManagementId,
                    Act6PipelineData.ResponseGenerationId
                },
                Act6PipelineData.BackendActionId);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(
                result.FirstBrokenComponentId,
                Is.EqualTo(Act6PipelineData.EntityExtractionId));
            Assert.That(result.Errors.Any(error => error.Contains("more than once")), Is.True);
        }

        [Test]
        public void CorrectRepliesAreGeneratedByTheTrace()
        {
            var result = Act6PipelineValidator.Validate(
                Act6PipelineData.CreateMainPipelineOrder(),
                Act6PipelineData.BackendActionId);

            foreach (var testResult in result.TestResults)
            {
                Assert.That(testResult.ActualReply, Is.EqualTo(testResult.TestCase.ExpectedReply));
                Assert.That(testResult.TraceSteps.Count, Is.GreaterThan(6));
            }

            Assert.That(
                result.TestResults[0].TraceSteps.Any(
                    step => step.Line.Contains("Decision=fallback")),
                Is.True);
            Assert.That(
                result.TestResults[1].TraceSteps.Any(
                    step => step.Line.Contains("Route=ask_room")),
                Is.True);
            Assert.That(
                result.TestResults[2].TraceSteps.Any(
                    step => step.Line.Contains("closing_time=8 PM")),
                Is.True);
        }
    }
}
