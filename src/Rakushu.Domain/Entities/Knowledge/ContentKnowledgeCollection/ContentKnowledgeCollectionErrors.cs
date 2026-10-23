using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;

public static class ContentKnowledgeCollectionErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"CONTENT_KNOWLEDGE_COLLECTION.NOT_FOUND", "The content knowledge collection was not found.");

	public static readonly Error ContentCategoryNotFound = Error.NotFound(
		"CONTENT_KNOWLEDGE_COLLECTION.CONTENT_CATEGORY_NOT_FOUND", "The content category was not found.");

	public static readonly Error KnowledgeCollectionNotFound = Error.NotFound(
		"CONTENT_KNOWLEDGE_COLLECTION.KNOWLEDGE_COLLECTION_NOT_FOUND", "The knowledge collection was not found.");

	public static readonly Error AlreadyAttached = Error.Conflict(
		"CONTENT_KNOWLEDGE_COLLECTION.ALREADY_ATTACHED", "This knowledge collection is already attached to the content category.");

	public static readonly Error InvalidPriority = Error.Validation(
		"CONTENT_KNOWLEDGE_COLLECTION.INVALID_PRIORITY", "Priority must be unique and continuous within a content category (1, 2, 3, ...).");

	public static readonly Error InvalidFilterType = Error.Validation(
		"CONTENT_KNOWLEDGE_COLLECTION.INVALID_FILTER_TYPE", "Type must be one of: Word, Collocation, Grammar, Terminology, ProperNoun, Mixed.");

	public static readonly Error InvalidFilterActive = Error.Validation(
		"CONTENT_KNOWLEDGE_COLLECTION.INVALID_FILTER_ACTIVE", "Active must be 'true' or 'false'.");
}
