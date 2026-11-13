using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSessionMember;

public static class ShadowingSessionMemberErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SHADOWING_SESSION_MEMBER.NOT_FOUND",
		"The session member was not found.");

	public static readonly Error AlreadyMember = Error.Conflict(
		"SHADOWING_SESSION_MEMBER.ALREADY_MEMBER",
		"The user is already a member of this session.");

	public static readonly Error CannotRemoveHost = Error.Conflict(
		"SHADOWING_SESSION_MEMBER.CANNOT_REMOVE_HOST",
		"The host cannot be removed from the session.");

	public static readonly Error NotOwner = Error.Forbidden(
		"SHADOWING_SESSION_MEMBER.NOT_OWNER",
		"You can only change your own role.");

	public static readonly Error RoleNotFound = Error.NotFound(
		"SHADOWING_SESSION_MEMBER.ROLE_NOT_FOUND",
		"The shadowing role was not found.");

	public static readonly Error RoleNotInMaterial = Error.Validation(
		"SHADOWING_SESSION_MEMBER.ROLE_NOT_IN_MATERIAL",
		"The role does not belong to the material of this session.");

	public static readonly Error RoleAlreadyTaken = Error.Conflict(
		"SHADOWING_SESSION_MEMBER.ROLE_ALREADY_TAKEN",
		"The role has already been chosen by another member.");
}
