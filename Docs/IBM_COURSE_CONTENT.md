# IBM Course Content Coverage Map

Date: 2026-06-25

Source:
- `unorganized_data/Course_IBM_chatbot.pdf`

Method:
- The PDF is image-based. It was rendered page-by-page to PNG for inspection.
- Page citations below refer to the rendered PDF page numbers.
- This document maps what the IBM course teaches against what Ghost currently teaches in-game.

Important framing:
- The goal is not to dump the course into dialogue or quizzes.
- The game should teach the IBM course content through playable Ghost problems, Lily's in-fiction guidance, player action, and visible Ghost consequences.
- Current Acts 1-3 often let the player practice a concept, but the concept is not always introduced or explained clearly in-game yet.

---

## 1. Course Teaching Outline

### 1.1 NLP and machine learning as pillars of AI-powered chatbots

Pages: pp. 1-2, p. 38

The course teaches that NLP and machine learning are the two main pillars of AI-powered chatbots.

Key teaching points:
- NLP is a subfield of AI focused on interaction between computers and humans through everyday human language.
- NLP helps chatbots understand context, language nuance, keywords, and the user's intent.
- Machine learning lets computers learn from data, recognize patterns, and improve responses over time.
- Machine learning supports personalization, predictive assistance, and proactive assistance.

### 1.2 Chatbot challenges

Pages: p. 3, p. 39

The course teaches that chatbots have common design and use challenges.

Key teaching points:
- Handling unstructured or unexpected input, such as slang, misspellings, or formats the bot was not trained to handle.
- Misunderstanding user queries because of ambiguous language or complex phrasing.
- Providing human-like interaction, because responses can otherwise feel robotic, scripted, or not warm/flexible.
- Contextual awareness, because bots may lose track of previous user questions or details across a conversation.

### 1.3 Components of a chatbot

Pages: pp. 5-6, p. 39

The course teaches that a chatbot contains several interacting components that work together to simulate conversation.

Key teaching points:
- User interface (UI): facilitates interaction between user and chatbot; lets the user input a query and displays the response.
- NLP engine: interprets and processes the user's input; processes intent and extracts relevant information.
- Dialogue management system: receives processed input and decides the next action or appropriate response based on predefined rules or learning models.
- Response generation module: formulates or generates a suitable reply.
- Backend integrations: fetch additional data from external systems or databases when needed.

### 1.4 Rule-based versus AI-enabled chatbots

Pages: pp. 7-14, p. 38, pp. 42-43

The course teaches that there are two main chatbot types: rule-based and AI-enabled.

Rule-based teaching points:
- Rule-based chatbots are also described as decision-tree bots.
- They use predefined rules and present choices like a flow chart.
- They can handle common issues but lack flexibility for complex or unusual situations.
- Pros include being quick to build, easy to integrate, secure, professional for FAQs, and scalable for simple automation.
- Cons include limited conversation, no learning from user interactions, more robotic interaction, and the need for regular updates.
- Good use cases include FAQs, appointment scheduling, surveys/feedback, navigation assistance, simple transactions, banking chatbots, airline FAQs, and hotels.

AI-enabled teaching points:
- AI-enabled chatbots use NLP and machine learning to understand customer queries and provide solutions.
- They can understand context, ask relevant follow-up questions, provide tailored advice, and personalize support.
- Pros include learning from interactions, multilingual understanding, improving over time, context awareness, and more accurate decisions.
- Cons include needing proper training, time-consuming error correction, and higher cost.
- Good use cases include advanced customer support, personalized recommendations, multilingual support, proactive engagement, complex problem solving, and human-like conversations.

The course assessment reinforces the difference by asking learners to identify when a bot shows learning, personalization, or adaptation (pp. 42-43).

### 1.5 Chatbot platforms and frameworks

Pages: p. 15

The course teaches that chatbot development platforms/frameworks provide pre-built modules and templates that reduce development time and complexity.

Examples named in the course:
- Dialogflow.
- Microsoft's Bot Framework.
- IBM watsonx Assistant.
- Open-source frameworks such as Rasa.

The course also teaches that the right platform depends on requirements and the development team's technical expertise.

### 1.6 NLP definition, technologies, and subtasks

Pages: pp. 16-19

The course teaches NLP as the layer that helps computers understand human language and lets virtual assistants respond smoothly and accurately.

Key technologies:
- Speech recognition: converts spoken words into text so the assistant can process voice input.
- Natural language generation (NLG): transforms data into human-like text for replies.
- Natural language understanding (NLU): helps the assistant comprehend meaning behind words and handle complex queries.

Main tasks/subtasks:
- Tokenization: breaks text into words or phrases known as tokens. The course names sentence segmentation and word segmentation as subtasks.
- Part-of-speech tagging: identifies grammatical parts of speech for tokens. The course names lexical analysis and syntactic analysis as subtasks.
- Named entity recognition (NER): identifies and classifies key information/entities in text into predefined categories, such as people, organizations, locations, dates, and other important terms.
- Sentiment analysis: determines the emotional tone behind a user's message, helps the assistant respond appropriately, and can support routing/escalation.
- Machine translation: translates text between languages. The course names rule-based, statistical, and neural machine translation.

### 1.7 Setting up IBM watsonx Assistant

Pages: pp. 20-24

The course teaches a practical product setup flow for IBM watsonx Assistant.

Key teaching points:
- Learners need an IBM Cloud account.
- From the IBM Cloud dashboard, create a resource.
- Search for watsonx Assistant in the catalog.
- Select location and pricing plan, agree to license terms, create the service, and launch watsonx Assistant.

### 1.8 Planning a chatbot

Pages: pp. 25-28, p. 44

The course teaches that chatbot development starts with planning before building.

Key teaching points:
- Define the chatbot goal. In the Nash restaurant activity, the goal is to handle simple online client queries, especially menu questions, and take orders.
- Choose the starting channel based on where users actually interact. The activity uses data showing website, app, and direct-call usage.
- Identify key topics from common user questions and prioritize what the chatbot should handle first.
- Plan for handoff when the chatbot cannot handle a request, including recognizing complex queries and transferring users to a live agent.

### 1.9 Creating and configuring a chatbot in watsonx Assistant

Pages: pp. 29-36

The course teaches a basic assistant creation workflow in IBM watsonx Assistant.

Key teaching points:
- The welcome screen has Create, Personalize, Customize, and Preview tabs.
- Create: name the assistant.
- Personalize: choose deployment context, industry, team role, and development goal.
- Customize: adjust the chat UI, avatar, theme, colors, and related presentation options.
- Preview: view the assistant UI, generate a link, and confirm before creation.
- The course summary states that the learner has created the basic structure of a first chatbot and can inspect its architecture.

### 1.10 Previewing and testing a chatbot

Pages: p. 37, p. 39

The course teaches that after creating the chatbot, the learner should preview and test it.

Key teaching points:
- Preview/testing verifies that interactions are smooth and accurate.
- The learner can make changes as required.
- The learning objectives include being able to build a conversational chatbot that offers options and suggestions, and use pre-built/custom conversations to generate chatbot dialog.

### 1.11 Assessment and scenario reasoning

Pages: pp. 4, 13-14, 40-44

The course includes short scenario questions that reinforce:
- What NLP helps chatbots do.
- Whether a scenario is rule-based or AI-enabled.
- Selecting suitable chatbot tasks from a goal.
- Recognizing machine-learning behavior such as personalization or recommendations from earlier data.
- Choosing domains to focus on from customer inquiry data.
- Identifying chatbot components.

---

## 2. Coverage Map Table

Status meanings:
- taught: introduced/explained in-game and practiced through a mechanic.
- partial: either explained without practice, practiced without explanation, or present only as technical infrastructure rather than learning content.
- missing: not currently taught in-game.

| IBM course teaching point | Course pages | Where Ghost currently addresses it | Introduced/explained in-game? | Practiced via mechanic? | Status |
|---|---:|---|---|---|---|
| Chatbot definition: a computer program that simulates conversation with humans | p. 38 | Ghost's premise implies communication repair, but the game does not clearly define chatbot yet. | N | N | missing |
| Chatbot benefits: efficiency, repetitive tasks, customer-service usefulness | p. 38 | Backend/progress/logging exists technically; benefits are not taught to the player. | N | N | missing |
| NLP and ML are pillars of AI-powered chatbots | pp. 1-2, p. 38 | Acts 1-2 practice selected NLP ideas; Lily chat/LLM exists technically. The pillar framing is not taught. | N | Y | partial |
| NLP: understand context, nuance, keywords, and intent | pp. 1, 16, 38 | Act 1 practices intent grouping; Act 2 practices key-detail extraction. | N | Y | partial |
| Machine learning: learn from data and improve responses | pp. 2, 38 | Lily chat uses an LLM backend, but the player does not learn ML through a mechanic. | N | N | missing |
| Rule-based vs AI-enabled chatbot distinction | pp. 7-14, p. 38 | Deterministic validators and Lily LLM support create a technical contrast, but the game does not teach it. | N | N | missing |
| Rule-based/decision-tree chatbot flow | pp. 8-10 | Act 3 Reply Map practices a flow/graph. It is not framed as a rule-based/decision-tree bot. | N | Y | partial |
| AI-enabled chatbot capabilities: context, follow-up, tailored advice, adaptation | pp. 11-12, 42-43 | Lily chat can demonstrate natural-language support, but it is not a teaching mechanic about AI-enabled bots. | N | N | missing |
| Pros/cons and use cases of rule-based chatbots | pp. 9-10 | No explicit in-game explanation or comparison. | N | N | missing |
| Pros/cons and use cases of AI-enabled chatbots | pp. 11-12 | No explicit in-game explanation or comparison. | N | N | missing |
| Five chatbot components as a system | pp. 5-6, p. 39 | Act 8 capstone is planned, but not implemented. | N | N | missing |
| User interface component | pp. 5-6, p. 39 | The player uses UI, but the UI is not taught as a chatbot component. | N | N | missing |
| NLP engine component: intent processing and information extraction | pp. 5-6 | Act 1 and Act 2 map strongly to this component. | N | Y | partial |
| Dialogue management system component | pp. 5-6 | Act 3 maps strongly to dialogue management through the Reply Map/node graph. | N | Y | partial |
| Response generation module component | pp. 5-6 | Act 3 response nodes and Ghost reactions partially map to this; `/responses` backend exists technically. | N | Y | partial |
| Backend integration component | pp. 5-6 | Backend/API/SQLite exist technically for progress, attempts, hints, and chat, but are not taught as a player concept. | N | N | missing |
| Challenge: unstructured or unexpected input | p. 3, p. 39 | Lily free-text chat receives open text, but the concept is not taught or turned into a puzzle consequence. | N | N | missing |
| Challenge: misunderstanding ambiguous or complex queries | p. 3, p. 39 | Act 1 and Act 2 failure states can show Ghost reacting to the wrong purpose/detail. | N | Y | partial |
| Challenge: contextual awareness | p. 3, p. 39 | Act 3's slot/context idea partially supports this, but it is not explained as context awareness. | N | Y | partial |
| Challenge: natural, human-like interaction | p. 3, p. 39 | Lily chat and Ghost response text support natural language, but the challenge is not taught through gameplay. | N | N | missing |
| Speech recognition | p. 16 | Not represented; current prototype is typed/visual. | N | N | missing |
| Natural language generation (NLG) | p. 16 | Ghost response generation and `/responses` exist, but the player does not learn NLG. | N | N | missing |
| Natural language understanding (NLU) | p. 16 | Acts 1-2 practice intent/entity aspects of NLU. | N | Y | partial |
| Tokenization | pp. 17-18 | Act 2 renders word chips, but tokenization is not explained as a concept. | N | Y | partial |
| Part-of-speech tagging | pp. 17-18 | Not represented. | N | N | missing |
| Named entity recognition (NER) | pp. 17, 19 | Act 2 practices entity spans, entity types, system/custom entities, and synonyms. | N | Y | partial |
| Sentiment analysis | pp. 17, 19 | Not represented. | N | N | missing |
| Machine translation | p. 18 | Not represented. | N | N | missing |
| Chatbot platforms/frameworks and choosing a platform | p. 15 | IBM Granite/Ollama/backend are used technically; platform choice is not taught in-game. | N | N | missing |
| IBM watsonx Assistant resource setup | pp. 20-24 | Not represented in gameplay. | N | N | missing |
| Planning: define the chatbot goal | p. 25 | Ghost levels have goals, but the player does not choose/design a chatbot goal. | N | N | missing |
| Planning: choose starting channel from usage data | p. 26 | Not represented. | N | N | missing |
| Planning: identify key topics from user queries | p. 27, p. 44 | Act 1 intent grouping resembles identifying user purposes/topics, but not explained as chatbot planning. | N | Y | partial |
| Planning: handoff to human support for complex cases | p. 28 | Not represented yet; likely future confidence/fallback/escalation content. | N | N | missing |
| Assistant creation workflow: Create/Personalize/Customize/Preview tabs | pp. 29-36 | No product workflow reproduction. | N | N | missing |
| Preview and test the chatbot, then revise | p. 37, p. 39 | Act 3's "Test Ghost's map" practices graph testing; broader chatbot testing is not explained yet. | N | Y | partial |
| Use pre-built/custom conversations to generate chatbot dialog | p. 39 | Act 3 lets the player build a custom conversation map; not linked to IBM wording yet. | N | Y | partial |

Headline reading of the map:
- Current Ghost is strongest where it lets the player practice intent classification, entity extraction/NER, dialogue management, response selection, and testing a small conversation map.
- Current Ghost is weakest where the course expects conceptual grounding: chatbot definition, rule-based vs AI-enabled chatbots, five components, four challenges, ML, platform/workflow framing, and the wider NLP pipeline.
- The most common current status is partial because mechanics exist but the in-game explanation does not yet teach the IBM term or why it matters.

---

## 3. Prioritized Gap List and Proposed Follow-Up Tasks

Priority principle:
1. Add fundamentals first, because the IBM course starts there and the user-corrected goal is that the game teaches the course content.
2. Strengthen Acts 1-3 so they explain the concept in-fiction as well as practice it.
3. Add breadth through Acts 4-7 and the Act 8 capstone.

### M0-T35 - Chatbot Fundamentals Teaching Pass

Goal:
Add a compact in-game fundamentals layer, likely in the Game Shell or an early "Ghost's voice is disconnected" sequence, teaching:
- what a chatbot is;
- NLP and ML as pillars;
- rule-based vs AI-enabled chatbots;
- benefits at a simple level;
- the five components;
- the four challenges.

Constraint:
Keep it playable and visual. Do not make it a lecture dump or pure quiz.

### M0-T36 - Strengthen Act 1 as Intent / Key Topic Teaching

Goal:
Keep the existing intent classification mechanic, but add in-fiction teaching so the player understands:
- intent means the user's purpose;
- different wording can share the same intent;
- intent grouping relates to identifying key topics/user requests in chatbot planning.

### M0-T37 - Strengthen Act 2 as Entity Extraction / NER Teaching

Goal:
Keep the existing span tagging mechanic, but add in-fiction teaching so the player understands:
- entity extraction / NER means finding important details in text;
- entities can include locations, dates/times, objects, people, and other important terms;
- system vs custom entities and synonyms matter;
- word chips lightly connect to tokenization.

### M0-T38 - Strengthen Act 3 as Dialogue Management / Rule-Based Flow Teaching

Goal:
Keep the current Reply Map mechanic, but teach:
- dialogue management decides the next response;
- a node graph is similar to a rule-based/decision-tree conversation flow;
- slot checks connect Act 2 entities to conversation flow;
- response nodes connect to response generation;
- testing the map is a preview/test step.

### M0-T39 - Chatbot Planning Mini-Level

Goal:
Add a short playable planning step for:
- defining a chatbot goal;
- choosing a starting channel from simple usage data;
- selecting key topics from common queries;
- planning a handoff path for complex requests.

This maps directly to the Nash planning activity in the course.

### M0-T40 - Act 4 Confidence, Fallback, and Handoff

Goal:
Extend the graph so the player handles uncertainty and failed understanding:
- fallback design;
- disambiguation;
- handoff/escalation for complex or unresolved cases;
- sentiment-based routing/escalation (negative or frustrated tone routes toward fallback/handoff).

This also helps teach chatbot challenges: misunderstanding, unstructured input, and context loss, and
introduces sentiment analysis (§1.6) as a routing signal — never as a correctness/scoring signal.

(Refinement, 2026-06-27, user-confirmed: sentiment is used here for routing/escalation, per §1.6; the
NLP-subtask treatment of sentiment is revisited in Act 7.)

### M0-T41 - Act 5 Testing and Debugging

Goal:
Turn preview/test/revise into gameplay:
- run multiple test conversations;
- inspect failed cases;
- fix graph/content issues;
- connect this to the course's preview/test workflow.

### M0-T42 - Act 6 Backend Integration and Response Generation

Goal:
Teach the backend integration and response generation components:
- when a bot needs external data;
- how backend action results affect replies;
- how a response generation module turns known data into a suitable reply.

The existing backend can support this technically, but the concept must be made playable.

### M0-T43 - Act 7 NLP Pipeline Lab

Goal:
Add a playable NLP pipeline lab covering the subtasks NOT already taught in earlier Acts:
- POS tagging;
- sentiment analysis (revisited as an NLP subtask, after its Act 4 routing use);
- machine translation (lightweight; may stay optional if time is short).

Tokenization and NER are taught in Act 2 (M0-T37), so Act 7 does not repeat them and focuses on the
remaining subtasks.

(Refinement, 2026-06-27, user-confirmed: dedup — tokenization/NER live in Act 2; Act 7 = POS / sentiment /
machine translation.)

### M0-T44 - Act 8 Five-Component Capstone

Goal:
Implement the capstone "Repair Ghost's Voice" pipeline:
- UI -> NLP engine -> dialogue management -> response generation -> UI;
- backend integration as a side connection when external data is needed.

This should make the full IBM component model visible as one playable system.

---

## 4. Reasonable Prototype Limitations / Future Work

Keep this section minimal; it is not an excuse list. It identifies topics that are present in the IBM course but are not ideal to fully reproduce in the playable prototype.

- Full IBM watsonx Assistant product UI walkthrough (pp. 20-24, 29-36): the game should not become a screen-by-screen clone of IBM Cloud/watsonx setup. It can teach the design concepts behind the workflow and cite the product walkthrough as contextual source material.
- Machine translation (p. 18): included in the course as an NLP subtask, but less central to the current Ghost chatbot-design loop than intent, entity, dialogue, fallback, testing, backend integration, and sentiment. It can be listed as future work or a small optional Act 7 extension.
- Speech recognition (p. 16): the current prototype is typed/visual and WebGL-focused. Speech recognition can be noted as an NLP technology but does not need to be implemented unless the project scope expands.
- Full platform comparison (p. 15): the course names platforms/frameworks, but the prototype can teach the high-level reason platforms exist without requiring players to compare real products in detail.

---

## 5. Design Implication

The current direction is valid, but the content coverage map shows that the next work should not be "more labels only."

The game needs a teaching pass where each concept has:
1. an in-fiction problem from Ghost;
2. Lily's short, characterful explanation;
3. a player action that practices the concept;
4. visible Ghost behaviour showing the consequence.

That is how Ghost can teach the IBM course content while staying a game rather than becoming a lecture or quiz dump.
