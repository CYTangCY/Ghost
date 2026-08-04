using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.VoicePipeline
{
    /// <summary>
    /// The stages a visitor can demand. Not every visitor demands all of them - that is the
    /// escalation: visitor 1 only needs to be understood, Lily needs the whole machine.
    /// </summary>
    public enum FinalChapterStage
    {
        Intent,
        Entities,
        Dialogue,
        Confidence,
        Backend,
        Response
    }

    /// <summary>
    /// Chapter 4's three-way call. Deliberately not a threshold slider: a slider invites the player
    /// to read the number off the screen and solve for it, which is the mistake Chapter 4 was
    /// rebuilt twice to remove.
    /// </summary>
    public enum FinalChapterAction
    {
        None,
        AnswerNow,
        AskAgain,
        HandOver
    }

    /// <summary>A pickable label. Used for intent, backend sources and response parts.</summary>
    public sealed class FinalChapterOption
    {
        public FinalChapterOption(
            string id,
            string label,
            string description,
            string failureReply,
            bool isDecoy = false)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Option id is required.", nameof(id));
            }

            Id = id.Trim();
            Label = label ?? string.Empty;
            Description = description ?? string.Empty;
            FailureReply = failureReply ?? string.Empty;
            IsDecoy = isDecoy;
        }

        public string Id { get; }

        public string Label { get; }

        public string Description { get; }

        /// <summary>What Ghost says if the player picks this. Every wrong option fails differently.</summary>
        public string FailureReply { get; }

        public bool IsDecoy { get; }
    }

    /// <summary>
    /// One draggable piece of the visitor's sentence. Chapter 2's rule holds: one fragment fills one
    /// slot, so a required value can never be a phrase the player has no way to assemble.
    /// </summary>
    public sealed class FinalChapterFragment
    {
        public FinalChapterFragment(
            string id,
            string text,
            bool isDecoy = false,
            string whyWrong = null,
            bool isUnknownMarker = false)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Fragment id is required.", nameof(id));
            }

            Id = id.Trim();
            Text = text ?? string.Empty;
            IsDecoy = isDecoy;
            WhyWrong = whyWrong ?? string.Empty;
            IsUnknownMarker = isUnknownMarker;
        }

        public string Id { get; }

        public string Text { get; }

        public bool IsDecoy { get; }

        public string WhyWrong { get; }

        /// <summary>
        /// Not a piece of the message - the card that records that the message never said. A slot the
        /// visitor left open is answered by dropping this in, rather than by leaving the slot blank:
        /// an empty slot reads as unfinished work, and the player deserves a way to say that she
        /// never told us - out loud, on the board, as an answer.
        /// </summary>
        public bool IsUnknownMarker { get; }
    }

    /// <summary>
    /// One wire the player has drawn between two route cards. Chapter 3's mechanic: a conversation is
    /// nodes joined by transitions, not a list typed in order.
    /// </summary>
    public sealed class FinalChapterLink
    {
        public FinalChapterLink(string fromId, string toId)
        {
            FromId = (fromId ?? string.Empty).Trim();
            ToId = (toId ?? string.Empty).Trim();
        }

        public string FromId { get; }

        public string ToId { get; }
    }

    /// <summary>A named hole the player drops one fragment into.</summary>
    public sealed class FinalChapterSlot
    {
        public FinalChapterSlot(string id, string label, string expectedFragmentId, string whyItMatters = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Slot id is required.", nameof(id));
            }

            Id = id.Trim();
            Label = label ?? string.Empty;
            ExpectedFragmentId = (expectedFragmentId ?? string.Empty).Trim();
            WhyItMatters = whyItMatters ?? string.Empty;
        }

        public string Id { get; }

        public string Label { get; }

        public string ExpectedFragmentId { get; }

        public string WhyItMatters { get; }
    }

    /// <summary>One node in the little dialogue route the player lays out for this visitor.</summary>
    public sealed class FinalChapterRouteStep
    {
        public FinalChapterRouteStep(string id, string label, string description)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Route step id is required.", nameof(id));
            }

            Id = id.Trim();
            Label = label ?? string.Empty;
            Description = description ?? string.Empty;
        }

        public string Id { get; }

        public string Label { get; }

        public string Description { get; }
    }

    /// <summary>
    /// A pipeline part for the response stage, tagged with the role it legitimately fills. Chapter 6's
    /// three responsibilities: where the fact comes from, what is asked for, how it is said.
    /// </summary>
    public sealed class FinalChapterResponsePart
    {
        public FinalChapterResponsePart(string id, string roleId, string label, string whyWrong = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Response part id is required.", nameof(id));
            }

            Id = id.Trim();
            RoleId = (roleId ?? string.Empty).Trim();
            Label = label ?? string.Empty;
            WhyWrong = whyWrong ?? string.Empty;
        }

        public string Id { get; }

        public string RoleId { get; }

        public string Label { get; }

        public string WhyWrong { get; }
    }

    /// <summary>
    /// Everything one visitor asks of the player. Built with object initialisers in
    /// <see cref="FinalChapterConversationData"/> - a twenty-argument constructor was unreadable.
    /// </summary>
    public sealed class FinalChapterVisitor
    {
        public string Id { get; set; } = string.Empty;

        public string SpeakerName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string ExpectedReply { get; set; } = string.Empty;

        /// <summary>Fallback when nothing more specific applies.</summary>
        public string FailureReply { get; set; } = string.Empty;

        public bool UsesLilyPortrait { get; set; }

        public IReadOnlyList<FinalChapterStage> Stages { get; set; } = new FinalChapterStage[0];

        // Intent
        public IReadOnlyList<string> IntentOptionIds { get; set; } = new string[0];

        public string ExpectedIntentId { get; set; } = string.Empty;

        // Entities
        public IReadOnlyList<FinalChapterFragment> Fragments { get; set; } = new FinalChapterFragment[0];

        public IReadOnlyList<FinalChapterSlot> Slots { get; set; } = new FinalChapterSlot[0];

        // Dialogue
        public IReadOnlyList<string> RouteStepPalette { get; set; } = new string[0];

        public IReadOnlyList<string> ExpectedRouteStepIds { get; set; } = new string[0];

        // Confidence
        public int ConfidencePercent { get; set; }

        /// <summary>
        /// True when the message genuinely leaves a required value undecided. This - not the
        /// percentage - is what makes asking again the right call.
        /// </summary>
        public bool RequestIsAmbiguous { get; set; }

        public string AnswerNowCost { get; set; } = string.Empty;

        public string AskAgainCost { get; set; } = string.Empty;

        public string HandOverCost { get; set; } = string.Empty;

        // Backend
        public IReadOnlyList<string> BackendOptionIds { get; set; } = new string[0];

        public IReadOnlyList<string> RequiredBackendIds { get; set; } = new string[0];

        // Response
        public IReadOnlyList<string> ResponsePartIds { get; set; } = new string[0];

        /// <summary>roleId -> the one part that belongs there.</summary>
        public IReadOnlyDictionary<string, string> ExpectedResponseParts { get; set; } =
            new Dictionary<string, string>();

        public bool Requires(FinalChapterStage stage)
        {
            foreach (var candidate in Stages)
            {
                if (candidate == stage)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Chapter 4's actual lesson: the defensible call follows from what the player built, not from
        /// the confidence number sitting next to it.
        /// </summary>
        public FinalChapterAction ExpectedAction
        {
            get { return RequestIsAmbiguous ? FinalChapterAction.AskAgain : FinalChapterAction.AnswerNow; }
        }
    }

    /// <summary>What the player has assembled for the visitor currently on screen.</summary>
    public sealed class FinalChapterConfiguration
    {
        private readonly Dictionary<string, string> slotAssignments;
        private readonly List<FinalChapterLink> routeLinks;
        private readonly HashSet<string> backendIds;
        private readonly Dictionary<string, string> responseParts;

        public FinalChapterConfiguration(
            string intentOptionId = null,
            IDictionary<string, string> slots = null,
            IEnumerable<FinalChapterLink> route = null,
            FinalChapterAction action = FinalChapterAction.None,
            IEnumerable<string> backends = null,
            IDictionary<string, string> response = null)
        {
            IntentOptionId = (intentOptionId ?? string.Empty).Trim();
            slotAssignments = slots == null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(slots);
            routeLinks = route == null ? new List<FinalChapterLink>() : new List<FinalChapterLink>(route);
            Action = action;
            backendIds = backends == null ? new HashSet<string>() : new HashSet<string>(backends);
            responseParts = response == null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(response);
        }

        public string IntentOptionId { get; }

        public IReadOnlyDictionary<string, string> SlotAssignments => slotAssignments;

        public IReadOnlyList<FinalChapterLink> RouteLinks => routeLinks;

        public FinalChapterAction Action { get; }

        public IReadOnlyCollection<string> BackendIds => backendIds;

        public IReadOnlyDictionary<string, string> ResponseParts => responseParts;
    }

    public sealed class FinalChapterTraceStep
    {
        public FinalChapterTraceStep(string stageId, string title, string line, bool succeeded)
        {
            StageId = stageId ?? string.Empty;
            Title = title ?? string.Empty;
            Line = line ?? string.Empty;
            Succeeded = succeeded;
        }

        public string StageId { get; }

        public string Title { get; }

        public string Line { get; }

        public bool Succeeded { get; }
    }

    public sealed class FinalChapterValidationResult
    {
        private readonly FinalChapterTraceStep[] traceSteps;
        private readonly string[] errors;

        public FinalChapterValidationResult(
            FinalChapterVisitor visitor,
            string actualReply,
            FinalChapterStage? firstBrokenStage,
            IEnumerable<FinalChapterTraceStep> trace,
            IEnumerable<string> validationErrors)
        {
            Visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
            ActualReply = actualReply ?? string.Empty;
            FirstBrokenStage = firstBrokenStage;
            traceSteps = trace == null
                ? new FinalChapterTraceStep[0]
                : new List<FinalChapterTraceStep>(trace).ToArray();
            errors = validationErrors == null
                ? new string[0]
                : new List<string>(validationErrors).ToArray();
            Passed = errors.Length == 0 &&
                !firstBrokenStage.HasValue &&
                string.Equals(ActualReply, Visitor.ExpectedReply, StringComparison.Ordinal);
        }

        public FinalChapterVisitor Visitor { get; }

        public string ActualReply { get; }

        public FinalChapterStage? FirstBrokenStage { get; }

        public IReadOnlyList<FinalChapterTraceStep> TraceSteps => traceSteps;

        public IReadOnlyList<string> Errors => errors;

        public bool Passed { get; }
    }
}
