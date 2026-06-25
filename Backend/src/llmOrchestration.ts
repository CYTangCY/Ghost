import { GhostDatabase, LearningContentSummary } from "./database";
import { OllamaTextClient } from "./ollamaClient";

export type LlmRequestBody = {
  profileId?: string;
  actId?: string;
  level?: string;
  trigger?: string;
  state?: unknown;
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

export function getStaticHint(actId: string): string {
  return staticHints[normalizeActId(actId)] ?? "Um... try looking at what Ghost needs to understand next, without jumping straight to the answer.";
}

export function getStaticResponse(actId: string): string {
  return staticResponses[normalizeActId(actId)] ?? "Ghost waits, a little confused, for the next clear step.";
}

function createSystemPrompt(kind: "hint" | "response", content: LearningContentSummary | null): string {
  const concept = content?.concept ?? "chatbot and NLP puzzle concepts";
  const objective = content?.learningObjective ?? "Help the player understand the concept through the puzzle action.";

  if (kind === "hint") {
    return [
      "You are Lily, a human postdoctoral senior in a lightly haunted research lab.",
      "Lily is nerdy, slightly timid, warm, and technically capable.",
      "Give one short hint for the Ghost educational puzzle.",
      `Curriculum concept: ${concept}.`,
      `Learning objective: ${objective}.`,
      "Do not reveal exact placements, exact spans, exact node wiring, answer keys, or step-by-step solutions.",
      "Do not decide correctness or scoring. The Unity deterministic validator is authoritative.",
      "Keep the hint under 45 words and make it a nudge, not a solution."
    ].join(" ");
  }

  return [
    "You are writing Ghost's natural-language reaction in a cute haunted lab puzzle game.",
    "Ghost is a cute ghost, not a chatbot or AI assistant.",
    `Curriculum concept: ${concept}.`,
    `Learning objective: ${objective}.`,
    "Generate a short flavour response or explanatory reaction only.",
    "Do not score the player, gate progression, reveal answer keys, or claim the puzzle is correct unless the provided state says so.",
    "Keep it under 35 words."
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

function warnStaticFallback(kind: "hint" | "response", actId: string, ollama: OllamaTextClient, error: unknown): void {
  const message = error instanceof Error ? error.message : String(error);
  const model = ollama.model ?? "unknown-model";
  const baseUrl = ollama.baseUrl ?? "unknown-ollama-url";
  console.warn(
    `[Ghost LLM] Falling back to static ${kind} for ${actId}. Ollama URL=${baseUrl}; model=${model}; error=${message}`
  );
}

function sanitizeGeneratedText(text: string, fallback: string): string {
  const trimmed = (text ?? "").trim();
  if (trimmed.length === 0) {
    return fallback;
  }

  return trimmed.length > 360 ? `${trimmed.slice(0, 357).trim()}...` : trimmed;
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

function normalizeTrigger(trigger: string | undefined): string {
  const normalized = (trigger ?? "").trim().toLowerCase();
  return normalized.length === 0 ? "unspecified" : normalized;
}

function normalizeActId(actId: string | undefined): string {
  const normalized = (actId ?? "").trim().toLowerCase();
  return normalized.length === 0 ? "unknown" : normalized;
}
