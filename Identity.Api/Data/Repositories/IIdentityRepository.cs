using Identity.Api.Data.Entities; using Identity.Api.DTOs;
namespace Identity.Api.Data.Repositories;
public interface IIdentityRepository
{
    Task<UserEntity?> GetUserForLoginAsync(string userName, CancellationToken ct);
    Task<PermissionAccessDto[]> GetPermissionAccessAsync(int userId, CancellationToken ct);
    Task AddSessionAsync(UserSessionEntity session, CancellationToken ct);
    Task AddAuditEventAsync(AuditEventEntity auditEvent, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
