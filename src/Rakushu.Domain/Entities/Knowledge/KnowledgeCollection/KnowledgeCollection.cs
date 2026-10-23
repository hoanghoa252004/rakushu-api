using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
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

	// ContentKnowledgeCollections
	private readonly List<Knowledge.ContentKnowledgeCollection.ContentKnowledgeCollection> _contentKnowledgeCollections = new();
	public IReadOnlyCollection<Knowledge.ContentKnowledgeCollection.ContentKnowledgeCollection> ContentKnowledgeCollections => _contentKnowledgeCollections.AsReadOnly();
	private KnowledgeCollection()
	{
	}

	private KnowledgeCollection(
		KnowledgeCollectionId id,
		string code,
		string name,
		string? description,
		KnowledgeCollectionType collectionType,
		KnowledgeCollectionScope scope,
		bool isActive,
		DateTimeOffset now) : base(id)
	{
		Code = code;
		Name = name;
		Description = description;
		CollectionType = collectionType;
		Scope = scope;
		IsActive = isActive;
		CreatedAt = now;
		UpdatedAt = now;
	}

	public static Result<KnowledgeCollection> Create(
		string code,
		string name,
		KnowledgeCollectionType collectionType,
		KnowledgeCollectionScope scope,
		bool isActive,
		DateTimeOffset now,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(code) || code.Length > 30)
			return Result.Failure<KnowledgeCollection>(KnowledgeCollectionErrors.InvalidCode);

		if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
			return Result.Failure<KnowledgeCollection>(KnowledgeCollectionErrors.InvalidName);

		return Result.Success(new KnowledgeCollection(
			KnowledgeCollectionId.Create(), code, name, description, collectionType, scope, isActive, now));
	}

	public Result Update(
		string name,
		KnowledgeCollectionType collectionType,
		KnowledgeCollectionScope scope,
		bool isActive,
		DateTimeOffset now,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
			return Result.Failure(KnowledgeCollectionErrors.InvalidName);

		Name = name;
		Description = description;
		CollectionType = collectionType;
		Scope = scope;
		IsActive = isActive;
		UpdatedAt = now;

		return Result.Success();
	}
}