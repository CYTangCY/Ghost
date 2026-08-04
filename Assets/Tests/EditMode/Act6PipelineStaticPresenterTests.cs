using System.Reflection;
using Ghost.Presentation.Act6VoicePipeline;
using Ghost.Puzzles.VoicePipeline;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ghost.Tests.EditMode
{
    public sealed class Act6PipelineStaticPresenterTests
    {
        [Test]
        public void ConfigureBoardRendersFixedEndpointsShortcutsAndThreeTests()
        {
            var existingEventSystem = Object.FindAnyObjectByType<EventSystem>();
            var canvasObject = new GameObject("Final Chapter Test Canvas", typeof(RectTransform), typeof(Canvas));
            var root = new GameObject("Final Chapter Test Root", typeof(RectTransform));
            root.transform.SetParent(canvasObject.transform, false);
            var presenter = root.AddComponent<Act6PipelineStaticPresenter>();

            try
            {
                presenter.RenderSampleData();
                var controllerField = typeof(Act6PipelineStaticPresenter).GetField(
                    "controller",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                Assert.That(controllerField, Is.Not.Null);
                var controller = controllerField.GetValue(presenter) as Act6PipelineInteractionController;
                Assert.That(controller, Is.Not.Null);

                controller.BeginAfterOnboarding();

                var palette = FindChild(root.transform, "Palette Grid");
                var mainPath = FindChild(root.transform, "Main Pipeline Slots");
                var tests = FindChild(root.transform, "Visitor Test Results");

                Assert.That(palette, Is.Not.Null);
                Assert.That(palette.childCount, Is.EqualTo(3));
                Assert.That(
                    FindChild(root.transform, "Palette Categories").childCount,
                    Is.EqualTo(6));
                Assert.That(FindChild(mainPath, "Visitor message Endpoint"), Is.Not.Null);
                Assert.That(FindChild(mainPath, "Ghost reply Endpoint"), Is.Not.Null);
                Assert.That(tests, Is.Not.Null);
                Assert.That(tests.childCount, Is.EqualTo(3));
                Assert.That(FindChild(root.transform, "Guided Repair Panel"), Is.Null);
                Assert.That(FindChild(root.transform, "Ask Lily Button"), Is.Null);

                FindChild(root.transform, "Entities Button")
                    .GetComponent<Button>().onClick.Invoke();
                palette = FindChild(root.transform, "Palette Grid");
                Assert.That(palette.childCount, Is.EqualTo(3));
                Assert.That(
                    FindChild(
                        palette,
                        "Palette Part - " + Act6PipelineData.NounsOnlyId),
                    Is.Not.Null);

                FindChild(root.transform, "Intent Button")
                    .GetComponent<Button>().onClick.Invoke();
                palette = FindChild(root.transform, "Palette Grid");
                mainPath = FindChild(root.transform, "Main Pipeline Slots");
                tests = FindChild(root.transform, "Visitor Test Results");
                Assert.That(palette.childCount, Is.EqualTo(3));

                var startButton = FindChild(
                    root.transform,
                    "Start visitor 1 Button").GetComponent<Button>();
                Assert.That(startButton.interactable, Is.False);

                var intentCard = FindChild(
                    palette,
                    "Palette Part - " + Act6PipelineData.IntentClassificationId);
                var intentLabel = FindChild(intentCard, "Part Label");
                var firstSlotDrop = FindChild(
                    mainPath,
                    "Main Slot 1").GetComponent<Act6PipelineSlotDropView>();
                var pointerData = new PointerEventData(
                    Object.FindAnyObjectByType<EventSystem>())
                {
                    pointerDrag = intentLabel.gameObject
                };

                firstSlotDrop.OnDrop(pointerData);

                Assert.That(
                    controller.GetMainSlotComponentId(0),
                    Is.EqualTo(Act6PipelineData.IntentClassificationId));

                controller.ResetPipeline();

                var order = Act6PipelineData.CreateMainPipelineOrder();
                for (var index = 0; index < order.Count; index++)
                {
                    controller.PlaceInMainSlot(order[index], index);
                }

                controller.PlaceInBackendSlot(Act6PipelineData.BackendActionId);
                controller.RunPipeline();

                Assert.That(
                    FindChild(root.transform, "Visitor Message").GetComponent<Text>().text,
                    Does.Contain("Could you do the thing from before?"));
                Assert.That(
                    FindChild(
                        FindChild(root.transform, "Visitor Test 1"),
                        "Test Status").GetComponent<Text>().text,
                    Does.Contain("RUNNING"));
                Assert.That(
                    FindChild(
                        FindChild(root.transform, "Visitor Test 2"),
                        "Test Status").GetComponent<Text>().text,
                    Does.Contain("WAITING"));
                Assert.That(
                    FindChild(
                        FindChild(root.transform, "Main Slot 1"),
                        "Prior Work").GetComponent<Text>().text,
                    Is.EqualTo("Waiting for this visitor."));
                Assert.That(
                    FindChild(root.transform, "Edit route Button"),
                    Is.Not.Null);

                controller.AdvanceVisitorTest();

                Assert.That(
                    FindChild(
                        FindChild(root.transform, "Main Slot 1"),
                        "Prior Work").GetComponent<Text>().text,
                    Does.Contain("Intent=unclear_request"));

                while (!controller.CurrentVisitorReplyShown)
                {
                    controller.AdvanceVisitorTest();
                }

                Assert.That(
                    FindChild(
                        FindChild(root.transform, "Visitor Test 1"),
                        "Test Status").GetComponent<Text>().text,
                    Does.Contain("PASS"));
                Assert.That(
                    FindChild(root.transform, "Visitor Message").GetComponent<Text>().text,
                    Does.Contain("Could you do the thing from before?"));

                controller.AdvanceVisitorTest();

                Assert.That(
                    FindChild(root.transform, "Visitor Message").GetComponent<Text>().text,
                    Does.Contain("Can you help me find my brass key?"));
                Assert.That(
                    FindChild(
                        FindChild(root.transform, "Visitor Test 2"),
                        "Test Status").GetComponent<Text>().text,
                    Does.Contain("RUNNING"));
                Assert.That(
                    FindChild(root.transform, "Conversation Title").GetComponent<Text>().text,
                    Is.EqualTo("Visitor message enters"));
                Assert.That(
                    FindChild(
                        FindChild(root.transform, "Main Slot 1"),
                        "Prior Work").GetComponent<Text>().text,
                    Is.EqualTo("Waiting for this visitor."));

                controller.AdvanceVisitorTest();

                Assert.That(
                    FindChild(
                        FindChild(root.transform, "Main Slot 1"),
                        "Prior Work").GetComponent<Text>().text,
                    Does.Contain("Intent=find_item"));
            }
            finally
            {
                Object.DestroyImmediate(canvasObject);
                if (existingEventSystem == null)
                {
                    var createdEventSystem = Object.FindAnyObjectByType<EventSystem>();
                    if (createdEventSystem != null)
                    {
                        Object.DestroyImmediate(createdEventSystem.gameObject);
                    }
                }
            }
        }

        private static Transform FindChild(Transform root, string childName)
        {
            if (root == null)
            {
                return null;
            }

            foreach (var child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == childName)
                {
                    return child;
                }
            }

            return null;
        }
    }
}