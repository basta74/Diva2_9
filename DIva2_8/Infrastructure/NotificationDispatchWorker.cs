using Diva2.Core.Main.Notifications;
using Diva2.Services.Managers.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Diva2Web.Infrastructure;

public sealed class NotificationDispatchWorker : BackgroundService
{
    private const int MaxAttempts = 5;
    private const string NotificationMigration = "20260822162949_AddNotifications";
    private readonly IConfiguration configuration;
    private readonly IPushNotificationProvider provider;
    private readonly ILogger<NotificationDispatchWorker> logger;

    public NotificationDispatchWorker(IConfiguration configuration, IPushNotificationProvider provider, ILogger<NotificationDispatchWorker> logger)
    {
        this.configuration = configuration;
        this.provider = provider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var tenant in TenantDatabaseContextFactory.GetUniqueDatabases(configuration))
            {
                try { await DispatchTenantAsync(tenant, stoppingToken); }
                catch (Exception ex) { logger.LogError(ex, "Notification dispatch failed for database {Database}.", tenant.db); }
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task DispatchTenantAsync(Diva2.Core.Main.Domains.SubDomain tenant, CancellationToken cancellationToken)
    {
        await using var db = TenantDatabaseContextFactory.Create(tenant);
        var appliedMigrations = await db.Database.GetAppliedMigrationsAsync(cancellationToken);
        if (!appliedMigrations.Contains(NotificationMigration, StringComparer.Ordinal))
        {
            return;
        }
        var pending = await db.UserNotifications
            .Where(x => x.SentAt == null && x.Attempts < MaxAttempts)
            .OrderBy(x => x.CreatedAt).Take(50).ToListAsync(cancellationToken);
        foreach (var notification in pending)
        {
            var devices = await db.UserDevices.Where(x => x.UserId == notification.UserId && x.Active).ToListAsync(cancellationToken);
            if (devices.Count == 0)
            {
                notification.Attempts++;
                notification.Error = "No active device is registered.";
                await db.SaveChangesAsync(cancellationToken);
                continue;
            }

            notification.Attempts++;

            foreach (var device in devices)
            {
                var delivery = await db.NotificationDeliveries.FirstOrDefaultAsync(x =>
                    x.UserNotificationId == notification.Id && x.UserDeviceId == device.Id, cancellationToken);
                if (delivery is null)
                {
                    delivery = new NotificationDelivery { UserNotificationId = notification.Id, UserDeviceId = device.Id };
                    db.NotificationDeliveries.Add(delivery);
                    await db.SaveChangesAsync(cancellationToken);
                }
                if (delivery.SentAt != null || delivery.Attempts >= MaxAttempts) continue;

                var currentAttempts = delivery.Attempts;
                var claimed = await db.NotificationDeliveries
                    .Where(x => x.Id == delivery.Id && x.SentAt == null && x.Attempts == currentAttempts)
                    .ExecuteUpdateAsync(x => x.SetProperty(d => d.Attempts, d => d.Attempts + 1), cancellationToken);
                if (claimed == 0) continue; // another application instance owns this delivery
                delivery.Attempts = currentAttempts + 1;
                await db.SaveChangesAsync(cancellationToken); // durable claim before the external call
                PushNotificationResult result;
                try
                {
                    result = await provider.SendAsync(new PushNotificationMessage(
                        delivery.Id, device.PushToken, device.Platform, notification.Title, notification.Text, notification.Id), cancellationToken);
                }
                catch (Exception ex)
                {
                    result = new PushNotificationResult(false, Error: ex.Message);
                }

                if (result.Success)
                {
                    delivery.SentAt = DateTime.UtcNow;
                    delivery.ProviderMessageId = result.ProviderMessageId;
                    delivery.Error = null;
                    notification.SentAt ??= delivery.SentAt;
                    notification.Error = null;
                }
                else
                {
                    delivery.Error = result.Error;
                    notification.Error = result.Error;
                }
                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
