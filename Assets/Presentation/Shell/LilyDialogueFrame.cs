using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Presentation.Shell
{
    public sealed class LilyDialogueFrame : MonoBehaviour
    {
        [SerializeField] private Text speakerNameText;
        [SerializeField] private Text dialogueText;

        public void Configure(Text speakerName, Text dialogue)
        {
            speakerNameText = speakerName;
            dialogueText = dialogue;
        }

        public void Show(ShellDialogueLine line)
        {
            if (speakerNameText != null)
            {
                speakerNameText.text = line.SpeakerName;
            }

            if (dialogueText != null)
            {
                dialogueText.text = line.Text;
            }
        }
    }
}
