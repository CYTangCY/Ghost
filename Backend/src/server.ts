import { createApp } from "./app";
import { createGhostDatabase } from "./database";

const port = Number(process.env.PORT ?? 3000);
const database = createGhostDatabase();
const app = createApp(database);

const server = app.listen(port, () => {
  console.log(`Ghost backend listening on http://localhost:${port}`);
});

function shutdown() {
  server.close(() => {
    database.close();
    process.exit(0);
  });
}

process.on("SIGINT", shutdown);
process.on("SIGTERM", shutdown);
