using System.Collections.Generic;
using Ghost.Presentation.Fundamentals;
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
            var nameEntryScreen = CreateNameEntryScreen(
                screenColumn,
                out var playerNameInput,
                out var confirmNameButton,
                out var accountIdentifierInput,
                out var createAccountButton,
                out var useAccountButton,
                out var accountStatusText);
            var fundamentalsScreen = CreateFundamentalsScreen(
                screenColumn,
                out var fundamentalsPresenter);
            var hubScreen = CreateActHubScreen(
                screenColumn,
                out var fundamentalsButton,
                out var act1Button,
                out var act2Button,
                out var act3Button,
                out var narrativeContinueButton,
                out var backToTitleButton);
            nameEntryScreen.SetActive(false);
            fundamentalsScreen.SetActive(false);
            hubScreen.SetActive(false);

            CreatePresencePanel(presenceColumn);
            var dialogueFrame = CreateLilyDialogueFrame(root);
            fundamentalsPresenter.SetDialogueFrame(dialogueFrame);
            var startButton = titleScreen.GetComponentInChildren<Button>();

            presenter.Configure(
                titleScreen,
                nameEntryScreen,
                fundamentalsScreen,
                hubScreen,
                dialogueFrame,
                fundamentalsPresenter,
                startButton,
                playerNameInput,
                confirmNameButton,
                accountIdentifierInput,
                createAccountButton,
                useAccountButton,
                accountStatusText,
                fundamentalsButton,
                act1Button,
                act2Button,
                act3Button,
                narrativeContinueButton,
                backToTitleButton);

            dialogueFrame.Show(ShellDialogueData.GetLine(ShellDialogueData.TitleScreenId));
            EditorUtility.SetDirty(presenter);
            EditorUtility.SetDirty(fundamentalsPresenter);
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

        private static GameObject CreateNameEntryScreen(
            Transform parent,
            out InputField playerNameInput,
            out Button confirmNameButton,
            out InputField accountIdentifierInput,
            out Button createAccountButton,
            out Button useAccountButton,
            out Text accountStatusText)
        {
            var screen = CreatePanel(
                "Name Entry Screen",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.99f, 0.96f)).gameObject;

            ConfigureScreenLayout(screen, 18f);

            CreateLabel("Name Entry Heading", screen.transform, "What should Ghost call you?", 38, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.17f, 0.11f, 0.25f), 58f);
            CreateLabel(
                "Name Entry Copy",
                screen.transform,
                "Lily writes your name on a slightly haunted lab clipboard. Leave it blank to use Junior.",
                22,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.28f, 0.22f, 0.36f),
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

            CreateLabel(
                "Guest Name Copy",
                guestPanel,
                "This is what Lily and Ghost call you. Guest mode still works offline.",
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.34f, 0.28f, 0.42f),
                44f);
            playerNameInput = CreateInputField("Player Name Input", guestPanel, "Junior", 340f, 46f);
            confirmNameButton = CreateButton("Confirm Name Button", guestPanel, "Continue as Guest", 230f, 42f);

            CreateLabel(
                "Account Copy",
                accountPanel,
                "No password yet. Create a username, or enter an existing username/account id to recover progress.",
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.34f, 0.28f, 0.42f),
                52f);
            accountIdentifierInput = CreateInputField("Account Identifier Input", accountPanel, "username or account id", 360f, 46f);
            var accountButtonRow = CreateButtonRow("Account Button Row", accountPanel, 46f);
            createAccountButton = CreateButton("Create Account Button", accountButtonRow, "Create Account", 168f, 40f);
            useAccountButton = CreateButton("Use Account Button", accountButtonRow, "Use Account", 146f, 40f);
            accountStatusText = CreateLabel(
                "Account Status Text",
                accountPanel,
                "Optional: continue as guest, create an account, or use an existing one.",
                14,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.35f, 0.30f, 0.42f),
                42f);
            return screen;
        }

        private static GameObject CreateFundamentalsScreen(
            Transform parent,
            out ChatbotFundamentalsPresenter fundamentalsPresenter)
        {
            var screen = CreatePanel(
                "Chatbot Fundamentals Screen",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.99f, 0.985f, 0.94f)).gameObject;

            ConfigureFundamentalsScreenLayout(screen);

            var progressText = CreateLabel(
                "Fundamentals Progress",
                screen.transform,
                "Fundamentals",
                18,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.30f, 0.25f, 0.38f),
                22f);
            var titleText = CreateLabel(
                "Fundamentals Title",
                screen.transform,
                "Ghost's Voice Basics",
                32,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.15f, 0.10f, 0.24f),
                40f);

            var ghostPanel = CreateCompactSubPanel(
                "Fundamentals Ghost Problem Panel",
                screen.transform,
                "Ghost's problem",
                new Color(0.94f, 0.98f, 1f));
            var ghostProblemText = CreateLabel(
                "Fundamentals Ghost Problem Text",
                ghostPanel,
                "Ghost is waiting.",
                15,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.24f, 0.24f, 0.34f),
                36f);

            var explanationText = CreateLabel(
                "Fundamentals Lily Explanation Text",
                screen.transform,
                "Lily explains the idea in a short line.",
                17,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.28f, 0.23f, 0.36f),
                42f);
            var actionPromptText = CreateLabel(
                "Fundamentals Action Prompt",
                screen.transform,
                "Try a small action.",
                16,
                FontStyle.Italic,
                TextAnchor.MiddleLeft,
                new Color(0.34f, 0.29f, 0.42f),
                26f);

            var actionButtonRoot = CreateDynamicButtonRoot("Fundamentals Action Buttons", screen.transform, 46f, true);
            var componentButtonRoot = CreateDynamicButtonRoot("Fundamentals Component Buttons", screen.transform, 46f, true);
            var challengeButtonRoot = CreateDynamicButtonRoot("Fundamentals Challenge Buttons", screen.transform, 46f, true);

            var componentChainText = CreateLabel(
                "Fundamentals Component Chain",
                screen.transform,
                string.Empty,
                16,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                new Color(0.16f, 0.22f, 0.32f),
                30f);
            var backendLinkText = CreateLabel(
                "Fundamentals Backend Link",
                screen.transform,
                string.Empty,
                15,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.28f, 0.32f, 0.40f),
                24f);

            var resultRow = CreateSplitRow("Fundamentals Result Row", screen.transform, 112f);
            var consequencePanel = CreateCompactSubPanel(
                "Fundamentals Consequence Panel",
                resultRow,
                "Visible consequence",
                new Color(1f, 0.98f, 0.91f));
            var consequenceText = CreateLabel(
                "Fundamentals Consequence Text",
                consequencePanel,
                "Ghost is waiting for a visible result.",
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.28f, 0.23f, 0.34f),
                62f);

            var ghostStatusPanel = CreateCompactSubPanel(
                "Fundamentals Ghost Status Panel",
                resultRow,
                "Ghost reaction",
                new Color(0.95f, 1f, 0.96f));
            var ghostStatusText = CreateLabel(
                "Fundamentals Ghost Status Text",
                ghostStatusPanel,
                "Ghost is watching the repair.",
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.22f, 0.30f, 0.26f),
                62f);

            var feedbackText = CreateLabel(
                "Fundamentals Feedback Text",
                screen.transform,
                "Small action required.",
                16,
                FontStyle.Italic,
                TextAnchor.MiddleLeft,
                new Color(0.28f, 0.25f, 0.38f),
                24f);

            var navRow = CreateButtonRow("Fundamentals Navigation Row", screen.transform, 42f);
            var previousButton = CreateButton("Fundamentals Previous Button", navRow, "Previous", 130f, 42f);
            var nextButton = CreateButton("Fundamentals Next Button", navRow, "Next", 130f, 42f);
            var skipButton = CreateButton("Fundamentals Skip Button", navRow, "Skip overview", 170f, 42f);

            fundamentalsPresenter = screen.AddComponent<ChatbotFundamentalsPresenter>();
            fundamentalsPresenter.Configure(
                progressText,
                titleText,
                ghostProblemText,
                explanationText,
                actionPromptText,
                consequenceText,
                ghostStatusText,
                componentChainText,
                backendLinkText,
                feedbackText,
                actionButtonRoot,
                componentButtonRoot,
                challengeButtonRoot,
                previousButton,
                nextButton,
                skipButton,
                null);

            return screen;
        }

        private static GameObject CreateActHubScreen(
            Transform parent,
            out Button fundamentalsButton,
            out Button act1Button,
            out Button act2Button,
            out Button act3Button,
            out Button narrativeContinueButton,
            out Button backToTitleButton)
        {
            var screen = CreatePanel(
                "Act Hub Screen",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.98f, 0.99f, 1f)).gameObject;

            ConfigureScreenLayout(screen, 12f);

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

            fundamentalsButton = CreateFundamentalsHubCard(screen.transform);

            var actCardRow = CreateActCardRow(screen.transform);

            act1Button = CreateActCard(
                actCardRow,
                "Act 1 Card",
                "Act 1: Intent Classification",
                "Group different messages by the same purpose so Ghost reacts to what the speaker wants.",
                "Start Act 1 Button",
                "Start Act 1",
                new Color(0.92f, 0.97f, 1f));

            act2Button = CreateActCard(
                actCardRow,
                "Act 2 Card",
                "Act 2: Entity Extraction",
                "Tag the important details in a message so Ghost notices places, objects, and times.",
                "Start Act 2 Button",
                "Start Act 2",
                new Color(0.93f, 1f, 0.96f));

            act3Button = CreateActCard(
                actCardRow,
                "Act 3 Card",
                "Act 3: Dialog Graph",
                "Build Ghost's reply map so it answers when details are known and asks when they are missing.",
                "Start Act 3 Button",
                "Start Act 3",
                new Color(1f, 0.965f, 0.88f));

            narrativeContinueButton = CreateButton("Narrative Continue Button", screen.transform, "Continue to Act", 220f, 44f);
            narrativeContinueButton.gameObject.SetActive(false);

            backToTitleButton = CreateButton("Back To Title Button", screen.transform, "Back to Title", 190f, 46f);
            return screen;
        }

        private static Button CreateFundamentalsHubCard(Transform parent)
        {
            var card = CreatePanel(
                "Fundamentals Hub Card",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.98f, 0.90f));

            var layoutElement = card.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 92f;
            layoutElement.preferredHeight = 92f;

            var outline = card.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.70f, 0.68f, 0.86f, 0.72f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var layout = card.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 12, 12);
            layout.spacing = 14f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var textColumn = new GameObject("Fundamentals Hub Text", typeof(RectTransform)).transform;
            textColumn.SetParent(card, false);
            var textColumnElement = textColumn.gameObject.AddComponent<LayoutElement>();
            textColumnElement.flexibleWidth = 1f;
            textColumnElement.minHeight = 64f;

            var textLayout = textColumn.gameObject.AddComponent<VerticalLayoutGroup>();
            textLayout.spacing = 4f;
            textLayout.childControlWidth = true;
            textLayout.childControlHeight = true;
            textLayout.childForceExpandWidth = true;
            textLayout.childForceExpandHeight = false;

            CreateLabel(
                "Fundamentals Hub Title",
                textColumn,
                "Ghost's Voice Basics",
                23,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.15f, 0.11f, 0.24f),
                30f);
            CreateLabel(
                "Fundamentals Hub Copy",
                textColumn,
                "A short playable overview: what a chatbot is, how its parts connect, and why Ghost gets confused.",
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.29f, 0.25f, 0.36f),
                32f);

            return CreateButton("Start Fundamentals Button", card, "Start Basics", 170f, 42f);
        }

        private static Transform CreateActCardRow(Transform parent)
        {
            var row = new GameObject("Act Card Row", typeof(RectTransform)).transform;
            row.SetParent(parent, false);

            var layoutElement = row.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 168f;
            layoutElement.preferredHeight = 168f;

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
            var card = CreatePanel(
                cardName,
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                color);

            var cardLayoutElement = card.gameObject.AddComponent<LayoutElement>();
            cardLayoutElement.minHeight = 168f;
            cardLayoutElement.preferredHeight = 168f;
            cardLayoutElement.flexibleWidth = 1f;

            var cardLayout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            cardLayout.padding = new RectOffset(16, 16, 10, 10);
            cardLayout.spacing = 5f;
            cardLayout.childControlWidth = true;
            cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = true;
            cardLayout.childForceExpandHeight = false;

            CreateLabel(cardName + " Title", card, title, 23, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.10f, 0.20f, 0.32f), 30f);
            CreateLabel(
                cardName + " Description",
                card,
                description,
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.31f, 0.40f),
                34f);

            return CreateButton(buttonName, card, buttonText, 180f, 38f);
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
            layoutElement.minHeight = 178f;
            layoutElement.preferredHeight = 178f;

            var layout = frame.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 18, 18);
            layout.spacing = 16f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var portrait = CreatePanel(
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

            var portraitOutline = portrait.gameObject.AddComponent<Outline>();
            portraitOutline.effectColor = new Color(0.66f, 0.58f, 0.78f, 0.7f);
            portraitOutline.effectDistance = new Vector2(2f, -2f);

            var portraitPlaceholder = CreateFillText(
                "Portrait Placeholder",
                portrait,
                "Lily",
                20,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(0.24f, 0.16f, 0.30f));
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

            var speaker = CreateLabel("Lily Speaker Name", textColumn, "Lily", 24, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.20f, 0.13f, 0.28f), 34f);
            var dialogue = CreateLabel("Lily Dialogue Text", textColumn, string.Empty, 22, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.28f, 0.22f, 0.36f), 86f);

            var dialogueFrame = frame.gameObject.AddComponent<LilyDialogueFrame>();
            dialogueFrame.Configure(speaker, dialogue, portrait.GetComponent<Image>(), portraitPlaceholder);
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

        private static void ConfigureFundamentalsScreenLayout(GameObject screen)
        {
            var layoutElement = screen.AddComponent<LayoutElement>();
            layoutElement.flexibleHeight = 1f;

            var layout = screen.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 14, 14);
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private static InputField CreateInputField(string name, Transform parent, string placeholder, float width, float height)
        {
            var inputTransform = CreatePanel(
                name,
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 1f, 1f));

            var image = inputTransform.GetComponent<Image>();
            image.raycastTarget = true;

            var outline = inputTransform.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.48f, 0.54f, 0.76f, 0.78f);
            outline.effectDistance = new Vector2(2f, -2f);

            var layoutElement = inputTransform.gameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = width;
            layoutElement.preferredWidth = width;
            layoutElement.minHeight = height;
            layoutElement.preferredHeight = height;

            var placeholderText = CreateFillText(
                "Placeholder",
                inputTransform,
                placeholder,
                20,
                FontStyle.Italic,
                TextAnchor.MiddleLeft,
                new Color(0.48f, 0.45f, 0.56f));

            var inputText = CreateFillText(
                "Text",
                inputTransform,
                string.Empty,
                22,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.15f, 0.12f, 0.22f));

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

        private static Transform CreateButtonRow(string name, Transform parent, float height)
        {
            var row = new GameObject(name, typeof(RectTransform)).transform;
            row.SetParent(parent, false);

            var layoutElement = row.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = height;
            layoutElement.preferredHeight = height;

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
            var panel = CreatePanel(
                name,
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                color);

            var outline = panel.gameObject.AddComponent<Outline>();
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

            CreateLabel(
                name + " Heading",
                panel,
                heading,
                20,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.17f, 0.11f, 0.25f),
                28f);
            return panel;
        }

        private static Text CreateFillText(
            string name,
            Transform parent,
            string text,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color)
        {
            var textTransform = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            textTransform.SetParent(parent, false);
            textTransform.anchorMin = Vector2.zero;
            textTransform.anchorMax = Vector2.one;
            textTransform.offsetMin = new Vector2(18f, 0f);
            textTransform.offsetMax = new Vector2(-18f, 0f);

            var label = textTransform.gameObject.AddComponent<Text>();
            label.text = text;
            label.font = GetBuiltinFont();
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.alignment = alignment;
            label.color = color;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.raycastTarget = false;

            return label;
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
                new EditorBuildSettingsScene(ShellSceneNames.Act1ScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.Act2ScenePath, true),
                new EditorBuildSettingsScene(ShellSceneNames.Act3ScenePath, true)
            };

            foreach (var existingScene in EditorBuildSettings.scenes)
            {
                if (existingScene.path == ShellSceneNames.GameShellScenePath ||
                    existingScene.path == ShellSceneNames.Act1ScenePath ||
                    existingScene.path == ShellSceneNames.Act2ScenePath ||
                    existingScene.path == ShellSceneNames.Act3ScenePath)
                {
                    continue;
                }

                scenes.Add(existingScene);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
