using Ghost.Presentation.Common;
using System;
using System.Collections.Generic;
using Ghost.Presentation.Characters;
using Ghost.Presentation.GhostAvatar;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.VoicePipeline;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Ghost.Presentation.Act6VoicePipeline
{
    public sealed class FinalChapterConversationPresenter : MonoBehaviour,
        IAct6PipelineInteractionHost,
        IFinalChapterWireHost
    {
        private const float WireThickness = 4f;
        private const float PortSize = 20f;
        private const float CardWidth = 196f;

        private static readonly Color PageColor = new Color(0.95f, 0.97f, 0.95f);
        private static readonly Color ConversationColor = new Color(0.90f, 0.96f, 1f);
        private static readonly Color PaletteColor = new Color(0.95f, 0.93f, 0.99f);
        private static readonly Color BoardColor = new Color(0.98f, 0.99f, 1f);
        private static readonly Color EmptyColor = new Color(0.92f, 0.94f, 0.95f);
        private static readonly Color SelectedColor = new Color(1f, 0.84f, 0.42f);
        private static readonly Color SuccessColor = new Color(0.80f, 0.95f, 0.83f);
        private static readonly Color FailureColor = new Color(1f, 0.84f, 0.80f);
        private static readonly Color ActiveColor = new Color(0.68f, 0.91f, 0.89f);
        private static readonly Color WireColor = new Color(0.24f, 0.52f, 0.46f);
        private static readonly Color PortColor = new Color(0.38f, 0.56f, 0.62f);
        private static readonly Color UnknownChipColor = new Color(0.90f, 0.90f, 0.93f);
        private static readonly Color BinColor = new Color(1f, 0.90f, 0.88f);
        private static readonly Color BinHighlightColor = new Color(1f, 0.72f, 0.68f);

        [SerializeField] private bool renderOnStart;

        private FinalChapterConversationController controller;
        private Act6EndingSequence endingSequence;
        private string selectedComponentId = string.Empty;

        // Rebuilt from scratch on every render, alongside the cards the ports belong to.
        private readonly Dictionary<string, FinalChapterRoutePortView> outPorts =
            new Dictionary<string, FinalChapterRoutePortView>();

        private readonly Dictionary<string, FinalChapterRoutePortView> inPorts =
            new Dictionary<string, FinalChapterRoutePortView>();

        private RectTransform wireLayer;
        private RectTransform nodeLayer;
        private RectTransform routeBin;
        private Image activeDragWire;
        private FinalChapterRoutePortView dragAnchorPort;

        public FinalChapterConversationController Controller => controller;

        private void Start()
        {
            if (renderOnStart)
            {
                RenderSampleData();
            }
        }

        private void OnDestroy()
        {
            if (controller != null)
            {
                controller.StateChanged -= RenderState;
            }
        }

        public void Configure(bool shouldRenderOnStart)
        {
            renderOnStart = shouldRenderOnStart;
        }

        public void RenderSampleData()
        {
            EnsureEventSystem();
            if (controller != null)
            {
                controller.StateChanged -= RenderState;
            }

            controller = new FinalChapterConversationController();
            controller.StateChanged += RenderState;
            selectedComponentId = string.Empty;
            RenderState();
        }
        public void StartEndingForTesting()
        {
            if (controller == null)
            {
                RenderSampleData();
            }

            controller.StartEndingForTesting();
        }


        public void SelectComponent(string componentId)
        {
            selectedComponentId = componentId ?? string.Empty;
            RenderState();
        }

        public void DropComponentOnMainSlot(string componentId, int slotIndex)
        {
            PlaceOnSlot(componentId, slotIndex);
        }

        public void DropComponentOnBackendSlot(string componentId)
        {
            controller?.ToggleBackend(componentId);
        }

        public void PlaceSelectedOnMainSlot(int slotIndex)
        {
            // Clicking a slot with nothing picked up empties it. Placement has to be reversible.
            if (string.IsNullOrEmpty(selectedComponentId))
            {
                ClearSlotAt(slotIndex);
                return;
            }

            PlaceOnSlot(selectedComponentId, slotIndex);
        }

        public void PlaceSelectedOnBackendSlot()
        {
            if (!string.IsNullOrEmpty(selectedComponentId))
            {
                controller?.ToggleBackend(selectedComponentId);
                selectedComponentId = string.Empty;
            }
        }

        public void RemoveOption(string optionId)
        {
            if (controller == null || string.IsNullOrEmpty(optionId))
            {
                return;
            }

            foreach (var slot in controller.ActiveVisitor.Slots)
            {
                if (string.Equals(controller.GetSlotAssignment(slot.Id), optionId, StringComparison.Ordinal))
                {
                    controller.ClearSlot(slot.Id);
                    return;
                }
            }

            foreach (var roleId in FinalChapterConversationData.ResponseRoleIds)
            {
                if (string.Equals(controller.GetResponsePart(roleId), optionId, StringComparison.Ordinal))
                {
                    controller.ClearResponseRole(roleId);
                    return;
                }
            }
        }

        public void RemoveSelectedOption()
        {
            RemoveOption(selectedComponentId);
            selectedComponentId = string.Empty;
        }

        /// <summary>
        /// The drop views speak in slot indices. What an index means depends on the stage on screen -
        /// an entity slot during Details, a response role during Response.
        /// </summary>
        private void PlaceOnSlot(string componentId, int slotIndex)
        {
            if (controller == null || string.IsNullOrEmpty(componentId))
            {
                return;
            }

            var visitor = controller.ActiveVisitor;
            if (controller.ActiveStage == FinalChapterStage.Entities &&
                slotIndex >= 0 && slotIndex < visitor.Slots.Count)
            {
                controller.AssignFragment(componentId, visitor.Slots[slotIndex].Id);
            }
            else if (controller.ActiveStage == FinalChapterStage.Response &&
                slotIndex >= 0 && slotIndex < FinalChapterConversationData.ResponseRoleIds.Length)
            {
                controller.PlaceResponsePart(
                    componentId,
                    FinalChapterConversationData.ResponseRoleIds[slotIndex]);
            }

            selectedComponentId = string.Empty;
        }

        private void ClearSlotAt(int slotIndex)
        {
            if (controller == null)
            {
                return;
            }

            var visitor = controller.ActiveVisitor;
            if (controller.ActiveStage == FinalChapterStage.Entities &&
                slotIndex >= 0 && slotIndex < visitor.Slots.Count)
            {
                controller.ClearSlot(visitor.Slots[slotIndex].Id);
            }
            else if (controller.ActiveStage == FinalChapterStage.Response &&
                slotIndex >= 0 && slotIndex < FinalChapterConversationData.ResponseRoleIds.Length)
            {
                controller.ClearResponseRole(FinalChapterConversationData.ResponseRoleIds[slotIndex]);
            }
        }

        private void RenderState()
        {
            if (controller == null)
            {
                return;
            }

            Act6PipelinePartDragView.ClearActivePreviews();
            ClearChildren(transform);

            // The ports and the wire layer were children of what was just destroyed. Holding on to the
            // references would leave the wire drawing chasing objects that no longer exist.
            outPorts.Clear();
            inPorts.Clear();
            wireLayer = null;
            nodeLayer = null;
            routeBin = null;
            activeDragWire = null;
            dragAnchorPort = null;

            ConfigureRoot();

            if (controller.CurrentPhase == FinalChapterPhase.Ending)
            {
                CreateEndingOverlay();
                Canvas.ForceUpdateCanvases();
                endingSequence.Play();
                return;
            }

            CreateHeader();
            CreateObjectiveStrip();

            if (controller.CurrentPhase == FinalChapterPhase.Onboarding)
            {
                CreateOnboarding();
            }
            else
            {
                CreateConversationPanel();
                CreateMainBody();
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            Canvas.ForceUpdateCanvases();

            // Wires are drawn from where the dots ended up, so this has to come after the layout has
            // actually run - not from inside the method that created the cards.
            DrawRouteWires();
        }

        private void ConfigureRoot()
        {
            var root = (RectTransform)transform;
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;

            var image = GetComponent<Image>();
            if (image == null)
            {
                image = gameObject.AddComponent<Image>();
            }

            image.color = PageColor;
            image.raycastTarget = true;

            var layout = GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(24, 24, 16, 16);
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
            AddHeight(header, 44f);
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
                "Final Chapter: Three Conversations",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);
            title.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            for (var index = 0; index < controller.Visitors.Count; index++)
            {
                var isActive = index == controller.ActiveVisitorIndex;
                var isComplete = controller.IsVisitorCompleted(index);
                var badge = GhostUITheme.Chip(
                    "Visitor Progress " + (index + 1),
                    header,
                    isComplete
                        ? SuccessColor
                        : isActive
                            ? ActiveColor
                            : EmptyColor).rectTransform;
                var badgeElement = badge.gameObject.AddComponent<LayoutElement>();
                badgeElement.preferredWidth = 48f;
                badgeElement.minWidth = 48f;
                GhostUITheme.Label(
                    "Label",
                    badge,
                    isComplete ? "OK" : (index + 1).ToString(),
                    GhostUITheme.BodySize,
                    FontStyle.Bold,
                    TextAnchor.MiddleCenter,
                    GhostUITheme.Ink, Vector2.zero);
            }
        }

        private void CreateObjectiveStrip()
        {
            var strip = GhostUITheme.Panel(
                "Objective Strip",
                transform,
                new Color(0.12f, 0.23f, 0.24f)).rectTransform;
            AddHeight(strip, 40f);
            strip.GetComponent<LayoutElement>().flexibleHeight = 0f;
            GhostUITheme.Label(
                "Objective",
                strip,
                GetObjectiveText(),
                GhostUITheme.HeadingSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                GhostUITheme.InkOnDark, Vector2.zero);
        }

        private string GetObjectiveText()
        {
            switch (controller.CurrentPhase)
            {
                case FinalChapterPhase.Onboarding:
                    return "Final chapter: one voice, three very different visitors.";
                case FinalChapterPhase.Playback:
                    return "Read the trace - the run stops at the first step that does not hold.";
                case FinalChapterPhase.ReadyForEnding:
                case FinalChapterPhase.Ending:
                    return "All three conversations worked.";
                default:
                    return "Visitor " + (controller.ActiveVisitorIndex + 1) + " of " +
                        controller.Visitors.Count + " - step " + (controller.ActiveStageIndex + 1) +
                        " of " + controller.ActiveVisitor.Stages.Count + ": " +
                        FinalChapterConversationData.GetStageLabel(controller.ActiveStage);
            }
        }

        private void CreateOnboarding()
        {
            var panel = GhostUITheme.Panel(
                "Final Chapter Onboarding",
                transform,
                new Color(1f, 0.96f, 0.80f)).rectTransform;
            // 28 padding + heading + 240 explanation + the start button, with the gaps between them.
            // Left flexible it would soak up every pixel the other blocks no longer take.
            AddHeight(panel, 362f);
            panel.GetComponent<LayoutElement>().flexibleHeight = 0f;
            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 14, 14);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Lily Heading",
                panel,
                "Lily: One voice, three very different visitors",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);
            GhostUITheme.Label(
                "Lily Explanation",
                panel,
                "Um... everything we repaired is in Ghost now. But nobody is going to tell you which "
                    + "part a conversation needs. That is the whole difference.\n\n"
                    + "Three visitors. The first only has to be understood. The second names two of the "
                    + "same thing, so a route has to be built for it. The third one... is me.\n\n"
                    + "Work through the steps along the top, then run the conversation once and read the "
                    + "trace before you trust it. Each visitor is separate - if one breaks, only that one goes back.",
                GhostUITheme.TitleSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 240f;

            var start = GhostUITheme.PushButton(
                panel,
                "Meet visitor 1",
                new Color(0.72f, 0.90f, 0.88f),
                220f);
            start.onClick.AddListener(controller.FinishOnboarding);
        }

        private void CreateConversationPanel()
        {
            var panel = GhostUITheme.Panel("Current Conversation", transform, ConversationColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.55f, 0.69f, 0.78f), new Vector2(1.5f, -1.5f));
            AddHeight(panel, controller.IsLilyVisitor ? 198f : 178f);
            // Same trap as the header: the inner group force-expands height, so without this the
            // panel reports flexible height and stretches down the rest of the page.
            panel.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var layout = panel.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 14f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            if (controller.IsLilyVisitor)
            {
                CreateLilyConversation(panel);
                return;
            }

            var faceRoot = GhostUITheme.Panel("Ghost Face", panel, Color.clear).rectTransform;
            var faceElement = faceRoot.gameObject.AddComponent<LayoutElement>();
            faceElement.minWidth = 132f;
            faceElement.preferredWidth = 132f;
            faceElement.flexibleWidth = 0f;
            faceRoot.gameObject.AddComponent<GhostFaceView>().SetMood(MapMood(controller.CurrentMood));

            var column = GhostUITheme.Panel("Conversation Text", panel, Color.clear).rectTransform;
            column.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var columnLayout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            columnLayout.spacing = 5f;
            columnLayout.childControlWidth = true;
            columnLayout.childControlHeight = true;
            columnLayout.childForceExpandWidth = true;
            columnLayout.childForceExpandHeight = false;

            var visitor = controller.ActiveVisitor;
            GhostUITheme.Label(
                "Speaker",
                column,
                visitor.SpeakerName + " - visitor " + (controller.ActiveVisitorIndex + 1) +
                    " of " + controller.Visitors.Count,
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.24f, 0.36f, 0.46f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 24f;

            GhostUITheme.Label(
                "Visitor Message",
                column,
                visitor.Message,
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 64f;

            GhostUITheme.Label(
                "Status",
                column,
                controller.StatusLine,
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.34f, 0.38f, 0.44f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 40f;
        }

        /// <summary>
        /// Lily is the only visitor Ghost already knows, and the last thing the player does is answer
        /// her. So her stretch is staged as a conversation rather than a ticket: she is on the left with
        /// what she said, Ghost is on the right with what he is making of it, and the two of them face
        /// each other across the panel instead of both facing the player.
        /// </summary>
        private void CreateLilyConversation(Transform panel)
        {
            var portrait = new GameObject("Lily Portrait", typeof(RectTransform)).GetComponent<RectTransform>();
            portrait.SetParent(panel, false);
            var portraitElement = portrait.gameObject.AddComponent<LayoutElement>();
            portraitElement.minWidth = 104f;
            portraitElement.preferredWidth = 104f;
            portraitElement.flexibleWidth = 0f;

            var portraitImage = portrait.gameObject.AddComponent<Image>();
            portraitImage.sprite = LilyPixelPortraitFactory.GetPortrait();
            portraitImage.preserveAspect = true;
            portraitImage.raycastTarget = false;

            var visitor = controller.ActiveVisitor;
            CreateSpeechColumn(
                panel,
                "Lily Speech",
                visitor.SpeakerName + " - visitor " + (controller.ActiveVisitorIndex + 1) +
                    " of " + controller.Visitors.Count,
                visitor.Message,
                TextAnchor.MiddleLeft,
                TextAnchor.UpperLeft);

            CreateSpeechColumn(
                panel,
                "Ghost Speech",
                "Ghost",
                controller.StatusLine,
                TextAnchor.MiddleRight,
                TextAnchor.UpperRight);

            var faceRoot = GhostUITheme.Panel("Ghost Face", panel, Color.clear).rectTransform;
            var faceElement = faceRoot.gameObject.AddComponent<LayoutElement>();
            faceElement.minWidth = 124f;
            faceElement.preferredWidth = 124f;
            faceElement.flexibleWidth = 0f;
            faceRoot.gameObject.AddComponent<GhostFaceView>().SetMood(MapMood(controller.CurrentMood));
        }

        private void CreateSpeechColumn(
            Transform parent,
            string name,
            string speaker,
            string line,
            TextAnchor speakerAlignment,
            TextAnchor lineAlignment)
        {
            var column = GhostUITheme.Panel(name, parent, Color.clear).rectTransform;
            column.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var layout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Speaker",
                column,
                speaker,
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                speakerAlignment,
                new Color(0.24f, 0.36f, 0.46f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 24f;

            GhostUITheme.Label(
                "Line",
                column,
                line,
                GhostUITheme.BodySize,
                FontStyle.Normal,
                lineAlignment,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 112f;
        }

        private void CreateMainBody()
        {
            var body = GhostUITheme.Panel("Main Body", transform, Color.clear).rectTransform;
            var bodyElement = body.gameObject.AddComponent<LayoutElement>();
            bodyElement.flexibleHeight = 1f;
            bodyElement.minHeight = 240f;

            var layout = body.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            if (controller.CurrentPhase == FinalChapterPhase.Playback)
            {
                CreateTraceBody(body);
                return;
            }

            if (controller.CurrentPhase == FinalChapterPhase.ReadyForEnding)
            {
                CreateEndingPrompt(body);
                return;
            }

            CreateStageStrip(body);
            CreateStageContent(body);
            CreateStageControls(body);
        }

        /// <summary>
        /// One chip per stage this visitor asks for. Visitor 1 shows two, Lily shows five - the strip
        /// is what makes the escalation visible instead of just felt.
        /// </summary>
        private void CreateStageStrip(Transform parent)
        {
            var strip = GhostUITheme.Panel("Stage Strip", parent, Color.clear).rectTransform;
            // PushButton floors its own height at 42, so a shorter strip just squeezes the chips.
            AddHeight(strip, 46f);
            strip.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var layout = strip.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            var stages = controller.ActiveVisitor.Stages;
            for (var i = 0; i < stages.Count; i++)
            {
                var index = i;
                var isActive = i == controller.ActiveStageIndex;
                var button = GhostUITheme.PushButton(
                    strip,
                    (i + 1) + ". " + FinalChapterConversationData.GetStageLabel(stages[i]),
                    isActive ? ActiveColor : (controller.HasVisitedStage(i) ? BoardColor : EmptyColor),
                    0f);
                button.onClick.AddListener(() => controller.GoToStage(index));
            }
        }

        private void CreateStageContent(Transform parent)
        {
            var board = GhostUITheme.Panel("Stage Board", parent, BoardColor).rectTransform;
            AddOutline(board.gameObject, new Color(0.62f, 0.66f, 0.74f), new Vector2(1.5f, -1.5f));
            var element = board.gameObject.AddComponent<LayoutElement>();
            element.flexibleHeight = 1f;
            element.minHeight = 200f;

            var layout = board.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            switch (controller.ActiveStage)
            {
                case FinalChapterStage.Intent:
                    CreateIntentStage(board);
                    break;
                case FinalChapterStage.Entities:
                    CreateEntityStage(board);
                    break;
                case FinalChapterStage.Dialogue:
                    CreateDialogueStage(board);
                    break;
                case FinalChapterStage.Confidence:
                    CreateConfidenceStage(board);
                    break;
                case FinalChapterStage.Backend:
                    CreateBackendStage(board);
                    break;
                case FinalChapterStage.Response:
                    CreateResponseStage(board);
                    break;
            }
        }

        // ------------------------------------------------------------------ intent

        private void CreateIntentStage(Transform parent)
        {
            var column = CreateColumn("Intent Options", parent, PaletteColor, 1f);
            GhostUITheme.Label(
                "Intent Title",
                column,
                "All four are things a visitor could want. Only one is what this one wants.",
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.36f, 0.34f, 0.44f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 26f;

            foreach (var optionId in controller.ActiveVisitor.IntentOptionIds)
            {
                var option = FinalChapterConversationData.GetOption(optionId);
                var chosen = string.Equals(optionId, controller.SelectedIntentId, StringComparison.Ordinal);
                var card = GhostUITheme.Panel("Intent " + optionId, column, chosen ? SelectedColor : EmptyColor)
                    .rectTransform;
                AddOutline(card.gameObject, new Color(0.60f, 0.62f, 0.70f), new Vector2(1f, -1f));
                AddHeight(card, 62f);
                card.GetComponent<LayoutElement>().flexibleHeight = 0f;

                var cardLayout = card.gameObject.AddComponent<VerticalLayoutGroup>();
                cardLayout.padding = new RectOffset(10, 10, 6, 6);
                cardLayout.spacing = 2f;
                cardLayout.childControlWidth = true;
                cardLayout.childControlHeight = true;
                cardLayout.childForceExpandWidth = true;
                cardLayout.childForceExpandHeight = false;

                GhostUITheme.Label(
                    "Label",
                    card,
                    option.Label,
                    GhostUITheme.BodySize,
                    FontStyle.Bold,
                    TextAnchor.MiddleLeft,
                    GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 24f;
                GhostUITheme.Label(
                    "Description",
                    card,
                    option.Description,
                    GhostUITheme.SmallSize,
                    FontStyle.Normal,
                    TextAnchor.UpperLeft,
                    new Color(0.36f, 0.36f, 0.42f)).gameObject.AddComponent<LayoutElement>()
                    .preferredHeight = 24f;

                var id = optionId;
                var button = card.gameObject.AddComponent<Button>();
                button.targetGraphic = card.GetComponent<Image>();
                button.onClick.AddListener(() => controller.SelectIntent(id));
            }
        }

        // ------------------------------------------------------------------ entities

        private void CreateEntityStage(Transform parent)
        {
            var visitor = controller.ActiveVisitor;

            var left = CreateColumn("Fragments", parent, PaletteColor, 1f);
            GhostUITheme.Label(
                "Fragments Title",
                left,
                "Pieces of what they said - and one card for what they did not",
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.36f, 0.34f, 0.44f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 22f;

            foreach (var fragment in visitor.Fragments)
            {
                if (controller.IsFragmentUsed(fragment.Id))
                {
                    continue;
                }

                CreateDraggableChip(
                    left,
                    fragment.Id,
                    fragment.Text,
                    fragment.IsUnknownMarker ? UnknownChipColor : new Color(0.86f, 0.92f, 1f),
                    fragment.IsUnknownMarker ? FontStyle.Italic : FontStyle.Bold);
            }

            var right = CreateColumn("Slots", parent, BoardColor, 1f);
            GhostUITheme.Label(
                "Slots Title",
                right,
                "What the reply needs",
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.30f, 0.38f, 0.46f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 22f;

            for (var i = 0; i < visitor.Slots.Count; i++)
            {
                var slot = visitor.Slots[i];
                var assigned = controller.GetSlotAssignment(slot.Id);
                CreateDropSlot(
                    right,
                    i,
                    slot.Label,
                    string.IsNullOrEmpty(assigned)
                        ? "(empty)"
                        : FinalChapterConversationData.GetFragment(visitor, assigned).Text,
                    !string.IsNullOrEmpty(assigned));
            }

            GhostUITheme.Label(
                "Slot Hint",
                right,
                "If the message never answers a slot, say so with the grey card. Do not guess, and do " +
                    "not leave it blank - an unanswered question is worth recording.",
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.42f, 0.44f, 0.50f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 34f;
        }

        // ------------------------------------------------------------------ dialogue

        /// <summary>
        /// Chapter 3's board, narrowed to one conversation. The palette on the left holds the steps this
        /// visitor could be routed through; nothing is on the map until the player drags it there, and
        /// the route only exists once the cards are wired from the visitor's message through to Ghost's
        /// reply. The column on the right reads those wires back as a numbered route, so what has been
        /// drawn always has a plain sentence beside it.
        /// </summary>
        private void CreateDialogueStage(Transform parent)
        {
            CreateRoutePalette(parent);
            CreateRouteCanvas(parent);
            CreateRouteReadout(parent);
        }

        private void CreateRoutePalette(Transform parent)
        {
            var column = CreateColumn("Route Palette", parent, PaletteColor, 1f);
            GhostUITheme.Label(
                "Palette Title",
                column,
                "Steps you could use",
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.36f, 0.34f, 0.44f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 22f;

            var remaining = 0;
            foreach (var stepId in controller.ActiveVisitor.RouteStepPalette)
            {
                if (controller.IsStepPlaced(stepId))
                {
                    continue;
                }

                remaining++;
                CreatePaletteCard(column, stepId);
            }

            GhostUITheme.Label(
                "Palette Hint",
                column,
                remaining == 0
                    ? "Every step is on the map now. Wire up the ones this visitor needs, and drop the " +
                        "rest in the bin."
                    : "Drag a step onto the map, then join the dots.",
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.46f, 0.46f, 0.54f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 46f;
        }

        private void CreatePaletteCard(Transform parent, string stepId)
        {
            var step = FinalChapterConversationData.GetRouteStep(stepId);
            var card = GhostUITheme.Panel("Palette " + stepId, parent, new Color(0.88f, 0.95f, 0.90f))
                .rectTransform;
            AddOutline(card.gameObject, new Color(0.60f, 0.68f, 0.64f), new Vector2(1f, -1f));
            AddHeight(card, 60f);
            card.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 5, 5);
            layout.spacing = 1f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Label",
                card,
                step.Label,
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 20f;

            GhostUITheme.Label(
                "Description",
                card,
                step.Description,
                GhostUITheme.TinySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.38f, 0.38f, 0.44f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 24f;

            card.gameObject.AddComponent<FinalChapterRoutePaletteDragView>().Initialize(this, stepId);
        }

        private void CreateRouteCanvas(Transform parent)
        {
            var column = CreateColumn("Reply Map", parent, BoardColor, 2.6f);
            GhostUITheme.Label(
                "Canvas Title",
                column,
                "Move cards freely. Drag from a card's bottom dot to the next card's top dot.",
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.30f, 0.38f, 0.46f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 24f;

            var board = GhostUITheme.Panel("Map", column, new Color(0.97f, 0.98f, 1f)).rectTransform;
            AddOutline(board.gameObject, new Color(0.62f, 0.66f, 0.74f), new Vector2(1.5f, -1.5f));
            var boardElement = board.gameObject.AddComponent<LayoutElement>();
            boardElement.flexibleHeight = 1f;
            boardElement.minHeight = 260f;

            // No layout group on the map: the overlays fill it, and every card sits where it was put.
            // Wires go underneath, so a line crossing the map passes behind the cards it is not joining.
            wireLayer = CreateOverlayLayer("Wire Layer", board);
            nodeLayer = CreateOverlayLayer("Node Layer", board);
            wireLayer.SetAsFirstSibling();
            nodeLayer.SetAsLastSibling();

            CreateRouteCard(FinalChapterConversationData.RouteStartId, false, true, true);
            CreateRouteCard(FinalChapterConversationData.RouteEndId, true, false, true);

            foreach (var stepId in controller.ActiveVisitor.RouteStepPalette)
            {
                if (controller.IsStepPlaced(stepId))
                {
                    CreateRouteCard(stepId, true, true, false);
                }
            }

            CreateRouteBin(column);
        }

        private static RectTransform CreateOverlayLayer(string name, Transform parent)
        {
            var layer = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            layer.SetParent(parent, false);
            layer.anchorMin = Vector2.zero;
            layer.anchorMax = Vector2.one;
            layer.offsetMin = Vector2.zero;
            layer.offsetMax = Vector2.zero;
            layer.pivot = new Vector2(0.5f, 0.5f);
            return layer;
        }

        private void CreateRouteCard(string stepId, bool hasInPort, bool hasOutPort, bool isEndpoint)
        {
            var step = FinalChapterConversationData.GetRouteStep(stepId);
            var position = IndexOnRoute(stepId);
            var fill = isEndpoint
                ? new Color(0.99f, 0.91f, 0.71f)
                : (position > 0 ? new Color(0.82f, 0.95f, 0.87f) : new Color(0.94f, 0.96f, 0.99f));

            var card = GhostUITheme.Panel("Route Card " + stepId, nodeLayer, fill).rectTransform;
            AddOutline(card.gameObject, new Color(0.58f, 0.62f, 0.70f), new Vector2(1.5f, -1.5f));

            var spot = controller.GetStepPosition(stepId);
            card.anchorMin = spot;
            card.anchorMax = spot;
            card.pivot = new Vector2(0.5f, 0.5f);
            card.anchoredPosition = Vector2.zero;
            card.sizeDelta = new Vector2(CardWidth, isEndpoint ? 54f : 70f);

            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 6, 6);
            layout.spacing = 1f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Label",
                card,
                (position > 0 ? position + ". " : string.Empty) + step.Label,
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 20f;

            GhostUITheme.Label(
                "Description",
                card,
                step.Description,
                GhostUITheme.TinySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.40f, 0.40f, 0.46f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = isEndpoint ? 18f : 32f;

            if (hasInPort)
            {
                inPorts[stepId] = CreatePort(card, stepId, false);
            }

            if (hasOutPort)
            {
                outPorts[stepId] = CreatePort(card, stepId, true);
            }

            // The two ends move like anything else - an unreadable tangle is fixed by dragging things
            // apart. What they will not do is go in the bin; the controller refuses that.
            card.gameObject.AddComponent<FinalChapterRouteCardDragView>().Initialize(this, stepId);
        }

        private void CreateRouteBin(Transform parent)
        {
            var bin = GhostUITheme.Panel("Card Bin", parent, BinColor).rectTransform;
            AddOutline(bin.gameObject, new Color(0.80f, 0.60f, 0.58f), new Vector2(1f, -1f));
            AddHeight(bin, 34f);
            bin.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var layout = bin.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 4, 4);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            GhostUITheme.Label(
                "Bin Label",
                bin,
                "Drop a card here to take it off the map",
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(0.52f, 0.28f, 0.26f));

            routeBin = bin;
        }

        private FinalChapterRoutePortView CreatePort(Transform card, string stepId, bool isOutput)
        {
            var wired = isOutput
                ? !string.IsNullOrEmpty(controller.GetLinkTarget(stepId))
                : IsWireTarget(stepId);

            var dot = GhostUITheme.Panel(
                (isOutput ? "Out Port " : "In Port ") + stepId,
                card,
                wired ? WireColor : PortColor).rectTransform;

            // The card is a layout group; the dots hang off its edges instead of taking rows in it.
            dot.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            dot.anchorMin = new Vector2(0.5f, isOutput ? 0f : 1f);
            dot.anchorMax = dot.anchorMin;
            dot.pivot = new Vector2(0.5f, 0.5f);
            dot.anchoredPosition = Vector2.zero;
            dot.sizeDelta = new Vector2(PortSize, PortSize);

            var port = dot.gameObject.AddComponent<FinalChapterRoutePortView>();
            port.Initialize(this, stepId, isOutput);
            return port;
        }

        private void CreateRouteReadout(Transform parent)
        {
            var right = CreateColumn("Route So Far", parent, BoardColor, 1f);
            GhostUITheme.Label(
                "Readout Title",
                right,
                "The route as you have wired it",
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.30f, 0.38f, 0.46f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 22f;

            var ordered = controller.RouteStepIds;
            if (ordered.Count == 0)
            {
                GhostUITheme.Label(
                    "Readout Empty",
                    right,
                    "Nothing leaves their message yet. Drag the steps you need onto the map, then " +
                        "join them up until the last one reaches Ghost's reply.",
                    GhostUITheme.SmallSize,
                    FontStyle.Italic,
                    TextAnchor.UpperLeft,
                    new Color(0.44f, 0.46f, 0.52f)).gameObject.AddComponent<LayoutElement>()
                    .preferredHeight = 48f;
            }
            else
            {
                for (var i = 0; i < ordered.Count; i++)
                {
                    GhostUITheme.Label(
                        "Readout " + i,
                        right,
                        (i + 1) + ". " + FinalChapterConversationData.GetRouteStep(ordered[i]).Label,
                        GhostUITheme.SmallSize,
                        FontStyle.Normal,
                        TextAnchor.MiddleLeft,
                        GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>()
                        .preferredHeight = 22f;
                }

                GhostUITheme.Label(
                    "Readout Tail",
                    right,
                    ReachesTheReply()
                        ? "...and then Ghost replies."
                        : "...and then it stops. Nothing carries it through to the reply yet.",
                    GhostUITheme.SmallSize,
                    FontStyle.Italic,
                    TextAnchor.UpperLeft,
                    new Color(0.44f, 0.46f, 0.52f)).gameObject.AddComponent<LayoutElement>()
                    .preferredHeight = 34f;
            }

            GhostUITheme.Label(
                "Readout Hint",
                right,
                "One wire leaves each card. Click a bottom dot to take its wire down; drag a card to " +
                    "the bin to take it off the map.",
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.50f, 0.52f, 0.58f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 46f;

            var clear = GhostUITheme.PushButton(
                right,
                "Clear the map",
                new Color(1f, 0.88f, 0.84f),
                0f);
            var element = clear.gameObject.GetComponent<LayoutElement>() ??
                clear.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 38f;
            element.preferredHeight = 38f;
            element.flexibleHeight = 0f;
            clear.onClick.AddListener(() => controller.ClearRoute());
        }

        /// <summary>Where this card sits on the wired path, or 0 if the path never reaches it.</summary>
        private int IndexOnRoute(string stepId)
        {
            var ordered = controller.RouteStepIds;
            for (var i = 0; i < ordered.Count; i++)
            {
                if (string.Equals(ordered[i], stepId, StringComparison.Ordinal))
                {
                    return i + 1;
                }
            }

            return 0;
        }

        private bool IsWireTarget(string stepId)
        {
            foreach (var link in controller.RouteLinks)
            {
                if (string.Equals(link.ToId, stepId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ReachesTheReply()
        {
            var ordered = controller.RouteStepIds;
            var last = ordered.Count == 0
                ? FinalChapterConversationData.RouteStartId
                : ordered[ordered.Count - 1];

            return string.Equals(
                controller.GetLinkTarget(last),
                FinalChapterConversationData.RouteEndId,
                StringComparison.Ordinal);
        }

        // ------------------------------------------------------------------ cards on the map

        public void TryPlaceStepAtPointer(string stepId, PointerEventData eventData)
        {
            if (nodeLayer == null || eventData == null || controller == null)
            {
                return;
            }

            // Released somewhere that is not the map: nothing happens, and the step is still in the
            // palette. Dropping a card into empty space should not cost the player anything.
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    nodeLayer,
                    eventData.position,
                    eventData.pressEventCamera))
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    nodeLayer,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var localPoint))
            {
                return;
            }

            controller.PlaceRouteStep(stepId, ToNormalized(nodeLayer, localPoint));
        }

        public void MoveStepToPointer(string stepId, RectTransform card, PointerEventData eventData)
        {
            if (nodeLayer == null || card == null || eventData == null || controller == null)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    nodeLayer,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var localPoint))
            {
                return;
            }

            if (!controller.MoveRouteStep(stepId, ToNormalized(nodeLayer, localPoint)))
            {
                return;
            }

            // Applied straight to the card rather than through a re-render: rebuilding the page mid-drag
            // would destroy the object the pointer is holding.
            var spot = controller.GetStepPosition(stepId);
            card.anchorMin = spot;
            card.anchorMax = spot;
            card.anchoredPosition = Vector2.zero;
            card.SetAsLastSibling();

            SetBinHighlight(IsOverBin(eventData));
            DrawRouteWires();
        }

        public void CompleteStepDrag(string stepId, RectTransform card, PointerEventData eventData)
        {
            SetBinHighlight(false);

            if (controller == null)
            {
                return;
            }

            if (IsOverBin(eventData) && controller.RemoveRouteStep(stepId))
            {
                return;
            }

            DrawRouteWires();
        }

        private static Vector2 ToNormalized(RectTransform root, Vector2 localPoint)
        {
            var rect = root.rect;
            return new Vector2(
                Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x),
                Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y));
        }

        private bool IsOverBin(PointerEventData eventData)
        {
            return routeBin != null &&
                eventData != null &&
                RectTransformUtility.RectangleContainsScreenPoint(
                    routeBin,
                    eventData.position,
                    eventData.pressEventCamera);
        }

        private void SetBinHighlight(bool shouldHighlight)
        {
            var image = routeBin == null ? null : routeBin.GetComponent<Image>();
            if (image != null)
            {
                image.color = shouldHighlight ? BinHighlightColor : BinColor;
            }
        }

        // ------------------------------------------------------------------ wires

        public void BeginWireDrag(FinalChapterRoutePortView port, PointerEventData eventData)
        {
            if (port == null || wireLayer == null ||
                controller == null || controller.CurrentPhase != FinalChapterPhase.Configure)
            {
                return;
            }

            dragAnchorPort = port;
            DestroyActiveDragWire();
            activeDragWire = CreateWireImage(
                "Temporary Wire",
                new Color(WireColor.r, WireColor.g, WireColor.b, 0.55f));
            activeDragWire.transform.SetAsLastSibling();
            UpdateWireDrag(eventData);
        }

        public void UpdateWireDrag(PointerEventData eventData)
        {
            if (dragAnchorPort == null || activeDragWire == null || wireLayer == null || eventData == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                wireLayer,
                eventData.position,
                eventData.pressEventCamera,
                out var pointerLocal);

            DrawLine(
                activeDragWire.rectTransform,
                GetPortLocalCenter(dragAnchorPort.RectTransform),
                pointerLocal,
                WireThickness);
        }

        public void EndWireDrag()
        {
            dragAnchorPort = null;
            DestroyActiveDragWire();
        }

        public void CompleteWireDrop(FinalChapterRoutePortView outPort, FinalChapterRoutePortView inPort)
        {
            dragAnchorPort = null;
            DestroyActiveDragWire();

            if (outPort == null || inPort == null || controller == null)
            {
                return;
            }

            // The re-render that follows redraws every wire from the controller's links, so nothing
            // here has to touch the temporary one again.
            controller.LinkRouteSteps(outPort.StepId, inPort.StepId);
        }

        public void ClearWireFrom(FinalChapterRoutePortView outPort)
        {
            if (outPort != null && controller != null)
            {
                controller.RemoveRouteLink(outPort.StepId);
            }
        }

        private void DrawRouteWires()
        {
            if (wireLayer == null || controller == null)
            {
                return;
            }

            ClearChildren(wireLayer);

            foreach (var link in controller.RouteLinks)
            {
                if (!outPorts.TryGetValue(link.FromId, out var from) ||
                    !inPorts.TryGetValue(link.ToId, out var to))
                {
                    continue;
                }

                var line = CreateWireImage("Wire " + link.FromId + " to " + link.ToId, WireColor);
                DrawLine(
                    line.rectTransform,
                    GetPortLocalCenter(from.RectTransform),
                    GetPortLocalCenter(to.RectTransform),
                    WireThickness);
            }
        }

        private Image CreateWireImage(string name, Color color)
        {
            var line = new GameObject(name, typeof(RectTransform)).AddComponent<Image>();
            line.transform.SetParent(wireLayer, false);
            line.color = color;
            line.raycastTarget = false;
            return line;
        }

        private static void DrawLine(RectTransform line, Vector2 start, Vector2 end, float thickness)
        {
            var delta = end - start;
            line.anchorMin = new Vector2(0.5f, 0.5f);
            line.anchorMax = new Vector2(0.5f, 0.5f);
            line.pivot = new Vector2(0f, 0.5f);
            line.anchoredPosition = start;
            line.sizeDelta = new Vector2(delta.magnitude, thickness);
            line.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        }

        private Vector2 GetPortLocalCenter(RectTransform port)
        {
            if (port == null || wireLayer == null)
            {
                return Vector2.zero;
            }

            var worldCenter = port.TransformPoint(port.rect.center);
            var localCenter = wireLayer.InverseTransformPoint(worldCenter);
            return new Vector2(localCenter.x, localCenter.y);
        }

        private void DestroyActiveDragWire()
        {
            if (activeDragWire == null)
            {
                return;
            }

            var wireObject = activeDragWire.gameObject;
            activeDragWire = null;
            wireObject.SetActive(false);

            if (Application.isPlaying)
            {
                Destroy(wireObject);
            }
            else
            {
                DestroyImmediate(wireObject);
            }
        }

        // ------------------------------------------------------------------ confidence

        private void CreateConfidenceStage(Transform parent)
        {
            var visitor = controller.ActiveVisitor;
            var column = CreateColumn("Confidence", parent, PaletteColor, 1f);

            GhostUITheme.Label(
                "Score",
                column,
                "Ghost's confidence on this message: " + visitor.ConfidencePercent + "%",
                GhostUITheme.HeadingSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.32f, 0.28f, 0.18f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 30f;

            GhostUITheme.Label(
                "Score Note",
                column,
                "The number says how sure Ghost is about the words. It does not know whether the " +
                    "request itself leaves anything open. Look at what you actually held on to.",
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.40f, 0.38f, 0.36f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 42f;

            CreateActionCard(column, FinalChapterAction.AnswerNow);
            CreateActionCard(column, FinalChapterAction.AskAgain);
            CreateActionCard(column, FinalChapterAction.HandOver);
        }

        private void CreateActionCard(Transform parent, FinalChapterAction action)
        {
            var chosen = controller.SelectedAction == action;
            var card = GhostUITheme.Panel("Action " + action, parent, chosen ? SelectedColor : EmptyColor)
                .rectTransform;
            AddOutline(card.gameObject, new Color(0.60f, 0.62f, 0.70f), new Vector2(1f, -1f));
            AddHeight(card, 64f);
            card.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 6, 6);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Label",
                card,
                FinalChapterConversationData.GetActionLabel(action),
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 24f;

            GhostUITheme.Label(
                "Cost",
                card,
                "Costs: " + controller.GetActionCost(action),
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.42f, 0.34f, 0.30f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 26f;

            var button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = card.GetComponent<Image>();
            button.onClick.AddListener(() => controller.ChooseAction(action));
        }

        // ------------------------------------------------------------------ backend

        private void CreateBackendStage(Transform parent)
        {
            var column = CreateColumn("Backend", parent, PaletteColor, 1f);
            GhostUITheme.Label(
                "Backend Title",
                column,
                "Attach a stored source only if the answer cannot be given without it.",
                GhostUITheme.SmallSize,
                FontStyle.Italic,
                TextAnchor.UpperLeft,
                new Color(0.36f, 0.34f, 0.44f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 26f;

            foreach (var optionId in controller.ActiveVisitor.BackendOptionIds)
            {
                var option = FinalChapterConversationData.GetOption(optionId);
                var selected = controller.IsBackendSelected(optionId);
                var id = optionId;
                var card = GhostUITheme.Panel("Backend " + optionId, column, selected ? SelectedColor : EmptyColor)
                    .rectTransform;
                AddOutline(card.gameObject, new Color(0.60f, 0.62f, 0.70f), new Vector2(1f, -1f));
                AddHeight(card, 58f);
                card.GetComponent<LayoutElement>().flexibleHeight = 0f;

                var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
                layout.padding = new RectOffset(10, 10, 6, 6);
                layout.spacing = 2f;
                layout.childControlWidth = true;
                layout.childControlHeight = true;
                layout.childForceExpandWidth = true;
                layout.childForceExpandHeight = false;

                GhostUITheme.Label(
                    "Label",
                    card,
                    (selected ? "[attached] " : string.Empty) + option.Label,
                    GhostUITheme.BodySize,
                    FontStyle.Bold,
                    TextAnchor.MiddleLeft,
                    GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 24f;
                GhostUITheme.Label(
                    "Description",
                    card,
                    option.Description,
                    GhostUITheme.SmallSize,
                    FontStyle.Normal,
                    TextAnchor.UpperLeft,
                    new Color(0.36f, 0.36f, 0.42f)).gameObject.AddComponent<LayoutElement>()
                    .preferredHeight = 22f;

                var button = card.gameObject.AddComponent<Button>();
                button.targetGraphic = card.GetComponent<Image>();
                button.onClick.AddListener(() => controller.ToggleBackend(id));
            }
        }

        // ------------------------------------------------------------------ response

        private void CreateResponseStage(Transform parent)
        {
            var left = CreateColumn("Response Parts", parent, PaletteColor, 1f);
            GhostUITheme.Label(
                "Parts Title",
                left,
                "Pieces to build the reply from",
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.36f, 0.34f, 0.44f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 22f;

            foreach (var partId in controller.ActiveVisitor.ResponsePartIds)
            {
                if (controller.IsResponsePartUsed(partId))
                {
                    continue;
                }

                var part = FinalChapterConversationData.GetResponsePart(partId);
                CreateDraggableChip(left, partId, part.Label, new Color(1f, 0.93f, 0.80f));
            }

            var right = CreateColumn("Response Slots", parent, BoardColor, 1f);
            GhostUITheme.Label(
                "Roles Title",
                right,
                "Three responsibilities",
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.30f, 0.38f, 0.46f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 22f;

            var roles = FinalChapterConversationData.ResponseRoleIds;
            for (var i = 0; i < roles.Length; i++)
            {
                var assigned = controller.GetResponsePart(roles[i]);
                CreateDropSlot(
                    right,
                    i,
                    FinalChapterConversationData.GetRoleLabel(roles[i]),
                    string.IsNullOrEmpty(assigned)
                        ? "(empty)"
                        : FinalChapterConversationData.GetResponsePart(assigned).Label,
                    !string.IsNullOrEmpty(assigned));
            }
        }

        // ------------------------------------------------------------------ shared pieces

        private void CreateDraggableChip(
            Transform parent,
            string componentId,
            string label,
            Color color,
            FontStyle fontStyle = FontStyle.Bold)
        {
            var chip = GhostUITheme.Panel("Chip " + componentId, parent, color).rectTransform;
            AddOutline(chip.gameObject, new Color(0.58f, 0.62f, 0.72f), new Vector2(1f, -1f));
            AddHeight(chip, 40f);
            chip.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var layout = chip.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 4, 4);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            GhostUITheme.Label(
                "Label",
                chip,
                label,
                GhostUITheme.SmallSize,
                fontStyle,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);

            chip.gameObject.AddComponent<Act6PipelinePartDragView>().Configure(this, componentId, label);
        }

        /// <summary>
        /// A drop target that also takes its contents back out when clicked - placement has to be
        /// reversible, the same requirement as Chapter 6.
        /// </summary>
        private void CreateDropSlot(Transform parent, int slotIndex, string title, string contents, bool filled)
        {
            var slot = GhostUITheme.Panel("Slot " + slotIndex, parent, filled ? ActiveColor : EmptyColor)
                .rectTransform;
            AddOutline(slot.gameObject, new Color(0.54f, 0.60f, 0.70f), new Vector2(1.5f, -1.5f));
            AddHeight(slot, 60f);
            slot.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var layout = slot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 5, 5);
            layout.spacing = 1f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Title",
                slot,
                title,
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.28f, 0.34f, 0.42f)).gameObject.AddComponent<LayoutElement>()
                .preferredHeight = 22f;

            GhostUITheme.Label(
                "Contents",
                slot,
                contents,
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                filled ? GhostUITheme.Ink : new Color(0.55f, 0.57f, 0.62f))
                .gameObject.AddComponent<LayoutElement>().preferredHeight = 24f;

            slot.gameObject.AddComponent<Act6PipelineSlotDropView>().ConfigureMain(this, slotIndex);
        }

        private void CreateStageControls(Transform parent)
        {
            var row = GhostUITheme.Panel("Stage Controls", parent, Color.clear).rectTransform;
            AddHeight(row, 46f);
            row.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            if (controller.ActiveStageIndex > 0)
            {
                GhostUITheme.PushButton(row, "Back", new Color(0.90f, 0.90f, 0.94f), 110f)
                    .onClick.AddListener(controller.PreviousStage);
            }

            if (controller.ActiveStageIndex < controller.ActiveVisitor.Stages.Count - 1)
            {
                GhostUITheme.PushButton(row, "Next step", new Color(0.84f, 0.92f, 1f), 150f)
                    .onClick.AddListener(controller.NextStage);
            }

            GhostUITheme.PushButton(row, "Start this visitor again", new Color(1f, 0.90f, 0.86f), 210f)
                .onClick.AddListener(controller.ResetCurrentVisitor);

            var spacer = GhostUITheme.Panel("Spacer", row, Color.clear).rectTransform;
            spacer.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            // Chapter 5's habit: run it and read the trace before trusting it.
            var run = GhostUITheme.PushButton(
                row,
                controller.CanRunTest ? "Run the conversation" : "Visit every step first",
                controller.CanRunTest ? new Color(0.78f, 0.94f, 0.82f) : EmptyColor,
                230f);
            run.interactable = controller.CanRunTest;
            run.onClick.AddListener(controller.RunTest);
        }

        // ------------------------------------------------------------------ playback

        private void CreateTraceBody(Transform parent)
        {
            var result = controller.LastResult;
            var board = GhostUITheme.Panel("Trace", parent, BoardColor).rectTransform;
            AddOutline(board.gameObject, new Color(0.62f, 0.66f, 0.74f), new Vector2(1.5f, -1.5f));
            var element = board.gameObject.AddComponent<LayoutElement>();
            element.flexibleHeight = 1f;
            element.minHeight = 200f;

            var layout = board.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 10, 10);
            layout.spacing = 5f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            for (var i = 0; i < result.TraceSteps.Count; i++)
            {
                var step = result.TraceSteps[i];
                var reached = i <= controller.CurrentTraceIndex;
                var fill = !reached
                    ? EmptyColor
                    : step.Succeeded ? SuccessColor : FailureColor;

                var row = GhostUITheme.Panel("Trace " + i, board, fill).rectTransform;
                AddHeight(row, 52f);
                row.GetComponent<LayoutElement>().flexibleHeight = 0f;

                var rowLayout = row.gameObject.AddComponent<VerticalLayoutGroup>();
                rowLayout.padding = new RectOffset(10, 10, 5, 5);
                rowLayout.spacing = 1f;
                rowLayout.childControlWidth = true;
                rowLayout.childControlHeight = true;
                rowLayout.childForceExpandWidth = true;
                rowLayout.childForceExpandHeight = false;

                GhostUITheme.Label(
                    "Title",
                    row,
                    (reached && !step.Succeeded ? "STOPS HERE - " : string.Empty) + step.Title,
                    GhostUITheme.SmallSize,
                    FontStyle.Bold,
                    TextAnchor.MiddleLeft,
                    new Color(0.26f, 0.30f, 0.36f)).gameObject.AddComponent<LayoutElement>()
                    .preferredHeight = 20f;

                GhostUITheme.Label(
                    "Line",
                    row,
                    reached ? step.Line : "...",
                    GhostUITheme.SmallSize,
                    FontStyle.Normal,
                    TextAnchor.UpperLeft,
                    GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 22f;
            }

            var controls = GhostUITheme.Panel("Trace Controls", parent, Color.clear).rectTransform;
            AddHeight(controls, 46f);
            controls.GetComponent<LayoutElement>().flexibleHeight = 0f;

            var controlLayout = controls.gameObject.AddComponent<HorizontalLayoutGroup>();
            controlLayout.spacing = 8f;
            controlLayout.childControlWidth = true;
            controlLayout.childControlHeight = true;
            controlLayout.childForceExpandWidth = false;
            controlLayout.childForceExpandHeight = true;

            var spacer = GhostUITheme.Panel("Spacer", controls, Color.clear).rectTransform;
            spacer.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var atEnd = controller.CurrentTraceIndex >= result.TraceSteps.Count - 1;
            var label = !atEnd
                ? "Next step"
                : result.Passed ? "That one worked - next visitor" : "Go back and fix that step";

            GhostUITheme.PushButton(
                controls,
                label,
                result.Passed ? new Color(0.78f, 0.94f, 0.82f) : new Color(0.84f, 0.92f, 1f),
                260f).onClick.AddListener(controller.AdvancePlayback);
        }

        private void CreateEndingPrompt(Transform parent)
        {
            var board = GhostUITheme.Panel("Ending Prompt", parent, BoardColor).rectTransform;
            AddOutline(board.gameObject, new Color(0.62f, 0.66f, 0.74f), new Vector2(1.5f, -1.5f));
            var element = board.gameObject.AddComponent<LayoutElement>();
            element.flexibleHeight = 1f;
            element.minHeight = 180f;

            var layout = board.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 18, 18);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Ending Title",
                board,
                "All three conversations worked.",
                GhostUITheme.HeadingSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 30f;

            GhostUITheme.Label(
                "Ending Body",
                board,
                "A courier who only needed to be understood. A student who named two of the same thing. " +
                    "And Lily, whose question you had to answer by admitting what you did not know.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink).gameObject.AddComponent<LayoutElement>().preferredHeight = 60f;

            var row = GhostUITheme.Panel("Ending Controls", board, Color.clear).rectTransform;
            AddHeight(row, 48f);
            row.GetComponent<LayoutElement>().flexibleHeight = 0f;
            var rowLayout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = true;

            GhostUITheme.PushButton(row, GetPrimaryActionLabel(), GetPrimaryActionColor(), 240f)
                .onClick.AddListener(HandlePrimaryAction);
        }

        private string GetPrimaryActionLabel()
        {
            return controller.CurrentPhase == FinalChapterPhase.ReadyForEnding
                ? "Let Ghost speak"
                : "Run the conversation";
        }

        private Color GetPrimaryActionColor()
        {
            return controller.CurrentPhase == FinalChapterPhase.ReadyForEnding
                ? new Color(0.86f, 0.82f, 0.98f)
                : new Color(0.78f, 0.94f, 0.82f);
        }

        private void HandlePrimaryAction()
        {
            if (controller.CurrentPhase == FinalChapterPhase.ReadyForEnding)
            {
                controller.BeginEnding();
                return;
            }

            controller.RunTest();
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

        private static Color GetVisitorColor(int index)
        {
            var colors = new[]
            {
                new Color(0.55f, 0.72f, 0.88f),
                new Color(0.75f, 0.60f, 0.84f),
                new Color(0.67f, 0.72f, 0.42f),
                new Color(0.46f, 0.73f, 0.68f)
            };
            return colors[Mathf.Clamp(index, 0, colors.Length - 1)];
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

        private static RectTransform CreateColumn(
            string name,
            Transform parent,
            Color color,
            float flexibleWidth)
        {
            var panel = GhostUITheme.Panel(name, parent, color).rectTransform;
            AddOutline(panel.gameObject, new Color(0.64f, 0.66f, 0.72f), new Vector2(1.5f, -1.5f));
            var element = panel.gameObject.AddComponent<LayoutElement>();
            element.flexibleWidth = flexibleWidth;
            element.flexibleHeight = 1f;
            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 9, 9);
            layout.spacing = 6f;
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
            Vector2 position,
            Vector2 size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private static void AddHeight(RectTransform rect, float height)
        {
            var element = rect.gameObject.AddComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
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
