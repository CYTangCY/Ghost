using System.Collections.Generic;
using Ghost.Presentation.Backend;
using Ghost.Presentation.Fundamentals;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Shell
{
    public sealed class GameShellPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject titleScreen;
        [SerializeField] private GameObject nameEntryScreen;
        [SerializeField] private GameObject fundamentalsScreen;
        [SerializeField] private GameObject actHubScreen;
        [SerializeField] private LilyDialogueFrame lilyDialogueFrame;
        [SerializeField] private ChatbotFundamentalsPresenter fundamentalsPresenter;
        [SerializeField] private Button startButton;
        [SerializeField] private InputField playerNameInput;
        [SerializeField] private Button confirmNameButton;
        [SerializeField] private InputField accountIdentifierInput;
        [SerializeField] private Button createAccountButton;
        [SerializeField] private Button useAccountButton;
        [SerializeField] private Text accountStatusText;
        [SerializeField] private Button fundamentalsButton;
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
            Configure(title, null, null, hub, dialogueFrame, null, start, null, null, null, null, null, null, null, act1, act2, act3, null, back);
        }

        public void Configure(
            GameObject title,
            GameObject nameEntry,
            GameObject hub,
            LilyDialogueFrame dialogueFrame,
            Button start,
            InputField nameInput,
            Button confirmName,
            InputField accountInput,
            Button createAccount,
            Button useAccount,
            Text accountStatus,
            Button act1,
            Button act2,
            Button act3,
            Button continueNarrative,
            Button back)
        {
            Configure(
                title,
                nameEntry,
                null,
                hub,
                dialogueFrame,
                null,
                start,
                nameInput,
                confirmName,
                accountInput,
                createAccount,
                useAccount,
                accountStatus,
                null,
                act1,
                act2,
                act3,
                continueNarrative,
                back);
        }

        public void Configure(
            GameObject title,
            GameObject nameEntry,
            GameObject fundamentals,
            GameObject hub,
            LilyDialogueFrame dialogueFrame,
            ChatbotFundamentalsPresenter fundamentalsSequence,
            Button start,
            InputField nameInput,
            Button confirmName,
            InputField accountInput,
            Button createAccount,
            Button useAccount,
            Text accountStatus,
            Button fundamentalsEntry,
            Button act1,
            Button act2,
            Button act3,
            Button continueNarrative,
            Button back)
        {
            titleScreen = title;
            nameEntryScreen = nameEntry;
            fundamentalsScreen = fundamentals;
            actHubScreen = hub;
            lilyDialogueFrame = dialogueFrame;
            fundamentalsPresenter = fundamentalsSequence;
            startButton = start;
            playerNameInput = nameInput;
            confirmNameButton = confirmName;
            accountIdentifierInput = accountInput;
            createAccountButton = createAccount;
            useAccountButton = useAccount;
            accountStatusText = accountStatus;
            fundamentalsButton = fundamentalsEntry;
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
            WireButton(createAccountButton, CreateAccountAndShowHub);
            WireButton(useAccountButton, UseAccountAndShowHub);
            WireButton(fundamentalsButton, ShowFundamentals);
            WireButton(act1Button, () => ShowActIntro(GhostNarrativeState.Act1Id));
            WireButton(act2Button, () => ShowActIntro(GhostNarrativeState.Act2Id));
            WireButton(act3Button, () => ShowActIntro(GhostNarrativeState.Act3Id));
            WireButton(narrativeContinueButton, ContinueNarrative);
            WireButton(backToTitleButton, ShowTitle);

            if (fundamentalsPresenter != null)
            {
                fundamentalsPresenter.Finished -= ShowActHub;
                fundamentalsPresenter.Finished += ShowActHub;
            }

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
            SetScreenActive(fundamentalsScreen, false);
            SetScreenActive(actHubScreen, false);
            ShowDialogue(ShellDialogueData.TitleScreenId);
        }

        public void ShowNameEntryOrHub()
        {
            if (nameEntryScreen == null)
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
            SetScreenActive(fundamentalsScreen, false);
            SetScreenActive(actHubScreen, false);
            PrepareAccountFields();
            ShowDialogue(ShellDialogueData.NameEntryScreenId);
        }

        public void ShowActHub()
        {
            ClearNarrativeFlow();
            SetScreenActive(titleScreen, false);
            SetScreenActive(nameEntryScreen, false);
            SetScreenActive(fundamentalsScreen, false);
            SetScreenActive(actHubScreen, true);
            ShowDialogue(ShellDialogueData.ActHubScreenId);
        }

        public void ShowFundamentals()
        {
            ClearNarrativeFlow();
            SetScreenActive(titleScreen, false);
            SetScreenActive(nameEntryScreen, false);
            SetScreenActive(actHubScreen, false);
            SetScreenActive(fundamentalsScreen, true);

            if (fundamentalsPresenter == null)
            {
                ShowActHub();
                return;
            }

            fundamentalsPresenter.Begin();
        }

        public void ConfirmPlayerNameAndShowHub()
        {
            GhostNarrativeState.SetPlayerName(playerNameInput == null ? null : playerNameInput.text);
            ShowActHub();
        }

        public void CreateAccountAndShowHub()
        {
            var userName = ReadAccountIdentifier();
            if (string.IsNullOrWhiteSpace(userName))
            {
                SetAccountStatus("Enter a username first. No password is used in this prototype.");
                return;
            }

            var displayName = ReadPlayerName();
            SetAccountButtonsInteractable(false);
            SetAccountStatus("Creating account...");

            GhostBackendClient.CreateAccount(userName, displayName, response =>
            {
                SetAccountButtonsInteractable(true);

                if (!response.Succeeded || response.Value == null)
                {
                    SetAccountStatus(response.StatusCode == 409
                        ? "That username already exists. Use Account, or choose a different username."
                        : "Account was not created. Check the backend, or use a 3-32 character username.");
                    return;
                }

                GhostNarrativeState.SetPlayerName(response.Value.displayName);
                BackendSync.PushProgress();
                SetAccountStatus("Account ready: " + response.Value.userName + " / " + response.Value.accountId);
                ShowActHub();
            });
        }

        public void UseAccountAndShowHub()
        {
            var identifier = ReadAccountIdentifier();
            if (string.IsNullOrWhiteSpace(identifier))
            {
                SetAccountStatus("Enter an account id or username first.");
                return;
            }

            SetAccountButtonsInteractable(false);
            SetAccountStatus("Looking up account...");

            GhostBackendClient.LookupAccount(identifier, response =>
            {
                if (!response.Succeeded || response.Value == null || string.IsNullOrWhiteSpace(response.Value.profileId))
                {
                    SetAccountButtonsInteractable(true);
                    SetAccountStatus("Account not found. Check the username/account id and backend server.");
                    return;
                }

                LoadAccountProgressAndShowHub(response.Value);
            });
        }

        public void ShowActIntro(string actId)
        {
            ClearNarrativeFlow();
            pendingLaunchActId = actId;
            SetScreenActive(titleScreen, false);
            SetScreenActive(nameEntryScreen, false);
            SetScreenActive(fundamentalsScreen, false);
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

        private void LoadAccountProgressAndShowHub(AccountResponse account)
        {
            GhostBackendClient.GetProgress(account.profileId, progressResponse =>
            {
                SetAccountButtonsInteractable(true);

                if (progressResponse.Succeeded && progressResponse.Value != null)
                {
                    var restoredName = progressResponse.Value.narrativeState == null ||
                        string.IsNullOrWhiteSpace(progressResponse.Value.narrativeState.playerName)
                            ? account.displayName
                            : progressResponse.Value.narrativeState.playerName;

                    GhostNarrativeState.ApplyBackendProgress(
                        restoredName,
                        progressResponse.Value.actsCompleted,
                        true,
                        true);
                    SetAccountStatus("Loaded account: " + account.userName);
                    ShowActHub();
                    return;
                }

                GhostNarrativeState.ApplyBackendProgress(account.displayName, new string[0], true, true);
                SetAccountStatus("Account found, but progress could not be loaded. Starting from an empty local state.");
                ShowActHub();
            });
        }

        private void OnDestroy()
        {
            if (fundamentalsPresenter != null)
            {
                fundamentalsPresenter.Finished -= ShowActHub;
            }
        }

        private void PrepareAccountFields()
        {
            if (playerNameInput != null && string.IsNullOrWhiteSpace(playerNameInput.text))
            {
                playerNameInput.text = GhostNarrativeState.PlayerName;
            }

            if (accountIdentifierInput != null && string.IsNullOrWhiteSpace(accountIdentifierInput.text))
            {
                accountIdentifierInput.text = string.IsNullOrWhiteSpace(GhostNarrativeState.BackendUserName)
                    ? GhostNarrativeState.BackendAccountId
                    : GhostNarrativeState.BackendUserName;
            }

            SetAccountStatus(string.IsNullOrWhiteSpace(GhostNarrativeState.BackendUserName)
                ? "Optional: create or use an account to recover progress on this backend."
                : "Current account: " + GhostNarrativeState.BackendUserName);
        }

        private string ReadPlayerName()
        {
            var value = playerNameInput == null ? string.Empty : playerNameInput.text;
            return string.IsNullOrWhiteSpace(value) ? GhostNarrativeState.DefaultPlayerName : value.Trim();
        }

        private string ReadAccountIdentifier()
        {
            return accountIdentifierInput == null ? string.Empty : (accountIdentifierInput.text ?? string.Empty).Trim();
        }

        private void SetAccountStatus(string message)
        {
            if (accountStatusText != null)
            {
                accountStatusText.text = message ?? string.Empty;
            }
        }

        private void SetAccountButtonsInteractable(bool interactable)
        {
            if (createAccountButton != null)
            {
                createAccountButton.interactable = interactable;
            }

            if (useAccountButton != null)
            {
                useAccountButton.interactable = interactable;
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
