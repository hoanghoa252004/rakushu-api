using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.ToggleSavedItemActive;

public sealed class ToggleSavedItemActiveValidator : AbstractValidator<ToggleSavedItemActiveCommand>
{
	public ToggleSavedItemActiveValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Saved item ID must not be empty.");
	}
}
