using System;
using System.Collections.Generic;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Banter;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.ConfidenceFallback;

namespace Ghost.Presentation.Act4ConfidenceFallback
{
    /// <summary>
    /// One evening on the front desk, run from two handles on a confidence axis instead of a single
    /// threshold. The player carves the axis into three bands - call Lily, ask to rephrase, answer -
    /// and attaches an action to the two that need one. The old single dial could not change any
    /// outcome inside its own pass band, so it was decorative; see Act4ConfidenceDemoData.
    /// </summary>
    public sealed class Act4ConfidenceInteractionController
    {
        private readonly IReadOnlyList<Act4VisitorMessage> allVisitors;
        private readonly List<Act4VisitorMessage> queue = new List<Act4VisitorMessage>();
        private int playbackIndex;

        public Act4ConfidenceInteractionController()
        {
            allVisitors = Act4ConfidenceDemoData.CreateVisitorMessages();
            HandoffEdge = Act4ConfidenceDemoData.StartingHandoffEdge;
            AnswerEdge = Act4ConfidenceDemoData.StartingAnswerEdge;
            queue.AddRange(allVisitors);
            CurrentPhase = Act4ConfidencePhase.Onboarding;
            CurrentMood = Act4GhostMood.Neutral;
            StatusLine = "Everything lands in the middle band right now, so Ghost asks the whole queue to say it again.";
        }

        public event Action StateChanged;

        public Act4ConfidencePhase CurrentPhase { get; private set; }

        public Act4GhostMood CurrentMood { get; private set; }

        /// <summary>Below this, Ghost calls Lily.</summary>
        public int HandoffEdge { get; private set; }

        /// <summary>At or above this, Ghost answers on its own.</summary>
        public int AnswerEdge { get; private set; }

        public bool RephraseWired { get; private set; }

        public bool LilyWired { get; private set; }

        public bool HasFailedRun { get; private set; }

        public string StatusLine { get; private set; }

        public int TotalVisitors => allVisitors.Count;

        /// <summary>
        /// Confidence scores stay hidden until the evening has been run once. Printing them up front
        /// turned the chapter into arithmetic: the player could place two numbers between six others
        /// without ever reading a message.
        /// </summary>
        public bool ScoresRevealed { get; private set; }

        public IReadOnlyList<Act4VisitorMessage> Visitors => queue;

        public Act4ConfidenceValidationResult LastValidationResult { get; private set; }

        public int CurrentVisitorNumber => CurrentPhase == Act4ConfidencePhase.Playback ? playbackIndex + 1 : 0;

        public Act4VisitorRunResult CurrentVisitorResult
        {
            get
            {
                if (CurrentPhase != Act4ConfidencePhase.Playback || LastValidationResult == null ||
                    playbackIndex < 0 || playbackIndex >= LastValidationResult.VisitorResults.Count)
                {
                    return null;
                }

                return LastValidationResult.VisitorResults[playbackIndex];
            }
        }

        public bool CanRunDay => RephraseWired && LilyWired;

        /// <summary>
        /// What would happen to this visitor at the current handle positions, without running the day.
        /// The panel uses it to recolour each visitor as the handles move, which is the feedback the
        /// single-dial version never gave.
        /// </summary>
        public Act4VisitorRunResult PreviewVisitor(Act4VisitorMessage visitor)
        {
            return visitor == null ? null : Act4ConfidenceValidator.RunVisitor(CreateConfiguration(), visitor);
        }

        public Act4Zone ZoneFor(int confidenceScore)
        {
            return CreateConfiguration().ZoneFor(confidenceScore);
        }

        public void BeginAfterOnboarding()
        {
            if (CurrentPhase != Act4ConfidencePhase.Onboarding)
            {
                return;
            }

            CurrentPhase = Act4ConfidencePhase.Configure;
            NotifyStateChanged();
        }

        public void ReplayOnboarding()
        {
            if (CurrentPhase == Act4ConfidencePhase.Playback || CurrentPhase == Act4ConfidencePhase.Complete)
            {
                return;
            }

            CurrentPhase = Act4ConfidencePhase.Onboarding;
            NotifyStateChanged();
        }

        // These deliberately do NOT raise StateChanged. The presenter rebuilds its whole hierarchy on
        // that event, which would destroy the slider the player is currently dragging - the drag felt
        // like it kept slipping. The presenter updates its own labels and previews instead.

        /// <summary>Moves the lower handle. It can never pass the answer handle.</summary>
        public void SetHandoffEdge(int value)
        {
            HandoffEdge = Clamp(value, 0, AnswerEdge);
        }

        /// <summary>Moves the upper handle. It can never drop below the Lily handle.</summary>
        public void SetAnswerEdge(int value)
        {
            AnswerEdge = Clamp(value, HandoffEdge, 100);
        }

        public void ToggleRephraseWiring()
        {
            if (CurrentPhase != Act4ConfidencePhase.Configure)
            {
                return;
            }

            RephraseWired = !RephraseWired;
            CurrentMood = Act4GhostMood.Neutral;
            StatusLine = RephraseWired
                ? "Attached: anyone in the middle band gets asked to rephrase."
                : "Detached: the middle band has no action, so Ghost stands there in silence.";
            NotifyStateChanged();
        }

        public void ToggleLilyWiring()
        {
            if (CurrentPhase != Act4ConfidencePhase.Configure)
            {
                return;
            }

            LilyWired = !LilyWired;
            CurrentMood = Act4GhostMood.Neutral;
            StatusLine = LilyWired
                ? "Attached: anyone in the bottom band gets handed to Lily."
                : "Detached: nobody can be handed over, however badly it is going.";
            NotifyStateChanged();
        }

        public void RunDay()
        {
            if (CurrentPhase != Act4ConfidencePhase.Configure)
            {
                return;
            }

            LastValidationResult = Act4ConfidenceValidator.Validate(CreateConfiguration(), queue);
            ScoresRevealed = true;
            playbackIndex = 0;
            CurrentPhase = Act4ConfidencePhase.Playback;
            HasFailedRun = false;
            ApplyCurrentVisitorReaction();
            NotifyStateChanged();
        }

        public void AdvancePlayback()
        {
            if (CurrentPhase != Act4ConfidencePhase.Playback || LastValidationResult == null)
            {
                return;
            }

            if (playbackIndex < LastValidationResult.VisitorResults.Count - 1)
            {
                playbackIndex++;
                ApplyCurrentVisitorReaction();
                NotifyStateChanged();
                return;
            }

            FinishDayRun();
        }

        private void FinishDayRun()
        {
            if (LastValidationResult.IsCorrect)
            {
                CurrentPhase = Act4ConfidencePhase.Complete;
                CurrentMood = Act4GhostMood.Happy;
                StatusLine = Act4ConfidenceDemoData.DescribePosture(LastValidationResult.Posture);
            }
            else
            {
                CurrentPhase = Act4ConfidencePhase.Configure;
                CurrentMood = HasSevereFailure(LastValidationResult) ? Act4GhostMood.Sad : Act4GhostMood.Confused;
                HasFailedRun = true;
                StatusLine = LastValidationResult.Errors.Count == 0
                    ? "The evening did not go the way it should have."
                    : LastValidationResult.Errors[0];

                AmbientBanterPanel.RequestHint(
                    GhostNarrativeState.Act4Id,
                    "after_incorrect_confidence_day",
                    "The player ran Act 4 with handles or band actions that hurt someone. Give a non-spoiler " +
                    "hint about who needs a person rather than a rephrase.");
            }

            GhostBackendClient.PostAttempt(
                GhostNarrativeState.Act4Id,
                GhostBackendClient.CreateAttemptResult(LastValidationResult.IsCorrect),
                GhostBackendClient.CreateAttemptDetails(
                    "act4-confidence-day",
                    LastValidationResult.Errors,
                    "lily=" + HandoffEdge + ", answer=" + AnswerEdge +
                    ", rephrase=" + RephraseWired + ", callLily=" + LilyWired + ", scoresRevealed=" + ScoresRevealed));

            NotifyStateChanged();
        }

        private Act4ZoneConfiguration CreateConfiguration()
        {
            return new Act4ZoneConfiguration(HandoffEdge, AnswerEdge, RephraseWired, LilyWired);
        }

        private void ApplyCurrentVisitorReaction()
        {
            var result = CurrentVisitorResult;
            if (result == null)
            {
                CurrentMood = Act4GhostMood.Sad;
                StatusLine = "Ghost cannot find the next visitor.";
                return;
            }

            StatusLine = result.Line;
            if (result.IsAccepted)
            {
                CurrentMood = Act4GhostMood.Happy;
                return;
            }

            CurrentMood = result.Outcome == Act4RouteOutcome.NoSafeRoute ||
                result.Outcome == Act4RouteOutcome.Meltdown
                    ? Act4GhostMood.Sad
                    : Act4GhostMood.Confused;
        }

        private static bool HasSevereFailure(Act4ConfidenceValidationResult result)
        {
            foreach (var visitorResult in result.VisitorResults)
            {
                if (visitorResult.Outcome == Act4RouteOutcome.NoSafeRoute ||
                    visitorResult.Outcome == Act4RouteOutcome.Meltdown)
                {
                    return true;
                }
            }

            return false;
        }

        public string BuildHintContext()
        {
            var summary = "Lily band below " + HandoffEdge + "; answer band from " + AnswerEdge +
                "; rephrase attached=" + RephraseWired + "; Lily attached=" + LilyWired + ". ";

            if (LastValidationResult == null)
            {
                return summary + "The evening has not been run.";
            }

            foreach (var result in LastValidationResult.VisitorResults)
            {
                if (!result.IsAccepted)
                {
                    return summary + "First problem: a visitor at " + result.Visitor.ConfidenceScore +
                        "% ended as " + result.Outcome + ".";
                }
            }

            return summary + "Everyone was routed acceptably.";
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }

        private void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }
    }

    public enum Act4ConfidencePhase
    {
        Onboarding,
        Configure,
        Playback,
        Complete
    }

    public enum Act4GhostMood
    {
        Neutral,
        Happy,
        Confused,
        Sad
    }
}
