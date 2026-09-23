using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EnTrackBag.Api.Data.Configurations;
public class AntennaEntityConfiguration : IEntityTypeConfiguration<AntennaEntity>
{
    public void Configure(EntityTypeBuilder<AntennaEntity> e)
    {
        e.ToTable("Antennas", "dbo");
        e.HasKey(x => x.ID);
    }
}
