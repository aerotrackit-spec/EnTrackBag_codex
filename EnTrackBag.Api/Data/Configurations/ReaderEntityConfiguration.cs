using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EnTrackBag.Api.Data.Configurations;
public class ReaderEntityConfiguration : IEntityTypeConfiguration<ReaderEntity>
{
    public void Configure(EntityTypeBuilder<ReaderEntity> e)
    {
        e.ToTable("Readers", "dbo");
        e.HasKey(x => x.ID);
    }
}
