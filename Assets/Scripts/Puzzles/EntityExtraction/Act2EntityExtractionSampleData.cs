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

            // Harder cases. Each one targets a specific way entity extraction goes wrong.
            var lanternRoom = "Ghost hummed in the lantern-room at 8am.";
            var carryToLibrary = "Ghost carried the lantern to the library at 6pm.";
            var tuneDecoy = "Ghost hummed the Lonely Corridor tune at 7pm.";

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
                    }),

                // Same word, different entity: "lantern" was an object above; here it names the room.
                new SampleMessage(
                    "lantern-room-context",
                    lanternRoom,
                    new[]
                    {
                        CreateSpan(lanternRoom, "lantern-room", roomType),
                        CreateSpan(lanternRoom, "8am", timeType)
                    }),

                // All three slots at once, with "lantern" back to being an object right after the
                // message above used "lantern-room" as a place.
                new SampleMessage(
                    "carry-to-library",
                    carryToLibrary,
                    new[]
                    {
                        CreateSpan(carryToLibrary, "lantern", objectType),
                        CreateSpan(carryToLibrary, "library", roomType),
                        CreateSpan(carryToLibrary, "6pm", timeType)
                    }),

                // Decoy: "Lonely Corridor" reads like a room but is the name of a tune, so the time is
                // the only entity here.
                new SampleMessage(
                    "tune-decoy",
                    tuneDecoy,
                    new[]
                    {
                        CreateSpan(tuneDecoy, "7pm", timeType)
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
