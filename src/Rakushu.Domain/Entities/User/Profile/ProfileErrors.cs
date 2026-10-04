using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.User.Profile;

public static class ProfileErrors
{
	public static readonly Error InvalidFullName = Error.Validation(
		"PROFILE.INVALID_FULLNAME", "FullName is required & cannot exceed 50 characters.");

	public static readonly Error NotFound = Error.NotFound(
		"PROFILE.NOT_FOUND", "Profile was not found.");

	public static readonly Error AlreadyExists = Error.Conflict(
		"PROFILE.ALREADY_EXISTS", "User already has a profile.");

	public static readonly Error InterestNotFound = Error.NotFound(
		"PROFILE.INTEREST_NOT_FOUND", "Interest was not found.");

	public static readonly Error InvalidLearningSettings = Error.Validation(
		"PROFILE.INVALID_LEARNING_SETTINGS", "Learning session minutes must be greater than 0 and " +
		"daily learning minutes must be greater than session duration minutes.");

	public static readonly Error CurrentNotHigherThanTargetLevel = Error.Validation(
		"PROFILE.CURRENT_NOT_HIGHER_THAN_TARGET_LEVEL", "Current not higher than target level.");

	public static readonly Error DuplicateInterestCategory = Error.Validation(
		"PROFILE.DUPLICATE_INTEREST_CATEGORY", "Duplicate interest category.");

	public static readonly Error DuplicateInterestPriority = Error.Validation(
		"PROFILE.DUPLICATE_INTEREST_PRIORITY", "Duplicate interest priority.");

	public static readonly Error ContainAtLeast1Interest = Error.Validation(
		"PROFILE.CONTAIN_AT_LEAST_1_INTEREST", "Profile must contain at least 1 interest.");
}
