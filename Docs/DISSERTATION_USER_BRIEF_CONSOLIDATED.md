# Consolidated Dissertation User Brief

## Purpose

This file collects the user's report instructions from the dissertation turns. It is the review brief for Claude. The current report is about `Ghost`, not a general AI education system.

## Main Writing Request

1. Complete the literature review, results, and evaluation sections.
2. Give the literature review first priority.
3. Use simple, natural academic English near IELTS 6.5 to 7.
4. Avoid rare substitutes, inflated academic wording, and stock AI-style phrases.
5. Keep technical terms only where they are needed. Explain them in plain English at first use.
6. Do not make the prose sound as if it was written to impress through difficult vocabulary.
7. Keep the report critical. Do not only list what each paper says.
8. Compare papers in the same paragraph, explain agreement or conflict, state the project's view, and explain why the final design follows that view.
9. Do not invent papers, study results, project results, quotations, or source details.
10. Use the student's own project evidence and keep claims within that evidence.

## Literature Review Request

The review should use three connected arguments. Each argument should use at least four or five relevant papers that support, limit, or conflict with one another. Recent 2025 and 2026 work should be included where it is real and relevant. Older work may remain when it supplies a basic theory.

### Argument 1: The learner should use the concept

Compare game-based learning, cognitive load, narrative learning, active AI education, and recent game-supported AI learning. The report should explain why `Ghost` uses a short explanation followed by a puzzle action and a visible result.

### Argument 2: Educational chatbots are usually tools, while chatbot-design teaching is growing

Avoid the false claim that educational chatbot research is scarce. Compare broad educational-chatbot reviews, recent tutor or teaching-assistant systems, and newer work in which learners or teachers design chatbots. Keep the gap narrow: fewer examples teach chatbot and NLP parts through connected narrative puzzle actions.

### Argument 3: An LLM tutor needs limits

Compare positive results from course-specific tutors with studies showing weaker later independent work when learners can obtain answers too easily. Explain why Lily gives optional short hints, stays outside scoring, should not reveal placements, and has a static fallback.

## Design and Tool Comparison Request

The report must compare realistic choices before stating the final decision. The discussion must include strengths, limits, scale, setup cost, fit to the project, and why a larger or heavier option was not needed.

Required comparisons:

- frontend: Unity 6 against Godot 4, Phaser, and Unreal Engine;
- backend: Node.js and TypeScript against Python with FastAPI, ASP.NET Core, and serverless functions;
- database: SQLite against PostgreSQL, MongoDB, and JSON files;
- model path: local IBM Granite through Ollama against hosted commercial APIs, direct Hugging Face serving, and static written hints;
- correctness: fixed validators and simulators against model-based scoring.

The final choices must be tied to this project's actual needs: C#, WebGL, 2D drag and graph interactions, a small REST service, local evaluation, relational progress data, an IBM-linked local model, graceful fallback, and repeatable scoring.

## Results and Evaluation Request

1. Separate method, result, and interpretation.
2. Report real counts only.
3. Compare the finished system with the expectations from the literature and with rejected design choices.
4. State clearly which aims and requirements were met.
5. Treat automated software checks as evidence of software behaviour, not evidence of learning gain.
6. Treat the Granite prompt-bank review as an internal developer check, not a validated education measure.
7. State the weak Lily score and answer-revealing outputs openly.
8. State that no participant study, pre-test, post-test, transfer test, later-recall test, or comparison group was run.
9. Do not claim that `Ghost` improves learning, enjoyment, usability, or accessibility.
10. Keep the human Play Mode result separate from automated evidence unless an observer record and screenshots exist.

## Format Request

1. Use the supplied KCL cover material, chapter instructions, and LaTeX template as reference inputs.
2. Write and maintain the report in LaTeX.
3. Follow the supplied chapter order: cover, abstract, acknowledgements, contents, nomenclature, lists, introduction, literature and theory, design, implementation, results and evaluation, professional issues, conclusion, references, and appendices.
4. Keep the main report and appendices in one document.
5. Number and refer to every figure and table.
6. Use consistent references and BibTeX.
7. Avoid contractions.
8. Show measured floating values to four decimal places where the supplied guidance requires it.
9. Add real figures when available. Otherwise use clearly labelled placeholders that the student can replace with screenshots.
10. Preserve the supplied files inside `Docs/dissertation_review_sources/` so Claude can review the same inputs.

## Mark Request

The user challenged an earlier high estimate and asked why the work deserved it. The review must use the supplied February 2025 `7CCSMPRJ` rubric and grade only the current evidence.

Claude must provide:

- a score for each weighted report area;
- the weighted total and KCL band;
- a realistic current range, not one optimistic number;
- a separate conditional range after named missing work is completed;
- reasons the report could reach that band;
- reasons it cannot yet receive a higher band; and
- the highest-value fixes before submission.

The report areas are Introduction 10%, Literature Review 10%, Specification and Design 15%, Implementation and Technical Achievement 25%, Evaluation 20%, General Scholarship 10%, and Legal, Social, Ethical and Professional Issues 10%.

## Style Acceptance Rules

The automated plain-English check is a guard, not a writing score. Claude should still read the prose for:

- unnatural transitions;
- repeated sentence shapes;
- vague claims;
- unnecessary passive voice;
- wording that sounds generic or machine-made;
- technical words that lack a plain explanation;
- paragraphs that list sources without comparing them; and
- overly simple prose that removes needed critical detail.

The current automated target is an average sentence length of no more than 22 words, no sentence over 32 words, and zero matches from the project's AI-style and uncommon-word lists. These rules do not prove IELTS level and must not be described as an official IELTS score.

## Source of Truth

Use current repository files, test logs, raw evidence, and supplied source files. Chat history is not evidence. Where the earlier report conflicts with current project documents, use `Docs/CONFIRMED_PROJECT_CONTEXT.md`, `Docs/LEARNING_CONTENT.md`, `Docs/ARCHITECTURE.md`, and the current implementation.