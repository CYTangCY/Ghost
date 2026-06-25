import { GhostDatabase, LearningContentSummary } from "./database";
import { OllamaTextClient } from "./ollamaClient";

export type LlmRequestBody = {
  profileId?: string;
  actId?: string;
  level?: string;
  trigger?: string;
  state?: unknown;
};

export type ChatHistoryItem = {
  role?: string;
  text?: string;
};

export type ChatRequestBody = {
  profileId?: string;
  actId?: string;
  level?: string;
  message?: string;
  history?: ChatHistoryItem[];
  playerName?: string;
};

export type HintOrResponseResult = {
  text: string;
  source: "llm" | "static";
  error?: string;
};

const staticHints: Record<string, string> = {
  act1: "Um... maybe don't look at the exact words first. What does the person want Ghost to do?",
  act2: "I think Ghost understands the broad request, but it lost the useful detail. Look for the place, time, or thing the message depends on.",
  act3: "Ghost knows the request and the details, but it still needs a safe order. Try checking whether the room is known before Ghost answers."
};

const staticResponses: Record<string, string> = {
  act1: "Ghost squints at the message cards, still trying to hear the purpose behind the words.",
  act2: "Ghost catches the shape of the request, then looks around for the missing detail.",
  act3: "Ghost traces the reply map carefully, trying to ask before it guesses."
};

const staticChatReplies: Record<string, string> = {
  act1: "Um... let's stay with Ghost's message purpose for now, okay?",
  act2: "I—I think we should focus on the detail Ghost needs from the message.",
  act3: "Um... let's help Ghost's reply map ask before it answers."
};

export async function createLilyHint(
  database: GhostDatabase,
  ollama: OllamaTextClient,
  body: LlmRequestBody
): Promise<HintOrResponseResult> {
  const actId = normalizeActId(body.actId);
  const content = database.getLearningContentSummary(actId);
  const fallback = getStaticHint(actId);

  let result: HintOrResponseResult;
  try {
    const generated = await ollama.generateText({
      system: createSystemPrompt("hint", content),
      prompt: createUserPrompt("hint", content, body)
    });

    result = {
      text: sanitizeGeneratedText(generated.text, fallback),
      source: "llm"
    };
  } catch (error) {
    warnStaticFallback("hint", actId, ollama, error);
    result = {
      text: fallback,
      source: "static",
      error: error instanceof Error ? error.message : "Ollama request failed."
    };
  }

  database.insertHintLog({
    profileId: body.profileId,
    actId,
    payload: {
      kind: "hint",
      source: result.source,
      level: body.level ?? null,
      trigger: normalizeTrigger(body.trigger),
      state: summarizeState(body.state),
      error: result.error ?? null
    }
  });

  return result;
}

export async function createGhostResponse(
  database: GhostDatabase,
  ollama: OllamaTextClient,
  body: LlmRequestBody
): Promise<HintOrResponseResult> {
  const actId = normalizeActId(body.actId);
  const content = database.getLearningContentSummary(actId);
  const fallback = getStaticResponse(actId);

  try {
    const generated = await ollama.generateText({
      system: createSystemPrompt("response", content),
      prompt: createUserPrompt("response", content, body)
    });

    return {
      text: sanitizeGeneratedText(generated.text, fallback),
      source: "llm"
    };
  } catch (error) {
    warnStaticFallback("response", actId, ollama, error);
    return {
      text: fallback,
      source: "static",
      error: error instanceof Error ? error.message : "Ollama request failed."
    };
  }
}

export async function createLilyChatReply(
  database: GhostDatabase,
  ollama: OllamaTextClient,
  body: ChatRequestBody
): Promise<HintOrResponseResult> {
  const actId = normalizeActId(body.actId);
  const content = database.getLearningContentSummary(actId);
  const fallback = getStaticChatReply(actId);
  const message = sanitizePlayerMessage(body.message);

  let result: HintOrResponseResult;
  try {
    const generated = await ollama.generateText({
      system: createLilyChatSystemPrompt(content, body.playerName),
      prompt: createLilyChatUserPrompt(content, body, message)
    });

    result = {
      text: sanitizeGeneratedText(generated.text, fallback),
      source: "llm"
    };
  } catch (error) {
    warnStaticFallback("chat", actId, ollama, error);
    result = {
      text: fallback,
      source: "static",
      error: error instanceof Error ? error.message : "Ollama request failed."
    };
  }

  database.insertHintLog({
    profileId: body.profileId,
    actId,
    payload: {
      kind: "chat",
      source: result.source,
      level: body.level ?? null,
      trigger: "chat_message",
      message: summarizeState(message),
      historyCount: Array.isArray(body.history) ? body.history.length : 0,
      error: result.error ?? null
    }
  });

  return result;
}

export function getStaticHint(actId: string): string {
  return staticHints[normalizeActId(actId)] ?? "Um... try looking at what Ghost needs to understand next, without jumping straight to the answer.";
}

export function getStaticResponse(actId: string): string {
  return staticResponses[normalizeActId(actId)] ?? "Ghost waits, a little confused, for the next clear step.";
}

export function getStaticChatReply(actId: string): string {
  return staticChatReplies[normalizeActId(actId)] ?? "Um... let's focus on helping Ghost right now, okay?";
}

function createSystemPrompt(kind: "hint" | "response", content: LearningContentSummary | null): string {
  const concept = content?.concept ?? "chatbot and NLP puzzle concepts";
  const objective = content?.learningObjective ?? "Help the player understand the concept through the puzzle action.";

  if (kind === "hint") {
    return [
      "You are Lily, a human postdoctoral senior in a lightly haunted research lab.",
      "Lily is nerdy, slightly timid, warm, technically capable, and stammers a little, such as 'Um...' or 'I—I think...'.",
      "Give one short hint for the Ghost educational puzzle in Lily's voice.",
      `Curriculum concept: ${concept}.`,
      `Learning objective: ${objective}.`,
      "Do not reveal exact placements, exact spans, exact node wiring, answer keys, or step-by-step solutions.",
      "Do not decide correctness or scoring. The Unity deterministic validator is authoritative.",
      "Return ONE short sentence under about 25 words. Make it a nudge, not a solution."
    ].join(" ");
  }

  return [
    "You are writing Ghost's natural-language reaction in a cute haunted lab puzzle game.",
    "Ghost is a cute ghost, not a chatbot or AI assistant.",
    `Curriculum concept: ${concept}.`,
    `Learning objective: ${objective}.`,
    "Generate a short flavour response or explanatory reaction only.",
    "Do not score the player, gate progression, reveal answer keys, or claim the puzzle is correct unless the provided state says so.",
    "Return ONE short sentence under about 25 words."
  ].join(" ");
}

function createLilyChatSystemPrompt(content: LearningContentSummary | null, playerName: string | undefined): string {
  const concept = content?.concept ?? "chatbot and NLP puzzle concepts";
  const objective = content?.learningObjective ?? "Help the player understand the concept through the puzzle action.";
  const addressedName = typeof playerName === "string" && playerName.trim().length > 0
    ? playerName.trim()
    : "the player";

  return [
    "You are Lily, a human postdoctoral senior in a lightly haunted research lab.",
    "Lily is not an AI, chatbot, or generic assistant.",
    "She is nerdy, timid, warm, technically capable, and a little hesitant.",
    "She stammers lightly using phrases like 'Um...', 'I—I think...', or trailing off.",
    `Address ${addressedName} by name when it feels natural.`,
    `Current curriculum concept: ${concept}.`,
    `Learning objective: ${objective}.`,
    "Only discuss the current act's chatbot/NLP concept and the Ghost story situation.",
    "If asked about Lily's private life, react flustered or annoyed and deflect in character.",
    "If asked off-topic, redirect to helping Ghost right now in character.",
    "Never reveal puzzle solutions, exact answer keys, spans, node wiring, or step-by-step fixes.",
    "Never decide correctness, scoring, or progression.",
    "Return ONE short sentence under about 25 words."
  ].join(" ");
}

function createUserPrompt(kind: "hint" | "response", content: LearningContentSummary | null, body: LlmRequestBody): string {
  const title = content?.title ?? normalizeActId(body.actId);
  const state = summarizeState(body.state);
  const trigger = normalizeTrigger(body.trigger);
  const task = kind === "hint"
    ? "Give Lily's next non-spoiler hint."
    : "Give Ghost's short response text.";

  return [
    `Act: ${title}.`,
    `Level: ${body.level ?? "1"}.`,
    `Trigger: ${trigger}.`,
    `Player-facing state summary: ${state}.`,
    task
  ].join("\n");
}

function createLilyChatUserPrompt(content: LearningContentSummary | null, body: ChatRequestBody, message: string): string {
  const title = content?.title ?? normalizeActId(body.actId);
  const history = summarizeChatHistory(body.history);

  return [
    `Act: ${title}.`,
    `Level: ${body.level ?? "1"}.`,
    `Recent chat history: ${history}.`,
    `Player message: ${message}.`,
    "Reply as Lily with one safe, short, in-character sentence."
  ].join("\n");
}

function warnStaticFallback(kind: "hint" | "response" | "chat", actId: string, ollama: OllamaTextClient, error: unknown): void {
  const message = error instanceof Error ? error.message : String(error);
  const model = ollama.model ?? "unknown-model";
  const baseUrl = ollama.baseUrl ?? "unknown-ollama-url";
  console.warn(
    `[Ghost LLM] Falling back to static ${kind} for ${actId}. Ollama URL=${baseUrl}; model=${model}; error=${message}`
  );
}

function sanitizeGeneratedText(text: string, fallback: string): string {
  const trimmed = (text ?? "").trim().replace(/\s+/g, " ");
  if (trimmed.length === 0) {
    return fallback;
  }

  return trimmed.length > 220 ? `${trimmed.slice(0, 217).trim()}...` : trimmed;
}

function summarizeState(state: unknown): string {
  if (state == null) {
    return "No detailed state supplied.";
  }

  if (typeof state === "string") {
    return state.length > 300 ? `${state.slice(0, 297)}...` : state;
  }

  try {
    const json = JSON.stringify(state);
    return json.length > 300 ? `${json.slice(0, 297)}...` : json;
  } catch {
    return "State was supplied but could not be summarized.";
  }
}

function sanitizePlayerMessage(message: string | undefined): string {
  const sanitized = (message ?? "").trim().replace(/\s+/g, " ");
  if (sanitized.length === 0) {
    return "The player asked Lily for help but did not type a message.";
  }

  return sanitized.length > 500 ? `${sanitized.slice(0, 497).trim()}...` : sanitized;
}

function summarizeChatHistory(history: ChatHistoryItem[] | undefined): string {
  if (!Array.isArray(history) || history.length === 0) {
    return "No recent chat history.";
  }

  const recent = history.slice(-6)
    .map((item) => {
      const role = item.role === "lily" ? "Lily" : "Player";
      const text = sanitizePlayerMessage(item.text);
      return `${role}: ${text}`;
    })
    .join(" | ");

  return recent.length > 700 ? `${recent.slice(0, 697).trim()}...` : recent;
}

function normalizeTrigger(trigger: string | undefined): string {
  const normalized = (trigger ?? "").trim().toLowerCase();
  return normalized.length === 0 ? "unspecified" : normalized;
}

function normalizeActId(actId: string | undefined): string {
  const normalized = (actId ?? "").trim().toLowerCase();
  return normalized.length === 0 ? "unknown" : normalized;
}
