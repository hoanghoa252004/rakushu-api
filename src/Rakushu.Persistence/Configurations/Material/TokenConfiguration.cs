using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.Token;

namespace Rakushu.Persistence.Configurations.Material;

internal sealed class TokenConfiguration : IEntityTypeConfiguration<Token>
{
	public void Configure(EntityTypeBuilder<Token> builder)
	{
		// Schema
		builder.Metadata.SetSchema("material");

		// Id
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Id)
			.HasConversion(
				id => id.Value,
				value => TokenId.From(value));

		// BunsetsuId
		builder.Property(t => t.BunsetsuId)
			.HasConversion(
				id => id.Value,
				value => BunsetsuId.From(value))
			.IsRequired();
		builder.HasOne(t => t.Bunsetsu)
			.WithMany(b => b.Tokens)
			.HasForeignKey(t => t.BunsetsuId)
			.OnDelete(DeleteBehavior.Cascade);

		// Surface
		builder.Property(t => t.Surface)
			.IsRequired();

		// Lemma
		builder.Property(t => t.Lemma)
			.IsRequired();

		// Reading
		builder.Property(t => t.Reading)
			.IsRequired();

		// HeadSequence
		builder.Property(t => t.HeadSequence)
			.IsRequired();

		// JapanesePartOfSpeech
		builder.Property(t => t.JapanesePartOfSpeechId)
			.HasConversion(
				id => id.Value,
				value => JapanesePartOfSpeechId.From(value))
			.IsRequired();
		builder.HasOne(t => t.JapanesePartOfSpeech)
			.WithMany(jop => jop.Tokens)
			.HasForeignKey(t => t.JapanesePartOfSpeechId)
			.OnDelete(DeleteBehavior.Restrict);

		// DependencyRelationship
		builder.Property(t => t.DependencyRelationshipId)
			.HasConversion(
				id => id.Value,
				value => DependencyRelationshipId.From(value))
			.IsRequired();
		builder.HasOne(t => t.DependencyRelationship)
			.WithMany(dr => dr.Tokens)
			.HasForeignKey(t => t.DependencyRelationshipId)
			.OnDelete(DeleteBehavior.Restrict);

		// StartIndex
		builder.Property(t => t.StartIndex)
			.IsRequired();

		// EndIndex
		builder.Property(t => t.EndIndex)
			.IsRequired();

		// Sequence
		builder.Property(t => t.Sequence)
			.IsRequired();
	}
}
