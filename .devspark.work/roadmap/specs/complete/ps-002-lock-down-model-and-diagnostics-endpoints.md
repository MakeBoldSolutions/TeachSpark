---
id: ps-002
title: Lock down model API and diagnostics
phase: 0
status: done
priority: P0
size: S
depends_on: []
sources: [.devspark.work/audit/2026-11-01_results.md (AUTH2), TeachSpark.Web/Controllers/ModelsController.cs, TeachSpark.Web/Controllers/DiagnosticsController.cs]
spec: "branch feat/ps-002-lock-down-endpoints (universal route, no spec bundle)"
delivered_in: "PR #17 (7360548)"
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

## Decisions (specification, 2026-11-02)

- **Nothing calls `api/models`.** The only frontend model call is `/api/worksheet-generator/models` from the signed-in Create page (`Views/WorksheetGenerator/Create.cshtml:199`).
- **Delete `ModelsController`** rather than securing it (decided by the maintainer). `WorksheetGeneratorApiController` already covers model listing, recommendation and cost estimation behind `[Authorize]`. This also removes `GET api/models?forceRefresh=true`, a second anonymous path to a provider call found during specification.
- **Diagnostics:** Development, or Admin role in any other environment; the `?enable` switch goes.
- Acceptance tests become: every former `api/models` route answers like a path that never existed (404 for GET, 405 for POST); the `api/worksheet-generator/models*` routes return 401 anonymously and 200 for a signed-in teacher; outside Development `/Diagnostics/Assets` returns 401 anonymously and 403 for non-admins (with or without `?enable`) and 200 for Admin, and in Development it is open. Implemented as the `Diagnostics` authorization policy (`AppPolicies.Diagnostics`), so the boundary is an attribute on the controller as constitution principle IV requires. Tests: `TeachSpark.Tests/EndpointAuthorizationTests.cs`.
