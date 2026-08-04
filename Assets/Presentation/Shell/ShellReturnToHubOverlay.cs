using Ghost.Presentation.Common;
using Ghost.Presentation.Act6VoicePipeline;
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
        private const string CheatButtonName = "Shell Test Pass Button";
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

            EnsureEventSystem();
            var canvas = CreateOverlayCanvas();
            if (GameObject.Find(OverlayButtonName) == null)
            {
                CreateReturnButton(canvas.transform);
            }

            if (GameObject.Find(CheatButtonName) == null)
            {
                CreateCheatButton(canvas.transform, scene.name);
            }
        }

        private static bool ShouldShowOverlay(string sceneName)
        {
            return sceneName == ShellSceneNames.Chapter0SceneName ||
                sceneName == ShellSceneNames.Act1SceneName ||
                sceneName == ShellSceneNames.Act2SceneName ||
                sceneName == ShellSceneNames.Act3SceneName ||
                sceneName == ShellSceneNames.Act4SceneName ||
                sceneName == ShellSceneNames.Act5SceneName ||
                sceneName == ShellSceneNames.Act6SceneName ||
                sceneName == ShellSceneNames.FinalChapterSceneName;
        }

        public static string GetChapterIdForScene(string sceneName)
        {
            if (sceneName == ShellSceneNames.Chapter0SceneName)
            {
                return GhostNarrativeState.Chapter0Id;
            }
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
            if (sceneName == ShellSceneNames.Act4SceneName)
            {
                return GhostNarrativeState.Act4Id;
            }
            if (sceneName == ShellSceneNames.Act5SceneName)
            {
                return GhostNarrativeState.Act5Id;
            }
            if (sceneName == ShellSceneNames.Act6SceneName)
            {
                return GhostNarrativeState.Act6Id;
            }
            return sceneName == ShellSceneNames.FinalChapterSceneName
                ? GhostNarrativeState.FinalChapterId
                : string.Empty;
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

            var button = GhostUITheme.PushButton(
                buttonRoot,
                "Return to Hub",
                new Color(0.93f, 0.95f, 1f, 0.96f),
                GhostUITheme.Ink);
            GhostUITheme.Label(
                button.GetComponentInChildren<Text>(),
                "Return to Hub",
                GhostUITheme.HeadingSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                GhostUITheme.Ink);

            var navigation = buttonRoot.AddComponent<ShellSceneNavigationButton>();
            navigation.Configure(ShellSceneNames.GameShellSceneName);
        }
        private static void CreateCheatButton(Transform parent, string sceneName)
        {
            var buttonRoot = new GameObject(CheatButtonName, typeof(RectTransform));
            buttonRoot.transform.SetParent(parent, false);

            // Top right, tucked under Return to Hub and over the right end of the objective strip.
            // It used to sit in the bottom-left corner, where every chapter puts its palette column -
            // this overlay canvas draws above everything, so it was swallowing clicks meant for the
            // palette. The strip is a label, so covering its empty end costs the player nothing.
            var rect = buttonRoot.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.one;
            rect.anchorMax = Vector2.one;
            rect.pivot = Vector2.one;
            rect.anchoredPosition = new Vector2(-28f, -80f);
            rect.sizeDelta = new Vector2(104f, 26f);

            var button = GhostUITheme.PushButton(
                buttonRoot,
                "TEST PASS",
                new Color(0.22f, 0.20f, 0.28f, 0.88f),
                new Color(1f, 0.86f, 0.55f));
            GhostUITheme.Label(
                button.GetComponentInChildren<Text>(),
                "TEST PASS",
                GhostUITheme.TinySize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(1f, 0.86f, 0.55f));
            button.onClick.AddListener(
                () => CompleteChapterAndReturn(sceneName));
        }
        private static void CompleteChapterAndReturn(string sceneName)
        {
            var chapterId = GetChapterIdForScene(sceneName);
            if (string.IsNullOrWhiteSpace(chapterId))
            {
                return;
            }

            if (chapterId == GhostNarrativeState.FinalChapterId)
            {
                var presenter =
                    Object.FindFirstObjectByType<FinalChapterConversationPresenter>();
                if (presenter == null)
                {
                    Debug.LogWarning("TEST PASS could not find the Final Chapter presenter.");
                    return;
                }

                var overlayCanvas = GameObject.Find(OverlayCanvasName);
                if (overlayCanvas != null)
                {
                    overlayCanvas.SetActive(false);
                }
                presenter.StartEndingForTesting();
                return;
            }

            GhostNarrativeState.MarkActCompleted(chapterId);
            GhostNarrativeState.RequestResumeAtHub();
            SceneManager.LoadScene(ShellSceneNames.GameShellSceneName);
        }

    }
}
