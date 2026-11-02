---
id: ps-004
title: EF Core migrations baseline
phase: 0
status: proposed
priority: P0
size: M
depends_on: []
sources: [TeachSpark.Web/Program.cs, TeachSpark.Web/Migrations/]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-004 — EF Core migrations baseline

## Why

The app creates its database with `EnsureCreated()`, which never alters an existing database. A `Migrations/` folder exists but is not applied. Several upcoming items add tables or columns (library, edits, ratings, differentiated sets, class groups, sharing). Without working migrations, each of them would mean rebuilding the production database.

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 0. Blocks ps-008, ps-011, ps-013 and every later schema change.

VERIFIED FINDINGS
- Program.cs:80 calls context.Database.EnsureCreated().
- TeachSpark.Web/Migrations/ holds migrations from 2025-06-20 onward plus ApplicationDbContextModelSnapshot.cs (a top-15 change hotspot, 9 changes), so the snapshot and the real schema may have drifted.
- Constitution principle III requires EF Core and DbContext patterns for data access.
- Production uses SQLite (connection string DefaultConnection).

GOAL
- Schema changes ship as EF Core migrations that apply safely to the existing production database and to fresh databases.

DESIGN
- Generate a migration that brings the snapshot in line with the current model; compare it with the schema EnsureCreated produces.
- For existing databases created by EnsureCreated, insert baseline rows into __EFMigrationsHistory so only new migrations apply.
- Replace EnsureCreated with Migrate() at startup (or a documented deploy step); keep the test factory working with a fresh temp database.
- Back up the SQLite file before migrating in production.

NON-GOALS
- Moving off SQLite.

ACCEPTANCE CRITERIA
- Fresh database: Migrate() produces the same schema as today's EnsureCreated.
- Copy of the production database: baseline plus Migrate() succeeds with no data loss.
- A sample additive migration applies cleanly to both.
- All existing tests pass.

ROLLOUT
- Rehearse on a copy of the production database; take a file backup before the first deploy.
```

## Open questions

- Is there one production database, or several environments to baseline?
