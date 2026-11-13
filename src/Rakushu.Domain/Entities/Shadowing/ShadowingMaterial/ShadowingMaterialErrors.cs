using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public static class ShadowingMaterialErrors
{
	public static readonly Error Duplicate =
	Error.Conflict(
		"ShadowingMaterial.Duplicate",
		"A shadowing material with the same transcript range and speakers already exists.");

	public static readonly Error NotFound = Error.NotFound(
		"SHADOWING_MATERIAL.NOT_FOUND",
		"The shadowing material was not found.");

	public static readonly Error InvalidName = Error.Validation(
		"SHADOWING_MATERIAL.INVALID_NAME",
		"Name is required and must not exceed 200 characters.");

	public static readonly Error SegmentNotFound = Error.NotFound(
		"SHADOWING_MATERIAL.SEGMENT_NOT_FOUND",
		"Start or end transcript segment was not found in the video.");

	public static readonly Error InvalidSegmentRange = Error.Validation(
		"SHADOWING_MATERIAL.INVALID_SEGMENT_RANGE",
		"Start segment must not come after end segment.");

	public static readonly Error TranscriptSpeakerInvalid = Error.Validation(
		"SHADOWING_MATERIAL.TRANSCRIPT_SPEAKER_INVALID",
		"Transcript speakers must exist in the video, be mapped to an active speaker, and appear within the selected segment range.");

	public static readonly Error ProcessingMaterialExists = Error.Conflict(
		"SHADOWING_MATERIAL.PROCESSING_EXISTS",
		"You already have a material being processed.");

	public static readonly Error RangeAlreadyExists = Error.Conflict(
		"SHADOWING_MATERIAL.RANGE_EXISTS",
		"A material with the same start and end segment already exists.");

	public static readonly Error HasSessions = Error.Conflict(
		"SHADOWING_MATERIAL.HAS_SESSIONS",
		"The material already has sessions.");

	public static readonly Error InvalidType = Error.Validation(
		"SHADOWING_MATERIAL.INVALID_TYPE",
		"Invalid shadowing material type.");

	public static readonly Error RequestRequired = Error.Validation(
		"SHADOWING_MATERIAL.REQUEST_REQUIRED",
		"A material of type RequestedByLearner must be linked to a request.");

	public static readonly Error CuratedCannotHaveRequest = Error.Validation(
		"SHADOWING_MATERIAL.CURATED_CANNOT_HAVE_REQUEST",
		"A curated material cannot be linked to a request.");

	public static readonly Error RolesMissingLines = Error.Validation(
		"SHADOWING_MATERIAL.ROLES_MISSING_LINES",
		"All roles must have at least one line before the material can be Ready.");

	public static readonly Error LearnerNotAllowed = Error.Forbidden(
		"SHADOWING_MATERIAL.LEARNER_NOT_ALLOWED",
		"A learner can only create a material for a Curated video or a video they uploaded, and can only modify their own materials.");

	public static readonly Error LearnerMaterialAlreadyExists = Error.Conflict(
		"SHADOWING_MATERIAL.LEARNER_MATERIAL_ALREADY_EXISTS",
		"A learner can only create one material for their video.");

	public static readonly Error VideoNotReady = Error.Validation(
		"SHADOWING_MATERIAL.VIDEO_NOT_READY",
		"A shadowing material can only be created for a video with Ready status.");

	public static readonly Error InvalidStatusTransition = Error.Validation(
		"SHADOWING_MATERIAL.INVALID_STATUS_TRANSITION",
		"Invalid status transition for the shadowing material.");

	public static readonly Error CannotDelete = Error.Conflict(
		"SHADOWING_MATERIAL.CANNOT_DELETE",
		"The shadowing material can only be deleted while its status is Processing.");

	public static readonly Error NotEditable = Error.Conflict(
		"SHADOWING_MATERIAL.NOT_EDITABLE",
		"An archived shadowing material cannot be modified.");

	public static readonly Error NotAvailable = Error.NotFound(
		"SHADOWING_MATERIAL.NOT_AVAILABLE",
		"The shadowing material is not available.");

	public static readonly Error NoTranscriptSpeakers = Error.Validation(
		"SHADOWING_MATERIAL.NO_TRANSCRIPT_SPEAKERS",
		"The video transcript has no segments with a mapped speaker.");
}
