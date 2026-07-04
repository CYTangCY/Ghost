using System.Collections.Generic;
using Ghost.Puzzles.EntityExtraction;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act2ErrandOutcomeEngineTests
    {
        [Test]
        public void Evaluate_WhenAllCorrectSpansSubmitted_ReturnsSuccess()
        {
            foreach (var errand in Act2ErrandDemoData.CreateErrands())
            {
                var result = Act2ErrandOutcomeEngine.Evaluate(
                    errand,
                    new List<EntitySpan>(errand.Message.CorrectSpans));

                Assert.That(result.IsSuccess, Is.True, errand.ErrandId);
                Assert.That(result.ValidatorResult.IsCorrect, Is.True, errand.ErrandId);
                Assert.That(result.Mood, Is.EqualTo(Act2ErrandGhostMood.Happy), errand.ErrandId);
                Assert.That(result.OutcomeLine, Is.EqualTo(errand.SuccessOutcomeLine), errand.ErrandId);
            }
        }

        [Test]
        public void Evaluate_WhenWhenSlotIsMissing_ReturnsAuthoredMissingWhenFailure()
        {
            var errand = FindErrand(Act2ErrandDemoData.LabAtNightErrandId);
            var roomSpan = FindExpectedSpan(errand, Act2EntityExtractionSampleData.RoomEntityTypeId);

            var result = Act2ErrandOutcomeEngine.Evaluate(errand, new[] { roomSpan });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Mood, Is.EqualTo(Act2ErrandGhostMood.Sad));
            Assert.That(result.OutcomeLine, Is.EqualTo(
                "Ghost finds the lab, but arrives at midnight after the humming has gone sleepy."));
            Assert.That(FindSlotResult(result, Act2ErrandSlotId.When).State, Is.EqualTo(Act2ErrandSlotState.Missing));
            Assert.That(FindSlotResult(result, Act2ErrandSlotId.Where).State, Is.EqualTo(Act2ErrandSlotState.Correct));
        }

        [Test]
        public void Evaluate_WhenWhatSlotUsesWrongToken_ReturnsAuthoredWrongWhatFailure()
        {
            var errand = FindErrand(Act2ErrandDemoData.LanternObjectErrandId);
            var wrongObjectSpan = new EntitySpan(
                0,
                "Ghost".Length,
                Act2EntityExtractionSampleData.CreateObjectEntityType());

            var result = Act2ErrandOutcomeEngine.Evaluate(errand, new[] { wrongObjectSpan });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Mood, Is.EqualTo(Act2ErrandGhostMood.Confused));
            Assert.That(result.OutcomeLine, Is.EqualTo(
                "Ghost proudly delivers the wrong object and then looks very sorry."));
            Assert.That(FindSlotResult(result, Act2ErrandSlotId.What).State, Is.EqualTo(Act2ErrandSlotState.Wrong));
        }

        [Test]
        public void Evaluate_WhenLaboratoryTaggedAsRoom_ReturnsSuccessAndResolutionText()
        {
            var errand = FindErrand(Act2ErrandDemoData.LaboratorySynonymErrandId);

            var result = Act2ErrandOutcomeEngine.Evaluate(
                errand,
                new List<EntitySpan>(errand.Message.CorrectSpans));
            var whereResult = FindSlotResult(result, Act2ErrandSlotId.Where);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(whereResult.State, Is.EqualTo(Act2ErrandSlotState.Correct));
            Assert.That(whereResult.ResolutionText, Is.EqualTo("laboratory -> lab room"));

            var resolution = Act2ErrandDemoData.CreateSynonymResolutions()[0];
            Assert.That(resolution.SurfaceTexts, Does.Contain("lab"));
            Assert.That(resolution.SurfaceTexts, Does.Contain("laboratory"));
            Assert.That(resolution.CanonicalLabel, Is.EqualTo(Act2ErrandDemoData.LabRoomCanonicalLabel));
        }

        private static Act2ErrandDemoData.ErrandDefinition FindErrand(string errandId)
        {
            foreach (var errand in Act2ErrandDemoData.CreateErrands())
            {
                if (errand.ErrandId == errandId)
                {
                    return errand;
                }
            }

            Assert.Fail("Errand not found: " + errandId);
            return null;
        }

        private static EntitySpan FindExpectedSpan(
            Act2ErrandDemoData.ErrandDefinition errand,
            string entityTypeId)
        {
            foreach (var span in errand.Message.CorrectSpans)
            {
                if (span.Type.Id == entityTypeId)
                {
                    return span;
                }
            }

            Assert.Fail("Expected span not found for type: " + entityTypeId);
            return null;
        }

        private static Act2ErrandSlotResult FindSlotResult(
            Act2ErrandOutcome outcome,
            Act2ErrandSlotId slotId)
        {
            foreach (var slotResult in outcome.SlotResults)
            {
                if (slotResult.SlotId == slotId)
                {
                    return slotResult;
                }
            }

            Assert.Fail("Slot result not found: " + slotId);
            return null;
        }
    }
}
