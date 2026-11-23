using FluentValidation;

namespace Rakushu.Application.Usecases.Analytics.Curator.GetCuratorDashboard;

internal sealed class GetCuratorDashboardValidator : AbstractValidator<GetCuratorDashboardQuery>
{
	public GetCuratorDashboardValidator()
	{
		RuleFor(x => x.Days)
			.InclusiveBetween(1, 365)
			.WithMessage("Days must be between 1 and 365.");
	}
}
