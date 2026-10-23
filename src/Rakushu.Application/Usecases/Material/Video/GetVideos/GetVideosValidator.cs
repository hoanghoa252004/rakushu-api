using FluentValidation;

namespace Rakushu.Application.Usecases.Material.Video.GetVideos;

internal sealed class GetVideosValidator : AbstractValidator<GetVideosQuery>
{
	public GetVideosValidator()
	{
		RuleFor(x => x.PageNumber)
			.GreaterThan(0).WithMessage("PageNumber must be greater than 0");

		RuleFor(x => x.PageSize)
			.GreaterThan(0).WithMessage("PageSize must be greater than 0")
			.LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");
	}
}
