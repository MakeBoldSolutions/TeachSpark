# Repository Story: TeachSpark

> Generated 2026-11-02 | Window: 12 months (full history used; repository is 500 days old) | Scope: full

## Executive Summary

TeachSpark is an AI-assisted worksheet generator for teachers, published under the Make Bold Solutions organization. It is an ASP.NET Core MVC web application (.NET 10) with a webpack-built Bootstrap frontend, an admin area for managing standards, Bloom levels and templates, and an OpenAI-backed generation service. The repository has a README, a code of conduct, a contributing guide and, since 2026-11-01, a formal constitution and DevSpark workflow.

The project is small and young: **75 commits over 500 days** (first commit 2025-06-19, latest 2026-11-01), from **one human author** (69 commits under two name spellings) plus **dependabot** (6 commits). Work arrives in bursts rather than as a steady cadence: 29 commits in the June 2025 launch month, near-silence for eight months (1 commit each in July 2025, November 2025 and January 2026), then three focused pushes — March 2026 (12), May 2026 (16) and November 2026 (15). Each burst has a clear theme: initial build, .NET 10 migration and dependency hygiene, CI and testing, and most recently a full brand redesign with tightened security.

Governance has matured sharply in the latest burst. Pull-request merges were rare (3 merge commits in total, two of them on 2026-11-01), but the most recent change, PR #14, went through a recorded constitution-based review, four blocking findings were fixed, and an independent evaluator confirmed the fixes before merge. 57% of non-merge commits (41 of 72) follow the conventional-commit format, and that share is close to 100% for the 2026-11 work. CI (GitHub Actions) runs lint, frontend build, .NET build and tests on every push.

There are **no tagged releases**. Delivery evidence lives in merged PRs (#10, #13, #14) rather than versions, so there is no single answer to "what version is in production?". The biggest structural risk is concentration: one person wrote essentially all of the code.

## Technical Analysis

### Development Velocity

| Month | Commits | Theme (from subjects) |
|-------|---------|------------------------|
| 2025-06 | 29 | Initial build: scaffolding, worksheet generator, admin area |
| 2025-07 | 1 | — |
| 2025-11 | 1 | — |
| 2026-01 | 1 | .NET 10 migration and upgrade documentation |
| 2026-03 | 12 | Dependabot merges (#2, #5, #6, #7), npm audit fixes, docs reorganization, Spec Kit scripts |
| 2026-05 | 16 | .NET 10 + new design, CI workflow and first integration tests, CODE_OF_CONDUCT, AssetsService, Google Analytics |
| 2026-11 | 15 | Dependency/security fixes (PR #13), Make Bold redesign, DevSpark install, admin role restriction, PR #14 review cycle |

Line volume is **+256,673 / −143,063** across non-merge commits. These numbers are heavily inflated by committed build output: hashed bundles such as `wwwroot/js/vendor.63b9b725.js` and `wwwroot/css/vendors.5a6d06bb.css` appear repeatedly in the hotspot list, so each rebuild added and removed whole bundles. A May 2026 commit ("gitignore build artifacts") stopped tracking them. Removing those files would show a much smaller, more conventional churn; as recorded, the ~56% removed-to-added ratio reflects regenerated artifacts, not refactoring.

The baseline month for comparison (2025-11) had a single commit, so a baseline delta is not meaningful; the useful comparison is burst-to-burst, and the three 2026 bursts are similar in size (12–16 commits).

### Contributor Dynamics

| Role | Commits | Share |
|------|---------|-------|
| Lead Architect (sole human author, two git identities) | 69 | 92% |
| Automated dependency bot | 6 | 8% |

**Bus factor: 1.** All human-authored commits come from one person. The bot's contributions are lockfile bumps (qs, webpack, js-yaml, npm security group). There is no evidence of team growth over the timeline; the project is best read as a solo-developer journey with automated dependency upkeep.

### Quality Signals

- **Tests:** 3 C# files in `TeachSpark.Tests` against 67 non-migration C# source files in `TeachSpark.Web` (≈1:22 by file). The test project only appeared in May 2026 ("ci: add GitHub Actions workflow and baseline integration tests"); before that the repository had no automated tests. As of the 2026-11-01 merge it holds 15 integration tests covering basic routing, the per-run test database, admin authorization (401/403/200, role-grant rules) and the Login/Register pages.
- **Conventional commits:** 41 of 72 non-merge commits (57%). Early history uses free-form subjects ("Refactor code structure…", "upddted csproj"); 2026-11 commits are consistently typed and scoped (`feat(ui):`, `fix(security):`, `chore(review):`).
- **Message quality:** average subject length 57.8 characters. Subject-type estimates: 24 features, 9 fixes, 7 refactors, 12 chores, 2 docs, 1 test, 20 unclassified.

### Governance & Process Maturity

- **Merge commits:** 3 of 75 (4%). Most history was committed directly to `main`; PR-based flow appears only in late May 2026 (#10, dependabot) and 2026-11-01 (#13, #14). Several dependabot PRs in March 2026 were merged as plain commits rather than merge commits.
- **Review discipline:** PR #14 is the first change with a recorded review (`.devspark.work/pr-review/pr-14.md`): request-changes at rev 1, four fixes in `f816a51`, independent confirmation, approve. That file is itself a hotspot (4 changes), which is a healthy sign of the review loop actually running.
- **Tags:** none. Releases are not versioned.
- **Branch strategy:** short-lived feature/fix branches (`feat/make-bold-redesign`, `fix/security-deps-and-repo-urls`) merged into `main` by PR in the most recent work.

### Architecture & Technology

- **Stack:** C#/ASP.NET Core MVC on .NET 10 (`TeachSpark.Web.csproj`, `global.json`), EF Core with SQLite, ASP.NET Core Identity with roles, OpenAI integration (`Services/Implementations/OpenAILlmService.cs`). Frontend: Bootstrap 5.3 via SCSS, webpack 5, ESLint, stylelint, Prettier, Husky and lint-staged.
- **Language signals:** JavaScript, TypeScript, PowerShell, shell, Python (DevSpark tooling) and Markdown are present; there is no Dockerfile and no `pyproject.toml`.
- **CI:** `.github/workflows/ci.yml` runs `npm ci`, `npm run quality`, `npm run build`, then `dotnet restore`, `dotnet build --configuration Release` and `dotnet test`.
- **Hotspots:** `Views/Shared/_Layout.cshtml` (15), `wwwroot/css/site.css` (14), `wwwroot/js/site.js` (13), `TeachSpark.Web.csproj` (13), `wwwroot/js/validation.js` (12), `Program.cs` (12), `OpenAILlmService.cs` (11).

## Change Patterns

The top five most-modified files tell a consistent story:

1. **`Views/Shared/_Layout.cshtml` (15)** — every redesign and every asset-pipeline change touches the shared layout. It has been rebuilt at least twice (May 2026 design, November 2026 Make Bold redesign).
2. **`wwwroot/css/site.css` (14)** and **`wwwroot/js/site.js` (13)** — build output that used to be committed. Their churn is a pipeline artifact, not real code instability; the source of truth is now `src/scss/site.scss` (10) and `src/js/site.js` (4).
3. **`TeachSpark.Web.csproj` (13)** — framework upgrades (.NET 10), package bumps and build-integration tweaks.
4. **`wwwroot/js/validation.js` (12)** — also a built file; same caveat.
5. **`Program.cs` (12)** — the composition root accumulates every cross-cutting change (Identity, roles, logging, EF setup). It is the main backend complexity hotspot and is worth keeping lean.

`OpenAILlmService.cs` (11) and `WorksheetGeneratorController.cs` (9) are the real product hotspots: the core generation flow is where feature work lands. `Migrations/ApplicationDbContextModelSnapshot.cs` (9) reflects schema evolution; the app currently creates its database with `EnsureCreated`, so migration files and runtime behavior should be kept consistent.

Directory-level: nearly all hotspots live under `TeachSpark.Web/`; repository-level files (`README.md`, `.gitignore`, `package.json`) change occasionally for tooling and docs.

## Milestone Timeline

No tags exist, so merged pull requests and themed bursts serve as the milestones:

| Date | Marker | Description |
|------|--------|-------------|
| 2025-06-19 | Initial commit | Project created; 29 commits that month build the first version |
| 2026-01-11 | .NET 10 migration | Upgrade documentation and project files moved to .NET 10 |
| 2026-03-29 | Dependency sweep | Dependabot PRs #2, #5, #6, #7 merged; npm audit fixes; docs reorganized |
| 2026-05-14 | CI + tests | GitHub Actions workflow and baseline integration tests added |
| 2026-05-27 | PR #10 | Dependabot npm group merged |
| 2026-11-01 | PR #13 | Dependabot alerts and NuGet vulnerabilities resolved; repository links moved to MakeBoldSolutions |
| 2026-11-01 | PR #14 | Make Bold redesign, DevSpark 9.4.0, admin area restricted to the Admin role, reviewed and independently verified |

Activity spikes coincide with these markers and drops to near zero between them; there is no pre-release ramp because there are no releases.

## Constitution Alignment

The constitution (`.knowledge/governance/constitution.md`, v1.0.1) was ratified recently, so most history predates it. How well does the commit history reflect its stated values?

| Principle | Evidence | Alignment |
|-----------|----------|-----------|
| I. New and changed code must be tested | Test project only since 2026-05; 3 test files vs 67 source files; but PR #14 added admin-authorization and identity-page tests alongside the code | Improving; historically weak |
| II. Frontend stays within Bootstrap and webpack | Built assets were committed until 2026-05; PR #14 moved brand fonts from Google Fonts into webpack (finding M-01). The admin layout still loads DataTables/pdfmake from CDNs | Mostly aligned; one known gap |
| III. Data access uses EF Core and DbContext | `ApplicationDbContext` and entities are hotspots; no raw-SQL signals in commit subjects | Aligned |
| IV. Authorization boundaries are explicit | Admin area was open to any signed-in user until `352191f` restricted it to the Admin role; the boundary is now documented in `.knowledge/architecture/admin-authorization.md` | Aligned since 2026-11-01 |

Delivery-workflow expectations (PR review, conventional commits) are met by the latest work but not by most of the history.

## Developer FAQ

### What does this project do?
TeachSpark generates standards-aligned classroom worksheets with an LLM (OpenAI) while keeping the teacher in control of the content. Teachers sign in, pick standards, Bloom levels and templates, and generate worksheets; administrators manage that reference data in the `/Admin` area. It is published by Make Bold Solutions under the MIT license.

### What tech stack does it use?
ASP.NET Core MVC on .NET 10 (SDK pinned in `global.json`), EF Core with SQLite, ASP.NET Core Identity with roles, and an OpenAI-backed `OpenAILlmService`. The frontend is Bootstrap 5.3 configured through SCSS (`src/scss/site.scss`), bundled by webpack 5, with self-hosted fonts from `@fontsource`. Tooling: ESLint, stylelint, Prettier, Husky, lint-staged, GitHub Actions, and DevSpark 9.4.0.

### Where do I start?
Read `TeachSpark.Web/Program.cs` (the composition root, 12 changes) and `Views/Shared/_Layout.cshtml` (the shared shell, 15 changes). The product flow lives in `Controllers/WorksheetGeneratorController.cs` and `Services/Implementations/OpenAILlmService.cs`. Before changing anything, read `.knowledge/governance/constitution.md`, as `CLAUDE.md` instructs.

### How do I run it locally?
From the repository root run `npm install`, then in `TeachSpark.Web` run `npm install` and `npm run build` (the frontend build must be run inside `TeachSpark.Web`). Then `dotnet run --project TeachSpark.Web/TeachSpark.Web.csproj`; the `http` launch profile listens on port 5213. An LLM API key (`LlmConfiguration:ApiKey`) is needed for generation, and `TeachSpark:AdminEmails` controls who gets the Admin role at startup.

### How do I run the tests?
`dotnet test TeachSpark.Tests` runs the xUnit integration suite (15 tests as of 2026-11-01) against an in-process `WebApplicationFactory<Program>` with a per-run temporary SQLite file. Frontend checks run with `npm run quality` in `TeachSpark.Web`. CI runs both on every push.

### What is the branching/PR workflow?
Historically most commits went straight to `main` (only 3 of 75 commits are merges). Current practice, per PRs #13 and #14 and the constitution's delivery workflow, is a short-lived `feat/…` or `fix/…` branch, a pull request, a `/devspark-pr-review` recorded in `.devspark.work/pr-review/`, and a merge once blockers are independently verified.

### Who do I ask when I'm stuck?
The Lead Architect, who authored 92% of commits and all human-authored code. There is no second maintainer in the history; use GitHub Issues or Discussions on `MakeBoldSolutions/TeachSpark` as the README directs.

### What areas of the code change most often?
The shared layout (`_Layout.cshtml`), the styling pipeline (`src/scss/site.scss` and formerly committed `wwwroot/css/site.css`), and `Program.cs`. On the product side, `OpenAILlmService.cs` and `WorksheetGeneratorController.cs` see the most feature work.

### Are there coding standards I must follow?
Yes. The constitution's four principles (tests for changed code, Bootstrap/webpack-only frontend, EF Core data access, explicit authorization) are non-negotiable. Linting and formatting are enforced by ESLint, stylelint and Prettier via `npm run quality` and Husky/lint-staged hooks. Use conventional commits; 57% of history does, and recent work does consistently.

### What version is currently released?
None is tagged; the repository has no release tags. The latest delivered state is the merge of PR #14 (`81c0417`, 2026-11-01), which brought the Make Bold redesign, DevSpark 9.4.0 and the Admin role restriction.

---

Generated by /devspark-repo-story | DevSpark v9.4.0 — Adaptive System Life Cycle Development
