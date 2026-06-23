# NARRATIVE.md

> Canonical narrative for the Acts 1–3 vertical slice. Drafted 2026-06-23 (M0-T26). Story framing only —
> puzzle rules stay deterministic and unchanged. Lines below are drafts; refine wording during
> implementation. Lily's voice follows `CONFIRMED_PROJECT_CONTEXT.md` §3.2.

## Decisions (confirmed 2026-06-23)

- **Working title:** Ghost.
- **Setting:** a *lightly haunted research lab* — cozy and cute, not horror.
- **Protagonist:** a junior lab member (Lily's kōhai); the **player enters their own name** at start.
  Dialogue may address them by `{playerName}`.
- **Lily:** the protagonist's nerdy, timid-but-capable postdoctoral senior.
- **Ghost:** a cute ghost (not an AI); becomes clearer/cuter as the player helps it.
- **Tone:** warm, light, a little awkward-funny.

## Premise (short)

Late in the lab, a cute Ghost keeps trying to talk but it comes out garbled. Lily ropes you (a lab
junior) into helping Ghost learn to understand people — first *what they want*, then *the details*, then
*how to hold a conversation*. As you help, Ghost's "voice" gets clearer.

## Through-line (Acts 1 → 3)

- **Act 1 (Intent):** Ghost reacts to the wrong purpose → you teach it to read what people *want*.
- **Act 2 (Entity):** Ghost gets the gist but misses details → you teach it to notice the *specifics*.
- **Act 3 (Dialog graph):** Ghost has the pieces but replies out of order → you build it a conversation
  map so it asks when unsure and answers when it knows.

Ghost grows clearer/cuter each act; Lily grows from nervous to quietly proud.

## Shell flow

1. Title ("Ghost") → Start → **name entry** ("What should Ghost call you?") → hub.
2. Hub: a Lily intro line (uses `{playerName}` + the current goal); Act 1/2/3 cards. Suggested order
   Act 1 → 2 → 3 (gate or just recommend — keep simple).
3. Enter an act → short **Lily intro beat** (sets up Ghost's problem) → puzzle (unchanged) → on return
   to hub, a **debrief beat** (Lily + Ghost react to success).
4. A small **narrative state** tracks which acts are debriefed (for ordering + a short closing beat after
   Act 3).

## Per-act beats (draft; implementer puts these in data)

**Act 1 — Intent**
- Lily (intro): "Um, {playerName}... Ghost keeps answering the wrong thing — like it hears the words but
  not what people actually *want*. Could you help it sort messages by purpose?"
- Lily (debrief): "It worked — Ghost's reacting to what people *mean* now. That's... kind of amazing."

**Act 2 — Entity**
- Lily (intro): "Okay, Ghost gets the gist now... but it keeps missing the important details — which
  room, which thing. Maybe help it notice those?"
- Lily (debrief): "It's catching the details now. {playerName}, it's really starting to understand."

**Act 3 — Dialog graph**
- Lily (intro): "Last piece: Ghost knows what people want *and* the details, but it blurts things out of
  order. Could you build it a little conversation map — ask when it's unsure, answer when it knows?"
- Lily (debrief): "...It actually held a conversation. We did it, {playerName}."
- Ghost (clear, after Act 3): a clear, cute line — e.g. "Thank you." — its voice repaired.
- Closing beat: short warm wrap (sets up the later capstone idea; no new mechanics here).

## Implementation notes (for the Codex task)

- Reuse `LilyDialogueFrame`; extend `ShellDialogueData` to be **act-aware** (intro + debrief per act) and
  support a `{playerName}` token.
- Add a **name-entry step** on Start; hold the player name + narrative state (which acts debriefed) in a
  simple in-memory frontend narrative-state object (no save/load/backend yet — that is M0-T27/M0-T28).
- Beats are **data-driven** (not hardcoded per screen); **static text** (no LLM yet — that is M0-T29).
- **Character portrait frame:** the dialogue frame includes a sized portrait **Image slot** for the
  current speaker (Lily / Ghost), with serialized `Sprite` fields left empty (a labelled placeholder box
  for now) so character art can be dropped in later.
- Do NOT change puzzle logic/validators or act mechanics.

## In-Act Ambient Banter (M0-T32)

Use the spare space in each act scene for a small area where Ghost + Lily chat with the player in real
time while they solve the puzzle. Fixed per-act dialogue loops for now (as many as feasible, fun); later
extended with player-choice branches + LLM.

Design:
- A non-blocking ambient dialogue strip in each act scene's spare space; cycles lines (timer and/or a
  "next" tap) and loops. Reuses the speaker portrait placeholder.
- Data-driven: per act, a list of ambient beats `{ speaker (Lily/Ghost), text (may use {playerName}),
  optional tag }`. Structured so future player-choice options + LLM-sourced lines can slot in.
- Lily's arc: Act 1 nervous/formal → Act 2 warmer → Act 3 comfortable & joking. Recurring nerdy beat:
  Lily makes a nerdy joke/pun, then immediately backpedals in an embarrassed tone ("...That was a joke.
  Sorry."). Keep it endearing.
- Ghost stays story-consistent: Act 1 garbled/confused, Act 2 catching details, Act 3 clearer & asking —
  cute, short.
- Tone: light, nerdy, fun. Static text only (no LLM yet).

Seed lines (expandable — add more per act):

**Act 1 (Lily nervous; Ghost garbled)**
- Lily: "Okay... read the *wish* behind the words, not the words. You've got this, {playerName}."
- Ghost: "...bl-blrb?"
- Lily: "Sorry — I think out loud when I'm nervous. Ignore me."
- Lily: "Whatever they're asking *for* — that's the group it belongs to."

**Act 2 (Lily warmer; Ghost catching details; first joke)**
- Lily: "You're quick at this. I mean — good teamwork, {playerName}."
- Ghost: "...key? lantern?"
- Lily: "Look, Ghost's noticing nouns now. They grow up so fast." (beat) "...That was a joke. Sorry."
- Lily: "It's kind of nice having company down here this late."

**Act 3 (Lily comfortable, jokier; Ghost clearer)**
- Lily: "Dialog trees — my natural habitat." (beat) "...okay, that one I meant. Mostly."
- Ghost: "where... room?"
- Lily: "It's asking follow-up questions. We basically raised a polite ghost."
- Lily: "If this works I'm putting 'ghost whisperer' on my CV. ...Kidding. Probably."
- Ghost: "thank... you."

Future: replace/extend with player-choice branches and LLM-generated lines (later); the deterministic
puzzle stays untouched.
