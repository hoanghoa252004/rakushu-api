using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsCardStateConfiguration : IEntityTypeConfiguration<SrsCardState>
{
    public void Configure(EntityTypeBuilder<SrsCardState> builder)
    {
        builder.Metadata.SetSchema("srs");
        builder.ToTable("srs_card_states", "srs", table =>
        {
            table.HasCheckConstraint("ck_srs_card_states_repetitions", "repetitions >= 0");
            table.HasCheckConstraint("ck_srs_card_states_lapses", "lapses >= 0");
            table.HasCheckConstraint("ck_srs_card_states_interval_days", "interval_days >= 0");
            table.HasCheckConstraint("ck_srs_card_states_easiness_factor", "easiness_factor >= 1.3");
        });

        builder.HasKey(state => state.SrsCardId);
        builder.Property(state => state.SrsCardId).HasConversion(id => id.Value, value => SrsCardId.From(value));
        builder.Property(state => state.EasinessFactor).HasDefaultValue(2.5).IsRequired();
        builder.Property(state => state.Repetitions).HasDefaultValue(0).IsRequired();
        builder.Property(state => state.Lapses).HasDefaultValue(0).IsRequired();
        builder.Property(state => state.IntervalDays).HasDefaultValue(0).IsRequired();

        builder.HasOne(state => state.SrsCard).WithOne(card => card.State)
            .HasForeignKey<SrsCardState>(state => state.SrsCardId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(state => state.DueAt);
    }
}