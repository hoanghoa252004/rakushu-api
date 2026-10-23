using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Material.LearningUnit;

public static class LearningUnitErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"LEARNING_UNIT.NOT_FOUND", "The learning unit was not found.");

	public static readonly Error KnowledgeMeaningNotFound = Error.NotFound(
		"LEARNING_UNIT.KNOWLEDGE_MEANING_NOT_FOUND", "The knowledge meaning was not found or its knowledge unit is inactive.");

	public static readonly Error InvalidTokenRange = Error.Validation(
		"LEARNING_UNIT.INVALID_TOKEN_RANGE", "The token range does not match the tokens of this segment.");

	public static readonly Error AlreadyExists = Error.Conflict(
		"LEARNING_UNIT.ALREADY_EXISTS", "This learning unit already exists in the segment.");
}
