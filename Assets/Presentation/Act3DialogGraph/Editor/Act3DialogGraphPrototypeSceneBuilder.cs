using Ghost.Presentation.Act3DialogGraph;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Act3.Editor
{
    public static class Act3DialogGraphPrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Act3DialogGraphPrototype.unity";

        [MenuItem("Ghost/Build Act 3 Dialog Graph Prototype Scene")]
        public static void BuildAct3DialogGraphPrototypeScene()
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
                "Act 3 Dialog Graph Prototype",
                canvasTransform,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.96f, 0.94f, 1f));

            var rootLayout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(42, 42, 30, 36);
            rootLayout.spacing = 18f;
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = true;
            rootLayout.childForceExpandHeight = false;

            var presenter = root.gameObject.AddComponent<Act3DialogGraphStaticPresenter>();

            CreateLabel(
                "Title",
                root,
                "Act 3: Ghost's Reply Map",
                44,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.15f, 0.12f, 0.22f),
                68f);

            CreateLabel(
                "Subtitle",
                root,
                "Add simple cards, move them around, then connect them so Ghost knows when to answer or ask for the room.",
                20,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.30f, 0.28f, 0.38f),
                70f);

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
            bodyLayout.spacing = 24f;
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = false;
            bodyLayout.childForceExpandHeight = true;

            var palettePanel = CreateColumnPanel("Node Palette Panel", body, 125f, 125f, 0f, 8, new Color(0.93f, 0.98f, 1f));
            var graphPanel = CreateColumnPanel("Graph Canvas Panel", body, 900f, 1120f, 1f, 18, new Color(0.98f, 0.98f, 1f));
            var goalPanel = CreateColumnPanel("Goal Test Panel", body, 290f, 290f, 0f, 18, new Color(1f, 0.985f, 0.94f));

            CreateLabel("Palette Panel Title", palettePanel, "Palette", 28, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.12f, 0.17f, 0.28f), 42f);
            var paletteRoot = CreateListRoot("Node Palette List", palettePanel, 6f);

            CreateLabel("Graph Panel Title", graphPanel, "Reply Map", 28, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.15f, 0.12f, 0.22f), 42f);
            var graphCanvasRoot = CreateGraphCanvasRoot(graphPanel);
            var validationRoot = CreateValidationRoot(graphPanel);

            CreateLabel("Goal Panel Title", goalPanel, "Guide", 28, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.18f, 0.13f, 0.22f), 42f);
            var goalRoot = CreateListRoot("Goal Test List", goalPanel, 8f);

            var templates = new GameObject("Templates", typeof(RectTransform));
            templates.transform.SetParent(root, false);
            templates.SetActive(false);

            var paletteItemTemplate = CreatePaletteItemTemplate(templates.transform);
            var testCaseTemplate = CreateTestCaseTemplate(templates.transform);

            presenter.Configure(
                paletteRoot,
                graphCanvasRoot,
                goalRoot,
                validationRoot,
                paletteItemTemplate,
                testCaseTemplate,
                true);

            presenter.RenderSampleData();
            EditorUtility.SetDirty(presenter);
        }

        private static RectTransform CreateColumnPanel(
            string name,
            Transform parent,
            float minWidth,
            float preferredWidth,
            float flexibleWidth,
            int horizontalPadding,
            Color color)
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
            layoutElement.minWidth = minWidth;
            layoutElement.preferredWidth = preferredWidth;
            layoutElement.flexibleWidth = flexibleWidth;
            layoutElement.flexibleHeight = 1f;

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(horizontalPadding, horizontalPadding, 12, 12);
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            return panel;
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

        private static RectTransform CreateGraphCanvasRoot(Transform parent)
        {
            var root = CreatePanel(
                "Graph Canvas Region",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.96f, 0.96f, 1f));

            var layoutElement = root.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleHeight = 1f;

            var layout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 10, 14);
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

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
            layoutElement.minHeight = 28f;
            layoutElement.preferredHeight = 28f;
            layoutElement.flexibleHeight = 0f;

            var layout = root.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(4, 4, 3, 3);
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            return root;
        }

        private static GameObject CreatePaletteItemTemplate(Transform parent)
        {
            var item = CreatePanel(
                "Palette Item Template",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.92f, 0.97f, 1f)).gameObject;

            CreateLabel("PaletteItemTitle", item.transform, "Node", 18, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.10f, 0.18f, 0.30f), 24f);
            CreateLabel("PaletteItemDetail", item.transform, "Description", 13, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.28f, 0.34f, 0.44f), 20f);
            return item;
        }

        private static GameObject CreateTestCaseTemplate(Transform parent)
        {
            var item = CreatePanel(
                "Test Case Template",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.985f, 0.92f)).gameObject;

            CreateLabel("TestCaseTitle", item.transform, "test-case-id", 17, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.18f, 0.13f, 0.22f), 24f);
            CreateLabel("TestCaseDetail", item.transform, "intent + entity -> response", 13, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.34f, 0.28f, 0.38f), 34f);
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
