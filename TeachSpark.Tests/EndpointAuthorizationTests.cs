using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using TeachSpark.Web.Services;
using TeachSpark.Web.Services.Interfaces;
using TeachSpark.Web.Services.Models;
using Xunit;

namespace TeachSpark.Tests;

/// <summary>
/// Model registry that never calls the provider, so signed-in API calls stay offline.
/// </summary>
public class StubModelRegistryService : IModelRegistryService
{
    public Task<ServiceResult<List<OpenAIModel>>> GetAvailableModelsAsync(bool forceRefresh = false, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult<List<OpenAIModel>>.SuccessResult(new List<OpenAIModel>()));

    public Task<ServiceResult<List<OpenAIModel>>> GetEducationRecommendedModelsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult<List<OpenAIModel>>.SuccessResult(new List<OpenAIModel>()));

    public Task<ServiceResult<ModelRecommendation>> RecommendModelAsync(ModelSelectionCriteria criteria, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult<ModelRecommendation>.ErrorResult("Not available in tests"));

    public Task<ServiceResult<OpenAIModel>> GetModelByIdAsync(string modelId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult<OpenAIModel>.ErrorResult("Not available in tests"));

    public Task<ServiceResult<decimal>> EstimateCostAsync(string modelId, int estimatedTokens, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult<decimal>.SuccessResult(0m));

    public Task<ServiceResult<bool>> RefreshModelRegistryAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult<bool>.SuccessResult(true));
}

/// <summary>
/// Header-authenticated test host (see <see cref="AdminAuthWebFactory"/>) with the model
/// registry stubbed out. Runs as the "Testing" environment, so it is not Development.
/// </summary>
public class EndpointAuthWebFactory : AdminAuthWebFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
            services.AddSingleton<IModelRegistryService, StubModelRegistryService>());
    }
}

/// <summary>
/// Same host running as Development, where diagnostics are open.
/// </summary>
public class DevelopmentEndpointAuthWebFactory : EndpointAuthWebFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Development");
    }
}

/// <summary>
/// ps-002: nothing that calls the LLM provider or reveals internals is reachable anonymously.
/// </summary>
public class EndpointAuthorizationTests : IClassFixture<EndpointAuthWebFactory>
{
    private const string Teacher = "teacher@teachspark.test";

    private readonly HttpClient _client;

    public EndpointAuthorizationTests(EndpointAuthWebFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    private static HttpRequestMessage Request(string method, string url, string? user = null, string? roles = null)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), url);
        if (method == "POST")
            request.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
        if (user != null)
            request.Headers.Add(TestAuthHandler.UserHeader, user);
        if (roles != null)
            request.Headers.Add(TestAuthHandler.RolesHeader, roles);
        return request;
    }

    // The anonymous api/models controller is gone. Each route must answer exactly like a path that
    // never existed (404 for GET; 405 for POST, from the GET-only static-asset fallback), for an
    // admin too, so the result is not an auth challenge.
    [Theory]
    [InlineData("GET", "/api/models")]
    [InlineData("GET", "/api/models?forceRefresh=true")]
    [InlineData("GET", "/api/models/education")]
    [InlineData("GET", "/api/models/recommend?worksheetType=reading-comprehension")]
    [InlineData("POST", "/api/models/recommend")]
    [InlineData("GET", "/api/models/gpt-4o-mini")]
    [InlineData("POST", "/api/models/gpt-4o-mini/estimate-cost")]
    [InlineData("POST", "/api/models/estimate-cost")]
    [InlineData("POST", "/api/models/refresh")]
    public async Task RemovedModelsApi_IsNotRouted(string method, string url)
    {
        var neverExisted = await _client.SendAsync(Request(method, "/api/never-existed/route"));
        var anonymous = await _client.SendAsync(Request(method, url));
        var admin = await _client.SendAsync(Request(method, url, user: AdminAuthWebFactory.AdminEmail, roles: AppRoles.Admin));

        Assert.Equal(method == "GET" ? HttpStatusCode.NotFound : HttpStatusCode.MethodNotAllowed, neverExisted.StatusCode);
        Assert.Equal(neverExisted.StatusCode, anonymous.StatusCode);
        Assert.Equal(neverExisted.StatusCode, admin.StatusCode);
    }

    [Theory]
    [InlineData("GET", "/api/worksheet-generator/models")]
    [InlineData("GET", "/api/worksheet-generator/models/recommend?worksheetType=reading-comprehension")]
    [InlineData("POST", "/api/worksheet-generator/models/estimate-cost")]
    public async Task WorksheetGeneratorApi_Anonymous_IsChallenged(string method, string url)
    {
        var response = await _client.SendAsync(Request(method, url));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WorksheetGeneratorModels_SignedInTeacher_IsAllowed()
    {
        var response = await _client.SendAsync(Request("GET", "/api/worksheet-generator/models", user: Teacher));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/Diagnostics/Assets")]
    [InlineData("/Diagnostics/Assets?enable")]
    [InlineData("/Diagnostics/Assets?enable=true")]
    public async Task Diagnostics_OutsideDevelopment_Anonymous_IsChallenged(string url)
    {
        var response = await _client.SendAsync(Request("GET", url));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/Diagnostics/Assets")]
    [InlineData("/Diagnostics/Assets?enable")]
    public async Task Diagnostics_OutsideDevelopment_SignedInWithoutAdminRole_IsForbidden(string url)
    {
        var response = await _client.SendAsync(Request("GET", url, user: Teacher));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Diagnostics_OutsideDevelopment_AdminRole_IsAllowed()
    {
        var response = await _client.SendAsync(Request("GET", "/Diagnostics/Assets", user: AdminAuthWebFactory.AdminEmail, roles: AppRoles.Admin));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public class DevelopmentDiagnosticsTests : IClassFixture<DevelopmentEndpointAuthWebFactory>
{
    private readonly HttpClient _client;

    public DevelopmentDiagnosticsTests(DevelopmentEndpointAuthWebFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    [Fact]
    public async Task Diagnostics_InDevelopment_Anonymous_IsAllowed()
    {
        var response = await _client.GetAsync("/Diagnostics/Assets");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
