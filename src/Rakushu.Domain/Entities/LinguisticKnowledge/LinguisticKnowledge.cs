using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticKnowledge;

public sealed class LinguisticKnowledge : AggregateRoot<LinguisticKnowledgeId>
{
	public LinguisticKnowledgeType KnowledgeType { get; private set; }
	public string Expression { get; private set; } = null!;
	public string Reading { get; private set; } = null!;
	public LinguisticKnowledgeSource SourceType { get; private set; }
	public LinguisticKnowledgeStatus Status { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// KnowledgePattern
	public LinguisticKnowledgePattern.LinguisticKnowledgePattern? KnowledgePattern { get; private set; }

	// KnowledgeMeanings
	private readonly List<LinguisticKnowledgeMeaning.LinguisticKnowledgeMeaning> _meanings = new();
	public IReadOnlyCollection<LinguisticKnowledgeMeaning.LinguisticKnowledgeMeaning> Meanings => _meanings.AsReadOnly();

	// LearningUnits
	private readonly List<LearningUnit.LearningUnit> _learningUnits = new();
	public IReadOnlyCollection<LearningUnit.LearningUnit> LearningUnits => _learningUnits.AsReadOnly();

	private LinguisticKnowledge()
	{
	}

	private LinguisticKnowledge(
		LinguisticKnowledgeId id,
		LinguisticKnowledgeType knowledgeType,
		string expression,
		string reading,
		LinguisticKnowledgeSource source,
		LinguisticKnowledgeStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		KnowledgeType = knowledgeType;
		Expression = expression;
		Reading = reading;
		SourceType = source;
		Status = status;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
