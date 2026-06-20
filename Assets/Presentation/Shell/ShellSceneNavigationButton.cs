using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Shell
{
    [RequireComponent(typeof(Button))]
    public sealed class ShellSceneNavigationButton : MonoBehaviour
    {
        [SerializeField] private string targetSceneName;

        private Button button;

        public void Configure(string sceneName)
        {
            targetSceneName = sceneName;
        }

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.RemoveListener(LoadTargetScene);
            button.onClick.AddListener(LoadTargetScene);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(LoadTargetScene);
            }
        }

        public void LoadTargetScene()
        {
            if (string.IsNullOrEmpty(targetSceneName))
            {
                return;
            }

            SceneManager.LoadScene(targetSceneName);
        }
    }
}
