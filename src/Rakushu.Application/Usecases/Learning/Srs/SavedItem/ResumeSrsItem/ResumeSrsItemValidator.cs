using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.ResumeSrsItem;

internal sealed class ResumeSrsItemValidator : AbstractValidator<ResumeSrsItemCommand>
{
	public ResumeSrsItemValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Saved item ID is required.");
	}
}
