<!--
Sync Impact Report
Version change: template -> 1.0.0
Modified principles:
- [PRINCIPLE_1_NAME] -> I. New And Changed Code Must Be Tested
- [PRINCIPLE_2_NAME] -> II. Frontend Work Must Stay Within The Bootstrap And Webpack System
- [PRINCIPLE_3_NAME] -> III. Data Access Must Use EF Core And DbContext Patterns
- [PRINCIPLE_4_NAME] -> IV. Authorization Boundaries Must Be Explicit
Added sections:
- Technical Standards
- Delivery Workflow
Removed sections:
- [PRINCIPLE_5_NAME] placeholder section
Templates requiring updates:
- ✅ .documentation/templates/plan-template.md
- ✅ .documentation/templates/spec-template.md
- ✅ .documentation/templates/tasks-template.md
- ✅ README.md
- ⚠ pending .github/agents/*.md review only; no file changes required for this amendment
Follow-up TODOs:
- TODO(RATIFICATION_DATE): original ratification date is unknown after the template-only constitution was restored; confirm historical adoption date if needed
-->
# TeachSpark Constitution

## Core Principles

### I. New And Changed Code Must Be Tested
Every new feature, bug fix, and materially changed behavior MUST include automated
tests. The repository currently lacks a dedicated test project, so this principle
is forward-looking: legacy gaps do not excuse untested new work. Test scope MUST
match the change, including unit, integration, or contract coverage when the
affected behavior crosses boundaries.

### II. Frontend Work Must Stay Within The Bootstrap And Webpack System
User-facing changes MUST stay within the established ASP.NET MVC, Razor,
Bootstrap 5, and webpack asset pipeline used by TeachSpark. Views MUST use
Bootstrap classes and component patterns. Asset changes MUST flow through the
existing webpack build and generated wwwroot outputs. Deviations require explicit
justification because visual consistency and build reproducibility are project
standards.

### III. Data Access Must Use EF Core And DbContext Patterns
Application persistence MUST use EF Core, ApplicationDbContext, and the existing
entity model structure by default. New database interactions SHOULD be expressed
through DbContext, entities, migrations, and service-layer orchestration. Raw SQL
is an exception path and MUST be justified by a concrete technical need that EF
Core cannot satisfy cleanly.

### IV. Authorization Boundaries Must Be Explicit
Protected admin, account, and API functionality MUST declare authorization
requirements explicitly at the boundary. Identity and cookie configuration MUST
remain centralized in application startup, while controllers, page models, and
endpoints that require protection MUST use authorization attributes or policies
close to the protected surface. Implicit or assumed protection is insufficient
for security-sensitive behavior.

## Technical Standards

- TeachSpark is a .NET 10 ASP.NET Core MVC application with Razor views and EF Core.
- Nullable reference types and implicit usings MUST remain enabled unless a change
	includes a strong justification and localized mitigation.
- Frontend assets MUST continue to build through the Node.js and webpack workflow
	defined in TeachSpark.Web/package.json and the webpack configuration files.
- Bootstrap-based UI composition is the default presentation model; documentation,
	generated specs, and implementation plans MUST assume that baseline.

## Delivery Workflow

- Specifications MUST describe how new or changed behavior will be tested.
- Plans MUST include a constitution check that covers testing, frontend alignment,
	EF Core data access, and explicit authorization boundaries when applicable.
- Tasks for behavior changes MUST include test work, not treat testing as optional.
- Pull request review MUST treat constitution violations as blocking issues.
- Documentation SHOULD be updated when a change alters architecture, workflows,
	or user-visible behavior.

## Governance

This constitution is the authoritative source for TeachSpark engineering
constraints within Spec Kit workflows and related project planning artifacts.
Amendments MUST be made by updating this file directly, summarizing the impact in
the sync report, and aligning dependent templates in the same change. Compliance
MUST be reviewed during specification, planning, task generation, implementation,
and pull request review.

Versioning policy follows semantic versioning for governance:

- MAJOR: Remove or redefine a principle in a backward-incompatible way.
- MINOR: Add a principle or materially expand enforcement expectations.
- PATCH: Clarify wording, fix formatting, or make non-semantic refinements.

This amendment establishes the first project-specific constitution from a
placeholder template, so version 1.0.0 is the correct initial formal release.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): original adoption date unknown | **Last Amended**: 2026-03-29
