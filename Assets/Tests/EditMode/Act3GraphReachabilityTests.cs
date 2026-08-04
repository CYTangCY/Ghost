using System.Linq;
using Ghost.Puzzles.DialogGraph;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    /// <summary>
    /// Chapter 3 gained a second intent. These tests exist because the palette used to be hand-written
    /// in the presenter while the correct graph lived in the data, so adding a required node could
    /// leave the chapter unsolvable with every rule-level test still green. That is the same failure
    /// that shipped twice in Chapter 2: correct rule, unreachable solution.
    /// </summary>
    public sealed class Act3GraphReachabilityTests
    {
        [Test]
        public void PaletteCoversEveryNodeTheCorrectGraphNeeds()
        {
            var graph = Act3DialogGraphSampleData.CreateCorrectGraph();
            var palette = Act3DialogGraphSampleData.CreatePaletteEntries();

            foreach (var node in graph.Nodes)
            {
                var offered = palette.Any(entry =>
                    entry.Type == node.Type &&
                    entry.IntentId == node.IntentId &&
                    entry.RequiredEntityType == node.RequiredEntityType &&
                    entry.ResponseId == node.ResponseId);

                Assert.That(
                    offered,
                    Is.True,
                    $"The correct graph needs a {node.Type} node ('{node.Id}') that the palette does " +
                    "not offer, so the player could never build it.");
            }
        }

        [Test]
        public void ThePaletteOffersNothingTheGraphCannotUse()
        {
            var graph = Act3DialogGraphSampleData.CreateCorrectGraph();
            var palette = Act3DialogGraphSampleData.CreatePaletteEntries();

            foreach (var entry in palette)
            {
                var used = graph.Nodes.Any(node =>
                    node.Type == entry.Type &&
                    node.IntentId == entry.IntentId &&
                    node.RequiredEntityType == entry.RequiredEntityType &&
                    node.ResponseId == entry.ResponseId);

                Assert.That(used, Is.True, $"Palette card '{entry.Title}' matches no node in the correct graph.");
            }
        }

        [Test]
        public void TheCorrectGraphPassesEveryAuthoredTestCase()
        {
            var cases = Act3DialogGraphSampleData.CreateTestCases();

            var result = DialogGraphValidator.Validate(
                Act3DialogGraphSampleData.CreateCorrectGraph(),
                cases);

            Assert.That(result.IsCorrect, Is.True, string.Join("\n", result.Errors));
            Assert.That(cases.Count, Is.EqualTo(3), "Two intents plus the re-ask branch means three cases.");
        }

        [Test]
        public void DroppingTheSecondIntentFailsValidation()
        {
            var graph = Act3DialogGraphSampleData.CreateCorrectGraph();

            var nodes = graph.Nodes
                .Where(node =>
                    node.Id != Act3DialogGraphSampleData.OpeningHoursBranchNodeId &&
                    node.Id != Act3DialogGraphSampleData.AnswerOpeningHoursNodeId)
                .ToArray();
            var transitions = graph.Transitions
                .Where(transition =>
                    transition.FromNodeId != Act3DialogGraphSampleData.OpeningHoursBranchNodeId &&
                    transition.ToNodeId != Act3DialogGraphSampleData.OpeningHoursBranchNodeId &&
                    transition.ToNodeId != Act3DialogGraphSampleData.AnswerOpeningHoursNodeId)
                .ToArray();

            var result = DialogGraphValidator.Validate(
                new DialogGraph(Act3DialogGraphSampleData.StartNodeId, nodes, transitions),
                Act3DialogGraphSampleData.CreateTestCases());

            Assert.That(
                result.IsCorrect,
                Is.False,
                "The single-intent graph must fail, otherwise the second intent is decorative.");
            Assert.That(result.Errors, Has.Some.Contains(Act3DialogGraphSampleData.OpeningHoursIntentId));
        }

        [Test]
        public void TheHoursBranchAnswersWithoutAskingForARoom()
        {
            var result = DialogGraphSimulator.Simulate(
                Act3DialogGraphSampleData.CreateCorrectGraph(),
                Act3DialogGraphSampleData.CreateOpeningHoursTurn(),
                new DialogContext());

            Assert.That(result.ResponseId, Is.EqualTo(Act3DialogGraphSampleData.AnswerOpeningHoursResponseId));
        }
    }
}
