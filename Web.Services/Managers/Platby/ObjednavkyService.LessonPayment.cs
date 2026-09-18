using System;
using System.Collections.Generic;
using System.Linq;
using Diva2.Core.Api.V1;
using Diva2.Core.Extensions;
using Diva2.Core.Main.Pobocky;
using Diva2.Core.Main.Trans;
using Microsoft.EntityFrameworkCore;

namespace Diva2.Services.Managers.Platby;

public partial class ObjednavkyService
{
    private bool RemoveLessonCredit(UserTransakce transaction, int increment, bool allowDebt)
    {
        // Keep split debits and the balance updates atomic.
        using var scope = dbContext.Database.CurrentTransaction is null
            ? dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable)
            : null;
        var lesson = repLekce.Table.First(item => item.Id == transaction.LekceId);
        var settings = dbContext.Set<PobockaIni>().Where(item => item.PobockaId == lesson.PobockaId).ToList();
        bool Enabled(string key)
        {
            var value = settings.FirstOrDefault(item => string.Equals(item.Name, key, StringComparison.OrdinalIgnoreCase))?.Value;
            return value == "1" || bool.TryParse(value, out var enabled) && enabled;
        }
        var timeCredits = repUserCreditsTime.Table.Where(item => item.UserId == transaction.UserId
            && item.PokladnaId == transaction.PokladnaId && item.Platny && item.Kredit > 0).ToList();
        var normal = repUserCredits.Table.Where(item => item.UserId == transaction.UserId
            && item.PokladnaId == transaction.PokladnaId).Select(item => (int?)item.Kredit).FirstOrDefault() ?? 0;
        var charged = repTransUser.Table.Where(item => item.UserId == transaction.UserId
            && item.LekceId == transaction.LekceId && item.PokladnaId == transaction.PokladnaId
            && item.Status == "-" && (item.Typ == "k" || item.Typ == "kc"))
            .Sum(item => (int?)item.Kredit) ?? 0;
        var requested = new LessonCreditDemand
        {
            LessonId = transaction.LekceId,
            StartsAt = DateTimeExtensions.FromUnix(transaction.UnixTime),
            Credits = transaction.Kredit,
            AllowTime = Enabled("kreditCasovy"),
            AllowUnlimited = Enabled("kreditNeomezeny"),
            EntryIndex = transaction.Kredit > 0 ? charged / transaction.Kredit : 0
        };
        var sources = timeCredits.Select(item => new LessonCreditSource
        {
            Id = item.Id,
            Credits = item.Kredit,
            PeopleLimit = item.PocetLidi,
            PurchasedAt = DateTimeExtensions.FromUnix(item.KreditUnixTime),
            ValidFrom = item.PlatnostOd,
            ValidTo = item.PlatnostDo,
            IsActive = item.Aktivni,
            Months = item.PocetMesicu,
            PausedAt = item.PlatnostUnixBreak > 0 ? DateTimeExtensions.FromUnix(item.PlatnostUnixBreak.Value) : null
        });
        var debt = allowDebt ? (int)Math.Min(int.MaxValue, Math.Max(0L, (long)transaction.Kredit - normal)) : 0;
        var plan = LessonPaymentCalculator.Calculate(requested, sources, normal, debt, Array.Empty<LessonCreditDemand>());
        if (!plan.CanReserve)
        {
            return false;
        }
        foreach (var part in plan.Parts)
        {
            var debit = (UserTransakce)transaction.Clone();
            debit.Kredit = part.Credits;
            debit.Increment = ++increment;
            if (part.Type == "time")
            {
                var wallet = timeCredits.Single(item => item.Id == part.SourceId);
                if (!wallet.Aktivni)
                {
                    wallet.Aktivni = true;
                    wallet.PlatnostOdUnix = transaction.UnixTime;
                    wallet.PlatnostDoUnix = DateTimeExtensions.ToUnix(part.ValidTo.Value.DateTime);
                }
                if (wallet.PlatnostUnixBreak > 0)
                {
                    wallet.PlatnostDoUnix = DateTimeExtensions.ToUnix(part.ValidTo.Value.DateTime);
                    wallet.PlatnostUnixBreak = null;
                }
                wallet.Kredit -= part.Credits;
                debit.PlatbaId = wallet.PlatbaId;
                debit.PlatnostOdUnix = wallet.PlatnostOdUnix;
                debit.PlatnostDoUnix = wallet.PlatnostDoUnix;
                debit.PlatnostOd = wallet.PlatnostOd;
                debit.PlatnostDo = wallet.PlatnostDo;
                debit.Zbytek = wallet.Kredit;
                debit.ZbyvaDni = (wallet.PlatnostDoUnix - transaction.UnixTime) / (60 * 60 * 24);
                InsertTransactionTime(debit);
                repUserCreditsTime.Update(wallet);
            }
            else
            {
                normal -= part.Credits;
                debit.Zbytek = normal;
                debit.ZbyvaDni = 0;
                InsertTransactionStandard(debit);
            }
        }
        scope?.Commit();
        ClearZbytekUzivatele(transaction.UserId);
        return true;
    }
}
