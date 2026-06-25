using System.Collections.Generic;
using Ghost.Presentation.Backend;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Shell
{
    public sealed class GameShellPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject titleScreen;
        [SerializeField] private GameObject nameEntryScreen;
        [SerializeField] private GameObject actHubScreen;
        [SerializeField] private LilyDialogueFrame lilyDialogueFrame;
        [SerializeField] private Button startButton;
        [SerializeField] private InputField playerNameInput;
        [SerializeField] private Button confirmNameButton;
        [SerializeField] private Button act1Button;
        [SerializeField] private Button act2Button;
        [SerializeField] private Button act3Button;
        [SerializeField] private Button narrativeContinueButton;
        [SerializeField] private Button backToTitleButton;

        private readonly Queue<ShellDialogueLine> queuedNarrativeLines = new Queue<ShellDialogueLine>();
        private string pendingLaunchActId;

        public void Configure(
            GameObject title,
            GameObject hub,
            LilyDialogueFrame dialogueFrame,
            Button start,
            Button act1,
            Button act2,
            Button act3,
            Button back)
        {
            Configure(title, null, hub, dialogueFrame, start, null, null, act1, act2, act3, null, back);
        }

        public void Configure(
            GameObject title,
            GameObject nameEntry,
            GameObject hub,
            LilyDialogueFrame dialogueFrame,
            Button start,
            InputField nameInput,
            Button confirmName,
            Button act1,
            Button act2,
            Button act3,
            Button continueNarrative,
            Button back)
        {
            titleScreen = title;
            nameEntryScreen = nameEntry;
            actHubScreen = hub;
            lilyDialogueFrame = dialogueFrame;
            startButton = start;
            playerNameInput = nameInput;
            confirmNameButton = confirmName;
            act1Button = act1;
            act2Button = act2;
            act3Button = act3;
            narrativeContinueButton = continueNarrative;
            backToTitleButton = back;
        }

        private void Start()
        {
            BackendSync.EnsureStarted();

            WireButton(startButton, ShowNameEntryOrHub);
            WireButton(confirmNameButton, ConfirmPlayerNameAndShowHub);
            WireButton(act1Button, () => ShowActIntro(GhostNarrativeState.Act1Id));
            WireButton(act2Button, () => ShowActIntro(GhostNarrativeState.Act2Id));
            WireButton(act3Button, () => ShowActIntro(GhostNarrativeState.Act3Id));
            WireButton(narrativeContinueButton, ContinueNarrative);
            WireButton(backToTitleButton, ShowTitle);

            if (!string.IsNullOrWhiteSpace(GhostNarrativeState.PendingDebriefActId))
            {
                ShowActHub();
                PlayPendingDebrief();
                return;
            }

            ShowTitle();
        }

        public void ShowTitle()
        {
            ClearNarrativeFlow();
            SetScreenActive(titleScreen, true);
            SetScreenActive(nameEntryScreen, false);
            SetScreenActive(actHubScreen, false);
            ShowDialogue(ShellDialogueData.TitleScreenId);
        }

        public void ShowNameEntryOrHub()
        {
            if (GhostNarrativeState.HasPlayerName || nameEntryScreen == null)
            {
                ShowActHub();
                return;
            }

            ShowNameEntry();
        }

        public void ShowNameEntry()
        {
            ClearNarrativeFlow();
            SetScreenActive(titleScreen, false);
            SetScreenActive(nameEntryScreen, true);
            SetScreenActive(actHubScreen, false);
            ShowDialogue(ShellDialogueData.NameEntryScreenId);
        }

        public void ShowActHub()
        {
            ClearNarrativeFlow();
            SetScreenActive(titleScreen, false);
            SetScreenActive(nameEntryScreen, false);
            SetScreenActive(actHubScreen, true);
            ShowDialogue(ShellDialogueData.ActHubScreenId);
        }

        public void ConfirmPlayerNameAndShowHub()
        {
            GhostNarrativeState.SetPlayerName(playerNameInput == null ? null : playerNameInput.text);
            ShowActHub();
        }

        public void ShowActIntro(string actId)
        {
            ClearNarrativeFlow();
            pendingLaunchActId = actId;
            SetScreenActive(titleScreen, false);
            SetScreenActive(nameEntryScreen, false);
            SetScreenActive(actHubScreen, true);
            ShowLine(ShellDialogueData.GetBeat(actId, ShellDialogueData.IntroPhaseId));
            SetNarrativeContinueVisible("Continue to " + ShellDialogueData.GetActTitle(actId));
        }

        public void StartAct1()
        {
            SceneManager.LoadScene(ShellSceneNames.Act1SceneName);
        }

        public void StartAct2()
        {
            SceneManager.LoadScene(ShellSceneNames.Act2SceneName);
        }

        public void StartAct3()
        {
            SceneManager.LoadScene(ShellSceneNames.Act3SceneName);
        }

        private void ContinueNarrative()
        {
            if (!string.IsNullOrWhiteSpace(pendingLaunchActId))
            {
                var actId = pendingLaunchActId;
                pendingLaunchActId = null;
                LoadAct(actId);
                return;
            }

            ShowNextQueuedNarrativeLine();
        }

        private void PlayPendingDebrief()
        {
            var actId = GhostNarrativeState.ConsumePendingDebriefAct();
            if (string.IsNullOrWhiteSpace(actId))
            {
                return;
            }

            if (GhostNarrativeState.IsActCompleted(actId))
            {
                return;
            }

            queuedNarrativeLines.Clear();
            pendingLaunchActId = null;
            queuedNarrativeLines.Enqueue(ShellDialogueData.GetBeat(actId, ShellDialogueData.DebriefPhaseId));

            if (string.Equals(actId, GhostNarrativeState.Act3Id, System.StringComparison.Ordinal))
            {
                queuedNarrativeLines.Enqueue(ShellDialogueData.GetBeat(actId, ShellDialogueData.ClosingPhaseId));
            }

            GhostNarrativeState.MarkActCompleted(actId);
            ShowNextQueuedNarrativeLine();
        }

        private void ShowNextQueuedNarrativeLine()
        {
            if (queuedNarrativeLines.Count == 0)
            {
                HideNarrativeContinue();
                return;
            }

            ShowLine(queuedNarrativeLines.Dequeue());

            if (queuedNarrativeLines.Count > 0)
            {
                SetNarrativeContinueVisible("Continue");
                return;
            }

            HideNarrativeContinue();
        }

        private void LoadAct(string actId)
        {
            switch (actId)
            {
                case GhostNarrativeState.Act1Id:
                    StartAct1();
                    return;
                case GhostNarrativeState.Act2Id:
                    StartAct2();
                    return;
                case GhostNarrativeState.Act3Id:
                    StartAct3();
                    return;
            }
        }

        private void ClearNarrativeFlow()
        {
            queuedNarrativeLines.Clear();
            pendingLaunchActId = null;
            HideNarrativeContinue();
        }

        private void ShowDialogue(string screenId)
        {
            ShowLine(ShellDialogueData.GetLine(screenId));
        }

        private void ShowLine(ShellDialogueLine line)
        {
            if (lilyDialogueFrame == null)
            {
                return;
            }

            lilyDialogueFrame.Show(line);
        }

        private void SetNarrativeContinueVisible(string label)
        {
            if (narrativeContinueButton == null)
            {
                return;
            }

            SetButtonLabel(narrativeContinueButton, label);
            narrativeContinueButton.gameObject.SetActive(true);
        }

        private void HideNarrativeContinue()
        {
            if (narrativeContinueButton != null)
            {
                narrativeContinueButton.gameObject.SetActive(false);
            }
        }

        private static void SetButtonLabel(Button button, string label)
        {
            if (button == null)
            {
                return;
            }

            foreach (var text in button.GetComponentsInChildren<Text>(true))
            {
                text.text = label;
                return;
            }
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
