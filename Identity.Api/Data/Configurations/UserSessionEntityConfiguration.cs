using Identity.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Identity.Api.Data.Configurations;

public class UserSessionEntityConfiguration : IEntityTypeConfiguration<UserSessionEntity>
{
    public void Configure(EntityTypeBuilder<UserSessionEntity> e)
    {
        e.ToTable("UserSessions", "dbo");
        e.HasKey(x => x.SessionId);
        e.Property(x => x.RemoteIp).HasMaxLength(64);
        e.Property(x => x.UserAgent).HasMaxLength(1000);
        e.Property(x => x.CorrelationId).HasMaxLength(100);
        e.Property(x => x.LogoutReason).HasMaxLength(250);
        e.HasOne(x => x.User).WithMany(x => x.Sessions).HasForeignKey(x => x.UserId);
    }
}
