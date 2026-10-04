using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.BunsetsuDependencyRelationship;
using Rakushu.Domain.Entities.Linguistic.DependencyRelationship;

namespace Rakushu.Persistence.Configurations;

internal sealed class BunsetsuDependencyRelationshipConfiguration : IEntityTypeConfiguration<BunsetsuDependencyRelationship>
{
	public void Configure(EntityTypeBuilder<BunsetsuDependencyRelationship> builder)
	{
		// Id
		builder.HasKey(b => b.Id);
		builder.Property(b => b.Id)
			.HasConversion(
				id => id.Value,
				value => BunsetsuDependencyRelationshipId.From(value));

		// FromBunsetsuId
		builder.Property(b => b.FromBunsetsuId)
			.HasConversion(
				id => id.Value,
				value => BunsetsuId.From(value))
			.IsRequired();
		builder.HasOne(b => b.FromBunsetsu)
			.WithMany(bs => bs.OutgoingRelations)
			.HasForeignKey(b => b.FromBunsetsuId)
			.OnDelete(DeleteBehavior.Cascade);

		// ToBunsetsuId
		builder.Property(b => b.ToBunsetsuId)
			.HasConversion(
				id => id.Value,
				value => BunsetsuId.From(value))
			.IsRequired();
		builder.HasOne(b => b.ToBunsetsu)
			.WithMany(bs => bs.IncomingRelations)
			.HasForeignKey(b => b.ToBunsetsuId)
			.OnDelete(DeleteBehavior.Cascade);

		// DependencyRelationshipId
		builder.Property(b => b.DependencyRelationshipId)
			.HasConversion(
				id => id.Value,
				value => DependencyRelationshipId.From(value))
			.IsRequired();
		builder.HasOne(b => b.DependencyRelationship)
			.WithMany(dr => dr.BunsetsuDependencyRelationships)
			.HasForeignKey(b => b.DependencyRelationshipId)
			.OnDelete(DeleteBehavior.Restrict);

		// CreatedAt
		builder.Property(b => b.CreatedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(b => b.UpdatedAt)
			.IsRequired();
	}
}
