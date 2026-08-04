using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.VoicePipeline
{
    /// <summary>
    /// The capstone's authored content. Three visitors, escalating: the first only has to be
    /// understood, the second needs a route built for it, and Lily needs the whole machine at once.
    ///
    /// Everything the player can touch is listed here, and the validator only ever reads from here.
    /// Chapter 3 once shipped unsolvable because its palette was hand-written in the presenter and
    /// drifted away from the puzzle - deriving both from this file makes that failure impossible
    /// rather than merely unlikely.
    /// </summary>
    public static class FinalChapterConversationData
    {
        public const string CourierVisitorId = "courier";
        public const string VendingVisitorId = "vending";
        public const string LilyVisitorId = "lily";

        public const string SourceRoleId = "source";
        public const string ActionRoleId = "action";
        public const string VoiceRoleId = "voice";

        /// <summary>
        /// The two fixed ends of every route. The player wires between them; they are never in a
        /// palette, because where a conversation starts and where it has to arrive are not choices.
        /// </summary>
        public const string RouteStartId = "route_start";

        public const string RouteEndId = "route_end";

        /// <summary>The card a visitor's message never gave an answer for. Not a piece of what they said.</summary>
        public const string NotStatedFragmentId = "frag_not_stated";

        public static readonly string[] ResponseRoleIds = { SourceRoleId, ActionRoleId, VoiceRoleId };

        private static readonly Dictionary<string, FinalChapterOption> Options = BuildOptions();

        private static readonly Dictionary<string, FinalChapterRouteStep> RouteSteps = BuildRouteSteps();

        private static readonly Dictionary<string, FinalChapterResponsePart> Parts = BuildResponseParts();

        private static readonly FinalChapterVisitor[] Visitors = BuildVisitors();

        public static IReadOnlyList<FinalChapterVisitor> CreateVisitors()
        {
            return Visitors;
        }

        public static FinalChapterVisitor GetVisitor(string visitorId)
        {
            foreach (var visitor in Visitors)
            {
                if (string.Equals(visitor.Id, visitorId, StringComparison.Ordinal))
                {
                    return visitor;
                }
            }

            throw new KeyNotFoundException("Unknown visitor: " + visitorId);
        }

        public static FinalChapterOption GetOption(string optionId)
        {
            if (Options.TryGetValue(optionId ?? string.Empty, out var option))
            {
                return option;
            }

            throw new KeyNotFoundException("Unknown option: " + optionId);
        }

        public static FinalChapterRouteStep GetRouteStep(string stepId)
        {
            if (RouteSteps.TryGetValue(stepId ?? string.Empty, out var step))
            {
                return step;
            }

            throw new KeyNotFoundException("Unknown route step: " + stepId);
        }

        public static FinalChapterResponsePart GetResponsePart(string partId)
        {
            if (Parts.TryGetValue(partId ?? string.Empty, out var part))
            {
                return part;
            }

            throw new KeyNotFoundException("Unknown response part: " + partId);
        }

        public static FinalChapterFragment GetFragment(FinalChapterVisitor visitor, string fragmentId)
        {
            foreach (var fragment in visitor.Fragments)
            {
                if (string.Equals(fragment.Id, fragmentId, StringComparison.Ordinal))
                {
                    return fragment;
                }
            }

            throw new KeyNotFoundException("Unknown fragment: " + fragmentId);
        }

        public static string GetStageLabel(FinalChapterStage stage)
        {
            switch (stage)
            {
                case FinalChapterStage.Intent:
                    return "Intent";
                case FinalChapterStage.Entities:
                    return "Details";
                case FinalChapterStage.Dialogue:
                    return "Dialogue route";
                case FinalChapterStage.Confidence:
                    return "Confidence call";
                case FinalChapterStage.Backend:
                    return "Stored data";
                case FinalChapterStage.Response:
                    return "Response";
                default:
                    return stage.ToString();
            }
        }

        public static string GetRoleLabel(string roleId)
        {
            switch (roleId)
            {
                case SourceRoleId:
                    return "DATA SOURCE";
                case ActionRoleId:
                    return "ACTION";
                case VoiceRoleId:
                    return "RESPONSE";
                default:
                    return roleId ?? string.Empty;
            }
        }

        public static string GetActionLabel(FinalChapterAction action)
        {
            switch (action)
            {
                case FinalChapterAction.AnswerNow:
                    return "Answer now";
                case FinalChapterAction.AskAgain:
                    return "Ask one more question";
                case FinalChapterAction.HandOver:
                    return "Hand over to a person";
                default:
                    return "No decision yet";
            }
        }

        private static Dictionary<string, FinalChapterOption> BuildOptions()
        {
            var options = new List<FinalChapterOption>
            {
                // Courier intents. The correct one sits second.
                new FinalChapterOption(
                    "intent_directions",
                    "Asking for directions",
                    "Lost, and needs to be told where a room is.",
                    "Ghost: The Lantern Lab is on the second floor, left at the stairs.",
                    true),
                new FinalChapterOption(
                    "intent_delivery",
                    "Dropping off a delivery",
                    "Carrying something that belongs to someone here.",
                    string.Empty),
                new FinalChapterOption(
                    "intent_building_question",
                    "A general building question",
                    "Something about the building. Unspecified.",
                    "Ghost: The building is open from eight until eleven.",
                    true),
                new FinalChapterOption(
                    "intent_report_fault",
                    "Reporting something broken",
                    "Something here has stopped working.",
                    "Ghost: I have logged a fault. Someone will look at it.",
                    true),

                // Vending intents. The correct one sits last.
                new FinalChapterOption(
                    "intent_refund",
                    "Asking for a refund",
                    "Wants their money back.",
                    "Ghost: I have started a refund. It takes five working days.",
                    true),
                new FinalChapterOption(
                    "intent_buy_something",
                    "Wanting to buy something",
                    "Trying to make a purchase.",
                    "Ghost: The nearest shop is across the courtyard.",
                    true),
                new FinalChapterOption(
                    "intent_report_machine",
                    "Reporting a broken machine, and asking who to tell",
                    "Something is broken, and they want the right person for it.",
                    string.Empty),

                // Lily's intents. The correct one sits first.
                new FinalChapterOption(
                    "intent_check_job",
                    "Checking whether something she left running has finished",
                    "She wants the state of something she started earlier.",
                    string.Empty),
                new FinalChapterOption(
                    "intent_book_room",
                    "Booking a room for this morning",
                    "She wants a room reserved.",
                    "Ghost: The lantern room is free from eleven.",
                    true),
                new FinalChapterOption(
                    "intent_small_talk",
                    "Making conversation",
                    "Just chatting. No request in it.",
                    "Ghost: Good morning. It is a nice day.",
                    true),

                // Stored sources.
                new FinalChapterOption(
                    "backend_room_bookings",
                    "Room bookings",
                    "Who has which room, and when.",
                    "Ghost: The lantern room is booked until noon.",
                    true),
                new FinalChapterOption(
                    "backend_job_queue",
                    "The job queue",
                    "What she started, and whether each one is still running.",
                    string.Empty),
                new FinalChapterOption(
                    "backend_direct_messages",
                    "Her direct messages",
                    "Everything she has written to anyone.",
                    "Ghost: I read through her messages until I found it.",
                    true)
            };

            var map = new Dictionary<string, FinalChapterOption>();
            foreach (var option in options)
            {
                map.Add(option.Id, option);
            }

            return map;
        }

        private static Dictionary<string, FinalChapterRouteStep> BuildRouteSteps()
        {
            var steps = new List<FinalChapterRouteStep>
            {
                new FinalChapterRouteStep(
                    RouteStartId,
                    "They finish speaking",
                    "Every route starts here."),
                new FinalChapterRouteStep(
                    RouteEndId,
                    "Ghost replies",
                    "Every route has to arrive here."),
                new FinalChapterRouteStep(
                    "route_detect_intent",
                    "Work out what they want",
                    "Read the purpose behind the wording."),
                new FinalChapterRouteStep(
                    "route_ask_which",
                    "Ask which one they mean",
                    "Only when the message really does leave it open."),
                new FinalChapterRouteStep(
                    "route_answer_now",
                    "Answer straight away",
                    "Reply from what is already known."),
                new FinalChapterRouteStep(
                    "route_report_path",
                    "Give them the reporting route",
                    "Say who handles it, and what happens next."),
                new FinalChapterRouteStep(
                    "route_hand_to_human",
                    "Hand it to a person",
                    "Stop, and pass the whole conversation on."),
                new FinalChapterRouteStep(
                    "route_look_up",
                    "Look it up",
                    "Fetch the stored fact the answer depends on.")
            };

            var map = new Dictionary<string, FinalChapterRouteStep>();
            foreach (var step in steps)
            {
                map.Add(step.Id, step);
            }

            return map;
        }

        private static Dictionary<string, FinalChapterResponsePart> BuildResponseParts()
        {
            var parts = new List<FinalChapterResponsePart>
            {
                new FinalChapterResponsePart(
                    "part_her_messages",
                    SourceRoleId,
                    "her direct messages",
                    "Reading her messages to answer something she did not ask about them."),
                new FinalChapterResponsePart(
                    "part_job_queue",
                    SourceRoleId,
                    "the job queue",
                    string.Empty),
                new FinalChapterResponsePart(
                    "part_my_memory",
                    SourceRoleId,
                    "whatever I remember hearing",
                    "Ghost remembering a conversation is not a stored fact. Chapter 1 opened on this."),

                new FinalChapterResponsePart(
                    "part_pick_recent",
                    ActionRoleId,
                    "pick whichever one started last",
                    "Guessing which job she meant instead of finding out."),
                new FinalChapterResponsePart(
                    "part_list_running",
                    ActionRoleId,
                    "list the jobs of hers still running",
                    string.Empty),
                new FinalChapterResponsePart(
                    "part_cancel_all",
                    ActionRoleId,
                    "stop all of them",
                    "She asked a question. Acting on her data was never invited."),

                new FinalChapterResponsePart(
                    "part_raw_row",
                    VoiceRoleId,
                    "read the row out exactly as stored",
                    "Chapter 6's lesson: the route was right and Ghost still blurted raw data."),
                new FinalChapterResponsePart(
                    "part_just_yes",
                    VoiceRoleId,
                    "say yes, it finished",
                    "A confident sentence about the wrong job is worse than a question."),
                new FinalChapterResponsePart(
                    "part_name_and_ask",
                    VoiceRoleId,
                    "name both, then ask which she means",
                    string.Empty)
            };

            var map = new Dictionary<string, FinalChapterResponsePart>();
            foreach (var part in parts)
            {
                map.Add(part.Id, part);
            }

            return map;
        }

        private static FinalChapterVisitor[] BuildVisitors()
        {
            var courier = new FinalChapterVisitor
            {
                Id = CourierVisitorId,
                SpeakerName = "Courier",
                Message =
                    "Morning. I've got a parcel for the Lantern Lab - " +
                    "do I leave it at reception, or bring it up?",
                ExpectedReply =
                    "Ghost: The parcel belongs to the Lantern Lab, so please bring it up to the second floor.",
                FailureReply = "Ghost: Sorry - I did not follow that.",
                Stages = new[] { FinalChapterStage.Intent, FinalChapterStage.Entities },
                IntentOptionIds = new[]
                {
                    "intent_directions",
                    "intent_delivery",
                    "intent_building_question",
                    "intent_report_fault"
                },
                ExpectedIntentId = "intent_delivery",
                Fragments = new[]
                {
                    new FinalChapterFragment("frag_parcel", "parcel"),
                    new FinalChapterFragment("frag_lantern_lab", "Lantern Lab"),
                    new FinalChapterFragment(
                        "frag_reception",
                        "reception",
                        true,
                        "Reception is one of the two options he is offering. It is not where the parcel goes."),
                    new FinalChapterFragment(
                        "frag_morning",
                        "Morning",
                        true,
                        "A greeting. It looks like a word worth keeping, and it carries nothing."),
                    new FinalChapterFragment(
                        NotStatedFragmentId,
                        "(they never said)",
                        true,
                        "He said it plainly. Recording it as unsaid throws away something you were given.",
                        true)
                },
                Slots = new[]
                {
                    new FinalChapterSlot(
                        "slot_item",
                        "What is being delivered",
                        "frag_parcel",
                        "Without the item, the reply cannot say what arrived."),
                    new FinalChapterSlot(
                        "slot_destination",
                        "Where it belongs",
                        "frag_lantern_lab",
                        "The destination is stated once, and it is not the place he suggests.")
                },
                ConfidencePercent = 88
            };

            var vending = new FinalChapterVisitor
            {
                Id = VendingVisitorId,
                SpeakerName = "Student",
                Message =
                    "The vending machine on floor two just ate my money, and the one by the library " +
                    "is dead as well - who do I actually tell about this?",
                ExpectedReply =
                    "Ghost: Broken machines go to Building Services. Which one should I log for you - " +
                    "floor two, or the one by the library?",
                FailureReply = "Ghost: Sorry - I did not follow that.",
                Stages = new[]
                {
                    FinalChapterStage.Intent,
                    FinalChapterStage.Entities,
                    FinalChapterStage.Dialogue
                },
                IntentOptionIds = new[]
                {
                    "intent_refund",
                    "intent_buy_something",
                    "intent_report_fault",
                    "intent_report_machine"
                },
                ExpectedIntentId = "intent_report_machine",
                Fragments = new[]
                {
                    new FinalChapterFragment("frag_floor_two", "floor two"),
                    new FinalChapterFragment("frag_by_library", "by the library"),
                    new FinalChapterFragment(
                        "frag_my_money",
                        "my money",
                        true,
                        "What went missing, not where a machine is. Same sentence, different job."),
                    new FinalChapterFragment(
                        "frag_building_services",
                        "Building Services",
                        true,
                        "He never said this. It is your answer, not a detail he gave you."),
                    new FinalChapterFragment(
                        NotStatedFragmentId,
                        "(they never said)",
                        true,
                        "Both machines are named in the message. Neither slot is open.",
                        true)
                },
                Slots = new[]
                {
                    new FinalChapterSlot(
                        "slot_machine_one",
                        "First machine",
                        "frag_floor_two",
                        "Two machines of the same type are named. Each needs its own slot."),
                    new FinalChapterSlot(
                        "slot_machine_two",
                        "Second machine",
                        "frag_by_library",
                        "Drop the second one and Ghost logs a single machine, confidently, and wrongly.")
                },
                RouteStepPalette = new[]
                {
                    "route_detect_intent",
                    "route_answer_now",
                    "route_ask_which",
                    "route_report_path",
                    "route_hand_to_human"
                },
                ExpectedRouteStepIds = new[]
                {
                    "route_detect_intent",
                    "route_report_path",
                    "route_ask_which"
                },
                ConfidencePercent = 64
            };

            var lily = new FinalChapterVisitor
            {
                Id = LilyVisitorId,
                SpeakerName = "Lily",
                Message =
                    "Um... could you check whether the thing I left running has finished? " +
                    "...the one from this morning.",
                ExpectedReply =
                    "Ghost: Two of yours are still running from this morning - the lantern sweep, and " +
                    "the long index rebuild. Which one did you mean?",
                FailureReply = "Ghost: Sorry - I did not follow that.",
                UsesLilyPortrait = true,
                Stages = new[]
                {
                    FinalChapterStage.Intent,
                    FinalChapterStage.Entities,
                    FinalChapterStage.Dialogue,
                    FinalChapterStage.Backend,
                    FinalChapterStage.Confidence,
                    FinalChapterStage.Response
                },
                IntentOptionIds = new[]
                {
                    "intent_check_job",
                    "intent_book_room",
                    "intent_small_talk"
                },
                ExpectedIntentId = "intent_check_job",
                Fragments = new[]
                {
                    new FinalChapterFragment("frag_this_morning", "this morning"),
                    new FinalChapterFragment(
                        "frag_the_thing",
                        "the thing I left running",
                        true,
                        "She did say it - and it names two different jobs at once. Holding it looks like " +
                        "an answer and narrows nothing."),
                    new FinalChapterFragment(
                        "frag_finished",
                        "finished",
                        true,
                        "This is what she is asking about, not a detail that narrows it down."),
                    new FinalChapterFragment(
                        "frag_um",
                        "Um...",
                        true,
                        "Hesitation. Lily does this when she is asking for something."),
                    new FinalChapterFragment(
                        NotStatedFragmentId,
                        "(she never said which)",
                        false,
                        string.Empty,
                        true)
                },
                Slots = new[]
                {
                    new FinalChapterSlot(
                        "slot_when",
                        "When she started it",
                        "frag_this_morning",
                        "The only detail in the message that actually narrows the search."),
                    new FinalChapterSlot(
                        "slot_which_job",
                        "Which job she means",
                        NotStatedFragmentId,
                        "She never says which one. Record that with the card that admits it - an " +
                        "unanswered question is a finding, not an unfinished slot.")
                },
                RouteStepPalette = new[]
                {
                    "route_detect_intent",
                    "route_look_up",
                    "route_answer_now",
                    "route_ask_which",
                    "route_hand_to_human"
                },
                // The lookup comes before the question on purpose: Ghost does not know the request is
                // open until the queue hands back two jobs. The ambiguity is discovered, not guessed.
                ExpectedRouteStepIds = new[]
                {
                    "route_detect_intent",
                    "route_look_up",
                    "route_ask_which"
                },
                BackendOptionIds = new[]
                {
                    "backend_room_bookings",
                    "backend_job_queue",
                    "backend_direct_messages"
                },
                RequiredBackendIds = new[] { "backend_job_queue" },
                ConfidencePercent = 78,
                RequestIsAmbiguous = true,
                AnswerNowCost =
                    "Ghost picks one of the two and sounds certain. Guess wrong and Lily believes " +
                    "something finished when it did not.",
                AskAgainCost =
                    "Lily has to say one more sentence. She is tired - and she came to Ghost anyway.",
                HandOverCost =
                    "There is nobody on the other end of this one. Handing Lily over means nothing happens.",
                ResponsePartIds = new[]
                {
                    "part_her_messages",
                    "part_job_queue",
                    "part_my_memory",
                    "part_pick_recent",
                    "part_list_running",
                    "part_cancel_all",
                    "part_raw_row",
                    "part_just_yes",
                    "part_name_and_ask"
                },
                ExpectedResponseParts = new Dictionary<string, string>
                {
                    { SourceRoleId, "part_job_queue" },
                    { ActionRoleId, "part_list_running" },
                    { VoiceRoleId, "part_name_and_ask" }
                }
            };

            return new[] { courier, vending, lily };
        }
    }
}
