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
        [SerializeField] private InputField accountIdentifierInput;
        [SerializeField] private Button createAccountButton;
        [SerializeField] private Button useAccountButton;
        [SerializeField] private Text accountStatusText;
        [SerializeField] private Button chapter0Button;
        [SerializeField] private Button act1Button;
        [SerializeField] private Button act2Button;
        [SerializeField] private Button act3Button;
        [SerializeField] private Button act4Button;
        [SerializeField] private Button act5Button;
        [SerializeField] private Button act6Button;
        [SerializeField] private Button finalChapterButton;
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
            Configure(title, null, hub, dialogueFrame, start, null, null, null, null, null, null, act1, act2, act3, null, null, null, null, back);
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
            Button act4,
            Button act5,
            Button act6,
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
            accountIdentifierInput = accountInput;
            createAccountButton = createAccount;
            useAccountButton = useAccount;
            accountStatusText = accountStatus;
            act1Button = act1;
            act2Button = act2;
            act3Button = act3;
            act4Button = act4;
            act5Button = act5;
            act6Button = act6;
            narrativeContinueButton = continueNarrative;
            backToTitleButton = back;
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
            Button act4,
            Button act5,
            Button act6,
            Button continueNarrative,
            Button back,
            Button chapter0Entry,
            Button finalChapterEntry)
        {
            Configure(
                title,
                nameEntry,
                hub,
                dialogueFrame,
                start,
                nameInput,
                confirmName,
                accountInput,
                createAccount,
                useAccount,
                accountStatus,
                act1,
                act2,
                act3,
                act4,
                act5,
                act6,
                continueNarrative,
                back);
            chapter0Button = chapter0Entry;
            finalChapterButton = finalChapterEntry;
        }
        private void Start()
        {
            BackendSync.EnsureStarted();

            WireButton(startButton, ShowNameEntryOrHub);
            WireButton(confirmNameButton, ConfirmPlayerNameAndShowHub);
            WireButton(createAccountButton, CreateAccountAndShowHub);
            WireButton(useAccountButton, UseAccountAndShowHub);
            WireButton(chapter0Button, StartChapter0);
            WireButton(act1Button, () => ShowActIntro(GhostNarrativeState.Act1Id));
            WireButton(act2Button, () => ShowActIntro(GhostNarrativeState.Act2Id));
            WireButton(act3Button, () => ShowActIntro(GhostNarrativeState.Act3Id));
            WireButton(act4Button, () => ShowActIntro(GhostNarrativeState.Act4Id));
            WireButton(act5Button, () => ShowActIntro(GhostNarrativeState.Act5Id));
            WireButton(act6Button, () => ShowActIntro(GhostNarrativeState.Act6Id));
            WireButton(finalChapterButton, () => ShowActIntro(GhostNarrativeState.FinalChapterId));
            WireButton(narrativeContinueButton, ContinueNarrative);
            WireButton(backToTitleButton, ShowTitle);

            if (!string.IsNullOrWhiteSpace(GhostNarrativeState.PendingDebriefActId))
            {
                ShowActHub();
                PlayPendingDebrief();
                return;
            }

            if (GhostNarrativeState.ConsumeResumeAtHub())
            {
                ShowActHub();
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
            SetScreenActive(actHubScreen, false);
            PrepareAccountFields();
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
            ShowChapter0OrHub();
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
                ShowChapter0OrHub();
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
            SetScreenActive(actHubScreen, true);
            ShellDialogueLine introLine;
            if (string.Equals(actId, GhostNarrativeState.Act6Id, System.StringComparison.Ordinal))
            {
                introLine = ShellDialogueData.GetAct6Intro(AreEarlierActsComplete());
            }
            else if (string.Equals(actId, GhostNarrativeState.FinalChapterId, System.StringComparison.Ordinal))
            {
                introLine = ShellDialogueData.GetFinalChapterIntro(AreTeachingChaptersComplete());
            }
            else
            {
                introLine = ShellDialogueData.GetBeat(actId, ShellDialogueData.IntroPhaseId);
            }
            ShowLine(introLine);
            SetNarrativeContinueVisible("Continue to " + ShellDialogueData.GetActTitle(actId));
        }

        public void StartChapter0()
        {
            SceneManager.LoadScene(ShellSceneNames.Chapter0SceneName);
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

        public void StartAct4()
        {
            SceneManager.LoadScene(ShellSceneNames.Act4SceneName);
        }

        public void StartAct5()
        {
            SceneManager.LoadScene(ShellSceneNames.Act5SceneName);
        }

        public void StartAct6()
        {
            SceneManager.LoadScene(ShellSceneNames.Act6SceneName);
        }

        public void StartFinalChapter()
        {
            SceneManager.LoadScene(ShellSceneNames.FinalChapterSceneName);
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
                case GhostNarrativeState.Act4Id:
                    StartAct4();
                    return;
                case GhostNarrativeState.Act5Id:
                    StartAct5();
                    return;
                case GhostNarrativeState.Act6Id:
                    StartAct6();
                    return;
                case GhostNarrativeState.FinalChapterId:
                    StartFinalChapter();
                    return;
            }
        }

        private static bool AreEarlierActsComplete()
        {
            return GhostNarrativeState.IsActCompleted(GhostNarrativeState.Act1Id) &&
                GhostNarrativeState.IsActCompleted(GhostNarrativeState.Act2Id) &&
                GhostNarrativeState.IsActCompleted(GhostNarrativeState.Act3Id) &&
                GhostNarrativeState.IsActCompleted(GhostNarrativeState.Act4Id) &&
                GhostNarrativeState.IsActCompleted(GhostNarrativeState.Act5Id);
        }

        private static bool AreTeachingChaptersComplete()
        {
            return AreEarlierActsComplete() &&
                GhostNarrativeState.IsActCompleted(GhostNarrativeState.Act6Id);
        }

        private void ShowChapter0OrHub()
        {
            if (GhostNarrativeState.IsActCompleted(GhostNarrativeState.Chapter0Id))
            {
                ShowActHub();
                return;
            }

            StartChapter0();
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
                    ShowChapter0OrHub();
                    return;
                }

                GhostNarrativeState.ApplyBackendProgress(account.displayName, new string[0], true, true);
                SetAccountStatus("Account found, but progress could not be loaded. Starting from an empty local state.");
                ShowChapter0OrHub();
            });
        }

        private void OnDestroy()
        {
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
