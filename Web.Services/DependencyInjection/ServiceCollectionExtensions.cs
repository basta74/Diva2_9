using Diva2.Data;
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
using Microsoft.Extensions.DependencyInjection;

namespace Diva2.Services.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers services that form the reusable Diva2 platform foundation.
    /// Application-specific services must be registered separately.
    /// </summary>
    public static IServiceCollection AddDiva2PlatformServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<ICacheHelper, CacheHelper>();
        services.AddScoped<IEmailSenderService, FakeEmailSenderService>();
        services.AddScoped<IUser8Service, User8Service>();
        services.AddScoped<IRuleService, RuleService>();
        services.AddScoped<IComunicationService, ComunicationService>();
        services.AddScoped<ILogs8Service, Logs8Service>();

        return services;
    }

    /// <summary>
    /// Registers the reservation-domain services used by the Diva2 application.
    /// These services are intentionally kept outside the reusable platform set.
    /// </summary>
    public static IServiceCollection AddDiva2ReservationServices(this IServiceCollection services)
    {
        services.AddScoped<IPobockaService, PobockaService>();
        services.AddScoped<ILekceService, LekceService>();
        services.AddScoped<ILekceTypService, LekceTypService>();
        services.AddScoped<ILekceMustrService, LekceMustrService>();
        services.AddScoped<ISkupinaZakaznikaService, SkupinaZakaznikaService>();
        services.AddScoped<ILektorService, LektorService>();
        services.AddScoped<IPlatbaService, PlatbaService>();
        services.AddScoped<IObjednavkyService, ObjednavkyService>();
        services.AddScoped<ILekceAddonsService, LekceAddonsService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<ICalendarService, CalendarService>();

        return services;
    }
}
