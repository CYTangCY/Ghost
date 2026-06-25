import { createOllamaConfigFromEnv, listOllamaModels, OllamaClient } from "./ollamaClient";

async function main() {
  const config = createOllamaConfigFromEnv();

  console.log(`Checking Ollama at ${config.baseUrl}`);
  console.log(`Required model: ${config.model}`);
  console.log(`Tags timeout: ${config.checkTimeoutMs} ms`);
  console.log(`Generation timeout: ${config.generateTimeoutMs} ms`);

  let modelWasFound = false;
  try {
    const models = await listOllamaModels(config);
    if (!models.includes(config.model)) {
      console.log(`Ollama is reachable, but model '${config.model}' was not found.`);
      console.log("Available models:");
      if (models.length === 0) {
        console.log("  (none reported)");
      } else {
        for (const model of models) {
          console.log(`  - ${model}`);
        }
      }
      console.log(`Run: ollama pull ${config.model}`);
      process.exitCode = 1;
      return;
    }

    modelWasFound = true;
    console.log(`OK: model '${config.model}' is available.`);
    console.log("Running a timed test generation...");

    const startedAt = Date.now();
    const generated = await new OllamaClient(config).generateText({
      system: "You are checking a local Ghost development setup. Reply with one short plain sentence.",
      prompt: "Say that Ollama generation is working."
    });
    const durationMs = Date.now() - startedAt;

    console.log(`OK: test generation succeeded in ${durationMs} ms.`);
    console.log(`Sample: ${generated.text}`);
  } catch (error) {
    console.log("Ollama check failed.");
    console.log(error instanceof Error ? error.message : String(error));
    if (modelWasFound) {
      console.log("The model was found, but generation did not complete.");
      console.log("If this was a cold start, try again or increase OLLAMA_TIMEOUT_MS.");
    } else {
      console.log("Make sure Ollama is installed and running, then run:");
      console.log(`ollama pull ${config.model}`);
    }
    process.exitCode = 1;
  }
}

main();
