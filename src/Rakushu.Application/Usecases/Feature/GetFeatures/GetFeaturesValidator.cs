using FluentValidation;

namespace Rakushu.Application.Usecases.Feature.GetFeatures;

internal sealed class GetFeaturesValidator : AbstractValidator<GetFeaturesQuery>
{
	public GetFeaturesValidator()
	{
		RuleFor(x => x.PageNumber)
			.GreaterThan(0);

		RuleFor(x => x.PageSize)
			.GreaterThan(0)
			.LessThanOrEqualTo(100);
	}
}
