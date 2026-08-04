using System.Collections.Generic;
using System.Linq;
using Ghost.Puzzles.ConfidenceFallback;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    /// <summary>
    /// Chapter 4 is a trade-off, not a puzzle with a right answer. The confidence score is a proxy:
    /// one visitor scores low while being perfectly clear, another scores higher while being genuinely
    /// ambiguous, so no pair of handles can serve everyone. These tests keep that true. An earlier
    /// version had a wide pass band with a perfect answer inside it, which made the chapter solvable
    /// by reading the numbers off the screen instead of reading the messages.
    /// </summary>
    public sealed class Act4ConfidenceValidatorTests
    {
        private static IReadOnlyList<Act4VisitorMessage> Queue()
        {
            return Act4ConfidenceDemoData.CreateVisitorMessages();
        }

        private static Act4ZoneConfiguration Handles(int lily, int answer, bool rephrase = true, bool callLily = true)
        {
            return new Act4ZoneConfiguration(lily, answer, rephrase, callLily);
        }

        private static Act4VisitorRunResult Find(Act4ConfidenceValidationResult result, string id)
        {
            return result.VisitorResults.First(visitor => visitor.Visitor.Id == id);
        }

        private static IEnumerable<(int Lily, int Answer)> AllHandlePairs()
        {
            for (var lily = 0; lily <= 100; lily++)
            {
                for (var answer = lily; answer <= 100; answer++)
                {
                    yield return (lily, answer);
                }
            }
        }

        // The property the whole chapter rests on.
        [Test]
        public void NoSettingPleasesEveryone()
        {
            foreach (var pair in AllHandlePairs())
            {
                var result = Act4ConfidenceValidator.Validate(Handles(pair.Lily, pair.Answer), Queue());

                Assert.That(
                    result.VisitorResults.All(visitor => visitor.IsAccepted),
                    Is.False,
                    "Handles " + pair.Lily + "/" + pair.Answer + " serve every visitor perfectly. " +
                    "The chapter is supposed to have no ideal answer, only a defensible trade-off.");
            }
        }

        [Test]
        public void TheScoreIsAProxyNotTheTruth()
        {
            var queue = Queue();
            var clearButLowScoring = queue.First(visitor => visitor.Id == "odd-phrasing");
            var vagueButHigherScoring = queue.First(visitor => visitor.Id == "genuinely-vague");

            Assert.That(clearButLowScoring.ConfidenceScore, Is.LessThan(vagueButHigherScoring.ConfidenceScore));
            Assert.That(clearButLowScoring.IdealOutcome, Is.EqualTo(Act4RouteOutcome.IntentReply));
            Assert.That(vagueButHigherScoring.IdealOutcome, Is.EqualTo(Act4RouteOutcome.Fallback));
        }

        [Test]
        public void TheHardFloorIsReachable()
        {
            var passing = AllHandlePairs()
                .Count(pair => Act4ConfidenceValidator.Validate(Handles(pair.Lily, pair.Answer), Queue()).IsCorrect);

            Assert.That(passing, Is.GreaterThan(0), "No handle position clears the critical visitors.");
        }

        [Test]
        public void EverySettingThatClearsTheFloorStillCostsSomebody()
        {
            foreach (var pair in AllHandlePairs())
            {
                var result = Act4ConfidenceValidator.Validate(Handles(pair.Lily, pair.Answer), Queue());
                if (!result.IsCorrect)
                {
                    continue;
                }

                Assert.That(
                    result.Tally.OverCautious + result.Tally.OverConfident,
                    Is.GreaterThan(0),
                    "Handles " + pair.Lily + "/" + pair.Answer + " clear the floor at no cost at all.");
            }
        }

        [Test]
        public void TheOpeningHandlesDoNotAlreadyClearTheFloor()
        {
            var opening = Handles(
                Act4ConfidenceDemoData.StartingHandoffEdge,
                Act4ConfidenceDemoData.StartingAnswerEdge);

            Assert.That(Act4ConfidenceValidator.Validate(opening, Queue()).IsCorrect, Is.False);
        }

        [TestCase(Act4Posture.Cautious)]
        [TestCase(Act4Posture.Bold)]
        public void BothPosturesAreReachable(Act4Posture posture)
        {
            var found = AllHandlePairs().Any(pair =>
            {
                var result = Act4ConfidenceValidator.Validate(Handles(pair.Lily, pair.Answer), Queue());
                return result.IsCorrect && result.Posture == posture;
            });

            Assert.That(found, Is.True, "No passing setting produces the " + posture + " trade-off.");
        }

        [Test]
        public void TheUpsetVisitorMustReachAPersonOrTheEveningFails()
        {
            // Handles that drop her into the rephrase band instead of the handoff band.
            var result = Act4ConfidenceValidator.Validate(Handles(20, 80), Queue());

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(Find(result, "locked-out").Outcome, Is.EqualTo(Act4RouteOutcome.Meltdown));
            Assert.That(result.Errors, Has.Some.Contains("say it all again"));
        }

        [Test]
        public void AnsweringTheUpsetVisitorFailsForADifferentReason()
        {
            var result = Act4ConfidenceValidator.Validate(Handles(20, 25), Queue());

            Assert.That(Find(result, "locked-out").Zone, Is.EqualTo(Act4Zone.Answer));
            Assert.That(result.Errors, Has.Some.Contains("confidently and wrongly"));
        }

        [Test]
        public void BouncingTheObviousQuestionFailsTheEvening()
        {
            var result = Act4ConfidenceValidator.Validate(Handles(35, 95), Queue());

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(Find(result, "vending-machine").Outcome, Is.EqualTo(Act4RouteOutcome.Fallback));
        }

        [Test]
        public void AnUnattachedBandLeavesGhostSilentAndFailsTheEvening()
        {
            var noRephrase = Act4ConfidenceValidator.Validate(Handles(35, 70, rephrase: false), Queue());
            var noLily = Act4ConfidenceValidator.Validate(Handles(35, 70, callLily: false), Queue());

            Assert.That(noRephrase.IsCorrect, Is.False);
            Assert.That(noRephrase.Errors, Has.Some.Contains("no way to ask for a rephrase"));
            Assert.That(noLily.IsCorrect, Is.False);
            Assert.That(noLily.Errors, Has.Some.Contains("cannot call Lily"));
        }

        [Test]
        public void TheScoreboardSeparatesTooCautiousFromTooConfident()
        {
            var cautious = Act4ConfidenceValidator.Validate(Handles(31, 80), Queue());

            Assert.That(cautious.IsCorrect, Is.True, string.Join("\n", cautious.Errors));
            Assert.That(cautious.Tally.OverCautious, Is.GreaterThan(0));
            Assert.That(cautious.Posture, Is.EqualTo(Act4Posture.Cautious));
        }

        [Test]
        public void EveryVisitorFlips()
        {
            foreach (var visitor in Queue())
            {
                var seen = new HashSet<Act4RouteOutcome>();
                foreach (var pair in AllHandlePairs())
                {
                    seen.Add(Act4ConfidenceValidator.RunVisitor(Handles(pair.Lily, pair.Answer), visitor).Outcome);
                }

                Assert.That(
                    seen.Count,
                    Is.GreaterThan(1),
                    visitor.Id + " never changes outcome wherever the handles go, so the dial is " +
                    "decorative for them.");
            }
        }

        [Test]
        public void HandlesCannotCrossOver()
        {
            Assert.Throws<System.ArgumentException>(() => Handles(70, 40));
        }
    }
}
