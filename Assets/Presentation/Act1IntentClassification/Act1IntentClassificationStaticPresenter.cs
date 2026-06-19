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
        private const float GroupPreferredHeight = 216f;
        private const float AssignmentRootPreferredHeight = 68f;
        private const float AssignedTextPreferredHeight = 18f;
        private const float AssignedPlaceholderPreferredHeight = 22f;

        private static readonly Color CardDefaultColor = new Color(1f, 0.98f, 0.92f);
        private static readonly Color CardSelectedColor = new Color(1f, 0.89f, 0.45f);
        private static readonly Color CardAssignedColor = new Color(0.91f, 1f, 0.89f);
        private static readonly Color GroupDefaultColor = new Color(0.91f, 0.96f, 1f);
        private static readonly Color GroupReadyColor = new Color(0.84f, 0.92f, 1f);

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

        private IntentClassificationSession session;
        private string selectedCardId;

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
            session = new IntentClassificationSession(cards);

            foreach (var card in cards)
            {
                cardsById.Add(card.Id, card);
                CreateCardView(card);
            }

            foreach (var intentId in IntentIds)
            {
                CreateIntentGroupView(intentId);
            }

            UpdateVisualState();
        }

        private void ClearRenderedState()
        {
            selectedCardId = null;
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
            assignmentRootsByIntentId.Add(intentId, assignmentRoot);
            groupImagesById.Add(intentId, view.GetComponent<Image>());
        }

        private void SelectCard(string cardId)
        {
            selectedCardId = selectedCardId == cardId ? null : cardId;
            UpdateVisualState();
        }

        private void AssignSelectedCardToIntent(string intentId)
        {
            if (string.IsNullOrEmpty(selectedCardId) || session == null)
            {
                return;
            }

            session.MoveCardToGroup(selectedCardId, intentId);
            selectedCardId = null;
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            UpdateCardVisuals();
            UpdateIntentGroupVisuals();
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(cardListRoot);
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

                if (cardId == selectedCardId)
                {
                    image.color = CardSelectedColor;
                }
                else if (session != null && session.GetAssignedGroupId(cardId) != null)
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
                    groupImage.color = string.IsNullOrEmpty(selectedCardId) ? GroupDefaultColor : GroupReadyColor;
                }

                if (!assignmentRootsByIntentId.TryGetValue(intentId, out var assignmentRoot))
                {
                    continue;
                }

                ClearChildren(assignmentRoot);
                IReadOnlyList<string> assignedCardIds = session == null
                    ? new string[0]
                    : session.GetAssignedCardIds(intentId);

                if (assignedCardIds.Count == 0)
                {
                    CreateAssignedText(assignmentRoot, "No cards assigned yet.", true);
                    continue;
                }

                foreach (var cardId in assignedCardIds)
                {
                    if (cardsById.TryGetValue(cardId, out var card))
                    {
                        CreateAssignedText(assignmentRoot, card.MessageText, false);
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
            button.onClick.AddListener(() => SelectCard(cardId));
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
            button.onClick.AddListener(() => AssignSelectedCardToIntent(intentId));
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
            var existing = groupRoot.Find("AssignedCardsRoot");
            if (existing != null)
            {
                var existingRoot = (RectTransform)existing;
                ConfigureAssignmentRoot(existingRoot);
                return existingRoot;
            }

            var assignmentRoot = new GameObject("AssignedCardsRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            assignmentRoot.SetParent(groupRoot, false);

            ConfigureAssignmentRoot(assignmentRoot);

            return assignmentRoot;
        }

        private static void ConfigureAssignmentRoot(RectTransform assignmentRoot)
        {
            var layoutElement = assignmentRoot.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = assignmentRoot.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = AssignmentRootPreferredHeight;
            layoutElement.preferredHeight = AssignmentRootPreferredHeight;
            layoutElement.flexibleHeight = 1f;

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
            var preferredHeight = isPlaceholder ? AssignedPlaceholderPreferredHeight : AssignedTextPreferredHeight;
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;
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
