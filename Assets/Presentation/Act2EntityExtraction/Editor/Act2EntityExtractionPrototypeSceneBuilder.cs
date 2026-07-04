using Ghost.Presentation.Act2EntityExtraction;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Act2.Editor
{
    public static class Act2EntityExtractionPrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Act2EntityExtractionPrototype.unity";

        [MenuItem("Ghost/Build Act 2 Entity Extraction Prototype Scene")]
        public static void BuildAct2EntityExtractionPrototypeScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            var canvas = CreateCanvas();
            CreateEventSystem();
            CreatePresenterRoot(canvas.transform);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.10f, 0.09f, 0.14f);
            camera.orthographic = true;
            cameraObject.tag = "MainCamera";
        }

        private static Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("Canvas", typeof(RectTransform));
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static void CreateEventSystem()
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static void CreatePresenterRoot(Transform canvasTransform)
        {
            var root = new GameObject("Act 2 Ghost Errand Prototype", typeof(RectTransform)).GetComponent<RectTransform>();
            root.SetParent(canvasTransform, false);
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;

            var presenter = root.gameObject.AddComponent<Act2EntityExtractionStaticPresenter>();
            presenter.Configure(null, null, null, null, null, true);
            presenter.RenderSampleData();
            EditorUtility.SetDirty(presenter);
        }
    }
}
