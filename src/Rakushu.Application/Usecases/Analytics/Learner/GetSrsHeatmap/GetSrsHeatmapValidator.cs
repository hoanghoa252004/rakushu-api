using FluentValidation;

namespace Rakushu.Application.Usecases.Analytics.Learner.GetSrsHeatmap;

internal sealed class GetSrsHeatmapValidator : AbstractValidator<GetSrsHeatmapQuery>
{
	public GetSrsHeatmapValidator()
	{
		When(x => x.Year.HasValue, () =>
		{
			RuleFor(x => x.Year!.Value)
				.InclusiveBetween(2020, 2100)
				.WithMessage("Year must be between 2020 and 2100.");
		});
	}
}
