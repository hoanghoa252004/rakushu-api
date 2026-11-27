using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Learning.LearnerEncounterEvent;

public static class LearnerEncounterEventErrors
{
	public static readonly Error InvalidUserId = Error.Validation(
		"ENCOUNTER_EVENT.INVALID_USER_ID", "The user ID cannot be empty.");

	public static readonly Error InvalidLearningUnitId = Error.Validation(
		"ENCOUNTER_EVENT.INVALID_LEARNING_UNIT_ID", "The learning unit ID cannot be empty.");

	public static readonly Error InvalidMeaningId = Error.Validation(
		"ENCOUNTER_EVENT.INVALID_MEANING_ID", "The knowledge meaning ID cannot be empty.");

	public static readonly Error InvalidVideoId = Error.Validation(
		"ENCOUNTER_EVENT.INVALID_VIDEO_ID", "The video ID cannot be empty.");
}
