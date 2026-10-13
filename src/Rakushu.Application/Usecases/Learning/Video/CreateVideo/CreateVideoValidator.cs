using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Video.CreateVideo;

internal sealed class CreateVideoValidator : AbstractValidator<CreateVideoCommand>
{
	public CreateVideoValidator()
	{
		RuleFor(x => x.Slug)
			.NotEmpty().WithMessage("Slug is required")
			.MaximumLength(200).WithMessage("Slug must not exceed 200 characters");

		RuleFor(x => x.Title)
			.NotEmpty().WithMessage("Title is required")
			.MaximumLength(200).WithMessage("Title must not exceed 200 characters");

		RuleFor(x => x.Description)
			.MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

		RuleFor(x => x.Duration)
			.GreaterThan(TimeSpan.Zero).WithMessage("Duration must be greater than zero");

		RuleFor(x => x.ContentCategoryId)
			.NotEmpty().WithMessage("ContentCategoryId is required");

		RuleFor(x => x.SortOrder)
			.GreaterThan(0).WithMessage("SortOrder must be greater than 0");

		RuleFor(x => x.SourceType)
			.IsInEnum().WithMessage("Invalid VideoSource");

		RuleFor(x => x.StorageKeyOfVideo)
			.NotEmpty().WithMessage("StorageKeyOfVideo is required");
	}
}
