using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;

public static class LearnerEncounteredLearningUnitErrors
{
	public static readonly Error InvalidUserId = Error.Validation(
		"ENCOUNTERED_UNIT.INVALID_USER_ID", "The user ID cannot be empty.");

	public static readonly Error InvalidLearningUnitId = Error.Validation(
		"ENCOUNTERED_UNIT.INVALID_LEARNING_UNIT_ID", "The learning unit ID cannot be empty.");

	public static readonly Error InvalidMeaningId = Error.Validation(
		"ENCOUNTERED_UNIT.INVALID_MEANING_ID", "The knowledge meaning ID cannot be empty.");

	public static readonly Error InvalidKnowledgeUnitId = Error.Validation(
		"ENCOUNTERED_UNIT.INVALID_KNOWLEDGE_UNIT_ID", "The knowledge unit ID cannot be empty.");

	public static readonly Error InvalidVideoId = Error.Validation(
		"ENCOUNTERED_UNIT.INVALID_VIDEO_ID", "The video ID cannot be empty.");
}
