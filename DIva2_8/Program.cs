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

// Add services to the container.


builder.Services.AddControllersWithViews();


builder.Configuration.AddJsonFile("appsubdomain.json", optional: false, reloadOnChange: true);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IDomainService, DomainService>();


builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var domainService = sp.GetRequiredService<IDomainService>();
    var d = domainService.Domain;

    var conn = $"server=localhost;database={d.db};user={d.user};password={d.pass};";

    options.UseMySql(conn, ServerVersion.AutoDetect(conn));
});

builder.Services.AddIdentity<User8, Role8>()
    .AddEntityFrameworkStores<ApplicationDbContext>()

    .AddDefaultTokenProviders();


builder.Services.AddScoped<IPasswordHasher<User8>, SHA1PasswordHasher>();




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
builder.Services.AddScoped<IPasswordHasher<User8>, SHA1PasswordHasher>();
builder.Services.AddScoped<IEmailSenderService, FakeEmailSenderService>();
builder.Services.AddScoped<IUser8Service, User8Service>();
builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddScoped<IComunicationService, ComunicationService>();
builder.Services.AddScoped<ILogs8Service, Logs8Service>();
builder.Services.AddScoped<ILekceAddonsService, LekceAddonsService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build(); // ✅ až teď


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// ✅ SEM vlož routes
app.MapControllers();

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "rozvrh",
    pattern: "rozvrh",
    defaults: new { controller = "Home", action = "Board" });

// ... další tvoje routes

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



