using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.SaveSrsItem;

internal sealed class SaveSrsItemValidator : AbstractValidator<SaveSrsItemCommand>
{
	public SaveSrsItemValidator()
	{
		RuleFor(x => x.MeaningId)
			.NotEmpty()
			.WithMessage("Meaning ID is required.");
	}
}
