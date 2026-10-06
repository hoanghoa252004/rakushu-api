using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.LinguisticKnowledge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgeMeaning;

public sealed class LinguisticKnowledgeMeaning : Entity<LinguisticKnowledgeMeaningId>
{
	public int SortOrder { get; private set; }
	public LinguisticKnowledgeId LinguisticKnowledgeId { get; private set; } = null!;
	public string UniversalMeaning { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// LinguisticKnowledge
	public LinguisticKnowledge LinguisticKnowledge { get; private set; } = null!;


	private LinguisticKnowledgeMeaning()
	{
	}


	private LinguisticKnowledgeMeaning(
		LinguisticKnowledgeMeaningId id,
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
