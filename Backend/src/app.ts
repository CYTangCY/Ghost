import express, { Request, Response } from "express";
import { GhostDatabase, ProgressPayload } from "./database";

export function createApp(database: GhostDatabase) {
  const app = express();
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

  app.post("/hints", (_request: Request, response: Response) => {
    response.status(501).json({ error: "not implemented (M0-T29)" });
  });

  app.post("/responses", (_request: Request, response: Response) => {
    response.status(501).json({ error: "not implemented (M0-T29)" });
  });

  return app;
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
