using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

public sealed class KnowledgeUnit : AggregateRoot<KnowledgeUnitId>
{
	public KnowledgeUnitType KnowledgeType { get; private set; }
	public KnowledgeCollectionId KnowledgeCollectionId { get; private set; } = null!;
	public ProficiencyLevelId ProficiencyLevelId { get; private set; } = null!;
	public string Expression { get; private set; } = null!;
	public string Reading { get; private set; } = null!;
	public string? AudioKey { get; private set; }
	public KnowledgeUnitSourceType SourceType { get; private set; }
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// KnowledgeMeanings
	private readonly List<KnowledgeMeaning.KnowledgeMeaning> _meanings = new();
	public IReadOnlyCollection<KnowledgeMeaning.KnowledgeMeaning> Meanings => _meanings.AsReadOnly();

	// KnowledgeCollection
	public KnowledgeCollection.KnowledgeCollection KnowledgeCollection { get; private set; } = null!;

	// ProficiencyLevel
	public ProficiencyLevel.ProficiencyLevel ProficiencyLevel { get; private set; } = null!;

	private KnowledgeUnit()
	{
	}
}
