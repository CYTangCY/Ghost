using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Shell.Editor
{
    public static class GameShellSceneBuilder
    {
        [MenuItem("Ghost/Build Game Shell Scene")]
        public static void BuildGameShellScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            var canvas = CreateCanvas();
            CreateEventSystem();
            CreateShellUi(canvas.transform);

            EditorSceneManager.SaveScene(scene, ShellSceneNames.GameShellScenePath);
            RegisterBuildSettingsScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Ghost/Register Game Shell Build Settings")]
        public static void RegisterGameShellBuildSettings()
        {
            RegisterBuildSettingsScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.12f, 0.10f, 0.18f);
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

        private static void CreateShellUi(Transform canvasTransform)
        {
            var root = CreatePanel(
                "Game Shell Root",
                canvasTransform,
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.96f, 0.94f, 1f));

            var rootLayout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(44, 44, 34, 36);
            rootLayout.spacing = 18f;
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = true;
            rootLayout.childForceExpandHeight = false;

            var presenter = root.gameObject.AddComponent<GameShellPresenter>();

            CreateLabel(
                "Shell Title",
                root,
                "Ghost",
                52,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.16f, 0.10f, 0.24f),
                70f);

            CreateLabel(
                "Shell Subtitle",
                root,
                "A placeholder shell for Lily's guidance, Ghost's presence, and act selection.",
                22,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.32f, 0.25f, 0.42f),
                44f);

            var body = CreatePanel(
                "Shell Body",
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
            bodyLayout.childForceExpandWidth = true;
            bodyLayout.childForceExpandHeight = true;

            var screenColumn = CreateColumnPanel("Screen Panel", body, 0.68f, new Color(1f, 0.985f, 0.94f));
            var presenceColumn = CreateColumnPanel("Presence Panel", body, 0.32f, new Color(0.92f, 0.97f, 1f));

            var titleScreen = CreateTitleScreen(screenColumn);
            var hubScreen = CreateActHubScreen(screenColumn, out var act1Button, out var backToTitleButton);
            hubScreen.SetActive(false);

            CreatePresencePanel(presenceColumn);
            var dialogueFrame = CreateLilyDialogueFrame(root);
            var startButton = titleScreen.GetComponentInChildren<Button>();

            presenter.Configure(
                titleScreen,
                hubScreen,
                dialogueFrame,
                startButton,
                act1Button,
                backToTitleButton);

            dialogueFrame.Show(ShellDialogueData.GetLine(ShellDialogueData.TitleScreenId));
            EditorUtility.SetDirty(presenter);
            EditorUtility.SetDirty(dialogueFrame);
        }

        private static GameObject CreateTitleScreen(Transform parent)
        {
            var screen = CreatePanel(
                "Title Screen",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.99f, 0.96f)).gameObject;

            ConfigureScreenLayout(screen, 26f);

            CreateLabel("Title Screen Heading", screen.transform, "Welcome to Ghost", 42, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.17f, 0.11f, 0.25f), 62f);
            CreateLabel(
                "Title Screen Copy",
                screen.transform,
                "Help a cute ghost understand messages by repairing little chatbot and NLP puzzles.",
                24,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.28f, 0.22f, 0.36f),
                86f);

            CreateButton("Start Button", screen.transform, "Start / Continue", 240f, 58f);
            return screen;
        }

        private static GameObject CreateActHubScreen(Transform parent, out Button act1Button, out Button backToTitleButton)
        {
            var screen = CreatePanel(
                "Act Hub Screen",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.98f, 0.99f, 1f)).gameObject;

            ConfigureScreenLayout(screen, 18f);

            CreateLabel("Hub Heading", screen.transform, "Act Select", 40, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.12f, 0.17f, 0.28f), 58f);
            CreateLabel(
                "Hub Copy",
                screen.transform,
                "Choose a prototype act. More acts will unlock as their mechanics are built.",
                22,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.30f, 0.40f),
                62f);

            var act1Card = CreatePanel(
                "Act 1 Card",
                screen.transform,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.92f, 0.97f, 1f));

            var cardLayoutElement = act1Card.gameObject.AddComponent<LayoutElement>();
            cardLayoutElement.minHeight = 178f;
            cardLayoutElement.preferredHeight = 178f;

            var cardLayout = act1Card.gameObject.AddComponent<VerticalLayoutGroup>();
            cardLayout.padding = new RectOffset(18, 18, 16, 16);
            cardLayout.spacing = 10f;
            cardLayout.childControlWidth = true;
            cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = true;
            cardLayout.childForceExpandHeight = false;

            CreateLabel("Act 1 Title", act1Card, "Act 1: Intent Classification", 26, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.10f, 0.20f, 0.32f), 36f);
            CreateLabel(
                "Act 1 Description",
                act1Card,
                "Group different messages by the same purpose so Ghost reacts to what the speaker wants.",
                19,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.31f, 0.40f),
                54f);
            act1Button = CreateButton("Start Act 1 Button", act1Card, "Start Act 1", 190f, 46f);

            var futureCard = CreatePanel(
                "Future Acts Placeholder",
                screen.transform,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.95f, 0.95f, 0.98f));

            var futureLayoutElement = futureCard.gameObject.AddComponent<LayoutElement>();
            futureLayoutElement.minHeight = 88f;
            futureLayoutElement.preferredHeight = 88f;

            var futureLayout = futureCard.gameObject.AddComponent<VerticalLayoutGroup>();
            futureLayout.padding = new RectOffset(18, 18, 12, 12);
            futureLayout.childControlWidth = true;
            futureLayout.childControlHeight = true;

            CreateLabel(
                "Future Acts Text",
                futureCard,
                "Act 2 and later: placeholder locks for future puzzle prototypes.",
                19,
                FontStyle.Italic,
                TextAnchor.MiddleLeft,
                new Color(0.42f, 0.42f, 0.50f),
                54f);

            backToTitleButton = CreateButton("Back To Title Button", screen.transform, "Back to Title", 190f, 46f);
            return screen;
        }

        private static void CreatePresencePanel(Transform parent)
        {
            CreateLabel("Presence Heading", parent, "Companions", 28, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.12f, 0.17f, 0.28f), 44f);
            CreateCharacterPlaceholder(
                parent,
                "Ghost Placeholder",
                "Ghost",
                "Cute ghost placeholder\nWaiting for clearer messages.",
                new Color(1f, 1f, 1f, 0.78f));
            CreateCharacterPlaceholder(
                parent,
                "Lily Placeholder",
                "Lily",
                "Lab senior placeholder\nNervous, kind, and very prepared.",
                new Color(1f, 0.98f, 0.93f, 0.86f));
        }

        private static void CreateCharacterPlaceholder(
            Transform parent,
            string name,
            string title,
            string description,
            Color color)
        {
            var card = CreatePanel(name, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, color);
            var layoutElement = card.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 150f;
            layoutElement.preferredHeight = 150f;

            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 14, 14);
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateLabel(title + " Label", card, title, 28, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.15f, 0.13f, 0.24f), 42f);
            CreateLabel(title + " Description", card, description, 18, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.30f, 0.27f, 0.38f), 72f);
        }

        private static LilyDialogueFrame CreateLilyDialogueFrame(Transform parent)
        {
            var frame = CreatePanel(
                "Lily Dialogue Frame",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.98f, 0.91f));

            var layoutElement = frame.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 172f;
            layoutElement.preferredHeight = 172f;

            var layout = frame.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 18, 18);
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var speaker = CreateLabel("Lily Speaker Name", frame, "Lily", 24, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.20f, 0.13f, 0.28f), 34f);
            var dialogue = CreateLabel("Lily Dialogue Text", frame, string.Empty, 22, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.28f, 0.22f, 0.36f), 86f);

            var dialogueFrame = frame.gameObject.AddComponent<LilyDialogueFrame>();
            dialogueFrame.Configure(speaker, dialogue);
            return dialogueFrame;
        }

        private static RectTransform CreateColumnPanel(string name, Transform parent, float flexibleWidth, Color color)
        {
            var panel = CreatePanel(name, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, color);

            var image = panel.GetComponent<Image>();
            image.color = color;

            var outline = panel.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.70f, 0.68f, 0.86f, 0.75f);
            outline.effectDistance = new Vector2(2f, -2f);

            var layoutElement = panel.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = flexibleWidth;
            layoutElement.flexibleHeight = 1f;

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 22, 22);
            layout.spacing = 16f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            return panel;
        }

        private static void ConfigureScreenLayout(GameObject screen, float spacing)
        {
            var layoutElement = screen.AddComponent<LayoutElement>();
            layoutElement.flexibleHeight = 1f;

            var layout = screen.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(26, 26, 24, 24);
            layout.spacing = spacing;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private static Button CreateButton(string name, Transform parent, string label, float width, float height)
        {
            var buttonTransform = CreatePanel(
                name,
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.84f, 0.92f, 1f));

            var image = buttonTransform.GetComponent<Image>();
            image.raycastTarget = true;

            var outline = buttonTransform.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.48f, 0.54f, 0.76f, 0.78f);
            outline.effectDistance = new Vector2(2f, -2f);

            var layoutElement = buttonTransform.gameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = width;
            layoutElement.preferredWidth = width;
            layoutElement.minHeight = height;
            layoutElement.preferredHeight = height;

            var button = buttonTransform.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var labelTransform = new GameObject("Button Text", typeof(RectTransform)).GetComponent<RectTransform>();
            labelTransform.SetParent(buttonTransform, false);
            labelTransform.anchorMin = Vector2.zero;
            labelTransform.anchorMax = Vector2.one;
            labelTransform.offsetMin = Vector2.zero;
            labelTransform.offsetMax = Vector2.zero;

            var text = labelTransform.gameObject.AddComponent<Text>();
            text.text = label;
            text.font = GetBuiltinFont();
            text.fontSize = 20;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.10f, 0.18f, 0.30f);
            text.raycastTarget = false;

            return button;
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

        private static void RegisterBuildSettingsScenes()
        {
            var scenes = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene(ShellSceneNames.GameShellScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.Act1ScenePath, true)
            };

            foreach (var existingScene in EditorBuildSettings.scenes)
            {
                if (existingScene.path == ShellSceneNames.GameShellScenePath ||
                    existingScene.path == ShellSceneNames.Act1ScenePath)
                {
                    continue;
                }

                scenes.Add(existingScene);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
