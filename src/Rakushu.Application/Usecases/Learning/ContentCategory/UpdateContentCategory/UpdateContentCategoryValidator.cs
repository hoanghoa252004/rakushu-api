using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.UpdateContentCategory;

internal sealed class UpdateContentCategoryValidator : AbstractValidator<UpdateContentCategoryCommand>
{
	public UpdateContentCategoryValidator()
	{
		RuleFor(x => x.ContentCategoryId)
			.NotEmpty()
			.WithMessage("Content Category ID is required");

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Content Category name is required")
			.MaximumLength(100).WithMessage("Content Category name must not exceed 100 characters");

		RuleFor(x => x.JapaneseName)
			.NotEmpty().WithMessage("Content Category JapaneseName is required")
			.MaximumLength(100).WithMessage("Content Category JapaneseName must not exceed 100 characters");

		RuleFor(x => x.DisplayOrder)
			.GreaterThan(0).WithMessage("Content Category display order must be greater than 0");

		RuleFor(x => x.ThemeColor)
			.NotEmpty().WithMessage("Theme color is required")
			.MaximumLength(30).WithMessage("Theme color must not exceed 30 characters");

		RuleFor(x => x.IsActive)
			.NotNull().WithMessage("Content Category IsActive is required");
	}
}
