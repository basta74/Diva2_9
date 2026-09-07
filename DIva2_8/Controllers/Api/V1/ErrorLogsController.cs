using System.Security.Claims;
using Diva2.Core.Api.V1;
using Diva2.Core.Main.Diagnostics;
using Diva2Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diva2Web.Controllers.Api.V1;

[ApiController]
[Authorize(AuthenticationSchemes = ApiTokenAuthenticationHandler.AuthenticationScheme)]
[Route("api/v1/error-logs")]
public sealed class ErrorLogsController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;

    public ErrorLogsController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ApiMobileAppErrorRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { message = "Message is required." });
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var error = new MobileAppError
        {
            UserId = userId,
            LessonId = request.LessonId,
            OccurredAt = request.OccurredAt?.UtcDateTime ?? DateTime.UtcNow,
            ReceivedAt = DateTime.UtcNow,
            Platform = Limit(request.Platform, 32, "Android"),
            OsVersion = Limit(request.OsVersion, 64),
            DeviceManufacturer = Limit(request.DeviceManufacturer, 100),
            DeviceModel = Limit(request.DeviceModel, 100),
            AppVersion = Limit(request.AppVersion, 32),
            AppBuild = Limit(request.AppBuild, 32),
            Page = Limit(request.Page, 200),
            Action = Limit(request.Action, 200),
            ExceptionType = Limit(request.ExceptionType, 500),
            Message = Limit(request.Message, 4000),
            StackTrace = Limit(request.StackTrace, 32000),
            InnerException = Limit(request.InnerException, 16000)
        };

        dbContext.MobileAppErrors.Add(error);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Created($"api/v1/error-logs/{error.Id}", new { error.Id });
    }

    private static string Limit(string? value, int maximumLength, string defaultValue = "")
    {
        var result = string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        return result.Length <= maximumLength ? result : result[..maximumLength];
    }
}
