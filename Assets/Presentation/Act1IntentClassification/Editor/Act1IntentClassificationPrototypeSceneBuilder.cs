using Ghost.Presentation.Act1IntentClassification;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Act1IntentClassification.Editor
{
    public static class Act1IntentClassificationPrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Act1IntentClassificationPrototype.unity";

        [MenuItem("Ghost/Build Act 1 Intent Classification Prototype Scene")]
        public static void BuildAct1IntentClassificationPrototypeScene()
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
            camera.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
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
                "Act 1 Intent Classification Prototype",
                canvasTransform,
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Color(0.96f, 0.94f, 1f));

            var rootLayout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(48, 48, 32, 40);
            rootLayout.spacing = 24f;
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = true;
            rootLayout.childForceExpandHeight = false;

            var presenter = root.gameObject.AddComponent<Act1IntentClassificationStaticPresenter>();

            CreateLabel(
                "Title",
                root,
                "Act 1: Intent Classification",
                44,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.12f, 0.28f),
                80f);

            CreateLabel(
                "Subtitle",
                root,
                "Group messages by what the speaker wants, not by exact wording.",
                25,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.27f, 0.22f, 0.36f),
                56f);

            var body = CreatePanel(
                "Prototype Body",
                root,
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Color(1f, 1f, 1f, 0f));

            var bodyLayoutElement = body.gameObject.AddComponent<LayoutElement>();
            bodyLayoutElement.flexibleHeight = 1f;

            var bodyLayout = body.gameObject.AddComponent<HorizontalLayoutGroup>();
            bodyLayout.spacing = 28f;
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = true;
            bodyLayout.childForceExpandHeight = true;

            var cardsPanel = CreateColumnPanel("Sample Message Cards", body, 0.54f);
            var groupsPanel = CreateColumnPanel("Intent Group Areas", body, 0.46f);

            var cardsRoot = CreateListRoot("Cards List", cardsPanel, 8f);
            var groupsRoot = CreateListRoot("Intent Groups List", groupsPanel, 12f);

            var templates = new GameObject("Templates", typeof(RectTransform));
            templates.transform.SetParent(root, false);
            templates.SetActive(false);

            var cardTemplate = CreateCardTemplate(templates.transform);
            var groupTemplate = CreateIntentGroupTemplate(templates.transform);

            SetPrivateField(presenter, "cardListRoot", cardsRoot);
            SetPrivateField(presenter, "intentGroupListRoot", groupsRoot);
            SetPrivateField(presenter, "cardTemplate", cardTemplate);
            SetPrivateField(presenter, "intentGroupTemplate", groupTemplate);
            SetPrivateField(presenter, "renderOnStart", true);

            presenter.RenderSampleData();
            EditorUtility.SetDirty(presenter);
        }

        private static RectTransform CreateColumnPanel(string title, Transform parent, float flexibleWidth)
        {
            var panel = CreatePanel(
                title + " Panel",
                parent,
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Color(0.98f, 0.98f, 1f));

            var image = panel.GetComponent<Image>();
            image.color = new Color(0.98f, 0.98f, 1f);

            var outline = panel.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.75f, 0.70f, 0.88f);
            outline.effectDistance = new Vector2(2f, -2f);

            var layoutElement = panel.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = flexibleWidth;
            layoutElement.flexibleHeight = 1f;

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 24, 24);
            layout.spacing = 18f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateLabel(
                title,
                panel,
                title,
                28,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.12f, 0.28f),
                44f);

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

        private static GameObject CreateCardTemplate(Transform parent)
        {
            var card = CreatePanel(
                "Card Template",
                parent,
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Color(1f, 0.98f, 0.92f)).gameObject;

            var layoutElement = card.AddComponent<LayoutElement>();
            layoutElement.minHeight = 72f;
            layoutElement.preferredHeight = 72f;

            var layout = card.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 10, 10);
            layout.spacing = 0f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateLabel("MessageText", card.transform, "Message", 20, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.12f, 0.09f, 0.18f), 48f);

            return card;
        }

        private static GameObject CreateIntentGroupTemplate(Transform parent)
        {
            var group = CreatePanel(
                "Intent Group Template",
                parent,
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Color(0.91f, 0.96f, 1f)).gameObject;

            var layoutElement = group.AddComponent<LayoutElement>();
            layoutElement.minHeight = 200f;
            layoutElement.preferredHeight = 200f;

            group.AddComponent<RectMask2D>();

            var layout = group.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 14, 12);
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateLabel("IntentTitleText", group.transform, "intent_id", 25, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.10f, 0.20f, 0.32f), 42f);
            CreateLabel("IntentHintText", group.transform, "Purpose description", 18, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.28f, 0.34f, 0.44f), 52f);
            CreateAssignmentScrollView(group.transform);

            return group;
        }

        private static void CreateAssignmentScrollView(Transform parent)
        {
            var viewport = new GameObject("AssignedCardsScrollView", typeof(RectTransform)).GetComponent<RectTransform>();
            viewport.SetParent(parent, false);

            var layoutElement = viewport.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 72f;
            layoutElement.preferredHeight = 72f;
            layoutElement.flexibleHeight = 1f;

            var image = viewport.gameObject.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.32f);
            image.raycastTarget = true;

            viewport.gameObject.AddComponent<RectMask2D>();

            var assignmentRoot = new GameObject("AssignedCardsRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            assignmentRoot.SetParent(viewport, false);
            assignmentRoot.anchorMin = new Vector2(0f, 1f);
            assignmentRoot.anchorMax = new Vector2(1f, 1f);
            assignmentRoot.pivot = new Vector2(0.5f, 1f);
            assignmentRoot.anchoredPosition = Vector2.zero;
            assignmentRoot.sizeDelta = Vector2.zero;

            var layout = assignmentRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = assignmentRoot.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scrollRect = viewport.gameObject.AddComponent<ScrollRect>();
            scrollRect.viewport = viewport;
            scrollRect.content = assignmentRoot;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.inertia = true;

            var button = viewport.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
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

        private static void SetPrivateField<T>(Act1IntentClassificationStaticPresenter presenter, string fieldName, T value)
        {
            var field = typeof(Act1IntentClassificationStaticPresenter).GetField(
                fieldName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            field.SetValue(presenter, value);
        }
    }
}
