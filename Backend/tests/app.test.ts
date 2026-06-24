import request from "supertest";
import { afterEach, describe, expect, it } from "vitest";
import { createApp } from "../src/app";
import { GhostDatabase } from "../src/database";

let database: GhostDatabase | null = null;

function createTestClient() {
  database = new GhostDatabase(":memory:");
  return request(createApp(database));
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

  it("POST /hints and /responses are explicit M0-T29 stubs", async () => {
    const client = createTestClient();

    await client.post("/hints").send({}).expect(501, { error: "not implemented (M0-T29)" });
    await client.post("/responses").send({}).expect(501, { error: "not implemented (M0-T29)" });
  });
});
