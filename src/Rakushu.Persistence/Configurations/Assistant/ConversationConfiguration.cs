using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Assisstant.Conversation;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Configurations.Assistant;

internal sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
	public void Configure(EntityTypeBuilder<Conversation> builder)
	{
		builder.Metadata.SetSchema("assistant");

		builder.HasKey(c => c.Id);
		builder.Property(c => c.Id)
			.HasConversion(id => id.Value, value => ConversationId.From(value));

		builder.Property(c => c.LearnerId)
			.HasConversion(id => id.Value, value => UserId.From(value))
			.IsRequired();
		builder.HasOne(c => c.Learner)
			.WithMany()
			.HasForeignKey(c => c.LearnerId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(c => c.VideoId)
			.HasConversion(id => id.Value, value => VideoId.From(value))
			.IsRequired();
		builder.HasOne(c => c.Video)
			.WithMany()
			.HasForeignKey(c => c.VideoId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(c => new { c.LearnerId, c.VideoId }).IsUnique();

		builder.Property(c => c.Title).HasMaxLength(100).IsRequired();
		builder.Property(c => c.CreatedAt).IsRequired();
		builder.Property(c => c.UpdatedAt).IsRequired();
	}
}
