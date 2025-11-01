using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Licensing;
using DotNetEnv;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Load local .env (optional for local development)
if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

// Syncfusion License
var licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
if (!string.IsNullOrWhiteSpace(licenseKey))
{
    SyncfusionLicenseProvider.RegisterLicense(licenseKey);
}

// SQLite DB
var dbPath = Environment.GetEnvironmentVariable("SQLITE_PATH") ?? "expense.db";
var dbDir = Path.GetDirectoryName(dbPath);
if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
{
    Directory.CreateDirectory(dbDir);
}

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

var culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

app.Run();
