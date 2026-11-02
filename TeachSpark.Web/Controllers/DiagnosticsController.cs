using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeachSpark.Web;
using TeachSpark.Web.Services;

namespace TeachSpark.Web.Controllers;

/// <summary>
/// Diagnostic controller for troubleshooting asset loading issues
/// Open in Development; limited to the Admin role in every other environment
/// </summary>
[Authorize(Policy = AppPolicies.Diagnostics)]
public class DiagnosticsController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly IAssetsService _assetsService;
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(
        IWebHostEnvironment environment,
        IAssetsService assetsService,
        ILogger<DiagnosticsController> logger)
    {
        _environment = environment;
        _assetsService = assetsService;
        _logger = logger;
    }

    /// <summary>
    /// Displays diagnostic information about asset configuration
    /// Access via: /Diagnostics/Assets
    /// </summary>
    public IActionResult Assets()
    {
        var diagnosticInfo = new
        {
            Environment = _environment.EnvironmentName,
            WebRootPath = _environment.WebRootPath,
            ContentRootPath = _environment.ContentRootPath,
            ManifestPath = Path.Combine(_environment.WebRootPath, "assets-manifest.json"),
            ManifestExists = System.IO.File.Exists(Path.Combine(_environment.WebRootPath, "assets-manifest.json")),
            ManifestContent = GetManifestContent(),
            AssetTests = GetAssetTests(),
            PhysicalFiles = GetPhysicalFiles()
        };

        return Json(diagnosticInfo);
    }

    private object? GetManifestContent()
    {
        try
        {
            var manifestPath = Path.Combine(_environment.WebRootPath, "assets-manifest.json");
            if (System.IO.File.Exists(manifestPath))
            {
                var content = System.IO.File.ReadAllText(manifestPath);
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading manifest content");
        }
        return null;
    }

    private List<object> GetAssetTests()
    {
        var testAssets = new[] { "js/site.js", "/js/site.js", "css/site.css", "/css/site.css" };
        var results = new List<object>();

        foreach (var asset in testAssets)
        {
            try
            {
                var resolvedPath = _assetsService.GetAssetPath(asset);
                var physicalPath = Path.Combine(_environment.WebRootPath, resolvedPath.TrimStart('/'));

                results.Add(new
                {
                    LogicalPath = asset,
                    ResolvedPath = resolvedPath,
                    PhysicalPath = physicalPath,
                    FileExists = System.IO.File.Exists(physicalPath)
                });
            }
            catch (Exception ex)
            {
                results.Add(new
                {
                    LogicalPath = asset,
                    Error = ex.Message
                });
            }
        }

        return results;
    }

    private object GetPhysicalFiles()
    {
        var result = new Dictionary<string, List<string>>();

        try
        {
            var jsDir = Path.Combine(_environment.WebRootPath, "js");
            var cssDir = Path.Combine(_environment.WebRootPath, "css");
            var fontsDir = Path.Combine(_environment.WebRootPath, "fonts");

            if (Directory.Exists(jsDir))
            {
                result["js"] = Directory.GetFiles(jsDir, "*", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName)
                    .Where(f => f != null)
                    .Cast<string>()
                    .ToList();
            }

            if (Directory.Exists(cssDir))
            {
                result["css"] = Directory.GetFiles(cssDir, "*", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName)
                    .Where(f => f != null)
                    .Cast<string>()
                    .ToList();
            }

            if (Directory.Exists(fontsDir))
            {
                result["fonts"] = Directory.GetFiles(fontsDir, "*", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName)
                    .Where(f => f != null)
                    .Cast<string>()
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing physical files");
            result["error"] = new List<string> { ex.Message };
        }

        return result;
    }
}
