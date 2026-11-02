---
id: ps-011
title: Review and edit before print
phase: 1
status: proposed
priority: P1
size: M
depends_on: [ps-004, ps-008]
sources: [TeachSpark.Web/Views/WorksheetGenerator/Display.cshtml, TeachSpark.Web/Data/Entities/Worksheet.cs, TeachSpark.Web/Services/Implementations/MarkdownRenderingService.cs]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-011 — Review and edit before print

## Why

"Teachers stay in control of the AI" is the product's promise, but a teacher cannot change a single word of a generated worksheet; they can only regenerate it. Small edits (fix a question, change a name, remove an item) are the most common need after generation.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 1. Builds on the library (ps-008); feeds PDF export (ps-012).

VERIFIED FINDINGS
- Worksheet stores ContentMarkdown and a cached RenderedHtml; MarkdownRenderingService renders markdown to HTML.
- Display.cshtml offers teacher and student views plus browser print, with no editing.
- Only admins can edit worksheets (Areas/Admin/Controllers/WorksheetsController.Edit).

GOAL
- A teacher can edit a generated worksheet's content and title, preview the result, and keep the AI original available.

DESIGN
- Edit view with a markdown editor and live preview, owner-only.
- Keep the original AI output (new column, e.g. OriginalContentMarkdown, via a ps-004 migration) and allow "revert to original".
- Regenerate a single section or question (optional, behind ps-003 limits).
- Invalidate RenderedHtml on save.

NON-GOALS
- Rich WYSIWYG editing, collaborative editing, version history beyond original plus current.

ACCEPTANCE CRITERIA
- Tests: the owner can edit and save; others get 404 or 403; revert restores the original; rendered HTML reflects edits; script tags in markdown are not executed (sanitised rendering).
- Edited worksheets are marked as edited in the library.

ROLLOUT
- Migration for the original-content column; backfill from current content.
```

## Open questions

- Is per-question regeneration in scope for the first version? (Recommendation: no; ship editing first.)
