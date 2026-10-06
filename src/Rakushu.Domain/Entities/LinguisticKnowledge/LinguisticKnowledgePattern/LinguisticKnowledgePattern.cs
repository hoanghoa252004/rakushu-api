using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.LinguisticKnowledge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgePattern;

public sealed class LinguisticKnowledgePattern : Entity<LinguisticKnowledgePatternId>	
{
	public LinguisticKnowledgeId LinguisticKnowledgeId { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// LinguisticKnowledge
	public LinguisticKnowledge LinguisticKnowledge { get; private set; } = null!;

	// KnowledgePatternElements
	private readonly List<LinguisticKnowledgePatternElement.LinguisticKnowledgePatternElement> _elements = new();
	public IReadOnlyCollection<LinguisticKnowledgePatternElement.LinguisticKnowledgePatternElement> Elements => _elements.AsReadOnly();

	// KnowledgePatternRelations
	private readonly List<LinguisticKnowledgePatternRelation.LinguisticKnowledgePatternRelation> _relations = new();
	public IReadOnlyCollection<LinguisticKnowledgePatternRelation.LinguisticKnowledgePatternRelation> Relations => _relations.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private LinguisticKnowledgePattern() { }

	private LinguisticKnowledgePattern(
		LinguisticKnowledgePatternId id,
		LinguisticKnowledgeId linguisticKnowledgeId,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt) : base(id)
	{
		LinguisticKnowledgeId = linguisticKnowledgeId;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
