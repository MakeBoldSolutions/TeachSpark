using Microsoft.AspNetCore.Identity;
using TeachSpark.Web.Data.Entities;

namespace TeachSpark.Web.Services;

/// <summary>
/// Application role names
/// </summary>
public static class AppRoles
{
    public const string Admin = "Admin";
}

/// <summary>
/// Authorization policy names
/// </summary>
public static class AppPolicies
{
    /// <summary>Anyone in Development; the Admin role in every other environment.</summary>
    public const string Diagnostics = "Diagnostics";
}

/// <summary>
/// Grants the Admin role to the accounts listed in TeachSpark:AdminEmails, at startup only.
/// No account is an administrator unless its email is configured there. Email ownership is
/// not verified (confirmation is off), so the role is never granted at registration: the
/// operator registers first, then adds the email and restarts.
/// </summary>
public class AdminRoleService
{
    public const string AdminEmailsConfigKey = "TeachSpark:AdminEmails";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AdminRoleService> _logger;
    private readonly HashSet<string> _adminEmails;

    public AdminRoleService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<AdminRoleService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _adminEmails = new HashSet<string>(
            configuration.GetSection(AdminEmailsConfigKey).Get<string[]>() ?? [],
            StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates the Admin role if missing and grants it to every existing configured account.
    /// Called once at startup.
    /// </summary>
    public async Task EnsureAdminsAsync()
    {
        if (!await _roleManager.RoleExistsAsync(AppRoles.Admin))
        {
            await _roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
        }

        if (_adminEmails.Count == 0)
        {
            _logger.LogWarning("No admin accounts configured ({ConfigKey}); the admin area is unreachable", AdminEmailsConfigKey);
            return;
        }

        foreach (var email in _adminEmails)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Configured admin email {Email} has no account; register it, then restart", email);
                continue;
            }

            await GrantAdminAsync(user);
        }
    }

    private async Task GrantAdminAsync(ApplicationUser user)
    {
        if (await _userManager.IsInRoleAsync(user, AppRoles.Admin))
            return;

        var result = await _userManager.AddToRoleAsync(user, AppRoles.Admin);
        if (result.Succeeded)
        {
            _logger.LogInformation("Granted {Role} role to {Email}", AppRoles.Admin, user.Email);
        }
        else
        {
            _logger.LogError("Failed to grant {Role} role to {Email}: {Errors}",
                AppRoles.Admin, user.Email, string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }
}
