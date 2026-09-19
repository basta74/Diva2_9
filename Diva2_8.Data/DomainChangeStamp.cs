using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace Diva2.Data.Infrastructure;

/// <summary>Process-local domain versions. A cold cache always gets a fresh UTC timestamp.</summary>
public static class DomainChangeStamp
{
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());
    private static readonly object Gate = new();
    private static long lastTicks;

    public static string Get(string domain)
    {
        lock (Gate)
        {
            return Cache.Get<string>(domain) ?? Touch(domain);
        }
    }

    public static string Touch(string domain)
    {
        lock (Gate)
        {
            lastTicks = Math.Max(DateTime.UtcNow.Ticks, lastTicks + 1);
            var stamp = new DateTime(lastTicks, DateTimeKind.Utc).ToString("O");
            Cache.Set(domain, stamp);
            return stamp;
        }
    }
}

// Explicit transactions must publish their stamp after commit, never before it.
internal sealed class DomainChangeTransactionInterceptor : DbTransactionInterceptor
{
    public override void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
    {
        if (eventData.Context is ApplicationDbContext context)
        {
            DomainChangeStamp.Touch(context.SubDomain);
        }
    }

    public override Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        TransactionCommitted(transaction, eventData);
        return Task.CompletedTask;
    }
}

// ExecuteUpdate/ExecuteDelete bypass SaveChanges.
internal sealed class DomainChangeCommandInterceptor : DbCommandInterceptor
{
    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        if (result > 0 && command.Transaction is null && eventData.Context is ApplicationDbContext context)
        {
            DomainChangeStamp.Touch(context.SubDomain);
        }
        return result;
    }

    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
        int result, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(NonQueryExecuted(command, eventData, result));
    }
}
