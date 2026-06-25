import express, { NextFunction, Request, Response } from "express";
import { ChatRequestBody, createGhostResponse, createLilyChatReply, createLilyHint, LlmRequestBody } from "./llmOrchestration";
import { OllamaClient, OllamaTextClient } from "./ollamaClient";
import { GhostDatabase, ProgressPayload } from "./database";

export function createApp(database: GhostDatabase, ollamaClient: OllamaTextClient = new OllamaClient()) {
  const app = express();
  app.use(addDevCorsHeaders);
  app.use(express.json({ limit: "1mb" }));

  app.get("/health", (_request: Request, response: Response) => {
    response.json({ ok: true });
  });

  app.get("/content", (_request: Request, response: Response) => {
    response.json(database.getContent());
  });

  app.post("/profiles", (_request: Request, response: Response) => {
    response.status(201).json(database.createProfile());
  });

  app.get("/progress/:profileId", (request: Request, response: Response) => {
    const profileId = request.params.profileId;
    const progress = database.getProgress(profileId);
    if (progress == null) {
      response.status(404).json({ error: "profile not found" });
      return;
    }

    response.json(progress);
  });

  app.put("/progress/:profileId", (request: Request, response: Response) => {
    const profileId = request.params.profileId;
    const payload = readObjectBody<ProgressPayload>(request);
    const progress = database.upsertProgress(profileId, payload);
    if (progress == null) {
      response.status(404).json({ error: "profile not found" });
      return;
    }

    response.json(progress);
  });

  app.post("/attempts", (request: Request, response: Response) => {
    const payload = readObjectBody<{
      profileId?: string;
      actId?: string;
      result?: string;
      details?: Record<string, unknown>;
    }>(request);

    if (isMissing(payload.profileId) || isMissing(payload.actId) || isMissing(payload.result)) {
      response.status(400).json({ error: "profileId, actId, and result are required" });
      return;
    }

    const attempt = database.insertAttempt({
      profileId: payload.profileId,
      actId: payload.actId,
      result: payload.result,
      details: payload.details
    });
    if (attempt == null) {
      response.status(404).json({ error: "profile not found" });
      return;
    }

    response.status(201).json(attempt);
  });

  app.post("/hints", async (request: Request, response: Response) => {
    const payload = readObjectBody<LlmRequestBody>(request);
    if (isMissing(payload.actId)) {
      response.status(400).json({ error: "actId is required" });
      return;
    }

    const hint = await createLilyHint(database, ollamaClient, payload);
    response.json({
      hint: hint.text,
      source: hint.source
    });
  });

  app.post("/responses", async (request: Request, response: Response) => {
    const payload = readObjectBody<LlmRequestBody>(request);
    if (isMissing(payload.actId)) {
      response.status(400).json({ error: "actId is required" });
      return;
    }

    const ghostResponse = await createGhostResponse(database, ollamaClient, payload);
    response.json({
      text: ghostResponse.text,
      source: ghostResponse.source
    });
  });

  app.post("/chat", async (request: Request, response: Response) => {
    const payload = readObjectBody<ChatRequestBody>(request);
    if (isMissing(payload.actId)) {
      response.status(400).json({ error: "actId is required" });
      return;
    }

    if (isMissing(payload.message)) {
      response.status(400).json({ error: "message is required" });
      return;
    }

    const reply = await createLilyChatReply(database, ollamaClient, payload);
    response.json({
      reply: reply.text,
      source: reply.source
    });
  });

  return app;
}

function addDevCorsHeaders(request: Request, response: Response, next: NextFunction) {
  response.setHeader("Access-Control-Allow-Origin", "*");
  response.setHeader("Access-Control-Allow-Headers", "Content-Type");
  response.setHeader("Access-Control-Allow-Methods", "GET,POST,PUT,OPTIONS");

  if (request.method === "OPTIONS") {
    response.sendStatus(204);
    return;
  }

  next();
}

function readObjectBody<T extends Record<string, unknown>>(request: Request): T {
  if (request.body != null && typeof request.body === "object" && !Array.isArray(request.body)) {
    return request.body as T;
  }

  return {} as T;
}

function isMissing(value: unknown): value is undefined {
  return typeof value !== "string" || value.trim().length === 0;
}
