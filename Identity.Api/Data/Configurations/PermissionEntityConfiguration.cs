using Identity.Api.Data.Entities; using Microsoft.EntityFrameworkCore; using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Identity.Api.Data.Configurations;
public class PermissionEntityConfiguration : IEntityTypeConfiguration<PermissionEntity>
{ public void Configure(EntityTypeBuilder<PermissionEntity> e){ e.ToTable("Permissions","dbo"); e.HasKey(x=>x.Id); e.Property(x=>x.Code).HasMaxLength(150).IsRequired(); e.Property(x=>x.Name).HasMaxLength(150).IsRequired(); e.Property(x=>x.Description).HasMaxLength(500); e.HasIndex(x=>x.Code).IsUnique(); } }
