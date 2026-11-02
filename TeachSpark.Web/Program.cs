using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeachSpark.Web;
using TeachSpark.Web.Data;
using TeachSpark.Web.Data.Entities;
using TeachSpark.Web.Extensions;
using TeachSpark.Web.Services;
using TeachSpark.Web.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);

LoggingUtility.ConfigureSerilogLogging(builder, "TeachSpark.Web");

// Add database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add Identity services
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Email confirmation
    options.SignIn.RequireConfirmedEmail = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

// Diagnostics reveal server paths and file listings: open in Development, Admin-only elsewhere
var isDevelopment = builder.Environment.IsDevelopment();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppPolicies.Diagnostics, policy =>
        policy.RequireAssertion(context => isDevelopment || context.User.IsInRole(AppRoles.Admin)));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the assets service for webpack asset resolution
builder.Services.AddSingleton<IAssetsService, AssetsService>();

// Register the admin role service (grants Admin to TeachSpark:AdminEmails)
builder.Services.AddScoped<AdminRoleService>();

// Register the academic standards import service
builder.Services.AddScoped<AcademicStandardsImportService>();

// Register CRUD services
builder.Services.AddCrudServices();

// Register worksheet generation services (LLM preparation)
builder.Services.AddWorksheetGenerationServices(builder.Configuration);

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    await scope.ServiceProvider.GetRequiredService<AdminRoleService>().EnsureAdminsAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Skip HTTPS redirection in non-Production environments (Replit proxy handles TLS)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Map Identity pages
app.MapRazorPages();

// Map Admin area routes
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

try
{
    app.Run();
}
finally
{
    Console.WriteLine("Application shutting down...");
}

// Expose Program to the test assembly (WebApplicationFactory<Program>)
public partial class Program { }
