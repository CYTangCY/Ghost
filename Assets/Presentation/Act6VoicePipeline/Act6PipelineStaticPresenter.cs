using Ghost.Presentation.Common;
using System;
using System.Collections.Generic;
using Ghost.Presentation.Banter;
using Ghost.Presentation.Characters;
using Ghost.Presentation.Shell;
using Ghost.Presentation.GhostAvatar;
using Ghost.Puzzles.VoicePipeline;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Ghost.Presentation.Act6VoicePipeline
{
    public sealed class Act6PipelineStaticPresenter : MonoBehaviour, IAct6PipelineInteractionHost
    {
        private const string TitleText = "Final Chapter: Repair Ghost's Voice";
        private const string IntentCategoryId = "intent";
        private const string EntityCategoryId = "entities";
        private const string ConfidenceCategoryId = "confidence";
        private const string DialogueCategoryId = "dialogue";
        private const string ResponseCategoryId = "response";
        private const string BackendCategoryId = "backend";

        private static readonly string[] PaletteCategoryIds =
        {
            IntentCategoryId,
            EntityCategoryId,
            ConfidenceCategoryId,
            DialogueCategoryId,
            ResponseCategoryId,
            BackendCategoryId
        };

        private static readonly Color PageColor = new Color(0.95f, 0.97f, 0.95f);
        private static readonly Color ObjectiveColor = new Color(0.12f, 0.23f, 0.24f);
        private static readonly Color WarmNoteColor = new Color(1f, 0.96f, 0.80f);
        private static readonly Color ConversationColor = new Color(0.91f, 0.96f, 1f);
        private static readonly Color PaletteColor = new Color(0.95f, 0.93f, 0.99f);
        private static readonly Color BoardColor = new Color(0.98f, 0.99f, 1f);
        private static readonly Color EmptySlotColor = new Color(0.93f, 0.94f, 0.95f);
        private static readonly Color SuccessColor = new Color(0.82f, 0.96f, 0.84f);
        private static readonly Color FailureColor = new Color(1f, 0.85f, 0.81f);
        private static readonly Color StaleColor = new Color(1f, 0.93f, 0.70f);
        private static readonly Color SelectedColor = new Color(1f, 0.84f, 0.42f);
        private static readonly Color ActiveColor = new Color(0.68f, 0.91f, 0.89f);

        [SerializeField] private bool renderOnStart = true;

        private Act6PipelineInteractionController controller;
        private Act6EndingSequence endingSequence;
        private string selectedPaletteCategoryId = IntentCategoryId;

        private void Start()
        {
            if (renderOnStart)
            {
                var conversationPresenter =
                    GetComponent<FinalChapterConversationPresenter>();
                if (conversationPresenter == null)
                {
                    conversationPresenter =
                        gameObject.AddComponent<FinalChapterConversationPresenter>();
                }
                conversationPresenter.Configure(false);
                conversationPresenter.RenderSampleData();
            }
        }

        private void OnDestroy()
        {
            DetachController();
        }

        public void Configure(bool shouldRenderOnStart)
        {
            renderOnStart = shouldRenderOnStart;
        }

        public void RenderSampleData()
        {
            EnsureEventSystem();
            DetachController();
            selectedPaletteCategoryId = IntentCategoryId;
            controller = new Act6PipelineInteractionController();
            controller.StateChanged += RenderState;
            RenderState();
        }

        public void SelectComponent(string componentId)
        {
            controller?.SelectComponent(componentId);
        }

        public void DropComponentOnMainSlot(string componentId, int slotIndex)
        {
            controller?.PlaceInMainSlot(componentId, slotIndex);
        }

        public void DropComponentOnBackendSlot(string componentId)
        {
            controller?.PlaceInBackendSlot(componentId);
        }

        public void PlaceSelectedOnMainSlot(int slotIndex)
        {
            controller?.PlaceSelectedInMainSlot(slotIndex);
        }

        public void PlaceSelectedOnBackendSlot()
        {
            controller?.PlaceSelectedInBackendSlot();
        }

        private void RenderState()
        {
            if (controller == null)
            {
                return;
            }

            AmbientBanterPanel.SetCurrentState(
                GhostNarrativeState.FinalChapterId,
                controller.BuildHintContext());
            Act6PipelinePartDragView.ClearActivePreviews();
            ClearChildren(transform);
            ConfigureRoot();

            if (controller.CurrentPhase == Act6PipelinePhase.Ending)
            {
                CreateEndingOverlay();
                Canvas.ForceUpdateCanvases();
                endingSequence.Play();
                return;
            }

            CreateHeader();
            CreateObjectiveStrip();

            if (controller.CurrentPhase == Act6PipelinePhase.Onboarding)
            {
                CreateOnboardingPanel();
            }
            else
            {
                CreateLilyNoteStrip();
            }

            CreateConversationPanel();

            if (controller.CurrentPhase != Act6PipelinePhase.Onboarding)
            {
                CreateMainBody();
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            Canvas.ForceUpdateCanvases();
        }

        private void ConfigureRoot()
        {
            var root = (RectTransform)transform;
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;

            var background = GetComponent<Image>();
            if (background == null)
            {
                background = gameObject.AddComponent<Image>();
            }

            background.color = PageColor;
            background.raycastTarget = true;

            var layout = GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(24, 24, 18, 18);
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private void CreateHeader()
        {
            var header = GhostUITheme.Panel("Header", transform, Color.clear).rectTransform;
            var element = header.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 44f;
            element.preferredHeight = 44f;

            var layout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12f;
            layout.padding = new RectOffset(0, 220, 0, 0);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var title = GhostUITheme.Label(
                "Title",
                header,
                TitleText,
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);
            title.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var progress = GhostUITheme.Label(
                "Progress",
                header,
                GetProgressText(),
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleRight,
                new Color(0.18f, 0.42f, 0.43f));
            var progressElement = progress.gameObject.AddComponent<LayoutElement>();
            progressElement.minWidth = 180f;
            progressElement.preferredWidth = 180f;
        }

        private string GetProgressText()
        {
            switch (controller.CurrentPhase)
            {
                case Act6PipelinePhase.Onboarding:
                    return "Final chapter";
                case Act6PipelinePhase.VisitorTesting:
                    return "Visitor " + (controller.CurrentTestIndex + 1) + "/" + controller.TestCases.Count;
                case Act6PipelinePhase.ReadyForEnding:
                    return "Voice restored";
                default:
                    return controller.LastValidationResult == null ? "Build and test" : "Revise and rerun";
            }
        }

        private void CreateObjectiveStrip()
        {
            var strip = GhostUITheme.Panel("Objective Strip", transform, ObjectiveColor).rectTransform;
            var element = strip.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 40f;
            element.preferredHeight = 40f;
            element.flexibleHeight = 0f;

            var layout = strip.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 4, 4);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            GhostUITheme.Label(
                "Objective",
                strip,
                GetObjectiveText(),
                GhostUITheme.HeadingSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkOnDark);
        }

        private string GetObjectiveText()
        {
            switch (controller.CurrentPhase)
            {
                case Act6PipelinePhase.Onboarding:
                    return "Goal: rebuild one voice path that works for all three visitors";
                case Act6PipelinePhase.VisitorTesting:
                    return "Follow visitor " + (controller.CurrentTestIndex + 1) + " through the route one stage at a time";
                case Act6PipelinePhase.ReadyForEnding:
                    return "Complete: all six chapters contributed to Ghost's full reply";
                default:
                    return controller.ResultsAreStale
                        ? "Revise the route, then restart with visitor 1"
                        : "Choose five learned stages, attach a backend action, then start visitor 1";
            }
        }

        private void CreateOnboardingPanel()
        {
            var panel = GhostUITheme.Panel("Onboarding Panel", transform, WarmNoteColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.84f, 0.58f, 0.20f), new Vector2(2f, -2f));
            var element = panel.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 218f;
            element.preferredHeight = 218f;

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 12, 12);
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Onboarding Title",
                panel,
                "One final repair: make the whole voice work together",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.28f, 0.18f, 0.07f));

            var body = GhostUITheme.Label(
                "Onboarding Body",
                panel,
                "Lily: Um... the visitor message and Ghost's reply are fixed. The middle is where our six repairs have become tangled.\n" +
                "Choose five learned stages for the main path and one backend action for the side socket. Some cards are tempting shortcuts.\n" +
                "Three visitors will enter one at a time. Follow each message through the route and watch Ghost's reply.\n" +
                "If a visitor gets the wrong reply, revise the same path and restart with visitor 1.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.20f, 0.17f));
            body.lineSpacing = 1.03f;
            body.gameObject.AddComponent<LayoutElement>().preferredHeight = 122f;

            var button = GhostUITheme.PushButton(panel, "Open the final test board", new Color(0.79f, 0.90f, 1f), 230f);
            button.onClick.AddListener(controller.BeginAfterOnboarding);
        }

        private void CreateLilyNoteStrip()
        {
            var panel = GhostUITheme.Panel("Lily Note Strip", transform, WarmNoteColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.84f, 0.58f, 0.20f), new Vector2(1.5f, -1.5f));
            var element = panel.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 96f;
            element.preferredHeight = 96f;

            var layout = panel.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var note = GhostUITheme.Label(
                "Lily Note",
                panel,
                controller.CurrentPhase == Act6PipelinePhase.VisitorTesting
                    ? "Lily: Watch what the active card changes for this visitor."
                    : "Lily: Build one route that can handle all three visitors.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.25f, 0.20f, 0.17f));
            note.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var replay = GhostUITheme.PushButton(panel, "Replay Lily", new Color(1f, 0.98f, 0.88f), 126f);
            replay.interactable = controller.CurrentPhase == Act6PipelinePhase.Configure;
            replay.onClick.AddListener(controller.ReplayOnboarding);
        }
        private void CreateConversationPanel()
        {
            var panel = GhostUITheme.Panel("Conversation Panel", transform, ConversationColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.49f, 0.66f, 0.79f), new Vector2(2f, -2f));
            var element = panel.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 170f;
            element.preferredHeight = 170f;

            var layout = panel.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 16f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var faceRoot = new GameObject("Ghost Face", typeof(RectTransform)).GetComponent<RectTransform>();
            faceRoot.SetParent(panel, false);
            var faceElement = faceRoot.gameObject.AddComponent<LayoutElement>();
            faceElement.minWidth = 150f;
            faceElement.preferredWidth = 150f;
            var face = faceRoot.gameObject.AddComponent<GhostFaceView>();
            face.SetMood(MapMood(controller.CurrentMood));

            var column = GhostUITheme.Panel("Conversation Text", panel, Color.clear).rectTransform;
            column.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var columnLayout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            columnLayout.spacing = 4f;
            columnLayout.childControlWidth = true;
            columnLayout.childControlHeight = true;
            columnLayout.childForceExpandWidth = true;
            columnLayout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Conversation Title",
                column,
                GetConversationTitle(),
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 30f;

            GhostUITheme.Label(
                "Visitor Message",
                column,
                controller.CurrentPhase == Act6PipelinePhase.VisitorTesting &&
                controller.ActiveTestCase != null
                    ? "Visitor: \"" + controller.ActiveTestCase.VisitorMessage + "\""
                    : controller.CurrentPhase == Act6PipelinePhase.ReadyForEnding
                        ? "All three visitors received the expected reply."
                        : "Three visitors are waiting to test Ghost's voice.",
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.31f, 0.44f)).gameObject.AddComponent<LayoutElement>().preferredHeight = 30f;

            var outcome = GhostUITheme.Label(
                "Ghost Outcome",
                column,
                controller.StatusLine,
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink);
            outcome.lineSpacing = 1.02f;
            outcome.gameObject.AddComponent<LayoutElement>().preferredHeight = 76f;
        }

        private string GetConversationTitle()
        {
            switch (controller.CurrentPhase)
            {
                case Act6PipelinePhase.Onboarding:
                    return "Ghost's voice stops between repaired parts";
                case Act6PipelinePhase.VisitorTesting:
                    return controller.ActiveTraceStep == null
                        ? "Visitor test in progress"
                        : controller.ActiveTraceStep.Title;
                case Act6PipelinePhase.ReadyForEnding:
                    return "Ghost speaks clearly for the first time";
                default:
                    if (controller.LastValidationResult != null &&
                        !controller.LastValidationResult.IsCorrect)
                    {
                        return "Some visitor tests still fail";
                    }

                    return "Three visitors are waiting at the input";
            }
        }

        private void CreateMainBody()
        {
            var body = GhostUITheme.Panel("Main Body", transform, Color.clear).rectTransform;
            var bodyElement = body.gameObject.AddComponent<LayoutElement>();
            bodyElement.minHeight = 96f;
            bodyElement.flexibleHeight = 1f;

            var layout = body.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 16f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreatePalettePanel(body);
            CreatePipelinePanel(body);
        }

        private void CreatePalettePanel(Transform parent)
        {
            if (controller.CanEditBackendForCurrentVisitor)
            {
                selectedPaletteCategoryId = BackendCategoryId;
            }

            var panel = CreateColumnPanel("Component Palette", parent, PaletteColor, 0.31f);
            GhostUITheme.Label(
                "Palette Title",
                panel,
                "Voice-path cards",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);

            var guide = GhostUITheme.Label(
                "Palette Guide",
                panel,
                controller.CanEditBackendForCurrentVisitor
                    ? "Visitor 3 needs stored data. Change Backend here without replaying visitors 1 and 2."
                    : controller.CurrentPhase == Act6PipelinePhase.VisitorTesting
                        ? "The main route is locked while this visitor is running."
                        : "Choose a category, then drag or select one of its three cards.",
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft);
            guide.gameObject.AddComponent<LayoutElement>().preferredHeight = 40f;

            CreatePaletteCategoryButtons(panel);
            GhostUITheme.Label(
                "Selected Palette Category",
                panel,
                GetPaletteCategoryLabel(selectedPaletteCategoryId) + " choices",
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 22f;

            var visibleComponents = new List<Act6PipelineComponent>();
            foreach (var component in controller.PaletteComponents)
            {
                if (!controller.IsComponentPlaced(component.Id) &&
                    string.Equals(
                        GetPaletteCategoryId(component.Id),
                        selectedPaletteCategoryId,
                        StringComparison.Ordinal))
                {
                    visibleComponents.Add(component);
                }
            }

            var gridRoot = GhostUITheme.Panel("Palette Grid", panel, Color.clear).rectTransform;
            var gridElement = gridRoot.gameObject.AddComponent<LayoutElement>();
            var rowCount = Mathf.Max(1, Mathf.CeilToInt(visibleComponents.Count / 2f));
            gridElement.minHeight = rowCount * 69f;
            gridElement.preferredHeight = rowCount * 69f;
            gridElement.flexibleHeight = 1f;

            var grid = gridRoot.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(205f, 64f);
            grid.spacing = new Vector2(7f, 5f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            grid.childAlignment = TextAnchor.UpperLeft;

            foreach (var component in visibleComponents)
            {
                CreatePaletteCard(gridRoot, component);
            }

            if (visibleComponents.Count == 0)
            {
                GhostUITheme.Label(
                    "Palette Empty",
                    gridRoot,
                    "The selected card is already on the board. Choose another category or move it from its slot.",
                    GhostUITheme.SmallSize,
                    FontStyle.Normal,
                    TextAnchor.UpperLeft,
                    GhostUITheme.InkSoft);
            }

            var reset = GhostUITheme.PushButton(panel, "Reset board", new Color(0.92f, 0.92f, 0.94f), 150f);
            reset.interactable = controller.CurrentPhase == Act6PipelinePhase.Configure;
            reset.onClick.AddListener(controller.ResetPipeline);
        }

        private void CreatePaletteCategoryButtons(Transform parent)
        {
            var categoryRoot = GhostUITheme.Panel("Palette Categories", parent, Color.clear).rectTransform;
            var element = categoryRoot.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 68f;
            element.preferredHeight = 68f;

            var grid = categoryRoot.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(132f, 30f);
            grid.spacing = new Vector2(5f, 5f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;
            grid.childAlignment = TextAnchor.MiddleLeft;

            foreach (var categoryId in PaletteCategoryIds)
            {
                var capturedCategoryId = categoryId;
                var selected = string.Equals(
                    selectedPaletteCategoryId,
                    categoryId,
                    StringComparison.Ordinal);
                var button = GhostUITheme.PushButton(
                    categoryRoot,
                    GetPaletteCategoryLabel(categoryId),
                    selected ? SelectedColor : new Color(0.90f, 0.91f, 0.97f),
                    132f);
                button.interactable =
                    controller.CurrentPhase == Act6PipelinePhase.Configure ||
                    (controller.CanEditBackendForCurrentVisitor &&
                        string.Equals(categoryId, BackendCategoryId, StringComparison.Ordinal));
                button.onClick.AddListener(
                    () => SelectPaletteCategory(capturedCategoryId));
            }
        }

        private void SelectPaletteCategory(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                return;
            }

            selectedPaletteCategoryId = categoryId;
            AmbientBanterPanel.RecordPlayerActivity(
                GhostNarrativeState.FinalChapterId);
            RenderState();
        }

        private static string GetPaletteCategoryLabel(string categoryId)
        {
            switch (categoryId)
            {
                case IntentCategoryId:
                    return "Intent";
                case EntityCategoryId:
                    return "Entities";
                case ConfidenceCategoryId:
                    return "Confidence";
                case DialogueCategoryId:
                    return "Dialogue";
                case ResponseCategoryId:
                    return "Response";
                case BackendCategoryId:
                    return "Backend";
                default:
                    return "Cards";
            }
        }

        private static string GetPaletteCategoryId(string componentId)
        {
            switch (componentId)
            {
                case Act6PipelineData.IntentClassificationId:
                case Act6PipelineData.KeywordGuessId:
                case Act6PipelineData.ExactWordingId:
                    return IntentCategoryId;
                case Act6PipelineData.EntityExtractionId:
                case Act6PipelineData.SkipDetailsId:
                case Act6PipelineData.NounsOnlyId:
                    return EntityCategoryId;
                case Act6PipelineData.ConfidenceFallbackId:
                case Act6PipelineData.AlwaysAnswerId:
                case Act6PipelineData.RejectAllId:
                    return ConfidenceCategoryId;
                case Act6PipelineData.DialogueManagementId:
                case Act6PipelineData.FirstReplyId:
                case Act6PipelineData.FixedRouteId:
                    return DialogueCategoryId;
                case Act6PipelineData.ResponseGenerationId:
                case Act6PipelineData.RawDataReplyId:
                case Act6PipelineData.FixedSentenceId:
                    return ResponseCategoryId;
                default:
                    return BackendCategoryId;
            }
        }

        private void CreatePaletteCard(Transform parent, Act6PipelineComponent component)
        {
            var selected = string.Equals(
                controller.SelectedComponentId,
                component.Id,
                StringComparison.Ordinal);
            var card = GhostUITheme.Panel(
                "Palette Part - " + component.Id,
                parent,
                GetComponentColor(component.Id)).rectTransform;
            AddOutline(
                card.gameObject,
                selected ? SelectedColor : new Color(0.52f, 0.55f, 0.66f),
                selected ? new Vector2(3f, -3f) : new Vector2(1.5f, -1.5f));

            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 4, 4);
            layout.spacing = 0f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Part Label",
                card,
                component.Label,
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 24f;
            GhostUITheme.Label(
                "Part Job",
                card,
                component.JobLine,
                GhostUITheme.TinySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft).gameObject.AddComponent<LayoutElement>().preferredHeight = 32f;

            if (controller.CurrentPhase == Act6PipelinePhase.Configure ||
                (controller.CanEditBackendForCurrentVisitor &&
                    component.IsBackend))
            {
                var drag = card.gameObject.AddComponent<Act6PipelinePartDragView>();
                drag.Configure(this, component.Id, component.Label);
            }
        }
        private void CreatePipelinePanel(Transform parent)
        {
            var panel = CreateColumnPanel("Pipeline Panel", parent, BoardColor, 0.73f);
            GhostUITheme.Label(
                "Pipeline Title",
                panel,
                "Ghost's Complete Voice Path",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);

            var guide = GhostUITheme.Label(
                "Pipeline Guide",
                panel,
                "MAIN PATH: message enters at 1 and reply returns at 5. SIDE LINK: backend fetches extra data without replacing a main stage.",
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft);
            guide.gameObject.AddComponent<LayoutElement>().preferredHeight = 42f;

            var board = GhostUITheme.Panel("Pipeline Board", panel, new Color(0.97f, 0.985f, 1f)).rectTransform;
            AddOutline(board.gameObject, new Color(0.62f, 0.70f, 0.76f), new Vector2(1.5f, -1.5f));
            board.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
            var boardLayout = board.gameObject.AddComponent<VerticalLayoutGroup>();
            boardLayout.padding = new RectOffset(12, 12, 10, 10);
            boardLayout.spacing = 10f;
            boardLayout.childControlWidth = true;
            boardLayout.childControlHeight = true;
            boardLayout.childForceExpandWidth = true;
            boardLayout.childForceExpandHeight = false;

            CreateMainSlotRow(board);
            CreateBackendRow(board);
            CreateTestResultRow(board);
            CreateFeedbackRow(board);
        }

        private void CreateMainSlotRow(Transform parent)
        {
            var row = GhostUITheme.Panel("Main Pipeline Slots", parent, Color.clear).rectTransform;
            var element = row.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 175f;
            element.preferredHeight = 175f;

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 5f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateFixedEndpoint(
                row,
                Act6PipelineData.VisitorMessageEndpointId,
                "FIXED START");
            CreatePipelineArrow(row, "Start Arrow");

            for (var slotIndex = 0; slotIndex < 5; slotIndex++)
            {
                CreateMainSlot(row, slotIndex);
                CreatePipelineArrow(row, "Arrow " + slotIndex);
            }

            CreateFixedEndpoint(
                row,
                Act6PipelineData.GhostReplyEndpointId,
                "FIXED END");
        }

        private void CreateFixedEndpoint(
            Transform parent,
            string componentId,
            string stageLabel)
        {
            var component = Act6PipelineData.GetComponent(componentId);
            var visibleStep = controller.GetVisibleTraceStepForComponent(componentId);
            var isActive = controller.ActiveTraceStep != null &&
                string.Equals(
                    controller.ActiveTraceStep.ComponentId,
                    componentId,
                    StringComparison.Ordinal);
            var endpoint = GhostUITheme.Card(
                component.Label + " Endpoint",
                parent,
                isActive ? ActiveColor : GetComponentColor(componentId)).rectTransform;
            var element = endpoint.gameObject.AddComponent<LayoutElement>();
            element.minWidth = 94f;
            element.preferredWidth = 104f;
            element.minHeight = 175f;
            AddOutline(
                endpoint.gameObject,
                isActive
                    ? new Color(0.08f, 0.55f, 0.48f)
                    : new Color(0.43f, 0.56f, 0.68f),
                isActive ? new Vector2(3f, -3f) : new Vector2(1.5f, -1.5f));

            var layout = endpoint.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(7, 7, 8, 8);
            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Endpoint Stage",
                endpoint,
                stageLabel,
                GhostUITheme.TinySize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(0.27f, 0.39f, 0.45f)).gameObject.AddComponent<LayoutElement>().preferredHeight = 22f;
            GhostUITheme.Label(
                "Endpoint Label",
                endpoint,
                component.Label,
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 56f;
            GhostUITheme.Label(
                "Endpoint Detail",
                endpoint,
                visibleStep == null ? component.JobLine : visibleStep.Line,
                GhostUITheme.TinySize,
                visibleStep == null ? FontStyle.Normal : FontStyle.Bold,
                TextAnchor.UpperCenter,
                visibleStep != null && !visibleStep.Succeeded ? GhostUITheme.Bad : GhostUITheme.InkSoft)
                .gameObject.AddComponent<LayoutElement>().preferredHeight = 72f;
        }

        private static void CreatePipelineArrow(Transform parent, string name)
        {
            var arrow = GhostUITheme.Label(
                name,
                parent,
                ">",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(0.26f, 0.41f, 0.48f));
            var arrowElement = arrow.gameObject.AddComponent<LayoutElement>();
            arrowElement.minWidth = 13f;
            arrowElement.preferredWidth = 13f;
        }

        private void CreateMainSlot(Transform parent, int slotIndex)
        {
            var componentId = controller.GetMainSlotComponentId(slotIndex);
            var hasComponent = !string.IsNullOrWhiteSpace(componentId);
            var component = hasComponent ? Act6PipelineData.GetComponent(componentId) : null;
            var visibleStep = controller.GetVisibleTraceStepForComponent(componentId);
            var isActive = controller.ActiveTraceStep != null &&
                string.Equals(
                    controller.ActiveTraceStep.ComponentId,
                    componentId,
                    StringComparison.Ordinal);
            var isSelected = hasComponent && string.Equals(
                controller.SelectedComponentId,
                componentId,
                StringComparison.Ordinal);
            var isVerified = controller.IsMainSlotVerified(slotIndex);
            var hasCurrentResult = controller.LastValidationResult != null &&
                !controller.ResultsAreStale &&
                controller.CurrentPhase != Act6PipelinePhase.VisitorTesting;

            var slot = GhostUITheme.DropZone(
                "Main Slot " + (slotIndex + 1),
                parent,
                isActive
                    ? ActiveColor
                    : hasComponent ? GetComponentColor(componentId) : EmptySlotColor).rectTransform;
            var element = slot.gameObject.AddComponent<LayoutElement>();
            element.minWidth = 112f;
            element.flexibleWidth = 1f;
            element.minHeight = 175f;

            AddOutline(
                slot.gameObject,
                isActive
                    ? new Color(0.08f, 0.55f, 0.48f)
                    : isSelected ? SelectedColor : new Color(0.48f, 0.55f, 0.64f),
                isActive || isSelected ? new Vector2(3f, -3f) : new Vector2(1.5f, -1.5f));

            var layout = slot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(7, 7, 6, 6);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Slot Number",
                slot,
                "STAGE " + (slotIndex + 1),
                GhostUITheme.TinySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.27f, 0.39f, 0.45f)).gameObject.AddComponent<LayoutElement>().preferredHeight = 18f;

            if (!hasComponent)
            {
                var empty = GhostUITheme.Label(
                    "Empty Slot",
                    slot,
                    "Drop a card",
                    GhostUITheme.SmallSize,
                    FontStyle.Bold,
                    TextAnchor.MiddleCenter,
                    GhostUITheme.InkSoft);
                empty.gameObject.AddComponent<LayoutElement>().preferredHeight = 140f;
            }
            else
            {
                GhostUITheme.Label(
                    "Component Label",
                    slot,
                    component.Label,
                    GhostUITheme.SmallSize,
                    FontStyle.Bold,
                    TextAnchor.MiddleLeft,
                    GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 42f;
                GhostUITheme.Label(
                    "Component Job",
                    slot,
                    component.JobLine,
                    GhostUITheme.TinySize,
                    FontStyle.Normal,
                    TextAnchor.UpperLeft,
                    GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 45f;

                var detailText = visibleStep != null
                    ? visibleStep.Line
                    : controller.CurrentPhase == Act6PipelinePhase.VisitorTesting
                        ? "Waiting for this visitor."
                        : isVerified
                            ? component.PriorWorkLine
                            : hasCurrentResult
                                ? "Failed at least one test."
                                : "Not tested.";
                var detail = GhostUITheme.Label(
                    "Prior Work",
                    slot,
                    detailText,
                    GhostUITheme.TinySize,
                    isVerified || visibleStep != null ? FontStyle.Bold : FontStyle.Normal,
                    TextAnchor.UpperLeft,
                    visibleStep != null && !visibleStep.Succeeded
                        ? GhostUITheme.Bad
                        : isVerified ? GhostUITheme.Good : GhostUITheme.InkSoft);
                detail.gameObject.AddComponent<LayoutElement>().preferredHeight = 56f;

                if (controller.CurrentPhase == Act6PipelinePhase.Configure)
                {
                    var drag = slot.gameObject.AddComponent<Act6PipelinePartDragView>();
                    drag.Configure(this, component.Id, component.Label);
                }
            }

            if (controller.CurrentPhase == Act6PipelinePhase.Configure)
            {
                var drop = slot.gameObject.AddComponent<Act6PipelineSlotDropView>();
                drop.ConfigureMain(this, slotIndex);
            }
        }
        private void CreateBackendRow(Transform parent)
        {
            var row = GhostUITheme.Panel("Backend Side Link Row", parent, new Color(0.94f, 0.98f, 0.94f)).rectTransform;
            var element = row.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 108f;
            element.preferredHeight = 108f;
            AddOutline(row.gameObject, new Color(0.55f, 0.68f, 0.56f), new Vector2(1f, -1f));

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 7, 7);
            layout.spacing = 14f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var explanation = GhostUITheme.Panel("Backend Explanation", row, Color.clear).rectTransform;
            explanation.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var explanationLayout = explanation.gameObject.AddComponent<VerticalLayoutGroup>();
            explanationLayout.spacing = 3f;
            explanationLayout.childControlWidth = true;
            explanationLayout.childControlHeight = true;
            explanationLayout.childForceExpandWidth = true;
            explanationLayout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Backend Heading",
                explanation,
                "BACKEND ACTION",
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.42f, 0.24f));
            GhostUITheme.Label(
                "Backend Description",
                explanation,
                controller.CanEditBackendForCurrentVisitor
                    ? "Choose the action for visitor 3. Earlier visitor results stay complete."
                    : "The hours visitor will need one stored closing-time fact.",
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 56f;

            CreateBackendSlot(row);
        }

        private void CreateBackendSlot(Transform parent)
        {
            var componentId = controller.BackendComponentId;
            var hasComponent = !string.IsNullOrWhiteSpace(componentId);
            var component = hasComponent ? Act6PipelineData.GetComponent(componentId) : null;
            var visibleStep = controller.GetVisibleTraceStepForComponent(componentId);
            var isActive = hasComponent && controller.ActiveTraceStep != null &&
                string.Equals(
                    controller.ActiveTraceStep.ComponentId,
                    componentId,
                    StringComparison.Ordinal);
            var isSelected = hasComponent && string.Equals(
                controller.SelectedComponentId,
                componentId,
                StringComparison.Ordinal);
            var isVerified = controller.IsBackendVerified();
            var hasCurrentResult = controller.LastValidationResult != null &&
                !controller.ResultsAreStale &&
                controller.CurrentPhase != Act6PipelinePhase.VisitorTesting;
            var slot = GhostUITheme.DropZone(
                "Backend Side Slot",
                parent,
                isActive
                    ? ActiveColor
                    : hasComponent ? GetComponentColor(componentId) : EmptySlotColor).rectTransform;
            var element = slot.gameObject.AddComponent<LayoutElement>();
            element.minWidth = 355f;
            element.preferredWidth = 355f;
            element.minHeight = 92f;

            AddOutline(
                slot.gameObject,
                isActive
                    ? new Color(0.08f, 0.55f, 0.48f)
                    : isSelected ? SelectedColor : new Color(0.48f, 0.55f, 0.64f),
                isActive || isSelected ? new Vector2(3f, -3f) : new Vector2(1.5f, -1.5f));

            var layout = slot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(11, 11, 6, 6);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Backend Slot Label",
                slot,
                hasComponent ? component.Label : "Drop a backend action",
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 27f;

            var detailText = !hasComponent
                ? controller.CanEditBackendForCurrentVisitor
                    ? "Choose a backend action for visitor 3."
                    : "Side route for stored data."
                : visibleStep != null
                    ? visibleStep.Line
                    : controller.CurrentPhase == Act6PipelinePhase.VisitorTesting
                        ? "Waiting for this visitor."
                        : isVerified
                            ? component.PriorWorkLine
                            : hasCurrentResult
                                ? "Failed the data test."
                                : "Not tested.";
            GhostUITheme.Label(
                "Backend Slot Detail",
                slot,
                detailText,
                GhostUITheme.TinySize,
                isVerified || visibleStep != null ? FontStyle.Bold : FontStyle.Normal,
                TextAnchor.UpperLeft,
                visibleStep != null && !visibleStep.Succeeded
                    ? GhostUITheme.Bad
                    : isVerified ? GhostUITheme.Good : GhostUITheme.InkSoft)
                .gameObject.AddComponent<LayoutElement>().preferredHeight = 48f;

            var canEditBackend =
                controller.CurrentPhase == Act6PipelinePhase.Configure ||
                controller.CanEditBackendForCurrentVisitor;
            if (hasComponent && canEditBackend)
            {
                var drag = slot.gameObject.AddComponent<Act6PipelinePartDragView>();
                drag.Configure(this, component.Id, component.Label);
            }

            if (canEditBackend)
            {
                var drop = slot.gameObject.AddComponent<Act6PipelineSlotDropView>();
                drop.ConfigureBackend(this);
            }
        }

        private void CreateTestResultRow(Transform parent)
        {
            var row = GhostUITheme.Panel("Visitor Test Results", parent, Color.clear).rectTransform;
            var element = row.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 166f;
            element.preferredHeight = 166f;

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            for (var index = 0; index < controller.TestCases.Count; index++)
            {
                CreateTestResultCard(row, controller.TestCases[index], index);
            }
        }

        private void CreateTestResultCard(
            Transform parent,
            Act6PipelineTestCase testCase,
            int testIndex)
        {
            var result = controller.IsTestResultVisible(testIndex)
                ? FindTestResult(testCase.Id)
                : null;
            var isRunning = controller.IsVisitorTestRunning(testIndex);
            var isStale = result != null && controller.ResultsAreStale;
            var color = result != null
                ? isStale
                    ? StaleColor
                    : result.Passed ? SuccessColor : FailureColor
                : isRunning
                    ? ActiveColor
                    : new Color(0.92f, 0.96f, 0.98f);
            var card = GhostUITheme.Card("Visitor Test " + (testIndex + 1), parent, color).rectTransform;
            card.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            AddOutline(
                card.gameObject,
                isRunning
                    ? new Color(0.08f, 0.55f, 0.48f)
                    : new Color(0.55f, 0.61f, 0.67f),
                isRunning ? new Vector2(3f, -3f) : new Vector2(1f, -1f));

            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(9, 9, 6, 6);
            layout.spacing = 1f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var status = result == null
                ? isRunning ? "RUNNING" : "WAITING"
                : isStale
                    ? "STALE"
                    : result.Passed ? "PASS" : "FAIL";
            GhostUITheme.Label(
                "Test Status",
                card,
                "VISITOR " + (testIndex + 1) + "  |  " + status,
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 22f;
            GhostUITheme.Label(
                "Test Visitor Message",
                card,
                "Visitor: " + testCase.VisitorMessage,
                GhostUITheme.TinySize,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                new Color(0.18f, 0.31f, 0.44f)).gameObject.AddComponent<LayoutElement>().preferredHeight = 38f;
            GhostUITheme.Label(
                "Expected Reply",
                card,
                "Expected: " + testCase.ExpectedReply,
                GhostUITheme.TinySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 40f;
            var currentLine = isRunning && controller.ActiveTraceStep != null
                ? controller.ActiveTraceStep.Line
                : "Waiting for this visitor.";
            GhostUITheme.Label(
                "Actual Reply",
                card,
                result == null
                    ? "Current: " + currentLine
                    : "Actual: " + result.ActualReply,
                    GhostUITheme.TinySize,
                result != null && !result.Passed ? FontStyle.Bold : FontStyle.Normal,
                TextAnchor.UpperLeft,
                result != null && !result.Passed
                    ? GhostUITheme.Bad
                    : GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 48f;
        }

        private Act6PipelineTestResult FindTestResult(string testCaseId)
        {
            if (controller.LastValidationResult == null)
            {
                return null;
            }

            foreach (var result in controller.LastValidationResult.TestResults)
            {
                if (string.Equals(
                        result.TestCase.Id,
                        testCaseId,
                        StringComparison.Ordinal))
                {
                    return result;
                }
            }

            return null;
        }
        private void CreateFeedbackRow(Transform parent)
        {
            var row = GhostUITheme.Panel("Feedback and Action Row", parent, GetFeedbackColor()).rectTransform;
            var element = row.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 76f;
            element.preferredHeight = 76f;
            AddOutline(row.gameObject, new Color(0.56f, 0.60f, 0.66f), new Vector2(1f, -1f));

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(14, 12, 8, 8);
            layout.spacing = 14f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var feedback = GhostUITheme.Label(
                "Pipeline Feedback",
                row,
                GetFeedbackText(),
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);
            feedback.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            if (controller.CurrentPhase == Act6PipelinePhase.VisitorTesting)
            {
                var edit = GhostUITheme.PushButton(
                    row,
                    "Edit route",
                    new Color(0.94f, 0.94f, 0.96f),
                    140f);
                edit.onClick.AddListener(controller.CancelVisitorTests);
            }

            var action = GhostUITheme.PushButton(row, GetPrimaryActionLabel(), GetPrimaryActionColor(), 230f);
            action.interactable = controller.CurrentPhase != Act6PipelinePhase.Configure ||
                controller.IsPipelineReadyToTest;
            action.onClick.AddListener(HandlePrimaryAction);
        }

        private string GetFeedbackText()
        {
            if (controller.CurrentPhase == Act6PipelinePhase.VisitorTesting)
            {
                return controller.ActiveTraceStep == null
                    ? "The current visitor is ready."
                    : "Visitor " + (controller.CurrentTestIndex + 1) + ": " +
                        controller.ActiveTraceStep.Line;
            }

            if (controller.CurrentPhase == Act6PipelinePhase.ReadyForEnding)
            {
                return "The same route handled uncertainty, missing information, and backend data.";
            }

            if (controller.LastValidationResult == null)
            {
                return GetPlacedMainCount() + "/5 main stages filled; " +
                    (controller.BackendAttached
                        ? "a backend action is ready."
                        : "choose the backend when a visitor needs stored data.");
            }

            if (controller.ResultsAreStale)
            {
                return "The route changed. Restart with visitor 1.";
            }

            if (!controller.LastValidationResult.IsCorrect)
            {
                var summary = "Visitors helped: " +
                    controller.LastValidationResult.PassedTestCount + "/" +
                    controller.LastValidationResult.TestResults.Count + ". ";
                return controller.LastValidationResult.Errors.Count == 0
                    ? summary + "Compare the first red card: expected versus actual."
                    : summary + controller.LastValidationResult.Errors[0];
            }

            return "The route is ready. Start with visitor 1.";
        }
        private Color GetFeedbackColor()
        {
            if (controller.CurrentPhase == Act6PipelinePhase.VisitorTesting)
            {
                return controller.ActiveTraceStep != null &&
                    !controller.ActiveTraceStep.Succeeded
                        ? FailureColor
                        : ActiveColor;
            }

            if (controller.CurrentPhase == Act6PipelinePhase.ReadyForEnding)
            {
                return SuccessColor;
            }

            if (controller.ResultsAreStale)
            {
                return StaleColor;
            }

            return controller.LastValidationResult != null &&
                !controller.LastValidationResult.IsCorrect
                ? FailureColor
                : new Color(0.92f, 0.96f, 0.98f);
        }

        private string GetPrimaryActionLabel()
        {
            switch (controller.CurrentPhase)
            {
                case Act6PipelinePhase.VisitorTesting:
                    return controller.ActiveTestResult != null &&
                        controller.CurrentTraceIndex >= controller.ActiveTestResult.TraceSteps.Count - 1
                            ? controller.CurrentTestIndex >= controller.TestCases.Count - 1
                                ? "Finish visitor tests"
                                : "Meet visitor " + (controller.CurrentTestIndex + 2)
                            : "Next stage";
                case Act6PipelinePhase.ReadyForEnding:
                    return "Hear Ghost speak";
                default:
                    return controller.LastValidationResult != null
                        ? "Restart with visitor 1"
                        : "Start visitor 1";
            }
        }
        private Color GetPrimaryActionColor()
        {
            if (controller.CurrentPhase == Act6PipelinePhase.ReadyForEnding)
            {
                return new Color(1f, 0.84f, 0.45f);
            }

            if (controller.CurrentPhase == Act6PipelinePhase.VisitorTesting)
            {
                return controller.ActiveTraceStep != null &&
                    !controller.ActiveTraceStep.Succeeded
                        ? FailureColor
                        : ActiveColor;
            }

            return controller.ResultsAreStale
                ? StaleColor
                : new Color(0.77f, 0.89f, 1f);
        }

        private void HandlePrimaryAction()
        {
            switch (controller.CurrentPhase)
            {
                case Act6PipelinePhase.Configure:
                    controller.RunPipeline();
                    return;
                case Act6PipelinePhase.VisitorTesting:
                    controller.AdvanceVisitorTest();
                    return;
                case Act6PipelinePhase.ReadyForEnding:
                    controller.BeginEnding();
                    return;
            }
        }

        private int GetPlacedMainCount()
        {
            var count = 0;
            for (var slotIndex = 0; slotIndex < 5; slotIndex++)
            {
                if (!string.IsNullOrWhiteSpace(controller.GetMainSlotComponentId(slotIndex)))
                {
                    count++;
                }
            }

            return count;
        }

        private void CreateEndingOverlay()
        {
            var overlay = GhostUITheme.Panel(
                "Final Ending Overlay",
                transform,
                new Color(0.08f, 0.12f, 0.14f)).rectTransform;
            var overlayElement = overlay.gameObject.AddComponent<LayoutElement>();
            overlayElement.ignoreLayout = true;
            overlay.anchorMin = Vector2.zero;
            overlay.anchorMax = Vector2.one;
            overlay.offsetMin = Vector2.zero;
            overlay.offsetMax = Vector2.zero;
            overlay.SetAsLastSibling();

            var canvasGroup = overlay.gameObject.AddComponent<CanvasGroup>();
            var advance = overlay.gameObject.AddComponent<Button>();
            advance.targetGraphic = overlay.GetComponent<Image>();
            advance.transition = Selectable.Transition.None;

            var skip = GhostUITheme.PushButton(
                overlay,
                "Skip ending",
                new Color(0.91f, 0.94f, 0.95f, 0.94f),
                150f);
            var skipRect = (RectTransform)skip.transform;
            skipRect.anchorMin = new Vector2(0f, 1f);
            skipRect.anchorMax = new Vector2(0f, 1f);
            skipRect.pivot = new Vector2(0f, 1f);
            skipRect.anchoredPosition = new Vector2(28f, -24f);
            skipRect.sizeDelta = new Vector2(150f, 44f);

            var ghostRoot = CreateAnchoredRect(
                "Ending Ghost",
                overlay,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(510f, 80f),
                new Vector2(230f, 230f));

            var glowRect = CreateAnchoredRect(
                "Ghost Glow",
                ghostRoot,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(205f, 205f));
            var glow = glowRect.gameObject.AddComponent<Image>();
            glow.color = Color.clear;
            glow.enabled = false;
            glow.raycastTarget = false;

            var faceRect = CreateAnchoredRect(
                "Happy Ghost Face",
                ghostRoot,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(190f, 190f));
            var face = faceRect.gameObject.AddComponent<GhostFaceView>();
            face.SetMood(GhostMood.Happy);

            var heading = GhostUITheme.Label(
                "Ending Heading",
                overlay,
                string.Empty,
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(1f, 0.91f, 0.62f));
            heading.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            heading.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            heading.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            heading.rectTransform.anchoredPosition = new Vector2(0f, 20f);
            heading.rectTransform.sizeDelta = new Vector2(760f, 62f);

            var body = GhostUITheme.Label(
                "Ending Dialogue",
                overlay,
                string.Empty,
                GhostUITheme.TitleSize,
                FontStyle.Normal,
                TextAnchor.MiddleCenter,
                new Color(0.94f, 0.97f, 1f));
            body.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            body.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            body.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            body.rectTransform.anchoredPosition = new Vector2(0f, -70f);
            body.rectTransform.sizeDelta = new Vector2(760f, 150f);

            var lilyRect = CreateAnchoredRect(
                "Ending Lily",
                overlay,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-510f, -70f),
                new Vector2(310f, 560f));
            var lily = lilyRect.gameObject.AddComponent<Image>();
            lily.sprite = LilyPixelPortraitFactory.GetFullBody();
            lily.color = Color.white;
            lily.preserveAspect = true;
            lily.raycastTarget = false;
            lily.gameObject.SetActive(false);

            var choiceRoot = CreateAnchoredRect(
                "Ending Choice",
                overlay,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -205f),
                new Vector2(420f, 56f));
            var choiceLayout = choiceRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
            choiceLayout.spacing = 20f;
            choiceLayout.childAlignment = TextAnchor.MiddleCenter;
            choiceLayout.childControlWidth = true;
            choiceLayout.childControlHeight = true;
            choiceLayout.childForceExpandWidth = true;
            choiceLayout.childForceExpandHeight = true;

            var yes = GhostUITheme.PushButton(
                choiceRoot,
                "Yes",
                new Color(0.72f, 0.91f, 0.78f),
                190f);
            var no = GhostUITheme.PushButton(
                choiceRoot,
                "No",
                new Color(0.95f, 0.76f, 0.76f),
                190f);
            choiceRoot.gameObject.SetActive(false);

            var advanceHint = GhostUITheme.Label(
                "Ending Continue Hint",
                overlay,
                string.Empty,
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(0.82f, 0.88f, 0.91f));
            advanceHint.raycastTarget = false;
            advanceHint.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            advanceHint.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            advanceHint.rectTransform.pivot = new Vector2(0.5f, 0f);
            advanceHint.rectTransform.anchoredPosition = new Vector2(0f, 28f);
            advanceHint.rectTransform.sizeDelta = new Vector2(520f, 36f);

            var creditsRoot = CreateAnchoredRect(
                "Credits Root",
                overlay,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -220f),
                new Vector2(780f, 460f));
            var credits = GhostUITheme.Label(
                "Credits",
                creditsRoot,
                string.Empty,
                GhostUITheme.TitleSize,
                FontStyle.Normal,
                TextAnchor.MiddleCenter,
                new Color(0.94f, 0.97f, 1f));
            credits.rectTransform.anchorMin = Vector2.zero;
            credits.rectTransform.anchorMax = Vector2.one;
            credits.rectTransform.offsetMin = Vector2.zero;
            credits.rectTransform.offsetMax = Vector2.zero;

            endingSequence = GetComponent<Act6EndingSequence>();
            if (endingSequence == null)
            {
                endingSequence = gameObject.AddComponent<Act6EndingSequence>();
            }

            endingSequence.Configure(
                overlay.gameObject,
                canvasGroup,
                advance,
                advanceHint,
                ghostRoot,
                glow,
                face,
                heading,
                body,
                lily,
                choiceRoot,
                yes,
                no,
                creditsRoot,
                credits,
                skip);
        }
        private static Color GetComponentColor(string componentId)
        {
            switch (componentId)
            {
                case Act6PipelineData.VisitorMessageEndpointId:
                    return new Color(0.84f, 0.92f, 1f);
                case Act6PipelineData.IntentClassificationId:
                case Act6PipelineData.KeywordGuessId:
                case Act6PipelineData.ExactWordingId:
                    return new Color(0.91f, 0.86f, 0.98f);
                case Act6PipelineData.EntityExtractionId:
                case Act6PipelineData.SkipDetailsId:
                case Act6PipelineData.NounsOnlyId:
                    return new Color(0.84f, 0.92f, 1f);
                case Act6PipelineData.ConfidenceFallbackId:
                case Act6PipelineData.AlwaysAnswerId:
                case Act6PipelineData.RejectAllId:
                    return new Color(1f, 0.90f, 0.76f);
                case Act6PipelineData.DialogueManagementId:
                case Act6PipelineData.FirstReplyId:
                case Act6PipelineData.FixedRouteId:
                    return new Color(0.82f, 0.95f, 0.87f);
                case Act6PipelineData.ResponseGenerationId:
                case Act6PipelineData.RawDataReplyId:
                case Act6PipelineData.FixedSentenceId:
                    return new Color(1f, 0.92f, 0.72f);
                case Act6PipelineData.GhostReplyEndpointId:
                    return new Color(0.98f, 0.86f, 0.90f);
                case Act6PipelineData.BackendActionId:
                case Act6PipelineData.ObjectRoomBackendId:
                case Act6PipelineData.VisitorProfileBackendId:
                    return new Color(0.92f, 0.89f, 0.78f);
                default:
                    return EmptySlotColor;
            }
        }
        private static GhostMood MapMood(Act6GhostMood mood)
        {
            switch (mood)
            {
                case Act6GhostMood.Happy:
                    return GhostMood.Happy;
                case Act6GhostMood.Confused:
                    return GhostMood.Confused;
                case Act6GhostMood.Sad:
                    return GhostMood.Sad;
                default:
                    return GhostMood.Neutral;
            }
        }

        private static RectTransform CreateColumnPanel(
            string name,
            Transform parent,
            Color color,
            float flexibleWidth)
        {
            var panel = GhostUITheme.Panel(name, parent, color).rectTransform;
            AddOutline(
                panel.gameObject,
                new Color(0.64f, 0.66f, 0.72f),
                new Vector2(1.5f, -1.5f));
            var element = panel.gameObject.AddComponent<LayoutElement>();
            element.flexibleWidth = flexibleWidth;
            element.flexibleHeight = 1f;

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(13, 13, 10, 10);
            layout.spacing = 7f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return panel;
        }




        private static RectTransform CreateAnchoredRect(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            return rect;
        }

        private static void AddOutline(
            GameObject target,
            Color color,
            Vector2 distance)
        {
            var outline = target.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = distance;
            outline.useGraphicAlpha = true;
        }


        private static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }

        private void DetachController()
        {
            if (controller != null)
            {
                controller.StateChanged -= RenderState;
                controller = null;
            }
        }

        private static void ClearChildren(Transform parent)
        {
            for (var index = parent.childCount - 1; index >= 0; index--)
            {
                var child = parent.GetChild(index).gameObject;
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
    }
}
