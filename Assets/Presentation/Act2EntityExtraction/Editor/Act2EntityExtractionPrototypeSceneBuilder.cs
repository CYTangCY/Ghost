using Ghost.Presentation.Act2EntityExtraction;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Act2.Editor
{
    public static class Act2EntityExtractionPrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Act2EntityExtractionPrototype.unity";

        [MenuItem("Ghost/Build Act 2 Entity Extraction Prototype Scene")]
        public static void BuildAct2EntityExtractionPrototypeScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            var canvas = CreateCanvas();
            CreateEventSystem();
            CreateStaticUi(canvas.transform);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.10f, 0.09f, 0.14f);
            camera.orthographic = true;
            cameraObject.tag = "MainCamera";
        }

        private static Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("Canvas", typeof(RectTransform));
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static void CreateEventSystem()
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static void CreateStaticUi(Transform canvasTransform)
        {
            var root = CreatePanel(
                "Act 2 Entity Extraction Prototype",
                canvasTransform,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.96f, 0.94f, 1f));

            var rootLayout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(48, 48, 34, 40);
            rootLayout.spacing = 22f;
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = true;
            rootLayout.childForceExpandHeight = false;

            var presenter = root.gameObject.AddComponent<Act2EntityExtractionStaticPresenter>();

            CreateLabel(
                "Title",
                root,
                "Act 2: Entity Extraction",
                44,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.16f, 0.10f, 0.24f),
                76f);

            CreateLabel(
                "Subtitle",
                root,
                "Display-only prototype: message chips, entity type palette, and placeholder validation.",
                22,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.30f, 0.24f, 0.38f),
                58f);

            var body = CreatePanel(
                "Prototype Body",
                root,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 1f, 1f, 0f));

            var bodyLayoutElement = body.gameObject.AddComponent<LayoutElement>();
            bodyLayoutElement.flexibleHeight = 1f;

            var bodyLayout = body.gameObject.AddComponent<HorizontalLayoutGroup>();
            bodyLayout.spacing = 28f;
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = true;
            bodyLayout.childForceExpandHeight = true;

            var messagePanel = CreateColumnPanel("Message Panel", body, 0.62f, new Color(1f, 0.985f, 0.94f));
            var palettePanel = CreateColumnPanel("Palette Panel", body, 0.38f, new Color(0.93f, 0.98f, 1f));

            CreateLabel(
                "Message Panel Title",
                messagePanel,
                "Message Word Chips",
                28,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.16f, 0.10f, 0.24f),
                44f);

            var chipRoot = CreateChipRoot(messagePanel);
            var validationRoot = CreateValidationRoot(messagePanel);

            CreateLabel(
                "Palette Panel Title",
                palettePanel,
                "Entity Types",
                28,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.12f, 0.17f, 0.28f),
                44f);

            var paletteRoot = CreateListRoot("Entity Type List", palettePanel, 12f);

            var templates = new GameObject("Templates", typeof(RectTransform));
            templates.transform.SetParent(root, false);
            templates.SetActive(false);

            var chipTemplate = CreateChipTemplate(templates.transform);
            var entityTypeTemplate = CreateEntityTypeTemplate(templates.transform);

            presenter.Configure(
                chipRoot,
                paletteRoot,
                validationRoot,
                chipTemplate,
                entityTypeTemplate,
                true);

            presenter.RenderSampleData();
            EditorUtility.SetDirty(presenter);
        }

        private static RectTransform CreateColumnPanel(string name, Transform parent, float flexibleWidth, Color color)
        {
            var panel = CreatePanel(
                name,
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                color);

            var outline = panel.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.70f, 0.68f, 0.86f, 0.75f);
            outline.effectDistance = new Vector2(2f, -2f);

            var layoutElement = panel.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = flexibleWidth;
            layoutElement.flexibleHeight = 1f;

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 22, 22);
            layout.spacing = 18f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            return panel;
        }

        private static RectTransform CreateChipRoot(Transform parent)
        {
            var root = new GameObject("Message Chip Flow", typeof(RectTransform)).GetComponent<RectTransform>();
            root.SetParent(parent, false);

            var layoutElement = root.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleHeight = 1f;

            var layout = root.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(170f, 58f);
            layout.spacing = new Vector2(10f, 10f);
            layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            layout.startAxis = GridLayoutGroup.Axis.Horizontal;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 4;

            return root;
        }

        private static RectTransform CreateValidationRoot(Transform parent)
        {
            var root = CreatePanel(
                "Validation Controls",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.99f, 0.94f));

            var layoutElement = root.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 82f;
            layoutElement.preferredHeight = 82f;

            return root;
        }

        private static RectTransform CreateListRoot(string name, Transform parent, float spacing)
        {
            var root = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            root.SetParent(parent, false);

            var layoutElement = root.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleHeight = 1f;

            var layout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            return root;
        }

        private static GameObject CreateChipTemplate(Transform parent)
        {
            var chip = CreatePanel(
                "Word Chip Template",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.985f, 0.92f)).gameObject;

            var layout = chip.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 8, 8);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateLabel("ChipText", chip.transform, "word", 20, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.13f, 0.10f, 0.20f), 40f);
            chip.AddComponent<Act2EntityChipView>();
            return chip;
        }

        private static GameObject CreateEntityTypeTemplate(Transform parent)
        {
            var item = CreatePanel(
                "Entity Type Template",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.92f, 0.97f, 1f)).gameObject;

            var layout = item.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 10, 10);
            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateLabel("EntityTypeText", item.transform, "entity_id", 24, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.10f, 0.18f, 0.30f), 34f);
            CreateLabel("EntityCategoryText", item.transform, "Category", 18, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.28f, 0.34f, 0.44f), 28f);
            return item;
        }

        private static RectTransform CreatePanel(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax,
            Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            panel.SetParent(parent, false);
            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
            panel.offsetMin = offsetMin;
            panel.offsetMax = offsetMax;

            var image = panel.gameObject.AddComponent<Image>();
            image.color = color;

            return panel;
        }

        private static Text CreateLabel(
            string name,
            Transform parent,
            string text,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color,
            float preferredHeight)
        {
            var label = new GameObject(name, typeof(RectTransform)).AddComponent<Text>();
            label.transform.SetParent(parent, false);
            label.text = text;
            label.font = GetBuiltinFont();
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.alignment = alignment;
            label.color = color;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.raycastTarget = false;

            var layoutElement = label.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;

            return label;
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
