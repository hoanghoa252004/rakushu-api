using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
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
	public string JapaneseMeaning { get; private set; } = null!;
	public string? Description { get; private set; }
	public KnowledgeUnitId KnowledgeUnitId { get; private set; } = null!;

	// NAVIGATION PROPERTIES
	// KnowledgeUnit
	public KnowledgeUnit.KnowledgeUnit KnowledgeUnit { get; private set; } = null!;

	// LearningUnits
	private readonly List<Material.LearningUnit.LearningUnit> _learningUnits = new();
	public IReadOnlyCollection<Material.LearningUnit.LearningUnit> LearningUnits => _learningUnits.AsReadOnly();

	private KnowledgeMeaning()
	{
	}

	public static Result<KnowledgeMeaning> Create(
		KnowledgeUnitId knowledgeUnitId,
		int sortOrder,
		string meaning,
		string japaneseMeaning,
		string? description = null)
	{
		var validation = Validate(sortOrder, meaning, japaneseMeaning);
		if (validation.IsFailure)
			return Result.Failure<KnowledgeMeaning>(validation.Error);

		return Result.Success(new KnowledgeMeaning
		{
			Id = KnowledgeMeaningId.Create(),
			KnowledgeUnitId = knowledgeUnitId,
			SortOrder = sortOrder,
			Meaning = meaning,
			JapaneseMeaning = japaneseMeaning,
			Description = description
		});
	}

	public Result Update(int sortOrder, string meaning, string japaneseMeaning, string? description = null)
	{
		var validation = Validate(sortOrder, meaning, japaneseMeaning);
		if (validation.IsFailure)
			return validation;

		SortOrder = sortOrder;
		Meaning = meaning;
		JapaneseMeaning = japaneseMeaning;
		Description = description;

		return Result.Success();
	}

	private static Result Validate(int sortOrder, string meaning, string japaneseMeaning)
	{
		if (sortOrder <= 0)
			return Result.Failure(KnowledgeMeaningErrors.InvalidSortOrder);

		if (string.IsNullOrWhiteSpace(meaning) || meaning.Length > 100
			|| string.IsNullOrWhiteSpace(japaneseMeaning) || japaneseMeaning.Length > 100)
			return Result.Failure(KnowledgeMeaningErrors.InvalidMeaning);

		return Result.Success();
	}
}
