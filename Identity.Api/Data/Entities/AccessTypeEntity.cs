namespace Identity.Api.Data.Entities;
public class AccessTypeEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();
}
