import request from "supertest";
import { afterEach, describe, expect, it } from "vitest";
import { createApp } from "../src/app";
import { GhostDatabase } from "../src/database";
import { OllamaGenerateRequest, OllamaGenerateResult, OllamaTextClient } from "../src/ollamaClient";

let database: GhostDatabase | null = null;

function createTestClient(ollamaClient: OllamaTextClient = new FailingOllamaClient()) {
  database = new GhostDatabase(":memory:");
  return request(createApp(database, ollamaClient));
}

afterEach(() => {
  database?.close();
  database = null;
});

describe("Ghost backend", () => {
  it("GET /content returns seeded Act 1-3 content", async () => {
    const client = createTestClient();

    const response = await client.get("/content").expect(200);

    expect(response.body.acts).toHaveLength(3);
    expect(response.body.puzzles).toHaveLength(3);
    expect(response.body.acts.map((act: { actId: string }) => act.actId)).toEqual(["act1", "act2", "act3"]);
    expect(response.body.deterministicCorrectness).toContain("Unity validators remain authoritative");
    expect(response.body.puzzles[0].answerKey).toBeUndefined();
  });

  it("POST /profiles then PUT and GET /progress round-trips progress", async () => {
    const client = createTestClient();

    const profile = await client.post("/profiles").send({}).expect(201);
    const profileId = profile.body.id as string;

    await client
      .put(`/progress/${profileId}`)
      .send({
        actsCompleted: ["act1"],
        levelsCompleted: ["act1:1"],
        narrativeState: { pendingDebriefAct: "act1" }
      })
      .expect(200);

    const progress = await client.get(`/progress/${profileId}`).expect(200);

    expect(progress.body.profileId).toBe(profileId);
    expect(progress.body.actsCompleted).toEqual(["act1"]);
    expect(progress.body.levelsCompleted).toEqual(["act1:1"]);
    expect(progress.body.narrativeState).toEqual({ pendingDebriefAct: "act1" });
  });

  it("POST /attempts inserts an attempt log", async () => {
    const client = createTestClient();
    const profile = await client.post("/profiles").send({}).expect(201);

    const attempt = await client
      .post("/attempts")
      .send({
        profileId: profile.body.id,
        actId: "act2",
        result: "incorrect",
        details: { errors: ["missing span"], source: "unity-client" }
      })
      .expect(201);

    expect(attempt.body.id).toBeTypeOf("number");
    expect(attempt.body.profileId).toBe(profile.body.id);
    expect(attempt.body.actId).toBe("act2");
    expect(attempt.body.result).toBe("incorrect");
    expect(attempt.body.details).toEqual({ errors: ["missing span"], source: "unity-client" });
  });

  it("POST /hints falls back to a static hint and logs when Ollama is unavailable", async () => {
    const client = createTestClient(new FailingOllamaClient());

    const response = await client
      .post("/hints")
      .send({
        actId: "act1",
        level: "1",
        trigger: "ask_lily_button",
        state: { reason: "player asked for help in test" }
      })
      .expect(200);

    expect(response.body.source).toBe("static");
    expect(response.body.hint).toContain("What does the person want");
    expect(database?.getHintLogCount()).toBe(1);
    expect(database?.getLatestHintLogPayload()).toMatchObject({
      kind: "hint",
      source: "static",
      level: "1",
      trigger: "ask_lily_button"
    });
    expect(database?.getLatestHintLogPayload()?.state).toContain("player asked for help");
    expect(database?.getLatestHintLogPayload()?.error).toContain("Ollama unavailable");
  });

  it("POST /hints returns LLM text and logs trigger/state when Ollama succeeds", async () => {
    const client = createTestClient(new SuccessfulOllamaClient("Try looking at the request purpose first."));

    const response = await client
      .post("/hints")
      .send({
        actId: "act2",
        level: "1",
        trigger: "after_incorrect_validate",
        state: {
          summary: "last validate result was incorrect",
          errorCount: 2
        }
      })
      .expect(200);

    expect(response.body.source).toBe("llm");
    expect(response.body.hint).toBe("Try looking at the request purpose first.");
    expect(database?.getHintLogCount()).toBe(1);
    expect(database?.getLatestHintLogPayload()).toMatchObject({
      kind: "hint",
      source: "llm",
      level: "1",
      trigger: "after_incorrect_validate",
      error: null
    });
    expect(database?.getLatestHintLogPayload()?.state).toContain("errorCount");
  });

  it("POST /responses falls back to static Ghost text when Ollama is unavailable", async () => {
    const client = createTestClient();

    const response = await client
      .post("/responses")
      .send({
        actId: "act3",
        state: { result: "incorrect" }
      })
      .expect(200);

    expect(response.body.source).toBe("static");
    expect(response.body.text).toContain("reply map");
  });
});

class FailingOllamaClient implements OllamaTextClient {
  public readonly baseUrl = "http://localhost:11434";
  public readonly model = "granite3.1-dense:2b";

  public generateText(_request: OllamaGenerateRequest): Promise<OllamaGenerateResult> {
    return Promise.reject(new Error("Ollama unavailable in test."));
  }
}

class SuccessfulOllamaClient implements OllamaTextClient {
  public readonly baseUrl = "http://localhost:11434";
  public readonly model = "granite3.1-dense:2b";

  public constructor(private readonly responseText: string) {
  }

  public generateText(_request: OllamaGenerateRequest): Promise<OllamaGenerateResult> {
    return Promise.resolve({ text: this.responseText });
  }
}
