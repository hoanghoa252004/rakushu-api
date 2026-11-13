using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SavedItemConfiguration : IEntityTypeConfiguration<SavedItem>
{
	public void Configure(EntityTypeBuilder<SavedItem> builder)
	{
		builder.ToTable("saved_items", "srs");

		builder.HasKey(si => si.Id);
		builder.Property(si => si.Id)
			.HasConversion(
				id => id.Value,
				value => SavedItemId.From(value));

		builder.Property(si => si.UserId)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();

		builder.HasOne(si => si.User)
			.WithMany()
			.HasForeignKey(si => si.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(si => si.KnowledgeMeaningId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeMeaningId.From(value))
			.IsRequired();

		builder.HasOne(si => si.KnowledgeMeaning)
			.WithMany()
			.HasForeignKey(si => si.KnowledgeMeaningId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Ignore(si => si.Expression);
		builder.Ignore(si => si.Reading);
		builder.Ignore(si => si.Meaning);
		builder.Ignore(si => si.IsRevoked);

		builder.Property(si => si.IsActive)
			.HasColumnName("is_active")
			.HasDefaultValue(true)
			.IsRequired();

		builder.Property(si => si.SavedAt)
			.HasColumnName("saved_at")
			.IsRequired();

		builder.HasIndex(si => new { si.UserId, si.KnowledgeMeaningId })
			.IsUnique();

		builder.HasOne(si => si.Schedule)
			.WithOne(sch => sch.SavedItem)
			.HasForeignKey<SrsSchedule>(sch => sch.SavedItemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
