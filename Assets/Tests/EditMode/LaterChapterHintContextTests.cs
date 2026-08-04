using System.Reflection;
using Ghost.Presentation.Act4ConfidenceFallback;
using Ghost.Presentation.Act5TestingDebugging;
using Ghost.Presentation.Act6BackendResponse;
using Ghost.Presentation.Act6VoicePipeline;
using Ghost.Presentation.Banter;
using Ghost.Presentation.Shell;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Tests.EditMode
{
    public sealed class LaterChapterHintContextTests
    {
        [Test]
        public void LaterChaptersProvideCurrentStateWithoutAnswerKeys()
        {
            var act4Controller = new Act4ConfidenceInteractionController();
            var act4 = act4Controller.BuildHintContext();
            var act5 = new Act5TestingInteractionController().BuildHintContext();
            var act6 = new Act6BackendInteractionController().BuildHintContext();
            var finalChapter = new Act6PipelineInteractionController().BuildHintContext();

            // Chapter 4 stopped being one threshold slider when it was rebuilt - it now has two handles
            // the player positions, and the hint context reports where they currently sit. Read them off
            // the controller rather than hard-coding the opening numbers, so retuning the starting
            // positions cannot fail this test again the way "Threshold=30" did.
            Assert.That(act4, Does.Contain("Lily band below " + act4Controller.HandoffEdge));
            Assert.That(act4, Does.Contain("answer band from " + act4Controller.AnswerEdge));
            Assert.That(act4, Does.Contain("has not been run"));
            Assert.That(act5, Does.Contain("not been run"));
            Assert.That(act6, Does.Contain("Data source=empty"));
            Assert.That(finalChapter, Does.Contain("Main stages: empty"));
            Assert.That(finalChapter, Does.Not.Contain("IntentClassificationId"));
        }

        [Test]
        public void FinalChapterCardChoiceUpdatesFloatingLilyReaction()
        {
            var panelObject = new GameObject("Final Lily Reaction Test");
            var speakerObject = new GameObject("Speaker");
            var dialogueObject = new GameObject("Dialogue");
            speakerObject.transform.SetParent(panelObject.transform, false);
            dialogueObject.transform.SetParent(panelObject.transform, false);
            var speaker = speakerObject.AddComponent<Text>();
            var dialogue = dialogueObject.AddComponent<Text>();
            var panel = panelObject.AddComponent<AmbientBanterPanel>();

            try
            {
                panel.Configure(
                    speaker,
                    dialogue,
                    null,
                    null,
                    null,
                    6f,
                    GhostNarrativeState.FinalChapterId);
                panel.Initialize(BanterData.GetBeats(GhostNarrativeState.FinalChapterId));

                var controller = new Act6PipelineInteractionController();
                controller.BeginAfterOnboarding();
                controller.SelectComponent(
                    Ghost.Puzzles.VoicePipeline.Act6PipelineData.IntentClassificationId);

                Assert.That(speaker.text, Is.EqualTo(ShellDialogueData.LilySpeakerName));
                Assert.That(dialogue.text, Does.Contain("visitor's purpose"));

                var idleField = typeof(AmbientBanterPanel).GetField(
                    "isWaitingForIdleAfterGuidance",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var elapsedField = typeof(AmbientBanterPanel).GetField(
                    "elapsedSeconds",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var idleSecondsField = typeof(AmbientBanterPanel).GetField(
                    "GuidanceIdleSeconds",
                    BindingFlags.NonPublic | BindingFlags.Static);

                Assert.That(idleField, Is.Not.Null);
                Assert.That(idleField.GetValue(panel), Is.True);
                Assert.That(elapsedField.GetValue(panel), Is.EqualTo(0f));
                Assert.That(idleSecondsField.GetRawConstantValue(), Is.EqualTo(25f));
            }
            finally
            {
                Object.DestroyImmediate(panelObject);
            }
        }

        [Test]
        public void LaterScenesUseTheFloatingPortraitBanterPanel()
        {
            var method = typeof(AmbientBanterHook).GetMethod(
                "GetActIdForScene",
                BindingFlags.NonPublic | BindingFlags.Static);

            Assert.That(method, Is.Not.Null);
            Assert.That(method.Invoke(null, new object[] { ShellSceneNames.Act4SceneName }),
                Is.EqualTo(GhostNarrativeState.Act4Id));
            Assert.That(method.Invoke(null, new object[] { ShellSceneNames.Act5SceneName }),
                Is.EqualTo(GhostNarrativeState.Act5Id));
            Assert.That(method.Invoke(null, new object[] { ShellSceneNames.Act6SceneName }),
                Is.EqualTo(GhostNarrativeState.Act6Id));
            Assert.That(method.Invoke(null, new object[] { ShellSceneNames.FinalChapterSceneName }),
                Is.EqualTo(GhostNarrativeState.FinalChapterId));

            Assert.That(BanterData.GetBeats(GhostNarrativeState.Act4Id), Is.Not.Empty);
            Assert.That(BanterData.GetBeats(GhostNarrativeState.Act5Id), Is.Not.Empty);
            Assert.That(BanterData.GetBeats(GhostNarrativeState.Act6Id), Is.Not.Empty);
            Assert.That(BanterData.GetBeats(GhostNarrativeState.FinalChapterId), Is.Not.Empty);
        }
    }
}
