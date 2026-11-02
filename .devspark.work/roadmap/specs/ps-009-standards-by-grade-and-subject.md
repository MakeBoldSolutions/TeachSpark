---
id: ps-009
title: Standards by grade and subject
phase: 1
status: proposed
priority: P0
size: M
depends_on: [ps-005]
sources: [TeachSpark.Web/Controllers/WorksheetGeneratorController.cs, TeachSpark.Web/Data/, TeachSpark.Web/Services/AcademicStandardsImportService.cs]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-009 — Standards by grade and subject

## Why

The generator only offers grade 8 English Language Arts standards, although CCSS ELA, CCSS Math and Colorado 2020 standards are already in the database. Most teachers are not grade 8 ELA teachers, so this one limit excludes most of the audience.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 1. Widens the audience with data that is already imported.

VERIFIED FINDINGS
- WorksheetGeneratorController.cs:324-325 filters CommonCoreStandardOptions to s.Grade == "8" ("Common Core Standards (8th grade ELA)").
- GetStandardsForGrade(string grade = "8") exists (line 295) but defaults to grade 8.
- CommonCoreStandard has Grade ("K".."11-12"), Subject and Domain columns (Data/Entities/CommonCoreStandard.cs).
- Source data in TeachSpark.Web/Data: CCSS English (~985 rows), CCSS Math (~511 rows), Colorado 2020 reading/writing (~1,483 rows), imported via AcademicStandardsImportService.
- Prompt templates and worksheet types (reading comprehension, grammar, vocabulary...) are ELA-oriented.

GOAL
- A teacher picks subject and grade, then a standard from that grade and subject, and the generated worksheet is appropriate to that grade.

DESIGN
- Subject and grade selectors on the create form drive the standards list (cascading, using the existing GetStandardsForGrade endpoint generalised by subject).
- Remember the teacher's last subject and grade.
- Pass grade level into the prompt so reading level and vocabulary match.
- Decide Math scope: either add Math-appropriate worksheet types or limit Math to the types that make sense; otherwise ship ELA K-12 first.

NON-GOALS
- Importing new state frameworks (open question 4).
- New worksheet layouts beyond what Math minimally needs.

ACCEPTANCE CRITERIA
- Tests (fake LLM): selecting grade 3 ELA lists only grade 3 ELA standards; the prompt sent to the LLM includes the grade; posting a standard that does not match the chosen grade is rejected.
- No hard-coded grade remains in the controller.

ROLLOUT
- Could ship ELA K-12 first and Math as a follow-up if Math worksheet types need design.
```

## Open questions

- ELA only, or ELA plus Math? (Roadmap open question 3.)
