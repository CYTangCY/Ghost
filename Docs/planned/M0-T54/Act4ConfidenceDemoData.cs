using System.Collections.Generic;

namespace Ghost.Puzzles.ConfidenceFallback
{
    /// <summary>
    /// One evening on the front desk. The scores below are picked so every visitor's outcome flips
    /// somewhere the player can actually drag a handle to. The previous version had a 65-80 pass band
    /// with no visitor score inside it, so the dial changed nothing and the puzzle solved itself.
    /// The EveryVisitorFlips test exists to stop that coming back.
    /// </summary>
    public static class Act4ConfidenceDemoData
    {
        // Deliberately silly opening state: everything lands in the middle band, so Ghost's first
        // instinct is to ask the entire queue to say it again.
        public const int StartingHandoffEdge = 10;
        public const int StartingAnswerEdge = 90;

        public static IReadOnlyList<Act4VisitorMessage> CreateVisitorMessages()
        {
            return new[]
            {
                // 88 - obvious. Sets the ceiling: park the answer handle above this and Ghost starts
                // bouncing questions a noticeboard could have answered.
                new Act4VisitorMessage(
                    "vending-machine",
                    "Which floor is the vending machine on?",
                    88,
                    new[] { Act4RouteOutcome.IntentReply },
                    new Act4VisitorLines(
                        "Ghost: Second floor, by the noticeboard! Not the one on three. That one eats coins.",
                        "Ghost asks her to say it again. She says it again. Slower. Identical. She is very " +
                        "patient about it, which somehow makes it worse.",
                        "Ghost calls Lily down for this. Lily: ...oh. Um. Second floor. (She goes back upstairs.)",
                        "Ghost has nothing attached for that band, so it just hovers. She waits, shrugs, " +
                        "and takes the stairs.")),

                // 63 - the actual decision. Answering and bouncing are both defensible and both cost
                // something, so the validator accepts either.
                new Act4VisitorMessage(
                    "courier-vague",
                    "I'm after that room, the one with the machine in it?",
                    63,
                    new[] { Act4RouteOutcome.IntentReply, Act4RouteOutcome.Fallback },
                    new Act4VisitorLines(
                        "Ghost sends him to the server room on three. He wanted the print room. He is back " +
                        "eleven minutes later, still holding the box, noticeably less cheerful.",
                        "Ghost: Sorry - which machine? He sighs, says 'the printer', and Ghost walks him " +
                        "straight there. Two minutes lost. Box delivered.",
                        "Ghost calls Lily down for a parcel. She blinks, points at the print room, and " +
                        "nobody needed her for that.",
                        "Ghost says nothing at all. He leaves the box by the door with no name on it.")),

                // 34 and furious. Only a human ends this well, and bouncing her is worse than bluffing -
                // which is the entire reason the third band has to exist.
                new Act4VisitorMessage(
                    "locked-out",
                    "your door system is rubbish, i've been stuck out here half an hour and my card won't scan",
                    34,
                    new[] { Act4RouteOutcome.Handoff },
                    new Act4VisitorLines(
                        "Ghost confidently reads out a door code. It is last term's code. She tries it twice, " +
                        "and then stops being polite.",
                        "Ghost: Could you rephrase that? She has been outside for half an hour. Asking her to " +
                        "say it again, more nicely, is the single worst thing anyone could have done.",
                        "Ghost calls Lily. Lily: oh - oh no, how long have you been out there? Come in, come " +
                        "in, I'll sort the card, I'm so sorry. (She sorts the card.)",
                        "Ghost goes quiet and hopes it blows over. It does not blow over."),
                    soundsUpset: true),

                // Arrives on the second pass at 71, right about where most players will have parked the
                // answer handle. Forces one deliberate re-decision instead of set-and-forget.
                new Act4VisitorMessage(
                    "hurried-parent",
                    "I'm in a rush - how do I get to meeting room three?",
                    71,
                    new[] { Act4RouteOutcome.IntentReply },
                    new Act4VisitorLines(
                        "Ghost: End of this corridor, left at the plant. She is gone before Ghost finishes " +
                        "saying 'plant'.",
                        "Ghost asks her to rephrase. She did say she was in a rush. She repeats it in four " +
                        "words, and Ghost still made her spend them.",
                        "Ghost calls Lily to give someone directions down one corridor.",
                        "Ghost stalls. She reads the fire-escape map instead and works it out herself."),
                    arrivesInPass: 2)
            };
        }

        public static string DescribePosture(Act4Posture posture)
        {
            switch (posture)
            {
                case Act4Posture.Bold:
                    return "Lily: you set Ghost to speak up more. It's quicker, and it did nearly send that " +
                        "parcel to the wrong floor. That's the trade, not a mistake.";
                case Act4Posture.Cautious:
                    return "Lily: you set Ghost to check before it commits. Nobody got sent anywhere wrong, " +
                        "and everybody had to say things twice. That's the trade, not a mistake.";
                default:
                    return "Lily: um - the dial isn't settled yet, so Ghost doesn't really know when it's " +
                        "allowed to speak.";
            }
        }
    }
}
