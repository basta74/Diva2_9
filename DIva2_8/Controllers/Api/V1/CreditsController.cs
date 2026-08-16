using System.Security.Claims;
using Diva2.Core.Api.V1;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diva2Web.Controllers.Api.V1;

[ApiController]
[Authorize(AuthenticationSchemes = ApiTokenAuthenticationHandler.AuthenticationScheme)]
[Route("api/v1/credits")]
public sealed class CreditsController : ControllerBase
{
    private readonly IObjednavkyService reservationService;
    private readonly IPobockaService branchService;

    public CreditsController(IObjednavkyService reservationService, IPobockaService branchService)
    {
        this.reservationService = reservationService;
        this.branchService = branchService;
    }

    [HttpGet("me")]
    public ActionResult<IReadOnlyList<ApiBranchCredits>> GetMine()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // The account page must reflect credits added in administration immediately.
        var balance = reservationService.GetZbytekUzivatele(userId, false);
        var nowUnix = DateTimeOffset.Now.ToUnixTimeSeconds();
        var branches = branchService.GetPobocky()
            .Where(branch => branch.Visible && !string.Equals(branch.Name, "Video", StringComparison.OrdinalIgnoreCase))
            .OrderBy(branch => branch.Order)
            .ToList();
        var cashDesks = branches.GroupBy(branch => branch.PokladnaId).ToList();
        var creditBlocks = cashDesks
            .Select(group =>
            {
                var branch = group.First();
                balance.Kredity.TryGetValue(group.Key, out var unlimitedCredits);
                var showUnlimited = group.Any(item => IsEnabled(branchService.GetPobockaInis(item.Id), "kreditNeomezeny"));
                var showTime = group.Any(item => IsEnabled(branchService.GetPobockaInis(item.Id), "kreditCasovy"));
                var timeCredits = balance.KredityCas
                    .Where(credit => credit.PokladnaId == group.Key && credit.PlatnostDoUnix > nowUnix)
                    .Select(credit => new ApiTimeCredit
                    {
                        Credits = credit.Kredit,
                        IsActive = credit.Aktivni,
                        ValidFrom = credit.Aktivni ? new DateTimeOffset(credit.PlatnostOd) : null,
                        ValidTo = credit.Aktivni ? new DateTimeOffset(credit.PlatnostDo) : null
                    })
                    .ToList();
                return new
                {
                    Branch = branch,
                    BranchName = string.Join(", ", group.Select(item => item.Name)),
                    ShowUnlimited = showUnlimited,
                    UnlimitedCredits = unlimitedCredits,
                    ShowTime = showTime,
                    TimeCredits = timeCredits
                };
            })
            .Where(item => item.ShowUnlimited || item.ShowTime && item.TimeCredits.Count > 0)
            .ToList();
        var showBranchNames = creditBlocks.Count > 1;
        var result = creditBlocks.Select(item => new ApiBranchCredits
        {
            BranchId = item.Branch.Id,
            BranchName = showBranchNames ? item.BranchName : string.Empty,
            ShowUnlimitedCredits = item.ShowUnlimited,
            UnlimitedCredits = item.UnlimitedCredits,
            ShowTimeCredits = item.ShowTime,
            TimeCredits = item.TimeCredits
        }).ToList();
        return Ok(result);
    }

    private static bool IsEnabled(IEnumerable<Diva2.Core.Main.Pobocky.PobockaIni> settings, string key)
    {
        var value = settings.FirstOrDefault(item => string.Equals(item.Name, key, StringComparison.OrdinalIgnoreCase))?.Value;
        return value == "1" || bool.TryParse(value, out var enabled) && enabled;
    }
}
