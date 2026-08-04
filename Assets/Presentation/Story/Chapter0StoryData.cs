using System.Collections.Generic;
using Ghost.Presentation.GhostAvatar;

namespace Ghost.Presentation.Story
{
    public readonly struct Chapter0StoryBeat
    {
        public Chapter0StoryBeat(string speaker, string text, GhostMood ghostMood)
        {
            Speaker = speaker ?? string.Empty;
            Text = text ?? string.Empty;
            GhostMood = ghostMood;
        }

        public string Speaker { get; }

        public string Text { get; }

        public GhostMood GhostMood { get; }
    }

    public static class Chapter0StoryData
    {
        public static IReadOnlyList<Chapter0StoryBeat> CreateBeats(string playerName)
        {
            var name = string.IsNullOrWhiteSpace(playerName) ? "Junior" : playerName.Trim();
            return new[]
            {
                new Chapter0StoryBeat(
                    "Lily",
                    "Um... " + name + "? Sorry to stop you this late. Something in the lab keeps trying to talk.",
                    GhostMood.Confused),
                new Chapter0StoryBeat(
                    "Ghost",
                    "...bl-blrb?",
                    GhostMood.Confused),
                new Chapter0StoryBeat(
                    "Lily",
                    "That's Ghost. It's cute, and this is a lightly haunted lab, not a dangerous one. Its words just keep coming out tangled.",
                    GhostMood.Sad),
                new Chapter0StoryBeat(
                    "Ghost",
                    "...help?",
                    GhostMood.Confused),
                new Chapter0StoryBeat(
                    "Lily",
                    "It hears people, but it needs help understanding what they want, noticing the details, and choosing what to say next.",
                    GhostMood.Neutral),
                new Chapter0StoryBeat(
                    "Lily",
                    "Could we help it one message at a time? I'll stay nearby. I mean... we're lab partners now, apparently.",
                    GhostMood.Happy)
            };
        }
    }
}
