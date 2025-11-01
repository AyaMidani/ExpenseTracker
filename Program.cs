using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Licensing;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load local .env file (ignored on Render)
if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

// Register Syncfusion license
var licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
if (!string.IsNullOrWhiteSpace(licenseKey))
{
    SyncfusionLicenseProvider.RegisterLicense(licenseKey);
}

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure SQLite database path
// Defaults to ./Data/ExpenseTracker.db locally, or use SQLITE_PATH from env
var dbPath = Environment.GetEnvironmentVariable("SQLITE_PATH") ?? "expense.db";
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlite($"Data Source={dbPath}"));

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// Bind to Render's dynamic PORT in production only
if (!app.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    app.Urls.Add($"http://*:{port}");
}

app.Run();
