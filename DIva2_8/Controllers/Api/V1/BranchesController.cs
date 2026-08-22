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
    private readonly ILekceTypService lessonTypeService;

    public BranchesController(IPobockaService branchService, ILekceService lessonService, ILektorService lectorService,
        IObjednavkyService reservationService, ILekceTypService lessonTypeService)
    {
        this.branchService = branchService;
        this.lessonService = lessonService;
        this.lectorService = lectorService;
        this.reservationService = reservationService;
        this.lessonTypeService = lessonTypeService;
    }

    [HttpGet("{branchId:int}/configuration")]
    public ActionResult<ApiBranchConfiguration> GetConfiguration(int branchId)
    {
        if (!branchService.GetPobocky().Any(branch => branch.Id == branchId && branch.Visible))
        {
            return NotFound();
        }

        var settings = branchService.GetPobockaInis(branchId)
            .GroupBy(setting => setting.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last().Value ?? string.Empty, StringComparer.OrdinalIgnoreCase);
        var lessonTypes = lessonTypeService.GetAll()
            .Where(type => type.PobockaId == branchId)
            .OrderBy(type => type.Nazev)
            .Select(type => new ApiLessonTypeInfo
            {
                Id = type.Id,
                Name = type.Nazev ?? string.Empty,
                Abbreviation = type.Zkratka ?? string.Empty,
                Description = type.Popis ?? string.Empty
            })
            .ToList();

        return Ok(new ApiBranchConfiguration
        {
            BranchId = branchId,
            Settings = settings,
            LessonTypes = lessonTypes
        });
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
        var reservations = reservationService.GetObjednaneLekceUzivatele(userId)
            .Where(item => item.Aktivni)
            .GroupBy(item => item.LekceId)
            .ToDictionary(group => group.Key, group => group.ToList());
        var lectors = lectorService.GetAll().ToDictionary(lector => lector.Id);

        var result = lessonService.GetWeaksApi(branchId).Lessons.Select(lesson =>
        {
            lectors.TryGetValue(lesson.Lektor1, out var lector);
            reservations.TryGetValue(lesson.Id, out var userReservations);
            var reservationStatus = userReservations is null
                ? null
                : userReservations.Any(item => item.Poradi <= lesson.PocetMist) ? "customer" : "waitingList";
            return new ApiLessonInfo
            {
                Id = lesson.Id,
                Name = lesson.Nazev,
                Lector = lector?.Jmeno ?? lector?.Nick ?? string.Empty,
                StartsAt = new DateTimeOffset(lesson.DatumHodina),
                Capacity = lesson.PocetMist,
                Occupied = lesson.PocetZakazniku,
                IsReservedByCurrentUser = reservationStatus is not null,
                ReservationStatus = reservationStatus,
                LessonTypeId = lesson.TypHodiny
            };
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{branchId:int}/lessons/{lessonId:int}/participants")]
    public ActionResult<IReadOnlyList<ApiLessonParticipant>> GetParticipants(int branchId, int lessonId)
    {
        var lesson = lessonService.GetById(lessonId);
        if (lesson is null || lesson.PobockaId != branchId ||
            !branchService.GetPobocky().Any(branch => branch.Id == branchId && branch.Visible))
            return NotFound();

        var showCustomers = branchService.GetPobockaInis(branchId)
            .FirstOrDefault(setting => string.Equals(setting.Name, "lekceUkazZakazniky", StringComparison.OrdinalIgnoreCase))?.Value;
        var customersEnabled = showCustomers == "1" || bool.TryParse(showCustomers, out var enabled) && enabled;
        if (!customersEnabled)
            return Forbid();

        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = reservationService.GetByLekce(lessonId)
            .Where(item => item.Aktivni && item.User is not null)
            .OrderBy(item => item.Poradi)
            .Select(item => new ApiLessonParticipant
            {
                DisplayName = $"{item.User.Jmeno} {item.User.Prijmeni}".Trim(),
                IsWaitingList = item.Poradi > lesson.PocetMist,
                IsCurrentUser = item.UserId == currentUserId
            })
            .ToList();
        return Ok(result);
    }
}
