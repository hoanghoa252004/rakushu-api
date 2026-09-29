using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.OovCandidate;

public static class OovCandidateErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"OovCandidate.NotFound",
		"The specified OOV candidate was not found.");

	public static readonly Error EmptyTerm = Error.Validation(
		"OovCandidate.EmptyTerm",
		"The OOV candidate term cannot be empty.");

	public static readonly Error AlreadyReviewed = Error.Conflict(
		"OovCandidate.AlreadyReviewed",
		"The OOV candidate has already been reviewed.");
}
