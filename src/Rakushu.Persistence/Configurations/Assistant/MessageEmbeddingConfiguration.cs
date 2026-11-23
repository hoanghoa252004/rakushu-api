using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Assisstant.Message;
using Rakushu.Domain.Entities.Assisstant.Message.MessageEmbedding;

namespace Rakushu.Persistence.Configurations.Assistant;

internal sealed class MessageEmbeddingConfiguration : IEntityTypeConfiguration<MessageEmbedding>
{
	public void Configure(EntityTypeBuilder<MessageEmbedding> builder)
	{
		builder.Metadata.SetSchema("assistant");

		builder.HasKey(e => e.Id);
		builder.Property(e => e.Id)
			.HasConversion(id => id.Value, value => MessageEmbeddingId.From(value));

		builder.Property(e => e.MessageId)
			.HasConversion(id => id.Value, value => MessageId.From(value))
			.IsRequired();
		builder.HasOne(e => e.Message)
			.WithOne(m => m.Embedding)
			.HasForeignKey<MessageEmbedding>(e => e.MessageId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasIndex(e => e.MessageId).IsUnique();

		builder.Property(e => e.SourceContent).IsRequired();
		builder.Property(e => e.ModelId).HasMaxLength(200).IsRequired();
		builder.Property(e => e.Dimensions).IsRequired();
		builder.Property(e => e.Embedding)
			.HasColumnType("vector(1024)")
			.IsRequired();
	}
}
