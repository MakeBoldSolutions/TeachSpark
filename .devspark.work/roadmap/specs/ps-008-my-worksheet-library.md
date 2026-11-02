---
id: ps-008
title: My worksheet library
phase: 1
status: proposed
priority: P0
size: M
depends_on: [ps-004, ps-005]
sources: [TeachSpark.Web/Views/Account/Dashboard.cshtml, TeachSpark.Web/Data/Entities/Worksheet.cs, TeachSpark.Web/Controllers/WorksheetGeneratorController.cs]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-008 — My worksheet library

## Why

A teacher can generate a worksheet but has no way to find it again: the dashboard only offers "New worksheet". Every worksheet is already stored with its owner, so this is mostly a missing screen, and it is what turns a one-off tool into one teachers return to.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 1. The highest-value teacher feature after the Phase 0 foundation.

VERIFIED FINDINGS
- Views/Account/Dashboard.cshtml links only to WorksheetGenerator/Create and Profile; AccountController.Dashboard loads no worksheets.
- Worksheet already stores UserId, Title, WorksheetType, DifficultyLevel, CommonCoreStandard, Tags, IsFavorite, CreatedAt, ViewCount, LastAccessedAt (Data/Entities/Worksheet.cs).
- IsFavorite and Tags are only editable in the admin area today.
- WorksheetGeneratorController.Display already loads a worksheet by id for its owner.

GOAL
- A signed-in teacher sees all their worksheets, can find one quickly, reopen it, favourite it, and delete it.

DESIGN
- "My worksheets" page: list with title, type, standard, grade, created date; search by title; filter by type, grade and favourites; newest first; paged.
- Favourite toggle and delete (with confirmation) scoped to the owner.
- Dashboard shows recent worksheets and favourites.
- "Generate similar" pre-fills the create form from an existing worksheet.

NON-GOALS
- Folders or class organisation (ps-016), sharing (ps-017), editing content (ps-011).

ACCEPTANCE CRITERIA
- Tests: a teacher sees only their own worksheets; cannot favourite, view or delete another user's (404 or 403); search and filters narrow results; delete removes the worksheet and its exports.
- Page works at phone width.

ROLLOUT
- Single PR; no migration needed unless indexes are added (then via ps-004 migrations).
```

## Open questions

- Soft delete (recoverable) or hard delete? (Recommendation: soft delete with a 30-day purge.)
