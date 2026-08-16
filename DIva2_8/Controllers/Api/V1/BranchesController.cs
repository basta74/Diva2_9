using System.Security.Claims;
using Diva2.Core.Api.V1;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Setting;
using Diva2Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diva2Web.Controllers.Api.V1;

[ApiController]
[Authorize(AuthenticationSchemes = ApiTokenAuthenticationHandler.AuthenticationScheme)]
[Route("api/v1/branches")]
public sealed class BranchesController : ControllerBase
{
    private readonly IPobockaService branchService;
    private readonly ILekceService lessonService;
    private readonly ILektorService lectorService;
    private readonly IObjednavkyService reservationService;

    public BranchesController(IPobockaService branchService, ILekceService lessonService, ILektorService lectorService, IObjednavkyService reservationService)
    {
        this.branchService = branchService;
        this.lessonService = lessonService;
        this.lectorService = lectorService;
        this.reservationService = reservationService;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<ApiBranchInfo>> GetBranches()
    {
        var result = branchService.GetPobocky()
            .Where(branch => branch.Visible)
            .OrderBy(branch => branch.Order)
            .Select(branch => new ApiBranchInfo
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Description ?? string.Empty
            })
            .ToList();
        return Ok(result);
    }

    [HttpGet("{branchId:int}/lessons")]
    public ActionResult<IReadOnlyList<ApiLessonInfo>> GetLessons(int branchId)
    {
        if (!branchService.GetPobocky().Any(branch => branch.Id == branchId && branch.Visible))
        {
            return NotFound();
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var reservedLessonIds = reservationService.GetObjednaneLekceUzivatele(userId)
            .Select(item => item.LekceId)
            .ToHashSet();
        var lectors = lectorService.GetAll().ToDictionary(lector => lector.Id);

        var result = lessonService.GetWeaksApi(branchId).Lessons.Select(lesson =>
        {
            lectors.TryGetValue(lesson.Lektor1, out var lector);
            return new ApiLessonInfo
            {
                Id = lesson.Id,
                Name = lesson.Nazev,
                Lector = lector?.Jmeno ?? lector?.Nick ?? string.Empty,
                StartsAt = new DateTimeOffset(lesson.DatumHodina),
                Capacity = lesson.PocetMist,
                Occupied = lesson.PocetZakazniku,
                IsReservedByCurrentUser = reservedLessonIds.Contains(lesson.Id)
            };
        }).ToList();

        return Ok(result);
    }
}
