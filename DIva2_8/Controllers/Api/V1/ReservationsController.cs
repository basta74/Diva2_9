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

    [HttpGet("lessons/{lessonId:int}/payment-preview")]
    public ActionResult<ApiLessonPaymentPreview> GetPaymentPreview(int lessonId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var lesson = lessonService.GetById(lessonId);
        if (lesson is null)
        {
            return NotFound();
        }
        var branch = branchService.GetPobocky().FirstOrDefault(item => item.Id == lesson.PobockaId && item.Visible);
        if (branch is null)
        {
            return NotFound();
        }
        return Ok(BuildPaymentPreview(userId, lessonId, branch.Id, branch.PokladnaId));
    }

    private ApiLessonPaymentPreview BuildPaymentPreview(int userId, int lessonId, int branchId, int cashDeskId)
    {
        var lesson = lessonService.GetById(lessonId);
        reservationService.ClearObjednaneLekceUzivatele(userId);
        var reservations = reservationService.GetObjednaneLekceUzivatele(userId)
            .Where(item => item.Aktivni && item.Lekce is not null && !item.Lekce.Zauctovano
                && item.PokladnaId == cashDeskId).ToList();
        var balance = reservationService.GetZbytekUzivatele(userId, false);
        var orderCount = reservationService.GetPocetObjednavekUzivatele(userId);
        var sources = balance.KredityCas.Where(item => item.PokladnaId == cashDeskId && item.Platny && item.Kredit > 0)
            .Select(item => new LessonCreditSource
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
        var demands = reservations.Select(item => new LessonCreditDemand
        {
            LessonId = item.LekceId,
            StartsAt = item.Lekce.DatumHodina,
            Credits = item.Lekce.Kredit,
            AllowTime = GetSettingBool(item.PobockaId, "kreditCasovy"),
            AllowUnlimited = GetSettingBool(item.PobockaId, "kreditNeomezeny")
        });
        var requested = new LessonCreditDemand
        {
            LessonId = lessonId,
            StartsAt = lesson.DatumHodina,
            Credits = lesson.Kredit,
            AllowTime = GetSettingBool(branchId, "kreditCasovy"),
            AllowUnlimited = GetSettingBool(branchId, "kreditNeomezeny")
        };
        var preview = LessonPaymentCalculator.Calculate(requested, sources, balance.KredityItem(cashDeskId),
            GetSettingInt(branchId, orderCount > 0 ? "maxKreditDoMinusu" : "maxKreditDoMinusu1", 0), demands);
        preview.ReservationCount = reservations.Count(item => item.LekceId == lessonId);
        preview.TimeCredits = balance.KredityCas
            .Where(item => item.PokladnaId == cashDeskId && item.PlatnostDoUnix > DateTimeOffset.Now.ToUnixTimeSeconds())
            .Sum(item => item.Kredit);
        preview.UnlimitedCredits = balance.KredityItem(cashDeskId);
        preview.MaxReservations = GetSettingInt(branchId, "lekce_max_count_per_user", 0);
        if (lesson.Zauctovano || lesson.DatumHodina <= DateTime.Now)
        {
            preview.CanReserve = false;
            preview.Message = "Na tuto hodinu se již nelze přihlásit.";
        }
        else if (preview.MaxReservations > 0 && preview.ReservationCount >= preview.MaxReservations)
        {
            preview.CanReserve = false;
            preview.Message = $"Na jednu hodinu lze vytvořit nejvýše {preview.MaxReservations} objednávek.";
        }
        else if (!requested.AllowTime && !requested.AllowUnlimited)
        {
            preview.CanReserve = false;
            preview.Message = "Pobočka nemá povolenou úhradu kredity.";
        }
        return preview;
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
        var startsAtUnix = DateTimeExtensions.ToUnix(lesson.DatumHodina);
        var timeCredits = balance.KredityCas.Where(item => item.PokladnaId == branch.PokladnaId
            && item.PlatnostOdUnix <= startsAtUnix && item.PlatnostDoUnix > startsAtUnix).Sum(item => item.Kredit);
        var allowedMinus = GetSettingInt(branch.Id, orderCount > 0 ? "maxKreditDoMinusu" : "maxKreditDoMinusu1", 0);
        var payment = BuildPaymentPreview(userId, lessonId, branch.Id, branch.PokladnaId);
        if (!payment.CanReserve)
        {
            return BadRequest(Failure(payment.Message));
        }

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
            Datum = lesson.Datum,
            Device = ReservationDevice.App
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

    [HttpDelete("lessons/{lessonId:int}")]
    public ActionResult<ApiCreateReservationResponse> Cancel(int lessonId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var lesson = lessonService.GetById(lessonId);
        if (lesson is null)
        {
            return NotFound(Failure("Hodina nebyla nalezena."));
        }

        var branch = branchService.GetPobocky().FirstOrDefault(item => item.Id == lesson.PobockaId && item.Visible);
        if (branch is null)
        {
            return NotFound(Failure("Pobočka nebyla nalezena."));
        }

        if (lesson.Zauctovano)
        {
            return BadRequest(Failure("Z této hodiny se již nelze odhlásit."));
        }

        var canCancel = DateTime.Now <= GetCancellationDeadline(branch.Id, lesson.DatumHodina);
        var canOffer = !canCancel && GetSettingBool(branch.Id, "lekceOdhlasNabidniVolne");
        if (!canCancel && !canOffer)
        {
            return BadRequest(Failure("Z této hodiny se již nelze odhlásit ani nabídnout místo."));
        }

        var reservations = reservationService.GetByLekce(lessonId, false)
            .Where(item => item.Aktivni)
            .ToList();
        var reservation = reservations.FirstOrDefault(item => item.UserId == userId);
        if (reservation is null)
        {
            return NotFound(Failure("Na tuto hodinu nejste přihlášeni."));
        }

        var logoutTime = DateTime.Now;
        var removedWasValid = reservation.Poradi <= lesson.PocetMist;
        var logoutLog = new UserLekceLogOut
        {
            ProvedlId = userId,
            LekceId = lesson.Id,
            UserId = userId,
            Poradi = reservation.Poradi,
            PocetZakazniku = reservations.Count,
            PocetMist = lesson.PocetMist,
            ExistujeNahradnik = reservations.Count > lesson.PocetMist,
            JePlatny = reservation.Poradi <= lesson.PocetMist,
            Ts = logoutTime
        };

        if (canOffer)
        {
            reservation.Aktivni = false;
            reservationService.Update(reservation);
        }
        else
        {
            reservations.Remove(reservation);
            reservationService.Delete(reservation);
            foreach (var remainingReservation in reservations.Where(item => item.Poradi > reservation.Poradi))
            {
                remainingReservation.Poradi--;
            }

            reservationService.Update(reservations);
        }

        reservationService.ClearObjednaneLekceUzivatele(userId);
        lesson.PocetZakazniku = reservations.Count(item => item.Aktivni);
        lessonService.Update(lesson);
        if (removedWasValid)
        {
            reservationService.PromoteFirstWaitingListCustomer(
                lesson.Id,
                canOffer ? reservation.Poradi : null);
        }
        reservationService.AddUserChange(new UserLekceChange
        {
            LekceId = lesson.Id,
            UserId = userId,
            ProvedlId = userId,
            Status = "-",
            Ts = logoutTime
        });
        reservationService.AddUserChangeLog(logoutLog);

        return Ok(new ApiCreateReservationResponse
        {
            Success = true,
            Message = canOffer
                ? "Vaše místo bylo nabídnuto dalším zájemcům."
                : "Z hodiny jste byli odhlášeni."
        });
    }

    private DateTime GetCancellationDeadline(int branchId, DateTime lessonStartsAt)
    {
        if (GetSettingBool(branchId, "lekceOdhlasPevne"))
        {
            var daysBefore = GetSettingInt(branchId, "lekceOdhlasPevneDenPred", 1);
            var timeText = branchService.GetPobockaInis(branchId)
                .FirstOrDefault(item => item.Name == "lekceOdhlasPevneHodDne")?.Value;
            var time = TimeSpan.TryParse(timeText, out var parsedTime)
                ? parsedTime
                : new TimeSpan(23, 59, 0);
            var deadline = lessonStartsAt.Date.AddDays(-daysBefore).Add(time);
            return deadline > lessonStartsAt ? lessonStartsAt.AddHours(-1) : deadline;
        }

        if (GetSettingBool(branchId, "lekceOdhlasPlov"))
        {
            return lessonStartsAt.AddHours(-GetSettingInt(branchId, "lekceOdhlasPlovHod", 1));
        }

        return lessonStartsAt.AddHours(-1);
    }

    [HttpGet("me")]
    public ActionResult<IReadOnlyList<ApiMyReservation>> GetMine()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var now = DateTime.Now;
        var result = reservationService.GetObjednaneLekceUzivatele(userId)
            .Where(item => item.Aktivni && item.Lekce is not null && item.Lekce.DatumHodina >= DateTime.Now)
            .OrderBy(item => item.Lekce.DatumHodina)
            .Select(item => new ApiMyReservation
            {
                LessonId = item.LekceId,
                BranchId = item.PobockaId,
                LessonName = item.Lekce.Nazev ?? string.Empty,
                StartsAt = new DateTimeOffset(item.Lekce.DatumHodina),
                ReservationStatus = item.Poradi > item.Lekce.PocetMist ? "waitingList" : "customer",
                CanCancel = !item.Lekce.Zauctovano && now <= GetCancellationDeadline(item.PobockaId, item.Lekce.DatumHodina),
                CanOffer = !item.Lekce.Zauctovano
                    && now > GetCancellationDeadline(item.PobockaId, item.Lekce.DatumHodina)
                    && GetSettingBool(item.PobockaId, "lekceOdhlasNabidniVolne")
            })
            .ToList();
        return Ok(result);
    }
}
