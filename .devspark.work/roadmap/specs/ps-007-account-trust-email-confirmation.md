---
id: ps-007
title: Email confirmation and password reset
phase: 1
status: proposed
priority: P1
size: M
depends_on: []
sources: [TeachSpark.Web/Program.cs, .knowledge/architecture/admin-authorization.md, .devspark.work/pr-review/pr-14.md (H-01 residual)]
spec: null
delivered_in: null
updated: 2026-11-02
---

# ps-007 — Email confirmation and password reset

## Why

Accounts are not tied to a verified email, and there is no way to recover a forgotten password. The PR #14 review left a residual risk: the admin grant trusts that a listed email was registered by its owner. Confirmed emails close that risk, enable password reset, and are a prerequisite for sharing (ps-017).

## Intake (paste into `/devspark-specify`)

```text
CONTEXT/TIMING
- Planned, Phase 1. Needs an email provider decision (roadmap open question 2).

VERIFIED FINDINGS
- Program.cs registers AddDefaultIdentity without RequireConfirmedAccount and registers no IEmailSender.
- .knowledge/architecture/admin-authorization.md: "Email confirmation is off, so a registration proves nothing about who owns an address."
- The independent verification of PR #14 noted: enabling email confirmation and checking EmailConfirmed would fully close the admin-grant risk.
- Identity's EmailConfirmed column already exists on AspNetUsers.

GOAL
- New accounts must confirm their email before signing in, users can reset forgotten passwords, and the admin grant requires a confirmed email.

DESIGN
- IEmailSender implementation for the chosen provider, configured via settings and secrets.
- RequireConfirmedAccount = true; scaffold or restyle Confirm Email, Resend Confirmation, Forgot Password and Reset Password pages in the ts-auth design.
- AdminRoleService skips listed emails whose account is not confirmed.
- Existing accounts: decide between grandfathering them as confirmed or asking them to confirm on next sign-in.

NON-GOALS
- Social login, MFA.

ACCEPTANCE CRITERIA
- Tests (with a fake IEmailSender): registering sends a confirmation link; sign-in before confirming is refused; the link confirms; forgot password sends a reset link that works.
- AdminRoleService does not grant Admin to an unconfirmed account (test).
- admin-authorization.md is updated.

ROLLOUT
- Decide the existing-account policy before deploy; send a notice if existing users must confirm.
```

## Open questions

- Which email provider? Grandfather existing accounts or not?
