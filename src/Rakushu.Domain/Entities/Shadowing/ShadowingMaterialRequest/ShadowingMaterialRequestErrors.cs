using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

public static class ShadowingMaterialRequestErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SHADOWING_MATERIAL_REQUEST.NOT_FOUND",
		"The shadowing material request was not found.");

	public static readonly Error InvalidCode = Error.Validation(
		"SHADOWING_MATERIAL_REQUEST.INVALID_CODE",
		"Shadowing material request code is required.");

	public static readonly Error VideoNotReady = Error.Validation(
		"SHADOWING_MATERIAL_REQUEST.VIDEO_NOT_READY",
		"A shadowing material can only be requested for a video with Ready status.");

	public static readonly Error AlreadyExists = Error.Conflict(
		"SHADOWING_MATERIAL_REQUEST.ALREADY_EXISTS",
		"A shadowing material request already exists for this video.");

	public static readonly Error InvalidStatusTransition = Error.Validation(
		"SHADOWING_MATERIAL_REQUEST.INVALID_STATUS_TRANSITION",
		"Invalid status transition for the shadowing material request.");

	public static readonly Error RejectReasonRequired = Error.Validation(
		"SHADOWING_MATERIAL_REQUEST.REJECT_REASON_REQUIRED",
		"Reject reason is required when rejecting a shadowing material request.");

	public static readonly Error CannotModify = Error.Conflict(
		"SHADOWING_MATERIAL_REQUEST.CANNOT_MODIFY",
		"The shadowing material request can only be deleted while its status is Pending.");

	public static readonly Error Forbidden = Error.Forbidden(
		"SHADOWING_MATERIAL_REQUEST.FORBIDDEN",
		"You do not have permission to access this shadowing material request.");
}
