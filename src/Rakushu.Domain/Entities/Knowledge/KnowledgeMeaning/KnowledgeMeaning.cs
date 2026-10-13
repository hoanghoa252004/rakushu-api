using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

public sealed class KnowledgeMeaning : Entity<KnowledgeMeaningId>
{
	public int SortOrder { get; private set; }
	public string Meaning { get; private set; } = null!;
	public string? Description { get; private set; }
	public KnowledgeUnitId KnowledgeUnitId { get; private set; } = null!;

	// NAVIGATION PROPERTIES
	// KnowledgeUnit
	public KnowledgeUnit.KnowledgeUnit KnowledgeUnit { get; private set; } = null!;


	private KnowledgeMeaning()
	{
	}
}
