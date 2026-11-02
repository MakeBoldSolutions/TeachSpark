using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TeachSpark.Web.Data;
using Xunit;

namespace TeachSpark.Tests;

/// <summary>
/// WebApplicationFactory that wires test-safe configuration overrides:
/// in-temp SQLite, a placeholder LLM key, and a writable log path.
/// </summary>
public class TeachSparkWebFactory : WebApplicationFactory<Program>
{
    public string DbPath { get; } =
        Path.Combine(Path.GetTempPath(), $"teachspark-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        // UseSetting, not ConfigureAppConfiguration: Program reads these while building
        // the host, before ConfigureAppConfiguration callbacks apply.
        builder.UseSetting("ConnectionStrings:DefaultConnection", $"Data Source={DbPath}");
        builder.UseSetting("LlmConfiguration:ApiKey", "test-placeholder-key");
        builder.UseSetting("TeachSpark:LogFilePath", Path.Combine(Path.GetTempPath(), "teachspark-test.log"));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        // Pooled connections keep the file open on Windows
        SqliteConnection.ClearAllPools();
        if (File.Exists(DbPath))
            File.Delete(DbPath);
    }
}

/// <summary>
/// Baseline integration tests: one success path, one validation/failure path.
/// These run against the full ASP.NET Core pipeline via an in-process test server.
/// </summary>
public class BasicIntegrationTests : IClassFixture<TeachSparkWebFactory>
{
    private readonly TeachSparkWebFactory _factory;
    private readonly HttpClient _client;

    public BasicIntegrationTests(TeachSparkWebFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    [Fact]
    public void Database_UsesPerRunTempFile()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Assert.Contains(_factory.DbPath, context.Database.GetConnectionString());
    }

    [Fact]
    public async Task Get_HomePage_Returns200OK()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_NonExistentRoute_Returns404()
    {
        var response = await _client.GetAsync("/this-route-xyz-does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
