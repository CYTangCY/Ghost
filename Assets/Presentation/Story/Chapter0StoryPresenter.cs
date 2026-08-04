using Ghost.Presentation.Common;
using System.Collections.Generic;
using Ghost.Presentation.Characters;
using Ghost.Presentation.GhostAvatar;
using Ghost.Presentation.Shell;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ghost.Presentation.Story
{
    public sealed class Chapter0StoryPresenter : MonoBehaviour
    {
        [SerializeField] private bool renderOnStart = true;

        private IReadOnlyList<Chapter0StoryBeat> beats;
        private int beatIndex;
        private bool finished;

        private void Start()
        {
            if (renderOnStart)
            {
                BeginStory();
            }
        }

        public void Configure(bool shouldRenderOnStart)
        {
            renderOnStart = shouldRenderOnStart;
        }

        public void BeginStory()
        {
            EnsureEventSystem();
            beats = Chapter0StoryData.CreateBeats(GhostNarrativeState.PlayerName);
            beatIndex = 0;
            finished = false;
            Render();
        }

        private void Advance()
        {
            if (finished || beats == null || beats.Count == 0)
            {
                return;
            }

            if (beatIndex >= beats.Count - 1)
            {
                FinishStory();
                return;
            }

            beatIndex++;
            Render();
        }

        private void FinishStory()
        {
            if (finished)
            {
                return;
            }

            finished = true;

            GhostNarrativeState.SetPendingDebriefAct(GhostNarrativeState.Chapter0Id);
            SceneManager.LoadScene(ShellSceneNames.GameShellSceneName);
        }

        private void Render()
        {
            ClearChildren(transform);
            ConfigureRoot();
            var beat = beats[beatIndex];

            CreateHeader();
            CreateLabStage(beat);
            CreateDialogue(beat);
            Canvas.ForceUpdateCanvases();
        }

        private void ConfigureRoot()
        {
            var rect = (RectTransform)transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
            image.color = new Color(0.90f, 0.95f, 0.94f);
        }

        private void CreateHeader()
        {
            var header = GhostUITheme.Panel(
                "Story Header",
                transform,
                new Vector2(0f, 0.89f),
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.12f, 0.20f, 0.21f));

            GhostUITheme.Label(
                "Chapter Title",
                header,
                "Chapter 0: The Late Shift",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                GhostUITheme.InkOnDark,
                new Vector2(0.03f, 0f),
                new Vector2(0.70f, 1f),
                Vector2.zero,
                Vector2.zero);

            GhostUITheme.Label(
                "Story Progress",
                header,
                "Opening story  " + (beatIndex + 1) + "/" + beats.Count,
                GhostUITheme.BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleRight,
                GhostUITheme.InkOnDark,
                new Vector2(0.55f, 0f),
                new Vector2(0.72f, 1f),
                Vector2.zero,
                Vector2.zero);

            var skip = GhostUITheme.PushButton(
                "Skip Opening Button",
                header,
                "Skip opening",
                new Vector2(0.735f, 0.22f),
                new Vector2(0.855f, 0.78f),
                new Color(0.91f, 0.94f, 0.95f),
                new Color(0.12f, 0.18f, 0.20f));
            skip.onClick.AddListener(FinishStory);
        }

        private void CreateLabStage(Chapter0StoryBeat beat)
        {
            var stage = GhostUITheme.Panel(
                "Late Lab Stage",
                transform,
                new Vector2(0f, 0.27f),
                new Vector2(1f, 0.89f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.82f, 0.91f, 0.88f));

            GhostUITheme.Panel(
                "Back Wall",
                stage,
                new Vector2(0.02f, 0.05f),
                new Vector2(0.98f, 0.95f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.88f, 0.93f, 0.90f));

            var window = GhostUITheme.Panel(
                "Night Window",
                stage,
                new Vector2(0.40f, 0.42f),
                new Vector2(0.62f, 0.91f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.12f, 0.24f, 0.30f));
            AddOutline(window.gameObject, new Color(0.34f, 0.47f, 0.49f), 3f);

            GhostUITheme.Panel(
                "Window Divider",
                window,
                new Vector2(0.48f, 0f),
                new Vector2(0.52f, 1f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.50f, 0.64f, 0.62f));

            GhostUITheme.Panel(
                "Desk",
                stage,
                new Vector2(0.05f, 0.08f),
                new Vector2(0.95f, 0.24f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.43f, 0.38f, 0.29f));

            GhostUITheme.Panel(
                "Notebook",
                stage,
                new Vector2(0.48f, 0.20f),
                new Vector2(0.58f, 0.27f),
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 0.91f, 0.52f));

            CreateLily(stage, beat.Speaker == "Lily");
            CreateGhost(stage, beat);
        }

        private void CreateLily(Transform stage, bool speaking)
        {
            var frame = GhostUITheme.Panel(
                "Lily Frame",
                stage,
                new Vector2(0.08f, 0.19f),
                new Vector2(0.31f, 0.88f),
                Vector2.zero,
                Vector2.zero,
                speaking
                    ? new Color(1f, 0.86f, 0.52f)
                    : new Color(0.94f, 0.95f, 0.93f));
            AddOutline(
                frame.gameObject,
                speaking
                    ? new Color(0.78f, 0.48f, 0.10f)
                    : new Color(0.55f, 0.61f, 0.62f),
                speaking ? 4f : 2f);

            var portrait = GhostUITheme.Panel(
                "Lily Portrait",
                frame,
                new Vector2(0.08f, 0.14f),
                new Vector2(0.92f, 0.92f),
                Vector2.zero,
                Vector2.zero,
                Color.clear).GetComponent<Image>();
            // Must go through Picture: this Image came from a surface factory, so it is still 9-sliced,
            // and a sliced Image ignores preserveAspect and stretches Lily flat.
            GhostUITheme.Picture(portrait, LilyPixelPortraitFactory.GetFullBody());

            GhostUITheme.Label(
                "Lily Name",
                frame,
                "Lily",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                GhostUITheme.Ink,
                new Vector2(0f, 0f),
                new Vector2(1f, 0.14f),
                Vector2.zero,
                Vector2.zero);
        }

        private void CreateGhost(Transform stage, Chapter0StoryBeat beat)
        {
            var speaking = beat.Speaker == "Ghost";
            var frame = GhostUITheme.Panel(
                "Ghost Frame",
                stage,
                new Vector2(0.69f, 0.19f),
                new Vector2(0.92f, 0.88f),
                Vector2.zero,
                Vector2.zero,
                speaking
                    ? new Color(0.66f, 0.89f, 0.90f)
                    : new Color(0.94f, 0.96f, 0.97f));
            AddOutline(
                frame.gameObject,
                speaking
                    ? new Color(0.10f, 0.55f, 0.58f)
                    : new Color(0.55f, 0.61f, 0.62f),
                speaking ? 4f : 2f);

            var ghostRoot = new GameObject("Ghost", typeof(RectTransform)).GetComponent<RectTransform>();
            ghostRoot.SetParent(frame, false);
            ghostRoot.anchorMin = new Vector2(0.12f, 0.16f);
            ghostRoot.anchorMax = new Vector2(0.88f, 0.92f);
            ghostRoot.offsetMin = Vector2.zero;
            ghostRoot.offsetMax = Vector2.zero;
            ghostRoot.gameObject.AddComponent<GhostFaceView>().SetMood(beat.GhostMood);

            GhostUITheme.Label(
                "Ghost Name",
                frame,
                "Ghost",
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                GhostUITheme.Ink,
                new Vector2(0f, 0f),
                new Vector2(1f, 0.14f),
                Vector2.zero,
                Vector2.zero);
        }

        private void CreateDialogue(Chapter0StoryBeat beat)
        {
            var dialogue = GhostUITheme.Panel(
                "Story Dialogue",
                transform,
                new Vector2(0.035f, 0.035f),
                new Vector2(0.965f, 0.245f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.98f, 0.99f, 0.97f));
            AddOutline(dialogue.gameObject, new Color(0.42f, 0.55f, 0.58f), 3f);

            GhostUITheme.Label(
                "Speaker",
                dialogue,
                beat.Speaker,
                GhostUITheme.TitleSize,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                beat.Speaker == "Ghost"
                    ? new Color(0.08f, 0.47f, 0.51f)
                    : new Color(0.48f, 0.28f, 0.08f),
                new Vector2(0.025f, 0.66f),
                new Vector2(0.72f, 0.94f),
                Vector2.zero,
                Vector2.zero);

            GhostUITheme.Label(
                "Story Line",
                dialogue,
                beat.Text,
                GhostUITheme.TitleSize,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                GhostUITheme.Ink,
                new Vector2(0.025f, 0.12f),
                new Vector2(0.76f, 0.68f),
                Vector2.zero,
                Vector2.zero);

            var label = beatIndex >= beats.Count - 1
                ? "Enter the lab"
                : "Continue";
            var next = GhostUITheme.PushButton(
                "Story Continue Button",
                dialogue,
                label,
                new Vector2(0.79f, 0.24f),
                new Vector2(0.97f, 0.68f),
                new Color(0.76f, 0.90f, 1f),
                new Color(0.10f, 0.18f, 0.23f));
            next.onClick.AddListener(Advance);
        }

        private static void AddOutline(GameObject target, Color color, float distance)
        {
            var outline = target.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(distance, -distance);
        }


        private static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
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
    }
}
