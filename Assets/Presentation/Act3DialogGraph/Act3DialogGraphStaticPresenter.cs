using System;
using System.Collections.Generic;
using System.Text;
using Ghost.Puzzles.DialogGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Ghost.Presentation.Act3DialogGraph
{
    public sealed class Act3DialogGraphStaticPresenter : MonoBehaviour
    {
        private const float PaletteItemPreferredHeight = 58f;
        private const float TestCasePreferredHeight = 74f;
        private const float ValidationControlsPreferredHeight = 86f;

        private const string TitleText = "Act 3: Dialog Graph";
        private const string InstructionText =
            "Display-only prototype for Ghost's conversation map: available node types, level vocabulary, an empty graph canvas, and target test conversations.";
        private const string PlaceholderFeedbackText =
            "Validation is a placeholder in this build.";

        private static readonly Color RootTextColor = new Color(0.15f, 0.12f, 0.22f);
        private static readonly Color SecondaryTextColor = new Color(0.30f, 0.28f, 0.38f);
        private static readonly Color NodeTypeColor = new Color(0.92f, 0.97f, 1f);
        private static readonly Color VocabularyColor = new Color(0.95f, 1f, 0.94f);
        private static readonly Color GoalColor = new Color(1f, 0.985f, 0.92f);
        private static readonly Color CanvasColor = new Color(0.96f, 0.96f, 1f);
        private static readonly Color ValidationColor = new Color(1f, 0.99f, 0.94f);
        private static readonly Color OutlineColor = new Color(0.62f, 0.60f, 0.78f, 0.72f);

        [SerializeField] private RectTransform nodePaletteRoot;
        [SerializeField] private RectTransform graphCanvasRoot;
        [SerializeField] private RectTransform goalTestRoot;
        [SerializeField] private RectTransform validationControlsRoot;
        [SerializeField] private GameObject paletteItemTemplate;
        [SerializeField] private GameObject testCaseTemplate;
        [SerializeField] private bool renderOnStart = true;

        private DialogGraphSession session;

        private void Start()
        {
            if (renderOnStart)
            {
                RenderSampleData();
            }
        }

        public void Configure(
            RectTransform nodePaletteRoot,
            RectTransform graphCanvasRoot,
            RectTransform goalTestRoot,
            RectTransform validationControlsRoot,
            GameObject paletteItemTemplate,
            GameObject testCaseTemplate,
            bool renderOnStart)
        {
            this.nodePaletteRoot = nodePaletteRoot;
            this.graphCanvasRoot = graphCanvasRoot;
            this.goalTestRoot = goalTestRoot;
            this.validationControlsRoot = validationControlsRoot;
            this.paletteItemTemplate = paletteItemTemplate;
            this.testCaseTemplate = testCaseTemplate;
            this.renderOnStart = renderOnStart;
        }

        public void RenderSampleData()
        {
            if (nodePaletteRoot == null ||
                graphCanvasRoot == null ||
                goalTestRoot == null ||
                validationControlsRoot == null ||
                paletteItemTemplate == null ||
                testCaseTemplate == null)
            {
                return;
            }

            EnsureEventSystem();
            EnsureInstructionText();
            ClearChildren(nodePaletteRoot);
            ClearChildren(graphCanvasRoot);
            ClearChildren(goalTestRoot);
            ClearChildren(validationControlsRoot);

            session = DialogGraphSession.CreateFromSampleData();

            RenderNodePalette();
            RenderGraphCanvasPlaceholder();
            RenderGoalTestCases();
            RenderValidationControls();

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodePaletteRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(graphCanvasRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(goalTestRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(validationControlsRoot);
        }

        private void RenderNodePalette()
        {
            CreateSectionLabel(nodePaletteRoot, "Node Types");

            foreach (DialogNodeType nodeType in Enum.GetValues(typeof(DialogNodeType)))
            {
                CreatePaletteItem(
                    nodeType.ToString(),
                    GetNodeTypeDescription(nodeType),
                    NodeTypeColor);
            }

            CreateSectionLabel(nodePaletteRoot, "Level Vocabulary");
            CreatePaletteItem("Intent", Act3DialogGraphSampleData.FindObjectIntentId, VocabularyColor);
            CreatePaletteItem("Entity slot", Act3DialogGraphSampleData.RoomEntityTypeId, VocabularyColor);
            CreatePaletteItem("Response", Act3DialogGraphSampleData.AnswerObjectLocationResponseId, VocabularyColor);
            CreatePaletteItem("Response", Act3DialogGraphSampleData.AskForRoomResponseId, VocabularyColor);
        }

        private void CreatePaletteItem(string title, string detail, Color color)
        {
            var item = Instantiate(paletteItemTemplate, nodePaletteRoot);
            item.name = $"Palette Item - {title}";
            item.SetActive(true);
            ConfigureListItem(item, color, PaletteItemPreferredHeight);
            SetChildText(item.transform, "PaletteItemTitle", title);
            SetChildText(item.transform, "PaletteItemDetail", detail);
        }

        private void RenderGraphCanvasPlaceholder()
        {
            ConfigurePanelSurface(graphCanvasRoot.gameObject, CanvasColor, true);

            CreateCanvasLabel(
                "Graph Canvas Placeholder Title",
                graphCanvasRoot,
                "Graph Canvas",
                34,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                RootTextColor,
                54f);

            CreateCanvasLabel(
                "Graph Canvas Placeholder Detail",
                graphCanvasRoot,
                "Empty for M0-T23. Nodes and connections are added in later Act 3 tasks.",
                21,
                FontStyle.Italic,
                TextAnchor.UpperCenter,
                SecondaryTextColor,
                92f);
        }

        private void RenderGoalTestCases()
        {
            if (session == null)
            {
                return;
            }

            CreateSectionLabel(goalTestRoot, "Target Conversations");

            foreach (var testCase in session.TestCases)
            {
                var view = Instantiate(testCaseTemplate, goalTestRoot);
                view.name = $"Test Case - {testCase.Id}";
                view.SetActive(true);
                ConfigureListItem(view, GoalColor, TestCasePreferredHeight);
                SetChildText(view.transform, "TestCaseTitle", string.IsNullOrWhiteSpace(testCase.Id) ? "Test case" : testCase.Id);
                SetChildText(view.transform, "TestCaseDetail", FormatTestCase(testCase));
            }
        }

        private void RenderValidationControls()
        {
            ConfigurePanelSurface(validationControlsRoot.gameObject, ValidationColor, false);

            var validateButton = CreateValidateButton(validationControlsRoot);
            validateButton.interactable = false;
            validateButton.onClick.RemoveAllListeners();

            var feedbackText = CreateValidationFeedbackText(validationControlsRoot);
            feedbackText.text = PlaceholderFeedbackText;
        }

        private static string FormatTestCase(DialogGraphTestCase testCase)
        {
            var builder = new StringBuilder();
            builder.Append(testCase.Turn.IntentId);

            if (testCase.Turn.Entities.Count == 0)
            {
                builder.Append(" (no ");
                builder.Append(Act3DialogGraphSampleData.RoomEntityTypeId);
                builder.Append(")");
            }
            else
            {
                builder.Append(" + ");
                builder.Append(FormatEntities(testCase.Turn.Entities));
            }

            builder.Append(" -> ");
            builder.Append(testCase.ExpectedResponseId);
            return builder.ToString();
        }

        private static string FormatEntities(IReadOnlyDictionary<string, string> entities)
        {
            var builder = new StringBuilder();
            var isFirst = true;

            foreach (var entity in entities)
            {
                if (!isFirst)
                {
                    builder.Append(", ");
                }

                builder.Append(entity.Key);
                builder.Append("=");
                builder.Append(entity.Value);
                isFirst = false;
            }

            return builder.ToString();
        }

        private static string GetNodeTypeDescription(DialogNodeType nodeType)
        {
            switch (nodeType)
            {
                case DialogNodeType.Start:
                    return "Conversation entry point";
                case DialogNodeType.IntentBranch:
                    return $"Routes intent {Act3DialogGraphSampleData.FindObjectIntentId}";
                case DialogNodeType.SlotCheck:
                    return $"Checks slot {Act3DialogGraphSampleData.RoomEntityTypeId}";
                case DialogNodeType.Response:
                    return "Reaches an authored response id";
                default:
                    return "Dialog node";
            }
        }

        private void EnsureInstructionText()
        {
            ConfigureExistingLabel(
                transform.Find("Title"),
                TitleText,
                44,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                RootTextColor,
                68f);

            ConfigureExistingLabel(
                transform.Find("Subtitle"),
                InstructionText,
                20,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                SecondaryTextColor,
                70f);
        }

        private static void ConfigureListItem(GameObject item, Color color, float preferredHeight)
        {
            ConfigurePanelSurface(item, color, true);

            var layoutElement = item.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = item.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;

            var layout = item.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = item.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(10, 10, 6, 6);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private static void ConfigurePanelSurface(GameObject target, Color color, bool withOutline)
        {
            var image = target.GetComponent<Image>();
            if (image == null)
            {
                image = target.AddComponent<Image>();
            }

            image.color = color;
            image.raycastTarget = false;

            if (!withOutline)
            {
                return;
            }

            var outline = target.GetComponent<Outline>();
            if (outline == null)
            {
                outline = target.AddComponent<Outline>();
            }

            outline.effectColor = OutlineColor;
            outline.effectDistance = new Vector2(2f, -2f);
        }

        private static void CreateSectionLabel(Transform parent, string value)
        {
            var label = new GameObject(value + " Label", typeof(RectTransform));
            label.transform.SetParent(parent, false);

            var layoutElement = label.AddComponent<LayoutElement>();
            layoutElement.minHeight = 28f;
            layoutElement.preferredHeight = 28f;

            var text = label.AddComponent<Text>();
            text.text = value;
            text.font = GetBuiltinFont();
            text.fontSize = 20;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = RootTextColor;
            text.raycastTarget = false;
        }

        private static void CreateCanvasLabel(
            string name,
            Transform parent,
            string value,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color,
            float preferredHeight)
        {
            var label = new GameObject(name, typeof(RectTransform));
            label.transform.SetParent(parent, false);

            var layoutElement = label.AddComponent<LayoutElement>();
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;

            var text = label.AddComponent<Text>();
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

        private static Button CreateValidateButton(Transform parent)
        {
            var buttonRoot = new GameObject("Validate Button", typeof(RectTransform));
            buttonRoot.transform.SetParent(parent, false);

            var image = buttonRoot.AddComponent<Image>();
            image.color = new Color(0.84f, 0.86f, 0.92f);
            image.raycastTarget = true;

            var button = buttonRoot.AddComponent<Button>();
            button.targetGraphic = image;

            var layoutElement = buttonRoot.AddComponent<LayoutElement>();
            layoutElement.minWidth = 190f;
            layoutElement.preferredWidth = 190f;

            var labelTransform = new GameObject("Button Text", typeof(RectTransform)).GetComponent<RectTransform>();
            labelTransform.SetParent(buttonRoot.transform, false);
            labelTransform.anchorMin = Vector2.zero;
            labelTransform.anchorMax = Vector2.one;
            labelTransform.offsetMin = Vector2.zero;
            labelTransform.offsetMax = Vector2.zero;

            var text = labelTransform.gameObject.AddComponent<Text>();
            text.text = "Validate graph";
            text.font = GetBuiltinFont();
            text.fontSize = 18;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.32f, 0.34f, 0.42f);
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
            text.color = SecondaryTextColor;
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
            Transform target,
            string value,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color,
            float preferredHeight)
        {
            if (target == null)
            {
                return;
            }

            var text = target.GetComponent<Text>();
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

            var layoutElement = target.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = target.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;
        }

        private static void ClearChildren(Transform root)
        {
            var children = new List<GameObject>();

            for (var index = 0; index < root.childCount; index++)
            {
                children.Add(root.GetChild(index).gameObject);
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
    }
}
