using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Series.GetVideoBySeriesSlugAndVideoSlug;

internal sealed class GetVideoBySeriesSlugAndVideoSlugValidator : AbstractValidator<GetVideoBySeriesSlugAndVideoSlugQuery>
{
	public GetVideoBySeriesSlugAndVideoSlugValidator()
	{
		RuleFor(x => x.SeriesSlug)
			.NotEmpty().WithMessage("SeriesSlug is required");

		RuleFor(x => x.VideoSlug)
			.NotEmpty().WithMessage("VideoSlug is required");
	}
}
