using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.EntityExtraction
{
    public static class Act2EntityExtractionSampleData
    {
        public const string TimeEntityTypeId = "time";
        public const string RoomEntityTypeId = "room";
        public const string ObjectEntityTypeId = "object";

        public static EntityType CreateTimeEntityType()
        {
            return new EntityType(TimeEntityTypeId, EntityCategory.System);
        }

        public static EntityType CreateRoomEntityType()
        {
            return new EntityType(RoomEntityTypeId, EntityCategory.Custom);
        }

        public static EntityType CreateObjectEntityType()
        {
            return new EntityType(ObjectEntityTypeId, EntityCategory.Custom);
        }

        public static IReadOnlyList<SampleMessage> CreateMessages()
        {
            var timeType = CreateTimeEntityType();
            var roomType = CreateRoomEntityType();
            var objectType = CreateObjectEntityType();

            var labAtNight = "Ghost heard humming in the lab at 9pm.";
            var laboratory = "Ghost heard humming in the laboratory.";
            var lantern = "Ghost tucked the lantern under the desk.";

            return new[]
            {
                new SampleMessage(
                    "lab-at-night",
                    labAtNight,
                    new[]
                    {
                        CreateSpan(labAtNight, "lab", roomType),
                        CreateSpan(labAtNight, "9pm", timeType)
                    }),
                new SampleMessage(
                    "laboratory-synonym",
                    laboratory,
                    new[]
                    {
                        CreateSpan(laboratory, "laboratory", roomType)
                    }),
                new SampleMessage(
                    "lantern-object",
                    lantern,
                    new[]
                    {
                        CreateSpan(lantern, "lantern", objectType)
                    })
            };
        }

        private static EntitySpan CreateSpan(string messageText, string surfaceText, EntityType type)
        {
            var start = messageText.IndexOf(surfaceText, StringComparison.Ordinal);
            if (start < 0)
            {
                throw new InvalidOperationException($"Sample surface text '{surfaceText}' was not found in message '{messageText}'.");
            }

            return new EntitySpan(start, surfaceText.Length, type);
        }

        public sealed class SampleMessage
        {
            private readonly EntitySpan[] correctSpans;

            public SampleMessage(string id, string messageText, IEnumerable<EntitySpan> correctSpans)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("Sample message id cannot be empty.", nameof(id));
                }

                if (correctSpans == null)
                {
                    throw new ArgumentNullException(nameof(correctSpans));
                }

                Id = id;
                MessageText = messageText ?? string.Empty;

                var spans = new List<EntitySpan>();
                foreach (var span in correctSpans)
                {
                    if (span == null)
                    {
                        throw new ArgumentException("Sample message cannot contain a null correct span.", nameof(correctSpans));
                    }

                    spans.Add(span);
                }

                this.correctSpans = spans.ToArray();
            }

            public string Id { get; }

            public string MessageText { get; }

            public IReadOnlyList<EntitySpan> CorrectSpans => correctSpans;
        }
    }
}
