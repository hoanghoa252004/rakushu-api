using FluentValidation;

namespace Rakushu.Application.Usecases.Analytics.Learner.GetLearnerDashboard;

internal sealed class GetLearnerDashboardValidator : AbstractValidator<GetLearnerDashboardQuery>
{
	public GetLearnerDashboardValidator()
	{
		RuleFor(x => x.Days)
			.InclusiveBetween(1, 365)
			.WithMessage("Days must be between 1 and 365.");

		RuleFor(x => x.ForecastDays)
			.InclusiveBetween(1, 30)
			.WithMessage("ForecastDays must be between 1 and 30.");

		When(x => x.HeatmapYear.HasValue, () =>
		{
			RuleFor(x => x.HeatmapYear!.Value)
				.InclusiveBetween(2020, 2100)
				.WithMessage("HeatmapYear must be between 2020 and 2100.");
		});
	}
}
