using System.Collections.Generic;
using Ghost.Puzzles.IntentClassification;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Ghost.Presentation.Act1IntentClassification
{
    public sealed class Act1IntentClassificationStaticPresenter : MonoBehaviour
    {
        private const float CardPreferredHeight = 58f;
        private const float MessageTextPreferredHeight = 38f;
        private const float GroupPreferredHeight = 166f;
        private const float AssignmentViewportPreferredHeight = 52f;
        private const float AssignedRowPreferredHeight = 20f;
        private const float AssignedPlaceholderPreferredHeight = 18f;
        private const float ValidationControlsPreferredHeight = 108f;
        private const float TeachingPanelPreferredHeight = 76f;

        private const string TitleText = "Act 1: Intent Sorting";
        private const string InstructionText =
            "Group cards by what each visitor wants. Drag mistakes back or between groups, then Validate.";
        private const string TeachingPanelTitleText = "Lily's Intent Note";
        private const string TeachingPanelBodyText =
            "Ghost problem: Ghost is matching exact words, so replies miss the purpose.\n" +
            "Lily: Um... intent means what the visitor wants. Different phrasings in one group become training examples.";

        private static readonly Color CardDefaultColor = new Color(1f, 0.99f, 0.94f);
        private static readonly Color CardSelectedColor = new Color(1f, 0.91f, 0.48f);
        private static readonly Color CardAssignedColor = new Color(0.88f, 1f, 0.90f);
        private static readonly Color CardOutlineDefaultColor = new Color(0.78f, 0.70f, 0.88f, 0.62f);
        private static readonly Color CardOutlineSelectedColor = new Color(0.98f, 0.64f, 0.10f, 0.88f);
        private static readonly Color CardOutlineAssignedColor = new Color(0.30f, 0.63f, 0.38f, 0.72f);
        private static readonly Color GroupDefaultColor = new Color(0.93f, 0.97f, 1f);
        private static readonly Color GroupReadyColor = new Color(0.82f, 0.93f, 1f);
        private static readonly Color GroupOutlineDefaultColor = new Color(0.62f, 0.70f, 0.86f, 0.75f);
        private static readonly Color GroupOutlineReadyColor = new Color(0.28f, 0.54f, 0.95f, 0.90f);
        private static readonly Color AssignmentViewportColor = new Color(1f, 1f, 1f, 0.46f);
        private static readonly Color ValidationPanelColor = new Color(1f, 0.99f, 0.94f, 0.92f);
        private static readonly Color ValidationPanelCorrectColor = new Color(0.89f, 1f, 0.91f, 0.96f);
        private static readonly Color ValidationPanelIncorrectColor = new Color(1f, 0.93f, 0.90f, 0.96f);
        private static readonly Color FeedbackDefaultColor = new Color(0.24f, 0.22f, 0.30f);
        private static readonly Color FeedbackCorrectColor = new Color(0.08f, 0.42f, 0.18f);
        private static readonly Color FeedbackIncorrectColor = new Color(0.62f, 0.16f, 0.13f);
        private static readonly Color AssignedRowColor = new Color(1f, 0.98f, 0.93f);
        private static readonly Color AssignedRowOutlineColor = new Color(0.76f, 0.70f, 0.88f, 0.55f);
        private static readonly Color TeachingPanelColor = new Color(1f, 0.96f, 0.82f, 0.96f);
        private static readonly Color TeachingPanelOutlineColor = new Color(0.86f, 0.58f, 0.22f, 0.95f);

        [SerializeField] private RectTransform cardListRoot;
        [SerializeField] private RectTransform intentGroupListRoot;
        [SerializeField] private GameObject cardTemplate;
        [SerializeField] private GameObject intentGroupTemplate;
        [SerializeField] private bool renderOnStart = true;

        private readonly Dictionary<string, IntentCard> cardsById = new Dictionary<string, IntentCard>();
        private readonly Dictionary<string, GameObject> cardViewsById = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, Image> cardImagesById = new Dictionary<string, Image>();
        private readonly Dictionary<string, Outline> cardOutlinesById = new Dictionary<string, Outline>();
        private readonly Dictionary<string, Image> groupImagesById = new Dictionary<string, Image>();
        private readonly Dictionary<string, Outline> groupOutlinesById = new Dictionary<string, Outline>();
        private readonly Dictionary<string, RectTransform> assignmentRootsByIntentId = new Dictionary<string, RectTransform>();

        private Act1IntentClassificationInteractionController controller;
        private Text validationFeedbackText;
        private Image validationPanelImage;
        private Outline validationPanelOutline;

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

        public void RenderSampleData()
        {
            if (cardListRoot == null ||
                intentGroupListRoot == null ||
                cardTemplate == null ||
                intentGroupTemplate == null)
            {
                return;
            }

            EnsureEventSystem();
            EnsureInstructionText();
            ClearRenderedState();

            var cards = Act1IntentClassificationSampleData.CreateCards();
            controller = new Act1IntentClassificationInteractionController(cards);
            controller.StateChanged += UpdateVisualState;
            controller.FeedbackChanged += ApplyValidationFeedback;
            ConfigureUnassignedDropTarget(cardListRoot.gameObject);

            foreach (var card in controller.Cards)
            {
                cardsById.Add(card.Id, card);
                CreateCardView(card);
            }

            foreach (var intentId in IntentIds)
            {
                CreateIntentGroupView(intentId);
            }

            EnsureValidationControls();
            ApplyValidationFeedback(controller.CurrentFeedback);
            UpdateVisualState();
        }

        private void ClearRenderedState()
        {
            DetachController();
            cardsById.Clear();
            cardViewsById.Clear();
            cardImagesById.Clear();
            cardOutlinesById.Clear();
            groupImagesById.Clear();
            groupOutlinesById.Clear();
            assignmentRootsByIntentId.Clear();
            ClearChildren(cardListRoot);
            ClearChildren(intentGroupListRoot);
        }

        private void CreateCardView(IntentCard card)
        {
            var view = Instantiate(cardTemplate, cardListRoot);
            view.name = "Card - " + card.Id;
            view.SetActive(true);

            ConfigureCardContainer(view, card.Id);
            ConfigureCardMessageText(view.transform, card.MessageText);
            SetChildActive(view.transform, "CardIdText", false);
            SetChildActive(view.transform, "IntentHintText", false);

            cardViewsById.Add(card.Id, view);
            cardImagesById.Add(card.Id, view.GetComponent<Image>());
            cardOutlinesById.Add(card.Id, view.GetComponent<Outline>());
        }

        private void CreateIntentGroupView(string intentId)
        {
            var view = Instantiate(intentGroupTemplate, intentGroupListRoot);
            view.name = "Intent Group - " + intentId;
            view.SetActive(true);

            ConfigureIntentGroupContainer(view, intentId);
            ConfigureExistingLabel(
                view.transform,
                "IntentTitleText",
                GetIntentTitle(intentId),
                21,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.10f, 0.20f, 0.32f),
                30f);
            ConfigureExistingLabel(
                view.transform,
                "IntentHintText",
                GetIntentDescription(intentId),
                15,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.28f, 0.34f, 0.44f),
                34f);

            var assignmentRoot = EnsureAssignmentRoot(view.transform);
            ConfigureAssignmentViewportClick(assignmentRoot, intentId);
            var assignmentViewport = assignmentRoot.parent;
            if (assignmentViewport != null)
            {
                ConfigureIntentGroupDropTarget(assignmentViewport.gameObject, intentId);
            }

            assignmentRootsByIntentId.Add(intentId, assignmentRoot);
            groupImagesById.Add(intentId, view.GetComponent<Image>());
            groupOutlinesById.Add(intentId, view.GetComponent<Outline>());
        }

        private void DetachController()
        {
            if (controller == null)
            {
                return;
            }

            controller.StateChanged -= UpdateVisualState;
            controller.FeedbackChanged -= ApplyValidationFeedback;
            controller = null;
        }

        private void EnsureInstructionText()
        {
            ConfigureRootLayout();

            ConfigureExistingLabel(
                transform,
                "Title",
                TitleText,
                42,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.12f, 0.28f),
                58f);

            ConfigureExistingLabel(
                transform,
                "Subtitle",
                InstructionText,
                20,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.26f, 0.21f, 0.35f),
                38f);

            EnsureTeachingPanel(transform);

            ConfigureColumnPanelLayout(cardListRoot.parent);
            ConfigureColumnPanelLayout(intentGroupListRoot.parent);
            ConfigureListRoot(cardListRoot, 6f);
            ConfigureListRoot(intentGroupListRoot, 8f);

            ConfigureExistingLabel(
                cardListRoot.parent,
                "Sample Message Cards",
                "Unassigned Messages",
                24,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.12f, 0.28f),
                34f);

            ConfigureExistingLabel(
                intentGroupListRoot.parent,
                "Intent Group Areas",
                "Intent Groups",
                24,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.12f, 0.28f),
                34f);

            ConfigurePanelSurface(
                cardListRoot.parent,
                new Color(1f, 0.985f, 0.94f),
                new Color(0.82f, 0.70f, 0.90f, 0.85f));

            ConfigurePanelSurface(
                intentGroupListRoot.parent,
                new Color(0.94f, 0.975f, 1f),
                new Color(0.60f, 0.72f, 0.90f, 0.90f));
        }

        private void ConfigureRootLayout()
        {
            var layout = transform.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                return;
            }

            layout.padding = new RectOffset(40, 40, 24, 24);
            layout.spacing = 12f;
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

            layout.padding = new RectOffset(20, 20, 18, 18);
            layout.spacing = 12f;
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

            var layoutElement = panel.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = panel.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = TeachingPanelPreferredHeight;
            layoutElement.preferredHeight = TeachingPanelPreferredHeight;

            var layout = panel.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(16, 16, 7, 7);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            ConfigureOrCreateLabel(
                panel,
                "Teaching Panel Title",
                TeachingPanelTitleText,
                15,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.30f, 0.20f, 0.08f),
                20f);

            ConfigureOrCreateLabel(
                panel,
                "Teaching Panel Body",
                TeachingPanelBodyText,
                15,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.20f, 0.18f),
                38f);
        }

        private void UpdateVisualState()
        {
            UpdateCardVisuals();
            UpdateIntentGroupVisuals();
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(cardListRoot);
            foreach (var assignmentRoot in assignmentRootsByIntentId.Values)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(assignmentRoot);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(intentGroupListRoot);
        }

        private void UpdateCardVisuals()
        {
            foreach (var pair in cardImagesById)
            {
                var cardId = pair.Key;
                var image = pair.Value;
                if (image == null)
                {
                    continue;
                }

                cardOutlinesById.TryGetValue(cardId, out var outline);
                if (controller != null && cardId == controller.SelectedCardId)
                {
                    image.color = CardSelectedColor;
                    SetOutline(outline, CardOutlineSelectedColor, new Vector2(3f, -3f));
                }
                else if (controller != null && controller.GetAssignedGroupId(cardId) != null)
                {
                    image.color = CardAssignedColor;
                    SetOutline(outline, CardOutlineAssignedColor, new Vector2(2f, -2f));
                }
                else
                {
                    image.color = CardDefaultColor;
                    SetOutline(outline, CardOutlineDefaultColor, new Vector2(1.5f, -1.5f));
                }
            }
        }

        private void UpdateIntentGroupVisuals()
        {
            foreach (var intentId in IntentIds)
            {
                if (groupImagesById.TryGetValue(intentId, out var groupImage) && groupImage != null)
                {
                    var isReadyDropArea = controller != null && controller.HasSelectedCard;
                    groupImage.color = isReadyDropArea ? GroupReadyColor : GroupDefaultColor;

                    groupOutlinesById.TryGetValue(intentId, out var groupOutline);
                    SetOutline(
                        groupOutline,
                        isReadyDropArea ? GroupOutlineReadyColor : GroupOutlineDefaultColor,
                        isReadyDropArea ? new Vector2(3f, -3f) : new Vector2(2f, -2f));
                }

                if (!assignmentRootsByIntentId.TryGetValue(intentId, out var assignmentRoot))
                {
                    continue;
                }

                ClearChildren(assignmentRoot);
                IReadOnlyList<string> assignedCardIds = controller == null
                    ? new string[0]
                    : controller.GetAssignedCardIds(intentId);

                if (assignedCardIds.Count == 0)
                {
                    CreateAssignedText(assignmentRoot, "Drop matching messages here.", true);
                    continue;
                }

                foreach (var cardId in assignedCardIds)
                {
                    if (cardsById.TryGetValue(cardId, out var card))
                    {
                        CreateAssignedCardRow(assignmentRoot, card.Id, card.MessageText);
                    }
                }
            }
        }

        private void ConfigureCardContainer(GameObject view, string cardId)
        {
            var image = view.GetComponent<Image>();
            if (image != null)
            {
                image.color = CardDefaultColor;
                image.raycastTarget = true;
            }

            var outline = view.GetComponent<Outline>();
            if (outline == null)
            {
                outline = view.AddComponent<Outline>();
            }

            SetOutline(outline, CardOutlineDefaultColor, new Vector2(1.5f, -1.5f));

            var button = view.GetComponent<Button>();
            if (button == null)
            {
                button = view.AddComponent<Button>();
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => controller?.SelectCard(cardId));
            button.targetGraphic = image;

            ConfigureCardDrag(view, cardId);

            var layoutElement = view.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = view.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = CardPreferredHeight;
            layoutElement.preferredHeight = CardPreferredHeight;

            var layout = view.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = view.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(14, 14, 7, 7);
            layout.spacing = 0f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private void ConfigureIntentGroupContainer(GameObject view, string intentId)
        {
            var image = view.GetComponent<Image>();
            if (image != null)
            {
                image.color = GroupDefaultColor;
                image.raycastTarget = true;
            }

            var outline = view.GetComponent<Outline>();
            if (outline == null)
            {
                outline = view.AddComponent<Outline>();
            }

            SetOutline(outline, GroupOutlineDefaultColor, new Vector2(2f, -2f));

            var button = view.GetComponent<Button>();
            if (button == null)
            {
                button = view.AddComponent<Button>();
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => controller?.AssignSelectedCardToIntent(intentId));
            button.targetGraphic = image;

            ConfigureIntentGroupDropTarget(view, intentId);

            var layoutElement = view.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = view.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = GroupPreferredHeight;
            layoutElement.preferredHeight = GroupPreferredHeight;

            if (view.GetComponent<RectMask2D>() == null)
            {
                view.AddComponent<RectMask2D>();
            }

            var layout = view.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = view.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(14, 14, 10, 9);
            layout.spacing = 5f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private void ConfigureCardDrag(GameObject view, string cardId)
        {
            var draggable = view.GetComponent<Act1IntentClassificationDraggableCard>();
            if (draggable == null)
            {
                draggable = view.AddComponent<Act1IntentClassificationDraggableCard>();
            }

            draggable.Initialize(cardId, view.GetComponentInParent<Canvas>());
        }

        private void ConfigureIntentGroupDropTarget(GameObject view, string intentId)
        {
            var dropTarget = view.GetComponent<Act1IntentClassificationDropTarget>();
            if (dropTarget == null)
            {
                dropTarget = view.AddComponent<Act1IntentClassificationDropTarget>();
            }

            EnsureRaycastTarget(view);
            dropTarget.InitializeIntentGroup(intentId, AssignDraggedCardToIntent);
        }

        private void ConfigureUnassignedDropTarget(GameObject view)
        {
            var dropTarget = view.GetComponent<Act1IntentClassificationDropTarget>();
            if (dropTarget == null)
            {
                dropTarget = view.AddComponent<Act1IntentClassificationDropTarget>();
            }

            EnsureRaycastTarget(view);
            dropTarget.InitializeUnassigned(MoveDraggedCardToUnassigned);
        }

        private void AssignDraggedCardToIntent(string cardId, string intentId)
        {
            if (controller == null || string.IsNullOrEmpty(cardId))
            {
                return;
            }

            if (controller.GetAssignedGroupId(cardId) == intentId)
            {
                return;
            }

            controller.AssignCardToIntent(cardId, intentId);
        }

        private void MoveDraggedCardToUnassigned(string cardId)
        {
            if (controller == null || string.IsNullOrEmpty(cardId))
            {
                return;
            }

            if (controller.GetAssignedGroupId(cardId) == null)
            {
                return;
            }

            controller.MoveAssignedCardToUnassigned(cardId);
        }

        private static void EnsureRaycastTarget(GameObject view)
        {
            var image = view.GetComponent<Image>();
            if (image == null)
            {
                image = view.AddComponent<Image>();
                image.color = new Color(1f, 1f, 1f, 0f);
            }

            image.raycastTarget = true;
        }

        private static void ConfigureCardMessageText(Transform cardRoot, string messageText)
        {
            var messageTransform = cardRoot.Find("MessageText");
            if (messageTransform == null)
            {
                var messageObject = new GameObject("MessageText", typeof(RectTransform));
                messageTransform = messageObject.transform;
                messageTransform.SetParent(cardRoot, false);
            }

            var text = messageTransform.GetComponent<Text>();
            if (text == null)
            {
                text = messageTransform.gameObject.AddComponent<Text>();
            }

            text.text = messageText;
            text.font = GetBuiltinFont();
            text.fontSize = 18;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = new Color(0.12f, 0.09f, 0.18f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;

            var layoutElement = messageTransform.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = messageTransform.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = MessageTextPreferredHeight;
            layoutElement.preferredHeight = MessageTextPreferredHeight;
            layoutElement.flexibleHeight = 0f;
        }

        private static RectTransform EnsureAssignmentRoot(Transform groupRoot)
        {
            var viewport = groupRoot.Find("AssignedCardsScrollView") as RectTransform;
            if (viewport == null)
            {
                viewport = new GameObject("AssignedCardsScrollView", typeof(RectTransform)).GetComponent<RectTransform>();
                viewport.SetParent(groupRoot, false);
            }

            var assignmentRoot = viewport.Find("AssignedCardsRoot") as RectTransform;
            var oldDirectRoot = groupRoot.Find("AssignedCardsRoot") as RectTransform;

            if (assignmentRoot == null && oldDirectRoot != null)
            {
                assignmentRoot = oldDirectRoot;
                assignmentRoot.SetParent(viewport, false);
            }

            if (assignmentRoot == null)
            {
                assignmentRoot = new GameObject("AssignedCardsRoot", typeof(RectTransform)).GetComponent<RectTransform>();
                assignmentRoot.SetParent(viewport, false);
            }

            ConfigureAssignmentScrollView(viewport, assignmentRoot);
            ConfigureAssignmentRoot(assignmentRoot);

            return assignmentRoot;
        }

        private static void ConfigureAssignmentScrollView(RectTransform viewport, RectTransform content)
        {
            var layoutElement = viewport.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = viewport.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = AssignmentViewportPreferredHeight;
            layoutElement.preferredHeight = AssignmentViewportPreferredHeight;
            layoutElement.flexibleHeight = 1f;

            var image = viewport.GetComponent<Image>();
            if (image == null)
            {
                image = viewport.gameObject.AddComponent<Image>();
            }

            image.color = AssignmentViewportColor;
            image.raycastTarget = true;

            if (viewport.GetComponent<RectMask2D>() == null)
            {
                viewport.gameObject.AddComponent<RectMask2D>();
            }

            var scrollRect = viewport.GetComponent<ScrollRect>();
            if (scrollRect == null)
            {
                scrollRect = viewport.gameObject.AddComponent<ScrollRect>();
            }

            scrollRect.viewport = viewport;
            scrollRect.content = content;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.inertia = true;

            var button = viewport.GetComponent<Button>();
            if (button == null)
            {
                button = viewport.gameObject.AddComponent<Button>();
            }

            button.targetGraphic = image;
        }

        private void ConfigureAssignmentViewportClick(RectTransform assignmentRoot, string intentId)
        {
            var viewport = assignmentRoot.parent;
            if (viewport == null)
            {
                return;
            }

            var button = viewport.GetComponent<Button>();
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => controller?.AssignSelectedCardToIntent(intentId));
        }

        private static void ConfigureAssignmentRoot(RectTransform assignmentRoot)
        {
            assignmentRoot.anchorMin = new Vector2(0f, 1f);
            assignmentRoot.anchorMax = new Vector2(1f, 1f);
            assignmentRoot.pivot = new Vector2(0.5f, 1f);
            assignmentRoot.anchoredPosition = Vector2.zero;
            assignmentRoot.sizeDelta = Vector2.zero;

            if (assignmentRoot.GetComponent<RectMask2D>() == null)
            {
                assignmentRoot.gameObject.AddComponent<RectMask2D>();
            }

            var layout = assignmentRoot.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = assignmentRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = assignmentRoot.GetComponent<ContentSizeFitter>();
            if (fitter == null)
            {
                fitter = assignmentRoot.gameObject.AddComponent<ContentSizeFitter>();
            }

            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private static void CreateAssignedText(Transform parent, string message, bool isPlaceholder)
        {
            var text = new GameObject(isPlaceholder ? "Assigned Placeholder" : "Assigned Card", typeof(RectTransform)).AddComponent<Text>();
            text.transform.SetParent(parent, false);
            text.text = isPlaceholder ? message : "- " + message;
            text.font = GetBuiltinFont();
            text.fontSize = isPlaceholder ? 13 : 12;
            text.fontStyle = isPlaceholder ? FontStyle.Italic : FontStyle.Normal;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = isPlaceholder ? new Color(0.42f, 0.47f, 0.56f) : new Color(0.12f, 0.20f, 0.30f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;

            var layoutElement = text.gameObject.AddComponent<LayoutElement>();
            var preferredHeight = isPlaceholder ? AssignedPlaceholderPreferredHeight : AssignedRowPreferredHeight;
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;
        }

        private void CreateAssignedCardRow(Transform parent, string cardId, string message)
        {
            var row = new GameObject("Assigned Card - " + cardId, typeof(RectTransform));
            row.transform.SetParent(parent, false);

            var image = row.AddComponent<Image>();
            image.color = AssignedRowColor;
            image.raycastTarget = true;

            var outline = row.AddComponent<Outline>();
            outline.effectColor = AssignedRowOutlineColor;
            outline.effectDistance = new Vector2(1f, -1f);

            var button = row.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => controller?.MoveAssignedCardToUnassigned(cardId));

            var layoutElement = row.AddComponent<LayoutElement>();
            layoutElement.minHeight = AssignedRowPreferredHeight;
            layoutElement.preferredHeight = AssignedRowPreferredHeight;

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(6, 6, 1, 1);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            var label = new GameObject("Assigned Card Text", typeof(RectTransform)).AddComponent<Text>();
            label.transform.SetParent(row.transform, false);
            label.text = "Back: " + message;
            label.font = GetBuiltinFont();
            label.fontSize = 10;
            label.fontStyle = FontStyle.Normal;
            label.alignment = TextAnchor.MiddleLeft;
            label.color = new Color(0.12f, 0.20f, 0.30f);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.raycastTarget = false;

            ConfigureCardDrag(row, cardId);
        }

        private void EnsureValidationControls()
        {
            var parent = intentGroupListRoot.parent;
            if (parent == null)
            {
                return;
            }

            var controls = parent.Find("Validation Controls") as RectTransform;
            if (controls == null)
            {
                controls = new GameObject("Validation Controls", typeof(RectTransform)).GetComponent<RectTransform>();
                controls.SetParent(parent, false);
            }

            controls.SetAsLastSibling();

            var image = controls.GetComponent<Image>();
            if (image == null)
            {
                image = controls.gameObject.AddComponent<Image>();
            }

            image.color = ValidationPanelColor;
            image.raycastTarget = false;
            validationPanelImage = image;

            var outline = controls.GetComponent<Outline>();
            if (outline == null)
            {
                outline = controls.gameObject.AddComponent<Outline>();
            }

            SetOutline(outline, CardOutlineDefaultColor, new Vector2(1.5f, -1.5f));
            validationPanelOutline = outline;

            var layoutElement = controls.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = controls.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = ValidationControlsPreferredHeight;
            layoutElement.preferredHeight = ValidationControlsPreferredHeight;

            var layout = controls.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = controls.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.spacing = 12f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.padding = new RectOffset(12, 12, 8, 8);

            var validateButton = EnsureValidateButton(controls);
            validateButton.onClick.RemoveAllListeners();
            validateButton.onClick.AddListener(() => controller?.ValidateCurrentGrouping());

            validationFeedbackText = EnsureValidationFeedbackText(controls);
        }

        private static Button EnsureValidateButton(Transform parent)
        {
            var buttonTransform = parent.Find("Validate Button") as RectTransform;
            if (buttonTransform == null)
            {
                buttonTransform = new GameObject("Validate Button", typeof(RectTransform)).GetComponent<RectTransform>();
                buttonTransform.SetParent(parent, false);
            }

            var image = buttonTransform.GetComponent<Image>();
            if (image == null)
            {
                image = buttonTransform.gameObject.AddComponent<Image>();
            }

            image.color = new Color(0.84f, 0.92f, 1f);
            image.raycastTarget = true;

            var button = buttonTransform.GetComponent<Button>();
            if (button == null)
            {
                button = buttonTransform.gameObject.AddComponent<Button>();
            }

            button.targetGraphic = image;

            var layoutElement = buttonTransform.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = buttonTransform.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minWidth = 170f;
            layoutElement.preferredWidth = 170f;

            var label = buttonTransform.Find("Button Text") as RectTransform;
            if (label == null)
            {
                label = new GameObject("Button Text", typeof(RectTransform)).GetComponent<RectTransform>();
                label.SetParent(buttonTransform, false);
            }

            label.anchorMin = Vector2.zero;
            label.anchorMax = Vector2.one;
            label.offsetMin = Vector2.zero;
            label.offsetMax = Vector2.zero;

            var text = label.GetComponent<Text>();
            if (text == null)
            {
                text = label.gameObject.AddComponent<Text>();
            }

            text.text = "Validate grouping";
            text.font = GetBuiltinFont();
            text.fontSize = 16;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.10f, 0.20f, 0.32f);
            text.raycastTarget = false;

            return button;
        }

        private static Text EnsureValidationFeedbackText(Transform parent)
        {
            var feedbackTransform = parent.Find("Validation Feedback") as RectTransform;
            if (feedbackTransform == null)
            {
                feedbackTransform = new GameObject("Validation Feedback", typeof(RectTransform)).GetComponent<RectTransform>();
                feedbackTransform.SetParent(parent, false);
            }

            var layoutElement = feedbackTransform.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = feedbackTransform.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.flexibleWidth = 1f;

            var text = feedbackTransform.GetComponent<Text>();
            if (text == null)
            {
                text = feedbackTransform.gameObject.AddComponent<Text>();
            }

            text.font = GetBuiltinFont();
            text.fontSize = 16;
            text.fontStyle = FontStyle.Normal;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;

            return text;
        }

        private void ApplyValidationFeedback(Act1IntentClassificationFeedback feedback)
        {
            if (validationFeedbackText == null)
            {
                return;
            }

            validationFeedbackText.text = feedback.Message;

            switch (feedback.Kind)
            {
                case Act1IntentClassificationFeedbackKind.Correct:
                    validationFeedbackText.color = FeedbackCorrectColor;
                    validationFeedbackText.fontSize = 17;
                    validationFeedbackText.fontStyle = FontStyle.Bold;
                    SetValidationPanelStyle(ValidationPanelCorrectColor, new Color(0.20f, 0.62f, 0.30f, 0.95f));
                    break;
                case Act1IntentClassificationFeedbackKind.Incorrect:
                    validationFeedbackText.color = FeedbackIncorrectColor;
                    validationFeedbackText.fontSize = 16;
                    validationFeedbackText.fontStyle = FontStyle.Normal;
                    SetValidationPanelStyle(ValidationPanelIncorrectColor, new Color(0.78f, 0.28f, 0.18f, 0.90f));
                    break;
                default:
                    validationFeedbackText.color = FeedbackDefaultColor;
                    validationFeedbackText.fontSize = 16;
                    validationFeedbackText.fontStyle = FontStyle.Normal;
                    SetValidationPanelStyle(ValidationPanelColor, CardOutlineDefaultColor);
                    break;
            }
        }

        private void SetValidationPanelStyle(Color panelColor, Color outlineColor)
        {
            if (validationPanelImage != null)
            {
                validationPanelImage.color = panelColor;
            }

            SetOutline(validationPanelOutline, outlineColor, new Vector2(1.5f, -1.5f));
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

            var layoutElement = child.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = child.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;
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
            if (root == null)
            {
                return;
            }

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

            var layoutElement = child.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = child.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;
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

        private static void SetChildActive(Transform root, string childName, bool isActive)
        {
            var child = root.Find(childName);
            if (child != null)
            {
                child.gameObject.SetActive(isActive);
            }
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
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

        private static string GetIntentDescription(string intentId)
        {
            switch (intentId)
            {
                case Act1IntentClassificationSampleData.FindItemIntentId:
                    return "These visitors all want Ghost to help find something.";
                case Act1IntentClassificationSampleData.AskLocationIntentId:
                    return "These visitors all want to know where Ghost is.";
                case Act1IntentClassificationSampleData.AskIdentityIntentId:
                    return "These visitors all want to know who Ghost is or what to call Ghost.";
                default:
                    return "These visitors all want the same kind of help from Ghost.";
            }
        }

        private static string GetIntentTitle(string intentId)
        {
            switch (intentId)
            {
                case Act1IntentClassificationSampleData.FindItemIntentId:
                    return "Purpose: find something";
                case Act1IntentClassificationSampleData.AskLocationIntentId:
                    return "Purpose: locate Ghost";
                case Act1IntentClassificationSampleData.AskIdentityIntentId:
                    return "Purpose: identify Ghost";
                default:
                    return "Purpose: shared intent";
            }
        }
    }
}
