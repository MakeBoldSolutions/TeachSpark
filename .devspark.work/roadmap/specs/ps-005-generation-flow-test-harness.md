---
id: ps-005
title: Generation-flow test harness
phase: 0
status: proposed
priority: P0
size: M
depends_on: []
sources: [.devspark.work/audit/2026-11-01_results.md (TEST1), TeachSpark.Tests/, TeachSpark.Web/Services/Interfaces/ILlmServices.cs]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-005 — Generation-flow test harness

## Why

Generation is the product, and it has no tests (audit TEST1). It cannot be tested today without calling OpenAI. A fake LLM seam makes the flow testable in CI, and it is the prerequisite for safely changing limits, standards, guardrails and differentiation.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 0. Most items that touch generation depend on it.

VERIFIED FINDINGS
- TeachSpark.Tests has 15 integration tests (routing, admin authorization, identity pages); none cover WorksheetGeneratorController.Create or Display.
- LLM access goes through service interfaces in Services/Interfaces/ILlmServices.cs, implemented by OpenAILlmService (560 lines) and OpenAIModelRegistryService (523 lines).
- TeachSparkWebFactory already isolates the database per run and sets a placeholder API key (TeachSpark.Tests/BasicIntegrationTests.cs).
- The CI workflow runs dotnet test on every push.

GOAL
- The full generate, save and display flow runs in CI against a deterministic fake LLM, with no network calls.

DESIGN
- A fake implementation of the LLM service interface returning canned markdown, cost and token counts; registered via ConfigureTestServices.
- Reuse TestAuthHandler for a signed-in teacher.
- Tests: a valid request saves a worksheet owned by the user and redirects to Display; invalid input re-renders the form; a provider failure shows an error without saving; a user cannot display another user's worksheet.
- Fail the run if any test attempts a real OpenAI call.

NON-GOALS
- Evaluating output quality (ps-010, ps-013).
- Refactoring OpenAILlmService beyond what the seam needs.

ACCEPTANCE CRITERIA
- At least the four scenarios above pass in CI.
- No test needs an API key or network access.

ROLLOUT
- Tests only; no production behaviour change.
```

## Open questions

- None blocking.
