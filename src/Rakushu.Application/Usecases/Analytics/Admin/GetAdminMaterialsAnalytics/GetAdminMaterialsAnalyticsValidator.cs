using FluentValidation;

namespace Rakushu.Application.Usecases.Analytics.Admin.GetAdminMaterialsAnalytics;

internal sealed class GetAdminMaterialsAnalyticsValidator : AbstractValidator<GetAdminMaterialsAnalyticsQuery>
{
	public GetAdminMaterialsAnalyticsValidator()
	{
		RuleFor(x => x.DaysWindow)
			.InclusiveBetween(1, 365)
			.WithMessage("DaysWindow must be between 1 and 365.");
	}
}
