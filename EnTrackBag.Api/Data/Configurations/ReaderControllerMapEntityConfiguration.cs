using EnTrackBag.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EnTrackBag.Api.Data.Configurations;

public class ReaderControllerMapEntityConfiguration : IEntityTypeConfiguration<ReaderControllerMapEntity>
{
    public void Configure(EntityTypeBuilder<ReaderControllerMapEntity> e)
    {
        e.ToTable("ReaderControllerMap", "dbo");
        e.HasKey(x => x.TID);
    }
}
