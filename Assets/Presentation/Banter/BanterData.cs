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
        private static AmbientBanterBeat Lily(string text, string tag)
        {
            return new AmbientBanterBeat(ShellDialogueData.LilySpeakerName, text, tag);
        }

        private static AmbientBanterBeat Ghost(string text, string tag)
        {
            return new AmbientBanterBeat(ShellDialogueData.GhostSpeakerName, text, tag);
        }

        private static readonly IReadOnlyList<AmbientBanterBeat> Act1Beats = new[]
        {
            Lily("Okay... read the wish behind the words, not the words. You've got this, {playerName}.", "act1_lily_nervous_hint"),
            Ghost("...bl-blrb?", "act1_ghost_garbled"),
            Lily("Sorry. I think out loud when I'm nervous. Ignore me.", "act1_lily_backpedal"),
            Ghost("fff... find?", "act1_ghost_find_fragment"),
            Lily("Whatever they're asking for, that's the group it belongs to.", "act1_lily_intent_hint"),
            Ghost("whooo... no, wait...", "act1_ghost_wrong_purpose"),
            Lily("If two messages want the same help, they can sit together.", "act1_lily_same_wish"),
            Ghost("room? broom? ...brm.", "act1_ghost_room_broom"),
            Lily("Try listening for the little verb. It usually gives away the wish.", "act1_lily_verb_hint"),
            Ghost("wish? wshhh.", "act1_ghost_wish"),
            Lily("I promise the lab is usually less haunted after midnight. Usually.", "act1_lily_lab_joke"),
            Ghost("card... card?", "act1_ghost_card"),
            Lily("Ghost is not being rude. I think it grabbed the wrong purpose.", "act1_lily_wrong_purpose"),
            Ghost("help...?", "act1_ghost_help"),
            Lily("Purpose first, exact words second. Sorry, that sounded like a poster.", "act1_lily_poster"),
            Ghost("meep.", "act1_ghost_meep"),
            Lily("If a message says find, lost, or seen, maybe it shares one little wish.", "act1_lily_find_lost_seen"),
            Ghost("float... float... lost.", "act1_ghost_lost"),
            Lily("Maybe ask: what would a helpful Ghost do next?", "act1_lily_reply_hint"),
            Ghost("name? nnn?", "act1_ghost_name"),
            Lily("It is okay to move a card after dropping it. Science also does this. Constantly.", "act1_lily_science"),
            Ghost("purpose... purrr...", "act1_ghost_purpose"),
            Lily("I am very calm. This clipboard is only slightly creased.", "act1_lily_clipboard"),
            Ghost("lab-lab? blab.", "act1_ghost_lab"),
            Lily("The categories are like tiny shelves for wishes.", "act1_lily_shelves"),
            Ghost("wrong-way...", "act1_ghost_wrong_way"),
            Lily("If a card feels sneaky, compare what it asks Ghost to do.", "act1_lily_sneaky_card"),
            Ghost("oh? ohh?", "act1_ghost_oh"),
            Lily("You're doing fine, {playerName}. I am only hovering emotionally.", "act1_lily_player_encourage"),
            Ghost("sort... s-sor?", "act1_ghost_sort"),
            Lily("When in doubt, group by the reply Ghost should choose.", "act1_lily_reply_group"),
            Ghost("thank... thk?", "act1_ghost_thanks")
        };

        private static readonly IReadOnlyList<AmbientBanterBeat> Act2Beats = new[]
        {
            Lily("You're quick at this. I mean, good teamwork, {playerName}.", "act2_lily_warmer"),
            Ghost("...key? lantern?", "act2_ghost_key_lantern"),
            Lily("Look, Ghost's noticing nouns now. They grow up so fast.", "act2_lily_first_joke"),
            Ghost("lab?", "act2_ghost_lab"),
            Lily("...That was a joke. Sorry.", "act2_lily_joke_backpedal"),
            Ghost("nine... pm?", "act2_ghost_time"),
            Lily("It's kind of nice having company down here this late.", "act2_lily_warm"),
            Ghost("shiny thing?", "act2_ghost_shiny"),
            Lily("Now it's less what they want and more which detail matters.", "act2_lily_detail_shift"),
            Ghost("room... room...", "act2_ghost_room"),
            Lily("A room name is tiny but very bossy.", "act2_lily_room_bossy"),
            Ghost("lantern! maybe.", "act2_ghost_lantern"),
            Lily("Times are the lab's built-in chaos. Convenient, but still chaos.", "act2_lily_time_joke"),
            Ghost("time... tick.", "act2_ghost_time_tick"),
            Lily("Custom details are our weird little lab vocabulary.", "act2_lily_custom_vocab"),
            Ghost("notebook? no... light?", "act2_ghost_notebook_light"),
            Lily("If a word points to a place or thing, Ghost probably needs it.", "act2_lily_place_thing"),
            Ghost("detail... det-tail.", "act2_ghost_detail"),
            Lily("Synonyms count, which is rude of language but useful.", "act2_lily_synonyms"),
            Ghost("where... lab.", "act2_ghost_where_lab"),
            Lily("Lantern, lamp, glow-thing. Same idea, different little coat.", "act2_lily_synonym_joke"),
            Ghost("which... thing?", "act2_ghost_which_thing"),
            Lily("I once labelled my mug as equipment. It did not pass review.", "act2_lily_mug"),
            Ghost("I caught one!", "act2_ghost_caught_one"),
            Lily("Ghost caught the request; now it needs the useful noun.", "act2_lily_useful_noun"),
            Ghost("word spark.", "act2_ghost_word_spark"),
            Lily("The exact surface word can change, but the role can stay the same.", "act2_lily_surface_role"),
            Ghost("night... nine.", "act2_ghost_night_nine"),
            Lily("If the chip answers where, when, or what, try tagging it.", "act2_lily_where_when_what"),
            Ghost("same thing? same?", "act2_ghost_same"),
            Lily("Good details make Ghost much less dramatic.", "act2_lily_less_dramatic"),
            Ghost("tiny clue!", "act2_ghost_tiny_clue")
        };

        private static readonly IReadOnlyList<AmbientBanterBeat> Act3Beats = new[]
        {
            Lily("Dialog trees: my natural habitat.", "act3_lily_nerdy_joke"),
            Ghost("where... room?", "act3_ghost_where_room"),
            Lily("...okay, that one I meant. Mostly.", "act3_lily_nerdy_joke_backpedal"),
            Ghost("thank... you.", "act3_ghost_thank_you"),
            Lily("It's asking follow-up questions. We basically raised a polite ghost.", "act3_lily_proud"),
            Ghost("start... then?", "act3_ghost_start_then"),
            Lily("If this works I'm putting 'ghost whisperer' on my CV. ...Kidding. Probably.", "act3_lily_cv_joke"),
            Ghost("I ask?", "act3_ghost_i_ask"),
            Lily("Start with where Ghost begins, then give it a path.", "act3_lily_start_path"),
            Ghost("I know room... answer.", "act3_ghost_known_answer"),
            Lily("A polite reply map is just a hallway with fewer flickering labels. Sorry, not helpful.", "act3_lily_map_joke"),
            Ghost("no room... ask.", "act3_ghost_missing_ask"),
            Lily("The check card is the little pause before Ghost blurts out an answer.", "act3_lily_check_pause"),
            Ghost("map! I follow.", "act3_ghost_follow_map"),
            Lily("Known room goes to answer; missing room goes to ask. It is almost elegant.", "act3_lily_known_missing"),
            Ghost("wrong path... oops.", "act3_ghost_wrong_path"),
            Lily("If a wire feels wrong, Ghost will wander into the wrong sentence.", "act3_lily_wire_wrong"),
            Ghost("wait... check first.", "act3_ghost_check_first"),
            Lily("I like that the dots look cheerful. Technical diagram morale matters.", "act3_lily_dot_morale"),
            Ghost("lab known.", "act3_ghost_lab_known"),
            Lily("Response cards are endpoints. Tiny conversational landing pads.", "act3_lily_endpoints"),
            Ghost("missing? question.", "act3_ghost_missing_question"),
            Lily("If Ghost answers before checking the room, it is guessing with confidence.", "act3_lily_guessing"),
            Ghost("wire tickles.", "act3_ghost_wire"),
            Lily("One map, two outcomes: answer if known, ask if missing.", "act3_lily_two_outcomes"),
            Ghost("I can remember.", "act3_ghost_remember"),
            Lily("I am not emotionally attached to a node graph. I am academically attached.", "act3_lily_academic_attachment"),
            Ghost("answer after room.", "act3_ghost_answer_after_room"),
            Lily("Follow the wires like a lab route after the lights flicker.", "act3_lily_lab_route"),
            Ghost("polite ghost.", "act3_ghost_polite"),
            Lily("{playerName}, this is close. Ghost is learning to wait its turn.", "act3_lily_player_close"),
            Ghost("conversation... clear.", "act3_ghost_conversation_clear")
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

        public static string GetStaticHint(string actId)
        {
            switch (actId)
            {
                case GhostNarrativeState.Act1Id:
                    return "Um... maybe don't look at the exact words first. What does the person want Ghost to do?";

                case GhostNarrativeState.Act2Id:
                    return "I think Ghost understands the broad request, but it lost the useful detail. Look for the place, time, or thing the message depends on.";

                case GhostNarrativeState.Act3Id:
                    return "Ghost knows the request and the details, but it still needs a safe order. Try checking whether the room is known before Ghost answers.";

                default:
                    return "Um... try looking at what Ghost needs to understand next, without jumping straight to the answer.";
            }
        }

        public static string GetStaticChatReply(string actId)
        {
            switch (actId)
            {
                case GhostNarrativeState.Act1Id:
                    return "Um... let's stay with what the message wants Ghost to do.";

                case GhostNarrativeState.Act2Id:
                    return "I-I think the useful detail matters most here.";

                case GhostNarrativeState.Act3Id:
                    return "Um... let's keep Ghost's reply map focused on checking before answering.";

                default:
                    return "Um... let's focus on helping Ghost right now, okay?";
            }
        }
    }
}
