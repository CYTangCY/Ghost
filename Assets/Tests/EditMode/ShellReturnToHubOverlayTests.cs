using System.IO;
using Ghost.Presentation.Shell;
using Ghost.Presentation.GhostAvatar;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Tests.EditMode
{
    public sealed class ShellReturnToHubOverlayTests
    {
        [Test]
        public void OverlaySourceDoesNotSetPendingDebrief()
        {
            var sourcePath = Path.Combine(
                Application.dataPath,
                "Presentation",
                "Shell",
                "ShellReturnToHubOverlay.cs");

            Assert.That(File.Exists(sourcePath), Is.True, sourcePath);
            var source = File.ReadAllText(sourcePath);
            Assert.That(source, Does.Not.Contain("SetPendingDebriefAct"));
            Assert.That(source, Does.Not.Contain("SetPendingDebriefForActiveScene"));
            Assert.That(source, Does.Contain("TEST PASS"));
            Assert.That(source, Does.Contain("MarkActCompleted"));
            Assert.That(source, Does.Contain("StartEndingForTesting"));
        }

        [TestCase(ShellSceneNames.Chapter0SceneName, GhostNarrativeState.Chapter0Id)]
        [TestCase(ShellSceneNames.Act1SceneName, GhostNarrativeState.Act1Id)]
        [TestCase(ShellSceneNames.Act2SceneName, GhostNarrativeState.Act2Id)]
        [TestCase(ShellSceneNames.Act3SceneName, GhostNarrativeState.Act3Id)]
        [TestCase(ShellSceneNames.Act4SceneName, GhostNarrativeState.Act4Id)]
        [TestCase(ShellSceneNames.Act5SceneName, GhostNarrativeState.Act5Id)]
        [TestCase(ShellSceneNames.Act6SceneName, GhostNarrativeState.Act6Id)]
        [TestCase(ShellSceneNames.FinalChapterSceneName, GhostNarrativeState.FinalChapterId)]
        public void CheatButtonMapsEveryPlayableSceneToItsChapter(
            string sceneName,
            string expectedChapterId)
        {
            Assert.That(
                ShellReturnToHubOverlay.GetChapterIdForScene(sceneName),
                Is.EqualTo(expectedChapterId));
        }

        [Test]
        public void CheatButtonRejectsNonChapterScene()
        {
            Assert.That(
                ShellReturnToHubOverlay.GetChapterIdForScene(
                    ShellSceneNames.GameShellSceneName),
                Is.Empty);
        }

        [TestCase("Characters/LilyPixelFullBody", 96, 128)]
        [TestCase("Characters/LilyPixelPortrait", 96, 96)]
        [TestCase("Characters/GhostPixelNeutral", 96, 96)]
        [TestCase("Characters/GhostPixelHappy", 96, 96)]
        [TestCase("Characters/GhostPixelConfused", 96, 96)]
        [TestCase("Characters/GhostPixelSad", 96, 96)]
        public void PixelCharacterTextureLoadsAtAuthoredResolution(
            string resourcePath,
            int expectedWidth,
            int expectedHeight)
        {
            var texture = Resources.Load<Texture2D>(resourcePath);

            Assert.That(texture, Is.Not.Null, resourcePath);
            Assert.That(texture.width, Is.EqualTo(expectedWidth));
            Assert.That(texture.height, Is.EqualTo(expectedHeight));
        }

        [Test]
        public void GhostFaceUsesPixelSpriteForEveryMood()
        {
            var root = new GameObject("Ghost Pixel Test", typeof(RectTransform));
            try
            {
                var view = root.AddComponent<GhostFaceView>();
                view.SetMood(GhostMood.Neutral);
                var body = root.transform.Find("Ghost Body").GetComponent<Image>();
                var leftEye = body.transform.Find("Left Eye").GetComponent<Image>();

                foreach (GhostMood mood in System.Enum.GetValues(typeof(GhostMood)))
                {
                    view.SetMood(mood);
                    Assert.That(body.sprite, Is.Not.Null, mood.ToString());
                    Assert.That(body.sprite.name, Does.Contain("GhostPixel"), mood.ToString());
                    Assert.That(body.sprite.texture.filterMode, Is.EqualTo(FilterMode.Point));
                    Assert.That(leftEye.enabled, Is.False, mood.ToString());
                }
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}
