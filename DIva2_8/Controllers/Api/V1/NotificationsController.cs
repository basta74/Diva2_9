using System.Security.Claims;
using Diva2.Core.Main.Notifications;
using Diva2.Services.Managers.Notifications;
using Diva2Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diva2Web.Controllers.Api.V1;

[ApiController]
[Authorize(AuthenticationSchemes = ApiTokenAuthenticationHandler.AuthenticationScheme)]
[Route("api/v1/notifications")]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationService notifications;
    public NotificationsController(INotificationService notifications) => this.notifications = notifications;
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        var items = await notifications.GetForUserAsync(UserId, cancellationToken);
        return Ok(items.Select(x => new
        {
            x.Id,
            x.LessonId,
            type = x.Type.ToString(),
            x.Title,
            x.Text,
            x.CreatedAt,
            x.SentAt,
            x.DeliveredAt,
            x.ReadAt,
            x.ReactedAt,
            reaction = x.Reaction.ToString(),
            lesson = x.Lesson is null ? null : new { x.Lesson.Id, x.Lesson.Nazev, startsAt = x.Lesson.DatumHodina }
        }));
    }

    [HttpPost("{id:int}/delivered")]
    public async Task<IActionResult> Delivered(int id, [FromQuery] int? deviceId, CancellationToken cancellationToken) =>
        await notifications.MarkDeliveredAsync(id, UserId, deviceId, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> Read(int id, CancellationToken cancellationToken) =>
        await notifications.MarkReadAsync(id, UserId, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("{id:int}/react")]
    public async Task<IActionResult> React(int id, [FromBody] ReactRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<NotificationReaction>(request.Reaction, true, out var reaction) || reaction == NotificationReaction.None)
            return BadRequest(new { message = "Reaction must be Confirmed or Declined." });
        return await notifications.ReactAsync(id, UserId, reaction, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpPost("devices")]
    public async Task<ActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PushToken) || request.PushToken.Length > 512)
            return BadRequest(new { message = "PushToken is required and can have at most 512 characters." });
        if (!Enum.TryParse<DevicePlatform>(request.Platform, true, out var platform))
            return BadRequest(new { message = "Platform must be Android or Ios." });
        var device = await notifications.RegisterDeviceAsync(UserId, request.PushToken.Trim(), platform, cancellationToken);
        return Ok(new { device.Id, platform = device.Platform.ToString(), device.RegisteredAt, device.LastSeenAt, device.Active });
    }

    [HttpDelete("devices/{id:int}")]
    public async Task<IActionResult> DeactivateDevice(int id, CancellationToken cancellationToken) =>
        await notifications.DeactivateDeviceAsync(id, UserId, cancellationToken) ? NoContent() : NotFound();

    public sealed record ReactRequest(string Reaction);
    public sealed record RegisterDeviceRequest(string PushToken, string Platform);
}
