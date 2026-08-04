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
                GhostNarrativeState.Chapter0Id,
                DebriefPhaseId,
                LilySpeakerName,
                "So... that's Ghost. First we'll work out what a visitor wants, then teach each part of a useful reply. One small repair at a time."),
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
                "Ghost knows what people want and the details, but it blurts things out of order. Could you build a conversation map that asks when it is unsure and answers when it knows?"),
            new ShellDialogueBeat(
                GhostNarrativeState.Act3Id,
                DebriefPhaseId,
                LilySpeakerName,
                "...It actually held a conversation. We did it, {playerName}."),
            new ShellDialogueBeat(
                GhostNarrativeState.Act4Id,
                IntroPhaseId,
                LilySpeakerName,
                "Um, {playerName}... Ghost is either answering guesses too bravely or freezing on everyone. Could you tune its confidence dial and give it safe routes?"),
            new ShellDialogueBeat(
                GhostNarrativeState.Act4Id,
                DebriefPhaseId,
                LilySpeakerName,
                "That felt safer. Ghost answered when it really knew, asked when it did not, and handed the hard case to me. N-nicely done."),
            new ShellDialogueBeat(
                GhostNarrativeState.Act5Id,
                IntroPhaseId,
                LilySpeakerName,
                "Ghost's reply map looks finished, but rehearsals keep reaching the wrong answers. Could we run every test, compare the mismatches, and repair the wires?"),
            new ShellDialogueBeat(
                GhostNarrativeState.Act5Id,
                DebriefPhaseId,
                LilySpeakerName,
                "All four rehearsals stayed green after the repairs. Testing the whole map again really did catch what one preview missed. Nicely debugged."),
            new ShellDialogueBeat(
                GhostNarrativeState.Act6Id,
                IntroPhaseId,
                LilySpeakerName,
                "The route is tested, but Ghost still needs a real fact before it can answer. Build the backend chain: choose the data source, run the matching action, then turn the result into a complete reply."),
            new ShellDialogueBeat(
                GhostNarrativeState.Act6Id,
                DebriefPhaseId,
                LilySpeakerName,
                "The backend found the closing time, and the response template turned that raw result into a useful sentence. Ghost can answer with real information now."),
            new ShellDialogueBeat(
                GhostNarrativeState.FinalChapterId,
                IntroPhaseId,
                LilySpeakerName,
                "Every lesson repaired one part of Ghost's voice. Now connect the full path, carry one visitor message through it, and see whether Ghost can finally speak clearly."),
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
                        "Um... hi. I'm Lily from the lab. Something unusual happened during the late shift, and I could use a lab partner.");
                case NameEntryScreenId:
                    return new ShellDialogueLine(
                        LilySpeakerName,
                        "Before we go in, what should Ghost call you? Just a name is fine. I mean... only if you want.");
                case ActHubScreenId:
                    return new ShellDialogueLine(
                        LilySpeakerName,
                        "Chapter 0 tells how we met. Chapters 1 through 6 teach one repair each. The Final Chapter combines them when you're ready, {playerName}.");
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

        public static ShellDialogueLine GetAct6Intro(bool allEarlierActsComplete)
        {
            if (allEarlierActsComplete)
            {
                return GetBeat(GhostNarrativeState.Act6Id, IntroPhaseId);
            }

            return new ShellDialogueLine(
                LilySpeakerName,
                "A few earlier lessons are not marked complete yet, but this workbench is still open. Here you'll connect a data source, a backend action, and a response template so Ghost can answer with a real fact.");
        }

        public static ShellDialogueLine GetFinalChapterIntro(bool allTeachingChaptersComplete)
        {
            if (allTeachingChaptersComplete)
            {
                return GetBeat(GhostNarrativeState.FinalChapterId, IntroPhaseId);
            }

            return new ShellDialogueLine(
                LilySpeakerName,
                "The final workbench is open, though a few teaching chapters are not marked complete. You can try the full voice path now, or revisit any lesson first.");
        }

        public static string GetActTitle(string actId)
        {
            switch (actId)
            {
                case GhostNarrativeState.Chapter0Id:
                    return "Chapter 0";
                case GhostNarrativeState.Act1Id:
                    return "Chapter 1";
                case GhostNarrativeState.Act2Id:
                    return "Chapter 2";
                case GhostNarrativeState.Act3Id:
                    return "Chapter 3";
                case GhostNarrativeState.Act4Id:
                    return "Chapter 4";
                case GhostNarrativeState.Act5Id:
                    return "Chapter 5";
                case GhostNarrativeState.Act6Id:
                    return "Chapter 6";
                case GhostNarrativeState.FinalChapterId:
                    return "Final Chapter";
                default:
                    throw new ArgumentException("Unknown act id.", nameof(actId));
            }
        }
    }
}