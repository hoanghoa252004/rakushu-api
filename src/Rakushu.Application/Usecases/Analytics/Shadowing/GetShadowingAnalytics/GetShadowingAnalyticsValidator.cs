using FluentValidation;

namespace Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAnalytics;

internal sealed class GetShadowingAnalyticsValidator : AbstractValidator<GetShadowingAnalyticsQuery>
{
	private static readonly string[] AllowedPeriods = ["7d", "30d", "90d", "all"];

	public GetShadowingAnalyticsValidator()
	{
		RuleFor(x => x.Period)
			.Must(p => string.IsNullOrWhiteSpace(p)
				|| AllowedPeriods.Contains(p.Trim().ToLowerInvariant()))
			.WithMessage("Period must be one of: 7d, 30d, 90d, all.");
	}
}
