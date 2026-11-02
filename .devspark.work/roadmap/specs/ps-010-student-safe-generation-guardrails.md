---
id: ps-010
title: Student-safe generation guardrails
phase: 1
status: proposed
priority: P1
size: M
depends_on: [ps-005]
sources: [TeachSpark.Web/Services/Implementations/OpenAILlmService.cs, TeachSpark.Web/Services/Prompts/PromptTemplates.cs, TeachSpark.Web/Data/Entities/Worksheet.cs]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-010 — Student-safe generation guardrails

## Why

Worksheets go in front of children. Teachers can paste their own source text, and model output is shown as-is. There is no content-safety check on input or output and no structural check that the result is a usable worksheet. One inappropriate worksheet would cost far more trust than this item costs to build.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 1. Required before sharing (ps-017) and recommended before wider promotion.

VERIFIED FINDINGS
- No moderation or content-safety code exists in TeachSpark.Web (no references to moderation or content safety).
- Worksheet already has ConfidenceScore and Warnings (JSON) fields populated by OpenAILlmService (around lines 515-516), so there is a place to record findings.
- Teachers can supply SourceText that is sent to the model.
- Prompts live in Services/Prompts/PromptTemplates.cs.

GOAL
- Generated worksheets are age-appropriate for the selected grade and structurally complete, and problems are caught before the teacher sees the worksheet.

DESIGN
- Input check: run teacher-supplied source text through a moderation API; refuse clearly unsafe input with a helpful message.
- Output check: moderation on generated content; validate structure (title, instructions, the requested number of questions, answer key when requested).
- Record findings in Worksheet.Warnings; show non-blocking warnings to the teacher; block and retry once on hard failures.
- Strengthen system prompts with grade-appropriate content rules.

NON-GOALS
- Human moderation workflow (ps-017 adds reporting for shared content).
- Fact-checking content accuracy beyond structural checks.

ACCEPTANCE CRITERIA
- Tests (fake LLM and fake moderation): unsafe input is refused without generating; unsafe output is not shown; a missing answer key produces a warning; a clean worksheet passes untouched.
- Moderation cost and latency are logged; generation time stays within an agreed budget.

ROLLOUT
- Start in log-only mode for a week to measure false positives, then enforce.
```

## Open questions

- Which moderation service (OpenAI moderation is free and already integrated)? Acceptable added latency?
