using System.Net;
using Xunit;

namespace TeachSpark.Tests;

/// <summary>
/// Public marketing pages render and do not advertise features the app does not have.
/// See .devspark.work/roadmap/roadmap.md for what is live versus planned.
/// </summary>
public class PublicPagesTests : IClassFixture<TeachSparkWebFactory>
{
    private readonly HttpClient _client;

    public PublicPagesTests(TeachSparkWebFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_RoadmapPage_ShowsLiveAndPlannedSections()
    {
        var response = await _client.GetAsync("/Home/Plan");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Available today", html);
        Assert.Contains("Foundation and trust", html);
    }

    [Theory]
    [InlineData("/Home/Plan", "In Progress")]
    [InlineData("/Home/Plan", "Rate limiting")]
    [InlineData("/Home/Plan", "indigo")]
    [InlineData("/", "Editable prompts")]
    [InlineData("/", "Download, print, or share")]
    [InlineData("/", "edit both the system and user prompts")]
    public async Task Get_PublicPage_DoesNotClaimUnbuiltFeatures(string url, string claim)
    {
        var html = await _client.GetStringAsync(url);

        Assert.DoesNotContain(claim, html, StringComparison.OrdinalIgnoreCase);
    }
}
