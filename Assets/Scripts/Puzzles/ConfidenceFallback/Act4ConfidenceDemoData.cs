using System.Collections.Generic;

namespace Ghost.Puzzles.ConfidenceFallback
{
    /// <summary>
    /// One evening on the front desk.
    ///
    /// The point of this queue is that the confidence score is a *proxy*, not the truth. Two visitors
    /// break the ordering on purpose: one scores low but is perfectly clear once you read it, and one
    /// scores higher while being genuinely ambiguous. No pair of handles can therefore give everyone
    /// their ideal outcome, which is the whole lesson - there is no threshold that pleases everybody,
    /// only a trade-off you are willing to defend.
    ///
    /// NoSettingPleasesEveryone in the tests proves that property still holds.
    /// </summary>
    public static class Act4ConfidenceDemoData
    {
        // Opening state is deliberately unhelpful: everyone lands in the middle band.
        public const int StartingHandoffEdge = 10;
        public const int StartingAnswerEdge = 90;

        public static IReadOnlyList<Act4VisitorMessage> CreateVisitorMessages()
        {
            return new[]
            {
                // CRITICAL. Furious and locked out. Only a person can end this well.
                new Act4VisitorMessage(
                    "locked-out",
                    "your door system is rubbish, i've been stuck out here half an hour and my card won't scan",
                    30,
                    Act4RouteOutcome.Handoff,
                    new Act4VisitorLines(
                        "Ghost confidently reads out a door code. It is last term's code. She tries it twice, " +
                        "and then stops being polite.",
                        "Ghost: Could you rephrase that? She has been outside for half an hour. Asking her to " +
                        "say it again, more nicely, is the single worst thing anyone could have done.",
                        "Ghost calls Lily. Lily: oh - oh no, how long have you been out there? Come in, come " +
                        "in, I'll sort the card, I'm so sorry. (She sorts the card.)",
                        "Ghost goes quiet and hopes it blows over. It does not blow over."),
                    isCritical: true,
                    soundsUpset: true),

                // CRITICAL. Nothing about this is unclear.
                new Act4VisitorMessage(
                    "vending-machine",
                    "Which floor is the vending machine on?",
                    90,
                    Act4RouteOutcome.IntentReply,
                    new Act4VisitorLines(
                        "Ghost: Second floor, by the noticeboard! Not the one on three. That one eats coins.",
                        "Ghost asks her to say it again. She says it again. Slower. Identical. She is very " +
                        "patient about it, which somehow makes it worse.",
                        "Ghost calls Lily down for this. Lily: ...oh. Um. Second floor. (She goes back upstairs.)",
                        "Ghost hovers and says nothing. She shrugs and takes the stairs."),
                    isCritical: true),

                // The score says unsure; the sentence is actually perfectly clear. Answering is right.
                new Act4VisitorMessage(
                    "odd-phrasing",
                    "the coffee place - that's the one by the stairs, yes?",
                    52,
                    Act4RouteOutcome.IntentReply,
                    new Act4VisitorLines(
                        "Ghost: That's the one, by the stairs on the ground floor. He is delighted to be right.",
                        "Ghost asks him to rephrase. He says exactly the same words again, slightly louder, " +
                        "and looks a little hurt.",
                        "Ghost calls Lily to confirm where the coffee place is.",
                        "Ghost says nothing. He wanders off to find out for himself."),
                    isCritical: false),

                // Scores higher than the one above, yet nobody could act on it. Bouncing is right.
                new Act4VisitorMessage(
                    "genuinely-vague",
                    "I'm after that room, the one with the machine in it?",
                    68,
                    Act4RouteOutcome.Fallback,
                    new Act4VisitorLines(
                        "Ghost sends him to the server room on three. He wanted the print room. He is back " +
                        "eleven minutes later, still holding the box, noticeably less cheerful.",
                        "Ghost: Sorry - which machine? He sighs, says 'the printer', and Ghost walks him " +
                        "straight there. Two minutes lost. Box delivered.",
                        "Ghost calls Lily down for a parcel. She blinks, points at the print room, and " +
                        "nobody needed her for that.",
                        "Ghost says nothing at all. He leaves the box by the door with no name on it."),
                    isCritical: false),

                // A near-duplicate of the vague one, a few points lower.
                new Act4VisitorMessage(
                    "parcel-floor",
                    "parcel for someone upstairs - third floor, is that right?",
                    61,
                    Act4RouteOutcome.Fallback,
                    new Act4VisitorLines(
                        "Ghost picks a name off the third floor at random. It is the wrong name.",
                        "Ghost: Which name is on it? He reads it out, and Ghost points him at the right door.",
                        "Ghost calls Lily to read a label.",
                        "Ghost stalls. He leaves the parcel on the windowsill."),
                    isCritical: false),

                // Clear enough to answer, and in a hurry about it.
                new Act4VisitorMessage(
                    "print-room-hours",
                    "is the print room still open at this hour?",
                    74,
                    Act4RouteOutcome.IntentReply,
                    new Act4VisitorLines(
                        "Ghost: Open until eight. She thanks it and goes.",
                        "Ghost asks her to rephrase a question it plainly understood. She raises an eyebrow.",
                        "Ghost calls Lily to check opening hours that are printed on the door.",
                        "Ghost says nothing and she reads the sign herself.")),
            };
        }

        public static string DescribePosture(Act4Posture posture)
        {
            switch (posture)
            {
                case Act4Posture.Bold:
                    return "Lily: you let Ghost speak up more often. Fewer people had to repeat themselves, " +
                        "and it did answer a couple it should have checked. That's the trade you chose.";
                case Act4Posture.Cautious:
                    return "Lily: you made Ghost check before committing. Nobody was misled, and several " +
                        "people had to say things twice. That's the trade you chose.";
                default:
                    return "Lily: you split the difference - some were asked again, some were answered on " +
                        "a guess. There was never a setting that suited everyone.";
            }
        }
    }
}
