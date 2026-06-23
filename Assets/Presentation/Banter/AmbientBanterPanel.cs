using System.Collections.Generic;
using Ghost.Presentation.Shell;
using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Presentation.Banter
{
    public sealed class AmbientBanterPanel : MonoBehaviour
    {
        private const float DefaultCycleSeconds = 6f;

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

        public void Initialize(IReadOnlyList<AmbientBanterBeat> sourceBeats)
        {
            beats = sourceBeats;
            currentIndex = 0;
            elapsedSeconds = 0f;

            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(ShowNextBeat);
                nextButton.onClick.AddListener(ShowNextBeat);
            }

            ShowCurrentBeat();
        }

        private void OnDestroy()
        {
            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(ShowNextBeat);
            }
        }

        private void Update()
        {
            if (beats == null || beats.Count <= 1)
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
            float secondsPerBeat)
        {
            speakerNameText = speakerName;
            dialogueText = dialogue;
            speakerPortraitImage = portraitImage;
            portraitPlaceholderText = portraitPlaceholder;
            nextButton = next;
            cycleSeconds = secondsPerBeat <= 0f ? DefaultCycleSeconds : secondsPerBeat;
        }
    }
}
