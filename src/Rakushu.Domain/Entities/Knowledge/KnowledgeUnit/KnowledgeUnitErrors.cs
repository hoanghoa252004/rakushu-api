using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

public static class KnowledgeUnitErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"KNOWLEDGE_UNIT.NOT_FOUND", "The knowledge unit was not found.");

	public static readonly Error InvalidExpression = Error.Validation(
		"KNOWLEDGE_UNIT.INVALID_EXPRESSION", "Expression is required and cannot exceed 100 characters.");

	public static readonly Error InvalidReading = Error.Validation(
		"KNOWLEDGE_UNIT.INVALID_READING", "Reading is required and cannot exceed 100 characters.");

	public static readonly Error InvalidFilterType = Error.Validation(
		"KNOWLEDGE_UNIT.INVALID_FILTER_TYPE", "Type must be one of: Word, Collocation, Grammar, Terminology, ProperNoun.");

	public static readonly Error InvalidFilterActive = Error.Validation(
		"KNOWLEDGE_UNIT.INVALID_FILTER_ACTIVE", "Active must be 'true' or 'false'.");

	public static readonly Error CollectionNotFound = Error.NotFound(		"KNOWLEDGE_UNIT.COLLECTION_NOT_FOUND", "The knowledge collection was not found.");

	public static readonly Error ProficiencyLevelNotFound = Error.NotFound(
		"KNOWLEDGE_UNIT.PROFICIENCY_LEVEL_NOT_FOUND", "The proficiency level was not found.");
}
