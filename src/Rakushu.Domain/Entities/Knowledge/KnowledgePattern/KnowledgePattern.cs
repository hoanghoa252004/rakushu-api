using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Knowledge.LinguisticKnowledge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgePattern;

public sealed class KnowledgePattern : Entity<KnowledgePatternId>	
{
	public LinguisticKnowledgeId LinguisticKnowledgeId { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// LinguisticKnowledge
	public LinguisticKnowledge.LinguisticKnowledge LinguisticKnowledge { get; private set; } = null!;

	// KnowledgePatternElements
	private readonly List<KnowledgePatternElement.KnowledgePatternElement> _elements = new();
	public IReadOnlyCollection<KnowledgePatternElement.KnowledgePatternElement> Elements => _elements.AsReadOnly();

	// KnowledgePatternRelations
	private readonly List<KnowledgePatternRelation.KnowledgePatternRelation> _relations = new();
	public IReadOnlyCollection<KnowledgePatternRelation.KnowledgePatternRelation> Relations => _relations.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private KnowledgePattern() { }

	private KnowledgePattern(
		KnowledgePatternId id,
		LinguisticKnowledgeId linguisticKnowledgeId,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt) : base(id)
	{
		LinguisticKnowledgeId = linguisticKnowledgeId;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
