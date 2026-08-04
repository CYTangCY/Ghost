using Ghost.Presentation.Common;
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
            var root = GhostUITheme.Panel(
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

            GhostUITheme.Label(
                "Shell Title",
                root,
                "Ghost",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink,
                70f);

            GhostUITheme.Label(
                "Shell Subtitle",
                root,
                "A narrative puzzle about helping a shy little ghost find its voice.",
                GhostUITheme.TitleSize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft,
                44f);

            var body = GhostUITheme.Panel(
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
            var nameEntryScreen = CreateNameEntryScreen(
                screenColumn,
                out var playerNameInput,
                out var confirmNameButton,
                out var accountIdentifierInput,
                out var createAccountButton,
                out var useAccountButton,
                out var accountStatusText);
            var hubScreen = CreateActHubScreen(
                screenColumn,
                out var chapter0Button,
                out var act1Button,
                out var act2Button,
                out var act3Button,
                out var act4Button,
                out var act5Button,
                out var act6Button,
                out var finalChapterButton,
                out var narrativeContinueButton,
                out var backToTitleButton);
            nameEntryScreen.SetActive(false);
            hubScreen.SetActive(false);

            CreatePresencePanel(presenceColumn);
            var dialogueFrame = CreateLilyDialogueFrame(root);
            var startButton = titleScreen.GetComponentInChildren<Button>();

            presenter.Configure(
                titleScreen,
                nameEntryScreen,
                hubScreen,
                dialogueFrame,
                startButton,
                playerNameInput,
                confirmNameButton,
                accountIdentifierInput,
                createAccountButton,
                useAccountButton,
                accountStatusText,
                act1Button,
                act2Button,
                act3Button,
                act4Button,
                act5Button,
                act6Button,
                narrativeContinueButton,
                backToTitleButton,
                chapter0Button,
                finalChapterButton);

            dialogueFrame.Show(ShellDialogueData.GetLine(ShellDialogueData.TitleScreenId));
            EditorUtility.SetDirty(presenter);
            EditorUtility.SetDirty(dialogueFrame);
        }

        private static GameObject CreateTitleScreen(Transform parent)
        {
            var screen = GhostUITheme.Panel(
                "Title Screen",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.99f, 0.96f)).gameObject;

            ConfigureScreenLayout(screen, 26f);

            GhostUITheme.Label("Title Screen Heading", screen.transform, "Welcome to Ghost", GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 62f);
            GhostUITheme.Label(
                "Title Screen Copy",
                screen.transform,
                "Help a cute ghost understand messages by repairing little chatbot and NLP puzzles.",
                GhostUITheme.TitleSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft,
                86f);

            CreateButton("Start Button", screen.transform, "Start / Continue", 240f, 58f);
            return screen;
        }

        private static GameObject CreateNameEntryScreen(
            Transform parent,
            out InputField playerNameInput,
            out Button confirmNameButton,
            out InputField accountIdentifierInput,
            out Button createAccountButton,
            out Button useAccountButton,
            out Text accountStatusText)
        {
            var screen = GhostUITheme.Panel(
                "Name Entry Screen",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.99f, 0.96f)).gameObject;

            ConfigureScreenLayout(screen, 18f);

            GhostUITheme.Label("Name Entry Heading", screen.transform, "What should Ghost call you?", GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 58f);
            GhostUITheme.Label(
                "Name Entry Copy",
                screen.transform,
                "Lily writes your name on a slightly haunted lab clipboard. Leave it blank to use Junior.",
                GhostUITheme.TitleSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft,
                76f);

            var accountChoiceRow = CreateSplitRow("Account Choice Row", screen.transform, 262f);
            var guestPanel = CreateCompactSubPanel(
                "Guest Name Panel",
                accountChoiceRow,
                "Display name",
                new Color(1f, 0.985f, 0.94f));
            var accountPanel = CreateCompactSubPanel(
                "Account Recovery Panel",
                accountChoiceRow,
                "Optional account",
                new Color(0.94f, 0.98f, 1f));

            GhostUITheme.Label(
                "Guest Name Copy",
                guestPanel,
                "This is what Lily and Ghost call you. Guest mode still works offline.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.34f, 0.28f, 0.42f),
                44f);
            playerNameInput = CreateInputField("Player Name Input", guestPanel, "Junior", 340f, 46f);
            confirmNameButton = CreateButton("Confirm Name Button", guestPanel, "Continue as Guest", 230f, 42f);

            GhostUITheme.Label(
                "Account Copy",
                accountPanel,
                "No password yet. Create a username, or enter an existing username/account id to recover progress.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.34f, 0.28f, 0.42f),
                52f);
            accountIdentifierInput = CreateInputField("Account Identifier Input", accountPanel, "username or account id", 360f, 46f);
            var accountButtonRow = CreateButtonRow("Account Button Row", accountPanel, 46f);
            createAccountButton = CreateButton("Create Account Button", accountButtonRow, "Create Account", 168f, 40f);
            useAccountButton = CreateButton("Use Account Button", accountButtonRow, "Use Account", 146f, 40f);
            accountStatusText = GhostUITheme.Label(
                "Account Status Text",
                accountPanel,
                "Optional: continue as guest, create an account, or use an existing one.",
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.35f, 0.30f, 0.42f),
                42f);
            return screen;
        }

        private static GameObject CreateActHubScreen(
            Transform parent,
            out Button chapter0Button,
            out Button act1Button,
            out Button act2Button,
            out Button act3Button,
            out Button act4Button,
            out Button act5Button,
            out Button act6Button,
            out Button finalChapterButton,
            out Button narrativeContinueButton,
            out Button backToTitleButton)
        {
            var screen = GhostUITheme.Panel(
                "Act Hub Screen",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.98f, 0.99f, 1f)).gameObject;

            ConfigureHubScreenLayout(screen);

            GhostUITheme.Label("Hub Heading", screen.transform, "Chapter Select", GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 44f);
            GhostUITheme.Label(
                "Hub Copy",
                screen.transform,
                "Chapter 0 is the opening story. Chapters 1-6 are lessons. The Final Chapter combines every repair and ends Ghost's story.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.30f, 0.40f),
                40f);

            CreateStoryRouteRow(screen.transform, out chapter0Button, out finalChapterButton);

            var actCardGrid = CreateActCardGrid(screen.transform);
            var firstRow = CreateActCardRow("Act Card Row 1", actCardGrid);
            var secondRow = CreateActCardRow("Act Card Row 2", actCardGrid);

            act1Button = CreateActCard(
                firstRow,
                "Act 1 Card",
                "Chapter 1: Intent",
                "Sort messages by purpose so Ghost can tell what each visitor wants.",
                "Start Act 1 Button",
                "Start Chapter 1",
                new Color(0.92f, 0.97f, 1f));

            act2Button = CreateActCard(
                firstRow,
                "Act 2 Card",
                "Chapter 2: Entities",
                "Tag the place, object, or time details Ghost needs for a useful answer.",
                "Start Act 2 Button",
                "Start Chapter 2",
                new Color(0.93f, 1f, 0.96f));

            act3Button = CreateActCard(
                firstRow,
                "Act 3 Card",
                "Chapter 3: Dialog",
                "Wire a reply map that answers known details and asks for missing ones.",
                "Start Act 3 Button",
                "Start Chapter 3",
                new Color(1f, 0.965f, 0.88f));

            act4Button = CreateActCard(
                secondRow,
                "Act 4 Card",
                "Chapter 4: Confidence",
                "Tune answer, fallback, and Lily handoff routes for uncertain messages.",
                "Start Act 4 Button",
                "Start Chapter 4",
                new Color(0.94f, 0.96f, 1f));

            act5Button = CreateActCard(
                secondRow,
                "Act 5 Card",
                "Chapter 5: Testing",
                "Run every test conversation and repair expected-versus-actual mismatches.",
                "Start Act 5 Button",
                "Start Chapter 5",
                new Color(0.92f, 0.98f, 0.93f));

            act6Button = CreateActCard(
                secondRow,
                "Act 6 Card",
                "Chapter 6: Backend Reply",
                "Choose a data source, backend action, and response template; then run the reply.",
                "Start Act 6 Button",
                "Start Chapter 6",
                new Color(0.91f, 0.98f, 0.96f));

            narrativeContinueButton = CreateButton("Narrative Continue Button", screen.transform, "Continue to Chapter", 240f, 40f);
            narrativeContinueButton.gameObject.SetActive(false);

            backToTitleButton = CreateButton("Back To Title Button", screen.transform, "Back to Title", 190f, 40f);
            return screen;
        }

        private static void CreateStoryRouteRow(
            Transform parent,
            out Button chapter0Button,
            out Button finalChapterButton)
        {
            var row = GhostUITheme.Panel(
                "Story Route",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.91f, 0.96f, 0.98f));

            var element = row.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 60f;
            element.preferredHeight = 60f;

            var outline = row.gameObject.GetComponent<Outline>() ?? row.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.55f, 0.65f, 0.68f, 0.70f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 7, 7);
            layout.spacing = 12f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var routeLabel = GhostUITheme.Label(
                "Story Route Label",
                row,
                "Story route: meet Ghost, then return for the capstone and ending.",
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.14f, 0.25f, 0.29f),
                36f);
            routeLabel.GetComponent<LayoutElement>().flexibleWidth = 1f;

            chapter0Button = CreateButton(
                "Replay Chapter 0 Button",
                row,
                "Replay Chapter 0",
                190f,
                36f);
            finalChapterButton = CreateButton(
                "Start Final Chapter Button",
                row,
                "Final Chapter",
                180f,
                36f);
        }

        private static Transform CreateActCardGrid(Transform parent)
        {
            var grid = new GameObject("Act Card Grid", typeof(RectTransform)).transform;
            grid.SetParent(parent, false);

            var element = grid.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 288f;
            element.preferredHeight = 288f;

            var layout = grid.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            return grid;
        }

        private static Transform CreateActCardRow(string name, Transform parent)
        {
            var row = new GameObject(name, typeof(RectTransform)).transform;
            row.SetParent(parent, false);

            var layoutElement = row.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 140f;
            layoutElement.preferredHeight = 140f;
            layoutElement.flexibleHeight = 1f;

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            return row;
        }
        private static Button CreateActCard(
            Transform parent,
            string cardName,
            string title,
            string description,
            string buttonName,
            string buttonText,
            Color color)
        {
            var card = GhostUITheme.Card(
                cardName,
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                color);

            var cardLayoutElement = card.gameObject.AddComponent<LayoutElement>();
            cardLayoutElement.minHeight = 140f;
            cardLayoutElement.preferredHeight = 140f;
            cardLayoutElement.flexibleWidth = 1f;

            var cardLayout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            cardLayout.padding = new RectOffset(10, 10, 8, 8);
            cardLayout.spacing = 4f;
            cardLayout.childControlWidth = true;
            cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = true;
            cardLayout.childForceExpandHeight = false;

            GhostUITheme.Label(cardName + " Title", card, title, GhostUITheme.HeadingSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 32f);
            GhostUITheme.Label(
                cardName + " Description",
                card,
                description,
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.31f, 0.40f),
                36f);

            return CreateButton(buttonName, card, buttonText, 170f, 34f);
        }

        private static Transform CreateDynamicButtonRoot(string name, Transform parent, float height, bool horizontal)
        {
            var root = new GameObject(name, typeof(RectTransform)).transform;
            root.SetParent(parent, false);

            var layoutElement = root.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = height;
            layoutElement.preferredHeight = height;

            if (horizontal)
            {
                var layout = root.gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.spacing = 10f;
                layout.childControlWidth = true;
                layout.childControlHeight = true;
                layout.childForceExpandWidth = true;
                layout.childForceExpandHeight = false;
            }
            else
            {
                var layout = root.gameObject.AddComponent<VerticalLayoutGroup>();
                layout.spacing = 8f;
                layout.childControlWidth = true;
                layout.childControlHeight = true;
                layout.childForceExpandWidth = true;
                layout.childForceExpandHeight = false;
            }

            return root;
        }

        private static void CreatePresencePanel(Transform parent)
        {
            GhostUITheme.Label("Presence Heading", parent, "Companions", GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 44f);
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
            var card = GhostUITheme.Card(name, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, color);
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

            GhostUITheme.Label(title + " Label", card, title, GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 36f);
            GhostUITheme.Label(title + " Description", card, description, GhostUITheme.HeadingSize, FontStyle.Normal, TextAnchor.UpperLeft, GhostUITheme.InkSoft, 78f);
        }

        private static LilyDialogueFrame CreateLilyDialogueFrame(Transform parent)
        {
            var frame = GhostUITheme.Panel(
                "Lily Dialogue Frame",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.98f, 0.91f));

            var layoutElement = frame.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 178f;
            layoutElement.preferredHeight = 178f;

            var layout = frame.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 18, 18);
            layout.spacing = 16f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var portrait = GhostUITheme.Card(
                "Speaker Portrait Frame",
                frame,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.96f, 0.88f));

            var portraitLayoutElement = portrait.gameObject.AddComponent<LayoutElement>();
            portraitLayoutElement.minWidth = 118f;
            portraitLayoutElement.preferredWidth = 118f;
            portraitLayoutElement.minHeight = 118f;
            portraitLayoutElement.preferredHeight = 118f;

            var portraitOutline = portrait.gameObject.GetComponent<Outline>() ?? portrait.gameObject.AddComponent<Outline>();
            portraitOutline.effectColor = new Color(0.66f, 0.58f, 0.78f, 0.7f);
            portraitOutline.effectDistance = new Vector2(2f, -2f);

            var portraitPlaceholder = GhostUITheme.Label(
                "Portrait Placeholder",
                portrait,
                "Lily",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                GhostUITheme.Ink, new Vector2(18f, 0f));
            portraitPlaceholder.rectTransform.offsetMin = Vector2.zero;
            portraitPlaceholder.rectTransform.offsetMax = Vector2.zero;

            var textColumn = new GameObject("Dialogue Text Column", typeof(RectTransform)).GetComponent<RectTransform>();
            textColumn.SetParent(frame, false);

            var textColumnLayoutElement = textColumn.gameObject.AddComponent<LayoutElement>();
            textColumnLayoutElement.flexibleWidth = 1f;
            textColumnLayoutElement.minHeight = 118f;

            var textColumnLayout = textColumn.gameObject.AddComponent<VerticalLayoutGroup>();
            textColumnLayout.spacing = 8f;
            textColumnLayout.childControlWidth = true;
            textColumnLayout.childControlHeight = true;
            textColumnLayout.childForceExpandWidth = true;
            textColumnLayout.childForceExpandHeight = false;

            var speaker = GhostUITheme.Label("Lily Speaker Name", textColumn, "Lily", GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink, 34f);
            var dialogue = GhostUITheme.Label("Lily Dialogue Text", textColumn, string.Empty, GhostUITheme.TitleSize, FontStyle.Normal, TextAnchor.UpperLeft, GhostUITheme.InkSoft, 86f);

            var dialogueFrame = frame.gameObject.AddComponent<LilyDialogueFrame>();
            dialogueFrame.Configure(speaker, dialogue, portrait.GetComponent<Image>(), portraitPlaceholder);
            return dialogueFrame;
        }

        private static RectTransform CreateColumnPanel(string name, Transform parent, float flexibleWidth, Color color)
        {
            var panel = GhostUITheme.Panel(name, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, color);

            var image = panel.GetComponent<Image>();
            image.color = color;

            var outline = panel.gameObject.GetComponent<Outline>() ?? panel.gameObject.AddComponent<Outline>();
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

        private static void ConfigureHubScreenLayout(GameObject screen)
        {
            var layoutElement = screen.AddComponent<LayoutElement>();
            layoutElement.flexibleHeight = 1f;

            var layout = screen.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(26, 26, 12, 12);
            layout.spacing = 5f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
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

        private static InputField CreateInputField(string name, Transform parent, string placeholder, float width, float height)
        {
            var inputTransform = GhostUITheme.Panel(
                name,
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 1f, 1f));

            var image = inputTransform.GetComponent<Image>();
            image.raycastTarget = true;

            var outline = inputTransform.gameObject.GetComponent<Outline>() ?? inputTransform.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.48f, 0.54f, 0.76f, 0.78f);
            outline.effectDistance = new Vector2(2f, -2f);

            var layoutElement = inputTransform.gameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = width;
            layoutElement.preferredWidth = width;
            layoutElement.minHeight = height;
            layoutElement.preferredHeight = height;

            var placeholderText = GhostUITheme.Label(
                "Placeholder",
                inputTransform,
                placeholder,
                GhostUITheme.TitleSize,
                FontStyle.Italic,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft, new Vector2(18f, 0f));

            var inputText = GhostUITheme.Label(
                "Text",
                inputTransform,
                string.Empty,
                GhostUITheme.TitleSize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink, new Vector2(18f, 0f));

            var input = inputTransform.gameObject.AddComponent<InputField>();
            input.targetGraphic = image;
            input.textComponent = inputText;
            input.placeholder = placeholderText;
            input.characterLimit = 24;
            input.lineType = InputField.LineType.SingleLine;

            return input;
        }

        private static Button CreateButton(string name, Transform parent, string label, float width, float height)
        {
            var button = GhostUITheme.PushButton(name, parent, label);
            button.targetGraphic.color = new Color(0.84f, 0.92f, 1f);
            var buttonLabel = button.GetComponentInChildren<Text>();
            GhostUITheme.Label(
                buttonLabel,
                label,
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                GhostUITheme.Ink);

            var layout = button.gameObject.AddComponent<LayoutElement>();
            var contentWidth = Mathf.Ceil(buttonLabel.preferredWidth + 32f);
            var resolvedWidth = Mathf.Max(width, contentWidth);
            var resolvedHeight = Mathf.Max(44f, height);
            layout.minWidth = resolvedWidth;
            layout.preferredWidth = resolvedWidth;
            layout.minHeight = resolvedHeight;
            layout.preferredHeight = resolvedHeight;
            return button;
        }
        private static Transform CreateButtonRow(string name, Transform parent, float height)
        {
            var row = new GameObject(name, typeof(RectTransform)).transform;
            row.SetParent(parent, false);

            var layoutElement = row.gameObject.AddComponent<LayoutElement>();
            var resolvedHeight = Mathf.Max(44f, height);
            layoutElement.minHeight = resolvedHeight;
            layoutElement.preferredHeight = resolvedHeight;

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            return row;
        }

        private static Transform CreateSplitRow(string name, Transform parent, float height)
        {
            var row = new GameObject(name, typeof(RectTransform)).transform;
            row.SetParent(parent, false);

            var layoutElement = row.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = height;
            layoutElement.preferredHeight = height;

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 16f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            return row;
        }

        private static Transform CreateCompactSubPanel(string name, Transform parent, string heading, Color color)
        {
            var panel = GhostUITheme.Card(
                name,
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                color);

            var outline = panel.gameObject.GetComponent<Outline>() ?? panel.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.70f, 0.68f, 0.86f, 0.72f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var layoutElement = panel.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 12, 12);
            layout.spacing = 7f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                name + " Heading",
                panel,
                heading,
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink,
                28f);
            return panel;
        }

        private static void RegisterBuildSettingsScenes()
        {
            var scenes = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene(ShellSceneNames.GameShellScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.Chapter0ScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.Act1ScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.Act2ScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.Act3ScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.Act4ScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.Act5ScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.Act6ScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.FinalChapterScenePath, true)
            };

            foreach (var existingScene in EditorBuildSettings.scenes)
            {
                if (existingScene.path == ShellSceneNames.GameShellScenePath ||
                    existingScene.path == ShellSceneNames.Chapter0ScenePath ||
                    existingScene.path == ShellSceneNames.Act1ScenePath ||
                    existingScene.path == ShellSceneNames.Act2ScenePath ||
                    existingScene.path == ShellSceneNames.Act3ScenePath ||
                    existingScene.path == ShellSceneNames.Act4ScenePath ||
                    existingScene.path == ShellSceneNames.Act5ScenePath ||
                    existingScene.path == ShellSceneNames.Act6ScenePath ||
                    existingScene.path == ShellSceneNames.FinalChapterScenePath)
                {
                    continue;
                }

                scenes.Add(existingScene);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
