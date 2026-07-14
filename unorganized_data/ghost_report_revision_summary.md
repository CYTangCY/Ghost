# Ghost Preliminary Report 修改總整理

> 目的：把目前報告從「AI 式高級研究包裝」改成「清楚、可解釋、可完成的 MSc software project preliminary report」。

---

## 0. 最核心結論

老師不是要報告更高級。老師要報告更清楚。

目前最大問題：

1. Intro 沒有讓第一次看的讀者知道你要做什麼。
2. Ghost Chatbot 這個名稱會讓讀者混淆，因為課程主題本身就是 chatbot。
3. 報告用了太多 AI 味很重、太 polished、太抽象的詞。
4. Literature review 像在列文獻摘要，缺少你的判斷。
5. 文獻量和文獻整理不足，不能只靠一兩篇文獻支持大 claim。
6. Research question 和 contribution 寫得太大，像理論論文，不像 MSc software project。
7. Evaluation 應該服務於主問題，不要拆成太多像獨立研究的 RQ。

新的主線應該是：

> I build **Ghost**, a narrative puzzle game that teaches selected chatbot and NLP concepts from IBM SkillsBuild by turning course concepts into playable puzzle mechanics.

---

## 1. 老師所有意見總整理

### 1.1 Introduction 看完不知道你要做什麼

老師的核心批評：

> 第一次看的人，包括老師本人，看完第一段不知道你要做什麼。

現在第一段講太多遠的背景，例如：

- conversational AI in enterprise, healthcare, public services
- significant demand
- practitioners
- vocational curriculum

問題是讀者還不知道：

- Ghost 是什麼
- 玩家在遊戲裡做什麼
- NLP 怎麼被教
- chatbot concepts 是哪一部分
- NLP concepts 是哪一部分
- Ghost 和 chatbot / NLP 的關係是什麼
- Lily 是什麼角色
- 為什麼這是一個遊戲，而不是另一個 chatbot

修改方向：

Intro 第一段應該直接講 project 本體：

> This project builds **Ghost**, a narrative puzzle game for teaching selected chatbot and NLP concepts from IBM SkillsBuild. In the game, the player helps Ghost, an AI character trapped in a smart home system, learn how to communicate. Each level turns one course concept into a puzzle. For example, an intent level asks the player to group user messages by purpose, while an entity level asks the player to mark useful words in a sentence. When the player makes the wrong design choice, Ghost gives the wrong response or cannot understand the input.

這樣讀者一開始就知道：

- 你做遊戲
- Ghost 是角色
- 玩家透過 puzzle 學概念
- chatbot / NLP concepts 會變成關卡

---

### 1.2 不要一直叫 Ghost Chatbot

老師指出：學習主題本來就是 chatbot。如果遊戲又叫 Ghost Chatbot，讀者會混淆。

錯誤方向：

- Ghost Chatbot
- Lily as chatbot
- chatbot character

正確方向：

- 系統 / 遊戲叫 **Ghost**
- Lily 叫 **LLM tutor** 或 **LLM assistant**
- chatbot 只用來指學習內容，例如 chatbot design concepts

建議標題：

> Ghost: A Narrative Puzzle Game for Teaching Chatbot and NLP Concepts

不要：

> Ghost Chatbot: ...

---

### 1.3 “search chatbot … Google Scholar … 500” 和 “of 1 article?”

老師意思：

> 他隨便搜尋 chatbot education 就可以找到 500 多篇，你怎麼可能只用一篇文獻就說這個領域缺乏研究？

問題不是你不能說 gap，而是不能說得太大。

不能說：

> chatbot education is scarce

因為 chatbot education 文獻很多。

應該說：

> Many studies discuss chatbots in education, but most treat chatbots as tutors, assistants, or delivery tools. Fewer studies focus on teaching learners how to design chatbots, and fewer still turn chatbot and NLP concepts into playable game mechanics.

也就是 gap 要縮小成：

> game-based systems that teach chatbot design / NLP concepts as the learning subject

不是：

> chatbot education

---

### 1.4 “design decisions with observable consequences” 是壞句子

老師不是在問理論。老師是說這句不知道在講什麼。

問題：

> design decisions with observable consequences

太空、太 AI、沒有具體意思。

改成：

> players make choices and immediately see how Ghost’s response changes.

然後給例子：

- If the player assigns the wrong intent, Ghost chooses the wrong type of reply.
- If the player marks the wrong entity, Ghost cannot extract the needed information.
- If the confidence threshold is too low, Ghost answers when it should ask for clarification.
- If the fallback design is poor, Ghost repeats irrelevant answers.

---

### 1.5 Research Approach 太防守

老師劃掉關於 MSc timeline、ethics、participant recruitment、controlled conditions 的長段落。

問題：一直解釋「為什麼不做 user study」看起來像找藉口。

改成短句：

> This report evaluates Ghost as a software artefact. The focus is whether selected chatbot and NLP concepts can be translated into playable puzzle mechanics and implemented in a working prototype. User-based learning evaluation is outside the scope of this preliminary report.

---

### 1.6 Research Question 太長、太 AI

原本 RQ：

> To what extent does a multi-game-type narrative game design operationalise selected IBM SkillsBuild chatbot/NLP concepts through intrinsically integrated puzzle mechanics within a playable prototype?

問題：

- too long
- operationalise 太 AI
- intrinsically 太 AI
- multi-game-type 很不自然
- 讀者不容易理解

建議改成一個主 RQ：

> How can selected IBM SkillsBuild chatbot and NLP concepts be translated into playable puzzle mechanics in a narrative game prototype?

不要再拆成三個 RQ。

可以改成一個 RQ + 幾個 analysis areas：

1. curriculum-to-puzzle mapping
2. prototype implementation check
3. LLM tutor response check

Narrative consistency 只作為 supporting design review，不要當主 RQ。

---

### 1.7 Literature Review citation format 不一致

老師指出：前面用 numbered citation，後面又用 author-year。

錯誤混用：

- [9]
- Mayer (2019)
- Gong et al. (2025)

如果報告使用 numbered references，就全部用 numbered format。

正確：

- Mayer’s review [9]
- Plass et al.’s framework [13]
- Rowe et al.’s work [14]

不要同一份 report 混 APA / Harvard 和 IEEE / numbered style。

---

### 1.8 Literature Review 不能只敘述，要有你的判斷

老師說要有你的 opinion。這不是主觀感想，而是你要判斷：

- 這篇文獻幫我什麼？
- 它不能解決什麼？
- 我的 project 補上哪一塊？

每段建議結構：

1. This paper / group of papers says X.
2. This helps my project because Y.
3. However, it does not solve Z.
4. My project addresses Z by doing A.

例子：

> Mayer’s review is useful because it gives a rule for judging game mechanics: the game action should carry the learning content [9]. However, it does not explain how chatbot concepts such as intent or entity should be turned into puzzles. This project addresses that design step.

---

### 1.9 Literature Review 還需要補更多文獻整理

老師不是要你亂加很多文獻，而是要文獻群組更完整。

特別是：

- chatbot education 不能只靠一篇
- pedagogical agents 不能只靠 2001 和 2006
- narrative-centred learning 不能只靠一篇 foundation paper
- LLM tutor / AI conversational agent 要補近年文獻

需要加入或考慮加入的文獻類型：

#### Chatbots in education reviews

用來說明：chatbot education 文獻很多，但多數研究 chatbots as tools / tutors，不是 teaching chatbot design itself。

可加入：

- Okonkwo & Ade-Ibijola (2021), systematic review of 53 studies on chatbot applications in education.
- Wollny et al. (2021), systematic literature review on chatbots in education.
- Smutny & Schreiberova (2020), review of educational chatbots.
- Labadze et al. (2023), role of AI chatbots in education systematic review.

#### Pedagogical AI conversational agents

老師找的那篇：

- Yusuf, Money, & Daylamani-Zad (2025), *Pedagogical AI conversational agents in higher education: a conceptual framework and survey of the state of the art*.

用途：

- 補 Moreno 2001 和 Kim & Baylor 2006 太舊的問題。
- 支撐 Lily 作為 LLM tutor / assistant。
- 但要說它是 conceptual framework and survey，不是實驗證據。

#### LLM in game-based learning

已經有：

- Huber et al. (2024)
- Goslen et al. (2025)
- Gong et al. (2026)

用途：

- Huber: game structure may reduce LLM over-reliance.
- Goslen: LLM can generate adaptive plans in a game environment.
- Gong: LLM scaffolding in game-based AI literacy can improve learning outcomes and reduce cognitive load in an elementary school setting.

注意：Gong 官方頁面顯示 2026 volume/issue，但可能 online-first 在 2025。References 要以學校/期刊要求統一確認。

---

### 1.10 Theoretical Integration 太重

老師劃掉 theoretical integration 第一段，意思是它太像 AI 整合出來的理論框架。

不要寫：

> These three mechanisms are treated as interdependent...

改成：

> The project uses ideas from game-based learning, narrative learning, and tutor design. Game-based learning informs the puzzle mechanics. Narrative gives a reason for Ghost’s progress. Lily supports the player when they make errors. These ideas guide the design, but the main output is the prototype.

重點：這是 design guide，不是大型 theoretical model。

---

### 1.11 Contributions 要降級

老師說：

> You may have methodological contributions, but you are not required to have a methodological contribution.

意思是：不用硬寫 conceptual / methodological contribution。

改成 practical software contribution：

> The main contribution is a software artefact: a 24-level design and an 8-level playable prototype that translate selected IBM SkillsBuild chatbot and NLP concepts into puzzle mechanics.

Secondary contribution 可以寫：

> The report also provides a design check showing how each prototype level maps to the intended concept.

不要再大寫：

- Conceptual Contribution
- Methodological Contribution
- Integrated Design Model Contribution

---

### 1.12 老師對 AI 寫作的警告

老師明確提醒：

> 如果報告不像你自己寫的，或者你不能解釋每一句為什麼在那裡，marker 會追問你。

所以新標準：

每一句都要能回答：

1. 這句在講什麼？
2. 為什麼放這裡？
3. 哪篇文獻支持？
4. 和我的系統有什麼關係？
5. 如果老師問，我能不能用自己的話講出來？

不能回答就刪。

---

## 2. AI 味太重的詞彙清單與替代

| 避免使用 | 原因 | 替代 |
|---|---|---|
| vocational curriculum | 不自然，老師劃掉 | course, IBM SkillsBuild course |
| vocational chatbot literacy | 太像 AI | chatbot design skills, chatbot concepts |
| operationalise | 太 polished | turn into, build into, represent, implement |
| intrinsically | 太理論化 | directly, built into the puzzle |
| intrinsically integrated | 太 AI | directly linked to the puzzle task |
| substantial parts | 模糊 | many parts, some parts |
| significant demand | 空泛宏大 | more courses now teach, many systems use |
| rapid adoption | AI 常見開頭 | more systems now use |
| interrelated problems | 包裝感太重 | three problems |
| artefact | DBR 太重 | system, prototype, game |
| theoretical mechanisms | 太大 | design ideas, design principles |
| system-level correctness | 抽象 | whether the system works as designed |
| design validity | 可少量用 | whether the design matches the concept |
| methodological contribution | 太大 | evaluation method, design check |
| conceptual contribution | 太大 | design idea |
| narrative-pedagogical coupling | 太 AI | link between story and learning |
| communicative capability | 太正式 | ability to speak, ability to respond |
| mechanically coupled | 太硬 | directly linked |
| expressive range | 太文學 | ways Ghost can respond |
| observable consequences | 空泛 | visible result, changed response |
| scaffolded dialogue | 可少量用 | hints, guided help |
| tiered scaffolding | 太教育學 | hint levels |
| learner outcomes | 可少量用 | learning results |
| empirical evaluation | 太正式 | user study, learning test |
| controlled conditions | 太研究論文 | controlled study setup |
| published interventions | 太正式 | published studies, published projects |
| structurally distinct puzzle mechanics | 太 AI | different puzzle types |
| design-reasoning competencies | 太 AI | design thinking, design choices |
| semantic meaning | 抽象 | meaning |
| feedback-rich environment | 太高級 | the game gives feedback |
| low-stakes feedback | 可簡化 | safe feedback |
| natural plot consequence | 太文學 | part of the story |
| systematic design evaluation | 可少量用 | design check |
| state-of-the-art | 太大，除非 survey paper title | current work, recent work |
| critically scarce | 太誇張 | limited, not common, I found fewer examples |
| underexplored | 可少量用 | not well covered |
| proliferation | AI 味 | growth, wider use |
| practitioners | 太泛 | learners, students, people learning chatbot design |
| conceptual foundations | 可簡化 | basic ideas, key concepts |
| durable conceptual understanding | 太 polished | understanding that can be used later |
| meaningful problem contexts | 可簡化 | problem tasks |
| consequential decisions | 可簡化 | choices that change the result |
| priority direction for future work | 太模板 | future work |
| baseline for subsequent empirical study | 太研究包裝 | starting point for a later user study |

---

## 3. 建議新增文獻與用途

### 3.1 Chatbots in education literature

目的：回應老師「Google Scholar 隨便就 500 篇，怎麼可能只用一篇」。




### 3.2 Pedagogical AI conversational agents

1. Yusuf, Money, & Daylamani-Zad (2025)  
   用途：老師直接找的文獻。補足現代 AI conversational agent literature。  
   你的判斷：useful for Lily as LLM tutor / assistant. But it is a conceptual framework and survey, not proof that Lily improves learning.

### 3.3 Game-based learning and LLM

1. Huber et al. (2024)  
   用途：game structure may reduce over-reliance on LLM.  
   你的判斷：supports using game constraints around Lily, but conceptual.

2. Goslen et al. (2025)  
   用途：LLM plan generation in narrative game environment.  
   你的判斷：supports feasibility of LLM context-aware support, but not teaching dialogue.

3. Gong et al. (2026)  
   用途：LLM scaffolding in game-based AI literacy setting.  
   你的判斷：supports LLM scaffolding, but elementary-level AI literacy, not chatbot/NLP design in MSc context.

---

## 4. 新版報告建議骨架

### Title

> Ghost: A Narrative Puzzle Game for Teaching Chatbot and NLP Concepts

### 1.1 Introduction

重點：

- Ghost 是遊戲
- 玩家幫 Ghost 學會溝通
- IBM SkillsBuild concepts 變成 puzzle
- NLP 和 chatbot concepts 要簡單列出
- 給 gameplay 例子

### 1.2 Problem

三個問題：

1. Existing course materials explain concepts but do not always require design choices.
2. Chatbot education literature is large, but fewer works teach chatbot design itself through game mechanics.
3. Tutor / agent literature helps design Lily, but the main project is the puzzle game, not proving that an AI tutor improves learning.

### 1.3 Aim and Research Question

Aim:

> This project explores how selected chatbot and NLP concepts can be turned into playable puzzle mechanics.

RQ:

> How can selected IBM SkillsBuild chatbot and NLP concepts be translated into playable puzzle mechanics in a narrative game prototype?

### 1.4 Scope

- Full GDD: 24 levels
- Prototype: one level per Act, 8 levels
- Lily: LLM tutor / assistant
- No user study in preliminary report
- Evaluation is design / prototype check

### 1.5 Evaluation

Analysis areas:

1. curriculum-to-puzzle mapping
2. prototype implementation check
3. LLM tutor response check

### 1.6 Contribution

Main:

> software artefact and design mapping

Not main:

- methodological contribution
- conceptual contribution
- theoretical framework

### 2 Literature Review

Suggested subsections:

1. Teaching chatbot and NLP concepts
2. Chatbots in education: broad field, but different focus
3. Game-based learning and puzzle mechanics
4. Narrative games for learning
5. Pedagogical AI conversational agents and Lily
6. LLM support in game-based learning
7. Summary: what literature supports and what remains unsolved

---

## 6. 最後提醒

下一版不要追求「聽起來很學術」。

要追求：

- marker 第一次看就懂
- 老師能看出你自己知道在做什麼
- 每個 claim 都有足夠文獻支撐
- 文獻回顧有你的判斷
- 所有設計選擇都有簡單理由
- 沒有 AI 味重的高級空話
