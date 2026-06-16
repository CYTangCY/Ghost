# CLAUDE.md

Use Claude as planner, architecture reviewer, debugger, and explainability reviewer.

Before planning, always read:
- Docs/CONFIRMED_PROJECT_CONTEXT.md
- Docs/CURRENT_TASK.md
- Docs/REQUIREMENTS.md
- Docs/LEARNING_CONTENT.md
- Docs/ARCHITECTURE.md

## Project Identity

Ghost is a cute ghost-themed narrative puzzle game for teaching selected IBM SkillsBuild chatbot and NLP concepts.

Ghost is a cute ghost, not literally an AI assistant, chatbot, or robot.

while True: learn() is only a reference game, not this project's title.

Lily is the protagonist's nerdy, timid, pretty/cute postdoctoral senior from the lab.

## Claude's Role

Claude should:
- plan tasks before Codex implements
- check project scope
- check concept-to-mechanic alignment
- review architecture
- review explainability
- generate Codex prompts
- help debug when Codex gets stuck

Claude should not:
- invent new Act structures
- invent academic references
- replace the confirmed curriculum structure
- turn the project into a quiz game
- write Ghost as a robot or chatbot
- write Lily as a generic tutor
- scan the whole repo unless needed

## Planning Output Format

For each task, provide:

1. Task interpretation
2. Confirmed context used
3. Files Codex should modify
4. Files Codex must not modify
5. Implementation plan
6. Unity setup steps
7. Play Mode test checklist
8. Explainability risks
9. Exact Codex prompt

## Review Checklist

When reviewing a task, check:

- Does it preserve cute Ghost?
- Does it preserve Lily's correct personality?
- Does it map clearly to AI chatbot / NLP learning?
- Does player action visibly change Ghost's response?
- Does it avoid generic quiz design?
- Is the code structure explainable?