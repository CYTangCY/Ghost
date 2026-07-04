using System;
using System.Collections.Generic;
using Ghost.Presentation.GhostAvatar;
using Ghost.Puzzles.EntityExtraction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Ghost.Presentation.Act2EntityExtraction
{
    public sealed class Act2EntityExtractionStaticPresenter : MonoBehaviour
    {
        private const string TitleText = "Act 2: Ghost's Errand";
        private const string OnboardingTitle = "Lily's quick errand loop";
        private const string OnboardingBody =
            "Lily: Um... before touching the note, watch Ghost mess up one errand.\n" +
            "Lily: Then split the sentence into word tokens and put the useful details into Ghost's action card.\n" +
            "Lily: Send Ghost. If a slot is fuzzy, the errand itself will show what went wrong.";

        private static readonly Color PageColor = new Color(0.96f, 0.94f, 1f);
        private static readonly Color PanelColor = new Color(1f, 0.985f, 0.94f);
        private static readonly Color BluePanelColor = new Color(0.91f, 0.97f, 1f);
        private static readonly Color ConversationColor = new Color(0.93f, 0.97f, 1f);
        private static readonly Color ObjectiveColor = new Color(0.14f, 0.18f, 0.32f);
        private static readonly Color WarmNoteColor = new Color(1f, 0.96f, 0.82f);
        private static readonly Color TextColor = new Color(0.14f, 0.11f, 0.22f);
        private static readonly Color SubtleTextColor = new Color(0.35f, 0.32f, 0.45f);
        private static readonly Color TokenColor = new Color(1f, 0.985f, 0.92f);
        private static readonly Color SelectedTokenColor = new Color(1f, 0.93f, 0.68f);
        private static readonly Color AssignedTokenColor = new Color(0.90f, 1f, 0.92f);
        private static readonly Color SystemSlotColor = new Color(0.85f, 0.93f, 1f);
        private static readonly Color CustomSlotColor = new Color(0.90f, 1f, 0.92f);
        private static readonly Color CorrectColor = new Color(0.76f, 0.96f, 0.78f);
        private static readonly Color MissingColor = new Color(1f, 0.90f, 0.72f);
        private static readonly Color WrongColor = new Color(1f, 0.82f, 0.78f);

        [SerializeField] private bool renderOnStart = true;

        private Act2EntityExtractionInteractionController controller;

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

        public void Configure(
            RectTransform messageChipRoot,
            RectTransform entityPaletteRoot,
            RectTransform validationControlsRoot,
            GameObject chipTemplate,
            GameObject entityTypeTemplate,
            bool renderOnStart)
        {
            this.renderOnStart = renderOnStart;
        }

        public void RenderSampleData()
        {
            EnsureEventSystem();
            DetachController();
            controller = new Act2EntityExtractionInteractionController();
            controller.StateChanged += RenderState;
            RenderState();
        }

        private void RenderState()
        {
            if (controller == null)
            {
                return;
            }

            ClearChildren(transform);
            ConfigureRoot();
            CreateHeader();
            CreateObjectiveStrip();

            if (controller.CurrentPhase == Act2ErrandPhase.Onboarding)
            {
                CreateOnboardingPanel();
                CreateConversationPanel();
            }
            else
            {
                CreateLilyNoteStrip();
                CreateConversationPanel();
                CreateMainBody();
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }

        private void ConfigureRoot()
        {
            var image = GetOrAdd<Image>(gameObject);
            image.color = PageColor;
            image.raycastTarget = false;

            var layout = GetOrAdd<VerticalLayoutGroup>(gameObject);
            layout.padding = new RectOffset(36, 36, 26, 24);
            layout.spacing = 9f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private void CreateHeader()
        {
            var header = CreateLayoutPanel("Header", transform, new Color(1f, 1f, 1f, 0f));
            var headerLayout = GetOrAdd<HorizontalLayoutGroup>(header.gameObject);
            headerLayout.spacing = 16f;
            headerLayout.childControlWidth = true;
            headerLayout.childControlHeight = true;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = true;

            var layoutElement = header.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 56f;
            layoutElement.minHeight = 56f;

            var title = CreateText("Title", header, TitleText, 38, FontStyle.Bold, TextAnchor.MiddleLeft, TextColor);
            var titleLayout = title.gameObject.AddComponent<LayoutElement>();
            titleLayout.flexibleWidth = 1f;

            var progress = CreateText(
                "Errand Progress",
                header,
                "Errand " + controller.CurrentErrandNumber + "/" + controller.ErrandCount,
                20,
                FontStyle.Bold,
                TextAnchor.MiddleRight,
                SubtleTextColor);
            var progressLayout = progress.gameObject.AddComponent<LayoutElement>();
            progressLayout.preferredWidth = 210f;
            progressLayout.minWidth = 210f;
        }

        private void CreateObjectiveStrip()
        {
            var strip = CreateLayoutPanel("Objective Strip", transform, ObjectiveColor);
            var layoutElement = strip.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 48f;
            layoutElement.preferredHeight = 48f;

            var layout = GetOrAdd<HorizontalLayoutGroup>(strip.gameObject);
            layout.padding = new RectOffset(18, 18, 7, 7);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            var label = CreateText("Objective Text", strip, GetObjectiveText(), 18, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
        }

        private string GetObjectiveText()
        {
            switch (controller.CurrentPhase)
            {
                case Act2ErrandPhase.Onboarding:
                    return "Setup: learn the loop before touching Ghost's errand card";
                case Act2ErrandPhase.IntroFail:
                    return "1/3 Watch Ghost's errand fail, then split the sentence";
                case Act2ErrandPhase.Fill:
                    if (controller.LastOutcome != null && !controller.LastOutcome.IsSuccess)
                    {
                        return "Retry: fix the highlighted slot result, then send Ghost again";
                    }

                    return "2/3 Split + fill " + FormatCurrentSlotNames() + " with token details";
                case Act2ErrandPhase.Run:
                    return "3/3 Send Ghost and check the slot outcome";
                case Act2ErrandPhase.Complete:
                    return "Complete: every errand worked through deterministic entity extraction";
                default:
                    return "Act 2 errand";
            }
        }

        private string FormatCurrentSlotNames()
        {
            var names = new List<string>();
            foreach (var slot in controller.CurrentErrand.Slots)
            {
                names.Add(slot.DisplayName);
            }

            return string.Join("/", names.ToArray());
        }

        private void CreateOnboardingPanel()
        {
            var panel = CreateLayoutPanel("Onboarding Panel", transform, WarmNoteColor);
            AddOutline(panel.gameObject, new Color(0.86f, 0.58f, 0.22f, 0.95f), new Vector2(2f, -2f));

            var layoutElement = panel.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 180f;
            layoutElement.preferredHeight = 180f;

            var layout = GetOrAdd<VerticalLayoutGroup>(panel.gameObject);
            layout.padding = new RectOffset(18, 18, 12, 12);
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateText("Onboarding Title", panel, OnboardingTitle, 20, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.28f, 0.18f, 0.08f));
            var body = CreateText("Onboarding Body", panel, OnboardingBody, 17, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.25f, 0.20f, 0.18f));
            body.lineSpacing = 1.04f;
            body.gameObject.AddComponent<LayoutElement>().preferredHeight = 88f;

            var button = CreateButton(panel, "Watch Ghost fail", new Color(0.84f, 0.92f, 1f), 190f);
            button.onClick.AddListener(controller.BeginAfterOnboarding);
        }

        private void CreateLilyNoteStrip()
        {
            var panel = CreateLayoutPanel("Lily Note Strip", transform, WarmNoteColor);
            AddOutline(panel.gameObject, new Color(0.86f, 0.58f, 0.22f, 0.85f), new Vector2(1.5f, -1.5f));

            var layoutElement = panel.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 54f;
            layoutElement.preferredHeight = 54f;

            var layout = GetOrAdd<HorizontalLayoutGroup>(panel.gameObject);
            layout.padding = new RectOffset(16, 12, 7, 7);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var note = CreateText(
                "Lily Note",
                panel,
                "Lily: Um... split the sentence, tag only the detail Ghost needs, then let the errand prove it.",
                15,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.25f, 0.20f, 0.18f));
            note.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var replay = CreateButton(panel, "Replay Lily", new Color(1f, 0.98f, 0.88f), 130f);
            replay.onClick.AddListener(controller.ReplayOnboarding);
        }

        private void CreateConversationPanel()
        {
            var panel = CreateLayoutPanel("Conversation Panel", transform, ConversationColor);
            AddOutline(panel.gameObject, new Color(0.58f, 0.68f, 0.88f, 0.85f), new Vector2(2f, -2f));

            var layoutElement = panel.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 170f;
            layoutElement.preferredHeight = 170f;

            var layout = GetOrAdd<HorizontalLayoutGroup>(panel.gameObject);
            layout.padding = new RectOffset(18, 18, 10, 10);
            layout.spacing = 16f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var faceRoot = new GameObject("Ghost Face", typeof(RectTransform));
            faceRoot.transform.SetParent(panel, false);
            var faceLayout = faceRoot.AddComponent<LayoutElement>();
            faceLayout.minWidth = 150f;
            faceLayout.preferredWidth = 150f;
            faceLayout.minHeight = 150f;
            faceLayout.preferredHeight = 150f;
            var face = faceRoot.AddComponent<GhostFaceView>();
            face.SetMood(MapMood(controller.CurrentMood));

            var textColumn = CreateLayoutPanel("Conversation Text Column", panel, new Color(1f, 1f, 1f, 0f));
            textColumn.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var columnLayout = GetOrAdd<VerticalLayoutGroup>(textColumn.gameObject);
            columnLayout.spacing = 4f;
            columnLayout.childControlWidth = true;
            columnLayout.childControlHeight = true;
            columnLayout.childForceExpandWidth = true;
            columnLayout.childForceExpandHeight = false;

            CreateText("Errand Label", textColumn, GetConversationLabel(), 18, FontStyle.Bold, TextAnchor.MiddleLeft, TextColor);
            var message = CreateText("Visitor Message", textColumn, "Visitor note: " + controller.MessageText, 17, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.10f, 0.18f, 0.30f));
            message.gameObject.AddComponent<LayoutElement>().preferredHeight = 36f;

            var outcome = CreateText("Outcome Text", textColumn, GetConversationOutcomeText(), 16, FontStyle.Normal, TextAnchor.UpperLeft, SubtleTextColor);
            outcome.lineSpacing = 1.03f;
            outcome.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
        }

        private string GetConversationLabel()
        {
            if (controller.CurrentPhase == Act2ErrandPhase.Onboarding)
            {
                return "Ghost has an errand problem";
            }

            if (controller.CurrentPhase == Act2ErrandPhase.Complete)
            {
                return "Ghost's errands are working";
            }

            return "Errand " + controller.CurrentErrandNumber + ": Ghost tries using the note";
        }

        private string GetConversationOutcomeText()
        {
            if (controller.CurrentPhase == Act2ErrandPhase.Onboarding)
            {
                return "The next screen shows Ghost failing first, then asks you to turn the sentence into usable details.";
            }

            if (string.IsNullOrWhiteSpace(controller.OutcomeLine))
            {
                return "Ghost is waiting for the action card.";
            }

            return "Ghost: " + controller.OutcomeLine;
        }

        private void CreateMainBody()
        {
            var body = CreateLayoutPanel("Main Body", transform, new Color(1f, 1f, 1f, 0f));
            var bodyLayoutElement = body.gameObject.AddComponent<LayoutElement>();
            bodyLayoutElement.flexibleHeight = 1f;

            var bodyLayout = GetOrAdd<HorizontalLayoutGroup>(body.gameObject);
            bodyLayout.spacing = 18f;
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = true;
            bodyLayout.childForceExpandHeight = true;

            CreateMessagePanel(body);
            CreateActionCardPanel(body);
        }

        private void CreateMessagePanel(Transform parent)
        {
            var panel = CreateColumnPanel("Message Panel", parent, PanelColor, 0.52f);
            CreateText("Message Panel Title", panel, controller.HasSplitCurrentMessage ? "Message Tokens" : "Solid Sentence", 25, FontStyle.Bold, TextAnchor.MiddleLeft, TextColor);

            if (!controller.HasSplitCurrentMessage)
            {
                CreateSolidSentence(panel);
                var splitButton = CreateButton(panel, "Split", new Color(1f, 0.93f, 0.68f), 150f);
                splitButton.interactable = controller.CurrentPhase == Act2ErrandPhase.IntroFail;
                splitButton.onClick.AddListener(controller.SplitMessage);
                return;
            }

            CreateTokenGrid(panel);
        }

        private void CreateSolidSentence(Transform parent)
        {
            var sentence = CreateLayoutPanel("Solid Sentence Card", parent, new Color(1f, 0.99f, 0.94f));
            AddOutline(sentence.gameObject, new Color(0.78f, 0.70f, 0.88f, 0.72f), new Vector2(2f, -2f));
            sentence.gameObject.AddComponent<LayoutElement>().preferredHeight = 130f;

            var layout = GetOrAdd<VerticalLayoutGroup>(sentence.gameObject);
            layout.padding = new RectOffset(16, 16, 14, 14);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            var text = CreateText("Sentence Text", sentence, controller.MessageText, 24, FontStyle.Bold, TextAnchor.MiddleCenter, TextColor);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
        }

        private void CreateTokenGrid(Transform parent)
        {
            var gridPanel = CreateLayoutPanel("Token Drop Area", parent, new Color(1f, 1f, 1f, 0f));
            gridPanel.GetComponent<Image>().raycastTarget = true;
            gridPanel.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
            var returnDrop = GetOrAdd<Act2EntityTokenReturnDropTarget>(gridPanel.gameObject);
            returnDrop.Configure(controller.RemoveTokenAssignment);

            var grid = GetOrAdd<GridLayoutGroup>(gridPanel.gameObject);
            grid.cellSize = new Vector2(142f, 54f);
            grid.spacing = new Vector2(9f, 9f);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperLeft;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;

            foreach (var token in controller.Tokens)
            {
                CreateTokenChip(gridPanel, token, false);
            }
        }

        private void CreateActionCardPanel(Transform parent)
        {
            var panel = CreateColumnPanel("Action Card Panel", parent, BluePanelColor, 0.48f);
            CreateText("Action Card Title", panel, "Ghost's Action Card", 25, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.10f, 0.18f, 0.30f));

            var slotList = CreateLayoutPanel("Slot List", panel, new Color(1f, 1f, 1f, 0f));
            slotList.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;

            var listLayout = GetOrAdd<VerticalLayoutGroup>(slotList.gameObject);
            listLayout.spacing = 9f;
            listLayout.childControlWidth = true;
            listLayout.childControlHeight = true;
            listLayout.childForceExpandWidth = true;
            listLayout.childForceExpandHeight = false;

            foreach (var slot in controller.CurrentErrand.Slots)
            {
                CreateSlotView(slotList, slot);
            }

            CreateActionButtons(panel);
        }

        private RectTransform CreateColumnPanel(string name, Transform parent, Color color, float flexibleWidth)
        {
            var panel = CreateLayoutPanel(name, parent, color);
            AddOutline(panel.gameObject, new Color(0.70f, 0.68f, 0.86f, 0.75f), new Vector2(2f, -2f));

            var layoutElement = panel.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = flexibleWidth;
            layoutElement.flexibleHeight = 1f;

            var layout = GetOrAdd<VerticalLayoutGroup>(panel.gameObject);
            layout.padding = new RectOffset(18, 18, 14, 14);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            return panel;
        }

        private void CreateSlotView(Transform parent, Act2ErrandDemoData.ErrandSlot slot)
        {
            var slotResult = controller.GetSlotResult(slot.SlotId);
            var assignment = controller.GetAssignment(slot.SlotId);
            var slotColor = GetSlotColor(slot, slotResult);
            var view = CreateLayoutPanel("Slot " + slot.DisplayName, parent, slotColor);
            AddOutline(view.gameObject, new Color(0.46f, 0.58f, 0.78f, 0.82f), new Vector2(2f, -2f));

            var layoutElement = view.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 128f;
            layoutElement.preferredHeight = 128f;

            var dropTarget = GetOrAdd<Act2EntitySlotDropTarget>(view.gameObject);
            dropTarget.Configure(slot.SlotId, HandleSlotDrop);

            var button = GetOrAdd<Button>(view.gameObject);
            button.targetGraphic = view.GetComponent<Image>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => controller.AssignSelectedTokenToSlot(slot.SlotId));
            button.interactable = controller.CurrentPhase == Act2ErrandPhase.Fill;

            var layout = GetOrAdd<VerticalLayoutGroup>(view.gameObject);
            layout.padding = new RectOffset(14, 14, 8, 8);
            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var title = CreateText("Slot Header", view, slot.DisplayName + "  " + slot.KindLabel, 18, FontStyle.Bold, TextAnchor.MiddleLeft, TextColor);
            title.gameObject.AddComponent<LayoutElement>().preferredHeight = 26f;

            if (assignment == null)
            {
                CreateText("Slot Placeholder", view, "Drop matching message token here.", 15, FontStyle.Italic, TextAnchor.MiddleLeft, SubtleTextColor);
            }
            else
            {
                CreateAssignedTokenChip(view, assignment);
                var resolution = GetResolutionText(slot, assignment, slotResult);
                if (!string.IsNullOrWhiteSpace(resolution))
                {
                    CreateText("Resolution", view, resolution, 14, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.12f, 0.44f, 0.26f));
                }
            }

            if (slotResult != null)
            {
                CreateText("Slot State", view, FormatSlotResult(slotResult), 14, FontStyle.Bold, TextAnchor.MiddleLeft, GetSlotStateTextColor(slotResult.State));
            }
        }

        private void CreateAssignedTokenChip(Transform parent, Act2EntityExtractionInteractionController.SlotAssignment assignment)
        {
            var chip = CreateTokenSurface("Assigned Token - " + assignment.TokenText, parent, assignment.TokenText, AssignedTokenColor);
            var dragView = chip.gameObject.AddComponent<Act2EntityTokenDragView>();
            dragView.Configure(assignment.ChipKey, assignment.TokenText);

            var button = chip.gameObject.AddComponent<Button>();
            button.targetGraphic = chip.GetComponent<Image>();
            button.onClick.AddListener(() => controller.RemoveTokenAssignment(assignment.ChipKey));
        }

        private void CreateTokenChip(Transform parent, Act2EntityExtractionInteractionController.TokenInfo token, bool assignedView)
        {
            var assignedSlot = controller.GetAssignedSlot(token.ChipKey);
            var color = assignedSlot.HasValue
                ? AssignedTokenColor
                : controller.IsSelected(token.ChipKey)
                    ? SelectedTokenColor
                    : TokenColor;

            var chip = CreateTokenSurface("Token - " + token.Text, parent, token.Text, color);
            var dragView = chip.gameObject.AddComponent<Act2EntityTokenDragView>();
            dragView.Configure(token.ChipKey, token.Text);

            var chipView = chip.gameObject.AddComponent<Act2EntityChipView>();
            chipView.Configure(token.Start, token.Length, token.Text);

            var button = chip.gameObject.AddComponent<Button>();
            button.targetGraphic = chip.GetComponent<Image>();
            button.onClick.AddListener(() =>
            {
                if (assignedSlot.HasValue)
                {
                    controller.RemoveTokenAssignment(token.ChipKey);
                    return;
                }

                controller.SelectToken(token.ChipKey);
            });
        }

        private RectTransform CreateTokenSurface(string name, Transform parent, string tokenText, Color color)
        {
            var chip = CreateLayoutPanel(name, parent, color);
            AddOutline(chip.gameObject, new Color(0.78f, 0.70f, 0.88f, 0.72f), new Vector2(1.5f, -1.5f));

            var layout = GetOrAdd<HorizontalLayoutGroup>(chip.gameObject);
            layout.padding = new RectOffset(10, 10, 6, 6);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            var label = CreateText("ChipText", chip, tokenText, 17, FontStyle.Bold, TextAnchor.MiddleCenter, TextColor);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;

            return chip;
        }

        private void CreateActionButtons(Transform parent)
        {
            var row = CreateLayoutPanel("Action Buttons", parent, new Color(1f, 1f, 1f, 0f));
            var layoutElement = row.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 58f;
            layoutElement.preferredHeight = 58f;

            var layout = GetOrAdd<HorizontalLayoutGroup>(row.gameObject);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            if (controller.CurrentPhase == Act2ErrandPhase.Fill)
            {
                var runLabel = controller.LastOutcome != null && !controller.LastOutcome.IsSuccess
                    ? "Try again"
                    : "Go, Ghost!";
                var run = CreateButton(row, runLabel, new Color(0.84f, 0.92f, 1f), 170f);
                run.onClick.AddListener(controller.RunErrand);
                return;
            }

            if (controller.CurrentPhase == Act2ErrandPhase.Run && controller.LastOutcome != null && controller.LastOutcome.IsSuccess)
            {
                var label = controller.CurrentErrandNumber == controller.ErrandCount ? "Complete" : "Next errand";
                var next = CreateButton(row, label, new Color(0.78f, 0.94f, 0.80f), 170f);
                next.onClick.AddListener(controller.ContinueAfterSuccess);
                return;
            }

            if (controller.CurrentPhase == Act2ErrandPhase.Run)
            {
                var revise = CreateButton(row, "Revise card", new Color(1f, 0.90f, 0.72f), 170f);
                revise.onClick.AddListener(controller.ReviseCurrentErrand);
                return;
            }

            if (controller.CurrentPhase == Act2ErrandPhase.Complete)
            {
                CreateText("Complete Text", row, "All errands pass the existing entity validator.", 16, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.14f, 0.46f, 0.24f));
            }
        }

        private void HandleSlotDrop(Act2ErrandSlotId slotId, string chipKey)
        {
            controller.AssignTokenToSlot(chipKey, slotId);
        }

        private static Color GetSlotColor(Act2ErrandDemoData.ErrandSlot slot, Act2ErrandSlotResult result)
        {
            if (result != null)
            {
                switch (result.State)
                {
                    case Act2ErrandSlotState.Correct:
                        return CorrectColor;
                    case Act2ErrandSlotState.Missing:
                        return MissingColor;
                    case Act2ErrandSlotState.Wrong:
                        return WrongColor;
                }
            }

            return slot.EntityType.Category == EntityCategory.System ? SystemSlotColor : CustomSlotColor;
        }

        private static Color GetSlotStateTextColor(Act2ErrandSlotState state)
        {
            switch (state)
            {
                case Act2ErrandSlotState.Correct:
                    return new Color(0.12f, 0.44f, 0.24f);
                case Act2ErrandSlotState.Missing:
                    return new Color(0.64f, 0.38f, 0.08f);
                case Act2ErrandSlotState.Wrong:
                    return new Color(0.64f, 0.16f, 0.12f);
                default:
                    return SubtleTextColor;
            }
        }

        private static string FormatSlotResult(Act2ErrandSlotResult result)
        {
            switch (result.State)
            {
                case Act2ErrandSlotState.Correct:
                    return "Correct: Ghost can use " + result.ExpectedSurfaceText + ".";
                case Act2ErrandSlotState.Missing:
                    return "Missing: this detail is still blank.";
                case Act2ErrandSlotState.Wrong:
                    return "Wrong: Ghost put the wrong token here.";
                default:
                    return string.Empty;
            }
        }

        private static string GetResolutionText(
            Act2ErrandDemoData.ErrandSlot slot,
            Act2EntityExtractionInteractionController.SlotAssignment assignment,
            Act2ErrandSlotResult slotResult)
        {
            if (slotResult != null && !string.IsNullOrWhiteSpace(slotResult.ResolutionText))
            {
                return slotResult.ResolutionText;
            }

            foreach (var resolution in Act2ErrandDemoData.CreateSynonymResolutions())
            {
                if (resolution.Matches(slot.EntityType.Id, assignment.TokenText))
                {
                    return assignment.TokenText + " -> " + resolution.CanonicalLabel;
                }
            }

            return string.Empty;
        }

        private static GhostMood MapMood(Act2ErrandGhostMood mood)
        {
            switch (mood)
            {
                case Act2ErrandGhostMood.Happy:
                    return GhostMood.Happy;
                case Act2ErrandGhostMood.Confused:
                    return GhostMood.Confused;
                case Act2ErrandGhostMood.Sad:
                    return GhostMood.Sad;
                default:
                    return GhostMood.Neutral;
            }
        }

        private Button CreateButton(Transform parent, string labelText, Color color, float width)
        {
            var root = CreateLayoutPanel(labelText + " Button", parent, color);
            AddOutline(root.gameObject, new Color(0.48f, 0.54f, 0.76f, 0.70f), new Vector2(2f, -2f));

            var layoutElement = root.gameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = width;
            layoutElement.preferredWidth = width;
            layoutElement.minHeight = 42f;

            var button = root.gameObject.AddComponent<Button>();
            button.targetGraphic = root.GetComponent<Image>();

            var label = CreateText("Button Text", root, labelText, 16, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.12f, 0.18f, 0.30f));
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = Vector2.zero;
            label.rectTransform.offsetMax = Vector2.zero;
            return button;
        }

        private static RectTransform CreateLayoutPanel(string name, Transform parent, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            panel.SetParent(parent, false);
            var image = panel.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = color.a > 0.01f;
            return panel;
        }

        private static Text CreateText(
            string name,
            Transform parent,
            string value,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color)
        {
            var text = new GameObject(name, typeof(RectTransform)).AddComponent<Text>();
            text.transform.SetParent(parent, false);
            text.text = value ?? string.Empty;
            text.font = GetBuiltinFont();
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        private static T GetOrAdd<T>(GameObject target)
            where T : Component
        {
            var component = target.GetComponent<T>();
            if (component == null)
            {
                component = target.AddComponent<T>();
            }

            return component;
        }

        private static void AddOutline(GameObject target, Color color, Vector2 distance)
        {
            var outline = target.GetComponent<Outline>();
            if (outline == null)
            {
                outline = target.AddComponent<Outline>();
            }

            outline.effectColor = color;
            outline.effectDistance = distance;
        }

        private void DetachController()
        {
            if (controller == null)
            {
                return;
            }

            controller.StateChanged -= RenderState;
            controller = null;
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
            if (FindAnyObjectByType<EventSystem>() != null)
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
