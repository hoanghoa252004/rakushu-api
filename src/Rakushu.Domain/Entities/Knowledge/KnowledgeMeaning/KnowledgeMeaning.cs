using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Knowledge.LinguisticKnowledge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

public sealed class KnowledgeMeaning : Entity<KnowledgeMeaningId>
{
	public int SortOrder { get; private set; }
	public LinguisticKnowledgeId LinguisticKnowledgeId { get; private set; } = null!;
	public string UniversalMeaning { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// LinguisticKnowledge
	public LinguisticKnowledge.LinguisticKnowledge LinguisticKnowledge { get; private set; } = null!;

	// KnowledgeMeaningDetails
	private readonly List<KnowledgeMeaningDetail.KnowledgeMeaningDetail> _details = new();
	public IReadOnlyCollection<KnowledgeMeaningDetail.KnowledgeMeaningDetail> Details => _details.AsReadOnly();


	private KnowledgeMeaning()
	{
	}


	private KnowledgeMeaning(
		KnowledgeMeaningId id,
		LinguisticKnowledgeId linguisticKnowledgeId,
		int sortOrder,
		string universalMeaning,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		LinguisticKnowledgeId = linguisticKnowledgeId;
		SortOrder = sortOrder;
		UniversalMeaning = universalMeaning;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
