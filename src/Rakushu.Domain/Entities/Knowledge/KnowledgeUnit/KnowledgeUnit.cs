using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;
using Rakushu.Domain.Entities.Material.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
	public string? KnowledgeHash { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// KnowledgeUnitElements
	private readonly List<KnowledgeUnitElement.KnowledgeUnitElement> _elements = new();
	public IReadOnlyCollection<KnowledgeUnitElement.KnowledgeUnitElement> Elements => _elements.AsReadOnly();

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

	public static Result<KnowledgeUnit> Create(
		KnowledgeCollectionId knowledgeCollectionId,
		ProficiencyLevelId proficiencyLevelId,
		KnowledgeUnitType knowledgeType,
		string expression,
		string reading,
		KnowledgeUnitSourceType sourceType,
		bool isActive,
		DateTimeOffset now,
		string? audioKey = null)
	{
		var validation = Validate(expression, reading);
		if (validation.IsFailure)
			return Result.Failure<KnowledgeUnit>(validation.Error);

		return Result.Success(new KnowledgeUnit
		{
			Id = KnowledgeUnitId.Create(),
			KnowledgeCollectionId = knowledgeCollectionId,
			ProficiencyLevelId = proficiencyLevelId,
			KnowledgeType = knowledgeType,
			Expression = expression,
			Reading = reading,
			AudioKey = audioKey,
			SourceType = sourceType,
			IsActive = isActive,
			KnowledgeHash = string.Empty,
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public Result Update(
		KnowledgeCollectionId knowledgeCollectionId,
		ProficiencyLevelId proficiencyLevelId,
		KnowledgeUnitType knowledgeType,
		string expression,
		string reading,
		bool isActive,
		DateTimeOffset now,
		string? audioKey = null)
	{
		var validation = Validate(expression, reading);
		if (validation.IsFailure)
			return validation;

		KnowledgeCollectionId = knowledgeCollectionId;
		ProficiencyLevelId = proficiencyLevelId;
		KnowledgeType = knowledgeType;
		Expression = expression;
		Reading = reading;
		AudioKey = audioKey;
		IsActive = isActive;
		UpdatedAt = now;

		return Result.Success();
	}

	private static Result Validate(string expression, string reading)
	{
		if (string.IsNullOrWhiteSpace(expression) || expression.Length > 100)
			return Result.Failure(KnowledgeUnitErrors.InvalidExpression);

		if (string.IsNullOrWhiteSpace(reading) || reading.Length > 100)
			return Result.Failure(KnowledgeUnitErrors.InvalidReading);

		return Result.Success();
	}


	public Result<KnowledgeUnitElement.KnowledgeUnitElement> AddElement(
		string surface,
		string lemma,
		string reading,
		int headSequence,
		JapanesePartOfSpeechId japanesePartOfSpeechId,
		DependencyRelationshipId dependencyRelationshipId,
		int startIndex,
		int endIndex,
		int sequence)
	{
		var result = KnowledgeUnitElement.KnowledgeUnitElement.Create(
			Id,
			surface,
			lemma,
			reading,
			headSequence,
			japanesePartOfSpeechId,
			dependencyRelationshipId,
			startIndex,
			endIndex,
			sequence);

		if (result.IsFailure)
			return result;

		_elements.Add(result.Value);
		return result;
	}

	public bool RequiresNlpProcessing()
	{
		var currentHash = ComputeHash(this);

		return !string.Equals(
			KnowledgeHash,
			currentHash,
			StringComparison.Ordinal);
	}

	public void MarkNlpProcessed()
	{
		KnowledgeHash = ComputeHash(this);
	}

	public static string ComputeHash(KnowledgeUnit knowledgeUnit)
	{
		ArgumentNullException.ThrowIfNull(knowledgeUnit);

		var input = new
		{
			knowledgeUnit.Expression,
			KnowledgeType = (int)knowledgeUnit.KnowledgeType
		};

		var json = JsonSerializer.Serialize(input);
		var bytes = Encoding.UTF8.GetBytes(json);
		var hash = SHA256.HashData(bytes);

		return Convert.ToHexString(hash);
	}
	public void ClearElements()
	{
		_elements.Clear();
	}
}
