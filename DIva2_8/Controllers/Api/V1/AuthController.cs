using Diva2.Core.Api.V1;
using Diva2.Core.Main.Users;
using Diva2.Data.Infrastructure;
using Diva2.Services.Managers.Users;
using Diva2Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diva2Web.Controllers.Api.V1;

[ApiController]
[AllowAnonymous]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUser8Service userService;
    private readonly UserManager<User8> userManager;
    private readonly IApiTokenService tokenService;
    private readonly IDomainService domainService;

    public AuthController(IUser8Service userService, UserManager<User8> userManager, IApiTokenService tokenService, IDomainService domainService)
    {
        this.userService = userService;
        this.userManager = userManager;
        this.tokenService = tokenService;
        this.domainService = domainService;
    }

    [HttpPost("login")]
    [ProducesResponseType<ApiLoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiLoginResponse>> Login(ApiLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Unauthorized(new { message = "Neplatné přihlašovací údaje." });
        }

        var users = userService.GetByEmail(request.Email.Trim());
        var user = users.Count == 1 ? users[0] : null;
        if (user == null || user.Deleted || !user.Platnost || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Neplatné přihlašovací údaje." });
        }

        var expiresAt = DateTimeOffset.UtcNow.AddHours(12);
        return Ok(new ApiLoginResponse
        {
            AccessToken = tokenService.Create(user.Id, domainService.Domain.name, expiresAt),
            ExpiresAt = expiresAt,
            UserId = user.Id,
            DisplayName = string.Join(" ", new[] { user.Jmeno, user.Prijmeni }.Where(x => !string.IsNullOrWhiteSpace(x)))
        });
    }
}
