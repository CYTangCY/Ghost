using System.Collections;
using Ghost.Presentation.Shell;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Banter
{
    public static class AmbientBanterHook
    {
        private const string FallbackCanvasName = "Ambient Banter Canvas";
        private const string BootstrapperName = "Ambient Banter Bootstrapper";
        private const string PanelName = "Ambient Banter Panel";
        private const int FallbackSortingOrder = 120;
        private const float CycleSeconds = 6f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RegisterSceneHook()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            ScheduleForScene(SceneManager.GetActiveScene());
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ScheduleForScene(scene);
        }

        private static void ScheduleForScene(Scene scene)
        {
            if (string.IsNullOrWhiteSpace(GetActIdForScene(scene.name)))
            {
                return;
            }

            if (GameObject.Find(PanelName) != null || GameObject.Find(BootstrapperName) != null)
            {
                return;
            }

            var bootstrapperObject = new GameObject(BootstrapperName);
            var bootstrapper = bootstrapperObject.AddComponent<AmbientBanterBootstrapper>();
            bootstrapper.Configure(scene.name);
        }

        internal static void CreateForSceneAfterActLayout(string sceneName)
        {
            var actId = GetActIdForScene(sceneName);
            if (string.IsNullOrWhiteSpace(actId))
            {
                return;
            }

            if (GameObject.Find(PanelName) != null)
            {
                return;
            }

            var beats = BanterData.GetBeats(actId);
            if (beats.Count == 0)
            {
                return;
            }

            EnsureEventSystem();

            var placement = ResolvePlacement(sceneName, actId);
            var panel = CreatePanel(placement, actId);
            panel.Initialize(beats);

            if (placement.UsesLayout)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(placement.Parent);
            }
        }

        private static string GetActIdForScene(string sceneName)
        {
            if (sceneName == ShellSceneNames.Act1SceneName)
            {
                return GhostNarrativeState.Act1Id;
            }

            if (sceneName == ShellSceneNames.Act2SceneName)
            {
                return GhostNarrativeState.Act2Id;
            }

            if (sceneName == ShellSceneNames.Act3SceneName)
            {
                return GhostNarrativeState.Act3Id;
            }

            return null;
        }

        private static BanterPlacement ResolvePlacement(string sceneName, string actId)
        {
            if (actId == GhostNarrativeState.Act3Id)
            {
                var sidePanel = FindRectTransform("Goal Test List");
                if (sidePanel != null)
                {
                    return BanterPlacement.Layout(sidePanel, BanterPanelStyle.Act3Guide());
                }
            }

            if (actId == GhostNarrativeState.Act1Id || actId == GhostNarrativeState.Act2Id)
            {
                var validationRow = FindRectTransform("Validation Controls");
                if (validationRow != null)
                {
                    return BanterPlacement.Layout(
                        validationRow,
                        actId == GhostNarrativeState.Act1Id
                            ? BanterPanelStyle.Act1Validation()
                            : BanterPanelStyle.Act2Validation());
                }
            }

            var canvas = FindSceneCanvas();
            if (canvas == null)
            {
                canvas = CreateFallbackCanvas();
            }

            return BanterPlacement.Absolute(canvas.transform, CreateFallbackRect(sceneName), BanterPanelStyle.Fallback());
        }

        private static BanterRect CreateFallbackRect(string sceneName)
        {
            if (sceneName == ShellSceneNames.Act3SceneName)
            {
                return new BanterRect(
                    new Vector2(1f, 0.5f),
                    new Vector2(1f, 0.5f),
                    new Vector2(1f, 0.5f),
                    new Vector2(-28f, -250f),
                    BanterPanelStyle.Fallback().Size);
            }

            return new BanterRect(
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 22f),
                BanterPanelStyle.Fallback().Size);
        }

        private static AmbientBanterPanel CreatePanel(BanterPlacement placement, string actId)
        {
            var panelRoot = new GameObject(PanelName, typeof(RectTransform));
            panelRoot.transform.SetParent(placement.Parent, false);

            var rect = panelRoot.GetComponent<RectTransform>();
            if (placement.UsesLayout)
            {
                var layoutElement = panelRoot.AddComponent<LayoutElement>();
                layoutElement.minWidth = placement.Style.Size.x;
                layoutElement.preferredWidth = placement.Style.Size.x;
                layoutElement.minHeight = placement.Style.Size.y;
                layoutElement.preferredHeight = placement.Style.Size.y;
                layoutElement.flexibleWidth = placement.Style.FlexibleWidth;
                layoutElement.flexibleHeight = 0f;
            }
            else
            {
                rect.anchorMin = placement.Rect.AnchorMin;
                rect.anchorMax = placement.Rect.AnchorMax;
                rect.pivot = placement.Rect.Pivot;
                rect.anchoredPosition = placement.Rect.Position;
                rect.sizeDelta = placement.Rect.Size;
            }

            var image = panelRoot.AddComponent<Image>();
            image.color = new Color(1f, 0.98f, 0.91f, 0.94f);
            image.raycastTarget = false;

            var outline = panelRoot.AddComponent<Outline>();
            outline.effectColor = new Color(0.62f, 0.56f, 0.78f, 0.72f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var layout = panelRoot.AddComponent<HorizontalLayoutGroup>();
            layout.padding = placement.Style.Padding;
            layout.spacing = placement.Style.Spacing;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var portraitImage = CreatePortrait(panelRoot.transform, out var portraitPlaceholder, placement.Style);
            CreateTextColumn(panelRoot.transform, out var speakerText, out var dialogueText, placement.Style);
            var nextButton = CreateNextButton(panelRoot.transform, placement.Style);

            var panel = panelRoot.AddComponent<AmbientBanterPanel>();
            panel.Configure(speakerText, dialogueText, portraitImage, portraitPlaceholder, nextButton, CycleSeconds, actId);
            return panel;
        }

        private static Image CreatePortrait(Transform parent, out Text placeholder, BanterPanelStyle style)
        {
            var portraitRoot = new GameObject("Speaker Portrait", typeof(RectTransform));
            portraitRoot.transform.SetParent(parent, false);

            var layoutElement = portraitRoot.AddComponent<LayoutElement>();
            layoutElement.minWidth = style.PortraitSize;
            layoutElement.preferredWidth = style.PortraitSize;
            layoutElement.minHeight = style.PortraitSize;
            layoutElement.preferredHeight = style.PortraitSize;

            var image = portraitRoot.AddComponent<Image>();
            image.color = new Color(1f, 0.96f, 0.88f, 0.95f);
            image.raycastTarget = false;

            var outline = portraitRoot.AddComponent<Outline>();
            outline.effectColor = new Color(0.66f, 0.58f, 0.78f, 0.68f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            placeholder = CreateFillText(
                "Portrait Label",
                portraitRoot.transform,
                "Lily",
                style.PortraitFontSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(0.24f, 0.16f, 0.30f));
            placeholder.rectTransform.offsetMin = Vector2.zero;
            placeholder.rectTransform.offsetMax = Vector2.zero;

            return image;
        }

        private static void CreateTextColumn(Transform parent, out Text speakerText, out Text dialogueText, BanterPanelStyle style)
        {
            var column = new GameObject("Banter Text Column", typeof(RectTransform)).GetComponent<RectTransform>();
            column.SetParent(parent, false);

            var layoutElement = column.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;
            layoutElement.minHeight = style.TextColumnMinHeight;

            var layout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = style.TextSpacing;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            speakerText = CreateText(
                "Speaker Name",
                column,
                string.Empty,
                style.SpeakerFontSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.13f, 0.24f),
                style.SpeakerHeight);
            dialogueText = CreateText(
                "Banter Line",
                column,
                string.Empty,
                style.DialogueFontSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.30f, 0.25f, 0.36f),
                style.DialogueHeight);
        }

        private static Button CreateNextButton(Transform parent, BanterPanelStyle style)
        {
            var buttonRoot = new GameObject("Next Banter Button", typeof(RectTransform));
            buttonRoot.transform.SetParent(parent, false);

            var layoutElement = buttonRoot.AddComponent<LayoutElement>();
            layoutElement.minWidth = style.NextButtonWidth;
            layoutElement.preferredWidth = style.NextButtonWidth;
            layoutElement.minHeight = style.NextButtonHeight;
            layoutElement.preferredHeight = style.NextButtonHeight;

            var image = buttonRoot.AddComponent<Image>();
            image.color = new Color(0.86f, 0.92f, 1f, 0.95f);
            image.raycastTarget = true;

            var outline = buttonRoot.AddComponent<Outline>();
            outline.effectColor = new Color(0.50f, 0.56f, 0.78f, 0.74f);
            outline.effectDistance = new Vector2(1.2f, -1.2f);

            var button = buttonRoot.AddComponent<Button>();
            button.targetGraphic = image;

            CreateFillText(
                "Next Label",
                buttonRoot.transform,
                "Ask Lily",
                style.NextButtonFontSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(0.12f, 0.18f, 0.30f));

            return button;
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

        private static Text CreateFillText(
            string name,
            Transform parent,
            string value,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color)
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

            var rect = label.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return label;
        }

        private static RectTransform FindRectTransform(string objectName)
        {
            var target = GameObject.Find(objectName);
            return target == null ? null : target.GetComponent<RectTransform>();
        }

        private static Canvas FindSceneCanvas()
        {
            var mainCanvasObject = GameObject.Find("Canvas");
            var mainCanvas = mainCanvasObject == null ? null : mainCanvasObject.GetComponent<Canvas>();
            if (mainCanvas != null)
            {
                return mainCanvas;
            }

            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var canvas in canvases)
            {
                if (canvas != null &&
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay &&
                    !canvas.gameObject.name.Contains("Overlay") &&
                    canvas.gameObject.name != FallbackCanvasName)
                {
                    return canvas;
                }
            }

            return null;
        }

        private static Canvas CreateFallbackCanvas()
        {
            var existingCanvasObject = GameObject.Find(FallbackCanvasName);
            if (existingCanvasObject != null)
            {
                var existingCanvas = existingCanvasObject.GetComponent<Canvas>();
                if (existingCanvas != null)
                {
                    return existingCanvas;
                }
            }

            var canvasObject = new GameObject(FallbackCanvasName, typeof(RectTransform));
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = FallbackSortingOrder;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
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

        private readonly struct BanterRect
        {
            public BanterRect(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size)
            {
                AnchorMin = anchorMin;
                AnchorMax = anchorMax;
                Pivot = pivot;
                Position = position;
                Size = size;
            }

            public Vector2 AnchorMin { get; }

            public Vector2 AnchorMax { get; }

            public Vector2 Pivot { get; }

            public Vector2 Position { get; }

            public Vector2 Size { get; }
        }

        private readonly struct BanterPanelStyle
        {
            private BanterPanelStyle(
                Vector2 size,
                RectOffset padding,
                float spacing,
                float flexibleWidth,
                float portraitSize,
                int portraitFontSize,
                int speakerFontSize,
                float speakerHeight,
                int dialogueFontSize,
                float dialogueHeight,
                float textColumnMinHeight,
                float textSpacing,
                float nextButtonWidth,
                float nextButtonHeight,
                int nextButtonFontSize)
            {
                Size = size;
                Padding = padding;
                Spacing = spacing;
                FlexibleWidth = flexibleWidth;
                PortraitSize = portraitSize;
                PortraitFontSize = portraitFontSize;
                SpeakerFontSize = speakerFontSize;
                SpeakerHeight = speakerHeight;
                DialogueFontSize = dialogueFontSize;
                DialogueHeight = dialogueHeight;
                TextColumnMinHeight = textColumnMinHeight;
                TextSpacing = textSpacing;
                NextButtonWidth = nextButtonWidth;
                NextButtonHeight = nextButtonHeight;
                NextButtonFontSize = nextButtonFontSize;
            }

            public Vector2 Size { get; }

            public RectOffset Padding { get; }

            public float Spacing { get; }

            public float FlexibleWidth { get; }

            public float PortraitSize { get; }

            public int PortraitFontSize { get; }

            public int SpeakerFontSize { get; }

            public float SpeakerHeight { get; }

            public int DialogueFontSize { get; }

            public float DialogueHeight { get; }

            public float TextColumnMinHeight { get; }

            public float TextSpacing { get; }

            public float NextButtonWidth { get; }

            public float NextButtonHeight { get; }

            public int NextButtonFontSize { get; }

            public static BanterPanelStyle Act1Validation()
            {
                return new BanterPanelStyle(
                    new Vector2(520f, 78f),
                    new RectOffset(10, 10, 8, 8),
                    8f,
                    0f,
                    76f,
                    11,
                    13,
                    18f,
                    13,
                    40f,
                    58f,
                    2f,
                    44f,
                    32f,
                    11);
            }

            public static BanterPanelStyle Act2Validation()
            {
                return new BanterPanelStyle(
                    new Vector2(360f, 48f),
                    new RectOffset(8, 8, 5, 5),
                    7f,
                    0f,
                    32f,
                    9,
                    11,
                    13f,
                    11,
                    21f,
                    34f,
                    1f,
                    68f,
                    26f,
                    10);
            }

            public static BanterPanelStyle Act3Guide()
            {
                return new BanterPanelStyle(
                    new Vector2(270f, 170f),
                    new RectOffset(12, 10, 10, 10),
                    10f,
                    1f,
                    56f,
                    13,
                    15,
                    22f,
                    13,
                    112f,
                    136f,
                    3f,
                    78f,
                    34f,
                    12);
            }

            public static BanterPanelStyle Fallback()
            {
                return new BanterPanelStyle(
                    new Vector2(640f, 128f),
                    new RectOffset(12, 10, 10, 10),
                    10f,
                    1f,
                    58f,
                    13,
                    15,
                    22f,
                    14,
                    72f,
                    96f,
                    3f,
                    78f,
                    34f,
                    12);
            }
        }

        private readonly struct BanterPlacement
        {
            private BanterPlacement(
                RectTransform parent,
                bool usesLayout,
                BanterRect rect,
                BanterPanelStyle style)
            {
                Parent = parent;
                UsesLayout = usesLayout;
                Rect = rect;
                Style = style;
            }

            public RectTransform Parent { get; }

            public bool UsesLayout { get; }

            public BanterRect Rect { get; }

            public BanterPanelStyle Style { get; }

            public static BanterPlacement Layout(RectTransform parent, BanterPanelStyle style)
            {
                return new BanterPlacement(
                    parent,
                    true,
                    new BanterRect(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero),
                    style);
            }

            public static BanterPlacement Absolute(Transform parent, BanterRect rect, BanterPanelStyle style)
            {
                return new BanterPlacement(parent as RectTransform, false, rect, style);
            }
        }
    }

    internal sealed class AmbientBanterBootstrapper : MonoBehaviour
    {
        private string sceneName;

        public void Configure(string loadedSceneName)
        {
            sceneName = loadedSceneName;
        }

        private IEnumerator Start()
        {
            yield return null;
            yield return null;

            AmbientBanterHook.CreateForSceneAfterActLayout(sceneName);
            Destroy(gameObject);
        }
    }
}
