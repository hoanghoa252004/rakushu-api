using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.User.Profile;
using Rakushu.Domain.Entities.User.Profile.Interest;

namespace Rakushu.Persistence.Configurations;

internal sealed class InterestConfiguration : IEntityTypeConfiguration<Interest>
{
	public void Configure(EntityTypeBuilder<Interest> builder)
	{
		// Id
		builder.HasKey(i => i.Id);
		builder.Property(i => i.Id)
			.HasConversion(
				id => id.Value,
				value => InterestId.From(value));

		// ProfileId
		builder.Property(i => i.ProfileId)
			.HasConversion(
				id => id.Value,
				value => ProfileId.From(value))
			.IsRequired();
		builder.HasOne(i => i.Profile)
			.WithMany(p => p.Interests)
			.HasForeignKey(i => i.ProfileId)
			.OnDelete(DeleteBehavior.Cascade);

		// ContentCategoryId
		builder.Property(i => i.ContentCategoryId)
			.HasConversion(
				id => id.Value,
				value => ContentCategoryId.From(value))
			.IsRequired();
		builder.HasOne(i => i.ContentCategory)
			.WithMany(cc => cc.Interests)
			.HasForeignKey(i => i.ContentCategoryId)
			.OnDelete(DeleteBehavior.Cascade);

		// Priority
		builder.Property(i => i.Priority)
			.IsRequired();
	}
}
