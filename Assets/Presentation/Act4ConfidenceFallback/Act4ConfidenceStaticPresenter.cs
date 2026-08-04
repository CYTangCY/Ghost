using Ghost.Presentation.Common;
using System;
using System.Collections.Generic;
using Ghost.Presentation.Banter;
using Ghost.Presentation.GhostAvatar;
using Ghost.Presentation.Shell;
using Ghost.Puzzles.ConfidenceFallback;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Act4ConfidenceFallback
{
    public sealed class Act4ConfidenceStaticPresenter : MonoBehaviour
    {
        private const int SliderLaneInset = 40;

        private const string TitleText = "Act 4: Ghost's Confidence Dial";
        private const string OnboardingTitle = "Goal: stop Ghost from bluffing when unsure";
        private const string OnboardingBody =
            "Each message has a confidence score: how sure Ghost is that it understood the visitor.\n" +
            "Two handles cut that scale into three bands - call Lily, ask to rephrase, or let Ghost answer.\n" +
            "Attach an action to the two bands that need one, then run the evening. There is no perfect setting.";

        private static readonly Color PageColor = new Color(0.96f, 0.94f, 1f);
        private static readonly Color ObjectiveColor = new Color(0.14f, 0.18f, 0.32f);
        private static readonly Color WarmNoteColor = new Color(1f, 0.96f, 0.82f);
        private static readonly Color ConversationColor = new Color(0.93f, 0.97f, 1f);
        private static readonly Color QueueColor = new Color(1f, 0.985f, 0.94f);
        private static readonly Color ControlColor = new Color(0.91f, 0.97f, 1f);
        private static readonly Color AttachedColor = new Color(0.84f, 0.97f, 0.84f);
        private static readonly Color MissingColor = new Color(1f, 0.90f, 0.72f);
        private static readonly Color FailureColor = new Color(1f, 0.84f, 0.80f);

        [SerializeField] private bool renderOnStart = true;

        private readonly List<VisitorPreview> visitorPreviewLabels = new List<VisitorPreview>();
        private Act4ConfidenceInteractionController controller;

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
            controller = new Act4ConfidenceInteractionController();
            controller.StateChanged += RenderState;
            RenderState();
        }

        private void RenderState()
        {
            if (controller == null)
            {
                return;
            }

            AmbientBanterPanel.SetCurrentState(
                GhostNarrativeState.Act4Id,
                controller.BuildHintContext());
            // The previous frame's labels are about to be destroyed; keeping them would leave the
            // refresh loop writing into dead objects.
            visitorPreviewLabels.Clear();
            ClearChildren(transform);
            ConfigureRoot();
            CreateHeader();
            CreateObjectiveStrip();

            if (controller.CurrentPhase == Act4ConfidencePhase.Onboarding)
            {
                CreateOnboardingPanel();
            }
            else
            {
                CreateLilyNoteStrip();
            }

            CreateConversationPanel();

            if (controller.CurrentPhase != Act4ConfidencePhase.Onboarding)
            {
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
            var headerElement = header.gameObject.AddComponent<LayoutElement>();
            headerElement.minHeight = 44f;
            headerElement.preferredHeight = 44f;
            // The header is a fixed title row, but its inner horizontal group force-expands height,
            // which reports flexible height to the page and let the header eat all the spare space.
            headerElement.flexibleHeight = 0f;

            var layout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 16f;
            layout.padding = new RectOffset(0, 220, 0, 0);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var title = GhostUITheme.Label("Title", header, TitleText, GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink);
            title.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var progress = GhostUITheme.Label(
                "Phase Progress",
                header,
                GetProgressText(),
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleRight,
                GhostUITheme.InkSoft);
            var progressElement = progress.gameObject.AddComponent<LayoutElement>();
            progressElement.minWidth = 240f;
            progressElement.preferredWidth = 240f;
        }

        private string GetProgressText()
        {
            switch (controller.CurrentPhase)
            {
                case Act4ConfidencePhase.Onboarding:
                    return "Setup";
                case Act4ConfidencePhase.Playback:
                    return "Visitor " + controller.CurrentVisitorNumber + "/" + controller.Visitors.Count;
                case Act4ConfidencePhase.Complete:
                    return "Phase 3/3";
                default:
                    return controller.ScoresRevealed ? "Revising" : "Reading the queue";
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

            GhostUITheme.Label("Objective Text", strip, GetObjectiveText(), GhostUITheme.HeadingSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.InkOnDark);
        }

        private string GetObjectiveText()
        {
            switch (controller.CurrentPhase)
            {
                case Act4ConfidencePhase.Onboarding:
                    return "Goal: answer clear requests without guessing at uncertain ones";
                case Act4ConfidencePhase.Playback:
                    return "Check each rule: clear -> answer | unsure -> rephrase | upset/complex -> Lily";
                case Act4ConfidencePhase.Complete:
                    return "Complete: Ghost now answers confidently without bluffing";
                default:
                    return controller.RephraseWired && controller.LilyWired
                        ? "Step 3/3: run all six visitors and check where each one goes"
                        : "Steps 1-2: set the answer threshold and attach both safe routes";
            }
        }

        private void CreateOnboardingPanel()
        {
            var panel = GhostUITheme.Panel("Onboarding Panel", transform, WarmNoteColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.86f, 0.58f, 0.22f, 0.95f), new Vector2(2f, -2f));
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

            GhostUITheme.Label("Onboarding Title", panel, OnboardingTitle, GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.28f, 0.18f, 0.08f));
            var body = GhostUITheme.Label("Onboarding Body", panel, OnboardingBody, GhostUITheme.BodySize, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.25f, 0.20f, 0.18f));
            body.lineSpacing = 1.04f;
            body.gameObject.AddComponent<LayoutElement>().preferredHeight = 118f;

            var button = GhostUITheme.PushButton(panel, "Show me the controls", new Color(0.84f, 0.92f, 1f), 210f);
            button.onClick.AddListener(controller.BeginAfterOnboarding);
        }

        private void CreateLilyNoteStrip()
        {
            var panel = GhostUITheme.Panel("Lily Note Strip", transform, WarmNoteColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.86f, 0.58f, 0.22f, 0.85f), new Vector2(1.5f, -1.5f));
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
                "Lily: The threshold is the minimum score needed to answer. Lower answers more; higher asks for help more.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.25f, 0.20f, 0.18f));
            note.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var replay = GhostUITheme.PushButton(panel, "Replay Lily", new Color(1f, 0.98f, 0.88f), 130f);
            replay.interactable = controller.CurrentPhase == Act4ConfidencePhase.Configure;
            replay.onClick.AddListener(controller.ReplayOnboarding);
        }

        private void CreateConversationPanel()
        {
            var panel = GhostUITheme.Panel("Conversation Panel", transform, ConversationColor).rectTransform;
            AddOutline(panel.gameObject, new Color(0.58f, 0.68f, 0.88f, 0.85f), new Vector2(2f, -2f));
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

            var column = GhostUITheme.Panel("Conversation Text Column", panel, Color.clear).rectTransform;
            column.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var columnLayout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            columnLayout.spacing = 4f;
            columnLayout.childControlWidth = true;
            columnLayout.childControlHeight = true;
            columnLayout.childForceExpandWidth = true;
            columnLayout.childForceExpandHeight = false;

            GhostUITheme.Label("Conversation Label", column, GetConversationLabel(), GhostUITheme.HeadingSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink);
            var visitor = GhostUITheme.Label("Visitor Message", column, GetVisitorLine(), GhostUITheme.BodySize, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.10f, 0.18f, 0.30f));
            visitor.gameObject.AddComponent<LayoutElement>().preferredHeight = 42f;
            var outcome = GhostUITheme.Label("Outcome Text", column, controller.StatusLine, GhostUITheme.BodySize, FontStyle.Normal, TextAnchor.UpperLeft, GhostUITheme.InkSoft);
            outcome.lineSpacing = 1.03f;
            outcome.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
        }

        private string GetConversationLabel()
        {
            if (controller.CurrentPhase == Act4ConfidencePhase.Onboarding)
            {
                return "Example: everyone lands in the middle band, so everyone gets asked to repeat themselves";
            }

            if (controller.CurrentPhase == Act4ConfidencePhase.Playback && controller.CurrentVisitorResult != null)
            {
                var result = controller.CurrentVisitorResult;
                return result.Visitor.ConfidenceScore + "% lands in the " + FormatZone(result.Zone) +
                    " band -> " + FormatOutcome(result.Outcome);
            }

            if (controller.CurrentPhase == Act4ConfidencePhase.Complete)
            {
                return "The lab's visitor day is safe";
            }

            return controller.HasFailedRun ? "The last run needs revision" : "Ghost tries the current reply map";
        }

        private string GetVisitorLine()
        {
            if (controller.CurrentPhase == Act4ConfidencePhase.Playback && controller.CurrentVisitorResult != null)
            {
                return "Visitor: " + controller.CurrentVisitorResult.Visitor.Message;
            }

            if (controller.CurrentPhase == Act4ConfidencePhase.Onboarding)
            {
                return "Visitor: Could you deal with that thing from before?";
            }

            return "Bands: call Lily below " + controller.HandoffEdge + "% | ask to rephrase " +
                controller.HandoffEdge + "-" + controller.AnswerEdge + "% | answer from " +
                controller.AnswerEdge + "% | rephrase " + FormatWiring(controller.RephraseWired) +
                " | Lily " + FormatWiring(controller.LilyWired);
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

            CreateVisitorQueue(body);
            CreateConfidenceControls(body);
        }

        private void CreateVisitorQueue(Transform parent)
        {
            var panel = CreateColumnPanel("Visitor Queue", parent, QueueColor, 0.52f);
            var title = GhostUITheme.Label("Queue Title", panel, "Tonight's queue (" + controller.TotalVisitors + ")", GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink);
            var titleElement = title.gameObject.AddComponent<LayoutElement>();
            titleElement.minHeight = 36f;
            titleElement.preferredHeight = 36f;
            var guide = GhostUITheme.Label(
                "Queue Guide",
                panel,
                GetQueueGuideText(),
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft);
            var guideElement = guide.gameObject.AddComponent<LayoutElement>();
            guideElement.minHeight = 30f;
            guideElement.preferredHeight = 30f;

            for (var index = 0; index < controller.Visitors.Count; index++)
            {
                CreateVisitorRow(panel, index, controller.Visitors[index]);
            }
        }

        private void CreateVisitorRow(Transform parent, int index, Act4VisitorMessage visitor)
        {
            var isCurrent = controller.CurrentPhase == Act4ConfidencePhase.Playback && controller.CurrentVisitorNumber == index + 1;
            var row = GhostUITheme.Panel("Visitor " + (index + 1), parent, isCurrent ? new Color(0.86f, 0.94f, 1f) : Color.white).rectTransform;
            AddOutline(row.gameObject, isCurrent ? new Color(0.35f, 0.54f, 0.86f) : new Color(0.76f, 0.70f, 0.84f, 0.55f), new Vector2(1f, -1f));
            var element = row.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 62f;
            element.preferredHeight = 62f;

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 5, 5);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var message = GhostUITheme.Label("Message", row, visitor.Message, GhostUITheme.BodySize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink);
            var messageElement = message.gameObject.AddComponent<LayoutElement>();
            messageElement.flexibleWidth = 1f;
            messageElement.preferredWidth = 0f;

            var signal = controller.ScoresRevealed
                ? "confidence " + visitor.ConfidenceScore + "%"
                : "score hidden";
            var score = GhostUITheme.Label("Signal", row, signal, GhostUITheme.SmallSize, FontStyle.Bold, TextAnchor.MiddleRight, controller.ScoresRevealed ? GhostUITheme.InkSoft : new Color(0.62f, 0.60f, 0.70f));
            var scoreElement = score.gameObject.AddComponent<LayoutElement>();
            scoreElement.minWidth = 118f;
            scoreElement.preferredWidth = 118f;

            // Where this visitor would land right now. Recoloured live as the handles move, so the
            // trade-off is visible before "Run the evening" is ever pressed.
            var status = GhostUITheme.Label("Predicted Route", row, string.Empty, GhostUITheme.SmallSize, FontStyle.Bold, TextAnchor.MiddleRight, GhostUITheme.InkSoft);
            var statusElement = status.gameObject.AddComponent<LayoutElement>();
            statusElement.minWidth = 132f;
            statusElement.preferredWidth = 132f;
            visitorPreviewLabels.Add(new VisitorPreview(visitor, status, row.GetComponent<Image>()));
            ApplyVisitorPreview(visitorPreviewLabels[visitorPreviewLabels.Count - 1]);
        }

        /// <summary>Repaints every queued visitor for the current handle positions.</summary>
        private void RefreshVisitorPreview()
        {
            foreach (var preview in visitorPreviewLabels)
            {
                ApplyVisitorPreview(preview);
            }
        }

        private void ApplyVisitorPreview(VisitorPreview preview)
        {
            if (preview == null || preview.Label == null)
            {
                return;
            }

            var result = controller.PreviewVisitor(preview.Visitor);
            if (result == null)
            {
                return;
            }

            preview.Label.text = controller.ScoresRevealed ? FormatZone(result.Zone) : "-";
            preview.Label.color = !controller.ScoresRevealed
                ? GhostUITheme.InkSoft
                : result.IsAccepted ? GhostUITheme.Good : GhostUITheme.Bad;

            if (preview.Background != null)
            {
                preview.Background.color = !controller.ScoresRevealed
                    ? Color.white
                    : result.IsAccepted ? new Color(0.93f, 0.99f, 0.95f) : new Color(1f, 0.94f, 0.92f);
            }
        }

        private sealed class VisitorPreview
        {
            public VisitorPreview(Act4VisitorMessage visitor, Text label, Image background)
            {
                Visitor = visitor;
                Label = label;
                Background = background;
            }

            public Act4VisitorMessage Visitor { get; }

            public Text Label { get; }

            public Image Background { get; }
        }

        private void CreateConfidenceControls(Transform parent)
        {
            var panel = CreateColumnPanel("Confidence Controls", parent, ControlColor, 0.48f);
            GhostUITheme.Label("Controls Title", panel, "Reply Safety Map", GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.10f, 0.18f, 0.30f));
            GhostUITheme.Label(
                "Rule Summary",
                panel,
                "Three bands: call Lily, ask to rephrase, or answer. Drag the two handles to place them.",
                GhostUITheme.BodySize,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkSoft).gameObject.AddComponent<LayoutElement>().preferredHeight = 28f;

            GhostUITheme.Label("Task Guide Title", panel, "Your task", GhostUITheme.BodySize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink);
            var taskGuide = GhostUITheme.Label(
                "Task Guide",
                panel,
                "1. Read the messages. Scores stay hidden until you have run the evening once.\n" +
                "2. Attach both actions, place the handles where you think the lines go, then run it.\n" +
                "3. No setting suits everyone. Decide which mistake you can live with.",
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.InkSoft);
            taskGuide.lineSpacing = 1.03f;
            taskGuide.gameObject.AddComponent<LayoutElement>().preferredHeight = 62f;

            CreateThresholdSlider(panel);
            CreateRouteControl(panel, "Ask to rephrase", "Attach to the middle band, or Ghost stands there silently.", controller.RephraseWired, controller.ToggleRephraseWiring);
            CreateRouteControl(panel, "Call Lily", "Attach to the bottom band, for the ones a person has to take.", controller.LilyWired, controller.ToggleLilyWiring);

            var spacer = new GameObject("Flexible Spacer", typeof(RectTransform));
            spacer.transform.SetParent(panel, false);
            spacer.AddComponent<LayoutElement>().flexibleHeight = 1f;

            if (controller.HasFailedRun && controller.LastValidationResult != null)
            {
                var failure = GhostUITheme.Label(
                    "Failure Detail",
                    panel,
                    BuildFailureSummary(controller.LastValidationResult.Errors),
                    GhostUITheme.SmallSize,
                    FontStyle.Normal,
                    TextAnchor.UpperLeft,
                    GhostUITheme.Bad);
                var failureElement = failure.gameObject.AddComponent<LayoutElement>();
                failureElement.minHeight = 62f;
                failureElement.preferredHeight = 62f;
                failureElement.flexibleWidth = 1f;
                failureElement.preferredWidth = 0f;
            }

            var action = GhostUITheme.PushButton(panel, GetPrimaryActionLabel(), GetPrimaryActionColor(), 220f);
            action.onClick.AddListener(HandlePrimaryAction);
        }

        private void CreateThresholdSlider(Transform parent)
        {
            var section = GhostUITheme.Panel("Confidence Dial", parent, Color.white).rectTransform;
            AddOutline(section.gameObject, new Color(0.58f, 0.68f, 0.88f, 0.70f), new Vector2(1.5f, -1.5f));
            var element = section.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 164f;
            element.preferredHeight = 164f;

            var layout = section.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 9, 9);
            layout.spacing = 5f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var valueText = GhostUITheme.Label("Dial Value", section, GetBandRuleText(), GhostUITheme.HeadingSize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink);
            valueText.gameObject.AddComponent<LayoutElement>().preferredHeight = 28f;
            var reasonText = GhostUITheme.Label("Dial Reason", section, GetBandReasonText(), GhostUITheme.SmallSize, FontStyle.Normal, TextAnchor.UpperLeft, GhostUITheme.InkSoft);
            reasonText.gameObject.AddComponent<LayoutElement>().preferredHeight = 40f;

            var interactable = controller.CurrentPhase == Act4ConfidencePhase.Configure;

            var lilySlider = CreateSlider(section, controller.HandoffEdge);
            lilySlider.name = "Lily Handle";
            lilySlider.interactable = interactable;

            var answerSlider = CreateSlider(section, controller.AnswerEdge);
            answerSlider.name = "Answer Handle";
            answerSlider.interactable = interactable;

            // Two handles on the same axis. Each one re-reads the controller afterwards, because the
            // controller refuses to let them cross and may have clamped the value.
            lilySlider.onValueChanged.AddListener(value =>
            {
                controller.SetHandoffEdge(Mathf.RoundToInt(value));
                lilySlider.SetValueWithoutNotify(controller.HandoffEdge);
                valueText.text = GetBandRuleText();
                reasonText.text = GetBandReasonText();
                RefreshVisitorPreview();
            });

            answerSlider.onValueChanged.AddListener(value =>
            {
                controller.SetAnswerEdge(Mathf.RoundToInt(value));
                answerSlider.SetValueWithoutNotify(controller.AnswerEdge);
                valueText.text = GetBandRuleText();
                reasonText.text = GetBandReasonText();
                RefreshVisitorPreview();
            });

            var tradeoff = GhostUITheme.Label(
                "Dial Tradeoff",
                section,
                "Top bar: who needs Lily.        Bottom bar: who Ghost may answer itself.",
                GhostUITheme.SmallSize,
                FontStyle.Normal,
                TextAnchor.MiddleCenter,
                GhostUITheme.InkSoft);
            tradeoff.gameObject.AddComponent<LayoutElement>().preferredHeight = 24f;
        }

        private string GetBandRuleText()
        {
            return "Lily below " + controller.HandoffEdge + "%  |  rephrase " + controller.HandoffEdge +
                "-" + controller.AnswerEdge + "%  |  answer from " + controller.AnswerEdge + "%";
        }

        private string GetBandReasonText()
        {
            return "Why: the top bar sets the line below which a person has to take over. The bottom bar " +
                "sets how sure Ghost must be before it answers on its own.";
        }

        private void CreateRouteControl(Transform parent, string title, string description, bool wired, Action toggleAction)
        {
            var row = GhostUITheme.Panel(title + " Route", parent, wired ? AttachedColor : MissingColor).rectTransform;
            AddOutline(row.gameObject, new Color(0.58f, 0.54f, 0.72f, 0.65f), new Vector2(1f, -1f));
            var element = row.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 70f;
            element.preferredHeight = 70f;

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 10, 8, 8);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var column = GhostUITheme.Panel("Route Text", row, Color.clear).rectTransform;
            column.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var columnLayout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            columnLayout.spacing = 2f;
            columnLayout.childControlWidth = true;
            columnLayout.childControlHeight = true;
            columnLayout.childForceExpandWidth = true;
            columnLayout.childForceExpandHeight = false;
            GhostUITheme.Label("Route Title", column, title + " - " + FormatWiring(wired), GhostUITheme.BodySize, FontStyle.Bold, TextAnchor.MiddleLeft, GhostUITheme.Ink);
            GhostUITheme.Label("Route Description", column, description, GhostUITheme.SmallSize, FontStyle.Normal, TextAnchor.MiddleLeft, GhostUITheme.InkSoft);

            var button = GhostUITheme.PushButton(row, wired ? "Detach" : "Attach", wired ? new Color(1f, 0.93f, 0.80f) : new Color(0.84f, 0.92f, 1f), 112f);
            button.interactable = controller.CurrentPhase == Act4ConfidencePhase.Configure;
            button.onClick.AddListener(() => toggleAction());
        }

        private Slider CreateSlider(Transform parent, int value)
        {
            // Both bars share one lane width so they line up with each other.
            var lane = new GameObject("Slider Lane", typeof(RectTransform));
            lane.transform.SetParent(parent, false);
            lane.AddComponent<LayoutElement>().preferredHeight = 26f;
            var laneLayout = lane.AddComponent<HorizontalLayoutGroup>();
            laneLayout.childControlWidth = true;
            laneLayout.childControlHeight = true;
            laneLayout.childForceExpandWidth = true;
            laneLayout.childForceExpandHeight = true;
            laneLayout.childAlignment = TextAnchor.MiddleCenter;
            laneLayout.padding = new RectOffset(SliderLaneInset, SliderLaneInset, 0, 0);

            var root = new GameObject("Threshold Slider", typeof(RectTransform));
            root.transform.SetParent(lane.transform, false);
            root.AddComponent<LayoutElement>().preferredHeight = 26f;

            var background = CreateAnchoredImage("Background", root.transform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0f, 0f), new Vector2(0f, 6f), new Color(0.78f, 0.82f, 0.90f));
            var fillArea = CreateAnchoredRect("Fill Area", root.transform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(5f, 0f), new Vector2(-5f, 6f));
            var fill = CreateAnchoredImage("Fill", fillArea, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.30f, 0.58f, 0.84f));
            var handleArea = CreateAnchoredRect("Handle Slide Area", root.transform, Vector2.zero, Vector2.one, new Vector2(6f, 0f), new Vector2(-6f, 0f));
            var handle = CreateAnchoredImage("Handle", handleArea, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), Vector2.zero, new Vector2(14f, 14f), new Color(1f, 0.82f, 0.36f));
            handle.sprite = GhostUITheme.RoundedSprite(GhostUITheme.ChipRadius);
            handle.type = Image.Type.Sliced;

            var slider = root.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.wholeNumbers = true;
            slider.value = value;
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            background.raycastTarget = true;
            return slider;
        }

        private void HandlePrimaryAction()
        {
            switch (controller.CurrentPhase)
            {
                case Act4ConfidencePhase.Playback:
                    controller.AdvancePlayback();
                    return;
                case Act4ConfidencePhase.Complete:
                    GhostNarrativeState.SetPendingDebriefAct(GhostNarrativeState.Act4Id);
                    SceneManager.LoadScene(ShellSceneNames.GameShellSceneName);
                    return;
                default:
                    controller.RunDay();
                    return;
            }
        }

        private string GetPrimaryActionLabel()
        {
            if (controller.CurrentPhase == Act4ConfidencePhase.Playback)
            {
                return controller.CurrentVisitorNumber >= controller.Visitors.Count ? "Finish the day" : "Next visitor";
            }

            if (controller.CurrentPhase == Act4ConfidencePhase.Complete)
            {
                return "Complete Act";
            }

            return controller.HasFailedRun ? "Try again" : "Run the day";
        }

        private Color GetPrimaryActionColor()
        {
            if (controller.CurrentPhase == Act4ConfidencePhase.Complete)
            {
                return AttachedColor;
            }

            return controller.HasFailedRun ? FailureColor : new Color(0.78f, 0.88f, 1f);
        }

        private RectTransform CreateColumnPanel(string name, Transform parent, Color color, float flexibleWidth)
        {
            var panel = GhostUITheme.Panel(name, parent, color).rectTransform;
            AddOutline(panel.gameObject, new Color(0.70f, 0.68f, 0.86f, 0.75f), new Vector2(2f, -2f));
            var element = panel.gameObject.AddComponent<LayoutElement>();
            element.flexibleWidth = flexibleWidth;
            element.flexibleHeight = 1f;

            // A horizontal layout hands out preferred width first and only shares the remainder by
            // flexibleWidth. Left to itself, a long failure message gave the right column an enormous
            // preferred width and it swallowed the visitor queue. Pinning it to zero makes the split
            // purely proportional.
            element.preferredWidth = 0f;
            element.minWidth = 0f;

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 12, 12);
            layout.spacing = 8f;
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
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            return rect;
        }

        private static Image CreateAnchoredImage(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 sizeDelta,
            Color color)
        {
            var rect = CreateAnchoredRect(name, parent, anchorMin, anchorMax, anchoredPosition, sizeDelta);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }


        /// <summary>
        /// Doubles as the end-of-shift scoreboard once the evening has been run - the "so what" the
        /// single-dial version never gave the player.
        /// </summary>
        private string GetQueueGuideText()
        {
            var result = controller.LastValidationResult;
            if (result == null)
            {
                return "Scores are hidden. Read each message and judge how clear it really is.";
            }

            var tally = result.Tally;
            return "Answered " + tally.Answered + "  |  asked again " + tally.Rephrased +
                "  |  handed to Lily " + tally.HandedOff +
                "     -     made to repeat needlessly " + tally.OverCautious +
                "  |  answered on a guess " + tally.OverConfident;
        }

        /// <summary>
        /// Shows the two most useful problems on their own lines. Concatenating all six into one
        /// paragraph made the label demand a huge width, and the layout took that space off the
        /// visitor queue next to it.
        /// </summary>
        private static string BuildFailureSummary(IReadOnlyList<string> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                return "The evening did not go well.";
            }

            var summary = "Last run: " + errors[0];
            if (errors.Count > 1)
            {
                summary += Environment.NewLine + errors[1];
            }

            if (errors.Count > 2)
            {
                summary += Environment.NewLine + "(+" + (errors.Count - 2) + " more)";
            }

            return summary;
        }

        private static string FormatWiring(bool wired)
        {
            return wired ? "attached" : "missing";
        }

        private static string FormatZone(Act4Zone zone)
        {
            switch (zone)
            {
                case Act4Zone.CallLily:
                    return "call Lily";
                case Act4Zone.AskRephrase:
                    return "ask to rephrase";
                default:
                    return "Ghost answers";
            }
        }

        private static string FormatOutcome(Act4RouteOutcome outcome)
        {
            switch (outcome)
            {
                case Act4RouteOutcome.IntentReply:
                    return "intent reply";
                case Act4RouteOutcome.Fallback:
                    return "ask to rephrase";
                case Act4RouteOutcome.Handoff:
                    return "Lily handoff";
                case Act4RouteOutcome.NoSafeRoute:
                    return "no safe route";
                default:
                    return "meltdown";
            }
        }

        private static GhostMood MapMood(Act4GhostMood mood)
        {
            switch (mood)
            {
                case Act4GhostMood.Happy:
                    return GhostMood.Happy;
                case Act4GhostMood.Confused:
                    return GhostMood.Confused;
                case Act4GhostMood.Sad:
                    return GhostMood.Sad;
                default:
                    return GhostMood.Neutral;
            }
        }

        private static T GetOrAdd<T>(GameObject target)
            where T : Component
        {
            var component = target.GetComponent<T>();
            return component == null ? target.AddComponent<T>() : component;
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
            if (FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

    }
}
