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

    public readonly struct ShellDialogueBeat
    {
        public ShellDialogueBeat(string actId, string phase, string speakerName, string text)
        {
            ActId = actId ?? string.Empty;
            Phase = phase ?? string.Empty;
            Line = new ShellDialogueLine(speakerName, text);
        }

        public string ActId { get; }

        public string Phase { get; }

        public ShellDialogueLine Line { get; }
    }

    public static class ShellDialogueData
    {
        public const string TitleScreenId = "title";
        public const string NameEntryScreenId = "name_entry";
        public const string ActHubScreenId = "act_hub";
        public const string IntroPhaseId = "intro";
        public const string DebriefPhaseId = "debrief";
        public const string ClosingPhaseId = "closing";
        public const string LilySpeakerName = "Lily";
        public const string GhostSpeakerName = "Ghost";

        private static readonly ShellDialogueBeat[] ActBeats =
        {
            new ShellDialogueBeat(
                GhostNarrativeState.Act1Id,
                IntroPhaseId,
                LilySpeakerName,
                "Um, {playerName}... Ghost keeps answering the wrong thing, like it hears the words but not what people actually want. Could you help it sort messages by purpose?"),
            new ShellDialogueBeat(
                GhostNarrativeState.Act1Id,
                DebriefPhaseId,
                LilySpeakerName,
                "It worked. Ghost's reacting to what people mean now. That's... kind of amazing."),
            new ShellDialogueBeat(
                GhostNarrativeState.Act2Id,
                IntroPhaseId,
                LilySpeakerName,
                "Okay, Ghost gets the gist now... but it keeps missing the important details, like which room or which thing. Maybe help it notice those?"),
            new ShellDialogueBeat(
                GhostNarrativeState.Act2Id,
                DebriefPhaseId,
                LilySpeakerName,
                "It's catching the details now. {playerName}, it's really starting to understand."),
            new ShellDialogueBeat(
                GhostNarrativeState.Act3Id,
                IntroPhaseId,
                LilySpeakerName,
                "Last piece: Ghost knows what people want and the details, but it blurts things out of order. Could you build it a little conversation map, ask when it's unsure, answer when it knows?"),
            new ShellDialogueBeat(
                GhostNarrativeState.Act3Id,
                DebriefPhaseId,
                LilySpeakerName,
                "...It actually held a conversation. We did it, {playerName}."),
            new ShellDialogueBeat(
                GhostNarrativeState.Act3Id,
                ClosingPhaseId,
                GhostSpeakerName,
                "Thank you, {playerName}.")
        };

        public static ShellDialogueLine GetLine(string screenId)
        {
            switch (screenId)
            {
                case TitleScreenId:
                    return new ShellDialogueLine(
                        LilySpeakerName,
                        "Um... hi. I'm Lily from the lab. Ghost's messages are getting tangled, so maybe we can help it understand what people mean.");
                case NameEntryScreenId:
                    return new ShellDialogueLine(
                        LilySpeakerName,
                        "Before we go in, what should Ghost call you? Just a name is fine. I mean... only if you want.");
                case ActHubScreenId:
                    return new ShellDialogueLine(
                        LilySpeakerName,
                        "Okay, {playerName}. First Ghost learns what people want, then the details, then how to reply in order. I'll stay nearby if that helps.");
                default:
                    throw new ArgumentException("Unknown shell dialogue screen id.", nameof(screenId));
            }
        }

        public static ShellDialogueLine GetBeat(string actId, string phase)
        {
            foreach (var beat in ActBeats)
            {
                if (string.Equals(beat.ActId, actId, StringComparison.Ordinal) &&
                    string.Equals(beat.Phase, phase, StringComparison.Ordinal))
                {
                    return beat.Line;
                }
            }

            throw new ArgumentException("Unknown shell dialogue beat.", nameof(actId));
        }

        public static string GetActTitle(string actId)
        {
            switch (actId)
            {
                case GhostNarrativeState.Act1Id:
                    return "Act 1";
                case GhostNarrativeState.Act2Id:
                    return "Act 2";
                case GhostNarrativeState.Act3Id:
                    return "Act 3";
                default:
                    throw new ArgumentException("Unknown act id.", nameof(actId));
            }
        }
    }
}
