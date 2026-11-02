using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.DeleteSrsItem;

internal sealed class DeleteSrsItemValidator : AbstractValidator<DeleteSrsItemCommand>
{
	public DeleteSrsItemValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Saved item ID is required.");
	}
}
