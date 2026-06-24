export type SeedLearningContent = {
  actId: string;
  title: string;
  concept: string;
  learningObjective: string;
  metadata: Record<string, unknown>;
};

export type SeedPuzzle = {
  id: string;
  actId: string;
  level: string;
  title: string;
  content: Record<string, unknown>;
  answerKey: Record<string, unknown>;
};

const act1Cards = [
  { id: "find-item-lost-key", messageText: "I lost my little brass key.", intentId: "find_item" },
  { id: "find-item-seen-notebook", messageText: "Has anyone seen my notebook?", intentId: "find_item" },
  { id: "find-item-help-look", messageText: "Can Ghost help me look for the missing lantern?", intentId: "find_item" },
  { id: "ask-location-where-floating", messageText: "Where are you floating right now?", intentId: "ask_location" },
  { id: "ask-location-which-room", messageText: "Which room are you in?", intentId: "ask_location" },
  { id: "ask-location-near-door", messageText: "Are you near the old door?", intentId: "ask_location" },
  { id: "ask-identity-who", messageText: "Who are you?", intentId: "ask_identity" },
  { id: "ask-identity-name", messageText: "What should I call this little ghost?", intentId: "ask_identity" },
  { id: "ask-identity-tell-name", messageText: "Can you tell me your name?", intentId: "ask_identity" }
];

const act1Intents = [
  { id: "find_item", label: "Find Item" },
  { id: "ask_location", label: "Ask Location" },
  { id: "ask_identity", label: "Ask Identity" }
];

function createEntitySpan(messageText: string, surfaceText: string, typeId: string, category: "System" | "Custom") {
  const start = messageText.indexOf(surfaceText);
  if (start < 0) {
    throw new Error(`Seed span '${surfaceText}' was not found in '${messageText}'.`);
  }

  return {
    start,
    length: surfaceText.length,
    surfaceText,
    type: { id: typeId, category }
  };
}

const labAtNight = "Ghost heard humming in the lab at 9pm.";
const laboratory = "Ghost heard humming in the laboratory.";
const lantern = "Ghost tucked the lantern under the desk.";

const act2Messages = [
  {
    id: "lab-at-night",
    messageText: labAtNight,
    correctSpans: [
      createEntitySpan(labAtNight, "lab", "room", "Custom"),
      createEntitySpan(labAtNight, "9pm", "time", "System")
    ]
  },
  {
    id: "laboratory-synonym",
    messageText: laboratory,
    correctSpans: [
      createEntitySpan(laboratory, "laboratory", "room", "Custom")
    ]
  },
  {
    id: "lantern-object",
    messageText: lantern,
    correctSpans: [
      createEntitySpan(lantern, "lantern", "object", "Custom")
    ]
  }
];

const act3Graph = {
  startNodeId: "start",
  nodes: [
    { id: "start", type: "Start" },
    { id: "intent_find_object", type: "IntentBranch", intentId: "find_object" },
    { id: "check_room", type: "SlotCheck", requiredEntityType: "room" },
    { id: "response_answer_object_location", type: "Response", responseId: "answer_object_location" },
    { id: "response_ask_for_room", type: "Response", responseId: "ask_for_room" }
  ],
  transitions: [
    { fromNodeId: "start", toNodeId: "intent_find_object", condition: "Always" },
    { fromNodeId: "intent_find_object", toNodeId: "check_room", condition: "Always" },
    { fromNodeId: "check_room", toNodeId: "response_answer_object_location", condition: "SlotPresent" },
    { fromNodeId: "check_room", toNodeId: "response_ask_for_room", condition: "SlotMissing" }
  ]
};

const act3TestCases = [
  {
    id: "find-object-with-room",
    turn: {
      intentId: "find_object",
      entities: { room: "lab" }
    },
    expectedResponseId: "answer_object_location"
  },
  {
    id: "find-object-missing-room",
    turn: {
      intentId: "find_object",
      entities: {}
    },
    expectedResponseId: "ask_for_room"
  }
];

export const learningContent: SeedLearningContent[] = [
  {
    actId: "act1",
    title: "Act 1: Intent Classification",
    concept: "Intent classification",
    learningObjective: "Group different messages by the speaker's underlying purpose.",
    metadata: {
      mechanic: "drag-and-drop classification",
      ghostProblem: "Ghost reacts to the wrong purpose behind a message."
    }
  },
  {
    actId: "act2",
    title: "Act 2: Entity Extraction",
    concept: "Entity extraction",
    learningObjective: "Tag important details such as rooms, times, and objects.",
    metadata: {
      mechanic: "span annotation with entity typing",
      ghostProblem: "Ghost gets the purpose but misses key details."
    }
  },
  {
    actId: "act3",
    title: "Act 3: Dialog Management",
    concept: "Dialog node configuration",
    learningObjective: "Connect conversation steps so Ghost asks when information is missing and answers when it is known.",
    metadata: {
      mechanic: "node graph assembly",
      ghostProblem: "Ghost understands the pieces but replies out of order."
    }
  }
];

export const puzzles: SeedPuzzle[] = [
  {
    id: "act1-intent-sorting",
    actId: "act1",
    level: "1",
    title: "Intent Sorting",
    content: {
      intents: act1Intents,
      cards: act1Cards
    },
    answerKey: {
      referenceOnly: true,
      note: "Reference/analytics only. Unity client validators remain authoritative.",
      correctGroups: [
        ["find-item-lost-key", "find-item-seen-notebook", "find-item-help-look"],
        ["ask-location-where-floating", "ask-location-which-room", "ask-location-near-door"],
        ["ask-identity-who", "ask-identity-name", "ask-identity-tell-name"]
      ]
    }
  },
  {
    id: "act2-entity-extraction",
    actId: "act2",
    level: "1",
    title: "Entity Extraction",
    content: {
      entityTypes: [
        { id: "time", category: "System" },
        { id: "room", category: "Custom" },
        { id: "object", category: "Custom" }
      ],
      messages: act2Messages.map((message) => ({
        id: message.id,
        messageText: message.messageText
      }))
    },
    answerKey: {
      referenceOnly: true,
      note: "Reference/analytics only. Unity client validators remain authoritative.",
      messages: act2Messages
    }
  },
  {
    id: "act3-dialog-graph",
    actId: "act3",
    level: "1",
    title: "Dialog Graph",
    content: {
      vocabulary: {
        intentIds: ["find_object"],
        entityTypeIds: ["room"],
        responseIds: ["answer_object_location", "ask_for_room"]
      },
      testCases: act3TestCases
    },
    answerKey: {
      referenceOnly: true,
      note: "Reference/analytics only. Unity client graph simulator/validator remains authoritative.",
      targetGraph: act3Graph,
      testCases: act3TestCases
    }
  }
];
