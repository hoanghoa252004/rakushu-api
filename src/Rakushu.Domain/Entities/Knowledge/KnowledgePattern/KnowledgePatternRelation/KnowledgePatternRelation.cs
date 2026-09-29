using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternElement;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternRelation;

public sealed class KnowledgePatternRelation
	: Entity<KnowledgePatternRelationId>
{
	public KnowledgePatternId KnowledgePatternId { get; private set; } = null!;
	public KnowledgePatternElementId FromElementId { get; private set; } = null!;
	public KnowledgePatternElementId ToElementId { get; private set; } = null!;
	public KnowledgePatternRelationType RelationType { get; private set; }
	public DependencyRelationshipId? DependencyRelationshipId { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }


	// NAVIGATION PROPERTIES
	// KnowledgePattern
	public KnowledgePattern KnowledgePattern { get; private set; } = null!;

	// KnowledgePatternElement [FROM]
	public KnowledgePatternElement.KnowledgePatternElement FromElement { get; private set; } = null!;

	// KnowledgePatternElement [TO]
	public KnowledgePatternElement.KnowledgePatternElement ToElement { get; private set; } = null!;

	// DependencyRelationship
	public DependencyRelationship? DependencyRelationship { get; private set; }



	// CONSTRUCTORS & FACTORY METHODS
	private KnowledgePatternRelation()
	{
	}

	private KnowledgePatternRelation(
		KnowledgePatternRelationId id,
		KnowledgePatternId knowledgePatternId,
		KnowledgePatternElementId fromElementId,
		KnowledgePatternElementId toElementId,
		KnowledgePatternRelationType relationType,
		DependencyRelationshipId? dependencyRelationshipId,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		KnowledgePatternId = knowledgePatternId;
		FromElementId = fromElementId;
		ToElementId = toElementId;
		RelationType = relationType;
		DependencyRelationshipId = dependencyRelationshipId;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}