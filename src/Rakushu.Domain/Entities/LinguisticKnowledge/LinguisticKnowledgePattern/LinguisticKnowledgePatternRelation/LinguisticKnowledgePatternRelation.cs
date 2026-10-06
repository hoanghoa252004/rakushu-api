using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgePattern.LinguisticKnowledgePatternElement;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgePattern.LinguisticKnowledgePatternRelation;

public sealed class LinguisticKnowledgePatternRelation
	: Entity<LinguisticKnowledgePatternRelationId>
{
	public LinguisticKnowledgePatternId KnowledgePatternId { get; private set; } = null!;
	public LinguisticKnowledgePatternElementId FromElementId { get; private set; } = null!;
	public LinguisticKnowledgePatternElementId ToElementId { get; private set; } = null!;
	public LinguisticKnowledgePatternRelationType RelationType { get; private set; }
	public DependencyRelationshipId? DependencyRelationshipId { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }


	// NAVIGATION PROPERTIES
	// KnowledgePattern
	public LinguisticKnowledgePattern KnowledgePattern { get; private set; } = null!;

	// KnowledgePatternElement [FROM]
	public LinguisticKnowledgePatternElement.LinguisticKnowledgePatternElement FromElement { get; private set; } = null!;

	// KnowledgePatternElement [TO]
	public LinguisticKnowledgePatternElement.LinguisticKnowledgePatternElement ToElement { get; private set; } = null!;

	// DependencyRelationship
	public DependencyRelationship? DependencyRelationship { get; private set; }



	// CONSTRUCTORS & FACTORY METHODS
	private LinguisticKnowledgePatternRelation()
	{
	}

	private LinguisticKnowledgePatternRelation(
		LinguisticKnowledgePatternRelationId id,
		LinguisticKnowledgePatternId knowledgePatternId,
		LinguisticKnowledgePatternElementId fromElementId,
		LinguisticKnowledgePatternElementId toElementId,
		LinguisticKnowledgePatternRelationType relationType,
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