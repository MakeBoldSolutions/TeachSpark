using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TeachSpark.Tests;

/// <summary>
/// Smoke tests for the redesigned Login and Register pages: they render,
/// expose the form fields Identity binds to, and still bind a posted form.
/// </summary>
public class IdentityPagesTests : IClassFixture<TeachSparkWebFactory>
{
    private readonly HttpClient _client;

    public IdentityPagesTests(TeachSparkWebFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    private static string AntiforgeryToken(string html)
    {
        var token = Regex.Match(html, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"").Groups[1].Value;
        Assert.NotEmpty(token);
        return token;
    }

    [Theory]
    [InlineData("/Identity/Account/Login", "Input.Email", "Input.Password", "Input.RememberMe")]
    [InlineData("/Identity/Account/Register", "Input.Email", "Input.Password", "Input.ConfirmPassword")]
    public async Task Get_IdentityPage_RendersExpectedFormFields(string url, params string[] fields)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        foreach (var field in fields)
            Assert.Contains($"name=\"{field}\"", html);
        AntiforgeryToken(html);
        // No external providers are configured in tests, so that block stays hidden
        Assert.DoesNotContain("id=\"external-account\"", html);
    }

    [Fact]
    public async Task Post_Login_WithUnknownUser_RerendersWithError()
    {
        var page = await _client.GetStringAsync("/Identity/Account/Login");

        var response = await _client.PostAsync("/Identity/Account/Login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Input.Email"] = "nobody@teachspark.test",
            ["Input.Password"] = "Password123",
            ["__RequestVerificationToken"] = AntiforgeryToken(page),
        }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Invalid login attempt.", await response.Content.ReadAsStringAsync());
    }
}
