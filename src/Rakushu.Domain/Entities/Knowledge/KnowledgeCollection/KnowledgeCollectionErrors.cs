using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

public static class KnowledgeCollectionErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"KNOWLEDGE_COLLECTION.NOT_FOUND", "The knowledge collection was not found.");

	public static readonly Error InvalidCode = Error.Validation(
		"KNOWLEDGE_COLLECTION.INVALID_CODE", "Collection code is required and cannot exceed 30 characters.");

	public static readonly Error InvalidName = Error.Validation(
		"KNOWLEDGE_COLLECTION.INVALID_NAME", "Collection name is required and cannot exceed 100 characters.");

	public static readonly Error DuplicateCode = Error.Conflict(
		"KNOWLEDGE_COLLECTION.DUPLICATE_CODE", "A knowledge collection with this code already exists.");

	public static readonly Error InvalidFilterType = Error.Validation(
		"KNOWLEDGE_COLLECTION.INVALID_FILTER_TYPE", "Type must be one of: Word, Collocation, Grammar, Terminology, ProperNoun, Mixed.");

	public static readonly Error InvalidFilterActive = Error.Validation(
		"KNOWLEDGE_COLLECTION.INVALID_FILTER_ACTIVE", "Active must be 'true' or 'false'.");

	public static readonly Error IsInUse = Error.Conflict(
		"KNOWLEDGE_COLLECTION.IN_USE", "Cannot delete a collection that is linked to content categories.");
}
