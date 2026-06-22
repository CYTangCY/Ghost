using System.Collections.Generic;
using Ghost.Puzzles.EntityExtraction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Ghost.Presentation.Act2EntityExtraction
{
    public sealed class Act2EntityExtractionStaticPresenter : MonoBehaviour
    {
        private const float ChipPreferredHeight = 56f;
        private const float PaletteItemPreferredHeight = 78f;
        private const float ValidationControlsPreferredHeight = 82f;

        private const string TitleText = "Act 2: Entity Extraction";
        private const string InstructionText =
            "Display-only span annotation prototype.\n" +
            "The message is split into word chips with stored character offsets for a later interaction task.";
        private const string PlaceholderFeedbackText =
            "Validation placeholder only. Span selection and type assignment arrive in a later task.";

        private static readonly Color ChipColor = new Color(1f, 0.985f, 0.92f);
        private static readonly Color ChipOutlineColor = new Color(0.78f, 0.70f, 0.88f, 0.64f);
        private static readonly Color SystemEntityColor = new Color(0.86f, 0.94f, 1f);
        private static readonly Color CustomEntityColor = new Color(0.90f, 1f, 0.92f);
        private static readonly Color EntityOutlineColor = new Color(0.56f, 0.66f, 0.82f, 0.78f);
        private static readonly Color ValidationPanelColor = new Color(1f, 0.99f, 0.94f, 0.92f);

        [SerializeField] private RectTransform messageChipRoot;
        [SerializeField] private RectTransform entityPaletteRoot;
        [SerializeField] private RectTransform validationControlsRoot;
        [SerializeField] private GameObject chipTemplate;
        [SerializeField] private GameObject entityTypeTemplate;
        [SerializeField] private bool renderOnStart = true;

        private Text validationFeedbackText;

        private void Start()
        {
            if (renderOnStart)
            {
                RenderSampleData();
            }
        }

        public void Configure(
            RectTransform messageChipRoot,
            RectTransform entityPaletteRoot,
            RectTransform validationControlsRoot,
            GameObject chipTemplate,
            GameObject entityTypeTemplate,
            bool renderOnStart)
        {
            this.messageChipRoot = messageChipRoot;
            this.entityPaletteRoot = entityPaletteRoot;
            this.validationControlsRoot = validationControlsRoot;
            this.chipTemplate = chipTemplate;
            this.entityTypeTemplate = entityTypeTemplate;
            this.renderOnStart = renderOnStart;
        }

        public void RenderSampleData()
        {
            if (messageChipRoot == null ||
                entityPaletteRoot == null ||
                validationControlsRoot == null ||
                chipTemplate == null ||
                entityTypeTemplate == null)
            {
                return;
            }

            EnsureEventSystem();
            EnsureInstructionText();
            ClearChildren(messageChipRoot);
            ClearChildren(entityPaletteRoot);
            ClearChildren(validationControlsRoot);

            var sampleMessage = Act2EntityExtractionSampleData.CreateMessages()[0];
            var session = EntityExtractionSession.CreateFromSampleMessage(sampleMessage);
            RenderMessageChips(session.MessageText);
            RenderEntityPalette();
            RenderValidationPlaceholder();

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageChipRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(entityPaletteRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(validationControlsRoot);
        }

        private void RenderMessageChips(string messageText)
        {
            foreach (var token in CreateWordTokens(messageText))
            {
                var chip = Instantiate(chipTemplate, messageChipRoot);
                chip.name = $"Chip - {token.Text} [{token.Start},{token.Length}]";
                chip.SetActive(true);

                ConfigureChipContainer(chip);
                SetChildText(chip.transform, "ChipText", token.Text);

                var chipView = chip.GetComponent<Act2EntityChipView>();
                if (chipView == null)
                {
                    chipView = chip.AddComponent<Act2EntityChipView>();
                }

                chipView.Configure(token.Start, token.Length, token.Text);
            }
        }

        private void RenderEntityPalette()
        {
            CreateEntityTypeView(Act2EntityExtractionSampleData.CreateTimeEntityType());
            CreateEntityTypeView(Act2EntityExtractionSampleData.CreateRoomEntityType());
            CreateEntityTypeView(Act2EntityExtractionSampleData.CreateObjectEntityType());
        }

        private void CreateEntityTypeView(EntityType entityType)
        {
            var view = Instantiate(entityTypeTemplate, entityPaletteRoot);
            view.name = $"Entity Type - {entityType.Id}";
            view.SetActive(true);

            ConfigureEntityTypeContainer(view, entityType.Category);
            SetChildText(view.transform, "EntityTypeText", entityType.Id);
            SetChildText(view.transform, "EntityCategoryText", entityType.Category.ToString());
        }

        private void RenderValidationPlaceholder()
        {
            ConfigureValidationContainer(validationControlsRoot.gameObject);
            var validateButton = CreateValidatePlaceholderButton(validationControlsRoot);
            validateButton.onClick.RemoveAllListeners();
            validateButton.interactable = false;

            validationFeedbackText = CreateValidationFeedbackText(validationControlsRoot);
            validationFeedbackText.text = PlaceholderFeedbackText;
        }

        private void EnsureInstructionText()
        {
            ConfigureExistingLabel(
                transform,
                "Title",
                TitleText,
                42,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.16f, 0.10f, 0.24f),
                70f);

            ConfigureExistingLabel(
                transform,
                "Subtitle",
                InstructionText,
                20,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.28f, 0.22f, 0.36f),
                76f);

            ConfigureExistingLabel(
                messageChipRoot.parent,
                "Message Panel Title",
                "Message Word Chips",
                28,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.16f, 0.10f, 0.24f),
                44f);

            ConfigureExistingLabel(
                entityPaletteRoot.parent,
                "Palette Panel Title",
                "Entity Types",
                28,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.12f, 0.17f, 0.28f),
                44f);
        }

        private static IEnumerable<WordToken> CreateWordTokens(string messageText)
        {
            if (string.IsNullOrEmpty(messageText))
            {
                yield break;
            }

            var index = 0;
            while (index < messageText.Length)
            {
                while (index < messageText.Length && char.IsWhiteSpace(messageText[index]))
                {
                    index++;
                }

                if (index >= messageText.Length)
                {
                    yield break;
                }

                var rawStart = index;
                while (index < messageText.Length && !char.IsWhiteSpace(messageText[index]))
                {
                    index++;
                }

                var rawEnd = index - 1;
                var start = rawStart;
                var end = rawEnd;

                while (start <= end && !char.IsLetterOrDigit(messageText[start]))
                {
                    start++;
                }

                while (end >= start && !char.IsLetterOrDigit(messageText[end]))
                {
                    end--;
                }

                if (start > end)
                {
                    continue;
                }

                var length = end - start + 1;
                yield return new WordToken(start, length, messageText.Substring(start, length));
            }
        }

        private static void ConfigureChipContainer(GameObject chip)
        {
            var image = chip.GetComponent<Image>();
            if (image == null)
            {
                image = chip.AddComponent<Image>();
            }

            image.color = ChipColor;
            image.raycastTarget = false;

            var outline = chip.GetComponent<Outline>();
            if (outline == null)
            {
                outline = chip.AddComponent<Outline>();
            }

            outline.effectColor = ChipOutlineColor;
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var layoutElement = chip.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = chip.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = ChipPreferredHeight;
            layoutElement.preferredHeight = ChipPreferredHeight;

            var layout = chip.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = chip.AddComponent<HorizontalLayoutGroup>();
            }

            layout.padding = new RectOffset(16, 16, 8, 8);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
        }

        private static void ConfigureEntityTypeContainer(GameObject view, EntityCategory category)
        {
            var image = view.GetComponent<Image>();
            if (image == null)
            {
                image = view.AddComponent<Image>();
            }

            image.color = category == EntityCategory.System ? SystemEntityColor : CustomEntityColor;
            image.raycastTarget = false;

            var outline = view.GetComponent<Outline>();
            if (outline == null)
            {
                outline = view.AddComponent<Outline>();
            }

            outline.effectColor = EntityOutlineColor;
            outline.effectDistance = new Vector2(2f, -2f);

            var layoutElement = view.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = view.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = PaletteItemPreferredHeight;
            layoutElement.preferredHeight = PaletteItemPreferredHeight;

            var layout = view.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = view.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(16, 16, 10, 10);
            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private static void ConfigureValidationContainer(GameObject view)
        {
            var image = view.GetComponent<Image>();
            if (image == null)
            {
                image = view.AddComponent<Image>();
            }

            image.color = ValidationPanelColor;
            image.raycastTarget = false;

            var layoutElement = view.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = view.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = ValidationControlsPreferredHeight;
            layoutElement.preferredHeight = ValidationControlsPreferredHeight;

            var layout = view.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = view.AddComponent<HorizontalLayoutGroup>();
            }

            layout.padding = new RectOffset(12, 12, 8, 8);
            layout.spacing = 12f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
        }

        private static Button CreateValidatePlaceholderButton(Transform parent)
        {
            var buttonRoot = new GameObject("Validate Placeholder Button", typeof(RectTransform));
            buttonRoot.transform.SetParent(parent, false);

            var image = buttonRoot.AddComponent<Image>();
            image.color = new Color(0.84f, 0.92f, 1f);
            image.raycastTarget = true;

            var button = buttonRoot.AddComponent<Button>();
            button.targetGraphic = image;

            var outline = buttonRoot.AddComponent<Outline>();
            outline.effectColor = new Color(0.48f, 0.54f, 0.76f, 0.70f);
            outline.effectDistance = new Vector2(2f, -2f);

            var layoutElement = buttonRoot.AddComponent<LayoutElement>();
            layoutElement.minWidth = 190f;
            layoutElement.preferredWidth = 190f;

            var label = new GameObject("Button Text", typeof(RectTransform)).GetComponent<RectTransform>();
            label.SetParent(buttonRoot.transform, false);
            label.anchorMin = Vector2.zero;
            label.anchorMax = Vector2.one;
            label.offsetMin = Vector2.zero;
            label.offsetMax = Vector2.zero;

            var text = label.gameObject.AddComponent<Text>();
            text.text = "Validate spans";
            text.font = GetBuiltinFont();
            text.fontSize = 17;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.12f, 0.18f, 0.30f);
            text.raycastTarget = false;

            return button;
        }

        private static Text CreateValidationFeedbackText(Transform parent)
        {
            var feedback = new GameObject("Validation Feedback", typeof(RectTransform));
            feedback.transform.SetParent(parent, false);

            var layoutElement = feedback.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;

            var text = feedback.AddComponent<Text>();
            text.font = GetBuiltinFont();
            text.fontSize = 16;
            text.fontStyle = FontStyle.Italic;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = new Color(0.36f, 0.31f, 0.42f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;

            return text;
        }

        private static void SetChildText(Transform root, string childName, string value)
        {
            var child = root.Find(childName);
            if (child == null)
            {
                return;
            }

            var text = child.GetComponent<Text>();
            if (text == null)
            {
                return;
            }

            text.text = value;
            text.raycastTarget = false;
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

        private readonly struct WordToken
        {
            public WordToken(int start, int length, string text)
            {
                Start = start;
                Length = length;
                Text = text;
            }

            public int Start { get; }

            public int Length { get; }

            public string Text { get; }
        }
    }
}
