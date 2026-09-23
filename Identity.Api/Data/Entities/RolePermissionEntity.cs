namespace Identity.Api.Data.Entities;
public class RolePermissionEntity
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public int AccessTypeId { get; set; }
    public DateTime AssignedAt { get; set; }
    public int? AssignedBy { get; set; }
    public RoleEntity Role { get; set; } = null!;
    public PermissionEntity Permission { get; set; } = null!;
    public AccessTypeEntity AccessType { get; set; } = null!;
}
