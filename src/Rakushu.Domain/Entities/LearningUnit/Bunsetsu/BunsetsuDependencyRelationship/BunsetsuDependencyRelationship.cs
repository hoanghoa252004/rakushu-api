using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Linguistic.DependencyRelationship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LearningUnit.Bunsetsu.BunsetsuDependencyRelationship;

public sealed class BunsetsuDependencyRelationship
	: Entity<BunsetsuDependencyRelationshipId>
{
	public BunsetsuId FromBunsetsuId { get; private set; } = null!;
	public BunsetsuId ToBunsetsuId { get; private set; } = null!;
	public DependencyRelationshipId DependencyRelationshipId { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// From Bunsetsu
	public Bunsetsu FromBunsetsu { get; private set; } = null!;

	// To Bunsetsu
	public Bunsetsu ToBunsetsu { get; private set; } = null!;

	// Dependency Relationship
	public DependencyRelationship DependencyRelationship { get; private set; } = null!;


	// CONSTRUCTORS & FACTORY METHODS
	private BunsetsuDependencyRelationship()
	{
	}

	private BunsetsuDependencyRelationship(
		BunsetsuDependencyRelationshipId id,
		BunsetsuId fromBunsetsuId,
		BunsetsuId toBunsetsuId,
		DependencyRelationshipId dependencyRelationshipId,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		FromBunsetsuId = fromBunsetsuId;
		ToBunsetsuId = toBunsetsuId;
		DependencyRelationshipId = dependencyRelationshipId;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
