using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Configurations.Knowledge;

internal sealed class KnowledgeCollectionConfiguration : IEntityTypeConfiguration<KnowledgeCollection>
{
	public void Configure(EntityTypeBuilder<KnowledgeCollection> builder)
	{
		// Schema
		builder.Metadata.SetSchema("knowledge");

		// Id
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Id)
			.HasConversion(
				id => id.Value,
				value => KnowledgeCollectionId.From(value));

		// Code
		builder.Property(c => c.Code)
			.HasMaxLength(30)
			.IsRequired();
		builder.HasIndex(c => c.Code)
			.IsUnique();

		// Name
		builder.Property(c => c.Name)
			.HasMaxLength(100)
			.IsRequired();

		// Description
		builder.Property(c => c.Description);

		// KnowledgeCollectionType (enum)
		builder.Property(lk => lk.CollectionType)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// KnowledgeCollectionScope (enum)
		builder.Property(lk => lk.Scope)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// IsActive
		builder.Property(c => c.IsActive)
			.IsRequired();

		// CreatedAt
		builder.Property(c => c.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(c => c.UpdatedAt)
			.IsRequired();
	}
}
