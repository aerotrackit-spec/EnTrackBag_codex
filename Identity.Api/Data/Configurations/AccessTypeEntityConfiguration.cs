using Identity.Api.Data.Entities; using Microsoft.EntityFrameworkCore; using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Identity.Api.Data.Configurations;
public class AccessTypeEntityConfiguration : IEntityTypeConfiguration<AccessTypeEntity>
{ public void Configure(EntityTypeBuilder<AccessTypeEntity> e){ e.ToTable("AccessTypes","dbo"); e.HasKey(x=>x.Id); e.Property(x=>x.Code).HasMaxLength(50).IsRequired(); e.Property(x=>x.Name).HasMaxLength(100).IsRequired(); e.Property(x=>x.Description).HasMaxLength(500); e.HasIndex(x=>x.Code).IsUnique(); } }
