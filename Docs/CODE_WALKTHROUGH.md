# CODE_WALKTHROUGH.md

## Purpose

This file explains the implemented code in a way that can be used for report writing, debugging, and final project explanation.

Do not describe scripts that do not exist yet.

Each implemented C# file should use the format below.

---

## Script Template

### Script Name

TBD

### Purpose

What does this script do?

### Attached GameObject

Which Unity GameObject should this script be attached to?

### Runtime Role

When does this script run during Play Mode?

### Important Fields

List important serialized fields and what should be dragged into them in Inspector.

### Important Methods

Explain the key methods.

### Input

What player input, Unity event, or data does it receive?

### Output

What UI, state, Ghost response, or progress does it change?

### Failure Cases

What can go wrong? For example:
- missing Inspector reference
- empty data
- wrong tag
- invalid puzzle state

### Unity Test

How to test this script manually in Play Mode.

---

## Intent Classification Runtime

### Script Name

IntentCard.cs

### Purpose

Defines one message card for the Act 1 intent-classification puzzle. Each card has:
- an `Id` used by puzzle submissions
- `MessageText` shown later by UI
- an `IntentId` used as the correct purpose / intent group

### Attached GameObject

None. This is pure C# data and should not be attached to a GameObject.

### Runtime Role

Created by future level data or tests before validation. It does not run by itself.

### Important Fields

No serialized Unity fields. The constructor receives `id`, `messageText`, and `intentId`.

### Important Methods

- `IntentCard(string id, string messageText, string intentId)`: creates a card and rejects empty card ids or intent ids.

### Input

Plain C# constructor values.

### Output

An immutable card object that the validator can read.

### Failure Cases

- Empty card id throws an `ArgumentException`.
- Empty intent id throws an `ArgumentException`.
- Null message text is converted to an empty string.

### Unity Test

Use EditMode tests in `IntentClassificationValidatorTests.cs`. This script has no Play Mode behaviour.

---

### Script Name

IntentClassificationValidator.cs

### Purpose

Validates whether submitted groups of message card ids match the intended Act 1 learning concept: messages should be grouped by shared purpose / intent, not by exact wording.

### Attached GameObject

None. This is pure C# puzzle logic and should not be attached to a GameObject.

### Runtime Role

Future UI or puzzle controller code can call `IntentClassificationValidator.Validate(...)` after the player arranges cards into groups.

### Important Fields

No serialized Unity fields.

### Important Methods

- `Validate(IEnumerable<IntentCard> cards, IEnumerable<IEnumerable<string>> submittedGroups)`: compares the submitted card-id groups against the card intent ids.
- `IntentClassificationResult.IsCorrect`: true only when every known card appears exactly once and each intent appears in exactly one pure group.
- `IntentClassificationResult.Errors`: validation messages for incorrect or invalid submissions.

### Input

- A list of `IntentCard` objects representing the puzzle answer key.
- A list of submitted groups, where each group contains card ids.

### Output

An `IntentClassificationResult` with a boolean correctness flag and error details for UI feedback or tests.

### Failure Cases

The validator returns incorrect results with errors for:
- empty level card data
- duplicate card ids in level data
- null cards
- empty groups
- duplicate submitted cards
- unknown submitted card ids
- missing known cards
- groups that mix different intents
- one intent split across multiple groups

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/IntentClassificationValidatorTests.cs`.

---

### Script Name

Act1IntentClassificationSampleData.cs

### Purpose

Provides reusable sample data for the Act 1 intent-classification puzzle. The data demonstrates the learning concept that different wording can still share the same purpose / intent.

### Attached GameObject

None. This is pure C# sample data and should not be attached to a GameObject.

### Runtime Role

Future UI or puzzle controller code can call this class to get sample message cards and the correct intent groups. The class does not run by itself.

### Important Fields

No serialized Unity fields.

Constants:
- `FindItemIntentId`
- `AskLocationIntentId`
- `AskIdentityIntentId`

### Important Methods

- `CreateCards()`: returns fresh `IntentCard` objects for the Act 1 sample puzzle.
- `CreateCorrectGroups()`: returns the correct grouping by card id, ready to pass into `IntentClassificationValidator.Validate(...)`.

### Input

None. The sample data is created by method calls.

### Output

- Three intent groups.
- Nine message cards total.
- Three differently worded messages per intent.

Sample intent groups:
- `find_item`: messages about finding a missing key, notebook, or lantern.
- `ask_location`: messages asking where Ghost is.
- `ask_identity`: messages asking who Ghost is or what to call the little ghost.

### Failure Cases

- If card ids are edited later, `CreateCorrectGroups()` must be updated to match.
- If sample cards are moved to data files later, tests should continue validating that every card appears in exactly one correct group.

### Unity Test

Run the EditMode tests under `Assets/Tests/EditMode/Act1IntentClassificationSampleDataTests.cs`.
