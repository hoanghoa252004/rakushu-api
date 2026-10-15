using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsSessionConfiguration : IEntityTypeConfiguration<SrsSession>
{
    public void Configure(EntityTypeBuilder<SrsSession> builder)
    {
        builder.ToTable("srs_sessions", "srs", table =>
        {
            table.HasCheckConstraint("ck_srs_sessions_counts", "reviewed_items >= 0 AND total_items >= 0 AND reviewed_items <= total_items");
            table.HasCheckConstraint("ck_srs_sessions_status", "status IN ('InProgress', 'Completed', 'Abandoned')");
            table.HasCheckConstraint("ck_srs_sessions_completed_at", "status <> 'Completed' OR completed_at IS NOT NULL");
        });

        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id)
            .HasConversion(id => id.Value, value => SrsSessionId.From(value));

        builder.Property(session => session.UserId)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .IsRequired();

        builder.Property(session => session.Status)
            .HasConversion(status => status.ToString(), value => Enum.Parse<SrsSessionStatus>(value))
            .HasMaxLength(20)
            .HasDefaultValue(SrsSessionStatus.InProgress)
            .IsRequired();

        builder.HasOne(session => session.User)
            .WithMany()
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(session => session.Items)
            .WithOne(item => item.SrsSession)
            .HasForeignKey(item => item.SrsSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(session => new { session.UserId, session.StartedAt });
    }
}