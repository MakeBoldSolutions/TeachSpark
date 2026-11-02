---
id: ps-002
title: Lock down model API and diagnostics
phase: 0
status: ready
priority: P0
size: S
depends_on: []
sources: [.devspark.work/audit/2026-11-01_results.md (AUTH2), TeachSpark.Web/Controllers/ModelsController.cs, TeachSpark.Web/Controllers/DiagnosticsController.cs]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-002 — Lock down model API and diagnostics

## Why

The 2026-11-01 audit rated this HIGH (AUTH2): `ModelsController` has no authorization at all, including `POST api/models/refresh`, and the diagnostics assets page is switched on by a query string. Anonymous callers should not be able to trigger provider calls or see internals. Constitution principle IV requires explicit authorization boundaries.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 0. Follow-up to PR #14, which closed the admin-area hole (AUTH1).

VERIFIED FINDINGS
- Controllers/ModelsController.cs: [Route("api/[controller]")] with no [Authorize]; exposes GET list/education/recommend/{id}, POST recommend/estimate-cost/{id}/estimate-cost and POST refresh (audit AUTH2).
- Controllers/Api/WorksheetGeneratorApiController.cs duplicates several of these routes under api/worksheet-generator, with [Authorize].
- DiagnosticsController exposes asset diagnostics behind an "?enable" query switch (audit low-priority item).
- The admin boundary pattern already exists: BaseAdminController + AppRoles.Admin, tested in TeachSpark.Tests/AdminAuthorizationTests.cs.

GOAL
- No endpoint that calls the LLM provider, changes server state, or reveals internals is reachable anonymously.

DESIGN
- [Authorize] on ModelsController; [Authorize(Roles = AppRoles.Admin)] on refresh.
- Decide whether ModelsController and WorksheetGeneratorApiController should be merged; remove the duplicate surface if nothing in src/js uses it.
- Diagnostics: available only in Development, or to Admin in other environments; drop the query-string switch.

NON-GOALS
- API-key authentication for external callers (ps-018).

ACCEPTANCE CRITERIA
- Tests: anonymous gets 401 on every ModelsController route; signed-in non-admin gets 403 on refresh; admin gets 200 on refresh; diagnostics returns 404 or 403 in Production for non-admins.
- Frontend calls that use these endpoints still work for signed-in teachers.

ROLLOUT
- Single PR. Check the generator page's network calls for any anonymous use before merging.
```

## Open questions

- Does any signed-out page call `api/models`? A grep of `src/js` during specification answers this.
