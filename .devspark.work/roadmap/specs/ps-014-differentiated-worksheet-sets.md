---
id: ps-014
title: Differentiated worksheet sets
phase: 2
status: proposed
priority: P1
size: M
depends_on: [ps-003, ps-008]
sources: [TeachSpark.Web/Views/Home/Plan.cshtml ("Differentiated Output"), TeachSpark.Web/Controllers/WorksheetGeneratorController.cs (FormatDifficultyLevel)]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-014 — Differentiated worksheet sets

## Why

Every classroom has students below, at and above grade level. Teachers already pick one difficulty level (simplified, standard, advanced) per worksheet; producing all three for the same topic currently means three separate generations with no link between them. One request that produces a matched set saves the most teacher time per click of anything on this roadmap.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 2. Triples generation cost per request, so it depends on enforced limits (ps-003).

VERIFIED FINDINGS
- Difficulty levels "simplified", "standard", "advanced" exist (WorksheetGeneratorController.FormatDifficultyLevel) and are stored in Worksheet.DifficultyLevel.
- The public page lists "Differentiated Output" (same topic, three reading levels) under On the Horizon.
- No entity groups related worksheets.

GOAL
- From one request, a teacher gets a linked set of worksheets on the same standard and topic at two or three levels, with consistent structure so they can be used in the same lesson.

DESIGN
- "Generate as set" option on the create form choosing which levels.
- WorksheetSet entity linking the worksheets; the library shows sets together.
- Generate levels in sequence or in parallel, counting each against ps-003 quotas; one failure does not lose the others.
- Prompts keep question count and structure aligned across levels while varying text complexity and scaffolding.

NON-GOALS
- Assigning levels to individual students (no student data); per-class presets come with ps-016.

ACCEPTANCE CRITERIA
- Tests (fake LLM): a set request creates N linked worksheets with the right difficulty levels; quota is charged N times; a partial failure keeps the successful ones and reports the failure.
- Print/PDF (ps-012) can export each level.

ROLLOUT
- Behind the quota system; consider limiting sets to signed-in teachers with confirmed email (ps-007).
```

## Open questions

- Should a set count as one or N against the daily quota? (Recommendation: N; it is N provider calls.)
