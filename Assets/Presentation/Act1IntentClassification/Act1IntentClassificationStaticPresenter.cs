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
        private const float CardPreferredHeight = 72f;
        private const float MessageTextPreferredHeight = 48f;
        private const float GroupPreferredHeight = 200f;
        private const float AssignmentViewportPreferredHeight = 72f;
        private const float AssignedRowPreferredHeight = 32f;
        private const float AssignedPlaceholderPreferredHeight = 22f;
        private const float ValidationControlsPreferredHeight = 58f;

        private static readonly Color CardDefaultColor = new Color(1f, 0.98f, 0.92f);
        private static readonly Color CardSelectedColor = new Color(1f, 0.89f, 0.45f);
        private static readonly Color CardAssignedColor = new Color(0.91f, 1f, 0.89f);
        private static readonly Color GroupDefaultColor = new Color(0.91f, 0.96f, 1f);
        private static readonly Color GroupReadyColor = new Color(0.84f, 0.92f, 1f);
        private static readonly Color FeedbackDefaultColor = new Color(0.24f, 0.22f, 0.30f);
        private static readonly Color FeedbackCorrectColor = new Color(0.08f, 0.42f, 0.18f);
        private static readonly Color FeedbackIncorrectColor = new Color(0.62f, 0.16f, 0.13f);
        private static readonly Color AssignedRowColor = new Color(0.97f, 0.99f, 1f);

        [SerializeField] private RectTransform cardListRoot;
        [SerializeField] private RectTransform intentGroupListRoot;
        [SerializeField] private GameObject cardTemplate;
        [SerializeField] private GameObject intentGroupTemplate;
        [SerializeField] private bool renderOnStart = true;

        private readonly Dictionary<string, IntentCard> cardsById = new Dictionary<string, IntentCard>();
        private readonly Dictionary<string, GameObject> cardViewsById = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, Image> cardImagesById = new Dictionary<string, Image>();
        private readonly Dictionary<string, Image> groupImagesById = new Dictionary<string, Image>();
        private readonly Dictionary<string, RectTransform> assignmentRootsByIntentId = new Dictionary<string, RectTransform>();

        private Act1IntentClassificationInteractionController controller;
        private Text validationFeedbackText;

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
            ClearRenderedState();

            var cards = Act1IntentClassificationSampleData.CreateCards();
            controller = new Act1IntentClassificationInteractionController(cards);
            controller.StateChanged += UpdateVisualState;
            controller.FeedbackChanged += ApplyValidationFeedback;

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
            groupImagesById.Clear();
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
        }

        private void CreateIntentGroupView(string intentId)
        {
            var view = Instantiate(intentGroupTemplate, intentGroupListRoot);
            view.name = "Intent Group - " + intentId;
            view.SetActive(true);

            ConfigureIntentGroupContainer(view, intentId);
            SetChildText(view.transform, "IntentTitleText", intentId);
            SetChildText(view.transform, "IntentHintText", GetIntentDescription(intentId));

            var assignmentRoot = EnsureAssignmentRoot(view.transform);
            ConfigureAssignmentViewportClick(assignmentRoot, intentId);
            assignmentRootsByIntentId.Add(intentId, assignmentRoot);
            groupImagesById.Add(intentId, view.GetComponent<Image>());
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

                if (controller != null && cardId == controller.SelectedCardId)
                {
                    image.color = CardSelectedColor;
                }
                else if (controller != null && controller.GetAssignedGroupId(cardId) != null)
                {
                    image.color = CardAssignedColor;
                }
                else
                {
                    image.color = CardDefaultColor;
                }
            }
        }

        private void UpdateIntentGroupVisuals()
        {
            foreach (var intentId in IntentIds)
            {
                if (groupImagesById.TryGetValue(intentId, out var groupImage) && groupImage != null)
                {
                    groupImage.color = controller == null || !controller.HasSelectedCard
                        ? GroupDefaultColor
                        : GroupReadyColor;
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
                    CreateAssignedText(assignmentRoot, "No cards assigned yet.", true);
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

            var button = view.GetComponent<Button>();
            if (button == null)
            {
                button = view.AddComponent<Button>();
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => controller?.SelectCard(cardId));
            button.targetGraphic = image;

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

            layout.padding = new RectOffset(18, 18, 10, 10);
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

            var button = view.GetComponent<Button>();
            if (button == null)
            {
                button = view.AddComponent<Button>();
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => controller?.AssignSelectedCardToIntent(intentId));
            button.targetGraphic = image;

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

            layout.padding = new RectOffset(18, 18, 14, 12);
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
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
            text.fontSize = 20;
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

            image.color = new Color(1f, 1f, 1f, 0.32f);
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

            var button = row.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => controller?.MoveAssignedCardToUnassigned(cardId));

            var layoutElement = row.AddComponent<LayoutElement>();
            layoutElement.minHeight = AssignedRowPreferredHeight;
            layoutElement.preferredHeight = AssignedRowPreferredHeight;

            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 2, 2);
            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            var label = new GameObject("Assigned Card Text", typeof(RectTransform)).AddComponent<Text>();
            label.transform.SetParent(row.transform, false);
            label.text = "Back: " + message;
            label.font = GetBuiltinFont();
            label.fontSize = 12;
            label.fontStyle = FontStyle.Normal;
            label.alignment = TextAnchor.MiddleLeft;
            label.color = new Color(0.12f, 0.20f, 0.30f);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.raycastTarget = false;
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

            layoutElement.minWidth = 138f;
            layoutElement.preferredWidth = 138f;

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

            text.text = "Validate";
            text.font = GetBuiltinFont();
            text.fontSize = 18;
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
                    break;
                case Act1IntentClassificationFeedbackKind.Incorrect:
                    validationFeedbackText.color = FeedbackIncorrectColor;
                    break;
                default:
                    validationFeedbackText.color = FeedbackDefaultColor;
                    break;
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
                    return "Messages asking Ghost to help find something.";
                case Act1IntentClassificationSampleData.AskLocationIntentId:
                    return "Messages asking where Ghost is.";
                case Act1IntentClassificationSampleData.AskIdentityIntentId:
                    return "Messages asking who Ghost is.";
                default:
                    return "Messages with the same purpose belong together.";
            }
        }
    }
}
