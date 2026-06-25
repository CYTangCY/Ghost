import fs from "node:fs";
import path from "node:path";
import { randomUUID } from "node:crypto";
import Database from "better-sqlite3";
import { learningContent, puzzles } from "./seedData";

type SqliteDatabase = Database.Database;

export type ProgressPayload = {
  actsCompleted?: string[];
  levelsCompleted?: string[];
  narrativeState?: Record<string, unknown>;
};

export type AttemptPayload = {
  profileId: string;
  actId: string;
  result: string;
  details?: Record<string, unknown>;
};

export type HintLogPayload = {
  profileId?: string;
  actId: string;
  payload: Record<string, unknown>;
};

export type LearningContentSummary = {
  actId: string;
  title: string;
  concept: string;
  learningObjective: string;
  metadata: Record<string, unknown>;
};

type LearningContentRow = {
  act_id: string;
  title: string;
  concept: string;
  learning_objective: string;
  metadata_json: string;
};

type PuzzleRow = {
  id: string;
  act_id: string;
  level: string;
  title: string;
  content_json: string;
};

type ProfileRow = {
  id: string;
  created_at: string;
};

type ProgressRow = {
  profile_id: string;
  acts_completed_json: string;
  levels_completed_json: string;
  narrative_state_json: string;
  updated_at: string;
};

export class GhostDatabase {
  private static readonly AnonymousHintProfileId = "profile_anonymous";

  private readonly db: SqliteDatabase;

  public constructor(dbPath: string) {
    ensureDatabaseDirectory(dbPath);
    this.db = new Database(dbPath);
    this.db.pragma("foreign_keys = ON");
    this.createSchema();
    this.seedIfEmpty();
  }

  public close(): void {
    this.db.close();
  }

  public getContent() {
    const contentRows = this.db
      .prepare("SELECT act_id, title, concept, learning_objective, metadata_json FROM learning_content ORDER BY act_id")
      .all() as LearningContentRow[];
    const puzzleRows = this.db
      .prepare("SELECT id, act_id, level, title, content_json FROM puzzles ORDER BY act_id, level")
      .all() as PuzzleRow[];

    return {
      deterministicCorrectness: "Backend serves content and logs attempts; Unity validators remain authoritative.",
      acts: contentRows.map((row) => ({
        actId: row.act_id,
        title: row.title,
        concept: row.concept,
        learningObjective: row.learning_objective,
        metadata: parseJson(row.metadata_json, {})
      })),
      puzzles: puzzleRows.map((row) => ({
        id: row.id,
        actId: row.act_id,
        level: row.level,
        title: row.title,
        content: parseJson(row.content_json, {})
      }))
    };
  }

  public getLearningContentSummary(actId: string): LearningContentSummary | null {
    const row = this.db
      .prepare(
        "SELECT act_id, title, concept, learning_objective, metadata_json FROM learning_content WHERE act_id = ?"
      )
      .get(actId) as LearningContentRow | undefined;

    if (row == null) {
      return null;
    }

    return {
      actId: row.act_id,
      title: row.title,
      concept: row.concept,
      learningObjective: row.learning_objective,
      metadata: parseJson(row.metadata_json, {})
    };
  }

  public createProfile() {
    const id = `profile_${randomUUID()}`;
    const createdAt = nowIso();
    this.db
      .prepare("INSERT INTO profiles (id, created_at) VALUES (?, ?)")
      .run(id, createdAt);

    return { id, createdAt };
  }

  public profileExists(profileId: string): boolean {
    const row = this.db
      .prepare("SELECT id FROM profiles WHERE id = ?")
      .get(profileId) as Pick<ProfileRow, "id"> | undefined;
    return row != null;
  }

  public getProgress(profileId: string) {
    const profile = this.db
      .prepare("SELECT id, created_at FROM profiles WHERE id = ?")
      .get(profileId) as ProfileRow | undefined;
    if (profile == null) {
      return null;
    }

    const row = this.db
      .prepare(
        "SELECT profile_id, acts_completed_json, levels_completed_json, narrative_state_json, updated_at FROM progress WHERE profile_id = ?"
      )
      .get(profileId) as ProgressRow | undefined;

    if (row == null) {
      return {
        profileId,
        actsCompleted: [],
        levelsCompleted: [],
        narrativeState: {},
        updatedAt: profile.created_at
      };
    }

    return progressFromRow(row);
  }

  public upsertProgress(profileId: string, payload: ProgressPayload) {
    if (!this.profileExists(profileId)) {
      return null;
    }

    const current = this.getProgress(profileId);
    const actsCompleted = Array.isArray(payload.actsCompleted)
      ? payload.actsCompleted
      : current?.actsCompleted ?? [];
    const levelsCompleted = Array.isArray(payload.levelsCompleted)
      ? payload.levelsCompleted
      : current?.levelsCompleted ?? [];
    const narrativeState = payload.narrativeState != null && typeof payload.narrativeState === "object"
      ? payload.narrativeState
      : current?.narrativeState ?? {};
    const updatedAt = nowIso();

    this.db
      .prepare(
        `INSERT INTO progress
          (profile_id, acts_completed_json, levels_completed_json, narrative_state_json, updated_at)
         VALUES (?, ?, ?, ?, ?)
         ON CONFLICT(profile_id) DO UPDATE SET
          acts_completed_json = excluded.acts_completed_json,
          levels_completed_json = excluded.levels_completed_json,
          narrative_state_json = excluded.narrative_state_json,
          updated_at = excluded.updated_at`
      )
      .run(
        profileId,
        JSON.stringify(actsCompleted),
        JSON.stringify(levelsCompleted),
        JSON.stringify(narrativeState),
        updatedAt
      );

    return {
      profileId,
      actsCompleted,
      levelsCompleted,
      narrativeState,
      updatedAt
    };
  }

  public insertAttempt(payload: AttemptPayload) {
    if (!this.profileExists(payload.profileId)) {
      return null;
    }

    const createdAt = nowIso();
    const result = payload.result.trim();
    const details = payload.details ?? {};
    const insertResult = this.db
      .prepare(
        "INSERT INTO attempts (profile_id, act_id, result, details_json, created_at) VALUES (?, ?, ?, ?, ?)"
      )
      .run(payload.profileId, payload.actId, result, JSON.stringify(details), createdAt);

    return {
      id: Number(insertResult.lastInsertRowid),
      profileId: payload.profileId,
      actId: payload.actId,
      result,
      details,
      createdAt
    };
  }

  public insertHintLog(payload: HintLogPayload) {
    const profileId = this.resolveHintProfileId(payload.profileId);
    const createdAt = nowIso();
    const insertResult = this.db
      .prepare(
        "INSERT INTO hint_logs (profile_id, act_id, payload_json, created_at) VALUES (?, ?, ?, ?)"
      )
      .run(profileId, payload.actId, JSON.stringify(payload.payload ?? {}), createdAt);

    return {
      id: Number(insertResult.lastInsertRowid),
      profileId,
      actId: payload.actId,
      payload: payload.payload ?? {},
      createdAt
    };
  }

  public getHintLogCount(): number {
    const row = this.db
      .prepare("SELECT COUNT(*) AS count FROM hint_logs")
      .get() as { count: number };
    return row.count;
  }

  public getLatestHintLogPayload(): Record<string, unknown> | null {
    const row = this.db
      .prepare("SELECT payload_json FROM hint_logs ORDER BY id DESC LIMIT 1")
      .get() as { payload_json: string } | undefined;

    if (row == null) {
      return null;
    }

    return parseJson<Record<string, unknown>>(row.payload_json, {});
  }

  private createSchema(): void {
    this.db.exec(`
      CREATE TABLE IF NOT EXISTS learning_content (
        act_id TEXT PRIMARY KEY,
        title TEXT NOT NULL,
        concept TEXT NOT NULL,
        learning_objective TEXT NOT NULL,
        metadata_json TEXT NOT NULL
      );

      CREATE TABLE IF NOT EXISTS puzzles (
        id TEXT PRIMARY KEY,
        act_id TEXT NOT NULL,
        level TEXT NOT NULL,
        title TEXT NOT NULL,
        content_json TEXT NOT NULL,
        answer_key_json TEXT NOT NULL,
        created_at TEXT NOT NULL,
        FOREIGN KEY (act_id) REFERENCES learning_content(act_id)
      );

      CREATE TABLE IF NOT EXISTS profiles (
        id TEXT PRIMARY KEY,
        created_at TEXT NOT NULL
      );

      CREATE TABLE IF NOT EXISTS progress (
        profile_id TEXT PRIMARY KEY,
        acts_completed_json TEXT NOT NULL,
        levels_completed_json TEXT NOT NULL,
        narrative_state_json TEXT NOT NULL,
        updated_at TEXT NOT NULL,
        FOREIGN KEY (profile_id) REFERENCES profiles(id) ON DELETE CASCADE
      );

      CREATE TABLE IF NOT EXISTS attempts (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        profile_id TEXT NOT NULL,
        act_id TEXT NOT NULL,
        result TEXT NOT NULL,
        details_json TEXT NOT NULL,
        created_at TEXT NOT NULL,
        FOREIGN KEY (profile_id) REFERENCES profiles(id) ON DELETE CASCADE
      );

      CREATE TABLE IF NOT EXISTS hint_logs (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        profile_id TEXT NOT NULL,
        act_id TEXT NOT NULL,
        payload_json TEXT NOT NULL,
        created_at TEXT NOT NULL,
        FOREIGN KEY (profile_id) REFERENCES profiles(id) ON DELETE CASCADE
      );
    `);
  }

  private seedIfEmpty(): void {
    const contentCount = this.db
      .prepare("SELECT COUNT(*) AS count FROM learning_content")
      .get() as { count: number };
    if (contentCount.count === 0) {
      const insertContent = this.db.prepare(
        `INSERT INTO learning_content
          (act_id, title, concept, learning_objective, metadata_json)
         VALUES (?, ?, ?, ?, ?)`
      );
      const insertMany = this.db.transaction(() => {
        for (const item of learningContent) {
          insertContent.run(
            item.actId,
            item.title,
            item.concept,
            item.learningObjective,
            JSON.stringify(item.metadata)
          );
        }
      });
      insertMany();
    }

    const puzzleCount = this.db
      .prepare("SELECT COUNT(*) AS count FROM puzzles")
      .get() as { count: number };
    if (puzzleCount.count === 0) {
      const createdAt = nowIso();
      const insertPuzzle = this.db.prepare(
        `INSERT INTO puzzles
          (id, act_id, level, title, content_json, answer_key_json, created_at)
         VALUES (?, ?, ?, ?, ?, ?, ?)`
      );
      const insertMany = this.db.transaction(() => {
        for (const puzzle of puzzles) {
          insertPuzzle.run(
            puzzle.id,
            puzzle.actId,
            puzzle.level,
            puzzle.title,
            JSON.stringify(puzzle.content),
            JSON.stringify(puzzle.answerKey),
            createdAt
          );
        }
      });
      insertMany();
    }
  }

  private resolveHintProfileId(profileId: string | undefined): string {
    if (typeof profileId === "string" && profileId.trim().length > 0 && this.profileExists(profileId.trim())) {
      return profileId.trim();
    }

    const anonymousProfileId = GhostDatabase.AnonymousHintProfileId;
    if (!this.profileExists(anonymousProfileId)) {
      this.db
        .prepare("INSERT INTO profiles (id, created_at) VALUES (?, ?)")
        .run(anonymousProfileId, nowIso());
    }

    return anonymousProfileId;
  }
}

export function createGhostDatabase(dbPath = process.env.GHOST_DB_PATH ?? path.join("data", "ghost.sqlite")) {
  return new GhostDatabase(dbPath);
}

function progressFromRow(row: ProgressRow) {
  return {
    profileId: row.profile_id,
    actsCompleted: parseJson<string[]>(row.acts_completed_json, []),
    levelsCompleted: parseJson<string[]>(row.levels_completed_json, []),
    narrativeState: parseJson<Record<string, unknown>>(row.narrative_state_json, {}),
    updatedAt: row.updated_at
  };
}

function parseJson<T>(value: string, fallback: T): T {
  try {
    return JSON.parse(value) as T;
  } catch {
    return fallback;
  }
}

function nowIso(): string {
  return new Date().toISOString();
}

function ensureDatabaseDirectory(dbPath: string): void {
  if (dbPath === ":memory:") {
    return;
  }

  const directory = path.dirname(dbPath);
  if (directory === "." || directory === "") {
    return;
  }

  fs.mkdirSync(directory, { recursive: true });
}
