using Identity.Api.Data.Entities; using Microsoft.EntityFrameworkCore; using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Identity.Api.Data.Configurations;
public class RolePermissionEntityConfiguration : IEntityTypeConfiguration<RolePermissionEntity>
{ public void Configure(EntityTypeBuilder<RolePermissionEntity> e){ e.ToTable("RolePermissions","dbo"); e.HasKey(x=>new{x.RoleId,x.PermissionId,x.AccessTypeId}); e.HasOne(x=>x.Role).WithMany(x=>x.RolePermissions).HasForeignKey(x=>x.RoleId); e.HasOne(x=>x.Permission).WithMany(x=>x.RolePermissions).HasForeignKey(x=>x.PermissionId); e.HasOne(x=>x.AccessType).WithMany(x=>x.RolePermissions).HasForeignKey(x=>x.AccessTypeId); } }
