using System;
using System.Collections.Generic;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Banter;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.VoicePipeline;

namespace Ghost.Presentation.Act6VoicePipeline
{
    /// <summary>
    /// Drives the capstone. Holds what the player has assembled for the visitor currently on screen,
    /// and hands it to <see cref="FinalChapterConversationValidator"/> when they run the test.
    ///
    /// Nothing here decides correctness - it records choices and asks the validator.
    /// </summary>
    public sealed class FinalChapterConversationController
    {
        private readonly IReadOnlyList<FinalChapterVisitor> visitors;
        private readonly bool[] completed;

        // Working state for the visitor on screen. Cleared on retry and when the next one arrives.
        private readonly Dictionary<string, string> slots = new Dictionary<string, string>();
        private readonly Dictionary<string, string> responseParts = new Dictionary<string, string>();
        private readonly List<FinalChapterLink> routeLinks = new List<FinalChapterLink>();

        // stepId -> where its card sits on the board, in 0..1 of the board's rect. Chapter 3's model:
        // the palette is what is available, the board is what the player has actually laid out.
        private readonly Dictionary<string, UnityEngine.Vector2> placedSteps =
            new Dictionary<string, UnityEngine.Vector2>();
        private readonly HashSet<string> backends = new HashSet<string>();
        private readonly HashSet<int> visitedStages = new HashSet<int>();

        private string intentId = string.Empty;
        private FinalChapterAction action = FinalChapterAction.None;

        public FinalChapterConversationController()
        {
            visitors = FinalChapterConversationData.CreateVisitors();
            completed = new bool[visitors.Count];
            CurrentPhase = FinalChapterPhase.Onboarding;
            CurrentMood = Act6GhostMood.Neutral;
            StatusLine = "Lily has one last thing to show you.";
            visitedStages.Add(0);
            SeedBoard();
        }

        public event Action StateChanged;

        public FinalChapterPhase CurrentPhase { get; private set; }

        public int ActiveVisitorIndex { get; private set; }

        public int ActiveStageIndex { get; private set; }

        public string StatusLine { get; private set; }

        public Act6GhostMood CurrentMood { get; private set; }

        public FinalChapterValidationResult LastResult { get; private set; }

        public int CurrentTraceIndex { get; private set; }

        public IReadOnlyList<FinalChapterVisitor> Visitors => visitors;

        public FinalChapterVisitor ActiveVisitor => visitors[ActiveVisitorIndex];

        public FinalChapterStage ActiveStage => ActiveVisitor.Stages[ActiveStageIndex];

        public bool IsLilyVisitor => ActiveVisitor.UsesLilyPortrait;

        public string SelectedIntentId => intentId;

        public FinalChapterAction SelectedAction => action;

        public IReadOnlyList<FinalChapterLink> RouteLinks => routeLinks;

        /// <summary>
        /// The cards the wires actually pass through, in the order they are reached. Derived rather
        /// than stored - the wires are the state, and this is only what they read as. Stops at the
        /// first branch or loop so the presenter never numbers a route that does not exist.
        /// </summary>
        public IReadOnlyList<string> RouteStepIds
        {
            get
            {
                var ordered = new List<string>();
                var seen = new HashSet<string> { FinalChapterConversationData.RouteStartId };
                var current = FinalChapterConversationData.RouteStartId;

                while (true)
                {
                    var next = GetLinkTarget(current);
                    if (string.IsNullOrEmpty(next) ||
                        string.Equals(next, FinalChapterConversationData.RouteEndId, StringComparison.Ordinal) ||
                        !seen.Add(next))
                    {
                        return ordered;
                    }

                    ordered.Add(next);
                    current = next;
                }
            }
        }

        public IReadOnlyCollection<string> SelectedBackendIds => backends;

        public int CompletedVisitorCount
        {
            get
            {
                var total = 0;
                foreach (var done in completed)
                {
                    if (done)
                    {
                        total++;
                    }
                }

                return total;
            }
        }

        public FinalChapterTraceStep ActiveTraceStep
        {
            get
            {
                if (LastResult == null || LastResult.TraceSteps.Count == 0)
                {
                    return null;
                }

                var index = Math.Max(0, Math.Min(CurrentTraceIndex, LastResult.TraceSteps.Count - 1));
                return LastResult.TraceSteps[index];
            }
        }

        public bool IsVisitorCompleted(int visitorIndex)
        {
            return visitorIndex >= 0 && visitorIndex < completed.Length && completed[visitorIndex];
        }

        public bool HasVisitedStage(int stageIndex)
        {
            return visitedStages.Contains(stageIndex);
        }

        /// <summary>
        /// True once the player has been through every stage this visitor asks for. Deliberately not
        /// "everything is filled in correctly" - a button that greys out until the board is right is a
        /// second validator, and a silent one. The run has to be allowed to fail.
        /// </summary>
        public bool CanRunTest
        {
            get
            {
                for (var i = 0; i < ActiveVisitor.Stages.Count; i++)
                {
                    if (!visitedStages.Contains(i))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public void FinishOnboarding()
        {
            if (CurrentPhase != FinalChapterPhase.Onboarding)
            {
                return;
            }

            CurrentPhase = FinalChapterPhase.Configure;
            StatusLine = "Visitor 1 of " + visitors.Count + ". " + PromptFor(ActiveStage);
            ShowLily("Same five ideas as before. This time nobody tells you which one to reach for.");
            NotifyStateChanged();
        }

        // ------------------------------------------------------------------ stage navigation

        public void GoToStage(int stageIndex)
        {
            if (CurrentPhase != FinalChapterPhase.Configure ||
                stageIndex < 0 ||
                stageIndex >= ActiveVisitor.Stages.Count)
            {
                return;
            }

            ActiveStageIndex = stageIndex;
            visitedStages.Add(stageIndex);
            StatusLine = PromptFor(ActiveStage);
            NotifyStateChanged();
        }

        public void NextStage()
        {
            GoToStage(Math.Min(ActiveStageIndex + 1, ActiveVisitor.Stages.Count - 1));
        }

        public void PreviousStage()
        {
            GoToStage(Math.Max(ActiveStageIndex - 1, 0));
        }

        // ------------------------------------------------------------------ intent

        public void SelectIntent(string optionId)
        {
            if (CurrentPhase != FinalChapterPhase.Configure || string.IsNullOrWhiteSpace(optionId))
            {
                return;
            }

            intentId = optionId.Trim();
            NotifyStateChanged();
        }

        // ------------------------------------------------------------------ entities

        public string GetSlotAssignment(string slotId)
        {
            return slots.TryGetValue(slotId ?? string.Empty, out var fragmentId) ? fragmentId : string.Empty;
        }

        public bool IsFragmentUsed(string fragmentId)
        {
            foreach (var pair in slots)
            {
                if (string.Equals(pair.Value, fragmentId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// One fragment fills one slot - Chapter 2's rule. Dropping a fragment that is already
        /// somewhere else moves it rather than silently cloning it.
        /// </summary>
        public bool AssignFragment(string fragmentId, string slotId)
        {
            if (CurrentPhase != FinalChapterPhase.Configure ||
                string.IsNullOrWhiteSpace(fragmentId) ||
                string.IsNullOrWhiteSpace(slotId))
            {
                return false;
            }

            var previous = FindKeyHolding(slots, fragmentId);
            if (!string.IsNullOrEmpty(previous))
            {
                slots.Remove(previous);
            }

            slots[slotId.Trim()] = fragmentId.Trim();
            NotifyStateChanged();
            return true;
        }

        public bool ClearSlot(string slotId)
        {
            if (CurrentPhase != FinalChapterPhase.Configure || !slots.Remove(slotId ?? string.Empty))
            {
                return false;
            }

            NotifyStateChanged();
            return true;
        }

        // ------------------------------------------------------------------ dialogue route

        /// <summary>The cards on the board, including the two fixed ends.</summary>
        public IReadOnlyDictionary<string, UnityEngine.Vector2> PlacedSteps => placedSteps;

        public bool IsStepPlaced(string stepId)
        {
            return placedSteps.ContainsKey(stepId ?? string.Empty);
        }

        public UnityEngine.Vector2 GetStepPosition(string stepId)
        {
            return placedSteps.TryGetValue(stepId ?? string.Empty, out var position)
                ? position
                : new UnityEngine.Vector2(0.5f, 0.5f);
        }

        /// <summary>Drops a card from the palette onto the board. One card per step.</summary>
        public bool PlaceRouteStep(string stepId, UnityEngine.Vector2 position)
        {
            if (CurrentPhase != FinalChapterPhase.Configure || string.IsNullOrWhiteSpace(stepId))
            {
                return false;
            }

            var id = stepId.Trim();
            if (placedSteps.ContainsKey(id))
            {
                return false;
            }

            placedSteps[id] = Clamp(position);
            NotifyStateChanged();
            return true;
        }

        /// <summary>
        /// Slides a card while the pointer is still down. Deliberately silent: raising StateChanged
        /// here would rebuild the whole page under the player's finger and drop the drag.
        /// </summary>
        public bool MoveRouteStep(string stepId, UnityEngine.Vector2 position)
        {
            if (CurrentPhase != FinalChapterPhase.Configure ||
                !placedSteps.ContainsKey(stepId ?? string.Empty))
            {
                return false;
            }

            placedSteps[stepId] = Clamp(position);
            return true;
        }

        /// <summary>
        /// Takes a card off the board, and any wire touching it with it - a wire to a card that is no
        /// longer there is not a route, it is a leftover.
        /// </summary>
        public bool RemoveRouteStep(string stepId)
        {
            var id = stepId ?? string.Empty;
            if (CurrentPhase != FinalChapterPhase.Configure || IsFixedEnd(id) || !placedSteps.Remove(id))
            {
                return false;
            }

            for (var i = routeLinks.Count - 1; i >= 0; i--)
            {
                if (string.Equals(routeLinks[i].FromId, id, StringComparison.Ordinal) ||
                    string.Equals(routeLinks[i].ToId, id, StringComparison.Ordinal))
                {
                    routeLinks.RemoveAt(i);
                }
            }

            NotifyStateChanged();
            return true;
        }

        public static bool IsFixedEnd(string stepId)
        {
            return string.Equals(stepId, FinalChapterConversationData.RouteStartId, StringComparison.Ordinal) ||
                string.Equals(stepId, FinalChapterConversationData.RouteEndId, StringComparison.Ordinal);
        }

        private static UnityEngine.Vector2 Clamp(UnityEngine.Vector2 position)
        {
            // Keep a card's centre well inside the map, so it can never be dropped half off the edge
            // or somewhere it cannot be picked up again.
            return new UnityEngine.Vector2(
                UnityEngine.Mathf.Clamp(position.x, 0.15f, 0.85f),
                UnityEngine.Mathf.Clamp(position.y, 0.15f, 0.85f));
        }

        private void SeedBoard()
        {
            placedSteps.Clear();
            placedSteps[FinalChapterConversationData.RouteStartId] = new UnityEngine.Vector2(0.20f, 0.82f);
            placedSteps[FinalChapterConversationData.RouteEndId] = new UnityEngine.Vector2(0.78f, 0.16f);
        }

        /// <summary>Where the wire leaving this card lands, or empty if it has none.</summary>
        public string GetLinkTarget(string fromId)
        {
            foreach (var link in routeLinks)
            {
                if (string.Equals(link.FromId, fromId, StringComparison.Ordinal))
                {
                    return link.ToId;
                }
            }

            return string.Empty;
        }

        public bool HasLink(string fromId, string toId)
        {
            return string.Equals(GetLinkTarget(fromId), toId, StringComparison.Ordinal) &&
                !string.IsNullOrEmpty(toId);
        }

        /// <summary>
        /// Draws a wire. One leaves each card, so re-dragging from a card that already has one moves
        /// it rather than forking the conversation - the same reversibility every other stage has.
        /// Wiring is accepted even when it is wrong; the validator is what says no.
        /// </summary>
        public bool LinkRouteSteps(string fromId, string toId)
        {
            if (CurrentPhase != FinalChapterPhase.Configure ||
                string.IsNullOrWhiteSpace(fromId) ||
                string.IsNullOrWhiteSpace(toId))
            {
                return false;
            }

            var from = fromId.Trim();
            var to = toId.Trim();

            // The two ends of the conversation are fixed: nothing arrives before the message, and
            // nothing leaves after the reply. And a wire needs a card at each end to hang from.
            if (string.Equals(from, to, StringComparison.Ordinal) ||
                string.Equals(to, FinalChapterConversationData.RouteStartId, StringComparison.Ordinal) ||
                string.Equals(from, FinalChapterConversationData.RouteEndId, StringComparison.Ordinal) ||
                !placedSteps.ContainsKey(from) ||
                !placedSteps.ContainsKey(to))
            {
                return false;
            }

            RemoveLinksFrom(from);
            routeLinks.Add(new FinalChapterLink(from, to));
            NotifyStateChanged();
            return true;
        }

        public bool RemoveRouteLink(string fromId)
        {
            if (CurrentPhase != FinalChapterPhase.Configure || !RemoveLinksFrom(fromId ?? string.Empty))
            {
                return false;
            }

            NotifyStateChanged();
            return true;
        }

        /// <summary>Sweeps the board back to the two fixed ends - every card and every wire.</summary>
        public void ClearRoute()
        {
            if (routeLinks.Count == 0 && placedSteps.Count <= 2)
            {
                return;
            }

            routeLinks.Clear();
            SeedBoard();
            NotifyStateChanged();
        }

        private bool RemoveLinksFrom(string fromId)
        {
            var removed = false;
            for (var i = routeLinks.Count - 1; i >= 0; i--)
            {
                if (string.Equals(routeLinks[i].FromId, fromId, StringComparison.Ordinal))
                {
                    routeLinks.RemoveAt(i);
                    removed = true;
                }
            }

            return removed;
        }

        // ------------------------------------------------------------------ confidence

        public void ChooseAction(FinalChapterAction chosen)
        {
            if (CurrentPhase != FinalChapterPhase.Configure)
            {
                return;
            }

            action = chosen;
            ShowLily("Whatever you pick, something is worse for it. That is the whole job.");
            NotifyStateChanged();
        }

        public string GetActionCost(FinalChapterAction candidate)
        {
            switch (candidate)
            {
                case FinalChapterAction.AnswerNow:
                    return ActiveVisitor.AnswerNowCost;
                case FinalChapterAction.AskAgain:
                    return ActiveVisitor.AskAgainCost;
                case FinalChapterAction.HandOver:
                    return ActiveVisitor.HandOverCost;
                default:
                    return string.Empty;
            }
        }

        // ------------------------------------------------------------------ backend

        public bool ToggleBackend(string optionId)
        {
            if (CurrentPhase != FinalChapterPhase.Configure || string.IsNullOrWhiteSpace(optionId))
            {
                return false;
            }

            var id = optionId.Trim();
            if (!backends.Remove(id))
            {
                backends.Add(id);
            }

            NotifyStateChanged();
            return true;
        }

        public bool IsBackendSelected(string optionId)
        {
            return backends.Contains(optionId ?? string.Empty);
        }

        // ------------------------------------------------------------------ response

        public string GetResponsePart(string roleId)
        {
            return responseParts.TryGetValue(roleId ?? string.Empty, out var partId) ? partId : string.Empty;
        }

        public bool IsResponsePartUsed(string partId)
        {
            return !string.IsNullOrEmpty(FindKeyHolding(responseParts, partId));
        }

        /// <summary>
        /// Placement has to be reversible - the same requirement as Chapter 6. A part dropped on a role
        /// it does not belong to is accepted here; the validator is what says no.
        /// </summary>
        public bool PlaceResponsePart(string partId, string roleId)
        {
            if (CurrentPhase != FinalChapterPhase.Configure ||
                string.IsNullOrWhiteSpace(partId) ||
                string.IsNullOrWhiteSpace(roleId))
            {
                return false;
            }

            var previous = FindKeyHolding(responseParts, partId);
            if (!string.IsNullOrEmpty(previous))
            {
                responseParts.Remove(previous);
            }

            responseParts[roleId.Trim()] = partId.Trim();
            NotifyStateChanged();
            return true;
        }

        public bool ClearResponseRole(string roleId)
        {
            if (CurrentPhase != FinalChapterPhase.Configure ||
                !responseParts.Remove(roleId ?? string.Empty))
            {
                return false;
            }

            NotifyStateChanged();
            return true;
        }

        // ------------------------------------------------------------------ run and playback

        public FinalChapterConfiguration BuildConfiguration()
        {
            return new FinalChapterConfiguration(
                intentId,
                slots,
                routeLinks,
                action,
                backends,
                responseParts);
        }

        public void RunTest()
        {
            if (CurrentPhase != FinalChapterPhase.Configure || !CanRunTest)
            {
                return;
            }

            LastResult = FinalChapterConversationValidator.Validate(ActiveVisitor, BuildConfiguration());
            PostAttempt();
            CurrentTraceIndex = 0;
            CurrentPhase = FinalChapterPhase.Playback;
            CurrentMood = LastResult.Passed ? Act6GhostMood.Happy : Act6GhostMood.Confused;
            StatusLine = LastResult.Passed
                ? "Every step held. Walk through what Ghost did."
                : "It stops somewhere. Step through it and find where.";

            if (!LastResult.Passed)
            {
                // A wrong answer highlights Ask Lily rather than opening it over the puzzle.
                AmbientBanterPanel.RequestHint(
                    GhostNarrativeState.FinalChapterId,
                    LastResult.Errors.Count > 0 ? LastResult.Errors[0] : StatusLine,
                    BuildHintContext());
            }

            NotifyStateChanged();
        }

        public void AdvancePlayback()
        {
            if (CurrentPhase != FinalChapterPhase.Playback || LastResult == null)
            {
                return;
            }

            if (CurrentTraceIndex < LastResult.TraceSteps.Count - 1)
            {
                CurrentTraceIndex++;
                NotifyStateChanged();
                return;
            }

            if (LastResult.Passed)
            {
                FinishCurrentVisitor();
                return;
            }

            // Drop the player back on the exact stage that broke, with everything else intact.
            CurrentPhase = FinalChapterPhase.Configure;
            ActiveStageIndex = IndexOfStage(LastResult.FirstBrokenStage ?? ActiveVisitor.Stages[0]);
            CurrentMood = Act6GhostMood.Neutral;
            StatusLine = "Fix that one step - the rest of the route was fine.";
            NotifyStateChanged();
        }

        public void RetryCurrentVisitor()
        {
            ResetCurrentVisitor();
        }

        public void ResetCurrentVisitor()
        {
            ClearWorkingState();
            LastResult = null;
            CurrentTraceIndex = 0;
            CurrentPhase = FinalChapterPhase.Configure;
            CurrentMood = Act6GhostMood.Neutral;
            StatusLine = PromptFor(ActiveStage);
            ShowLily("Only this conversation resets. The ones you already finished stay finished.");
            NotifyStateChanged();
        }

        public void BeginEnding()
        {
            if (CurrentPhase != FinalChapterPhase.ReadyForEnding)
            {
                return;
            }

            CurrentPhase = FinalChapterPhase.Ending;
            NotifyStateChanged();
        }

        public void StartEndingForTesting()
        {
            for (var i = 0; i < completed.Length; i++)
            {
                completed[i] = true;
            }

            CurrentPhase = FinalChapterPhase.ReadyForEnding;
            NotifyStateChanged();
        }

        public string BuildHintContext()
        {
            switch (CurrentPhase)
            {
                case FinalChapterPhase.Onboarding:
                    return "final_onboarding";
                case FinalChapterPhase.Playback:
                    return LastResult != null && LastResult.Passed
                        ? "final_playback_pass"
                        : "final_playback_fail";
                case FinalChapterPhase.ReadyForEnding:
                case FinalChapterPhase.Ending:
                    return "final_ending";
                default:
                    return "final_" + ActiveVisitor.Id + "_" + ActiveStage.ToString().ToLowerInvariant();
            }
        }

        private void FinishCurrentVisitor()
        {
            completed[ActiveVisitorIndex] = true;
            CurrentMood = Act6GhostMood.Happy;

            if (ActiveVisitorIndex >= visitors.Count - 1)
            {
                CurrentPhase = FinalChapterPhase.ReadyForEnding;
                StatusLine = "That was the last one.";
                ShowLily("A-all three... even mine. Um. When you are ready, let Ghost speak.");
                NotifyStateChanged();
                return;
            }

            ActiveVisitorIndex++;
            ClearWorkingState();
            LastResult = null;
            CurrentTraceIndex = 0;
            CurrentPhase = FinalChapterPhase.Configure;
            StatusLine = "Visitor " + (ActiveVisitorIndex + 1) + " of " + visitors.Count + ". " +
                PromptFor(ActiveStage);
            ShowLily(OpeningLineFor(ActiveVisitor.Id));
            NotifyStateChanged();
        }

        private void ClearWorkingState()
        {
            slots.Clear();
            responseParts.Clear();
            routeLinks.Clear();
            SeedBoard();
            backends.Clear();
            visitedStages.Clear();
            intentId = string.Empty;
            action = FinalChapterAction.None;
            ActiveStageIndex = 0;
            visitedStages.Add(0);
        }

        private int IndexOfStage(FinalChapterStage stage)
        {
            for (var i = 0; i < ActiveVisitor.Stages.Count; i++)
            {
                if (ActiveVisitor.Stages[i] == stage)
                {
                    return i;
                }
            }

            return 0;
        }

        private void PostAttempt()
        {
            if (!UnityEngine.Application.isPlaying || LastResult == null)
            {
                return;
            }

            GhostBackendClient.PostAttempt(
                GhostNarrativeState.FinalChapterId,
                GhostBackendClient.CreateAttemptResult(LastResult.Passed),
                GhostBackendClient.CreateAttemptDetails(
                    "final-visitor-" + ActiveVisitor.Id,
                    LastResult.Errors,
                    "visitor_index=" + (ActiveVisitorIndex + 1) +
                    "; stages=" + ActiveVisitor.Stages.Count));
        }

        private static string FindKeyHolding(Dictionary<string, string> map, string value)
        {
            foreach (var pair in map)
            {
                if (string.Equals(pair.Value, value, StringComparison.Ordinal))
                {
                    return pair.Key;
                }
            }

            return string.Empty;
        }

        private static string OpeningLineFor(string visitorId)
        {
            switch (visitorId)
            {
                case FinalChapterConversationData.VendingVisitorId:
                    return "This one names two of the same thing. Watch what that does to the route.";
                case FinalChapterConversationData.LilyVisitorId:
                    return "Oh. Um. That one is... me. Sorry. Just treat me like the others.";
                default:
                    return "A new conversation. Nothing carries over except what you learned.";
            }
        }

        private static string PromptFor(FinalChapterStage stage)
        {
            switch (stage)
            {
                case FinalChapterStage.Intent:
                    return "What is this visitor actually here for?";
                case FinalChapterStage.Entities:
                    return "Drag in the details the reply depends on. If they never said, use the card " +
                        "that says so.";
                case FinalChapterStage.Dialogue:
                    return "Wire their message through to Ghost's reply - through the steps this " +
                        "visitor needs, and nothing they do not.";
                case FinalChapterStage.Confidence:
                    return "Every option costs something. Pick the one you can defend.";
                case FinalChapterStage.Backend:
                    return "Attach only what this answer genuinely depends on.";
                case FinalChapterStage.Response:
                    return "Three responsibilities, three slots.";
                default:
                    return string.Empty;
            }
        }

        private static void ShowLily(string line)
        {
            AmbientBanterPanel.ShowReaction(GhostNarrativeState.FinalChapterId, line);
        }

        private void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }
    }

    public enum FinalChapterPhase
    {
        Onboarding,
        Configure,
        Playback,
        ReadyForEnding,
        Ending
    }
}
