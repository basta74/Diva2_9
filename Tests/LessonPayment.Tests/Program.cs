using Diva2.Core.Api.V1;

var starts = new DateTime(2026, 10, 15, 18, 30, 0);
var checks = 0;
void Check(bool condition, string message)
{
    checks++;
    if (!condition)
    {
        throw new Exception(message);
    }
}
LessonCreditDemand Demand(int id = 1, int cost = 10) => new()
{
    LessonId = id, StartsAt = starts, Credits = cost, AllowTime = true, AllowUnlimited = true
};
LessonCreditSource Source(int id = 1, int credit = 100) => new()
{
    Id = id, Credits = credit, PeopleLimit = 1, IsActive = true,
    PurchasedAt = starts.AddMonths(-1), ValidFrom = starts.AddMonths(-1), ValidTo = starts.AddMonths(2), Months = 3
};
ApiLessonPaymentPreview Preview(IEnumerable<LessonCreditSource> sources, int normal = 40,
    IEnumerable<LessonCreditDemand>? existing = null, LessonCreditDemand? target = null, int minus = 0)
    => LessonPaymentCalculator.Calculate(target ?? Demand(), sources, normal, minus, existing ?? []);

var result = Preview([]);
Check(result.CanReserve && result.AvailableCredits == 40 && result.Parts.Single().Credits == 10, "40 - 10 unlimited");
result = Preview([Source()]);
Check(result.Parts.Single().Type == "time" && result.ValidCredits == 140, "Time credits first");
var newer = Source(2);
newer.PurchasedAt = starts;
result = Preview([newer, Source()]);
Check(result.Parts.Single().SourceId == 1, "Oldest purchase first independent of list order");
result = Preview([Source(1, 4), newer], 0);
Check(result.Parts.Count == 2 && result.Parts[0].Credits == 4 && result.Parts[1].Credits == 6, "Split across time wallets");
result = Preview([Source(1, 4)]);
Check(result.Parts[0].Credits == 4 && result.Parts[1].Type == "unlimited" && result.Parts[1].Credits == 6, "Time then unlimited split");
var expired = Source();
expired.ValidTo = starts;
result = Preview([expired], 0);
Check(!result.CanReserve && result.ValidCredits == 0, "Expiry at exact lesson time excluded");
var future = Source();
future.ValidFrom = starts.AddMinutes(1);
Check(!Preview([future], 0).CanReserve, "Future validity excluded");
future.ValidFrom = starts;
Check(Preview([future], 0).CanReserve, "Validity starts inclusively");
result = Preview([Source()], 40, [Demand()]);
Check(result.Parts.Single().Type == "unlimited", "Second person cannot use single-person time pack");
Check(!Preview([Source()], 0, [Demand()]).CanReserve, "Single-person pack alone cannot pay another person");
var family = Source();
family.PeopleLimit = 2;
Check(Preview([family], 0, [Demand()]).CanReserve, "Two-person pack permits second entry");
Check(!Preview([family], 0, [Demand(), Demand()]).CanReserve, "Two-person pack rejects third entry");
result = Preview([], 40, [Demand(2, 30)]);
Check(result.CanReserve && result.AvailableCredits == 10, "Pending reservations reduce available credits");
Check(!Preview([], 40, [Demand(2, 40)]).CanReserve, "Pending credits cannot be spent twice");
result = Preview([], 4, minus: 6);
Check(result.CanReserve && result.Parts.Single().Credits == 10, "Configured overdraft");
Check(!Preview([], 4, minus: 5).CanReserve, "Overdraft limit enforced");
var timeOnly = Demand();
timeOnly.AllowUnlimited = false;
Check(!Preview([], 100, target: timeOnly, minus: 100).CanReserve, "Disabled unlimited credit and overdraft excluded");
var normalOnly = Demand();
normalOnly.AllowTime = false;
Check(!Preview([Source()], 0, target: normalOnly).CanReserve, "Disabled time credits excluded");
var inactive = Source();
inactive.IsActive = false;
inactive.ValidFrom = DateTime.MinValue;
inactive.ValidTo = DateTime.MaxValue;
result = Preview([inactive], 0);
Check(result.CanReserve && result.Parts.Single().ActivatesOnUse && result.Parts.Single().ValidTo!.Value.DateTime == starts.AddMonths(3), "Inactive pack activation forecast");
var later = Demand(2);
later.StartsAt = starts.AddMonths(4);
Check(!Preview([inactive], 0, [Demand()], later).CanReserve, "Simulated activation expiry applies to later lessons");
Check(inactive.Credits == 100 && !inactive.IsActive, "Preview never mutates account credits");
Check(Preview([], 0, target: Demand(cost: 0)).CanReserve, "Free lesson");
var second = Demand();
second.EntryIndex = 1;
Check(Preview([Source()], 40, target: second).Parts.Single().Type == "unlimited", "Accounting second entry matches preview");
Check(!Preview([Source(), Source(2)], 0, [Demand()]).CanReserve, "Buying two personal packs does not allow a second person");
var paused = Source();
paused.PausedAt = starts.AddDays(-30);
paused.ValidTo = starts.AddDays(-10);
Check(Preview([paused], 0).Parts.Single().ValidTo!.Value.DateTime == starts.AddDays(20), "Paused pack resumes remaining validity");
Check(Preview([], int.MaxValue, minus: 10).CanReserve, "Available credit sum does not overflow in overdraft calculation");
Console.WriteLine($"Passed {checks} payment checks.");
