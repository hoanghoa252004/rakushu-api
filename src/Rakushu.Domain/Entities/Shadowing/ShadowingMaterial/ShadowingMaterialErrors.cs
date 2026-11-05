using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public static class ShadowingMaterialErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SHADOWING_MATERIAL.NOT_FOUND",
		"The shadowing material was not found.");

	public static readonly Error InvalidType = Error.Validation(
		"SHADOWING_MATERIAL.INVALID_TYPE",
		"Invalid shadowing material type.");

	public static readonly Error RequestRequired = Error.Validation(
		"SHADOWING_MATERIAL.REQUEST_REQUIRED",
		"A material of type RequestedByLearner must be linked to a request.");

	public static readonly Error CuratedCannotHaveRequest = Error.Validation(
		"SHADOWING_MATERIAL.CURATED_CANNOT_HAVE_REQUEST",
		"A curated material cannot be linked to a request.");

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
		"Roles and role lines can only be modified while the material status is Reviewing.");

	public static readonly Error NotAvailable = Error.NotFound(
		"SHADOWING_MATERIAL.NOT_AVAILABLE",
		"The shadowing material is not available.");

	public static readonly Error NoTranscriptSpeakers = Error.Validation(
		"SHADOWING_MATERIAL.NO_TRANSCRIPT_SPEAKERS",
		"The video transcript has no segments with a mapped speaker.");
}
