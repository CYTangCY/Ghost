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
                new IntentCard("find-item-lost-key", "I lost my little brass key.", FindItemIntentId),
                new IntentCard("find-item-seen-notebook", "Has anyone seen my notebook?", FindItemIntentId),
                new IntentCard("find-item-help-look", "Can Ghost help me look for the missing lantern?", FindItemIntentId),

                new IntentCard("ask-location-where-floating", "Where are you floating right now?", AskLocationIntentId),
                new IntentCard("ask-location-which-room", "Which room are you in?", AskLocationIntentId),
                new IntentCard("ask-location-near-door", "Are you near the old door?", AskLocationIntentId),

                new IntentCard("ask-identity-who", "Who are you?", AskIdentityIntentId),
                new IntentCard("ask-identity-name", "What should I call this little ghost?", AskIdentityIntentId),
                new IntentCard("ask-identity-tell-name", "Can you tell me your name?", AskIdentityIntentId)
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
