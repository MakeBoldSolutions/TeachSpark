---
id: ps-018
title: Public worksheet API
phase: 3
status: proposed
priority: P3
size: L
depends_on: [ps-002, ps-003, ps-005]
sources: [TeachSpark.Web/Data/Entities/ApiKey.cs, TeachSpark.Web/Areas/Admin/Controllers/ApiKeysController.cs, TeachSpark.Web/Controllers/Api/WorksheetGeneratorApiController.cs]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-018 — Public worksheet API

## Why

The public page lists "REST API Endpoints" as in progress, for LMS integrations and third-party tools. The groundwork exists (an `ApiKey` entity with hashing, limits and IP allow-lists, and admin screens to manage keys), but nothing authenticates with those keys, and the only API endpoints serve model metadata. This is last because it multiplies cost and abuse exposure and is only worth doing once the core flow is stable.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 3 (horizon). Revisit only when a concrete integration partner or demand exists.

VERIFIED FINDINGS
- ApiKey entity: hashed KeyValue, KeyPrefix, UserId, IsActive, ExpiresAt, RequestCount, DailyRequestLimit (1000), MonthlyRequestLimit (10000), AllowedIpAddresses (Data/Entities/ApiKey.cs).
- Admins can create and manage keys (Areas/Admin/Controllers/ApiKeysController.cs); no authentication handler uses them.
- Existing API controllers expose only model listing, recommendation and cost estimation (Controllers/Api/WorksheetGeneratorApiController.cs, Controllers/ModelsController.cs).

GOAL
- A teacher or approved integrator can create an API key and generate, list and fetch their worksheets over a documented, versioned REST API with enforced limits.

DESIGN
- API-key authentication scheme (header), hashed lookup, expiry, IP allow-list, per-key limits sharing the ps-003 quota machinery.
- Endpoints under /api/v1: POST worksheets (generate), GET worksheets, GET worksheets/{id} (markdown, HTML, PDF link), GET standards.
- OpenAPI document and a short developer guide.
- Teachers manage their own keys (not just admins).

NON-GOALS
- OAuth for third-party apps, LMS-specific plugins, webhooks.

ACCEPTANCE CRITERIA
- Tests: missing, invalid, expired and IP-blocked keys are refused; per-key daily limits enforced; a key only sees its owner's worksheets; generation through the API uses the same guardrails (ps-010) and quotas (ps-003).
- The OpenAPI document is published.

ROLLOUT
- Keys issued by request first; general availability once usage is understood.
```

## Open questions

- Is there a concrete integration (an LMS or school) driving this? If not, keep it on the horizon.
