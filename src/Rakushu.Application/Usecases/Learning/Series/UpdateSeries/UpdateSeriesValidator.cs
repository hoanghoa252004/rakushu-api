using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Series.UpdateSeries;

internal sealed class UpdateSeriesValidator : AbstractValidator<UpdateSeriesCommand>
{
	public UpdateSeriesValidator()
	{
		RuleFor(x => x.SeriesId)
			.NotEmpty().WithMessage("SeriesId is required");

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Name is required");

		RuleFor(x => x.DisplayOrder)
			.GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be >= 0");
	}
}
