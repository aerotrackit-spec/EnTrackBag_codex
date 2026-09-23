using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EnTrackBag.Api.Data.Configurations;
public class ControllerEntityConfiguration : IEntityTypeConfiguration<ControllerEntity>
{
    public void Configure(EntityTypeBuilder<ControllerEntity> e)
    {
        e.ToTable("Controllers", "dbo");
        e.HasKey(x => x.ID);
    }
}
