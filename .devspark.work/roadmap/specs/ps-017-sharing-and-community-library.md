---
id: ps-017
title: Sharing and community library
phase: 3
status: proposed
priority: P2
size: L
depends_on: [ps-007, ps-010, ps-013]
sources: [TeachSpark.Web/Data/Entities/Worksheet.cs (IsPublic), TeachSpark.Web/Data/Entities/WorksheetTemplate.cs (IsPublic)]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-017 — Sharing and community library

## Why

Good worksheets are reusable. Sharing turns each teacher's best generations into a resource for others, cuts generation cost (reuse instead of regenerate), and is the strongest growth loop available. The schema already anticipates it: `Worksheet.IsPublic` is commented "Can be shared with other teachers".

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 3. Requires verified accounts (ps-007), safety checks (ps-010) and ratings to rank content (ps-013).

VERIFIED FINDINGS
- Worksheet.IsPublic and WorksheetTemplate.IsPublic exist but are only editable by admins; no page lists public worksheets.
- Worksheets record standard, grade, type and difficulty, which are the natural browse facets.

GOAL
- Teachers can publish a worksheet to a community library, others can find it by standard, grade and type, copy it into their own library, and problems can be reported and removed.

DESIGN
- Share action (confirmed-email accounts only) with a publish confirmation; author shown by display name only.
- Library browse and search by standard, grade, subject, type; ranked by ratings and copies.
- "Copy to my worksheets" creates an owned copy (editable via ps-011).
- Report button and an admin moderation queue; unpublish and remove actions.
- Optional share-by-link for a single worksheet without publishing.

NON-GOALS
- Selling content, comments or discussion threads, following authors.

ACCEPTANCE CRITERIA
- Tests: unconfirmed accounts cannot publish; unpublished worksheets never appear; copying creates an independent copy; a reported item can be removed by an admin and disappears from the library.
- Content-safety checks run again on publish.

ROLLOUT
- Start invite-only or admin-approved publishing; open up once moderation volume is understood.
```

## Open questions

- Licence for shared content (e.g. CC BY)? Should publishing require admin approval at first? (Recommendation: yes.)
