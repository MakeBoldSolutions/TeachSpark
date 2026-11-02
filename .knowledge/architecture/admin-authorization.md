---
id: admin-authorization
type: architecture
title: Admin Area Authorization
source_of_truth:
  - TeachSpark.Web/Services/AdminRoleService.cs
  - TeachSpark.Web/Areas/Admin/Controllers/BaseAdminController.cs
  - TeachSpark.Web/Program.cs
  - TeachSpark.Tests/AdminAuthorizationTests.cs
appliesTo:
  - TeachSpark.Web/Areas/Admin/**
  - TeachSpark.Web/Services/AdminRoleService.cs
  - TeachSpark.Web/Views/Shared/_Layout.cshtml
last_verified: 2026-11-01
aliases:
  - admin role
  - who is an admin
  - AdminEmails
---

# Admin Area Authorization

The admin area (`/Admin/*`) is limited to accounts in the ASP.NET Core Identity role `Admin`
(`AppRoles.Admin`). Signing in is not enough.

## Contract

- **Boundary.** `BaseAdminController` carries `[Authorize(Roles = AppRoles.Admin)]`, and every admin
  controller inherits it. Anonymous requests are challenged (redirect to login); signed-in users
  without the role are forbidden (redirect to access denied).
- **Who is an admin.** Only accounts whose email is listed in the configuration key
  `TeachSpark:AdminEmails` (a string array, case-insensitive). The default in `appsettings.json` is an
  empty list.
- **How the role is granted.** At startup, `AdminRoleService.EnsureAdminsAsync` creates the `Admin`
  role if missing and adds it to each listed email that already has an account. A listed email with
  no account is logged as a warning and skipped.
- **Never at registration.** Email confirmation is off, so a registration proves nothing about who
  owns an address. Registering with a listed email does not grant the role. The operator registers
  first, then adds the email and restarts the app.
- **Empty list.** With no configured emails the admin area is unreachable for everyone, and startup
  logs a warning. This is the intended safe default.
- **Removing an email** from the list does not revoke an already-granted role; remove the role from
  the account directly.
- **Navigation.** The Admin menu in the main layout renders only when `User.IsInRole(AppRoles.Admin)`.

## Operational notes

- Roles travel in the auth cookie. An account granted the role while signed in sees the admin area
  after signing out and back in, or after the security-stamp refresh.
- Set `TeachSpark:AdminEmails` through user secrets, environment variables
  (`TeachSpark__AdminEmails__0`) or the deployed appsettings, not in source control.
