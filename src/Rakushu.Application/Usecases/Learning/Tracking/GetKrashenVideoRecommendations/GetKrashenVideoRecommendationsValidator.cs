using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetKrashenVideoRecommendations;

internal sealed class GetKrashenVideoRecommendationsValidator : AbstractValidator<GetKrashenVideoRecommendationsQuery>
{
	public GetKrashenVideoRecommendationsValidator()
	{
		RuleFor(x => x.Limit)
			.InclusiveBetween(1, 20)
			.WithMessage("Limit must be between 1 and 20.");

		When(x => x.ExcludeVideoId.HasValue, () =>
		{
			RuleFor(x => x.ExcludeVideoId!.Value)
				.NotEmpty()
				.WithMessage("ExcludeVideoId cannot be empty.");
		});
	}
}
