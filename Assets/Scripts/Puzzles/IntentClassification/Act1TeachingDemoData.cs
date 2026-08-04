using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.IntentClassification
{
    public static class Act1TeachingDemoData
    {
        public static IReadOnlyList<IntroFailure> CreateIntroFailures()
        {
            return new[]
            {
                new IntroFailure(
                    "Visitor: Where did the lantern go?",
                    "Ghost: I am Ghost! A very small Ghost!"),
                new IntroFailure(
                    "Visitor: Who are you?",
                    "Ghost: I will search under the sleepy carpet."),
                new IntroFailure(
                    "Visitor: Which room are you haunting right now?",
                    "Ghost: Your notebook sounds shiny.")
            };
        }

        public static IReadOnlyDictionary<string, string> CreateReplyLines()
        {
            return new Dictionary<string, string>(StringComparer.Ordinal)
            {
                {
                    Act1IntentClassificationSampleData.FindItemIntentId,
                    "Ghost: I can help look for the missing thing."
                },
                {
                    Act1IntentClassificationSampleData.AskLocationIntentId,
                    "Ghost: I am drifting by the second-floor noticeboard."
                },
                {
                    Act1IntentClassificationSampleData.AskIdentityIntentId,
                    "Ghost: I am Ghost. I mostly haunt the stairwell."
                }
            };
        }

        public static IReadOnlyList<TestMessage> CreateTestMessages()
        {
            return new[]
            {
                new TestMessage(
                    "find-flask",
                    "Visitor: I cannot find my bike lights anywhere.",
                    Act1IntentClassificationSampleData.FindItemIntentId,
                    new[]
                    {
                        "find-item-lost-key",
                        "find-item-seen-notebook",
                        "find-item-help-look"
                    }),
                new TestMessage(
                    "find-badge",
                    "Visitor: My door card has vanished again.",
                    Act1IntentClassificationSampleData.FindItemIntentId,
                    new[]
                    {
                        "find-item-lost-key",
                        "find-item-seen-notebook",
                        "find-item-help-look"
                    }),
                new TestMessage(
                    "ask-floating-spot",
                    "Visitor: Where did you drift off to?",
                    Act1IntentClassificationSampleData.AskLocationIntentId,
                    new[]
                    {
                        "ask-location-where-floating",
                        "ask-location-which-room",
                        "ask-location-near-door"
                    }),
                new TestMessage(
                    "ask-name",
                    "Visitor: What name should I use for you?",
                    Act1IntentClassificationSampleData.AskIdentityIntentId,
                    new[]
                    {
                        "ask-identity-who",
                        "ask-identity-name",
                        "ask-identity-tell-name"
                    })
            };
        }

        public sealed class IntroFailure
        {
            public IntroFailure(string visitorLine, string ghostWrongReply)
            {
                if (string.IsNullOrWhiteSpace(visitorLine))
                {
                    throw new ArgumentException("Visitor line cannot be empty.", nameof(visitorLine));
                }

                if (string.IsNullOrWhiteSpace(ghostWrongReply))
                {
                    throw new ArgumentException("Ghost wrong reply cannot be empty.", nameof(ghostWrongReply));
                }

                VisitorLine = visitorLine;
                GhostWrongReply = ghostWrongReply;
            }

            public string VisitorLine { get; }

            public string GhostWrongReply { get; }
        }

        public sealed class TestMessage
        {
            private readonly string[] relatedCardIds;

            public TestMessage(
                string id,
                string text,
                string trueIntentId,
                IEnumerable<string> relatedCardIds)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("Test message id cannot be empty.", nameof(id));
                }

                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new ArgumentException("Test message text cannot be empty.", nameof(text));
                }

                if (string.IsNullOrWhiteSpace(trueIntentId))
                {
                    throw new ArgumentException("True intent id cannot be empty.", nameof(trueIntentId));
                }

                if (relatedCardIds == null)
                {
                    throw new ArgumentNullException(nameof(relatedCardIds));
                }

                var ids = new List<string>();
                foreach (var cardId in relatedCardIds)
                {
                    if (string.IsNullOrWhiteSpace(cardId))
                    {
                        throw new ArgumentException("Related card ids cannot contain an empty id.", nameof(relatedCardIds));
                    }

                    ids.Add(cardId);
                }

                if (ids.Count == 0)
                {
                    throw new ArgumentException("Test message requires at least one related card id.", nameof(relatedCardIds));
                }

                Id = id;
                Text = text;
                TrueIntentId = trueIntentId;
                this.relatedCardIds = ids.ToArray();
            }

            public string Id { get; }

            public string Text { get; }

            public string TrueIntentId { get; }

            public IReadOnlyList<string> RelatedCardIds => relatedCardIds;
        }
    }
}
