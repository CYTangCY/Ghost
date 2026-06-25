using System.Collections.Generic;
using Ghost.Presentation.Shell;
using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Presentation.Banter
{
    public sealed class AmbientBanterPanel : MonoBehaviour
    {
        private const float DefaultCycleSeconds = 6f;
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

        public static AmbientBanterPanel ActivePanel { get; private set; }

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
            if (elapsedSeconds >= cycleSeconds)
            {
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

        public static void RequestHint(string requestedActId, string trigger, string stateSummary)
        {
            if (ActivePanel == null)
            {
                return;
            }

            ActivePanel.OpenChatWindow(requestedActId, IncorrectValidateOpeningLine);
        }

        private void HandleActionButtonClicked()
        {
            OpenChatWindow(actId, AskLilyOpeningLine);
        }

        public void PauseForChat()
        {
            isPausedForChat = true;
        }

        public void ResumeAfterChat()
        {
            isPausedForChat = false;
            elapsedSeconds = 0f;
        }

        private void OpenChatWindow(string requestedActId, string openingLine)
        {
            var normalizedActId = string.IsNullOrWhiteSpace(requestedActId)
                ? actId
                : requestedActId;
            LilyChatWindow.Open(normalizedActId, openingLine);
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
                return ghostPortrait;
            }

            return lilyPortrait;
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
        }

        private void SetButtonLabel(string value)
        {
            SetText(nextButtonLabel, value);
        }
    }
}
