---
id: ps-003
title: Enforce AI cost and usage limits
phase: 0
status: proposed
priority: P0
size: M
depends_on: [ps-005]
sources: [TeachSpark.Web/Configuration/LlmConfiguration.cs, TeachSpark.Web/Data/Entities/ApiUsage.cs, TeachSpark.Web/Services/Implementations/LlmLoggingService.cs]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-003 — Enforce AI cost and usage limits

## Why

Anyone can register, and every generation spends money with OpenAI. A `DailyCostLimit` setting exists ($50 in production, $100 in development), but nothing reads it. There is currently no ceiling on spend and no per-user limit, which makes this the largest operational risk.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 0. Must land before any growth or sharing feature.

VERIFIED FINDINGS
- LlmConfiguration.DailyCostLimit (Configuration/LlmConfiguration.cs:21; appsettings.json 50.0, appsettings.Development.json 100.0) has no reader in the codebase.
- No ASP.NET Core rate limiter is registered in Program.cs.
- Cost per generation is already computed and stored (Worksheet.GenerationCost and TokensUsed, the ApiUsage entity, LlmLoggingService).
- The public roadmap page advertises "Rate limiting & cost caps" as live.

GOAL
- Total and per-user AI spend are capped and enforced before a provider call is made, and teachers get a clear message when a limit is hit.

DESIGN
- Global daily cost ceiling checked against recorded usage before each generation; refuse with a friendly error when reached.
- Per-user daily generation quota (count-based, configurable; admins exempt or higher).
- ASP.NET Core rate limiter (fixed window per user) on the generation POST to absorb bursts.
- Admin dashboard shows today's spend against the ceiling.

NON-GOALS
- Billing, paid tiers or payment processing.
- Per-API-key limits (ps-018 reuses this mechanism).

ACCEPTANCE CRITERIA
- With the ps-005 fake LLM: generation succeeds under the limits; at the global ceiling and at the per-user quota it is refused without calling the provider; bursts get 429.
- Limits are configuration-only (no code change to adjust).

ROLLOUT
- Ship with conservative defaults; log refusals so limits can be tuned from real usage.
```

## Open questions

- Free with quotas, or paid tiers later? (Roadmap open question 1.) The quota design should not block either.
