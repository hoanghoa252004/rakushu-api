using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

public static class KnowledgeMeaningErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"KNOWLEDGE_MEANING.NOT_FOUND", "The knowledge meaning was not found.");

	public static readonly Error InvalidSortOrder = Error.Validation(
		"KNOWLEDGE_MEANING.INVALID_SORT_ORDER", "Sort order must be a positive integer.");

	public static readonly Error InvalidMeaning = Error.Validation(
		"KNOWLEDGE_MEANING.INVALID_MEANING", "Meaning and Japanese meaning are required and cannot exceed 100 characters.");

	public static readonly Error UnitNotFound = Error.NotFound(
		"KNOWLEDGE_MEANING.UNIT_NOT_FOUND", "The knowledge unit was not found.");

	public static readonly Error IsInUse = Error.Conflict(
		"KNOWLEDGE_MEANING.IN_USE", "Cannot delete a meaning that is used by learning units.");
}
