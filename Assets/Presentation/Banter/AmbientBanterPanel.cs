using System.Collections.Generic;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Shell;
using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Presentation.Banter
{
    public sealed class AmbientBanterPanel : MonoBehaviour
    {
        private const float DefaultCycleSeconds = 6f;
        private const string AskLilyButtonLabel = "Ask Lily";
        private const string BackButtonLabel = "Back";
        private const string AskLilyTrigger = "ask_lily_button";

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
        private bool isShowingHint;
        private bool isRequestInFlight;
        private int hintRequestVersion;

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

            isShowingHint = false;
            isRequestInFlight = false;
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
            if (isShowingHint || isRequestInFlight || beats == null || beats.Count <= 1)
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

            ActivePanel.RequestHintInternal(requestedActId, trigger, stateSummary);
        }

        private void HandleActionButtonClicked()
        {
            if (isShowingHint || isRequestInFlight)
            {
                ResumeBanter();
                return;
            }

            RequestHintInternal(
                actId,
                AskLilyTrigger,
                "The player pressed Ask Lily for a non-spoiler hint.");
        }

        private void RequestHintInternal(string requestedActId, string trigger, string stateSummary)
        {
            var normalizedActId = string.IsNullOrWhiteSpace(requestedActId)
                ? actId
                : requestedActId;

            if (string.IsNullOrWhiteSpace(normalizedActId))
            {
                ShowHintText(BanterData.GetStaticHint(string.Empty));
                return;
            }

            var requestToken = ++hintRequestVersion;
            ShowAskingState();

            GhostBackendClient.PostHint(normalizedActId, trigger, stateSummary, response =>
            {
                if (requestToken != hintRequestVersion)
                {
                    return;
                }

                isRequestInFlight = false;
                if (response.Succeeded && response.Value != null && !string.IsNullOrWhiteSpace(response.Value.hint))
                {
                    ShowHintText(response.Value.hint);
                    return;
                }

                ShowHintText(BanterData.GetStaticHint(normalizedActId));
            });
        }

        private void ShowAskingState()
        {
            elapsedSeconds = 0f;
            isShowingHint = true;
            isRequestInFlight = true;
            SetButtonLabel(BackButtonLabel);
            SetText(speakerNameText, ShellDialogueData.LilySpeakerName);
            SetText(dialogueText, "Asking Lily...");
            UpdatePortrait(ShellDialogueData.LilySpeakerName);
        }

        private void ShowHintText(string hint)
        {
            elapsedSeconds = 0f;
            isShowingHint = true;
            SetButtonLabel(BackButtonLabel);
            SetText(speakerNameText, ShellDialogueData.LilySpeakerName);
            SetText(dialogueText, FormatText(hint));
            UpdatePortrait(ShellDialogueData.LilySpeakerName);
        }

        private void ResumeBanter()
        {
            hintRequestVersion++;
            isShowingHint = false;
            isRequestInFlight = false;
            SetButtonLabel(AskLilyButtonLabel);
            ShowCurrentBeat();
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
