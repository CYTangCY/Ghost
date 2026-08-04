using Ghost.Presentation.Common;
using System.Collections.Generic;
using Ghost.Presentation.Characters;
using Ghost.Presentation.GhostAvatar;
using Ghost.Presentation.Shell;
using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Presentation.Banter
{
    public sealed class AmbientBanterPanel : MonoBehaviour
    {
        private static readonly Dictionary<string, string> CurrentStateByAct =
            new Dictionary<string, string>();

        // A hint that a wrong answer queued up, waiting for the player to press Ask Lily.
        private static readonly Dictionary<string, string> PendingHintTriggerByAct =
            new Dictionary<string, string>();
        private const float DefaultCycleSeconds = 6f;
        private const float GuidanceIdleSeconds = 25f;
        private const string AskLilyButtonLabel = "Ask Lily";
        private const string AskLilyOpeningLine = "Um... what should we think through for Ghost?";
        private const string IncorrectValidateOpeningLine = "I-I think Ghost missed something there; ask me what you want to check.";

        [SerializeField] private Text speakerNameText;
        [SerializeField] private Text dialogueText;
        [SerializeField] private Image speakerPortraitImage;
        [SerializeField] private Text portraitPlaceholderText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Sprite lilyPortrait;
        [SerializeField] private Sprite ghostPortrait;
        [SerializeField] private float cycleSeconds = DefaultCycleSeconds;

        private IReadOnlyList<AmbientBanterBeat> beats;
        private int currentIndex;
        private float elapsedSeconds;
        private string actId;
        private Text nextButtonLabel;
        private bool isPausedForChat;
        private bool isWaitingForIdleAfterGuidance;
        private bool isHintSuggested;
        private Color defaultActionButtonColor = Color.white;

        public static AmbientBanterPanel ActivePanel { get; private set; }

        public static bool IsHintSuggested =>
            ActivePanel != null && ActivePanel.isHintSuggested;

        public void Initialize(IReadOnlyList<AmbientBanterBeat> sourceBeats)
        {
            beats = sourceBeats;
            currentIndex = 0;
            elapsedSeconds = 0f;

            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(HandleActionButtonClicked);
                nextButton.onClick.AddListener(HandleActionButtonClicked);
            }

            isPausedForChat = false;
            isWaitingForIdleAfterGuidance = false;
            ClearHintSuggestion();
            SetButtonLabel(AskLilyButtonLabel);
            ActivePanel = this;
            ShowCurrentBeat();
        }

        private void OnDestroy()
        {
            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(HandleActionButtonClicked);
            }

            if (ActivePanel == this)
            {
                ActivePanel = null;
            }
        }

        private void Update()
        {
            if (isPausedForChat || beats == null || beats.Count <= 1)
            {
                return;
            }

            elapsedSeconds += Time.deltaTime;
            var waitSeconds = isWaitingForIdleAfterGuidance
                ? GuidanceIdleSeconds
                : cycleSeconds;
            if (elapsedSeconds >= waitSeconds)
            {
                isWaitingForIdleAfterGuidance = false;
                ShowNextBeat();
            }
        }

        private void ShowNextBeat()
        {
            if (beats == null || beats.Count == 0)
            {
                return;
            }

            currentIndex = (currentIndex + 1) % beats.Count;
            ShowCurrentBeat();
        }

        /// <summary>
        /// Called when a chapter's answer comes back wrong. This used to shove Lily's window open in
        /// the player's face; now it only lights up the Ask Lily button and holds the hint until they
        /// choose to ask. Opening stays player-initiated, via HandleActionButtonClicked.
        /// </summary>
        public static void RequestHint(string requestedActId, string trigger, string stateSummary)
        {
            SetCurrentState(requestedActId, stateSummary);
            PendingHintTriggerByAct[requestedActId ?? string.Empty] = trigger ?? string.Empty;
            SuggestHint(requestedActId);
        }

        public static void SuggestHint(string requestedActId)
        {
            if (ActivePanel == null ||
                !string.Equals(
                    ActivePanel.actId,
                    requestedActId,
                    System.StringComparison.Ordinal))
            {
                return;
            }

            ActivePanel.ShowHintSuggestion();
        }

        public static void SetCurrentState(string requestedActId, string stateSummary)
        {
            if (string.IsNullOrWhiteSpace(requestedActId))
            {
                return;
            }

            CurrentStateByAct[requestedActId] = stateSummary ?? string.Empty;
        }

        public static void ShowReaction(string requestedActId, string line)
        {
            if (ActivePanel == null ||
                !string.Equals(ActivePanel.actId, requestedActId, System.StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            ActivePanel.ShowLilyReaction(line);
        }

        public static void RecordPlayerActivity(string requestedActId)
        {
            if (ActivePanel == null ||
                !string.Equals(
                    ActivePanel.actId,
                    requestedActId,
                    System.StringComparison.Ordinal))
            {
                return;
            }

            ActivePanel.elapsedSeconds = 0f;
        }

        private void HandleActionButtonClicked()
        {
            // If a wrong answer left a hint waiting, honour that trigger and opening line so the player
            // gets the specific help - just at the moment they asked for it.
            var key = actId ?? string.Empty;
            var hasPendingHint = PendingHintTriggerByAct.TryGetValue(key, out var pendingTrigger);
            if (hasPendingHint)
            {
                PendingHintTriggerByAct.Remove(key);
            }

            ClearHintSuggestion();
            OpenChatWindow(
                actId,
                hasPendingHint ? IncorrectValidateOpeningLine : AskLilyOpeningLine,
                GetCurrentState(actId),
                hasPendingHint ? pendingTrigger : "manual_ask_lily");
        }

        public void PauseForChat()
        {
            isPausedForChat = true;
        }

        public void ResumeAfterChat()
        {
            isPausedForChat = false;
            elapsedSeconds = 0f;
            isWaitingForIdleAfterGuidance = true;
        }

        private void OpenChatWindow(
            string requestedActId,
            string openingLine,
            string stateSummary,
            string requestedTrigger)
        {
            var normalizedActId = string.IsNullOrWhiteSpace(requestedActId)
                ? actId
                : requestedActId;
            LilyChatWindow.Open(
                normalizedActId,
                openingLine,
                stateSummary,
                requestedTrigger);
        }

        private static string GetCurrentState(string requestedActId)
        {
            return !string.IsNullOrWhiteSpace(requestedActId) &&
                CurrentStateByAct.TryGetValue(requestedActId, out var state)
                    ? state
                    : string.Empty;
        }

        private void ShowLilyReaction(string line)
        {
            elapsedSeconds = 0f;
            isWaitingForIdleAfterGuidance = true;
            SetText(speakerNameText, ShellDialogueData.LilySpeakerName);
            SetText(dialogueText, FormatText(line));
            UpdatePortrait(ShellDialogueData.LilySpeakerName);
        }

        private void ShowCurrentBeat()
        {
            if (beats == null || beats.Count == 0)
            {
                SetText(speakerNameText, string.Empty);
                SetText(dialogueText, string.Empty);
                return;
            }

            elapsedSeconds = 0f;
            isWaitingForIdleAfterGuidance = false;

            var beat = beats[currentIndex];
            SetText(speakerNameText, beat.Speaker);
            SetText(dialogueText, FormatText(beat.Text));
            UpdatePortrait(beat.Speaker);
        }

        private static string FormatText(string text)
        {
            return (text ?? string.Empty).Replace("{playerName}", GhostNarrativeState.PlayerName);
        }

        private static void SetText(Text target, string value)
        {
            if (target != null)
            {
                target.text = value ?? string.Empty;
            }
        }

        private void UpdatePortrait(string speaker)
        {
            var portrait = GetPortraitForSpeaker(speaker);

            if (speakerPortraitImage != null)
            {
                speakerPortraitImage.sprite = portrait;
                speakerPortraitImage.preserveAspect = true;
                speakerPortraitImage.color = portrait == null
                    ? new Color(1f, 0.96f, 0.88f, 0.95f)
                    : Color.white;
            }

            if (portraitPlaceholderText != null)
            {
                portraitPlaceholderText.text = string.IsNullOrWhiteSpace(speaker)
                    ? "Speaker"
                    : speaker;
                portraitPlaceholderText.enabled = portrait == null;
            }
        }

        private Sprite GetPortraitForSpeaker(string speaker)
        {
            if (string.Equals(speaker, ShellDialogueData.GhostSpeakerName, System.StringComparison.OrdinalIgnoreCase))
            {
                return ghostPortrait != null
                    ? ghostPortrait
                    : GhostPixelSpriteFactory.GetSprite(GhostMood.Neutral);
            }

            return lilyPortrait != null ? lilyPortrait : LilyPixelPortraitFactory.GetPortrait();
        }

        public void Configure(
            Text speakerName,
            Text dialogue,
            Image portraitImage,
            Text portraitPlaceholder,
            Button next,
            float secondsPerBeat,
            string configuredActId)
        {
            speakerNameText = speakerName;
            dialogueText = dialogue;
            speakerPortraitImage = portraitImage;
            portraitPlaceholderText = portraitPlaceholder;
            nextButton = next;
            nextButtonLabel = next == null ? null : next.GetComponentInChildren<Text>();
            cycleSeconds = secondsPerBeat <= 0f ? DefaultCycleSeconds : secondsPerBeat;
            actId = configuredActId ?? string.Empty;

            GhostUITheme.Label(
                speakerNameText,
                speakerNameText == null ? string.Empty : speakerNameText.text,
                GhostUITheme.HeadingSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);
            GhostUITheme.Label(
                dialogueText,
                dialogueText == null ? string.Empty : dialogueText.text,
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink);
            GhostUITheme.Label(
                portraitPlaceholderText,
                portraitPlaceholderText == null ? string.Empty : portraitPlaceholderText.text,
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                GhostUITheme.InkSoft);

            if (nextButton != null && nextButton.targetGraphic != null)
            {
                defaultActionButtonColor = nextButton.targetGraphic.color;
                nextButton = GhostUITheme.PushButton(
                    nextButton.gameObject,
                    AskLilyButtonLabel,
                    defaultActionButtonColor,
                    GhostUITheme.Ink);
                nextButtonLabel = nextButton.GetComponentInChildren<Text>();
            }
        }

        private void ShowHintSuggestion()
        {
            isHintSuggested = true;
            SetButtonLabel("Ask Lily - hint ready");
            if (nextButton != null && nextButton.targetGraphic != null)
            {
                nextButton.targetGraphic.color = new Color(1f, 0.82f, 0.38f);
            }
        }

        private void ClearHintSuggestion()
        {
            isHintSuggested = false;
            SetButtonLabel(AskLilyButtonLabel);
            if (nextButton != null && nextButton.targetGraphic != null)
            {
                nextButton.targetGraphic.color = defaultActionButtonColor;
            }
        }

        private void SetButtonLabel(string value)
        {
            SetText(nextButtonLabel, value);
        }
    }
}
