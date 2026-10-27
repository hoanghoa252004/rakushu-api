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
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(si => si.Expression)
			.HasMaxLength(100)
			.IsRequired();

		builder.Property(si => si.Reading)
			.HasMaxLength(100);

		builder.Property(si => si.Meaning)
			.HasMaxLength(500)
			.IsRequired();

		builder.Property(si => si.SavedAt)
			.IsRequired();

		builder.Property(si => si.CreatedAt)
			.IsRequired();

		builder.HasIndex(si => new { si.UserId, si.KnowledgeMeaningId })
			.IsUnique();

		builder.HasOne(si => si.Schedule)
			.WithOne(sch => sch.SavedItem)
			.HasForeignKey<SrsSchedule>(sch => sch.SavedItemId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(si => si.Details)
			.WithOne(d => d.SavedItem)
			.HasForeignKey(d => d.SavedItemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
