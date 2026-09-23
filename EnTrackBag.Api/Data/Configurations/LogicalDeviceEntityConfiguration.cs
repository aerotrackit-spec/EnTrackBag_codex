using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EnTrackBag.Api.Data.Configurations;

public class LogicalDeviceEntityConfiguration : IEntityTypeConfiguration<LogicalDeviceEntity>
{
    public void Configure(EntityTypeBuilder<LogicalDeviceEntity> e)
    {
        e.ToTable("LogicalDevice", "dbo");
        e.HasKey(x => x.ID);
    }
}
