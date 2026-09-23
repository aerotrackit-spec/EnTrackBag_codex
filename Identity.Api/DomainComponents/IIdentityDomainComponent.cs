using Identity.Api.DTOs;
namespace Identity.Api.DomainComponents;
public interface IIdentityDomainComponent { Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string? ip, string? userAgent, string? machineName, CancellationToken ct); }
