using System;
using System.Collections.Generic;
using Ghost.Presentation.Banter;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.BackendResponse;

namespace Ghost.Presentation.Act6BackendResponse
{
    public sealed class Act6BackendInteractionController
    {
        private readonly Dictionary<string, string> placements =
            new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly IReadOnlyList<Act6BackendCard> palette;
        private readonly IReadOnlyList<Act6BackendPlaybackStep> playbackSteps;

        public Act6BackendInteractionController()
        {
            foreach (var roleId in Act6BackendResponseData.CreateRoleOrder())
            {
                placements[roleId] = string.Empty;
            }

            palette = Act6BackendResponseData.CreatePalette();
            playbackSteps = Act6BackendResponseData.CreatePlaybackSteps();
            CurrentPhase = Act6BackendPhase.Onboarding;
            CurrentMood = Act6BackendMood.Confused;
            SelectedCardId = string.Empty;
            PlaybackIndex = -1;
            StatusLine =
                "Ghost has the right route, but it still needs a data source, an action, and a response.";
        }

        public event Action StateChanged;

        public IReadOnlyList<Act6BackendCard> Palette => palette;

        public IReadOnlyList<Act6BackendPlaybackStep> PlaybackSteps => playbackSteps;

        public Act6BackendPhase CurrentPhase { get; private set; }

        public Act6BackendMood CurrentMood { get; private set; }

        public Act6BackendValidationResult LastValidation { get; private set; }

        public string SelectedCardId { get; private set; }

        public int PlaybackIndex { get; private set; }

        public string StatusLine { get; private set; }

        public Act6BackendPlaybackStep ActivePlaybackStep =>
            PlaybackIndex >= 0 && PlaybackIndex < playbackSteps.Count
                ? playbackSteps[PlaybackIndex]
                : null;

        public void BeginAfterOnboarding()
        {
            if (CurrentPhase != Act6BackendPhase.Onboarding)
            {
                return;
            }

            CurrentPhase = Act6BackendPhase.Configure;
            CurrentMood = Act6BackendMood.Neutral;
            StatusLine =
                "Fill DATA SOURCE, ACTION, and RESPONSE. Then run the tested lab-hours route.";
            Notify();
        }

        public void ReplayOnboarding()
        {
            if (CurrentPhase != Act6BackendPhase.Configure)
            {
                return;
            }

            CurrentPhase = Act6BackendPhase.Onboarding;
            SelectedCardId = string.Empty;
            StatusLine =
                "Ghost has the right route, but it still needs a data source, an action, and a response.";
            Notify();
        }

        public string GetPlacedCardId(string roleId)
        {
            return roleId != null && placements.TryGetValue(roleId, out var cardId)
                ? cardId
                : string.Empty;
        }

        public bool IsCardPlaced(string cardId)
        {
            foreach (var placement in placements)
            {
                if (string.Equals(placement.Value, cardId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public void SelectCard(string cardId)
        {
            if (CurrentPhase != Act6BackendPhase.Configure || string.IsNullOrWhiteSpace(cardId))
            {
                return;
            }

            SelectedCardId = string.Equals(SelectedCardId, cardId, StringComparison.Ordinal)
                ? string.Empty
                : cardId;
            StatusLine = string.IsNullOrWhiteSpace(SelectedCardId)
                ? "Selection cleared. Drag a card, or select one and then select a role socket."
                : Act6BackendResponseData.GetCard(SelectedCardId).Label +
                    " selected. Now choose DATA SOURCE, ACTION, or RESPONSE.";
            Notify();
        }

        public bool PlaceSelectedOnRole(string roleId)
        {
            return PlaceCardOnRole(SelectedCardId, roleId);
        }

        public bool ReturnRoleCardToPalette(string roleId)
        {
            if (CurrentPhase != Act6BackendPhase.Configure ||
                !Act6BackendResponseData.IsKnownRole(roleId))
            {
                return false;
            }

            var cardId = GetPlacedCardId(roleId);
            if (string.IsNullOrWhiteSpace(cardId))
            {
                return false;
            }

            placements[roleId] = string.Empty;
            LastValidation = null;
            PlaybackIndex = -1;
            SelectedCardId = string.Empty;
            CurrentMood = Act6BackendMood.Neutral;
            StatusLine = Act6BackendResponseData.GetCard(cardId).Label +
                " returned to the palette.";
            Notify();
            return true;
        }

        public bool PlaceCardOnRole(string cardId, string roleId)
        {
            if (CurrentPhase != Act6BackendPhase.Configure ||
                !Act6BackendResponseData.IsKnownRole(roleId) ||
                string.IsNullOrWhiteSpace(cardId))
            {
                return false;
            }

            Act6BackendCard card;
            try
            {
                card = Act6BackendResponseData.GetCard(cardId);
            }
            catch (ArgumentException)
            {
                return false;
            }

            var oldRoleId = FindRoleForCard(cardId);
            if (string.Equals(oldRoleId, roleId, StringComparison.Ordinal))
            {
                SelectedCardId = string.Empty;
                Notify();
                return true;
            }

            var displacedCardId = placements[roleId];
            placements[roleId] = cardId;
            if (!string.IsNullOrWhiteSpace(oldRoleId))
            {
                placements[oldRoleId] = displacedCardId;
            }

            LastValidation = null;
            PlaybackIndex = -1;
            SelectedCardId = string.Empty;
            CurrentMood = Act6BackendMood.Neutral;
            StatusLine = card.Label + " placed in " +
                Act6BackendResponseData.GetRoleLabel(roleId) + ".";
            Notify();
            return true;
        }

        public void ResetBoard()
        {
            if (CurrentPhase != Act6BackendPhase.Configure)
            {
                return;
            }

            foreach (var roleId in Act6BackendResponseData.CreateRoleOrder())
            {
                placements[roleId] = string.Empty;
            }

            LastValidation = null;
            PlaybackIndex = -1;
            SelectedCardId = string.Empty;
            CurrentMood = Act6BackendMood.Neutral;
            StatusLine = "Board cleared. Follow the fact from storage to action to complete reply.";
            Notify();
        }

        public void RunRoute()
        {
            if (CurrentPhase != Act6BackendPhase.Configure)
            {
                return;
            }

            LastValidation = Act6BackendResponseValidator.Validate(
                GetPlacedCardId(Act6BackendResponseData.DataSourceRoleId),
                GetPlacedCardId(Act6BackendResponseData.ActionRoleId),
                GetPlacedCardId(Act6BackendResponseData.ResponseRoleId));

            GhostBackendClient.PostAttempt(
                GhostNarrativeState.Act6Id,
                GhostBackendClient.CreateAttemptResult(LastValidation.IsCorrect),
                GhostBackendClient.CreateAttemptDetails(
                    "act6-backend-response",
                    LastValidation.Errors,
                    "firstBrokenRole=" + LastValidation.FirstBrokenRoleId));

            if (!LastValidation.IsCorrect)
            {
                CurrentMood = Act6BackendMood.Confused;
                StatusLine = LastValidation.Errors[0] +
                    " Repair the first broken responsibility and run the full route again.";
                AmbientBanterPanel.RequestHint(
                    GhostNarrativeState.Act6Id,
                    "after_failed_backend_response",
                    "The player connected the backend action and response-generation lesson incorrectly. Give a non-spoiler hint that separates where the fact is stored, the action that requests it, and the sentence that presents it.");
                Notify();
                return;
            }

            CurrentPhase = Act6BackendPhase.Playback;
            CurrentMood = Act6BackendMood.Neutral;
            PlaybackIndex = 0;
            SelectedCardId = string.Empty;
            StatusLine = playbackSteps[0].Line;
            Notify();
        }

        public void AdvancePlayback()
        {
            if (CurrentPhase != Act6BackendPhase.Playback)
            {
                return;
            }

            if (PlaybackIndex < playbackSteps.Count - 1)
            {
                PlaybackIndex++;
                CurrentMood = PlaybackIndex == playbackSteps.Count - 1
                    ? Act6BackendMood.Happy
                    : Act6BackendMood.Neutral;
                StatusLine = playbackSteps[PlaybackIndex].Line;
                Notify();
                return;
            }

            CurrentPhase = Act6BackendPhase.Complete;
            CurrentMood = Act6BackendMood.Happy;
            StatusLine = "Ghost: \"" + Act6BackendResponseData.FinalReply + "\"";
            Notify();
        }

        public string BuildHintContext()
        {
            var summary = "Data source=" +
                GetPlacedCardLabel(Act6BackendResponseData.DataSourceRoleId) +
                "; action=" +
                GetPlacedCardLabel(Act6BackendResponseData.ActionRoleId) +
                "; response=" +
                GetPlacedCardLabel(Act6BackendResponseData.ResponseRoleId) + ".";
            return LastValidation == null
                ? summary + " The route has not been run."
                : summary + " First broken role=" +
                    (string.IsNullOrWhiteSpace(LastValidation.FirstBrokenRoleId)
                        ? "none"
                        : LastValidation.FirstBrokenRoleId) + ".";
        }

        private string GetPlacedCardLabel(string roleId)
        {
            var cardId = GetPlacedCardId(roleId);
            return string.IsNullOrWhiteSpace(cardId)
                ? "empty"
                : Act6BackendResponseData.GetCard(cardId).Label;
        }

        private string FindRoleForCard(string cardId)
        {
            foreach (var placement in placements)
            {
                if (string.Equals(placement.Value, cardId, StringComparison.Ordinal))
                {
                    return placement.Key;
                }
            }

            return string.Empty;
        }

        private void Notify()
        {
            StateChanged?.Invoke();
        }
    }

    public enum Act6BackendPhase
    {
        Onboarding,
        Configure,
        Playback,
        Complete
    }

    public enum Act6BackendMood
    {
        Neutral,
        Confused,
        Happy
    }
}
