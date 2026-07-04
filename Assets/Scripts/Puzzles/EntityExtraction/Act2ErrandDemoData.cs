using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.EntityExtraction
{
    public static class Act2ErrandDemoData
    {
        public const string LabAtNightErrandId = "errand-lab-at-night";
        public const string LaboratorySynonymErrandId = "errand-laboratory-synonym";
        public const string LanternObjectErrandId = "errand-lantern-object";
        public const string LabRoomCanonicalLabel = "lab room";

        public static IReadOnlyList<ErrandDefinition> CreateErrands()
        {
            var messages = Act2EntityExtractionSampleData.CreateMessages();

            return new[]
            {
                new ErrandDefinition(
                    LabAtNightErrandId,
                    FindMessage(messages, "lab-at-night"),
                    "Ghost rushes to the hallway at noon and hears only the refrigerator hum.",
                    "Ghost floats to the lab at 9pm and catches the tiny humming clue.",
                    new[]
                    {
                        new SlotFailureLine(
                            Act2ErrandSlotId.Where,
                            "Ghost knows when to listen, but drifts through every room until the humming fades.",
                            "Ghost waits in the wrong room at 9pm and waves at a coat rack."),
                        new SlotFailureLine(
                            Act2ErrandSlotId.When,
                            "Ghost finds the lab, but arrives at midnight after the humming has gone sleepy.",
                            "Ghost reaches the lab at the wrong time and hears only tired old pipes.")
                    }),
                new ErrandDefinition(
                    LaboratorySynonymErrandId,
                    FindMessage(messages, "laboratory-synonym"),
                    "Ghost checks a storage closet because the long word feels suspiciously fancy.",
                    "Ghost maps laboratory to the lab room and floats to the right doorway.",
                    new[]
                    {
                        new SlotFailureLine(
                            Act2ErrandSlotId.Where,
                            "Ghost circles the building with no room in mind.",
                            "Ghost picks a room that is not the lab and politely haunts a mop bucket.")
                    }),
                new ErrandDefinition(
                    LanternObjectErrandId,
                    FindMessage(messages, "lantern-object"),
                    "Ghost brings a teacup because small shiny things blur together.",
                    "Ghost tucks the lantern under the desk, exactly where the note needed it.",
                    new[]
                    {
                        new SlotFailureLine(
                            Act2ErrandSlotId.What,
                            "Ghost reaches under the desk with empty little hands.",
                            "Ghost proudly delivers the wrong object and then looks very sorry.")
                    })
            };
        }

        public static IReadOnlyList<SynonymResolution> CreateSynonymResolutions()
        {
            return new[]
            {
                new SynonymResolution(
                    Act2EntityExtractionSampleData.RoomEntityTypeId,
                    LabRoomCanonicalLabel,
                    new[] { "lab", "laboratory" })
            };
        }

        public static IReadOnlyList<ErrandSlot> CreateSlotsForMessage(
            Act2EntityExtractionSampleData.SampleMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var slots = new List<ErrandSlot>();
            AddSlotIfExpected(message, slots, Act2ErrandSlotId.What, Act2EntityExtractionSampleData.ObjectEntityTypeId, "WHAT", "custom: lab object words");
            AddSlotIfExpected(message, slots, Act2ErrandSlotId.Where, Act2EntityExtractionSampleData.RoomEntityTypeId, "WHERE", "custom: lab room words");
            AddSlotIfExpected(message, slots, Act2ErrandSlotId.When, Act2EntityExtractionSampleData.TimeEntityTypeId, "WHEN", "system: built-in time");
            return slots;
        }

        private static void AddSlotIfExpected(
            Act2EntityExtractionSampleData.SampleMessage message,
            ICollection<ErrandSlot> slots,
            Act2ErrandSlotId slotId,
            string entityTypeId,
            string displayName,
            string kindLabel)
        {
            foreach (var span in message.CorrectSpans)
            {
                if (!string.Equals(span.Type.Id, entityTypeId, StringComparison.Ordinal))
                {
                    continue;
                }

                slots.Add(new ErrandSlot(slotId, displayName, kindLabel, span.Type));
                return;
            }
        }

        private static Act2EntityExtractionSampleData.SampleMessage FindMessage(
            IEnumerable<Act2EntityExtractionSampleData.SampleMessage> messages,
            string messageId)
        {
            foreach (var message in messages)
            {
                if (string.Equals(message.Id, messageId, StringComparison.Ordinal))
                {
                    return message;
                }
            }

            throw new InvalidOperationException("Act 2 sample message was not found: " + messageId);
        }

        public sealed class ErrandDefinition
        {
            private readonly ErrandSlot[] slots;
            private readonly SlotFailureLine[] failureLines;

            public ErrandDefinition(
                string errandId,
                Act2EntityExtractionSampleData.SampleMessage message,
                string introFailureOutcomeLine,
                string successOutcomeLine,
                IEnumerable<SlotFailureLine> failureLines)
            {
                if (string.IsNullOrWhiteSpace(errandId))
                {
                    throw new ArgumentException("Errand id cannot be empty.", nameof(errandId));
                }

                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                if (failureLines == null)
                {
                    throw new ArgumentNullException(nameof(failureLines));
                }

                ErrandId = errandId;
                Message = message;
                IntroFailureOutcomeLine = introFailureOutcomeLine ?? string.Empty;
                SuccessOutcomeLine = successOutcomeLine ?? string.Empty;
                slots = CopySlots(CreateSlotsForMessage(message));
                this.failureLines = CopyFailureLines(failureLines);
            }

            public string ErrandId { get; }

            public Act2EntityExtractionSampleData.SampleMessage Message { get; }

            public string IntroFailureOutcomeLine { get; }

            public string SuccessOutcomeLine { get; }

            public IReadOnlyList<ErrandSlot> Slots => slots;

            public IReadOnlyList<SlotFailureLine> FailureLines => failureLines;

            public string GetFailureLine(Act2ErrandSlotId slotId, Act2ErrandSlotState state)
            {
                foreach (var failureLine in failureLines)
                {
                    if (failureLine.SlotId == slotId)
                    {
                        return state == Act2ErrandSlotState.Missing
                            ? failureLine.MissingLine
                            : failureLine.WrongLine;
                    }
                }

                return "Ghost tries the errand, but the action card still has a fuzzy detail.";
            }

            private static ErrandSlot[] CopySlots(IEnumerable<ErrandSlot> source)
            {
                var copied = new List<ErrandSlot>();
                foreach (var slot in source)
                {
                    if (slot == null)
                    {
                        throw new ArgumentException("Errand slots cannot contain null.", nameof(source));
                    }

                    copied.Add(slot);
                }

                if (copied.Count == 0)
                {
                    throw new ArgumentException("Errand must have at least one slot.", nameof(source));
                }

                return copied.ToArray();
            }

            private static SlotFailureLine[] CopyFailureLines(IEnumerable<SlotFailureLine> source)
            {
                var copied = new List<SlotFailureLine>();
                foreach (var line in source)
                {
                    if (line == null)
                    {
                        throw new ArgumentException("Failure lines cannot contain null.", nameof(source));
                    }

                    copied.Add(line);
                }

                return copied.ToArray();
            }
        }

        public sealed class ErrandSlot
        {
            public ErrandSlot(
                Act2ErrandSlotId slotId,
                string displayName,
                string kindLabel,
                EntityType entityType)
            {
                if (string.IsNullOrWhiteSpace(displayName))
                {
                    throw new ArgumentException("Slot display name cannot be empty.", nameof(displayName));
                }

                if (entityType == null)
                {
                    throw new ArgumentNullException(nameof(entityType));
                }

                SlotId = slotId;
                DisplayName = displayName;
                KindLabel = kindLabel ?? string.Empty;
                EntityType = entityType;
            }

            public Act2ErrandSlotId SlotId { get; }

            public string DisplayName { get; }

            public string KindLabel { get; }

            public EntityType EntityType { get; }
        }

        public sealed class SlotFailureLine
        {
            public SlotFailureLine(
                Act2ErrandSlotId slotId,
                string missingLine,
                string wrongLine)
            {
                SlotId = slotId;
                MissingLine = missingLine ?? string.Empty;
                WrongLine = wrongLine ?? string.Empty;
            }

            public Act2ErrandSlotId SlotId { get; }

            public string MissingLine { get; }

            public string WrongLine { get; }
        }

        public sealed class SynonymResolution
        {
            private readonly string[] surfaceTexts;

            public SynonymResolution(
                string entityTypeId,
                string canonicalLabel,
                IEnumerable<string> surfaceTexts)
            {
                if (string.IsNullOrWhiteSpace(entityTypeId))
                {
                    throw new ArgumentException("Entity type id cannot be empty.", nameof(entityTypeId));
                }

                if (surfaceTexts == null)
                {
                    throw new ArgumentNullException(nameof(surfaceTexts));
                }

                var copied = new List<string>();
                foreach (var surfaceText in surfaceTexts)
                {
                    if (string.IsNullOrWhiteSpace(surfaceText))
                    {
                        throw new ArgumentException("Synonym surface text cannot be empty.", nameof(surfaceTexts));
                    }

                    copied.Add(surfaceText);
                }

                EntityTypeId = entityTypeId;
                CanonicalLabel = canonicalLabel ?? string.Empty;
                this.surfaceTexts = copied.ToArray();
            }

            public string EntityTypeId { get; }

            public string CanonicalLabel { get; }

            public IReadOnlyList<string> SurfaceTexts => surfaceTexts;

            public bool Matches(string entityTypeId, string surfaceText)
            {
                if (!string.Equals(EntityTypeId, entityTypeId, StringComparison.Ordinal))
                {
                    return false;
                }

                foreach (var synonym in surfaceTexts)
                {
                    if (string.Equals(synonym, surfaceText, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }

    public enum Act2ErrandSlotId
    {
        What,
        Where,
        When
    }

    public enum Act2ErrandSlotState
    {
        Correct,
        Missing,
        Wrong
    }

    public enum Act2ErrandGhostMood
    {
        Neutral,
        Happy,
        Confused,
        Sad
    }
}
