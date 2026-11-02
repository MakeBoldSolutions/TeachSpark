using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeachSpark.Web.Data.Entities;
using TeachSpark.Web.Services;
using Xunit;

namespace TeachSpark.Tests;

/// <summary>
/// Authenticates requests from test headers so authorization rules can be
/// exercised without driving the Identity login UI.
/// X-Test-User: the user name; X-Test-Roles: comma-separated roles.
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";
    public const string UserHeader = "X-Test-User";
    public const string RolesHeader = "X-Test-Roles";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserHeader, out var user) || string.IsNullOrEmpty(user))
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new List<Claim> { new(ClaimTypes.Name, user.ToString()) };
        if (Request.Headers.TryGetValue(RolesHeader, out var roles))
        {
            claims.AddRange(roles.ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
    }
}

/// <summary>
/// Test host that swaps cookie authentication for <see cref="TestAuthHandler"/>
/// and configures one admin email.
/// </summary>
public class AdminAuthWebFactory : TeachSparkWebFactory
{
    public const string AdminEmail = "admin@teachspark.test";

    // Configured but with no account at startup: registering it must not grant Admin
    public const string UnregisteredAdminEmail = "late-admin@teachspark.test";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseSetting($"{AdminRoleService.AdminEmailsConfigKey}:0", AdminEmail);
        builder.UseSetting($"{AdminRoleService.AdminEmailsConfigKey}:1", UnregisteredAdminEmail);

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = TestAuthHandler.SchemeName;
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                options.DefaultForbidScheme = TestAuthHandler.SchemeName;
            });
        });
    }
}

/// <summary>
/// The admin area is limited to the Admin role: anonymous users are challenged,
/// signed-in users without the role are forbidden.
/// </summary>
public class AdminAuthorizationTests : IClassFixture<AdminAuthWebFactory>
{
    private readonly AdminAuthWebFactory _factory;
    private readonly HttpClient _client;

    public AdminAuthorizationTests(AdminAuthWebFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    private static HttpRequestMessage Get(string url, string? user = null, string? roles = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (user != null)
            request.Headers.Add(TestAuthHandler.UserHeader, user);
        if (roles != null)
            request.Headers.Add(TestAuthHandler.RolesHeader, roles);
        return request;
    }

    [Theory]
    [InlineData("/Admin/Dashboard")]
    [InlineData("/Admin/ApiKeys")]
    [InlineData("/Admin/Worksheets")]
    public async Task AdminArea_Anonymous_IsChallenged(string url)
    {
        var response = await _client.SendAsync(Get(url));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/Admin/Dashboard")]
    [InlineData("/Admin/ApiKeys")]
    [InlineData("/Admin/Worksheets")]
    public async Task AdminArea_SignedInWithoutAdminRole_IsForbidden(string url)
    {
        var response = await _client.SendAsync(Get(url, user: "teacher@teachspark.test"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminArea_AdminRole_IsAllowed()
    {
        var response = await _client.SendAsync(Get("/Admin/Dashboard", user: AdminAuthWebFactory.AdminEmail, roles: AppRoles.Admin));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task EnsureAdminsAsync_GrantsAdminOnlyToConfiguredEmails()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var adminRoles = scope.ServiceProvider.GetRequiredService<AdminRoleService>();

        var admin = new ApplicationUser { UserName = AdminAuthWebFactory.AdminEmail, Email = AdminAuthWebFactory.AdminEmail };
        var teacher = new ApplicationUser { UserName = "teacher2@teachspark.test", Email = "teacher2@teachspark.test" };
        Assert.True((await userManager.CreateAsync(admin, "Password123")).Succeeded);
        Assert.True((await userManager.CreateAsync(teacher, "Password123")).Succeeded);

        await adminRoles.EnsureAdminsAsync();

        Assert.True(await userManager.IsInRoleAsync(admin, AppRoles.Admin));
        Assert.False(await userManager.IsInRoleAsync(teacher, AppRoles.Admin));
    }

    [Fact]
    public async Task Register_WithConfiguredAdminEmail_DoesNotGrantAdmin()
    {
        var page = await _client.GetStringAsync("/Identity/Account/Register");
        var token = Regex.Match(page, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"").Groups[1].Value;
        Assert.NotEmpty(token);

        var response = await _client.PostAsync("/Identity/Account/Register", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Input.Email"] = AdminAuthWebFactory.UnregisteredAdminEmail,
            ["Input.Password"] = "Password123",
            ["Input.ConfirmPassword"] = "Password123",
            ["__RequestVerificationToken"] = token,
        }));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync(AdminAuthWebFactory.UnregisteredAdminEmail);
        Assert.NotNull(user);
        Assert.False(await userManager.IsInRoleAsync(user, AppRoles.Admin));
    }
}
