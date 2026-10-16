using FluentValidation;

namespace Rakushu.Application.Usecases.Material.Series.CreateSeries;

internal sealed class CreateSeriesValidator : AbstractValidator<CreateSeriesCommand>
{
	public CreateSeriesValidator()
	{
		RuleFor(x => x.Slug)
			.NotEmpty().WithMessage("Slug is required");

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Name is required");

		RuleFor(x => x.DisplayOrder)
			.GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be >= 0");
	}
}
