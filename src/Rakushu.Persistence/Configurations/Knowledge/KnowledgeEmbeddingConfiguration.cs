using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;
using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations.Knowledge;


public sealed class KnowledgeEmbeddingConfiguration : IEntityTypeConfiguration<KnowledgeEmbedding>
{
	public void Configure(EntityTypeBuilder<KnowledgeEmbedding> builder)
	{
		// Schema
		builder.Metadata.SetSchema("knowledge");

		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id)
			.HasDefaultValueSql("gen_random_uuid()");

		builder.Property(km => km.KnowledgeMeaningId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeMeaningId.From(value))
			.IsRequired();
		builder.HasIndex(u => u.KnowledgeMeaningId)
			.IsUnique();

		builder.Property(x => x.EmbeddingText)
			.IsRequired();

		builder.Property(x => x.ContentHash)
			.HasMaxLength(64)
			.IsRequired();

		builder.Property(x => x.Dimensions)
			.HasDefaultValue(1024)
			.IsRequired();

		builder.Property(x => x.Embedding)
			.HasColumnType("vector(1024)")
			.IsRequired();

		builder.Property(x => x.CreatedAt)
			.HasDefaultValueSql("CURRENT_TIMESTAMP")
			.IsRequired();

		builder.Property(x => x.UpdatedAt)
			.HasDefaultValueSql("CURRENT_TIMESTAMP")
			.IsRequired();

		// Foreign Key: KnowledgeEmbedding -> KnowledgeMeaning
		builder.HasOne<KnowledgeMeaning>()
			.WithMany()
			.HasForeignKey(x => x.KnowledgeMeaningId)
			.OnDelete(DeleteBehavior.Cascade)
			.HasConstraintName(
				"fk_knowledge_embeddings_knowledge_meanings");
		

		builder.ToTable(table =>
		{
			table.HasCheckConstraint(
				"ck_knowledge_embeddings_dimensions",
				"dimensions = 1024");
		});
	}
}

public sealed class KnowledgeEmbedding : Entity<Guid>
{
	public KnowledgeMeaningId KnowledgeMeaningId { get; set; } = null!;
	public string EmbeddingText { get; set; } = string.Empty;
	public string ContentHash { get; set; } = string.Empty;
	public int Dimensions { get; set; } = 1024;
	// Vector được tạo bởi Amazon Bedrock Embedding
	public Vector Embedding { get; set; } = null!;
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }

	public static KnowledgeEmbedding Create(
		KnowledgeMeaningId knowledgeMeaningId,
		string embeddingText,
		string contentHash,
		Vector embedding)
	{
		return new KnowledgeEmbedding
		{
			Id = Guid.NewGuid(),
			KnowledgeMeaningId = knowledgeMeaningId,
			EmbeddingText = embeddingText,
			ContentHash = contentHash,
			Dimensions = 1024,
			Embedding = embedding,
			CreatedAt = DateTimeOffset.UtcNow,
			UpdatedAt = DateTimeOffset.UtcNow
		};
	}	
}