using Ghost.Presentation.Common;
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
            var root = GhostUITheme.Panel(
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

            GhostUITheme.Label(
                "Title",
                root,
                "Act 3: Ghost's Reply Map",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink,
                68f);

            GhostUITheme.Label(
                "Subtitle",
                root,
                "Add simple cards, move them around, then connect them so Ghost knows when to answer or ask for the room.",
                GhostUITheme.TitleSize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft,
                70f);

            var body = GhostUITheme.Panel(
                "Prototype Body",
                root,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 1f, 1f, 0f));

            var bodyLayoutElement = body.gameObject.AddComponent<LayoutElement>();
            bodyLayoutElement.minHeight = 96f;
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

            GhostUITheme.Label("Palette Panel Title", palettePanel, "Palette", GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 42f);
            var paletteViewport = CreateScrollViewport("Node Palette Viewport", palettePanel);
            var paletteRoot = CreateListRoot("Node Palette List", paletteViewport, 6f);
            AttachScroll(paletteViewport, paletteRoot);

            GhostUITheme.Label("Graph Panel Title", graphPanel, "Reply Map", GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 42f);
            var graphCanvasRoot = CreateGraphCanvasRoot(graphPanel);
            var validationRoot = CreateValidationRoot(graphPanel);

            GhostUITheme.Label("Goal Panel Title", goalPanel, "Guide", GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 42f);
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
            var panel = GhostUITheme.Panel(
                name,
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                color);

            var outline = panel.gameObject.GetComponent<Outline>();
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

        /// <summary>
        /// A clipped, scrollable viewport. The palette now holds cards for both visitors, which needs
        /// roughly 868px in a column about 430px tall - without this, over half the cards existed but
        /// could never be reached.
        /// </summary>
        private static RectTransform CreateScrollViewport(string name, Transform parent)
        {
            var viewport = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            viewport.SetParent(parent, false);

            var element = viewport.gameObject.AddComponent<LayoutElement>();
            element.flexibleHeight = 1f;
            element.minHeight = 200f;

            viewport.gameObject.AddComponent<RectMask2D>();
            return viewport;
        }

        private static void AttachScroll(RectTransform viewport, RectTransform content)
        {
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;

            // The content must size itself to its children, otherwise the ScrollRect has nothing to
            // scroll through.
            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var listElement = content.GetComponent<LayoutElement>();
            if (listElement != null)
            {
                listElement.flexibleHeight = 0f;
            }

            var scroll = viewport.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 24f;
            scroll.inertia = true;
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
            var root = GhostUITheme.Panel(
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
            var root = GhostUITheme.Panel(
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
            var item = GhostUITheme.Card(
                "Palette Item Template",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.92f, 0.97f, 1f)).gameObject;

            GhostUITheme.Label("PaletteItemTitle", item.transform, "Node", GhostUITheme.HeadingSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 24f);
            GhostUITheme.Label("PaletteItemDetail", item.transform, "Description", GhostUITheme.SmallSize, FontStyle.Normal, TextAnchor.UpperLeft, GhostUITheme.InkSoft, 44f);
            return item;
        }

        private static GameObject CreateTestCaseTemplate(Transform parent)
        {
            var item = GhostUITheme.Card(
                "Test Case Template",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.985f, 0.92f)).gameObject;

            GhostUITheme.Label("TestCaseTitle", item.transform, "test-case-id", GhostUITheme.BodySize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 24f);
            GhostUITheme.Label("TestCaseDetail", item.transform, "intent + entity -> response", GhostUITheme.SmallSize, FontStyle.Normal, TextAnchor.MiddleLeft, GhostUITheme.InkSoft, 34f);
            return item;
        }

    }
}
