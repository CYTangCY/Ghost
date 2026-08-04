using Ghost.Presentation.Common;
using System;
using Ghost.Presentation.Banter;
using Ghost.Presentation.GhostAvatar;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.BackendResponse;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Act6BackendResponse
{
    public sealed class Act6BackendStaticPresenter : MonoBehaviour, IAct6BackendInteractionHost
    {
        private static readonly Color PageColor = new Color(0.95f, 0.97f, 0.95f);
        private static readonly Color ObjectiveColor = new Color(0.12f, 0.25f, 0.25f);
        private static readonly Color WarmNoteColor = new Color(1f, 0.96f, 0.80f);
        private static readonly Color ConversationColor = new Color(0.91f, 0.96f, 1f);
        private static readonly Color PaletteColor = new Color(0.95f, 0.93f, 0.99f);
        private static readonly Color BoardColor = new Color(0.98f, 0.99f, 1f);
        private static readonly Color EmptyColor = new Color(0.92f, 0.94f, 0.95f);
        private static readonly Color SuccessColor = new Color(0.82f, 0.96f, 0.84f);
        private static readonly Color FailureColor = new Color(1f, 0.85f, 0.81f);
        private static readonly Color SelectedColor = new Color(1f, 0.86f, 0.52f);
        private static readonly Color ActiveColor = new Color(0.67f, 0.91f, 0.89f);

        [SerializeField] private bool renderOnStart = true;
        private Act6BackendInteractionController controller;

        private void Start()
        {
            if (renderOnStart)
            {
                RenderSampleData();
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
            controller = new Act6BackendInteractionController();
            controller.StateChanged += RenderState;
            RenderState();
        }

        public void SelectCard(string cardId)
        {
            controller?.SelectCard(cardId);
        }

        public void DropCardOnRole(string cardId, string roleId)
        {
            controller?.PlaceCardOnRole(cardId, roleId);
        }

        public void HandleRoleSocketClick(string roleId)
        {
            if (controller == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(controller.GetPlacedCardId(roleId)))
            {
                controller.PlaceSelectedOnRole(roleId);
                return;
            }

            controller.ReturnRoleCardToPalette(roleId);
        }

        private void RenderState()
        {
            if (controller == null)
            {
                return;
            }

            AmbientBanterPanel.SetCurrentState(
                GhostNarrativeState.Act6Id,
                controller.BuildHintContext());
            ClearChildren(transform);
            ConfigureRoot();
            CreateHeader();
            CreateObjectiveStrip();

            if (controller.CurrentPhase == Act6BackendPhase.Onboarding)
            {
                CreateOnboardingPanel();
            }
            else
            {
                CreateLilyNote();
            }

            CreateConversationPanel();

            if (controller.CurrentPhase != Act6BackendPhase.Onboarding)
            {
                CreateMainBody();
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            Canvas.ForceUpdateCanvases();
        }

        private void ConfigureRoot()
        {
            var rect = (RectTransform)transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
            image.color = PageColor;

            var layout = GetComponent<VerticalLayoutGroup>() ??
                gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 18, 18);
            layout.spacing = 14f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private void CreateHeader()
        {
            var header = GhostUITheme.Panel("Header", transform, Color.clear).rectTransform;
            SetHeight(header, 44f);
            // The header is a fixed title row, but its inner horizontal group force-expands height,
            // which reports flexible height to the page and let the header eat all the spare space.
            header.GetComponent<LayoutElement>().flexibleHeight = 0f;
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
                "Chapter 6: Backend Action and Response",
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
                new Color(0.18f, 0.43f, 0.43f));
            var progressElement = progress.gameObject.AddComponent<LayoutElement>();
            progressElement.minWidth = 220f;
            progressElement.preferredWidth = 220f;
        }

        private string GetProgressText()
        {
            switch (controller.CurrentPhase)
            {
                case Act6BackendPhase.Onboarding:
                    return "Lesson 6/6";
                case Act6BackendPhase.Playback:
                    return "Run " + (controller.PlaybackIndex + 1) + "/" +
                        controller.PlaybackSteps.Count;
                case Act6BackendPhase.Complete:
                    return "Lesson complete";
                default:
                    return "Build the route";
            }
        }

        private void CreateObjectiveStrip()
        {
            var strip = GhostUITheme.Panel("Objective Strip", transform, ObjectiveColor).rectTransform;
            SetHeight(strip, 40f);
            strip.GetComponent<LayoutElement>().flexibleHeight = 0f;
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
                case Act6BackendPhase.Onboarding:
                    return "Goal: separate where a fact lives, the action that gets it, and the reply that says it";
                case Act6BackendPhase.Playback:
                    return "Run: watch stored data become a visitor-facing sentence one responsibility at a time";
                case Act6BackendPhase.Complete:
                    return "Complete: Ghost fetched the right fact and phrased the answer clearly";
                default:
                    return "Build: fill DATA SOURCE, ACTION, and RESPONSE for the tested lab-hours route";
            }
        }

        private void CreateOnboardingPanel()
        {
            var panel = GhostUITheme.Panel("Onboarding Panel", transform, WarmNoteColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.84f, 0.58f, 0.20f));
            // Has to cover padding + title + body + the start button; trim it and the button is the
            // thing that gets squeezed out, which leaves no way into the chapter.
            SetHeight(panel, 248f);
            var layout = AddVerticalLayout(panel, new RectOffset(22, 22, 16, 16), 8f);

            GhostUITheme.Label(
                "Onboarding Title",
                panel,
                "The route is correct, but Ghost still blurts raw data",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 34f;

            var body = GhostUITheme.Label(
                "Onboarding Body",
                panel,
                "Lily: Um... Chapters 1-5 already recognized this as a lab-hours request and chose a safe route. What happens next is three different jobs.\n" +
                "1. DATA SOURCE owns the fact.  2. ACTION asks for one precise field.  3. RESPONSE puts the returned value into a complete sentence.\n" +
                "Drag a card into each socket, or click a card and then a socket. Run the whole route; Ghost stops at the first responsibility that does not match.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink);
            body.lineSpacing = 1.03f;
            body.gameObject.AddComponent<LayoutElement>().preferredHeight = 126f;

            var button = GhostUITheme.PushButton(
                panel,
                "Open the backend bench",
                new Color(0.78f, 0.90f, 1f),
                250f);
            button.onClick.AddListener(controller.BeginAfterOnboarding);
        }

        private void CreateLilyNote()
        {
            var panel = GhostUITheme.Panel("Lily Note", transform, WarmNoteColor).rectTransform;
            SetHeight(panel, 96f);
            var layout = panel.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 12f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var note = GhostUITheme.Label(
                "Lily Reminder",
                panel,
                "Lily: Storage has the fact; an action requests it; response generation says it to the visitor.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);
            note.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var replay = GhostUITheme.PushButton(panel, "Replay Lily", new Color(1f, 0.98f, 0.88f), 128f);
            replay.interactable = controller.CurrentPhase == Act6BackendPhase.Configure;
            replay.onClick.AddListener(controller.ReplayOnboarding);
        }

        private void CreateConversationPanel()
        {
            var panel = GhostUITheme.Panel("Conversation Panel", transform, ConversationColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.49f, 0.66f, 0.79f));
            SetHeight(panel, 178f);
            // Same trap as the header: the inner group force-expands height, so without this the
            // panel reports flexible height and stretches down the rest of the page.
            panel.GetComponent<LayoutElement>().flexibleHeight = 0f;
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
            faceRoot.gameObject.AddComponent<GhostFaceView>().SetMood(MapMood(controller.CurrentMood));

            var column = GhostUITheme.Panel("Conversation Text", panel, Color.clear).rectTransform;
            column.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            AddVerticalLayout(column, new RectOffset(), 4f);

            GhostUITheme.Label(
                "Conversation Title",
                column,
                GetConversationTitle(),
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 30f;

            GhostUITheme.Label(
                "Visitor",
                column,
                "Visitor: \"" + Act6BackendResponseData.VisitorMessage + "\"",
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.31f, 0.44f)).gameObject.AddComponent<LayoutElement>().preferredHeight = 28f;

            var outcome = GhostUITheme.Label(
                "Outcome",
                column,
                controller.StatusLine,
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink);
            outcome.lineSpacing = 1.02f;
            outcome.gameObject.AddComponent<LayoutElement>().preferredHeight = 78f;
        }

        private string GetConversationTitle()
        {
            if (controller.CurrentPhase == Act6BackendPhase.Playback &&
                controller.ActivePlaybackStep != null)
            {
                return controller.ActivePlaybackStep.Title;
            }

            if (controller.CurrentPhase == Act6BackendPhase.Complete)
            {
                return "Ghost turns the backend result into a real reply";
            }

            if (controller.LastValidation != null && !controller.LastValidation.IsCorrect)
            {
                return "The route stopped at the first broken responsibility";
            }

            return controller.CurrentPhase == Act6BackendPhase.Onboarding
                ? "Ghost understands the request, then gets stuck"
                : "The tested lab-hours route is waiting";
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
            CreatePalette(body);
            CreateRouteBoard(body);
        }

        private void CreatePalette(Transform parent)
        {
            var panel = CreateColumnPanel("Card Palette", parent, 0.34f, PaletteColor);
            AddOutline(panel.gameObject, new Color(0.69f, 0.61f, 0.81f));

            GhostUITheme.Label(
                "Palette Heading",
                panel,
                "Route parts",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 30f;

            GhostUITheme.Label(
                "Palette Guide",
                panel,
                "Drag, or select a palette card and then select a role socket. Click a filled socket to return its card.",
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft).gameObject.AddComponent<LayoutElement>().preferredHeight = 38f;

            foreach (var card in controller.Palette)
            {
                if (!controller.IsCardPlaced(card.Id))
                {
                    CreatePaletteCard(panel, card);
                }
            }

            var spacer = new GameObject("Palette Spacer", typeof(RectTransform));
            spacer.transform.SetParent(panel, false);
            spacer.AddComponent<LayoutElement>().flexibleHeight = 1f;

            var reset = GhostUITheme.PushButton(panel, "Reset board", new Color(0.92f, 0.92f, 0.94f), 150f);
            reset.interactable = controller.CurrentPhase == Act6BackendPhase.Configure;
            reset.onClick.AddListener(controller.ResetBoard);
        }

        private void CreatePaletteCard(Transform parent, Act6BackendCard card)
        {
            var selected = string.Equals(
                controller.SelectedCardId,
                card.Id,
                StringComparison.Ordinal);
            var root = GhostUITheme.Card(
                "Card " + card.Id,
                parent,
                selected ? SelectedColor : GetRoleColor(card.RoleId)).rectTransform;
            SetHeight(root, 72f);
            AddOutline(
                root.gameObject,
                selected
                    ? new Color(0.83f, 0.55f, 0.10f)
                    : new Color(0.54f, 0.57f, 0.65f));

            var layout = AddVerticalLayout(root, new RectOffset(12, 12, 7, 7), 2f);
            GhostUITheme.Label(
                "Card Label",
                root,
                card.Label,
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 34f;

            GhostUITheme.Label(
                "Card Role",
                root,
                Act6BackendResponseData.GetRoleLabel(card.RoleId),
                GhostUITheme.TinySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft).gameObject.AddComponent<LayoutElement>().preferredHeight = 20f;

            var drag = root.gameObject.AddComponent<Act6BackendCardDragView>();
            drag.Configure(card.Id, card.Label, GetRootCanvasRect(), this, true);
        }

        private void CreateRouteBoard(Transform parent)
        {
            var panel = CreateColumnPanel("Backend Route Board", parent, 0.66f, BoardColor);
            AddOutline(panel.gameObject, new Color(0.55f, 0.67f, 0.75f));

            GhostUITheme.Label(
                "Board Heading",
                panel,
                "FROM FACT TO REPLY",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 30f;

            GhostUITheme.Label(
                "Board Guide",
                panel,
                "The dialogue route is already tested. Complete only what happens after it chooses lab hours.",
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft).gameObject.AddComponent<LayoutElement>().preferredHeight = 38f;

            var row = GhostUITheme.Panel("Role Row", panel, Color.clear).rectTransform;
            SetHeight(row, 260f);
            var rowLayout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 10f;
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = true;
            rowLayout.childForceExpandHeight = true;

            var roles = Act6BackendResponseData.CreateRoleOrder();
            for (var index = 0; index < roles.Count; index++)
            {
                CreateRoleSocket(row, roles[index], index + 1);
                if (index < roles.Count - 1)
                {
                    var arrow = GhostUITheme.Label(
                        "Arrow " + index,
                        row,
                        "->",
                        GhostUITheme.TitleSize,
                        FontStyle.Bold,
                        TextAnchor.MiddleCenter,
                        new Color(0.28f, 0.44f, 0.50f));
                    var arrowElement = arrow.gameObject.AddComponent<LayoutElement>();
                    arrowElement.minWidth = 34f;
                    arrowElement.preferredWidth = 34f;
                }
            }

            CreatePlaybackPanel(panel);
            CreateActionRow(panel);
        }

        private void CreateRoleSocket(Transform parent, string roleId, int number)
        {
            var placedCardId = controller.GetPlacedCardId(roleId);
            var hasCard = !string.IsNullOrWhiteSpace(placedCardId);
            var tested = controller.LastValidation != null;
            var correct = tested && controller.LastValidation.IsRoleCorrect(roleId);
            var failed = tested && !correct;
            var active = controller.CurrentPhase == Act6BackendPhase.Playback &&
                controller.ActivePlaybackStep != null &&
                string.Equals(
                    controller.ActivePlaybackStep.RoleId,
                    roleId,
                    StringComparison.Ordinal);
            var selected = controller.CurrentPhase == Act6BackendPhase.Configure &&
                !string.IsNullOrWhiteSpace(controller.SelectedCardId);
            var color = active
                ? ActiveColor
                : correct
                    ? SuccessColor
                    : failed
                        ? FailureColor
                        : selected
                            ? SelectedColor
                            : EmptyColor;

            var socket = GhostUITheme.DropZone("Role " + roleId, parent, color).rectTransform;
            socket.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            AddOutline(
                socket.gameObject,
                active
                    ? new Color(0.10f, 0.60f, 0.55f)
                    : correct
                        ? new Color(0.28f, 0.62f, 0.36f)
                        : failed
                            ? new Color(0.72f, 0.31f, 0.24f)
                            : new Color(0.54f, 0.57f, 0.65f));

            var layout = AddVerticalLayout(socket, new RectOffset(12, 12, 10, 10), 6f);
            GhostUITheme.Label(
                "Role Number",
                socket,
                number + ". " + Act6BackendResponseData.GetRoleLabel(roleId),
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.20f, 0.33f, 0.38f)).gameObject.AddComponent<LayoutElement>().preferredHeight = 26f;

            var label = hasCard
                ? Act6BackendResponseData.GetCard(placedCardId).Label
                : "Drop or click a card here";
            GhostUITheme.Label(
                "Placed Card",
                socket,
                label,
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 78f;

            string detail;
            if (!hasCard)
            {
                detail = GetEmptyRoleGuide(roleId);
            }
            else if (!tested)
            {
                detail = "PLACED - run the route to test this responsibility.";
            }
            else if (correct)
            {
                detail = "VERIFIED\n" + Act6BackendResponseData.GetCard(placedCardId).JobLine;
            }
            else
            {
                detail = "NEEDS REPAIR\nThis responsibility failed the route validator.";
            }

            var detailText = GhostUITheme.Label(
                "Role Detail",
                socket,
                detail,
                GhostUITheme.SmallSize,
                tested ? FontStyle.Bold : FontStyle.Normal,
                TextAnchor.UpperLeft,
                correct
                    ? GhostUITheme.Good
                    : failed
                        ? GhostUITheme.Bad
                        : GhostUITheme.InkSoft);
            detailText.lineSpacing = 1.02f;
            detailText.gameObject.AddComponent<LayoutElement>().preferredHeight = 86f;

            var drop = socket.gameObject.AddComponent<Act6BackendSlotDropView>();
            drop.Configure(roleId, this);

            if (hasCard)
            {
                var drag = socket.gameObject.AddComponent<Act6BackendCardDragView>();
                var card = Act6BackendResponseData.GetCard(placedCardId);
                drag.Configure(card.Id, card.Label, GetRootCanvasRect(), this, false);
            }
        }

        private static string GetEmptyRoleGuide(string roleId)
        {
            switch (roleId)
            {
                case Act6BackendResponseData.DataSourceRoleId:
                    return "Which stored system owns closing_time?";
                case Act6BackendResponseData.ActionRoleId:
                    return "Which operation requests closing_time?";
                default:
                    return "Which sentence uses {closing_time}?";
            }
        }

        private void CreatePlaybackPanel(Transform parent)
        {
            var panel = GhostUITheme.Card(
                "Playback",
                parent,
                controller.CurrentPhase == Act6BackendPhase.Playback
                    ? ActiveColor
                    : new Color(0.92f, 0.96f, 0.98f)).rectTransform;
            SetHeight(panel, 116f);
            var layout = AddVerticalLayout(panel, new RectOffset(14, 14, 9, 9), 4f);

            GhostUITheme.Label(
                "Playback Heading",
                panel,
                controller.ActivePlaybackStep == null
                    ? "RUN PREVIEW"
                    : controller.ActivePlaybackStep.Title,
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 26f;

            var line = controller.ActivePlaybackStep == null
                ? "Run the route to see request -> backend result -> generated reply as separate stages."
                : controller.ActivePlaybackStep.Line;
            GhostUITheme.Label(
                "Playback Line",
                panel,
                line,
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 66f;
        }

        private void CreateActionRow(Transform parent)
        {
            var row = GhostUITheme.Panel("Action Row", parent, Color.clear).rectTransform;
            SetHeight(row, 64f);
            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var feedback = GhostUITheme.Label(
                "Feedback",
                row,
                GetFeedbackText(),
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                controller.LastValidation != null && !controller.LastValidation.IsCorrect
                    ? GhostUITheme.Bad
                    : GhostUITheme.Ink);
            feedback.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var action = GhostUITheme.PushButton(row, GetActionLabel(), GetActionColor(), 250f);
            action.onClick.AddListener(HandlePrimaryAction);
        }

        private string GetFeedbackText()
        {
            if (controller.CurrentPhase == Act6BackendPhase.Complete)
            {
                return "Source, action, and response all performed their own job.";
            }

            if (controller.LastValidation != null && !controller.LastValidation.IsCorrect)
            {
                return "First broken role: " +
                    Act6BackendResponseData.GetRoleLabel(
                        controller.LastValidation.FirstBrokenRoleId) + ".";
            }

            var placed = 0;
            foreach (var roleId in Act6BackendResponseData.CreateRoleOrder())
            {
                if (!string.IsNullOrWhiteSpace(controller.GetPlacedCardId(roleId)))
                {
                    placed++;
                }
            }

            return placed + "/3 responsibilities filled. Raw backend data is not a reply until RESPONSE phrases it.";
        }

        private string GetActionLabel()
        {
            switch (controller.CurrentPhase)
            {
                case Act6BackendPhase.Playback:
                    return controller.PlaybackIndex >= controller.PlaybackSteps.Count - 1
                        ? "Finish the route"
                        : "Next responsibility";
                case Act6BackendPhase.Complete:
                    return "Complete Chapter 6";
                default:
                    return controller.LastValidation != null &&
                        !controller.LastValidation.IsCorrect
                        ? "Try the full route again"
                        : "Run the full route";
            }
        }

        private Color GetActionColor()
        {
            if (controller.CurrentPhase == Act6BackendPhase.Complete)
            {
                return new Color(1f, 0.84f, 0.45f);
            }

            return controller.CurrentPhase == Act6BackendPhase.Playback
                ? SuccessColor
                : new Color(0.77f, 0.89f, 1f);
        }

        private void HandlePrimaryAction()
        {
            switch (controller.CurrentPhase)
            {
                case Act6BackendPhase.Configure:
                    controller.RunRoute();
                    return;
                case Act6BackendPhase.Playback:
                    controller.AdvancePlayback();
                    return;
                case Act6BackendPhase.Complete:
                    GhostNarrativeState.SetPendingDebriefAct(GhostNarrativeState.Act6Id);
                    SceneManager.LoadScene(ShellSceneNames.GameShellSceneName);
                    return;
            }
        }

        private RectTransform GetRootCanvasRect()
        {
            var canvas = GetComponentInParent<Canvas>();
            return canvas == null ? (RectTransform)transform : (RectTransform)canvas.transform;
        }

        private static RectTransform CreateColumnPanel(
            string name,
            Transform parent,
            float flexibleWidth,
            Color color)
        {
            var panel = GhostUITheme.Panel(name, parent, color).rectTransform;
            var element = panel.gameObject.AddComponent<LayoutElement>();
            element.flexibleWidth = flexibleWidth;
            element.minWidth = 0f;
            AddVerticalLayout(panel, new RectOffset(14, 14, 12, 12), 8f);
            return panel;
        }

        private static VerticalLayoutGroup AddVerticalLayout(
            Transform parent,
            RectOffset padding,
            float spacing)
        {
            var layout = parent.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = padding;
            layout.spacing = spacing;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return layout;
        }




        private static void SetHeight(RectTransform rect, float height)
        {
            var element = rect.gameObject.AddComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
        }

        private static void AddOutline(GameObject target, Color color)
        {
            var outline = target.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }

        private static Color GetRoleColor(string roleId)
        {
            switch (roleId)
            {
                case Act6BackendResponseData.DataSourceRoleId:
                    return new Color(0.84f, 0.92f, 1f);
                case Act6BackendResponseData.ActionRoleId:
                    return new Color(0.84f, 0.95f, 0.87f);
                default:
                    return new Color(1f, 0.91f, 0.75f);
            }
        }

        private static GhostMood MapMood(Act6BackendMood mood)
        {
            switch (mood)
            {
                case Act6BackendMood.Confused:
                    return GhostMood.Confused;
                case Act6BackendMood.Happy:
                    return GhostMood.Happy;
                default:
                    return GhostMood.Neutral;
            }
        }


        private static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
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
