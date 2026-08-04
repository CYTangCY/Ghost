using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.ConfidenceFallback
{
    public enum Act4RouteOutcome
    {
        IntentReply,
        Fallback,
        Handoff,
        NoSafeRoute,
        Meltdown
    }

    /// <summary>
    /// The three bands the player carves out of the 0-100 confidence axis with two handles.
    /// </summary>
    public enum Act4Zone
    {
        CallLily,
        AskRephrase,
        Answer
    }

    /// <summary>
    /// Which way the player's unavoidable misses fell. No pair of handles serves everyone, so the
    /// debrief names the trade-off they chose rather than declaring them right.
    /// </summary>
    public enum Act4Posture
    {
        Balanced,
        Bold,
        Cautious
    }

    /// <summary>
    /// Authored text for every way one visitor's evening can end. Ghost needs a line for each because
    /// the player is allowed to route anyone anywhere, including badly.
    /// </summary>
    public sealed class Act4VisitorLines
    {
        public Act4VisitorLines(string answered, string rephrased, string handedOff, string silence)
        {
            Answered = answered ?? string.Empty;
            Rephrased = rephrased ?? string.Empty;
            HandedOff = handedOff ?? string.Empty;
            Silence = silence ?? string.Empty;
        }

        public string Answered { get; }

        public string Rephrased { get; }

        public string HandedOff { get; }

        public string Silence { get; }

        /// <summary>
        /// Keyed on the band, not the outcome, so an upset visitor's meltdown still reads differently
        /// depending on whether Ghost bluffed at her or told her to say it all again.
        /// </summary>
        public string For(Act4Zone zone, bool wired)
        {
            if (!wired)
            {
                return Silence;
            }

            switch (zone)
            {
                case Act4Zone.Answer:
                    return Answered;
                case Act4Zone.AskRephrase:
                    return Rephrased;
                default:
                    return HandedOff;
            }
        }
    }

    public sealed class Act4VisitorMessage
    {
        public Act4VisitorMessage(
            string id,
            string message,
            int confidenceScore,
            Act4RouteOutcome idealOutcome,
            Act4VisitorLines lines,
            bool isCritical = false,
            bool soundsUpset = false)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Visitor id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Visitor message cannot be empty.", nameof(message));
            }

            if (confidenceScore < 0 || confidenceScore > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(confidenceScore), "Confidence must be 0-100.");
            }

            Id = id;
            Message = message;
            ConfidenceScore = confidenceScore;
            IdealOutcome = idealOutcome;
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
            IsCritical = isCritical;
            SoundsUpset = soundsUpset;
        }

        public string Id { get; }

        public string Message { get; }

        public int ConfidenceScore { get; }

        /// <summary>What should ideally happen to this person. Often unreachable alongside the others.</summary>
        public Act4RouteOutcome IdealOutcome { get; }

        /// <summary>
        /// A critical visitor must get their ideal outcome or the evening fails outright. Everyone
        /// else is a trade-off: missing them costs something but does not end the shift.
        /// </summary>
        public bool IsCritical { get; }

        public Act4VisitorLines Lines { get; }

        /// <summary>Upset visitors only end well if a human takes them. Rephrasing makes it worse.</summary>
        public bool SoundsUpset { get; }

        public bool Accepts(Act4RouteOutcome outcome)
        {
            return outcome == IdealOutcome;
        }
    }

    /// <summary>
    /// Where the player put the two handles, and whether each band actually has an action attached.
    /// An unwired band means Ghost has nothing to do and just goes quiet.
    /// </summary>
    public sealed class Act4ZoneConfiguration
    {
        public Act4ZoneConfiguration(int handoffEdge, int answerEdge, bool rephraseWired, bool lilyWired)
        {
            if (handoffEdge < 0 || handoffEdge > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(handoffEdge), "Handle must be 0-100.");
            }

            if (answerEdge < 0 || answerEdge > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(answerEdge), "Handle must be 0-100.");
            }

            if (handoffEdge > answerEdge)
            {
                throw new ArgumentException("The Lily handle cannot sit above the answer handle.", nameof(handoffEdge));
            }

            HandoffEdge = handoffEdge;
            AnswerEdge = answerEdge;
            RephraseWired = rephraseWired;
            LilyWired = lilyWired;
        }

        /// <summary>Anything below this goes to Lily.</summary>
        public int HandoffEdge { get; }

        /// <summary>Anything at or above this, Ghost answers itself.</summary>
        public int AnswerEdge { get; }

        public bool RephraseWired { get; }

        public bool LilyWired { get; }

        public Act4Zone ZoneFor(int confidenceScore)
        {
            if (confidenceScore < HandoffEdge)
            {
                return Act4Zone.CallLily;
            }

            return confidenceScore < AnswerEdge ? Act4Zone.AskRephrase : Act4Zone.Answer;
        }
    }

    public sealed class Act4VisitorRunResult
    {
        internal Act4VisitorRunResult(Act4VisitorMessage visitor, Act4Zone zone, Act4RouteOutcome outcome, string line)
        {
            Visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
            Zone = zone;
            Outcome = outcome;
            Line = line ?? string.Empty;
        }

        public Act4VisitorMessage Visitor { get; }

        public Act4Zone Zone { get; }

        public Act4RouteOutcome Outcome { get; }

        public string Line { get; }

        public bool IsAccepted => Visitor.Accepts(Outcome);
    }

    /// <summary>Counts for the end-of-shift scoreboard.</summary>
    public sealed class Act4ShiftTally
    {
        internal Act4ShiftTally(int answered, int rephrased, int handedOff, int upset, int overCautious, int overConfident)
        {
            OverCautious = overCautious;
            OverConfident = overConfident;
            Answered = answered;
            Rephrased = rephrased;
            HandedOff = handedOff;
            Upset = upset;
        }

        public int Answered { get; }

        public int Rephrased { get; }

        public int HandedOff { get; }

        /// <summary>Meltdowns plus people Ghost left standing in silence.</summary>
        public int Upset { get; }

        /// <summary>People Ghost made repeat themselves who did not need to.</summary>
        public int OverCautious { get; }

        /// <summary>People Ghost answered when it should have checked or escalated.</summary>
        public int OverConfident { get; }
    }

    public sealed class Act4ConfidenceValidationResult
    {
        private readonly Act4VisitorRunResult[] visitorResults;
        private readonly string[] errors;

        internal Act4ConfidenceValidationResult(
            IEnumerable<Act4VisitorRunResult> visitorResults,
            IEnumerable<string> errors,
            Act4Posture posture,
            Act4ShiftTally tally)
        {
            this.visitorResults = visitorResults == null
                ? Array.Empty<Act4VisitorRunResult>()
                : new List<Act4VisitorRunResult>(visitorResults).ToArray();
            this.errors = errors == null ? Array.Empty<string>() : new List<string>(errors).ToArray();
            Posture = posture;
            Tally = tally;
        }

        public bool IsCorrect => errors.Length == 0;

        public IReadOnlyList<Act4VisitorRunResult> VisitorResults => visitorResults;

        public IReadOnlyList<string> Errors => errors;

        public Act4Posture Posture { get; }

        public Act4ShiftTally Tally { get; }
    }
}
