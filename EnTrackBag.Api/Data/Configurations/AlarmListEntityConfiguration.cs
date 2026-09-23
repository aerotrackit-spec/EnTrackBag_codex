using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EnTrackBag.Api.Data.Configurations;
public class AlarmListEntityConfiguration : IEntityTypeConfiguration<AlarmListEntity>
{
    public void Configure(EntityTypeBuilder<AlarmListEntity> e)
    {
        e.ToTable("AlarmList", "dbo");
        e.HasKey(x => x.AlarmID);
    }
}
