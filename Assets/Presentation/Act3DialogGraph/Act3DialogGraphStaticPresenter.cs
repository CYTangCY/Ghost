using System;
using System.Collections.Generic;
using Ghost.Presentation.GhostAvatar;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.DialogGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Act3DialogGraph
{
    public sealed class Act3DialogGraphStaticPresenter : MonoBehaviour
    {
        private const float PaletteItemPreferredHeight = 70f;
        private const float TestCasePreferredHeight = 58f;
        private const float ObjectiveStripHeight = 48f;
        private const float OnboardingPanelHeight = 180f;
        private const float ConversationPanelHeight = 170f;
        private const float LilyNoteStripHeight = 54f;
        private const float ValidationControlsPreferredHeight = 28f;
        private const float NodeCardWidth = 210f;
        private const float NodeCardHeight = 112f;
        private const float BoardMinHeight = 430f;
        private const float WireThickness = 4f;
        private const float PortDotSize = 18f;
        private const float PaletteColumnWidth = 125f;
        private const float GraphColumnMinWidth = 900f;
        private const float GraphColumnPreferredWidth = 1120f;
        private const float GuideColumnWidth = 290f;

        private const string TitleText = "Act 3: Ghost's Reply Map";
        private const string InstructionText =
            "Help Ghost reply in the right order. Add simple cards, move them around, then drag wires between their ports.";
        private const string PlaceholderFeedbackText =
            "Build the map, then test Ghost's replies.";
        private const string OnboardingTitleText = "Lily's quick reply-map loop";
        private const string OnboardingBodyText =
            "Lily: Um... Ghost needs one reply map before it can answer in order.\n" +
            "Lily: Intent cards choose a branch; slot checks use the details you caught in Act 2; response cards answer.\n" +
            "Lily: Build the map, then test it on both visitors. A failed route will show what to revise.";
        private const string LilyNoteText =
            "Lily: Um... route the request, check the detail, then let both visitors test Ghost's next reply.";

        private static readonly Color RootTextColor = new Color(0.15f, 0.12f, 0.22f);
        private static readonly Color SecondaryTextColor = new Color(0.30f, 0.28f, 0.38f);
        private static readonly Color FlowPaletteColor = new Color(0.92f, 0.97f, 1f);
        private static readonly Color CheckPaletteColor = new Color(0.93f, 1f, 0.94f);
        private static readonly Color ReplyPaletteColor = new Color(1f, 0.96f, 0.90f);
        private static readonly Color GoalColor = new Color(1f, 0.985f, 0.92f);
        private static readonly Color CanvasColor = new Color(0.96f, 0.96f, 1f);
        private static readonly Color BoardColor = new Color(0.985f, 0.985f, 1f);
        private static readonly Color ValidationColor = new Color(1f, 0.99f, 0.94f);
        private static readonly Color ObjectiveColor = new Color(1f, 0.965f, 0.78f);
        private static readonly Color ObjectiveStripColor = new Color(0.14f, 0.18f, 0.32f);
        private static readonly Color WarmNoteColor = new Color(1f, 0.96f, 0.82f);
        private static readonly Color OutlineColor = new Color(0.62f, 0.60f, 0.78f, 0.72f);
        private static readonly Color NodeCardColor = new Color(1f, 0.995f, 0.94f);
        private static readonly Color SelectedNodeColor = new Color(1f, 0.92f, 0.68f);
        private static readonly Color StartNodeColor = new Color(0.88f, 1f, 0.92f);
        private static readonly Color InputPortColor = new Color(0.45f, 0.58f, 0.86f);
        private static readonly Color AlwaysPortColor = new Color(0.32f, 0.53f, 0.88f);
        private static readonly Color SlotPresentPortColor = new Color(0.20f, 0.62f, 0.35f);
        private static readonly Color SlotMissingPortColor = new Color(0.78f, 0.42f, 0.26f);
        private static readonly Color ButtonColor = new Color(0.87f, 0.91f, 1f);
        private static readonly Color TrashColor = new Color(1f, 0.91f, 0.88f);
        private static readonly Color TrashHighlightColor = new Color(1f, 0.72f, 0.62f);
        private static readonly Color FeedbackNeutralColor = new Color(0.36f, 0.31f, 0.42f);
        private static readonly Color FeedbackCorrectColor = new Color(0.12f, 0.45f, 0.22f);
        private static readonly Color FeedbackIncorrectColor = new Color(0.72f, 0.24f, 0.18f);

        [SerializeField] private RectTransform nodePaletteRoot;
        [SerializeField] private RectTransform graphCanvasRoot;
        [SerializeField] private RectTransform goalTestRoot;
        [SerializeField] private RectTransform validationControlsRoot;
        [SerializeField] private GameObject paletteItemTemplate;
        [SerializeField] private GameObject testCaseTemplate;
        [SerializeField] private bool renderOnStart = true;

        private readonly Dictionary<string, RectTransform> inputPortsByNodeId = new Dictionary<string, RectTransform>(StringComparer.Ordinal);
        private readonly Dictionary<string, RectTransform> outputPortsByKey = new Dictionary<string, RectTransform>(StringComparer.Ordinal);

        private Act3DialogGraphInteractionController controller;
        private Canvas rootCanvas;
        private RectTransform pageHeader;
        private Text phaseProgressText;
        private RectTransform objectiveStrip;
        private Text objectiveText;
        private RectTransform onboardingPanel;
        private RectTransform onboardingBridgePanel;
        private GhostFaceView onboardingGhostFaceView;
        private Text conversationLabelText;
        private Text conversationVisitorText;
        private RectTransform lilyNoteStrip;
        private Button replayOnboardingButton;
        private RectTransform prototypeBody;
        private RectTransform graphBoardRoot;
        private RectTransform wireLayer;
        private RectTransform nodeLayer;
        private RectTransform trashDropRoot;
        private Image trashDropImage;
        private Image activeDragWire;
        private Act3DialogGraphOutputPortView activeOutputPort;
        private Text validationFeedbackText;
        private Text ghostOutcomeText;
        private Button primaryActionButton;
        private Text primaryActionButtonText;
        private string selectedWireFromId;
        private string selectedWireToId;
        private DialogTransitionCondition selectedWireCondition;
        private bool isDraggingNodeOverTrash;

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

        private void Update()
        {
            if (Keyboard.current == null
                || controller == null
                || controller.CurrentPhase == Act3ExperiencePhase.Onboarding)
            {
                return;
            }

            if (Keyboard.current.deleteKey.wasPressedThisFrame || Keyboard.current.backspaceKey.wasPressedThisFrame)
            {
                RemoveSelectedGraphItem();
            }
        }

        public void Configure(
            RectTransform nodePaletteRoot,
            RectTransform graphCanvasRoot,
            RectTransform goalTestRoot,
            RectTransform validationControlsRoot,
            GameObject paletteItemTemplate,
            GameObject testCaseTemplate,
            bool renderOnStart)
        {
            this.nodePaletteRoot = nodePaletteRoot;
            this.graphCanvasRoot = graphCanvasRoot;
            this.goalTestRoot = goalTestRoot;
            this.validationControlsRoot = validationControlsRoot;
            this.paletteItemTemplate = paletteItemTemplate;
            this.testCaseTemplate = testCaseTemplate;
            this.renderOnStart = renderOnStart;
        }

        public void RenderSampleData()
        {
            if (nodePaletteRoot == null ||
                graphCanvasRoot == null ||
                goalTestRoot == null ||
                validationControlsRoot == null ||
                paletteItemTemplate == null ||
                testCaseTemplate == null)
            {
                return;
            }

            EnsureEventSystem();
            EnsureInstructionText();
            DetachController();
            ClearChildren(nodePaletteRoot);
            ClearChildren(graphCanvasRoot);
            ClearChildren(goalTestRoot);
            ClearChildren(validationControlsRoot);
            ClearRenderedGraphState(true);
            validationFeedbackText = null;
            ghostOutcomeText = null;
            primaryActionButton = null;
            primaryActionButtonText = null;

            rootCanvas = GetComponentInParent<Canvas>();
            ConfigureGeneratedColumnLayout();
            controller = new Act3DialogGraphInteractionController();
            controller.StateChanged += HandleControllerStateChanged;
            controller.FeedbackChanged += ApplyValidationFeedback;

            EnsureExperienceChrome();
            RenderNodePalette();
            RenderSidePanel();
            RefreshGraphCanvas();
            RenderValidationControls();
            UpdateExperienceChrome();

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(nodePaletteRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(graphCanvasRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(goalTestRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(validationControlsRoot);
        }

        public void BeginWireDrag(Act3DialogGraphOutputPortView outputPort, PointerEventData eventData)
        {
            if (!CanEditGraph() || outputPort == null || wireLayer == null)
            {
                return;
            }

            activeOutputPort = outputPort;
            DestroyActiveDragWire();
            activeDragWire = CreateWireImage("Temporary Wire", new Color(0.12f, 0.20f, 0.34f, 0.72f));
            activeDragWire.transform.SetAsLastSibling();
            UpdateWireDrag(eventData);
        }

        public void UpdateWireDrag(PointerEventData eventData)
        {
            if (activeOutputPort == null || activeDragWire == null || wireLayer == null || eventData == null)
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

            DrawLine(activeDragWire.rectTransform, GetPortLocalCenter(activeOutputPort.RectTransform), pointerLocal, WireThickness);
        }

        public void EndWireDrag(Act3DialogGraphOutputPortView outputPort)
        {
            if (outputPort == null || activeOutputPort != outputPort)
            {
                return;
            }

            activeOutputPort = null;
            DestroyActiveDragWire();
        }

        public void CompleteWireDrop(Act3DialogGraphOutputPortView outputPort, Act3DialogGraphInputPortView inputPort)
        {
            if (!CanEditGraph() || outputPort == null || inputPort == null)
            {
                return;
            }

            var shouldUseDrop = activeOutputPort == null || activeOutputPort == outputPort;
            if (!shouldUseDrop)
            {
                return;
            }

            activeOutputPort = null;
            DestroyActiveDragWire();
            selectedWireFromId = null;
            selectedWireToId = null;
            controller.ConnectNodes(outputPort.NodeId, inputPort.NodeId, outputPort.Condition);
        }

        public void TryPlacePaletteNodeAtPointer(
            DialogNodeType type,
            string intentId,
            string requiredEntityType,
            string responseId,
            PointerEventData eventData)
        {
            if (!CanEditGraph() || nodeLayer == null || eventData == null)
            {
                return;
            }

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

            controller.PlaceNode(type, intentId, requiredEntityType, responseId, LocalPointToNormalizedPosition(nodeLayer, localPoint));
        }

        public void SelectNode(string nodeId)
        {
            if (!CanEditGraph())
            {
                return;
            }

            selectedWireFromId = null;
            selectedWireToId = null;
            controller?.SelectNode(nodeId);
        }

        public void MoveNodeToPointer(string nodeId, RectTransform nodeCard, PointerEventData eventData)
        {
            if (!CanEditGraph() || nodeLayer == null || nodeCard == null || eventData == null)
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

            var normalizedPosition = LocalPointToNormalizedPosition(nodeLayer, localPoint);
            controller.SetNodePosition(nodeId, normalizedPosition);
            ApplyNodePosition(nodeCard, controller.GetNodePosition(FindCurrentNode(nodeId)));
            nodeCard.SetAsLastSibling();
            SetTrashHighlight(IsNodeOverTrash(nodeCard, eventData));
            DrawCommittedWires();
        }

        public void CompleteNodeDrag(string nodeId, RectTransform nodeCard, PointerEventData eventData)
        {
            if (!CanEditGraph() || trashDropRoot == null)
            {
                return;
            }

            if (isDraggingNodeOverTrash || IsNodeOverTrash(nodeCard, eventData))
            {
                selectedWireFromId = null;
                selectedWireToId = null;
                controller.RemoveNode(nodeId);
            }

            SetTrashHighlight(false);
        }

        private bool IsNodeOverTrash(RectTransform nodeCard, PointerEventData eventData)
        {
            return trashDropRoot != null
                && (IsPointerOverTrash(eventData) || RectTransformsOverlap(nodeCard, trashDropRoot));
        }

        private bool IsPointerOverTrash(PointerEventData eventData)
        {
            return eventData != null
                && trashDropRoot != null
                && RectTransformUtility.RectangleContainsScreenPoint(
                    trashDropRoot,
                    eventData.position,
                    eventData.pressEventCamera);
        }

        private void SetTrashHighlight(bool shouldHighlight)
        {
            if (trashDropImage == null || isDraggingNodeOverTrash == shouldHighlight)
            {
                return;
            }

            isDraggingNodeOverTrash = shouldHighlight;
            trashDropImage.color = shouldHighlight ? TrashHighlightColor : TrashColor;
        }

        private void ConfigureGeneratedColumnLayout()
        {
            var palettePanel = nodePaletteRoot.parent as RectTransform;
            var graphPanel = graphCanvasRoot.parent as RectTransform;
            var guidePanel = goalTestRoot.parent as RectTransform;

            ConfigureColumnLayoutElement(palettePanel, PaletteColumnWidth, PaletteColumnWidth, 0f);
            ConfigureColumnLayoutElement(graphPanel, GraphColumnMinWidth, GraphColumnPreferredWidth, 1f);
            ConfigureColumnLayoutElement(guidePanel, GuideColumnWidth, GuideColumnWidth, 0f);

            var bodyLayout = palettePanel != null && palettePanel.parent != null
                ? palettePanel.parent.GetComponent<HorizontalLayoutGroup>()
                : null;

            if (bodyLayout == null)
            {
                return;
            }

            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = false;
            bodyLayout.childForceExpandHeight = true;
            bodyLayout.spacing = 18f;
        }

        private void EnsureExperienceChrome()
        {
            objectiveStrip = transform.Find("Objective Strip") as RectTransform;
            if (objectiveStrip == null)
            {
                objectiveStrip = new GameObject("Objective Strip", typeof(RectTransform)).GetComponent<RectTransform>();
                objectiveStrip.SetParent(transform, false);
            }

            ClearChildren(objectiveStrip);
            ConfigurePanelSurface(objectiveStrip.gameObject, ObjectiveStripColor, false);
            var objectiveLayoutElement = objectiveStrip.GetComponent<LayoutElement>();
            if (objectiveLayoutElement == null)
            {
                objectiveLayoutElement = objectiveStrip.gameObject.AddComponent<LayoutElement>();
            }

            objectiveLayoutElement.minHeight = ObjectiveStripHeight;
            objectiveLayoutElement.preferredHeight = ObjectiveStripHeight;

            var objectiveLayout = objectiveStrip.GetComponent<HorizontalLayoutGroup>();
            if (objectiveLayout == null)
            {
                objectiveLayout = objectiveStrip.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            objectiveLayout.padding = new RectOffset(18, 18, 7, 7);
            objectiveLayout.childControlWidth = true;
            objectiveLayout.childControlHeight = true;
            objectiveLayout.childForceExpandWidth = true;
            objectiveLayout.childForceExpandHeight = true;
            objectiveText = CreateText(
                "Objective Text",
                objectiveStrip,
                string.Empty,
                18,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                Color.white,
                34f);

            onboardingPanel = transform.Find("Onboarding Panel") as RectTransform;
            if (onboardingPanel == null)
            {
                onboardingPanel = new GameObject("Onboarding Panel", typeof(RectTransform)).GetComponent<RectTransform>();
                onboardingPanel.SetParent(transform, false);
            }

            ClearChildren(onboardingPanel);
            ConfigurePanelSurface(onboardingPanel.gameObject, WarmNoteColor, true);
            var onboardingLayoutElement = onboardingPanel.GetComponent<LayoutElement>();
            if (onboardingLayoutElement == null)
            {
                onboardingLayoutElement = onboardingPanel.gameObject.AddComponent<LayoutElement>();
            }

            onboardingLayoutElement.minHeight = OnboardingPanelHeight;
            onboardingLayoutElement.preferredHeight = OnboardingPanelHeight;

            var onboardingLayout = onboardingPanel.GetComponent<VerticalLayoutGroup>();
            if (onboardingLayout == null)
            {
                onboardingLayout = onboardingPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            onboardingLayout.padding = new RectOffset(18, 18, 12, 12);
            onboardingLayout.spacing = 6f;
            onboardingLayout.childControlWidth = true;
            onboardingLayout.childControlHeight = true;
            onboardingLayout.childForceExpandWidth = true;
            onboardingLayout.childForceExpandHeight = false;

            CreateText(
                "Onboarding Title",
                onboardingPanel,
                OnboardingTitleText,
                20,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.28f, 0.18f, 0.08f),
                26f);
            CreateText(
                "Onboarding Body",
                onboardingPanel,
                OnboardingBodyText,
                17,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.25f, 0.20f, 0.18f),
                72f);
            var beginButton = CreateOnboardingButton(onboardingPanel, "Build the map");
            beginButton.onClick.AddListener(controller.BeginAfterOnboarding);

            onboardingBridgePanel = transform.Find("Conversation Panel") as RectTransform;
            if (onboardingBridgePanel == null)
            {
                onboardingBridgePanel = transform.Find("Ghost Problem Preview") as RectTransform;
                if (onboardingBridgePanel == null)
                {
                    onboardingBridgePanel = new GameObject("Conversation Panel", typeof(RectTransform)).GetComponent<RectTransform>();
                    onboardingBridgePanel.SetParent(transform, false);
                }
            }

            onboardingBridgePanel.name = "Conversation Panel";

            ClearChildren(onboardingBridgePanel);
            ConfigurePanelSurface(onboardingBridgePanel.gameObject, new Color(0.93f, 0.97f, 1f), true);
            var bridgeLayoutElement = onboardingBridgePanel.GetComponent<LayoutElement>();
            if (bridgeLayoutElement == null)
            {
                bridgeLayoutElement = onboardingBridgePanel.gameObject.AddComponent<LayoutElement>();
            }

            bridgeLayoutElement.minHeight = ConversationPanelHeight;
            bridgeLayoutElement.preferredHeight = ConversationPanelHeight;

            var bridgeLayout = onboardingBridgePanel.GetComponent<HorizontalLayoutGroup>();
            if (bridgeLayout == null)
            {
                bridgeLayout = onboardingBridgePanel.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            bridgeLayout.padding = new RectOffset(18, 18, 10, 10);
            bridgeLayout.spacing = 16f;
            bridgeLayout.childControlWidth = true;
            bridgeLayout.childControlHeight = true;
            bridgeLayout.childForceExpandWidth = false;
            bridgeLayout.childForceExpandHeight = true;

            var bridgeFaceRoot = new GameObject("Ghost Face", typeof(RectTransform)).GetComponent<RectTransform>();
            bridgeFaceRoot.SetParent(onboardingBridgePanel, false);
            var bridgeFaceLayout = bridgeFaceRoot.gameObject.AddComponent<LayoutElement>();
            bridgeFaceLayout.minWidth = 150f;
            bridgeFaceLayout.preferredWidth = 150f;
            bridgeFaceLayout.minHeight = 150f;
            bridgeFaceLayout.preferredHeight = 150f;
            onboardingGhostFaceView = bridgeFaceRoot.gameObject.AddComponent<GhostFaceView>();

            var bridgeTextColumn = new GameObject("Ghost Problem Text", typeof(RectTransform)).GetComponent<RectTransform>();
            bridgeTextColumn.SetParent(onboardingBridgePanel, false);
            bridgeTextColumn.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var bridgeTextLayout = bridgeTextColumn.gameObject.AddComponent<VerticalLayoutGroup>();
            bridgeTextLayout.spacing = 5f;
            bridgeTextLayout.childControlWidth = true;
            bridgeTextLayout.childControlHeight = true;
            bridgeTextLayout.childForceExpandWidth = true;
            bridgeTextLayout.childForceExpandHeight = false;
            conversationLabelText = CreateText(
                "Conversation Label",
                bridgeTextColumn,
                string.Empty,
                18,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                RootTextColor,
                28f);
            conversationVisitorText = CreateText(
                "Visitor Message",
                bridgeTextColumn,
                string.Empty,
                17,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.10f, 0.18f, 0.30f),
                36f);
            ghostOutcomeText = CreateText(
                "Ghost Outcome",
                bridgeTextColumn,
                string.Empty,
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                SecondaryTextColor,
                64f);

            lilyNoteStrip = transform.Find("Lily Note Strip") as RectTransform;
            if (lilyNoteStrip == null)
            {
                lilyNoteStrip = new GameObject("Lily Note Strip", typeof(RectTransform)).GetComponent<RectTransform>();
                lilyNoteStrip.SetParent(transform, false);
            }

            ClearChildren(lilyNoteStrip);
            ConfigurePanelSurface(lilyNoteStrip.gameObject, WarmNoteColor, true);
            var noteLayoutElement = lilyNoteStrip.GetComponent<LayoutElement>();
            if (noteLayoutElement == null)
            {
                noteLayoutElement = lilyNoteStrip.gameObject.AddComponent<LayoutElement>();
            }

            noteLayoutElement.minHeight = LilyNoteStripHeight;
            noteLayoutElement.preferredHeight = LilyNoteStripHeight;

            var noteLayout = lilyNoteStrip.GetComponent<HorizontalLayoutGroup>();
            if (noteLayout == null)
            {
                noteLayout = lilyNoteStrip.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            noteLayout.padding = new RectOffset(16, 12, 7, 7);
            noteLayout.spacing = 10f;
            noteLayout.childControlWidth = true;
            noteLayout.childControlHeight = true;
            noteLayout.childForceExpandWidth = false;
            noteLayout.childForceExpandHeight = true;

            var lilyNote = CreateText(
                "Lily Note",
                lilyNoteStrip,
                LilyNoteText,
                15,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.25f, 0.20f, 0.18f),
                40f);
            lilyNote.GetComponent<LayoutElement>().flexibleWidth = 1f;
            replayOnboardingButton = CreateOnboardingButton(lilyNoteStrip, "Replay Lily");
            var replayLayout = replayOnboardingButton.GetComponent<LayoutElement>();
            replayLayout.minWidth = 130f;
            replayLayout.preferredWidth = 130f;
            replayOnboardingButton.onClick.AddListener(controller.ReplayOnboarding);

            prototypeBody = transform.Find("Prototype Body") as RectTransform;
            if (pageHeader != null)
            {
                objectiveStrip.SetSiblingIndex(pageHeader.GetSiblingIndex() + 1);
                onboardingPanel.SetSiblingIndex(objectiveStrip.GetSiblingIndex() + 1);
                lilyNoteStrip.SetSiblingIndex(onboardingPanel.GetSiblingIndex() + 1);
                onboardingBridgePanel.SetSiblingIndex(lilyNoteStrip.GetSiblingIndex() + 1);
            }

            if (prototypeBody != null)
            {
                prototypeBody.SetSiblingIndex(onboardingBridgePanel.GetSiblingIndex() + 1);
            }
        }

        private void HandleControllerStateChanged()
        {
            RefreshGraphCanvas();
            UpdateExperienceChrome();
        }

        private void UpdateExperienceChrome()
        {
            if (controller == null)
            {
                return;
            }

            if (objectiveText != null)
            {
                objectiveText.text = GetObjectiveText();
            }

            if (phaseProgressText != null)
            {
                phaseProgressText.text = GetPhaseProgressText();
            }

            var isOnboarding = controller.CurrentPhase == Act3ExperiencePhase.Onboarding;
            if (onboardingPanel != null)
            {
                onboardingPanel.gameObject.SetActive(isOnboarding);
            }

            if (onboardingBridgePanel != null)
            {
                onboardingBridgePanel.gameObject.SetActive(true);
            }

            if (lilyNoteStrip != null)
            {
                lilyNoteStrip.gameObject.SetActive(!isOnboarding);
            }

            if (prototypeBody != null)
            {
                prototypeBody.gameObject.SetActive(!isOnboarding);
            }

            if (onboardingGhostFaceView != null)
            {
                onboardingGhostFaceView.SetMood(MapGhostMood(controller.CurrentReaction));
            }

            if (replayOnboardingButton != null)
            {
                replayOnboardingButton.gameObject.SetActive(
                    !isOnboarding && controller.CurrentPhase != Act3ExperiencePhase.Complete);
            }

            if (primaryActionButton != null)
            {
                primaryActionButton.gameObject.SetActive(!isOnboarding);
            }

            if (primaryActionButtonText != null)
            {
                primaryActionButtonText.text = controller.CurrentPhase == Act3ExperiencePhase.Complete
                    ? "Complete Act"
                    : controller.HasFailedValidation
                        ? "Try again"
                        : "Test Ghost's map";
            }

            if (!controller.HasValidationAttempt)
            {
                if (validationFeedbackText != null)
                {
                    validationFeedbackText.text = PlaceholderFeedbackText;
                    validationFeedbackText.color = FeedbackNeutralColor;
                }

                if (ghostOutcomeText != null)
                {
                    ghostOutcomeText.text = "Ghost is waiting for a tested reply map.";
                    ghostOutcomeText.color = FeedbackNeutralColor;
                }
            }

            UpdateConversationPanel();

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }

        private string GetObjectiveText()
        {
            if (controller.CurrentPhase == Act3ExperiencePhase.Onboarding)
            {
                return "Setup: learn how to assemble and test Ghost's reply map";
            }

            if (controller.CurrentPhase == Act3ExperiencePhase.Complete)
            {
                return "Complete: Ghost passed both reply-map tests";
            }

            return controller.HasFailedValidation
                ? "2/2 Revise the map from Ghost's failed test, then try again"
                : "1/2 Build Ghost's reply map from the available cards and routes";
        }

        private string GetPhaseProgressText()
        {
            if (controller.CurrentPhase == Act3ExperiencePhase.Onboarding)
            {
                return "Setup";
            }

            if (controller.CurrentPhase == Act3ExperiencePhase.Complete)
            {
                return "Complete";
            }

            return controller.HasValidationAttempt ? "Test 2/2" : "Build 1/2";
        }

        private void UpdateConversationPanel()
        {
            if (conversationLabelText == null || conversationVisitorText == null || ghostOutcomeText == null)
            {
                return;
            }

            if (controller.CurrentPhase == Act3ExperiencePhase.Onboarding)
            {
                conversationLabelText.text = "Ghost has a reply-order problem";
                conversationVisitorText.text = "Visitor: Can you find the lantern in the archive?";
                ghostOutcomeText.text = "Ghost recognizes the request and catches the room, but answers before checking which reply should come next.";
                ghostOutcomeText.color = SecondaryTextColor;
                return;
            }

            if (!controller.HasValidationAttempt)
            {
                conversationLabelText.text = "Ghost is waiting for a reply map";
                conversationVisitorText.text = "Two visitors will test the same request with different room details.";
                ghostOutcomeText.text = "Build the route, then test whether Ghost chooses an appropriate next reply for each visitor.";
                ghostOutcomeText.color = SecondaryTextColor;
                return;
            }

            conversationLabelText.text = controller.LastValidationWasCorrect
                ? "Ghost's reply map works"
                : "Ghost's reply map needs another pass";
            conversationVisitorText.text = "The two authored visitors ran through the current map.";
            ghostOutcomeText.text = CreateGhostOutcomeMessage(
                controller.LastValidationWasCorrect,
                controller.LastValidationErrors);
            ghostOutcomeText.color = controller.LastValidationWasCorrect
                ? FeedbackCorrectColor
                : FeedbackIncorrectColor;
        }

        private bool CanEditGraph()
        {
            return controller != null && controller.CurrentPhase != Act3ExperiencePhase.Onboarding;
        }

        private static GhostMood MapGhostMood(Act3GhostReaction reaction)
        {
            switch (reaction)
            {
                case Act3GhostReaction.Happy:
                    return GhostMood.Happy;
                case Act3GhostReaction.Confused:
                    return GhostMood.Confused;
                case Act3GhostReaction.Sad:
                    return GhostMood.Sad;
                default:
                    return GhostMood.Neutral;
            }
        }

        private void HandlePrimaryAction()
        {
            if (controller == null)
            {
                return;
            }

            if (controller.CurrentPhase == Act3ExperiencePhase.Complete)
            {
                GhostNarrativeState.SetPendingDebriefAct(GhostNarrativeState.Act3Id);
                SceneManager.LoadScene(ShellSceneNames.GameShellSceneName);
                return;
            }

            controller.ValidateCurrentState();
        }

        private static void ConfigureColumnLayoutElement(
            RectTransform panel,
            float minWidth,
            float preferredWidth,
            float flexibleWidth)
        {
            if (panel == null)
            {
                return;
            }

            var layoutElement = panel.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = panel.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minWidth = minWidth;
            layoutElement.preferredWidth = preferredWidth;
            layoutElement.flexibleWidth = flexibleWidth;
            layoutElement.flexibleHeight = 1f;
        }

        private static bool RectTransformsOverlap(RectTransform first, RectTransform second)
        {
            if (first == null || second == null)
            {
                return false;
            }

            var firstRect = CreateWorldRect(first);
            var secondRect = CreateWorldRect(second);
            return firstRect.Overlaps(secondRect);
        }

        private static Rect CreateWorldRect(RectTransform target)
        {
            var corners = new Vector3[4];
            target.GetWorldCorners(corners);

            var minX = corners[0].x;
            var maxX = corners[0].x;
            var minY = corners[0].y;
            var maxY = corners[0].y;

            for (var index = 1; index < corners.Length; index++)
            {
                minX = Mathf.Min(minX, corners[index].x);
                maxX = Mathf.Max(maxX, corners[index].x);
                minY = Mathf.Min(minY, corners[index].y);
                maxY = Mathf.Max(maxY, corners[index].y);
            }

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        private void RenderNodePalette()
        {
            CreateSectionLabel(nodePaletteRoot, "Flow");

            CreatePalettePlacementItem("Start here", "Where Ghost begins.", FlowPaletteColor, DialogNodeType.Start);
            CreatePalettePlacementItem(
                "Recognize request",
                "Visitor wants help finding something.",
                FlowPaletteColor,
                DialogNodeType.IntentBranch,
                intentId: Act3DialogGraphSampleData.FindObjectIntentId);

            CreateSectionLabel(nodePaletteRoot, "Check");
            CreatePalettePlacementItem(
                "Check room",
                "Does Ghost know which room?",
                CheckPaletteColor,
                DialogNodeType.SlotCheck,
                requiredEntityType: Act3DialogGraphSampleData.RoomEntityTypeId);

            CreateSectionLabel(nodePaletteRoot, "Reply");
            CreatePalettePlacementItem(
                "Answer location",
                "Use this when the room is known.",
                ReplyPaletteColor,
                DialogNodeType.Response,
                responseId: Act3DialogGraphSampleData.AnswerObjectLocationResponseId);
            CreatePalettePlacementItem(
                "Ask which room",
                "Use this when the room is missing.",
                ReplyPaletteColor,
                DialogNodeType.Response,
                responseId: Act3DialogGraphSampleData.AskForRoomResponseId);
        }

        private void CreatePalettePlacementItem(
            string title,
            string detail,
            Color color,
            DialogNodeType type,
            string intentId = null,
            string requiredEntityType = null,
            string responseId = null)
        {
            var item = Instantiate(paletteItemTemplate, nodePaletteRoot);
            item.name = $"Place Node - {title} {detail}";
            item.SetActive(true);
            ConfigureListItem(item, color, PaletteItemPreferredHeight);
            SetChildText(item.transform, "PaletteItemTitle", title);
            SetChildText(item.transform, "PaletteItemDetail", detail);

            var image = item.GetComponent<Image>();
            image.raycastTarget = true;

            var dragView = item.GetComponent<Act3DialogGraphPaletteItemDragView>();
            if (dragView == null)
            {
                dragView = item.AddComponent<Act3DialogGraphPaletteItemDragView>();
            }

            dragView.Initialize(this, type, intentId, requiredEntityType, responseId);

            var button = item.GetComponent<Button>();
            if (button == null)
            {
                button = item.AddComponent<Button>();
            }

            button.targetGraphic = image;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (!CanEditGraph())
                {
                    return;
                }

                controller.PlaceNode(type, intentId, requiredEntityType, responseId);
            });
        }

        private void RefreshGraphCanvas()
        {
            if (graphCanvasRoot == null || controller == null)
            {
                return;
            }

            ClearChildren(graphCanvasRoot);
            ClearRenderedGraphState(false);
            ConfigureGraphCanvasRoot();

            CreateObjectivePanel();
            CreateCanvasLabel(
                "Graph Canvas Instruction",
                graphCanvasRoot,
                "Move cards freely. Drag from a bottom port to the next card's top port.",
                15,
                FontStyle.Italic,
                TextAnchor.MiddleLeft,
                SecondaryTextColor,
                28f);

            CreateGraphBoard();
            RenderPlacedNodes();

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(graphCanvasRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(graphBoardRoot);
            DrawCommittedWires();
        }

        private void CreateObjectivePanel()
        {
            var panel = new GameObject("Objective Panel", typeof(RectTransform));
            panel.transform.SetParent(graphCanvasRoot, false);
            ConfigurePanelSurface(panel, ObjectiveColor, true);

            var layoutElement = panel.AddComponent<LayoutElement>();
            layoutElement.minHeight = 58f;
            layoutElement.preferredHeight = 58f;

            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 8, 8);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateText(
                "Objective Text",
                panel.transform,
                CreateObjectiveText(),
                17,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                RootTextColor,
                42f);
        }

        private static string CreateObjectiveText()
        {
            return "Build a route that uses the visitor's intent, checks available details, and reaches an appropriate response.";
        }

        private void CreateGraphBoard()
        {
            graphBoardRoot = new GameObject("Node Board", typeof(RectTransform)).GetComponent<RectTransform>();
            graphBoardRoot.SetParent(graphCanvasRoot, false);
            ConfigurePanelSurface(graphBoardRoot.gameObject, BoardColor, true);

            var layoutElement = graphBoardRoot.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = BoardMinHeight;
            layoutElement.flexibleHeight = 1f;

            wireLayer = CreateOverlayLayer("Wire Layer", graphBoardRoot);
            nodeLayer = CreateOverlayLayer("Node Layer", graphBoardRoot);
            wireLayer.SetAsFirstSibling();
            nodeLayer.SetAsLastSibling();

            var nodeLayerCanvas = nodeLayer.gameObject.AddComponent<Canvas>();
            nodeLayerCanvas.overrideSorting = true;
            nodeLayerCanvas.sortingOrder = 8;
            nodeLayer.gameObject.AddComponent<GraphicRaycaster>();
        }

        private void CreateTrashDropZone(Transform parent)
        {
            trashDropRoot = new GameObject("Trash Drop Zone", typeof(RectTransform)).GetComponent<RectTransform>();
            trashDropRoot.SetParent(parent, false);
            ConfigurePanelSurface(trashDropRoot.gameObject, TrashColor, true);
            trashDropImage = trashDropRoot.GetComponent<Image>();

            var layoutElement = trashDropRoot.gameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = 118f;
            layoutElement.preferredWidth = 118f;

            var layout = trashDropRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(4, 4, 3, 3);
            layout.spacing = 0f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateText("Trash Label", trashDropRoot, "X drop card", 12, FontStyle.Bold, TextAnchor.MiddleCenter, FeedbackIncorrectColor, 18f);
        }

        private void RenderPlacedNodes()
        {
            if (controller.CurrentNodes.Count == 0)
            {
                CreateBoardCenteredText("Click a card on the left to add it here.\nThen drag cards to arrange Ghost's reply map.");
                return;
            }

            foreach (var node in controller.CurrentNodes)
            {
                CreateNodeCard(node);
            }
        }

        private void CreateNodeCard(DialogNode node)
        {
            var card = new GameObject($"Node Card - {node.Id}", typeof(RectTransform)).GetComponent<RectTransform>();
            card.SetParent(nodeLayer, false);
            card.pivot = new Vector2(0.5f, 0.5f);
            card.sizeDelta = new Vector2(NodeCardWidth, NodeCardHeight);
            ApplyNodePosition(card, controller.GetNodePosition(node));

            var isSelected = string.Equals(controller.SelectedNodeId, node.Id, StringComparison.Ordinal);
            var isStart = string.Equals(controller.CurrentStartNodeId, node.Id, StringComparison.Ordinal);
            ConfigurePanelSurface(card.gameObject, isSelected ? SelectedNodeColor : isStart ? StartNodeColor : NodeCardColor, true);

            var image = card.GetComponent<Image>();
            image.raycastTarget = true;

            var dragView = card.gameObject.AddComponent<Act3DialogGraphNodeDragView>();
            dragView.Initialize(this, node.Id);

            var button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => SelectNode(node.Id));

            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 20, 20);
            layout.spacing = 3f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            if (node.Type != DialogNodeType.Start)
            {
                CreateInputPortOnCard(card, node.Id);
            }
            CreateNodeTitleRow(card, node, isStart);
            CreateText("Node Config", card, FormatNodeConfig(node), 12, FontStyle.Normal, TextAnchor.MiddleCenter, SecondaryTextColor, 18f);
            CreateOutputPortsOnCard(card, node);
        }

        private void CreateInputPortOnCard(Transform parent, string nodeId)
        {
            var port = CreateAnchoredPortDot(
                "Input Port",
                parent,
                InputPortColor,
                PortDotSize,
                new Vector2(0.5f, 1f),
                new Vector2(0f, -1f));
            var inputPort = port.gameObject.AddComponent<Act3DialogGraphInputPortView>();
            inputPort.Initialize(this, nodeId);
            inputPortsByNodeId[nodeId] = port;
        }

        private void CreateNodeTitleRow(Transform parent, DialogNode node, bool isStart)
        {
            var row = CreateHorizontalRow("Node Title Row", parent, 24f, 4f);
            var title = CreateText(
                "Node Title",
                row,
                isStart ? $"{GetNodeDisplayName(node)} [start]" : GetNodeDisplayName(node),
                13,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                RootTextColor,
                24f);
            title.horizontalOverflow = HorizontalWrapMode.Overflow;
            title.GetComponent<LayoutElement>().flexibleWidth = 1f;
        }

        private void CreateOutputPortsOnCard(Transform parent, DialogNode node)
        {
            switch (node.Type)
            {
                case DialogNodeType.Start:
                case DialogNodeType.IntentBranch:
                    CreateOutputPortOnCard(
                        parent,
                        node.Id,
                        DialogTransitionCondition.Always,
                        "next",
                        AlwaysPortColor,
                        new Vector2(0.5f, 0f));
                    break;
                case DialogNodeType.SlotCheck:
                    CreateOutputPortOnCard(
                        parent,
                        node.Id,
                        DialogTransitionCondition.SlotPresent,
                        "room yes",
                        SlotPresentPortColor,
                        new Vector2(0.36f, 0f));
                    CreateOutputPortOnCard(
                        parent,
                        node.Id,
                        DialogTransitionCondition.SlotMissing,
                        "room no",
                        SlotMissingPortColor,
                        new Vector2(0.64f, 0f));
                    break;
            }
        }

        private void CreateOutputPortOnCard(
            Transform parent,
            string nodeId,
            DialogTransitionCondition condition,
            string label,
            Color color,
            Vector2 anchor)
        {
            var port = CreateAnchoredPortDot("Output Port - " + label, parent, color, PortDotSize, anchor, new Vector2(0f, 1f));
            var outputPort = port.gameObject.AddComponent<Act3DialogGraphOutputPortView>();
            outputPort.Initialize(this, nodeId, condition);
            outputPortsByKey[CreatePortKey(nodeId, condition)] = port;
        }

        private RectTransform CreateAnchoredPortDot(
            string name,
            Transform parent,
            Color color,
            float size,
            Vector2 anchor,
            Vector2 offset)
        {
            var port = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            port.SetParent(parent, false);
            port.anchorMin = anchor;
            port.anchorMax = anchor;
            port.pivot = new Vector2(0.5f, 0.5f);
            port.anchoredPosition = offset;
            port.sizeDelta = new Vector2(size, size);

            var layoutElement = port.gameObject.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            var image = port.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = true;

            var outline = port.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.86f);
            outline.effectDistance = new Vector2(2f, -2f);
            return port;
        }

        private RectTransform CreatePortDot(
            string name,
            Transform parent,
            Color color,
            float size)
        {
            var port = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            port.SetParent(parent, false);

            var image = port.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = true;

            var layoutElement = port.gameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = size;
            layoutElement.preferredWidth = size;
            layoutElement.minHeight = size;
            layoutElement.preferredHeight = size;

            var outline = port.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.86f);
            outline.effectDistance = new Vector2(2f, -2f);
            return port;
        }

        private void DrawCommittedWires()
        {
            if (wireLayer == null)
            {
                return;
            }

            ClearChildren(wireLayer);

            foreach (var transition in controller.CurrentTransitions)
            {
                if (!outputPortsByKey.TryGetValue(CreatePortKey(transition.FromNodeId, transition.Condition), out var outputPort)
                    || !inputPortsByNodeId.TryGetValue(transition.ToNodeId, out var inputPort))
                {
                    continue;
                }

                var line = CreateWireImage(
                    $"Wire - {transition.FromNodeId} to {transition.ToNodeId}",
                    IsSelectedWire(transition) ? SelectedNodeColor : GetWireColor(transition.Condition));
                line.raycastTarget = true;

                var button = line.gameObject.AddComponent<Button>();
                button.targetGraphic = line;
                var transitionCopy = transition;
                button.onClick.AddListener(() => SelectWire(transitionCopy));

                DrawLine(
                    line.rectTransform,
                    GetPortLocalCenter(outputPort),
                    GetPortLocalCenter(inputPort),
                    WireThickness);
            }
        }

        private void SelectWire(DialogTransition transition)
        {
            selectedWireFromId = transition.FromNodeId;
            selectedWireToId = transition.ToNodeId;
            selectedWireCondition = transition.Condition;

            if (controller != null && !string.IsNullOrWhiteSpace(controller.SelectedNodeId))
            {
                controller.ClearSelection();
                return;
            }

            DrawCommittedWires();
        }

        private void RemoveSelectedGraphItem()
        {
            if (!CanEditGraph())
            {
                return;
            }

            if (RemoveSelectedWire())
            {
                return;
            }

            RemoveSelectedNode();
        }

        private bool RemoveSelectedWire()
        {
            if (controller == null || string.IsNullOrWhiteSpace(selectedWireFromId) || string.IsNullOrWhiteSpace(selectedWireToId))
            {
                return false;
            }

            if (controller.RemoveTransition(selectedWireFromId, selectedWireToId, selectedWireCondition))
            {
                selectedWireFromId = null;
                selectedWireToId = null;
                return true;
            }

            return false;
        }

        private bool RemoveSelectedNode()
        {
            if (controller == null || string.IsNullOrWhiteSpace(controller.SelectedNodeId))
            {
                return false;
            }

            return controller.RemoveNode(controller.SelectedNodeId);
        }

        private bool IsSelectedWire(DialogTransition transition)
        {
            return string.Equals(selectedWireFromId, transition.FromNodeId, StringComparison.Ordinal)
                && string.Equals(selectedWireToId, transition.ToNodeId, StringComparison.Ordinal)
                && selectedWireCondition == transition.Condition;
        }

        private Image CreateWireImage(string name, Color color)
        {
            var line = new GameObject(name, typeof(RectTransform)).AddComponent<Image>();
            line.transform.SetParent(wireLayer, false);
            line.color = color;
            line.raycastTarget = false;
            return line;
        }

        private void DrawLine(RectTransform line, Vector2 start, Vector2 end, float thickness)
        {
            var delta = end - start;
            var length = delta.magnitude;
            var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            line.anchorMin = new Vector2(0.5f, 0.5f);
            line.anchorMax = new Vector2(0.5f, 0.5f);
            line.pivot = new Vector2(0f, 0.5f);
            line.anchoredPosition = start;
            line.sizeDelta = new Vector2(length, thickness);
            line.localRotation = Quaternion.Euler(0f, 0f, angle);
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

        private void RenderSidePanel()
        {
            if (controller == null)
            {
                return;
            }

            var guideLayout = goalTestRoot.GetComponent<VerticalLayoutGroup>();
            if (guideLayout != null)
            {
                guideLayout.spacing = 6f;
            }

            CreateGuideSectionLabel(goalTestRoot, "How to play");
            CreateSidePanelText(
                "Play Steps",
                "Drag cards into the map and connect their dots. Drag to X, or select and press Del, to remove.",
                56f);

            CreateGuideSectionLabel(goalTestRoot, "Legend");
            CreateLegendRow("Blue dot", "next step", AlwaysPortColor);
            CreateLegendRow("Green dot", "room is known", SlotPresentPortColor);
            CreateLegendRow("Orange dot", "room is missing", SlotMissingPortColor);
            CreateLegendRow("Top dot", "drop a wire here", InputPortColor);

            CreateGuideSectionLabel(goalTestRoot, "Ghost should handle");
            foreach (var testCase in controller.TestCases)
            {
                CreateCompactTestCaseRow(testCase);
            }

        }

        private Text CreateSidePanelText(string name, string value, float preferredHeight)
        {
            var panel = new GameObject(name + " Panel", typeof(RectTransform));
            panel.transform.SetParent(goalTestRoot, false);
            ConfigurePanelSurface(panel, GoalColor, true);

            var layoutElement = panel.AddComponent<LayoutElement>();
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;

            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 8, 8);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            return CreateText(name, panel.transform, value, 13, FontStyle.Normal, TextAnchor.MiddleLeft, SecondaryTextColor, preferredHeight - 16f);
        }

        private void CreateLegendRow(string label, string detail, Color color)
        {
            var row = CreateHorizontalRow("Legend Row - " + label, goalTestRoot, 28f, 8f);

            var dot = CreatePortDot("Legend Dot", row, color, PortDotSize);
            dot.GetComponent<LayoutElement>().minWidth = 26f;
            dot.GetComponent<LayoutElement>().preferredWidth = 26f;

            var text = CreateText(
                "Legend Text",
                row,
                $"{label}: {detail}",
                12,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                SecondaryTextColor,
                28f);
            text.GetComponent<LayoutElement>().flexibleWidth = 1f;
        }

        private void CreateCompactTestCaseRow(DialogGraphTestCase testCase)
        {
            var panel = new GameObject("Ghost Check - " + FormatTestCaseTitle(testCase), typeof(RectTransform));
            panel.transform.SetParent(goalTestRoot, false);
            ConfigurePanelSurface(panel, GoalColor, true);

            var layoutElement = panel.AddComponent<LayoutElement>();
            layoutElement.minHeight = TestCasePreferredHeight;
            layoutElement.preferredHeight = TestCasePreferredHeight;

            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 7, 7);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateText("Check Title", panel.transform, FormatTestCaseTitle(testCase), 13, FontStyle.Bold, TextAnchor.MiddleLeft, RootTextColor, 18f);
            CreateText("Check Detail", panel.transform, FormatTestCase(testCase), 11, FontStyle.Normal, TextAnchor.MiddleLeft, SecondaryTextColor, 24f);
        }

        private void RenderValidationControls()
        {
            ConfigurePanelSurface(validationControlsRoot.gameObject, ValidationColor, false);
            ConfigureValidationControlsRoot();

            primaryActionButton = CreateValidateButton(validationControlsRoot, out primaryActionButtonText);
            primaryActionButton.interactable = true;
            primaryActionButton.onClick.RemoveAllListeners();
            primaryActionButton.onClick.AddListener(HandlePrimaryAction);

            validationFeedbackText = CreateValidationFeedbackText(validationControlsRoot);
            validationFeedbackText.text = PlaceholderFeedbackText;
            validationFeedbackText.color = FeedbackNeutralColor;
            CreateTrashDropZone(validationControlsRoot);
        }

        private void ApplyValidationFeedback(string message, bool isCorrect, IReadOnlyList<string> errors)
        {
            if (validationFeedbackText != null)
            {
                validationFeedbackText.text = message ?? string.Empty;
                validationFeedbackText.color = isCorrect ? FeedbackCorrectColor : FeedbackIncorrectColor;
            }

            if (ghostOutcomeText != null)
            {
                ghostOutcomeText.text = CreateGhostOutcomeMessage(isCorrect, errors);
                ghostOutcomeText.color = isCorrect ? FeedbackCorrectColor : FeedbackIncorrectColor;
            }

            UpdateExperienceChrome();
        }

        private void ConfigureValidationControlsRoot()
        {
            var layoutElement = validationControlsRoot.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = validationControlsRoot.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = ValidationControlsPreferredHeight;
            layoutElement.preferredHeight = ValidationControlsPreferredHeight;
            layoutElement.flexibleHeight = 0f;

            var layout = validationControlsRoot.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = validationControlsRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.padding = new RectOffset(6, 6, 3, 3);
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
        }

        private void ConfigureGraphCanvasRoot()
        {
            ConfigurePanelSurface(graphCanvasRoot.gameObject, CanvasColor, true);

            var layout = graphCanvasRoot.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = graphCanvasRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(14, 14, 10, 14);
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private void EnsureInstructionText()
        {
            var rootLayout = transform.GetComponent<VerticalLayoutGroup>();
            if (rootLayout != null)
            {
                rootLayout.padding = new RectOffset(36, 36, 26, 24);
                rootLayout.spacing = 9f;
                rootLayout.childControlWidth = true;
                rootLayout.childControlHeight = true;
                rootLayout.childForceExpandWidth = true;
                rootLayout.childForceExpandHeight = false;
            }

            var rootImage = transform.GetComponent<Image>();
            if (rootImage != null)
            {
                rootImage.color = new Color(0.96f, 0.94f, 1f);
                rootImage.raycastTarget = false;
            }

            EnsurePageHeader();
        }

        private void EnsurePageHeader()
        {
            pageHeader = transform.Find("Header") as RectTransform;
            if (pageHeader == null)
            {
                pageHeader = new GameObject("Header", typeof(RectTransform)).GetComponent<RectTransform>();
                pageHeader.SetParent(transform, false);
            }

            pageHeader.SetAsFirstSibling();
            var headerLayoutElement = pageHeader.GetComponent<LayoutElement>();
            if (headerLayoutElement == null)
            {
                headerLayoutElement = pageHeader.gameObject.AddComponent<LayoutElement>();
            }

            headerLayoutElement.minHeight = 56f;
            headerLayoutElement.preferredHeight = 56f;

            var headerLayout = pageHeader.GetComponent<HorizontalLayoutGroup>();
            if (headerLayout == null)
            {
                headerLayout = pageHeader.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            headerLayout.spacing = 16f;
            headerLayout.childControlWidth = true;
            headerLayout.childControlHeight = true;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = true;

            var titleRoot = pageHeader.Find("Title") as RectTransform;
            if (titleRoot == null)
            {
                titleRoot = transform.Find("Title") as RectTransform;
                if (titleRoot == null)
                {
                    titleRoot = new GameObject("Title", typeof(RectTransform)).GetComponent<RectTransform>();
                }

                titleRoot.SetParent(pageHeader, false);
            }

            if (titleRoot.GetComponent<Text>() == null)
            {
                titleRoot.gameObject.AddComponent<Text>();
            }

            ConfigureExistingLabel(titleRoot, TitleText, 38, FontStyle.Bold, TextAnchor.MiddleLeft, RootTextColor, 56f);
            titleRoot.GetComponent<LayoutElement>().flexibleWidth = 1f;

            var progressRoot = pageHeader.Find("Phase Progress") as RectTransform;
            if (progressRoot == null)
            {
                progressRoot = new GameObject("Phase Progress", typeof(RectTransform)).GetComponent<RectTransform>();
                progressRoot.SetParent(pageHeader, false);
            }

            phaseProgressText = progressRoot.GetComponent<Text>();
            if (phaseProgressText == null)
            {
                phaseProgressText = progressRoot.gameObject.AddComponent<Text>();
            }

            ConfigureExistingLabel(progressRoot, string.Empty, 20, FontStyle.Bold, TextAnchor.MiddleRight, SecondaryTextColor, 56f);
            var progressLayout = progressRoot.GetComponent<LayoutElement>();
            progressLayout.minWidth = 210f;
            progressLayout.preferredWidth = 210f;
            progressLayout.flexibleWidth = 0f;

            var subtitle = transform.Find("Subtitle");
            if (subtitle != null)
            {
                subtitle.gameObject.SetActive(false);
            }
        }

        private void DetachController()
        {
            if (controller == null)
            {
                return;
            }

            controller.StateChanged -= HandleControllerStateChanged;
            controller.FeedbackChanged -= ApplyValidationFeedback;
            controller = null;
        }

        private void ClearRenderedGraphState(bool includeTrash)
        {
            inputPortsByNodeId.Clear();
            outputPortsByKey.Clear();
            graphBoardRoot = null;
            wireLayer = null;
            nodeLayer = null;
            activeOutputPort = null;
            activeDragWire = null;

            if (includeTrash)
            {
                trashDropRoot = null;
                trashDropImage = null;
                isDraggingNodeOverTrash = false;
                selectedWireFromId = null;
                selectedWireToId = null;
                selectedWireCondition = DialogTransitionCondition.Always;
            }
        }

        private static RectTransform CreateOverlayLayer(string name, Transform parent)
        {
            var layer = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            layer.SetParent(parent, false);
            layer.anchorMin = Vector2.zero;
            layer.anchorMax = Vector2.one;
            layer.offsetMin = Vector2.zero;
            layer.offsetMax = Vector2.zero;
            return layer;
        }

        private static void ConfigureListItem(GameObject item, Color color, float preferredHeight)
        {
            ConfigurePanelSurface(item, color, true);

            var layoutElement = item.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = item.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;

            var layout = item.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = item.AddComponent<VerticalLayoutGroup>();
            }

            layout.padding = new RectOffset(10, 10, 6, 6);
            layout.spacing = 2f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private static void ConfigurePanelSurface(GameObject target, Color color, bool withOutline)
        {
            var image = target.GetComponent<Image>();
            if (image == null)
            {
                image = target.AddComponent<Image>();
            }

            image.color = color;
            image.raycastTarget = false;

            if (!withOutline)
            {
                return;
            }

            var outline = target.GetComponent<Outline>();
            if (outline == null)
            {
                outline = target.AddComponent<Outline>();
            }

            outline.effectColor = OutlineColor;
            outline.effectDistance = new Vector2(2f, -2f);
        }

        private static void CreateSectionLabel(Transform parent, string value)
        {
            CreateText(
                value + " Label",
                parent,
                value,
                18,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                RootTextColor,
                24f);
        }

        private static void CreateGuideSectionLabel(Transform parent, string value)
        {
            CreateText(
                value + " Guide Label",
                parent,
                value,
                12,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                RootTextColor,
                16f);
        }

        private static Text CreateCanvasLabel(
            string name,
            Transform parent,
            string value,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color,
            float preferredHeight)
        {
            return CreateText(name, parent, value, fontSize, fontStyle, alignment, color, preferredHeight);
        }

        private static Transform CreateHorizontalRow(string name, Transform parent, float preferredHeight, float spacing)
        {
            var row = new GameObject(name, typeof(RectTransform)).transform;
            row.SetParent(parent, false);

            var layoutElement = row.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            return row;
        }

        private static void CreateSpacer(Transform parent)
        {
            var spacer = new GameObject("Spacer", typeof(RectTransform));
            spacer.transform.SetParent(parent, false);
            var layoutElement = spacer.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;
        }

        private static Button CreateValidateButton(Transform parent, out Text label)
        {
            var buttonRoot = new GameObject("Validate Button", typeof(RectTransform));
            buttonRoot.transform.SetParent(parent, false);

            var image = buttonRoot.AddComponent<Image>();
            image.color = new Color(0.84f, 0.92f, 1f);
            image.raycastTarget = true;

            var button = buttonRoot.AddComponent<Button>();
            button.targetGraphic = image;

            var layoutElement = buttonRoot.AddComponent<LayoutElement>();
            layoutElement.minWidth = 154f;
            layoutElement.preferredWidth = 154f;

            var labelTransform = new GameObject("Button Text", typeof(RectTransform)).GetComponent<RectTransform>();
            labelTransform.SetParent(buttonRoot.transform, false);
            labelTransform.anchorMin = Vector2.zero;
            labelTransform.anchorMax = Vector2.one;
            labelTransform.offsetMin = Vector2.zero;
            labelTransform.offsetMax = Vector2.zero;

            label = labelTransform.gameObject.AddComponent<Text>();
            label.text = "Test Ghost's map";
            label.font = GetBuiltinFont();
            label.fontSize = 14;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(0.12f, 0.18f, 0.30f);
            label.raycastTarget = false;

            return button;
        }

        private static Button CreateOnboardingButton(Transform parent, string labelText)
        {
            var buttonRoot = new GameObject(labelText + " Button", typeof(RectTransform));
            buttonRoot.transform.SetParent(parent, false);

            var image = buttonRoot.AddComponent<Image>();
            image.color = new Color(0.84f, 0.92f, 1f);
            image.raycastTarget = true;

            var button = buttonRoot.AddComponent<Button>();
            button.targetGraphic = image;

            var layoutElement = buttonRoot.AddComponent<LayoutElement>();
            layoutElement.minWidth = 190f;
            layoutElement.preferredWidth = 190f;
            layoutElement.minHeight = 42f;
            layoutElement.preferredHeight = 42f;

            var labelTransform = new GameObject("Button Text", typeof(RectTransform)).GetComponent<RectTransform>();
            labelTransform.SetParent(buttonRoot.transform, false);
            labelTransform.anchorMin = Vector2.zero;
            labelTransform.anchorMax = Vector2.one;
            labelTransform.offsetMin = Vector2.zero;
            labelTransform.offsetMax = Vector2.zero;

            var label = labelTransform.gameObject.AddComponent<Text>();
            label.text = labelText;
            label.font = GetBuiltinFont();
            label.fontSize = 14;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(0.12f, 0.18f, 0.30f);
            label.raycastTarget = false;
            return button;
        }

        private static Text CreateValidationFeedbackText(Transform parent)
        {
            var feedback = new GameObject("Validation Feedback", typeof(RectTransform));
            feedback.transform.SetParent(parent, false);

            var layoutElement = feedback.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;

            var text = feedback.AddComponent<Text>();
            text.font = GetBuiltinFont();
            text.fontSize = 13;
            text.fontStyle = FontStyle.Italic;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;

            return text;
        }

        private static Text CreateText(
            string name,
            Transform parent,
            string value,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color,
            float preferredHeight)
        {
            var label = new GameObject(name, typeof(RectTransform)).AddComponent<Text>();
            label.transform.SetParent(parent, false);
            label.text = value;
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

        private void CreateBoardCenteredText(string value)
        {
            if (nodeLayer == null)
            {
                return;
            }

            var labelRoot = new GameObject("Board Empty State", typeof(RectTransform)).GetComponent<RectTransform>();
            labelRoot.SetParent(nodeLayer, false);
            labelRoot.anchorMin = Vector2.zero;
            labelRoot.anchorMax = Vector2.one;
            labelRoot.offsetMin = Vector2.zero;
            labelRoot.offsetMax = Vector2.zero;

            var text = labelRoot.gameObject.AddComponent<Text>();
            text.text = value;
            text.font = GetBuiltinFont();
            text.fontSize = 18;
            text.fontStyle = FontStyle.Italic;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = SecondaryTextColor;
            text.raycastTarget = false;
        }

        private static void ApplyNodePosition(RectTransform card, Vector2 normalizedPosition)
        {
            card.anchorMin = normalizedPosition;
            card.anchorMax = normalizedPosition;
            card.anchoredPosition = Vector2.zero;
        }

        private static Vector2 LocalPointToNormalizedPosition(RectTransform root, Vector2 localPoint)
        {
            var rect = root.rect;
            return new Vector2(
                Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x),
                Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y));
        }

        private DialogNode FindCurrentNode(string nodeId)
        {
            if (controller == null)
            {
                return null;
            }

            foreach (var node in controller.CurrentNodes)
            {
                if (string.Equals(node.Id, nodeId, StringComparison.Ordinal))
                {
                    return node;
                }
            }

            return null;
        }

        private static string FormatNodeConfig(DialogNode node)
        {
            switch (node.Type)
            {
                case DialogNodeType.IntentBranch:
                    return "Visitor wants help finding something.";
                case DialogNodeType.SlotCheck:
                    return "Check whether Ghost knows the room.";
                case DialogNodeType.Response:
                    if (string.Equals(node.ResponseId, Act3DialogGraphSampleData.AskForRoomResponseId, StringComparison.Ordinal))
                    {
                        return "Ask the visitor which room.";
                    }

                    return "Answer with the location.";
                default:
                    return "Begin Ghost's reply map.";
            }
        }

        private static string FormatTestCaseTitle(DialogGraphTestCase testCase)
        {
            return testCase.Turn.Entities.Count == 0
                ? "Room is missing"
                : "Room is known";
        }

        private static string FormatTestCase(DialogGraphTestCase testCase)
        {
            if (testCase.Turn.Entities.Count == 0)
            {
                return "Visitor asks for the lantern. Ghost should ask which room.";
            }

            if (testCase.Turn.TryGetEntityValue(Act3DialogGraphSampleData.RoomEntityTypeId, out var roomValue))
            {
                return $"Visitor asks for the lantern in {roomValue}. Ghost should answer.";
            }

            return "choose reply";
        }

        private string CreateGhostOutcomeMessage(bool isCorrect, IReadOnlyList<string> errors)
        {
            if (isCorrect)
            {
                return "Ghost understands the route: with a room, it answers; without a room, it politely asks which room.";
            }

            var startNode = FindFirstNode(DialogNodeType.Start);
            var intentNode = FindFirstNode(DialogNodeType.IntentBranch);
            var slotNode = FindFirstNode(DialogNodeType.SlotCheck);
            var answerNode = FindResponseNode(Act3DialogGraphSampleData.AnswerObjectLocationResponseId);
            var askNode = FindResponseNode(Act3DialogGraphSampleData.AskForRoomResponseId);

            if (startNode == null)
            {
                return "Ghost cannot even begin the reply map yet. Add Start here first.";
            }

            if (intentNode == null)
            {
                return "Ghost starts, then drifts away without recognizing that the visitor wants help finding something.";
            }

            if (slotNode == null)
            {
                return "Ghost skips the room check, so it may answer before knowing where to look.";
            }

            if (answerNode == null)
            {
                return "Ghost can check the room, but has no card for answering when the room is known.";
            }

            if (askNode == null)
            {
                return "Ghost can check the room, but has no card for asking a visitor to name the room.";
            }

            if (!HasTransition(startNode.Id, intentNode.Id, DialogTransitionCondition.Always))
            {
                return "Ghost begins, but the start card does not lead to recognizing the request.";
            }

            if (!HasTransition(intentNode.Id, slotNode.Id, DialogTransitionCondition.Always))
            {
                if (HasAnyTransitionToResponse(intentNode.Id))
                {
                    return "Ghost jumps straight to a reply before checking whether the room is known.";
                }

                return "Ghost recognizes the request, but the route does not continue to Check room.";
            }

            if (HasTransition(slotNode.Id, askNode.Id, DialogTransitionCondition.SlotPresent))
            {
                return "The green route is crossed: Ghost asks which room even when the visitor already named it.";
            }

            if (HasTransition(slotNode.Id, answerNode.Id, DialogTransitionCondition.SlotMissing))
            {
                return "The orange route is crossed: Ghost guesses an answer even when the room is missing.";
            }

            if (!HasTransition(slotNode.Id, answerNode.Id, DialogTransitionCondition.SlotPresent))
            {
                return "The green route is missing: when the room is known, Ghost still cannot reach Answer location.";
            }

            if (!HasTransition(slotNode.Id, askNode.Id, DialogTransitionCondition.SlotMissing))
            {
                return "The orange route is missing: when the room is absent, Ghost cannot ask which room.";
            }

            if (errors == null || errors.Count == 0)
            {
                return "Ghost follows the map for a while, then gets stuck before producing the right reply.";
            }

            var firstError = errors[0] ?? string.Empty;

            if (firstError.IndexOf("no nodes", StringComparison.OrdinalIgnoreCase) >= 0
                || firstError.IndexOf("start node is not set", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Ghost has no start point, so the reply map never begins.";
            }

            if (firstError.IndexOf("unreachable", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "One card is floating outside the route, so Ghost never uses it.";
            }

            if (firstError.IndexOf("dead", StringComparison.OrdinalIgnoreCase) >= 0
                || firstError.IndexOf("no usable", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Ghost reaches a dead end and has no next reply to follow.";
            }

            if (firstError.IndexOf("expected response", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Ghost reaches a reply, but it is not the one this visitor needs.";
            }

            if (firstError.IndexOf("intent", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "The visitor's request is not routed through the right part of the map.";
            }

            return "One route still makes Ghost reply in the wrong order.";
        }

        private DialogNode FindFirstNode(DialogNodeType type)
        {
            if (controller == null)
            {
                return null;
            }

            foreach (var node in controller.CurrentNodes)
            {
                if (node.Type == type)
                {
                    return node;
                }
            }

            return null;
        }

        private DialogNode FindResponseNode(string responseId)
        {
            if (controller == null)
            {
                return null;
            }

            foreach (var node in controller.CurrentNodes)
            {
                if (node.Type == DialogNodeType.Response
                    && string.Equals(node.ResponseId, responseId, StringComparison.Ordinal))
                {
                    return node;
                }
            }

            return null;
        }

        private bool HasTransition(string fromId, string toId, DialogTransitionCondition condition)
        {
            if (controller == null)
            {
                return false;
            }

            foreach (var transition in controller.CurrentTransitions)
            {
                if (string.Equals(transition.FromNodeId, fromId, StringComparison.Ordinal)
                    && string.Equals(transition.ToNodeId, toId, StringComparison.Ordinal)
                    && transition.Condition == condition)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasAnyTransitionToResponse(string fromId)
        {
            if (controller == null)
            {
                return false;
            }

            foreach (var transition in controller.CurrentTransitions)
            {
                if (!string.Equals(transition.FromNodeId, fromId, StringComparison.Ordinal))
                {
                    continue;
                }

                var target = FindCurrentNode(transition.ToNodeId);
                if (target != null && target.Type == DialogNodeType.Response)
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetNodeDisplayName(DialogNode node)
        {
            if (node == null)
            {
                return "Unknown card";
            }

            switch (node.Type)
            {
                case DialogNodeType.Start:
                    return "Start here";

                case DialogNodeType.IntentBranch:
                    return "Recognize request";

                case DialogNodeType.SlotCheck:
                    return "Check room";

                case DialogNodeType.Response:
                    if (string.Equals(node.ResponseId, Act3DialogGraphSampleData.AskForRoomResponseId, StringComparison.Ordinal))
                    {
                        return "Ask which room";
                    }

                    return "Answer location";

                default:
                    return "Dialog card";
            }
        }

        private static string CreatePortKey(string nodeId, DialogTransitionCondition condition)
        {
            return nodeId + "|" + condition;
        }

        private static Color GetWireColor(DialogTransitionCondition condition)
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

        private static void SetChildText(Transform root, string childName, string value)
        {
            var child = root.Find(childName);
            if (child == null)
            {
                return;
            }

            var text = child.GetComponent<Text>();
            if (text == null)
            {
                return;
            }

            text.text = value;
            text.raycastTarget = false;
        }

        private static void ConfigureExistingLabel(
            Transform target,
            string value,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color,
            float preferredHeight)
        {
            if (target == null)
            {
                return;
            }

            var text = target.GetComponent<Text>();
            if (text != null)
            {
                text.text = value;
                text.font = GetBuiltinFont();
                text.fontSize = fontSize;
                text.fontStyle = fontStyle;
                text.alignment = alignment;
                text.color = color;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
                text.raycastTarget = false;
            }

            var layoutElement = target.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = target.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;
        }

        private static void ClearChildren(Transform root)
        {
            var children = new List<GameObject>();

            for (var index = 0; index < root.childCount; index++)
            {
                children.Add(root.GetChild(index).gameObject);
            }

            foreach (var child in children)
            {
                child.SetActive(false);

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
            if (FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
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
