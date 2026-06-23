using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Presentation.Shell
{
    public sealed class LilyDialogueFrame : MonoBehaviour
    {
        [SerializeField] private Text speakerNameText;
        [SerializeField] private Text dialogueText;
        [SerializeField] private Image speakerPortraitImage;
        [SerializeField] private Text portraitPlaceholderText;
        [SerializeField] private Sprite lilyPortrait;
        [SerializeField] private Sprite ghostPortrait;

        public void Configure(Text speakerName, Text dialogue)
        {
            Configure(speakerName, dialogue, null, null);
        }

        public void Configure(Text speakerName, Text dialogue, Image portraitImage, Text portraitPlaceholder)
        {
            speakerNameText = speakerName;
            dialogueText = dialogue;
            speakerPortraitImage = portraitImage;
            portraitPlaceholderText = portraitPlaceholder;
        }

        public void Show(ShellDialogueLine line)
        {
            if (speakerNameText != null)
            {
                speakerNameText.text = line.SpeakerName;
            }

            if (dialogueText != null)
            {
                dialogueText.text = FormatText(line.Text);
            }

            UpdatePortrait(line.SpeakerName);
        }

        private static string FormatText(string text)
        {
            return (text ?? string.Empty).Replace("{playerName}", GhostNarrativeState.PlayerName);
        }

        private void UpdatePortrait(string speakerName)
        {
            var portrait = GetPortraitForSpeaker(speakerName);

            if (speakerPortraitImage != null)
            {
                speakerPortraitImage.sprite = portrait;
                speakerPortraitImage.preserveAspect = true;
                speakerPortraitImage.color = portrait == null
                    ? new Color(1f, 0.96f, 0.88f)
                    : Color.white;
            }

            if (portraitPlaceholderText != null)
            {
                portraitPlaceholderText.text = string.IsNullOrWhiteSpace(speakerName)
                    ? "Speaker"
                    : speakerName;
                portraitPlaceholderText.enabled = portrait == null;
            }
        }

        private Sprite GetPortraitForSpeaker(string speakerName)
        {
            if (string.Equals(speakerName, ShellDialogueData.GhostSpeakerName, System.StringComparison.OrdinalIgnoreCase))
            {
                return ghostPortrait;
            }

            return lilyPortrait;
        }
    }
}
