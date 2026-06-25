export type OllamaGenerateRequest = {
  system: string;
  prompt: string;
};

export type OllamaGenerateResult = {
  text: string;
};

export interface OllamaTextClient {
  readonly baseUrl?: string;
  readonly model?: string;
  generateText(request: OllamaGenerateRequest): Promise<OllamaGenerateResult>;
}

export type OllamaClientConfig = {
  baseUrl: string;
  model: string;
  generateTimeoutMs: number;
  checkTimeoutMs: number;
};

type OllamaGenerateResponse = {
  response?: unknown;
  message?: {
    content?: unknown;
  };
};

export const defaultOllamaModel = "granite3.1-dense:2b";
export const defaultOllamaGenerateTimeoutMs = 60000;
export const defaultOllamaCheckTimeoutMs = 5000;

export function createOllamaConfigFromEnv(): OllamaClientConfig {
  return {
    baseUrl: trimTrailingSlash(process.env.OLLAMA_URL ?? "http://localhost:11434"),
    model: process.env.OLLAMA_MODEL ?? defaultOllamaModel,
    generateTimeoutMs: parsePositiveInt(process.env.OLLAMA_TIMEOUT_MS, defaultOllamaGenerateTimeoutMs),
    checkTimeoutMs: parsePositiveInt(process.env.OLLAMA_CHECK_TIMEOUT_MS, defaultOllamaCheckTimeoutMs)
  };
}

export class OllamaClient implements OllamaTextClient {
  public constructor(private readonly config = createOllamaConfigFromEnv()) {
  }

  public get baseUrl(): string {
    return this.config.baseUrl;
  }

  public get model(): string {
    return this.config.model;
  }

  public async generateText(request: OllamaGenerateRequest): Promise<OllamaGenerateResult> {
    const controller = new AbortController();
    let didTimeout = false;
    const timeout = setTimeout(() => {
      didTimeout = true;
      controller.abort();
    }, this.config.generateTimeoutMs);

    try {
      const response = await fetch(`${this.config.baseUrl}/api/generate`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          model: this.config.model,
          system: request.system,
          prompt: request.prompt,
          stream: false,
          options: {
            temperature: 0.45,
            top_p: 0.9,
            num_predict: 140
          }
        }),
        signal: controller.signal
      });

      if (!response.ok) {
        if (response.status === 404) {
          throw new Error(
            `Ollama model '${this.config.model}' was not found at ${this.config.baseUrl}. Run: ollama pull ${this.config.model}`
          );
        }

        throw new Error(`Ollama returned HTTP ${response.status}`);
      }

      const payload = await response.json() as OllamaGenerateResponse;
      const text = readGeneratedText(payload);
      if (text.length === 0) {
        throw new Error("Ollama returned an empty response.");
      }

      return { text };
    } catch (error) {
      if (didTimeout) {
        throw new Error(`Ollama generation timed out after ${this.config.generateTimeoutMs} ms.`);
      }

      throw error;
    } finally {
      clearTimeout(timeout);
    }
  }
}

export async function listOllamaModels(config = createOllamaConfigFromEnv()): Promise<string[]> {
  const controller = new AbortController();
  let didTimeout = false;
  const timeout = setTimeout(() => {
    didTimeout = true;
    controller.abort();
  }, config.checkTimeoutMs);

  try {
    const response = await fetch(`${config.baseUrl}/api/tags`, {
      method: "GET",
      signal: controller.signal
    });

    if (!response.ok) {
      throw new Error(`Ollama returned HTTP ${response.status}`);
    }

    const payload = await response.json() as { models?: Array<{ name?: unknown }> };
    return Array.isArray(payload.models)
      ? payload.models
        .map((model) => typeof model.name === "string" ? model.name : "")
        .filter((name) => name.length > 0)
      : [];
  } catch (error) {
    if (didTimeout) {
      throw new Error(`Ollama model check timed out after ${config.checkTimeoutMs} ms.`);
    }

    throw error;
  } finally {
    clearTimeout(timeout);
  }
}

function readGeneratedText(payload: OllamaGenerateResponse): string {
  if (typeof payload.response === "string") {
    return payload.response.trim();
  }

  if (typeof payload.message?.content === "string") {
    return payload.message.content.trim();
  }

  return "";
}

function parsePositiveInt(value: string | undefined, fallback: number): number {
  if (value == null) {
    return fallback;
  }

  const parsed = Number.parseInt(value, 10);
  return Number.isFinite(parsed) && parsed > 0 ? parsed : fallback;
}

function trimTrailingSlash(value: string): string {
  return value.trim().replace(/\/+$/, "");
}
