using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.CreateContentCategory;

internal sealed class CreateContentCategoryValidator : AbstractValidator<CreateContentCategoryCommand>
{
	public CreateContentCategoryValidator()
	{
		RuleFor(x => x.Slug)
			.NotEmpty().WithMessage("Slug is required")
			.MaximumLength(100).WithMessage("Slug must not exceed 100 characters");

		RuleFor(x => x.Code)
			.NotEmpty().WithMessage("Code is required")
			.MaximumLength(30).WithMessage("Code must not exceed 30 characters");

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
