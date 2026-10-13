using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

public sealed class KnowledgeCollection : AggregateRoot<KnowledgeCollectionId>
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public KnowledgeCollectionType CollectionType { get; private set; }
	public KnowledgeCollectionScope Scope { get; private set; }
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// KnowledgeUnits
	private readonly List<KnowledgeUnit.KnowledgeUnit> _knowledgeUnits = new();
	public IReadOnlyCollection<KnowledgeUnit.KnowledgeUnit> KnowledgeUnits => _knowledgeUnits.AsReadOnly();
	private KnowledgeCollection()
	{
	}
}