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
        private const string CanvasName = "Ambient Banter Canvas";
        private const string PanelName = "Ambient Banter Panel";
        private const int SortingOrder = 30000;
        private const float CycleSeconds = 6f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RegisterSceneHook()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            CreateForScene(SceneManager.GetActiveScene());
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CreateForScene(scene);
        }

        private static void CreateForScene(Scene scene)
        {
            var actId = GetActIdForScene(scene.name);
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

            var canvas = CreateCanvas();
            var panel = CreatePanel(canvas.transform);
            panel.Initialize(beats);
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

        private static Canvas CreateCanvas()
        {
            var existingCanvasObject = GameObject.Find(CanvasName);
            if (existingCanvasObject != null)
            {
                var existingCanvas = existingCanvasObject.GetComponent<Canvas>();
                if (existingCanvas != null)
                {
                    return existingCanvas;
                }
            }

            var canvasObject = new GameObject(CanvasName, typeof(RectTransform));
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = SortingOrder;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static AmbientBanterPanel CreatePanel(Transform parent)
        {
            var panelRoot = new GameObject(PanelName, typeof(RectTransform));
            panelRoot.transform.SetParent(parent, false);

            var rect = panelRoot.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition = new Vector2(-28f, 28f);
            rect.sizeDelta = new Vector2(520f, 136f);

            var image = panelRoot.AddComponent<Image>();
            image.color = new Color(1f, 0.98f, 0.91f, 0.92f);
            image.raycastTarget = false;

            var outline = panelRoot.AddComponent<Outline>();
            outline.effectColor = new Color(0.62f, 0.56f, 0.78f, 0.72f);
            outline.effectDistance = new Vector2(2f, -2f);

            var layout = panelRoot.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(14, 12, 12, 12);
            layout.spacing = 12f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var portraitImage = CreatePortrait(panelRoot.transform, out var portraitPlaceholder);
            var textColumn = CreateTextColumn(panelRoot.transform, out var speakerText, out var dialogueText);
            var nextButton = CreateNextButton(panelRoot.transform);

            var panel = panelRoot.AddComponent<AmbientBanterPanel>();
            panel.Configure(speakerText, dialogueText, portraitImage, portraitPlaceholder, nextButton, CycleSeconds);
            return panel;
        }

        private static Image CreatePortrait(Transform parent, out Text placeholder)
        {
            var portraitRoot = new GameObject("Speaker Portrait", typeof(RectTransform));
            portraitRoot.transform.SetParent(parent, false);

            var layoutElement = portraitRoot.AddComponent<LayoutElement>();
            layoutElement.minWidth = 76f;
            layoutElement.preferredWidth = 76f;
            layoutElement.minHeight = 76f;
            layoutElement.preferredHeight = 76f;

            var image = portraitRoot.AddComponent<Image>();
            image.color = new Color(1f, 0.96f, 0.88f, 0.95f);
            image.raycastTarget = false;

            var outline = portraitRoot.AddComponent<Outline>();
            outline.effectColor = new Color(0.66f, 0.58f, 0.78f, 0.68f);
            outline.effectDistance = new Vector2(2f, -2f);

            placeholder = CreateFillText(
                "Portrait Label",
                portraitRoot.transform,
                "Lily",
                16,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(0.24f, 0.16f, 0.30f));
            placeholder.rectTransform.offsetMin = Vector2.zero;
            placeholder.rectTransform.offsetMax = Vector2.zero;

            return image;
        }

        private static RectTransform CreateTextColumn(Transform parent, out Text speakerText, out Text dialogueText)
        {
            var column = new GameObject("Banter Text Column", typeof(RectTransform)).GetComponent<RectTransform>();
            column.SetParent(parent, false);

            var layoutElement = column.gameObject.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;
            layoutElement.minHeight = 96f;

            var layout = column.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            speakerText = CreateText(
                "Speaker Name",
                column,
                string.Empty,
                18,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.18f, 0.13f, 0.24f),
                24f);
            dialogueText = CreateText(
                "Banter Line",
                column,
                string.Empty,
                16,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.30f, 0.25f, 0.36f),
                62f);

            return column;
        }

        private static Button CreateNextButton(Transform parent)
        {
            var buttonRoot = new GameObject("Next Banter Button", typeof(RectTransform));
            buttonRoot.transform.SetParent(parent, false);

            var layoutElement = buttonRoot.AddComponent<LayoutElement>();
            layoutElement.minWidth = 58f;
            layoutElement.preferredWidth = 58f;
            layoutElement.minHeight = 36f;
            layoutElement.preferredHeight = 36f;

            var image = buttonRoot.AddComponent<Image>();
            image.color = new Color(0.86f, 0.92f, 1f, 0.95f);
            image.raycastTarget = true;

            var outline = buttonRoot.AddComponent<Outline>();
            outline.effectColor = new Color(0.50f, 0.56f, 0.78f, 0.74f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var button = buttonRoot.AddComponent<Button>();
            button.targetGraphic = image;

            CreateFillText(
                "Next Label",
                buttonRoot.transform,
                "Next",
                14,
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
    }
}
