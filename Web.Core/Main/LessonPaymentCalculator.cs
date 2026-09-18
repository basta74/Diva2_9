using System;
using System.Collections.Generic;
using System.Linq;

namespace Diva2.Core.Api.V1;

public sealed class LessonCreditSource
{
    public int Id { get; set; }
    public int Credits { get; set; }
    public int PeopleLimit { get; set; }
    public DateTime PurchasedAt { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
    public int Months { get; set; }
    public DateTime? PausedAt { get; set; }
}

public sealed class LessonCreditDemand
{
    public int LessonId { get; set; }
    public DateTime StartsAt { get; set; }
    public int Credits { get; set; }
    public bool AllowTime { get; set; }
    public bool AllowUnlimited { get; set; }
    public int EntryIndex { get; set; }
}

/// <summary>Read-only forecast. Existing reservations have priority over a new reservation.</summary>
public static class LessonPaymentCalculator
{
    public static ApiLessonPaymentPreview Calculate(LessonCreditDemand requested,
        IEnumerable<LessonCreditSource> sources, int unlimitedCredits, int allowedMinus,
        IEnumerable<LessonCreditDemand> reservations)
    {
        var wallets = sources.OrderBy(source => source.PurchasedAt).ThenBy(source => source.Id)
            .Select(source => new Wallet(source)).ToList();
        var result = new ApiLessonPaymentPreview
        {
            LessonCredits = Math.Max(0, requested.Credits),
            ValidCredits = (requested.AllowUnlimited ? unlimitedCredits : 0)
                + (requested.AllowTime ? wallets.Where(wallet => wallet.IsValid(requested.StartsAt)).Sum(wallet => wallet.Remaining) : 0)
        };
        var normal = unlimitedCredits;
        var unmetReservations = 0;
        var entries = new Dictionary<int, int>();

        List<ApiCreditPaymentPart> Allocate(LessonCreditDemand demand, bool allowDebt)
        {
            var parts = new List<ApiCreditPaymentPart>();
            var remaining = Math.Max(0, demand.Credits);
            var entryIndex = demand.EntryIndex + entries.GetValueOrDefault(demand.LessonId);
            if (demand.AllowTime)
            {
                foreach (var wallet in wallets.Where(wallet => wallet.CanUse(demand, entryIndex)))
                {
                    if (remaining == 0)
                    {
                        break;
                    }
                    var amount = Math.Min(wallet.Remaining, remaining);
                    if (!wallet.Active)
                    {
                        wallet.From = demand.StartsAt;
                        wallet.To = demand.StartsAt.AddMonths(Math.Max(1, wallet.Source.Months));
                        wallet.Active = true;
                    }
                    if (wallet.PausedAt.HasValue)
                    {
                        wallet.To = wallet.ValidUntil(demand.StartsAt);
                        wallet.PausedAt = null;
                    }
                    wallet.Remaining -= amount;
                    remaining -= amount;
                    parts.Add(new ApiCreditPaymentPart
                    {
                        SourceId = wallet.Source.Id,
                        Type = "time",
                        Credits = amount,
                        ValidTo = new DateTimeOffset(wallet.To),
                        ActivatesOnUse = !wallet.Source.IsActive
                    });
                }
            }
            if (demand.AllowUnlimited && remaining > 0)
            {
                var amount = (int)Math.Min(remaining, Math.Max(0L, (long)normal + (allowDebt ? Math.Max(0, allowedMinus) : 0)));
                if (amount > 0)
                {
                    parts.Add(new ApiCreditPaymentPart { Type = "unlimited", Credits = amount });
                    normal -= amount;
                    remaining -= amount;
                }
            }
            entries[demand.LessonId] = entries.GetValueOrDefault(demand.LessonId) + 1;
            return parts;
        }

        foreach (var demand in reservations.OrderBy(item => item.StartsAt).ThenBy(item => item.LessonId))
        {
            var paid = Allocate(demand, true).Sum(part => part.Credits);
            unmetReservations += Math.Max(0, demand.Credits - paid);
        }
        result.AvailableCredits = (requested.AllowUnlimited ? normal : 0)
            + (requested.AllowTime ? wallets.Where(wallet => wallet.CanUse(requested,
                requested.EntryIndex + entries.GetValueOrDefault(requested.LessonId))).Sum(wallet => wallet.Remaining) : 0);
        result.Parts = Allocate(requested, true);
        result.MissingCredits = Math.Max(0, result.LessonCredits - result.Parts.Sum(part => part.Credits));
        result.CanReserve = result.MissingCredits == 0 && unmetReservations == 0;
        if (unmetReservations > 0)
        {
            result.Message = "Kredity nestačí na již vytvořené rezervace.";
        }
        else if (result.MissingCredits > 0)
        {
            result.Message = $"Pro další rezervaci chybí {result.MissingCredits} kreditů.";
        }
        return result;
    }

    private sealed class Wallet
    {
        public LessonCreditSource Source { get; }
        public int Remaining;
        public DateTime From;
        public DateTime To;
        public bool Active;
        public DateTime? PausedAt;

        public Wallet(LessonCreditSource source)
        {
            Source = source;
            Remaining = Math.Max(0, source.Credits);
            From = source.ValidFrom;
            To = source.ValidTo;
            Active = source.IsActive;
            PausedAt = source.PausedAt;
        }

        public DateTime ValidUntil(DateTime startsAt) => PausedAt.HasValue && startsAt > PausedAt.Value
            ? To.Add(startsAt - PausedAt.Value) : To;
        public bool IsValid(DateTime startsAt) => Remaining > 0 && (!Active && Source.Months > 0
            || Active && From <= startsAt && startsAt < ValidUntil(startsAt));
        public bool CanUse(LessonCreditDemand demand, int entryIndex) => IsValid(demand.StartsAt)
            && entryIndex < Math.Max(1, Source.PeopleLimit);
    }
}
