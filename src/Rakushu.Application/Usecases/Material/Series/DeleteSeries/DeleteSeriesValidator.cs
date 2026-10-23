using FluentValidation;

namespace Rakushu.Application.Usecases.Material.Series.DeleteSeries;

internal sealed class DeleteSeriesValidator : AbstractValidator<DeleteSeriesCommand>
{
	public DeleteSeriesValidator()
	{
		RuleFor(x => x.SeriesId)
			.NotEmpty().WithMessage("SeriesId is required");
	}
}
