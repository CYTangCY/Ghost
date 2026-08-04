using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.VoicePipeline
{
    /// <summary>
    /// Decides whether the route the player assembled actually answers this visitor. Deterministic and
    /// self-contained: no language model touches any part of this.
    ///
    /// It walks only the stages the visitor asks for and stops at the first one that breaks, so the
    /// trace ends where the fault is rather than reporting five failures caused by one mistake.
    /// </summary>
    public static class FinalChapterConversationValidator
    {
        public static FinalChapterValidationResult Validate(
            FinalChapterVisitor visitor,
            FinalChapterConfiguration configuration)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var trace = new List<FinalChapterTraceStep>
            {
                new FinalChapterTraceStep("input", visitor.SpeakerName, visitor.Message, true)
            };

            var errors = new List<string>();
            FinalChapterStage? broken = null;
            var reply = visitor.ExpectedReply;

            foreach (var stage in visitor.Stages)
            {
                string line;
                string failureReply;
                var ok = CheckStage(visitor, configuration, stage, out line, out failureReply);

                trace.Add(
                    new FinalChapterTraceStep(
                        stage.ToString(),
                        FinalChapterConversationData.GetStageLabel(stage),
                        line,
                        ok));

                if (!ok)
                {
                    broken = stage;
                    errors.Add(line);
                    reply = string.IsNullOrWhiteSpace(failureReply) ? visitor.FailureReply : failureReply;
                    break;
                }
            }

            trace.Add(new FinalChapterTraceStep("reply", "Ghost's reply", reply, !broken.HasValue));
            return new FinalChapterValidationResult(visitor, reply, broken, trace, errors);
        }

        /// <summary>The configuration a player who understood the visitor would end up with.</summary>
        public static FinalChapterConfiguration CreateReferenceConfiguration(FinalChapterVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            var slots = new Dictionary<string, string>();
            foreach (var slot in visitor.Slots)
            {
                slots.Add(slot.Id, slot.ExpectedFragmentId);
            }

            var response = new Dictionary<string, string>();
            foreach (var pair in visitor.ExpectedResponseParts)
            {
                response.Add(pair.Key, pair.Value);
            }

            // The reference route is the chain a player draws when they get it right: the message, the
            // steps in order, the reply - and no wire anywhere else.
            var route = new List<FinalChapterLink>();
            if (visitor.Requires(FinalChapterStage.Dialogue))
            {
                var previous = FinalChapterConversationData.RouteStartId;
                foreach (var stepId in visitor.ExpectedRouteStepIds)
                {
                    route.Add(new FinalChapterLink(previous, stepId));
                    previous = stepId;
                }

                route.Add(new FinalChapterLink(previous, FinalChapterConversationData.RouteEndId));
            }

            return new FinalChapterConfiguration(
                visitor.ExpectedIntentId,
                slots,
                route,
                visitor.Requires(FinalChapterStage.Confidence)
                    ? visitor.ExpectedAction
                    : FinalChapterAction.None,
                visitor.RequiredBackendIds,
                response);
        }

        private static bool CheckStage(
            FinalChapterVisitor visitor,
            FinalChapterConfiguration configuration,
            FinalChapterStage stage,
            out string line,
            out string failureReply)
        {
            failureReply = string.Empty;

            switch (stage)
            {
                case FinalChapterStage.Intent:
                    return CheckIntent(visitor, configuration, out line, out failureReply);
                case FinalChapterStage.Entities:
                    return CheckEntities(visitor, configuration, out line);
                case FinalChapterStage.Dialogue:
                    return CheckDialogue(visitor, configuration, out line);
                case FinalChapterStage.Confidence:
                    return CheckConfidence(visitor, configuration, out line);
                case FinalChapterStage.Backend:
                    return CheckBackend(visitor, configuration, out line, out failureReply);
                case FinalChapterStage.Response:
                    return CheckResponse(visitor, configuration, out line);
                default:
                    line = string.Empty;
                    return true;
            }
        }

        private static bool CheckIntent(
            FinalChapterVisitor visitor,
            FinalChapterConfiguration configuration,
            out string line,
            out string failureReply)
        {
            failureReply = string.Empty;
            var selected = configuration.IntentOptionId;

            if (string.IsNullOrWhiteSpace(selected))
            {
                line = "No intent chosen, so nothing downstream knows what this visitor wants.";
                return false;
            }

            var option = FinalChapterConversationData.GetOption(selected);
            if (!string.Equals(selected, visitor.ExpectedIntentId, StringComparison.Ordinal))
            {
                failureReply = option.FailureReply;
                line = "\"" + option.Label + "\" is not what this visitor is here for.";
                return false;
            }

            line = "\"" + option.Label + "\" is the purpose behind the wording.";
            return true;
        }

        private static bool CheckEntities(
            FinalChapterVisitor visitor,
            FinalChapterConfiguration configuration,
            out string line)
        {
            foreach (var slot in visitor.Slots)
            {
                configuration.SlotAssignments.TryGetValue(slot.Id, out var assigned);
                assigned = (assigned ?? string.Empty).Trim();

                // A slot the message genuinely cannot fill has to stay empty. Filling it is the
                // mistake, not leaving it - that is the whole point of Lily's request.
                if (string.IsNullOrWhiteSpace(slot.ExpectedFragmentId))
                {
                    if (!string.IsNullOrWhiteSpace(assigned))
                    {
                        var wrong = FinalChapterConversationData.GetFragment(visitor, assigned);
                        line = "\"" + slot.Label + "\" was filled with \"" + wrong.Text +
                            "\". " + wrong.WhyWrong;
                        return false;
                    }

                    continue;
                }

                if (string.IsNullOrWhiteSpace(assigned))
                {
                    line = "\"" + slot.Label + "\" is empty. " + slot.WhyItMatters;
                    return false;
                }

                if (!string.Equals(assigned, slot.ExpectedFragmentId, StringComparison.Ordinal))
                {
                    var wrong = FinalChapterConversationData.GetFragment(visitor, assigned);
                    line = "\"" + slot.Label + "\" holds \"" + wrong.Text + "\". " + wrong.WhyWrong;
                    return false;
                }
            }

            line = "Every detail the reply depends on is held, and nothing invented.";
            return true;
        }

        /// <summary>
        /// Walks the wires the player drew, from the visitor's message to Ghost's reply, and compares
        /// the cards it passes through against the route this visitor needs.
        ///
        /// The walk is what makes a drawn graph checkable without giving up on a single right answer:
        /// a chain start -> a -> b -> end reduces to the ordered list [a, b], so the same authored
        /// <see cref="FinalChapterVisitor.ExpectedRouteStepIds"/> still decides correctness. What the
        /// walk adds is the failures a list could not have: a branch, a loop, a dead end, and cards
        /// wired to each other off to one side of the conversation.
        /// </summary>
        private static bool CheckDialogue(
            FinalChapterVisitor visitor,
            FinalChapterConfiguration configuration,
            out string line)
        {
            var links = configuration.RouteLinks;
            if (links.Count == 0)
            {
                line = "Nothing is wired up, so the conversation has nowhere to go after the intent.";
                return false;
            }

            var ordered = new List<string>();
            var seen = new HashSet<string> { FinalChapterConversationData.RouteStartId };
            var current = FinalChapterConversationData.RouteStartId;
            var followed = 0;

            while (true)
            {
                string next;
                var outgoing = CountOutgoing(links, current, out next);

                if (outgoing > 1)
                {
                    line = "\"" + LabelOf(current) + "\" has more than one wire leaving it. " +
                        "A conversation goes one way at a time.";
                    return false;
                }

                if (outgoing == 0)
                {
                    line = "The route stops at \"" + LabelOf(current) +
                        "\" and never reaches Ghost's reply.";
                    return false;
                }

                followed++;

                if (string.Equals(next, FinalChapterConversationData.RouteEndId, StringComparison.Ordinal))
                {
                    break;
                }

                if (!seen.Add(next))
                {
                    line = "The route loops back to \"" + LabelOf(next) +
                        "\" instead of arriving at the reply.";
                    return false;
                }

                ordered.Add(next);
                current = next;
            }

            var expected = visitor.ExpectedRouteStepIds;
            for (var i = 0; i < expected.Count && i < ordered.Count; i++)
            {
                if (!string.Equals(expected[i], ordered[i], StringComparison.Ordinal))
                {
                    line = "Step " + (i + 1) + " is \"" + LabelOf(ordered[i]) +
                        "\", which sends this visitor down the wrong branch.";
                    return false;
                }
            }

            if (ordered.Count != expected.Count)
            {
                line = ordered.Count < expected.Count
                    ? "The route reaches the reply after " + ordered.Count +
                        " step(s) - this visitor still needs " + (expected.Count - ordered.Count) + " more."
                    : "The route has " + (ordered.Count - expected.Count) +
                        " step(s) this visitor never needed.";
                return false;
            }

            if (followed < links.Count)
            {
                line = "Some cards are wired to each other but sit outside the conversation. " +
                    "Every wire has to be on the path from their message to the reply.";
                return false;
            }

            line = "The route matches what this particular visitor needs, and nothing more.";
            return true;
        }

        private static int CountOutgoing(
            IReadOnlyList<FinalChapterLink> links,
            string fromId,
            out string toId)
        {
            toId = string.Empty;
            var count = 0;

            foreach (var link in links)
            {
                if (!string.Equals(link.FromId, fromId, StringComparison.Ordinal))
                {
                    continue;
                }

                count++;
                if (count == 1)
                {
                    toId = link.ToId;
                }
            }

            return count;
        }

        private static string LabelOf(string routeStepId)
        {
            return FinalChapterConversationData.GetRouteStep(routeStepId).Label;
        }

        private static bool CheckConfidence(
            FinalChapterVisitor visitor,
            FinalChapterConfiguration configuration,
            out string line)
        {
            if (configuration.Action == FinalChapterAction.None)
            {
                line = "No call made. Ghost cannot stall forever with someone waiting.";
                return false;
            }

            if (configuration.Action == visitor.ExpectedAction)
            {
                line = FinalChapterConversationData.GetActionLabel(configuration.Action) +
                    " - defensible, and it costs something: " + CostOf(visitor, configuration.Action);
                return true;
            }

            line = FinalChapterConversationData.GetActionLabel(configuration.Action) + " - " +
                CostOf(visitor, configuration.Action);
            return false;
        }

        private static string CostOf(FinalChapterVisitor visitor, FinalChapterAction action)
        {
            switch (action)
            {
                case FinalChapterAction.AnswerNow:
                    return visitor.AnswerNowCost;
                case FinalChapterAction.AskAgain:
                    return visitor.AskAgainCost;
                case FinalChapterAction.HandOver:
                    return visitor.HandOverCost;
                default:
                    return string.Empty;
            }
        }

        private static bool CheckBackend(
            FinalChapterVisitor visitor,
            FinalChapterConfiguration configuration,
            out string line,
            out string failureReply)
        {
            failureReply = string.Empty;
            var selected = configuration.BackendIds;

            foreach (var id in selected)
            {
                if (!Contains(visitor.RequiredBackendIds, id))
                {
                    var option = FinalChapterConversationData.GetOption(id);
                    failureReply = option.FailureReply;
                    line = "\"" + option.Label + "\" was attached, and this answer never needed it.";
                    return false;
                }
            }

            foreach (var id in visitor.RequiredBackendIds)
            {
                if (!Contains(selected, id))
                {
                    var option = FinalChapterConversationData.GetOption(id);
                    line = "\"" + option.Label + "\" is missing, so the fact has nowhere to come from.";
                    return false;
                }
            }

            line = visitor.RequiredBackendIds.Count == 0
                ? "No stored fact needed - the message already carries everything."
                : "Exactly the one source this answer depends on, and nothing else of hers.";
            return true;
        }

        private static bool CheckResponse(
            FinalChapterVisitor visitor,
            FinalChapterConfiguration configuration,
            out string line)
        {
            foreach (var roleId in FinalChapterConversationData.ResponseRoleIds)
            {
                if (!visitor.ExpectedResponseParts.TryGetValue(roleId, out var expectedPartId))
                {
                    continue;
                }

                configuration.ResponseParts.TryGetValue(roleId, out var assigned);
                assigned = (assigned ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(assigned))
                {
                    line = FinalChapterConversationData.GetRoleLabel(roleId) +
                        " is empty, so the reply cannot be built.";
                    return false;
                }

                if (!string.Equals(assigned, expectedPartId, StringComparison.Ordinal))
                {
                    var wrong = FinalChapterConversationData.GetResponsePart(assigned);
                    line = FinalChapterConversationData.GetRoleLabel(roleId) + " holds \"" +
                        wrong.Label + "\". " + wrong.WhyWrong;
                    return false;
                }
            }

            line = "Source, action and wording each do their own job.";
            return true;
        }

        private static bool Contains(IEnumerable<string> values, string expected)
        {
            foreach (var value in values)
            {
                if (string.Equals(value, expected, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
