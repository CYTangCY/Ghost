using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Shell
{
    public static class ShellReturnToHubOverlay
    {
        private const string OverlayCanvasName = "Shell Return To Hub Overlay Canvas";
        private const string OverlayButtonName = "Shell Return To Hub Overlay";
        private const int OverlaySortingOrder = 32767;

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
            if (!ShouldShowOverlay(scene.name))
            {
                return;
            }

            if (GameObject.Find(OverlayButtonName) != null)
            {
                return;
            }

            EnsureEventSystem();
            var canvas = CreateOverlayCanvas();
            CreateReturnButton(canvas.transform);
        }

        private static bool ShouldShowOverlay(string sceneName)
        {
            return sceneName == ShellSceneNames.Act1SceneName ||
                sceneName == ShellSceneNames.Act2SceneName ||
                sceneName == ShellSceneNames.Act3SceneName;
        }

        private static Canvas CreateOverlayCanvas()
        {
            var existingCanvasObject = GameObject.Find(OverlayCanvasName);
            if (existingCanvasObject != null)
            {
                var existingCanvas = existingCanvasObject.GetComponent<Canvas>();
                if (existingCanvas != null)
                {
                    return existingCanvas;
                }
            }

            var canvasObject = new GameObject(OverlayCanvasName, typeof(RectTransform));
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = OverlaySortingOrder;

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

        private static void CreateReturnButton(Transform parent)
        {
            var buttonRoot = new GameObject(OverlayButtonName, typeof(RectTransform));
            buttonRoot.transform.SetParent(parent, false);

            var rect = buttonRoot.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-28f, -24f);
            rect.sizeDelta = new Vector2(210f, 48f);

            var image = buttonRoot.AddComponent<Image>();
            image.color = new Color(0.93f, 0.95f, 1f, 0.96f);
            image.raycastTarget = true;

            var outline = buttonRoot.AddComponent<Outline>();
            outline.effectColor = new Color(0.52f, 0.57f, 0.78f, 0.85f);
            outline.effectDistance = new Vector2(2f, -2f);

            var button = buttonRoot.AddComponent<Button>();
            button.onClick.AddListener(SetPendingDebriefForActiveScene);

            var navigation = buttonRoot.AddComponent<ShellSceneNavigationButton>();
            navigation.Configure(ShellSceneNames.GameShellSceneName);

            CreateLabel(buttonRoot.transform);
        }

        private static void SetPendingDebriefForActiveScene()
        {
            var actId = GetActIdForScene(SceneManager.GetActiveScene().name);
            if (!string.IsNullOrWhiteSpace(actId))
            {
                GhostNarrativeState.SetPendingDebriefAct(actId);
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

        private static void CreateLabel(Transform parent)
        {
            var labelObject = new GameObject("Return Label", typeof(RectTransform));
            labelObject.transform.SetParent(parent, false);

            var rect = labelObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var text = labelObject.AddComponent<Text>();
            text.text = "Return to Hub";
            text.font = GetBuiltinFont();
            text.fontSize = 18;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.12f, 0.16f, 0.28f);
            text.raycastTarget = false;
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
