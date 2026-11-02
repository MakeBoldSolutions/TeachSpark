---
id: ps-012
title: Classroom-ready PDF export
phase: 1
status: proposed
priority: P1
size: M
depends_on: [ps-011]
sources: [TeachSpark.Web/Data/Entities/WorksheetExport.cs, TeachSpark.Web/Areas/Admin/Controllers/WorksheetExportsController.cs, TeachSpark.Web/Views/WorksheetGenerator/Display.cshtml]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-012 — Classroom-ready PDF export

## Why

Worksheets end up on paper. Browser printing gives inconsistent margins, fonts and page breaks, and the answer key prints with the questions unless the teacher toggles views. A one-click PDF with a separate answer key is the "classroom-ready" step the roadmap page promises.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 1. Exports the edited content from ps-011.

VERIFIED FINDINGS
- Display.cshtml provides browser print and a "Student Version" view (printWorksheet(), loadPrintVersion()).
- WorksheetExport entity already models exports (ExportFormat PDF/DOCX/HTML, FilePath, ExpiresAt, IsTemporary, DownloadCount), and the admin area can list and clean up exports, but nothing creates them.
- Worksheet has HasAnswerKey and DownloadCount fields.

GOAL
- A teacher downloads a print-ready PDF of the student worksheet and, separately, the answer key, with consistent layout.

DESIGN
- Server-side PDF rendering from the worksheet HTML with a print stylesheet (name and date lines, readable fonts, sensible page breaks, footer with the standard code).
- Student PDF excludes answers; teacher PDF includes the answer key.
- Generate on demand and record a temporary WorksheetExport row; reuse the existing cleanup.
- Increment DownloadCount.

NON-GOALS
- DOCX export, custom branding or layout editors.

ACCEPTANCE CRITERIA
- Tests: the owner gets application/pdf for both versions; the student PDF contains no answer-key section; others get 404 or 403; an export row is recorded.
- Visual check of three worksheet types printed on Letter and A4.

ROLLOUT
- Keep browser print available as a fallback.
```

## Open questions

- PDF library choice (licensing matters: e.g. QuestPDF community licence vs a headless-Chromium approach).
