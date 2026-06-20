using System;

namespace Ghost.Presentation.Shell
{
    public readonly struct ShellDialogueLine
    {
        public ShellDialogueLine(string speakerName, string text)
        {
            SpeakerName = speakerName ?? string.Empty;
            Text = text ?? string.Empty;
        }

        public string SpeakerName { get; }

        public string Text { get; }
    }

    public static class ShellDialogueData
    {
        public const string TitleScreenId = "title";
        public const string ActHubScreenId = "act_hub";

        public static ShellDialogueLine GetLine(string screenId)
        {
            switch (screenId)
            {
                case TitleScreenId:
                    return new ShellDialogueLine(
                        "Lily",
                        "Um... hi. I'm Lily from the lab. Ghost's messages are getting tangled, so maybe we can help it understand what people mean.");
                case ActHubScreenId:
                    return new ShellDialogueLine(
                        "Lily",
                        "Let's start small: in Act 1, sort messages by what the speaker wants, not by the exact words. I'll stay nearby if that helps.");
                default:
                    throw new ArgumentException("Unknown shell dialogue screen id.", nameof(screenId));
            }
        }
    }
}
