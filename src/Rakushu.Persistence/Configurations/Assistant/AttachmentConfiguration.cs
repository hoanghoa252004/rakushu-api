using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Assisstant.Attachment;
using Rakushu.Domain.Entities.Assisstant.Message;

namespace Rakushu.Persistence.Configurations.Assistant;

internal sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
	public void Configure(EntityTypeBuilder<Attachment> builder)
	{
		builder.Metadata.SetSchema("assistant");

		builder.HasKey(a => a.Id);
		builder.Property(a => a.Id)
			.HasConversion(id => id.Value, value => AttachmentId.From(value));

		builder.Property(a => a.MessageId)
			.HasConversion(id => id.Value, value => MessageId.From(value))
			.IsRequired();
		builder.HasOne(a => a.Message)
			.WithOne(m => m.Attachment)
			.HasForeignKey<Attachment>(a => a.MessageId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasIndex(a => a.MessageId).IsUnique();

		builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
		builder.Property(a => a.FileKey).HasMaxLength(500).IsRequired();
		builder.Property(a => a.ContentType).HasMaxLength(100).IsRequired();
		builder.Property(a => a.FileSize).IsRequired();
		builder.Property(a => a.Timestamp).IsRequired();
	}
}
