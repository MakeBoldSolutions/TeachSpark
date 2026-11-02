---
id: ps-015
title: Usage, cost and quality analytics
phase: 2
status: proposed
priority: P2
size: M
depends_on: [ps-003, ps-013]
sources: [TeachSpark.Web/Data/Entities/ApiUsage.cs, TeachSpark.Web/Areas/Admin/Controllers/ApiUsageController.cs, TeachSpark.Web/Areas/Admin/Controllers/DashboardController.cs]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-015 — Usage, cost and quality analytics

## Why

Decisions on this roadmap (quotas, models, which worksheet types to invest in, whether SQLite is enough) need numbers. Cost, tokens and timing are already recorded per generation, but there is no view that answers "what does a worksheet cost, which types do teachers use, and which are rated well?"

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 2. Uses spend data from ps-003 and quality data from ps-013.

VERIFIED FINDINGS
- Worksheet stores GenerationCost, GenerationTime, TokensUsed, LlmModel, WorksheetType; ApiUsage records provider usage; admin has ApiUsage and Dashboard controllers.
- The public page lists "Usage & Impact Analytics" and claims "<60s average generation time" without a measured source.
- Google Analytics is already on public pages for traffic.

GOAL
- Admins can see weekly active teachers, worksheets generated, cost per worksheet, generation time and rating by type and model, over time.

DESIGN
- Admin analytics page with weekly trends: active teachers, worksheets, cost per worksheet (median and p90), generation time (median and p90), rating by type, template and model.
- Teacher-facing: a simple "your month" summary on the dashboard (worksheets created, favourites).
- Feed the measured median generation time to the public roadmap page (ps-001) instead of a hard-coded claim.

NON-GOALS
- External BI tools, per-student learning outcomes (no student data).

ACCEPTANCE CRITERIA
- Tests: aggregates are correct for seeded data; the page is admin-only.
- Queries stay fast on a database with 100k worksheets (benchmark).

ROLLOUT
- Read-only feature; add indexes via migrations if needed.
```

## Open questions

- Which few metrics define success for the project? (Suggest: weekly active teachers, worksheets per teacher, share printed or exported, cost per worksheet.)
