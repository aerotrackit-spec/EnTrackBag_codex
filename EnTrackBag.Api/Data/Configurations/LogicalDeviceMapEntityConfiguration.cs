using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EnTrackBag.Api.Data.Configurations;

public class LogicalDeviceMapEntityConfiguration : IEntityTypeConfiguration<LogicalDeviceMapEntity>
{
    public void Configure(EntityTypeBuilder<LogicalDeviceMapEntity> e)
    {
        e.ToTable("LogicalDeviceMap", "dbo");
        e.HasKey(x => x.ID);
    }
}
