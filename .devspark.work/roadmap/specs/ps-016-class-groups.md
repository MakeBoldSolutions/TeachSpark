---
id: ps-016
title: Class groups (no student data)
phase: 3
status: proposed
priority: P2
size: M
depends_on: [ps-008, ps-014]
sources: [TeachSpark.Web/Views/Home/Plan.cshtml ("Student Cohort Management")]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-016 — Class groups (no student data)

## Why

The public page promises "Student Cohort Management": grouping students by grade, Bloom readiness and learning style. Storing data about students would bring privacy obligations (FERPA, COPPA, state student-privacy laws) that a small project should avoid. The teacher's real need is to organise work by class and reuse that class's settings, which needs no student data at all.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 3. Reframes the "Student Cohort Management" item on the public page.

VERIFIED FINDINGS
- No cohort, class or student entity exists in Data/Entities.
- The public page shows "Student Cohort Management" as In Progress.
- Worksheets already carry grade (via standard), difficulty level and accessibility options that a class could default.

GOAL
- A teacher creates class groups (e.g. "Period 3 - Grade 7 ELA") with default grade, subject, difficulty mix and accessibility options, generates worksheets for a class in one step, and filters the library by class.

DESIGN
- ClassGroup entity owned by a teacher: name, grade, subject, default levels (for ps-014 sets), accessibility defaults, optional notes. No student names, rosters or identifiers.
- Assign worksheets to one or more classes; library filter by class.
- "New worksheet for this class" pre-fills the create form.
- Update the public page to describe class groups rather than student cohorts.

NON-GOALS
- Student rosters, student accounts, grades or progress tracking.

ACCEPTANCE CRITERIA
- Tests: a teacher sees only their own classes; class defaults pre-fill generation; deleting a class does not delete its worksheets.
- A privacy review confirms no personal student data fields exist.

ROLLOUT
- Migration for the new tables; feature visible to all teachers.
```

## Open questions

- Is a free-text notes field a risk (teachers typing student names)? Consider omitting it or warning in the UI.
