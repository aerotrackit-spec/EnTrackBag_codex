using Identity.Api.DomainComponents;
using Identity.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EnTrackBag.Sessions;
using Identity.Api.Security;
namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IIdentityDomainComponent _identityDomainComponent;
    public AuthController(IIdentityDomainComponent identityDomainComponent)
    {
        _identityDomainComponent = identityDomainComponent;
    }
    [HttpPost("login")]
    [ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken ct)
    {
        var ua = Request.Headers["User-Agent"].ToString();
        // Use the connection address, not an untrusted client-supplied forwarding header.
        var ip = ClientIpAddress.Normalize(HttpContext.Connection.RemoteIpAddress?.ToString());
        var result = await _identityDomainComponent.LoginAsync(request, ip, ua, null, ct);
        return result is null ? Unauthorized() : Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromServices] SessionRepository sessions, CancellationToken ct)
    {
        await sessions.LogoutAsync(long.Parse(User.FindFirstValue("session_id")!),
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!), ct);
        return NoContent();
    }

    [Authorize]
    [HttpPost("activity")]
    public IActionResult Activity() => NoContent();

    [Authorize]
    [HttpGet("session")]
    public IActionResult Session() => NoContent();
}
