using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Shell
{
    public sealed class GameShellPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject titleScreen;
        [SerializeField] private GameObject actHubScreen;
        [SerializeField] private LilyDialogueFrame lilyDialogueFrame;
        [SerializeField] private Button startButton;
        [SerializeField] private Button act1Button;
        [SerializeField] private Button backToTitleButton;

        public void Configure(
            GameObject title,
            GameObject hub,
            LilyDialogueFrame dialogueFrame,
            Button start,
            Button act1,
            Button back)
        {
            titleScreen = title;
            actHubScreen = hub;
            lilyDialogueFrame = dialogueFrame;
            startButton = start;
            act1Button = act1;
            backToTitleButton = back;
        }

        private void Start()
        {
            WireButton(startButton, ShowActHub);
            WireButton(act1Button, StartAct1);
            WireButton(backToTitleButton, ShowTitle);
            ShowTitle();
        }

        public void ShowTitle()
        {
            SetScreenActive(titleScreen, true);
            SetScreenActive(actHubScreen, false);
            ShowDialogue(ShellDialogueData.TitleScreenId);
        }

        public void ShowActHub()
        {
            SetScreenActive(titleScreen, false);
            SetScreenActive(actHubScreen, true);
            ShowDialogue(ShellDialogueData.ActHubScreenId);
        }

        public void StartAct1()
        {
            SceneManager.LoadScene(ShellSceneNames.Act1SceneName);
        }

        private void ShowDialogue(string screenId)
        {
            if (lilyDialogueFrame == null)
            {
                return;
            }

            lilyDialogueFrame.Show(ShellDialogueData.GetLine(screenId));
        }

        private static void SetScreenActive(GameObject screen, bool isActive)
        {
            if (screen != null)
            {
                screen.SetActive(isActive);
            }
        }

        private static void WireButton(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }
    }
}
