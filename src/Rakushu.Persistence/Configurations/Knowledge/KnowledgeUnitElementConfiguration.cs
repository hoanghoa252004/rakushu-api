using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnitElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations.Knowledge;

internal sealed class KnowledgeUnitElementConfiguration : IEntityTypeConfiguration<KnowledgeUnitElement>
{
	public void Configure(EntityTypeBuilder<KnowledgeUnitElement> builder)
	{
		// Schema
		builder.Metadata.SetSchema("knowledge");

		// Id
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Id)
			.HasConversion(
				id => id.Value,
				value => KnowledgeUnitElementId.From(value));

		// KnowledgeUnitId
		builder.Property(t => t.KnowledgeUnitId)
			.HasConversion(
				id => id.Value,
				value => KnowledgeUnitId.From(value))
			.IsRequired();
		builder.HasOne(t => t.KnowledgeUnit)
			.WithMany(ku => ku.Elements)
			.HasForeignKey(t => t.KnowledgeUnitId)
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
			.WithMany(jop => jop.KnowledgeUnitElements)
			.HasForeignKey(t => t.JapanesePartOfSpeechId)
			.OnDelete(DeleteBehavior.Restrict);

		// DependencyRelationship
		builder.Property(t => t.DependencyRelationshipId)
			.HasConversion(
				id => id.Value,
				value => DependencyRelationshipId.From(value))
			.IsRequired();
		builder.HasOne(t => t.DependencyRelationship)
			.WithMany(dr => dr.KnowledgeUnitElements)
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