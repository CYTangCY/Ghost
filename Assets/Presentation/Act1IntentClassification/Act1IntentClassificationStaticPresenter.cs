using Ghost.Presentation.Common;
using System;
using System.Collections.Generic;
using Ghost.Presentation.GhostAvatar;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.IntentClassification;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Ghost.Presentation.Act1IntentClassification
{
    public sealed class Act1IntentClassificationStaticPresenter : MonoBehaviour
    {
        private const float CardPreferredHeight = 52f;

        // A transcript sitting in a pile is already sorted, so it does not need the full reading height
        // it gets in the left-hand list. Compact rows are what let a pile show all of its contents
        // without swallowing the column.
        private const float CardInPileHeight = 46f;
        private const float PileMinHeight = 150f;
        private const int PileColumnCount = 3;
        private const float LabelChipHeight = 38f;
        private const float ObjectiveStripHeight = 40f;
        private const float OnboardingPanelHeight = 180f;
        private const float ConversationPanelHeight = 178f;
        private const float TeachingPanelPreferredHeight = 96f;
        private const float ControlsPreferredHeight = 96f;

        private const string TitleText = "Act 1: Train Ghost to Greet Visitors";
        private const string InstructionText =
            "Watch Ghost fail, cluster visitor transcripts into training piles, label each purpose, then teach Ghost.";
        private const string TeachingPanelBodyText =
            "Lily: Um... build piles around visitor purpose, then let new visitors test what Ghost learned.";
        private const string OnboardingTitleText = "Lily's quick training loop";
        private const string OnboardingBodyText =
            "Lily: Um... first, watch Ghost fail when a visitor uses new words.\n" +
            "Lily: Then cluster the old transcripts into piles and label each pile's purpose.\n" +
            "Lily: Teach Ghost with those piles, then watch it answer new visitors.";

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
        private static readonly Color ObjectiveStripColor = new Color(0.14f, 0.18f, 0.32f);

        [SerializeField] private RectTransform cardListRoot;
        [SerializeField, FormerlySerializedAs("pileList")] private RectTransform pileList;
        [SerializeField] private GameObject cardTemplate;
        [SerializeField] private GameObject intentGroupTemplate;
        [SerializeField] private bool renderOnStart = true;

        private readonly Dictionary<string, GameObject> cardViewsById = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, Image> cardImagesById = new Dictionary<string, Image>();
        private readonly Dictionary<string, Outline> cardOutlinesById = new Dictionary<string, Outline>();
        private readonly Dictionary<string, Image> labelImagesByIntentId = new Dictionary<string, Image>();

        private Act1IntentClassificationInteractionController controller;
        private Canvas rootCanvas;
        private RectTransform pageHeader;
        private Text phaseProgressText;
        private RectTransform objectiveStrip;
        private Text objectiveText;
        private RectTransform onboardingPanel;
        private RectTransform prototypeBody;
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
        private Button replayOnboardingButton;
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
                pileList == null ||
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

            EnsureExperienceChrome();
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
            UpdateExperienceChrome();

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(cardListRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(pileList);
        }

        private void ClearRenderedState()
        {
            cardViewsById.Clear();
            cardImagesById.Clear();
            cardOutlinesById.Clear();
            labelImagesByIntentId.Clear();
            ClearChildren(cardListRoot);
            ClearChildren(pileList);
        }

        private void RenderUnpiledCards()
        {
            GhostUITheme.Label(
                cardListRoot.parent,
                "Sample Message Cards",
                "Transcript Cards",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink,
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
            GhostUITheme.Label(
                pileList.parent,
                "Intent Group Areas",
                "Training Piles",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink,
                30f);

            CreateNowInstruction(pileList);

            // Before the Build step the piles cannot be used, so showing the label palette and the
            // drop zone just presented controls that quietly did nothing.
            var canBuild = controller.Phase == Act1TeachingPhase.Build ||
                controller.Phase == Act1TeachingPhase.Demo ||
                controller.Phase == Act1TeachingPhase.Complete;

            if (!canBuild)
            {
                return;
            }

            CreateLabelPalette(pileList);
            CreatePileColumns(pileList);
        }

        private void CreateLabelPalette(Transform parent)
        {
            var palette = GhostUITheme.Panel("Purpose Label Chips", parent, new Color(1f, 1f, 1f, 0f)).rectTransform;
            var paletteLayoutElement = palette.gameObject.AddComponent<LayoutElement>();
            paletteLayoutElement.minHeight = 88f;
            paletteLayoutElement.preferredHeight = 88f;

            var layout = palette.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label("Purpose Labels", palette, "Purpose labels", GhostUITheme.SmallSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 18f);

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
            var chip = GhostUITheme.Chip("Purpose Label - " + intentId, parent, LabelColor).rectTransform;
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

            var label = GhostUITheme.Label(chip, GetPurposeLabel(intentId), GhostUITheme.SmallSize, FontStyle.Bold, TextAnchor.MiddleCenter);
            label.color = new Color(0.10f, 0.18f, 0.30f);
        }

        /// <summary>
        /// Three side-by-side columns, one per purpose. Stacking piles vertically meant that once the
        /// third pile existed the panel overflowed with no way to scroll, so the player could not see
        /// what they had built. There are exactly three purposes, so three columns always fit.
        /// </summary>
        private void CreatePileColumns(Transform parent)
        {
            var row = new GameObject("Pile Columns", typeof(RectTransform)).GetComponent<RectTransform>();
            row.SetParent(parent, false);

            var element = row.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 210f;
            element.flexibleHeight = 1f;

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            for (var i = 0; i < PileColumnCount; i++)
            {
                if (i < controller.Piles.Count)
                {
                    CreatePileView(row, controller.Piles[i]);
                }
                else
                {
                    CreateNewPileDropZone(row);
                }
            }
        }

        private void CreateNewPileDropZone(Transform parent)
        {
            var zone = GhostUITheme.DropZone("New Pile Drop Zone", parent, new Color(1f, 0.985f, 0.90f)).rectTransform;
            zone.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var outline = zone.gameObject.AddComponent<Outline>();
            SetOutline(outline, new Color(0.86f, 0.58f, 0.22f, 0.95f), new Vector2(1.5f, -1.5f));

            var button = zone.gameObject.AddComponent<Button>();
            button.targetGraphic = zone.GetComponent<Image>();
            button.onClick.AddListener(() =>
            {
                if (controller.HasSelectedLabel)
                {
                    controller.AssignSelectedLabelToNewPile();
                    return;
                }

                controller.MoveSelectedCardToNewPile();
            });

            var drop = zone.gameObject.AddComponent<Act1IntentTeachingDropTarget>();
            drop.InitializeNewPile(controller.MoveCardToNewPile, controller.AssignLabelToNewPile);

            var text = GhostUITheme.Label(zone, "Drop a transcript here\nto start this pile", GhostUITheme.SmallSize, FontStyle.Italic, TextAnchor.MiddleCenter);
            text.color = new Color(0.34f, 0.25f, 0.14f);
        }

        /// <summary>
        /// A plain "do this next" line inside the panel where the doing happens. The objective strip at
        /// the top of the page carries the same phase, but players were reading the play area and
        /// finding no statement of what they were meant to be doing there.
        /// </summary>
        private void CreateNowInstruction(Transform parent)
        {
            var banner = GhostUITheme.Card("Now Instruction", parent, new Color(1f, 0.95f, 0.80f)).rectTransform;
            ConfigureLayoutElement(banner.gameObject, 0f, 86f, 1f);

            var layout = banner.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 8, 8);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var heading = GhostUITheme.Label(
                banner,
                "Now Heading",
                GetNowHeading(),
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink,
                24f);
            heading.raycastTarget = false;

            var detail = GhostUITheme.Label(
                banner,
                "Now Detail",
                GetNowDetail(),
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft,
                44f);
            detail.raycastTarget = false;
        }

        private bool IsReadyToTeach()
        {
            if (controller.UnpiledCardIds.Count > 0 || controller.Piles.Count == 0)
            {
                return false;
            }

            foreach (var pile in controller.Piles)
            {
                if (string.IsNullOrEmpty(pile.IntentLabelId) || pile.CardIds.Count == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private string GetNowHeading()
        {
            switch (controller.Phase)
            {
                case Act1TeachingPhase.Intro:
                    return "Now: watch Ghost get it wrong";
                case Act1TeachingPhase.Build:
                    if (controller.Piles.Count == 0)
                    {
                        return "Now: start your first pile";
                    }

                    return IsReadyToTeach()
                        ? "Now: press Teach Ghost"
                        : "Now: sort every transcript, then label each pile";
                case Act1TeachingPhase.Demo:
                    return "Now: teach Ghost and watch a new visitor";
                case Act1TeachingPhase.Complete:
                    return "Done - Ghost answers by purpose now";
                default:
                    return "Now: read Lily's loop above";
            }
        }

        private string GetNowDetail()
        {
            switch (controller.Phase)
            {
                case Act1TeachingPhase.Intro:
                    return "Press Next in Ghost's panel until the visitor runs out of patience.";
                case Act1TeachingPhase.Build:
                    if (controller.Piles.Count == 0)
                    {
                        return "Drag a transcript - or a purpose label - onto one of the empty columns below.";
                    }

                    return IsReadyToTeach()
                        ? "Every transcript is sorted and every pile is labelled. See what Ghost learned."
                        : "Drag more transcripts onto a pile, then drop a purpose label onto its socket.";
                case Act1TeachingPhase.Demo:
                    return "Press Next visitor to hear each one, and watch which pile Ghost leans on.";
                case Act1TeachingPhase.Complete:
                    return "Press Complete Act to take this back to Lily.";
                default:
                    return "Ghost only repeats sentences it has heard word for word.";
            }
        }

        private void CreatePileView(Transform parent, Act1IntentPileState pile)
        {
            var pileView = GhostUITheme.DropZone("Training Pile - " + pile.Id, parent, PileColor).rectTransform;
            var pileElement = pileView.gameObject.AddComponent<LayoutElement>();
            pileElement.minHeight = PileMinHeight;
            pileElement.flexibleWidth = 1f;

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
            ConfigureLayoutElement(header.gameObject, 0f, 34f, 0f);

            var layout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var socket = GhostUITheme.DropZone("Label Socket", header, string.IsNullOrEmpty(pile.IntentLabelId) ? new Color(1f, 1f, 1f, 0.55f) : LabelColor).rectTransform;
            ConfigureLayoutElement(socket.gameObject, 0f, 32f, 1f);
            var socketButton = socket.gameObject.AddComponent<Button>();
            socketButton.targetGraphic = socket.GetComponent<Image>();
            socketButton.onClick.AddListener(() => controller.AssignSelectedLabelToPile(pile.Id));
            var socketText = GhostUITheme.Label(
                socket,
                string.IsNullOrEmpty(pile.IntentLabelId) ? "Drop label" : GetPurposeLabel(pile.IntentLabelId),
                GhostUITheme.SmallSize,
                string.IsNullOrEmpty(pile.IntentLabelId) ? FontStyle.Italic : FontStyle.Bold,
                TextAnchor.MiddleCenter);
            socketText.color = GhostUITheme.Ink;

            // How many transcripts are in here. Previously you had to count the visible cards, and the
            // list was clipped, so the number was simply not knowable.
            var count = GhostUITheme.Chip("Card Count", header, new Color(0.83f, 0.90f, 1f)).rectTransform;
            ConfigureLayoutElement(count.gameObject, 34f, 32f, 0f);
            var countText = GhostUITheme.Label(
                count,
                pile.CardIds.Count.ToString(),
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter);
            countText.color = GhostUITheme.Ink;

            var clear = GhostUITheme.Chip("Clear Label", header, new Color(1f, 0.94f, 0.90f)).rectTransform;
            ConfigureLayoutElement(clear.gameObject, 52f, 32f, 0f);
            var clearButton = clear.gameObject.AddComponent<Button>();
            clearButton.targetGraphic = clear.GetComponent<Image>();
            clearButton.onClick.AddListener(() => controller.ClearPileLabel(pile.Id));
            var clearText = GhostUITheme.Label(clear, "Clear", GhostUITheme.TinySize, FontStyle.Bold, TextAnchor.MiddleCenter);
            clearText.color = new Color(0.34f, 0.14f, 0.12f);
        }

        private void CreatePileCards(Transform parent, Act1IntentPileState pile)
        {
            var cardRoot = new GameObject("Pile Cards", typeof(RectTransform)).GetComponent<RectTransform>();
            cardRoot.SetParent(parent, false);
            ConfigureLayoutElement(cardRoot.gameObject, 0f, 0f, 1f);

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

            // Every transcript stays visible. Grouping them IS the puzzle, so hiding the contents
            // behind a "+N more" summary took away the only feedback the player has about their own
            // work. They are rendered compactly instead - see CardInPileHeight.
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
            var isSelected = controller.SelectedCardId == cardId;
            var isHighlighted = controller.IsCardHighlighted(cardId);
            var color = isHighlighted
                ? CardMisleadingColor
                : isSelected
                    ? CardSelectedColor
                    : isInPile ? CardInPileColor : CardDefaultColor;
            var image = GhostUITheme.Card(view, color);
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

            ConfigureLayoutElement(view, 0f, isInPile ? CardInPileHeight : CardPreferredHeight, 0f);

            if (isInPile)
            {
                // The template styles transcripts at TitleSize for the wide left-hand list. In a
                // third-width column that size wraps to three lines and gets clipped.
                var message = FindChildText(view.transform, "MessageText");
                if (message != null)
                {
                    GhostUITheme.Label(message, message.text, GhostUITheme.SmallSize, FontStyle.Normal, TextAnchor.MiddleLeft, GhostUITheme.Ink);
                }
            }

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
            EnsurePageHeader();

            EnsureTeachingPanel(transform);
            ConfigureColumnPanelLayout(cardListRoot.parent);
            ConfigureColumnPanelLayout(pileList.parent);
            ConfigureListRoot(cardListRoot, 6f);
            ConfigureListRoot(pileList, 8f);
            ConfigurePanelSurface(cardListRoot.parent, new Color(1f, 0.985f, 0.94f), new Color(0.82f, 0.70f, 0.90f, 0.85f));
            ConfigurePanelSurface(pileList.parent, PanelColor, new Color(0.60f, 0.72f, 0.90f, 0.90f));

            prototypeBody = cardListRoot.parent != null ? cardListRoot.parent.parent as RectTransform : null;
            var bodyLayout = prototypeBody != null ? prototypeBody.GetComponent<HorizontalLayoutGroup>() : null;
            if (prototypeBody != null)
            {
                var bodyElement = prototypeBody.GetComponent<LayoutElement>() ??
                    prototypeBody.gameObject.AddComponent<LayoutElement>();
                bodyElement.minHeight = 96f;
                bodyElement.flexibleHeight = 1f;
            }

            if (bodyLayout != null)
            {
                bodyLayout.spacing = 18f;
                bodyLayout.childControlWidth = true;
                bodyLayout.childControlHeight = true;
                bodyLayout.childForceExpandWidth = true;
                bodyLayout.childForceExpandHeight = true;
            }
        }

        private void EnsurePageHeader()
        {
            pageHeader = transform.Find("Header") as RectTransform;
            if (pageHeader == null)
            {
                pageHeader = new GameObject("Header", typeof(RectTransform)).GetComponent<RectTransform>();
                pageHeader.SetParent(transform, false);
            }

            pageHeader.SetAsFirstSibling();
            GhostUITheme.Panel(pageHeader.gameObject, Color.clear).raycastTarget = false;
            ConfigureLayoutElement(pageHeader.gameObject, 0f, 44f, 0f);
            // The header is a fixed title row, but its inner horizontal group force-expands height,
            // which reports flexible height to the page and let the header eat all the spare space.
            pageHeader.GetComponent<LayoutElement>().flexibleHeight = 0f;
            var headerLayout = pageHeader.GetComponent<HorizontalLayoutGroup>();
            if (headerLayout == null)
            {
                headerLayout = pageHeader.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            headerLayout.spacing = 16f;
            headerLayout.padding = new RectOffset(0, 220, 0, 0);
            headerLayout.childControlWidth = true;
            headerLayout.childControlHeight = true;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = true;

            var titleRoot = pageHeader.Find("Title") as RectTransform;
            if (titleRoot == null)
            {
                titleRoot = transform.Find("Title") as RectTransform;
                if (titleRoot == null)
                {
                    titleRoot = new GameObject("Title", typeof(RectTransform)).GetComponent<RectTransform>();
                }

                titleRoot.SetParent(pageHeader, false);
            }

            var title = titleRoot.GetComponent<Text>();
            if (title == null)
            {
                title = titleRoot.gameObject.AddComponent<Text>();
            }

            GhostUITheme.Label(title, TitleText, GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink);
            ConfigureLayoutElement(titleRoot.gameObject, 0f, 44f, 1f);

            var progressRoot = pageHeader.Find("Phase Progress") as RectTransform;
            if (progressRoot == null)
            {
                progressRoot = new GameObject("Phase Progress", typeof(RectTransform)).GetComponent<RectTransform>();
                progressRoot.SetParent(pageHeader, false);
            }

            phaseProgressText = progressRoot.GetComponent<Text>();
            if (phaseProgressText == null)
            {
                phaseProgressText = progressRoot.gameObject.AddComponent<Text>();
            }

            GhostUITheme.Label(phaseProgressText, string.Empty, GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleRight, GhostUITheme.InkSoft);
            ConfigureLayoutElement(progressRoot.gameObject, 210f, 44f, 0f);

            var subtitle = transform.Find("Subtitle");
            if (subtitle != null)
            {
                subtitle.gameObject.SetActive(false);
            }
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

            var image = GhostUITheme.Panel(conversationPanel.gameObject, new Color(0.94f, 0.975f, 1f, 0.96f));
            image.raycastTarget = false;

            var outline = conversationPanel.GetComponent<Outline>();
            if (outline == null)
            {
                outline = conversationPanel.gameObject.AddComponent<Outline>();
            }

            SetOutline(outline, new Color(0.54f, 0.66f, 0.86f, 0.90f), new Vector2(2f, -2f));

            ConfigureLayoutElement(conversationPanel.gameObject, 0f, ConversationPanelHeight, 0f);
            // Same trap as the header: the inner group force-expands height, so without this the
            // panel reports flexible height and stretches down the rest of the page.
            conversationPanel.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var layout = conversationPanel.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = conversationPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 14f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            ghostFaceView = EnsureGhostFace(conversationPanel);
            var textColumn = EnsureConversationTextColumn(conversationPanel);
            visitorText = EnsureConversationText(textColumn, "Visitor Line", GhostUITheme.BodySize, FontStyle.Bold, 34f);
            ghostReplyText = EnsureConversationText(textColumn, "Ghost Reply", GhostUITheme.HeadingSize, FontStyle.Bold, 42f);
            conversationNoteText = EnsureConversationText(textColumn, "Conversation Note", GhostUITheme.SmallSize, FontStyle.Italic, 32f);
            conversationAdvanceButton = EnsureConversationButton(conversationPanel, out conversationAdvanceButtonText);
            conversationAdvanceButton.onClick.RemoveAllListeners();
            conversationAdvanceButton.onClick.AddListener(() => controller.AdvanceConversation());
        }

        private void EnsureControls()
        {
            var parent = pileList.parent;
            controlsRoot = parent.Find("Validation Controls") as RectTransform;
            if (controlsRoot == null)
            {
                controlsRoot = new GameObject("Validation Controls", typeof(RectTransform)).GetComponent<RectTransform>();
                controlsRoot.SetParent(parent, false);
            }

            controlsRoot.SetAsLastSibling();

            var image = GhostUITheme.Panel(controlsRoot.gameObject, new Color(1f, 0.99f, 0.94f, 0.94f));
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

        private void EnsureExperienceChrome()
        {
            objectiveStrip = EnsureObjectiveStrip(transform, out objectiveText);
            onboardingPanel = EnsureOnboardingPanel(transform);
            prototypeBody = transform.Find("Prototype Body") as RectTransform;

            var onboardingButton = EnsureControlButton(
                onboardingPanel,
                "Watch Ghost Fail Button",
                "Watch Ghost fail",
                190f);
            onboardingButton.onClick.RemoveAllListeners();
            onboardingButton.onClick.AddListener(controller.BeginAfterOnboarding);

            var teachingPanel = transform.Find("Lily Intent Teaching Panel");
            var teachingNoteRow = teachingPanel != null ? teachingPanel.Find("Teaching Note Row") : null;
            if (teachingNoteRow != null)
            {
                replayOnboardingButton = EnsureControlButton(
                    teachingNoteRow,
                    "Replay Lily Button",
                    "Replay Lily",
                    130f);
                replayOnboardingButton.onClick.RemoveAllListeners();
                replayOnboardingButton.onClick.AddListener(controller.ReplayOnboarding);
            }

            if (pageHeader != null)
            {
                objectiveStrip.SetSiblingIndex(pageHeader.GetSiblingIndex() + 1);
                onboardingPanel.SetSiblingIndex(objectiveStrip.GetSiblingIndex() + 1);
            }

            if (teachingPanel != null)
            {
                teachingPanel.SetSiblingIndex(onboardingPanel.GetSiblingIndex() + 1);
                conversationPanel.SetSiblingIndex(teachingPanel.GetSiblingIndex() + 1);
            }

            if (prototypeBody != null)
            {
                prototypeBody.SetSiblingIndex(conversationPanel.GetSiblingIndex() + 1);
            }
        }

        private void UpdateExperienceChrome()
        {
            if (controller == null)
            {
                return;
            }

            if (objectiveText != null)
            {
                objectiveText.text = GetObjectiveText();
            }

            if (phaseProgressText != null)
            {
                phaseProgressText.text = GetPhaseProgressText();
            }

            var isOnboarding = controller.Phase == Act1TeachingPhase.Onboarding;
            if (onboardingPanel != null)
            {
                onboardingPanel.gameObject.SetActive(isOnboarding);
            }

            var teachingPanel = transform.Find("Lily Intent Teaching Panel");
            if (teachingPanel != null)
            {
                teachingPanel.gameObject.SetActive(!isOnboarding);
            }

            if (conversationPanel != null)
            {
                conversationPanel.gameObject.SetActive(true);
            }

            if (prototypeBody != null)
            {
                prototypeBody.gameObject.SetActive(!isOnboarding);
            }

            if (replayOnboardingButton != null)
            {
                replayOnboardingButton.gameObject.SetActive(
                    !isOnboarding && controller.Phase != Act1TeachingPhase.Complete);
            }
        }

        private string GetObjectiveText()
        {
            switch (controller.Phase)
            {
                case Act1TeachingPhase.Onboarding:
                    return "Setup: learn the training loop before touching the transcripts";
                case Act1TeachingPhase.Intro:
                    return "1/3 Watch Ghost fail when exact-word matching breaks";
                case Act1TeachingPhase.Build:
                    return "2/3 Build + label training piles by visitor purpose";
                case Act1TeachingPhase.Demo:
                    return "3/3 Teach Ghost and check how it answers new visitors";
                case Act1TeachingPhase.Complete:
                    return "Complete: Ghost can answer new visitors from the training piles";
                default:
                    return "Act 1 training";
            }
        }

        private string GetPhaseProgressText()
        {
            switch (controller.Phase)
            {
                case Act1TeachingPhase.Onboarding:
                    return "Step 0/3";
                case Act1TeachingPhase.Intro:
                    return "Step 1/3";
                case Act1TeachingPhase.Build:
                    return "Step 2/3";
                case Act1TeachingPhase.Demo:
                    return "Step 3/3";
                case Act1TeachingPhase.Complete:
                    return "Complete";
                default:
                    return string.Empty;
            }
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
            if (controller.Phase == Act1TeachingPhase.Onboarding)
            {
                return GhostMood.Neutral;
            }

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
                    feedbackText.color = GhostUITheme.Good;
                    feedbackText.fontStyle = FontStyle.Bold;
                    break;
                case Act1IntentClassificationFeedbackKind.Incorrect:
                    feedbackText.color = GhostUITheme.Bad;
                    feedbackText.fontStyle = FontStyle.Bold;
                    break;
                default:
                    feedbackText.color = GhostUITheme.InkSoft;
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

            layout.padding = new RectOffset(36, 36, 26, 24);
            layout.spacing = 14f;
            // Blocks size to their content now, so centre whatever slack is left instead of
            // letting one of them absorb it.
            layout.childAlignment = TextAnchor.MiddleCenter;

            var image = transform.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color(0.96f, 0.94f, 1f);
                image.raycastTarget = false;
            }
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

            var image = GhostUITheme.Panel(panel.gameObject, TeachingPanelColor);
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

            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 0f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var oldTitle = panel.Find("Teaching Panel Title");
            if (oldTitle != null)
            {
                oldTitle.gameObject.SetActive(false);
            }

            var oldBody = panel.Find("Teaching Panel Body");
            if (oldBody != null)
            {
                oldBody.gameObject.SetActive(false);
            }

            var row = panel.Find("Teaching Note Row") as RectTransform;
            if (row == null)
            {
                row = new GameObject("Teaching Note Row", typeof(RectTransform)).GetComponent<RectTransform>();
                row.SetParent(panel, false);
            }

            ConfigureLayoutElement(row.gameObject, 0f, 40f, 0f);
            var rowLayout = row.GetComponent<HorizontalLayoutGroup>();
            if (rowLayout == null)
            {
                rowLayout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            rowLayout.spacing = 10f;
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = true;

            GhostUITheme.Label(
                row,
                "Teaching Note Text",
                TeachingPanelBodyText,
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.25f, 0.20f, 0.18f),
                40f);
            row.Find("Teaching Note Text").GetComponent<LayoutElement>().flexibleWidth = 1f;
        }

        private static RectTransform EnsureObjectiveStrip(Transform root, out Text label)
        {
            var strip = root.Find("Objective Strip") as RectTransform;
            if (strip == null)
            {
                strip = new GameObject("Objective Strip", typeof(RectTransform)).GetComponent<RectTransform>();
                strip.SetParent(root, false);
            }

            var image = GhostUITheme.Panel(strip.gameObject, ObjectiveStripColor);
            image.raycastTarget = false;
            ConfigureLayoutElement(strip.gameObject, 0f, ObjectiveStripHeight, 0f);

            // ConfigureLayoutElement's last argument is flexibleWidth, so height was never pinned and
            // the strip swallowed all the spare space during setup, before the puzzle body exists.
            strip.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var layout = strip.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = strip.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.padding = new RectOffset(12, 12, 4, 4);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            GhostUITheme.Label(
                strip,
                "Objective Text",
                string.Empty,
                GhostUITheme.HeadingSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkOnDark,
                34f);
            label = strip.Find("Objective Text").GetComponent<Text>();
            return strip;
        }

        private static RectTransform EnsureOnboardingPanel(Transform root)
        {
            var panel = root.Find("Onboarding Panel") as RectTransform;
            if (panel == null)
            {
                panel = new GameObject("Onboarding Panel", typeof(RectTransform)).GetComponent<RectTransform>();
                panel.SetParent(root, false);
            }

            var image = GhostUITheme.Panel(panel.gameObject, TeachingPanelColor);
            image.raycastTarget = false;

            var outline = panel.GetComponent<Outline>();
            if (outline == null)
            {
                outline = panel.gameObject.AddComponent<Outline>();
            }

            SetOutline(outline, TeachingPanelOutlineColor, new Vector2(2f, -2f));
            ConfigureLayoutElement(panel.gameObject, 0f, OnboardingPanelHeight, 0f);

            var layout = panel.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(18, 18, 12, 12);
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                panel,
                "Onboarding Title",
                OnboardingTitleText,
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.28f, 0.18f, 0.08f),
                26f);
            GhostUITheme.Label(
                panel,
                "Onboarding Body",
                OnboardingBodyText,
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.20f, 0.18f),
                88f);
            return panel;
        }

        private static GhostFaceView EnsureGhostFace(Transform parent)
        {
            var root = parent.Find("Ghost Face") as RectTransform;
            if (root == null)
            {
                root = new GameObject("Ghost Face", typeof(RectTransform)).GetComponent<RectTransform>();
                root.SetParent(parent, false);
            }

            ConfigureLayoutElement(root.gameObject, 150f, 150f, 0f);
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

            var label = text.GetComponent<Text>() ?? text.gameObject.AddComponent<Text>();
            GhostUITheme.Label(
                label,
                string.Empty,
                fontSize,
                fontStyle,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);
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

            var button = GhostUITheme.PushButton(
                root.gameObject,
                "Next",
                new Color(0.84f, 0.92f, 1f),
                GhostUITheme.Ink);
            ConfigureLayoutElement(root.gameObject, 128f, 42f, 0f);
            label = button.GetComponentInChildren<Text>();
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

            var button = GhostUITheme.PushButton(
                root.gameObject,
                text,
                new Color(0.84f, 0.92f, 1f),
                GhostUITheme.Ink);
            ConfigureLayoutElement(root.gameObject, width, 42f, 0f);
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
            var text = root.GetComponent<Text>() ?? root.gameObject.AddComponent<Text>();
            GhostUITheme.Label(
                text,
                string.Empty,
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft);
            text.raycastTarget = false;
            return text;
        }
        private static void CreatePlaceholder(Transform parent, string message, float height)
        {
            var text = GhostUITheme.Label(
                "Placeholder",
                parent,
                message,
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.MiddleCenter,
                GhostUITheme.InkSoft,
                height);
            text.raycastTarget = false;
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



        private static void ConfigurePanelSurface(Transform root, Color color, Color outlineColor)
        {
            if (root == null)
            {
                return;
            }

            var image = GhostUITheme.Panel(root.gameObject, color);
            image.raycastTarget = color.a > 0.01f;

            var outline = root.GetComponent<Outline>();
            if (outline != null)
            {
                SetOutline(outline, outlineColor, new Vector2(2f, -2f));
            }
        }

        private static Text FindChildText(Transform root, string childName)
        {
            var child = root.Find(childName);
            return child == null ? null : child.GetComponent<Text>();
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
                    return "looking for something";
                case Act1IntentClassificationSampleData.AskLocationIntentId:
                    return "asking where Ghost is";
                case Act1IntentClassificationSampleData.AskIdentityIntentId:
                    return "asking who Ghost is";
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

    }
}
