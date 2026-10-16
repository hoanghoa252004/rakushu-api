using FluentValidation;

namespace Rakushu.Application.Usecases.Material.Video.GetAllVideos;

internal sealed class GetAllVideosValidator : AbstractValidator<GetAllVideosQuery>
{
	public GetAllVideosValidator()
	{
		RuleFor(x => x.PageNumber)
			.GreaterThan(0).WithMessage("PageNumber must be greater than 0");

		RuleFor(x => x.PageSize)
			.GreaterThan(0).WithMessage("PageSize must be greater than 0")
			.LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");
	}
}
