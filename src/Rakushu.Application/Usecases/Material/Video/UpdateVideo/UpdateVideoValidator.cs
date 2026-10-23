using FluentValidation;

namespace Rakushu.Application.Usecases.Material.Video.UpdateVideo;

internal sealed class UpdateVideoValidator : AbstractValidator<UpdateVideoCommand>
{
	public UpdateVideoValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.Title)
			.NotEmpty().WithMessage("Title is required")
			.MaximumLength(200).WithMessage("Title must not exceed 200 characters");

		RuleFor(x => x.Description)
			.MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

		//RuleFor(x => x.ContentCategoryId)
		//	.NotEmpty().WithMessage("ContentCategoryId is required");

		RuleFor(x => x.SortOrder)
			.GreaterThan(0).WithMessage("SortOrder must be greater than 0");
	}
}
