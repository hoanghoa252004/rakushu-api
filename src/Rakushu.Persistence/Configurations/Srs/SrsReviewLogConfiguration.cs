using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsReviewLogConfiguration : IEntityTypeConfiguration<SrsReviewLog>
{
    public void Configure(EntityTypeBuilder<SrsReviewLog> builder)
    {
        builder.ToTable("srs_review_logs", "srs", table =>
        {
            table.HasCheckConstraint("ck_srs_review_logs_single_source",
                "(srs_session_item_id IS NOT NULL AND quiz_attempt_item_id IS NULL) OR (srs_session_item_id IS NULL AND quiz_attempt_item_id IS NOT NULL)");
            table.HasCheckConstraint("ck_srs_review_logs_elapsed_ms", "elapsed_ms IS NULL OR elapsed_ms >= 0");
        });
        builder.HasKey(log => log.Id);
        builder.Property(log => log.Id).HasConversion(id => id.Value, value => SrsReviewLogId.From(value));
        builder.Property(log => log.SrsCardId).HasConversion(id => id.Value, value => SrsCardId.From(value));
        builder.Property(log => log.SrsSessionItemId).HasConversion(
            id => id == null ? (Guid?)null : id.Value,
            value => value == null ? null : SrsSessionItemId.From(value.Value));
        builder.Property(log => log.QuizAttemptItemId).HasConversion(
            id => id == null ? (Guid?)null : id.Value,
            value => value == null ? null : QuizAttemptItemId.From(value.Value));
        builder.Property(log => log.Rating).HasMaxLength(20).IsRequired();
        builder.Property(log => log.SchedulerVersion).HasMaxLength(30).HasDefaultValue("SM2-v1").IsRequired();
        builder.HasOne(log => log.SrsCard).WithMany().HasForeignKey(log => log.SrsCardId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<SrsSessionItem>().WithMany().HasForeignKey(log => log.SrsSessionItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<QuizAttemptItem>().WithMany().HasForeignKey(log => log.QuizAttemptItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(log => log.SrsSessionItemId).IsUnique();
        builder.HasIndex(log => log.QuizAttemptItemId).IsUnique();
        builder.HasIndex(log => new { log.SrsCardId, log.ReviewedAt });
    }
}
