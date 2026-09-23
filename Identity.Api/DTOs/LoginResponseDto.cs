namespace Identity.Api.DTOs;
public record LoginResponseDto(string AccessToken, DateTime ExpiresAtUtc, string UserName, string DisplayName, IReadOnlyList<string> Roles, IReadOnlyList<PermissionAccessDto> Permissions, long SessionId);
