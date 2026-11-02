---
id: ps-001
title: Truthful public roadmap page
phase: 0
status: in-progress
priority: P1
size: S
depends_on: []
sources: [TeachSpark.Web/Views/Home/Plan.cshtml, .devspark.work/roadmap/roadmap.md]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-001 — Truthful public roadmap page

## Why

The public `/Home/Plan` page tells teachers that three features are *In Progress*, but none of them exists in the code, and it describes cost limits that are not enforced. A teacher-facing project loses trust quickly if its roadmap is fiction. This is also the cheapest item on the roadmap.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 0. The roadmap in .devspark.work/roadmap/roadmap.md now exists and is the source of truth.

VERIFIED FINDINGS
- Plan.cshtml lists "Student Cohort Management", "REST API Endpoints", "Teacher Feedback Loop" as In Progress; no cohort entity, no feedback entity, and the only API is model metadata (Controllers/Api/WorksheetGeneratorApiController.cs, Controllers/ModelsController.cs).
- The page claims "daily cost limits" and "Rate limiting & cost caps"; LlmConfiguration.DailyCostLimit is never read outside its declaration, and no rate limiter is registered.
- The page says "indigo/amber palette"; PR #14 replaced it with the Make Bold rust/ember/ink/cream system.
- Headline counters (6 shipped / 3 in development / 4 planned / <60s) are hard-coded.

GOAL
- The public roadmap page accurately reflects roadmap.md: what is live, what is next, and what is later, with no claim the code does not support.

DESIGN
- Restructure the page into Live / Next (Phase 0-1) / Later (Phase 2-3) using the roadmap's items in teacher-facing wording.
- Keep the content in one place in the view (a single data array) so changing a status is a one-line edit; consider a small JSON file read at startup if that proves simpler.
- Remove or soften unverifiable metrics (e.g. "<60s" unless measured from ApiUsage data).

NON-GOALS
- Auto-generating the page from roadmap.md.
- Changing any feature behaviour.

ACCEPTANCE CRITERIA
- Every "Live" item on the page maps to working code; every "In progress" item maps to a ps item with status in-progress.
- An integration test confirms /Home/Plan returns 200.
- roadmap.md "How to use" step 5 names the exact file to edit.

ROLLOUT
- Single PR; no flags.
```

## Open questions

- Should the page show phase numbers, or only Now / Next / Later? (Recommendation: Now / Next / Later.)
