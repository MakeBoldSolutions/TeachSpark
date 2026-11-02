---
id: ps-013
title: Teacher feedback and ratings
phase: 2
status: proposed
priority: P1
size: M
depends_on: [ps-004, ps-008]
sources: [TeachSpark.Web/Views/Home/Plan.cshtml ("Teacher Feedback Loop", "Worksheet Rating & Favorites"), TeachSpark.Web/Data/Entities/WorksheetTemplate.cs]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-013 — Teacher feedback and ratings

## Why

The public page promises a "Teacher Feedback Loop" (shown as in progress) and "Worksheet Rating". Nothing exists yet. Without feedback there is no signal for which prompts, templates and models produce worksheets teachers actually use, and no way to rank shared content later (ps-017).

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 2. Needs migrations (ps-004) and the library (ps-008) as the place to rate from.

VERIFIED FINDINGS
- No rating or feedback entity exists in Data/Entities.
- Worksheet records LlmModel, TemplateId, WorksheetType, DifficultyLevel and GenerationPrompt, so ratings can be tied to what produced the worksheet.
- WorksheetTemplate has UsageCount but no quality measure.

GOAL
- Teachers can quickly rate a worksheet and say what was wrong, and that signal is visible per template, model and worksheet type.

DESIGN
- WorksheetFeedback entity: worksheet, user, rating (1-5 or thumbs), reason tags (wrong level, inaccurate, formatting, off-standard, great), optional comment.
- One-click rating on Display and in the library; optional follow-up tags.
- Admin view: average rating and top reasons by template, model and worksheet type; link to low-rated examples.
- Use the signal manually to improve prompts and templates (no automatic prompt changes).

NON-GOALS
- Model fine-tuning (deliberately off the roadmap).
- Public display of ratings (ps-017).

ACCEPTANCE CRITERIA
- Tests: a teacher can rate only their own or shared worksheets, once per worksheet (re-rating updates); admin aggregates are correct for seeded data.
- Rating takes one click.

ROLLOUT
- Migration for the new table; prompt for a rating at most once per worksheet.
```

## Open questions

- Five-star or thumbs up/down? (Recommendation: thumbs plus reason tags; easier to act on.)
