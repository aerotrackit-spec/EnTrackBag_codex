using Identity.Api.Data.Entities; using Microsoft.EntityFrameworkCore; using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Identity.Api.Data.Configurations;
public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{ public void Configure(EntityTypeBuilder<RoleEntity> e){ e.ToTable("Roles","dbo"); e.HasKey(x=>x.Id); e.Property(x=>x.Name).HasMaxLength(100).IsRequired(); e.Property(x=>x.Description).HasMaxLength(500); e.HasIndex(x=>x.Name).IsUnique(); } }
