using System;
using System.Collections.Generic;
using Ghost.Presentation.GhostAvatar;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.IntentClassification;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Act1IntentClassification
{
    public sealed class Act1IntentClassificationStaticPresenter : MonoBehaviour
    {
        private const float CardPreferredHeight = 52f;
        private const float PilePreferredHeight = 128f;
        private const float LabelChipHeight = 38f;
        private const float ConversationPanelHeight = 150f;
        private const float TeachingPanelPreferredHeight = 66f;
        private const float ControlsPreferredHeight = 92f;

        private const string TitleText = "Act 1: Train Ghost to Greet Visitors";
        private const string InstructionText =
            "Watch Ghost fail, cluster visitor transcripts into training piles, label each purpose, then teach Ghost.";
        private const string TeachingPanelTitleText = "Lily's Intent Note";
        private const string TeachingPanelBodyText =
            "Lily: Um... Ghost memorizes sentences. We need piles that show the visitor's purpose, not exact words.";

        private static readonly Color CardDefaultColor = new Color(1f, 0.99f, 0.94f);
        private static readonly Color CardSelectedColor = new Color(1f, 0.91f, 0.52f);
        private static readonly Color CardInPileColor = new Color(0.90f, 1f, 0.91f);
        private static readonly Color CardMisleadingColor = new Color(1f, 0.89f, 0.78f);
        private static readonly Color CardOutlineDefaultColor = new Color(0.78f, 0.70f, 0.88f, 0.62f);
        private static readonly Color CardOutlineSelectedColor = new Color(0.98f, 0.64f, 0.10f, 0.90f);
        private static readonly Color CardOutlineMisleadingColor = new Color(0.88f, 0.26f, 0.12f, 0.95f);
        private static readonly Color PanelColor = new Color(0.98f, 0.985f, 1f);
        private static readonly Color PileColor = new Color(0.92f, 0.97f, 1f);
        private static readonly Color PileReadyColor = new Color(0.84f, 0.93f, 1f);
        private static readonly Color LabelColor = new Color(0.88f, 0.95f, 1f);
        private static readonly Color LabelSelectedColor = new Color(1f, 0.92f, 0.62f);
        private static readonly Color TeachingPanelColor = new Color(1f, 0.96f, 0.82f, 0.96f);
        private static readonly Color TeachingPanelOutlineColor = new Color(0.86f, 0.58f, 0.22f, 0.95f);
        private static readonly Color FeedbackNeutralColor = new Color(0.24f, 0.22f, 0.30f);
        private static readonly Color FeedbackCorrectColor = new Color(0.08f, 0.42f, 0.18f);
        private static readonly Color FeedbackIncorrectColor = new Color(0.62f, 0.16f, 0.13f);

        [SerializeField] private RectTransform cardListRoot;
        [SerializeField] private RectTransform intentGroupListRoot;
        [SerializeField] private GameObject cardTemplate;
        [SerializeField] private GameObject intentGroupTemplate;
        [SerializeField] private bool renderOnStart = true;

        private readonly Dictionary<string, GameObject> cardViewsById = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, Image> cardImagesById = new Dictionary<string, Image>();
        private readonly Dictionary<string, Outline> cardOutlinesById = new Dictionary<string, Outline>();
        private readonly Dictionary<string, Image> labelImagesByIntentId = new Dictionary<string, Image>();

        private Act1IntentClassificationInteractionController controller;
        private Canvas rootCanvas;
        private RectTransform conversationPanel;
        private GhostFaceView ghostFaceView;
        private Text visitorText;
        private Text ghostReplyText;
        private Text conversationNoteText;
        private Button conversationAdvanceButton;
        private Text conversationAdvanceButtonText;
        private RectTransform controlsRoot;
        private Button teachButton;
        private Button reviseButton;
        private Button completeButton;
        private Text feedbackText;

        private static readonly string[] IntentIds =
        {
            Act1IntentClassificationSampleData.FindItemIntentId,
            Act1IntentClassificationSampleData.AskLocationIntentId,
            Act1IntentClassificationSampleData.AskIdentityIntentId
        };

        private void Start()
        {
            if (renderOnStart)
            {
                RenderSampleData();
            }
        }

        private void OnDestroy()
        {
            DetachController();
        }

        public void RenderSampleData()
        {
            if (cardListRoot == null ||
                intentGroupListRoot == null ||
                cardTemplate == null ||
                intentGroupTemplate == null)
            {
                return;
            }

            rootCanvas = GetComponentInParent<Canvas>();
            EnsureEventSystem();
            EnsureInstructionText();
            EnsureConversationPanel();
            EnsureControls();
            DetachController();

            controller = new Act1IntentClassificationInteractionController(
                Act1IntentClassificationSampleData.CreateCards());
            controller.StateChanged += RefreshAll;
            controller.FeedbackChanged += ApplyFeedback;

            RefreshAll();
        }

        private void RefreshAll()
        {
            ClearRenderedState();
            ConfigureUnpiledDropTarget(cardListRoot.gameObject);
            RenderUnpiledCards();
            RenderLabelPaletteAndPiles();
            UpdateConversationPanel();
            UpdateControls();
            ApplyFeedback(controller.CurrentFeedback);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(cardListRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(intentGroupListRoot);
        }

        private void ClearRenderedState()
        {
            cardViewsById.Clear();
            cardImagesById.Clear();
            cardOutlinesById.Clear();
            labelImagesByIntentId.Clear();
            ClearChildren(cardListRoot);
            ClearChildren(intentGroupListRoot);
        }

        private void RenderUnpiledCards()
        {
            ConfigureExistingLabel(
                cardListRoot.parent,
                "Sample Message Cards",
                "Transcript Cards",
                22,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.12f, 0.28f),
                30f);

            foreach (var cardId in controller.UnpiledCardIds)
            {
                CreateCardView(cardListRoot, controller.GetCard(cardId), false);
            }

            if (controller.UnpiledCardIds.Count == 0)
            {
                CreatePlaceholder(cardListRoot, "All transcripts are in piles. Drag one back here to remove it.", 44f);
            }
        }

        private void RenderLabelPaletteAndPiles()
        {
            ConfigureExistingLabel(
                intentGroupListRoot.parent,
                "Intent Group Areas",
                "Training Piles",
                22,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.12f, 0.28f),
                30f);

            CreateLabelPalette(intentGroupListRoot);
            CreateNewPileDropZone(intentGroupListRoot);

            foreach (var pile in controller.Piles)
            {
                CreatePileView(intentGroupListRoot, pile);
            }

            if (controller.Piles.Count == 0)
            {
                CreatePlaceholder(intentGroupListRoot, "Drag a transcript to the new-pile zone to start Ghost's training data.", 40f);
            }
        }

        private void CreateLabelPalette(Transform parent)
        {
            var palette = CreatePanel("Purpose Label Chips", parent, new Color(1f, 1f, 1f, 0f));
            var paletteLayoutElement = palette.gameObject.AddComponent<LayoutElement>();
            paletteLayoutElement.minHeight = 88f;
            paletteLayoutElement.preferredHeight = 88f;

            var layout = palette.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateSmallText(palette, "Purpose labels", 13, FontStyle.Bold, TextAnchor.MiddleLeft, 18f);

            var row = new GameObject("Purpose Label Row", typeof(RectTransform)).GetComponent<RectTransform>();
            row.SetParent(palette, false);
            var rowLayout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 8f;
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = true;
            rowLayout.childForceExpandHeight = false;

            var rowElement = row.gameObject.AddComponent<LayoutElement>();
            rowElement.minHeight = LabelChipHeight;
            rowElement.preferredHeight = LabelChipHeight;

            foreach (var intentId in IntentIds)
            {
                CreateLabelChip(row, intentId);
            }
        }

        private void CreateLabelChip(Transform parent, string intentId)
        {
            var chip = CreatePanel("Purpose Label - " + intentId, parent, LabelColor);
            ConfigureLayoutElement(chip.gameObject, 110f, LabelChipHeight, 1f);

            var image = chip.GetComponent<Image>();
            labelImagesByIntentId[intentId] = image;
            var isSelected = controller != null && controller.SelectedLabelIntentId == intentId;
            image.color = isSelected ? LabelSelectedColor : LabelColor;

            var outline = chip.gameObject.AddComponent<Outline>();
            SetOutline(outline, isSelected ? CardOutlineSelectedColor : CardOutlineDefaultColor, new Vector2(1.5f, -1.5f));

            var button = chip.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => controller.SelectLabel(intentId));

            var drag = chip.gameObject.AddComponent<Act1IntentClassificationLabelDragView>();
            drag.Initialize(intentId, rootCanvas);

            var label = CreateFillText(chip, GetPurposeLabel(intentId), 13, FontStyle.Bold, TextAnchor.MiddleCenter);
            label.color = new Color(0.10f, 0.18f, 0.30f);
        }

        private void CreateNewPileDropZone(Transform parent)
        {
            var zone = CreatePanel("New Pile Drop Zone", parent, new Color(1f, 0.985f, 0.90f));
            ConfigureLayoutElement(zone.gameObject, 0f, 48f, 0f);
            var outline = zone.gameObject.AddComponent<Outline>();
            SetOutline(outline, new Color(0.86f, 0.58f, 0.22f, 0.95f), new Vector2(1.5f, -1.5f));

            var button = zone.gameObject.AddComponent<Button>();
            button.targetGraphic = zone.GetComponent<Image>();
            button.onClick.AddListener(() => controller.MoveSelectedCardToNewPile());

            var drop = zone.gameObject.AddComponent<Act1IntentTeachingDropTarget>();
            drop.InitializeNewPile(controller.MoveCardToNewPile);

            var text = CreateFillText(zone, "Drop a transcript here to start a new training pile", 14, FontStyle.Italic, TextAnchor.MiddleCenter);
            text.color = new Color(0.34f, 0.25f, 0.14f);
        }

        private void CreatePileView(Transform parent, Act1IntentPileState pile)
        {
            var pileView = CreatePanel("Training Pile - " + pile.Id, parent, PileColor);
            ConfigureLayoutElement(pileView.gameObject, 0f, PilePreferredHeight, 0f);

            var image = pileView.GetComponent<Image>();
            image.color = controller.HasSelectedCard || controller.HasSelectedLabel ? PileReadyColor : PileColor;

            var outline = pileView.gameObject.AddComponent<Outline>();
            SetOutline(outline, new Color(0.58f, 0.68f, 0.84f, 0.82f), new Vector2(1.5f, -1.5f));

            var layout = pileView.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 8, 8);
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var button = pileView.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() =>
            {
                if (controller.HasSelectedCard)
                {
                    controller.MoveSelectedCardToPile(pile.Id);
                }
                else if (controller.HasSelectedLabel)
                {
                    controller.AssignSelectedLabelToPile(pile.Id);
                }
            });

            var drop = pileView.gameObject.AddComponent<Act1IntentTeachingDropTarget>();
            drop.InitializePile(pile.Id, controller.MoveCardToPile, controller.AssignLabelToPile);

            CreatePileHeader(pileView, pile);
            CreatePileCards(pileView, pile);
        }

        private void CreatePileHeader(Transform parent, Act1IntentPileState pile)
        {
            var header = new GameObject("Pile Header", typeof(RectTransform)).GetComponent<RectTransform>();
            header.SetParent(parent, false);
            ConfigureLayoutElement(header.gameObject, 0f, 32f, 0f);

            var layout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var socket = CreatePanel("Label Socket", header, string.IsNullOrEmpty(pile.IntentLabelId) ? new Color(1f, 1f, 1f, 0.55f) : LabelColor);
            ConfigureLayoutElement(socket.gameObject, 0f, 30f, 1f);
            var socketButton = socket.gameObject.AddComponent<Button>();
            socketButton.targetGraphic = socket.GetComponent<Image>();
            socketButton.onClick.AddListener(() => controller.AssignSelectedLabelToPile(pile.Id));
            var socketText = CreateFillText(
                socket,
                string.IsNullOrEmpty(pile.IntentLabelId) ? "Drop purpose label here" : GetPurposeLabel(pile.IntentLabelId),
                13,
                string.IsNullOrEmpty(pile.IntentLabelId) ? FontStyle.Italic : FontStyle.Bold,
                TextAnchor.MiddleCenter);
            socketText.color = new Color(0.12f, 0.18f, 0.30f);

            var clear = CreatePanel("Clear Label", header, new Color(1f, 0.94f, 0.90f));
            ConfigureLayoutElement(clear.gameObject, 72f, 30f, 0f);
            var clearButton = clear.gameObject.AddComponent<Button>();
            clearButton.targetGraphic = clear.GetComponent<Image>();
            clearButton.onClick.AddListener(() => controller.ClearPileLabel(pile.Id));
            var clearText = CreateFillText(clear, "Clear", 12, FontStyle.Bold, TextAnchor.MiddleCenter);
            clearText.color = new Color(0.34f, 0.14f, 0.12f);
        }

        private void CreatePileCards(Transform parent, Act1IntentPileState pile)
        {
            var cardRoot = new GameObject("Pile Cards", typeof(RectTransform)).GetComponent<RectTransform>();
            cardRoot.SetParent(parent, false);
            ConfigureLayoutElement(cardRoot.gameObject, 0f, 74f, 1f);

            var layout = cardRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            if (pile.CardIds.Count == 0)
            {
                CreatePlaceholder(cardRoot, "Drop matching transcripts here.", 30f);
                return;
            }

            foreach (var cardId in pile.CardIds)
            {
                CreateCardView(cardRoot, controller.GetCard(cardId), true);
            }
        }

        private void CreateCardView(Transform parent, IntentCard card, bool isInPile)
        {
            var view = Instantiate(cardTemplate, parent);
            view.name = "Card - " + card.Id;
            view.SetActive(true);

            ConfigureCardContainer(view, card.Id, isInPile);
            SetChildText(view.transform, "MessageText", card.MessageText);
            SetChildActive(view.transform, "CardIdText", false);
            SetChildActive(view.transform, "IntentHintText", false);

            cardViewsById[card.Id] = view;
            cardImagesById[card.Id] = view.GetComponent<Image>();
            cardOutlinesById[card.Id] = view.GetComponent<Outline>();
        }

        private void ConfigureCardContainer(GameObject view, string cardId, bool isInPile)
        {
            var image = view.GetComponent<Image>();
            if (image == null)
            {
                image = view.AddComponent<Image>();
            }

            var isSelected = controller.SelectedCardId == cardId;
            var isHighlighted = controller.IsCardHighlighted(cardId);
            image.color = isHighlighted ? CardMisleadingColor : isSelected ? CardSelectedColor : isInPile ? CardInPileColor : CardDefaultColor;
            image.raycastTarget = true;

            var outline = view.GetComponent<Outline>();
            if (outline == null)
            {
                outline = view.AddComponent<Outline>();
            }

            SetOutline(
                outline,
                isHighlighted ? CardOutlineMisleadingColor : isSelected ? CardOutlineSelectedColor : CardOutlineDefaultColor,
                isHighlighted || isSelected ? new Vector2(2.5f, -2.5f) : new Vector2(1.2f, -1.2f));

            var button = view.GetComponent<Button>();
            if (button == null)
            {
                button = view.AddComponent<Button>();
            }

            button.targetGraphic = image;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => controller.SelectCard(cardId));

            var draggable = view.GetComponent<Act1IntentClassificationDraggableCard>();
            if (draggable == null)
            {
                draggable = view.AddComponent<Act1IntentClassificationDraggableCard>();
            }

            draggable.Initialize(cardId, rootCanvas);

            ConfigureLayoutElement(view, 0f, CardPreferredHeight, 0f);

            var layout = view.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = view.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(12, 12, 6, 6);
            layout.spacing = 0f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private void EnsureInstructionText()
        {
            ConfigureRootLayout();
            ConfigureExistingLabel(
                transform,
                "Title",
                TitleText,
                40,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.12f, 0.28f),
                50f);

            ConfigureExistingLabel(
                transform,
                "Subtitle",
                InstructionText,
                18,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.26f, 0.21f, 0.35f),
                32f);

            EnsureTeachingPanel(transform);
            ConfigureColumnPanelLayout(cardListRoot.parent);
            ConfigureColumnPanelLayout(intentGroupListRoot.parent);
            ConfigureListRoot(cardListRoot, 6f);
            ConfigureListRoot(intentGroupListRoot, 8f);
            ConfigurePanelSurface(cardListRoot.parent, new Color(1f, 0.985f, 0.94f), new Color(0.82f, 0.70f, 0.90f, 0.85f));
            ConfigurePanelSurface(intentGroupListRoot.parent, PanelColor, new Color(0.60f, 0.72f, 0.90f, 0.90f));
        }

        private void EnsureConversationPanel()
        {
            conversationPanel = transform.Find("Ghost Conversation Demo") as RectTransform;
            if (conversationPanel == null)
            {
                conversationPanel = new GameObject("Ghost Conversation Demo", typeof(RectTransform)).GetComponent<RectTransform>();
                conversationPanel.SetParent(transform, false);
            }

            var teachingPanel = transform.Find("Lily Intent Teaching Panel");
            if (teachingPanel != null)
            {
                conversationPanel.SetSiblingIndex(teachingPanel.GetSiblingIndex() + 1);
            }

            var image = conversationPanel.GetComponent<Image>();
            if (image == null)
            {
                image = conversationPanel.gameObject.AddComponent<Image>();
            }

            image.color = new Color(0.94f, 0.975f, 1f, 0.96f);
            image.raycastTarget = false;

            var outline = conversationPanel.GetComponent<Outline>();
            if (outline == null)
            {
                outline = conversationPanel.gameObject.AddComponent<Outline>();
            }

            SetOutline(outline, new Color(0.54f, 0.66f, 0.86f, 0.90f), new Vector2(2f, -2f));

            ConfigureLayoutElement(conversationPanel.gameObject, 0f, ConversationPanelHeight, 0f);

            var layout = conversationPanel.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = conversationPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.padding = new RectOffset(16, 14, 10, 10);
            layout.spacing = 14f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            ghostFaceView = EnsureGhostFace(conversationPanel);
            var textColumn = EnsureConversationTextColumn(conversationPanel);
            visitorText = EnsureConversationText(textColumn, "Visitor Line", 17, FontStyle.Bold, 34f);
            ghostReplyText = EnsureConversationText(textColumn, "Ghost Reply", 18, FontStyle.Bold, 42f);
            conversationNoteText = EnsureConversationText(textColumn, "Conversation Note", 14, FontStyle.Italic, 32f);
            conversationAdvanceButton = EnsureConversationButton(conversationPanel, out conversationAdvanceButtonText);
            conversationAdvanceButton.onClick.RemoveAllListeners();
            conversationAdvanceButton.onClick.AddListener(() => controller.AdvanceConversation());
        }

        private void EnsureControls()
        {
            var parent = intentGroupListRoot.parent;
            controlsRoot = parent.Find("Validation Controls") as RectTransform;
            if (controlsRoot == null)
            {
                controlsRoot = new GameObject("Validation Controls", typeof(RectTransform)).GetComponent<RectTransform>();
                controlsRoot.SetParent(parent, false);
            }

            controlsRoot.SetAsLastSibling();

            var image = controlsRoot.GetComponent<Image>();
            if (image == null)
            {
                image = controlsRoot.gameObject.AddComponent<Image>();
            }

            image.color = new Color(1f, 0.99f, 0.94f, 0.94f);
            image.raycastTarget = false;
            ConfigureLayoutElement(controlsRoot.gameObject, 0f, ControlsPreferredHeight, 0f);

            var layout = controlsRoot.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = controlsRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.padding = new RectOffset(10, 10, 8, 8);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            teachButton = EnsureControlButton(controlsRoot, "Teach Ghost Button", "Teach Ghost", 140f);
            teachButton.onClick.RemoveAllListeners();
            teachButton.onClick.AddListener(() => controller.TeachGhost());

            reviseButton = EnsureControlButton(controlsRoot, "Revise Piles Button", "Revise piles", 130f);
            reviseButton.onClick.RemoveAllListeners();
            reviseButton.onClick.AddListener(() => controller.ReturnToBuild());

            completeButton = EnsureControlButton(controlsRoot, "Complete Act Button", "Complete Act", 142f);
            completeButton.onClick.RemoveAllListeners();
            completeButton.onClick.AddListener(CompleteActAndReturnToHub);

            feedbackText = EnsureFeedbackText(controlsRoot);
        }

        private void UpdateConversationPanel()
        {
            var beat = controller.GetCurrentConversationBeat();
            visitorText.text = beat.VisitorLine;
            ghostReplyText.text = beat.GhostReply;
            conversationNoteText.text = beat.Note;
            conversationAdvanceButton.gameObject.SetActive(beat.HasAdvanceButton);
            conversationAdvanceButtonText.text = beat.AdvanceButtonText;
            ghostFaceView.SetMood(GetGhostMood());
        }

        private void UpdateControls()
        {
            teachButton.gameObject.SetActive(controller.Phase == Act1TeachingPhase.Build);
            reviseButton.gameObject.SetActive(controller.Phase == Act1TeachingPhase.Demo);
            completeButton.gameObject.SetActive(controller.Phase == Act1TeachingPhase.Complete);
        }

        private void CompleteActAndReturnToHub()
        {
            GhostNarrativeState.SetPendingDebriefAct(GhostNarrativeState.Act1Id);
            SceneManager.LoadScene(ShellSceneNames.GameShellSceneName);
        }

        private GhostMood GetGhostMood()
        {
            if (controller.Phase == Act1TeachingPhase.Complete)
            {
                return GhostMood.Happy;
            }

            if (controller.Phase == Act1TeachingPhase.Intro)
            {
                return GhostMood.Confused;
            }

            if (controller.Phase == Act1TeachingPhase.Demo)
            {
                return controller.CurrentDemoResult != null && controller.CurrentDemoResult.IsCorrect
                    ? GhostMood.Happy
                    : GhostMood.Confused;
            }

            return GhostMood.Neutral;
        }

        private void ApplyFeedback(Act1IntentClassificationFeedback feedback)
        {
            if (feedbackText == null)
            {
                return;
            }

            feedbackText.text = feedback.Message;
            switch (feedback.Kind)
            {
                case Act1IntentClassificationFeedbackKind.Correct:
                    feedbackText.color = FeedbackCorrectColor;
                    feedbackText.fontStyle = FontStyle.Bold;
                    break;
                case Act1IntentClassificationFeedbackKind.Incorrect:
                    feedbackText.color = FeedbackIncorrectColor;
                    feedbackText.fontStyle = FontStyle.Bold;
                    break;
                default:
                    feedbackText.color = FeedbackNeutralColor;
                    feedbackText.fontStyle = FontStyle.Normal;
                    break;
            }
        }

        private void ConfigureUnpiledDropTarget(GameObject view)
        {
            var target = view.GetComponent<Act1IntentTeachingDropTarget>();
            if (target == null)
            {
                target = view.AddComponent<Act1IntentTeachingDropTarget>();
            }

            target.InitializeUnassigned(controller.MoveCardToUnpiled);
        }

        private void DetachController()
        {
            if (controller == null)
            {
                return;
            }

            controller.StateChanged -= RefreshAll;
            controller.FeedbackChanged -= ApplyFeedback;
            controller = null;
        }

        private void ConfigureRootLayout()
        {
            var layout = transform.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                return;
            }

            layout.padding = new RectOffset(36, 36, 22, 26);
            layout.spacing = 10f;
        }

        private static void ConfigureColumnPanelLayout(Transform root)
        {
            if (root == null)
            {
                return;
            }

            var layout = root.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                return;
            }

            layout.padding = new RectOffset(18, 18, 14, 14);
            layout.spacing = 10f;
        }

        private static void ConfigureListRoot(RectTransform root, float spacing)
        {
            if (root == null)
            {
                return;
            }

            var layout = root.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                return;
            }

            layout.spacing = spacing;
        }

        private static void EnsureTeachingPanel(Transform root)
        {
            if (root == null)
            {
                return;
            }

            var panel = root.Find("Lily Intent Teaching Panel") as RectTransform;
            if (panel == null)
            {
                panel = new GameObject("Lily Intent Teaching Panel", typeof(RectTransform)).GetComponent<RectTransform>();
                panel.SetParent(root, false);
            }

            var subtitle = root.Find("Subtitle");
            if (subtitle != null)
            {
                panel.SetSiblingIndex(subtitle.GetSiblingIndex() + 1);
            }

            var image = panel.GetComponent<Image>();
            if (image == null)
            {
                image = panel.gameObject.AddComponent<Image>();
            }

            image.color = TeachingPanelColor;
            image.raycastTarget = false;

            var outline = panel.GetComponent<Outline>();
            if (outline == null)
            {
                outline = panel.gameObject.AddComponent<Outline>();
            }

            SetOutline(outline, TeachingPanelOutlineColor, new Vector2(2f, -2f));
            ConfigureLayoutElement(panel.gameObject, 0f, TeachingPanelPreferredHeight, 0f);

            var layout = panel.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(16, 16, 6, 6);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            ConfigureOrCreateLabel(
                panel,
                "Teaching Panel Title",
                TeachingPanelTitleText,
                14,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.30f, 0.20f, 0.08f),
                18f);

            ConfigureOrCreateLabel(
                panel,
                "Teaching Panel Body",
                TeachingPanelBodyText,
                14,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.20f, 0.18f),
                34f);
        }

        private static GhostFaceView EnsureGhostFace(Transform parent)
        {
            var root = parent.Find("Ghost Face") as RectTransform;
            if (root == null)
            {
                root = new GameObject("Ghost Face", typeof(RectTransform)).GetComponent<RectTransform>();
                root.SetParent(parent, false);
            }

            ConfigureLayoutElement(root.gameObject, 150f, 128f, 0f);
            var view = root.GetComponent<GhostFaceView>();
            if (view == null)
            {
                view = root.gameObject.AddComponent<GhostFaceView>();
            }

            return view;
        }

        private static RectTransform EnsureConversationTextColumn(Transform parent)
        {
            var column = parent.Find("Conversation Text Column") as RectTransform;
            if (column == null)
            {
                column = new GameObject("Conversation Text Column", typeof(RectTransform)).GetComponent<RectTransform>();
                column.SetParent(parent, false);
            }

            ConfigureLayoutElement(column.gameObject, 0f, 120f, 1f);
            var layout = column.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return column;
        }

        private static Text EnsureConversationText(
            Transform parent,
            string name,
            int fontSize,
            FontStyle fontStyle,
            float preferredHeight)
        {
            var text = parent.Find(name) as RectTransform;
            if (text == null)
            {
                text = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
                text.SetParent(parent, false);
            }

            var label = text.GetComponent<Text>();
            if (label == null)
            {
                label = text.gameObject.AddComponent<Text>();
            }

            label.font = GetBuiltinFont();
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.color = new Color(0.14f, 0.16f, 0.24f);
            label.raycastTarget = false;
            ConfigureLayoutElement(text.gameObject, 0f, preferredHeight, 0f);
            return label;
        }

        private static Button EnsureConversationButton(Transform parent, out Text label)
        {
            var root = parent.Find("Conversation Advance Button") as RectTransform;
            if (root == null)
            {
                root = new GameObject("Conversation Advance Button", typeof(RectTransform)).GetComponent<RectTransform>();
                root.SetParent(parent, false);
            }

            var image = root.GetComponent<Image>();
            if (image == null)
            {
                image = root.gameObject.AddComponent<Image>();
            }

            image.color = new Color(0.84f, 0.92f, 1f);
            image.raycastTarget = true;
            ConfigureLayoutElement(root.gameObject, 128f, 42f, 0f);

            var button = root.GetComponent<Button>();
            if (button == null)
            {
                button = root.gameObject.AddComponent<Button>();
            }

            button.targetGraphic = image;
            label = CreateFillText(root, "Next", 13, FontStyle.Bold, TextAnchor.MiddleCenter);
            label.color = new Color(0.12f, 0.18f, 0.30f);
            return button;
        }

        private static Button EnsureControlButton(Transform parent, string name, string text, float width)
        {
            var root = parent.Find(name) as RectTransform;
            if (root == null)
            {
                root = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
                root.SetParent(parent, false);
            }

            var image = root.GetComponent<Image>();
            if (image == null)
            {
                image = root.gameObject.AddComponent<Image>();
            }

            image.color = new Color(0.84f, 0.92f, 1f);
            image.raycastTarget = true;
            ConfigureLayoutElement(root.gameObject, width, 42f, 0f);

            var button = root.GetComponent<Button>();
            if (button == null)
            {
                button = root.gameObject.AddComponent<Button>();
            }

            button.targetGraphic = image;
            var label = CreateFillText(root, text, 13, FontStyle.Bold, TextAnchor.MiddleCenter);
            label.color = new Color(0.12f, 0.18f, 0.30f);
            return button;
        }

        private static Text EnsureFeedbackText(Transform parent)
        {
            var root = parent.Find("Training Feedback") as RectTransform;
            if (root == null)
            {
                root = new GameObject("Training Feedback", typeof(RectTransform)).GetComponent<RectTransform>();
                root.SetParent(parent, false);
            }

            ConfigureLayoutElement(root.gameObject, 0f, 58f, 1f);
            var text = root.GetComponent<Text>();
            if (text == null)
            {
                text = root.gameObject.AddComponent<Text>();
            }

            text.font = GetBuiltinFont();
            text.fontSize = 14;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        private static void CreatePlaceholder(Transform parent, string message, float height)
        {
            var text = new GameObject("Placeholder", typeof(RectTransform)).AddComponent<Text>();
            text.transform.SetParent(parent, false);
            text.text = message;
            text.font = GetBuiltinFont();
            text.fontSize = 13;
            text.fontStyle = FontStyle.Italic;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.42f, 0.40f, 0.50f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            ConfigureLayoutElement(text.gameObject, 0f, height, 0f);
        }

        private static Text CreateSmallText(Transform parent, string message, int fontSize, FontStyle style, TextAnchor alignment, float height)
        {
            var text = new GameObject("Small Text", typeof(RectTransform)).AddComponent<Text>();
            text.transform.SetParent(parent, false);
            text.text = message;
            text.font = GetBuiltinFont();
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = new Color(0.22f, 0.19f, 0.30f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            ConfigureLayoutElement(text.gameObject, 0f, height, 0f);
            return text;
        }

        private static RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            panel.SetParent(parent, false);
            var image = panel.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = true;
            return panel;
        }

        private static Text CreateFillText(Transform parent, string value, int fontSize, FontStyle fontStyle, TextAnchor alignment)
        {
            var textObject = parent.Find("Text") as RectTransform;
            if (textObject == null)
            {
                textObject = new GameObject("Text", typeof(RectTransform)).GetComponent<RectTransform>();
                textObject.SetParent(parent, false);
            }

            textObject.anchorMin = Vector2.zero;
            textObject.anchorMax = Vector2.one;
            textObject.offsetMin = new Vector2(6f, 2f);
            textObject.offsetMax = new Vector2(-6f, -2f);

            var text = textObject.GetComponent<Text>();
            if (text == null)
            {
                text = textObject.gameObject.AddComponent<Text>();
            }

            text.text = value;
            text.font = GetBuiltinFont();
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        private static void ConfigureLayoutElement(GameObject view, float preferredWidth, float preferredHeight, float flexibleWidth)
        {
            var layoutElement = view.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = view.AddComponent<LayoutElement>();
            }

            if (preferredWidth > 0f)
            {
                layoutElement.minWidth = preferredWidth;
                layoutElement.preferredWidth = preferredWidth;
            }

            if (preferredHeight > 0f)
            {
                layoutElement.minHeight = preferredHeight;
                layoutElement.preferredHeight = preferredHeight;
            }

            layoutElement.flexibleWidth = flexibleWidth;
        }

        private static void ConfigureExistingLabel(
            Transform root,
            string childName,
            string value,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color,
            float preferredHeight)
        {
            if (root == null)
            {
                return;
            }

            var child = root.Find(childName);
            if (child == null)
            {
                return;
            }

            var text = child.GetComponent<Text>();
            if (text != null)
            {
                text.text = value;
                text.font = GetBuiltinFont();
                text.fontSize = fontSize;
                text.fontStyle = fontStyle;
                text.alignment = alignment;
                text.color = color;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
                text.raycastTarget = false;
            }

            ConfigureLayoutElement(child.gameObject, 0f, preferredHeight, 0f);
        }

        private static void ConfigureOrCreateLabel(
            Transform root,
            string childName,
            string value,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color,
            float preferredHeight)
        {
            var child = root.Find(childName) as RectTransform;
            if (child == null)
            {
                child = new GameObject(childName, typeof(RectTransform)).GetComponent<RectTransform>();
                child.SetParent(root, false);
            }

            var text = child.GetComponent<Text>();
            if (text == null)
            {
                text = child.gameObject.AddComponent<Text>();
            }

            text.text = value;
            text.font = GetBuiltinFont();
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            ConfigureLayoutElement(child.gameObject, 0f, preferredHeight, 0f);
        }

        private static void ConfigurePanelSurface(Transform root, Color color, Color outlineColor)
        {
            if (root == null)
            {
                return;
            }

            var image = root.GetComponent<Image>();
            if (image != null)
            {
                image.color = color;
            }

            var outline = root.GetComponent<Outline>();
            if (outline != null)
            {
                SetOutline(outline, outlineColor, new Vector2(2f, -2f));
            }
        }

        private static void SetChildText(Transform root, string childName, string value)
        {
            var child = root.Find(childName);
            if (child == null)
            {
                return;
            }

            var text = child.GetComponent<Text>();
            if (text != null)
            {
                text.text = value;
                text.raycastTarget = false;
            }
        }

        private static void SetChildActive(Transform root, string childName, bool isActive)
        {
            var child = root.Find(childName);
            if (child != null)
            {
                child.gameObject.SetActive(isActive);
            }
        }

        private static void SetOutline(Outline outline, Color color, Vector2 distance)
        {
            if (outline == null)
            {
                return;
            }

            outline.effectColor = color;
            outline.effectDistance = distance;
        }

        private static string GetPurposeLabel(string intentId)
        {
            switch (intentId)
            {
                case Act1IntentClassificationSampleData.FindItemIntentId:
                    return "find something";
                case Act1IntentClassificationSampleData.AskLocationIntentId:
                    return "where is Ghost";
                case Act1IntentClassificationSampleData.AskIdentityIntentId:
                    return "who is Ghost";
                default:
                    return "shared purpose";
            }
        }

        private static void ClearChildren(Transform root)
        {
            var children = new List<GameObject>();
            for (var i = 0; i < root.childCount; i++)
            {
                children.Add(root.GetChild(i).gameObject);
            }

            foreach (var child in children)
            {
                child.SetActive(false);
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

        private static void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
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
    }
}
