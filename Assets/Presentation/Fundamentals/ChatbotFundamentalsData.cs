using System.Collections.Generic;

namespace Ghost.Presentation.Fundamentals
{
    public enum ChatbotFundamentalsInteractionKind
    {
        SingleAction,
        ToggleCompare,
        ComponentOrder,
        ChallengeModes
    }

    public sealed class ChatbotFundamentalsBeat
    {
        public ChatbotFundamentalsBeat(
            string id,
            string title,
            string ghostProblem,
            string lilyExplanation,
            string actionPrompt,
            string consequence,
            ChatbotFundamentalsInteractionKind interactionKind,
            params string[] actionLabels)
        {
            Id = id ?? string.Empty;
            Title = title ?? string.Empty;
            GhostProblem = ghostProblem ?? string.Empty;
            LilyExplanation = lilyExplanation ?? string.Empty;
            ActionPrompt = actionPrompt ?? string.Empty;
            Consequence = consequence ?? string.Empty;
            InteractionKind = interactionKind;
            ActionLabels = actionLabels ?? new string[0];
        }

        public string Id { get; }

        public string Title { get; }

        public string GhostProblem { get; }

        public string LilyExplanation { get; }

        public string ActionPrompt { get; }

        public string Consequence { get; }

        public ChatbotFundamentalsInteractionKind InteractionKind { get; }

        public IReadOnlyList<string> ActionLabels { get; }
    }

    public readonly struct ChatbotComponentPart
    {
        public ChatbotComponentPart(string id, string label, string shortDescription)
        {
            Id = id ?? string.Empty;
            Label = label ?? string.Empty;
            ShortDescription = shortDescription ?? string.Empty;
        }

        public string Id { get; }

        public string Label { get; }

        public string ShortDescription { get; }
    }

    public readonly struct ChatbotChallengeMode
    {
        public ChatbotChallengeMode(string id, string label, string problem, string consequence)
        {
            Id = id ?? string.Empty;
            Label = label ?? string.Empty;
            Problem = problem ?? string.Empty;
            Consequence = consequence ?? string.Empty;
        }

        public string Id { get; }

        public string Label { get; }

        public string Problem { get; }

        public string Consequence { get; }
    }

    public static class ChatbotFundamentalsData
    {
        public const string BackendComponentId = "backend_integration";

        private static readonly ChatbotFundamentalsBeat[] Beats =
        {
            new ChatbotFundamentalsBeat(
                "chatbot_definition",
                "1. A chatbot is a conversation program",
                "Ghost can hear a visitor, but its reply loop is disconnected: message in, silence out.",
                "Um... a chatbot is a program that simulates conversation: it receives a message, works out a reply, and sends one back.",
                "Patch Ghost's reply loop so one visitor message can become one Ghost answer.",
                "Ghost now has a simple conversation loop: visitor message in, Ghost reply out.",
                ChatbotFundamentalsInteractionKind.SingleAction,
                "Patch the reply loop"),
            new ChatbotFundamentalsBeat(
                "nlp_ml_pillars",
                "2. NLP and ML are the two support pillars",
                "Ghost sees words, but it needs help understanding language and learning from examples.",
                "I-I think of it as two supports: NLP helps understand language and intent; ML learns patterns from data.",
                "Light both support lamps for Ghost.",
                "Both supports are lit: NLP helps Ghost interpret the message, and ML represents learning from past examples.",
                ChatbotFundamentalsInteractionKind.SingleAction,
                "Light NLP",
                "Light ML"),
            new ChatbotFundamentalsBeat(
                "rule_vs_ai",
                "3. Rule-based and AI-enabled behave differently",
                "Ghost can follow exact rules, while Lily's chat can phrase flexible help, but neither should secretly score puzzles.",
                "Rules are predictable, like our validators. AI-enabled help is flexible language, like my chat hints, but it must not decide correctness.",
                "Try both sides and compare what changes.",
                "The contrast is visible: rules give stable checks; AI support gives flexible wording without controlling progression.",
                ChatbotFundamentalsInteractionKind.ToggleCompare,
                "Run rule path",
                "Ask AI-style support"),
            new ChatbotFundamentalsBeat(
                "benefits",
                "4. Chatbots help with repeatable work",
                "Three visitors ask the same lab question, and Ghost keeps starting from scratch.",
                "A useful chatbot can handle repeated routine questions efficiently, so humans can focus on the unusual parts.",
                "Let Ghost handle the repeated request once.",
                "Ghost answers the repeated request faster, leaving Lily free to handle the strange haunted edge cases.",
                ChatbotFundamentalsInteractionKind.SingleAction,
                "Handle repeated request"),
            new ChatbotFundamentalsBeat(
                "five_components",
                "5. Five components make the conversation work",
                "Ghost's voice parts are scattered, so its message cannot travel through the whole chatbot system.",
                "The overview path is UI input, NLP engine, dialogue management, response generation, then UI output; backend can connect on the side.",
                "Place the voice parts in order, then attach the backend side link.",
                "Ghost's voice path is connected. A message can enter, be understood, choose a next step, become a reply, and return to the user.",
                ChatbotFundamentalsInteractionKind.ComponentOrder),
            new ChatbotFundamentalsBeat(
                "four_challenges",
                "6. Real chatbots meet messy input",
                "Ghost looks confident, but four common chatbot problems can still trip it up.",
                "The course warns about messy input, misunderstood queries, human-like interaction, and context awareness. Let's show them as tiny Ghost failures.",
                "Trigger each failure mode once and watch Ghost's reaction.",
                "Ghost has seen the four risk zones. Future acts repair these problems one piece at a time.",
                ChatbotFundamentalsInteractionKind.ChallengeModes)
        };

        private static readonly ChatbotComponentPart[] ComponentOrder =
        {
            new ChatbotComponentPart("ui_input", "UI input", "Receives the visitor's message."),
            new ChatbotComponentPart("nlp_engine", "NLP engine", "Finds intent and key details."),
            new ChatbotComponentPart("dialogue_management", "Dialogue management", "Chooses the next step."),
            new ChatbotComponentPart("response_generation", "Response generation", "Builds a suitable reply."),
            new ChatbotComponentPart("ui_output", "UI output", "Shows Ghost's answer.")
        };

        private static readonly ChatbotComponentPart[] ComponentPaletteOrder =
        {
            new ChatbotComponentPart("response_generation", "Response generation", "Builds a suitable reply."),
            new ChatbotComponentPart("ui_input", "UI input", "Receives the visitor's message."),
            new ChatbotComponentPart("dialogue_management", "Dialogue management", "Chooses the next step."),
            new ChatbotComponentPart("ui_output", "UI output", "Shows Ghost's answer."),
            new ChatbotComponentPart("nlp_engine", "NLP engine", "Finds intent and key details.")
        };

        private static readonly ChatbotChallengeMode[] ChallengeModes =
        {
            new ChatbotChallengeMode(
                "unstructured_input",
                "Messy input",
                "The visitor writes: 'uhh lamp?? lab-ish??'",
                "Ghost squints at the messy wording and asks Lily for cleaner clues."),
            new ChatbotChallengeMode(
                "misunderstanding",
                "Misunderstanding",
                "The visitor asks to find a lantern, but Ghost hears a lunch order.",
                "Ghost offers soup. Cute, but wrong."),
            new ChatbotChallengeMode(
                "human_like_interaction",
                "Human-like reply",
                "Ghost answers with a stiff system message.",
                "Lily winces; the reply works, but it does not feel natural yet."),
            new ChatbotChallengeMode(
                "context_awareness",
                "Context awareness",
                "The visitor says 'there' after naming the lab earlier.",
                "Ghost forgets the earlier room and asks for it again.")
        };

        public static IReadOnlyList<ChatbotFundamentalsBeat> CreateBeats()
        {
            return (ChatbotFundamentalsBeat[])Beats.Clone();
        }

        public static IReadOnlyList<ChatbotComponentPart> CreateComponentOrder()
        {
            return (ChatbotComponentPart[])ComponentOrder.Clone();
        }

        public static IReadOnlyList<ChatbotComponentPart> CreateComponentPaletteOrder()
        {
            return (ChatbotComponentPart[])ComponentPaletteOrder.Clone();
        }

        public static IReadOnlyList<ChatbotChallengeMode> CreateChallengeModes()
        {
            return (ChatbotChallengeMode[])ChallengeModes.Clone();
        }
    }
}
