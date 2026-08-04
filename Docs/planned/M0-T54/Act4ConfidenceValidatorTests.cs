using System.Collections.Generic;
using System.Linq;
using Ghost.Puzzles.ConfidenceFallback;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act4ConfidenceValidatorTests
    {
        // Both of these pass the evening. They are the two risk postures the chapter is about.
        private static Act4ZoneConfiguration Bold()
        {
            return new Act4ZoneConfiguration(40, 60, rephraseWired: true, lilyWired: true);
        }

        private static Act4ZoneConfiguration Cautious()
        {
            return new Act4ZoneConfiguration(40, 68, rephraseWired: true, lilyWired: true);
        }

        private static IReadOnlyList<Act4VisitorMessage> Queue()
        {
            return Act4ConfidenceDemoData.CreateVisitorMessages();
        }

        private static Act4VisitorRunResult Find(Act4ConfidenceValidationResult result, string id)
        {
            return result.VisitorResults.First(visitor => visitor.Visitor.Id == id);
        }

        [Test]
        public void BoldHandlesPassTheEvening()
        {
            var result = Act4ConfidenceValidator.Validate(Bold(), Queue());

            Assert.That(result.IsCorrect, Is.True, string.Join("\n", result.Errors));
            Assert.That(result.Posture, Is.EqualTo(Act4Posture.Bold));
            Assert.That(Find(result, "courier-vague").Outcome, Is.EqualTo(Act4RouteOutcome.IntentReply));
        }

        [Test]
        public void CautiousHandlesAlsoPassTheEvening()
        {
            var result = Act4ConfidenceValidator.Validate(Cautious(), Queue());

            Assert.That(result.IsCorrect, Is.True, string.Join("\n", result.Errors));
            Assert.That(result.Posture, Is.EqualTo(Act4Posture.Cautious));
            Assert.That(Find(result, "courier-vague").Outcome, Is.EqualTo(Act4RouteOutcome.Fallback));
        }

        [Test]
        public void TheTwoPosturesDisagreeAboutTheCourierAndAgreeOnEveryoneElse()
        {
            var bold = Act4ConfidenceValidator.Validate(Bold(), Queue());
            var cautious = Act4ConfidenceValidator.Validate(Cautious(), Queue());

            Assert.That(
                Find(bold, "courier-vague").Outcome,
                Is.Not.EqualTo(Find(cautious, "courier-vague").Outcome),
                "The courier is the whole decision; if both postures route him the same way there is no choice.");

            foreach (var id in new[] { "vending-machine", "locked-out", "hurried-parent" })
            {
                Assert.That(Find(bold, id).Outcome, Is.EqualTo(Find(cautious, id).Outcome), id);
            }
        }

        [Test]
        public void AnswerHandleAboveTheObviousQuestionBouncesIt()
        {
            var config = new Act4ZoneConfiguration(40, 95, rephraseWired: true, lilyWired: true);

            var result = Act4ConfidenceValidator.Validate(config, Queue());

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(Find(result, "vending-machine").Outcome, Is.EqualTo(Act4RouteOutcome.Fallback));
        }

        [Test]
        public void UpsetVisitorLeftInTheRephraseBandMeltsDown()
        {
            var config = new Act4ZoneConfiguration(20, 60, rephraseWired: true, lilyWired: true);

            var result = Act4ConfidenceValidator.Validate(config, Queue());
            var lockedOut = Find(result, "locked-out");

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(lockedOut.Zone, Is.EqualTo(Act4Zone.AskRephrase));
            Assert.That(lockedOut.Outcome, Is.EqualTo(Act4RouteOutcome.Meltdown));
            Assert.That(result.Errors, Has.Some.Contains("say it all again"));
        }

        [Test]
        public void UpsetVisitorAnsweredByGhostAlsoMeltsDownButForTheOtherReason()
        {
            var config = new Act4ZoneConfiguration(20, 30, rephraseWired: true, lilyWired: true);

            var result = Act4ConfidenceValidator.Validate(config, Queue());
            var lockedOut = Find(result, "locked-out");

            Assert.That(lockedOut.Zone, Is.EqualTo(Act4Zone.Answer));
            Assert.That(lockedOut.Outcome, Is.EqualTo(Act4RouteOutcome.Meltdown));
            Assert.That(result.Errors, Has.Some.Contains("confidently and wrongly"));
        }

        [Test]
        public void CallingLilyForTheCourierIsOverEscalation()
        {
            var config = new Act4ZoneConfiguration(70, 71, rephraseWired: true, lilyWired: true);

            var result = Act4ConfidenceValidator.Validate(config, Queue());

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(Find(result, "courier-vague").Outcome, Is.EqualTo(Act4RouteOutcome.Handoff));
            Assert.That(result.Errors, Has.Some.Contains("did not need a human"));
        }

        [Test]
        public void AnUnwiredBandLeavesGhostSilent()
        {
            var config = new Act4ZoneConfiguration(40, 68, rephraseWired: false, lilyWired: true);

            var result = Act4ConfidenceValidator.Validate(config, Queue());

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(Find(result, "courier-vague").Outcome, Is.EqualTo(Act4RouteOutcome.NoSafeRoute));
            Assert.That(result.Errors, Has.Some.Contains("no way to ask for a rephrase"));
        }

        [Test]
        public void WithoutLilyTheUpsetVisitorIsLeftStandingThere()
        {
            var config = new Act4ZoneConfiguration(40, 60, rephraseWired: true, lilyWired: false);

            var result = Act4ConfidenceValidator.Validate(config, Queue());
            var lockedOut = Find(result, "locked-out");

            Assert.That(lockedOut.Zone, Is.EqualTo(Act4Zone.CallLily));
            Assert.That(lockedOut.Outcome, Is.EqualTo(Act4RouteOutcome.Meltdown));
            Assert.That(lockedOut.Line, Is.EqualTo(lockedOut.Visitor.Lines.Silence));
            Assert.That(result.Errors, Has.Some.Contains("standing in silence"));
        }

        [Test]
        public void ScoreboardCountsTheEvening()
        {
            var tally = Act4ConfidenceValidator.Validate(Cautious(), Queue()).Tally;

            Assert.That(tally.Answered, Is.EqualTo(2));
            Assert.That(tally.Rephrased, Is.EqualTo(1));
            Assert.That(tally.HandedOff, Is.EqualTo(1));
            Assert.That(tally.Upset, Is.EqualTo(0));
        }

        // The bug this chapter was rebuilt to kill: the old pass band was 65-80 and not one visitor
        // scored inside it, so sliding the dial changed nothing and the puzzle solved itself. Each
        // case below is the exact handle position where that visitor's fate changes.
        [TestCase("vending-machine", 88, 89)]
        [TestCase("courier-vague", 63, 64)]
        [TestCase("hurried-parent", 71, 72)]
        public void AnswerHandleFlipsTheVisitorAtTheDocumentedBoundary(string id, int answers, int bounces)
        {
            var answered = Act4ConfidenceValidator.Validate(
                new Act4ZoneConfiguration(35, answers, rephraseWired: true, lilyWired: true), Queue());
            var bounced = Act4ConfidenceValidator.Validate(
                new Act4ZoneConfiguration(35, bounces, rephraseWired: true, lilyWired: true), Queue());

            Assert.That(Find(answered, id).Outcome, Is.EqualTo(Act4RouteOutcome.IntentReply));
            Assert.That(Find(bounced, id).Outcome, Is.EqualTo(Act4RouteOutcome.Fallback));
        }

        [Test]
        public void LilyHandleFlipsTheUpsetVisitorAtThirtyFive()
        {
            var toLily = Act4ConfidenceValidator.Validate(
                new Act4ZoneConfiguration(35, 60, rephraseWired: true, lilyWired: true), Queue());
            var toRephrase = Act4ConfidenceValidator.Validate(
                new Act4ZoneConfiguration(34, 60, rephraseWired: true, lilyWired: true), Queue());

            Assert.That(Find(toLily, "locked-out").Outcome, Is.EqualTo(Act4RouteOutcome.Handoff));
            Assert.That(Find(toRephrase, "locked-out").Outcome, Is.EqualTo(Act4RouteOutcome.Meltdown));
        }

        [Test]
        public void EveryVisitorFlips()
        {
            foreach (var visitor in Queue())
            {
                var seen = new HashSet<Act4RouteOutcome>();

                for (var lily = 0; lily <= 100; lily += 5)
                {
                    for (var answer = lily; answer <= 100; answer += 5)
                    {
                        var config = new Act4ZoneConfiguration(lily, answer, rephraseWired: true, lilyWired: true);
                        seen.Add(Act4ConfidenceValidator.RunVisitor(config, visitor).Outcome);
                    }
                }

                Assert.That(
                    seen.Count,
                    Is.GreaterThan(1),
                    visitor.Id + " never changes outcome no matter where the handles go, so the dial is " +
                    "decorative for them. Re-author the score.");
            }
        }

        [Test]
        public void HandlesCannotCrossOver()
        {
            Assert.Throws<System.ArgumentException>(
                () => new Act4ZoneConfiguration(70, 40, rephraseWired: true, lilyWired: true));
        }
    }
}
