using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Assisstant.Conversation;
using Rakushu.Domain.Entities.Assisstant.Message;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;

namespace Rakushu.Persistence.Configurations.Assistant;

internal sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
	public void Configure(EntityTypeBuilder<Message> builder)
	{
		builder.Metadata.SetSchema("assistant");

		builder.HasKey(m => m.Id);
		builder.Property(m => m.Id)
			.HasConversion(id => id.Value, value => MessageId.From(value));

		builder.Property(m => m.ConversationId)
			.HasConversion(id => id.Value, value => ConversationId.From(value))
			.IsRequired();
		builder.HasOne(m => m.Conversation)
			.WithMany(c => c.Messages)
			.HasForeignKey(m => m.ConversationId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(m => m.TranscriptSegmentId)
			.HasConversion(id => id.Value, value => TranscriptSegmentId.From(value))
			.IsRequired();
		builder.HasOne(m => m.TranscriptSegment)
			.WithMany()
			.HasForeignKey(m => m.TranscriptSegmentId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(m => m.LearningUnitId)
			.HasConversion(
				id => id == null ? (Guid?)null : id.Value,
				value => value == null ? null : Rakushu.Domain.Entities.Material.LearningUnit.LearningUnitId.From(value.Value));
		builder.HasOne(m => m.LearningUnit)
			.WithMany()
			.HasForeignKey(m => m.LearningUnitId)
			.IsRequired(false)
			.OnDelete(DeleteBehavior.SetNull);

		builder.Property(m => m.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
		builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
		builder.Property(m => m.Content);
		builder.Property(m => m.ErrorMessage).HasMaxLength(1000);
		builder.Property(m => m.AiRequestPayload);
		builder.Property(m => m.CreatedAt).IsRequired();

		builder.HasIndex(m => new { m.ConversationId, m.CreatedAt });
	}
}
