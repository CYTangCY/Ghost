using System.Collections.Generic;
using System.Linq;
using Ghost.Presentation.Act6VoicePipeline;
using Ghost.Puzzles.VoicePipeline;
using NUnit.Framework;
using UnityEngine;

namespace Ghost.Tests.EditMode
{
    /// <summary>
    /// The capstone's guard rail. Three earlier chapters shipped puzzles whose passing state the
    /// player could not reach while every rule-level test stayed green, so the first fixture here
    /// checks reachability - that the reference solution actually passes - before anything else.
    /// </summary>
    public sealed class FinalChapterConversationTests
    {
        private static IReadOnlyList<FinalChapterVisitor> Visitors =>
            FinalChapterConversationData.CreateVisitors();

        // ------------------------------------------------------------------ reachability

        [Test]
        public void EveryVisitorsReferenceSolutionActuallyPasses()
        {
            foreach (var visitor in Visitors)
            {
                var config = FinalChapterConversationValidator.CreateReferenceConfiguration(visitor);
                var result = FinalChapterConversationValidator.Validate(visitor, config);

                Assert.IsTrue(
                    result.Passed,
                    visitor.Id + " cannot be solved. Broken at " + result.FirstBrokenStage +
                    ": " + string.Join(" | ", result.Errors));
                Assert.AreEqual(visitor.ExpectedReply, result.ActualReply, visitor.Id);
            }
        }

        [Test]
        public void EveryRequiredAnswerIsAmongTheThingsThePlayerCanPick()
        {
            foreach (var visitor in Visitors)
            {
                if (visitor.Requires(FinalChapterStage.Intent))
                {
                    CollectionAssert.Contains(visitor.IntentOptionIds, visitor.ExpectedIntentId, visitor.Id);
                }

                foreach (var slot in visitor.Slots.Where(s => !string.IsNullOrWhiteSpace(s.ExpectedFragmentId)))
                {
                    Assert.IsTrue(
                        visitor.Fragments.Any(f => f.Id == slot.ExpectedFragmentId),
                        visitor.Id + " slot " + slot.Id + " wants a fragment that is never offered.");
                }

                foreach (var stepId in visitor.ExpectedRouteStepIds)
                {
                    CollectionAssert.Contains(visitor.RouteStepPalette, stepId, visitor.Id);
                }

                foreach (var backendId in visitor.RequiredBackendIds)
                {
                    CollectionAssert.Contains(visitor.BackendOptionIds, backendId, visitor.Id);
                }

                foreach (var pair in visitor.ExpectedResponseParts)
                {
                    CollectionAssert.Contains(visitor.ResponsePartIds, pair.Value, visitor.Id);
                    Assert.AreEqual(
                        pair.Key,
                        FinalChapterConversationData.GetResponsePart(pair.Value).RoleId,
                        visitor.Id + " part " + pair.Value + " is filed under the wrong role.");
                }
            }
        }

        [Test]
        public void OneFragmentFillsOneSlot()
        {
            foreach (var visitor in Visitors)
            {
                var wanted = visitor.Slots
                    .Select(s => s.ExpectedFragmentId)
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .ToList();

                Assert.AreEqual(
                    wanted.Count,
                    wanted.Distinct().Count(),
                    visitor.Id + " expects one fragment to fill two slots.");
            }
        }

        [Test]
        public void EveryDecoyFragmentExplainsWhyItIsWrong()
        {
            foreach (var visitor in Visitors)
            {
                Assert.IsTrue(visitor.Fragments.Any(f => f.IsDecoy), visitor.Id + " has no decoy.");

                foreach (var decoy in visitor.Fragments.Where(f => f.IsDecoy))
                {
                    Assert.IsFalse(
                        string.IsNullOrWhiteSpace(decoy.WhyWrong),
                        visitor.Id + " decoy " + decoy.Id + " gives the player no reason.");
                }
            }
        }

        // ------------------------------------------------------------------ distractor quality

        [Test]
        public void TheCorrectIntentIsNotAlwaysInTheSamePlace()
        {
            var positions = Visitors
                .Where(v => v.Requires(FinalChapterStage.Intent))
                .Select(v => v.IntentOptionIds.ToList().IndexOf(v.ExpectedIntentId))
                .ToList();

            CollectionAssert.DoesNotContain(positions, -1);
            Assert.Greater(
                positions.Distinct().Count(),
                1,
                "The correct answer sits at the same index every time, so it can be memorised.");
        }

        [Test]
        public void EveryWrongIntentFailsInItsOwnWay()
        {
            foreach (var visitor in Visitors.Where(v => v.Requires(FinalChapterStage.Intent)))
            {
                var replies = new List<string>();

                foreach (var optionId in visitor.IntentOptionIds.Where(id => id != visitor.ExpectedIntentId))
                {
                    var result = FinalChapterConversationValidator.Validate(
                        visitor,
                        new FinalChapterConfiguration(optionId));

                    Assert.IsFalse(result.Passed, visitor.Id + "/" + optionId + " was accepted.");
                    Assert.AreNotEqual(
                        visitor.FailureReply,
                        result.ActualReply,
                        visitor.Id + "/" + optionId + " falls back to the generic failure - it is filler.");
                    replies.Add(result.ActualReply);
                }

                Assert.AreEqual(
                    replies.Count,
                    replies.Distinct().Count(),
                    visitor.Id + " has two wrong intents that fail identically.");
            }
        }

        [Test]
        public void EveryStageAVisitorAsksForCanActuallyFailThem()
        {
            foreach (var visitor in Visitors)
            {
                foreach (var stage in visitor.Stages)
                {
                    var result = FinalChapterConversationValidator.Validate(
                        visitor,
                        BreakOneStage(visitor, stage));

                    Assert.IsFalse(result.Passed, visitor.Id + " passed with " + stage + " broken.");
                    Assert.AreEqual(stage, result.FirstBrokenStage, visitor.Id + " blamed the wrong stage.");
                }
            }
        }

        // ------------------------------------------------------------------ the chapter lessons

        [Test]
        public void ReadingTheConfidenceNumberGivesTheWrongCall()
        {
            foreach (var visitor in Visitors.Where(v => v.Requires(FinalChapterStage.Confidence)))
            {
                var whatTheNumberSuggests = visitor.ConfidencePercent >= 70
                    ? FinalChapterAction.AnswerNow
                    : FinalChapterAction.AskAgain;

                Assert.AreNotEqual(
                    visitor.ExpectedAction,
                    whatTheNumberSuggests,
                    visitor.Id + ": the score points straight at the right answer, so the player can " +
                    "solve it by arithmetic instead of by reading the request.");
            }
        }

        [Test]
        public void EveryConfidenceOptionCostsSomething()
        {
            foreach (var visitor in Visitors.Where(v => v.Requires(FinalChapterStage.Confidence)))
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(visitor.AnswerNowCost), visitor.Id);
                Assert.IsFalse(string.IsNullOrWhiteSpace(visitor.AskAgainCost), visitor.Id);
                Assert.IsFalse(string.IsNullOrWhiteSpace(visitor.HandOverCost), visitor.Id);
            }
        }

        [Test]
        public void TheSlotHerMessageNeverAnswersTakesTheNotStatedCard()
        {
            var lily = FinalChapterConversationData.GetVisitor(FinalChapterConversationData.LilyVisitorId);
            var openSlot = lily.Slots.Single(
                s => s.ExpectedFragmentId == FinalChapterConversationData.NotStatedFragmentId);

            Assert.IsTrue(
                lily.Fragments.Any(f => f.Id == FinalChapterConversationData.NotStatedFragmentId &&
                    f.IsUnknownMarker && !f.IsDecoy),
                "Lily has no card for the question she never answered, so the only way to be right " +
                "would be to leave a slot blank and hope that reads as an answer.");

            var guessed = lily.Slots.ToDictionary(s => s.Id, s => s.ExpectedFragmentId);
            guessed[openSlot.Id] = "frag_the_thing";
            var guessedResult = FinalChapterConversationValidator.Validate(
                lily,
                Configure(lily, slots: guessed));

            Assert.IsFalse(guessedResult.Passed, "Guessing which job she meant was accepted.");
            Assert.AreEqual(FinalChapterStage.Entities, guessedResult.FirstBrokenStage);

            var blank = lily.Slots.ToDictionary(s => s.Id, s => s.ExpectedFragmentId);
            blank[openSlot.Id] = string.Empty;
            var blankResult = FinalChapterConversationValidator.Validate(lily, Configure(lily, slots: blank));

            Assert.IsFalse(
                blankResult.Passed,
                "An empty slot passed. Saying nothing and saying \"she never said which\" are " +
                "different answers, and only one of them is a finding.");
            Assert.AreEqual(FinalChapterStage.Entities, blankResult.FirstBrokenStage);
        }

        [Test]
        public void ClaimingAVisitorNeverSaidSomethingTheyDidSayIsRejected()
        {
            foreach (var visitor in Visitors)
            {
                var card = visitor.Fragments.SingleOrDefault(f => f.IsUnknownMarker);
                Assert.IsNotNull(
                    card,
                    visitor.Id + " has no not-stated card. Offering it only to Lily would announce " +
                    "which visitor left something open.");

                foreach (var slot in visitor.Slots.Where(
                    s => s.ExpectedFragmentId != FinalChapterConversationData.NotStatedFragmentId))
                {
                    var slots = visitor.Slots.ToDictionary(s => s.Id, s => s.ExpectedFragmentId);
                    slots[slot.Id] = card.Id;

                    var result = FinalChapterConversationValidator.Validate(
                        visitor,
                        Configure(visitor, slots: slots));

                    Assert.IsFalse(
                        result.Passed,
                        visitor.Id + "/" + slot.Id + " accepted \"they never said\" for something " +
                        "the message states plainly.");
                    Assert.AreEqual(FinalChapterStage.Entities, result.FirstBrokenStage, visitor.Id);
                }
            }
        }

        // ------------------------------------------------------------------ the drawn route

        [Test]
        public void BothLaterVisitorsBuildTheirRouteByWiringIt()
        {
            var withDialogue = Visitors.Where(v => v.Requires(FinalChapterStage.Dialogue)).ToList();

            CollectionAssert.AreEqual(
                new[]
                {
                    FinalChapterConversationData.VendingVisitorId,
                    FinalChapterConversationData.LilyVisitorId
                },
                withDialogue.Select(v => v.Id).ToList(),
                "The capstone asks for a dialogue route twice on purpose - once on its own, once " +
                "inside the whole machine.");

            foreach (var visitor in withDialogue)
            {
                Assert.Greater(
                    visitor.RouteStepPalette.Count,
                    visitor.ExpectedRouteStepIds.Count,
                    visitor.Id + " offers no step it does not need, so the route cannot be got wrong.");
            }
        }

        [Test]
        public void ARouteThatBranchesIsRejected()
        {
            foreach (var visitor in Visitors.Where(v => v.Requires(FinalChapterStage.Dialogue)))
            {
                var links = Chain(visitor.ExpectedRouteStepIds);
                links.Add(new FinalChapterLink(
                    FinalChapterConversationData.RouteStartId,
                    visitor.RouteStepPalette.First(id => !visitor.ExpectedRouteStepIds.Contains(id))));

                var result = FinalChapterConversationValidator.Validate(
                    visitor,
                    ConfigureWithRoute(visitor, links));

                Assert.IsFalse(
                    result.Passed,
                    visitor.Id + " accepted a conversation going two ways at once.");
                Assert.AreEqual(FinalChapterStage.Dialogue, result.FirstBrokenStage, visitor.Id);
            }
        }

        [Test]
        public void ARouteThatNeverReachesTheReplyIsRejected()
        {
            foreach (var visitor in Visitors.Where(v => v.Requires(FinalChapterStage.Dialogue)))
            {
                var links = Chain(visitor.ExpectedRouteStepIds);
                links.RemoveAt(links.Count - 1);

                var result = FinalChapterConversationValidator.Validate(
                    visitor,
                    ConfigureWithRoute(visitor, links));

                Assert.IsFalse(
                    result.Passed,
                    visitor.Id + " accepted a route that stops before Ghost ever speaks.");
                Assert.AreEqual(FinalChapterStage.Dialogue, result.FirstBrokenStage, visitor.Id);
            }
        }

        [Test]
        public void WiresDrawnOffToOneSideAreRejected()
        {
            foreach (var visitor in Visitors.Where(v => v.Requires(FinalChapterStage.Dialogue)))
            {
                var spare = visitor.RouteStepPalette
                    .Where(id => !visitor.ExpectedRouteStepIds.Contains(id))
                    .Take(2)
                    .ToList();

                Assert.AreEqual(2, spare.Count, visitor.Id + " has too few spare cards for this check.");

                var links = Chain(visitor.ExpectedRouteStepIds);
                links.Add(new FinalChapterLink(spare[0], spare[1]));

                var result = FinalChapterConversationValidator.Validate(
                    visitor,
                    ConfigureWithRoute(visitor, links));

                Assert.IsFalse(
                    result.Passed,
                    visitor.Id + " accepted an otherwise correct route with two cards wired to each " +
                    "other off to one side.");
                Assert.AreEqual(FinalChapterStage.Dialogue, result.FirstBrokenStage, visitor.Id);
            }
        }

        [Test]
        public void RedrawingAWireFromACardMovesItRatherThanForking()
        {
            var controller = new FinalChapterConversationController();
            controller.FinishOnboarding();
            SolveActiveVisitor(controller);

            var visitor = controller.ActiveVisitor;
            Assert.IsTrue(visitor.Requires(FinalChapterStage.Dialogue), "Visitor 2 lost its dialogue stage.");

            controller.ClearRoute();
            var first = visitor.RouteStepPalette[0];
            var second = visitor.RouteStepPalette[1];
            controller.PlaceRouteStep(first, new Vector2(0.4f, 0.6f));
            controller.PlaceRouteStep(second, new Vector2(0.6f, 0.4f));

            var start = FinalChapterConversationData.RouteStartId;
            controller.LinkRouteSteps(start, first);
            controller.LinkRouteSteps(start, second);

            Assert.AreEqual(
                1,
                controller.RouteLinks.Count(link => link.FromId == start),
                "Dragging a second wire off the same card forked the conversation instead of moving it.");
            Assert.AreEqual(second, controller.GetLinkTarget(start));

            controller.RemoveRouteLink(start);
            Assert.AreEqual(string.Empty, controller.GetLinkTarget(start));
        }

        [Test]
        public void AStepIsOnlyOnTheMapOnceThePlayerPutsItThere()
        {
            var controller = new FinalChapterConversationController();
            controller.FinishOnboarding();
            SolveActiveVisitor(controller);
            controller.ClearRoute();

            var stepId = controller.ActiveVisitor.RouteStepPalette[0];

            Assert.IsFalse(controller.IsStepPlaced(stepId), "A palette step started out already placed.");
            Assert.IsFalse(
                controller.LinkRouteSteps(FinalChapterConversationData.RouteStartId, stepId),
                "A wire was drawn to a card that is not on the map.");

            Assert.IsTrue(controller.PlaceRouteStep(stepId, new Vector2(0.5f, 0.5f)));
            Assert.IsTrue(controller.IsStepPlaced(stepId));
            Assert.IsFalse(
                controller.PlaceRouteStep(stepId, new Vector2(0.3f, 0.3f)),
                "The same step was placed on the map twice.");
        }

        [Test]
        public void BinningACardTakesItsWiresWithIt()
        {
            var controller = new FinalChapterConversationController();
            controller.FinishOnboarding();
            SolveActiveVisitor(controller);

            var firstStep = controller.ActiveVisitor.ExpectedRouteStepIds[0];
            var start = FinalChapterConversationData.RouteStartId;

            controller.PlaceRouteStep(firstStep, new Vector2(0.5f, 0.5f));
            controller.LinkRouteSteps(start, firstStep);
            Assert.AreEqual(firstStep, controller.GetLinkTarget(start));

            Assert.IsTrue(controller.RemoveRouteStep(firstStep));
            Assert.IsFalse(controller.IsStepPlaced(firstStep));
            Assert.AreEqual(
                string.Empty,
                controller.GetLinkTarget(start),
                "A wire was left hanging off a card that is no longer on the map.");
        }

        [Test]
        public void TheTwoEndsOfTheConversationCannotBeBinned()
        {
            var controller = new FinalChapterConversationController();
            controller.FinishOnboarding();

            Assert.IsTrue(controller.IsStepPlaced(FinalChapterConversationData.RouteStartId));
            Assert.IsTrue(controller.IsStepPlaced(FinalChapterConversationData.RouteEndId));
            Assert.IsFalse(controller.RemoveRouteStep(FinalChapterConversationData.RouteStartId));
            Assert.IsFalse(controller.RemoveRouteStep(FinalChapterConversationData.RouteEndId));
        }

        [Test]
        public void AttachingHerPrivateMessagesIsRejected()
        {
            var lily = FinalChapterConversationData.GetVisitor(FinalChapterConversationData.LilyVisitorId);

            var result = FinalChapterConversationValidator.Validate(
                lily,
                Configure(lily, backends: new[] { "backend_job_queue", "backend_direct_messages" }));

            Assert.IsFalse(result.Passed, "Reading her messages was accepted.");
            Assert.AreEqual(FinalChapterStage.Backend, result.FirstBrokenStage);
        }

        [Test]
        public void TheTraceStopsAtTheFaultRatherThanAfterIt()
        {
            foreach (var visitor in Visitors)
            {
                var result = FinalChapterConversationValidator.Validate(
                    visitor,
                    BreakOneStage(visitor, visitor.Stages[0]));

                Assert.AreEqual(1, result.Errors.Count, visitor.Id + " reported cascading errors.");
                Assert.AreEqual(
                    2,
                    result.TraceSteps.Count(step => !step.Succeeded),
                    visitor.Id + " marked more than the broken stage and the reply as failed.");
            }
        }

        [Test]
        public void TheThreeVisitorsEscalateRatherThanRepeat()
        {
            var visitors = Visitors;

            for (var i = 1; i < visitors.Count; i++)
            {
                Assert.Greater(
                    visitors[i].Stages.Count,
                    visitors[i - 1].Stages.Count,
                    visitors[i].Id + " asks no more of the player than " + visitors[i - 1].Id + " did.");
            }

            Assert.AreEqual(FinalChapterConversationData.LilyVisitorId, visitors[visitors.Count - 1].Id);
            Assert.IsTrue(visitors[visitors.Count - 1].UsesLilyPortrait);
        }

        // ------------------------------------------------------------------ controller

        [Test]
        public void TheTestCannotBeRunUntilEveryStageHasBeenVisited()
        {
            var controller = new FinalChapterConversationController();
            controller.FinishOnboarding();

            Assert.IsFalse(controller.CanRunTest, "Runnable before the player had seen every stage.");

            for (var i = 0; i < controller.ActiveVisitor.Stages.Count; i++)
            {
                controller.GoToStage(i);
            }

            Assert.IsTrue(controller.CanRunTest);
        }

        [Test]
        public void AFragmentMovesBetweenSlotsRatherThanBeingCloned()
        {
            var controller = new FinalChapterConversationController();
            controller.FinishOnboarding();
            var visitor = controller.ActiveVisitor;

            controller.AssignFragment("frag_parcel", visitor.Slots[0].Id);
            controller.AssignFragment("frag_parcel", visitor.Slots[1].Id);

            Assert.AreEqual(string.Empty, controller.GetSlotAssignment(visitor.Slots[0].Id));
            Assert.AreEqual("frag_parcel", controller.GetSlotAssignment(visitor.Slots[1].Id));
        }

        [Test]
        public void ClearingASlotPutsTheFragmentBackInThePalette()
        {
            var controller = new FinalChapterConversationController();
            controller.FinishOnboarding();
            var slotId = controller.ActiveVisitor.Slots[0].Id;

            controller.AssignFragment("frag_parcel", slotId);
            Assert.IsTrue(controller.IsFragmentUsed("frag_parcel"));

            controller.ClearSlot(slotId);
            Assert.IsFalse(controller.IsFragmentUsed("frag_parcel"));
        }

        [Test]
        public void AFailedRunReturnsThePlayerToTheStageThatBroke()
        {
            var controller = new FinalChapterConversationController();
            controller.FinishOnboarding();
            var visitor = controller.ActiveVisitor;

            for (var i = 0; i < visitor.Stages.Count; i++)
            {
                controller.GoToStage(i);
            }

            // Right intent, wrong detail. The run has to stop on Details, not on Intent.
            controller.SelectIntent(visitor.ExpectedIntentId);
            controller.AssignFragment("frag_parcel", visitor.Slots[0].Id);
            controller.AssignFragment("frag_reception", visitor.Slots[1].Id);
            controller.RunTest();

            Assert.AreEqual(FinalChapterPhase.Playback, controller.CurrentPhase);
            Assert.IsFalse(controller.LastResult.Passed);

            StepToEndOfPlayback(controller);

            Assert.AreEqual(FinalChapterPhase.Configure, controller.CurrentPhase);
            Assert.AreEqual(FinalChapterStage.Entities, controller.ActiveStage);
        }

        [Test]
        public void SolvingAllThreeVisitorsReachesTheEnding()
        {
            var controller = new FinalChapterConversationController();
            controller.FinishOnboarding();

            for (var visitorIndex = 0; visitorIndex < 3; visitorIndex++)
            {
                Assert.AreEqual(
                    FinalChapterPhase.Configure,
                    controller.CurrentPhase,
                    "Stalled before visitor " + visitorIndex);
                SolveActiveVisitor(controller);
            }

            Assert.AreEqual(FinalChapterPhase.ReadyForEnding, controller.CurrentPhase);
            Assert.AreEqual(3, controller.CompletedVisitorCount);

            controller.BeginEnding();
            Assert.AreEqual(FinalChapterPhase.Ending, controller.CurrentPhase);
        }

        [Test]
        public void RetryingClearsOnlyTheVisitorOnScreen()
        {
            var controller = new FinalChapterConversationController();
            controller.FinishOnboarding();
            SolveActiveVisitor(controller);

            Assert.IsTrue(controller.IsVisitorCompleted(0));
            Assert.AreEqual(1, controller.ActiveVisitorIndex);

            controller.SelectIntent(controller.ActiveVisitor.ExpectedIntentId);
            controller.ResetCurrentVisitor();

            Assert.AreEqual(string.Empty, controller.SelectedIntentId);
            Assert.IsTrue(controller.IsVisitorCompleted(0), "An earlier visitor was un-completed.");
            Assert.AreEqual(1, controller.ActiveVisitorIndex);
        }

        // ------------------------------------------------------------------ it actually renders

        [Test]
        public void ThePresenterRendersEveryStageOfEveryVisitorWithoutThrowing()
        {
            var root = new GameObject("Final Chapter Test Root", typeof(RectTransform));
            try
            {
                var presenter = root.AddComponent<FinalChapterConversationPresenter>();
                presenter.Configure(false);
                presenter.RenderSampleData();

                var controller = presenter.Controller;
                Assert.IsNotNull(controller);
                controller.FinishOnboarding();

                for (var visitorIndex = 0; visitorIndex < 3; visitorIndex++)
                {
                    for (var stage = 0; stage < controller.ActiveVisitor.Stages.Count; stage++)
                    {
                        controller.GoToStage(stage);
                        Assert.Greater(
                            root.transform.childCount,
                            0,
                            "Nothing rendered for " + controller.ActiveVisitor.Id +
                            " at " + controller.ActiveStage);
                    }

                    SolveActiveVisitor(controller);
                }

                Assert.AreEqual(FinalChapterPhase.ReadyForEnding, controller.CurrentPhase);
                Assert.Greater(root.transform.childCount, 0);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        // ------------------------------------------------------------------ helpers

        private static FinalChapterConfiguration Configure(
            FinalChapterVisitor visitor,
            IDictionary<string, string> slots = null,
            IEnumerable<string> backends = null)
        {
            return new FinalChapterConfiguration(
                visitor.ExpectedIntentId,
                slots ?? visitor.Slots.ToDictionary(s => s.Id, s => s.ExpectedFragmentId),
                Chain(visitor.ExpectedRouteStepIds),
                visitor.Requires(FinalChapterStage.Confidence)
                    ? visitor.ExpectedAction
                    : FinalChapterAction.None,
                backends ?? visitor.RequiredBackendIds,
                visitor.ExpectedResponseParts.ToDictionary(p => p.Key, p => p.Value));
        }

        private static FinalChapterConfiguration ConfigureWithRoute(
            FinalChapterVisitor visitor,
            IEnumerable<FinalChapterLink> route)
        {
            return new FinalChapterConfiguration(
                visitor.ExpectedIntentId,
                visitor.Slots.ToDictionary(s => s.Id, s => s.ExpectedFragmentId),
                route,
                visitor.Requires(FinalChapterStage.Confidence)
                    ? visitor.ExpectedAction
                    : FinalChapterAction.None,
                visitor.RequiredBackendIds,
                visitor.ExpectedResponseParts.ToDictionary(p => p.Key, p => p.Value));
        }

        /// <summary>
        /// The wires a player draws when they get the route right: the message, the steps in order, the
        /// reply. Written out here rather than borrowed from the validator so the tests would still
        /// notice if the validator started agreeing with itself.
        /// </summary>
        private static List<FinalChapterLink> Chain(IEnumerable<string> stepIds)
        {
            var links = new List<FinalChapterLink>();
            var previous = FinalChapterConversationData.RouteStartId;

            foreach (var stepId in stepIds)
            {
                links.Add(new FinalChapterLink(previous, stepId));
                previous = stepId;
            }

            links.Add(new FinalChapterLink(previous, FinalChapterConversationData.RouteEndId));
            return links;
        }

        private static void StepToEndOfPlayback(FinalChapterConversationController controller)
        {
            for (var i = 0; i < 16 && controller.CurrentPhase == FinalChapterPhase.Playback; i++)
            {
                controller.AdvancePlayback();
            }
        }

        private static void SolveActiveVisitor(FinalChapterConversationController controller)
        {
            var visitor = controller.ActiveVisitor;

            for (var i = 0; i < visitor.Stages.Count; i++)
            {
                controller.GoToStage(i);
            }

            controller.SelectIntent(visitor.ExpectedIntentId);

            foreach (var slot in visitor.Slots.Where(s => !string.IsNullOrWhiteSpace(s.ExpectedFragmentId)))
            {
                controller.AssignFragment(slot.ExpectedFragmentId, slot.Id);
            }

            controller.ClearRoute();
            if (visitor.Requires(FinalChapterStage.Dialogue))
            {
                var previous = FinalChapterConversationData.RouteStartId;
                foreach (var stepId in visitor.ExpectedRouteStepIds)
                {
                    // Nothing can be wired until it is on the map - the same order the player works in.
                    controller.PlaceRouteStep(stepId, new Vector2(0.5f, 0.5f));
                    controller.LinkRouteSteps(previous, stepId);
                    previous = stepId;
                }

                controller.LinkRouteSteps(previous, FinalChapterConversationData.RouteEndId);
            }

            if (visitor.Requires(FinalChapterStage.Confidence))
            {
                controller.ChooseAction(visitor.ExpectedAction);
            }

            foreach (var backendId in visitor.RequiredBackendIds)
            {
                controller.ToggleBackend(backendId);
            }

            foreach (var pair in visitor.ExpectedResponseParts)
            {
                controller.PlaceResponsePart(pair.Value, pair.Key);
            }

            controller.RunTest();
            Assert.IsTrue(
                controller.LastResult.Passed,
                visitor.Id + " could not be solved through the controller: " +
                string.Join(" | ", controller.LastResult.Errors));

            StepToEndOfPlayback(controller);
        }

        /// <summary>Reference configuration with exactly one stage sabotaged.</summary>
        private static FinalChapterConfiguration BreakOneStage(
            FinalChapterVisitor visitor,
            FinalChapterStage stage)
        {
            var slots = visitor.Slots.ToDictionary(s => s.Id, s => s.ExpectedFragmentId);
            var route = Chain(visitor.ExpectedRouteStepIds);
            var action = visitor.Requires(FinalChapterStage.Confidence)
                ? visitor.ExpectedAction
                : FinalChapterAction.None;
            var backends = visitor.RequiredBackendIds.ToList();
            var response = visitor.ExpectedResponseParts.ToDictionary(p => p.Key, p => p.Value);
            var intent = visitor.ExpectedIntentId;

            switch (stage)
            {
                case FinalChapterStage.Intent:
                    intent = visitor.IntentOptionIds.First(id => id != visitor.ExpectedIntentId);
                    break;
                case FinalChapterStage.Entities:
                    var target = visitor.Slots.First();
                    slots[target.Id] = visitor.Fragments.First(f => f.Id != target.ExpectedFragmentId).Id;
                    break;
                case FinalChapterStage.Dialogue:
                    // A complete route through the wrong card, not a half-drawn one - the failure has
                    // to be about where the conversation went, not about the player not being finished.
                    route = Chain(
                        new[] { visitor.RouteStepPalette.First(id => id != visitor.ExpectedRouteStepIds[0]) });
                    break;
                case FinalChapterStage.Confidence:
                    action = visitor.ExpectedAction == FinalChapterAction.HandOver
                        ? FinalChapterAction.AnswerNow
                        : FinalChapterAction.HandOver;
                    break;
                case FinalChapterStage.Backend:
                    backends = visitor.BackendOptionIds
                        .Where(id => !visitor.RequiredBackendIds.Contains(id))
                        .Take(1)
                        .ToList();
                    break;
                case FinalChapterStage.Response:
                    var role = visitor.ExpectedResponseParts.First();
                    response[role.Key] = visitor.ResponsePartIds.First(
                        id => id != role.Value &&
                            FinalChapterConversationData.GetResponsePart(id).RoleId == role.Key);
                    break;
            }

            return new FinalChapterConfiguration(intent, slots, route, action, backends, response);
        }
    }
}
