using Ghost.Presentation.Common;
using System;
using System.Collections.Generic;
using Ghost.Presentation.Act3DialogGraph;
using Ghost.Presentation.Banter;
using Ghost.Presentation.GhostAvatar;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.DialogGraph;
using Ghost.Puzzles.TestingDebugging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Act5TestingDebugging
{
    public sealed class Act5TestingStaticPresenter : MonoBehaviour, IDialogGraphWireInteractionHost
    {
        private const string TitleText = "Act 5: Test Ghost's Reply Map";
        private const float PortSize = 26f;
        private const float WireThickness = 5f;

        private static readonly Color PageColor = new Color(0.955f, 0.97f, 0.95f);
        private static readonly Color ObjectiveColor = new Color(0.13f, 0.22f, 0.28f);
        private static readonly Color WarmNoteColor = new Color(1f, 0.96f, 0.82f);
        private static readonly Color ConversationColor = new Color(0.93f, 0.97f, 1f);
        private static readonly Color GraphPanelColor = new Color(0.93f, 0.96f, 0.98f);
        private static readonly Color TestPanelColor = new Color(0.985f, 0.97f, 0.91f);
        private static readonly Color BoardColor = new Color(0.985f, 0.99f, 1f);
        private static readonly Color StartNodeColor = new Color(0.86f, 0.92f, 1f);
        private static readonly Color IntentNodeColor = new Color(0.90f, 0.86f, 0.98f);
        private static readonly Color SlotNodeColor = new Color(0.84f, 0.95f, 0.88f);
        private static readonly Color ResponseNodeColor = new Color(1f, 0.93f, 0.82f);
        private static readonly Color PendingColor = new Color(0.94f, 0.94f, 0.95f);
        private static readonly Color PassedColor = new Color(0.83f, 0.96f, 0.84f);
        private static readonly Color FailedColor = new Color(1f, 0.85f, 0.82f);
        private static readonly Color StaleColor = new Color(1f, 0.94f, 0.75f);
        private static readonly Color InputPortColor = new Color(0.42f, 0.55f, 0.82f);
        private static readonly Color AlwaysPortColor = new Color(0.28f, 0.52f, 0.86f);
        private static readonly Color SlotPresentPortColor = new Color(0.18f, 0.62f, 0.34f);
        private static readonly Color SlotMissingPortColor = new Color(0.84f, 0.43f, 0.22f);

        [SerializeField] private bool renderOnStart = true;

        private readonly Dictionary<string, RectTransform> inputPortsByNodeId =
            new Dictionary<string, RectTransform>(StringComparer.Ordinal);
        private readonly Dictionary<string, RectTransform> outputPortsByKey =
            new Dictionary<string, RectTransform>(StringComparer.Ordinal);

        private Act5TestingInteractionController controller;
        private RectTransform wireLayer;
        private RectTransform nodeLayer;
        private Act3DialogGraphOutputPortView activeOutputPort;
        private Act3DialogGraphInputPortView reverseDragInputPort;
        private Image activeDragWire;

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
            controller = new Act5TestingInteractionController();
            controller.StateChanged += RenderState;
            RenderState();
        }

        public void BeginWireDrag(
            Act3DialogGraphOutputPortView outputPort,
            PointerEventData eventData)
        {
            if (!CanEditGraph() || outputPort == null || wireLayer == null)
            {
                return;
            }

            CancelActiveWire();
            activeOutputPort = outputPort;
            activeDragWire = CreateWireImage("Active Wire", wireLayer, GetWireColor(outputPort.Condition));
            UpdateWireDrag(eventData);
        }

        public void UpdateWireDrag(PointerEventData eventData)
        {
            // Either end may be the anchor: an output port on a normal drag, an input port when the
            // player grabbed the receiving end instead.
            var anchor = activeOutputPort != null
                ? activeOutputPort.RectTransform
                : reverseDragInputPort != null ? reverseDragInputPort.RectTransform : null;

            if (anchor == null || activeDragWire == null || wireLayer == null || eventData == null)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    wireLayer,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var pointerLocal))
            {
                return;
            }

            DrawLine(activeDragWire.rectTransform, GetPortLocalCenter(anchor), pointerLocal);
        }

        public void EndWireDrag(Act3DialogGraphOutputPortView outputPort)
        {
            if (outputPort == null || activeOutputPort != outputPort)
            {
                return;
            }

            CancelActiveWire();
        }

        /// <summary>Same wire, grabbed from the receiving end instead of the source.</summary>
        public void BeginReverseWireDrag(Act3DialogGraphInputPortView inputPort, PointerEventData eventData)
        {
            if (!CanEditGraph() || inputPort == null || wireLayer == null)
            {
                return;
            }

            CancelActiveWire();
            reverseDragInputPort = inputPort;
            activeDragWire = CreateWireImage("Active Wire", wireLayer, GetWireColor(DialogTransitionCondition.Always));
            UpdateWireDrag(eventData);
        }

        public void EndReverseWireDrag(Act3DialogGraphInputPortView inputPort)
        {
            reverseDragInputPort = null;
            CancelActiveWire();
        }

        public void CompleteWireDrop(
            Act3DialogGraphOutputPortView outputPort,
            Act3DialogGraphInputPortView inputPort)
        {
            if (!CanEditGraph() || outputPort == null || inputPort == null)
            {
                CancelActiveWire();
                return;
            }

            var fromNodeId = outputPort.NodeId;
            var toNodeId = inputPort.NodeId;
            var condition = outputPort.Condition;
            CancelActiveWire();
            controller.ConnectNodes(fromNodeId, toNodeId, condition);
        }

        private void RenderState()
        {
            if (controller == null)
            {
                return;
            }

            CancelActiveWire();
            inputPortsByNodeId.Clear();
            outputPortsByKey.Clear();
            wireLayer = null;
            nodeLayer = null;

            AmbientBanterPanel.SetCurrentState(
                GhostNarrativeState.Act5Id,
                controller.BuildHintContext());
            ClearChildren(transform);
            ConfigureRoot();
            CreateHeader();
            CreateObjectiveStrip();

            if (controller.CurrentPhase == Act5TestingPhase.Onboarding)
            {
                CreateOnboardingPanel();
            }
            else
            {
                CreateLilyNoteStrip();
            }

            CreateConversationPanel();

            if (controller.CurrentPhase != Act5TestingPhase.Onboarding)
            {
                CreateMainBody();
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            Canvas.ForceUpdateCanvases();
            RebuildWires();
        }

        private void ConfigureRoot()
        {
            var image = GetOrAdd<Image>(gameObject);
            image.color = PageColor;
            image.raycastTarget = false;

            var layout = GetOrAdd<VerticalLayoutGroup>(gameObject);
            layout.padding = new RectOffset(32, 32, 24, 22);
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
            var element = header.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 44f;
            element.preferredHeight = 44f;
            // The header is a fixed title row, but its inner horizontal group force-expands height,
            // which reports flexible height to the page and let the header eat all the spare space.
            element.flexibleHeight = 0f;

            var layout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 16f;
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
                GhostUITheme.HeadingSize,
                FontStyle.Bold,
                TextAnchor.MiddleRight,
                GhostUITheme.InkSoft);
            var progressElement = progress.gameObject.AddComponent<LayoutElement>();
            progressElement.minWidth = 250f;
            progressElement.preferredWidth = 250f;
        }

        private string GetProgressText()
        {
            if (controller.CurrentPhase == Act5TestingPhase.Onboarding)
            {
                return "Setup";
            }

            if (controller.CurrentPhase == Act5TestingPhase.Complete)
            {
                return "Complete";
            }

            if (!controller.HasRunTests)
            {
                return "Test 1/3";
            }

            return controller.ResultsAreStale ? "Rerun 3/3" : "Repair 2/3";
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
            if (controller.CurrentPhase == Act5TestingPhase.Onboarding)
            {
                return "Goal: preview every conversation, repair mismatches, then rerun the full suite";
            }

            if (controller.CurrentPhase == Act5TestingPhase.Complete)
            {
                return "Complete: all 4 test conversations now reach their expected replies";
            }

            if (!controller.HasRunTests)
            {
                return "Step 1/3: run all 4 tests before changing the finished-looking map";
            }

            if (controller.ResultsAreStale)
            {
                return "Step 3/3: rerun all 4 tests so one fix cannot hide a new regression";
            }

            return "Step 2/3: compare expected vs actual, then reconnect the faulty route";
        }

        private void CreateOnboardingPanel()
        {
            var panel = GhostUITheme.Panel("Onboarding Panel", transform, WarmNoteColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.84f, 0.59f, 0.22f), new Vector2(2f, -2f));
            var element = panel.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 210f;
            element.preferredHeight = 210f;

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
                "A tidy map can still give the wrong answer",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.28f, 0.18f, 0.08f));

            var body = GhostUITheme.Label(
                "Onboarding Body",
                panel,
                "Lily: Um... before Ghost meets real visitors, run all four rehearsal conversations.\n" +
                "A red card shows what the visitor expected and what Ghost actually said. Trace that mismatch back to a wire.\n" +
                "Drag a colored output dot to the correct node, then rerun every test. All four must be green together.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.20f, 0.18f));
            body.lineSpacing = 1.04f;
            body.gameObject.AddComponent<LayoutElement>().preferredHeight = 116f;

            var button = GhostUITheme.PushButton(panel, "Open the test bench", new Color(0.82f, 0.91f, 1f), 210f);
            button.onClick.AddListener(controller.BeginAfterOnboarding);
        }

        private void CreateLilyNoteStrip()
        {
            var panel = GhostUITheme.Panel("Lily Note Strip", transform, WarmNoteColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.84f, 0.59f, 0.22f), new Vector2(1.5f, -1.5f));
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
                "Lily: Compare expected with actual, follow that route on the map, then rerun every rehearsal.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.25f, 0.20f, 0.18f));
            note.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var replay = GhostUITheme.PushButton(panel, "Replay Lily", new Color(1f, 0.98f, 0.88f), 126f);
            replay.interactable = controller.CurrentPhase != Act5TestingPhase.Complete;
            replay.onClick.AddListener(controller.ReplayOnboarding);
        }

        private void CreateConversationPanel()
        {
            var panel = GhostUITheme.Panel("Conversation Panel", transform, ConversationColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.54f, 0.67f, 0.86f), new Vector2(2f, -2f));
            var element = panel.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 178f;
            element.preferredHeight = 178f;
            // Same trap as the header: the inner group force-expands height, so without this the
            // panel reports flexible height and stretches down the rest of the page.
            element.flexibleHeight = 0f;

            var layout = panel.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.spacing = 16f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var faceRoot = new GameObject("Ghost Face", typeof(RectTransform));
            faceRoot.transform.SetParent(panel, false);
            var faceElement = faceRoot.AddComponent<LayoutElement>();
            faceElement.minWidth = 150f;
            faceElement.preferredWidth = 150f;
            faceElement.minHeight = 150f;
            faceElement.preferredHeight = 150f;
            var face = faceRoot.AddComponent<GhostFaceView>();
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
                "Conversation Label",
                column,
                GetConversationLabel(),
                GhostUITheme.HeadingSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);

            var visitor = GhostUITheme.Label(
                "Visitor Message",
                column,
                GetConversationVisitorLine(),
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.10f, 0.18f, 0.30f));
            visitor.gameObject.AddComponent<LayoutElement>().preferredHeight = 36f;

            var outcome = GhostUITheme.Label(
                "Outcome",
                column,
                GetConversationOutcomeLine(),
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft);
            outcome.lineSpacing = 1.03f;
            outcome.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
        }

        private string GetConversationLabel()
        {
            if (controller.CurrentPhase == Act5TestingPhase.Onboarding)
            {
                return "Preview mismatch: a lab-hours question follows the wrong wire";
            }

            if (controller.CurrentPhase == Act5TestingPhase.Complete)
            {
                return "All 4 test conversations are green";
            }

            if (!controller.HasRunTests)
            {
                return "The reply map has not been tested yet";
            }

            var failure = controller.FindFirstFailure();
            if (failure == null)
            {
                return "Test suite result";
            }

            return controller.ResultsAreStale
                ? "Previous failure before the latest edit: " + failure.Conversation.Id
                : "First failed test: " + failure.Conversation.Id;
        }

        private string GetConversationVisitorLine()
        {
            if (controller.CurrentPhase == Act5TestingPhase.Onboarding)
            {
                return "Visitor: When does the lab close tonight?";
            }

            if (controller.CurrentPhase == Act5TestingPhase.Complete)
            {
                return "Visitor: Hello, Ghost!";
            }

            var failure = controller.FindFirstFailure();
            return failure == null
                ? "Visitor: Run all four rehearsals to preview Ghost's current replies."
                : "Visitor: " + failure.Conversation.VisitorMessage;
        }

        private string GetConversationOutcomeLine()
        {
            if (controller.CurrentPhase == Act5TestingPhase.Onboarding)
            {
                return "Expected: Ghost explains the lab hours.\nActual: Ghost asks which room to search. Testing reveals the mismatch before a real visitor sees it.";
            }

            if (controller.CurrentPhase == Act5TestingPhase.Complete)
            {
                return "Expected: Ghost greets the visitor. Actual: Ghost greets the visitor.\n" + controller.StatusLine;
            }

            var failure = controller.FindFirstFailure();
            if (failure == null)
            {
                return controller.StatusLine;
            }

            return "Expected: " +
                Act5BuggyGraphData.GetResponseLine(failure.Conversation.TestCase.ExpectedResponseId) +
                "\nActual: " +
                Act5BuggyGraphData.GetResponseLine(failure.ActualResponseId) +
                "\n" +
                controller.StatusLine;
        }
        private void CreateMainBody()
        {
            var body = GhostUITheme.Panel("Main Body", transform, Color.clear).rectTransform;
            var bodyElement = body.gameObject.AddComponent<LayoutElement>();
            bodyElement.minHeight = 96f;
            bodyElement.flexibleHeight = 1f;

            var layout = body.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 18f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateGraphPanel(body);
            CreateTestPanel(body);
        }

        private void CreateGraphPanel(Transform parent)
        {
            var panel = CreateColumnPanel("Graph Panel", parent, GraphPanelColor, 0.62f);
            GhostUITheme.Label(
                "Graph Title",
                panel,
                "Ghost's Pre-built Reply Map",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);

            var guide = GhostUITheme.Label(
                "Graph Guide",
                panel,
                GetGraphGuideText(),
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft);
            guide.gameObject.AddComponent<LayoutElement>().preferredHeight = 54f;

            var board = GhostUITheme.Panel("Graph Board", panel, BoardColor).rectTransform;
            AddOutline(board.gameObject, new Color(0.63f, 0.70f, 0.78f), new Vector2(1.5f, -1.5f));
            var boardElement = board.gameObject.AddComponent<LayoutElement>();
            // Node anchors are normalised, so the board's pixel size is what turns them into real gaps.
            // The tightest vertical pair is 0.24 apart; at 390 that is 94px of spacing for cards that
            // are now 96 tall, which is why they sat on top of each other. 520 gives ~125px.
            boardElement.minHeight = 520f;
            boardElement.flexibleHeight = 1f;

            wireLayer = CreateAnchoredRect(
                "Wire Layer",
                board,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero);
            nodeLayer = CreateAnchoredRect(
                "Node Layer",
                board,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero);

            foreach (var node in controller.Nodes)
            {
                CreateNodeCard(node);
            }
        }

        private void CreateNodeCard(DialogNode node)
        {
            var position = GetNodePosition(node.Id);
            var card = GhostUITheme.Card("Node - " + node.Id, nodeLayer, GetNodeColor(node.Type)).rectTransform;
            card.anchorMin = position;
            card.anchorMax = position;
            card.pivot = new Vector2(0.5f, 0.5f);
            card.anchoredPosition = Vector2.zero;
            // Ports straddle the card edge, so each one eats PortSize/2 of the gap between columns.
            // Columns are 0.21 apart (~210px), so the card must leave PortSize plus clearance:
            // 210 - 170 = 40px of gap, of which the two facing ports use 26 and 14 stays free. At 194
            // the gap was 16px and facing ports overlapped completely, which is why they stopped
            // responding to clicks - the dot on top swallowed every drag.
            // Title 24 + subtitle 22 + padding 16 + spacing 2 = 64, so 72 leaves a little slack. The
            // tightest row pair is 0.24 apart on a 520px board, which is 125px - so this leaves a
            // clear 53px between rows instead of the 21px that made the board feel packed.
            card.sizeDelta = new Vector2(170f, 72f);
            AddOutline(card.gameObject, new Color(0.48f, 0.53f, 0.64f), new Vector2(1.5f, -1.5f));

            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(13, 13, 9, 7);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var title = GhostUITheme.Label(
                "Node Title",
                card,
                Act5BuggyGraphData.GetNodeTitle(node),
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);
            title.gameObject.AddComponent<LayoutElement>().preferredHeight = 24f;

            var subtitle = GhostUITheme.Label(
                "Node Detail",
                card,
                GetNodeSubtitle(node),
                GhostUITheme.TinySize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft);
            // Subtitles are one short line now, so they no longer need two rows' worth of space.
            subtitle.gameObject.AddComponent<LayoutElement>().preferredHeight = 22f;

            if (node.Type != DialogNodeType.Start)
            {
                CreateInputPort(card, node.Id);
            }

            switch (node.Type)
            {
                case DialogNodeType.Start:
                case DialogNodeType.IntentBranch:
                    CreateOutputPort(card, node.Id, DialogTransitionCondition.Always, 0.5f);
                    break;
                case DialogNodeType.SlotCheck:
                    CreateOutputPort(card, node.Id, DialogTransitionCondition.SlotPresent, 0.69f);
                    CreateOutputPort(card, node.Id, DialogTransitionCondition.SlotMissing, 0.31f);
                    break;
            }
        }

        private void CreateInputPort(RectTransform card, string nodeId)
        {
            var port = CreatePortDot(
                "Input Port",
                card,
                InputPortColor,
                new Vector2(0f, 0.5f),
                new Vector2(-PortSize * 0.5f, 0f));
            var view = port.gameObject.AddComponent<Act3DialogGraphInputPortView>();
            view.Initialize(this, nodeId);
            inputPortsByNodeId[nodeId] = port;
        }

        private void CreateOutputPort(
            RectTransform card,
            string nodeId,
            DialogTransitionCondition condition,
            float verticalAnchor)
        {
            var port = CreatePortDot(
                "Output Port - " + condition,
                card,
                GetOutputPortColor(condition),
                new Vector2(1f, verticalAnchor),
                new Vector2(PortSize * 0.5f, 0f));
            var view = port.gameObject.AddComponent<Act3DialogGraphOutputPortView>();
            view.Initialize(this, nodeId, condition);
            outputPortsByKey[CreatePortKey(nodeId, condition)] = port;
        }

        private RectTransform CreatePortDot(
            string name,
            Transform parent,
            Color color,
            Vector2 anchor,
            Vector2 anchoredPosition)
        {
            var port = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            port.SetParent(parent, false);
            port.anchorMin = anchor;
            port.anchorMax = anchor;
            port.pivot = new Vector2(0.5f, 0.5f);
            port.anchoredPosition = anchoredPosition;
            port.sizeDelta = new Vector2(PortSize, PortSize);

            var element = port.gameObject.AddComponent<LayoutElement>();
            element.ignoreLayout = true;

            var image = port.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = true;
            AddOutline(port.gameObject, new Color(0.18f, 0.20f, 0.28f), new Vector2(1f, -1f));
            return port;
        }

        private void CreateTestPanel(Transform parent)
        {
            var panel = CreateColumnPanel("Test Panel", parent, TestPanelColor, 0.38f);
            GhostUITheme.Label(
                "Test Title",
                panel,
                "Test Bench: Expected vs Actual",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);

            var summary = GhostUITheme.Label(
                "Test Summary",
                panel,
                GetTestSummary(),
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft);
            summary.gameObject.AddComponent<LayoutElement>().preferredHeight = 30f;

            foreach (var conversation in controller.Conversations)
            {
                CreateTestCard(panel, conversation);
            }

            var spacer = new GameObject("Flexible Spacer", typeof(RectTransform));
            spacer.transform.SetParent(panel, false);
            spacer.AddComponent<LayoutElement>().flexibleHeight = 1f;

            var action = GhostUITheme.PushButton(panel, GetPrimaryActionLabel(), GetPrimaryActionColor(), 220f);
            action.onClick.AddListener(HandlePrimaryAction);
        }

        private string GetTestSummary()
        {
            if (!controller.HasRunTests)
            {
                return "Step 1: click the button below. Red means Ghost gave the wrong reply.";
            }

            var summary = controller.LastSuiteResult.PassedCount + "/" +
                controller.LastSuiteResult.CaseResults.Count + " passed";

            if (controller.ResultsAreStale)
            {
                return "Wiring changed. Previous results are stale; rerun all four.";
            }

            return summary + ". Step 2: use a red card to repair one wire.";
        }

        private void CreateTestCard(
            Transform parent,
            Act5TestConversation conversation)
        {
            var result = controller.FindLastResult(conversation.Id);
            var card = GhostUITheme.Card(
                "Test - " + conversation.Id,
                parent,
                GetTestCardColor(result)).rectTransform;
            AddOutline(card.gameObject, new Color(0.63f, 0.59f, 0.66f), new Vector2(1f, -1f));
            var element = card.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 92f;
            element.preferredHeight = 92f;

            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(11, 11, 6, 6);
            layout.spacing = 1f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GhostUITheme.Label(
                "Test Status",
                card,
                GetTestStatus(result),
                GhostUITheme.SmallSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                result == null || result.Passed
                    ? GhostUITheme.Good
                    : GhostUITheme.Bad);

            GhostUITheme.Label(
                "Visitor",
                card,
                "Visitor: " + conversation.VisitorMessage,
                GhostUITheme.TinySize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.Ink);

            GhostUITheme.Label(
                "Expected",
                card,
                "Expected: " +
                    Act5BuggyGraphData.GetResponseLine(
                        conversation.TestCase.ExpectedResponseId),
                GhostUITheme.TinySize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft);

            GhostUITheme.Label(
                "Actual",
                card,
                result == null
                    ? "Actual: run the suite to preview Ghost"
                    : "Actual: " +
                        Act5BuggyGraphData.GetResponseLine(result.ActualResponseId),
                GhostUITheme.TinySize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft);
        }

        private Color GetTestCardColor(Act5TestCaseResult result)
        {
            if (result == null)
            {
                return PendingColor;
            }

            if (controller.ResultsAreStale)
            {
                return StaleColor;
            }

            return result.Passed ? PassedColor : FailedColor;
        }

        private string GetTestStatus(Act5TestCaseResult result)
        {
            if (result == null)
            {
                return "NOT RUN";
            }

            var status = result.Passed ? "PASS" : "FAIL";
            return controller.ResultsAreStale
                ? status + " - PREVIOUS WIRING"
                : status;
        }

        private void HandlePrimaryAction()
        {
            if (controller.CurrentPhase == Act5TestingPhase.Complete)
            {
                GhostNarrativeState.SetPendingDebriefAct(GhostNarrativeState.Act5Id);
                SceneManager.LoadScene(ShellSceneNames.GameShellSceneName);
                return;
            }

            controller.RunAllTests();
        }

        private string GetPrimaryActionLabel()
        {
            if (controller.CurrentPhase == Act5TestingPhase.Complete)
            {
                return "Complete Act";
            }

            return controller.HasRunTests
                ? "3. Rerun all 4 tests"
                : "1. Run all 4 tests";
        }

        private Color GetPrimaryActionColor()
        {
            if (controller.CurrentPhase == Act5TestingPhase.Complete)
            {
                return PassedColor;
            }

            return controller.ResultsAreStale
                ? StaleColor
                : new Color(0.79f, 0.89f, 1f);
        }
        private string GetGraphGuideText()
        {
            if (!controller.HasRunTests)
            {
                return "1. TEST: Click 'Run all 4 tests' on the right. Colored right-side dots unlock after the wrong replies appear.";
            }

            if (controller.CurrentPhase == Act5TestingPhase.Complete)
            {
                return "All routes are verified by the full test suite.";
            }

            if (controller.ResultsAreStale)
            {
                return "3. RERUN: The wiring changed. Rerun all four tests; you win only when every card passes together.";
            }

            return "2. REPAIR: Pick a red card. Drag the wrong route's colored RIGHT dot onto the Expected reply's blue LEFT dot.";
        }

        private bool CanEditGraph()
        {
            return controller != null &&
                controller.HasRunTests &&
                controller.CurrentPhase == Act5TestingPhase.Configure;
        }

        private void RebuildWires()
        {
            if (wireLayer == null || controller == null)
            {
                return;
            }

            foreach (var transition in controller.Transitions)
            {
                if (!outputPortsByKey.TryGetValue(
                        CreatePortKey(
                            transition.FromNodeId,
                            transition.Condition),
                        out var outputPort) ||
                    !inputPortsByNodeId.TryGetValue(
                        transition.ToNodeId,
                        out var inputPort))
                {
                    continue;
                }

                var wire = CreateWireImage(
                    "Wire - " +
                        transition.FromNodeId +
                        " - " +
                        transition.ToNodeId,
                    wireLayer,
                    GetWireColor(transition.Condition));
                DrawLine(
                    wire.rectTransform,
                    GetPortLocalCenter(outputPort),
                    GetPortLocalCenter(inputPort));
            }
        }

        private static Image CreateWireImage(
            string name,
            Transform parent,
            Color color)
        {
            var rect = new GameObject(name, typeof(RectTransform))
                .GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static void DrawLine(
            RectTransform line,
            Vector2 start,
            Vector2 end)
        {
            var delta = end - start;
            line.anchorMin = new Vector2(0.5f, 0.5f);
            line.anchorMax = new Vector2(0.5f, 0.5f);
            line.pivot = new Vector2(0f, 0.5f);
            line.anchoredPosition = start;
            line.sizeDelta = new Vector2(delta.magnitude, WireThickness);
            line.localRotation = Quaternion.Euler(
                0f,
                0f,
                Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        }

        private Vector2 GetPortLocalCenter(RectTransform port)
        {
            if (port == null || wireLayer == null)
            {
                return Vector2.zero;
            }

            var worldCenter = port.TransformPoint(port.rect.center);
            return wireLayer.InverseTransformPoint(worldCenter);
        }

        private void CancelActiveWire()
        {
            reverseDragInputPort = null;
            activeOutputPort = null;
            if (activeDragWire == null)
            {
                return;
            }

            Destroy(activeDragWire.gameObject);
            activeDragWire = null;
        }

        private static string CreatePortKey(
            string nodeId,
            DialogTransitionCondition condition)
        {
            return nodeId + "|" + condition;
        }

        private static Color GetWireColor(
            DialogTransitionCondition condition)
        {
            switch (condition)
            {
                case DialogTransitionCondition.SlotPresent:
                    return SlotPresentPortColor;
                case DialogTransitionCondition.SlotMissing:
                    return SlotMissingPortColor;
                default:
                    return AlwaysPortColor;
            }
        }

        private static Color GetNodeColor(DialogNodeType type)
        {
            switch (type)
            {
                case DialogNodeType.Start:
                    return StartNodeColor;
                case DialogNodeType.IntentBranch:
                    return IntentNodeColor;
                case DialogNodeType.SlotCheck:
                    return SlotNodeColor;
                default:
                    return ResponseNodeColor;
            }
        }

        // Four columns, pushed right so the wires have room to breathe. Cards are 170 wide, so on a
        // ~975px board these gaps are roughly 55px, 95px and 85px between card edges - previously the
        // last three columns sat 0.31 / 0.53 / 0.81 and the cards nearly touched.
        private const float StartColumn = 0.10f;
        private const float IntentColumn = 0.33f;
        private const float CheckColumn = 0.60f;
        private const float ReplyColumn = 0.86f;

        private static Vector2 GetNodePosition(string nodeId)
        {
            switch (nodeId)
            {
                case Act5BuggyGraphData.StartNodeId:
                    return new Vector2(StartColumn, 0.50f);
                case Act5BuggyGraphData.FindObjectBranchNodeId:
                    return new Vector2(IntentColumn, 0.77f);
                case Act5BuggyGraphData.CheckHoursBranchNodeId:
                    return new Vector2(IntentColumn, 0.50f);
                case Act5BuggyGraphData.GreetingBranchNodeId:
                    return new Vector2(IntentColumn, 0.23f);
                case Act5BuggyGraphData.CheckRoomNodeId:
                    return new Vector2(CheckColumn, 0.77f);
                case Act5BuggyGraphData.AnswerLocationNodeId:
                    return new Vector2(ReplyColumn, 0.88f);
                case Act5BuggyGraphData.AskForRoomNodeId:
                    return new Vector2(ReplyColumn, 0.64f);
                case Act5BuggyGraphData.AnswerLabHoursNodeId:
                    return new Vector2(ReplyColumn, 0.40f);
                case Act5BuggyGraphData.FriendlyGreetingNodeId:
                    return new Vector2(ReplyColumn, 0.16f);
                default:
                    return new Vector2(0.5f, 0.5f);
            }
        }

        private static string GetNodeSubtitle(DialogNode node)
        {
            switch (node.Type)
            {
                // Short on purpose. These repeat on every card and the Guide panel already carries the
                // full colour legend; the long versions were the reason the cards needed so much width
                // that their ports ran into each other.
                case DialogNodeType.Start:
                    return "out: by intent";
                case DialogNodeType.IntentBranch:
                    return "in / out";
                case DialogNodeType.SlotCheck:
                    return "out: known or missing";
                default:
                    return "in: drop wire";
            }
        }

        private Color GetOutputPortColor(DialogTransitionCondition condition)
        {
            var color = GetWireColor(condition);
            return controller != null && controller.HasRunTests
                ? color
                : Color.Lerp(color, PendingColor, 0.58f);
        }

        private static GhostMood MapMood(Act5GhostMood mood)
        {
            switch (mood)
            {
                case Act5GhostMood.Happy:
                    return GhostMood.Happy;
                case Act5GhostMood.Confused:
                    return GhostMood.Confused;
                case Act5GhostMood.Sad:
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
                new Color(0.67f, 0.68f, 0.75f),
                new Vector2(2f, -2f));
            var element = panel.gameObject.AddComponent<LayoutElement>();
            element.flexibleWidth = flexibleWidth;
            element.flexibleHeight = 1f;

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 11, 11);
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
            var rect = new GameObject(name, typeof(RectTransform))
                .GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
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

        private static void EnsureEventSystem()
        {
            var eventSystem = FindAnyObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                var eventSystemObject = new GameObject("EventSystem");
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }
        }

        private void DetachController()
        {
            if (controller != null)
            {
                controller.StateChanged -= RenderState;
                controller = null;
            }
        }

        private static T GetOrAdd<T>(GameObject target)
            where T : Component
        {
            var component = target.GetComponent<T>();
            return component == null
                ? target.AddComponent<T>()
                : component;
        }
    }
}