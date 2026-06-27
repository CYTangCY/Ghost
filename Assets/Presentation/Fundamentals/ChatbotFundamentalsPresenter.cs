using System;
using System.Collections.Generic;
using Ghost.Presentation.Shell;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Ghost.Presentation.Fundamentals
{
    public sealed class ChatbotFundamentalsPresenter : MonoBehaviour
    {
        [SerializeField] private Text progressText;
        [SerializeField] private Text titleText;
        [SerializeField] private Text ghostProblemText;
        [SerializeField] private Text lilyExplanationText;
        [SerializeField] private Text actionPromptText;
        [SerializeField] private Text consequenceText;
        [SerializeField] private Text ghostStatusText;
        [SerializeField] private Text componentChainText;
        [SerializeField] private Text backendLinkText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private Transform actionButtonRoot;
        [SerializeField] private Transform componentButtonRoot;
        [SerializeField] private Transform challengeButtonRoot;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private LilyDialogueFrame dialogueFrame;

        private readonly HashSet<string> completedSingleActions = new HashSet<string>();
        private readonly HashSet<string> compareTogglesSeen = new HashSet<string>();
        private readonly List<string> selectedComponentIds = new List<string>();
        private readonly HashSet<string> challengeModesSeen = new HashSet<string>();

        private IReadOnlyList<ChatbotFundamentalsBeat> beats;
        private int currentBeatIndex;
        private bool currentBeatComplete;
        private bool componentPathCorrect;
        private bool backendSideLinked;

        public event Action Finished;

        public void Configure(
            Text progress,
            Text title,
            Text ghostProblem,
            Text lilyExplanation,
            Text actionPrompt,
            Text consequence,
            Text ghostStatus,
            Text componentChain,
            Text backendLink,
            Text feedback,
            Transform actionButtons,
            Transform componentButtons,
            Transform challengeButtons,
            Button previous,
            Button next,
            Button skip,
            LilyDialogueFrame dialogue)
        {
            progressText = progress;
            titleText = title;
            ghostProblemText = ghostProblem;
            lilyExplanationText = lilyExplanation;
            actionPromptText = actionPrompt;
            consequenceText = consequence;
            ghostStatusText = ghostStatus;
            componentChainText = componentChain;
            backendLinkText = backendLink;
            feedbackText = feedback;
            actionButtonRoot = actionButtons;
            componentButtonRoot = componentButtons;
            challengeButtonRoot = challengeButtons;
            previousButton = previous;
            nextButton = next;
            skipButton = skip;
            dialogueFrame = dialogue;
        }

        public void SetDialogueFrame(LilyDialogueFrame frame)
        {
            dialogueFrame = frame;
        }

        public void Begin()
        {
            EnsureEventSystem();
            LoadBeatsIfNeeded();
            WireNavigationButtons();
            currentBeatIndex = 0;
            RenderCurrentBeat();
        }

        private void LoadBeatsIfNeeded()
        {
            if (beats == null)
            {
                beats = ChatbotFundamentalsData.CreateBeats();
            }
        }

        private void WireNavigationButtons()
        {
            WireButton(previousButton, ShowPreviousBeat);
            WireButton(nextButton, ShowNextBeat);
            WireButton(skipButton, FinishSequence);
        }

        private void ShowPreviousBeat()
        {
            if (currentBeatIndex <= 0)
            {
                return;
            }

            currentBeatIndex--;
            RenderCurrentBeat();
        }

        private void ShowNextBeat()
        {
            if (!currentBeatComplete)
            {
                SetFeedback("Try the small action first so Ghost can react.", false);
                return;
            }

            if (beats == null || currentBeatIndex >= beats.Count - 1)
            {
                FinishSequence();
                return;
            }

            currentBeatIndex++;
            RenderCurrentBeat();
        }

        private void FinishSequence()
        {
            Finished?.Invoke();
        }

        private void RenderCurrentBeat()
        {
            LoadBeatsIfNeeded();

            if (beats == null || beats.Count == 0)
            {
                return;
            }

            ResetInteractionState();
            ClearDynamicButtons();

            var beat = beats[currentBeatIndex];
            SetText(progressText, "Fundamentals " + (currentBeatIndex + 1) + " / " + beats.Count);
            SetText(titleText, beat.Title);
            SetText(ghostProblemText, beat.GhostProblem);
            SetText(lilyExplanationText, beat.LilyExplanation);
            SetText(actionPromptText, beat.ActionPrompt);
            SetText(consequenceText, "Ghost is waiting for your action.");
            SetText(ghostStatusText, beat.GhostProblem);
            SetText(componentChainText, string.Empty);
            SetText(backendLinkText, string.Empty);
            SetTextActive(componentChainText, false);
            SetTextActive(backendLinkText, false);
            SetFeedback("Small action required.", true);
            ShowLilyExplanation(beat.LilyExplanation);

            SetRootActive(actionButtonRoot, false);
            SetRootActive(componentButtonRoot, false);
            SetRootActive(challengeButtonRoot, false);

            switch (beat.InteractionKind)
            {
                case ChatbotFundamentalsInteractionKind.SingleAction:
                    RenderSingleActionBeat(beat);
                    break;
                case ChatbotFundamentalsInteractionKind.ToggleCompare:
                    RenderToggleCompareBeat(beat);
                    break;
                case ChatbotFundamentalsInteractionKind.ComponentOrder:
                    RenderComponentOrderBeat(beat);
                    break;
                case ChatbotFundamentalsInteractionKind.ChallengeModes:
                    RenderChallengeModesBeat(beat);
                    break;
            }

            SetCurrentBeatComplete(false);
            UpdateNavigationButtons();
        }

        private void ResetInteractionState()
        {
            completedSingleActions.Clear();
            compareTogglesSeen.Clear();
            selectedComponentIds.Clear();
            challengeModesSeen.Clear();
            componentPathCorrect = false;
            backendSideLinked = false;
            currentBeatComplete = false;
        }

        private void RenderSingleActionBeat(ChatbotFundamentalsBeat beat)
        {
            SetRootActive(actionButtonRoot, true);

            foreach (var label in beat.ActionLabels)
            {
                var capturedLabel = label;
                var button = CreateDynamicButton(actionButtonRoot, capturedLabel, new Color(0.84f, 0.92f, 1f));
                button.onClick.AddListener(() => HandleSingleAction(beat, capturedLabel));
            }
        }

        private void HandleSingleAction(ChatbotFundamentalsBeat beat, string actionLabel)
        {
            completedSingleActions.Add(actionLabel);

            if (beat.ActionLabels.Count > 1 && completedSingleActions.Count < beat.ActionLabels.Count)
            {
                SetText(
                    consequenceText,
                    actionLabel + " is lit. Ghost still needs the other support before this idea is complete.");
                SetText(ghostStatusText, "One support is glowing, but Ghost's understanding is still lopsided.");
                SetFeedback("Light the other support too.", true);
                return;
            }

            SetText(consequenceText, beat.Consequence);
            SetText(ghostStatusText, "Ghost's reply becomes clearer.");
            SetFeedback("Consequence shown. You can continue.", true);
            SetCurrentBeatComplete(true);
        }

        private void RenderToggleCompareBeat(ChatbotFundamentalsBeat beat)
        {
            SetRootActive(actionButtonRoot, true);

            foreach (var label in beat.ActionLabels)
            {
                var capturedLabel = label;
                var color = capturedLabel.Contains("rule")
                    ? new Color(0.92f, 0.97f, 1f)
                    : new Color(1f, 0.95f, 0.82f);
                var button = CreateDynamicButton(actionButtonRoot, capturedLabel, color);
                button.onClick.AddListener(() => HandleCompareToggle(beat, capturedLabel));
            }
        }

        private void HandleCompareToggle(ChatbotFundamentalsBeat beat, string label)
        {
            compareTogglesSeen.Add(label);

            if (label.Contains("rule"))
            {
                SetText(
                    consequenceText,
                    "Rule path: Ghost follows exact deterministic checks. The same state gives the same result.");
                SetText(ghostStatusText, "Ghost follows a stable rule path.");
            }
            else
            {
                SetText(
                    consequenceText,
                    "AI-style support: Lily can phrase flexible help, but the validator still decides correctness.");
                SetText(ghostStatusText, "Lily's help sounds flexible, while scoring stays deterministic.");
            }

            if (compareTogglesSeen.Count >= beat.ActionLabels.Count)
            {
                SetText(consequenceText, beat.Consequence);
                SetFeedback("Both sides compared. You can continue.", true);
                SetCurrentBeatComplete(true);
                return;
            }

            SetFeedback("Try the other side too.", true);
        }

        private void RenderComponentOrderBeat(ChatbotFundamentalsBeat beat)
        {
            SetRootActive(actionButtonRoot, true);
            SetRootActive(componentButtonRoot, true);
            SetTextActive(componentChainText, true);
            SetTextActive(backendLinkText, true);
            UpdateComponentTexts();

            var backendButton = CreateDynamicButton(
                actionButtonRoot,
                "Attach backend side link",
                new Color(0.93f, 1f, 0.95f));
            backendButton.onClick.AddListener(() =>
            {
                backendSideLinked = true;
                UpdateComponentTexts();
                CheckComponentCompletion(beat);
            });

            foreach (var part in ChatbotFundamentalsData.CreateComponentPaletteOrder())
            {
                var capturedPart = part;
                var button = CreateDynamicButton(componentButtonRoot, capturedPart.Label, new Color(0.96f, 0.96f, 1f));
                button.onClick.AddListener(() => HandleComponentSelection(beat, capturedPart));
            }
        }

        private void HandleComponentSelection(ChatbotFundamentalsBeat beat, ChatbotComponentPart part)
        {
            if (selectedComponentIds.Contains(part.Id) || componentPathCorrect)
            {
                return;
            }

            selectedComponentIds.Add(part.Id);
            UpdateComponentTexts();

            var expected = ChatbotFundamentalsData.CreateComponentOrder();
            if (selectedComponentIds.Count < expected.Count)
            {
                SetFeedback("Keep building the voice path.", true);
                return;
            }

            componentPathCorrect = IsSelectedComponentOrderCorrect(expected);
            if (!componentPathCorrect)
            {
                selectedComponentIds.Clear();
                UpdateComponentTexts();
                SetText(
                    consequenceText,
                    "That path sends Ghost's voice through the parts in the wrong order, so the reply fizzles out.");
                SetText(ghostStatusText, "Ghost makes a tiny confused sound and waits for a cleaner path.");
                SetFeedback("Try the component order again.", false);
                return;
            }

            SetText(
                consequenceText,
                "The main path is correct. Attach the backend side link if it is not connected yet.");
            SetText(ghostStatusText, "Ghost's voice path is almost connected.");
            CheckComponentCompletion(beat);
        }

        private bool IsSelectedComponentOrderCorrect(IReadOnlyList<ChatbotComponentPart> expected)
        {
            if (selectedComponentIds.Count != expected.Count)
            {
                return false;
            }

            for (var i = 0; i < expected.Count; i++)
            {
                if (!string.Equals(selectedComponentIds[i], expected[i].Id, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private void CheckComponentCompletion(ChatbotFundamentalsBeat beat)
        {
            if (!componentPathCorrect || !backendSideLinked)
            {
                SetFeedback("Main path and backend side link are both needed.", true);
                return;
            }

            SetText(consequenceText, beat.Consequence);
            SetText(ghostStatusText, "Ghost's voice glows from input to reply.");
            SetFeedback("Voice parts connected. You can continue.", true);
            SetCurrentBeatComplete(true);
        }

        private void UpdateComponentTexts()
        {
            var selectedLabels = new List<string>();
            foreach (var id in selectedComponentIds)
            {
                selectedLabels.Add(GetComponentLabel(id));
            }

            var chain = selectedLabels.Count == 0
                ? "Voice path: (empty)"
                : "Voice path: " + string.Join(" -> ", selectedLabels.ToArray());

            SetText(componentChainText, chain);
            SetText(
                backendLinkText,
                backendSideLinked
                    ? "Backend side link: connected for stored progress, logs, or outside data."
                    : "Backend side link: not attached yet.");
        }

        private static string GetComponentLabel(string id)
        {
            foreach (var part in ChatbotFundamentalsData.CreateComponentOrder())
            {
                if (string.Equals(id, part.Id, StringComparison.Ordinal))
                {
                    return part.Label;
                }
            }

            return id;
        }

        private void RenderChallengeModesBeat(ChatbotFundamentalsBeat beat)
        {
            SetRootActive(challengeButtonRoot, true);
            SetFeedback("Trigger all four tiny failure modes.", true);

            foreach (var mode in ChatbotFundamentalsData.CreateChallengeModes())
            {
                var capturedMode = mode;
                var button = CreateDynamicButton(challengeButtonRoot, capturedMode.Label, new Color(1f, 0.95f, 0.86f));
                button.onClick.AddListener(() => HandleChallengeMode(beat, capturedMode));
            }
        }

        private void HandleChallengeMode(ChatbotFundamentalsBeat beat, ChatbotChallengeMode mode)
        {
            challengeModesSeen.Add(mode.Id);
            SetText(ghostProblemText, mode.Problem);
            SetText(consequenceText, mode.Consequence);
            SetText(ghostStatusText, "Ghost shows the failure mode: " + mode.Label + ".");

            if (challengeModesSeen.Count >= ChatbotFundamentalsData.CreateChallengeModes().Count)
            {
                SetText(consequenceText, beat.Consequence);
                SetFeedback("All four challenges seen. You can continue.", true);
                SetCurrentBeatComplete(true);
                return;
            }

            SetFeedback(challengeModesSeen.Count + " / 4 failure modes seen.", true);
        }

        private void ShowLilyExplanation(string text)
        {
            if (dialogueFrame != null)
            {
                dialogueFrame.Show(new ShellDialogueLine(ShellDialogueData.LilySpeakerName, text));
            }
        }

        private void SetCurrentBeatComplete(bool isComplete)
        {
            currentBeatComplete = isComplete;
            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            if (previousButton != null)
            {
                previousButton.interactable = currentBeatIndex > 0;
            }

            if (nextButton != null)
            {
                nextButton.interactable = currentBeatComplete;
                SetButtonLabel(nextButton, beats != null && currentBeatIndex >= beats.Count - 1 ? "Back to Acts" : "Next");
            }

            if (skipButton != null)
            {
                skipButton.interactable = true;
            }
        }

        private void SetFeedback(string message, bool neutral)
        {
            SetText(feedbackText, message);

            if (feedbackText != null)
            {
                feedbackText.color = neutral
                    ? new Color(0.28f, 0.25f, 0.38f)
                    : new Color(0.62f, 0.20f, 0.16f);
            }
        }

        private void ClearDynamicButtons()
        {
            ClearChildren(actionButtonRoot);
            ClearChildren(componentButtonRoot);
            ClearChildren(challengeButtonRoot);
        }

        private static void ClearChildren(Transform root)
        {
            if (root == null)
            {
                return;
            }

            for (var i = root.childCount - 1; i >= 0; i--)
            {
                var child = root.GetChild(i).gameObject;
                if (Application.isPlaying)
                {
                    Destroy(child);
                }
                else
                {
                    DestroyImmediate(child);
                }
            }
        }

        private static Button CreateDynamicButton(Transform parent, string label, Color color)
        {
            var buttonObject = new GameObject(label + " Button", typeof(RectTransform));
            buttonObject.transform.SetParent(parent, false);

            var image = buttonObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = true;

            var outline = buttonObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.48f, 0.54f, 0.76f, 0.78f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var layoutElement = buttonObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 42f;
            layoutElement.preferredHeight = 42f;
            layoutElement.flexibleWidth = 1f;

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;

            var textTransform = new GameObject("Button Text", typeof(RectTransform)).GetComponent<RectTransform>();
            textTransform.SetParent(buttonObject.transform, false);
            textTransform.anchorMin = Vector2.zero;
            textTransform.anchorMax = Vector2.one;
            textTransform.offsetMin = new Vector2(10f, 2f);
            textTransform.offsetMax = new Vector2(-10f, -2f);

            var text = textTransform.gameObject.AddComponent<Text>();
            text.text = label;
            text.font = GetBuiltinFont();
            text.fontSize = 15;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = new Color(0.12f, 0.16f, 0.28f);
            text.raycastTarget = false;

            return button;
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

        private static void SetButtonLabel(Button button, string label)
        {
            if (button == null)
            {
                return;
            }

            var text = button.GetComponentInChildren<Text>(true);
            if (text != null)
            {
                text.text = label;
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value ?? string.Empty;
            }
        }

        private static void SetTextActive(Text text, bool isActive)
        {
            if (text != null)
            {
                text.gameObject.SetActive(isActive);
            }
        }

        private static void SetRootActive(Transform root, bool isActive)
        {
            if (root != null)
            {
                root.gameObject.SetActive(isActive);
            }
        }

        private static Font GetBuiltinFont()
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font != null)
            {
                return font;
            }

            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }
    }
}
