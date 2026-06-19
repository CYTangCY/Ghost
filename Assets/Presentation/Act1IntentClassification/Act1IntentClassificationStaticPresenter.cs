using System.Collections.Generic;
using Ghost.Puzzles.IntentClassification;
using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Presentation.Act1IntentClassification
{
    public sealed class Act1IntentClassificationStaticPresenter : MonoBehaviour
    {
        private const float CardPreferredHeight = 72f;
        private const float MessageTextPreferredHeight = 48f;

        [SerializeField] private RectTransform cardListRoot;
        [SerializeField] private RectTransform intentGroupListRoot;
        [SerializeField] private GameObject cardTemplate;
        [SerializeField] private GameObject intentGroupTemplate;
        [SerializeField] private bool renderOnStart = true;

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

            ClearChildren(cardListRoot);
            ClearChildren(intentGroupListRoot);

            foreach (var card in Act1IntentClassificationSampleData.CreateCards())
            {
                CreateCardView(card);
            }

            foreach (var intentId in IntentIds)
            {
                CreateIntentGroupView(intentId);
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(cardListRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(intentGroupListRoot);
        }

        private void CreateCardView(IntentCard card)
        {
            var view = Instantiate(cardTemplate, cardListRoot);
            view.name = "Card - " + card.Id;
            view.SetActive(true);

            ConfigureCardContainer(view);
            ConfigureCardMessageText(view.transform, card.MessageText);
            SetChildActive(view.transform, "CardIdText", false);
            SetChildActive(view.transform, "IntentHintText", false);
        }

        private void CreateIntentGroupView(string intentId)
        {
            var view = Instantiate(intentGroupTemplate, intentGroupListRoot);
            view.name = "Intent Group - " + intentId;
            view.SetActive(true);

            SetChildText(view.transform, "IntentTitleText", intentId);
            SetChildText(view.transform, "IntentHintText", GetIntentDescription(intentId));
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

        private static void ConfigureCardContainer(GameObject view)
        {
            var image = view.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color(1f, 0.98f, 0.92f);
            }

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
