using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.CuratorReview;

public static class CuratorReviewErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"CuratorReview.NotFound",
		"The specified curator review was not found.");

	public static readonly Error InvalidDecision = Error.Validation(
		"CuratorReview.InvalidDecision",
		"The specified curator review decision is invalid.");
}
