using System.Collections.Generic;

namespace Ghost.Puzzles.IntentClassification
{
    public static class Act1IntentClassificationSampleData
    {
        public const string FindItemIntentId = "find_item";
        public const string AskLocationIntentId = "ask_location";
        public const string AskIdentityIntentId = "ask_identity";

        public static IReadOnlyList<IntentCard> CreateCards()
        {
            return new[]
            {
                new IntentCard("find-item-lost-key", "My brass key was in my pocket, and now it is not.", FindItemIntentId),
                new IntentCard("find-item-seen-notebook", "Has anyone handed in a blue umbrella?", FindItemIntentId),
                new IntentCard("find-item-help-look", "Could you help me find the parcel I left by the door?", FindItemIntentId),

                new IntentCard("ask-location-where-floating", "Whereabouts are you hiding right now?", AskLocationIntentId),
                new IntentCard("ask-location-which-room", "Which room are you in at the moment?", AskLocationIntentId),
                new IntentCard("ask-location-near-door", "Are you anywhere near the vending machine?", AskLocationIntentId),

                new IntentCard("ask-identity-who", "Sorry - who exactly are you?", AskIdentityIntentId),
                new IntentCard("ask-identity-name", "Do you have a name, little one?", AskIdentityIntentId),
                new IntentCard("ask-identity-tell-name", "What should I put on the visitor log for you?", AskIdentityIntentId)
            };
        }

        public static IReadOnlyList<IReadOnlyList<string>> CreateCorrectGroups()
        {
            return new IReadOnlyList<string>[]
            {
                new[]
                {
                    "find-item-lost-key",
                    "find-item-seen-notebook",
                    "find-item-help-look"
                },
                new[]
                {
                    "ask-location-where-floating",
                    "ask-location-which-room",
                    "ask-location-near-door"
                },
                new[]
                {
                    "ask-identity-who",
                    "ask-identity-name",
                    "ask-identity-tell-name"
                }
            };
        }
    }
}
