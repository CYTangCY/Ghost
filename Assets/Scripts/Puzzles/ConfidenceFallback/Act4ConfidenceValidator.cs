using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.ConfidenceFallback
{
    public static class Act4ConfidenceValidator
    {
        public static Act4ConfidenceValidationResult Validate(
            Act4ZoneConfiguration configuration,
            IEnumerable<Act4VisitorMessage> visitors)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (visitors == null)
            {
                throw new ArgumentNullException(nameof(visitors));
            }

            var errors = new List<string>();
            var results = new List<Act4VisitorRunResult>();

            if (!configuration.RephraseWired)
            {
                errors.Add("Nothing is attached to the middle band, so Ghost has no way to ask for a rephrase.");
            }

            if (!configuration.LilyWired)
            {
                errors.Add("Nothing is attached to the bottom band, so Ghost cannot call Lily.");
            }

            foreach (var visitor in visitors)
            {
                if (visitor == null)
                {
                    errors.Add("The queue contains an empty visitor.");
                    continue;
                }

                var result = RunVisitor(configuration, visitor);
                results.Add(result);

                // Only critical visitors can fail the evening. Everyone else is a trade-off, because
                // no pair of handles serves the whole queue - see Act4ConfidenceDemoData.
                if (!result.IsAccepted && result.Visitor.IsCritical)
                {
                    errors.Add(Explain(result));
                }
                else if (result.Outcome == Act4RouteOutcome.NoSafeRoute ||
                         result.Outcome == Act4RouteOutcome.Meltdown)
                {
                    errors.Add(Explain(result));
                }
            }

            if (results.Count == 0)
            {
                errors.Add("The queue is empty.");
            }

            return new Act4ConfidenceValidationResult(results, errors, ReadPosture(results), Count(results));
        }

        public static Act4VisitorRunResult RunVisitor(Act4ZoneConfiguration configuration, Act4VisitorMessage visitor)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            var zone = configuration.ZoneFor(visitor.ConfidenceScore);
            var wired = IsWired(configuration, zone);
            var outcome = Route(zone, wired);

            // Someone who is already angry only ends well if a person takes over. Bluffing at her and
            // telling her to say it all again are both meltdowns - the line still differs, because the
            // two mistakes are worth learning separately.
            if (visitor.SoundsUpset && outcome != Act4RouteOutcome.Handoff)
            {
                outcome = Act4RouteOutcome.Meltdown;
            }

            return new Act4VisitorRunResult(visitor, zone, outcome, visitor.Lines.For(zone, wired));
        }

        private static bool IsWired(Act4ZoneConfiguration configuration, Act4Zone zone)
        {
            switch (zone)
            {
                case Act4Zone.AskRephrase:
                    return configuration.RephraseWired;
                case Act4Zone.CallLily:
                    return configuration.LilyWired;
                default:
                    return true;
            }
        }

        private static Act4RouteOutcome Route(Act4Zone zone, bool wired)
        {
            if (!wired)
            {
                return Act4RouteOutcome.NoSafeRoute;
            }

            switch (zone)
            {
                case Act4Zone.Answer:
                    return Act4RouteOutcome.IntentReply;
                case Act4Zone.AskRephrase:
                    return Act4RouteOutcome.Fallback;
                default:
                    return Act4RouteOutcome.Handoff;
            }
        }

        /// <summary>
        /// Reads the shape of the misses the player could not avoid. Asking people who did not need
        /// asking is cautious; answering people who needed checking is bold.
        /// </summary>
        private static Act4Posture ReadPosture(IEnumerable<Act4VisitorRunResult> results)
        {
            var cautious = 0;
            var bold = 0;

            foreach (var result in results)
            {
                if (result.IsAccepted)
                {
                    continue;
                }

                if (IsOverCautious(result))
                {
                    cautious++;
                }
                else if (IsOverConfident(result))
                {
                    bold++;
                }
            }

            if (cautious > bold)
            {
                return Act4Posture.Cautious;
            }

            return bold > cautious ? Act4Posture.Bold : Act4Posture.Balanced;
        }

        /// <summary>Ghost asked for more than the person's message actually needed.</summary>
        private static bool IsOverCautious(Act4VisitorRunResult result)
        {
            return Rank(result.Outcome) < Rank(result.Visitor.IdealOutcome);
        }

        /// <summary>Ghost committed further than the message justified.</summary>
        private static bool IsOverConfident(Act4VisitorRunResult result)
        {
            return Rank(result.Outcome) > Rank(result.Visitor.IdealOutcome);
        }

        /// <summary>How far Ghost is willing to go: hand over, ask again, or answer.</summary>
        private static int Rank(Act4RouteOutcome outcome)
        {
            switch (outcome)
            {
                case Act4RouteOutcome.Handoff:
                    return 0;
                case Act4RouteOutcome.Fallback:
                    return 1;
                case Act4RouteOutcome.IntentReply:
                    return 2;
                default:
                    return -1;
            }
        }

        private static Act4ShiftTally Count(IEnumerable<Act4VisitorRunResult> results)
        {
            int answered = 0, rephrased = 0, handedOff = 0, upset = 0, overCautious = 0, overConfident = 0;

            foreach (var result in results)
            {
                switch (result.Outcome)
                {
                    case Act4RouteOutcome.IntentReply:
                        answered++;
                        break;
                    case Act4RouteOutcome.Fallback:
                        rephrased++;
                        break;
                    case Act4RouteOutcome.Handoff:
                        handedOff++;
                        break;
                    default:
                        upset++;
                        break;
                }

                if (result.IsAccepted)
                {
                    continue;
                }

                if (IsOverCautious(result))
                {
                    overCautious++;
                }
                else if (IsOverConfident(result))
                {
                    overConfident++;
                }
            }

            return new Act4ShiftTally(answered, rephrased, handedOff, upset, overCautious, overConfident);
        }

        private static string Explain(Act4VisitorRunResult result)
        {
            var who = "'" + result.Visitor.Id + "' (" + result.Visitor.ConfidenceScore + ")";

            switch (result.Outcome)
            {
                case Act4RouteOutcome.NoSafeRoute:
                    return who + " landed in a band with nothing attached, so Ghost said nothing.";
                case Act4RouteOutcome.Meltdown:
                    if (result.Zone == Act4Zone.Answer)
                    {
                        return who + " was already upset and Ghost answered anyway, confidently and wrongly.";
                    }

                    return result.Zone == Act4Zone.AskRephrase
                        ? who + " was already upset and Ghost asked her to say it all again."
                        : who + " was already upset and Ghost left her standing in silence.";
                case Act4RouteOutcome.Handoff:
                    return who + " did not need a human; Lily was called for something Ghost could handle.";
                case Act4RouteOutcome.Fallback:
                    return who + " was clear enough to answer, but Ghost asked for a rephrase instead.";
                default:
                    return who + " was answered by Ghost when it should not have been.";
            }
        }
    }
}
