using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CreateFlashcardSession;

internal sealed class CreateFlashcardSessionValidator : AbstractValidator<CreateFlashcardSessionCommand>
{
	public CreateFlashcardSessionValidator()
	{
		RuleFor(x => x.ItemIds)
			.NotNull()
			.NotEmpty()
			.WithMessage("At least one item must be selected to create a flashcard session.")
			.Must(x => x != null && x.Count <= 100)
			.WithMessage("You cannot select more than 100 items for a single flashcard session.");

		RuleForEach(x => x.ItemIds)
			.NotEmpty()
			.WithMessage("Item ID cannot be empty.");
	}
}
