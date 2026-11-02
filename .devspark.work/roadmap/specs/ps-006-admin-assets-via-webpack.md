---
id: ps-006
title: Admin assets through webpack
phase: 0
status: ready
priority: P2
size: S
depends_on: []
sources: [.devspark.work/audit/2026-11-01_results.md (FE1), TeachSpark.Web/Areas/Admin/Views/Shared/_AdminLayout.cshtml]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-006 — Admin assets through webpack

## Why

Constitution principle II says frontend assets go through Bootstrap and webpack. The public site now complies (PR #14 moved the fonts), but the admin layout still loads 13 assets from CDNs (DataTables, jszip, pdfmake, Chart.js), some unpinned (audit FE1, HIGH).

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 0. Low user impact; constitution compliance.

VERIFIED FINDINGS
- _AdminLayout.cshtml (around lines 25-27 and 186-195) references CDN scripts and styles (audit FE1).
- The main layout already uses AssetsService.GetAssetPath for webpack outputs, including vendors.css.
- DataTables is already an npm dependency imported by site.scss.

GOAL
- The admin area loads no third-party CDN assets; everything is versioned through package.json and bundled by webpack.

DESIGN
- Add an "admin" webpack entry that imports the DataTables plugins, jszip, pdfmake and Chart.js from npm.
- Reference it with AssetsService in _AdminLayout.cshtml.

NON-GOALS
- Redesigning admin pages.

ACCEPTANCE CRITERIA
- No external asset URLs in _AdminLayout.cshtml (Google Analytics excepted).
- Admin tables, exports and charts still work (manual check plus the existing admin 200 test).
- npm run build has no new warnings.

ROLLOUT
- Single PR.
```

## Open questions

- Is Chart.js still used on the admin dashboard? Drop it if not.
