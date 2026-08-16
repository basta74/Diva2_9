using Diva2.Core;
using Diva2.Core.Main.Users;
using Diva2.Data.Infrastructure;
using Diva2.Services.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Diva2Web.Infrastructure;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// controllers
builder.Services.AddControllersWithViews();

// config
builder.Configuration.AddJsonFile("appsubdomain.json", optional: false, reloadOnChange: true);

// services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IDomainService, DomainService>();
builder.Services.AddScoped<IConnectionStringProvider, DomainConnectionStringProvider>();
builder.Services.AddSingleton<ITenantCatalog, ConfigurationTenantCatalog>();
builder.Services.AddScoped<IApiTokenService, ApiTokenService>();

// DB (per domain)
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var connectionStringProvider = sp.GetRequiredService<IConnectionStringProvider>();
    var conn = connectionStringProvider.GetConnectionString();

    options.UseMySql(conn, ServerVersion.AutoDetect(conn));
});

// Identity
builder.Services.AddIdentity<User8, Role8>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, ApiTokenAuthenticationHandler>(
        ApiTokenAuthenticationHandler.AuthenticationScheme,
        _ => { });

// cookie config (DŮLEŽITÉ)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;

    // pokud jedeš na HTTPS (produkce), nech tohle:
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// password hasher (jen jednou!)
builder.Services.AddScoped<IPasswordHasher<User8>, SHA1PasswordHasher>();

// společný základ a rezervační doména
builder.Services.AddDiva2PlatformServices();
builder.Services.AddDiva2ReservationServices();

// webová implementace kontextu zůstává ve webové aplikaci
builder.Services.AddScoped<IWorkContext, WebWorkContext>();

// session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// static files dřív
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// 🔴 KLÍČOVÉ
app.UseAuthentication();
app.UseAuthorization();

// routing
app.MapControllers();

// AREA
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
);

// vlastní routy
app.MapControllerRoute("rozvrh", "rozvrh", new { controller = "Home", action = "Board" });
app.MapControllerRoute("cenik", "cenik", new { controller = "Home", action = "Prices" });
app.MapControllerRoute("mojedata", "moje-data/{id?}", new { controller = "Home", action = "MyData" });
app.MapControllerRoute("mojevidea", "moje-videa", new { controller = "Video", action = "MyVideos" });
app.MapControllerRoute("help", "pomoc", new { controller = "Home", action = "Help" });
app.MapControllerRoute("gdpr", "gdpr", new { controller = "Home", action = "Gdpr" });
app.MapControllerRoute("lektori", "lektori", new { controller = "Home", action = "Lectors" });
app.MapControllerRoute("video", "/home/video", new { controller = "Video", action = "Index" });
app.MapControllerRoute("info", "info", new { controller = "Home", action = "Info" });
app.MapControllerRoute("about", "about", new { controller = "Home", action = "About" });

// default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
