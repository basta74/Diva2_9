using Diva2.Core;
using Diva2.Core.Main.Users;
using Diva2.Data;
using Diva2.Data.Infrastructure;
using Diva2.Services;
using Diva2.Services.Emailing;
using Diva2.Services.Managers.Calendar;
using Diva2.Services.Managers.Content;
using Diva2.Services.Managers.Customers;
using Diva2.Services.Managers.Emails;
using Diva2.Services.Managers.Mains;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Setting;
using Diva2.Services.Managers.Users;
using Diva2.Services.Managers.Videa;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// controllers
builder.Services.AddControllersWithViews();

// config
builder.Configuration.AddJsonFile("appsubdomain.json", optional: false, reloadOnChange: true);

// services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IDomainService, DomainService>();

// DB (per domain)
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var domainService = sp.GetRequiredService<IDomainService>();
    var d = domainService.Domain;

    var conn = $"server=localhost;database={d.db};user={d.user};password={d.pass};";

    options.UseMySql(conn, ServerVersion.AutoDetect(conn));
});

// Identity
builder.Services.AddIdentity<User8, Role8>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

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

// další služby
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<ICacheHelper, CacheHelper>();
builder.Services.AddScoped<IWorkContext, WebWorkContext>();
builder.Services.AddScoped<IPobockaService, PobockaService>();
builder.Services.AddScoped<ILekceService, LekceService>();
builder.Services.AddScoped<ILekceTypService, LekceTypService>();
builder.Services.AddScoped<ILekceMustrService, LekceMustrService>();
builder.Services.AddScoped<ISkupinaZakaznikaService, SkupinaZakaznikaService>();
builder.Services.AddScoped<ILektorService, LektorService>();
builder.Services.AddScoped<IPlatbaService, PlatbaService>();
builder.Services.AddScoped<IObjednavkyService, ObjednavkyService>();
builder.Services.AddScoped<IEmailSenderService, FakeEmailSenderService>();
builder.Services.AddScoped<IUser8Service, User8Service>();
builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddScoped<IComunicationService, ComunicationService>();
builder.Services.AddScoped<ILogs8Service, Logs8Service>();
builder.Services.AddScoped<ILekceAddonsService, LekceAddonsService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();

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
app.MapControllerRoute("video", "video", new { controller = "Video", action = "Index" });
app.MapControllerRoute("video2", "home/video", new { controller = "Video", action = "Index" });
app.MapControllerRoute("info", "info", new { controller = "Home", action = "Info" });
app.MapControllerRoute("about", "about", new { controller = "Home", action = "About" });

// default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();