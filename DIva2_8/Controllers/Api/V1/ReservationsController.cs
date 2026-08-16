using System.Security.Claims;
using Diva2.Core.Api.V1;
using Diva2.Services.Managers.Platby;
using Diva2.Services.Managers.Pobocky;
using Diva2.Services.Managers.Setting;
using Diva2.Core.Extensions;
using Diva2.Core.Main.Trans;
using Diva2Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diva2Web.Controllers.Api.V1;

[ApiController]
[Authorize(AuthenticationSchemes = ApiTokenAuthenticationHandler.AuthenticationScheme)]
[Route("api/v1/reservations")]
public sealed class ReservationsController : ControllerBase
{
    private readonly IObjednavkyService reservationService;
    private readonly ILekceService lessonService;
    private readonly IPobockaService branchService;

    public ReservationsController(IObjednavkyService reservationService, ILekceService lessonService, IPobockaService branchService)
    {
        this.reservationService = reservationService;
        this.lessonService = lessonService;
        this.branchService = branchService;
    }

    [HttpPost("lessons/{lessonId:int}")]
    public ActionResult<ApiCreateReservationResponse> Create(int lessonId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var lesson = lessonService.GetById(lessonId);
        if (lesson is null) return NotFound();
        var branch = branchService.GetPobocky().FirstOrDefault(item => item.Id == lesson.PobockaId && item.Visible);
        if (branch is null) return NotFound();
        if (lesson.Zauctovano || lesson.DatumHodina <= DateTime.Now)
            return BadRequest(Failure("Na tuto hodinu se již nelze přihlásit."));

        var existing = reservationService.GetByLekce(lessonId, false);
        var currentUserReservations = existing.Where(item => item.UserId == userId && item.Aktivni).ToList();
        var maxPerUser = GetSettingInt(branch.Id, "lekce_max_count_per_user", 0);
        if (maxPerUser > 0 && currentUserReservations.Count >= maxPerUser)
            return Conflict(Failure($"Na jednu hodinu lze vytvořit nejvýše {maxPerUser} objednávek."));

        var orderedReservations = reservationService.GetObjednaneLekceUzivatele(userId);
        var orderedCredits = orderedReservations.Where(item => item.Lekce is not null).Sum(item => item.Lekce.Kredit);
        var orderCount = reservationService.GetPocetObjednavekUzivatele(userId);
        var balance = reservationService.GetZbytekUzivatele(userId, false);
        var normalCredits = balance.Kredity.FirstOrDefault(item => item.Key == branch.PokladnaId).Value;
        var lessonUnix = DateTimeExtensions.ToUnix(lesson.Datum);
        var timeCredits = balance.KredityCas.Where(item => item.PlatnostDoUnix > lessonUnix).Sum(item => item.Kredit);
        var allowedMinus = GetSettingInt(branch.Id, orderCount > 0 ? "maxKreditDoMinusu" : "maxKreditDoMinusu1", 0);
        var normalEnabled = GetSettingBool(branch.Id, "kreditNeomezeny");
        var timeEnabled = GetSettingBool(branch.Id, "kreditCasovy");
        var availableCredits = (normalEnabled ? normalCredits : 0) + (timeEnabled ? timeCredits : 0);
        if ((!normalEnabled && !timeEnabled) || availableCredits + allowedMinus < orderedCredits + lesson.Kredit)
            return BadRequest(Failure("Pro tuto objednávku nemáte dostatek kreditu."));

        var reservation = new LekceUser
        {
            UserId = userId,
            LekceId = lesson.Id,
            PobockaId = branch.Id,
            PokladnaId = branch.PokladnaId,
            Poradi = existing.Count + 1,
            KontCislo = reservationService.GetRandom(existing),
            Nahradnik = existing.Count >= lesson.PocetMist,
            NahradnikJa = existing.Count >= lesson.PocetMist,
            Premiera = orderCount == 0,
            Aktivni = true,
            DoMzdy = true,
            Unix = lessonUnix,
            Datum = lesson.Datum
        };
        balance.SetZbytekToActualLekceUser(reservation, lesson);
        var loginLog = new UserLekceLogIn(reservation)
        {
            PocetZakazniku = existing.Count,
            PocetMist = lesson.PocetMist,
            ProvedlId = userId,
            ObjednanychLekci = orderCount,
            ObjednanychVLekci = currentUserReservations.Count,
            KreditInit = allowedMinus,
            KreditLekce = lesson.Kredit,
            Ts = DateTime.Now,
            ObjednaneKredity = orderedCredits,
            ZbytekKredit = normalCredits,
            ZbytekKreditCas = timeCredits,
            Pridat = true,
            FromAdministrace = false
        };
        reservationService.Insert(reservation);
        reservationService.ClearObjednaneLekceUzivatele(userId);

        lesson.PocetZakazniku = existing.Count(item => item.Aktivni) + 1;
        lessonService.Update(lesson);
        reservationService.AddUserChange(new UserLekceChange
        {
            LekceId = lesson.Id,
            UserId = userId,
            ProvedlId = userId,
            Status = "+",
            Ts = DateTime.Now
        });
        reservationService.AddUserChangeLogIn(loginLog);

        var status = reservation.Poradi > lesson.PocetMist ? "waitingList" : "customer";
        return Ok(new ApiCreateReservationResponse
        {
            Success = true,
            ReservationStatus = status,
            Message = status == "waitingList" ? "Byli jste přidáni jako náhradník." : "Objednávka byla vytvořena."
        });
    }

    private int GetSettingInt(int branchId, string key, int defaultValue)
    {
        var value = branchService.GetPobockaInis(branchId).FirstOrDefault(item => item.Name == key)?.Value;
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    private bool GetSettingBool(int branchId, string key)
    {
        var value = branchService.GetPobockaInis(branchId).FirstOrDefault(item => item.Name == key)?.Value;
        return value == "1" || bool.TryParse(value, out var result) && result;
    }

    private static ApiCreateReservationResponse Failure(string message) => new() { Success = false, Message = message };

    [HttpGet("me")]
    public ActionResult<IReadOnlyList<ApiMyReservation>> GetMine()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = reservationService.GetObjednaneLekceUzivatele(userId)
            .Where(item => item.Lekce is not null && item.Lekce.DatumHodina >= DateTime.Now)
            .OrderBy(item => item.Lekce.DatumHodina)
            .Select(item => new ApiMyReservation
            {
                LessonId = item.LekceId,
                BranchId = item.PobockaId,
                LessonName = item.Lekce.Nazev ?? string.Empty,
                StartsAt = new DateTimeOffset(item.Lekce.DatumHodina),
                ReservationStatus = item.Poradi > item.Lekce.PocetMist ? "waitingList" : "customer"
            })
            .ToList();
        return Ok(result);
    }
}
