using System;
using System.Collections.Generic;
using Ghost.Presentation.Shell;

namespace Ghost.Presentation.Banter
{
    public enum AmbientBanterBeatKind
    {
        Line
    }

    public readonly struct AmbientBanterBeat
    {
        public AmbientBanterBeat(
            string speaker,
            string text,
            string tag = null,
            AmbientBanterBeatKind kind = AmbientBanterBeatKind.Line,
            IReadOnlyList<string> futureChoices = null)
        {
            Speaker = speaker ?? string.Empty;
            Text = text ?? string.Empty;
            Tag = tag ?? string.Empty;
            Kind = kind;
            FutureChoices = futureChoices ?? Array.Empty<string>();
        }

        public string Speaker { get; }

        public string Text { get; }

        public string Tag { get; }

        public AmbientBanterBeatKind Kind { get; }

        public IReadOnlyList<string> FutureChoices { get; }
    }

    public static class BanterData
    {
        private static readonly IReadOnlyList<AmbientBanterBeat> Act1Beats = new[]
        {
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "Okay... read the wish behind the words, not the words. You've got this, {playerName}.",
                "lily_nervous_hint"),
            new AmbientBanterBeat(
                ShellDialogueData.GhostSpeakerName,
                "...bl-blrb?",
                "ghost_garbled"),
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "Sorry. I think out loud when I'm nervous. Ignore me.",
                "lily_backpedal"),
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "Whatever they're asking for, that's the group it belongs to.",
                "lily_intent_hint")
        };

        private static readonly IReadOnlyList<AmbientBanterBeat> Act2Beats = new[]
        {
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "You're quick at this. I mean, good teamwork, {playerName}.",
                "lily_warmer"),
            new AmbientBanterBeat(
                ShellDialogueData.GhostSpeakerName,
                "...key? lantern?",
                "ghost_details"),
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "Look, Ghost's noticing nouns now. They grow up so fast.",
                "lily_first_joke"),
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "...That was a joke. Sorry.",
                "lily_joke_backpedal"),
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "It's kind of nice having company down here this late.",
                "lily_warm")
        };

        private static readonly IReadOnlyList<AmbientBanterBeat> Act3Beats = new[]
        {
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "Dialog trees: my natural habitat.",
                "lily_nerdy_joke"),
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "...okay, that one I meant. Mostly.",
                "lily_nerdy_joke_backpedal"),
            new AmbientBanterBeat(
                ShellDialogueData.GhostSpeakerName,
                "where... room?",
                "ghost_question"),
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "It's asking follow-up questions. We basically raised a polite ghost.",
                "lily_proud"),
            new AmbientBanterBeat(
                ShellDialogueData.LilySpeakerName,
                "If this works I'm putting 'ghost whisperer' on my CV. ...Kidding. Probably.",
                "lily_cv_joke"),
            new AmbientBanterBeat(
                ShellDialogueData.GhostSpeakerName,
                "thank... you.",
                "ghost_clear")
        };

        public static IReadOnlyList<AmbientBanterBeat> GetBeats(string actId)
        {
            switch (actId)
            {
                case GhostNarrativeState.Act1Id:
                    return Act1Beats;
                case GhostNarrativeState.Act2Id:
                    return Act2Beats;
                case GhostNarrativeState.Act3Id:
                    return Act3Beats;
                default:
                    return Array.Empty<AmbientBanterBeat>();
            }
        }
    }
}
